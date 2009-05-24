using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.Repositories;
using System.Collections.Generic;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
	[TestFixture]
	public class MailingListControllerTests
	{
		MailingListController controller;
		IRepository<Country> countryRepository;
		IRepository<MailingListSubscription> mailingListRepository;

		[SetUp]
		public void Setup()
		{
			countryRepository = MockRepository.GenerateStub<IRepository<Country>>();
			mailingListRepository = MockRepository.GenerateStub<IRepository<MailingListSubscription>>();
			controller = new MailingListController(countryRepository, mailingListRepository);
		}

		[Test]
		public void Index_RendersViewWithCountries()
		{
			var countries = new List<Country>().AsQueryable();
			countryRepository.Expect(x => x.GetAll()).Return(countries);

			controller.Index()
				.ReturnsViewResult()
				.WithModel<ShopViewData>()
				.AssertAreSame(countries, x => x.Countries);
		}

		[Test]
		public void IndexWithPost_RedirectsOnSuccessfulBindingAndInsertsSubscription()
		{
			var subscription = new MailingListSubscription() { Email = "foo" };
			controller.Index(subscription)
				.ReturnsRedirectToRouteResult()
				.ToAction("Confirm");

				mailingListRepository.AssertWasCalled(x => x.InsertOnSubmit(subscription));
			
		}

		[Test]
		public void IndexWithPost_RendersViewOnFailedBinding()
		{
			controller.ModelState.AddModelError("foo", "bar");
			var subscription = new MailingListSubscription() { Email = "foo"};

			controller.Index(subscription)
				.ReturnsViewResult()
				.WithModel<ShopViewData>()
				.AssertAreEqual(subscription, x => x.MailingListSubscription);

		}
	}
}