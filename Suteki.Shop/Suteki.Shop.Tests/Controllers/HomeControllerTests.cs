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

            ViewResult result = homeController.Index() as ViewResult;

            Assert.AreEqual(view, result.ViewName);
        }
    }
}
