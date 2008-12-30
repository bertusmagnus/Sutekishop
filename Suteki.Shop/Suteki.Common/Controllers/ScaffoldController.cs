using System.Collections.Specialized;
using System.Reflection;
using System.Web.Mvc;
using MvcContrib;
using MvcContrib.Pagination;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Common.ViewData;

namespace Suteki.Common.Controllers
{
    public class ScaffoldController<T> : ConventionController where T : class, new()
    {
        public IRepository<T> Repository { get; set; }
        public IRepositoryResolver repositoryResolver { get; set; }
        public IValidatingBinder ValidatingBinder { get; set; }
        public IHttpContextService httpContextService { get; set; }

        public virtual ActionResult Index(int? page)
        {
            return RenderIndexView(page);
        }

        protected virtual ActionResult RenderIndexView(int? page)
        {
            var items = Repository.GetAll().AsPagination(page ?? 1);
            return View("Index", ScaffoldView.Data<T>().With(items));
        }

        public virtual ActionResult New()
        {
            var item = new T();
            return View("Edit", BuildEditViewData().With(item));
        }

        [NonAction]
        public virtual ScaffoldViewData<T> BuildEditViewData()
        {
            var viewData = ScaffoldView.Data<T>();
            AppendLookupLists(viewData);
            return viewData;
        }

        public virtual ActionResult Edit(int id)
        {
            T item = Repository.GetById(id);
            return View("Edit", BuildEditViewData().With(item));
        }

        public virtual ActionResult Delete(int id, int? page)
        {
            T item = Repository.GetById(id);
            Repository.DeleteOnSubmit(item);
            Repository.SubmitChanges();

            return RedirectToAction("Index", new {page});
        }

        public virtual ActionResult Update(FormCollection form)
        {
            var id = GetPrimaryKey();

            var item = id == 0 ? 
                new T() : 
                Repository.GetById(id);

            try
            {
                ValidatingBinder.UpdateFrom(item, form, ModelState);
                if (id == 0)
                {
                    Repository.InsertOnSubmit(item);
                }
                Repository.SubmitChanges();

                return RedirectToAction("Index");
            }
            catch (ValidationException validationException)
            {
                return View("Edit", BuildEditViewData().With(item).WithErrorMessage(validationException.Message));
            }
        }

        public virtual NameValueCollection Form
        {
            get
            {
                return Request.Form;
            }
        }

        public virtual int GetPrimaryKey()
        {
            return int.Parse(httpContextService.FormOrQuerystring[typeof(T).GetPrimaryKey().Name]);
        }

        /// <summary>
        /// Appends any lookup lists T might need for editing
        /// </summary>
        /// <param name="viewData"></param>
        [NonAction]
        public virtual void AppendLookupLists(ScaffoldViewData<T> viewData)
        {
            // find any properties that are attributed as a linq entity
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.PropertyType.IsLinqEntity())
                {
                    AppendLookupList(viewData, property);
                }
            }
        }

        private void AppendLookupList(ScaffoldViewData<T> viewData, PropertyInfo property)
        {
            var repository = repositoryResolver.GetRepository(property.PropertyType);

            // get the items
            object items = repository.GetAll();

            // add the items to the viewData
            viewData.WithLookupList(property.PropertyType, items);
        }
    }
}