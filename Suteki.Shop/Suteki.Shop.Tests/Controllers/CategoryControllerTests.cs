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
				.WithModel<ShopViewData>()
				.AssertAreEqual(category, x => x.Category)
				.AssertNull(x => x.Message);
    	}

   
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


		[Test]
		public void NewWithPost_should_insert_new_category()
		{
			const int categoryId = 0;
			const string name = "My Category";
			const int parentid = 78;

			var category = new Category 
			{
				CategoryId = categoryId,
				Name = name,
				ParentId = parentid
			};

			categoryController.New(category)
				.ReturnRedirectToRouteResult()
				.ToAction("Index");

			categoryRepository.AssertWasCalled(x => x.InsertOnSubmit(category));
			categoryController.Message.ShouldNotBeNull();
		}

    	[Test]
    	public void NewWithPost_should_render_view_on_error()
    	{
			categoryController.ModelState.AddModelError("foo", "bar");

			categoryController.New(new Category())
				.ReturnsViewResult()
				.ForView("Edit")
				.WithModel<ShopViewData>();

			categoryRepository.AssertWasNotCalled(x => x.InsertOnSubmit(Arg<Category>.Is.Anything));

    	}

    }
}
