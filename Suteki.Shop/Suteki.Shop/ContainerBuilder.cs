using System.Reflection;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Microsoft.Practices.ServiceLocation;
using Suteki.Common.Repositories;
using Suteki.Common.Windsor;
using Suteki.Shop.IoC;
using Suteki.Shop.Services;

namespace Suteki.Shop
{
	public static class ContainerBuilder
	{
		public static IWindsorContainer Build(string configPath)
		{
			var container = new WindsorContainer(new XmlInterpreter(configPath));

			// register handler selectors
			container.Kernel.AddHandlerSelector(new UrlBasedComponentSelector(
				typeof(IBaseControllerService),
				typeof(IImageFileService),
				typeof(IConnectionStringProvider)
				));

			// automatically register controllers
			container.Register(AllTypes
				.Of<Controller>()
				.FromAssembly(Assembly.GetExecutingAssembly())
				.Configure(c => c.LifeStyle.Transient.Named(c.Implementation.Name.ToLower())));

			container.Register(
				Component.For<IFormsAuthentication>().ImplementedBy<FormsAuthenticationWrapper>(),
				Component.For<IServiceLocator>().Instance(new WindsorServiceLocator(container))
			);

			return container;
		}
	}
}