using NUnit.Framework;
using Suteki.Shop.Controllers;
using System.Web.Mvc;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        HomeController homeController;

        [SetUp]
        public void SetUp()
        {
            homeController = new HomeController();
        }

        [Test]
        public void IndexShouldRenderViewIndex()
        {
            string view = "Index";

            RenderViewResult result = homeController.Index() as RenderViewResult;

            Assert.AreEqual(view, result.ViewName);
        }
    }
}
