using System.Data.Linq;

namespace Suteki.Common.Repositories
{
    public class DataContextProvider : IDataContextProvider
    {
        private readonly DataContext dataContext;

        public IConnectionStringProvider ConnectionStringProvider { get; private set; }

        public DataContextProvider(IConnectionStringProvider connectionStringProvider)
        {
            ConnectionStringProvider = connectionStringProvider;
            dataContext = new DataContext(ConnectionStringProvider.ConnectionString);
        }

        public DataContext DataContext
        {
            get
            {
                return dataContext;
            }
        }
    }
}
