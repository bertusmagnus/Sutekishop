using NUnit.Framework;
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
        private ControllerTestContext testContext;

        private IRepository<Product> productRepository;

        private IRepository<Category> categoryRepository;

        private IRepository<ProductImage> productImageRepository;

        private IHttpFileService httpFileService;
        private ISizeService sizeService;
        private IOrderableService<Product> productOrderableService;
        private IOrderableService<ProductImage> productImageOrderableService;
        private IValidatingBinder validatingBinder;
        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the product controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new[] { "Administrator" });

            categoryRepository = MockRepositoryBuilder.CreateCategoryRepository();

            productRepository = MockRepositoryBuilder.CreateProductRepository();

            productImageRepository = MockRepository.GenerateStub<IRepository<ProductImage>>();

            httpFileService = MockRepository.GenerateStub<IHttpFileService>();
            sizeService = MockRepository.GenerateStub<ISizeService>();

            productOrderableService = MockRepository.GenerateStub<IOrderableService<Product>>();
            productImageOrderableService = MockRepository.GenerateStub<IOrderableService<ProductImage>>();

            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());
            userService = MockRepository.GenerateStub<IUserService>();

            var mocks = new MockRepository();
            productController = mocks.PartialMock<ProductController>(
                productRepository, 
                categoryRepository,
                productImageRepository,
                httpFileService,
                sizeService,
                productOrderableService,
                productImageOrderableService,
                validatingBinder,
                userService);

            mocks.ReplayAll();

            testContext = new ControllerTestContext(productController);

            userService.Expect(c => c.CurrentUser).Return(new User { RoleId = Role.AdministratorId });
        }

        [Test]
        public void Index_ShouldShowProductListForCategoryOnIndexView()
        {
            const int categoryId = 4;

            var category = new Category
                                    {
                                        CategoryId = categoryId,
                                        Products =
                                            {
                                                new Product(),
                                                new Product()
                                            }
                                    };

            categoryRepository.Expect(r => r.GetById(categoryId)).Return(category);

            var result = productController.Index(categoryId) as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
            if (result.ViewData.Model == null) Assert.Fail("ViewData.Model is null");
            var viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.IsNotNull(viewData.Products, "viewData.Products should not be null");
            Assert.IsNotNull(viewData.Category, "viewData.Category should not be null");
            Assert.AreEqual(categoryId, viewData.Category.CategoryId);

            ProductRepositoryExtensionsTests.AssertProductsReturnedBy_WhereCategoryIdIs4_AreCorrect(
                viewData.Products);
            categoryRepository.VerifyAllExpectations();
        }

        [Test]
        public void Item_ShouldShowItemView()
        {
            const string urlName = "Product_4";

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
        public void Update_ShouldInsertTheNewProductIntoRepository()
        {
            const int productId = 0; // means it's a new product
            const int categoryId = 4;
            const string name = "My New Product";
            const string description = "A description of my new product";

            // create the form
            var form = new NameValueCollection
            {
                {"productid", productId.ToString()},
                {"categoryid", categoryId.ToString()},
                {"name", name},
                {"description", description}
            };

            testContext.TestContext.Request.Expect(r => r.Form).Return(form);

            // add expectations for product repository insertion
            Product product = null;

            productRepository.Expect(r => r.InsertOnSubmit(Arg<Product>.Is.Anything))
                .WhenCalled(invocation => { product = invocation.Arguments[0] as Product; });
            productRepository.Expect(r => r.SubmitChanges());

            // add expectations for image upload
            var images = new List<Image>
            {
                new Image(),
                new Image()
            };

            var httpRequest = testContext.TestContext.Request;
            httpFileService.Expect(h => h.GetUploadedImages(httpRequest)).Return(images);

            // expect the size service to be called
            sizeService.Expect(s => s.WithValues(form)).Return(sizeService);
            sizeService.Expect(s => s.Update(Arg<Product>.Is.Anything));

            // excercise the method
            var result = productController.Update(productId) as ViewResult;

            AssertEditViewIsCorrectlyCalled(result);

            // assert the product was created correctly
            Assert.AreEqual(categoryId, product.CategoryId, "product.CategoryId is incorrect");
            Assert.AreEqual(name, product.Name, "product.Name is incorrect");
            Assert.AreEqual(description, product.Description, "product.Description is incorrect");

            // assert the images were added to the product
            Assert.AreSame(images[0], product.ProductImages[0].Image, "First product image was not added");
            Assert.AreSame(images[1], product.ProductImages[1].Image, "Second product image was not added");

            productRepository.VerifyAllExpectations();
            httpFileService.VerifyAllExpectations();
            sizeService.VerifyAllExpectations();
        }

        [Test]
        public void Update_ShouldUpdateAnExistingProduct()
        {
            const int productId = 44; // non-zero means it's an existing product
            const int categoryId = 4;
            const string name = "My New Product";
            const string description = "A description of my new product";

            // create the form
            var form = new NameValueCollection
            {
                {"productid", productId.ToString()},
                {"categoryid", categoryId.ToString()},
                {"name", name},
                {"description", description}
            };
            testContext.TestContext.Request.Expect(r => r.Form).Return(form);

            var product = new Product
            {
                ProductId = productId,
                CategoryId = 6,
                Name = "The old name",
                Description = "The old description"
            };

            productRepository.Expect(r => r.GetById(productId)).Return(product);
            productRepository.Expect(r => r.SubmitChanges());

            // expect the size service to be called
            sizeService.Expect(s => s.WithValues(form)).Return(sizeService);
            sizeService.Expect(s => s.Update(Arg<Product>.Is.Anything));

            productController.Expect(
                c => c.UpdateImages(Arg<Product>.Is.Same(product), Arg<HttpRequestBase>.Is.Anything));

            // excercise the method
            var result = productController.Update(productId) as ViewResult;

            AssertEditViewIsCorrectlyCalled(result);

            // assert the product was created correctly
            Assert.AreEqual(categoryId, product.CategoryId, "product.CategoryId is incorrect");
            Assert.AreEqual(name, product.Name, "product.Name is incorrect");
            Assert.AreEqual(description, product.Description, "product.Description is incorrect");

            productRepository.VerifyAllExpectations();
        }

        [Test]
        public void UpdateImages_ShouldAddUploadedImagesToProduct()
        {
            const int nextPosition = 23;
            var product = new Product();
            var request = MockRepository.GenerateMock<HttpRequestBase>();

            var image1 = new Image();
            var image2 = new Image();
            var image3 = new Image();

            var images = new List<Image>{image1, image2, image3};

            httpFileService.Expect(fs => fs.GetUploadedImages(request)).Return(images);
            productImageOrderableService.Expect(os => os.NextPosition).Return(nextPosition);

            productController.UpdateImages(product, request);

            // UpdateImages should add the three images to the product with the correct
            // positions

            Assert.That(product.ProductImages.Count, Is.EqualTo(3), "Wrong number of images");

            Assert.That(product.ProductImages[0].Image, Is.SameAs(image1), "Incorect first image");
            Assert.That(product.ProductImages[0].Position, Is.EqualTo(nextPosition), "Incorect first position");

            Assert.That(product.ProductImages[1].Image, Is.SameAs(image2), "Incorect second image");
            Assert.That(product.ProductImages[1].Position, Is.EqualTo(nextPosition + 1), "Incorect second position");
            
            Assert.That(product.ProductImages[2].Image, Is.SameAs(image3), "Incorect third image");
            Assert.That(product.ProductImages[2].Position, Is.EqualTo(nextPosition + 2), "Incorect third position");
        }
    }
}
