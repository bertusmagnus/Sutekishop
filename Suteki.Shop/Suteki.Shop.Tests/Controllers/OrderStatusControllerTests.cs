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
		IEmailService emailService;
		[SetUp]
		public void Setup()
		{
			userService = MockRepository.GenerateStub<IUserService>();
			repository = MockRepository.GenerateStub<IRepository<Order>>();
			emailService = MockRepository.GenerateStub<IEmailService>();
			controller = new OrderStatusController(repository, userService, emailService);

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
		public void Dispatch_SendsDispatchEmail()
		{
			const int orderId = 44;
			var order = new Order() { OrderStatusId = OrderStatus.CreatedId};

			repository.Expect(x => x.GetById(orderId)).Return(order);
			controller.Dispatch(orderId);

			emailService.AssertWasCalled(x => x.SendDispatchNotification(order));
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