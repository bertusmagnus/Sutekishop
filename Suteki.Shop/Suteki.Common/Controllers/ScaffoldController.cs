using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.Mvc;
using MvcContrib;
using MvcContrib.Pagination;
using Suteki.Common.Binders;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Common.ViewData;

namespace Suteki.Common.Controllers
{
    public class ScaffoldController<T> : Controller where T : class, new()
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

		
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult New([DataBind(Fetch = false)] T item)
		{
			if(ModelState.IsValid)
			{
				Repository.InsertOnSubmit(item);
				TempData["message"] = "Item successfully added."; //Make use of the CopyMessageFromTempDataToViewData filter to show this in the view.
				return RedirectToAction("Index"); //can't use strongly typed redirect here or the wrong controller name will be picked up	
			}

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

		[AcceptVerbs(HttpVerbs.Post)]
		public virtual ActionResult Edit([DataBind] T item)
		{
			if(ModelState.IsValid)
			{
				TempData["message"] = "Item successfully updated.";
				return RedirectToAction("Index");
			}

			return View("Edit", BuildEditViewData().With(item));
		}

        public virtual ActionResult Delete(int id, int? page)
        {
            T item = Repository.GetById(id);
            Repository.DeleteOnSubmit(item);
            //Repository.SubmitChanges();

            return RedirectToAction("Index", new {page});
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