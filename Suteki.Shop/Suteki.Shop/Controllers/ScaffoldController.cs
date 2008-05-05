using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;
using Suteki.Shop.Validation;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Controllers
{
    public class ScaffoldController<T> : ControllerBase where T : class, IOrderable, new()
    {
        public IRepository<T> Repository { get; set; }
        public IOrderableService<T> OrderableService { get; set; }

        public ActionResult Index()
        {
            return RenderIndexView();
        }

        private ActionResult RenderIndexView()
        {
            var items = Repository.GetAll().InOrder();
            return RenderView("Index", Scaffold.Data<T>().With(items));
        }

        public ActionResult New()
        {
            T item = new T
            {
                Position = OrderableService.NextPosition
            };
            return RenderView("Edit", Scaffold.Data<T>().With(item));
        }

        public ActionResult Edit(int id)
        {
            T item = Repository.GetById(id);
            return RenderView("Edit", Scaffold.Data<T>().With(item));
        }

        public ActionResult Update()
        {
            int id = int.Parse(this.ReadFromRequest(typeof(T).GetPrimaryKey().Name));
            T item = null;

            if (id == 0)
            {
                item = new T();
            }
            else
            {
                item = Repository.GetById(id);
            }

            try
            {
                ValidatingBinder.UpdateFrom(item, Request.Form);
                if (id == 0)
                {
                    Repository.InsertOnSubmit(item);
                }
                Repository.SubmitChanges();

                return RenderIndexView();
            }
            catch (ValidationException validationException)
            {
                return RenderView("Edit", Scaffold.Data<T>().With(item)
                    .WithErrorMessage(validationException.Message));
            }
        }

        public ActionResult MoveUp(int id)
        {
            OrderableService.MoveItemAtPosition(id).UpOne();
            return RenderIndexView();
        }

        public ActionResult MoveDown(int id)
        {
            OrderableService.MoveItemAtPosition(id).DownOne();
            return RenderIndexView();
        }
    }
}
