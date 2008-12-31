using System;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
using Suteki.Common.ViewData;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;
using Suteki.Shop.Tests.Repositories;
using Rhino.Mocks;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class LoginControllerTests
    {
        private IRepository<User> userRepository;
        private IUserService userService;

        private LoginController loginController;

        [SetUp]
        public void SetUp()
        {
            userRepository = MockRepositoryBuilder.CreateUserRepository();
            userService = MockRepository.GenerateStub<IUserService>();

            loginController = new LoginController(userRepository, userService);
        }

        [Test]
        public void Index_ShouldDisplayIndexView()
        {
            const string view = "Index";

            loginController.Index()
                .ReturnsViewResult()
                .ForView(view);
        }

        [Test]
        public void Authenticate_ShouldAuthenticateValidUser()
        {
            const string email = "Henry@suteki.co.uk";
            const string password = "henry1";

            // should set cookie
            userService.Expect(c => c.SetAuthenticationCookie(email));

            loginController.Authenticate(email, password)
                .ReturnRedirectToRouteResult()
                .ToAction("Index")
                .ToController("Home");

            userService.VerifyAllExpectations();
        }

        [Test]
        public void Authenticate_ShouldNotAuthenticateInvalidUser()
        {
            const string email = "Henry@suteki.co.uk";
            const string password = "henry3";

            // throw if SetAuthenticationToken is called
            userService.Expect(c => c.SetAuthenticationCookie(email))
                .Throw(new Exception("SetAuthenticationToken shouldn't be called"));

            loginController.Authenticate(email, password)
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<IErrorViewData>()
                .AssertAreEqual("Unknown email or password", vd => vd.ErrorMessage);
        }

        [Test]
        public void Logout_ShouldLogUserOut()
        {
            userService.Expect(c => c.RemoveAuthenticationCookie());

            loginController.Logout()
                .ReturnRedirectToRouteResult()
                .ToAction("Index")
                .ToController("Home");

            userService.VerifyAllExpectations();
        }

        [Test, Explicit]
        public void EncryptPasswordSpike()
        {
            Console.WriteLine(loginController.EncryptPassword("henry1"));   // 6C80B78681161C8349552872CFA0739CF823E87B
            Console.WriteLine(loginController.EncryptPassword("george1"));  // DC25F9DC0DF2BE9E6A83E6F0B26F4B41F57ADF6D
            Console.WriteLine(loginController.EncryptPassword("sky1"));     // 980BC222DA7FDD0D37BE816D60084894124509A1
        }
    }
}
