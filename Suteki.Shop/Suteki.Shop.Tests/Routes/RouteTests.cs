using System;
using System.Web.Routing;
using NUnit.Framework;
using Suteki.Shop.Routes;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Tests.Routes
{
    [TestFixture]
    public class RouteTests
    {
        RouteCollection Routes;

        [SetUp]
        public void SetUp()
        {
            Routes = new RouteCollection();
            RouteManager.RegisterRoutes(Routes);
        }

        [Test]
        public void DefaultShouldRouteToHomeIndex()
        {
            AssertRoutes(Routes, "~/Default.aspx", new { Controller = "Home", Action = "Index" });
        }

        [Test]
        public void DomainOnlyShouldRouteToHomeIndex()
        {
            AssertRoutes(Routes, "~/", new { Controller = "Home", Action = "Index" });
        }

        [Test]
        public void ControllerOnlyShouldDefaultToIndex()
        {
            AssertRoutes(Routes, "~/MyController", new { Controller = "MyController", Action = "Index" });
        }

        [Test]
        public void AnyControllerActionPairShouldRouteToCorrectControllerAction()
        {
            AssertRoutes(Routes, "~/MyController/MyAction", new { Controller = "MyController", Action = "MyAction" });
        }

        [Test]
        public void IdNotSpecifiedShouldDefaultToEmptyString()
        {
            AssertRoutes(Routes, "~/SomeController/SomeAction", new { Id = string.Empty });
        }

        [Test]
        public void IdSpecifiedShouldBecomeIdProperty()
        {
            AssertRoutes(Routes, "~/SomeController/SomeAction/103", new { Id = "103" });
        }

        [Test]
        public void CmsUrlsShouldMapToCmsControllerIndexAction()
        {
            AssertRoutes(Routes, "~/cms/help", new { Controller = "Cms", Action = "Index", UrlName = "help" });
        }

        /// <summary>
        /// Common method for asserting routes
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="relativeUrl"></param>
        private static void AssertRoutes(RouteCollection routes, string relativeUrl, object propertyBag)
        {
            HttpContextTestContext testContext = new HttpContextTestContext();

            testContext.RequestMock.ExpectGet(request => request.AppRelativeCurrentExecutionFilePath)
                .Returns(relativeUrl);

            testContext.RequestMock.ExpectGet(request => request.PathInfo).Returns(string.Empty);

            RouteData routeData = routes.GetRouteData(testContext.Context);

            foreach (var property in propertyBag.GetProperties())
            {
                Assert.AreEqual(property.Value, routeData.Values[property.Name]);
            }
        }
    }
}
