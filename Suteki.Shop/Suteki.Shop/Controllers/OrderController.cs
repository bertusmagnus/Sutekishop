using System;
using System.Web;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
    public class OrderController : ControllerBase
    {
        readonly IRepository<Order> orderRepository;
        readonly IRepository<Basket> basketRepository;
        readonly IRepository<Country> countryRepository;
        readonly IRepository<CardType> cardTypeRepository;

        readonly IEncryptionService encryptionService;
        readonly IEmailSender emailSender;
        readonly IPostageService postageService;

        public OrderController(
            IRepository<Order> orderRepository,
            IRepository<Basket> basketRepository,
            IRepository<Country> countryRepository,
            IRepository<CardType> cardTypeRepository,
            IEncryptionService encryptionService,
            IEmailSender emailSender)
        {
            this.orderRepository = orderRepository;
            this.basketRepository = basketRepository;
            this.countryRepository = countryRepository;
            this.cardTypeRepository = cardTypeRepository;
            this.encryptionService = encryptionService;
            this.emailSender = emailSender;
        }

        public OrderController(
            IRepository<Order> orderRepository, 
            IRepository<Basket> basketRepository, 
            IRepository<Country> countryRepository, 
            IRepository<CardType> cardTypeRepository, 
            IEncryptionService encryptionService, 
            IEmailSender emailSender, 
            IPostageService postageService)
        {
            this.orderRepository = orderRepository;
            this.basketRepository = basketRepository;
            this.countryRepository = countryRepository;
            this.cardTypeRepository = cardTypeRepository;
            this.encryptionService = encryptionService;
            this.emailSender = emailSender;
            this.postageService = postageService;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Index()
        {
            var criteria = new OrderSearchCriteria();

            try
            {
                ValidatingBinder.UpdateFrom(criteria, Request.Form);
            }
            catch (ValidationException) { } // ignore validation exceptions

            var orders = orderRepository
                .GetAll()
                .ThatMatch(criteria)
                .ByCreatedDate()
                .ToPagedList(Request.PageNumber(), 20);

            return View("Index", ShopView.Data
                .WithOrders(orders)
                .WithOrderSearchCriteria(criteria));
        }

        public ActionResult Item(int id)
        {
            return ItemView(id);
        }

        private ViewResult ItemView(int id)
        {
            Order order = orderRepository.GetById(id);

            if (CurrentUser.IsAdministrator)
            {
                var cookie = Request.Cookies["privateKey"];
                if (cookie != null)
                {
                    string privateKey = cookie.Value.Replace("%3D", "=");

                    if (!order.PayByTelephone)
                    {
                        Card card = order.Card.Copy();
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

            CheckCurrentUserCanViewOrder(order);
            return View("Item", CheckoutViewData(order));
        }

        [NonAction]
        public virtual void CheckCurrentUserCanViewOrder(Order order)
        {
            if (!CurrentUser.IsAdministrator)
            {
                if (order.Basket.UserId != CurrentUser.UserId)
                {
                    throw new ApplicationException("You are attempting to view an order that was not created by you");
                }
            }            
        }

        public ActionResult Print(int id)
        {
            ViewResult viewResult = ItemView(id);
            viewResult.MasterName = "Print";
            return viewResult;
        }

        

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult ShowCard(int orderId, string privateKey)
        {
            Order order = orderRepository.GetById(orderId);

            Card card = order.Card.Copy();

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
            Order order = new Order { UseCardHolderContact = true };

            Basket basket = basketRepository.GetById(id);
            PopulateOrderForView(order, basket);

            return View("Checkout", CheckoutViewData(order));
        }

        private void PopulateOrderForView(Order order, Basket basket)
        {
            if (order.Basket == null) order.Basket = basket;
            if (order.Contact == null) order.Contact = new Contact();
            if (order.Contact1 == null) order.Contact1 = new Contact();
            if (order.Card == null) order.Card = new Card();
        }

        private ShopViewData CheckoutViewData(Order order)
        {
            CheckCurrentUserCanViewOrder(order);

            postageService.CalculatePostageFor(order);

            return ShopView.Data
                .WithCountries(countryRepository.GetAll().Active().InOrder())
                .WithCardTypes(cardTypeRepository.GetAll())
                .WithOrder(order);
        }

        public ActionResult PlaceOrder()
        {
            Order order = new Order();
            try
            {
                UpdateOrderFromForm(order);
                orderRepository.InsertOnSubmit(order);
                CurrentUser.CreateNewBasket();
                orderRepository.SubmitChanges();
                EmailOrder(order);

                return RedirectToRoute(new { Controller = "Order", Action = "Item", id = order.OrderId });
            }
            catch (ValidationException validationException)
            {
                Basket basket = basketRepository.GetById(order.BasketId);
                PopulateOrderForView(order, basket);
                return View("Checkout", CheckoutViewData(order)
                    .WithErrorMessage(validationException.Message)
                    );
            }
        }

        private void UpdateOrderFromForm(Order order)
        {
            order.OrderStatusId = OrderStatus.CreatedId;
            order.CreatedDate = DateTime.Now;
            order.DispatchedDate = DateTime.Now;

            Validator validator = new Validator
                                  {
                                      () => UpdateOrder(order),
                                      () => UpdateCardContact(order),
                                      () => UpdateDeliveryContact(order),
                                      () => UpdateCard(order)
                                  };

            validator.Validate();
        }

        [NonAction]
        public virtual void EmailOrder(Order order)
        {
            string subject = "{0}: your order".With(this.BaseControllerService.ShopName);

            string message = this.CaptureActionHtml(c => (ViewResult)c.Print(order.OrderId));

            var toAddresses = new[] { order.Email, BaseControllerService.EmailAddress };

            // send the message
            emailSender.Send(toAddresses, subject, message);
        }

        private void UpdateCardContact(Order order)
        {
            Contact cardContact = new Contact();
            order.Contact = cardContact;
            UpdateContact(cardContact, "cardcontact");
        }

        private void UpdateDeliveryContact(Order order)
        {
            if (order.UseCardHolderContact) return;

            Contact deliveryContact = new Contact();
            order.Contact1 = deliveryContact;
            UpdateContact(deliveryContact, "deliverycontact");
        }

        private void UpdateContact(Contact contact, string prefix)
        {
            try
            {
                ValidatingBinder.UpdateFrom(contact, Request.Form, prefix);
            }
            finally
            {
                if (contact.CountryId != 0 && contact.Country == null)
                {
                    contact.Country = countryRepository.GetById(contact.CountryId);
                }
            }
        }

        private void UpdateCard(Order order)
        {
            if (order.PayByTelephone) return;

            Card card = new Card();
            order.Card = card;
            ValidatingBinder.UpdateFrom(card, Request.Form, "card");
            encryptionService.EncryptCard(card);
        }

        private void UpdateOrder(Order order)
        {
            ValidatingBinder.UpdateFrom(order, Request.Form, "order");
            string confirmEmail = this.ReadFromRequest("emailconfirm");
            if (order.Email != confirmEmail)
            {
                throw new ValidationException("Email and Confirm Email do not match");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Dispatch(int id)
        {
            Order order = orderRepository.GetById(id);

            if (order.IsCreated)
            {
                order.OrderStatusId = OrderStatus.DispatchedId;
                order.DispatchedDate = DateTime.Now;
                order.UserId = CurrentUser.UserId;
                orderRepository.SubmitChanges();
            }

            return RedirectToRoute(new { Controller = "Order", Action = "Item", id = order.OrderId });
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Reject(int id)
        {
            Order order = orderRepository.GetById(id);

            if (order.IsCreated)
            {
                order.OrderStatusId = OrderStatus.RejectedId;
                order.UserId = CurrentUser.UserId;
                orderRepository.SubmitChanges();
            }

            return RedirectToRoute(new { Controller = "Order", Action = "Item", id = order.OrderId });
        }

        public ActionResult UpdateCountry(int id, int countryId)
        {
            Basket basket = basketRepository.GetById(id);
            basket.CountryId = countryId;
            basketRepository.SubmitChanges();

            Order order = new Order();

            try
            {
                UpdateOrderFromForm(order);
            }
            catch (ValidationException)
            {
                // ignore validation exceptions
            }

            PopulateOrderForView(order, basket);
            return View("Checkout", CheckoutViewData(order));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]        
        public ActionResult Invoice(int id)
        {
            Order order = orderRepository.GetById(id);
            postageService.CalculatePostageFor(order);

            AppendTitle("Invoice {0}".With(order.OrderId));

            return View("Invoice", ShopView.Data.WithOrder(order));
        }
    }
}
