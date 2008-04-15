using System;
using System.Web.Mvc;

namespace Suteki.Shop.Controllers
{
    public abstract class ControllerBase : Controller
    {
        public virtual void RedirectToAction2(string actionName, string controllerName)
        {
            RedirectToAction(actionName, controllerName);
        }
    }
}
