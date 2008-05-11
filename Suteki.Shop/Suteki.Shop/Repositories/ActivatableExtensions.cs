using System.Linq;
using System.Collections.Generic;

namespace Suteki.Shop.Repositories
{
    public static class ActivatableExtensions
    {
        public static IQueryable<T> Active<T>(this IQueryable<T> items) where T : IActivatable
        {
            return items.Where(item => item.IsActive);
        }

        public static IEnumerable<T> Active<T>(this IEnumerable<T> items) where T : IActivatable
        {
            return items.AsQueryable().Active();
        }
    }
}
