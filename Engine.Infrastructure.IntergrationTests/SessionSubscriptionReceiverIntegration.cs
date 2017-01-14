using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Infrastructure;
using Engine.Infrastructure.Messaging;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Xunit;
using System.Diagnostics;

namespace Engine.Infrastructure.IntergrationTests
{
    public class SessionSubscriptionReceiverIntegration
    {
        private ServiceBusSettings _settings;
        private TopicClient _topicClient;

        public SessionSubscriptionReceiverIntegration()
        {
            _settings = InfrastructureSettings.Read(@"C:\Users\KONSTANTINOS\Desktop\Engine\Engine\WorkerRoleCommandProcessor\bin\Debug\Settings.xml").ServiceBus;
            var messagingFactory = MessagingFactory.CreateFromConnectionString(_settings.ConnectionString);
            _topicClient = messagingFactory.CreateTopicClient("engines/taxivehicleevents");
            _topicClient.RetryPolicy = new RetryExponential(minBackoff: TimeSpan.FromSeconds(0.1),
                                            maxBackoff: TimeSpan.FromSeconds(30),
                                            maxRetryCount: 3);
        }

        
        [Fact]
        public void Can_Recieve_Message()
        {
            var sessionSubReciever = new SessionSubscriptionReciever(_settings, "engines/taxivehicleevents", "testsession", true);
            sessionSubReciever.Start(
                delegate(BrokeredMessage msg)
                {
                    return MessageReleaseAction.CompleteMessage;
                });

        }
    }
}
