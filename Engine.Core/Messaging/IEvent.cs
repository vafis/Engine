using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.Messaging
{
    /// <summary>
    /// Represents an event message
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Get the intentifier or the source originating the event
        /// </summary>
        Guid SourceId { get; }
    }
}
