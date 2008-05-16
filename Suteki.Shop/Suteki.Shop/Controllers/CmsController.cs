using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suteki.Shop.Controllers
{
    public class CmsController : ControllerBase
    {
        public ActionResult Index(string urlName)
        {
            return RenderView("Index");
        }
    }
}
