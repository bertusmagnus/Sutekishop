using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Shop.Filters;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Controllers
{
	[AdministratorsOnly]
	public class InvoiceController : ControllerBase
	{
		readonly IRepository<Order> orderRepository;
		readonly IPostageService postageService;

		public InvoiceController(IRepository<Order> orderRepository, IPostageService postageService)
		{
			this.orderRepository = orderRepository;
			this.postageService = postageService;
		}

		public ActionResult Show(int id)
		{
			var order = orderRepository.GetById(id);
			postageService.CalculatePostageFor(order);

			AppendTitle("Invoice {0}".With(order.OrderId));

			return View(ShopView.Data.WithOrder(order));
		}
	}
}