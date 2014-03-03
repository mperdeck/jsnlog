using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.App_Code
{
    public static class SiteConstants
    {
        public static string CurrentVersion = Generated.Version;
        public const string JsnlogJsFileSize = "1.5kb";

        public const string NugetDownloadUrl = "http://www.nuget.org/packages/JSNLog/";

        public static string DownloadLinkJsnlogJs 
        {
            get { return string.Format("https://raw.github.com/mperdeck/jsnlog.js/{0}/jsnlog.min.js", CurrentVersion); }
        }
    }
}

