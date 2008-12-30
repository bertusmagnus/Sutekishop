using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        HomeController homeController;
        IRepository<Content> contentRepository;

        [SetUp]
        public void SetUp()
        {
            contentRepository = new Mock<IRepository<Content>>().Object;
            homeController = new HomeController(contentRepository);
        }

        [Test]
        public void IndexShouldRenderViewIndex()
        {
            var contents = new List<Content>
                {
                    new TextContent { UrlName = HomeController.Shopfront }
                }.AsQueryable();

            Mock.Get(contentRepository).Expect(cr => cr.GetAll()).Returns(contents);

            homeController.Index()
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<CmsViewData>()
                .AssertAreSame(
                    contents.OfType<ITextContent>().First(), 
                    vd => vd.TextContent);

        }
    }
}
