using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;
using System.Web.Security;
using Suteki.Shop.Services;

namespace Suteki.Shop.Controllers
{
    public class BasketController : ControllerBase
    {
        IRepository<Basket> basketRepository;
        IRepository<BasketItem> basketItemRepository;
        IRepository<User> userRepository;
        IRepository<Postage> postageRepository;
        IUserService userService;

        public BasketController(
            IRepository<Basket> basketRepository,
            IRepository<BasketItem> basketItemRepository,
            IRepository<User> userRepository,
            IRepository<Postage> postageRepository,
            IUserService userService)
        {
            this.basketRepository = basketRepository;
            this.basketItemRepository = basketItemRepository;
            this.userRepository = userRepository;
            this.postageRepository = postageRepository;
            this.userService = userService;
        }

        public ActionResult Index()
        {
            User user = this.ControllerContext.HttpContext.User as User;
            if (user == null) throw new ApplicationException("HttpContext.User is not a Suteki.Shop.User");

            return RenderIndexView(user.CurrentBasket);
        }

        public ActionResult Update()
        {
            User user = CurrentUser;

            // if the current user is a guest, promote them to a new customer
            if (user.RoleId == Role.GuestId)
            {
                user = userService.CreateNewCustomer();
                this.SetAuthenticationCookie(user.Email);
                this.SetContextUserTo(user);
            }

            Basket basket = user.CurrentBasket;

            BasketItem basketItem = new BasketItem();
            try
            {
                ValidatingBinder.UpdateFrom(basketItem, Request.Form);
                basket.BasketItems.Add(basketItem);
                basketRepository.SubmitChanges();
                return RenderIndexView(basket);
            }
            catch(ValidationException)
            {
                // we shouldn't get a validation exception here, so this is a genuine system error
                throw;
            }
        }

        private ActionResult RenderIndexView(Basket basket)
        {
            var postages = postageRepository.GetAll();

            return RenderView("Index", View.Data
                .WithBasket(basket)
                .WithTotalPostage(basket.CalculatePostage(postages)));
        }

        public ActionResult Remove(int id)
        {
            Basket basket = CurrentUser.CurrentBasket;
            BasketItem basketItem = basket.BasketItems.Where(item => item.BasketItemId == id).SingleOrDefault();

            if (basketItem != null)
            {
                basketItemRepository.DeleteOnSubmit(basketItem);
                basketItemRepository.SubmitChanges();
            }

            return RenderIndexView(basket);
        }
    }
}
