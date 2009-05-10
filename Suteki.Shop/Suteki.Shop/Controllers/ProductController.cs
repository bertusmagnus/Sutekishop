using System.Linq;
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
		readonly ISizeService sizeService;
		readonly IOrderableService<Product> productOrderableService;
		readonly IUserService userService;
		readonly IUnitOfWorkManager uow;

		public ProductController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, ISizeService sizeService, IOrderableService<Product> productOrderableService, IUserService userService, IUnitOfWorkManager uow)
		{
			this.productRepository = productRepository;
			this.uow = uow;
			this.userService = userService;
			this.categoryRepository = categoryRepository;
			this.sizeService = sizeService;
			this.productOrderableService = productOrderableService;
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

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
		public ActionResult New([BindProduct(Fetch = false)] Product product)
		{
			if (ModelState.IsValid)
			{
				productRepository.InsertOnSubmit(product);
				uow.Commit(); //Need explicit commit in order to get the product id.
				Message = "Product successfully added.";
				return this.RedirectToAction(x => x.Edit(product.ProductId));
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

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork, AdministratorsOnly, ValidateInput(false)]
		public ActionResult Edit([BindProduct] Product product)
		{
			if (ModelState.IsValid)
			{
				Message = "Product successfully saved.";
				return this.RedirectToAction(x => x.Edit(product.ProductId));
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

		[AdministratorsOnly, UnitOfWork]
		public ActionResult MoveUp(int id, int position)
		{
			productOrderableService
				.MoveItemAtPosition(position)
				.ConstrainedBy(product => product.ProductCategories.Any(pc => pc.CategoryId == id))
				.UpOne();


			return this.RedirectToAction(x => x.Index(id));
		}

		[AdministratorsOnly, UnitOfWork]
		public ActionResult MoveDown(int id, int position)
		{
			productOrderableService
				.MoveItemAtPosition(position)
				.ConstrainedBy(product => product.ProductCategories.Any(pc => pc.CategoryId == id))
				.DownOne();

			return this.RedirectToAction(x => x.Index(id));
		}

		

		[AdministratorsOnly, UnitOfWork]
		public ActionResult ClearSizes(int id)
		{
			var product = productRepository.GetById(id);
			sizeService.Clear(product);
			Message = "Sizes have been cleared.";

			return this.RedirectToAction(c => c.Edit(id));
		}

		public ShopViewData EditViewData
		{
			get { return ShopView.Data.WithCategories(categoryRepository.GetAll().Alphabetical()); }
		}
	}
}