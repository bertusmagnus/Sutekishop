using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Repositories
{
    [TestFixture]
    public class ProductRepositoryExtensionsTests
    {
        [Test]
        public void WhereCategoryIdIs_ShouldReturnListOfProductsForCategory()
        {
            var productRepositoryMock = MockRepositoryBuilder.CreateProductRepository();

            const int categoryId = 4;
            var products = productRepositoryMock.GetAll().WhereCategoryIdIs(categoryId);

            AssertProductsReturnedBy_WhereCategoryIdIs4_AreCorrect(products);
        }

        public static void AssertProductsReturnedBy_WhereCategoryIdIs4_AreCorrect(IEnumerable<Product> products)
        {
            const int categoryId = 4;
            Assert.AreEqual(2, products.Count(), "Unexpected number of products returned");
            foreach (var product in products)
            {
                Assert.AreEqual(categoryId, product.CategoryId, "Incorrect categoryId returned");
            }
        }
    }
}
