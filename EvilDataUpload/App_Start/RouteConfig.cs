using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EvilDataUpload
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.config");

            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "DataUpload", action = "UploadFile", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DataUpload",
                url: "Shared/UploadData",
                defaults: new { controller = "DataUpload", action = "UploadFile", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DataDisplay",
                url: "Shared/DisplayData",
                defaults: new { controller = "DataDisplay", action = "DisplayData", id = UrlParameter.Optional }
            );

            routes.MapRoute( 
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
