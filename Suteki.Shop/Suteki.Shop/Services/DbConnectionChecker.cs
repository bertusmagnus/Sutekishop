using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Suteki.Common.Repositories;

namespace Suteki.Shop.Services
{
    public class DbConnectionChecker : IDbConnectionChecker
    {
        private readonly IConnectionStringProvider[] connectionStringProviders;

        public DbConnectionChecker(params IConnectionStringProvider[] connectionStringProviders)
        {
            this.connectionStringProviders = connectionStringProviders;
        }

        public void CheckConnection()
        {
            var messages = new List<string>();
            foreach(var connectionStringProvider in connectionStringProviders)
            {
                try
                {
                    var connection = new SqlConnection(connectionStringProvider.ConnectionString);
                    connection.Open();
                }
                catch (SqlException sqlException)
                {
                    var message = string.Format(
                        "Could not open database using connection string: '{0}'\nError was:\n{1}", 
                        connectionStringProvider.ConnectionString,
                        sqlException.Message);
                    messages.Add(message);
                }
            }
            if(messages.Count > 0)
            {
                throw new DbConnectionException(string.Join("\n", messages.ToArray()));
            }
        }
    }

    [Serializable]
    public class DbConnectionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DbConnectionException()
        {
        }

        public DbConnectionException(string message) : base(message)
        {
        }

        public DbConnectionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DbConnectionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}