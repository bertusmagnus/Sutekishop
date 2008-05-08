using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Suteki.Shop.Tests.Models
{
    [TestFixture]
    public class OrderTests
    {
        [Test]
        public void CalculatePostage_ShouldCalculateCorrectPostage()
        {
            // total weight = 350
            Order order = CreateOrder();
            var postages = CreatePostages();

            Assert.AreEqual(1.10M * 2.5M, order.CalculatePostage(postages).Price, "incorrect figure calculated");
        }

        [Test]
        public void CalculatePostage_WhenNoPostagesGivenThenPhone()
        {
            // total weight = 350
            Order order = CreateOrder();
            var postages = new List<Postage>().AsQueryable();

            Assert.IsTrue(order.CalculatePostage(postages).Phone, "phone is false");
        }

        [Test]
        public void CalculatePostage_ShouldPhoneOnMaxWeightBand()
        {
            // total weight = 350
            Order order = CreateOrder();

            // add one more item to make max weight band (weight now 450)
            order.Basket.BasketItems.Add(new BasketItem
            {
                Size = new Size
                    {
                        Product = new Product { Weight = 100 }
                    }
            });

            var postages = CreatePostages();

            Assert.IsTrue(order.CalculatePostage(postages).Phone, "phone is false");
        }

        private static IQueryable<Postage> CreatePostages()
        {
            var postages = new List<Postage>
            {
                new Postage { IsActive = true, MaxWeight = 0, Price = 0M },
                new Postage { IsActive = true, MaxWeight = 200, Price = 0.50M },
                new Postage { IsActive = true, MaxWeight = 400, Price = 1.10M }, // this should be chosen
                new Postage { IsActive = true, MaxWeight = int.MaxValue, Price = 2.20M }
            }.AsQueryable();
            return postages;
        }

        private static Order CreateOrder()
        {
            Order order = new Order
            {
                Basket = new Basket
                {
                    BasketItems = new System.Data.Linq.EntitySet<BasketItem>
                    {
                        new BasketItem { Size = new Size
                        {
                            Product = new Product { Weight = 100 }
                        }},
                        new BasketItem { Size = new Size
                        {
                            Product = new Product { Weight = 50 }
                        }},
                        new BasketItem { Size = new Size
                        {
                            Product = new Product { Weight = 200 }
                        }}
                    }
                },
                UseCardHolderContact = true,
                Contact = new Contact { Country = new Country { PostZone = new PostZone { Multiplier = 2.5M, AskIfMaxWeight = true } } }
            };
            return order;
        }
    }
}
