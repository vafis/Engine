using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.EventSourcing;
using Engine.Core.Messaging;
using Engine.Core.Messaging.Handling;
using Engine.Registration.Commands;

namespace Engine.Registration.Handlers
{
    public class TaxiVehiclesAvailabilityHandler :
        ICommandHandler<AddTaxiVehicle>
    {
        private readonly IEventSourcedRepository<TaxiVehiclesAvailability> _repository;

        public TaxiVehiclesAvailabilityHandler(IEventSourcedRepository<TaxiVehiclesAvailability> repository)
        {
            _repository = repository;
        }

        public void Handle(AddTaxiVehicle command)
        {
            var availability = _repository.Find(command.TaxiVehicleId);
        }

    }
}
