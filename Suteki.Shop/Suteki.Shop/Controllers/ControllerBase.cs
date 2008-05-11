using System;
using System.Web.Mvc;
using Suteki.Shop.Services;
using System.Web.Security;

namespace Suteki.Shop.Controllers
{
    public abstract class ControllerBase : Controller
    {
        /// <summary>
        /// Supplies services and configuration to all controllers
        /// </summary>
        public IBaseControllerService BaseControllerService { get; set; }

        public virtual User CurrentUser
        {
            get
            {
                User user = this.ControllerContext.HttpContext.User as User;
                if (user == null) throw new ApplicationException("HttpContext.User is not a Suteki.Shop.User");
                return user;
            }
        }

        [NonAction]
        public virtual void SetAuthenticationCookie(string email)
        {
            FormsAuthentication.SetAuthCookie(email, true);
        }

        [NonAction]
        public virtual void SetContextUserTo(User user)
        {
            System.Threading.Thread.CurrentPrincipal = this.ControllerContext.HttpContext.User = user;
        }

        [NonAction]
        public virtual void RemoveAuthenticationCookie()
        {
            FormsAuthentication.SignOut();
        }
    }
}
