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

            var lessBundle = new StyleBundle("~/Content/bootstrap");
            lessBundle.Include("~/Content/bootstrap-3.1.1/bootstrap.less");
            lessBundle.Transforms.Add(new CssTransformer());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);




            
        //        .Include("~/Content/bootstrap-3.1.1/grid.less")







            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            //            "~/Scripts/jquery-ui-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.unobtrusive*",
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            //bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
            //            "~/Content/themes/base/jquery.ui.core.css",
            //            "~/Content/themes/base/jquery.ui.resizable.css",
            //            "~/Content/themes/base/jquery.ui.selectable.css",
            //            "~/Content/themes/base/jquery.ui.accordion.css",
            //            "~/Content/themes/base/jquery.ui.autocomplete.css",
            //            "~/Content/themes/base/jquery.ui.button.css",
            //            "~/Content/themes/base/jquery.ui.dialog.css",
            //            "~/Content/themes/base/jquery.ui.slider.css",
            //            "~/Content/themes/base/jquery.ui.tabs.css",
            //            "~/Content/themes/base/jquery.ui.datepicker.css",
            //            "~/Content/themes/base/jquery.ui.progressbar.css",
            //            "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}