using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.API.Models
{
    public class FakeDeliveryRepository : IDeliveryRepository
    {
        public Delivery GetDeliveryById(string deliveryId)
        {
            throw new NotImplementedException();
        }

        public Task<Delivery> GetDeliveryByIdAsync(string deliveryId)
        {
            throw new NotImplementedException();
        }

        public Delivery AddDelivery(Delivery delivery)
        {
            throw new NotImplementedException();
        }

        public Task<Delivery> AddDeliveryAsync(Delivery delivery)
        {
            throw new NotImplementedException();
        }

        public Delivery UpdateDelivery(string deliveryId, Delivery delivery)
        {
            throw new NotImplementedException();
        }

        public Task<Delivery> UpdateDeliveryAsync(string deliveryId, Delivery delivery)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Delivery>> GetAllDeliveriesAsync(string toWhom, DateTime when)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Delivery> GetAllDeliveries(string toWhom, DateTime when)
        {
            throw new NotImplementedException();
        }
    }
}