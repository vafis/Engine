using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace Engine.Booking.RedisModel.Services
{
    /// <summary>
    /// Redis persistance repocitory
    /// </summary>
    public class RedisRepository : IRedisRepository, IDisposable
    {
        private readonly IRedisClientsManager _redisManager;

        public RedisRepository(IRedisClientsManager redisManager)
        {
            _redisManager = redisManager;
        }

        public bool ContainsKey<T>(Guid Id)
        {
            var key = "urn:" + typeof (T).Name.ToLower() + ":" + Id;
            using (var redis = _redisManager.GetReadOnlyClient())
            {
                IRedisTypedClient<T> redisOfT = redis.As<T>();
                return redisOfT.ContainsKey(key);
            }
        }

        public bool ContainsKey(Guid Id)
        {
            return true;
        }

        public T StoreAsHash<T>(T obj) //where T: IRedisEntity
        {
            using (var redis = _redisManager.GetReadOnlyClient())
            {
                IRedisTypedClient<T> redisOfT = redis.As<T>();
                redisOfT.StoreAsHash(obj);
                return redisOfT.GetFromHash(obj.GetObjectId());
            }
        }

        public void Delete<T>(T obj)
        {
            using (var redis = _redisManager.GetClient())
            {
                IRedisTypedClient<T> redisOfT = redis.As<T>();
                redisOfT.DeleteById(obj.GetId());
            }
            
        }


        /*
        public T Find<T>(Guid id) //where T : IRedisEntity
        {
            using (var redis = _redisManager.GetReadOnlyClient())
            {
                IRedisTypedClient<T> redisOfT = redis.As<T>();
                return redisOfT.GetById(id);
            }
        }



        public T GetFromHash<T>(T obj)
        {
            using (var redis=_redisManager.GetClient())
            {
                IRedisTypedClient<T> redisOfT = redis.As<T>();
                return redisOfT.GetFromHash(obj.GetId());
            }
        }

 */

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                using (_redisManager as IDisposable) { }
            }
        }
    }
}
