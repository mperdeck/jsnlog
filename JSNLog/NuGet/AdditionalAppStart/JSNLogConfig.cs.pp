using System;
using System.Web.Routing;
using System.Web.Mvc;

[assembly: WebActivatorEx.PostApplicationStartMethod(
    typeof($rootnamespace$.App_Start.JSNLogConfig), "PostStart")]

namespace $rootnamespace$.App_Start {
    public static class JSNLogConfig {
        public static void PostStart() {
            // Insert a route that ignores the jsnlog.logger route. That way, 
			// requests for jsnlog.logger will get through to the handler defined
            // in web.config.
            RouteTable.Routes.IgnoreRoute("jsnlog.logger/{*pathInfo}");
        }
    }
}

