using System.Web;
using System.Web.Optimization;
using BundleTransformer.Core.Transformers;

namespace WebSite
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // About CDNs and fallbacks, see
            // http://www.hanselman.com/blog/CDNsFailButYourScriptsDontHaveToFallbackFromCDNToLocalJQuery.aspx
            //
            // CDN performance and reliability
            // http://www.cdnperf.com/
            //
            // Most popular CDN
            // http://trends.builtwith.com/cdn


            bundles.UseCdn = true;
            //########### BundleTable.EnableOptimizations = true; //force optimization while debugging

            var jquery = new ScriptBundle("~/bundles/jquery", "//ajax.aspnetcdn.com/ajax/jquery/jquery-1.11.0.min.js").Include(
                    "~/Scripts/jquery/jquery-{version}.js");
            jquery.CdnFallbackExpression = "window.jQuery";
            bundles.Add(jquery);

            // --------------------

            var bootstrap = new ScriptBundle("~/bundles/bootstrap", "//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js").Include(
                    "~/Scripts/bootstrap-3.1.1/bootstrap.js");
            jquery.CdnFallbackExpression = "$.fn.modal";
            bundles.Add(bootstrap);

            // --------------------

            // Does not work on live host. Throws
            // Could not load file or assembly 'ClearScriptV8-32.dll' or one of its dependencies. Access is denied.
            //var lessBundle = new StyleBundle("~/Content/bootstrap");
            //lessBundle.Include("~/Content/bootstrap-3.1.1/bootstrap.less");
            //lessBundle.Transforms.Add(new CssTransformer());
            //lessBundle.Transforms.Add(new CssMinify());
            //bundles.Add(lessBundle);


            var cssBundle = new StyleBundle("~/Content/css");
            cssBundle.Include("~/Content/bootstrap-3.1.1/bootstrap.css");
            cssBundle.Include("~/Content/Site.css");
            bundles.Add(cssBundle);
        }
    }
}