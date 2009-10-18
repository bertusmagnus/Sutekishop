using System;
using System.Collections.Generic;
using System.Linq;
using Suteki.Common.Extensions;

namespace Suteki.Shop.ViewData
{
    public static class CategoryViewDataExtensions
    {
        public static CategoryViewData GetRoot(this IEnumerable<CategoryViewData> categories)
        {
            return categories.Single(cat => cat.CategoryId == 1);
        }

        public static IEnumerable<CategoryViewData> MapToViewData(this IEnumerable<Category> domainCategories)
        {
            // get all categories at once to defeat lazy loading and map to CategoryViewData
            var categories = domainCategories
                .Select(category => new CategoryViewData
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    ParentId = category.ParentId,
                    Position = category.Position,
                    IsActive = category.IsActive,
                    ImageId = category.ImageId
                }).ToList();

            // tie parents to children
            categories
                .Where(cat => cat.ParentId.HasValue)
                .ForEach(category => categories.Single(cat => cat.CategoryId == category.ParentId).AddChild(category));

            return categories;
        }
    }
}