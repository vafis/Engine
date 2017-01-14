using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Booking.RedisModel;
using Engine.Booking.RedisModel.Services;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using ServiceStack.Text;
using Xunit;

namespace Engine.Booking.Tests
{
    public class RedisFixture
    {
        private IRedisClientsManager _clientManager;
        private TaxiVelicleCoordinates _taxiVelicleCoordinates;

        public RedisFixture()
        {

            _clientManager=new BasicRedisClientManager(Settings.RedisConnectionString);
            //https://github.com/ServiceStack/ServiceStack/blob/master/src/ServiceStack.Common/ModelConfig.cs
           // ModelConfig<TaxiVelicleCoordinates>.Id(x=>x.TaxiVehicleId);
            _taxiVelicleCoordinates = new TaxiVelicleCoordinates
            {
                Latitude = -0.010603,
                Longitude = 0.001802,
                Id = Guid.NewGuid()
            };


        }

        private void SetRedisModel()
        {
            var sut = new RedisRepository(_clientManager);
            _taxiVelicleCoordinates = new TaxiVelicleCoordinates
            {
                Latitude = -0.010603,
                Longitude = 0.001802,
                Id = Guid.NewGuid()
            };

        }

        [Fact]
        public void StoreAsHash_Test()
        {
            var sut = new RedisRepository(_clientManager);
            var stored = sut.StoreAsHash<TaxiVelicleCoordinates>(_taxiVelicleCoordinates);
            Assert.NotNull(stored);
            Assert.Equal(_taxiVelicleCoordinates.Id,stored.Id);
            //Delete it
            sut.Delete(stored);
        }
        [Fact]
        public void ContainsKey_Test()
        {
            var sut = new RedisRepository(_clientManager);
            var stored = sut.StoreAsHash<TaxiVelicleCoordinates>(_taxiVelicleCoordinates);
            var ret = sut.ContainsKey<TaxiVelicleCoordinates>(stored.Id);
            Assert.True(ret);
            //Delete it
            sut.Delete(stored);
        }

        [Fact]
        public void Delete_Test()
        {
            var sut = new RedisRepository(_clientManager);
            var stored = sut.StoreAsHash<TaxiVelicleCoordinates>(_taxiVelicleCoordinates);
            Assert.NotNull(stored);
            sut.Delete<TaxiVelicleCoordinates>(stored);
            var ret = sut.ContainsKey<TaxiVelicleCoordinates>(stored.Id);
            Assert.False(ret);
        }








        /*
        [Fact]
        public void Can_Create()
        {
            var sut = new RedisRepository(_clientManager);
            _taxiVelicleCoordinates = new TaxiVelicleCoordinates
            {
                Latitude = -0.010603,
                Longitude = 0.001802,
                Id = Guid.NewGuid()
            };
         //   var ret = sut.StoreAsHash(_taxiVelicleCoordinates);
        }
        [Fact]
        public void Can_Find()
        {
            var sut = new RedisRepository(_clientManager);
        //    var ret = sut.Find<TaxiVelicleCoordinates>(Guid.Parse("94317004-0e43-4646-9a8e-f4ae487ba9ae"));

        }
        */

        [Fact]
        public void Can_Save_a_new_TaxiVelicleCoordinates_Object()
        {
            Type type = typeof(TaxiVelicleCoordinates);
            var lst=new List<TaxiVelicleCoordinates>();


            using (var redis = _clientManager.GetClient())
            {
                var redisPerson = redis.As<TaxiVelicleCoordinates>();
                var rett = redisPerson.GetFromHash("3b1340f3-066e-48de-94b0-afedde016a1a");


                

                _taxiVelicleCoordinates.Id=Guid.NewGuid();
                redisPerson.StoreAsHash(_taxiVelicleCoordinates);


                var ret1 = redisPerson.GetFromHash("3b1340f3-066e-48de-94b0-afedde016a1a1");
               TaxiVelicleCoordinates ret = redisPerson.GetById("urn:taxiveliclecoordinates:cb63419e-cff4-4872-a675-9b854b52cbce");



                var fromRedis = redisPerson.GetFromHash(_taxiVelicleCoordinates.Id);
                fromRedis.PrintDump();

                _taxiVelicleCoordinates.Latitude = -1.010603;
                _taxiVelicleCoordinates.Longitude = 0.001802;
                redisPerson.StoreAsHash(_taxiVelicleCoordinates);
                fromRedis = redisPerson.GetFromHash(_taxiVelicleCoordinates.Id);
                fromRedis.PrintDump();
                redisPerson.DeleteById(_taxiVelicleCoordinates.Id);
                fromRedis = redisPerson.GetFromHash(_taxiVelicleCoordinates.Id);
                fromRedis.PrintDump();
            }
        }

        /*
        [Fact]
        public void Can_GetFromHash()
        {
            var sut=new RedisRepository(_clientManager);
            var ret = sut.GetFromHash(_taxiVelicleCoordinates);
        }
       */
    }
    
}
