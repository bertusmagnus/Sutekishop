using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using NUnit.Framework;
using Suteki.Shop.Controllers;
using ControllerBase=Suteki.Shop.Controllers.ControllerBase;

namespace Suteki.Shop.Tests
{
	[TestFixture]
	public class IoCTests
	{
		private IWindsorContainer container;

		[SetUp]
		public void Setup()
		{
			container = ContainerBuilder.Build("Windsor.config");
			
			//This is necessary until we remove the use of PrincipalPermission attributes from controllers
			Thread.CurrentPrincipal = new FakePrincipal();

			//Hackery in order to get the PerWebRequest lifecycle working in a test environment
			//Surely there must be a better way to do this?
			HttpContext.Current = new HttpContext(new HttpRequest("foo", "http://localhost", ""), new HttpResponse(new StringWriter()));
			HttpContext.Current.ApplicationInstance = new HttpApplication();
			var module = new  PerWebRequestLifestyleModule();
			module.Init(HttpContext.Current.ApplicationInstance);
		}

		[TearDown]
		public void Teardown()
		{
			HttpContext.Current = null;
			container.Dispose();
		}

		[Test]
		public void ShouldConstructControllers()
		{
			var controllerTypes = from type in typeof (HomeController).Assembly.GetExportedTypes()
			                      where typeof (Controller).IsAssignableFrom(type)
			                      where !type.IsAbstract
			                      select type;


			foreach(var type in controllerTypes)
			{
				var controller = (Controller) container.Resolve(type);
				var baseController = controller as ControllerBase;

				if(baseController != null)
				{
					baseController.BaseControllerService.ShouldNotBeNull();
				}
			}
		}

		private class FakePrincipal : IPrincipal, IIdentity
		{
			public bool IsInRole(string role)
			{
				return true;
			}

			public IIdentity Identity
			{
				get { return this; }
			}

			public string Name
			{
				get { return "foo"; }
			}

			public string AuthenticationType
			{
				get { return "foo"; }
			}

			public bool IsAuthenticated
			{
				get { return true; }
			}
		}
	}
}