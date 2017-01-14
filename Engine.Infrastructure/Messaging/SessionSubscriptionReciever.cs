using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Engine.Infrastructure.ETW;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Engine.Infrastructure.Messaging
{
    /// <summary>
    /// Implements an asynchronous Azure Service Bus messaging reciever
    /// Azure Service Bus topic subscription using sessions
    /// </summary>
    public class SessionSubscriptionReciever : IMessageReciever
    {
        private readonly ServiceBusSettings _serviceBusSettings;
        private readonly string _topic;
        private readonly string _subscription;
        private bool _requiresSequentialProcessing;
        private SubscriptionClient _client;
        private readonly object _lockObject = new object();
        public CancellationTokenSource _cancellationTokenSource;
        private CountdownEvent _unreleasedMessages;

        private readonly TaskFactory _taskFactory;



        public SessionSubscriptionReciever(ServiceBusSettings settings, string topic, string subscription,
            bool requiresSequentialProcessing)
            : this(
                settings,
                topic,
                subscription,
                requiresSequentialProcessing,
                new RetryExponential(TimeSpan.FromSeconds(0.1),
                    TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), 3))
        {
        }

        protected SessionSubscriptionReciever(ServiceBusSettings settings, string topic, string subscription,
            bool requiresSequentialProcessing, RetryExponential retryExponential)
        {
            _serviceBusSettings = settings;
            _topic = topic;
            _subscription = subscription;
            _requiresSequentialProcessing = requiresSequentialProcessing;

            var factory = MessagingFactory.CreateFromConnectionString(_serviceBusSettings.ConnectionString);
            _client = factory.CreateSubscriptionClient(topic, subscription);
            if (_requiresSequentialProcessing)
            {
                _client.PrefetchCount = 10;
            }
            else
            {
                _client.PrefetchCount = 15;
            }
            _client.RetryPolicy = retryExponential;

            _taskFactory = Task.Factory;
        }

        private void AcceptSession2(Func<BrokeredMessage, Task> sessionMessageHandler,
            OnMessageOptions onMessageOptions)
        {
            var session = _client.AcceptMessageSessionAsync().ConfigureAwait(true).GetAwaiter().GetResult();
            if (session != null)
            {
                Task.Factory.StartNew(() => { AcceptSession2(sessionMessageHandler, onMessageOptions); });
                session.OnMessageAsync(sessionMessageHandler, onMessageOptions);
            }
        }

        //https://msdn.microsoft.com/en-us/library/hh680892(v=PandP.50).aspx
        //http://stackoverflow.com/questions/8650903/scale-windows-azure-roles-programmatically
        //http://stackoverflow.com/questions/17176754/create-azure-worker-role-or-cloud-service-in-code


        public async Task AcceptSessionAsync(Func<BrokeredMessage, Task> sessionMessageHandler,
            OnMessageOptions onMessageOptions, CancellationToken cancellationToken)
        {
            Action doAcceptSessionAsync = () =>
            {
                Task.Run(
                    () =>
                        AcceptSessionAsync(sessionMessageHandler, onMessageOptions, cancellationToken)
                            .ConfigureAwait(false), cancellationToken);
            };

            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var session = await _client.AcceptMessageSessionAsync();
                    if (session != null)
                    {
                        doAcceptSessionAsync();
                        session.OnMessageAsync(sessionMessageHandler, onMessageOptions);
                        await session.CloseAsync();
                    }
                    else
                    {
                        doAcceptSessionAsync();
                    }
                }
                catch (Exception ex)
                {
                    InfrastructureEventSource.Log.Error(ex.Message);
                    doAcceptSessionAsync();
                }
            }


        }

        protected virtual MessageReleaseAction InvokeMessageHandler(BrokeredMessage message)
        {
            return MessageHandler != null ? MessageHandler(message) : MessageReleaseAction.AbandonMessage;
        }

        protected Func<BrokeredMessage, MessageReleaseAction> MessageHandler { get; private set; }


        private async Task SessionMessageHandler(BrokeredMessage message)
        {
            if (message == null)
                return;

            CountdownEvent unReleasedMessages = new CountdownEvent(1);
            var roundtripStopwatch = Stopwatch.StartNew();
            long schedulingElapsedMilliseconds = 0;
            long processingElapsedMilliseconds = 0;


            var releaseAction = MessageReleaseAction.AbandonMessage;
            schedulingElapsedMilliseconds = roundtripStopwatch.ElapsedMilliseconds;
            if (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    releaseAction = InvokeMessageHandler(message);
                    processingElapsedMilliseconds = roundtripStopwatch.ElapsedMilliseconds -
                                                    schedulingElapsedMilliseconds;
                }
                catch (Exception ex)
                {
                    //InfrastructureEventSource.Log.Error("Error in InvokeMessageHandler " + ex.Message);
                    throw;
                }
                finally
                {

                    RealeaseMessage(message, releaseAction, unReleasedMessages, processingElapsedMilliseconds,
                       schedulingElapsedMilliseconds, roundtripStopwatch);
                }
            }
        }
        //http://stackoverflow.com/questions/30467896/brokeredmessage-automatically-disposed-after-calling-onmessage

        private async void RealeaseMessage(BrokeredMessage message, MessageReleaseAction releaseAction, CountdownEvent countdown, long processingElapsedMilliseconds, long schedulingElapsedMilliseconds, Stopwatch roundtripStopwatch)
        {
            switch (releaseAction.Kind)
            {
                case MessageRealiseActionKind.Complete:
                    await message.CompleteAsync().ContinueWith(t =>
                    {
                        roundtripStopwatch.Stop();

                    }, TaskContinuationOptions.OnlyOnRanToCompletion).ContinueWith(t =>
                    {
                        roundtripStopwatch.Stop();
                        AggregateException exception = t.Exception;
                        InfrastructureEventSource.Log.Error(
                            string.Format(
                                "An error occurred while completing message {0} in subscription {1} with processing time {3} (scheduling {4} request {5} roundtrip {6}). Error message: {2}",
                                message.MessageId,
                                _subscription,
                                exception.Message,
                                processingElapsedMilliseconds,
                                schedulingElapsedMilliseconds,
                                roundtripStopwatch));
                    }, TaskContinuationOptions.OnlyOnFaulted);


                    break;
                case MessageRealiseActionKind.Abandon:
                    break;
                case MessageRealiseActionKind.DeadLetter:
                    break;
                default:
                    break;
                    
            }
        }

  


   
        public void Start(Func<BrokeredMessage, MessageReleaseAction> messageHandler)
        {
            Monitor.Enter(_lockObject);
            {
                try
                {
                    if (_cancellationTokenSource == null)
                    {
                        MessageHandler = messageHandler;
                        _cancellationTokenSource = new CancellationTokenSource();
                        //http://www.drdobbs.com/parallel/specialized-task-schedulers-in-net-4-par/228800428
                        //http://www.kjetilk.com/2012/11/unit-testing-asynchronous-operations.html
                        Task.Factory.StartNew(() =>
                        {
                            OnMessageOptions onMessageOptions = new OnMessageOptions()
                            {
                                AutoComplete = false,
                                MaxConcurrentCalls = 10,
                                //xpired peek lock
                                AutoRenewTimeout = TimeSpan.FromMinutes(60)
                            };
                            onMessageOptions.ExceptionReceived += (sender, e) =>
                            {
                                var i = 12;
                            };

                           // AcceptSession2(SessionMessageHandler, onMessageOptions);
                            AcceptSessionAsync(SessionMessageHandler, onMessageOptions, _cancellationTokenSource.Token).ConfigureAwait(false);

                        }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                    }

                }
                finally { Monitor.Exit(_lockObject);}
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
