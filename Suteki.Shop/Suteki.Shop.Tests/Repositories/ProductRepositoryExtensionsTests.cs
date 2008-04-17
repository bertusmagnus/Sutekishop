using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Repositories;
using System.Collections.Generic;

namespace Suteki.Shop.Tests.Repositories
{
    [TestFixture]
    public class ProductRepositoryExtensionsTests
    {
        [Test]
        public void WhereCategoryIdIs_ShouldReturnListOfProductsForCategory()
        {
            Mock<Repository<Product>> productRepositoryMock = MockRepositoryBuilder.CreateProductRepository();

            int categoryId = 4;
            var products = productRepositoryMock.Object.GetAll().WhereCategoryIdIs(categoryId);

            AssertProductsReturnedBy_WhereCategoryIdIs4_AreCorrect(products);
        }

        public static void AssertProductsReturnedBy_WhereCategoryIdIs4_AreCorrect(IEnumerable<Product> products)
        {
            int categoryId = 4;
            Assert.AreEqual(2, products.Count(), "Unexpected number of products returned");
            foreach (Product product in products)
            {
                Assert.AreEqual(categoryId, product.CategoryId, "Incorrect categoryId returned");
            }
        }
    }
}
