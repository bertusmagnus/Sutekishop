using System;
using Suteki.Shop.Repositories;
using NUnit.Framework;
using Moq;

namespace Suteki.Shop.Tests.Repositories
{
    [TestFixture]
    public class RepositoryTests
    {
        IRepository<User> userRepository;

        Mock<ShopDataContext> dataContextMock;
        ShopDataContext dataContext;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = new Mock<ShopDataContext>();
            dataContext = dataContextMock.Object;
            userRepository = new Repository<User>(dataContext);
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
    }
}
