using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;
using System.Security.Permissions;
using MvcContrib.Filters;

namespace Suteki.Shop.Controllers
{
    public class CategoryController : ControllerBase
    {
        IRepository<Category> categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public void Index()
        {
            Category root = categoryRepository.GetRootCategory();
            RenderView("Index", View.Data.WithCategory(root));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void New()
        {
            Category defaultCategory = new Category { ParentId = 1 };
            RenderView("Edit", EditViewData.WithCategory(defaultCategory)); 
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void Edit(int id)
        {
            Category category = categoryRepository.GetById(id);
            RenderView("Edit", EditViewData.WithCategory(category));
        }

        [PostOnly]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void Update(int categoryId)
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
                RenderView("Edit", EditViewData.WithCategory(category)
                    .WithErrorMessage(validationException.Message));
                return;
            }

            if (categoryId == 0)
            {
                categoryRepository.InsertOnSubmit(category);
            }

            categoryRepository.SubmitChanges();

            RenderView("Edit", EditViewData.WithCategory(category).WithMessage("The category has been saved"));
        }

        private ShopViewData EditViewData
        {
            get
            {
                return View.Data.WithCategories(categoryRepository.GetAll().Alphabetical());
            }
        }
    }
}
