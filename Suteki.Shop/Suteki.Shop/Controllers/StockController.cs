using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Shop.Filters;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Controllers
{
	[AdministratorsOnly]
    public class StockController : ControllerBase
    {
        IRepository<Category> categoryRepository;
        IRepository<Size> sizeRepository;

        public StockController(
            IRepository<Category> categoryRepository,
            IRepository<Size> sizeRepository)
        {
            this.categoryRepository = categoryRepository;
            this.sizeRepository = sizeRepository;
        }

        public ActionResult Index()
        {
            return RenderIndexView();
        }

        private ActionResult RenderIndexView()
        {
            Category root = categoryRepository.GetRootCategory();
            return View("Index", ShopView.Data.WithCategory(root));
        }

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork]
        public ActionResult Index(FormCollection form)
        {
            var sizes = sizeRepository.GetAll().ToList();
            UpdateFromForm(sizes, form);
            return RenderIndexView();
        }

        private static void UpdateFromForm(IEnumerable<Size> sizes, NameValueCollection form)
        {
            foreach (var size in sizes)
            {
                if (form["stockitem_{0}".With(size.SizeId)] != null)
                {
                    size.IsInStock = form["stockitem_{0}".With(size.SizeId)].Contains("true");
                }
            }
        }
    }
}
