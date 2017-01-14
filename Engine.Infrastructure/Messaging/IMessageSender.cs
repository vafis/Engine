using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Engine.Infrastructure.Messaging
{
    /// <summary>
    /// Abstracts the behaviour of sending a message
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends the specified message asynchronously
        /// </summary>
        /// <param name="messageFactory"></param>
        /// <returns></returns>
        Task SendAsync(Func<BrokeredMessage> messageFactory);
    }
}
