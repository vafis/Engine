using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Engine.Infrastructure.Messaging
{
    /// <summary>
    /// Abstracts the behaviour of a recieving component
    /// that raises an event for every revieving event
    /// </summary>
    public interface IMessageReciever
    {
        /// <summary>
        /// Starts the listener
        /// </summary>
        /// <param name="messageHandler">Hangler for incoming messages. The return value indicates how to release the message lock</param>
        void Start(Func<BrokeredMessage, MessageReleaseAction> messageHandler);
        /// <summary>
        /// Stops the listener
        /// </summary>
        void Stop();
    }
}
