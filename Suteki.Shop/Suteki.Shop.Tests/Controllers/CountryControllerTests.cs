using System;
using System.Linq;
using System.Data.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using System.Web.Mvc;
using System.Collections.Generic;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using Suteki.Shop.Services;
using System.Threading;
using System.Security.Principal;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CountryControllerTests
    {
        CountryController countryController;
        IRepository<Country> countryRepository;
        ControllerTestContext testContext;
        IOrderableService<Country> countryOrderableService;

        IQueryable<Country> countries;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the country controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { "Administrator" });

            countryRepository = new Mock<IRepository<Country>>().Object;
            countryOrderableService = new Mock<IOrderableService<Country>>().Object;

            countryController = new CountryController(countryRepository, countryOrderableService);
            testContext = new ControllerTestContext(countryController);

            countries = new Country[]
            {
                new Country(),
                new Country()
            }.AsQueryable();

            Mock.Get(countryRepository).Expect(cr => cr.GetAll()).Returns(countries);
        }

        [Test]
        public void Index_ShouldShowAllCountries()
        {

            RenderViewResult result = countryController.Index() as RenderViewResult;

            AssertIndexView(result);
        }

        private void AssertIndexView(RenderViewResult result)
        {
            Assert.AreEqual("Index", result.ViewName, "ViewName is incorrect");
            ShopViewData shopViewData = result.ViewData as ShopViewData;
            Assert.AreEqual(shopViewData.Countries.Count(), countries.Count(), "countries have not been passed to viewData");
        }

        [Test]
        public void New_ShouldShowEditViewWithDefaultCountry()
        {
            Mock.Get(countryOrderableService).ExpectGet(os => os.NextPosition).Returns(4);

            RenderViewResult result = countryController.New() as RenderViewResult;

            Assert.AreEqual("Edit", result.ViewName, "ViewName is incorrect");
            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.IsNotNull(viewData.Country);
            Assert.AreEqual(4, viewData.Country.Position);
        }

        [Test]
        public void Edit_ShouldDisplayGivenCountryInEditView()
        {
            int countryId = 33;

            Country country = new Country();

            // expectations
            Mock.Get(countryRepository).Expect(cr => cr.GetById(countryId)).Returns(country).Verifiable();

            // exercise action
            RenderViewResult result = countryController.Edit(countryId) as RenderViewResult;

            // assertions
            Assert.AreEqual("Edit", result.ViewName, "ViewName is incorrect");
            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.AreSame(country, viewData.Country);

            Mock.Get(countryRepository).Verify();
        }

        [Test]
        public void Update_ShouldAddNewCountry()
        {
            int countryId = 0;
            Country country = null;

            NameValueCollection form = new NameValueCollection();
            form.Add("name", "Bulgaria");
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);
            testContext.TestContext.RequestMock.ExpectGet(r => r.QueryString).Returns(() => new NameValueCollection());

            // expectations
            Mock.Get(countryRepository).Expect(cr => cr.InsertOnSubmit(It.IsAny<Country>()))
                .Callback<Country>(c => country = c)
                .Verifiable();
            Mock.Get(countryRepository).Expect(cr => cr.SubmitChanges()).Verifiable();

            // exercise action
            RenderViewResult result = countryController.Update(countryId) as RenderViewResult;

            // assertions
            Assert.AreEqual("Edit", result.ViewName, "ViewName is incorrect");
            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.AreSame(country, viewData.Country);
            Assert.AreEqual("Bulgaria", country.Name);

            Mock.Get(countryRepository).Verify();
        }

        [Test]
        public void Update_ShouldUpdateExistingCountry()
        {
            int countryId = 67;  // existing country
            Country country = new Country { Name = "Bulgrogia" };

            NameValueCollection form = new NameValueCollection();
            form.Add("name", "Bulgaria");
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);
            testContext.TestContext.RequestMock.ExpectGet(r => r.QueryString).Returns(() => new NameValueCollection());

            // expectations
            Mock.Get(countryRepository).Expect(cr => cr.GetById(countryId))
                .Returns(country)
                .Verifiable();
            Mock.Get(countryRepository).Expect(cr => cr.SubmitChanges()).Verifiable();

            // exercise action
            RenderViewResult result = countryController.Update(countryId) as RenderViewResult;

            // assertions
            Assert.AreEqual("Edit", result.ViewName, "ViewName is incorrect");
            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.AreSame(country, viewData.Country);
            Assert.AreEqual("Bulgaria", country.Name);

            Mock.Get(countryRepository).Verify();
        }

        [Test]
        public void MoveUp_ShouldMoveItemUp()
        {
            int postion = 5;

            IOrderServiceWithPosition<Country> orderResult = new Mock<IOrderServiceWithPosition<Country>>().Object;

            Mock.Get(countryOrderableService).Expect(os => os.MoveItemAtPosition(postion))
                .Returns(orderResult).Verifiable();
            Mock.Get(orderResult).Expect(or => or.UpOne()).Verifiable();

            RenderViewResult result = countryController.MoveUp(postion) as RenderViewResult;

            AssertIndexView(result);

            Mock.Get(countryOrderableService).Verify();
            Mock.Get(orderResult).Verify();
        }

        [Test]
        public void MoveDown_ShouldMoveItemDown()
        {
            int postion = 5;

            IOrderServiceWithPosition<Country> orderResult = new Mock<IOrderServiceWithPosition<Country>>().Object;

            Mock.Get(countryOrderableService).Expect(os => os.MoveItemAtPosition(postion))
                .Returns(orderResult).Verifiable();
            Mock.Get(orderResult).Expect(or => or.DownOne()).Verifiable();

            RenderViewResult result = countryController.MoveDown(postion) as RenderViewResult;

            AssertIndexView(result);

            Mock.Get(countryOrderableService).Verify();
            Mock.Get(orderResult).Verify();
        }
    }
}
