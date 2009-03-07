using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Filters;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using MvcContrib;
namespace Suteki.Shop.Controllers
{
	[AdministratorsOnly]
    public class UserController : ControllerBase
    {
        readonly IRepository<User> userRepository;
        readonly IRepository<Role> roleRepository;
    	private readonly IUserService userService;

    	public UserController(IRepository<User> userRepository, IRepository<Role> roleRepository, IUserService userService)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        	this.userService = userService;
        }

        public ActionResult Index()
        {
            var users = userRepository.GetAll().Editable();
            return View("Index", ShopView.Data.WithUsers(users));
        }

        public ActionResult New()
        {
            return View("Edit", EditViewData.WithUser(Shop.User.DefaultUser));
        }

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult New(User user, string password)
		{
			if(! string.IsNullOrEmpty(password))
			{
				user.Password = userService.HashPassword(password);
			}

			try
			{
				user.Validate();
			}
			catch(ValidationException ex)
			{
				ex.CopyToModelState(ModelState, "user");
				return View("Edit", EditViewData.WithUser(user));
			}

			userRepository.InsertOnSubmit(user);
			Message = "User has been added.";

			return this.RedirectToAction(c => c.Index());
		}

        public ActionResult Edit(int id)
        {
            User user = userRepository.GetById(id);
            return View("Edit", EditViewData.WithUser(user));
        }

		[AcceptVerbs(HttpVerbs.Post), UnitOfWork]
		public ActionResult Edit([DataBind] User user, string password)
		{
			if(! string.IsNullOrEmpty(password))
			{
				user.Password = userService.HashPassword(password);
			}

			try
			{
				user.Validate();
			}
			catch (ValidationException validationException) 
			{
				validationException.CopyToModelState(ModelState, "user");
				return View("Edit", EditViewData.WithUser(user));
			}

			return View("Edit", EditViewData.WithUser(user).WithMessage("Changes have been saved")); 
		}

        public ShopViewData EditViewData
        {
            get
            {
                return ShopView.Data.WithRoles(roleRepository.GetAll());
            }
        }
    }
}
