using System;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Extensions;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Filters;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using MvcContrib;
namespace Suteki.Shop.Controllers
{
	public class MenuController : ControllerBase
	{
		private readonly IRepository<Menu> menuRepository;
		private readonly IRepository<Category> categoryRepository;
		private readonly IOrderableService<Content> contentOrderableService;

		public MenuController(IRepository<Menu> menuRepository, IRepository<Category> categoryRepository, IOrderableService<Content> contentOrderableService)
		{
			this.menuRepository = menuRepository;
			this.contentOrderableService = contentOrderableService;
			this.categoryRepository = categoryRepository;
		}

		public ActionResult MainMenu()
		{
			return View(menuRepository.GetById(1));
		}

		public ActionResult LeftMenu()
		{
			return View(categoryRepository.GetRootCategory());
		}

		[AdministratorsOnly]
		public ViewResult Edit(int id)
		{
			return View(GetEditViewData(id).WithContent(menuRepository.GetById(id)));
		}

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Edit([DataBind] Menu content)
		{
			if(ModelState.IsValid)
			{
				Message = "Changes have been saved.";
				return this.RedirectToAction<CmsController>(c => c.List(content.ParentContentId.Value));
			}

			return View("Edit", GetEditViewData(content.ParentContentId.Value).WithContent(content));
		}

		[AdministratorsOnly]
		public ViewResult New(int id)
		{
			var parentMenu = menuRepository.GetById(id);

			if (parentMenu == null) 
			{
				throw new ApplicationException("Content with id = {0} is not a menu".With(id));
			}

			var menu = Menu.CreateDefaultMenu(contentOrderableService.NextPosition, parentMenu);
			return View("Edit", GetEditViewData(0).WithContent(menu));
		}

		[AcceptVerbs(HttpVerbs.Post), AdministratorsOnly, UnitOfWork]
		public ActionResult New([DataBind(Fetch=false)] Menu content)
		{
			if(ModelState.IsValid)
			{
				menuRepository.InsertOnSubmit(content);
				Message = "New menu has been successfully added.";
				return this.RedirectToAction<CmsController>(c => c.List(content.ParentContentId.Value));
			}
			
			return View("Edit", GetEditViewData(content.ParentContentId.Value).WithContent(content));
		}

		private CmsViewData GetEditViewData(int contentId) 
		{
			var menus = menuRepository.GetAll().NotIncluding(contentId);
			return CmsView.Data.WithMenus(menus);
		}
	}
}