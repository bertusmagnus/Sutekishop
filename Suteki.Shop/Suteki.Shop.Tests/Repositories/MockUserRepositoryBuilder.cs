using System.Collections.Generic;
using System.Linq;
using Moq;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Repositories
{
    public static class MockUserRepositoryBuilder
    {
        public static Mock<Repository<User>> Create()
        {
            Mock<ShopDataContext> dataContextMock = new Mock<ShopDataContext>();
            Mock<Repository<User>> userRepositoryMock = new Mock<Repository<User>>(dataContextMock.Object);

            List<User> users = new List<User>()
            {
                new User { UserId = 1, Email = "Henry@suteki.co.uk" },
                new User { UserId = 2, Email = "George@suteki.co.uk" },
                new User { UserId = 3, Email = "Sky@suteki.co.uk" }
            };

            userRepositoryMock.Expect(ur => ur.GetAll()).Returns(() => users.AsQueryable());

            return userRepositoryMock;
        }
    }
}
