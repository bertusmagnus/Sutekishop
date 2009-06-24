using System.Linq;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Filters;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using MvcContrib;

namespace Suteki.Shop.Controllers
{
	[AdministratorsOnly]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IOrderableService<Category> orderableService;
		private readonly IHttpFileService httpFileService;

        public CategoryController(IRepository<Category> categoryRepository, IOrderableService<Category> orderableService, IHttpFileService httpFileService)
        {
            this.categoryRepository = categoryRepository;
        	this.httpFileService = httpFileService;
        	this.orderableService = orderableService;
        }

        public ActionResult Index()
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
		public ActionResult New([DataBind(Fetch = false)] Category category)
		{
			if(! ModelState.IsValid)
			{
				return View("Edit", EditViewData.WithCategory(category));
			}

			var image = httpFileService.GetUploadedImages(Request, ImageDefinition.CategoryImage).SingleOrDefault();

			if(image != null)
			{
				category.Image = image;
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
		public ActionResult Edit([DataBind] Category category)
		{
			var viewData = EditViewData.WithCategory(category);

			if(ModelState.IsValid)
			{
				Message = "The category has been saved.";
				return this.RedirectToAction(c => c.Index());
			}
			else
			{
				return View(viewData);					
			}
		}

        private ShopViewData EditViewData
        {
            get
            {
                return ShopView.Data.WithCategories(categoryRepository.GetAll().Alphabetical());
            }
        }

		[UnitOfWork]
        public ActionResult MoveUp(int id)
        {
            MoveThis(id).UpOne();
			return this.RedirectToAction(c => c.Index());
        }

		[UnitOfWork]
        public ActionResult MoveDown(int id)
        {
            MoveThis(id).DownOne();
			return this.RedirectToAction(c => c.Index());
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
