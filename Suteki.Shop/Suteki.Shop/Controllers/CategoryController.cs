using System.Web.Mvc;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Filters;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using MvcContrib;
using Suteki.Shop.Binders;

namespace Suteki.Shop.Controllers
{
	[AdministratorsOnly]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IOrderableService<Category> orderableService;
        private readonly IValidatingBinder validatingBinder;

        public CategoryController(
            IRepository<Category> categoryRepository,
            IOrderableService<Category> orderableService,
            IValidatingBinder validatingBinder)
        {
            this.categoryRepository = categoryRepository;
            this.orderableService = orderableService;
            this.validatingBinder = validatingBinder;
        }

        public ActionResult Index()
        {
            return RenderIndexView();
        }

        private ActionResult RenderIndexView()
        {
            var root = categoryRepository.GetRootCategory();
            return View("Index", ShopView.Data.WithCategory(root));
        }

        public ActionResult New(int id)
        {
			var defaultCategory = Category.DefaultCategory(id, orderableService.NextPosition);
            return View("Edit", EditViewData.WithCategory(defaultCategory)); 
        }

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult New(FormCollection form) //TODO: modify DataBind attribute to only fetch optionally. 
		{
			var category = new Category();
			try
			{
				validatingBinder.UpdateFrom(category, form, ModelState, "category");	
			}
			catch(ValidationException ex)
			{
				return View("Edit", EditViewData.WithCategory(category)
					.WithErrorMessage(ex.Message));
			}

			categoryRepository.InsertOnSubmit(category);
			Message = "New category has been added.";

			return this.RedirectToAction(c => c.Index());
		}

        public ActionResult Edit(int id)
        {
            var category = categoryRepository.GetById(id);
            return View("Edit", EditViewData.WithCategory(category));
        }

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Edit([DataBind] Category category) //TODO: remove duplication
		{
			if(ModelState.IsValid)
			{
				return View("Edit", EditViewData.WithCategory(category).WithMessage("The category has been saved."));
			}
			else
			{
				return View("Edit", EditViewData.WithCategory(category));
					//.WithErrorMessage(validationException.Message));
			}
		}

        /*public ActionResult Update(int categoryId)
        {
            var category = categoryId == 0 ? 
                new Category() : 
                categoryRepository.GetById(categoryId);

            try
            {
                validatingBinder.UpdateFrom(category, Request.Form);
            }
            catch (ValidationException validationException)
            {
                return View("Edit", EditViewData.WithCategory(category)
                    .WithErrorMessage(validationException.Message));
            }

            if (categoryId == 0)
            {
                categoryRepository.InsertOnSubmit(category);
            }

            categoryRepository.SubmitChanges();

            return View("Edit", EditViewData.WithCategory(category).WithMessage("The category has been saved"));
        }*/

        private ShopViewData EditViewData
        {
            get
            {
                return ShopView.Data.WithCategories(categoryRepository.GetAll().Alphabetical());
            }
        }

        public ActionResult MoveUp(int id)
        {
            MoveThis(id).UpOne();
            return RenderIndexView();
        }

        public ActionResult MoveDown(int id)
        {
            MoveThis(id).DownOne();
            return RenderIndexView();
        }

        private IOrderServiceWithConstrainedPosition<Category> MoveThis(int id)
        {
            var category = categoryRepository.GetById(id);
            return orderableService
                .MoveItemAtPosition(category.Position)
                .ConstrainedBy(c => c.ParentId == category.ParentId);
        }
    }
}
