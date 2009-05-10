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

			orders = orders.Where(x => x.OrderStatusId > 0); //Exclude 'Pending'

            if (criteria.OrderId != 0)
            {
                orders = orders.Where(o => o.OrderId == criteria.OrderId);
            }
            if (!string.IsNullOrEmpty(criteria.Email))
            {
                orders = orders.Where(o => o.Email == criteria.Email);
            }
            if (!string.IsNullOrEmpty(criteria.Postcode))
            {
                orders = orders
                    .Where(o => 
                        o.Contact.Postcode == criteria.Postcode || 
                        o.Contact1.Postcode == criteria.Postcode);
            }
            if (!string.IsNullOrEmpty(criteria.Lastname))
            {
                orders = orders.Where(o =>
                    o.Contact.Lastname == criteria.Lastname ||
                    o.Contact1.Lastname == criteria.Lastname);
            }
            if (criteria.OrderStatusId != 0)
            {
                orders = orders.Where(o => o.OrderStatusId == criteria.OrderStatusId);
            }
            return orders;
        }
    }
}
