﻿using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
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

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult New(int id)
        {
            var defaultCategory = new Category 
            { 
                ParentId = id,
                Position = orderableService.NextPosition
            };
            return View("Edit", EditViewData.WithCategory(defaultCategory)); 
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Edit(int id)
        {
            var category = categoryRepository.GetById(id);
            return View("Edit", EditViewData.WithCategory(category));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Update(int categoryId)
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
        }

        private ShopViewData EditViewData
        {
            get
            {
                return ShopView.Data.WithCategories(categoryRepository.GetAll().Alphabetical());
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
            var category = categoryRepository.GetById(id);
            return orderableService
                .MoveItemAtPosition(category.Position)
                .ConstrainedBy(c => c.ParentId == category.ParentId);
        }
    }
}
