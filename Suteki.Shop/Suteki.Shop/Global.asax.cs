using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using MvcContrib.Castle;
using MvcContrib.ControllerFactories;
using Suteki.Shop.Routes;

namespace Suteki.Shop
{
    public class GlobalApplication : System.Web.HttpApplication, IContainerAccessor
    {
        private static WindsorContainer container;

        public static IWindsorContainer Container
        {
            get { return container; }
        }

        IWindsorContainer IContainerAccessor.Container
        {
            get { return Container; }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteManager.RegisterRoutes(RouteTable.Routes);
            InitializeWindsor();
        }

        /// <summary>
        /// This web application uses the Castle Project's IoC container, Windsor see:
        /// http://www.castleproject.org/container/index.html
        /// </summary>
        protected virtual void InitializeWindsor()
        {
            if (container == null)
            {
                // create a new Windsor Container
                container = new WindsorContainer(new XmlInterpreter("Configuration\\Windsor.config"));

                // automatically register controllers
                container.RegisterControllers(Assembly.GetExecutingAssembly());

                // set the controller factory to the Windsor controller factory (in MVC Contrib)
                ControllerBuilder.Current.SetControllerFactory(typeof(WindsorControllerFactory));
            }
        }
    }
}