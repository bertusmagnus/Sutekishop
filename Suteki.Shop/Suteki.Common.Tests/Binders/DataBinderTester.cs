using System;
using System.Collections.Specialized;
using System.Data.Linq.Mapping;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Binders;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;

namespace Suteki.Common.Tests.Binders
{
	[TestFixture]
	public class DataBinderTester
	{
		private DataBinder binder;
		private IValidatingBinder validatingBinder;
		private IRepository repository;
		private ModelBindingContext context;
		private ControllerContext controllerContext;

		[SetUp]
		public void Setup()
		{
			validatingBinder = MockRepository.GenerateStub<IValidatingBinder>();
			repository = MockRepository.GenerateStub<IRepository>();
			var repositoryResolver = MockRepository.GenerateStub<IRepositoryResolver>();
			repositoryResolver.Expect(x => x.GetRepository(typeof(TestEntity))).Return(repository);
			binder = new DataBinder(validatingBinder, repositoryResolver);

			context = new ModelBindingContext()
            {
                ModelName = "foo", 
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TestEntity)),
                ModelState = new ModelStateDictionary()
            };

			controllerContext= new ControllerContext()
            {
                HttpContext = MockRepository.GenerateStub<HttpContextBase>() 
            };

			controllerContext.HttpContext.Expect(x => x.Request).Return(MockRepository.GenerateStub<HttpRequestBase>());
			controllerContext.HttpContext.Request.Expect(x => x.Form).Return(new NameValueCollection() { { "foo.Id", "3"}, { "foo.Name", "Jeremy" } }).Repeat.Any();

		}

		[Test]
		public void Should_fetch_from_primary_key()
		{
			var testEntity = new TestEntity();
			repository.Expect(x => x.GetById(3)).Return(testEntity);
			
			var result = binder.BindModel(controllerContext, context);
			result.ShouldBeTheSameAs(testEntity);
		}

		[Test]
		public void Should_throw_when_id_not_found()
		{
			context.ModelName = "bar";
			typeof(InvalidOperationException).ShouldBeThrownBy(() => binder.BindModel(controllerContext, context));
		}

		[Test]
		public void Should_validate()
		{
			var testEntity = new TestEntity();
			repository.Expect(x => x.GetById(3)).Return(testEntity);
			binder.BindModel(controllerContext, context);
			validatingBinder.AssertWasCalled(x => x.UpdateFrom(testEntity, controllerContext.HttpContext.Request.Form, context.ModelState, "foo"));
		}

		[Test]
		public void Should_not_fetch_when_fetch_disabled()
		{
			var attribute = new DataBindAttribute() { Fetch = false };
			binder.Accept(attribute);
			binder.BindModel(controllerContext, context);

			validatingBinder.AssertWasCalled(x => x.UpdateFrom(Arg<TestEntity>.Is.Anything, Arg<NameValueCollection>.Is.Equal(controllerContext.HttpContext.Request.Form), Arg<ModelStateDictionary>.Is.Anything, Arg<string>.Is.Equal("foo")));
			repository.AssertWasNotCalled(x => x.GetById(3));
		}

		private class TestEntity
		{
			[Column(IsPrimaryKey = true)]
			public int Id { get; set; }
			public string Name { get; set; }
		}
	}
}