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

        [Test]
        public void Should_throw_a_nice_exception_when_repository_throws_a_SQL_exception()
        {

            try
            {
                dbConnectionChecker.CheckConnection();
            }
            catch (DbConnectionException exception)
            {
                Assert.That(exception.Message.Replace("\r\n", "\n"), Is.EqualTo(expectedMessage.Replace("\r\n", "\n")));
            }
        }

        private const string expectedMessage =
@"Could not open database using connection string: 'Data Source=.\SQLEXPRESS;Initial Catalog=SutekiShopX;Integrated Security=True'
Error was:
Cannot open database ""SutekiShopX"" requested by the login. The login failed.
Login failed for user 'TINTIN\Mike'.
Could not open database using connection string: 'Data Source=.\SQLEXPRESS;Initial Catalog=SomeOtherNonExistantDatabase;Integrated Security=True'
Error was:
Cannot open database ""SomeOtherNonExistantDatabase"" requested by the login. The login failed.
Login failed for user 'TINTIN\Mike'.";
    }
}