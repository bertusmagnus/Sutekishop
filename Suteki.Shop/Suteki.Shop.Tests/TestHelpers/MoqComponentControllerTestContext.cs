using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Moq;
using System.Web.Routing;
using Suteki.Shop.Components;

namespace Suteki.Shop.Tests
{
    public class MoqComponentControllerTestContext
    {
        public HttpContextTestContext TestContext { get; private set; }

        public MoqComponentControllerTestContext(ComponentControllerBase componentController)
        {
            TestContext = new HttpContextTestContext();
            Controller controller = new Mock<Controller>().Object;

            ControllerContext controllerContext = new ControllerContext(
                new RequestContext(TestContext.Context, new RouteData()),
                controller);

            ViewContext viewContext = new ViewContext(controllerContext, "viewName", "masterName", null, null);
            componentController.Context = viewContext;
        }
    }
}
