using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Suteki.Common.Windsor;

namespace Suteki.Common.Tests.Windsor
{
    [TestFixture]
    public class WindsorApplicationTests
    {
        private TestApplication application;

        [SetUp]
        public void SetUp()
        {
            application = new TestApplication();
            application.Start();
        }

        [Test]
        public void ShouldBeAbleToUseServiceLocator()
        {
            var controller = ServiceLocator.Current.GetInstance<IController>("testcontroller");

            Assert.That(controller is TestController);
        }
    }

    public class TestController : IController
    {
        public void Execute(RequestContext requestContext)
        {
            // not required for test
        }
    }

    public class TestApplication : WindsorApplication
    {
        public override void RegisterRoutes(RouteCollection routes)
        {
            // don't need to register routes for tests
        }

        protected override Assembly GetControllerAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        public void Start()
        {
            Application_Start(null, null);
        }
    }
}