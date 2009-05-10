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
		public void ShouldBuildCriteriaAndExecuteSearch() {
			orders.Add(new Order { OrderId = 2, OrderStatusId = 1});
			orders.Add(new Order { OrderId = 3, OrderStatusId = 2 });

			var results = service.PerformSearch(new OrderSearchCriteria() { OrderId = 3 });
			
			results.Single().ShouldBeTheSameAs(orders[1]);
		}

		[Test]
		public void ShouldExcludePending()
		{
			orders.Add(new Order() { OrderStatusId = 0 });
			orders.Add(new Order() { OrderStatusId = 1 });
			orders.Add(new Order() { OrderStatusId = 1 });

			var results = service.PerformSearch(new OrderSearchCriteria());
			results.Count.ShouldEqual(2);
		}

		//TODO: Test coverage here is lacking.
	}
}