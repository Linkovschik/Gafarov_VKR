using System.Web;
using System.Web.Optimization;

namespace Gafarov_VKR
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/arthurScripts").Include(
                     "~/Scripts/myScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminScripts").Include(
                    "~/Scripts/adminScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/userRatingScripts").Include(
                    "~/Scripts/userRatingScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/algorithmScripts").Include(
                    "~/Scripts/algorithmScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/registrationScripts").Include(
                    "~/Scripts/registrationScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/authorizationScripts").Include(
                    "~/Scripts/authorizationScripts.js"));

            bundles.Add(new ScriptBundle("~/bundles/leaflet").Include(
                     "~/Scripts/lrm-graphhopper-1.2.0.min.js",
                     "~/Scripts/L.Path.Transform.js",
                     "~/Scripts/leaflet-geometryutil.js",
                     "~/Scripts/leaflet-arrowheads.js",
                     "~/Scripts/Leaflet.Editable.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
