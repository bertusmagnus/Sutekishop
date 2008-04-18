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
        public void Index()
        {
            RenderView("Index", new CommonViewData());
        }
    }
}
