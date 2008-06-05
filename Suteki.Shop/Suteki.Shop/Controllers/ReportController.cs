using System;
using System.Linq;
using System.Security.Permissions;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;

namespace Suteki.Shop.Controllers
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
    public class ReportController : ControllerBase
    {
        readonly private IRepository<Order> orderRepository;

        public ReportController(IRepository<Order> orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Orders()
        {
            string ordersCsv = orderRepository.GetAll().Select(o => new 
            {
                o.OrderId, 
                o.Email,
                OrderStatus = o.OrderStatus.Name, 
                o.CreatedDate, 
                o.Basket.Total
            }).AsCsv();

            return Content(ordersCsv, "text/csv");
        }
    }
}
