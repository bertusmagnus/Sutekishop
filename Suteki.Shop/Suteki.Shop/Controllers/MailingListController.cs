using System;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Shop.Binders;
using Suteki.Shop.ViewData;
using MvcContrib;
namespace Suteki.Shop.Controllers
{
	public class MailingListController : ControllerBase
	{
		IRepository<Country> countryRepository;
		IRepository<MailingListSubscription> subscriptionRepository;

		public MailingListController(IRepository<Country> countryRepository, IRepository<MailingListSubscription> subscriptionRepository)
		{
			this.countryRepository = countryRepository;
			this.subscriptionRepository = subscriptionRepository;
		}

		public ActionResult Index()
		{
			return View(
				ShopView.Data.WithCountries(countryRepository.GetAll())
			);
		}

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Index([BindMailingList] MailingListSubscription mailingListSubscription)
		{
			if(ModelState.IsValid)
			{
				subscriptionRepository.InsertOnSubmit(mailingListSubscription);
				return this.RedirectToAction(c => c.Confirm());
			}

			return View(
				ShopView.Data.WithCountries(countryRepository.GetAll())
				.WithSubscription(mailingListSubscription)
			);
		}

		public ActionResult Confirm()
		{
			return View();
		}
	}
}