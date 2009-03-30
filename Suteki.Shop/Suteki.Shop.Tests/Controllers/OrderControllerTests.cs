using System;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.TestHelpers;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using System.Web.Mvc;
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
		IOrderSearchService searchService;

        private ControllerTestContext testContext;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the order controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            orderRepository = MockRepository.GenerateStub<IRepository<Order>>();
            basketRepository = MockRepository.GenerateStub<IRepository<Basket>>();
            countryRepository = MockRepository.GenerateStub<IRepository<Country>>();
            cardTypeRepository = MockRepository.GenerateStub<IRepository<CardType>>();
			

            emailSender = MockRepository.GenerateStub<IEmailSender>();
            encryptionService = MockRepository.GenerateStub<IEncryptionService>();
            postageService = MockRepository.GenerateStub<IPostageService>();
            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());
            httpContextService = MockRepository.GenerateStub<IHttpContextService>();
            userService = MockRepository.GenerateStub<IUserService>();
			searchService = MockRepository.GenerateStub<IOrderSearchService>();

            var mocks = new MockRepository();
            orderController = mocks.PartialMock<OrderController>(
                orderRepository,
                basketRepository,
                countryRepository,
                cardTypeRepository,
                encryptionService,
                emailSender,
                postageService,
                validatingBinder,
                userService,
				searchService
				);

            testContext = new ControllerTestContext(orderController);

            postageService.Expect(ps => ps.CalculatePostageFor(Arg<Order>.Is.Anything));

            userService.Expect(us => us.CurrentUser).Return(new User { UserId = 4, RoleId = Role.AdministratorId });

            testContext.TestContext.Context.User = new User { UserId = 4 };
            testContext.TestContext.Request.RequestType = "GET";
            testContext.TestContext.Request.Stub(r => r.QueryString).Return(new NameValueCollection());
            testContext.TestContext.Request.Stub(r => r.Form).Return(new NameValueCollection());

            mocks.ReplayAll();
        }

        [Test]
        public void PlaceOrder_ShouldCreateANewOrder()
        {
            // mock the request form
            var form = BuildPlaceOrderRequest();
            testContext.TestContext.Request.Stub(r => r.Form).Return(form);

            // expectations
            var basket = new Basket();
            var order = new Order();

            orderController.Expect(c => c.EmailOrder(Arg<Order>.Is.Anything));

            orderRepository.Expect(or => or.InsertOnSubmit(null))
                .IgnoreArguments()
                .WhenCalled(invocation => { order = invocation.Arguments[0] as Order; order.Basket = basket; });

            // exercise PlaceOrder action
            var result = orderController.PlaceOrder(form) as RedirectToRouteResult;

            // Assertions
            Assert.IsNotNull(result, "result is not a RedirectToRouteResult");
            Assert.That(result.RouteValues["action"], Is.EqualTo("Item"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(order.OrderId));

            // Order
            Assert.AreEqual(10, order.OrderId, "OrderId is incorrect");
            Assert.AreEqual(form["order.email"], order.Email, "Email is incorrect");
            Assert.AreEqual(form["order.additionalinformation"], order.AdditionalInformation, "AdditionalInformation is incorrect");
            Assert.IsFalse(order.UseCardHolderContact, "UseCardHolderContact is incorrect");
            Assert.IsFalse(order.PayByTelephone, "PayByTelephone is incorrect");

            Assert.AreEqual(1, order.OrderStatusId, "OrderStatusId is incorrect");
            Assert.AreEqual(DateTime.Now.ToShortDateString(), order.CreatedDate.ToShortDateString(), "CreatedDate is incorrect");

            // Card Contact
            var cardContact = order.Contact;
            AssertContactIsCorrect(form, cardContact, "cardcontact");

            // Delivery Contact
            var deliveryContact = order.Contact1;
            AssertContactIsCorrect(form, deliveryContact, "deliverycontact");

            // Card
            var card = order.Card;
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

            encryptionService.AssertWasCalled(es => es.EncryptCard(Arg<Card>.Is.Anything));
            orderRepository.AssertWasCalled(or => or.SubmitChanges());
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

        private static FormCollection BuildPlaceOrderRequest()
        {
            var form = new FormCollection
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
            var orders = new PagedList<Order>(new List<Order>(), 1, 1);
			searchService.Expect(x => x.PerformSearch(null)).IgnoreArguments().Return(orders);

            orderController.Index(null)
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>()
                .AssertAreSame(orders, vd => vd.Orders);
        }



        [Test]
        public void Index_ShouldBuildCriteriaAndExecuteSearch()
        {
			var criteria = new OrderSearchCriteria();
        	var results = new PagedList<Order>(new List<Order>(), 1, 1);
			searchService.Expect(x => x.PerformSearch(criteria)).Return(results);

			orderController.Index(criteria)
				.ReturnsViewResult()
				.ForView("Index")
				.WithModel<ShopViewData>()
				.AssertAreSame(criteria, vd => vd.OrderSearchCriteria)
				.AssertAreSame(results, vd => vd.Orders);

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

            orderRepository.Stub(or => or.GetById(orderId)).Return(order);

            orderController.ShowCard(orderId, privateKey)
                .ReturnsViewResult()
                .ForView("Item")
                .WithModel<ShopViewData>()
                .AssertAreEqual(order.Card.Number, vd => vd.Card.Number)
                .AssertAreEqual(order.Card.ExpiryYear, vd => vd.Card.ExpiryYear);

            encryptionService.AssertWasCalled(es => es.DecryptCard(Arg<Card>.Is.Anything));
        }
    }
}
