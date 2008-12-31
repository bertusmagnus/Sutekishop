using NUnit.Framework;
using Rhino.Mocks;
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
            userRepository = MockRepository.GenerateStub<IRepository<User>>();
            userService = new UserService(userRepository);
        }

        [Test]
        public void CreateNewCustomer_ShouldReturnANewCustomerAddedToTheRepository()
        {
            userRepository.Expect(ur => ur.InsertOnSubmit(Arg<User>.Is.Anything));
            userRepository.Expect(ur => ur.SubmitChanges());

            User user = userService.CreateNewCustomer();

            Assert.IsNotNull(user, "returned user is null");
            Assert.IsTrue(user.IsCustomer, "returned user is not a customer");
        }
    }
}
