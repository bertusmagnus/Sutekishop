using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CmsControllerTests
    {
        CmsController cmsController;

        [SetUp]
        public void SetUp()
        {
            cmsController = new CmsController();
        }

        [Test]
        public void Index_ShouldRenderIndexView()
        {
            string urlName = "home";

            cmsController.Index(urlName)
                .ReturnsRenderViewResult()
                .ForView("Index");
        }
    }
}
