using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Collections.Generic;

namespace Suteki.Shop.Tests.Models
{
    [TestFixture]
    public class OrderTests
    {
        public static Order Create350GramOrder()
        {
            Order order = new Order
            {
                Basket = BasketTests.Create350GramBasket(),
                UseCardHolderContact = true,
                Contact = new Contact { Country = new Country
                    {
                       PostZone = new PostZone { Multiplier = 2.5M, FlatRate = 10.00M, AskIfMaxWeight = false }
                    } },
                Email = "mike@mike.com",
                CreatedDate = new DateTime(2008, 10, 18),
                OrderStatus = new OrderStatus { Name = "Dispatched" }
            };
            return order;
        }

        public static Order Create450GramOrder()
        {
            Order order = Create350GramOrder();

            // add one more item to make max weight band (weight now 450)
            order.Basket.BasketItems.Add(new BasketItem
            {
                Quantity = 1,
                Size = new Size
                {
                    Product = new Product { Weight = 100 }
                }
            });
            return order;
        }
    }
}
