using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Booking.RedisModel.Services
{
    public interface IRedisRepository : IDisposable
    {
        bool ContainsKey<T>(Guid Id);
        T StoreAsHash<T>(T obj);
        void Delete<T>(T obj);
    }
}
