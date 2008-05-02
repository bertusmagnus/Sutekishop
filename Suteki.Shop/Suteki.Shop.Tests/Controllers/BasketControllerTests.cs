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
    public class BasketControllerTests
    {
        BasketController basketController;
        ControllerTestContext testContext;
        
        IRepository<Basket> basketRepository;
        IRepository<BasketItem> basketItemRepository;
        IRepository<User> userRepository;
        IUserService userService;

        [SetUp]
        public void SetUp()
        {
            basketRepository = new Mock<IRepository<Basket>>().Object;
            basketItemRepository = new Mock<IRepository<BasketItem>>().Object;
            userRepository = new Mock<IRepository<User>>().Object;
            userService = new Mock<IUserService>().Object;

            basketController = new Mock<BasketController>(
                basketRepository, 
                basketItemRepository, 
                userRepository,
                userService).Object;
            testContext = new ControllerTestContext(basketController);
        }

        [Test]
        public void Index_ShouldShowIndexViewWithCurrentBasket()
        {
            User user = CreateUserWithBasket();
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            RenderViewResult result = basketController.Index() as RenderViewResult;

            Assert.AreEqual("Index", result.ViewName);
            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");

            Assert.AreSame(user.Baskets[0], viewData.Basket, "The user's basket has not been shown");
        }

        private User CreateUserWithBasket()
        {
            User user = new User
            {
                RoleId = Role.GuestId,
                Baskets =
                {
                    new Basket()
                }
            };
            return user;
        }

        [Test]
        public void Update_ShouldAddBasketLineToCurrentBasket()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("sizeid", "5");
            form.Add("quantity", "2");
            testContext.TestContext.RequestMock.ExpectGet(request => request.Form).Returns(form);

            User user = CreateUserWithBasket();
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            // expect 
            Mock.Get(basketRepository).Expect(or => or.SubmitChanges()).Verifiable();
            Mock.Get(userService).Expect(us => us.CreateNewCustomer()).Returns(user).Verifiable();
            Mock.Get(basketController).Expect(oc => oc.SetAuthenticationCookie(user.Email)).Verifiable();
            Mock.Get(basketController).Expect(oc => oc.SetContextUserTo(user)).Verifiable();

            RenderViewResult result = basketController.Update() as RenderViewResult;

            Assert.AreEqual("Index", result.ViewName);

            Assert.AreEqual(1, user.Baskets[0].BasketItems.Count, "expected BasketItem is missing");
            Assert.AreEqual(5, user.Baskets[0].BasketItems[0].SizeId);
            Assert.AreEqual(2, user.Baskets[0].BasketItems[0].Quantity);
            
            Mock.Get(basketRepository).Verify();
            Mock.Get(basketController).Verify();
            Mock.Get(userService).Verify();
        }

        [Test]
        public void Remove_ShouldRemoveItemFromBasket()
        {
            int basketItemIdToRemove = 3;

            User user = CreateUserWithBasket();
            BasketItem basketItem = new BasketItem { BasketItemId = basketItemIdToRemove };
            user.Baskets[0].BasketItems.Add(basketItem);
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            // expect 
            Mock.Get(basketItemRepository).Expect(ir => ir.DeleteOnSubmit(basketItem)).Verifiable();
            Mock.Get(basketItemRepository).Expect(ir => ir.SubmitChanges());

            RenderViewResult result = basketController.Remove(basketItemIdToRemove) as RenderViewResult;

            Assert.AreEqual("Index", result.ViewName);
            Mock.Get(basketItemRepository).Verify();
        }
    }
}
