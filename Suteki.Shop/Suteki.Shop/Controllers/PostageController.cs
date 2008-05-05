using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;
using Suteki.Shop.Validation;

namespace Suteki.Shop.Controllers
{
    public class PostageController : ControllerBase
    {
        IRepository<Postage> postageRepository;
        IOrderableService<Postage> orderableService;

        public PostageController(
            IRepository<Postage> postageRepository,
            IOrderableService<Postage> orderableService)
        {
            this.postageRepository = postageRepository;
            this.orderableService = orderableService;
        }

        public ActionResult Index()
        {
            return RenderIndexView();
        }

        private ActionResult RenderIndexView()
        {
            var postages = postageRepository.GetAll().InOrder();
            return RenderView("Index", View.Data.WithPostages(postages));
        }

        public ActionResult New()
        {
            Postage postage = new Postage
            {
                IsActive = true,
                Position = orderableService.NextPosition
            };
            return RenderView("Edit", View.Data.WithPostage(postage));
        }

        public ActionResult Edit(int id)
        {
            Postage postage = postageRepository.GetById(id);
            return RenderView("Edit", View.Data.WithPostage(postage));
        }

        public ActionResult Update(int postageId)
        {
            Postage postage = null;
            if (postageId == 0)
            {
                postage = new Postage();
            }
            else
            {
                postage = postageRepository.GetById(postageId);
            }

            try
            {
                ValidatingBinder.UpdateFrom(postage, Request.Form);
                if (postageId == 0)
                {
                    postageRepository.InsertOnSubmit(postage);
                }
                postageRepository.SubmitChanges();

                return RenderIndexView();
            }
            catch (ValidationException validationException)
            {
                return RenderView("Edit", View.Data
                    .WithPostage(postage)
                    .WithErrorMessage(validationException.Message));
            }
        }

        public ActionResult MoveUp(int id)
        {
            orderableService.MoveItemAtPosition(id).UpOne();
            return RenderIndexView();
        }

        public ActionResult MoveDown(int id)
        {
            orderableService.MoveItemAtPosition(id).DownOne();
            return RenderIndexView();
        }
    }
}
