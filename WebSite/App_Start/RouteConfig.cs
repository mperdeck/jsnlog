using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new String[] { "MainSite.Controllers" } 
            );

            routes.MapRoute(
                name: "Default2",
                url: "Index",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new String[] { "MainSite.Controllers" } 
            );

            routes.MapRoute(
                name: "PhpJs",
                url: "phpjs",
                defaults: new { controller = "Home", action = "PhpJs", id = UrlParameter.Optional },
                namespaces: new String[] { "MainSite.Controllers" }
            );

            routes.MapRoute(
                name: "Documentation",
                url: "Documentation/{*pathInfo}",
                defaults: new { controller = "Documentation", action = "Index" },
                namespaces: new String[] { "MainSite.Controllers" } 
            );
        }
    }
}
