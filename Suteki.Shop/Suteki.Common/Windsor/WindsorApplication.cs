using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Microsoft.Practices.ServiceLocation;
using MvcContrib;
using MvcContrib.Castle;
using Suteki.Common.Controllers;
using Suteki.Common.Models;
using Suteki.Common.UI;

namespace Suteki.Common.Windsor
{
    /// <summary>
    /// Creates and disposes of the Windsor Container
    /// </summary>
    public abstract class WindsorApplication : System.Web.HttpApplication, IContainerAccessor
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

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
            InitializeWindsor();
            Initialize();
        }

        /// <summary>
        /// Override this to register routes
        /// </summary>
        /// <param name="routes"></param>
        public abstract void RegisterRoutes(RouteCollection routes);

        /// <summary>
        /// Override this to do any other initialization
        /// </summary>
        public virtual void Initialize()
        {
            
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            Container.Dispose();
        }

        protected virtual void InitializeWindsor()
        {
            if (container == null)
            {
                // create a new Windsor Container
                container = CreateContainer();

                // add array resolver
                RegisterArrayResolver(container);

                // automatically register controllers
                RegisterControllers(container);

                // automatically register entity renderers
                RegisterEntityRenderers(container);

                // automatically register services
                RegisterServices(container);

                // set the controller factory to the Windsor controller factory (in MVC Contrib)
                RegisterWindsorControllerFactory(container);

                // register Windsor with the common Microsoft ServiceLocator
                RegisterWithServiceLocator(container);
            }
        }

        protected virtual void RegisterArrayResolver(IWindsorContainer windsorContainer)
        {
            windsorContainer.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));
        }

        protected virtual void RegisterControllers(IWindsorContainer windsorContainer)
        {
            windsorContainer.Register(AllTypes
               .Of<IController>()
               .FromAssembly(GetControllerAssembly())
               .Configure(c => c.LifeStyle.Transient.Named(c.Implementation.Name.ToLower())));
        }


        protected abstract Assembly GetControllerAssembly();

        /// <summary>
        /// Override this method to implement service registration
        /// </summary>
        /// <param name="windsorContainer"></param>
        protected virtual void RegisterServices(IWindsorContainer windsorContainer)
        {
            
        }

        /// <summary>
        /// Entity renderers provide a default way of rendering entities. Add implementations of 
        /// IEntityRenderer<> to your controller assembly. They will be automatically registered here.
        /// Use:
        /// 
        /// Html.RenderEntity(myEntity)
        /// 
        /// in your views. The correct renderer for the given entity will be chosen
        /// </summary>
        /// <param name="windsorContainer"></param>
        protected virtual void RegisterEntityRenderers(IWindsorContainer windsorContainer)
        {
            windsorContainer.Register(
                Component.For(typeof(IEntityRenderer<>)).ImplementedBy(typeof(DefaultEntityRenderer<>)),
                Component.For<IEntityRenderer<CollectionEntity>>().ImplementedBy<CollectionEntityRenderer>(),
                AllTypes
                   .FromAssembly(GetControllerAssembly())
                   .BasedOn(typeof(IEntityRenderer<>))
                   .WithService.Base()
            );
        }

        protected virtual WindsorContainer CreateContainer()
        {
            return new WindsorContainer(new XmlInterpreter());
        }

        protected virtual void RegisterWindsorControllerFactory(IWindsorContainer windsorContainer)
        {
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(windsorContainer));
        }

        protected virtual void RegisterWithServiceLocator(IWindsorContainer windsorContainer)
        {
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
        }
    }
}