using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebMVC.Controllers;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;
using Xunit;

namespace WebMVC.UnitTests
{
    public class DeliveryControllerTests
    {
        [Fact]
        public async void CannotSendEmptyCapsule()
        {
            // Arrange
            var capsule = new Capsule(Guid.NewGuid().ToString());
            var delivery = new Delivery(capsule);

            var capsuleSrvMock = new Mock<ICapsuleService>();
            capsuleSrvMock.Setup(srv => srv.GetCapsuleByUserAsync()).Returns(() => Task<Capsule>.Run(() => capsule));

            var deliverySrvMock = new Mock<IDeliveryService>();
            deliverySrvMock.Setup(srv => srv.DeliverCapsuleAsync(It.IsAny<Delivery>())).Verifiable();

            var controller = new DeliveryController(capsuleService: capsuleSrvMock.Object, deliveryService: deliverySrvMock.Object);

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", result?.ViewName);
            deliverySrvMock.Verify(srv => srv.DeliverCapsuleAsync(It.IsAny<Delivery>()), Times.Never);
            Assert.False(result?.ViewData?.ModelState.IsValid);
        }

        [Fact]
        public async void CanSendNotEmptyCapsule()
        {
            // Arrange
            var message = new Message() { Id = int.MaxValue };
            var capsule = new Capsule(Guid.NewGuid().ToString());
            capsule.AddMessage(message);
            var delivery = new Delivery(capsule);

            var capsuleSrvMock = new Mock<ICapsuleService>();
            capsuleSrvMock.Setup(srv => srv.GetCapsuleByUserAsync()).Returns(() => Task<Capsule>.Run(() => capsule));

            var deliverySrvMock = new Mock<IDeliveryService>();
            deliverySrvMock.Setup(srv => srv.DeliverCapsuleAsync(It.IsAny<Delivery>())).Verifiable();

            var controller = new DeliveryController(capsuleService: capsuleSrvMock.Object, deliveryService: deliverySrvMock.Object);

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", result?.ViewName);
            deliverySrvMock.Verify(srv => srv.DeliverCapsuleAsync(It.IsAny<Delivery>()), Times.Never);
            Assert.True(result?.ViewData?.ModelState.IsValid);
        }

        [Fact]
        public async void CannotSendInvalidCapsuleDeliveryDetails()
        {
            // Arrange
            var message = new Message() { Id = int.MaxValue };
            var capsule = new Capsule(Guid.NewGuid().ToString());
            capsule.AddMessage(message);
            var delivery = new Delivery(capsule);

            var capsuleSrvMock = new Mock<ICapsuleService>();
            capsuleSrvMock.Setup(srv => srv.GetCapsuleByUserAsync()).Returns(() => Task<Capsule>.Run(() => capsule));

            var deliverySrvMock = new Mock<IDeliveryService>();
            deliverySrvMock.Setup(srv => srv.DeliverCapsuleAsync(It.IsAny<Delivery>())).Verifiable();

            var controller = new DeliveryController(capsuleService: capsuleSrvMock.Object, deliveryService: deliverySrvMock.Object);
            controller.ModelState.AddModelError("error", "error");

            // Act
            ViewResult result = await controller.SendCapsule(delivery) as ViewResult;
            

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", result?.ViewName);
            deliverySrvMock.Verify(srv => srv.DeliverCapsuleAsync(It.IsAny<Delivery>()), Times.Never);
            Assert.False(result?.ViewData?.ModelState.IsValid);
        }

        [Fact]
        public async void CanSendCapsuleDelivery()
        {
            // Arrange
            var message = new Message() { Id = int.MaxValue };
            var capsule = new Capsule(Guid.NewGuid().ToString());
            capsule.AddMessage(message);
            var delivery = new Delivery(capsule);
            delivery.When = DateTime.Now.AddDays(+1);
            delivery.ToWhom = "Random User";

            var capsuleSrvMock = new Mock<ICapsuleService>();
            capsuleSrvMock.Setup(srv => srv.GetCapsuleByUserAsync()).Returns(() => Task<Capsule>.Run(() => capsule));

            var deliverySrvMock = new Mock<IDeliveryService>();
            deliverySrvMock.Setup(srv => srv.DeliverCapsuleAsync(It.IsAny<Delivery>())).Returns(() => Task<Capsule>.Run(() => { return; }));

            var controller = new DeliveryController(capsuleService: capsuleSrvMock.Object, deliveryService: deliverySrvMock.Object);

            // Act
            RedirectToActionResult result = await controller.SendCapsule(delivery) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Delivery", result?.ControllerName);
            Assert.Equal(nameof(controller.Success), result?.ActionName);
            deliverySrvMock.Verify(srv => srv.DeliverCapsuleAsync(It.IsAny<Delivery>()), Times.Once);
        }
    }
}
