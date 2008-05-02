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
            Basket basket = basketRepository.GetById(id);

            // create a default order
            Order order = new Order
            {
                Basket = basket,
                Contact = new Contact(),
                Contact1 = new Contact(),
                Card = new Card()
            };
            
            return RenderView("Checkout", CheckoutViewData.WithOrder(order));
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
            Contact cardContact = new Contact();
            Contact deliveryContact = new Contact();
            Card card = new Card();

            Validator validator = new Validator
            {
                () => ValidatingBinder.UpdateFrom(order, Request.Form, "order"),
                () => ValidatingBinder.UpdateFrom(cardContact, Request.Form, "cardcontact"),
                () => ValidatingBinder.UpdateFrom(deliveryContact, Request.Form, "deliverycontact"),
                () => ValidatingBinder.UpdateFrom(card, Request.Form, "card")
            };

            order.Contact = cardContact;
            order.Contact1 = deliveryContact;
            order.Card = card;

            try
            {
                validator.Validate();
                orderRepository.InsertOnSubmit(order);
                orderRepository.SubmitChanges();
                return RenderView("Item", View.Data.WithOrder(order));
            }
            catch (ValidationException validationException)
            {
                return RenderView("Checkout", CheckoutViewData
                    .WithOrder(order)
                    .WithErrorMessage(validationException.Message)
                    );
            }
        }
    }
}
