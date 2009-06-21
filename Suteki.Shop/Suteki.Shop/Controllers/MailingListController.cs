using System;
using System.Linq;
using System.Web.Mvc;
using MvcContrib.Pagination;
using Suteki.Common.Binders;
using Suteki.Common.Extensions;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Shop.Binders;
using Suteki.Shop.Filters;
using Suteki.Shop.Models;
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
		public ActionResult Index([BindMailingList] MailingListSubscription mailingListSubscription, bool? addAnother)
		{
			if(ModelState.IsValid)
			{
				subscriptionRepository.InsertOnSubmit(mailingListSubscription);
				if(addAnother.GetValueOrDefault())
				{
					return this.RedirectToAction(c => c.Index());
				}
				else
				{
					return this.RedirectToAction(c => c.Confirm());					
				}
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

		[AdministratorsOnly, LoadUsing(typeof(MailingListSubscriptionsWithCountries))]
		public ActionResult List()
		{
			var subscriptions = subscriptionRepository
				.GetAll()
				.OrderBy(x => x.DateSubscribed)
				.AsPagination(1);
			return View(ShopView.Data.WithSubscriptions(subscriptions));
		}

		[AdministratorsOnly]
		public ActionResult Edit(int id)
		{
			var subscription = subscriptionRepository.GetById(id);
			var countries = countryRepository.GetAll();
			return View(ShopView.Data.WithSubscription(subscription).WithCountries(countries));
		}

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Edit([BindMailingList(Fetch = true, ValidateConfirmEmail = false)] MailingListSubscription mailingListSubscription)
		{
			if(ModelState.IsValid)
			{
				Message = "Changed have been saved.";
				return this.RedirectToAction(c => c.List());
			}

			var countries = countryRepository.GetAll();

			return View(
				ShopView.Data.WithSubscription(mailingListSubscription).WithCountries(countries)
			);
		}

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Delete(int id)
		{
			var subscription = subscriptionRepository.GetById(id);
			subscriptionRepository.DeleteOnSubmit(subscription);
			Message = "Subscription deleted.";

			return this.RedirectToAction(c => c.List());
		}
	}
}