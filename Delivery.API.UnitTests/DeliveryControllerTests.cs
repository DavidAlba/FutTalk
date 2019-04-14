using Delivery.API.Controllers;
using Delivery.API.Infrastructure.Exceptions;
using Delivery.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Delivery.API.UnitTests
{
    public class DeliveryControllerTests
    {
        [Theory]
        [ClassData(typeof(DeliveriesTestData))]
        public async void SaveDeliveryCreated(Models.Delivery[] deliveries)
        {
            // Arrange
            Models.Delivery deliveryToSave = deliveries[0];
            var mock = new Mock<IDeliveryRepository>();
            DeliveryController controller = new DeliveryController(mock.Object);
            mock.Setup(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>())).Returns(Task<Models.Delivery>.Run(() => default(Models.Delivery)));

            // Act            
            CreatedAtRouteResult result = await controller.SaveDelivery(deliveryToSave) as CreatedAtRouteResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.Created, result.StatusCode);
            mock.Verify(rep => rep.AddDeliveryAsync(It.IsAny<Models.Delivery>()), Times.Once);            
        }

        [Fact]
        public void SaveDeliveryThrowsException()
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            mock.Setup(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>())).Throws<DeliveryDomainException>();
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act & Assert
            Exception ex = Assert.ThrowsAsync<DeliveryDomainException>(
                async () => await controller.SaveDelivery(new Models.Delivery() { Id = "Random Id" })
            )?.Result;
            Assert.Equal(expected: typeof(DeliveryDomainException), actual: ex.GetType());
        }

        [Theory]
        [ClassData(typeof(DeliveriesTestData))]
        public async void SaveDeliveryBadRequestAlreadyExists(Models.Delivery[] deliveries)
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            DeliveryController controller = new DeliveryController(mock.Object);
            mock.Setup(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>())).Returns(Task<ModelsDelivery>.Run(() => deliveries[0]));

            // Act            
            BadRequestObjectResult result = await controller.SaveDelivery(deliveries[0]) as BadRequestObjectResult;

            // Assert
            mock.Verify(m => m.AddDeliveryAsync(It.IsAny<Models.Delivery>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async void SaveDeliveryBadRequestIdIsNullOrEmpty()
        {
            // Arrange
            Models.Delivery delivery = new Models.Delivery() { Id = string.Empty };
            var mock = new Mock<IDeliveryRepository>();
            DeliveryController controller = new DeliveryController(mock.Object);           

            // Act            
            BadRequestObjectResult result = await controller.SaveDelivery(delivery) as BadRequestObjectResult;

            // Assert
            mock.Verify(rep => rep.AddDeliveryAsync(It.IsAny<Models.Delivery>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async void SaveDeliveryBadRequestDeliveryIsNull()
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act            
            BadRequestObjectResult result = await controller.SaveDelivery(null) as BadRequestObjectResult;

            // Assert
            mock.Verify(rep => rep.AddDeliveryAsync(It.IsAny<Models.Delivery>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async void GetAllDeliveriesThrowsException()
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            mock.Setup(rep => rep.GetAllDeliveriesAsync(It.IsAny<string>(), It.IsAny<DateTime>())).Throws<DeliveryDomainException>();
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act & Assert            
            Exception ex = await Assert.ThrowsAsync<DeliveryDomainException>(async () => await controller.GetAllDeliveries("Random user", It.IsAny<DateTime>()));
            Assert.Equal(expected: typeof(DeliveryDomainException), actual: ex.GetType());
        }

        [Fact]
        public async void GetAllDeliveriesNoContent()
        {
            // Arrange
            IEnumerable<Models.Delivery> emptyList = new List<Models.Delivery>().AsEnumerable<Models.Delivery>();
            var mock = new Mock<IDeliveryRepository>();
            mock.Setup(rep => rep.GetAllDeliveriesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task<IEnumerable<Models.Delivery>>.Run(() => emptyList));
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act
            NoContentResult model = await controller.GetAllDeliveries("Unexisting user", DateTime.MaxValue) as NoContentResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.NoContent, model.StatusCode);
        }

        [Fact]
        public async void GetAllDeliveriesToWhomIsInvalidBadRequest()
        {
            // Arrange
            string toWhom = string.Empty;
            DateTime when = DateTime.Now;
            var mock = new Mock<IDeliveryRepository>();
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act
            BadRequestObjectResult result = await controller.GetAllDeliveries(toWhom: toWhom, when: when) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Theory]
        [ClassData(typeof(DeliveriesTestData))]
        public async void GetAllDeliveriesOk(Models.Delivery[] deliveries)
        {
            // Arrange
            string toWhom = deliveries[1].ToWhom;
            DateTime when = deliveries[1].When;
            var mock = new Mock<IDeliveryRepository>();
            mock.Setup(rep => rep.GetAllDeliveriesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(() => Task<IEnumerable<Models.Delivery>>.Run(() => deliveries.AsEnumerable<Models.Delivery>()));
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act
            OkObjectResult result = await controller.GetAllDeliveries(toWhom, when) as OkObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(deliveries, (IEnumerable<Models.Delivery>)result?.Value, Comparer.Get<Models.Delivery>((m1, m2) => m1.Id == m2.Id));
        }

        [Fact]
        public async void UpdateDeliveryThrowsException()
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            mock.Setup(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>())).Throws<DeliveryDomainException>();
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<DeliveryDomainException>(
                async () => { await controller.UpdateDelivery("Random Id", new JsonPatchDocument<Models.Delivery>()); }
            );

            Assert.Equal(typeof(DeliveryDomainException), ex.GetType());
        }

        [Fact]
        public void UpdateDeliveryBadRequestDeliveryIsNull()
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act
            BadRequestObjectResult result = controller.UpdateDelivery(
                "Random Id",
                null).Result as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, result.StatusCode);
            mock.Verify(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async void UpdateDeliveryBadRequestIdIsNull()
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act
            BadRequestObjectResult result = await controller.UpdateDelivery(
                "Random User",
                It.IsAny<JsonPatchDocument<Models.Delivery>>()) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, result.StatusCode);
            mock.Verify(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>()), Times.Never);
            mock.Verify(rep => rep.UpdateDeliveryAsync(It.IsAny<string>(), It.IsAny<Models.Delivery>()), Times.Never);
        }

        [Fact]
        public async void UpdateDeliveryNotFound()
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            mock.Setup(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>()))
                .Returns(Task<Models.Delivery>.Run(() => default(Models.Delivery)));
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act
            NotFoundObjectResult result = await controller.UpdateDelivery(
                "Unexisting delivery",
                new JsonPatchDocument<Models.Delivery>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.NotFound, result.StatusCode);
            mock.Verify(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>()), Times.Once);
            mock.Verify(rep => rep.UpdateDeliveryAsync(It.IsAny<string>(), It.IsAny<Models.Delivery>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(DeliveriesTestData))]
        public async void UpdateDeliveryOk(Models.Delivery[] deliveries)
        {
            // Arrange
            var mock = new Mock<IDeliveryRepository>();
            Models.Delivery deliveryToUpdate = deliveries[0];
            mock.Setup(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>()))
                .Returns(Task<Models.Delivery>.Run(() => deliveryToUpdate));
            DeliveryController controller = new DeliveryController(mock.Object);

            // Act
            OkObjectResult model = await controller.UpdateDelivery(
                deliveryToUpdate.Id,
                new JsonPatchDocument<Models.Delivery>()) as OkObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.OK, model.StatusCode);
            mock.Verify(rep => rep.GetDeliveryByIdAsync(It.IsAny<string>()), Times.Once);
            mock.Verify(rep => rep.UpdateDeliveryAsync(It.IsAny<string>(), It.IsAny<Models.Delivery>()), Times.Once);
        }
    }
}
