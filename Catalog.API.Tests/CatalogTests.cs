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

        [Theory]
        [ClassData(typeof(MessageTestData))]
        public void GetAllMessages(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.SetupGet(m => m.Messages).Returns(messages);
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
