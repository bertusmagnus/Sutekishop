using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using Rhino.Mocks;

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
            categoryRepository = MockRepository.GenerateStub<IRepository<Category>>();
            sizeRepository = MockRepository.GenerateStub<IRepository<Size>>();

            stockController = new StockController(
                categoryRepository,
                sizeRepository);
        }

        [Test]
        public void Index_ShouldPassRootCategoryToIndexView()
        {
            var root = BuildCategories();

            categoryRepository.Expect(cr => cr.GetById(1)).Return(root);

            stockController.Index()
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>()
                .AssertAreSame(root, vd => vd.Category);
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

            sizeRepository.Expect(s => s.GetAll()).Return(sizes);
            sizeRepository.Expect(s => s.SubmitChanges());

            var root = BuildCategories();
            categoryRepository.Expect(cr => cr.GetById(1)).Return(root);

            stockController.Index(form)
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>()
                .AssertNotNull(vd => vd.Category);

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
