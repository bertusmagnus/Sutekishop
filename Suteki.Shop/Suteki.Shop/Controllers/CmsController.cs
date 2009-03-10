using System;
using System.Web;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Filters;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using MvcContrib;
namespace Suteki.Shop.Controllers
{
	public class
		CmsController : ControllerBase
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

		public ActionResult Index(string urlName)
		{
			var content = string.IsNullOrEmpty(urlName)
			              	?
			              		contentRepository.GetAll().DefaultText(null)
			              	:
			              		contentRepository.GetAll().WithUrlName(urlName);

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

			var textContent = new TextContent
			                  {
			                  	Content1 = parentContent,
			                  	IsActive = true,
			                  	ContentTypeId = ContentType.TextContentId,
			                  	Position = contentOrderableService.NextPosition
			                  };

			return View("Edit", GetEditViewData(0).WithContent(textContent));
		}

		[AdministratorsOnly]
		public ActionResult Edit(int id)
		{
			var content = contentRepository.GetById(id);
			return View("Edit", GetEditViewData(id).WithContent(content));
		}

		private CmsViewData GetEditViewData(int contentId)
		{
			var menus = contentRepository.GetAll().NotIncluding(contentId).Menus();
			return CmsView.Data.WithMenus(menus);
		}

		[AdministratorsOnly]
		public ActionResult Update(int id, FormCollection form)
		{
			var content = id == 0
			              	?
			              		CreateContent(form)
			              	:
			              		contentRepository.GetById(id);

			try
			{
				validatingBinder.UpdateFrom(content, form);
				// we spefically want HTML in textContent
				var textContent = content as ITextContent;
				if (textContent != null)
				{
					textContent.Text = HttpUtility.HtmlDecode(textContent.Text);
				}
			}
			catch (ValidationException validationException)
			{
				return View("Edit",
				            GetEditViewData(id).WithContent(content).WithErrorMessage(validationException.Message));
			}

			if (id == 0)
			{
				contentRepository.InsertOnSubmit(content);
			}
			contentRepository.SubmitChanges();

			return View("List", CmsView.Data.WithContent(content.Content1));
		}

		/// <summary>
		/// Create the correct subtype of content based on the contentTypeId returned in the form post
		/// </summary>
		/// <returns></returns>
		private Content CreateContent(FormCollection form)
		{
			Content content;
			var contentTypeId = int.Parse(form["contenttypeid"]);
			switch (contentTypeId)
			{
				case ContentType.TextContentId:
					content = new TextContent();
					break;
				case ContentType.MenuId:
					content = new Menu();
					break;
				default:
					throw new ApplicationException("Unknown ContentTypeId: {0}".With(contentTypeId));
			}
			return content;
		}

		private ActionResult RenderListView(int contentId)
		{
			var menu = contentRepository.GetById(contentId) as Menu;
			return View("List", CmsView.Data.WithContent(menu));
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