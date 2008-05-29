using System;
using System.Web.Mvc;
using Suteki.Shop.Services;
using System.Web.Security;
using MvcContrib;
using MvcContrib.Attributes;
using System.Collections.Specialized;

namespace Suteki.Shop.Controllers
{
    // don't forget to change back to ConventionController when mvcContrib catches up with CTP 3
    [Rescue("Default")]
    public abstract class ControllerBase : ConventionController, IProvidesBaseService
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

        public virtual NameValueCollection Form
        {
            get
            {
                if (Request.RequestType == "POST")
                {
                    return Request.Form;
                }
                return Request.QueryString;
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
