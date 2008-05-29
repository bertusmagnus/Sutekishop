using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using Suteki.Shop.Repositories;
using System.Web.Mvc;
using Suteki.Shop.Services;
using System.Collections.Generic;

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
        IRepository<Postage> postageRepository;
        IRepository<Size> sizeRepository;

        IUserService userService;

        [SetUp]
        public void SetUp()
        {
            basketRepository = new Mock<IRepository<Basket>>().Object;
            basketItemRepository = new Mock<IRepository<BasketItem>>().Object;
            userRepository = new Mock<IRepository<User>>().Object;
            postageRepository = new Mock<IRepository<Postage>>().Object;
            sizeRepository = new Mock<IRepository<Size>>().Object;

            userService = new Mock<IUserService>().Object;

            basketController = new Mock<BasketController>(
                basketRepository, 
                basketItemRepository, 
                userRepository,
                postageRepository,
                sizeRepository,
                userService).Object;
            testContext = new ControllerTestContext(basketController);


            Mock.Get(postageRepository).Expect(p => p.GetAll()).Returns(new List<Postage>().AsQueryable());
        }

        [Test]
        public void Index_ShouldShowIndexViewWithCurrentBasket()
        {
            User user = CreateUserWithBasket();
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            ViewResult result = basketController.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
            ShopViewData viewData = result.ViewData.Model as ShopViewData;
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
            Mock.Get(basketController).Expect(bc => bc.CurrentUser).Returns(user);
            return user;
        }

        [Test]
        public void Update_ShouldAddBasketLineToCurrentBasket()
        {
            CreateUpdateForm();
            User user = CreateUserWithBasket();

            // expect 
            Mock.Get(basketRepository).Expect(or => or.SubmitChanges()).Verifiable();
            Mock.Get(userService).Expect(us => us.CreateNewCustomer()).Returns(user).Verifiable();
            Mock.Get(basketController).Expect(bc => bc.SetAuthenticationCookie(user.Email)).Verifiable();
            Mock.Get(basketController).Expect(bc => bc.SetContextUserTo(user)).Verifiable();

            Size size = new Size
            {
                IsInStock = true,
            };
            Mock.Get(sizeRepository).Expect(sr => sr.GetById(5)).Returns(size);

            basketController.Update()
                .ReturnsViewResult()
                .ForView("Index");

            Assert.AreEqual(1, user.Baskets[0].BasketItems.Count, "expected BasketItem is missing");
            Assert.AreEqual(5, user.Baskets[0].BasketItems[0].SizeId);
            Assert.AreEqual(2, user.Baskets[0].BasketItems[0].Quantity);
            
            Mock.Get(basketRepository).Verify();
            Mock.Get(basketController).Verify();
            Mock.Get(userService).Verify();
        }

        private void CreateUpdateForm()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("sizeid", "5");
            form.Add("quantity", "2");
            testContext.TestContext.RequestMock.ExpectGet(request => request.Form).Returns(form);
        }

        [Test]
        public void Update_ShouldShowErrorMessageIfItemIsOutOfStock()
        {
            CreateUpdateForm();
            var user = CreateUserWithBasket();

            // expect 
            Mock.Get(basketRepository).Expect(or => or.SubmitChanges());
            Mock.Get(userService).Expect(us => us.CreateNewCustomer()).Returns(user);
            Mock.Get(basketController).Expect(bc => bc.SetAuthenticationCookie(user.Email));
            Mock.Get(basketController).Expect(bc => bc.SetContextUserTo(user));

            var size = new Size
            {
                Name = "S",
                IsInStock = false,
                IsActive = true,
                Product = new Product { Name = "Denim Jacket" }
            };
            Mock.Get(sizeRepository).Expect(sr => sr.GetById(5)).Returns(size);

            var expectedMessage = "Sorry, Denim Jacket, Size S is out of stock.";

            basketController.Update()
                .ReturnsViewResult()
                .ForView("Index")
                .AssertAreEqual<ShopViewData, string>(expectedMessage, vd => vd.ErrorMessage);

            Assert.AreEqual(0, user.Baskets[0].BasketItems.Count, "should not be any basket items");

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

            ViewResult result = basketController.Remove(basketItemIdToRemove) as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
            Mock.Get(basketItemRepository).Verify();
        }
    }
}
