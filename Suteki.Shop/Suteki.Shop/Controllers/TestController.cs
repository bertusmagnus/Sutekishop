using System.Web.Mvc;
using MvcContrib.UI;

namespace Suteki.Shop.Controllers
{
    public class TestController : Controller
    {
        private OrderController orderController;

        public TestController(OrderController orderController)
        {
            this.orderController = orderController;
        }

        public ActionResult Index()
        {
            // pass the current controller context to orderController
            orderController.ControllerContext = ControllerContext;

            // create a new BlockRenderer
            var blockRenderer = new BlockRenderer(HttpContext);

            // execute the Item action
            var viewResult = (ViewResult) orderController.Item(1);

            // change the master page name
            viewResult.MasterName = "Print";

            // we have to set the controller route value to the name of the controller we want to execute
            // because the 
            this.RouteData.Values["controller"] = "Order";

            string result = blockRenderer.Capture(() => viewResult.ExecuteResult(ControllerContext));

            return Content(result);
        }
    }
}
