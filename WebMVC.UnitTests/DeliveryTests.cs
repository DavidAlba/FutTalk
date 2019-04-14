using System;
using System.Collections.Generic;
using System.Text;
using WebMVC.Models;
using Xunit;

namespace WebMVC.UnitTests
{
    public class DeliveryTests
    {

        [Fact]
        public void CreateNewDelivery()
        {
            // Arrange
            string deliveryId = Guid.NewGuid().ToString();
            string capsuleId = Guid.NewGuid().ToString();
            Capsule capsule = new Capsule(capsuleId);

            // Act
            Delivery delivery = new Delivery(deliveryId, capsule);

            // Assert
            Assert.Equal(deliveryId, delivery.Id);
            Assert.Equal(DateTime.Now, delivery.Created, Comparer.Get<DateTime>((d1, d2) => d1.Day == d2.Day && d1.Month == d2.Month && d1.Year == d2.Year));
            Assert.InRange(delivery.TimeStamp, DateTimeOffset.UnixEpoch.ToUnixTimeSeconds(), DateTimeOffset.UnixEpoch.ToUnixTimeSeconds() + 10);
            Assert.NotNull(delivery.Capsule);
            Assert.Equal(capsuleId, delivery.Capsule.Id);

        }

        [Fact]
        public void SetWhenGreaterThanCapsuleCreated()
        {
            // Arrange
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());            
            Delivery delivery = new Delivery(capsule)
            {
                Capsule = capsule,
                ToWhom = "random user"
            };

            // Act 
            delivery.When = DateTime.Now.AddDays(1);

            // Assert
            Assert.True(delivery.When > capsule.Created);
        }

        [Fact]
        public void SetWhenLessThanCapsuleCreatedThrowsException()
        {
            // Arrange
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());
            Delivery delivery = new Delivery(capsule)
            {
                Capsule = capsule,
                ToWhom = "random user"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                {
                    delivery.When = DateTime.Now.AddDays(-1);
                });

        }

        [Fact]
        public void SetWhenLessThanNowThrowsException()
        {
            // Arrange
            Capsule capsule = new Capsule(Guid.NewGuid().ToString());
            Delivery delivery = new Delivery(capsule)
            {
                Capsule = capsule,
                ToWhom = "random user"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                delivery.When = DateTime.Now;
            });

        }
    }
}
