using System;
using System.Linq;

namespace Suteki.Shop.Repositories
{
    public static class ProductRepositoryExtensions
    {
        public static IQueryable<Product> WhereCategoryIdIs(this IQueryable<Product> products, int categoryId)
        {
            return products.Where(p => p.CategoryId == categoryId);
        }
    }
}
