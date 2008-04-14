using System;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryExtensionsTests
    {
        [Test]
        public void WhereEmailIs_ShouldReturnCorrectUserFromList()
        {
            Mock<Repository<User>> userRepositoryMock = MockUserRepositoryBuilder.Create();
            IRepository<User> userRepository = userRepositoryMock.Object;

            string email = "Henry@suteki.co.uk";
            User user = userRepository.GetAll().WhereEmailIs(email);

            Assert.IsNotNull(user);
            Assert.AreEqual(email, user.Email);
        }
    }
}
