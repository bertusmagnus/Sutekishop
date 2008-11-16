namespace Suteki.Common.Repositories
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        public string ConnectionString { get; private set; }

        public ConnectionStringProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
