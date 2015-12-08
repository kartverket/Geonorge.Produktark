using System.Web;
using System.Web.Optimization;

namespace Kartverket.Produktark
{
    public class BundleConfig
    {
        
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                           "~/Content/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/jquery.validate*"));
                
        }
    }
}
