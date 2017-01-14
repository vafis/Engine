using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core.Messaging;

namespace TaxiVehicle.Contracts
{
    /// <summary>
    /// Event TaxiVehicleGotOnlined whenever a TaxiVehicle is got online
    /// </summary>
    public class TaxiVehicleGotOnlined : IEvent
    {
        public Guid SourceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
