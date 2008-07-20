using System.Linq;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Suteki.Common.Repositories;
using Suteki.Shop.Services;
using Suteki.Shop.Tests.Models;
using System.Collections.Generic;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class PostageServiceTests
    {
        private IPostageService postageService;
        private IRepository<Postage> postageRepository;

        [SetUp]
        public void SetUp()
        {
            postageRepository = new Mock<IRepository<Postage>>().Object;

            postageService = new PostageService(postageRepository);

            var postages = PostageTests.CreatePostages();
            Mock.Get(postageRepository).Expect(pr => pr.GetAll()).Returns(postages);
        }

        [Test]
        public void CalculatePostageForBasket_ShouldCalculatePostageForABasket()
        {
            Basket basket = BasketTests.Create350GramBasket();
            postageService.CalculatePostageFor(basket);

            Assert.That(basket.PostageTotal, Is.EqualTo("£2.75"));
        }

        [Test]
        public void CalculatePostageForOrder_ShouldWorkForCompleteOrder()
        {
            Order order = OrderTests.Create350GramOrder();
            postageService.CalculatePostageFor(order);

            Assert.That(order.Basket.PostageTotal, Is.EqualTo("£2.75"));
        }

        [Test]
        public void CalculatePostageForOrder_ShouldWorkForIncompleteOrder()
        {
            Order order = OrderTests.Create350GramOrder();

            // remove the contacts
            order.Contact = null;
            order.Contact1 = null;

            postageService.CalculatePostageFor(order);

            Assert.That(order.Basket.PostageTotal, Is.EqualTo("£2.75"));
        }

        [Test]
        public void CalculatePostage_ShouldCalculateCorrectPostage()
        {
            // total weight = 350
            Order order = OrderTests.Create350GramOrder();

            Assert.AreEqual(1.10M * 2.5M, postageService.CalculatePostageFor(order).Price, "incorrect figure calculated");
        }

        [Test]
        public void CalculatePostage_WhenNoPostagesGivenThenUseFlatRate()
        {
            // total weight = 350
            Order order = OrderTests.Create350GramOrder();
            IQueryable<Postage> postages = new List<Postage>().AsQueryable();
            Mock.Get(postageRepository).Expect(pr => pr.GetAll()).Returns(postages);

            Assert.IsFalse(postageService.CalculatePostageFor(order).Phone, "phone is true");
            Assert.That(postageService.CalculatePostageFor(order).Price, Is.EqualTo(10.00M), "Incorrect price calculated");
        }

        [Test]
        public void CalculatePostage_ShouldUseFlatRateOnMaxWeightBand()
        {
            Order order = OrderTests.Create450GramOrder();

            Assert.IsFalse(postageService.CalculatePostageFor(order).Phone, "phone is true");
            Assert.That(postageService.CalculatePostageFor(order).Price, Is.EqualTo(10.00M), "Incorrect price calculated");
        }

        [Test]
        public void CalculatePostage_ShouldPhoneIfPhoneOnMaxWeightIsTrue()
        {
            Order order = OrderTests.Create450GramOrder();

            // replace the order contact (AskIfMaxWeight is true, FlatRate is 123.45)
            order.Contact = new Contact
            {
                Country = new Country
                {
                    PostZone = new PostZone { Multiplier = 2.5M, AskIfMaxWeight = true, FlatRate = 123.45M }
                }
            };
            order.UpdateBasket();

            Assert.That(postageService.CalculatePostageFor(order).Phone, Is.True, "phone is false");
        }
    }
}
