using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViewExtensions;

namespace WebSite.App_Code
{
    public static class Utils
    {
        public static string DefaultAjaxUrlLink(string configSource)
        {
            bool showWebConfigRelated = (configSource == "web.config");

            if (showWebConfigRelated)
            {
                return LinkExtensions.ViewLink("webconfig-jsnlog", "defaultAjaxUrl", null, "defaultAjaxUrl");
            }
            else
            {
                return LinkExtensions.ViewLink("jsnlogjs-jl-setOptions", "defaultAjaxUrl", null, "defaultAjaxUrl");
            }
        }
    }
}

