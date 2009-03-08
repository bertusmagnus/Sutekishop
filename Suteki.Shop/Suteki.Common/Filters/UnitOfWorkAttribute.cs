using System.Web.Mvc;
using Suteki.Common.Repositories;

namespace Suteki.Common.Filters
{
	public class UnitOfWorkAttribute : FilterUsingAttribute
	{
		public UnitOfWorkAttribute() : base(typeof (UnitOfWorkFilter))
		{
		}
	}

	public class UnitOfWorkFilter : IActionFilter
	{
		private readonly IDataContextProvider provider;

		public UnitOfWorkFilter(IDataContextProvider provider)
		{
			this.provider = provider;
		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var context = provider.DataContext;

			if (filterContext.Controller.ViewData.ModelState.IsValid)
			{
				context.SubmitChanges();
			}
		}
	}
}