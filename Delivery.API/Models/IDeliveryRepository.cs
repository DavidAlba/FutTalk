using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delivery.API.Models
{
    public interface IDeliveryRepository
    {
        Delivery AddDelivery(Delivery delivery);
        Task<Delivery> AddDeliveryAsync(Delivery delivery);
        Delivery GetDeliveryById(string deliveryId);
        Task<Delivery> GetDeliveryByIdAsync(string deliveryId);
        Delivery UpdateDelivery(string deliveryId, Delivery delivery);
        Task<Delivery> UpdateDeliveryAsync(string deliveryId, Delivery delivery);
        Task<IEnumerable<Delivery>> GetAllDeliveriesAsync(string toWhom, DateTime when);
        IEnumerable<Delivery> GetAllDeliveries(string toWhom, DateTime when);
    }
}
