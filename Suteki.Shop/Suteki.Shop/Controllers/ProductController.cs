using System.Web.Mvc;
using MvcContrib;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Binders;
using Suteki.Shop.Filters;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Controllers
{
	public class ProductController : ControllerBase
	{
		readonly IRepository<Product> productRepository;
		readonly IRepository<Category> categoryRepository;
		readonly IRepository<ProductImage> productImageRepository;
		readonly ISizeService sizeService;
		readonly IOrderableService<Product> productOrderableService;
		readonly IOrderableService<ProductImage> productImageOrderableService;
		readonly IUserService userService;

		public ProductController(
			IRepository<Product> productRepository, 
			IRepository<Category> categoryRepository,
		    IRepository<ProductImage> productImageRepository, 
			ISizeService sizeService,
		    IOrderableService<Product> productOrderableService,
		    IOrderableService<ProductImage> productImageOrderableService, 
			IUserService userService)
		{
			this.productRepository = productRepository;
			this.userService = userService;
			this.categoryRepository = categoryRepository;
			this.productImageRepository = productImageRepository;
			this.sizeService = sizeService;
			this.productOrderableService = productOrderableService;
			this.productImageOrderableService = productImageOrderableService;
		}

		public override string GetControllerName()
		{
			return "";
		}

		public ActionResult Index(int id)
		{
			return RenderIndexView(id);
		}

		ActionResult RenderIndexView(int id)
		{
			var category = categoryRepository.GetById(id);

			AppendTitle(category.Name);

			var products = category.Products.InOrder();

			if (!userService.CurrentUser.IsAdministrator)
			{
				products = products.Active();
			}

			return View("Index", ShopView.Data.WithProducts(products).WithCategory(category));
		}

		public ActionResult Item(string urlName)
		{
			return RenderItemView(urlName);
		}

		ActionResult RenderItemView(string urlName)
		{
			var product = productRepository.GetAll().WithUrlName(urlName);
			AppendTitle(product.Name);
			AppendMetaDescription(product.Description);
			return View("Item", ShopView.Data.WithProduct(product));
		}

		[AdministratorsOnly]
		public ActionResult New(int id)
		{
			var defaultProduct = Product.DefaultProduct(id, productOrderableService.NextPosition);
			return View("Edit", EditViewData.WithProduct(defaultProduct));
		}

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult New([BindProduct(Fetch = false)] Product product)
		{
			if (ModelState.IsValid)
			{
				productRepository.InsertOnSubmit(product);
				Message = "Product successfully added.";
				return this.RedirectToAction(x => x.Index(product.CategoryId));
			}
			else
			{
				return View("Edit", EditViewData.WithProduct(product));
			}
		}

		[AdministratorsOnly]
		public ActionResult Edit(int id)
		{
			return RenderEditView(id);
		}

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork, AdministratorsOnly]
		public ActionResult Edit([BindProduct] Product product)
		{
			if (ModelState.IsValid)
			{
				return this.RedirectToAction(c => c.Index(product.CategoryId));
			}
			else
			{
				return View("Edit", EditViewData.WithProduct(product));
			}
		}

		ActionResult RenderEditView(int id)
		{
			var product = productRepository.GetById(id);
			return View("Edit", EditViewData.WithProduct(product));
		}

		[AdministratorsOnly]
		public ActionResult MoveUp(int id, int position)
		{
			productOrderableService
				.MoveItemAtPosition(position)
				.ConstrainedBy(product => product.CategoryId == id)
				.UpOne();

			return RenderIndexView(id);
		}

		[AdministratorsOnly]
		public ActionResult MoveDown(int id, int position)
		{
			productOrderableService
				.MoveItemAtPosition(position)
				.ConstrainedBy(product => product.CategoryId == id)
				.DownOne();

			return RenderIndexView(id);
		}

		[AdministratorsOnly]
		public ActionResult MoveImageUp(int id, int position)
		{
			productImageOrderableService
				.MoveItemAtPosition(position)
				.ConstrainedBy(productImage => productImage.ProductId == id)
				.UpOne();

			return RenderEditView(id);
		}

		[AdministratorsOnly]
		public ActionResult MoveImageDown(int id, int position)
		{
			productImageOrderableService
				.MoveItemAtPosition(position)
				.ConstrainedBy(productImage => productImage.ProductId == id)
				.DownOne();

			return RenderEditView(id);
		}

		[AdministratorsOnly, UnitOfWork]
		public ActionResult DeleteImage(int id, int productImageId)
		{
			var productImage = productImageRepository.GetById(productImageId);
			productImageRepository.DeleteOnSubmit(productImage);

			return RenderEditView(id);
		}

		[AdministratorsOnly, UnitOfWork]
		public ActionResult ClearSizes(int id)
		{
			var product = productRepository.GetById(id);
			sizeService.Clear(product);

			return View("Edit", EditViewData.WithProduct(product));
		}

		public ShopViewData EditViewData
		{
			get { return ShopView.Data.WithCategories(categoryRepository.GetAll().Alphabetical()); }
		}
	}
}