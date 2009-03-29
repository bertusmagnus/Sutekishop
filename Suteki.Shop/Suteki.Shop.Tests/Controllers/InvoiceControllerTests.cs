using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
	[TestFixture]
	public class InvoiceControllerTests
	{
		InvoiceController controller;
		IRepository<Order> repository;
		IPostageService postageService;

		[SetUp]
		public void Setup()
		{
			repository = MockRepository.GenerateStub<IRepository<Order>>();
			postageService = MockRepository.GenerateStub<IPostageService>();
			controller = new InvoiceController(repository, postageService);
		}

		[Test]
		public void Invoice_ShouldShowOrderInInvoiceView() {
			const int orderId = 10;

			var order = new Order();

			repository.Expect(or => or.GetById(orderId)).Return(order);

			controller.Show(orderId)
				.ReturnsViewResult()
				.WithModel<ShopViewData>()
				.AssertAreSame(order, vd => vd.Order);
		}

	}
}