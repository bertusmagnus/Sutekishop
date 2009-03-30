using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
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

		[SetUp]
		public void Setup()
		{
			basketRepository = MockRepository.GenerateStub<IRepository<Basket>>();

			userService = MockRepository.GenerateStub<IUserService>();
			postageService = MockRepository.GenerateStub<IPostageService>();
			countryRepository = MockRepository.GenerateStub<IRepository<Country>>();
			cardTypeRepository = MockRepository.GenerateStub<IRepository<CardType>>();
			controller = new CheckoutController(
				basketRepository,
				userService,
				postageService,
				countryRepository,
				cardTypeRepository
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

	}
}