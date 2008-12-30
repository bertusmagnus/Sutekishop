using System.Linq;
using NUnit.Framework;
using Moq;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using Suteki.Shop.Tests.Repositories;
using System.Collections.Specialized;
using System.Threading;
using System.Security.Principal;
using System.Web.Mvc;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CategoryControllerTests
    {
        private CategoryController categoryController;
        private Mock<CategoryController> categoryControllerMock;
        private ControllerTestContext testContext;

        private Mock<Repository<Category>> categoryRepositoryMock;
        private IOrderableService<Category> orderableService;
        private IValidatingBinder validatingBinder;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the category controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            categoryRepositoryMock = MockRepositoryBuilder.CreateCategoryRepository();
            orderableService = new Mock<IOrderableService<Category>>().Object;
            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());

            categoryControllerMock = new Mock<CategoryController>(
                categoryRepositoryMock.Object, 
                orderableService, 
                validatingBinder);

            categoryController = categoryControllerMock.Object;

            testContext = new ControllerTestContext(categoryController);
        }

        [Test]
        public void Index_ShouldDisplayAListOfCategories()
        {
            var result = categoryController.Index() as ViewResult;

            Assert.IsNotNull(result, "expected a ViewResult");

            Assert.AreEqual("Index", result.ViewName, "ViewName is incorrect");

            var viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");

            MockRepositoryBuilder.AssertCategoryGraphIsCorrect(viewData.Category);
        }

        [Test]
        public void New_ShouldDisplayCategoryEditView()
        {
            var result = categoryController.New(1) as ViewResult;

            AssertEditViewIsCorrectlyShown(result);
        }

        private static void AssertEditViewIsCorrectlyShown(ViewResultBase result)
        {
            Assert.AreEqual("Edit", result.ViewName, "ViewName is incorrect");

            var viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");

            Assert.IsNotNull(viewData.Category, "Category is null");
            Assert.IsNotNull(viewData.Categories, "Categories is null");
            Assert.AreEqual(6, viewData.Categories.Count(), "Expected six categories");
        }

        [Test]
        public void Edit_ShouldDisplayCategoryEditViewWithCorrectCategory()
        {
            const int categoryId = 3;

            var category = new Category
            {
                CategoryId = categoryId,
                Name = "My Category",
                ParentId = 23
            };

            categoryRepositoryMock.Expect(cr => cr.GetById(categoryId)).Returns(category);

            var result = categoryController.Edit(categoryId) as ViewResult;

            AssertEditViewIsCorrectlyShown(result);
            categoryRepositoryMock.Verify();
        }

        [Test]
        public void Update_ShouldInsertNewCategory()
        {
            const int categoryId = 0;
            const string name = "My Category";
            const int parentid = 78;

            // set up the request form
            var form = new NameValueCollection();
            form.Add("categoryid", categoryId.ToString());
            form.Add("name", name);
            form.Add("parentid", parentid.ToString());
            testContext.TestContext.Request.Expect(r => r.Form).Return(form);

            // set up expectations on the repository
            Category category = null;

            categoryRepositoryMock.Expect(cr => cr.InsertOnSubmit(It.IsAny<Category>()))
                .Callback<Category>(c => { category = c; }).Verifiable();

            categoryRepositoryMock.Expect(cr => cr.SubmitChanges()).Verifiable();

            var result = categoryController.Update(categoryId) as ViewResult;

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
            const int categoryId = 12;
            const string name = "My Category";
            const int parentid = 78;

            // set up the request form
            var form = new NameValueCollection
            {
                {"categoryid", categoryId.ToString()},
                {"name", name},
                {"parentid", parentid.ToString()}
            };
            testContext.TestContext.Request.Expect(r => r.Form).Return(form);

            var category = new Category
            {
                CategoryId = 12,
                Name = "Old Name",
                ParentId = 22
            };

            categoryRepositoryMock.Expect(cr => cr.GetById(categoryId)).Returns(category).Verifiable();
            categoryRepositoryMock.Expect(cr => cr.SubmitChanges()).Verifiable();

            var result = categoryController.Update(categoryId) as ViewResult;

            // assert that the category has the correct values from the form
            Assert.IsNotNull(category, "category is null");
            Assert.AreEqual(name, category.Name);
            Assert.AreEqual(parentid, category.ParentId);

            AssertEditViewIsCorrectlyShown(result);

            categoryControllerMock.Verify();
        }
    }
}
