using System;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Controllers
{
	[TestFixture]
	public class OrderStatusControllerTests
	{
		OrderStatusController controller;
		IUserService userService;
		IRepository<Order> repository;

		[SetUp]
		public void Setup()
		{
			userService = MockRepository.GenerateStub<IUserService>();
			repository = MockRepository.GenerateStub<IRepository<Order>>();
			controller = new OrderStatusController(repository, userService);

			userService.Expect(x => x.CurrentUser).Return(new User() { UserId = 4 });
		}


		[Test]
		public void Dispatch_ShouldChangeOrderStatusAndDispatchedDate()
		{
			const int orderId = 44;
			var order = new Order
			{
				OrderId = orderId,
				OrderStatusId = OrderStatus.CreatedId,
				Basket = new Basket()
			};

			repository.Expect(or => or.GetById(orderId)).Return(order);

			controller.Dispatch(orderId);

			order.IsDispatched.ShouldBeTrue();
			order.DispatchedDateAsString.ShouldEqual(DateTime.Now.ToShortDateString());
			order.UserId.ShouldEqual(4);
		}

		[Test]
		public void UndoStatus_ShouldChangeOrderStatusToCreated()
		{
			const int orderId = 44;
			var order = new Order
			{
				OrderId = orderId,
				OrderStatusId = OrderStatus.DispatchedId,
				DispatchedDate = DateTime.Now,
				Basket = new Basket()
			};

			repository.Expect(or => or.GetById(orderId)).Return(order);

			controller.UndoStatus(orderId);
			order.IsCreated.ShouldBeTrue();
		}
	}
}