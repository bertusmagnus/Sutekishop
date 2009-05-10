using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Services;

namespace Suteki.Shop.Binders
{
	public class BindProductAttribute : DataBindAttribute
	{
		public BindProductAttribute() : base(typeof (ProductBinder))
		{
		}
	}

	public class ProductBinder : DataBinder
	{
		readonly IRepository<Product> repository;
		readonly IHttpFileService httpFileService;
		readonly IOrderableService<ProductImage> orderableService;
		readonly ISizeService sizeService;

		public ProductBinder(IValidatingBinder validatingBinder, IRepositoryResolver resolver, IRepository<Product> repository, IHttpFileService httpFileService, IOrderableService<ProductImage> orderableService, ISizeService sizeService)
			: base(validatingBinder, resolver)
		{
			this.repository = repository;
			this.httpFileService = httpFileService;
			this.orderableService = orderableService;
			this.sizeService = sizeService;
		}

		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var product = base.BindModel(controllerContext, bindingContext) as Product;

			if(product != null)
			{
				UpdateImages(product, controllerContext.HttpContext.Request);
				CheckForDuplicateNames(product, bindingContext);
				UpdateSizes(controllerContext.HttpContext.Request.Form, product);
				UpdateCategories(product, bindingContext);
			}
	
			return product;
		}

		void UpdateCategories(Product product, ModelBindingContext context)
		{
			product.ProductCategories.Clear();

			var valueProviderResult = context.ValueProvider["categories"];
			
			if(valueProviderResult != null)
			{
				var categoryIds = valueProviderResult.ConvertTo(typeof(int[])) as int[];

				if(categoryIds != null)
				{
					foreach (var id in categoryIds) 
					{
						product.ProductCategories.Add(new ProductCategory { CategoryId = id });
					}
				}
			}

			if(product.ProductCategories.Count == 0)
			{
				context.ModelState.AddModelError("categories", "Please select at least 1 category for the product.");
			}
		}

		void UpdateSizes(NameValueCollection form, Product product)
		{
			sizeService.WithValues(form).Update(product);
		}

		void UpdateImages(Product product, HttpRequestBase request)
		{
			var images = httpFileService.GetUploadedImages(request);
			var position = orderableService.NextPosition;
			foreach (var image in images)
			{
				product.ProductImages.Add(new ProductImage
				{
					Image = image,
					Position = position
				});
				position++;
			}
		}

		void CheckForDuplicateNames(Product product, ModelBindingContext bindingContext)
		{
			if (!string.IsNullOrEmpty(product.Name))
			{
				bool productWithNameAlreadyExists =
					repository.GetAll().Any(x => x.ProductId != product.ProductId && x.Name == product.Name);

				if (productWithNameAlreadyExists)
				{
					string key = bindingContext.ModelName + ".ProductId";
					bindingContext.ModelState.AddModelError(key, "Product names must be unique and there is already a product called '{0}'".With(product.Name));
				}
			}
		}
	}
}