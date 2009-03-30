using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.TestHelpers;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;
using System.Collections.Generic;

namespace Suteki.Shop.Tests.Controllers
{
	[TestFixture]
	public class CheckoutControllerTester
	{
		CheckoutController controller;
		IRepository<Basket> basketRepository;
		IUserService userService;
		IPostageService postageService;
		IRepository<Country> countryRepository;
		IRepository<CardType> cardTypeRepository;
		IRepository<Order> orderRepository;
		IEmailSender emailSender;
		IValidatingBinder validatingBinder;
		IEncryptionService encryptionService;
		ControllerTestContext testContext;

		[SetUp]
		public void Setup()
		{
			basketRepository = MockRepository.GenerateStub<IRepository<Basket>>();

			userService = MockRepository.GenerateStub<IUserService>();
			postageService = MockRepository.GenerateStub<IPostageService>();
			countryRepository = MockRepository.GenerateStub<IRepository<Country>>();
			cardTypeRepository = MockRepository.GenerateStub<IRepository<CardType>>();
			orderRepository = MockRepository.GenerateStub<IRepository<Order>>();
			emailSender = MockRepository.GenerateStub<IEmailSender>();
			validatingBinder = MockRepository.GenerateStub<IValidatingBinder>();
			encryptionService = MockRepository.GenerateStub<IEncryptionService>();
			controller = new CheckoutController(
				basketRepository,
				userService,
				postageService,
				countryRepository,
				cardTypeRepository,
				orderRepository,
				validatingBinder,
				emailSender,
				encryptionService
			);

			userService.Expect(us => us.CurrentUser).Return(new User { UserId = 4, RoleId = Role.AdministratorId });
			testContext = new ControllerTestContext(controller);
		}

		[Test]
		public void Checkout_ShouldDisplayCheckoutForm() {
			const int basketId = 6;

			var basket = new Basket { BasketId = basketId };
			var countries = new List<Country> { new Country() }.AsQueryable();
			var cardTypes = new List<CardType> { new CardType() }.AsQueryable();

			// stubs
			basketRepository.Stub(br => br.GetById(basketId)).Return(basket);
			countryRepository.Stub(cr => cr.GetAll()).Return(countries);
			cardTypeRepository.Stub(ctr => ctr.GetAll()).Return(cardTypes);

			// exercise Checkout action
			controller.Index(basketId)
				.ReturnsViewResult()
				.WithModel<ShopViewData>()
				.AssertNotNull(vd => vd.Order)
				.AssertNotNull(vd => vd.Order.Contact)
				.AssertNotNull(vd => vd.Order.Contact1)
				.AssertNotNull(vd => vd.Order.Card)
				.AssertAreSame(basket, vd => vd.Order.Basket)
				.AssertNotNull(vd => vd.Countries)
				.AssertAreSame(cardTypes, vd => vd.CardTypes);
		}

		[Test]
		public void PlaceOrder_ShouldCreateANewOrder() {
			// mock the request form
			var form = BuildPlaceOrderRequest();
			testContext.TestContext.Request.Stub(r => r.Form).Return(form);

			// expectations
			var basket = new Basket();
			var order = new Order();

			controller.Expect(c => c.EmailOrder(Arg<Order>.Is.Anything));

			orderRepository.Expect(or => or.InsertOnSubmit(null))
				.IgnoreArguments()
				.WhenCalled(invocation => { order = invocation.Arguments[0] as Order; order.Basket = basket; });

			// exercise PlaceOrder action
			var result = controller.PlaceOrder(form) as RedirectToRouteResult;

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


		private static void AssertContactIsCorrect(NameValueCollection form, Contact contact, string prefix) {
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

		private static FormCollection BuildPlaceOrderRequest() {
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
	}
}