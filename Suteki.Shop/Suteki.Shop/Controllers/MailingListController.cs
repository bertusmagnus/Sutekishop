using System;
using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Controllers
{
	public class MailingListController : ControllerBase
	{
		IRepository<Country> countryRepository;

		public MailingListController(IRepository<Country> countryRepository)
		{
			this.countryRepository = countryRepository;
		}

		public ActionResult Index()
		{
			return View(
				ShopView.Data.WithCountries(countryRepository.GetAll())
			);
		}
	}
}