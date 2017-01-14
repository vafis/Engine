using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;

namespace Engine.Infrastructure.Messaging
{
    /// <summary>
    /// Implements an asychronous BrokeredMessage sender to Azure ESB
    /// </summary>
    public class TopicSender : IMessageSender
    {
        private TopicClient _topicClient;
        private ServiceBusSettings _settings;
        private string _topic;
        private RetryPolicy _retryPolicy;


        public TopicSender(ServiceBusSettings settings, string topic)
            :this(settings,topic, new RetryExponential(maxRetryCount:10, 
                                                       minBackoff: TimeSpan.FromMilliseconds(100),
                                                       maxBackoff: TimeSpan.FromMilliseconds(1)))
        {            
        }

        protected TopicSender(ServiceBusSettings settings, string topic, RetryPolicy retryPolicy)
        {
            _settings = settings;
            _topic = topic;
            _retryPolicy = retryPolicy;

            var factory = MessagingFactory.CreateFromConnectionString(_settings.ConnectionString);
            _topicClient = factory.CreateTopicClient(_topic);
            _topicClient.RetryPolicy = _retryPolicy;
          
        }



        public async Task SendAsync(Func<BrokeredMessage> messageFactory)
        {
            // http://www.getcodesamples.com/src/77471F6A/DB5AF9FC
            //http://www.getcodesamples.com/src/77471F6A/286BECA0
            //https://github.com/App-vNext/Polly

            var message = messageFactory.Invoke();
            
            try
            {
               await _topicClient.SendAsync(message);
            }
            catch (Exception)
            {
                message.Dispose();
                throw;
            }
        }
    }

}
