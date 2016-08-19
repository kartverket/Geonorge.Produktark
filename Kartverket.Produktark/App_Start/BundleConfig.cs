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

            bundles.Add(new ScriptBundle("~/Content/bower_components/kartverket-felleskomponenter/assets/scripts").Include(
               "~/Content/bower_components/kartverket-felleskomponenter/assets/js/vendor.min.js",
               "~/Content/bower_components/kartverket-felleskomponenter/assets/js/main.js"
           ));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/jquery.validate*"));
                
        }
    }
}
