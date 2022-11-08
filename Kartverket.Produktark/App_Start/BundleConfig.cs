using System.Web;
using System.Web.Optimization;

namespace Kartverket.Produktark
{
    public class BundleConfig
    {
        
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/bower_components/kartverket-felleskomponenter/assets/css/styles").Include(
                "~/Content/bower_components/kartverket-felleskomponenter/assets/css/vendor.min.css",
                "~/Content/bower_components/kartverket-felleskomponenter/assets/css/vendorfonts.min.css",
                "~/Content/bower_components/kartverket-felleskomponenter/assets/css/main.min.css"
            ));

            bundles.Add(new ScriptBundle("~/node-modules/scripts").Include(
               "~/node_modules/@kartverket/geonorge-web-components/MainNavigation.js",
               "~/node_modules/@kartverket/geonorge-web-components/GeoNorgeFooter.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/jquery-3.4.1.js",
                "~/Scripts/jquery.validate*"));
                
        }
    }
}
