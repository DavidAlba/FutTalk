using Catalog.API.Controllers;
using Catalog.API.Infrastructure;
using Catalog.API.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Catalog.API.Tests
{
    public class CatalogTests
    {
        [Fact]
        public void GetAllMessagesThrowFutTalkException()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupGet(m => m.Messages).Throws<FutTalkException>();
            CatalogController controller = new CatalogController(mock.Object);

            // Act & Assert            
            Exception ex = Assert.Throws<FutTalkException>(() => controller.Get());
            Assert.Equal(expected: typeof(FutTalkException), actual: ex.GetType());
        }

        [Fact]
        public void GetAllMessages()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupGet(m => m.Messages).Returns
                (
                    new Message[] {
                        new Message
                        {
                            Id = 1,
                            Name = "Msg1 Name",
                            Body = "Msg1 Body"
                        },
                        new Message
                        {
                            Id = 2,
                            Name = "Msg2 Name",
                            Body = "Msg2 Body"
                        },
                        new Message
                        {
                            Id = 3,
                            Name = "Msg3 Name",
                            Body = "Msg3 Body"
                        }
                    }
                );
            CatalogController controller = new CatalogController(mock.Object);

            // Act
            IQueryable<Message> model = controller.Get().AsQueryable<Message>();

            // Assert
            Assert.Equal(
                expected: controller.Repository.Messages,
                actual: model,
                comparer: Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));

        }
    }
}
