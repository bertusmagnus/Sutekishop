using System;
using MvcContrib;
using System.Collections.Specialized;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Extensions;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Binders;
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
		readonly IRepository<Order> orderRepository;
		readonly IEmailSender emailSender;
		readonly IUnitOfWorkManager unitOfWork;

		public CheckoutController(
			IRepository<Basket> basketRepository, 
			IUserService userService, 
			IPostageService postageService, 
			IRepository<Country> countryRepository, 
			IRepository<CardType> cardTypeRepository, 
			IRepository<Order> orderRepository, 
			IEmailSender emailSender, 
			IUnitOfWorkManager unitOfWork)
		{
			this.basketRepository = basketRepository;
			this.unitOfWork = unitOfWork;
			this.emailSender = emailSender;
			this.orderRepository = orderRepository;
			this.cardTypeRepository = cardTypeRepository;
			this.countryRepository = countryRepository;
			this.postageService = postageService;
			this.userService = userService;
		}

		public ActionResult Index(int id)
		{
			// create a default order
			var order = CurrentOrder ?? new Order {UseCardHolderContact = true};

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

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Index([BindUsing(typeof(OrderBinder))] Order order)
		{

			if (ModelState.IsValid)
			{
				orderRepository.InsertOnSubmit(order);
				userService.CurrentUser.CreateNewBasket();

				//we need an explicit Commit in order to obtain the db-generated Order Id
				unitOfWork.Commit();

				EmailOrder(order);
				
				
				return this.RedirectToAction<OrderController>(c => c.Item(order.OrderId));
			}

			var basket = basketRepository.GetById(order.BasketId);
			PopulateOrderForView(order, basket);

			return View(CheckoutViewData(order));
		}

		[NonAction]
		public virtual void EmailOrder(Order order)
		{
			//TODO: This needs cleaning up. 
			ViewData["Title"] = "Order Confirmation";
			PopulateOrderForView(order, order.Basket);
			var result = View("~/Views/Order/Item.aspx", "Print", CheckoutViewData(order));

			var subject = "{0}: your order".With(BaseControllerService.ShopName);
			var message = this.CaptureActionHtml(c => result);
			var toAddresses = new[] { order.Email, BaseControllerService.EmailAddress };

			// send the message
			emailSender.Send(toAddresses, subject, message);
		}

		[UnitOfWork]
		public ActionResult UpdateCountry(int id, int countryId, [BindUsing(typeof(OrderBinder))] Order order)
		{
			//Ignore any errors - if there are any errors in modelstate then the UnitOfWork will not commit.
			ModelState.Clear(); 

			var basket = basketRepository.GetById(id);
			basket.CountryId = countryId;
			CurrentOrder = order;
			return this.RedirectToAction(c => c.Index(id));
		}

		private Order CurrentOrder
		{
			get { return TempData["order"] as Order; }
			set { TempData["order"] = value; }
		}
	}
}