using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;
using System.Web.Security;
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

        public ActionResult Authenticate(string email, string password)
        {
            if (userRepository.GetAll().ContainsUser(email, EncryptPassword(password)))
            {
                userService.SetAuthenticationCookie(email);
                return RedirectToAction("Index", "Home");
            }

            return View("Index", ShopView.Data.WithErrorMessage("Unknown email or password"));
        }

        public ActionResult Logout()
        {
            userService.RemoveAuthenticationCookie();
            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        public virtual string EncryptPassword(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
        }
    }
}
