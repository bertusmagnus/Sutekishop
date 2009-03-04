using System.Data;
using System.Data.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Tests.TestHelpers;

namespace Suteki.Common.Tests.Filters
{
	[TestFixture]
	public class UnitOfWorkFilterTester
	{
		MockContext context;
		UnitOfWorkFilter filter;

		[SetUp]
		public void Setup()
		{
			context = new MockContext();
			var contextProvider = MockRepository.GenerateStub<IDataContextProvider>();
			contextProvider.Expect(x => x.DataContext).Return(context);
			filter = new UnitOfWorkFilter(contextProvider);
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