using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Shop.Filters;

namespace Suteki.Shop.Tests.Filters
{
	[TestFixture]
	public class TransactionFilterTester
	{
		MockContext context;
		TransactionFilter filter;

		[SetUp]
		public void Setup()
		{
			context = new MockContext();
			var contextProvider = MockRepository.GenerateStub<IDataContextProvider>();
			contextProvider.Expect(x => x.DataContext).Return(context);
			filter = new TransactionFilter(contextProvider);
		}

		[Test]
		public void Changes_should_be_submitted_when_result_executed()
		{
			filter.OnResultExecuted(new ResultExecutedContext() { Controller = new TestController() });
			context.ChangesSubmitted.ShouldBeTrue();
		}

		[Test]
		public void Changes_should_not_be_submitted_if_there_are_errors_in_modelstate()
		{
			var controller = new TestController();
			controller.ModelState.AddModelError("foo", "bar");
			filter.OnResultExecuted(new ResultExecutedContext() { Controller = controller });
			context.ChangesSubmitted.ShouldBeFalse();
		}

		private class MockContext : DataContext
		{
			public bool ChangesSubmitted;

			public MockContext() : base("foo")
			{
			}

			public override void SubmitChanges(ConflictMode failureMode) 
			{
				ChangesSubmitted = true;
			}
		}

	}
}