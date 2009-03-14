using System;
using System.Web;
using System.Web.Mvc;
using MvcContrib;
using Suteki.Common.Binders;
using Suteki.Common.Extensions;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Filters;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Controllers
{
	[ValidateInput(false)] //Html must be allowed in the Save actions. 
	public class CmsController : ControllerBase
	{
		public readonly IRepository<Content> contentRepository;
		public readonly IOrderableService<Content> contentOrderableService;
		public readonly IValidatingBinder validatingBinder;

		public CmsController(
			IRepository<Content> contentRepository,
			IOrderableService<Content> contentOrderableService,
			IValidatingBinder validatingBinder)
		{
			this.contentRepository = contentRepository;
			this.contentOrderableService = contentOrderableService;
			this.validatingBinder = validatingBinder;
		}

		public override string GetControllerName()
		{
			return "";
		}

		//TODO: Possibly look at slimming down this action.
		public ActionResult Index(string urlName)
		{
			var content = string.IsNullOrEmpty(urlName)
			              	? contentRepository.GetAll().DefaultText(null)
			              	: contentRepository.GetAll().WithUrlName(urlName);

			if (content is Menu)
			{
				content = contentRepository.GetAll()
					.WithParent(content)
					.DefaultText(content as Menu);
			}

			if (content is ActionContent)
			{
				var actionContent = content as ActionContent;
				return RedirectToAction(actionContent.Action, actionContent.Controller);
			}

			AppendTitle(content.Name);

			if (content is TopContent)
			{
				return View("TopPage", CmsView.Data.WithContent(content));
			}

			return View("SubPage", CmsView.Data.WithContent(content));
		}

		[AdministratorsOnly]
		public ActionResult Add(int id)
		{
			var parentContent = contentRepository.GetById(id);
			var textContent = TextContent.DefaultTextContent(parentContent, contentOrderableService.NextPosition);
			return View("Edit", GetEditViewData(0).WithContent(textContent));
		}

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Add([DataBind(Fetch = false)] TextContent content)
		{
			if(ModelState.IsValid)
			{
				content.Text = HttpUtility.HtmlDecode(content.Text);
				contentRepository.InsertOnSubmit(content);
				Message = "Changes have been saved.";
				return this.RedirectToAction<MenuController>(c => c.List(content.ParentContentId.Value));
			}

			return View("Edit", GetEditViewData(content.ContentId).WithContent(content));
		}

		[AdministratorsOnly]
		public ActionResult Edit(int id)
		{
			var content = contentRepository.GetById(id);
			return View("Edit", GetEditViewData(id).WithContent(content));
		}

		[AdministratorsOnly, UnitOfWork, AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Edit([DataBind] Content content)
		{
			var textContent = (TextContent)content; //will always be TextContent
			if (ModelState.IsValid)
			{
				textContent.Text = HttpUtility.HtmlDecode(textContent.Text);
				Message = "Changes have been saved.";
				return this.RedirectToAction<MenuController>(c => c.List(content.ParentContentId.Value));
			}

			//Error
			return View(GetEditViewData(content.ContentId).WithContent(content));
		}

		CmsViewData GetEditViewData(int contentId)
		{
			var menus = contentRepository.GetAll().NotIncluding(contentId).Menus();
			return CmsView.Data.WithMenus(menus);
		}

		[AdministratorsOnly]
		public ActionResult MoveUp(int id)
		{
			var content = contentRepository.GetById(id);

			contentOrderableService
				.MoveItemAtPosition(content.Position)
				.ConstrainedBy(c => c.ParentContentId == content.ParentContentId)
				.UpOne();

			return this.RedirectToAction<MenuController>(c => c.List(content.ParentContentId.Value));
		}

		[AdministratorsOnly]
		public ActionResult MoveDown(int id)
		{
			var content = contentRepository.GetById(id);

			contentOrderableService
				.MoveItemAtPosition(content.Position)
				.ConstrainedBy(c => c.ParentContentId == content.ParentContentId)
				.DownOne();

			return this.RedirectToAction<MenuController>(c => c.List(content.ParentContentId.Value));
		}
	}
}