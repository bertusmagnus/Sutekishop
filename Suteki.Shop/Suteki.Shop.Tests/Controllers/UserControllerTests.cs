using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Tests.Repositories;
using System.Web;
using System.Collections.Specialized;
using System.Threading;
using System.Security.Principal;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        UserController userController;
        Mock<UserController> userControllerMock;
        ControllerTestContext testContext;

        Mock<Repository<User>> userRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the user controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { "Administrator" });

            userRepositoryMock = MockRepositoryBuilder.CreateUserRepository();
            Mock<Repository<Role>> roleRepositoryMock = MockRepositoryBuilder.CreateRoleRepository();

            userControllerMock = new Mock<UserController>(userRepositoryMock.Object, roleRepositoryMock.Object);
            userController = userControllerMock.Object;
            testContext = new ControllerTestContext(userController);

            // don't worry about encrypting passwords here, just stub out this call so that it has no effect
            // on the user
            userControllerMock.Expect(c => c.EncryptPassword(It.IsAny<User>()));
        }

        [Test]
        public void Index_ShouldShowAListOfUsers()
        {
            userController.Index();

            // should show view Index
            Assert.AreEqual("Index", testContext.ViewEngine.ViewContext.ViewName);

            // ViewData should be UserViewData
            UserViewData viewData = testContext.ViewEngine.ViewContext.ViewData as UserViewData;
            Assert.IsNotNull(viewData, "ViewData is not UserViewData");

            // there should be some users
            Assert.IsNotNull(viewData.Users, "ViewData.Users is null");

            // there should be three users
            Assert.AreEqual(3, viewData.Users.Count(), "ViewData.Users.Count() is not equal to 3");
        }

        [Test]
        public void New_ShouldDisplayUserEditView()
        {
            userController.New();

            // should show Edit view
            Assert.AreEqual("Edit", testContext.ViewEngine.ViewContext.ViewName);

            AssertUserEditViewDataIsCorrect();
        }

        [Test]
        public void Edit_ShouldSelectCorrectUser()
        {
            int userId = 23;

            User user = new User
            {
                UserId = userId,
                Email = "mike@mike.com",
                Password = "password",
                RoleId = 2
            };

            userRepositoryMock.Expect(ur => ur.GetById(userId)).Returns(user).Verifiable();

            userController.Edit(userId);

            AssertUserEditViewDataIsCorrect();
            userRepositoryMock.Verify();
        }

        private void AssertUserEditViewDataIsCorrect()
        {
            UserEditViewData viewData = testContext.ViewEngine.ViewContext.ViewData as UserEditViewData;
            Assert.IsNotNull(viewData, "ViewData is not UserEditViewData");

            // there should be some roles
            Assert.IsNotNull(viewData.Roles, "viewData.Roles is null");

            // add should have inserted a default user into the viewData
            Assert.IsNotNull(viewData.User, "viewData User is null");
        }

        [Test]
        public void Update_ShouldInsertNewUser()
        {
            string email = "blogs@blogs.com";
            string password = "bl0gs";
            int roleId = 3;
            bool isEnabled = false;

            // set up the request form
            NameValueCollection form = new NameValueCollection();
            form.Add("userId", "0");
            form.Add("email", email);
            form.Add("password", password);
            form.Add("roleid", roleId.ToString());
            form.Add("isenabled", isEnabled.ToString());

            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);

            // setup expectations on the userRepository
            User user = null;
            
            userRepositoryMock.Expect(ur => ur.InsertOnSubmit(It.IsAny<User>()))
                .Callback<User>(u => { user = u; })
                .Verifiable();
            
            userRepositoryMock.Expect(ur => ur.SubmitChanges())
                .Verifiable();

            // call Update
            userController.Update(0);

            // Assertions
            Assert.IsNotNull(user, "user is null");
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(password, user.Password);
            Assert.AreEqual(roleId, user.RoleId);
            Assert.AreEqual(isEnabled, user.IsEnabled);

            AssertUserEditViewDataIsCorrect();

            userRepositoryMock.Verify();
        }

        [Test]
        public void Update_ShouldUpdateExistingUser()
        {
            int userId = 34;
            string email = "blogs@blogs.com";
            string password = "bl0gs";
            int roleId = 3;
            bool isEnabled = false;

            // set up the request form
            NameValueCollection form = new NameValueCollection();
            form.Add("userId", userId.ToString());
            form.Add("email", email);
            form.Add("password", password);
            form.Add("roleid", roleId.ToString());
            form.Add("isenabled", isEnabled.ToString());

            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);

            // setup expectations on the userRepository
            User user = new User
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
            userController.Update(userId);

            // Assertions
            Assert.IsNotNull(user, "user is null");
            Assert.AreEqual(email, user.Email);
            Assert.AreEqual(password, user.Password);
            Assert.AreEqual(roleId, user.RoleId);
            Assert.AreEqual(isEnabled, user.IsEnabled);

            AssertUserEditViewDataIsCorrect();

            userRepositoryMock.Verify();

        }
    }
}
