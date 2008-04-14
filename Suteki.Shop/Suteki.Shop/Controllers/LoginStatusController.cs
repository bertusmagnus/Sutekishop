using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suteki.Shop.Controllers
{
    public class LoginStatusController : ComponentController
    {
        public void Index()
        {
            RenderView("Index");
        }
    }
}
