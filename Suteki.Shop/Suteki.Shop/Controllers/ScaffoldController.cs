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
using Castle.MicroKernel;
using System.Reflection;

namespace Suteki.Shop.Controllers
{
    public class ScaffoldController<T> : ControllerBase where T : class, IOrderable, new()
    {
        public IKernel Kernel { get; set; }
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
            return RenderView("Edit", BuildEditViewData().With(item));
        }

        public ScaffoldViewData<T> BuildEditViewData()
        {
            ScaffoldViewData<T> viewData = Scaffold.Data<T>();
            AppendLookupLists(viewData);
            return viewData;
        }

        public ActionResult Edit(int id)
        {
            T item = Repository.GetById(id);
            return RenderView("Edit", BuildEditViewData().With(item));
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
                return RenderView("Edit", BuildEditViewData().With(item)
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

        /// <summary>
        /// Appends any lookup lists T might need for editing
        /// </summary>
        /// <param name="viewData"></param>
        public void AppendLookupLists(ScaffoldViewData<T> viewData)
        {
            // find any properties that implement IEntity
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (typeof(IEntity).IsAssignableFrom(property.PropertyType))
                {
                    if (Kernel == null)
                    {
                        throw new ApplicationException("The Kernel property must be set before AppendLookupLists is called");
                    }

                    // get the repository for this Entity
                    Type repositoryType = typeof(IRepository<>).MakeGenericType(new Type[] { property.PropertyType });
                    object repository = Kernel.Resolve(repositoryType);
                    if (repository == null)
                    {
                        throw new ApplicationException("Could not find IRepository<{0}> in kernel".With(property.PropertyType));
                    }

                    MethodInfo getAllMethod = repositoryType.GetMethod("GetAll");

                    // get the items
                    object items = getAllMethod.Invoke(repository, new object[] { });

                    // add the items to the viewData
                    viewData.WithLookupList(property.PropertyType, items);
                }
            }
        }
    }
}
