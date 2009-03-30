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
		IUnitOfWorkManager unitOfWorkManager;

		[SetUp]
		public void Setup()
		{
			basketRepository = MockRepository.GenerateStub<IRepository<Basket>>();
			unitOfWorkManager = MockRepository.GenerateStub<IUnitOfWorkManager>();

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
				encryptionService,
				unitOfWorkManager
			);

			userService.Expect(us => us.CurrentUser).Return(new User { UserId = 4, RoleId = Role.AdministratorId });
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
			var order = new Order() { OrderId = 4 };

			controller.PlaceOrder(order)
				.ReturnsRedirectToRouteResult()
				.ToController("Order")
				.ToAction("Item")
				.WithRouteValue("id", "4");

			orderRepository.AssertWasCalled(x => x.InsertOnSubmit(order));
			unitOfWorkManager.AssertWasCalled(x => x.Commit());
			encryptionService.AssertWasCalled(es => es.EncryptCard(Arg<Card>.Is.Anything));
		}

		[Test]
		public void PlaceOrder_ShouldRedirectOnError()
		{
			controller.ModelState.AddModelError("foo", "bar");
			var order = new Order() { BasketId = 6 };
			controller.PlaceOrder(order)
				.ReturnsRedirectToRouteResult()
				.ToController("Checkout")
				.ToAction("Index")
				.WithRouteValue("id", "6");
		}
	}
}