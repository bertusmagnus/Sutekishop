using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Shop.Binders;

namespace Suteki.Shop.Tests.Binders
{
	[TestFixture]
	public class BindUsingAttributeTester
	{
		[SetUp]
		public void Setup()
		{
			var locator = MockRepository.GenerateStub<IServiceLocator>();
			locator.Expect(x => x.GetInstance(Arg<Type>.Is.Anything)).Do(new Func<Type, object>(Activator.CreateInstance));
			ServiceLocator.SetLocatorProvider(() => locator);
		}

		[TearDown]
		public void Teardown()
		{
			ServiceLocator.SetLocatorProvider(null);
		}

		[Test]
		public void Should_delegate_to_inner_binder()
		{
			var attribute = new BindUsingAttribute(typeof(TestBinder));
			attribute.GetBinder().ShouldBe<TestBinder>();
		}

		[Test]
		public void Should_throw_if_type_is_not_IModelBinder()
		{
			typeof(InvalidOperationException).ShouldBeThrownBy(() => new BindUsingAttribute(typeof(IDisposable)));
		}

		[Test]
		public void Binder_should_Accept_attribute()
		{
			var attribute = new BindUsingAttribute(typeof(TestBinder2));
			var binder = attribute.GetBinder() as TestBinder2;
			binder.Attribute.ShouldNotBeNull();
		}

		private class TestBinder2 : IModelBinder, IAcceptsAttribute
		{
			public BindUsingAttribute Attribute;

			public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
			{
				return null;
			}


			public void Accept(Attribute attribute)
			{
				this.Attribute = (BindUsingAttribute) attribute;
			}
		}

		private class TestBinder : IModelBinder
		{
			public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
			{
				return null;
			}
		}
	}
}