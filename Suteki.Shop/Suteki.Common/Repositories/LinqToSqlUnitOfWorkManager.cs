using System;

namespace Suteki.Common.Repositories
{
	public class LinqToSqlUnitOfWorkManager : IUnitOfWorkManager
	{
		private readonly IDataContextProvider contextProvider;

		public LinqToSqlUnitOfWorkManager(IDataContextProvider contextProvider)
		{
			this.contextProvider = contextProvider;
		}


		public void Commit()
		{
			contextProvider.DataContext.SubmitChanges();
		}
	}
}