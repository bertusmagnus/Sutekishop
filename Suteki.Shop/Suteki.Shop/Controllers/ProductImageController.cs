using System.Web.Mvc;
using MvcContrib;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Shop.Filters;

namespace Suteki.Shop.Controllers
{
	[AdministratorsOnly]
	public class ProductImageController : ControllerBase
	{
		readonly IOrderableService<ProductImage> productImageOrderableService;
		readonly IRepository<ProductImage> productImageRepository;

		public ProductImageController(IOrderableService<ProductImage> productImageOrderableService,
		                              IRepository<ProductImage> productImageRepository)
		{
			this.productImageOrderableService = productImageOrderableService;
			this.productImageRepository = productImageRepository;
		}

		[AdministratorsOnly, UnitOfWork]
		public ActionResult MoveImageUp(int id, int position)
		{
			productImageOrderableService
				.MoveItemAtPosition(position)
				.ConstrainedBy(productImage => productImage.ProductId == id)
				.UpOne();

			return this.RedirectToAction<ProductController>(c => c.Edit(id));
		}

		[AdministratorsOnly, UnitOfWork]
		public ActionResult MoveImageDown(int id, int position)
		{
			productImageOrderableService
				.MoveItemAtPosition(position)
				.ConstrainedBy(productImage => productImage.ProductId == id)
				.DownOne();

			return this.RedirectToAction<ProductController>(c => c.Edit(id));
		}

		[AdministratorsOnly, UnitOfWork]
		public ActionResult DeleteImage(int id, int productImageId)
		{
			var productImage = productImageRepository.GetById(productImageId);
			productImageRepository.DeleteOnSubmit(productImage);
			Message = "Image deleted.";

			return this.RedirectToAction<ProductController>(c => c.Edit(id));
		}
	}
}
