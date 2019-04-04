using Catalog.API.Controllers;
using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Exceptions;
using Catalog.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.API.UnitTests
{
    public class MessageControllerTests
    {
        [Fact]
        public async void GetAllMessagesThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetAllMessagesAsync()).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert            
            Exception ex = await Assert.ThrowsAsync<FutTalkException>(async () => await controller.GetAllMessages());
            Assert.Equal(expected: typeof(FutTalkException), actual: ex.GetType());
        }

        [Fact]
        public async void GetAllMessagesNoContent()
        {
            // Arrange
            IEnumerable<Message> emptyList = new List<Message>().AsEnumerable<Message>();
            var mock = new Mock<IRepository>();            
            mock.SetupSequence(rep => rep.GetAllMessagesAsync())
                .Returns(Task<IEnumerable<Message>>.Run(() => emptyList));
            MessageController controller = new MessageController(mock.Object);

            // Act
            NoContentResult model = await controller.GetAllMessages() as NoContentResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.NoContent, model.StatusCode);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void GetAllMessagesOk(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetAllMessagesAsync())
                .Returns(() => Task<IEnumerable<Message>>.Run(() => messages.AsEnumerable<Message>()));
            MessageController controller = new MessageController(mock.Object);

            // Act
            OkObjectResult model = await controller.GetAllMessages() as OkObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.OK, model.StatusCode);
            Assert.Equal(messages, (IEnumerable<Message>)model.Value, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Fact]
        public async void GetMessageByIdThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<FutTalkException>(
                async () => await controller.GetMessage(int.MaxValue)
            );
        
            Assert.Equal(expected: typeof(FutTalkException), actual: ex.GetType());
        }

        [Fact]
        public async void GetMessageByIdBadRequest()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);

            // Act
            BadRequestObjectResult model = await controller.GetMessage(It.IsInRange<int>(0, int.MinValue, Range.Inclusive)) as BadRequestObjectResult;

            // Assert            
            Assert.Equal(400, model.StatusCode);
            mock.Verify(rep => rep.GetMessageById(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void GetMessageByIdNotFound(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetAllMessagesAsync())
                .Returns(Task<IEnumerable<Message>>.Run(() => messages.AsEnumerable<Message>()));            
            MessageController controller = new MessageController(mock.Object);

            // Act
            NotFoundResult model = await controller.GetMessage(int.MaxValue) as NotFoundResult;

            // Assert            
            Assert.Equal((int?)HttpStatusCode.NotFound, model.StatusCode);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void GetMessageByIdOk(Message[] messages)
        {
            // Arrange
            var messageToFind = messages.SingleOrDefault<Message>((m) => m.Id == 2);
            var mock = new Mock<IRepository>();            
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>()))
                .Returns(Task<Message>.Run(() => messageToFind));
            MessageController controller = new MessageController(mock.Object);
        
            // Act            
            OkObjectResult model = await controller.GetMessage(messageToFind.Id) as OkObjectResult;

            // Assert   
            Assert.Equal((int?)HttpStatusCode.OK, model.StatusCode);
            Assert.Equal(messageToFind, (Message)model.Value, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Fact]
        public void CreateMessageThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();            
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert
            Exception ex = Assert.ThrowsAsync<FutTalkException>(
                async () => await controller.CreateMessage(new Message() { Id = int.MaxValue })
            )?.Result;
            Assert.Equal(expected: typeof(FutTalkException), actual: ex.GetType());
        }

        [Fact]
        public async void CreateMessageCreated()
        {
            // Arrange
            Message messageCreated =
                new Message
                {
                    Id = int.MaxValue,
                    Name = $"Name {int.MaxValue}",
                    Body = $"Body {int.MaxValue}"
                };

            var mock = new Mock<IRepository>();            

            MessageController controller = new MessageController(mock.Object);
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>())).Returns(Task<Message>.Run(() => default(Message)));

            // Act            
            CreatedAtRouteResult model = await controller.CreateMessage(messageCreated) as CreatedAtRouteResult;

            // Assert
            mock.Verify(rep => rep.AddMessageAsync(It.IsAny<Message>()), Times.Once);
            Assert.Equal((int?)HttpStatusCode.Created, model.StatusCode);
        }

        [Fact]
        public async void CreateMessageBadRequestMessageIdIsLessThanOne()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);
            Message message = new Message()
            {
                Id = int.MinValue,
                Name = $"Name {int.MinValue}",
                Body = $"Body {int.MinValue}"
            };

            // Act            
            BadRequestObjectResult model = await controller.CreateMessage(message) as BadRequestObjectResult;

            // Assert
            mock.Verify(rep => rep.AddMessageAsync(It.IsAny<Message>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
        }

        [Fact]        
        public async  void CreateMessageBadRequestMessageIsNull()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);            

            // Act            
            BadRequestObjectResult model = await controller.CreateMessage(null) as BadRequestObjectResult;

            // Assert
            mock.Verify(rep => rep.AddMessageAsync(It.IsAny<Message>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void CreateMessageBadRequestMessageAlreadyExists(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>())).Returns(Task<Message>.Run(() => messages[0]));           

            // Act            
            BadRequestObjectResult model = await controller.CreateMessage(messages[0]) as BadRequestObjectResult;

            // Assert
            mock.Verify(m => m.AddMessage(It.IsAny<Message>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
        }

        [Fact]
        public async void ReplaceMessageThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>())).Throws<FutTalkException>();            
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<FutTalkException>(async () => await controller.ReplaceMessage(new Message() { Id = int.MaxValue }));
            Assert.Equal(typeof(FutTalkException), ex.GetType());
        }

        [Fact]
        public async void ReplaceMessageIdIsLessThanOne()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);

            // Act            
            BadRequestObjectResult model = await controller.ReplaceMessage(new Message() { Id = int.MinValue }) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
            mock.Verify(rep => rep.ReplaceMessageAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async void ReplaceMessageIsNull()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);

            // Act            
            BadRequestObjectResult model = await controller.ReplaceMessage(null) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
            mock.Verify(rep => rep.ReplaceMessageAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async void ReplaceMessageNotFound() 
        {
            // Arrange
            Message messageNotFound = new Message() { Id = Int32.MaxValue, Name = "Not found", Body = "Not found" };
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>()))
                .Returns(Task<Message>.Run(() => default(Message)));
            MessageController controller = new MessageController(mock.Object);

            // Act            
            NotFoundObjectResult model = await controller.ReplaceMessage(messageNotFound) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.NotFound, model.StatusCode);
            mock.Verify(rep => rep.ReplaceMessageAsync(It.IsAny<Message>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void ReplaceMessageCreated(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            Message messageReplaced = messages[0];
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>()))
                .Returns(
                    Task<Message>.Run(() => {
                        return messageReplaced;
                    }
                )
            );

            MessageController controller = new MessageController(mock.Object);

            // Act            
            CreatedAtRouteResult model = await controller.ReplaceMessage(messageReplaced) as CreatedAtRouteResult;

            // Assert            
            mock.Verify(rep => rep.ReplaceMessageAsync(It.IsAny<Message>()), Times.Once);
            Assert.Equal((int?)HttpStatusCode.Created, model.StatusCode);
        }

        [Fact]
        public async void DeleteMessageThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();            
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<FutTalkException>(
                async () => await controller.DeleteMessage(int.MaxValue)
                );
                
            Assert.Equal(typeof(FutTalkException), ex.GetType());
        }

        [Fact]
        public async void DeleteMessageBadRequest()
        {
            // Arrange
            var mock = new Mock<IRepository>();            
            MessageController controller = new MessageController(mock.Object);

            // Act
            await controller.DeleteMessage(int.MinValue);

            // Assert            
            mock.Verify(rep => rep.RemoveMessageAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]        
        public async void DeleteMessageNotFound()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>())).Returns(Task<Message>.Run(() => default(Message)));
            MessageController controller = new MessageController(mock.Object);

            // Act
            NotFoundResult model = await controller.DeleteMessage(int.MaxValue) as NotFoundResult;

            // Assert            
            mock.Verify(rep => rep.RemoveMessageAsync(int.MaxValue), Times.Never);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void DeleteMessageNoContent(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            Message messageDeleted = messages[0];
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(messageDeleted.Id)).Returns(Task<Message>.Run(() => messageDeleted));
            MessageController controller = new MessageController(mock.Object);
        
            // Act            
            NoContentResult model = await controller.DeleteMessage(messageDeleted.Id) as NoContentResult;

            // Assert            
            mock.Verify(rep => rep.RemoveMessageAsync(It.IsAny<int>()), Times.Once);
            Assert.Equal((int?)HttpStatusCode.NoContent, model.StatusCode);
        }

        [Fact]
        public async void UpdateMessageThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);
            
            // Act & Assert
            Exception ex = await Assert.ThrowsAsync<FutTalkException>(
                async () => { await controller.UpdateMessage(int.MaxValue, new JsonPatchDocument<Message>()); }
            );

            Assert.Equal(typeof(FutTalkException), ex.GetType());
        }

        [Fact]
        public void UpdateMessageBadRequestMessageIsNull()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);

            // Act
            BadRequestObjectResult model = controller.UpdateMessage(
                It.IsInRange<int>(0, int.MinValue, Range.Inclusive),
                null).Result as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
            mock.Verify(rep => rep.GetMessageById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async void UpdateMessageBadRequestMessageIdIsLessThanOne()
        {
            // Arrange
            var mock = new Mock<IRepository>();            
            MessageController controller = new MessageController(mock.Object);

            // Act
            BadRequestObjectResult model = await controller.UpdateMessage(
                It.IsInRange<int>(0, int.MinValue, Range.Inclusive),
                It.IsAny<JsonPatchDocument<Message>>()) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
            mock.Verify(rep => rep.GetMessageByIdAsync(It.IsAny<int>()), Times.Never);
            mock.Verify(rep => rep.UpdateMessageAsync(It.IsAny<int>(), It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async void UpdateMessageNotFound()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>()))
                .Returns(Task<Message>.Run(() => default(Message)));
            MessageController controller = new MessageController(mock.Object);

            // Act
            NotFoundObjectResult model = await controller.UpdateMessage(
                int.MaxValue,
                new JsonPatchDocument<Message>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.NotFound, model.StatusCode);
            mock.Verify(rep => rep.GetMessageByIdAsync(It.IsAny<int>()), Times.Once);
            mock.Verify(rep => rep.UpdateMessageAsync(It.IsAny<int>(), It.IsAny<Message>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void UpdateMessageOk(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            Message messageToUpdate = messages[0];
            mock.SetupSequence(rep => rep.GetMessageByIdAsync(It.IsAny<int>()))
                .Returns(Task<Message>.Run(() => messageToUpdate));
            MessageController controller = new MessageController(mock.Object);

            // Act
            OkObjectResult model = await controller.UpdateMessage(
                messageToUpdate.Id,
                new JsonPatchDocument<Message>()) as OkObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.OK, model.StatusCode);
            mock.Verify(rep => rep.GetMessageByIdAsync(It.IsAny<int>()), Times.Once);
            mock.Verify(rep => rep.UpdateMessageAsync(It.IsAny<int>(), It.IsAny<Message>()), Times.Once);
        }
    }
}