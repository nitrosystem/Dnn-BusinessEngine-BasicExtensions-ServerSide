using System.Web.Routing;
using System;
using DotNetNuke.Web.Api;

namespace NitroSystem.Dnn.BusinessEngine.BasicActions.Startup
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("BusinessEngineBasicActions", "default", "{controller}/{action}", new[] { "NitroSystem.Dnn.BusinessEngine.BasicActions.Controllers" });
        }
    }
}