using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Shop.Controllers;
using System.Collections.Specialized;
using Suteki.Shop.Tests.TestHelpers;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class StockControllerTests
    {
        StockController stockController;
        IRepository<Category> categoryRepository;
        IRepository<Size> sizeRepository;

        [SetUp]
        public void SetUp()
        {
            categoryRepository = new Mock<IRepository<Category>>().Object;
            sizeRepository = new Mock<IRepository<Size>>().Object;

            stockController = new Mock<StockController>(
                categoryRepository,
                sizeRepository).Object;
        }

        [Test]
        public void Index_ShouldPassRootCategoryToIndexView()
        {
            var root = BuildCategories();

            Mock.Get(categoryRepository).Expect(cr => cr.GetById(1)).Returns(root);

            stockController.Index()
                .ReturnsViewResult()
                .ForView("Index")
                .AssertAreSame<ShopViewData, Category>(root, vd => vd.Category);
        }

        private static Category BuildCategories()
        {
            var root = new Category { CategoryId = 1, Name = "Root" };
            return root;
        }

        [Test]
        public void Update_ShouldUpdateStockItems()
        {
            var form = new FormCollection
            {
                {"stockitem_0", "false"},
                {"stockitem_1", "true,false"},
                {"stockitem_2", "false"}
            };

            var sizes = CreateSizes();

            Mock.Get(sizeRepository).Expect(s => s.GetAll()).Returns(sizes);
            Mock.Get(sizeRepository).Expect(s => s.SubmitChanges());

            var root = BuildCategories();
            Mock.Get(categoryRepository).Expect(cr => cr.GetById(1)).Returns(root);

            stockController.Update(form)
                .ReturnsViewResult()
                .ForView("Index")
                .AssertNotNull<ShopViewData, Category>(vd => vd.Category);

            Assert.That(sizes.First().IsInStock, Is.True);
            Assert.That(sizes.Last().IsInStock, Is.False);
        }

        private static IQueryable<Size> CreateSizes()
        {
            return new List<Size>
            {
                new Size { SizeId = 1, IsInStock = false },
                new Size { SizeId = 2, IsInStock = true }
            }.AsQueryable();
        }
    }
}
