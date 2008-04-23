using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.Repositories;
using System.Web.Security;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Controllers
{
    public class LoginController : ControllerBase
    {
        IRepository<User> userRepository;

        public LoginController(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public ActionResult Index()
        {
            return RenderView("Index", View.Data);
        }

        public ActionResult Authenticate(string email, string password)
        {
            if (userRepository.GetAll().ContainsUser(email, EncryptPassword(password)))
            {
                SetAuthenticationCookie(email);
                return RedirectToAction("Index", "Home");
            }

            return RenderView("Index", View.Data.WithErrorMessage("Unknown email or password"));
        }

        public ActionResult Logout()
        {
            RemoveAuthenticationCookie();
            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        public virtual void SetAuthenticationCookie(string email)
        {
            FormsAuthentication.SetAuthCookie(email, true);
        }

        [NonAction]
        public virtual void RemoveAuthenticationCookie()
        {
            FormsAuthentication.SignOut();
        }

        [NonAction]
        public virtual string EncryptPassword(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
        }
    }
}
