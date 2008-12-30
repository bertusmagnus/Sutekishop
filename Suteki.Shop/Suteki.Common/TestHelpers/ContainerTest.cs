using Castle.Facilities.NHibernateIntegration;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor;

namespace Suteki.Common.TestHelpers
{
    public abstract class ContainerTest
    {
        protected ContainerTest()
        {
            Reset();
        }

        protected void Reset()
        {
            Container = new WindsorContainer(new XmlInterpreter(ConfigFileName));
            var sessionManager = Container.Resolve<ISessionManager>();
            var session = sessionManager.OpenSession();
            session.Clear();
        }

        protected abstract string ConfigFileName { get; }

        protected T GetComponent<T>()
        {
            return Container.Resolve<T>();
        }

        protected void AddCompontent<T>()
        {
            Container.AddComponent<T>();
        }

        public IWindsorContainer Container { get; private set; }
    }
}