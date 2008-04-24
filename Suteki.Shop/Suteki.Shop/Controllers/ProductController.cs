using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;
using System.Security.Permissions;
using MvcContrib.Filters;
using Suteki.Shop.Services;

namespace Suteki.Shop.Controllers
{
    public class ProductController : ControllerBase
    {
        IRepository<Product> productRepository;
        IRepository<Category> categoryRepository;
        IHttpFileService httpFileService;
        ISizeService sizeService;
        IOrderableService<Product> productOrderableService;

        public ProductController(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IHttpFileService httpFileService,
            ISizeService sizeService,
            IOrderableService<Product> productOrderableService)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.httpFileService = httpFileService;
            this.sizeService = sizeService;
            this.productOrderableService = productOrderableService;
        }

        public ActionResult Index(int id)
        {
            return RenderIndexView(id);
        }

        private ActionResult RenderIndexView(int id)
        {
            Category category = categoryRepository.GetById(id);
            return RenderView("Index", View.Data.WithProducts(category.Products.InOrder()).WithCategory(category));
        }

        public ActionResult Item(int id)
        {
            Product product = productRepository.GetById(id);
            return RenderView("Item", View.Data.WithProduct(product)); 
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult New(int id)
        {
            Product defaultProduct = new Product
            {
                ProductId = 0,
                CategoryId = id,
                Position = productOrderableService.NextPosition
            };

            return RenderView("Edit", EditViewData.WithProduct(defaultProduct)); 
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Update(int productId)
        {
            Product product = null;
            if (productId == 0)
            {
                product = new Product();
            }
            else
            {
                product = productRepository.GetById(productId);
            }

            try
            {
                ValidatingBinder.UpdateFrom(product, Request.Form);
                UpdateImages(product, Request);
                sizeService.WithVaues(Request.Form).Update(product);
            }
            catch (ValidationException validationException)
            {
                return RenderView("Edit", EditViewData.WithProduct(product).WithErrorMessage(validationException.Message));
            }

            if (productId == 0)
            {
                productRepository.InsertOnSubmit(product);
            }
            
            productRepository.SubmitChanges();

            return RenderView("Edit", EditViewData.WithProduct(product).WithMessage("This product has been saved"));
        }

        private void UpdateImages(Product product, HttpRequestBase request)
        {
            IEnumerable<Image> images = httpFileService.GetUploadedImages(request);
            foreach (Image image in images)
            {
                product.ProductImages.Add(new ProductImage 
                {
                    Image = image
                });
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult Edit(int id)
        {
            Product product = productRepository.GetById(id);
            return RenderView("Edit", EditViewData.WithProduct(product));
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult MoveUp(int id, int position)
        {
            productOrderableService.MoveItemAtPosition(position).UpOne();
            return RenderIndexView(id);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public ActionResult MoveDown(int id, int position)
        {
            productOrderableService.MoveItemAtPosition(position).DownOne();
            return RenderIndexView(id);
        }

        public ShopViewData EditViewData
        {
            get
            {
                return View.Data.WithCategories(categoryRepository.GetAll().Alphabetical());
            }
        }
    }
}
