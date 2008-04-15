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
                new User { UserId = 1, Email = "Henry@suteki.co.uk", 
                    Password = "6C80B78681161C8349552872CFA0739CF823E87B", IsEnabled = true }, // henry1

                new User { UserId = 2, Email = "George@suteki.co.uk", 
                    Password = "DC25F9DC0DF2BE9E6A83E6F0B26F4B41F57ADF6D", IsEnabled = true }, // george1

                new User { UserId = 3, Email = "Sky@suteki.co.uk", 
                    Password = "980BC222DA7FDD0D37BE816D60084894124509A1", IsEnabled = true } // sky1
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
