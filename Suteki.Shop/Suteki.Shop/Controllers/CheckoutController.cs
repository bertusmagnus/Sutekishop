using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
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
		readonly IValidatingBinder validatingBinder;
		readonly IEmailSender emailSender;
		readonly IEncryptionService encryptionService;

		public CheckoutController(IRepository<Basket> basketRepository, IUserService userService,
		                          IPostageService postageService, IRepository<Country> countryRepository,
		                          IRepository<CardType> cardTypeRepository, IRepository<Order> orderRepository, 
			IValidatingBinder validatingBinder, IEmailSender emailSender, IEncryptionService encryptionService)
		{
			this.basketRepository = basketRepository;
			this.encryptionService = encryptionService;
			this.emailSender = emailSender;
			this.validatingBinder = validatingBinder;
			this.orderRepository = orderRepository;
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

		public ActionResult PlaceOrder(FormCollection form)
		{
			var order = new Order();
			try
			{
				UpdateOrderFromForm(order, form);
				orderRepository.InsertOnSubmit(order);
				userService.CurrentUser.CreateNewBasket();
				orderRepository.SubmitChanges();
				EmailOrder(order);

				return RedirectToRoute(new {Controller = "Order", Action = "Item", id = order.OrderId});
			}
			catch (ValidationException validationException)
			{
				var basket = basketRepository.GetById(order.BasketId);
				PopulateOrderForView(order, basket);
				return View("Checkout", CheckoutViewData(order).WithErrorMessage(validationException.Message));
			}
		}

		private void UpdateOrderFromForm(Order order, NameValueCollection form) 
		{
			order.OrderStatusId = OrderStatus.CreatedId;
			order.CreatedDate = DateTime.Now;
			order.DispatchedDate = DateTime.Now;

			var validator = new Validator
                                  {
                                      () => UpdateOrder(order, form),
                                      () => UpdateCardContact(order, form),
                                      () => UpdateDeliveryContact(order, form),
                                      () => UpdateCard(order, form)
                                  };

			validator.Validate();
		}


		private void UpdateCardContact(Order order, NameValueCollection form) {
			var cardContact = new Contact();
			order.Contact = cardContact;
			UpdateContact(cardContact, "cardcontact", form);
		}

		private void UpdateDeliveryContact(Order order, NameValueCollection form) {
			if (order.UseCardHolderContact) return;

			var deliveryContact = new Contact();
			order.Contact1 = deliveryContact;
			UpdateContact(deliveryContact, "deliverycontact", form);
		}

		private void UpdateContact(Contact contact, string prefix, NameValueCollection form) {
			try {
				validatingBinder.UpdateFrom(contact, form, prefix);
			}
			finally {
				if (contact.CountryId != 0 && contact.Country == null) {
					contact.Country = countryRepository.GetById(contact.CountryId);
				}
			}
		}

		private void UpdateCard(Order order, NameValueCollection form) {
			if (order.PayByTelephone) return;

			var card = new Card();
			order.Card = card;
			validatingBinder.UpdateFrom(card, form, "card");
			encryptionService.EncryptCard(card);
		}

		private void UpdateOrder(Order order, NameValueCollection form) {
			validatingBinder.UpdateFrom(order, form, "order");
			var confirmEmail = form["emailconfirm"];
			if (order.Email != confirmEmail) {
				throw new ValidationException("Email and Confirm Email do not match");
			}
		}

		[NonAction]
		public virtual void EmailOrder(Order order) {
			/*var subject = "{0}: your order".With(BaseControllerService.ShopName);
			var message = this.CaptureActionHtml(c => (ViewResult)c.Print(order.OrderId));
			var toAddresses = new[] { order.Email, BaseControllerService.EmailAddress };

			// send the message
			emailSender.Send(toAddresses, subject, message);*/
			throw new NotImplementedException();
		}

		public ActionResult UpdateCountry(int id, int countryId, FormCollection form) {
			var basket = basketRepository.GetById(id);
			basket.CountryId = countryId;
			basketRepository.SubmitChanges();

			var order = new Order();

			try {
				UpdateOrderFromForm(order, form);
			}
			catch (ValidationException) {
				// ignore validation exceptions
			}

			PopulateOrderForView(order, basket);
			return View("Checkout", CheckoutViewData(order));
		}

	}
}