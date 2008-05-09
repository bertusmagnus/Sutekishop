using System;
using System.Linq;

namespace Suteki.Shop.Repositories
{
    public static class OrderRepositoryExtensions
    {
        public static IQueryable<Order> ByCreatedDate(this IQueryable<Order> orders)
        {
            return orders.OrderByDescending(o => o.CreatedDate);
        }

        public static IQueryable<Order> ThatMatch(this IQueryable<Order> orders, OrderSearchCriteria criteria)
        {
            if (criteria.HasOrderId)
            {
                orders = orders.Where(o => o.OrderId == criteria.OrderId);
            }
            if (criteria.HasEmail)
            {
                orders = orders.Where(o => o.Email == criteria.Email);
            }
            if (criteria.HasPostcode)
            {
                orders = orders
                    .Where(o => 
                        o.Contact.Postcode == criteria.Postcode || 
                        o.Contact1.Postcode == criteria.Postcode);
            }
            if (criteria.HasLastname)
            {
                orders = orders.Where(o =>
                    o.Contact.Lastname == criteria.Lastname ||
                    o.Contact1.Lastname == criteria.Lastname);
            }
            if (criteria.HasOrderStatusId)
            {
                orders = orders.Where(o => o.OrderStatusId == criteria.OrderStatusId);
            }
            return orders;
        }
    }
}
