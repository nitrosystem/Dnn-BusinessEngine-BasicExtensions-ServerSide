using System.Web.Routing;
using System;
using DotNetNuke.Web.Api;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.Startup
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("BusinessEngineBasicServices", "default", "{controller}/{action}", new[] { "NitroSystem.Dnn.BusinessEngine.BasicServices.Controllers" });
        }
    }
}