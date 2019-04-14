using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using WebMVC.Infrastructure.TagHelpers;
using WebMVC.Models;
using WebMVC.Models.Pagination;
using Xunit;

namespace WebMVC.UnitTests
{
    public class PageLinkTagHelperTests
    {
        [Fact]
        public void CanGeneratePageLinks()
        {
            // Arrange
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.SetupSequence(
                x => x.Action(It.IsAny<UrlActionContext>()))
                    .Returns("Test/Page1")
                    .Returns("Test/Page2")
                    .Returns("Test/Page3");

            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory.Setup(
                f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                     .Returns(urlHelper.Object);

            MessagePageLinkTagHelper helper = new MessagePageLinkTagHelper(urlHelperFactory.Object)
            {
                PageModelMessages = new PagingInfo<Message>
                    {
                        CurrentPage = 2,
                        TotalItems = 28,
                        ItemsPerPage = 10
                    },
                PageAction = "Test"
            };

            TagHelperContext ctx = 
                new TagHelperContext(
                    new TagHelperAttributeList(),
                    new Dictionary<object, object>(), 
                    string.Empty);

            var content = new Mock<TagHelperContent>();
            TagHelperOutput output = 
                new TagHelperOutput(
                    "div",
                    new TagHelperAttributeList(),
                    (cache, encoder) => Task.FromResult(content.Object));
            
            // Act
            helper.Process(ctx, output);
            
            // Assert
            Assert.Equal(@"<a href=""Test/Page1"">1</a>"
                    + @"<a href=""Test/Page2"">2</a>"
                    + @"<a href=""Test/Page3"">3</a>",
                    output.Content.GetContent());
        }
    }
}
