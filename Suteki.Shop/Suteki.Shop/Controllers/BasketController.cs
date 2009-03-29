using System.Linq;
using System.Web.Mvc;
using Suteki.Common.Binders;
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
    	private readonly IRepository<BasketItem> basketItemRepository;
        private readonly IRepository<Size> sizeRepository;
        private readonly IUserService userService;
        private readonly IPostageService postageService;
        private readonly IRepository<Country> countryRepository;

    	public BasketController(IRepository<BasketItem> basketItemRepository, IRepository<Size> sizeRepository, IUserService userService, IPostageService postageService, IRepository<Country> countryRepository)
        {
    		this.basketItemRepository = basketItemRepository;
            this.sizeRepository = sizeRepository;
            this.userService = userService;
            this.postageService = postageService;
            this.countryRepository = countryRepository;
        }

        public ActionResult Index()
        {
            var user = userService.CurrentUser;
            return  View("Index", IndexViewData(user.CurrentBasket));
        }

        [UnitOfWork, AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Update([CurrentBasket] Basket basket, [DataBind(Fetch = false)] BasketItem basketItem)
        {
			var size = sizeRepository.GetById(basketItem.SizeId);

            if (!size.IsInStock)
            {
                Message = RenderIndexViewWithError(size);
				return this.RedirectToAction<ProductController>(c => c.Item(size.Product.UrlName));
            }

            basket.BasketItems.Add(basketItem);

			return this.RedirectToAction(c => c.Index());
        }

        private string RenderIndexViewWithError(Size size)
        {
        	if (size.Product.HasSize)
            {
                return "Sorry, {0}, Size {1} is out of stock.".With(size.Product.Name, size.Name);
            }

        	return "Sorry, {0} is out of stock.".With(size.Product.Name);
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
