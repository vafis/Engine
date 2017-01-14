using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Engine.Infrastructure.Messaging
{
    public class ServiceBusConfig
    {
        private const string RuleName = "Custom";
        private bool initialized;
        private ServiceBusSettings _settings;
        //private string _connectionString = @"Endpoint=sb://asuslaptop/ServiceBusDefaultNamespace;StsEndpoint=https://asuslaptop:9355/ServiceBusDefaultNamespace;RuntimePort=9354;ManagementPort=9355;WindowsUsername=KONSTANTINOS;WindowsPassword=vafis200974;WindowsDomain=asuslaptop";


        public ServiceBusConfig(ServiceBusSettings serviceBusSettings)
        {
            _settings = serviceBusSettings;
        }

        public void Initialize()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_settings.ConnectionString);
            _settings.Topics.AsParallel().ForAll(topic =>
            {
                CreateTopicIfNotExists(namespaceManager, topic);
                topic.Subscriptions.AsParallel().ForAll(subscription =>
                {
                    CreateSubscriptionIfNotExists(namespaceManager, topic, subscription);
                    UpdateRules(namespaceManager, topic, subscription);
                });
            });

            initialized = true;
        }

        private void CreateTopicIfNotExists(NamespaceManager namespaceManager, TopicSettings topic)
        {
            var topicDescription = new TopicDescription(topic.Path)
            {
                RequiresDuplicateDetection = true
               // DuplicateDetectionHistoryTimeWindow = topic.DuplicateDetectionHistoryTimeWindow
            };
            try
            {
                namespaceManager.CreateTopic(topicDescription);
            }
            catch (Exception ex){}
        }

        private void CreateSubscriptionIfNotExists(NamespaceManager namespaceManager, TopicSettings topic,
            SubscriptionSettings subscription)
        {
            var subscriptionDesciption = new SubscriptionDescription(topic.Path, subscription.Name)
            {
                RequiresSession = subscription.RequiresSession,
                LockDuration = TimeSpan.FromSeconds(150)
            };
            try
            {
                namespaceManager.CreateSubscription(subscriptionDesciption);
            }
            catch (Exception ex){}
        }

        private void UpdateRules(NamespaceManager namespaceManager, TopicSettings topic,
            SubscriptionSettings subscription)
        {
            string sqlExpression = null;
            if (!string.IsNullOrEmpty(subscription.SqlFilter))
            {
                sqlExpression = subscription.SqlFilter;
            }
            UpdateSqlFilter(namespaceManager, sqlExpression, subscription.Name, topic.Path);
        }

        private void UpdateSqlFilter(NamespaceManager namespaceManager, string sqlExpression, string subscriptionName,
            string topicPath)
        {
            bool needReset = false;
            List<RuleDescription> existingRules;
            try
            {
                existingRules = namespaceManager.GetRules(topicPath, subscriptionName).ToList();
            }
            catch (MessagingEntityNotFoundException) 
            {
                //The subscription does not exists
                //No need to update rules
                return;
            }
            if (existingRules.Count != 1)
            {
                needReset = true;
            }
            else
            {
                var existingRule = existingRules.First();
                if (sqlExpression != null && existingRule.Name == RuleDescription.DefaultRuleName)
                {
                    needReset = true;
                }
                else if (sqlExpression==null && existingRule.Name!=RuleDescription.DefaultRuleName)
                {
                    needReset = true;
                }
                else if (sqlExpression != null && existingRule.Name != RuleName)
                {
                    needReset = true;
                }
                else if(sqlExpression!=null && existingRule.Name==RuleName)
                {
                    var filter = existingRule.Filter as SqlFilter;
                    if (filter != null || filter.SqlExpression != sqlExpression)
                    {
                        needReset = true;
                    }
                }
            }

            if (needReset)
            {
                MessagingFactory factory = null;
                try
                {
                    factory = MessagingFactory.CreateFromConnectionString(_settings.ConnectionString);
                    SubscriptionClient client = null;
                    try
                    {
                        client = factory.CreateSubscriptionClient(topicPath, subscriptionName);
                        // first add the default rule, so no new messages are lost while we are updating the subscription
                        TryAddRule(client, new RuleDescription(RuleDescription.DefaultRuleName, new TrueFilter()));
                        // then delete every rule but the Default one
                        foreach (var existing in existingRules.Where(x => x.Name != RuleDescription.DefaultRuleName))
                        {
                            TryRemoveRule(client, existing.Name);
                        }

                        if (sqlExpression != null)
                        {
                            // Add the desired rule.
                            TryAddRule(client, new RuleDescription(RuleName, new SqlFilter(sqlExpression)));

                            // once the desired rule was added, delete the default rule.
                            TryRemoveRule(client, RuleDescription.DefaultRuleName);
                        }
                    }
                    finally
                    {
                        if (client != null)
                            client.Close();
                    }
                }
                finally
                {
                    if(factory!=null)
                        factory.Close();
                }

            }
        }

        private void TryAddRule(SubscriptionClient client, RuleDescription rule)
        {
            try
            {
                client.AddRule(rule);
            }
            catch (Exception ex){}
        }

        private void TryRemoveRule(SubscriptionClient client, string ruleName)
        {
            try
            {
                client.RemoveRule(ruleName);
            }
            catch (Exception ex) { }
        }
    
    }
}
