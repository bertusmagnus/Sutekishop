using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;
using System.Collections.Specialized;
using NUnit.Framework.SyntaxHelpers;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class StockControllerTests
    {
        StockController stockController;
        IStockService stockService;

        [SetUp]
        public void SetUp()
        {
            stockService = new Mock<IStockService>().Object;

            stockController = new Mock<StockController>(stockService).Object;
        }

        [Test]
        public void Index_ShouldShowAllStock()
        {
            IEnumerable<StockItem> stockItems = CreateStockItems();

            Mock.Get(stockService).Expect(s => s.GetAll()).Returns(stockItems);

            stockController.Index()
                .ReturnsRenderViewResult()
                .ForView("Index")
                .AssertAreSame(stockItems, vd => vd.StockItems);
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

            var stockItems = CreateStockItems();

            Mock.Get(stockService).Expect(s => s.GetAll()).Returns(stockItems);
            Mock.Get(stockService).Expect(s => s.Update(stockItems));

            stockController.Update()
                .ReturnsRenderViewResult()
                .ForView("Index")
                .AssertAreSame(stockItems, vd => vd.StockItems);

            Assert.That(stockItems.First().IsInStock, Is.True);
            Assert.That(stockItems.Last().IsInStock, Is.False);
        }
    }
}
