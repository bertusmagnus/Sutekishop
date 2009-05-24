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

		[SetUp]
		public void Setup()
		{
			countryRepository = MockRepository.GenerateStub<IRepository<Country>>();
			controller = new MailingListController(countryRepository);
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
	}
}