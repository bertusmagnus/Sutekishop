using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;

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
        public ActionResult Index(string email, string password)
        {
			if (userRepository.GetAll().ContainsUser(email, userService.HashPassword(password)))
            {
                userService.SetAuthenticationCookie(email);
                return RedirectToAction("Index", "Home");
            }

            return View(ShopView.Data.WithErrorMessage("Unknown email or password"));
        }

        public ActionResult Logout()
        {
            userService.RemoveAuthenticationCookie();
            return RedirectToAction("Index", "Home");
        }
    }
}
