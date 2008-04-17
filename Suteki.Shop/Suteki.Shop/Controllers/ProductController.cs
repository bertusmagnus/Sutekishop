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

namespace Suteki.Shop.Controllers
{
    public class ProductController : ControllerBase
    {
        IRepository<Product> productRepository;
        IRepository<Category> categoryRepository;

        public ProductController(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
        }

        public void Index(int id)
        {
            Category category = categoryRepository.GetById(id);
            var products = productRepository.GetAll().WhereCategoryIdIs(id);

            RenderView("Index", new ProductListViewData 
            { 
                Products = products,
                Category = category
            });
        }

        public void Item(int id)
        {
            Product product = productRepository.GetById(id);

            RenderView("Item", new ProductItemViewData { Product = product });
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void New(int id)
        {
            Product defaultProduct = new Product
            {
                ProductId = 0,
                CategoryId = id,
            };

            RenderView("Edit", new ProductItemViewData 
            { 
                Product = defaultProduct,
                Categories = categoryRepository.GetAll().Alphabetical()
            });
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
            }
            catch (ValidationException validationException)
            {
                RenderView("Edit", new ProductItemViewData
                {
                    ErrorMessage = validationException.Message,
                    Product = product,
                    Categories = categoryRepository.GetAll().Alphabetical()
                });
                return;
            }

            if (productId == 0)
            {
                productRepository.InsertOnSubmit(product);
            }
            
            productRepository.SubmitChanges();

            RenderView("Edit", new ProductItemViewData
                {
                    Message = "This product has been saved",
                    Product = product,
                    Categories = categoryRepository.GetAll().Alphabetical()
                });
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void Edit(int id)
        {
            Product product = productRepository.GetById(id);

            RenderView("Edit", new ProductItemViewData
            {
                Product = product,
                Categories = categoryRepository.GetAll().Alphabetical()
            });
        }
    }
}
