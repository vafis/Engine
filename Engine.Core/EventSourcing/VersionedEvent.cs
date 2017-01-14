using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.EventSourcing
{
    public abstract class VersionedEvent : IVersionedEvent
    {
        public int Version { get; set; }
        public Guid SourceId { get; set; }
    }
}
