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
using WebMVC.Controllers;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;
using WebMVC.Models.Pagination;
using WebMVC.Models.ViewModels;
using Xunit;

namespace WebMVC.UnitTests
{
    public class MessageControllerTests
    {
        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void GetAllMessages(Message[] messages)
        {
            // Arrange
            var mock = new Mock<IMessageService>();
            mock.SetupSequence(srv => srv.GetAllPaginatedMessagesAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
               .Returns(() => Task<PagingInfo<Message>>.Run(() =>
               {
                   return new PagingInfo<Message>
                   {
                       Items = messages.AsEnumerable<Message>(),
                       CurrentPage = 1,
                       ItemsPerPage = int.MaxValue,
                       TotalItems = messages.LongLength
                   };
               })
                );
            MessageController controller = new MessageController(mock.Object) { PageSize = int.MaxValue };

            // Act
            var result = await controller.Index(null) as ViewResult;
            var model = result?.Model as MessagesListViewModel;

            // Assert            
            Assert.Equal(messages.LongLength, model.PagingInfo.TotalItems);
            Assert.Equal(messages, model?.Messages, Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void GetPaginatedMessages(Message[] messages)
        {
            // Arrange
            int pageSize = 3;
            int page = 2;
            var mock = new Mock<IMessageService>();
            mock.SetupSequence(srv => srv.GetAllPaginatedMessagesAsync(null, pageSize, page))
               .Returns(() => Task<PagingInfo<Message>>.Run(() =>
                   {
                       List<Message> ls = new List<Message>();
                       ls.Add(messages[3]);
                       ls.Add(messages[4]);

                       return new PagingInfo<Message>
                       {
                           Items = ls.AsEnumerable<Message>(),
                           CurrentPage = page,
                           ItemsPerPage = pageSize,
                           TotalItems = messages.LongLength
                       };
                   })
                );
            MessageController controller = new MessageController(mock.Object) { PageSize = pageSize };

            // Act
            var result = await controller.Index(null, page) as ViewResult;
            var model = result?.Model as MessagesListViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.IsType<MessagesListViewModel>(model);
            Assert.Equal(model.PagingInfo.CurrentPage, page);
            Assert.Equal(model.PagingInfo.ItemsPerPage, pageSize);            
            Assert.Equal(messages.LongLength, model.PagingInfo.TotalItems);
            Assert.Equal(messages[3], model?.Messages?.ToArray<Message>()[0], Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
            Assert.Equal(messages[4], model?.Messages?.ToArray<Message>()[1], Comparer.Get<Message>((m1, m2) => m1.Id == m2.Id));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void GetMessagesByCategory(Message[] messages)
        {
            // Arrange
            string category = "Cat1";
            int pageSize = 5;
            int page = 1;
            var mock = new Mock<IMessageService>();
            mock.SetupSequence(srv => srv.GetAllPaginatedMessagesAsync(category, pageSize, page))
               .Returns(() => Task<PagingInfo<Message>>.Run(() =>
               {
                   List<Message> ls = new List<Message>();
                   ls.Add(messages[0]);
                   ls.Add(messages[2]);
                   ls.Add(messages[4]);

                   return new PagingInfo<Message>
                   {
                       Items = ls.AsEnumerable<Message>(),
                       CurrentPage = page,
                       ItemsPerPage = pageSize,
                       TotalItems = messages.LongLength
                   };
               })
                );
            MessageController controller = new MessageController(mock.Object) { PageSize = pageSize };

            // Act
            var result = await controller.Index(category, page) as ViewResult;
            var model = result?.Model as MessagesListViewModel;

            // Assert
            Assert.Equal(3, model.Messages.Count());
            Assert.Equal(model?.Messages?.ToArray<Message>()[0].Category, category);
            Assert.Equal(model?.Messages?.ToArray<Message>()[1].Category, category);
            Assert.Equal(model?.Messages?.ToArray<Message>()[2].Category, category);
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void CanPaginateNext(Message[] messages)
        {
            // Arrange
            int pageSize = 2;
            int page = 2;
            var mock = new Mock<IMessageService>();
            mock.SetupSequence(srv => srv.GetAllPaginatedMessagesAsync(null, pageSize, page))
               .Returns(() => Task<PagingInfo<Message>>.Run(() =>
                   {
                       return new PagingInfo<Message>
                       {
                           Items = messages.AsEnumerable<Message>(),
                           CurrentPage = page,
                           ItemsPerPage = pageSize,
                           TotalItems = messages.LongLength
                       };
                   })
                );
            MessageController controller = new MessageController(mock.Object) { PageSize = pageSize };

            // Act
            var result = await controller.Index(null, page) as ViewResult;
            var model = result?.Model as MessagesListViewModel;

            // Assert
            Assert.True(model.PagingInfo.CanNext());
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void CanNotPaginateNext(Message[] messages)
        {
            // Arrange
            int pageSize = 3;
            int page = 2;
            var mock = new Mock<IMessageService>();
            mock.SetupSequence(srv => srv.GetAllPaginatedMessagesAsync(null, pageSize, page))
               .Returns(() => Task<PagingInfo<Message>>.Run(() =>
                   {
                       List<Message> ls = new List<Message>();
                       ls.Add(messages[0]);

                       return new PagingInfo<Message>
                       {
                           Items = ls.AsEnumerable<Message>(),
                           CurrentPage = page,
                           ItemsPerPage = pageSize,
                           TotalItems = messages.LongLength
                       };
                   })
                );
            MessageController controller = new MessageController(mock.Object) { PageSize = pageSize };

            // Act
            var result = await controller.Index(null, page) as ViewResult;
            var model = result?.Model as MessagesListViewModel;

            // Assert            
            Assert.False(model.PagingInfo.CanNext());
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void CanPaginatePrevious(Message[] messages)
        {
            // Arrange
            int pageSize = 2;
            int page = 2;
            var mock = new Mock<IMessageService>();
            mock.SetupSequence(srv => srv.GetAllPaginatedMessagesAsync(null, pageSize, page))
               .Returns(() => Task<PagingInfo<Message>>.Run(() =>
                   {
                       return new PagingInfo<Message>
                       {
                           Items = messages.AsEnumerable<Message>(),
                           CurrentPage = page,
                           ItemsPerPage = pageSize,
                           TotalItems = messages.LongLength
                       };
                   })
               );
            MessageController controller = new MessageController(mock.Object) { PageSize = pageSize };

            // Act
            var result = await controller.Index(null, page) as ViewResult;
            var model = result?.Model as MessagesListViewModel;

            // Assert
            Assert.False(model.PagingInfo.CanPrevious());
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public async void CanNotPaginatePrevious(Message[] messages)
        {
            // Arrange
            int pageSize = 3;
            int page = 1;
            var mock = new Mock<IMessageService>();
            mock.SetupSequence(srv => srv.GetAllPaginatedMessagesAsync(null, pageSize, page))
               .Returns(() => Task<PagingInfo<Message>>.Run(() =>
                   {
                       return new PagingInfo<Message>
                       {
                           Items = messages.AsEnumerable<Message>(),
                           CurrentPage = page,
                           ItemsPerPage = pageSize,
                           TotalItems = messages.LongLength
                       };
                   })
               );
            MessageController controller = new MessageController(mock.Object) { PageSize = pageSize };

            // Act
            var result = await controller.Index(null, page) as ViewResult;
            var model = result?.Model as MessagesListViewModel;

            // Assert            
            Assert.True(model.PagingInfo.CanPrevious());
        }
    }
}
