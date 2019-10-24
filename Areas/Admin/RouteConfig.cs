using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Penguin.Web.Abstractions.Interfaces;

namespace Penguin.Cms.Modules.Admin.Areas.Admin
{
    public class RouteConfig : IRouteConfig
    {
        //the throwaway values are because ASP.NET tried to route anything where the last section of the URL contained a period
        //to a static file. I havent double checked to see if ASP.NET core does the same thing. They might be vestigial
        public void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                "Admin_Init",
                "Admin/Init",
                new { area = "admin", controller = "Init", action = "Init" }

            );

            routes.MapRoute(
                "Admin_Index",
                "Admin/Index",
                new { area = "admin", controller = "Home", action = "Index" }

            );
        }
    }
}