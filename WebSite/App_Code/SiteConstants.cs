using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.App_Code
{
    public static class SiteConstants
    {
        public const string CurrentVersion = "2.4.0";
        public const string JsnlogJsFileSize = "1.5kb";

        public const string NugetDownloadUrl = "http://www.nuget.org/packages/JSNLog/";

        public static string DownloadLinkJsnlogJs 
        {
            get { return string.Format("https://raw.github.com/mperdeck/jsnlog/{0}/JSNLog/Scripts/jsnlog.min.js", CurrentVersion); }
        }
    }
}

