using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class SiteMapControllerTests
    {
        private SiteMapController siteMapController;
        private IRepository<Product> productRepository;
        private IRepository<Content> contentRepository;
        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            productRepository = new Mock<IRepository<Product>>().Object;
            contentRepository = new Mock<IRepository<Content>>().Object;
            userService = new Mock<IUserService>().Object;

            siteMapController = new SiteMapController(productRepository, contentRepository, userService);

            Mock.Get(userService).ExpectGet(c => c.CurrentUser).Returns(new User());

        }

        [Test]
        public void Index_ShouldShowListOfProductsAndContent()
        {
            var products = new List<Product>().AsQueryable();
            var contents = new List<Content>().AsQueryable();

            Mock.Get(productRepository).Expect(pr => pr.GetAll()).Returns(products);
            Mock.Get(contentRepository).Expect(cr => cr.GetAll()).Returns(contents);

            siteMapController.Index()
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>()
                .AssertNotNull(vd => vd.Products)
                .AssertNotNull(vd => vd.Contents);
        }
    }
}
