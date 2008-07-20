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
                Country = new Country
                {
                    PostZone = new PostZone
                    {
                        AskIfMaxWeight = false,
                        Multiplier = 2.5M,
                        FlatRate = 10.00M
                    }
                },
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
