using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;
using Suteki.Shop.Services;
using System.Security.Permissions;
using MvcContrib.Filters;

namespace Suteki.Shop.Controllers
{
    public class CategoryController : ControllerBase
    {
        IRepository<Category> categoryRepository;
        IOrderableService<Category> orderableService;

        public CategoryController(
            IRepository<Category> categoryRepository,
            IOrderableService<Category> orderableService)
        {
            this.categoryRepository = categoryRepository;
            this.orderableService = orderableService;
        }

        public ActionResult Index()
        {
            return RenderIndexView();
        }

        private ActionResult RenderIndexView()
        {
            Category root = categoryRepository.GetRootCategory();
            return RenderView("Index", View.Data.WithCategory(root));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult New()
        {
            Category defaultCategory = new Category 
            { 
                ParentId = 1,
                Position = orderableService.NextPosition
            };
            return RenderView("Edit", EditViewData.WithCategory(defaultCategory)); 
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Edit(int id)
        {
            Category category = categoryRepository.GetById(id);
            return RenderView("Edit", EditViewData.WithCategory(category));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Update(int categoryId)
        {
            Category category = null;
            if (categoryId == 0)
            {
                category = new Category();
            }
            else
            {
                category = categoryRepository.GetById(categoryId);
            }

            try
            {
                ValidatingBinder.UpdateFrom(category, Request.Form);
            }
            catch (ValidationException validationException)
            {
                return RenderView("Edit", EditViewData.WithCategory(category)
                    .WithErrorMessage(validationException.Message));
            }

            if (categoryId == 0)
            {
                categoryRepository.InsertOnSubmit(category);
            }

            categoryRepository.SubmitChanges();

            return RenderView("Edit", EditViewData.WithCategory(category).WithMessage("The category has been saved"));
        }

        private ShopViewData EditViewData
        {
            get
            {
                return View.Data.WithCategories(categoryRepository.GetAll().Alphabetical());
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult MoveUp(int id)
        {
            MoveThis(id).UpOne();
            return RenderIndexView();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult MoveDown(int id)
        {
            MoveThis(id).DownOne();
            return RenderIndexView();
        }

        private IOrderServiceWithConstrainedPosition<Category> MoveThis(int id)
        {
            Category category = categoryRepository.GetById(id);
            return orderableService
                .MoveItemAtPosition(category.Position)
                .ConstrainedBy(c => c.ParentId == category.ParentId);
        }
    }
}
