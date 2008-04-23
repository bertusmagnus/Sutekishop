using System;
using System.Web.Routing;
using System.Web.Mvc;

namespace Suteki.Shop.Routes
{
    public class RouteManager
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Note: Change the URL to "{controller}.mvc/{action}/{id}" to enable
            //       automatic support on IIS6 and IIS7 classic mode

            routes.MapRoute("Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = "" },
                new { controller = @"[^\.]*" });

            routes.MapRoute("DefaultAspx",
                "Default.aspx",
                new { controller = "Home", action = "Index", id = "" });

        }
    }
}
