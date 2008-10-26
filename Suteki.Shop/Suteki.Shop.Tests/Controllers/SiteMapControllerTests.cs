using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.TestHelpers;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class SiteMapControllerTests
    {
        private SiteMapController siteMapController;
        private IRepository<Product> productRepository;
        private IRepository<Content> contentRepository;

        [SetUp]
        public void SetUp()
        {
            productRepository = new Mock<IRepository<Product>>().Object;
            contentRepository = new Mock<IRepository<Content>>().Object;

            siteMapController = new Mock<SiteMapController>(productRepository, contentRepository).Object;

            Mock.Get(siteMapController).ExpectGet(c => c.CurrentUser).Returns(new User());

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
                .AssertNotNull<ShopViewData, IEnumerable<Product>>(vd => vd.Products)
                .AssertNotNull<ShopViewData, IEnumerable<Content>>(vd => vd.Contents);
        }
    }
}
