using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Services;
using NUnit.Framework.SyntaxHelpers;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class StockServiceTests
    {
        IStockService stockService;
        IRepository<Size> sizeRepository;

        [SetUp]
        public void SetUp()
        {
            sizeRepository = new Mock<IRepository<Size>>().Object;
            stockService = new StockService(sizeRepository);
        }

        [Test]
        public void GetAll_ShouldReturnAllStockItems()
        {
            var sizes = CreateSizes();
            Mock.Get(sizeRepository).Expect(sr => sr.GetAll()).Returns(sizes);

            var stockItems = stockService.GetAll();

            Assert.That(stockItems, Is.Not.Null);
            Assert.That(stockItems.Count(), Is.EqualTo(3));
            Assert.That(stockItems.Single(i => i.SizeId == 1).Name, Is.EqualTo("product one"));
            Assert.That(stockItems.Single(i => i.SizeId == 1).IsInStock, Is.True);
            Assert.That(stockItems.Single(i => i.SizeId == 5).Name, Is.EqualTo("product three"));
            Assert.That(stockItems.Single(i => i.SizeId == 3).IsInStock, Is.False);
        }

        [Test]
        public void Update_ShouldUpdateAllStockItems()
        {
            var stockItems = new List<StockItem>
            {
                new StockItem { SizeId = 1, IsInStock = false, Name = "whatever" },
                new StockItem { SizeId = 3, IsInStock = true, Name = "whatever" }
            };

            // expectations
            var sizes = CreateSizes();
            Mock.Get(sizeRepository).Expect(sr => sr.GetAll()).Returns(sizes);
            Mock.Get(sizeRepository).Expect(sr => sr.SubmitChanges());

            stockService.Update(stockItems);

            Assert.That(sizes.Single(s => s.SizeId == 1).IsInStock, Is.False);
            Assert.That(sizes.Single(s => s.SizeId == 3).IsInStock, Is.True);
            Assert.That(sizes.Single(s => s.SizeId == 5).IsInStock, Is.True);
        }

        private static IQueryable<Size> CreateSizes()
        {
            var sizes = new List<Size>
            {
                new Size { SizeId = 1, IsInStock = true, Product = new Product { Name = "product one" }, IsActive = true },
                new Size { SizeId = 3, IsInStock = false, Product = new Product { Name = "product two" }, IsActive = true },
                new Size { SizeId = 5, IsInStock = true, Product = new Product { Name = "product three" }, IsActive = true },
                new Size { SizeId = 7, IsInStock = false, Product = new Product { Name = "not active" }, IsActive = false }
            }.AsQueryable();
            return sizes;
        }
    }
}

