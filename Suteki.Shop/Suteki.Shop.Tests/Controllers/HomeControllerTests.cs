using NUnit.Framework;
using Suteki.Shop.Controllers;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        HomeController homeController;
        ControllerTestContext testContext;

        [SetUp]
        public void SetUp()
        {
            homeController = new HomeController();
            testContext = new ControllerTestContext(homeController);
        }

        [Test]
        public void IndexShouldRenderViewIndex()
        {
            string view = "Index";

            homeController.Index();

            Assert.AreEqual(view, testContext.ViewEngine.ViewContext.ViewName);
        }
    }
}
