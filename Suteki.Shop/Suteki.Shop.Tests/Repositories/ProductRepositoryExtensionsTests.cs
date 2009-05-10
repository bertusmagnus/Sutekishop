using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Repositories
{
    public class ProductRepositoryExtensionsTests
    {
       public static void AssertProductsReturnedBy_WhereCategoryIdIs4_AreCorrect(IEnumerable<Product> products)
        {
            const int categoryId = 4;
            Assert.AreEqual(2, products.Count(), "Unexpected number of products returned");
            foreach (var product in products)
            {
				bool success = product.ProductCategories.Any(x => x.CategoryId == categoryId);
				Assert.IsTrue(success, "Incorrect categoryId returned");
            }
        }
    }
}
