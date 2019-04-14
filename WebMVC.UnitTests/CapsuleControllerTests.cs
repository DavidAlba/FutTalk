using Xunit;
using Moq;
using WebMVC.Models;
using WebMVC.Infrastructure.Services;
using WebMVC.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebMVC.UnitTests
{
    public class CapsuleControllerTests
    {
        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void AddMessageToCapsule(Message[] messages)
        {
            // Arrange
            var messageToAdd = messages[0];
            var capsule = new Models.Capsule("Capsule id");

            var messageSrvMock = new Mock<IMessageService>();
            messageSrvMock.Setup(srv => srv.GetMessageByIdAsync(It.IsAny<int>())).Returns(() => Task<Message>.Run(() => messageToAdd));

            var capsuleSrvMock = new Mock<ICapsuleService>();
            capsuleSrvMock.Setup(srv => srv.SaveCapsuleAsync(It.IsAny<Capsule>())).Verifiable();
            capsuleSrvMock.Setup(srv => srv.GetCapsuleByUserAsync()).Returns(() => Task<Capsule>.Run(() => capsule));

            var controller = new CapsuleController(messageService: messageSrvMock.Object, capsuleService: capsuleSrvMock.Object);

            // Act
            var result = await controller.AddMessageToCapsule(messageId: messageToAdd.Id, returnUrl: null) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Capsule", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
            messageSrvMock.Verify(srv => srv.GetMessageByIdAsync(It.IsAny<int>()), Times.Once);
            capsuleSrvMock.Verify(srv => srv.GetCapsuleByUserAsync(), Times.Once);
            capsuleSrvMock.Verify(srv => srv.SaveCapsuleAsync(It.IsAny<Capsule>()), Times.Once);
        }

        [Fact]
        public async void AddMessageToCapsuleMessageDoesNotExist()
        {
            // Arrange            
            var messageSrvMock = new Mock<IMessageService>();
            messageSrvMock.Setup(srv => srv.GetMessageByIdAsync(It.IsAny<int>())).Returns(() => Task<Message>.Run(() => default(Message)));

            var capsuleSrvMock = new Mock<ICapsuleService>();
            capsuleSrvMock.Setup(srv => srv.GetCapsuleByUserAsync()).Verifiable();
            capsuleSrvMock.Setup(srv => srv.SaveCapsuleAsync(It.IsAny<Capsule>())).Verifiable();

            var controller = new CapsuleController(messageService: messageSrvMock.Object, capsuleService: capsuleSrvMock.Object);

            // Act
            var result = await controller.AddMessageToCapsule(messageId: int.MaxValue, returnUrl: null) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Capsule", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
            messageSrvMock.Verify(srv => srv.GetMessageByIdAsync(It.IsAny<int>()), Times.Once);
            capsuleSrvMock.Verify(srv => srv.GetCapsuleByUserAsync(), Times.Never);
            capsuleSrvMock.Verify(srv => srv.SaveCapsuleAsync(It.IsAny<Capsule>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(CapsulesTestData))]
        public async void RemoveMessageFromCapsule(Models.Capsule[] capsules)
        {
            // Arrange
            var capsule = capsules[0];
            var messageToRemove = capsule[0];
            var messageSrvMock = new Mock<IMessageService>();
            messageSrvMock.Setup(srv => srv.GetMessageByIdAsync(It.IsAny<int>())).Returns(() => Task<Message>.Run(() => messageToRemove));

            var capsuleSrvMock = new Mock<ICapsuleService>();
            capsuleSrvMock.Setup(srv => srv.GetCapsuleByUserAsync()).Returns(Task<Capsule>.Run(() => capsule));
            capsuleSrvMock.Setup(srv => srv.SaveCapsuleAsync(It.IsAny<Capsule>())).Verifiable();

            var controller = new CapsuleController(messageService: messageSrvMock.Object, capsuleService: capsuleSrvMock.Object);

            // Act
            var result = await controller.RemoveFromCapsule(messageId: messageToRemove.Id, returnUrl: string.Empty) as RedirectToActionResult;

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Capsule", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
            capsuleSrvMock.Verify(srv => srv.GetCapsuleByUserAsync(), Times.Once);
            capsuleSrvMock.Verify(srv => srv.SaveCapsuleAsync(It.IsAny<Capsule>()), Times.Once);
        }
    }
}
