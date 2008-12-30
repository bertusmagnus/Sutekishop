using NHibernate.Proxy;
using NUnit.Framework;
using Suteki.Common.Services;
using Suteki.Common.Tests.TestModel;

namespace Suteki.Common.Tests.Services
{
    [TestFixture]
    public class NHibernateEntityTypeResolverTests
    {
        private NHibernateEntityTypeResolver resolver;

        [SetUp]
        public void SetUp()
        {
            resolver = new NHibernateEntityTypeResolver();
        }

        [Test]
        public void Non_Proxy_Should_Be_Resolved_As_Own_Type()
        {
            var resolvedType = resolver.GetRealTypeOf(typeof (Customer));

            Assert.That(resolvedType, Is.EqualTo(typeof(Customer)) );
        }

        [Test]
        public void Proxy_Should_Be_Resolved_As_Base_Type()
        {
            var resolvedType = resolver.GetRealTypeOf(typeof (PretendProxy));

            Assert.That(resolvedType, Is.EqualTo(typeof(Customer)));
        }

        private class PretendProxy : Customer, INHibernateProxy
        {
            public ILazyInitializer HibernateLazyInitializer
            {
                get { throw new System.NotImplementedException(); }
            }
        }
    }
}