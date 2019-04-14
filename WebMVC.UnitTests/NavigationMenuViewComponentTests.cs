using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using Xunit;
using WebMVC.Models;
using WebMVC.Infrastructure.Services;
using WebMVC.Infrastructure.ViewComponents;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace WebMVC.UnitTests
{
    public class NavigationMenuViewComponentTests
    {
        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void GetAllCategories(Message[] messages)
        {
            // Arrange
            Mock<IMessageService> mock = new Mock<IMessageService>();
            mock.Setup(srv => srv.GetAllMessages()).Returns(messages);
            MessageNavigationMenuViewComponent target = new MessageNavigationMenuViewComponent(mock.Object);

            // Acts
            string[] results = ((IEnumerable<string>)(target.Invoke() as ViewViewComponentResult).ViewData.Model).ToArray();

            // Assert            
            Assert.True(Enumerable.SequenceEqual(new string[] { "Cat1", "Cat2" }, results));
        }

        [Theory]
        [ClassData(typeof(MessagesTestData))]
        public void HighlightSelectedCategory(Message[] messages)
        {
            // Arrange
            string categoryToSelect = "Cat2";
            // Arrange
            Mock<IMessageService> mock = new Mock<IMessageService>();
            mock.Setup(srv => srv.GetAllMessages()).Returns(messages);
            MessageNavigationMenuViewComponent target = new MessageNavigationMenuViewComponent(mock.Object);

            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    RouteData = new RouteData()
                }
            };
            target.RouteData.Values["category"] = categoryToSelect;

            // Action
            string result = (string)(target.Invoke() as ViewViewComponentResult).ViewData["SelectedCategory"];

            // Assert
            Assert.Equal(categoryToSelect, result);
        }
    }
}
