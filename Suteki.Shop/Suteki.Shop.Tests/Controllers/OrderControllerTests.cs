using System.Security.Principal;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.TestHelpers;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using System.Collections.Generic;
using System.Collections.Specialized;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        private OrderController orderController;

        private IRepository<Order> orderRepository;
        private IRepository<Country> countryRepository;
        private IRepository<CardType> cardTypeRepository;

        private IEncryptionService encryptionService;
        private IPostageService postageService;
        private IUserService userService;
		IOrderSearchService searchService;

        private ControllerTestContext testContext;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the order controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            orderRepository = MockRepository.GenerateStub<IRepository<Order>>();
            countryRepository = MockRepository.GenerateStub<IRepository<Country>>();
            cardTypeRepository = MockRepository.GenerateStub<IRepository<CardType>>();
			

            encryptionService = MockRepository.GenerateStub<IEncryptionService>();
            postageService = MockRepository.GenerateStub<IPostageService>();
            userService = MockRepository.GenerateStub<IUserService>();
			searchService = MockRepository.GenerateStub<IOrderSearchService>();

            var mocks = new MockRepository();
            orderController = new OrderController(
                orderRepository,
                countryRepository,
                cardTypeRepository,
                encryptionService,
                postageService,
                userService,
				searchService
				);

            testContext = new ControllerTestContext(orderController);

            postageService.Expect(ps => ps.CalculatePostageFor(Arg<Order>.Is.Anything));

            userService.Expect(us => us.CurrentUser).Return(new User { UserId = 4, RoleId = Role.AdministratorId });

            testContext.TestContext.Context.User = new User { UserId = 4 };
            testContext.TestContext.Request.RequestType = "GET";
            testContext.TestContext.Request.Stub(r => r.QueryString).Return(new NameValueCollection());
            testContext.TestContext.Request.Stub(r => r.Form).Return(new NameValueCollection());

            mocks.ReplayAll();
        }



        [Test]
        public void Index_ShouldDisplayAListOfOrders()
        {
            var orders = new PagedList<Order>(new List<Order>(), 1, 1);
			searchService.Expect(x => x.PerformSearch(null)).IgnoreArguments().Return(orders);

            orderController.Index(null)
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>()
                .AssertAreSame(orders, vd => vd.Orders);
        }



        [Test]
        public void Index_ShouldBuildCriteriaAndExecuteSearch()
        {
			var criteria = new OrderSearchCriteria();
        	var results = new PagedList<Order>(new List<Order>(), 1, 1);
			searchService.Expect(x => x.PerformSearch(criteria)).Return(results);

			orderController.Index(criteria)
				.ReturnsViewResult()
				.ForView("Index")
				.WithModel<ShopViewData>()
				.AssertAreSame(criteria, vd => vd.OrderSearchCriteria)
				.AssertAreSame(results, vd => vd.Orders);

        }

        [Test]
        public void ShowCard_ShouldDecryptCardAndShowOrder()
        {
            const int orderId = 10;
            const string privateKey = "abcd";

            var order = new Order
            {
                Card = new Card
                {
                    CardTypeId = 1,
                    Holder = "Jon Anderson",
                    IssueNumber = "",
                    StartMonth = 1,
                    StartYear = 2004,
                    ExpiryMonth = 3,
                    ExpiryYear = 2010
                },
                Basket = new Basket(),
            };
            order.Card.SetEncryptedNumber("asldfkjaslfjdslsdjkfjflkdjdlsakj");
            order.Card.SetEncryptedSecurityCode("asldkfjsadlfjdskjfdlkd");

            orderRepository.Stub(or => or.GetById(orderId)).Return(order);

            orderController.ShowCard(orderId, privateKey)
                .ReturnsViewResult()
                .ForView("Item")
                .WithModel<ShopViewData>()
                .AssertAreEqual(order.Card.Number, vd => vd.Card.Number)
                .AssertAreEqual(order.Card.ExpiryYear, vd => vd.Card.ExpiryYear);

            encryptionService.AssertWasCalled(es => es.DecryptCard(Arg<Card>.Is.Anything));
        }
    }
}
