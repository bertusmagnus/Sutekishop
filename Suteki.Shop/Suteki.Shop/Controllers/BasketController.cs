using System.Linq;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Binders;
using Suteki.Shop.ViewData;
using Suteki.Shop.Services;
using MvcContrib;
namespace Suteki.Shop.Controllers
{
    public class BasketController : ControllerBase
    {
        private readonly IRepository<Basket> basketRepository;
        private readonly IRepository<BasketItem> basketItemRepository;
        private readonly IRepository<Size> sizeRepository;
        private readonly IUserService userService;
        private readonly IPostageService postageService;
        private readonly IRepository<Country> countryRepository;
        private readonly IValidatingBinder validatingBinder;

        public BasketController(
            IRepository<Basket> basketRepository,
            IRepository<BasketItem> basketItemRepository,
            IRepository<Size> sizeRepository,
            IUserService userService,
            IPostageService postageService, 
            IRepository<Country> countryRepository,
            IValidatingBinder validatingBinder)
        {
            this.basketRepository = basketRepository;
            this.basketItemRepository = basketItemRepository;
            this.sizeRepository = sizeRepository;
            this.userService = userService;
            this.postageService = postageService;
            this.countryRepository = countryRepository;
            this.validatingBinder = validatingBinder;
        }

        public ActionResult Index()
        {
            var user = userService.CurrentUser;
            return  View("Index", IndexViewData(user.CurrentBasket));
        }

        [UnitOfWork, AcceptVerbs(HttpVerbs.Post)] //TODO: Change this to use DataBind after the ProductController has been updated
		public ActionResult Update([CurrentBasket] Basket basket, FormCollection form)
        {
            var basketItem = new BasketItem();
            validatingBinder.UpdateFrom(basketItem, form, ModelState);

            var size = sizeRepository.GetById(basketItem.SizeId);

            if (!size.IsInStock)
            {
                return RenderIndexViewWithError(basket, size);
            }

            basket.BasketItems.Add(basketItem);

			return this.RedirectToAction(c => c.Index());
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

        private ShopViewData IndexViewData(Basket basket)
        {
            if (basket.Country == null)
            {
                basket.Country = countryRepository.GetById(basket.CountryId);
            }
            return ShopView.Data.WithBasket(basket)
                .WithTotalPostage(postageService.CalculatePostageFor(basket));
        }

		[UnitOfWork]
        public ActionResult Remove(int id)
        {
            var basket = userService.CurrentUser.CurrentBasket;
            var basketItem = basket.BasketItems.Where(item => item.BasketItemId == id).SingleOrDefault();

            if (basketItem != null)
            {
                basketItemRepository.DeleteOnSubmit(basketItem);
            }

			return this.RedirectToAction(c => c.Index());
        }
    }
}
