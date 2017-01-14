using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Messaging;

namespace Engine.Core.EventSourcing
{
    /// <summary>
    /// Represents an event message that belongs to an ordered event strem
    /// </summary>
    public interface IVersionedEvent : IEvent
    {
        /// <summary>
        /// Get the version or order of the event in the stream
        /// </summary>
        int Version { get; }
    }
}
