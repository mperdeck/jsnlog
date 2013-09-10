using System;
using System.Web.Routing;

[assembly: WebActivatorEx.PostApplicationStartMethod(
    typeof($rootnamespace$.App_Start.JSNLogConfig), "PostStart")]

namespace $rootnamespace$.App_Start {
    public static class JSNLogConfig {
        public static void PostStart() {
            // Insert a route at the very start of the routing table (so it gets picked up before all other routes)
            // that ignores the jsnlog.logger route. That way, it will get through to the handler defined
            // in web.config.
            RouteTable.Routes.Insert(0, new Route("jsnlog.logger/{*pathInfo}", new StopRoutingHandler()));
        }
    }
}

