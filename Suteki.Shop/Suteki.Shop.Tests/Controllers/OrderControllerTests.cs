using System;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Controllers;
using System.Web.Mvc;
using Suteki.Shop.Tests.TestHelpers;
using Suteki.Shop.ViewData;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using Suteki.Shop.Services;

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

        IEncryptionService encryptionService;
        IEmailSender emailSender;
        IPostageService postageService;

        ControllerTestContext testContext;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the order controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { "Administrator" });

            orderRepository = new Mock<IRepository<Order>>().Object;
            basketRepository = new Mock<IRepository<Basket>>().Object;
            countryRepository = new Mock<IRepository<Country>>().Object;
            cardTypeRepository = new Mock<IRepository<CardType>>().Object;

            emailSender = new Mock<IEmailSender>().Object;
            encryptionService = new Mock<IEncryptionService>().Object;
            postageService = new Mock<IPostageService>().Object;

            orderController = new Mock<OrderController>(
                orderRepository,
                basketRepository,
                countryRepository,
                cardTypeRepository,
                encryptionService,
                emailSender,
                postageService).Object;

            testContext = new ControllerTestContext(orderController);

            Mock.Get(postageService).Expect(ps => ps.CalculatePostageFor(It.IsAny<Order>()));
            
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

            Mock.Get(orderController).Expect(c => c.CheckCurrentUserCanViewOrder(It.IsAny<Order>()));

            // exercise Checkout action
            orderController.Checkout(basketId)
                .ReturnsViewResult()
                .AssertNotNull<ShopViewData, Order>(vd => vd.Order)
                .AssertNotNull<ShopViewData, Contact>(vd => vd.Order.Contact)
                .AssertNotNull<ShopViewData, Contact>(vd => vd.Order.Contact1)
                .AssertNotNull<ShopViewData, Card>(vd => vd.Order.Card)
                .AssertAreSame<ShopViewData, Basket>(basket, vd => vd.Order.Basket)
                .AssertNotNull<ShopViewData, IEnumerable<Country>>(vd => vd.Countries)
                .AssertAreSame<ShopViewData, IEnumerable<CardType>>(cardTypes, vd => vd.CardTypes);

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

            Mock.Get(encryptionService).Expect(es => es.EncryptCard(It.IsAny<Card>())).Verifiable();

            Mock.Get(orderRepository).Expect(or => or.InsertOnSubmit(It.IsAny<Order>()))
                .Callback<Order>(o => { order = o; order.Basket = basket; })
                .Verifiable();
            Mock.Get(orderRepository).Expect(or => or.SubmitChanges()).Verifiable();

            Mock.Get(orderController).Expect(c => c.EmailOrder(It.IsAny<Order>())).Verifiable();

            // exercise PlaceOrder action
            var result = orderController.PlaceOrder() as RedirectToRouteResult;

            // Assertions
            Assert.IsNotNull(result, "result is not a RedirectToRouteResult");
            Assert.That(result.Values["action"], Is.EqualTo("Item"));
            Assert.That(result.Values["id"], Is.EqualTo(order.OrderId));

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
            Mock.Get(encryptionService).Verify();
            Mock.Get(orderController).Verify();
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
                .ReturnsViewResult()
                .ForView("Index")
                .AssertAreSame<ShopViewData, Order>(orders.First(), vd => vd.Orders.First());
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

            orderController.Dispatch(orderId);

            Assert.IsTrue(order.IsDispatched, "order has not been dispatched");
            Assert.AreEqual(DateTime.Now.ToShortDateString(), order.DispatchedDateAsString, 
                "DispatchedDateAsString is incorrect");
            Assert.AreEqual(4, order.UserId, "UserId is incorrect"); // set GetFullPath_ShouldReturnFullPage
        }

        [Test]
        public void UndoStatus_ShouldChangeOrderStatusToCreated()
        {
            int orderId = 44;
            Order order = new Order
            {
                OrderId = orderId,
                OrderStatusId = OrderStatus.DispatchedId,
                DispatchedDate = DateTime.Now,
                Basket = new Basket()
            };

            Mock.Get(orderRepository).Expect(or => or.GetById(orderId)).Returns(order);
            Mock.Get(orderRepository).Expect(or => or.SubmitChanges());

            orderController.UndoStatus(orderId);

            Assert.IsTrue(order.IsCreated, "order status has not been reset");
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
                .ReturnsViewResult()
                .ForView("Index")
                .AssertAreSame<ShopViewData, Order>(orders.ElementAt(1), vd => vd.Orders.First());
                
        }

        [Test]
        public void ShowCard_ShouldDecryptCardAndShowOrder()
        {
            int orderId = 10;
            string privateKey = "abcd";

            Order order = new Order
            {
                Card = new Card
                {
                    CardTypeId = 1,
                    Holder = "Jon Anderson",
                    IssueNumber = "",
                    StartMonth = 1,
                    StartYear = 2004,
                    ExpiryMonth = 3,
                    ExpiryYear = 2010
                },
                Basket = new Basket(),
            };
            order.Card.SetEncryptedNumber("asldfkjaslfjdslsdjkfjflkdjdlsakj");
            order.Card.SetEncryptedSecurityCode("asldkfjsadlfjdskjfdlkd");

            Mock.Get(orderRepository).Expect(or => or.GetById(orderId)).Returns(order);

            Mock.Get(encryptionService).ExpectSet(es => es.PrivateKey).Verifiable();
            Mock.Get(encryptionService).Expect(es => es.DecryptCard(It.IsAny<Card>())).Verifiable();

            Mock.Get(orderController).Expect(c => c.CheckCurrentUserCanViewOrder(It.IsAny<Order>()));

            orderController.ShowCard(orderId, privateKey)
                .ReturnsViewResult()
                .ForView("Item")
                .AssertAreEqual<ShopViewData, string>(order.Card.Number, vd => vd.Card.Number)
                .AssertAreEqual<ShopViewData, int>(order.Card.ExpiryYear, vd => vd.Card.ExpiryYear);

            Mock.Get(encryptionService).Verify();
        }

        [Test]
        public void Invoice_ShouldShowOrderInInvoiceView()
        {
            int orderId = 10;

            Order order = new Order();

            Mock.Get(orderRepository).Expect(or => or.GetById(orderId)).Returns(order);

            orderController.Invoice(orderId)
                .ReturnsViewResult()
                .ForView("Invoice")
                .AssertAreSame<ShopViewData, Order>(order, vd => vd.Order);
        }
    }
}
