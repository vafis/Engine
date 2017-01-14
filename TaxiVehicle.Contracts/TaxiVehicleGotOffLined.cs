using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Messaging;

namespace TaxiVehicle.Contracts
{
    /// <summary>
    /// Event TaxiVehicleGotOffLined whenever a TaxiVehicle is got offline
    /// </summary>
    public class TaxiVehicleGotOffLined :IEvent
    {
        public Guid SourceId { get; set; }
    }
}
