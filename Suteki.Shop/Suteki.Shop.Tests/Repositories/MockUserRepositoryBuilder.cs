using System.Collections.Generic;
using System.Linq;
using Moq;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Repositories
{
    public static class MockRepositoryBuilder
    {
        public static Mock<Repository<User>> CreateUserRepository()
        {
            Mock<ShopDataContext> dataContextMock = new Mock<ShopDataContext>();
            Mock<Repository<User>> userRepositoryMock = new Mock<Repository<User>>(dataContextMock.Object);

            List<User> users = new List<User>()
            {
                new User { UserId = 1, Email = "Henry@suteki.co.uk", Password = "henry1" },
                new User { UserId = 2, Email = "George@suteki.co.uk", Password = "george1" },
                new User { UserId = 3, Email = "Sky@suteki.co.uk", Password = "sky1" }
            };

            userRepositoryMock.Expect(ur => ur.GetAll()).Returns(() => users.AsQueryable());

            return userRepositoryMock;
        }

        public static Mock<Repository<Role>> CreateRoleRepository()
        {
            Mock<ShopDataContext> dataContextMock = new Mock<ShopDataContext>();
            Mock<Repository<Role>> roleRepositoryMock = new Mock<Repository<Role>>(dataContextMock.Object);

            List<Role> roles = new List<Role>
            {
                new Role { RoleId = 1, Name = "Administrator" },
                new Role { RoleId = 2, Name = "Order Processor" },
                new Role { RoleId = 3, Name = "Customer" },
                new Role { RoleId = 4, Name = "Guest" }
            };

            roleRepositoryMock.Expect(r => r.GetAll()).Returns(() => roles.AsQueryable());

            return roleRepositoryMock;
        }
    }
}
