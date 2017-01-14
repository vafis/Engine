using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core.EventSourcing
{
    /// <summary> 
    /// Base class for event sourced entities that implement <see cref="IEventSourced"/>
    /// </summary>
    public abstract class EventSourced : IEventSourced
    {
        private readonly Dictionary<Type,Action<IVersionedEvent>> _handlers = new Dictionary<Type, Action<IVersionedEvent>>(); 
        private readonly List<IVersionedEvent> _pendingEvents = new List<IVersionedEvent>();

        private readonly Guid _Id;
        private int _version = -1;

        public Guid Id
        {
            get { return _Id; }
        }

        public int Version
        {
            get { return _version; }
            protected set { _version = value; }
        }
        /// <summary>
        /// Configures a handler for a event
        /// </summary>
        public IEnumerable<IVersionedEvent> Events
        {
            get { return _pendingEvents; }
        }

        protected void Handles<TEvent>(Action<TEvent> handler)
        {
            _handlers.Add(typeof (TEvent), @event => handler((TEvent) @event));
        }

        protected void FromLoad(IEnumerable<IVersionedEvent> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                _handlers[e.GetType()].Invoke(e);
                _version = e.Version;
            }
        }

        protected void Update(VersionedEvent e)
        {
            e.SourceId = _Id;
            e.Version = _version + 1;
            _handlers[e.GetType()].Invoke(e);
            _version = e.Version;
            _pendingEvents.Add(e);
        }
    }
}
