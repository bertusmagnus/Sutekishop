using System;
using System.Linq;
using Suteki.Shop.Repositories;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Data.Linq;

namespace Suteki.Shop.Tests.Repositories
{
    [TestFixture]
    public class RepositoryTests
    {
        Mock<Repository<User>> userRepositoryMock;
        Repository<User> userRepository;

        [SetUp]
        public void SetUp()
        {
            userRepositoryMock = MockRepositoryBuilder.CreateUserRepository();
            userRepository = userRepositoryMock.Object;
        }

        [Test]
        public void GetByIdShouldReturnCorrectUser()
        {
            int userId = 3; // a user we know exists in the database

            User user = userRepository.GetById(userId);
            Assert.AreEqual(userId, user.UserId);
        }

        [Test]
        public void CanMockGetAll()
        {
            User user2 = userRepository.GetAll().Where(u => u.UserId == 2).Single();
            Assert.AreEqual(2, user2.UserId);
        }

        [Test]
        public void InsertOnSubmitShouldCallSameOnTable()
        {
            User user = new User();

            Mock<ITable> userTableMock = new Mock<ITable>();

            userRepositoryMock.Expect(ur => ur.GetTable()).Returns(userTableMock.Object);
            userTableMock.Expect(ut => ut.InsertOnSubmit(user));
            
            userRepository.InsertOnSubmit(user);
        }

        [Test]
        public void DeleteOnSubmitShouldCallSameOnTable()
        {
            User user = new User();

            Mock<ITable> userTableMock = new Mock<ITable>();

            userRepositoryMock.Expect(ur => ur.GetTable()).Returns(userTableMock.Object);
            userTableMock.Expect(ut => ut.DeleteOnSubmit(user));

            userRepository.DeleteOnSubmit(user);
        }

    }
}
