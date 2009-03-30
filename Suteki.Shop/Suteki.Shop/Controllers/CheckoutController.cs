using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Controllers
{
	public class CheckoutController : ControllerBase
	{
		readonly IRepository<Basket> basketRepository;
		readonly IUserService userService;
		readonly IPostageService postageService;
		readonly IRepository<Country> countryRepository;
		readonly IRepository<CardType> cardTypeRepository;

		public CheckoutController(IRepository<Basket> basketRepository, IUserService userService, IPostageService postageService, IRepository<Country> countryRepository, IRepository<CardType> cardTypeRepository)
		{
			this.basketRepository = basketRepository;
			this.cardTypeRepository = cardTypeRepository;
			this.countryRepository = countryRepository;
			this.postageService = postageService;
			this.userService = userService;
		}

		public ActionResult Index(int id)
		{
			// create a default order
			var order = new Order {UseCardHolderContact = true};

			var basket = basketRepository.GetById(id);
			PopulateOrderForView(order, basket);

			return View(CheckoutViewData(order));
		}

		static void PopulateOrderForView(Order order, Basket basket)
		{
			if (order.Basket == null) order.Basket = basket;
			if (order.Contact == null) order.Contact = new Contact();
			if (order.Contact1 == null) order.Contact1 = new Contact();
			if (order.Card == null) order.Card = new Card();
		}

		ShopViewData CheckoutViewData(Order order)
		{
			userService.CurrentUser.EnsureCanViewOrder(order);
			postageService.CalculatePostageFor(order);

			return ShopView.Data
				.WithCountries(countryRepository.GetAll().Active().InOrder())
				.WithCardTypes(cardTypeRepository.GetAll())
				.WithOrder(order);
		}
	}
}