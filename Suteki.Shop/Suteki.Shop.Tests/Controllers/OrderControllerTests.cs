using System;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        OrderController orderController;
        ControllerTestContext testContext;
        IRepository<Order> orderRepository;

        [SetUp]
        public void SetUp()
        {
            orderRepository = new Mock<IRepository<Order>>().Object;
            orderController = new Mock<OrderController>(orderRepository).Object;
            testContext = new ControllerTestContext(orderController);
        }

        [Test]
        public void Index_ShouldShowIndexViewWithCurrentOrder()
        {
            User user = CreateUserWithOrder();
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            orderController.Index();

            Assert.AreEqual("Index", testContext.ViewEngine.ViewContext.ViewName);
            ShopViewData viewData = testContext.ViewEngine.ViewContext.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");

            Assert.AreSame(user.Orders[0], viewData.Order, "The user's order has not been shown");
        }

        private static User CreateUserWithOrder()
        {
            User user = new User
            {
                Orders =
                {
                    new Order()
                }
            };
            return user;
        }

        [Test]
        public void Update_ShouldAddOrderLineToCurrentOrder()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("sizeid", "5");
            form.Add("quantity", "2");
            testContext.TestContext.RequestMock.ExpectGet(request => request.Form).Returns(form);

            User user = CreateUserWithOrder();
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            // expect 
            Mock.Get(orderRepository).Expect(or => or.SubmitChanges()).Verifiable();

            orderController.Update();

            Assert.AreEqual("Index", testContext.ViewEngine.ViewContext.ViewName);

            Assert.AreEqual(1, user.Orders[0].OrderItems.Count, "expected OrderItem is missing");
            Assert.AreEqual(5, user.Orders[0].OrderItems[0].SizeId);
            Assert.AreEqual(2, user.Orders[0].OrderItems[0].Quantity);
            
            Mock.Get(orderRepository).Verify();
        }
    }
}
