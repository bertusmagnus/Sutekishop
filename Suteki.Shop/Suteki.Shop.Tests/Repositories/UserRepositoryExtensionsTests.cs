using System;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryExtensionsTests
    {
        Mock<Repository<User>> userRepositoryMock;
        IRepository<User> userRepository;

        [SetUp]
        public void SetUp()
        {
            userRepositoryMock = MockRepositoryBuilder.CreateUserRepository();
            userRepository = userRepositoryMock.Object;
        }

        [Test]
        public void WhereEmailIs_ShouldReturnCorrectUserFromList()
        {
            string email = "Henry@suteki.co.uk";
            User user = userRepository.GetAll().WhereEmailIs(email);

            Assert.IsNotNull(user);
            Assert.AreEqual(email, user.Email);
        }

        [Test]
        public void ContainsUser_ShouldReturnTrueForExistingUser()
        {
            string email = "Henry@suteki.co.uk";
            string password = "6C80B78681161C8349552872CFA0739CF823E87B";

            Assert.IsTrue(userRepository.GetAll().ContainsUser(email, password));
        }

        [Test]
        public void ContainsUser_ShouldReturnFalseForIncorrectPassword()
        {
            string email = "Henry@suteki.co.uk";
            string password = "xyz";

            Assert.IsFalse(userRepository.GetAll().ContainsUser(email, password));
        }

    }
}
