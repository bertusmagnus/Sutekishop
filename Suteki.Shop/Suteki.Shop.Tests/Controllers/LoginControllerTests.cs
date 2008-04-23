using System;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.Repositories;
using Suteki.Shop.Tests.Repositories;
using Suteki.Shop.ViewData;
using System.Web.Mvc;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class LoginControllerTests
    {
        Mock<LoginController> loginControllerMock;
        LoginController loginController;
        ControllerTestContext testContext;

        Mock<Repository<User>> userRepository;

        [SetUp]
        public void SetUp()
        {
            userRepository = MockRepositoryBuilder.CreateUserRepository();

            loginControllerMock = new Mock<LoginController>(userRepository.Object);
            loginController = loginControllerMock.Object;
            testContext = new ControllerTestContext(loginController);
        }

        [Test]
        public void Index_ShouldDisplayIndexView()
        {
            string view = "Index";

            RenderViewResult result = loginController.Index() as RenderViewResult;

            Assert.AreEqual(view, result.ViewName);
        }

        [Test]
        public void Authenticate_ShouldAuthenticateValidUser()
        {
            string email = "Henry@suteki.co.uk";
            string password = "henry1";

            // should set cookie
            loginControllerMock.Expect(c => c.SetAuthenticationCookie(email)).Verifiable();

            ActionRedirectResult result = loginController.Authenticate(email, password) as ActionRedirectResult;

            Assert.AreEqual("Index", result.Values["action"]);
            Assert.AreEqual("Home", result.Values["controller"]);

            loginControllerMock.Verify();
        }

        [Test]
        public void Authenticate_ShouldNotAuthenticateInvalidUser()
        {
            string email = "Henry@suteki.co.uk";
            string password = "henry3"; // invalid password

            // throw if SetAuthenticationToken is called
            loginControllerMock.Expect(c => c.SetAuthenticationCookie(email))
                .Throws(new Exception("SetAuthenticationToken shouldn't be called"));

            RenderViewResult result = loginController.Authenticate(email, password) as RenderViewResult;

            // should show view Index
            Assert.AreEqual("Index", result.ViewName);

            // Should show error message
            Assert.AreEqual("Unknown email or password",
                ((IErrorViewData)result.ViewData).ErrorMessage);
        }

        [Test]
        public void Logout_ShouldLogUserOut()
        {
            loginControllerMock.Expect(c => c.RemoveAuthenticationCookie()).Verifiable();

            ActionRedirectResult result = loginController.Logout() as ActionRedirectResult;

            Assert.AreEqual("Index", result.Values["action"]);
            Assert.AreEqual("Home", result.Values["controller"]);

            loginControllerMock.Verify();
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
