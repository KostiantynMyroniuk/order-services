using MassTransit;
using Notification.Worker.Services;
using Notification.Worker.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Worker.Extensions
{
    public static class Extensions
    {
        public static void AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumers(typeof(Extensions).Assembly);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
                    
                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddScoped<IMessageSender, EmailSenderService>();

            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailConfigs"));
        }
    }
}
