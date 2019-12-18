using System;
using System.Web;
using System.Web.Optimization;

namespace SITW
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862        
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.form.min.js",
                        "~/Scripts/jquery-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerysignalR").Include(
                        "~/Scripts/jquery.signalR-2.2.2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/GameBetDetail").Include(
                        "~/Scripts/GameBetDetail.js"));
            bundles.Add(new ScriptBundle("~/bundles/GameBetPoolDetail").Include(
                        "~/Scripts/GameBetPoolDetail.js"));
            bundles.Add(new ScriptBundle("~/bundles/NabobBetDetail").Include(
                        "~/Scripts/NabobBetDetail.js"));

            bundles.Add(new ScriptBundle("~/bundles/FullWebSite").Include(
                        "~/Scripts/UpdateMenu.js"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-toggle.min.js",
                      "~/Scripts/bootstrap-select.min.js",
                      "~/Scripts/i18n/defaults-zh_TW.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/themejs").Include(
                      "~/Scripts/slick.slider.min.js",
                      "~/Scripts/jquery.countdown.min.js",
                      "~/Scripts/fancybox.pack.js",
                      "~/Scripts/isotope.min.js",
                      "~/Scripts/progressbar.js",
                      "~/Scripts/counter.js",
                      "~/Scripts/functions.js"));
            bundles.Add(new ScriptBundle("~/bundles/adminjs").Include(                    
                      "~/Scripts/modernizr-2.6.2-respond-1.1.0.min.js",
                      "~/Scripts/jquery.js",
                      "~/Scripts/jquery.datetimepicker.full.min.js" ,
                      "~/Scripts/jquery.dcjqaccordion.2.7.js",
                      "~/Scripts/jquery.scrollTo.min.js",
                      "~/Scripts/jquery.nicescroll.js",
                      "~/Scripts/common-scripts.js",
                      "~/Scripts/bootstrap-switch.js",
                      "~/Scripts/jquery.tagsinput.js",
                      "~/Scripts/bootstrap-inputmask.min.js",
                      "~/Scripts/form-component.js"                   
                      ));           
                                         
                 

            //bundles.Add(new StyleBundle("~/Content/styles").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/bootstrap-toggle.min.css",
            //          "~/Content/bootstrap-datetimepicker.min.css",
            //          "~/Content/bootstrap-select.min.css"/*,
            //          "~/Content/site.css"*/));
            bundles.Add(new StyleBundle("~/Content/css_dark_scheme").Include(
                      "~/Content/css/font-awesome.css",
                      "~/Content/css/flaticon.css",
                      "~/Content/css/slick-slider.css",
                      "~/Content/css/fancybox.css",
                      "~/Content/style.css",
                      "~/Content/css/color.css",
                      "~/Content/css/responsive.css",
                      "~/Content/css/dark-scheme.css"
                      ));
            bundles.Add(new StyleBundle("~/Content/styles").Include(                   
                    "~/Content/assets/css/bootstrap.css",                
                    "~/Content/assets/css/style.css",
                    "~/Content/assets/css/style-responsive.css",
                    "~/Content/assets/css/table-responsive.css.css"
                     ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
