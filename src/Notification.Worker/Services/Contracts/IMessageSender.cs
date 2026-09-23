using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Worker.Services.Contracts
{
    public interface IMessageSender
    {
        Task SendMessageAsync(string userEmail);
    }
}
