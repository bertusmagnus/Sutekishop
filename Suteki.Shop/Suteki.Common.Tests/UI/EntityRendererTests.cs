using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Suteki.Common.Extensions;
using Suteki.Common.Models;
using Suteki.Common.Services;
using Suteki.Common.Tests.TestHelpers;
using Suteki.Common.Tests.UI.EntityRendererTestDomain;
using Rhino.Mocks;
using Suteki.Common.UI;

namespace Suteki.Common.Tests.UI
{
    [TestFixture]
    public class EntityRendererTests
    {
        [SetUp]
        public void SetUp()
        {
            var thingRenderer = new ThingRenderer();

            var serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            serviceLocator.Stub(sl => sl.GetInstance<IEntityRenderer<Thing>>()).Return(thingRenderer);
            serviceLocator.Stub(sl => sl.GetInstance(typeof (IEntityRenderer<Thing>))).Return(thingRenderer);

            var entityTypeResolver = new DefaultEntityTypeResolver();
            serviceLocator.Stub(sl => sl.GetInstance<IEntityTypeResolver>()).Return(entityTypeResolver);

            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.SetLocatorProvider(() => null);
        }

        [Test]
        public void ShouldCorrectlyRenderEntity()
        {
            var thing = new Thing
            {
                Description = "I am the Thing!",
                Age = 1864
            };

            var htmlHelper = MvcTestHelpers.CreateMockHtmlHelper();
            var html = EntityRenderer.Render(htmlHelper, thing);

            Assert.That(html, Is.EqualTo("I am the Thing!, age = 1864"));
        }

    }

    namespace EntityRendererTestDomain
    {
        public class Thing : Entity<Thing>
        {
            public int Age { get; set; }
            public string Description { get; set; }
        }

        public class ThingRenderer : IEntityRenderer<Thing>
        {
            public string Render(HtmlHelper htmlHelper, Thing entity)
            {
                return "{0}, age = {1}".With(entity.Description, entity.Age);
            }
        }
    }
}