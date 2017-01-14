using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Engine.Booking.RedisModel;
using Engine.Booking.RedisModel.Services;
using Engine.Core.Messaging.Handling;
using TaxiVehicle.Contracts;

namespace Engine.Booking.Handlers
{
    public class TaxiVehicleViewModelGenerator :
        IEventHandler<TaxiVehicleGotOnlined>,
        IEventHandler<TaxiVehicleGotOffLined>
    {
        private readonly Func<RedisRepository> _redisRepocitory;

        public TaxiVehicleViewModelGenerator(Func<RedisRepository> redisRepocitory )
        {
            _redisRepocitory = redisRepocitory;
        }

        public void Handle(TaxiVehicleGotOnlined @event)
        {
            using (var redisRepocitory=_redisRepocitory.Invoke())
            {
                if (redisRepocitory.ContainsKey(@event.SourceId))
                {
                    Trace.TraceWarning("Ignoring TaxiVehicleGotOnlined event for TaxiVehicle Id with ID {0} as it was already created.",
                        @event.SourceId);
                }
                else
                {
                    redisRepocitory.StoreAsHash<TaxiVelicleCoordinates>(
                        new TaxiVelicleCoordinates
                        {
                            Id = @event.SourceId,
                            Latitude = @event.Latitude,
                            Longitude = @event.Longitude
                            //todo dateCreate ??
                        });
                }
            }
        }

        public void Handle(TaxiVehicleGotOffLined @event)
        {
            using (var redisRepocitory = _redisRepocitory.Invoke())
            {
                if (!redisRepocitory.ContainsKey(@event.SourceId))
                {
                    Trace.TraceWarning("Ignoring TaxiVehicleGotOfflined event for TaxiVehicle Id with ID {0} as it was already created.",
                        @event.SourceId);
                }
                else
                {
                    redisRepocitory.Delete(
                        new TaxiVelicleCoordinates
                        {
                            Id=@event.SourceId
                        });
                }
            }

        }
    }
}
