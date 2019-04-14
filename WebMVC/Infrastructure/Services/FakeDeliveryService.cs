using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Pagination;

namespace WebMVC.Infrastructure.Services
{
    public class FakeDeliveryService : IDeliveryService
    {
        private List<Delivery> _deliveries;

        public FakeDeliveryService()
            => _deliveries = DeliveryData.GetDeliveries();

        public void DeliverCapsule(Delivery delivery)
            => DeliverCapsuleAsync(delivery);

        public Task DeliverCapsuleAsync(Delivery delivery)
            => Task.Run(() => { _deliveries.Add(delivery); } );

        public IEnumerable<Delivery> GetAllDeliveries(string toWhom, DateTime when)
            => GetAllDeliveriesAsync(toWhom, when).Result;

        public Task<IEnumerable<Delivery>> GetAllDeliveriesAsync(string toWhom, DateTime when)
            => Task.Run(() => GetAllPaginatedMessages(toWhom, when, null).Items);

        public PagingInfo<Delivery> GetAllPaginatedMessages(string toWhom, DateTime when, string category, int size = int.MaxValue, int index = 1)
        {
            IEnumerable<Delivery> deliveries = new List<Delivery>().AsEnumerable<Delivery>(); // Empty list
            var longCount = _deliveries.AsQueryable<Delivery>()
                 .Where(d => (category == null || d.Category == category)) // (d.ToWhom == toWhom) && (d.When.DayOfYear == when.DayOfYear) && 
                 .LongCount();

            if (longCount > 0L)
                deliveries = _deliveries.AsQueryable<Delivery>()
                   .Where(d => category == null || d.Category == category)
                   .OrderBy(d => d.Id)
                   .Skip((index - 1) * size)
                   .Take(size)
                   .AsEnumerable<Delivery>();

            return new PagingInfo<Delivery> { CurrentPage = index, ItemsPerPage = size, TotalItems = longCount, Items = deliveries };
        }

        public Task<PagingInfo<Delivery>> GetAllPaginatedMessagesAsync(string toWhom, DateTime when, string category, int size = int.MaxValue, int index = 1)
        {
            return Task<IEnumerable<Delivery>>.Run(() => {
                return this.GetAllPaginatedMessages(toWhom, when, category, size, index);
            });
        }

        public Delivery PickUp(string deliveryId)
            => PickUpAsync(deliveryId).Result;
       
        public Task<Delivery> PickUpAsync(string deliveryId)
            => Task.Run(() => 
            {
                var delivery = _deliveries.First(d => d.Id == deliveryId);
                delivery.Delivered = true;
                return delivery;
            });

        public class DeliveryData
        {
            public static List<Delivery> GetDeliveries()
            {
                var deliveries = new List<Models.Delivery>();
                for (int i = 0; i < 5; i++)
                {
                    var delivery = new Models.Delivery($"Delivery {i + 1}");
                    delivery.ToWhom = $"User {i + 1}";
                    delivery.When = DateTime.Now.AddDays(i + 1);
                    delivery.GiftWrap = (i % 2) == 0;
                    delivery.Category = string.Format("Delivery Cat{0}", (i % 2 == 0) ? "1" : "2");

                    for (int j = 0; j < 5; j++)
                    {
                        var capsule = new Capsule($"Capsule {j + 1}");
                        for (int z = 0; z < 5; z++)
                        {
                            capsule.AddMessage(
                                new Message()
                                {
                                    Id = z + 1,
                                    Name = $"Name {z + 1}",
                                    Body = $"Body {z + 1}"
                                });
                        }
                        delivery.Capsule = capsule;
                    }
                    deliveries.Add(delivery);
                }

                return deliveries;
            }
        }
    }    
}
