using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.EventSourcing
{
    /// <summary>
    /// Represents an Entity that is event sourced
    /// </summary>
    public interface IEventSourced
    {
        Guid Id { get; }
        int Version { get; }
        /// <summary>
        /// Get the collection of new events since entity was loaded as a consequence of command handling.
        /// </summary>
        IEnumerable<IVersionedEvent> Events { get; }
    }
}
