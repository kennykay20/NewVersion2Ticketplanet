using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;

namespace TicketPlanetV2.Web
{
    public class BundleConfig
    {
        public class NonOrderingBundleOrderer : IBundleOrderer
        {
            public virtual IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }


        }
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
          

           // bundles.IgnoreList.Clear();
            var bundle1 = new StyleBundle("~/Content/assets/css").Include(
                     "~/css/animate.css",
                     "~/css/font-awesome.css",
                     "~/css/fonts.css",
                      "~/css/flaticon.css",
                     "~/css/owl.carousel.css",
                     "~/css/owl.theme.default.css",
                     "~/css/dl-menu.css",
                     "~/css/nice-select.css",
                     "~/css/magnific-popup.css",
                        "~/css/venobox.css",
                        "~/css/slick.css",
                        "~/css/venobox.css",
                        "~/css/style2.css",
                        "~/css/responsive2.css",
                        "~/css/magnific-popup.css",
                        "~/js/plugin/rs_slider/layers.css",
                        "~/js/plugin/rs_slider/navigation.css",
                        "~/js/plugin/rs_slider/settings.css",
                            "~/css/style.css",
                            "~/css/SelectCss.css"
                       );


            bundle1.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(bundle1);




            var bundle = new ScriptBundle("~/Content/assets/js").Include(
                      "~/Scripts/jquery_min.js",
                      "~/js/modernizr.js",
                      "~/js/bootstrap.js",
                      "~/js/owl.carousel.js",
                      "~/js/jquery.dlmenu.js",
                      "~/js/jquery.sticky.js",
                      "~/js/jquery.nice-select.min.js",
                      "~/js/jquery.magnific-popup.js",
                      "~/js/jquery.bxslider.min.js",
                      "~/js/venobox.min.js",
                      "~/js/smothscroll_part1.js",
                      "~/js/smothscroll_part2.js",
                      "~/js/slick.js",
                      "~/js/custom2.js",
                      "~/js/ticketplanetjs/movies.js",
                      "~/js/ticketplanetjs/SelectJs.js",
                      "~/js/ticketplanetjs/aes.js",
                     "~/js/ticketplanetjs/crypto.js",
                      "~/js/ticketplanetjs/ScriptingMovies.js",
                       "~/js/ticketplanetjs/promoScript.js",
                        "~/js/ticketplanetjs/moviesPaystackInitialize.js",
                         "~/js/ticketplanetjs/flutterEventInitialize.js",
                          "~/js/ticketplanetjs/aes.js"
                  );

            bundles.Add(bundle);
          //  BundleTable.EnableOptimizations = true;



        }
    }
}
