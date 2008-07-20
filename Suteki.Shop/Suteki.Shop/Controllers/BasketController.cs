using System.Linq;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.ViewData;
using Suteki.Shop.Services;

namespace Suteki.Shop.Controllers
{
    public class BasketController : ControllerBase
    {
        readonly IRepository<Basket> basketRepository;
        readonly IRepository<BasketItem> basketItemRepository;
        readonly IRepository<Size> sizeRepository;
        readonly IUserService userService;
        readonly IPostageService postageService;
        readonly IRepository<Country> countryRepository;

        public BasketController(
            IRepository<Basket> basketRepository,
            IRepository<BasketItem> basketItemRepository,
            IRepository<Size> sizeRepository,
            IUserService userService,
            IPostageService postageService, 
            IRepository<Country> countryRepository)
        {
            this.basketRepository = basketRepository;
            this.basketItemRepository = basketItemRepository;
            this.sizeRepository = sizeRepository;
            this.userService = userService;
            this.postageService = postageService;
            this.countryRepository = countryRepository;
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
                SetAuthenticationCookie(user.Email);
                SetContextUserTo(user);
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

            return RedirectToRoute(new {Controller = "Basket", Action = "Index"});
        }

        private ActionResult RenderIndexViewWithError(Basket basket, Size size)
        {
            string message;
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
            if (basket.Country == null)
            {
                basket.Country = countryRepository.GetById(basket.CountryId);
            }
            return ShopView.Data.WithBasket(basket)
                .WithTotalPostage(postageService.CalculatePostageFor(basket));
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
