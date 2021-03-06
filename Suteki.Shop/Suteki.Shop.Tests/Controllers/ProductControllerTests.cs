﻿using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.TestHelpers;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using Suteki.Shop.ViewData;
using System.Collections.Generic;
using Suteki.Shop.Tests.Repositories;
using System.Collections.Specialized;
using System.Threading;
using System.Security.Principal;
using Suteki.Shop.Services;
using System.Web;
using System.Web.Mvc;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class ProductControllerTests
    {
        private ProductController productController;
    	private IRepository<Product> productRepository;
        private IRepository<Category> categoryRepository;
    	private ISizeService sizeService;
        private IOrderableService<Product> productOrderableService;
    	private IUserService userService;
        private const string urlName = "Product_4";

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the product controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            categoryRepository = MockRepositoryBuilder.CreateCategoryRepository();

            productRepository = MockRepositoryBuilder.CreateProductRepository();

            MockRepository.GenerateStub<IRepository<ProductImage>>();

            MockRepository.GenerateStub<IHttpFileService>();
            sizeService = MockRepository.GenerateStub<ISizeService>();

            productOrderableService = MockRepository.GenerateStub<IOrderableService<Product>>();
            MockRepository.GenerateStub<IOrderableService<ProductImage>>();

        	userService = MockRepository.GenerateStub<IUserService>();

			productController = new ProductController(productRepository, categoryRepository, sizeService, productOrderableService, userService, MockRepository.GenerateStub<IUnitOfWorkManager>());

        	userService.Expect(c => c.CurrentUser).Return(new User { RoleId = Role.AdministratorId });
        }

        [Test]
        public void Index_ShouldShowProductListForCategoryOnIndexView()
        {
            const int categoryId = 4;

            var category = new Category
                                    {
                                        CategoryId = categoryId,
										ProductCategories =
											{
												new ProductCategory() { Product = new Product() },
												new ProductCategory() { Product = new Product()}
											}
                                    };

            categoryRepository.Stub(r => r.GetById(categoryId)).Return(category);

            var viewData = productController.Index(categoryId)
                .ReturnsViewResult()
                .ForView("Index")
                .WithModel<ShopViewData>()
                .AssertNotNull(vd => vd.Products)
                .AssertNotNull(vd => vd.Category)
                .AssertAreEqual(categoryId, vd => vd.Category.CategoryId);

            ProductRepositoryExtensionsTests.AssertProductsReturnedBy_WhereCategoryIdIs4_AreCorrect(
                viewData.Products);
        }

        [Test]
        public void Item_ShouldShowItemView()
        {
            // product repository GetAll expectation is already set by
            // MockRepositoryBuilder.CreateProductRepository() in GetFullPath_ShouldReturnFullPage()

            productController.Item(urlName)
                .ReturnsViewResult()
                .ForView("Item")
                .WithModel<ShopViewData>()
                .AssertNotNull(vd => vd.Product)
                .AssertAreEqual(urlName, vd => vd.Product.UrlName);
        }

        [Test]
        public void Item_should_show_plain_text_description_in_meta_description()
        {
            var viewResult = productController.Item(urlName)
                .ReturnsViewResult()
                .ForView("Item");

            viewResult.ViewData["MetaDescription"].ShouldEqual("Description 4");
        }

        [Test]
        public void New_ShouldShowDefaultProductInEditView()
        {
            const int categoryId = 4;

            var result = productController.New(categoryId) as ViewResult;

            AssertEditViewIsCorrectlyCalled(result);
        }

        private static void AssertEditViewIsCorrectlyCalled(ViewResultBase result)
        {
            Assert.AreEqual("Edit", result.ViewName);
            var viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.IsNotNull(viewData.Product, "viewData.Product should not be null");
            Assert.IsNotNull(viewData.Categories, "viewData.Categories should not be null");
        }

        [Test]
        public void Edit_ShouldShowProductInEditView()
        {
            const int productId = 44;

            var product = new Product();
            productRepository.Expect(r => r.GetById(productId)).Return(product);

            var result = productController.Edit(productId) as ViewResult;

            AssertEditViewIsCorrectlyCalled(result);
        }

    	[Test]
    	public void EditWithPost_ShouldRedirectOnSucessfulBinding()
    	{
			var product = new Product() { ProductId = 5};
			productController.Edit(product)
				.ReturnsRedirectToRouteResult()
				.ToAction("Edit")
				.WithRouteValue("id", "5");
    	}

    	[Test]
    	public void EditWithPost_ShouldRenderViewWhenBindingFails()
    	{
			productController.ModelState.AddModelError("foo", "bar");
			var product = new Product() { ProductId = 5 };

			productController.Edit(product)
				.ReturnsViewResult()
				.ForView("Edit")
				.WithModel<ShopViewData>()
				.AssertAreSame(product, x => x.Product);
    	}

    	[Test]
    	public void NewWithPost_ShouldInsertNewProduct()
    	{
			var product = new Product() { ProductId = 5};
			productController.New(product)
				.ReturnsRedirectToRouteResult()
				.ToAction("Edit")
				.WithRouteValue("id", "5");

			productController.Message.ShouldNotBeNull();
			productRepository.AssertWasCalled(x => x.InsertOnSubmit(product));
    	}

    	[Test]
    	public void NewWithPost_ShouldRenderViewWhenThereAreBindingErrors()
    	{
    		productController.ModelState.AddModelError("foo", "bar");
    		var product = new Product();
			productController.New(product)
				.ReturnsViewResult()
				.ForView("Edit")
				.WithModel<ShopViewData>()
				.AssertAreSame(product, x => x.Product);
    	}

        [Test]
        public void Should_show_product_not_found_view_when_urlName_does_not_match()
        {
            productController.Item("xxx")
                .ReturnsViewResult()
                .ForView("NotFound");
        }
    }
}
