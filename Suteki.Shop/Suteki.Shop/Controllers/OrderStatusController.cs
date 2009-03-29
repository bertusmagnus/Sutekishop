using System;
using System.Web.Mvc;
using MvcContrib;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Shop.Filters;
using Suteki.Shop.Services;

namespace Suteki.Shop.Controllers
{
	[AdministratorsOnly]
	public class OrderStatusController : ControllerBase
	{
		readonly IRepository<Order> orderRepository;
		readonly IUserService userService;

		public OrderStatusController(IRepository<Order> orderRepository, IUserService userService)
		{
			this.orderRepository = orderRepository;
			this.userService = userService;
		}

		[UnitOfWork]
		public ActionResult Dispatch(int id)
		{
			var order = orderRepository.GetById(id);

			if (order.IsCreated)
			{
				order.OrderStatusId = OrderStatus.DispatchedId;
				order.DispatchedDate = DateTime.Now;
				order.UserId = userService.CurrentUser.UserId;
			}

			return this.RedirectToAction<OrderController>(c => c.Item(order.OrderId));
		}

		[UnitOfWork]
		public ActionResult Reject(int id)
		{
			var order = orderRepository.GetById(id);

			if (order.IsCreated)
			{
				order.OrderStatusId = OrderStatus.RejectedId;
				order.UserId = userService.CurrentUser.UserId;
			}

			return this.RedirectToAction<OrderController>(c => c.Item(order.OrderId));
		}

		[UnitOfWork]
		public ActionResult UndoStatus(int id)
		{
			var order = orderRepository.GetById(id);

			if (order.IsDispatched || order.IsRejected)
			{
				order.OrderStatusId = OrderStatus.CreatedId;
				order.UserId = null;
			}

			return this.RedirectToAction<OrderController>(c => c.Item(order.OrderId));
		}
	}
}