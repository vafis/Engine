using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Engine.Infrastructure.Messaging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Tracing;
using System.Diagnostics.Tracing;
using EventKeywords = System.Diagnostics.Tracing.EventKeywords;
using EventLevel = System.Diagnostics.Tracing.EventLevel;


using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using Diagnostics.Tracing;
using Diagnostics.Tracing.Parsers;

namespace Engine.Infrastructure.ConsoleTests
{
    class Program
    {
        private static Func<BrokeredMessage, MessageReleaseAction> MyDel = (msg) =>
        {
            MessageReleaseAction action;
            if (msg.MessageId != null)
            {
                action = MessageReleaseAction.CompleteMessage;
            }
            else
            {
                action = MessageReleaseAction.AbandonMessage;
            }
            return action;
        };

        private static ServiceBusSettings _settings =
            InfrastructureSettings.Read(
                    @"C:\Users\KONSTANTINOS\Desktop\Engine\Engine\WorkerRoleCommandProcessor\bin\Debug\Settings.xml")
                .ServiceBus;

        static void Main(string[] args)
        {

            var eventListener = ConsoleLog.CreateListener() as ObservableEventListener ??
                                new ObservableEventListener();
            eventListener.EnableEvents(
                "a307c7a2-a4cd-4d22-8093-94db72934152",
                EventLevel.Verbose,
                EventKeywords.All);





            string providerName = "Microsoft-ServiceBus-Client";
            Guid providerGuid = Guid.Parse("a307c7a2-a4cd-4d22-8093-94db72934152");//TraceEventSession.GetEventSourceGuidFromName(providerName);TraceEventSession.GetEventSourceGuidFromName(providerName);//
            string sessionName = "My Session";
            // the null second parameter means 'real time session'
            using (TraceEventSession traceEventSession = new TraceEventSession(sessionName, null))
            {
                using (ETWTraceEventSource etwTraceEventSource = new ETWTraceEventSource(sessionName,
                    TraceEventSourceType.Session))
                {
                    DynamicTraceEventParser dynamicTraceEventParser = new DynamicTraceEventParser(etwTraceEventSource);
                    dynamicTraceEventParser.All += delegate(TraceEvent traceEvent)
                    {
                        Console.WriteLine("Got Event: " + traceEvent.ToString());
                        Console.WriteLine();

                    };
                    traceEventSession.EnableProvider(providerGuid);




                    var sessionSubReciever = new SessionSubscriptionReciever(_settings, "engines/taxivehicleevents",
    "testsession", false);
                    sessionSubReciever.Start(MyDel);

                    //     Thread.CurrentThread.Join(4000);
                    //     sessionSubReciever._cancellationTokenSource.Cancel();
                 
                  //  etwTraceEventSource.Process();
                    Thread.CurrentThread.Join(99999500);
                }




            }
        }
    }
}

/*
                delegate(BrokeredMessage msg)
                {
                    if(msg.MessageId!=null)

                    return MessageReleaseAction.CompleteMessage;
                });

*/