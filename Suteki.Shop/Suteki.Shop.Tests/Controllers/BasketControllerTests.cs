using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.TestHelpers;
using Suteki.Shop.ViewData;
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
        IRepository<Size> sizeRepository;

        IUserService userService;
        IPostageService postageService;
        IRepository<Country> countryRepository;
        IValidatingBinder validatingBinder;

        [SetUp]
        public void SetUp()
        {
            basketRepository = new Mock<IRepository<Basket>>().Object;
            basketItemRepository = new Mock<IRepository<BasketItem>>().Object;
            sizeRepository = new Mock<IRepository<Size>>().Object;

            userService = new Mock<IUserService>().Object;
            postageService = new Mock<IPostageService>().Object;
            countryRepository = new Mock<IRepository<Country>>().Object;

            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());

            basketController = new BasketController(
                basketRepository, 
                basketItemRepository, 
                sizeRepository,
                userService,
                postageService,
                countryRepository,
                validatingBinder);

            testContext = new ControllerTestContext(basketController);

            Mock.Get(postageService).Expect(ps => ps.CalculatePostageFor(It.IsAny<Basket>()));
        }

        [Test]
        public void Index_ShouldShowIndexViewWithCurrentBasket()
        {
            var user = CreateUserWithBasket();
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            var result = basketController.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
            var viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");

            Assert.AreSame(user.Baskets[0], viewData.Basket, "The user's basket has not been shown");
        }

        private User CreateUserWithBasket()
        {
            var user = new User
            {
                RoleId = Role.GuestId,
                Baskets =
                {
                    new Basket()
                }
            };
            Mock.Get(userService).Expect(bc => bc.CurrentUser).Returns(user);
            return user;
        }

        [Test]
        public void Update_ShouldAddBasketLineToCurrentBasket()
        {
            var form = CreateUpdateForm();
            var user = CreateUserWithBasket();

            // expect 
            Mock.Get(basketRepository).Expect(or => or.SubmitChanges()).Verifiable();
            Mock.Get(userService).Expect(us => us.CreateNewCustomer()).Returns(user).Verifiable();
            Mock.Get(userService).Expect(bc => bc.SetAuthenticationCookie(user.Email)).Verifiable();
            Mock.Get(userService).Expect(bc => bc.SetContextUserTo(user)).Verifiable();

            var size = new Size
            {
                IsInStock = true,
                Product = new Product
                              {
                                  Weight = 10
                              }
            };
            Mock.Get(sizeRepository).Expect(sr => sr.GetById(5)).Returns(size);

            basketController.Update(form);

            Assert.AreEqual(1, user.Baskets[0].BasketItems.Count, "expected BasketItem is missing");
            Assert.AreEqual(5, user.Baskets[0].BasketItems[0].SizeId);
            Assert.AreEqual(2, user.Baskets[0].BasketItems[0].Quantity);
            
            Mock.Get(basketRepository).Verify();
            Mock.Get(userService).Verify();
        }

        private static FormCollection CreateUpdateForm()
        {
            var form = new FormCollection
            {
                {"sizeid", "5"}, 
                {"quantity", "2"}
            };
            return form;
        }

        [Test]
        public void Update_ShouldShowErrorMessageIfItemIsOutOfStock()
        {
            var form = CreateUpdateForm();
            var user = CreateUserWithBasket();

            // expect 
            Mock.Get(basketRepository).Expect(or => or.SubmitChanges());
            Mock.Get(userService).Expect(us => us.CreateNewCustomer()).Returns(user);
            Mock.Get(userService).Expect(bc => bc.SetAuthenticationCookie(user.Email));
            Mock.Get(userService).Expect(bc => bc.SetContextUserTo(user));

            var size = new Size
            {
                Name = "S",
                IsInStock = false,
                IsActive = true,
                Product = new Product { Name = "Denim Jacket" }
            };
            Mock.Get(sizeRepository).Expect(sr => sr.GetById(5)).Returns(size);

            var expectedMessage = "Sorry, Denim Jacket, Size S is out of stock.";

            basketController.Update(form)
                .ReturnsViewResult()
                .ForView("Index")
                .AssertAreEqual<ShopViewData, string>(expectedMessage, vd => vd.ErrorMessage);

            Assert.AreEqual(0, user.Baskets[0].BasketItems.Count, "should not be any basket items");

        }

        [Test]
        public void Remove_ShouldRemoveItemFromBasket()
        {
            const int basketItemIdToRemove = 3;

            var user = CreateUserWithBasket();
            var basketItem = new BasketItem 
            { 
                BasketItemId = basketItemIdToRemove,
                Quantity = 1,
                Size = new Size
                           {
                               Product = new Product { Weight = 100 }
                           }
            };
            user.Baskets[0].BasketItems.Add(basketItem);
            testContext.TestContext.ContextMock.ExpectGet(context => context.User).Returns(user);

            // expect 
            Mock.Get(basketItemRepository).Expect(ir => ir.DeleteOnSubmit(basketItem)).Verifiable();
            Mock.Get(basketItemRepository).Expect(ir => ir.SubmitChanges());

            var result = basketController.Remove(basketItemIdToRemove) as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
            Mock.Get(basketItemRepository).Verify();
        }
    }
}
