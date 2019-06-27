﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kartverket.Produktark
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "ProductSheets", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("OIDC-callback-signout", "signout-callback-oidc", new { controller = "Home", action = "SignOutCallback" });
        }
    }
}
