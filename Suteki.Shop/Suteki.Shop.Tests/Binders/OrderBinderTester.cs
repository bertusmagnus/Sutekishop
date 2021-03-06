using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Binders;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Binders
{
	[TestFixture]
	public class OrderBinderTester
	{
		OrderBinder binder;
		ControllerContext context;
		IEncryptionService encryptionService;
		IRepository<Basket> basketRepository;

		[SetUp]
		public void Setup()
		{
			encryptionService = MockRepository.GenerateStub<IEncryptionService>();
			basketRepository = MockRepository.GenerateStub<IRepository<Basket>>();
			binder = new OrderBinder(
				new ValidatingBinder(new SimplePropertyBinder()), 
				encryptionService,
				basketRepository
			);

			context = new ControllerContext()
			{
				HttpContext = MockRepository.GenerateStub<HttpContextBase>()
			};
			context.HttpContext.Expect(x => x.Request).Return(MockRepository.GenerateStub<HttpRequestBase>());
		}

		[Test]
		public void Should_Create_order() {
			// mock the request form
			basketRepository.Expect(x => x.GetById(22)).Return(new Basket());

			var form = BuildPlaceOrderRequest(true);
			context.HttpContext.Request.Expect(x => x.Form).Return(form);

			var order = (Order)binder.BindModel(context, new ModelBindingContext());

			// Order
			Assert.AreEqual(10, order.OrderId, "OrderId is incorrect");
			Assert.AreEqual(form["order.email"], order.Email, "Email is incorrect");
			Assert.AreEqual(form["order.additionalinformation"], order.AdditionalInformation, "AdditionalInformation is incorrect");
			Assert.IsFalse(order.UseCardHolderContact, "UseCardHolderContact is incorrect");
			Assert.IsFalse(order.PayByTelephone, "PayByTelephone is incorrect");

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
		}

		[Test]
		public void Updates_country()
		{
			var basket = new Basket();
			basketRepository.Expect(x => x.GetById(22)).Return(basket);

			var form = BuildPlaceOrderRequest(true);
			context.HttpContext.Request.Expect(x => x.Form).Return(form);

			var order = (Order)binder.BindModel(context, new ModelBindingContext());

			basket.CountryId.ShouldEqual(1);
		}


		[Test]
		public void Updates_country_when_there_is_no_delivery_contact()
		{
			var basket = new Basket();
			basketRepository.Expect(x => x.GetById(22)).Return(basket);

			var form = BuildPlaceOrderRequest(false);
			context.HttpContext.Request.Expect(x => x.Form).Return(form);

			var order = (Order)binder.BindModel(context, new ModelBindingContext());

			basket.CountryId.ShouldEqual(1);
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

		private static FormCollection BuildPlaceOrderRequest(bool includeDeliveryContact) {
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
                {"order.usecardholdercontact", (!includeDeliveryContact).ToString()},
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

			if(includeDeliveryContact)
			{
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
			}

			return form;
		}
	}
}