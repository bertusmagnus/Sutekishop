using System;
using System.Web.Mvc;
using MvcContrib;
using Suteki.Common;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Common.ViewData;
using Castle.MicroKernel;
using System.Reflection;

namespace Suteki.Common.Controllers
{
    public class ScaffoldController<T> : ConventionController where T : class, IOrderable, new()
    {
        public IKernel Kernel { get; set; }
        public IRepository<T> Repository { get; set; }
        public IOrderableService<T> OrderableService { get; set; }

        public virtual ActionResult Index()
        {
            return RenderIndexView();
        }

        private ActionResult RenderIndexView()
        {
            var items = Repository.GetAll().InOrder();
            return View("Index", ScaffoldView.Data<T>().With(items));
        }

        public virtual ActionResult New()
        {
            T item = new T
                         {
                             Position = OrderableService.NextPosition
                         };
            return View("Edit", BuildEditViewData().With(item));
        }

        [NonAction]
        public virtual ScaffoldViewData<T> BuildEditViewData()
        {
            ScaffoldViewData<T> viewData = ScaffoldView.Data<T>();
            AppendLookupLists(viewData);
            return viewData;
        }

        public virtual ActionResult Edit(int id)
        {
            T item = Repository.GetById(id);
            return View("Edit", BuildEditViewData().With(item));
        }

        public virtual ActionResult Update()
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
                return View("Edit", BuildEditViewData().With(item)
                                        .WithErrorMessage(validationException.Message));
            }
        }

        public virtual ActionResult MoveUp(int id)
        {
            OrderableService.MoveItemAtPosition(id).UpOne();
            return RenderIndexView();
        }

        public virtual ActionResult MoveDown(int id)
        {
            OrderableService.MoveItemAtPosition(id).DownOne();
            return RenderIndexView();
        }

        /// <summary>
        /// Appends any lookup lists T might need for editing
        /// </summary>
        /// <param name="viewData"></param>
        [NonAction]
        public virtual void AppendLookupLists(ScaffoldViewData<T> viewData)
        {
            // find any properties that are attributed as a linq entity
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.PropertyType.IsLinqEntity())
                {
                    AppendLookupList(viewData, property);
                }
            }
        }

        private void AppendLookupList(ScaffoldViewData<T> viewData, PropertyInfo property)
        {
            if (Kernel == null)
            {
                throw new ApplicationException("The Kernel property must be set before AppendLookupLists is called");
            }

            // get the repository for this Entity
            Type repositoryType = typeof(IRepository<>).MakeGenericType(new[] { property.PropertyType });

            object repository = Kernel.Resolve(repositoryType);
            if (repository == null)
            {
                throw new ApplicationException(StringExtensions.With("Could not find IRepository<{0}> in kernel", property.PropertyType));
            }

            MethodInfo getAllMethod = repositoryType.GetMethod("GetAll");

            // get the items
            object items = getAllMethod.Invoke(repository, new object[] { });

            // add the items to the viewData
            viewData.WithLookupList(property.PropertyType, items);
        }
    }
}