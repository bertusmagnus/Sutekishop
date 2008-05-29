using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using Suteki.Shop.Tests.Repositories;
using Suteki.Shop.Repositories;
using System.Collections.Specialized;
using System.Threading;
using System.Security.Principal;
using System.Web.Mvc;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CategoryControllerTests
    {
        CategoryController categoryController;
        Mock<CategoryController> categoryControllerMock;
        ControllerTestContext testContext;

        Mock<Repository<Category>> categoryRepositoryMock;
        IOrderableService<Category> orderableService;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the category controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            categoryRepositoryMock = MockRepositoryBuilder.CreateCategoryRepository();
            orderableService = new Mock<IOrderableService<Category>>().Object;

            categoryControllerMock = new Mock<CategoryController>(categoryRepositoryMock.Object, orderableService);
            categoryController = categoryControllerMock.Object;

            testContext = new ControllerTestContext(categoryController);
        }

        [Test]
        public void Index_ShouldDisplayAListOfCategories()
        {
            ViewResult result = categoryController.Index() as ViewResult;

            Assert.IsNotNull(result, "expected a ViewResult");

            Assert.AreEqual("Index", result.ViewName, "ViewName is incorrect");

            ShopViewData viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");

            MockRepositoryBuilder.AssertCategoryGraphIsCorrect(viewData.Category);
        }

        [Test]
        public void New_ShouldDisplayCategoryEditView()
        {
            ViewResult result = categoryController.New(1) as ViewResult;

            AssertEditViewIsCorrectlyShown(result);
        }

        private void AssertEditViewIsCorrectlyShown(ViewResult result)
        {
            Assert.AreEqual("Edit", result.ViewName, "ViewName is incorrect");

            ShopViewData viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");

            Assert.IsNotNull(viewData.Category, "Category is null");
            Assert.IsNotNull(viewData.Categories, "Categories is null");
            Assert.AreEqual(6, viewData.Categories.Count(), "Expected six categories");
        }

        [Test]
        public void Edit_ShouldDisplayCategoryEditViewWithCorrectCategory()
        {
            int categoryId = 3;

            Category category = new Category
            {
                CategoryId = categoryId,
                Name = "My Category",
                ParentId = 23
            };

            categoryRepositoryMock.Expect(cr => cr.GetById(categoryId)).Returns(category);

            ViewResult result = categoryController.Edit(categoryId) as ViewResult;

            AssertEditViewIsCorrectlyShown(result);
            categoryRepositoryMock.Verify();
        }

        [Test]
        public void Update_ShouldInsertNewCategory()
        {
            int categoryId = 0;
            string name = "My Category";
            int parentid = 78;

            // set up the request form
            NameValueCollection form = new NameValueCollection();
            form.Add("categoryid", categoryId.ToString());
            form.Add("name", name);
            form.Add("parentid", parentid.ToString());
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);

            // set up expectations on the repository
            Category category = null;

            categoryRepositoryMock.Expect(cr => cr.InsertOnSubmit(It.IsAny<Category>()))
                .Callback<Category>(c => { category = c; }).Verifiable();

            categoryRepositoryMock.Expect(cr => cr.SubmitChanges()).Verifiable();

            ViewResult result = categoryController.Update(categoryId) as ViewResult;

            // assert that the category has the correct values from the form
            Assert.IsNotNull(category, "category is null");
            Assert.AreEqual(name, category.Name);
            Assert.AreEqual(parentid, category.ParentId);

            AssertEditViewIsCorrectlyShown(result);

            categoryControllerMock.Verify();
        }

        [Test]
        public void Update_ShouldUpdateAnExistingCategory()
        {
            int categoryId = 12;
            string name = "My Category";
            int parentid = 78;

            // set up the request form
            NameValueCollection form = new NameValueCollection();
            form.Add("categoryid", categoryId.ToString());
            form.Add("name", name);
            form.Add("parentid", parentid.ToString());
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);

            Category category = new Category
            {
                CategoryId = 12,
                Name = "Old Name",
                ParentId = 22
            };

            categoryRepositoryMock.Expect(cr => cr.GetById(categoryId)).Returns(category).Verifiable();
            categoryRepositoryMock.Expect(cr => cr.SubmitChanges()).Verifiable();

            ViewResult result = categoryController.Update(categoryId) as ViewResult;

            // assert that the category has the correct values from the form
            Assert.IsNotNull(category, "category is null");
            Assert.AreEqual(name, category.Name);
            Assert.AreEqual(parentid, category.ParentId);

            AssertEditViewIsCorrectlyShown(result);

            categoryControllerMock.Verify();
        }
    }
}
