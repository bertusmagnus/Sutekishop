using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;

namespace Suteki.Shop.Controllers
{
    public class OrderController : ControllerBase
    {
        IRepository<Order> orderRepository;
        IRepository<Basket> basketRepository;
        IRepository<Country> countryRepository;
        IRepository<CardType> cardTypeRepository;

        public OrderController(
            IRepository<Order> orderRepository,
            IRepository<Basket> basketRepository,
            IRepository<Country> countryRepository,
            IRepository<CardType> cardTypeRepository)
        {
            this.orderRepository = orderRepository;
            this.basketRepository = basketRepository;
            this.countryRepository = countryRepository;
            this.cardTypeRepository = cardTypeRepository;
        }
        
        public ActionResult Index()
        {
            return null;
        }

        public ActionResult Checkout(int id)
        {
            // create a default order
            Order order = new Order();
            PopulateOrderForView(order, id);
            
            return RenderView("Checkout", CheckoutViewData.WithOrder(order));
        }

        private void PopulateOrderForView(Order order, int basketId)
        {
            if (order.Basket == null) order.Basket = basketRepository.GetById(basketId);
            if (order.Contact == null) order.Contact = new Contact();
            if (order.Contact1 == null) order.Contact1 = new Contact();
            if (order.Card == null) order.Card = new Card();
        }

        private ShopViewData CheckoutViewData
        {
            get
            {
                return View.Data
                    .WithCountries(countryRepository.GetAll().InOrder())
                    .WithCardTypes(cardTypeRepository.GetAll());
            }
        }

        public ActionResult PlaceOrder()
        {
            Order order = new Order();

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
                return RenderView("Item", View.Data.WithOrder(order));
            }
            catch (ValidationException validationException)
            {
                PopulateOrderForView(order, order.BasketId);
                return RenderView("Checkout", CheckoutViewData
                    .WithOrder(order)
                    .WithErrorMessage(validationException.Message)
                    );
            }
        }

        private void UpdateCardContact(Order order)
        {
            Contact cardContact = new Contact();
            order.Contact = cardContact;
            ValidatingBinder.UpdateFrom(cardContact, Request.Form, "cardcontact");
        }

        private void UpdateDeliveryContact(Order order)
        {
            if (order.UseCardHolderContact) return;

            Contact deliveryContact = new Contact();
            order.Contact1 = deliveryContact;
            ValidatingBinder.UpdateFrom(deliveryContact, Request.Form, "deliverycontact");
        }

        private void UpdateCard(Order order)
        {
            if (order.PayByTelephone) return;

            Card card = new Card();
            order.Card = card;
            ValidatingBinder.UpdateFrom(card, Request.Form, "card");
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
    }
}
