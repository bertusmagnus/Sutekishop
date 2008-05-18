using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;
using Suteki.Shop.Extensions;
using Suteki.Shop.Services;
using MvcContrib;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
    public class OrderController : ControllerBase
    {
        IRepository<Order> orderRepository;
        IRepository<Basket> basketRepository;
        IRepository<Country> countryRepository;
        IRepository<CardType> cardTypeRepository;
        IRepository<Postage> postageRepository;

        IEncryptionService encryptionService;

        public OrderController(
            IRepository<Order> orderRepository,
            IRepository<Basket> basketRepository,
            IRepository<Country> countryRepository,
            IRepository<CardType> cardTypeRepository,
            IRepository<Postage> postageRepository,
            IEncryptionService encryptionService)
        {
            this.orderRepository = orderRepository;
            this.basketRepository = basketRepository;
            this.countryRepository = countryRepository;
            this.cardTypeRepository = cardTypeRepository;
            this.postageRepository = postageRepository;
            this.encryptionService = encryptionService;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Index()
        {
            OrderSearchCriteria criteria = new OrderSearchCriteria();

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

            return RenderView("Index", ShopView.Data.WithOrders(orders));
        }

        public ActionResult Item(int id)
        {
            Order order = orderRepository.GetById(id);
            return RenderView("Item", CheckoutViewData(order));
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
                return RenderView("Item", CheckoutViewData(order).WithCard(card));
            }
            catch (ValidationException exception)
            {
                return RenderView("Item", CheckoutViewData(order).WithErrorMessage(exception.Message));
            }
        }

        public ActionResult Checkout(int id)
        {
            // create a default order
            Order order = new Order();
            PopulateOrderForView(order, id);
            
            return RenderView("Checkout", CheckoutViewData(order));
        }

        private void PopulateOrderForView(Order order, int basketId)
        {
            if (order.Basket == null) order.Basket = basketRepository.GetById(basketId);
            if (order.Contact == null) order.Contact = new Contact();
            if (order.Contact1 == null) order.Contact1 = new Contact();
            if (order.Card == null) order.Card = new Card();
        }

        private ShopViewData CheckoutViewData(Order order)
        {
            order.CalculatePostage(postageRepository.GetAll());

            return ShopView.Data
                .WithCountries(countryRepository.GetAll().InOrder())
                .WithCardTypes(cardTypeRepository.GetAll())
                .WithOrder(order);
        }

        public ActionResult PlaceOrder()
        {
            Order order = new Order
            {
                OrderStatusId = OrderStatus.CreatedId,
                CreatedDate = DateTime.Now,
                DispatchedDate = DateTime.Now
            };

            Validator validator = new Validator
            {
                () => UpdateOrder(order),
                () => UpdateCardContact(order),
                () => UpdateDeliveryContact(order),
                () => UpdateCard(order)
            };

            try
            {
                validator.Validate();
                orderRepository.InsertOnSubmit(order);
                orderRepository.SubmitChanges();
                return RenderView("Item", CheckoutViewData(order));
            }
            catch (ValidationException validationException)
            {
                PopulateOrderForView(order, order.BasketId);
                return RenderView("Checkout", CheckoutViewData(order)
                    .WithErrorMessage(validationException.Message)
                    );
            }
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

            return RenderView("Item", CheckoutViewData(order));
        }

        public ActionResult Reject(int id)
        {
            Order order = orderRepository.GetById(id);

            if (order.IsCreated)
            {
                order.OrderStatusId = OrderStatus.RejectedId;
                order.UserId = CurrentUser.UserId;
                orderRepository.SubmitChanges();
            }

            return RenderView("Item", CheckoutViewData(order));
        }
    }
}
