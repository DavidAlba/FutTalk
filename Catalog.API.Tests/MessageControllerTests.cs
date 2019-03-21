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
using Xunit;

namespace Catalog.API.Tests
{
    public class MessageControllerTests
    {
        [Fact]
        public void GetAllMessagesThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupGet(rep => rep.Messages).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert            
            Exception ex = Assert.Throws<FutTalkException>(() => controller.GetAllMessages());
            Assert.Equal(expected: typeof(FutTalkException), actual: ex.GetType());
        }

        [Fact]
        public void GetAllMessagesNoContent()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupGet(rep => rep.Messages).Returns(new List<Message>());
            MessageController controller = new MessageController(mock.Object);

            // Act
            NoContentResult model = controller.GetAllMessages() as NoContentResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.NoContent, model.StatusCode);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void GetAllMessagesOk(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupGet(rep => rep.Messages).Returns(messages);
            MessageController controller = new MessageController(mock.Object);

            // Act
            OkObjectResult model = controller.GetAllMessages() as OkObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.OK, model.StatusCode);
            Assert.Equal(messages, (IEnumerable<Message>)model.Value, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Fact]
        public void GetMessageByIdThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert
            Exception ex = Assert.Throws<FutTalkException>(() => controller.GetMessage(1));
            Assert.Equal(expected: typeof(FutTalkException), actual: ex.GetType());
        }

        [Fact]
        public void GetMessageByIdBadRequest()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);

            // Act
            BadRequestObjectResult model = controller.GetMessage(It.IsInRange<int>(0, int.MinValue, Range.Inclusive)) as BadRequestObjectResult;

            // Assert            
            Assert.Equal(400, model.StatusCode);
            mock.Verify(rep => rep.GetMessageById(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void GetMessageByIdNotFound(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupGet(m => m.Messages).Returns(messages);
            MessageController controller = new MessageController(mock.Object);

            // Act
            NotFoundResult model = controller.GetMessage(int.MaxValue) as NotFoundResult;

            // Assert            
            Assert.Equal((int?)HttpStatusCode.NotFound, model.StatusCode);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void GetMessageByIdOk(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            var message = messages.SingleOrDefault<Message>((m) => m.Id == 2);
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Returns(message);
            MessageController controller = new MessageController(mock.Object);

            // Act            
            OkObjectResult model = controller.GetMessage(message.Id) as OkObjectResult;

            // Assert   
            Assert.Equal((int?)HttpStatusCode.OK, model.StatusCode);
            Assert.Equal(message, (Message)model.Value, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Fact]
        public void CreateMessageThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.AddMessage(It.IsAny<Message>())).Throws<FutTalkException>();
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert
            Exception ex = Assert.Throws<FutTalkException>(() => controller.CreateMessage(new Message() { Id = int.MaxValue }));
            Assert.Equal(expected: typeof(FutTalkException), actual: ex.GetType());
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void CreateMessageCreated(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            Message messageCreated = new Message { Id = int.MaxValue, Name = $"Name {int.MaxValue}", Body = $"Body {int.MaxValue}" };
            MessageController controller = new MessageController(mock.Object);
            mock.SetupGet(rep => rep.Messages).Returns(messages);

            // Act            
            CreatedAtActionResult model = controller.CreateMessage(messageCreated) as CreatedAtActionResult;

            // Assert
            mock.Verify(rep => rep.AddMessage(It.IsAny<Message>()), Times.Once);
            Assert.Equal((int?)HttpStatusCode.Created, model.StatusCode);
        }

        [Fact]
        public void CreateMessageBadRequestMessageIdIsLessThanOne()
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
            BadRequestObjectResult model = controller.CreateMessage(message) as BadRequestObjectResult;

            // Assert
            mock.Verify(m => m.AddMessage(It.IsAny<Message>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
        }

        [Fact]        
        public void CreateMessageBadRequestMessageIsNull()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);            

            // Act            
            BadRequestObjectResult model = controller.CreateMessage(null) as BadRequestObjectResult;

            // Assert
            mock.Verify(m => m.AddMessage(It.IsAny<Message>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void CreateMessageBadRequestMessageAlreadyExists(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Returns(messages[0]);           

            // Act            
            BadRequestObjectResult model = controller.CreateMessage(messages[0]) as BadRequestObjectResult;

            // Assert
            mock.Verify(m => m.AddMessage(It.IsAny<Message>()), Times.Never);
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
        }

        [Fact]
        public void ReplaceMessageThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Throws<FutTalkException>();
            mock.SetupSequence(rep => rep.AddMessage(It.IsAny<Message>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert
            Exception ex = Assert.Throws<FutTalkException>(() => controller.ReplaceMessage(new Message() { Id = int.MaxValue }));
            Assert.Equal(typeof(FutTalkException), ex.GetType());
        }

        [Fact]
        public void ReplaceMessageIdIsLessThanOne()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);

            // Act            
            BadRequestObjectResult model = controller.ReplaceMessage(new Message() { Id = int.MinValue }) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
            mock.Verify(rep => rep.ReplaceMessage(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public void ReplaceMessageIsNull()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            MessageController controller = new MessageController(mock.Object);

            // Act            
            BadRequestObjectResult model = controller.ReplaceMessage(null) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
            mock.Verify(rep => rep.ReplaceMessage(It.IsAny<Message>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void ReplaceMessageNotFound(Message[] messages) 
        {
            // Arrange
            var mock = new Mock<IRepository>();
            Message messageNotFound = new Message() { Id = Int32.MaxValue, Name = "Not found", Body = "Not found" };
            mock.SetupGet(m => m.Messages).Returns(messages);
            MessageController controller = new MessageController(mock.Object);

            // Act            
            NotFoundObjectResult model = controller.ReplaceMessage(messageNotFound) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.NotFound, model.StatusCode);
            mock.Verify(rep => rep.ReplaceMessage(It.IsAny<Message>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void ReplaceMessageCreated(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            Message messageReplaced = messages[0];
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Returns(messageReplaced);
            MessageController controller = new MessageController(mock.Object);

            // Act            
            CreatedAtActionResult model = controller.ReplaceMessage(messageReplaced) as CreatedAtActionResult;

            // Assert            
            mock.Verify(rep => rep.ReplaceMessage(It.IsAny<Message>()), Times.Once);
            Assert.Equal((int?)HttpStatusCode.Created, model.StatusCode);
        }

        [Fact]
        public void DeleteMessageThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();            
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Assert
            Exception ex = Assert.Throws<FutTalkException>(() => controller.DeleteMessage(int.MaxValue));
            Assert.Equal(typeof(FutTalkException), ex.GetType());
        }

        [Fact]
        public void DeleteMessageBadRequest()
        {
            // Arrange
            var mock = new Mock<IRepository>();            
            MessageController controller = new MessageController(mock.Object);

            // Act
            controller.DeleteMessage(int.MinValue);

            // Assert            
            mock.Verify(rep => rep.RemoveMessage(It.IsAny<int>()), Times.Never);
        }

        [Fact]        
        public void DeleteMessageNotFound()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Returns(default(Message));
            MessageController controller = new MessageController(mock.Object);

            // Act
            NotFoundResult model = controller.DeleteMessage(int.MaxValue) as NotFoundResult;

            // Assert            
            mock.Verify(rep => rep.RemoveMessage(int.MaxValue), Times.Never);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void DeleteMessageNoContent(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            Message messageDeleted = messages[0];
            mock.SetupGet(rep => rep.Messages).Returns(messages);
            mock.SetupSequence(rep => rep.GetMessageById(messageDeleted.Id)).Returns(messageDeleted);            
            MessageController controller = new MessageController(mock.Object);

            // Act            
            NoContentResult model = controller.DeleteMessage(messageDeleted.Id) as NoContentResult;

            // Assert            
            mock.Verify(rep => rep.RemoveMessage(It.IsAny<int>()), Times.Once);
            Assert.Equal((int?)HttpStatusCode.NoContent, model.StatusCode);
        }

        [Fact]
        public void UpdateMessageThrowsException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Throws<FutTalkException>();
            MessageController controller = new MessageController(mock.Object);

            // Act & Asssert
            Exception ex = Assert.Throws<FutTalkException>(() => controller.UpdateMessage(
                int.MaxValue,
                new JsonPatchDocument<Message>()));
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
                null) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
            mock.Verify(rep => rep.GetMessageById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void UpdateMessageBadRequestMessageIdIsLessThanOne()
        {
            // Arrange
            var mock = new Mock<IRepository>();            
            MessageController controller = new MessageController(mock.Object);

            // Act
            BadRequestObjectResult model = controller.UpdateMessage(
                It.IsInRange<int>(0, int.MinValue, Range.Inclusive),
                It.IsAny<JsonPatchDocument<Message>>()) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.BadRequest, model.StatusCode);
            mock.Verify(rep => rep.GetMessageById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void UpdateMessageNotFound()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Returns(default(Message));
            MessageController controller = new MessageController(mock.Object);

            // Act
            NotFoundObjectResult model = controller.UpdateMessage(
                int.MaxValue,
                new JsonPatchDocument<Message>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.NotFound, model.StatusCode);
            mock.Verify(rep => rep.GetMessageById(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void UpdateMessageOk(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            Message message = messages[0];
            mock.SetupSequence(rep => rep.GetMessageById(It.IsAny<int>())).Returns(message);
            MessageController controller = new MessageController(mock.Object);

            // Act
            OkObjectResult model = controller.UpdateMessage(
                message.Id,
                new JsonPatchDocument<Message>()) as OkObjectResult;

            // Assert
            Assert.Equal((int?)HttpStatusCode.OK, model.StatusCode);
            mock.Verify(rep => rep.GetMessageById(It.IsAny<int>()), Times.Once);
            mock.Verify(rep => rep.UpdateMessage(It.IsAny<int>(), It.IsAny<Message>()), Times.Once);
        }
    }
}