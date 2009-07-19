using System;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class DbConnectionCheckerTests
    {
        private IDbConnectionChecker dbConnectionChecker;

        [SetUp]
        public void SetUp()
        {
            // an incorrect connection string
            var connectionStringProvider1 = 
                new ConnectionStringProvider(@"Data Source=.\SQLEXPRESS;Initial Catalog=SutekiShopX;Integrated Security=True");
            var connectionStringProvider2 = 
                new ConnectionStringProvider(@"Data Source=.\SQLEXPRESS;Initial Catalog=SomeOtherNonExistantDatabase;Integrated Security=True");
            dbConnectionChecker = new DbConnectionChecker(connectionStringProvider1, connectionStringProvider2);
        }

        [Test, ExpectedException(typeof(DbConnectionException))]
        public void Should_throw_a_nice_exception_when_repository_throws_a_SQL_exception()
        {
            dbConnectionChecker.CheckConnection();
        }
    }
}