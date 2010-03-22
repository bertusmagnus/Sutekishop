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
		static FakeValueProvider valueProvider;

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

			valueProvider = new FakeValueProvider();
			bindingContext = new ModelBindingContext() 
			{
				ModelState = new ModelStateDictionary(),
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(Product)),
				ModelName = "product",
				ValueProvider = valueProvider
			};

			binder = new ProductBinder(validator, resolver, repository, fileService, imageOrderableService, sizeService);
		}

		[Test]
		public void ShouldAddErrorToModelstateWhenNameNotUnique()
		{
			products.Add(new Product() { ProductId = 5, Name = "foo" });
			binder.Accept(new DataBindAttribute() { Fetch = false });
			SetupBoundProduct(p => p.Name = "foo");


			binder.BindModel(controllerContext, bindingContext);
			bindingContext.ModelState["product.ProductId"].ShouldNotBeNull();
		}

		[Test]
		public void ShouldUpdateImages()
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
		public void ShouldUpdateSizes()
		{
			binder.Accept(new DataBindAttribute() { Fetch = false });
			var product = (Product)binder.BindModel(controllerContext, bindingContext);
			sizeService.AssertWasCalled(x => x.Update(Arg<Product>.Is.Anything));
		}

		[Test]
		public void ShouldUpdateCategories()
		{
			valueProvider.AddValue("categories", new[] { 1, 2 }, "1,2");
			binder.Accept(new DataBindAttribute{Fetch = false});
			var product = (Product)binder.BindModel(controllerContext, bindingContext);

			product.ProductCategories.Count().ShouldEqual(2);
			product.ProductCategories.First().CategoryId.ShouldEqual(1);
			product.ProductCategories.Last().CategoryId.ShouldEqual(2);
		}


		[Test]
		public void AddsModelStateErrorWhenNoCategories()
		{
			binder.Accept(new DataBindAttribute { Fetch = false });
			var product = (Product)binder.BindModel(controllerContext, bindingContext);

			bindingContext.ModelState["categories"].Errors.Count.ShouldEqual(1);
		}

		private void SetupBoundProduct(Action<Product> action)
		{
			validator.Expect(x => x.UpdateFrom(null, null, null, null)).IgnoreArguments().Do(new Action<object, NameValueCollection, ModelStateDictionary, string>((obj, form, modelstate, prefix) => 
				action((Product)obj)
			));
		}
	}
}