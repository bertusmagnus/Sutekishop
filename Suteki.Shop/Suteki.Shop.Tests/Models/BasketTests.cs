using NUnit.Framework;

namespace Suteki.Shop.Tests.Models
{
    [TestFixture]
    public class BasketTests
    {
        public static Basket Create350GramBasket()
        {
            return new Basket
            {
                BasketItems = new System.Data.Linq.EntitySet<BasketItem>
                {
                    new BasketItem
                    {
                        Quantity = 10,
                        Size = new Size
                        {
                            Product = new Product {Weight = 10}
                        }
                    },
                    new BasketItem
                    {
                        Quantity = 5,
                        Size = new Size
                        {
                            Product = new Product {Weight = 10}
                        }
                    },
                    new BasketItem
                    {
                        Quantity = 4,
                        Size = new Size
                        {
                            Product = new Product {Weight = 50}
                        }
                    }
                }
            };
        }
    }
}
