using System;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
    public class CmsController : ControllerBase
    {
        IRepository<Content> contentRepository;
        IOrderableService<Content> contentOrderableService;

        public CmsController(
            IRepository<Content> contentRepository,
            IOrderableService<Content> contentOrderableService)
        {
            this.contentRepository = contentRepository;
            this.contentOrderableService = contentOrderableService;
        }

        public override string GetControllerName()
        {
            return "";
        }

        public ActionResult Index(string urlName)
        {
            Content content;

            if (string.IsNullOrEmpty(urlName))
            {
                content = contentRepository.GetAll().DefaultText();
            }
            else
            {
                content = contentRepository.GetAll().WithUrlName(urlName);
            }

            if (content is Menu)
            {
                content = contentRepository.GetAll()
                    .WithParent(content)
                    .DefaultText();
            }

            if (content is ActionContent)
            {
                ActionContent actionContent = content as ActionContent;
                return RedirectToAction(actionContent.Action, actionContent.Controller);
            }

            AppendTitle(content.Name);

            if (content is TopContent)
            {
                return View("TopPage", CmsView.Data.WithContent(content));
            }

            return View("SubPage", CmsView.Data.WithContent(content));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Add(int id)
        {
            TextContent textContent = new TextContent
            {
                ParentContentId = id,
                IsActive = true,
                ContentTypeId = ContentType.TextContentId,
                Position = contentOrderableService.NextPosition
            };

            return View("Edit", GetEditViewData(0).WithContent(textContent));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Edit(int id)
        {
            Content content = contentRepository.GetById(id);
            return View("Edit", GetEditViewData(id).WithContent(content));
        }

        private CmsViewData GetEditViewData(int contentId)
        {
                var menus = contentRepository.GetAll().NotIncluding(contentId).Menus();
                return CmsView.Data.WithMenus(menus);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Update(int id)
        {
            Content content = null;
            if (id == 0)
            {
                content = CreateContent();
            }
            else
            {
                content = contentRepository.GetById(id);
            }

            try
            {
                ValidatingBinder.UpdateFrom(content, Form);
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
        private Content CreateContent()
        {
            Content content = null;
            int contentTypeId = int.Parse(this.Form["contenttypeid"]);
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

        public ActionResult List(int id)
        {
            return RenderListView(id);
        }

        private ActionResult RenderListView(int contentId)
        {
            Menu menu = contentRepository.GetById(contentId) as Menu;
            return View("List", CmsView.Data.WithContent(menu));
        }

        public ActionResult MoveUp(int id)
        {
            Content content = contentRepository.GetById(id);

            contentOrderableService
                .MoveItemAtPosition(content.Position)
                .ConstrainedBy(c => c.ParentContentId == content.ParentContentId)
                .UpOne();

            return RenderListView(content.ParentContentId.Value);
        }

        public ActionResult MoveDown(int id)
        {
            Content content = contentRepository.GetById(id);

            contentOrderableService
                .MoveItemAtPosition(content.Position)
                .ConstrainedBy(c => c.ParentContentId == content.ParentContentId)
                .DownOne();

            return RenderListView(content.ParentContentId.Value);
        }

        public ActionResult NewMenu(int id)
        {
            Menu menu = new Menu
            {
                ContentTypeId = ContentType.MenuId,
                ParentContentId = id,
                IsActive = true,
                Position = contentOrderableService.NextPosition
            };

            return View("Edit", CmsView.Data.WithContent(menu));
        }
    }
}
