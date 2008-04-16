using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using Suteki.Shop.Tests.Repositories;
using Suteki.Shop.Repositories;
using System.Collections.Specialized;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CategoryControllerTests
    {
        CategoryController categoryController;
        Mock<CategoryController> categoryControllerMock;
        ControllerTestContext testContext;

        Mock<Repository<Category>> categoryRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            categoryRepositoryMock = MockRepositoryBuilder.CreateCategoryRepository();

            categoryControllerMock = new Mock<CategoryController>(categoryRepositoryMock.Object);
            categoryController = categoryControllerMock.Object;

            testContext = new ControllerTestContext(categoryController);
        }

        [Test]
        public void Index_ShouldDisplayAListOfCategories()
        {
            categoryController.Index();

            Assert.AreEqual("Index", testContext.ViewEngine.ViewContext.ViewName, "ViewName is incorrect");

            CategoryViewData viewData = testContext.ViewEngine.ViewContext.ViewData as CategoryViewData;
            Assert.IsNotNull(viewData, "viewData is not CategoryViewData");

            MockRepositoryBuilder.AssertCategoryGraphIsCorrect(viewData.Category);
        }

        [Test]
        public void New_ShouldDisplayCategoryEditView()
        {
            categoryController.New();

            AssertEditViewIsCorrectlyShown();
        }

        private void AssertEditViewIsCorrectlyShown()
        {
            Assert.AreEqual("Edit", testContext.ViewEngine.ViewContext.ViewName, "ViewName is incorrect");

            CategoryViewData viewData = testContext.ViewEngine.ViewContext.ViewData as CategoryViewData;
            Assert.IsNotNull(viewData, "viewData is not CategoryViewData");

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

            categoryController.Edit(categoryId);

            AssertEditViewIsCorrectlyShown();
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

            categoryController.Update(categoryId);

            // assert that the category has the correct values from the form
            Assert.IsNotNull(category, "category is null");
            Assert.AreEqual(name, category.Name);
            Assert.AreEqual(parentid, category.ParentId);

            AssertEditViewIsCorrectlyShown();

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

            categoryController.Update(categoryId);

            // assert that the category has the correct values from the form
            Assert.IsNotNull(category, "category is null");
            Assert.AreEqual(name, category.Name);
            Assert.AreEqual(parentid, category.ParentId);

            AssertEditViewIsCorrectlyShown();

            categoryControllerMock.Verify();
        }
    }
}
