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

            routes.Add(new Route("{controller}/{action}/{id}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { action = "Index", id = "" }),
            });

            routes.Add(new Route("Default.aspx", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { controller = "Home", action = "Index", id = "" }),
            });
        }
    }
}
