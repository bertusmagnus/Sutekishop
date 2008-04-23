using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return RenderView("Index", View.Data);
        }
    }
}
