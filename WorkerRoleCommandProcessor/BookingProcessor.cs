using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Engine.Infrastructure;
using Engine.Infrastructure.Messaging;
using WorkerRoleCommandProcessor.App_Start;


namespace WorkerRoleCommandProcessor
{
    public sealed class BookingProcessor : IDisposable
    {
        private IContainer _container;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _instrumentationEnabled;
        private InfrastructureSettings _azureSettings;
        private ServiceBusConfig _busConfig;
         

        public BookingProcessor(bool instrumentationEnabled = false)
        {
            _instrumentationEnabled = instrumentationEnabled;
            _azureSettings = InfrastructureSettings.Read("Settings.xml");
            _busConfig = new ServiceBusConfig(_azureSettings.ServiceBus);

            _busConfig.Initialize();
            _cancellationTokenSource=new CancellationTokenSource();

            _container = AutofacConfig.SetUp();
        }


        public void Dispose()
        {
            _container.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}
