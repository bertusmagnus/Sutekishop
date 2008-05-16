using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;
using System.Collections.Specialized;
using NUnit.Framework.SyntaxHelpers;
using Suteki.Shop.Repositories;

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
            Category root = BuildCategories();

            Mock.Get(categoryRepository).Expect(cr => cr.GetById(1)).Returns(root);

            stockController.Index()
                .ReturnsRenderViewResult()
                .ForView("Index")
                .AssertAreSame(root, vd => vd.Category);
        }

        private Category BuildCategories()
        {
            Category root = new Category { CategoryId = 1, Name = "Root" };
            return root;
        }

        private static IEnumerable<StockItem> CreateStockItems()
        {
            IEnumerable<StockItem> stockItems = new List<StockItem>
            {
                new StockItem { SizeId = 1, IsInStock = false },
                new StockItem { SizeId = 2, IsInStock = true }
            };
            return stockItems;
        }

        [Test]
        public void Update_ShouldUpdateStockItems()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("stockitem_1", "True");
            Mock.Get(stockController).ExpectGet(c => c.Form).Returns(form);

            var sizes = CreateSizes();

            Mock.Get(sizeRepository).Expect(s => s.GetAll()).Returns(sizes);
            Mock.Get(sizeRepository).Expect(s => s.SubmitChanges());

            Category root = BuildCategories();
            Mock.Get(categoryRepository).Expect(cr => cr.GetById(1)).Returns(root);

            stockController.Update()
                .ReturnsRenderViewResult()
                .ForView("Index")
                .AssertNotNull(vd => vd.Category);

            Assert.That(sizes.First().IsInStock, Is.True);
            Assert.That(sizes.Last().IsInStock, Is.False);
        }

        private IQueryable<Size> CreateSizes()
        {
            return new List<Size>
            {
                new Size { SizeId = 1, IsInStock = false },
                new Size { SizeId = 2, IsInStock = true }
            }.AsQueryable();
        }
    }
}
