using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Suteki.Shop.Components.LoginStatus;
using Moq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Suteki.Shop.Tests.Components
{
    [TestFixture]
    public class LoginStatusComponentControllerTests
    {
        LoginStatusComponentController loginStatus;
        Mock<LoginStatusComponentController> loginStatusMock;
        MoqComponentControllerTestContext testContext;

        [SetUp]
        public void SetUp()
        {
            loginStatusMock = new Mock<LoginStatusComponentController>();
            loginStatus = loginStatusMock.Object;
            testContext = new MoqComponentControllerTestContext(loginStatus);
        }

        [Test]
        public void Index_ShouldRenderViewIndex()
        {
            loginStatusMock.Expect(c => c.RenderView("Index"));

            loginStatus.Index();

            loginStatusMock.VerifyAll();
        }
    }
}
