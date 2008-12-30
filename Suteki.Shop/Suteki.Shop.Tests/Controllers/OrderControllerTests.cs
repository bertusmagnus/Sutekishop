using System;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
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
        private OrderController orderController;

        private IRepository<Order> orderRepository;
        private IRepository<Basket> basketRepository;
        private IRepository<Country> countryRepository;
        private IRepository<CardType> cardTypeRepository;

        private IEncryptionService encryptionService;
        private IEmailSender emailSender;
        private IPostageService postageService;
        private IValidatingBinder validatingBinder;
        private IHttpContextService httpContextService;
        private IUserService userService;

        private ControllerTestContext testContext;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the order controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            orderRepository = new Mock<IRepository<Order>>().Object;
            basketRepository = new Mock<IRepository<Basket>>().Object;
            countryRepository = new Mock<IRepository<Country>>().Object;
            cardTypeRepository = new Mock<IRepository<CardType>>().Object;

            emailSender = new Mock<IEmailSender>().Object;
            encryptionService = new Mock<IEncryptionService>().Object;
            postageService = new Mock<IPostageService>().Object;
            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());
            httpContextService = new Mock<IHttpContextService>().Object;
            userService = new Mock<IUserService>().Object;

            orderController = new Mock<OrderController>(
                orderRepository,
                basketRepository,
                countryRepository,
                cardTypeRepository,
                encryptionService,
                emailSender,
                postageService,
                validatingBinder,
                httpContextService,
                userService).Object;

            testContext = new ControllerTestContext(orderController);

            Mock.Get(postageService).Expect(ps => ps.CalculatePostageFor(It.IsAny<Order>()));

            Mock.Get(userService).Expect(us => us.CurrentUser).Returns(new User {UserId = 4});

            testContext.TestContext.ContextMock.ExpectGet(h => h.User).Returns(new User { UserId = 4 });
            testContext.TestContext.RequestMock.ExpectGet(r => r.RequestType).Returns("GET");
            testContext.TestContext.RequestMock.ExpectGet(r => r.QueryString).Returns(new NameValueCollection());
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(new NameValueCollection());
        }

        [Test]
        public void Checkout_ShouldDisplayCheckoutForm()
        {
            const int basketId = 6;

            var basket = new Basket { BasketId = basketId };
            var countries = new List<Country> { new Country() }.AsQueryable();
            var cardTypes = new List<CardType> { new CardType() }.AsQueryable();

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
            var form = BuildPlaceOrderRequest();
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);

            // expectations
            var basket = new Basket();
            var order = new Order();

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
            Assert.AreEqual(form["card.issuenumber"], card.IssueNumber);
            Assert.AreEqual(form["card.securitycode"], card.SecurityCode);

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
            var form = new NameValueCollection
            {
                {"order.orderid", "10"},
                {"order.basketid", "22"},
                {"cardcontact.firstname", "Mike"},
                {"cardcontact.lastname", "Hadlow"},
                {"cardcontact.address1", "23 The Street"},
                {"cardcontact.address2", "The Manor"},
                {"cardcontact.address3", ""},
                {"cardcontact.town", "Hove"},
                {"cardcontact.county", "East Sussex"},
                {"cardcontact.postcode", "BN6 2EE"},
                {"cardcontact.countryid", "1"},
                {"cardcontact.telephone", "01273 234234"},
                {"order.email", "mike@mike.com"},
                {"emailconfirm", "mike@mike.com"},
                {"order.usecardholdercontact", "False"},
                {"deliverycontact.firstname", "Mike"},
                {"deliverycontact.lastname", "Hadlow"},
                {"deliverycontact.address1", "23 The Street"},
                {"deliverycontact.address2", "The Manor"},
                {"deliverycontact.address3", ""},
                {"deliverycontact.town", "Hove"},
                {"deliverycontact.county", "East Sussex"},
                {"deliverycontact.postcode", "BN6 2EE"},
                {"deliverycontact.countryid", "1"},
                {"deliverycontact.telephone", "01273 234234"},
                {"order.additionalinformation", "some more info"},
                {"card.cardtypeid", "1"},
                {"card.holder", "MR M HADLOW"},
                {"card.number", "1111111111111117"},
                {"card.expirymonth", "3"},
                {"card.expiryyear", "2009"},
                {"card.startmonth", "2"},
                {"card.startyear", "2003"},
                {"card.issuenumber", "3"},
                {"card.securitycode", "235"},
                {"order.paybytelephone", "False"}
            };

            return form;
        }

        [Test]
        public void Index_ShouldDisplayAListOfOrders()
        {
            var orders = new List<Order> { new Order() }.AsQueryable();
            Mock.Get(orderRepository).Expect(or => or.GetAll()).Returns(orders).Verifiable();

            orderController.Index(new FormCollection())
                .ReturnsViewResult()
                .ForView("Index")
                .AssertAreSame<ShopViewData, Order>(orders.First(), vd => vd.Orders.First());
        }

        [Test]
        public void Dispatch_ShouldChangeOrderStatusAndDispatchedDate()
        {
            const int orderId = 44;
            var order = new Order 
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
            const int orderId = 44;
            var order = new Order
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
            var form = new NameValueCollection {{"orderid", "3"}};
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(form);

            var orders = new List<Order>
            {
                new Order { OrderId = 2 },
                new Order { OrderId = 3 }
            }.AsQueryable();

            Mock.Get(orderRepository).Expect(or => or.GetAll()).Returns(orders);

            orderController.Index(new FormCollection())
                .ReturnsViewResult()
                .ForView("Index")
                .AssertAreSame<ShopViewData, Order>(orders.ElementAt(0), vd => vd.Orders.First());
                
        }

        [Test]
        public void ShowCard_ShouldDecryptCardAndShowOrder()
        {
            const int orderId = 10;
            const string privateKey = "abcd";

            var order = new Order
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
            const int orderId = 10;

            var order = new Order();

            Mock.Get(orderRepository).Expect(or => or.GetById(orderId)).Returns(order);

            orderController.Invoice(orderId)
                .ReturnsViewResult()
                .ForView("Invoice")
                .AssertAreSame<ShopViewData, Order>(order, vd => vd.Order);
        }
    }
}
