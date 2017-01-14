using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Engine.Infrastructure.Messaging;

namespace WorkerRoleCommandProcessor
{
    public static class AutoFacContainerExtensions
    {
        public static void RegisterEventProcessor<T>(this IContainer container, ServiceBusConfig busConfig,
            string subscriptionName, bool instrumatationEnabled = false)
        {
            
        }
    }
}
