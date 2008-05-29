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
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Controllers
{
    public class BasketController : ControllerBase
    {
        IRepository<Basket> basketRepository;
        IRepository<BasketItem> basketItemRepository;
        IRepository<User> userRepository;
        IRepository<Postage> postageRepository;
        IRepository<Size> sizeRepository;
        IUserService userService;

        public BasketController(
            IRepository<Basket> basketRepository,
            IRepository<BasketItem> basketItemRepository,
            IRepository<User> userRepository,
            IRepository<Postage> postageRepository,
            IRepository<Size> sizeRepository,
            IUserService userService)
        {
            this.basketRepository = basketRepository;
            this.basketItemRepository = basketItemRepository;
            this.userRepository = userRepository;
            this.postageRepository = postageRepository;
            this.sizeRepository = sizeRepository;
            this.userService = userService;
        }

        public ActionResult Index()
        {
            User user = CurrentUser;
            return RenderIndexView(user.CurrentBasket);
        }

        public ActionResult Update()
        {
            var user = CurrentUser;

            // if the current user is a guest, promote them to a new customer
            if (user.RoleId == Role.GuestId)
            {
                user = userService.CreateNewCustomer();
                this.SetAuthenticationCookie(user.Email);
                this.SetContextUserTo(user);
            }

            var basket = user.CurrentBasket;

            var basketItem = new BasketItem();
            ValidatingBinder.UpdateFrom(basketItem, Request.Form);

            var size = sizeRepository.GetById(basketItem.SizeId);
            if (!size.IsInStock)
            {
                return RenderIndexViewWithError(basket, size);
            }

            basket.BasketItems.Add(basketItem);
            basketRepository.SubmitChanges();
            return RenderIndexView(basket);
        }

        private ActionResult RenderIndexViewWithError(Basket basket, Size size)
        {
            string message = null;
            if (size.Product.HasSize)
            {
                message = "Sorry, {0}, Size {1} is out of stock.".With(size.Product.Name, size.Name);
            }
            else
            {
                message = "Sorry, {0} is out of stock.".With(size.Product.Name);
            }
            return View("Index", IndexViewData(basket).WithErrorMessage(message));
        }

        private ActionResult RenderIndexView(Basket basket)
        {
            return View("Index", IndexViewData(basket));
        }

        private ShopViewData IndexViewData(Basket basket)
        {
            var postages = postageRepository.GetAll();
            return ShopView.Data.WithBasket(basket).WithTotalPostage(basket.CalculatePostage(postages));
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
