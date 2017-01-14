using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Engine.Booking.RedisModel.Services;
using ServiceStack.Redis;
using System.Configuration;

namespace WorkerRoleCommandProcessor.App_Start
{
    public class AutofacConfig
    {
        public static IContainer SetUp()
        {
            var builder = new ContainerBuilder();
            var redisConnectionString = ConfigurationManager.AppSettings["Redis.ConnectionString"];
            builder.RegisterType<PooledRedisClientManager>().As<IRedisClientsManager>().WithParameter(
                new TypedParameter(typeof (string), redisConnectionString));
            builder.RegisterType<RedisRepository>().InstancePerDependency(); //is Transient Lifetime

            RegisterEventProcessors(builder);

            return builder.Build();
        }

        private static void RegisterEventProcessors(ContainerBuilder container)
        {
            
        }
    }
}
