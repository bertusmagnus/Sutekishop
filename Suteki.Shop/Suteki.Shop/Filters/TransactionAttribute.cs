using System;
using System.Web.Mvc;
using Suteki.Common.Repositories;

namespace Suteki.Shop.Filters
{
	public class TransactionAttribute : FilterUsingAttribute
	{
		public TransactionAttribute() : base(typeof(TransactionFilter))
		{
		}
	}

	public class TransactionFilter : IResultFilter
	{
		private readonly IDataContextProvider provider;

		public TransactionFilter(IDataContextProvider provider)
		{
			this.provider = provider;
		}

		public void OnResultExecuting(ResultExecutingContext filterContext)
		{
		}

		public void OnResultExecuted(ResultExecutedContext filterContext)
		{
			var context = provider.DataContext;

            if(filterContext.Controller.ViewData.ModelState.IsValid)
            {
				context.SubmitChanges();            	
            }
		}
	}
}