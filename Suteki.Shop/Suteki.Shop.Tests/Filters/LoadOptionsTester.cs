using System.Data.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Shop.Filters;
using Suteki.Shop.Models;

namespace Suteki.Shop.Tests.Filters
{
	[TestFixture]
	public class LoadOptionsTester
	{
		private LoadUsingAttribute attribute;
		private DataContext context;
		private LoadUsingFilter filter;

		[SetUp]
		public void Setup()
		{
			attribute = new LoadUsingAttribute(typeof(TestLoadOptions));
			context = new DataContext("foo");
			var contextProvider = MockRepository.GenerateStub<IDataContextProvider>();
			contextProvider.Expect(x => x.DataContext).Return(context);
			filter = new LoadUsingFilter(contextProvider);
		}

		[Test]
		public void Should_store_types()
		{
			filter.Accept(attribute);
			filter.LoadOptions.Length.ShouldEqual(1);
			filter.LoadOptions[0].ShouldBe<TestLoadOptions>();
		}

		[Test]
		public void Should_set_loadoptions_on_context()
		{
			context.LoadOptions.ShouldBeNull();
			filter.Accept(attribute);
			filter.OnAuthorization(new AuthorizationContext());
			context.LoadOptions.ShouldNotBeNull();
		}

		private class TestLoadOptions : ILoadOptions
		{
			public void Build(DataLoadOptions options)
			{
			}
		}
	}
}