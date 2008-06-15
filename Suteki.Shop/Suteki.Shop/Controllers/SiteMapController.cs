using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Controllers
{
    public class SiteMapController : ControllerBase
    {
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<Content> contentRepository;

        public SiteMapController(IRepository<Product> productRepository, IRepository<Content> contentRepository)
        {
            this.productRepository = productRepository;
            this.contentRepository = contentRepository;
        }

        public ActionResult Index()
        {
            var products = productRepository.GetAll().ActiveFor(CurrentUser);
            var contents = contentRepository.GetAll().ActiveFor(CurrentUser);

            return View("Index", ShopView.Data
                .WithProducts(products)
                .WithContents(contents));
        }
    }
}
