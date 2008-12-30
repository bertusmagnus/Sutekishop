using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Controllers;
using Suteki.Shop.Tests.TestHelpers;
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
        private Mock<ProductController> productControllerMock;
        private ControllerTestContext testContext;

        private Repository<Product> productRepository;
        private Mock<Repository<Product>> productRepositoryMock;

        private Repository<Category> categoryRepository;
        private Mock<Repository<Category>> categoryRepositoryMock;

        private IRepository<ProductImage> productImageRepository;

        private IHttpFileService httpFileService;
        private ISizeService sizeService;
        private IOrderableService<Product> productOrderableService;
        private IOrderableService<ProductImage> productImageOrderableService;
        private IValidatingBinder validatingBinder;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the product controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { "Administrator" });

            categoryRepositoryMock = MockRepositoryBuilder.CreateCategoryRepository();
            categoryRepository = categoryRepositoryMock.Object;

            productRepositoryMock = MockRepositoryBuilder.CreateProductRepository();
            productRepository = productRepositoryMock.Object;

            productImageRepository = new Mock<IRepository<ProductImage>>().Object;

            httpFileService = new Mock<IHttpFileService>().Object;
            sizeService = new Mock<ISizeService>().Object;

            productOrderableService = new Mock<IOrderableService<Product>>().Object;
            productImageOrderableService = new Mock<IOrderableService<ProductImage>>().Object;

            validatingBinder = new ValidatingBinder(new SimplePropertyBinder());

            productControllerMock = new Mock<ProductController>(
                productRepository, 
                categoryRepository,
                productImageRepository,
                httpFileService,
                sizeService,
                productOrderableService,
                productImageOrderableService,
                validatingBinder);

            productController = productControllerMock.Object;
            testContext = new ControllerTestContext(productController);

            productControllerMock.ExpectGet(c => c.CurrentUser).Returns(new User { RoleId = Role.AdministratorId });
        }

        [Test]
        public void Index_ShouldShowProductListForCategoryOnIndexView()
        {
            int categoryId = 4;

            Category category = new Category
                                    {
                                        CategoryId = categoryId,
                                        Products =
                                            {
                                                new Product(),
                                                new Product()
                                            }
                                    };

            categoryRepositoryMock.Expect(r => r.GetById(categoryId)).Returns(category).Verifiable();

            var result = productController.Index(categoryId) as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
            if (result.ViewData.Model == null) Assert.Fail("ViewData.Model is null");
            ShopViewData viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.IsNotNull(viewData.Products, "viewData.Products should not be null");
            Assert.IsNotNull(viewData.Category, "viewData.Category should not be null");
            Assert.AreEqual(categoryId, viewData.Category.CategoryId);

            ProductRepositoryExtensionsTests.AssertProductsReturnedBy_WhereCategoryIdIs4_AreCorrect(
                viewData.Products);
            categoryRepositoryMock.Verify();
        }

        [Test]
        public void Item_ShouldShowItemView()
        {
            const string urlName = "Product_4";

            // product repository GetAll expectation is already set by
            // MockRepositoryBuilder.CreateProductRepository() in GetFullPath_ShouldReturnFullPage()

            ViewResult result = productController.Item(urlName)
                .ReturnsViewResult()
                .ForView("Item")
                .AssertNotNull<ShopViewData, Product>(vd => vd.Product)
                .AssertAreEqual<ShopViewData, string>(urlName, vd => vd.Product.UrlName);
        }

        [Test]
        public void New_ShouldShowDefaultProductInEditView()
        {
            int categoryId = 4;

            ViewResult result = productController.New(categoryId) as ViewResult;

            AssertEditViewIsCorrectlyCalled(result);
        }

        private void AssertEditViewIsCorrectlyCalled(ViewResult result)
        {
            Assert.AreEqual("Edit", result.ViewName);
            ShopViewData viewData = result.ViewData.Model as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            Assert.IsNotNull(viewData.Product, "viewData.Product should not be null");
            Assert.IsNotNull(viewData.Categories, "viewData.Categories should not be null");
        }

        [Test]
        public void Edit_ShouldShowProductInEditView()
        {
            int productId = 44;

            Product product = new Product();
            productRepositoryMock.Expect(r => r.GetById(productId)).Returns(product).Verifiable();

            ViewResult result = productController.Edit(productId) as ViewResult;

            AssertEditViewIsCorrectlyCalled(result);
            productRepositoryMock.Verify();
        }

        [Test]
        public void Update_ShouldInsertTheNewProductIntoRepository()
        {
            int productId = 0; // means it's a new product
            int categoryId = 4;
            string name = "My New Product";
            string description = "A description of my new product";

            // create the form
            NameValueCollection form = new NameValueCollection();
            form.Add("productid", productId.ToString());
            form.Add("categoryid", categoryId.ToString());
            form.Add("name", name);
            form.Add("description", description);
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);

            // add expectations for product repository insertion
            Product product = null;

            productRepositoryMock.Expect(r => r.InsertOnSubmit(It.IsAny<Product>()))
                .Callback<Product>(p => { product = p; })
                .Verifiable();
            productRepositoryMock.Expect(r => r.SubmitChanges()).Verifiable();

            // add expectations for image upload
            List<Image> images = new List<Image>
            {
                new Image(),
                new Image()
            };

            HttpRequestBase httpRequest = testContext.TestContext.Request;
            Mock.Get(httpFileService).Expect(h => h.GetUploadedImages(httpRequest)).Returns(images).Verifiable();

            // expect the size service to be called
            Mock.Get(sizeService).Expect(s => s.WithValues(form)).Returns(sizeService).Verifiable();
            Mock.Get(sizeService).Expect(s => s.Update(It.IsAny<Product>())).Verifiable();

            // excercise the method
            ViewResult result = productController.Update(productId) as ViewResult;

            AssertEditViewIsCorrectlyCalled(result);

            // assert the product was created correctly
            Assert.AreEqual(categoryId, product.CategoryId, "product.CategoryId is incorrect");
            Assert.AreEqual(name, product.Name, "product.Name is incorrect");
            Assert.AreEqual(description, product.Description, "product.Description is incorrect");

            // assert the images were added to the product
            Assert.AreSame(images[0], product.ProductImages[0].Image, "First product image was not added");
            Assert.AreSame(images[1], product.ProductImages[1].Image, "Second product image was not added");

            productRepositoryMock.Verify();
            Mock.Get(httpFileService).Verify();
            Mock.Get(sizeService).Verify();
        }

        [Test]
        public void Update_ShouldUpdateAnExistingProduct()
        {
            int productId = 44; // non-zero means it's an existing product
            int categoryId = 4;
            string name = "My New Product";
            string description = "A description of my new product";

            // create the form
            NameValueCollection form = new NameValueCollection();
            form.Add("productid", productId.ToString());
            form.Add("categoryid", categoryId.ToString());
            form.Add("name", name);
            form.Add("description", description);
            testContext.TestContext.RequestMock.ExpectGet(r => r.Form).Returns(() => form);

            Product product = new Product
            {
                ProductId = productId,
                CategoryId = 6,
                Name = "The old name",
                Description = "The old description"
            };

            productRepositoryMock.Expect(r => r.GetById(productId)).Returns(product).Verifiable();
            productRepositoryMock.Expect(r => r.SubmitChanges()).Verifiable();

            // expect the size service to be called
            Mock.Get(sizeService).Expect(s => s.WithValues(form)).Returns(sizeService).Verifiable();
            Mock.Get(sizeService).Expect(s => s.Update(It.IsAny<Product>())).Verifiable();

            // excercise the method
            ViewResult result = productController.Update(productId) as ViewResult;

            AssertEditViewIsCorrectlyCalled(result);

            // assert the product was created correctly
            Assert.AreEqual(categoryId, product.CategoryId, "product.CategoryId is incorrect");
            Assert.AreEqual(name, product.Name, "product.Name is incorrect");
            Assert.AreEqual(description, product.Description, "product.Description is incorrect");

            productRepositoryMock.Verify();
        }

        [Test]
        public void UpdateImages_ShouldAddUploadedImagesToProduct()
        {
            const int nextPosition = 23;
            var product = new Product();
            var request = new Mock<HttpRequestBase>().Object;

            var image1 = new Image();
            var image2 = new Image();
            var image3 = new Image();

            var images = new List<Image>{image1, image2, image3};

            Mock.Get(httpFileService).Expect(fs => fs.GetUploadedImages(request)).Returns(images);
            Mock.Get(productImageOrderableService).ExpectGet(os => os.NextPosition).Returns(nextPosition);

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
