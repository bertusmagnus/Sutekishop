using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        IUserService userService;
        IRepository<User> userRepository;

        [SetUp]
        public void SetUp()
        {
            userRepository = new Mock<IRepository<User>>().Object;
            userService = new UserService(userRepository);
        }

        [Test]
        public void CreateNewCustomer_ShouldReturnANewCustomerAddedToTheRepository()
        {
            Mock.Get(userRepository).Expect(ur => ur.InsertOnSubmit(It.IsAny<User>()));
            Mock.Get(userRepository).Expect(ur => ur.SubmitChanges());

            User user = userService.CreateNewCustomer();

            Assert.IsNotNull(user, "returned user is null");
            Assert.IsTrue(user.IsCustomer, "returned user is not a customer");
        }
    }
}
