using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using Suteki.Shop.Extensions;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Controllers
{
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

        public ActionResult Update()
        {
            var sizes = sizeRepository.GetAll().ToList();
            UpdateFromForm(sizes, Form);
            sizeRepository.SubmitChanges();

            return RenderIndexView();
        }

        private void UpdateFromForm(IEnumerable<Size> sizes, NameValueCollection form)
        {
            foreach (Size size in sizes)
            {
                size.IsInStock = form["stockitem_{0}".With(size.SizeId)] != null;
            }
        }
    }
}
