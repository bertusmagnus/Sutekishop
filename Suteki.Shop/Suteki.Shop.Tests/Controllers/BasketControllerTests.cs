using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.TestHelpers;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class BasketControllerTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            basketRepository = MockRepository.GenerateStub<IRepository<Basket>>();
            basketRepository = MockRepository.GenerateStub<IRepository<Basket>>();
            basketItemRepository = MockRepository.GenerateStub<IRepository<BasketItem>>();
            sizeRepository = MockRepository.GenerateStub<IRepository<Size>>();

            userService = MockRepository.GenerateStub<IUserService>();
            postageService = MockRepository.GenerateStub<IPostageService>();
            countryRepository = MockRepository.GenerateStub<IRepository<Country>>();

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

            user = CreateUserWithBasket();
            userService.Stub(us => us.CreateNewCustomer()).Return(user);
        }

        #endregion

        private User user;

        private BasketController basketController;
        private ControllerTestContext testContext;

        private IRepository<Basket> basketRepository;
        private IRepository<BasketItem> basketItemRepository;
        private IRepository<Size> sizeRepository;

        private IUserService userService;
        private IPostageService postageService;
        private IRepository<Country> countryRepository;
        private IValidatingBinder validatingBinder;

        private User CreateUserWithBasket()
        {
            var theUser = new User
            {
                RoleId = Role.GuestId,
                Baskets =
                    {
                        new Basket()
                    }
            };
            userService.Expect(bc => bc.CurrentUser).Return(theUser);
            return theUser;
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
        public void Index_ShouldShowIndexViewWithCurrentBasket()
        {
            testContext.TestContext.Context.User = user;

            basketController.Index()
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>()
                .AssertAreSame(user.Baskets[0], vd => vd.Basket);
        }

        [Test]
        public void Remove_ShouldRemoveItemFromBasket()
        {
            const int basketItemIdToRemove = 3;

            var basketItem = new BasketItem
            {
                BasketItemId = basketItemIdToRemove,
                Quantity = 1,
                Size = new Size
                {
                    Product = new Product {Weight = 100}
                }
            };
            user.Baskets[0].BasketItems.Add(basketItem);
            testContext.TestContext.Context.User = user;

            basketController.Remove(basketItemIdToRemove)
                .ReturnsViewResult()
                .ForView("Index");

            basketItemRepository.AssertWasCalled(ir => ir.DeleteOnSubmit(basketItem));
            basketItemRepository.AssertWasCalled(ir => ir.SubmitChanges());
        }

        [Test]
        public void Update_ShouldAddBasketLineToCurrentBasket()
        {
            var form = CreateUpdateForm();

            var size = new Size
            {
                IsInStock = true,
                Product = new Product
                {
                    Weight = 10
                }
            };
            sizeRepository.Stub(sr => sr.GetById(5)).Return(size);

            basketController.Update(form);

            Assert.AreEqual(1, user.Baskets[0].BasketItems.Count, "expected BasketItem is missing");
            Assert.AreEqual(5, user.Baskets[0].BasketItems[0].SizeId);
            Assert.AreEqual(2, user.Baskets[0].BasketItems[0].Quantity);

            basketRepository.AssertWasCalled(repository => repository.SubmitChanges());
            userService.AssertWasCalled(service => service.SetAuthenticationCookie(user.Email));
            userService.AssertWasCalled(service => service.SetContextUserTo(user));
        }

        [Test]
        public void Update_ShouldShowErrorMessageIfItemIsOutOfStock()
        {
            var form = CreateUpdateForm();

            var size = new Size
            {
                Name = "S",
                IsInStock = false,
                IsActive = true,
                Product = new Product {Name = "Denim Jacket"}
            };
            sizeRepository.Stub(sr => sr.GetById(5)).Return(size);

            const string expectedMessage = "Sorry, Denim Jacket, Size S is out of stock.";

            basketController.Update(form)
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>()
                .AssertAreEqual(expectedMessage, vd => vd.ErrorMessage);

            Assert.AreEqual(0, user.Baskets[0].BasketItems.Count, "should not be any basket items");

            userService.AssertWasCalled(bc => bc.SetAuthenticationCookie(user.Email));
            userService.AssertWasCalled(bc => bc.SetContextUserTo(user));
        }
    }
}