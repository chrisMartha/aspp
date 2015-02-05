using System.Web.Optimization;

namespace PSoC.ManagementService
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-multiselect.js",
                      "~/Scripts/select2.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/css/select2.css",
                      "~/Content/normalize.css",
                      "~/Content/font-awesome.css",
                      "~/Content/chosen.css",
                      "~/Content/site.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/css/jquery.dataTables.css"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                      "~/Scripts/knockout-3.2.0.js"));

            bundles.Add(new StyleBundle("~/Content/css/dashboard").Include(
                    "~/Content/themes/base/jquery-ui.css",
                    "~/Content/css/jquery.dataTables.min.css"
                 ));

             bundles.Add(new ScriptBundle("~/bundles/dashboard").Include(
                    "~/Scripts/jquery-ui-1.9.2.min.js",
                    "~/Scripts/DataTables-1.10.4/jquery.dataTables.min.js",
                    "~/Scripts/moment.js",
                    "~/Scripts/dashboard.js"
                 ));
        }
    }
}
