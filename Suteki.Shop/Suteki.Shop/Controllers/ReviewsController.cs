using System;
using System.Linq;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Shop.ActionResults;
using Suteki.Shop.Filters;
using Suteki.Shop.Models;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;
using MvcContrib;
namespace Suteki.Shop.Controllers
{
	public class ReviewsController : ControllerBase
	{
		readonly IRepository<Review> reviewRepository;
		readonly IRepository<Product> productRepository;

		public ReviewsController(IRepository<Review> repository, IRepository<Product> productRepository)
		{
			this.reviewRepository = repository;
			this.productRepository = productRepository;
		}

		[LoadUsing(typeof(ReviewsWithProducts)), AdministratorsOnly]
		public ActionResult Index()
		{
			return View(new ReviewViewData
			{
				Reviews = reviewRepository.GetAll().Unapproved().ToList()
			});
		}

		public ActionResult Show(int id)
		{
			var reviews = reviewRepository.GetAll().ForProduct(id).Approved().ToList();
			var product = productRepository.GetById(id);

			return View(new ReviewViewData 
			{
				Product = product,
                Reviews = reviews
			});
		}

		public ActionResult New(int id)
		{
			var product = productRepository.GetById(id);

			return View(new ReviewViewData
			{
				Product = product
			});
		}

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult New(int id, [DataBind(Fetch=false)] Review review)
		{
			if(ModelState.IsValid)
			{
				review.ProductId = id;
				reviewRepository.InsertOnSubmit(review);
				return this.RedirectToAction(x => x.Submitted(id));
			}

			var product = productRepository.GetById(id);

			return View(new ReviewViewData 
			{
				Product = product,
				Review = review
			});
		}

		public ActionResult Submitted(int id)
		{
			return View(new ReviewViewData
			{
				Product = productRepository.GetById(id)
			});
		}

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Approve(int id)
		{
			var review = reviewRepository.GetById(id);
			review.Approved = true;

			return this.RedirectToAction(x => x.Index());
		}

		[AdministratorsOnly, AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Delete(int id)
		{
			var review = reviewRepository.GetById(id);
			reviewRepository.DeleteOnSubmit(review);

			return new RedirectToReferrerResult();
		}
	}
}