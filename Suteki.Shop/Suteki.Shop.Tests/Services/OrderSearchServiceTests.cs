using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Services
{
	[TestFixture]
	public class OrderSearchServiceTests
	{
		IOrderSearchService service;
		List<Order> orders;
		IHttpContextService contextService;

		[SetUp]
		public void Setup()
		{
			orders = new List<Order>();
			var repository = MockRepository.GenerateStub<IRepository<Order>>();
			contextService = MockRepository.GenerateStub<IHttpContextService>();
			repository.Stub(x => x.GetAll()).Return(orders.AsQueryable());
			service = new OrderSearchService(repository, contextService);
		}

		[Test]
		public void Index_ShouldBuildCriteriaAndExecuteSearch() {
			orders.Add(new Order { OrderId = 2 });
			orders.Add(new Order { OrderId = 3 });

			var results = service.PerformSearch(new OrderSearchCriteria() { OrderId = 3 });
			
			results.Single().ShouldBeTheSameAs(orders[1]);
		}

		//TODO: Test coverage here is lacking.
	}
}