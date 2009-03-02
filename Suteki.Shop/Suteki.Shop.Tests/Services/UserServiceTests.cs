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
    	IFormsAuthentication formsAuthentication;

    	[SetUp]
        public void SetUp()
        {
            userRepository = MockRepository.GenerateStub<IRepository<User>>();
        	formsAuthentication = MockRepository.GenerateStub<IFormsAuthentication>();
            userService = new UserService(userRepository, formsAuthentication);
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

    	[Test]
    	public void HashPassword_should_delegate_to_formsAuthentication()
    	{
    		formsAuthentication.Expect(x => x.HashPasswordForStoringInConfigFile("foo")).Return("bar");
    		userService.HashPassword("foo").ShouldEqual("bar");
    	}

    	[Test]
    	public void RemoveAuthenticationCookie_should_delegate_to_formsAuthentication()
    	{
    		userService.RemoveAuthenticationCookie();
			formsAuthentication.AssertWasCalled(x => x.SignOut());
    	}

    	[Test]
    	public void SetAuthenticationCookie_should_delegate_to_formsAuthentication()
    	{
    		userService.SetAuthenticationCookie("foo@foo.com");
			formsAuthentication.AssertWasCalled(x => x.SetAuthCookie("foo@foo.com", true));
    	}
    }
}
