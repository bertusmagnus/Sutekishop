using System;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.Repositories;
using Suteki.Shop.Tests.Repositories;
using Suteki.Shop.ViewData;

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

            loginController.Index();

            Assert.AreEqual(view, testContext.ViewEngine.ViewContext.ViewName);
        }

        [Test]
        public void Authenticate_ShouldAuthenticateValidUser()
        {
            string email = "Henry@suteki.co.uk";
            string password = "henry1";

            // should set cookie
            loginControllerMock.Expect(c => c.SetAuthenticationCookie(email)).Verifiable();

            // should redirect to home->index
            loginControllerMock.Expect(c => c.RedirectToAction2("Index", "Home")).Verifiable();

            loginController.Authenticate(email, password);

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

            loginController.Authenticate(email, password);

            // should show view Index
            Assert.AreEqual("Index", testContext.ViewEngine.ViewContext.ViewName);

            // Should show error message
            Assert.AreEqual("Unknown email or password", 
                ((IErrorViewData)testContext.ViewEngine.ViewContext.ViewData).ErrorMessage);
        }

        [Test]
        public void Logout_ShouldLogUserOut()
        {
            loginControllerMock.Expect(c => c.RemoveAuthenticationCookie()).Verifiable();
            loginControllerMock.Expect(c => c.RedirectToAction2("Index", "Home")).Verifiable();

            loginController.Logout();

            loginControllerMock.Verify();
        }
    }
}
