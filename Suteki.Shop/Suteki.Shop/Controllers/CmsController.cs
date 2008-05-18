using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using Suteki.Shop.Validation;
using Suteki.Shop.Extensions;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
    public class CmsController : ControllerBase
    {
        IRepository<Content> contentRepository;

        public CmsController(IRepository<Content> contentRepository)
        {
            this.contentRepository = contentRepository;
        }

        public ActionResult Index(string urlName)
        {
            TextContent content;

            if (string.IsNullOrEmpty(urlName))
            {
                content = contentRepository.GetAll().OfType<TextContent>().InOrder().First();
            }
            else
            {
                content = contentRepository.GetAll().WithUrlName(urlName);
            }

            return RenderView("Index", CmsView.Data.WithTextContent(content));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Add(int id)
        {
            TextContent textContent = new TextContent
            {
                MenuId = id,
                IsActive = true,
                ContentTypeId = ContentType.TextId
            };

            return RenderView("Edit", CmsView.Data.WithTextContent(textContent));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Edit(int id)
        {
            TextContent content = contentRepository.GetById(id) as TextContent;
            if (content == null) throw new ApplicationException("Content with id = {0} is not TextContent".With(id));

            return RenderView("Edit", CmsView.Data.WithTextContent(content));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Update(int id)
        {
            TextContent content = null;
            if (id == 0)
            {
                content = new TextContent();
            }
            else
            {
                content = (TextContent)contentRepository.GetById(id);
                if (content == null) throw new ApplicationException("content, id = {0} is not TextContent".With(id));
            }

            try
            {
                ValidatingBinder.UpdateFrom(content, Form);
            }
            catch (ValidationException validationException)
            {
                return RenderView("Edit", 
                    CmsView.Data.WithTextContent(content).WithErrorMessage(validationException.Message));
            }

            if (id == 0)
            {
                contentRepository.InsertOnSubmit(content);
            }
            contentRepository.SubmitChanges();

            return RenderView("Index", CmsView.Data.WithTextContent(content));
        }
    }
}
