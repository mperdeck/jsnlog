using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using ViewExtensions;

namespace WebSite
{
    public class ViewExtensionsConfig
    {
        public static void RegisterViews()
        {
            Views.Load("/Views/Documentation");
        }

        public static void RegisterPageVersions()
        {
            var versionInfos = new[] 
            {
                new PageVersions.VersionInfo { VersionUrlName = "netjs", VersionName = "NetJs", Caption = ".Net + JS", IsDefault = true },
                new PageVersions.VersionInfo { VersionUrlName = "php", VersionName = "PhpJs", Caption = "PHP + JS" },
                new PageVersions.VersionInfo { VersionUrlName = "nodejs", VersionName = "NodeJs", Caption = "Node + JS" },
                new PageVersions.VersionInfo { VersionUrlName = "js", VersionName = "JsOnly", Caption = "JS Only" }
            };

            PageVersions.Load(versionInfos, false, true);
        }
    }
}