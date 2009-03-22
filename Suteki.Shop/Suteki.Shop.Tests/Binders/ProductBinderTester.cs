using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Binders;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Binders;
using System.Collections.Generic;
using System.Linq;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Binders
{
	[TestFixture]
	public class ProductBinderTester
	{
		ProductBinder binder;
		IValidatingBinder validator;
		IRepository<Product> repository;
		List<Product> products;
		ControllerContext controllerContext;
		IHttpFileService fileService;
		IOrderableService<ProductImage> imageOrderableService;
		ModelBindingContext bindingContext;
		List<Image> images;
		ISizeService sizeService;

		[SetUp]
		public void Setup()
		{
			products= new List<Product>();
			images = new List<Image>();
			validator = MockRepository.GenerateStub<IValidatingBinder>();
			repository = MockRepository.GenerateStub<IRepository<Product>>();
			repository.Expect(x => x.GetAll()).Return(products.AsQueryable());
			fileService = MockRepository.GenerateStub<IHttpFileService>();
			imageOrderableService = MockRepository.GenerateStub<IOrderableService<ProductImage>>();
			fileService.Expect(x => x.GetUploadedImages(null)).IgnoreArguments().Return(images);
			sizeService = MockRepository.GenerateStub<ISizeService>();
			
			var resolver = MockRepository.GenerateStub<IRepositoryResolver>();
			
			controllerContext = new ControllerContext()
			{
				HttpContext = MockRepository.GenerateStub<HttpContextBase>()
			};

			controllerContext.HttpContext.Stub(x => x.Request).Return(MockRepository.GenerateStub<HttpRequestBase>());
			sizeService.Expect(x => x.WithValues(controllerContext.HttpContext.Request.Form)).Return(sizeService);

			bindingContext = new ModelBindingContext() 
			{
				ModelState = new ModelStateDictionary(),
				ModelType = typeof(Product),
				ModelName = "product"
			};

			binder = new ProductBinder(validator, resolver, repository, fileService, imageOrderableService, sizeService);
		}

		[Test]
		public void Should_add_error_to_modelstate_when_name_not_unique()
		{
			products.Add(new Product() { ProductId = 5, Name = "foo" });
			binder.Accept(new DataBindAttribute() { Fetch = false });
			SetupBoundProduct(p => p.Name = "foo");


			binder.BindModel(controllerContext, bindingContext);
			bindingContext.ModelState["product.ProductId"].ShouldNotBeNull();
		}

		[Test]
		public void Should_update_images()
		{
			images.Add(new Image());
			images.Add(new Image());
			binder.Accept(new DataBindAttribute() { Fetch = false });
			var product = (Product)binder.BindModel(controllerContext, bindingContext);

			product.ProductImages.Count.ShouldEqual(2);
			product.ProductImages.First().Image.ShouldBeTheSameAs(images[0]);
			product.ProductImages.Last().Image.ShouldBeTheSameAs(images[1]);
			product.ProductImages.First().Position.ShouldEqual(0);
			product.ProductImages.Last().Position.ShouldEqual(1);
		}

		[Test]
		public void Should_update_sizes()
		{
			binder.Accept(new DataBindAttribute() { Fetch = false });
			var product = (Product)binder.BindModel(controllerContext, bindingContext);
			sizeService.AssertWasCalled(x => x.Update(Arg<Product>.Is.Anything));
		}
		
		private void SetupBoundProduct(Action<Product> action)
		{
			validator.Expect(x => x.UpdateFrom(null, null, null, null)).IgnoreArguments().Do(new Action<object, NameValueCollection, ModelStateDictionary, string>((obj, form, modelstate, prefix) => 
				action((Product)obj)
			));
		}
	}
}