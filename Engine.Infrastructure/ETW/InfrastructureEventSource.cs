using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventLevel = System.Diagnostics.Tracing.EventLevel;

namespace Engine.Infrastructure.ETW
{
    [EventSource(Name = "InfrastructureEventSource")]
    public sealed class InfrastructureEventSource : EventSource
    {
        public class Keywords
        {
            public const EventKeywords Diagnostic = (EventKeywords)5;
        }

        private static readonly Lazy<InfrastructureEventSource> Instance = new Lazy<InfrastructureEventSource>(() => new InfrastructureEventSource());

        private InfrastructureEventSource() { }

        public static InfrastructureEventSource Log { get { return Instance.Value; } }

        [Event(2, Level = EventLevel.Critical)]
        public void Critical(string msg)
        {
            WriteEvent(2, msg ?? "");
        }

        [Event(3, Level = EventLevel.Error)]
        public void Error(string msg)
        {
            WriteEvent(3, msg ?? "");
        }

        [Event(4, Level = EventLevel.Warning)]
        public void Warning(string msg)
        {
            WriteEvent(4, msg ?? "");
        }

        [Event(5,Level = EventLevel.Informational, Keywords= Keywords.Diagnostic)]
        internal void Informational(string msg)
        {
            WriteEvent(5, msg ?? "");
        }



    }
}
