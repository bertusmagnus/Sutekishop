using System;
using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Controllers
{
    public class HomeController : ControllerBase
    {
        public const string Shopfront = "Shopfront";

        readonly IRepository<Content> contentRepository;

        public HomeController(IRepository<Content> contentRepository)
        {
            this.contentRepository = contentRepository;
        }

        public ActionResult Index()
        {
            var content = contentRepository.GetAll().WithUrlName(Shopfront);
            return View("Index", CmsView.Data.WithContent(content));
        }

        public ActionResult Error()
        {
            throw new ApplicationException("This error was thrown!");
        }
    }
}
