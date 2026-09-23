using MailKit.Net.Smtp;
using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Worker.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Worker.Services
{
    public class EmailOptions
    {
        public required string SmtpServer { get; set; } = "smtp.gmail.com";
        public required int SmtpPort { get; set; } = 587;
        public required string FromEmail { get; set; }
        public required string AppPasswrod { get; set; }
    }

    public class EmailSenderService(
        IOptions<EmailOptions> options,
        ILogger<EmailSenderService> logger) : IMessageSender
    {
        public async Task SendMessageAsync(string userEmail)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(options.Value.FromEmail));
            message.To.Add(MailboxAddress.Parse(userEmail));
            message.Subject = "Order Created";
            message.Body = new TextPart("html")
            {
                Text = "<strong>Your order has been created successfully.</strong>"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(
                    options.Value.SmtpServer,
                    options.Value.SmtpPort,
                    MailKit.Security.SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    options.Value.FromEmail,
                    options.Value.AppPasswrod);

                await client.SendAsync(message);

                logger.LogInformation("Email sent to {userEmail} successfully.", userEmail);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
