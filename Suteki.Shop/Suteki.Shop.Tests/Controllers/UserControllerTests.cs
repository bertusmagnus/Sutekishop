using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.Repositories;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the user controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] {"Administrator"});

            userRepositoryMock = MockRepositoryBuilder.CreateUserRepository();
            Mock<Repository<Role>> roleRepositoryMock = MockRepositoryBuilder.CreateRoleRepository();

            userControllerMock = new Mock<UserController>(userRepositoryMock.Object, roleRepositoryMock.Object);
            userController = userControllerMock.Object;
            testContext = new ControllerTestContext(userController);

            // don't worry about encrypting passwords here, just stub out this call so that it has no effect
            // on the user
            userControllerMock.Expect(c => c.EncryptPassword(It.IsAny<User>()));

            // setup the querystring to return an empty name value collection by default
            testContext.TestContext.Request.Expect(r => r.QueryString).Return(new NameValueCollection());
        }

        #endregion

        private UserController userController;
        private Mock<UserController> userControllerMock;
        private ControllerTestContext testContext;

        private Mock<Repository<User>> userRepositoryMock;

        private static void AssertUserEditViewDataIsCorrect(ViewResultBase result)
        {
            var viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "ViewData is not ShopViewData");

            // there should be some roles
            Assert.IsNotNull(viewData.Roles, "viewData.Roles is null");

            // add should have inserted a default user into the viewData
            Assert.IsNotNull(viewData.User, "viewData User is null");
        }

        [Test]
        public void Edit_ShouldSelectCorrectUser()
        {
            const int userId = 23;

            var user = new User
            {
                UserId = userId,
                Email = "mike@mike.com",
                Password = "password",
                RoleId = 2
            };

            userRepositoryMock.Expect(ur => ur.GetById(userId)).Returns(user).Verifiable();

            var result = userController.Edit(userId) as ViewResult;

            AssertUserEditViewDataIsCorrect(result);
            userRepositoryMock.Verify();
        }

        [Test]
        public void Index_ShouldShowAListOfUsers()
        {
            var result = userController.Index() as ViewResult;

            // should show view Index
            Assert.AreEqual("Index", result.ViewName);

            // ViewData should be UserViewData
            var viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "ViewData is not ShopViewData");

            // there should be some users
            Assert.IsNotNull(viewData.Users, "ViewData.Users is null");

            // there should be three users
            Assert.AreEqual(3, viewData.Users.Count(), "ViewData.Users.Count() is not equal to 3");
        }

        [Test]
        public void New_ShouldDisplayUserEditView()
        {
            var result = userController.New() as ViewResult;

            // should show Edit view
            Assert.AreEqual("Edit", result.ViewName);

            AssertUserEditViewDataIsCorrect(result);
        }

        [Test]
        public void Update_ShouldInsertNewUser()
        {
            const string email = "blogs@blogs.com";
            const string password = "bl0gs";
            const int roleId = 3;
            const bool isEnabled = false;

            // set up the request form
            var form = new NameValueCollection
            {
                {"userId", "0"},
                {"email", email},
                {"password", password},
                {"roleid", roleId.ToString()},
                {"isenabled", "false"}
            };

            testContext.TestContext.Request.Expect(r => r.Form).Return(form);

            // setup expectations on the userRepository
            User user = null;

            userRepositoryMock.Expect(ur => ur.InsertOnSubmit(It.IsAny<User>()))
                .Callback<User>(u => { user = u; })
                .Verifiable();

            userRepositoryMock.Expect(ur => ur.SubmitChanges())
                .Verifiable();

            // call Update
            var result = userController.Update(0) as ViewResult;

            // Assertions
            Assert.IsNotNull(user, "user is null");
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(password, user.Password);
            Assert.AreEqual(roleId, user.RoleId);
            Assert.AreEqual(isEnabled, user.IsEnabled);

            AssertUserEditViewDataIsCorrect(result);

            userRepositoryMock.Verify();
        }

        [Test]
        public void Update_ShouldUpdateExistingUser()
        {
            const int userId = 34;
            const string email = "blogs@blogs.com";
            const string password = "bl0gs";
            const int roleId = 3;
            const bool isEnabled = false;

            // set up the request form
            var form = new NameValueCollection
            {
                {"userId", userId.ToString()},
                {"email", email},
                {"password", password},
                {"roleid", roleId.ToString()},
                {"isenabled", isEnabled.ToString()}
            };

            testContext.TestContext.Request.Expect(r => r.Form).Return(form);

            // setup expectations on the userRepository
            var user = new User
            {
                UserId = userId,
                Email = "old@old.com",
                Password = "oldpassword",
                RoleId = 1,
                IsEnabled = true
            };

            userRepositoryMock.Expect(ur => ur.GetById(userId))
                .Returns(user)
                .Verifiable();

            userRepositoryMock.Expect(ur => ur.SubmitChanges())
                .Verifiable();

            // call Update
            var result = userController.Update(userId) as ViewResult;

            // Assertions
            Assert.IsNotNull(user, "user is null");
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(password, user.Password);
            Assert.AreEqual(roleId, user.RoleId);
            Assert.AreEqual(isEnabled, user.IsEnabled);

            AssertUserEditViewDataIsCorrect(result);

            userRepositoryMock.Verify();
        }
    }
}