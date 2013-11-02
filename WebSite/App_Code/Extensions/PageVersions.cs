using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web;
using System.Text;

namespace WebSite.Extensions
{
    public static class PageVersions
    {
        public enum VersionEnum
        {
            NetJs,
            JsOnly
        };

        private class VersionInfo
        {
            public VersionEnum Version { get; set; }
            public string Caption { get; set; }
        }

        private static VersionInfo[] VersionInfos = new[] 
        {
            new VersionInfo { Version = VersionEnum.NetJs, Caption = ".Net + JS" },
            new VersionInfo { Version = VersionEnum.JsOnly, Caption = "JS Only" }
        };

        private const string VersionUrlParam = "version11";
        private const string CookieName = "version22";

        public static VersionEnum CurrentVersion()
        {
            // First try query string parameter

            string versionString = HttpContext.Current.Request.QueryString[VersionUrlParam];

            if (!String.IsNullOrEmpty(versionString))
            {
                if (Enum.IsDefined(typeof(VersionEnum), versionString))
                {
                    VersionEnum version = (VersionEnum)Enum.Parse(typeof(VersionEnum), versionString);
                    // Set cookie, so when other pages are opened user gets same version
                    HttpContext.Current.Response.Cookies[CookieName].Value = versionString;
                    HttpContext.Current.Request.Cookies[CookieName].Expires = DateTime.Now.AddYears(1);

                    return version;
                }
            }

            // Then try cookie

            versionString = (string)HttpContext.Current.Request.Cookies[CookieName].Value;
            if (!String.IsNullOrEmpty(versionString))
            {
                if (Enum.IsDefined(typeof(VersionEnum), versionString))
                {
                    VersionEnum version = (VersionEnum)Enum.Parse(typeof(VersionEnum), versionString);
                    return version;
                }
            }

            // If no cookie, use default

            return VersionEnum.NetJs;
        }

        public static MvcHtmlString VersionSwitcher(this HtmlHelper htmlHelper)
        {
            var version = CurrentVersion();
            var sb = new StringBuilder();

            foreach(var versionInfo in VersionInfos)
            {
                if (versionInfo.Version == version)
                {
                    sb.AppendFormat("<span>{0}</span>", versionInfo.Caption);
                }
                else
                {
                    sb.AppendFormat(
                        @"<a href=""?{0}={1}"">{2}</a>", VersionUrlParam, versionInfo.Version.ToString(), versionInfo.Caption);
                }
            }

            return new MvcHtmlString(sb.ToString());
        }
    }
}

