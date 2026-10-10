using System.Web.Mvc;
using System.Web.Routing;

namespace Web_TiemHoaMuaXuan
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
        name: "Admin",
        url: "Admin/{action}/{id}",
        defaults: new { controller = "Admin", action = "Dashboard", id = UrlParameter.Optional }
    );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}