using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Messaging;
using Engine.Core.Messaging.Handling;

namespace Engine.Registration.Commands
{
    public abstract class TaxiVehiclesAvailabilityCommand : ICommand, IMessageSessionProvider
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }

        protected TaxiVehiclesAvailabilityCommand()
        {
            this.Id = Guid.NewGuid();
        }

        string IMessageSessionProvider.SessionId
        {
            get { return this.SessionId; }
        }

        protected string SessionId
        {
            get { return "TaxiVehiclesAvailability_" + this.TaxiVehicleId.ToString(); }
        }
    }
}
