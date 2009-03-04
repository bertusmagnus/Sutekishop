using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Controllers
{
	public class MenuController : Controller
	{
		private readonly IRepository<Content> contentRepository;
		private readonly IRepository<Category> categoryRepository;

		public MenuController(IRepository<Content> contentRepository, IRepository<Category> categoryRepository)
		{
			this.contentRepository = contentRepository;
			this.categoryRepository = categoryRepository;
		}

		public ActionResult MainMenu()
		{
			return View(contentRepository.GetMainMenu());
		}

		public ActionResult LeftMenu()
		{
			return View(categoryRepository.GetRootCategory());
		}
	}
}