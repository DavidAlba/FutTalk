using Delivery.API.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Delivery.API.UnitTests
{
    public class DeliveriesTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { GetDeliveries() };
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private IEnumerable<object> GetDeliveries()
        {
            var deliveries = new List<Models.Delivery>();
            for (int i = 0; i < 5; i++)
            {
                var delivery = new Delivery.API.Models.Delivery();
                delivery.Id = $"Delivery {i + 1}";
                delivery.Created = DateTime.Now.AddDays(i - 1);                
                delivery.TimeStamp = DateTime.UnixEpoch.Second - i;
                delivery.ToWhom = $"User {i + 1}";
                delivery.When = DateTime.Now.AddDays(i + 1);
                delivery.GiftWrap = (i % 2) == 0;
                
                for (int j = 0; j < 5; j++)
                {
                    var capsule = new Capsule();
                    capsule.Id = $"Capsule {j + 1}";
                    capsule.Created = DateTime.Now.AddDays(i + j - 1);
                    delivery.TimeStamp = DateTime.UnixEpoch.Second - i - j;

                    var messages = new List<ModelsDelivery>();
                    for (int z = 0; z < 5; z++)
                    {
                        messages.Add(
                            new ModelsDelivery
                            {
                                Id = z + 1,
                                Name = $"Name {z + 1}",
                                Body = $"Body {z + 1}"
                            });
                    }

                    capsule.Messages = messages;
                    delivery.Capsule = capsule;
                }

                deliveries.Add(delivery);
            }

            return deliveries;
        }
    }
}
