using MassTransit;
using Notification.Worker.Services.Contracts;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Worker.Consumers
{
    public class OrderCreatedConsumer(
        IMessageSender messageSender,
        ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;

            await messageSender.SendMessageAsync(message.UserEmail);

            logger.LogInformation("Order created event consumed for user: {userEmail}.", message.UserEmail);
        }
    }
}
