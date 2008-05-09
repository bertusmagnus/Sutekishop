using System;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        OrderController orderController;

        IRepository<Order> orderRepository;
        IRepository<Basket> basketRepository;
        IRepository<Country> countryRepository;
        IRepository<CardType> cardTypeRepository;
        IRepository<Postage> postageRepository;

        ControllerTestContext testContext;

        [SetUp]
        public void SetUp()
        {
            orderRepository = new Mock<IRepository<Order>>().Object;
            basketRepository = new Mock<IRepository<Basket>>().Object;
            countryRepository = new Mock<IRepository<Country>>().Object;
            cardTypeRepository = new Mock<IRepository<CardType>>().Object;
            postageRepository = new Mock<IRepository<Postage>>().Object;

            orderController = new OrderController(
                orderRepository,
                basketRepository,
                countryRepository,
                cardTypeRepository,
                postageRepository);

            testContext = new ControllerTestContext(orderController);

            Mock.Get(postageRepository).Expect(p => p.GetAll()).Returns(new List<Postage>().AsQueryable());
            
            testContext.TestContext.ContextMock.ExpectGet(h => h.User).Returns(new User { UserId = 4 });
            testContext.TestContext.RequestMock.ExpectGet(r => r.RequestType).Returns("GET");
            testContext.TestContext.RequestMock.ExpectGet(r => r.QueryString).Returns(new NameValueCollection());
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(new NameValueCollection());
        }

        [Test]
        public void Checkout_ShouldDisplayCheckoutForm()
        {
            int basketId = 6;

            Basket basket = new Basket { BasketId = basketId };
            IQueryable<Country> countries = new List<Country> { new Country() }.AsQueryable();
            IQueryable<CardType> cardTypes = new List<CardType> { new CardType() }.AsQueryable();

            // expectations
            Mock.Get(basketRepository).Expect(br => br.GetById(basketId))
                .Returns(basket)
                .Verifiable();

            Mock.Get(countryRepository).Expect(cr => cr.GetAll())
                .Returns(countries)
                .Verifiable();

            Mock.Get(cardTypeRepository).Expect(ctr => ctr.GetAll())
                .Returns(cardTypes)
                .Verifiable();


            // exercise Checkout action
            orderController.Checkout(basketId)
                .ReturnsRenderViewResult()
                .AssertNotNull(vd => vd.Order)
                .AssertNotNull(vd => vd.Order.Contact)
                .AssertNotNull(vd => vd.Order.Contact1)
                .AssertNotNull(vd => vd.Order.Card)
                .AssertAreSame(basket, vd => vd.Order.Basket)
                .AssertNotNull(vd => vd.Countries)
                .AssertAreSame(cardTypes, vd => vd.CardTypes);

            Mock.Get(basketRepository).Verify();
            Mock.Get(countryRepository).Verify();
            Mock.Get(cardTypeRepository).Verify();
        }

        [Test]
        public void PlaceOrder_ShouldCreateANewOrder()
        {
            // mock the request form
            NameValueCollection form = BuildPlaceOrderRequest();
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);

            // expectations
            Basket basket = new Basket();
            Order order = new Order();

            Mock.Get(orderRepository).Expect(or => or.InsertOnSubmit(It.IsAny<Order>()))
                .Callback<Order>(o => { order = o; order.Basket = basket; })
                .Verifiable();
            Mock.Get(orderRepository).Expect(or => or.SubmitChanges()).Verifiable();

            // exercise PlaceOrder action
            RenderViewResult result = orderController.PlaceOrder() as RenderViewResult;

            // Assertions
            Assert.IsNotNull(result, "result is not a RenderViewResult");

            Assert.AreNotEqual("Checkout", result.ViewName, ((ShopViewData)result.ViewData).ErrorMessage);
            Assert.AreEqual("Item", result.ViewName, "View name is not 'Item'");

            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "view data is not ShopViewData");

            Assert.AreSame(order, viewData.Order, "The view data order not correct");

            // Order
            Assert.AreEqual(10, order.OrderId, "OrderId is incorrect");
            Assert.AreEqual(form["order.email"], order.Email, "Email is incorrect");
            Assert.AreEqual(form["order.additionalinformation"], order.AdditionalInformation, "AdditionalInformation is incorrect");
            Assert.IsFalse(order.UseCardHolderContact, "UseCardHolderContact is incorrect");
            Assert.IsFalse(order.PayByTelephone, "PayByTelephone is incorrect");

            Assert.AreEqual(1, order.OrderStatusId, "OrderStatusId is incorrect");
            Assert.AreEqual(DateTime.Now.ToShortDateString(), order.CreatedDate.ToShortDateString(), "CreatedDate is incorrect");

            // Card Contact
            Contact cardContact = order.Contact;
            AssertContactIsCorrect(form, cardContact, "cardcontact");

            // Delivery Contact
            Contact deliveryContact = order.Contact1;
            AssertContactIsCorrect(form, deliveryContact, "deliverycontact");

            // Card
            Card card = order.Card;
            Assert.IsNotNull(card, "card is null");
            Assert.AreEqual(form["card.cardtypeid"], card.CardTypeId.ToString());
            Assert.AreEqual(form["card.holder"], card.Holder);
            Assert.AreEqual(form["card.number"], card.Number);
            Assert.AreEqual(form["card.expirymonth"], card.ExpiryMonth.ToString());
            Assert.AreEqual(form["card.expiryyear"], card.ExpiryYear.ToString());
            Assert.AreEqual(form["card.startmonth"], card.StartMonth.ToString());
            Assert.AreEqual(form["card.startyear"], card.StartYear.ToString());
            Assert.AreEqual(form["card.issuenumber"], card.IssueNumber.ToString());
            Assert.AreEqual(form["card.securitycode"], card.SecurityCode.ToString());

            Mock.Get(orderRepository).Verify();
        }

        private static void AssertContactIsCorrect(NameValueCollection form, Contact contact, string prefix)
        {
            Assert.IsNotNull(contact, prefix + " is null");
            Assert.AreEqual(form[prefix + ".firstname"], contact.Firstname, prefix + " Firstname is incorrect");
            Assert.AreEqual(form[prefix + ".lastname"], contact.Lastname, prefix + " Lastname is incorrect");
            Assert.AreEqual(form[prefix + ".address1"], contact.Address1, prefix + " Address1 is incorrect");
            Assert.AreEqual(form[prefix + ".address2"], contact.Address2, prefix + " Address2 is incorrect");
            Assert.AreEqual(form[prefix + ".address3"], contact.Address3, prefix + " Address3 is incorrect");
            Assert.AreEqual(form[prefix + ".town"], contact.Town, prefix + " Town is incorrect");
            Assert.AreEqual(form[prefix + ".county"], contact.County, prefix + " County is incorrect");
            Assert.AreEqual(form[prefix + ".postcode"], contact.Postcode, prefix + " Postcode is incorrect");
            //Assert.AreEqual(form[prefix + ".countryid"], contact.CountryId.ToString(), prefix + " CountryId is incorrect");
            Assert.AreEqual(form[prefix + ".telephone"], contact.Telephone, prefix + " Telephone is incorrect");
        }

        private static NameValueCollection BuildPlaceOrderRequest()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("order.orderid", "10");
            form.Add("order.basketid", "22");

            form.Add("cardcontact.firstname", "Mike");
            form.Add("cardcontact.lastname", "Hadlow");
            form.Add("cardcontact.address1", "23 The Street");
            form.Add("cardcontact.address2", "The Manor");
            form.Add("cardcontact.address3", "");
            form.Add("cardcontact.town", "Hove");
            form.Add("cardcontact.county", "East Sussex");
            form.Add("cardcontact.postcode", "BN6 2EE");
            form.Add("cardcontact.countryid", "1");
            form.Add("cardcontact.telephone", "01273 234234");

            form.Add("order.email", "mike@mike.com");
            form.Add("emailconfirm", "mike@mike.com");

            form.Add("order.usecardholdercontact", "False");

            form.Add("deliverycontact.firstname", "Mike");
            form.Add("deliverycontact.lastname", "Hadlow");
            form.Add("deliverycontact.address1", "23 The Street");
            form.Add("deliverycontact.address2", "The Manor");
            form.Add("deliverycontact.address3", "");
            form.Add("deliverycontact.town", "Hove");
            form.Add("deliverycontact.county", "East Sussex");
            form.Add("deliverycontact.postcode", "BN6 2EE");
            form.Add("deliverycontact.countryid", "1");
            form.Add("deliverycontact.telephone", "01273 234234");

            form.Add("order.additionalinformation", "some more info");

            form.Add("card.cardtypeid", "1");
            form.Add("card.holder", "MR M HADLOW");
            form.Add("card.number", "1111111111111117");
            form.Add("card.expirymonth", "3");
            form.Add("card.expiryyear", "2009");
            form.Add("card.startmonth", "2");
            form.Add("card.startyear", "2003");
            form.Add("card.issuenumber", "3");
            form.Add("card.securitycode", "235");

            form.Add("order.paybytelephone", "False");
            return form;
        }

        [Test]
        public void Index_ShouldDisplayAListOfOrders()
        {
            var orders = new List<Order> { new Order() }.AsQueryable();
            Mock.Get(orderRepository).Expect(or => or.GetAll()).Returns(orders).Verifiable();

            orderController.Index()
                .ReturnsRenderViewResult()
                .ForView("Index")
                .AssertAreSame(orders.First(), vd => vd.Orders.First());
        }

        [Test]
        public void Dispatch_ShouldChangeOrderStatusAndDispatchedDate()
        {
            int orderId = 44;
            Order order = new Order 
            { 
                OrderId = orderId, 
                OrderStatusId = OrderStatus.CreatedId,
                Basket = new Basket()
            };

            Mock.Get(orderRepository).Expect(or => or.GetById(orderId)).Returns(order);
            Mock.Get(orderRepository).Expect(or => or.SubmitChanges());

            orderController.Dispatch(orderId)
                .ReturnsRenderViewResult()
                .ForView("Item");

            Assert.IsTrue(order.IsDispatched, "order has not been dispatched");
            Assert.AreEqual(DateTime.Now.ToShortDateString(), order.DispatchedDateAsString, 
                "DispatchedDateAsString is incorrect");
            Assert.AreEqual(4, order.UserId, "UserId is incorrect"); // set SetUp
        }

        [Test]
        public void Index_ShouldBuildCriteriaAndExecuteSearch()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("orderid", "3");
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(form);

            var orders = new List<Order>
            {
                new Order { OrderId = 2 },
                new Order { OrderId = 3 }
            }.AsQueryable();

            Mock.Get(orderRepository).Expect(or => or.GetAll()).Returns(orders);

            orderController.Index()
                .ReturnsRenderViewResult()
                .ForView("Index")
                .AssertAreSame(orders.ElementAt(1), vd => vd.Orders.First());
                
        }
    }
}
