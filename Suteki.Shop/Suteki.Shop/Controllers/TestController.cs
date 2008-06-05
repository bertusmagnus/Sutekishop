using System.Web.Mvc;
using Castle.Core.Logging;
using Suteki.Common.Extensions;

namespace Suteki.Shop.Controllers
{
    public class TestController : Controller
    {
        private readonly OrderController orderController;
        private readonly ILogger logger;

        public TestController(OrderController orderController, ILogger logger)
        {
            this.orderController = orderController;
            this.logger = logger;
        }

        public ActionResult Index()
        {
            string html = this.CaptureActionHtml(orderController, "Print", c => (ViewResult)c.Item(8));

            logger.Info(html);

            // do a redirect
            return RedirectToRoute(new {Controller = "Order", Action = "Item", Id = 8});
        }
    }
}
