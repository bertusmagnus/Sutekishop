using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.TestHelpers;
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
        private ControllerTestContext testContext;

        private IRepository<Category> categoryRepository;
        private IOrderableService<Category> orderableService;
        private IValidatingBinder validatingBinder;

        [SetUp]
        public void SetUp()
        {
            categoryRepository = MockRepositoryBuilder.CreateCategoryRepository();
            orderableService = MockRepository.GenerateStub<IOrderableService<Category>>();
			validatingBinder = new ValidatingBinder(new SimplePropertyBinder());

            categoryController = new CategoryController(
                categoryRepository, 
                orderableService, 
                validatingBinder);

            testContext = new ControllerTestContext(categoryController);
        }

        [Test]
        public void Index_ShouldDisplayAListOfCategories()
        {
            var viewData = categoryController.Index()
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>();

            MockRepositoryBuilder.AssertCategoryGraphIsCorrect(viewData.Category);
        }

        [Test]
        public void New_ShouldDisplayCategoryEditView()
        {
            var result = categoryController.New(1);
            AssertEditViewIsCorrectlyShown(result);
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

            categoryRepository.Stub(cr => cr.GetById(categoryId)).Return(category);

            var result = categoryController.Edit(categoryId);
            AssertEditViewIsCorrectlyShown(result);
        }

    	[Test]
    	public void EditWithPost_should_render_view_when_binding_succeeded()
    	{
    		var category = new Category();
			categoryController.Edit(category)
				.ReturnsViewResult()
				.ForView("Edit")
				.WithModel<ShopViewData>()
				.AssertAreEqual(category, x => x.Category)
				.AssertNotNull(x => x.Message);
    	}

    	[Test]
    	public void EditWithPost_should_render_view_with_error_when_binding_fails()
    	{
			categoryController.ModelState.AddModelError("foo", "bar");

			var category = new Category();
			categoryController.Edit(category)
				.ReturnsViewResult()
				.ForView("Edit")
				.WithModel<ShopViewData>()
				.AssertAreEqual(category, x => x.Category)
				.AssertNull(x => x.Message);
    	}

       /* [Test]
        public void Update_ShouldInsertNewCategory()
        {
            const int categoryId = 0;
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

            // set up expectations on the repository
            Category category = null;

            categoryRepository.Expect(cr => cr.InsertOnSubmit(Arg<Category>.Is.Anything))
                .Callback<Category>(c => 
                { 
                    category = c;
                    return true; 
                });

            categoryRepository.Expect(cr => cr.SubmitChanges());

            var result = categoryController.Update(categoryId) as ViewResult;

            // assert that the category has the correct values from the form
            Assert.IsNotNull(category, "category is null");
            Assert.AreEqual(name, category.Name);
            Assert.AreEqual(parentid, category.ParentId);

            AssertEditViewIsCorrectlyShown(result);
        }*/

        private static void AssertEditViewIsCorrectlyShown(ActionResult result)
        {
            result
                .ReturnsViewResult()
                .ForView("Edit")
                .WithModel<ShopViewData>()
                .AssertNotNull(vd => vd.Category)
                .AssertNotNull(vd => vd.Categories)
                .AssertAreEqual(6, vd => vd.Categories.Count());
        }

       /* [Test]
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

            categoryRepository.Expect(cr => cr.GetById(categoryId)).Return(category);
            categoryRepository.Expect(cr => cr.SubmitChanges());

            var result = categoryController.Update(categoryId) as ViewResult;

            // assert that the category has the correct values from the form
            Assert.IsNotNull(category, "category is null");
            Assert.AreEqual(name, category.Name);
            Assert.AreEqual(parentid, category.ParentId);

            AssertEditViewIsCorrectlyShown(result);
        }*/

		[Test]
		public void NewWithPost_should_insert_new_category()
		{
			const int categoryId = 0;
			const string name = "My Category";
			const int parentid = 78;

			// set up the request form
			var form = new FormCollection()
            {
                {"category.CategoryId", categoryId.ToString()},
                {"category.Name", name},
                {"category.ParentId", parentid.ToString()}
            };

			// set up expectations on the repository
			Category category = null;

			categoryRepository.Expect(x => x.InsertOnSubmit(Arg<Category>.Is.Anything))
				.Callback<Category>(x => { category = x; return true; });

			categoryController.New(form)
				.ReturnRedirectToRouteResult()
				.ToAction("Index");

			category.ShouldNotBeNull();
			category.Name.ShouldEqual(name);
			category.ParentId.ShouldEqual(parentid);

			categoryController.Message.ShouldNotBeNull();
		}

    	[Test]
    	public void NewWithPost_should_render_view_on_error()
    	{
			var form = new FormCollection 
			{
				{ "category.CategoryId", "foo" }
			};

			categoryController.New(form)
				.ReturnsViewResult()
				.ForView("Edit")
				.WithModel<ShopViewData>()
				.AssertNotNull(x => x.ErrorMessage);

			categoryController.ModelState.IsValid.ShouldBeFalse();
    	}

    }
}
