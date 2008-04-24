using System;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using Suteki.Shop.Repositories;
using System.Web.Mvc;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        OrderController orderController;
        ControllerTestContext testContext;
        
        IRepository<Order> orderRepository;
        IRepository<OrderItem> orderItemRepository;
        IRepository<User> userRepository;
        IUserService userService;

        [SetUp]
        public void SetUp()
        {
            orderRepository = new Mock<IRepository<Order>>().Object;
            orderItemRepository = new Mock<IRepository<OrderItem>>().Object;
            userRepository = new Mock<IRepository<User>>().Object;
            userService = new Mock<IUserService>().Object;

            orderController = new Mock<OrderController>(
                orderRepository, 
                orderItemRepository, 
                userRepository,
                userService).Object;
            testContext = new ControllerTestContext(orderController);
        }

        [Test]
        public void Index_ShouldShowIndexViewWithCurrentOrder()
        {
            User user = CreateUserWithOrder();
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            RenderViewResult result = orderController.Index() as RenderViewResult;

            Assert.AreEqual("Index", result.ViewName);
            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");

            Assert.AreSame(user.Orders[0], viewData.Order, "The user's order has not been shown");
        }

        private User CreateUserWithOrder()
        {
            User user = new User
            {
                RoleId = Role.GuestId,
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
            Mock.Get(userService).Expect(us => us.CreateNewCustomer()).Returns(user).Verifiable();
            Mock.Get(orderController).Expect(oc => oc.SetAuthenticationCookie(user.Email)).Verifiable();
            Mock.Get(orderController).Expect(oc => oc.SetContextUserTo(user)).Verifiable();

            RenderViewResult result = orderController.Update() as RenderViewResult;

            Assert.AreEqual("Index", result.ViewName);

            Assert.AreEqual(1, user.Orders[0].OrderItems.Count, "expected OrderItem is missing");
            Assert.AreEqual(5, user.Orders[0].OrderItems[0].SizeId);
            Assert.AreEqual(2, user.Orders[0].OrderItems[0].Quantity);
            
            Mock.Get(orderRepository).Verify();
            Mock.Get(orderController).Verify();
            Mock.Get(userService).Verify();
        }

        [Test]
        public void Remove_ShouldRemoveItemFromOrder()
        {
            int orderItemIdToRemove = 3;

            User user = CreateUserWithOrder();
            OrderItem orderItem = new OrderItem { OrderItemId = orderItemIdToRemove };
            user.Orders[0].OrderItems.Add(orderItem);
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            // expect 
            Mock.Get(orderItemRepository).Expect(ir => ir.DeleteOnSubmit(orderItem)).Verifiable();
            Mock.Get(orderItemRepository).Expect(ir => ir.SubmitChanges());

            RenderViewResult result = orderController.Remove(orderItemIdToRemove) as RenderViewResult;

            Assert.AreEqual("Index", result.ViewName);
            Mock.Get(orderItemRepository).Verify();
        }

        [Test]
        public void Checkout_ShouldDisplayCheckoutForm()
        {
            int orderId = 444;

            Order order = new Order();

            // expectations
            Mock.Get(orderRepository).Expect(or => or.GetById(orderId)).Returns(order);

            // exercist method
            RenderViewResult result = orderController.Checkout(orderId) as RenderViewResult;

            // assertions
            Assert.AreEqual("Checkout", result.ViewName, "ViewName is incorrect");
            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.AreSame(order, viewData.Order, "order has not been passed to view");
        }
    }
}
