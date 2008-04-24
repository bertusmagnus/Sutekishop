using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;

namespace Suteki.Shop.Repositories
{
    public static class IOrderableExtensions
    {
        public static IQueryable<T> InOrder<T>(this IQueryable<T> items) where T : IOrderable
        {
            return items.OrderBy(i => i.Position);
        }

        public static IEnumerable<T> InOrder<T>(this IEnumerable<T> items) where T : IOrderable
        {
            return InOrder(items.AsQueryable());
        }

        public static IOrderable AtPosition<T>(this IQueryable<T> items, int position) where T : IOrderable
        {
            return items.SingleOrDefault(i => i.Position == position);
        }

        public static int GetNextPosition<T>(this IEnumerable<T> items) where T : IOrderable
        {
            return items.Max(i => i.Position) + 1;
        }
    }
}
