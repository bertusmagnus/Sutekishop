using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;
using MvcContrib;

namespace Suteki.Shop.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly IRepository<User> userRepository;
        private readonly IUserService userService;

        public LoginController(IRepository<User> userRepository, IUserService userService)
        {
            this.userRepository = userRepository;
        	this.userService = userService;
        }

        public ActionResult Index()
        {
            return View("Index", ShopView.Data);
        }

		[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string email, string password, string returnUrl)
        {
			if (userRepository.GetAll().ContainsUser(email, userService.HashPassword(password)))
            {
                userService.SetAuthenticationCookie(email);

				if(! string.IsNullOrEmpty(returnUrl))
				{
					return Redirect(returnUrl);
				}

				return this.RedirectToAction<HomeController>(c => c.Index());
            }

            return View(ShopView.Data.WithErrorMessage("Unknown email or password"));
        }

        public ActionResult Logout()
        {
            userService.RemoveAuthenticationCookie();
			return this.RedirectToAction<HomeController>(c => c.Index());
        }
    }
}
