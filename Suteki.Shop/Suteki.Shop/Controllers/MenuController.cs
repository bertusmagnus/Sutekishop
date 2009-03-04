using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Controllers
{
	public class MenuController : Controller
	{
		private IRepository<Content> contentRepository;

		public MenuController(IRepository<Content> contentRepository)
		{
			this.contentRepository = contentRepository;
		}

		public ActionResult MainMenu()
		{
			return View(contentRepository.GetMainMenu());
		}
	}
}