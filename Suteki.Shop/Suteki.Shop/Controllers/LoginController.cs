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

        public void Index()
        {
            RenderView("Index", View.Data);
        }

        public void Authenticate(string email, string password)
        {
            if (userRepository.GetAll().ContainsUser(email, EncryptPassword(password)))
            {
                SetAuthenticationCookie(email);
                RedirectToAction2("Index", "Home");
                return;
            }

            RenderView("Index", View.Data.WithErrorMessage("Unknown email or password"));
        }

        public void Logout()
        {
            RemoveAuthenticationCookie();
            RedirectToAction2("Index", "Home");
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
