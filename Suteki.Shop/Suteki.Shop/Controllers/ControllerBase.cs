using System;
using System.Web.Mvc;
using Castle.Core.Logging;
using MvcContrib.Filters;
using Suteki.Common.Extensions;
using Suteki.Shop.Services;
using System.Web.Security;
using MvcContrib;
using System.Collections.Specialized;

namespace Suteki.Shop.Controllers
{
    // don't forget to change back to ConventionController when mvcContrib catches up with CTP 3
    [Rescue("Default")]
    public abstract class ControllerBase : ConventionController, IProvidesBaseService
    {
        private IBaseControllerService baseControllerService;

        /// <summary>
        /// Supplies services and configuration to all controllers
        /// </summary>
        public IBaseControllerService BaseControllerService
        {
            get { return baseControllerService; }
            set 
            { 
                baseControllerService = value;

                ViewData["Title"] = "{0}{1}".With(
                    baseControllerService.ShopName,
                    GetControllerName());

                ViewData["MetaDescription"] = "\"{0}\"".With(baseControllerService.MetaDescription);
            }
        }

        public ILogger Logger { get; set; }

        public virtual string GetControllerName()
        {
            return " - {0}".With(GetType().Name.Replace("Controller", ""));
        }


        public virtual void AppendTitle(string text)
        {
            ViewData["Title"] = "{0} - {1}".With(ViewData["Title"], text);
        }

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
