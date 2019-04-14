using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Pagination;

namespace WebMVC.Infrastructure.Services
{
    public interface IDeliveryService
    {
        Task DeliverCapsuleAsync(Delivery delivery);
        void DeliverCapsule(Delivery delivery);
        Task<Delivery> PickUpAsync(string deliveryId);
        Delivery PickUp(string deliveryId);
        Task<IEnumerable<Delivery>> GetAllDeliveriesAsync(string toWhom, DateTime when);
        IEnumerable<Delivery> GetAllDeliveries(string toWhom, DateTime when);
        PagingInfo<Delivery> GetAllPaginatedMessages(string toWhom, DateTime when, string category, int size = int.MaxValue, int index = 1);
        Task<PagingInfo<Delivery>> GetAllPaginatedMessagesAsync(string toWhom, DateTime when, string category, int size = int.MaxValue, int index = 1);
    }
}
