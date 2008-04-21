using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace Suteki.Shop.Repositories
{
    public static class OrderRepositoryExtensions
    {
        public static Order CurrentOrder(this IQueryable<Order> orders)
        {
            if (orders.Count() == 0) throw new ApplicationException("There is no current order");
            return orders.First();
        }

        public static Order CurrentOrder(this IEnumerable<Order> orders)
        {
            return orders.AsQueryable().CurrentOrder();
        }
    }
}
