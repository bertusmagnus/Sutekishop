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
			var mocks = new MockRepository(); //TODO: No need to partial mock once email sending is fixed
			controller = mocks.PartialMock<CheckoutController>(//new CheckoutController(
				basketRepository,
				userService,
				postageService,
				countryRepository,
				cardTypeRepository,
				orderRepository,
				emailSender,
				unitOfWorkManager
			);
			mocks.ReplayAll();
			userService.Expect(us => us.CurrentUser).Return(new User { UserId = 4, RoleId = Role.AdministratorId });
		}

		[Test]
		public void Index_ShouldDisplayCheckoutForm() {
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
		public void Index_should_load_order_from_tempdata()
		{
			var order = new Order();
			controller.TempData["order"] = order;

			controller.Index(4)
				.ReturnsViewResult()
				.WithModel<ShopViewData>()
				.AssertAreSame(order, vd => vd.Order);
		}

		[Test]
		public void IndexWithPost_ShouldCreateANewOrder() {
			var order = new Order() { OrderId = 4 };

			controller.Expect(x => x.EmailOrder(order));

			controller.Index(order)
				.ReturnsRedirectToRouteResult()
				.ToController("Order")
				.ToAction("Item")
				.WithRouteValue("id", "4");

			controller.AssertWasCalled(x => x.EmailOrder(order));
			orderRepository.AssertWasCalled(x => x.InsertOnSubmit(order));
			unitOfWorkManager.AssertWasCalled(x => x.Commit());
		}

		[Test]
		public void IndexWithPost_ShouldRenderViewOnError()
		{
			controller.ModelState.AddModelError("foo", "bar");
			var order = new Order() { BasketId = 6 };
			controller.Index(order)
				.ReturnsViewResult()
				.WithModel<ShopViewData>()
				.AssertAreEqual(order, x => x.Order);
		}

		[Test]
		public void UpdateCountry_should_update_country()
		{
			var basket = new Basket();
			var order = new Order();
			basketRepository.Expect(x => x.GetById(4)).Return(basket);

			controller.UpdateCountry(4, 5, order)
				.ReturnsRedirectToRouteResult()
				.ToController("Checkout")
				.ToAction("Index")
				.WithRouteValue("id", "4");

			basket.CountryId.ShouldEqual(5);
			controller.TempData["order"].ShouldBeTheSameAs(order);
		}

		[Test]
		public void UpdateCountry_Should_clear_modelstate_errors()
		{
			basketRepository.Expect(x => x.GetById(4)).Return(new Basket());
			controller.ModelState.AddModelError("foo", "bar");
			controller.UpdateCountry(4, 5, new Order());

			controller.ModelState.Count.ShouldEqual(0);
		}
	}
}