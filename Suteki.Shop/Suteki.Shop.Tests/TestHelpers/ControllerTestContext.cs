using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace Suteki.Shop.Tests
{
    public class MoqControllerTestContext
    {
        public MockViewEngine ViewEngine { get; private set; }
        public HttpContextTestContext TestContext { get; private set; }

        public MoqControllerTestContext(Controller controller)
        {
            ViewEngine = new MockViewEngine();
            controller.ViewEngine = ViewEngine;

            TestContext = new HttpContextTestContext();

            ControllerContext controllerContext = new ControllerContext(
                new RequestContext(TestContext.Context, new RouteData()),
                controller);

            controller.ControllerContext = controllerContext;
        }
    }
}
