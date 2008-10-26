using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;
using Suteki.Common.Repositories;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.Models;
using Suteki.Shop.Tests.TestHelpers;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class ReportControllerTests
    {
        private ReportController reportController;
        private IRepository<Order> orderRepository;
        
        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the report controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            orderRepository = new Mock<IRepository<Order>>().Object;

            reportController = new ReportController(orderRepository);
        }

        [Test]
        public void Index_ShouldShowIndexView()
        {
            reportController.Index()
                .ReturnsViewResult()
                .ForView("Index");
        }

        [Test]
        public void Orders_ShouldReturnACsvFileOfOrders()
        {
            IQueryable<Order> orders = new List<Order>
                                           {
                                               OrderTests.Create350GramOrder(),
                                               OrderTests.Create450GramOrder()
                                           }.AsQueryable();

            Mock.Get(orderRepository).Expect(or => or.GetAll()).Returns(orders);

            ContentResult result = reportController.Orders()
                .ReturnsContentResult();

            //Console.WriteLine(result.Content);

            Assert.That(result.Content, Is.EqualTo(expectedOrdersCsv));
            Assert.That(result.ContentType, Is.EqualTo("text/csv"));
        }

        private const string expectedOrdersCsv = 
@"0,""mike@mike.com"",""Dispatched"",""18/10/2008 00:00:00"",0
0,""mike@mike.com"",""Dispatched"",""18/10/2008 00:00:00"",0
";

    }
}
