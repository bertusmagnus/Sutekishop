using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Filters;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using System.Security.Permissions;
using Suteki.Common.Binders;

namespace Suteki.Shop.Controllers
{
    public class OrderController : ControllerBase
    {
        private readonly IRepository<Order> orderRepository;
        private readonly IRepository<Basket> basketRepository;
        private readonly IRepository<Country> countryRepository;
        private readonly IRepository<CardType> cardTypeRepository;

        private readonly IEncryptionService encryptionService;
        private readonly IEmailSender emailSender;
        private readonly IPostageService postageService;
        private readonly IValidatingBinder validatingBinder;
    	private readonly IUserService userService;
		readonly IOrderSearchService searchService;

        public OrderController(
			IRepository<Order> orderRepository, 
			IRepository<Basket> basketRepository, 
			IRepository<Country> countryRepository, 
			IRepository<CardType> cardTypeRepository, 
			IEncryptionService encryptionService, IEmailSender emailSender, 
			IPostageService postageService, 
			IValidatingBinder validatingBinder, 
			IUserService userService, 
			IOrderSearchService searchService)
        {
            this.orderRepository = orderRepository;
        	this.searchService = searchService;
        	this.userService = userService;
            this.basketRepository = basketRepository;
            this.countryRepository = countryRepository;
            this.cardTypeRepository = cardTypeRepository;
            this.encryptionService = encryptionService;
            this.emailSender = emailSender;
            this.postageService = postageService;
            this.validatingBinder = validatingBinder;
        }

        public OrderController(
            IRepository<Order> orderRepository, 
            IRepository<Basket> basketRepository, 
            IRepository<Country> countryRepository, 
            IRepository<CardType> cardTypeRepository, 
            IEncryptionService encryptionService, 
            IEmailSender emailSender, 
            IPostageService postageService, IUserService userService, IOrderSearchService searchService)
        {
            this.orderRepository = orderRepository;
        	this.searchService = searchService;
        	this.userService = userService;
            this.basketRepository = basketRepository;
            this.countryRepository = countryRepository;
            this.cardTypeRepository = cardTypeRepository;
            this.encryptionService = encryptionService;
            this.emailSender = emailSender;
            this.postageService = postageService;
        }

        [AcceptVerbs(HttpVerbs.Get), AdministratorsOnly]
        public ActionResult Index()
        {
            return Index(new OrderSearchCriteria());
        }

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index([DataBind(Fetch = false)] OrderSearchCriteria orderSearchCriteria)
        {
			var orders = searchService.PerformSearch(orderSearchCriteria);

            return View("Index", ShopView.Data
                .WithOrders(orders)
                .WithOrderSearchCriteria(orderSearchCriteria));
        }

        public ActionResult Item(int id)
        {
            return ItemView(id);
        }

        private ViewResult ItemView(int id)
        {
            var order = orderRepository.GetById(id);

            if (userService.CurrentUser.IsAdministrator)
            {
                var cookie = Request.Cookies["privateKey"];
                if (cookie != null)
                {
                    var privateKey = cookie.Value.Replace("%3D", "=");

                    if (!order.PayByTelephone)
                    {
                        var card = order.Card.Copy();
                        try
                        {
                            encryptionService.PrivateKey = privateKey;
                            encryptionService.DecryptCard(card);
                            return View("Item", CheckoutViewData(order).WithCard(card));
                        }
                        catch (ValidationException exception)
                        {
                            return View("Item", CheckoutViewData(order).WithErrorMessage(exception.Message));
                        }
                    }
                }
            }

			userService.CurrentUser.EnsureCanViewOrder(order);
            return View("Item", CheckoutViewData(order));
        }

        public ActionResult Print(int id)
        {
            var viewResult = ItemView(id);
            viewResult.MasterName = "Print";
            return viewResult;
        }



		[Suteki.Shop.Filters.AdministratorsOnly]
        public ActionResult ShowCard(int orderId, string privateKey)
        {
            var order = orderRepository.GetById(orderId);

            var card = order.Card.Copy();

            try
            {
                encryptionService.PrivateKey = privateKey;
                encryptionService.DecryptCard(card);
                return View("Item", CheckoutViewData(order).WithCard(card));
            }
            catch (ValidationException exception)
            {
                return View("Item", CheckoutViewData(order).WithErrorMessage(exception.Message));
            }
        }

        public ActionResult Checkout(int id)
        {
            // create a default order
            var order = new Order { UseCardHolderContact = true };

            var basket = basketRepository.GetById(id);
            PopulateOrderForView(order, basket);

            return View("Checkout", CheckoutViewData(order));
        }

        private static void PopulateOrderForView(Order order, Basket basket)
        {
            if (order.Basket == null) order.Basket = basket;
            if (order.Contact == null) order.Contact = new Contact();
            if (order.Contact1 == null) order.Contact1 = new Contact();
            if (order.Card == null) order.Card = new Card();
        }

        private ShopViewData CheckoutViewData(Order order)
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

                return RedirectToRoute(new { Controller = "Order", Action = "Item", id = order.OrderId });
            }
            catch (ValidationException validationException)
            {
                var basket = basketRepository.GetById(order.BasketId);
                PopulateOrderForView(order, basket);
                return View("Checkout", CheckoutViewData(order)
                    .WithErrorMessage(validationException.Message)
                    );
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

        [NonAction]
        public virtual void EmailOrder(Order order)
        {
            var subject = "{0}: your order".With(BaseControllerService.ShopName);
            var message = this.CaptureActionHtml(c => (ViewResult)c.Print(order.OrderId));
            var toAddresses = new[] { order.Email, BaseControllerService.EmailAddress };

            // send the message
            emailSender.Send(toAddresses, subject, message);
        }

        private void UpdateCardContact(Order order, NameValueCollection form)
        {
            var cardContact = new Contact();
            order.Contact = cardContact;
            UpdateContact(cardContact, "cardcontact", form);
        }

        private void UpdateDeliveryContact(Order order, NameValueCollection form)
        {
            if (order.UseCardHolderContact) return;

            var deliveryContact = new Contact();
            order.Contact1 = deliveryContact;
            UpdateContact(deliveryContact, "deliverycontact", form);
        }

        private void UpdateContact(Contact contact, string prefix, NameValueCollection form)
        {
            try
            {
                validatingBinder.UpdateFrom(contact, form, prefix);
            }
            finally
            {
                if (contact.CountryId != 0 && contact.Country == null)
                {
                    contact.Country = countryRepository.GetById(contact.CountryId);
                }
            }
        }

        private void UpdateCard(Order order, NameValueCollection form)
        {
            if (order.PayByTelephone) return;

            var card = new Card();
            order.Card = card;
            validatingBinder.UpdateFrom(card, form, "card");
            encryptionService.EncryptCard(card);
        }

        private void UpdateOrder(Order order, NameValueCollection form)
        {
            validatingBinder.UpdateFrom(order, form, "order");
            var confirmEmail = form["emailconfirm"];
            if (order.Email != confirmEmail)
            {
                throw new ValidationException("Email and Confirm Email do not match");
            }
        }

        public ActionResult UpdateCountry(int id, int countryId, FormCollection form)
        {
            var basket = basketRepository.GetById(id);
            basket.CountryId = countryId;
            basketRepository.SubmitChanges();

            var order = new Order();

            try
            {
                UpdateOrderFromForm(order, form);
            }
            catch (ValidationException)
            {
                // ignore validation exceptions
            }

            PopulateOrderForView(order, basket);
            return View("Checkout", CheckoutViewData(order));
        }
    }
}
