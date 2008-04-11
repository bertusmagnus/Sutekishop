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
        IRepository<User> userRepository;

        Mock<ShopDataContext> dataContextMock;
        ShopDataContext dataContext;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = new Mock<ShopDataContext>();
            dataContext = dataContextMock.Object;
            userRepositoryMock = new Mock<Repository<User>>(dataContext);
            userRepository = userRepositoryMock.Object;
        }

        [Test]
        public void GetByIdShouldReturnCorrectUser()
        {
            int userId = 1; // a user we know exists in the database

            User user = userRepository.GetById(userId);

            Assert.AreEqual(userId, user.UserId);
        }

        [Test]
        public void GetByIdShouldReturnCorrectRole()
        {
            int roleId = 2; // a role we know exists in the database

            IRepository<Role> roleRepository = new Repository<Role>(dataContext);
            Role role = roleRepository.GetById(roleId);

            Assert.AreEqual(roleId, role.RoleId);
        }

        [Test]
        public void CanMockGetAll()
        {
            Mock<IRepository<User>> userRepositoryMock = new Mock<IRepository<User>>();

            List<User> users = new List<User>()
            {
                new User { UserId = 1 },
                new User { UserId = 2 },
                new User { UserId = 3 }
            };

            userRepositoryMock.Expect(ur => ur.GetAll()).Returns(() => users.AsQueryable());

            User user2 = userRepositoryMock.Object.GetAll().Where(u => u.UserId == 2).Single();

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
