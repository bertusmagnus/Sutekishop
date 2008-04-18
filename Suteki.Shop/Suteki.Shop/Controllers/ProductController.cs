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

        public ProductController(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IHttpFileService httpFileService)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.httpFileService = httpFileService;
        }

        public void Index(int id)
        {
            Category category = categoryRepository.GetById(id);
            var products = productRepository.GetAll().WhereCategoryIdIs(id);

            RenderView("Index", View.Data.WithProducts(products).WithCategory(category)); 
        }

        public void Item(int id)
        {
            Product product = productRepository.GetById(id);

            RenderView("Item", View.Data.WithProduct(product)); 
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void New(int id)
        {
            Product defaultProduct = new Product
            {
                ProductId = 0,
                CategoryId = id,
            };

            RenderView("Edit", EditViewData.WithProduct(defaultProduct)); 
        }

        [PostOnly]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void Update(int productId)
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
            }
            catch (ValidationException validationException)
            {
                RenderView("Edit", EditViewData.WithProduct(product).WithErrorMessage(validationException.Message));
                return;
            }

            if (productId == 0)
            {
                productRepository.InsertOnSubmit(product);
            }
            
            productRepository.SubmitChanges();

            RenderView("Edit", EditViewData.WithProduct(product).WithMessage("This product has been saved"));
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
        public void Edit(int id)
        {
            Product product = productRepository.GetById(id);
            RenderView("Edit", EditViewData.WithProduct(product));
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
