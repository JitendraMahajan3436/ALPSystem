using System.Web;
using System.Web.Optimization;

namespace REIN_MES_System
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/PDFHighchart").Include(
                   "~/Scripts/PDFHighchart/highcharts.js",
                   "~/Scripts/Highchart/highcharts-more.js",
                     "~/Scripts/PDFHighchart/jspdf.js",
                     "~/Scripts/PDFHighchart/svg2pdf.js",
                    //"~/Scripts/PDFHighchart/exporting.js",
                    "~/Scripts/PDFHighchart/offline-exporting.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                   "~/Content/theme/plugins/datepicker/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                         "~/Scripts/additional-methods.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/fdtheme/bootstrap").Include(
                      //"~/Content/theme/bootstrap/css/bootstrap.min.css")
                      "~/Content/theme/bootstrap/css/bootstrapNew.css"
                      ));

            bundles.Add(new StyleBundle("~/fdtheme/css").Include(
                     //"~/Content/theme/dist/css/AdminLTE.min.css",
                      "~/Content/theme/dist/css/Admin.css",
                      "~/Content/theme/dist/css/skins/_all-skins.min.css"));

            bundles.Add(new StyleBundle("~/fdtheme/Font_Icons").Include(
                     "~/Content/theme/fonts/font-awesome.min.css",
                     "~/Content/theme/fonts/ionicons.min.css"));

            bundles.Add(new StyleBundle("~/Content/site").Include(
                     "~/Content/theme/site.css"));

            bundles.Add(new StyleBundle("~/Content/datepickercss").Include(
                      "~/Content/theme/plugins/datepicker/datepicker3.css"
                      ));

            bundles.Add(new ScriptBundle("~/fdtheme/js").Include(
                     "~/Content/theme/plugins/jQuery/jQuery-2.1.4.min.js",
                     "~/Content/theme/bootstrap/js/bootstrap.min.js",
                     "~/Content/theme/plugins/slimScroll/jquery.slimscroll.min.js",
                     "~/Content/theme/plugins/fastclick/fastclick.min.js",
                     "~/Content/theme/dist/js/app.min.js",
                     "~/Content/theme/dist/js/demo.js"));

            // datatable css
            bundles.Add(new StyleBundle("~/Content/theme/plugins/datatables/css").Include(
                      "~/Content/theme/plugins/datatables/dataTables.bootstrap.css"));

            // datatable js
            bundles.Add(new ScriptBundle("~/Content/theme/plugins/datatables/js").Include(
                      "~/Content/theme/plugins/datatables/jquery.dataTables.min.js",
                      "~/Content/theme/plugins/datatables/dataTables.bootstrap.min.js"));

            // datatable js
            bundles.Add(new ScriptBundle("~/Scripts/global").Include(
                      "~/Scripts/global.js"));

            //Printer
            bundles.Add(new ScriptBundle("~/bundles/PrinterScripts").Include(
                      "~/Scripts/Printer/print.min.js"));
            //Printer
            bundles.Add(new ScriptBundle("~/bundles/PrinterStyles").Include(
                      "~/Scripts/Printer/print.min.css"));

            //station to setup
            bundles.Add(new ScriptBundle("~/bundles/StationToSetupManagement_file").Include(
                        "~/Scripts/StationToSetup.js"));

            // user defined datatable js
            bundles.Add(new ScriptBundle("~/Scripts/user_datatable").Include(
                      "~/Scripts/user_datatable.js"));

            // user js
            bundles.Add(new ScriptBundle("~/fdtheme/ajaxroutine").Include(
                      "~/Scripts/ajaxroutine.js"));

            // masterplantsetupfile js
            bundles.Add(new ScriptBundle("~/bundles/master_plant_setup_file").Include(
                        "~/Scripts/master_plant_setup.js"));

            // master js
            bundles.Add(new ScriptBundle("~/bundles/master_file").Include(
                        "~/Scripts/master.js"));

            //calendar

            bundles.Add(new ScriptBundle("~/bundles/Calendar_file").Include(
                       "~/Content/theme/plugins/daterangepicker/moment.min.js",
                       "~/Content/theme/plugins/DateTimePicker/bootstrap-datetimepicker.min.js",
                       "~/Content/theme/plugins/datepicker/bootstrap-datepicker.js"));
            //calendar style
            bundles.Add(new StyleBundle("~/Content/Calendarstyles").Include(
                   "~/Content/theme/plugins/select2/select2.min.css"));

            // jquery js
            bundles.Add(new ScriptBundle("~/bundles/jquery_ui").Include(
                        "~/Content/theme/plugins/jQueryUI/jquery-ui-1.10.3.min.js"));

            // route configuration js
            bundles.Add(new ScriptBundle("~/bundles/route_configuration").Include(
                        "~/Scripts/route_configuration.js"));

            // route marriage configuration js
            bundles.Add(new ScriptBundle("~/bundles/route_marriage_configuration").Include(
                        "~/Scripts/route_marriage_configuration.js"));

            // route shop marriage configuration js
            bundles.Add(new ScriptBundle("~/bundles/route_shop_marriage_configuration").Include(
                        "~/Scripts/route_shop_marriage_configuration.js"));

            // quality audit configuration
            bundles.Add(new ScriptBundle("~/bundles/Quality_Audit_Configuration").Include(
                        "~/Scripts/Quality_Audit_Configuration.js"));

            // quality audit type configuration
            bundles.Add(new ScriptBundle("~/bundles/quality_audit").Include(
                        "~/Scripts/quality_audit.js"));

            // quality audi configuration master type configuration
            bundles.Add(new ScriptBundle("~/bundles/Audit_Configuration_Master").Include(
                        "~/Scripts/Audit_Configuration_Master.js"));

            // plant line configuration
            bundles.Add(new ScriptBundle("~/bundles/plant_line_configuration").Include(
                        "~/Scripts/plant_line_configuration.js"));

            // quality configuration
            bundles.Add(new ScriptBundle("~/bundles/quality_configuration").Include(
                        "~/Scripts/quality_configuration.js"));

            // quality log
            bundles.Add(new ScriptBundle("~/bundles/quality_log").Include(
                        "~/Scripts/quality_log.js"));


            // quality log
            bundles.Add(new ScriptBundle("~/bundles/quality_take_in_take_out_script").Include(
                        "~/Scripts/take_in_take_out.js"));

            // Associate js  
            bundles.Add(new ScriptBundle("~/bundles/AssociateManagement_file").Include(
                      "~/Scripts/associate.js"));

            // AssociateAllocation js  
            bundles.Add(new ScriptBundle("~/bundles/AssociateAllocationManagement_file").Include(
                      "~/Scripts/AssociateAllocation.js"));

            //supervisor
            bundles.Add(new ScriptBundle("~/bundles/SupervisorManagement_file").Include(
                       "~/Scripts/Supervisor.js"));

            //Manager
            bundles.Add(new ScriptBundle("~/bundles/ManagerManagement_file").Include(
                      "~/Scripts/Manager.js"));


            //critical station
            bundles.Add(new ScriptBundle("~/bundles/CriticalStationManagement_file").Include(
                       "~/Scripts/Critical_Station.js"));

            //supervisor to manager
            bundles.Add(new ScriptBundle("~/bundles/SupervisorToManagerManagement_file").Include(
                        "~/Scripts/SupervisorToManager.js"));

            //OperatortoSupervisor
            bundles.Add(new ScriptBundle("~/bundles/OperatorToSupervisorManagement_file").Include(
                       "~/Scripts/OperatorToSupervisor.js"));

            //Leave Management System
            bundles.Add(new ScriptBundle("~/bundles/LeaveManagement_file").Include(
                       "~/Scripts/LeaveManagement.js"));

            //Training
            bundles.Add(new ScriptBundle("~/bundles/TrainingManagement_file").Include(
                       "~/Scripts/Training.js"));

            //Session
            bundles.Add(new ScriptBundle("~/bundles/SessionManagement_file").Include(
                       "~/Scripts/Session.js"));

            //EmployeeToSession
            bundles.Add(new ScriptBundle("~/bundles/EmployeeToSessionManagement_file").Include(
                       "~/Scripts/EmployeeToSession.js"));

            //AssignGrade
            bundles.Add(new ScriptBundle("~/bundles/AssignGradeManagement_file").Include(
                       "~/Scripts/AssignGrade.js"));

            //AssignEmployeeSkillSet
            bundles.Add(new ScriptBundle("~/bundles/EmployeeSkillSetManagement_file").Include(
                       "~/Scripts/EmployeeSkillSet.js"));

            //DailyReport
            bundles.Add(new ScriptBundle("~/bundles/DailyReportManagement_file").Include(
                       "~/Scripts/DailyReport.js"));

            //Add Users
            bundles.Add(new ScriptBundle("~/bundles/AddUserManagement_file").Include(
                       "~/Scripts/AddUser.js"));

            //Trainings
            bundles.Add(new ScriptBundle("~/bundles/TrainingsManagement_file").Include(
                       "~/Scripts/Trainings.js"));


            //Report Ganeshan
            bundles.Add(new ScriptBundle("~/bundles/ReportManagement_file").Include(
                       "~/Scripts/Report.js"));

            bundles.Add(new ScriptBundle("~/bundles/QualityReportManagement_file").Include(
                      "~/Scripts/QualityReport.js"));

            bundles.Add(new ScriptBundle("~/bundles/MaintananceReportManagement_file").Include(
                     "~/Scripts/MaintananceReport.js"));

            //Session
            bundles.Add(new ScriptBundle("~/bundles/SessionManagement_file").Include(
                       "~/Scripts/Session.js"));

            //EmployeeToSession
            bundles.Add(new ScriptBundle("~/bundles/EmployeeToSessionManagement_file").Include(
                       "~/Scripts/EmployeeToSession.js"));

            //AssignGrade
            bundles.Add(new ScriptBundle("~/bundles/AssignGradeManagement_file").Include(
                       "~/Scripts/AssignGrade.js"));

            //AssignEmployeeSkillSet
            bundles.Add(new ScriptBundle("~/bundles/EmployeeSkillSetManagement_file").Include(
                       "~/Scripts/EmployeeSkillSet.js"));

            //DailyReport
            bundles.Add(new ScriptBundle("~/bundles/DailyReportManagement_file").Include(
                       "~/Scripts/DailyReport.js"));

            //Date Picker
            bundles.Add(new ScriptBundle("~/bundles/DateTime").Include(
                       "~/Content/theme/plugins/datepicker/bootstrap-datepicker.js"));

            //Dropzone Plugin Styling
            bundles.Add(new StyleBundle("~/Content/DZstyles").Include(
                    "~/Content/theme/plugins/Dropzone/dropzone.css"));
            //Dropzone Plugin JS
            bundles.Add(new ScriptBundle("~/bundles/DZscripts").Include(
                    "~/Content/theme/plugins/Dropzone/dropzone.min.js"));

            //Select2 Plugin Styling
            bundles.Add(new StyleBundle("~/Content/Select2styles").Include(
                   "~/Content/theme/plugins/select2/select2.min.css"));
            //Select2 Plugin JS
            bundles.Add(new ScriptBundle("~/bundles/Select2scripts").Include(
                   "~/Content/theme/plugins/select2/select2.full.js"));

            //Zooming JS
            bundles.Add(new ScriptBundle("~/bundles/Zooming").Include(
                   "~/Scripts/jquery.Zoom.min.js"));

            //CountDown JS
            //bundles.Add(new ScriptBundle("~/bundles/CountDown").Include(
            //       "~/Scripts/jquery.countdown.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/CountDown").Include(
                  "~/Scripts/jquery.countdownTimer.min.js"));


            // Shopfloor css
            bundles.Add(new StyleBundle("~/Content/shopfloor").Include(
                   "~/Content/theme/shopfloor.css"));

            // spinner css
            bundles.Add(new StyleBundle("~/fdtheme/spinner_css").Include(
                   "~/Content/theme/spinner.css"));

            // spinner js
            bundles.Add(new ScriptBundle("~/fdtheme/spinner_js").Include(
                   "~/Scripts/spinner.js"));


            // quality captures
            bundles.Add(new ScriptBundle("~/fdtheme/quality_captures").Include(
                   "~/Scripts/quality_captures.js"));

            // tiny toggle
            bundles.Add(new ScriptBundle("~/fdtheme/tinytoggel_js").Include(
                   "~/Content/theme/plugins/tinytoggle/js/tiny-toggle.js"));


            bundles.Add(new StyleBundle("~/fdtheme/tinytoggel_css").Include(
                   "~/Content/theme/plugins/tinytoggle/css/tiny-toggle.css"));

            // shop floor help support

            bundles.Add(new ScriptBundle("~/fdtheme/bootslidemenu_js").Include(
                   "~/Content/theme/plugins/shopbootslidemenu/js/BootSideMenu.js"));

            bundles.Add(new StyleBundle("~/fdtheme/bootslidemenu_css").Include(
                   "~/Content/theme/plugins/shopbootslidemenu/css/BootSideMenu.css"));
            bundles.Add(new ScriptBundle("~/fdtheme/bootslidemenu_shop_js").Include(
                   "~/Scripts/shop_help_support.js"));

            // notifications
            bundles.Add(new ScriptBundle("~/fdtheme/notification_js").Include(
                   "~/Content/theme/plugins/toastee/js/jquery.toastee.0.1.js"));

            bundles.Add(new StyleBundle("~/fdtheme/paint_shop_css").Include(
                   "~/Content/theme/paintshop.css"));

            bundles.Add(new StyleBundle("~/fdtheme/notification_css").Include(
                   "~/Content/theme/plugins/toastee/css/styles.css"));

            // color picker
            bundles.Add(new ScriptBundle("~/fdtheme/site_colorpicker_js").Include(
                   "~/Content/theme/plugins/colorpicker/bootstrap-colorpicker.min.js"));


            bundles.Add(new StyleBundle("~/fdtheme/site_colorpicker_css").Include(
                   "~/Content/theme/plugins/colorpicker/bootstrap-colorpicker.min.css"));

            // quality part to image

            bundles.Add(new ScriptBundle("~/fdtheme/quality_image_part_js").Include(
                   "~/Scripts/quality_image_parts.js"));

            //Abs Operator Transfer
            bundles.Add(new ScriptBundle("~/bundles/AbsOperatorTransferManagement_file").Include(
                        "~/Scripts/AbsOperatorTransfer.js"));

            //part to machine
            bundles.Add(new ScriptBundle("~/bundles/PartToMachineManagement_file").Include(
                        "~/Scripts/PartToMachine.js"));
            //Parts
            bundles.Add(new ScriptBundle("~/bundles/PartManagement_file").Include(
                      "~/Scripts/Part.js"));

            // quality capture shop floor image based
            bundles.Add(new ScriptBundle("~/fdtheme/quality_captures_image_js").Include(
                   "~/Scripts/quality_captures_image.js"));

            //IOTTag js            
            bundles.Add(new ScriptBundle("~/bundles/IOTTagManagement_file").Include(
                      "~/Scripts/IOTTag.js"));

            bundles.Add(new ScriptBundle("~/fdtheme/MMTUCharts_js").Include(
               "~/Content/theme/plugins/RGraph/RGraph.common.core.js",
               "~/Content/theme/plugins/RGraph/RGraph.common.dynamic.js",
               "~/Content/theme/plugins/RGraph/RGraph.drawing.xaxis.js",
               "~/Content/theme/plugins/RGraph/RGraph.drawing.yaxis.js",
               "~/Content/theme/plugins/RGraph/RGraph.common.effects.js",
               "~/Content/theme/plugins/RGraph/RGraph.gauge.js",
               "~/Content/theme/plugins/RGraph/RGraph.line.js",
               "~/Content/theme/plugins/RGraph/RGraph.thermometer.js",
               "~/Content/theme/plugins/RGraph/d3.v3.min.js",
               "~/Content/theme/plugins/RGraph/RGraph.svg.semicircularprogress.js",
               "~/Content/theme/plugins/RGraph/RGraph.semicircularprogress.js",
               "~/Content/theme/plugins/RGraph/liquidFillGauge.js"));


            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                       "~/Scripts/Calendar/jquery-{version}.js",
                       // needed for drag/move events in fullcalendar
                       "~/Scripts/Calendar/jquery-ui-{version}.js",
                       "~/Scripts/Calendar/bootstrap.js",
                       "~/Scripts/Calendar/bootstrap-modal.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/Calendar/jquery.unobtrusive*",
                        "~/Scripts/Calendar/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/Calendar/modernizr-*"));
            bundles.Add(new StyleBundle("~/Content/css1").Include(
                   "~/Content/Calendar/site.css",
                   "~/Content/Calendar/bootstrap.css"
                   ));
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                       "~/Content/themes/base/jquery.ui.core.css",
                       "~/Content/themes/base/jquery.ui.resizable.css",
                       "~/Content/themes/base/jquery.ui.selectable.css",
                       "~/Content/themes/base/jquery.ui.accordion.css",
                       "~/Content/themes/base/jquery.ui.autocomplete.css",
                       "~/Content/themes/base/jquery.ui.button.css",
                       "~/Content/themes/base/jquery.ui.dialog.css",
                       "~/Content/themes/base/jquery.ui.slider.css",
                       "~/Content/themes/base/jquery.ui.tabs.css",
                       "~/Content/themes/base/jquery.ui.datepicker.css",
                       "~/Content/themes/base/jquery.ui.progressbar.css",
                       "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/model_master").Include(
                // "~/Content/theme/plugins/jQueryUI/jquery-ui.min.js",
                "~/Scripts/model_master.js",
                "~/Content/theme/plugins/jQueryUI/jquery-ui.min.js"
                     ));

            bundles.Add(new StyleBundle("~/Content/theme/OrderSequencing").Include(
                   "~/Content/theme/OrderSequencing.css"
                   ));

            bundles.Add(new StyleBundle("~/Content/plugins/jQueryUI").Include(
                   "~/Content/theme/plugins/jQueryUI/jquery-ui.css"
                   ));
        }
    }
}
