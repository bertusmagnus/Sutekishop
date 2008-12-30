using System;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.ViewData;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;
using Suteki.Shop.Tests.Repositories;
using Suteki.Shop.Tests.TestHelpers;
using System.Web.Mvc;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class LoginControllerTests
    {
        private Mock<Repository<User>> userRepository;
        private IUserService userService;

        private LoginController loginController;

        [SetUp]
        public void SetUp()
        {
            userRepository = MockRepositoryBuilder.CreateUserRepository();
            userService = new Mock<IUserService>().Object;

            loginController = new LoginController(userRepository.Object, userService);
        }

        [Test]
        public void Index_ShouldDisplayIndexView()
        {
            const string view = "Index";

            var result = loginController.Index() as ViewResult;

            Assert.AreEqual(view, result.ViewName);
        }

        [Test]
        public void Authenticate_ShouldAuthenticateValidUser()
        {
            const string email = "Henry@suteki.co.uk";
            const string password = "henry1";

            // should set cookie
            Mock.Get(userService).Expect(c => c.SetAuthenticationCookie(email)).Verifiable();

            var result = loginController.Authenticate(email, password) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.Values["action"]);
            Assert.AreEqual("Home", result.Values["controller"]);

            Mock.Get(userService).Verify();
        }

        [Test]
        public void Authenticate_ShouldNotAuthenticateInvalidUser()
        {
            const string email = "Henry@suteki.co.uk";
            const string password = "henry3";

            // throw if SetAuthenticationToken is called
            Mock.Get(userService).Expect(c => c.SetAuthenticationCookie(email))
                .Throws(new Exception("SetAuthenticationToken shouldn't be called"));

            loginController.Authenticate(email, password)
                .ReturnsViewResult()
                .ForView("Index")
                .AssertAreEqual<IErrorViewData, string>("Unknown email or password", vd => vd.ErrorMessage);
        }

        [Test]
        public void Logout_ShouldLogUserOut()
        {
            Mock.Get(userService).Expect(c => c.RemoveAuthenticationCookie()).Verifiable();

            var result = loginController.Logout() as RedirectToRouteResult;

            Assert.AreEqual("Index", result.Values["action"]);
            Assert.AreEqual("Home", result.Values["controller"]);

            Mock.Get(userService).Verify();
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
