using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.Models;
using Rhino.Mocks;

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
            orderRepository = MockRepository.GenerateStub<IRepository<Order>>();
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

            orderRepository.Expect(or => or.GetAll()).Return(orders);

            ContentResult result = reportController.Orders()
                .ReturnsContentResult();

            var returnedOrderCsvArray = result.Content.Split(',', '\n');
            var expectedOrderCsvArray = expectedOrdersCsv.Split(',', '\n');

            Assert.That(returnedOrderCsvArray.Length, Is.EqualTo(11), "CSV file not in expected format");
            Assert.That(returnedOrderCsvArray[0], Is.EqualTo(expectedOrderCsvArray[0]));
            Assert.That(returnedOrderCsvArray[1], Is.EqualTo(expectedOrderCsvArray[1]));
            Assert.That(returnedOrderCsvArray[2], Is.EqualTo(expectedOrderCsvArray[2]));

            // Date compare fails for different language setups.
            // Assert.That(returnedOrderCsvArray[3], Is.EqualTo(expectedOrderCsvArray[3]));
            
            Assert.That(returnedOrderCsvArray[4], Is.EqualTo(expectedOrderCsvArray[4]));

            Assert.That(returnedOrderCsvArray[5], Is.EqualTo(expectedOrderCsvArray[5]));
            Assert.That(returnedOrderCsvArray[6], Is.EqualTo(expectedOrderCsvArray[6]));
            Assert.That(returnedOrderCsvArray[7], Is.EqualTo(expectedOrderCsvArray[7]));

            // Date compare fails for different language setups.
            // Assert.That(returnedOrderCsvArray[8], Is.EqualTo(expectedOrderCsvArray[8]));
            
            Assert.That(returnedOrderCsvArray[9], Is.EqualTo(expectedOrderCsvArray[9]));

            Assert.That(result.ContentType, Is.EqualTo("text/csv"));
        }

        private const string expectedOrdersCsv = 
@"0,""mike@mike.com"",""Dispatched"",""18/10/2008x 00:00:00"",0
0,""mike@mike.com"",""Dispatched"",""18/10/2008x 00:00:00"",0
";

    }
}
