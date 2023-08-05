using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JSNLog.Infrastructure
{
    // Moved from web site project.
    //TODO: move web site project to jsnlog solution, have only one copy of this file
    public static class SiteConstants
    {
        public static string CurrentVersion = Generated.Version;
        public static string CurrentFrameworkVersion = Generated.FrameworkVersion;
        public static string CurrentJSNLogJsVersion = Generated.JSNLogJsVersion;
        public const string JsnlogJsFileSize = "2kb";
        public const string HttpHeaderRequestIdName = "JSNLog-RequestId";
        public const string GlobalMethodCalledAfterJsnlogJsLoaded = "__jsnlog_configure";
        
        public const string HandlerExtension = ".logger";
        public const string DefaultDefaultAjaxUrl = "/jsnlog" + HandlerExtension;

        public const string DemoGithubUrl = "https://github.com/mperdeck/jsnlogSimpleWorkingDemos/tree/master";
        public const string DemoGithubUrlNetCore = DemoGithubUrl + "/jsnlogSimpleWorkingDemos/NetCore";
        public const string DemoAspNetCoreGithubUrl = DemoGithubUrlNetCore + "/JSNLogDemo_Core_NetCoreApp2";
        public const string AngularJsDemoGithubUrl = "https://github.com/mperdeck/JSNLog.AngularJS";
        public const string Angular2CoreDemoGithubUrl = "https://github.com/mperdeck/jsnlog.AngularCoreDemo";
        public const string LicenceUrl = "https://github.com/mperdeck/jsnlog/blob/master/LICENSE.md";
        public const string LicenceName = "MIT";
        public const string CdnJsUrl = "https://cdnjs.com/libraries/jsnlog";
        public static string CdnJsDownloadUrl = "https://cdnjs.cloudflare.com/ajax/libs/jsnlog/" + CurrentJSNLogJsVersion + "/jsnlog.min.js";
        public static string CdnJsScriptTag = @"<script crossorigin=""anonymous"" src=""" + CdnJsDownloadUrl + @"""></script>";

        public const string JsnlogHost = "https://jsnlog.com";
        public const string InstallPageUrl = JsnlogHost + "/Documentation/DownloadInstall";


        // This causes NuGet to search for all packages with "JSNLog" - so the user will see
        // JSNLog.NLog, etc. as well.
        public const string NugetDownloadUrl = "https://www.nuget.org/packages?q=jsnlog";

        public static string CurrentYear
        {
            get { return DateTime.Now.ToString("yyyy"); }
        }

        public static string DownloadLinkJsnlogJs 
        {
            get { return "https://raw.githubusercontent.com/mperdeck/jsnlog.js/master/jsnlog.min.js"; }
        }
    }
}

