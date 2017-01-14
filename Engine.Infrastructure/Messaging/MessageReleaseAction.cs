using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Engine.Infrastructure.Messaging
{
    public enum MessageRealiseActionKind
    {
        Complete,
        Abandon,
        DeadLetter
    }

    /// <summary>
    /// Specifies how the <see cref="BrokeredMessage"/> should be released
    /// </summary>
    public class MessageReleaseAction
    {
        public static readonly MessageReleaseAction CompleteMessage=new MessageReleaseAction(MessageRealiseActionKind.Complete);
        public static readonly MessageReleaseAction AbandonMessage=new MessageReleaseAction(MessageRealiseActionKind.Abandon);
        public MessageReleaseAction(MessageRealiseActionKind kind)
        {
            this.Kind = kind;
        }
        public MessageRealiseActionKind Kind { get; set; }
        public string DeadLetterReason { get; private set; }
        public string DeadLetterDescription { get; private set; }

        public MessageReleaseAction DeadLetterMessage(string reason, string description)
        {
            return new MessageReleaseAction(MessageRealiseActionKind.DeadLetter)
            {
                DeadLetterReason = reason,
                DeadLetterDescription = description
            };
        }
    }
}
