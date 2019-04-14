using Capsule.API.Controllers;
using Capsule.API.Infrastructure.Exceptions;
using Capsule.API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Capsule.API.UnitTests
{
    public class CapsuleControllerTests
    {
        [Theory]
        [ClassData(typeof(CapsulesTestData))]
        public async void GetCapsuleByUserOk(Models.Capsule[] capsules)
        {
            // Arrange
            var user = "User1";
            var capsuleToFind = capsules[0];
            var mock = new Mock<ICapsuleRepository>();
            mock.SetupSequence(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>())).Returns(Task<Models.Capsule>.Run(() => capsuleToFind));
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act            
            OkObjectResult result = await controller.GetCapsuleByUser(user) as OkObjectResult;

            // Assert   
            Assert.Equal((int?)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(capsuleToFind, (Models.Capsule)result.Value, Comparer.Get<Models.Capsule>((m1, m2) => m1.Id == m2.Id));
        }

        [Fact]
        public async void GetCapsuleByUserCreated()
        {
            // Arrange
            string userId = "Unexisting user";
            var mock = new Mock<ICapsuleRepository>();
            mock.Setup(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>())).Returns(Task<Models.Capsule>.Run(() => { return default(Models.Capsule); }));
            mock.Setup(rep => rep.CreateNewCapsuleAsync(It.IsAny<string>())).Returns(Task<Models.Capsule>.Run(() => { return new Models.Capsule(userId: userId); }));
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act            
            CreatedAtRouteResult result = await controller.GetCapsuleByUser(userId) as CreatedAtRouteResult;

            // Assert            
            Assert.Equal((int?)HttpStatusCode.Created, result.StatusCode);
            Assert.Equal(userId, ((Models.Capsule)result.Value).Id);
            Assert.Empty(((Models.Capsule)result.Value).Messages);
        }

        [Fact]
        public async void GetCapsuleByUserBadRequest()
        {
            // Arrange
            var mock = new Mock<ICapsuleRepository>();
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act
            BadRequestObjectResult result = await controller.GetCapsuleByUser(null) as BadRequestObjectResult;

            // Assert            
            Assert.Equal(400, result.StatusCode);
            mock.Verify(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async void GetMessageByIdThrowsException()
        {
            // Arrange
            var mock = new Mock<ICapsuleRepository>();
            mock.Setup(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>())).Throws<CapsuleDomainException>();
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<CapsuleDomainException>(
                async () => await controller.GetCapsuleByUser("User1")
            );

            Assert.Equal(expected: typeof(CapsuleDomainException), actual: ex.GetType());
        }

        [Theory]
        [ClassData(typeof(CapsulesTestData))]
        public async void SaveCapsuleOk(Models.Capsule[] capsules)
        {
            // Arrange
            var mock = new Mock<ICapsuleRepository>();
            Models.Capsule capsuleToSave = capsules[0];
            mock.Setup(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>())).Returns(Task<Models.Capsule>.Run(() => capsuleToSave ));
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act
            OkObjectResult result = await controller.SaveCapsule(capsuleToSave) as OkObjectResult;

            // Assert          
            Assert.Equal((int?)HttpStatusCode.OK, result?.StatusCode);
            mock.Verify(rep => rep.SaveCapsuleAsync(It.IsAny<Models.Capsule>()), Times.Once);            
        }

        [Theory]
        [ClassData(typeof(CapsulesTestData))]
        public async void SaveCapsuleCreated(Models.Capsule[] capsules)
        {
            // Arrange            
            var mock = new Mock<ICapsuleRepository>();
            Models.Capsule capsuleToSave = capsules[0];
            mock.Setup(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>())).Returns(Task<Models.Capsule>.Run(() => default(Models.Capsule)));
            mock.Setup(rep => rep.CreateNewCapsuleAsync(It.IsAny<string>())).Returns(Task<Models.Capsule>.Run(() => capsuleToSave));
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act            
            CreatedAtRouteResult result = await controller.SaveCapsule(capsuleToSave) as CreatedAtRouteResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.Created, result?.StatusCode);
            mock.Verify(rep => rep.SaveCapsuleAsync(It.IsAny<Models.Capsule>()), Times.Never);
            mock.Verify(rep => rep.CreateNewCapsuleAsync(It.IsAny<Models.Capsule>()), Times.Once);
        }

        [Fact]
        public async void SaveCapsuleThrowsException()
        {
            // Arrange
            var mock = new Mock<ICapsuleRepository>();
            mock.Setup(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>())).Throws<CapsuleDomainException>();
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<CapsuleDomainException>(
                async () => await controller.SaveCapsule(new Models.Capsule("Random user"))
            );

            Assert.Equal(expected: typeof(CapsuleDomainException), actual: ex.GetType());
        }

        [Fact]
        public async void SaveCapsuleInvalidIdBadRequest()
        {
            // Arrange
            var mock = new Mock<ICapsuleRepository>();
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act
            BadRequestObjectResult result = await controller.SaveCapsule(new Models.Capsule(string.Empty)) as BadRequestObjectResult;

            // Assert          
            Assert.Equal((int?)HttpStatusCode.BadRequest, result?.StatusCode);
            mock.Verify(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>()), Times.Never);
            mock.Verify(rep => rep.SaveCapsuleAsync(It.IsAny<Models.Capsule>()), Times.Never);
            mock.Verify(rep => rep.CreateNewCapsuleAsync(It.IsAny<Models.Capsule>()), Times.Never);
        }

        [Fact]
        public async void SaveCapsuleIsNullBadRequest()
        {
            // Arrange
            var mock = new Mock<ICapsuleRepository>();
            CapsuleController controller = new CapsuleController(mock.Object);

            // Act
            BadRequestObjectResult result = await controller.SaveCapsule(null) as BadRequestObjectResult;

            // Assert          
            Assert.Equal((int?)HttpStatusCode.BadRequest, result?.StatusCode);
            mock.Verify(rep => rep.GetCapsuleByUserAsync(It.IsAny<string>()), Times.Never);
            mock.Verify(rep => rep.SaveCapsuleAsync(It.IsAny<Models.Capsule>()), Times.Never);
            mock.Verify(rep => rep.CreateNewCapsuleAsync(It.IsAny<Models.Capsule>()), Times.Never);
        }
    }
}
