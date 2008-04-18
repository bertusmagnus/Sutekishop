using System;
using System.Web.Mvc;
using Suteki.Shop.Services;

namespace Suteki.Shop.Controllers
{
    public abstract class ControllerBase : Controller
    {
        /// <summary>
        /// Supplies services and configuration to all controllers (may be null!)
        /// </summary>
        public IBaseControllerService BaseControllerService { get; set; }

        public virtual void RedirectToAction2(string actionName, string controllerName)
        {
            RedirectToAction(actionName, controllerName);
        }
    }
}
