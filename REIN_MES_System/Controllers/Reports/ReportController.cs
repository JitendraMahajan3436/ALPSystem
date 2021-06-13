using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers
{
    public class ReportController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        
        // GET: Report
        public ActionResult Index()
        {

            try
            {
              

                //var mM_AM_Training = db.MM_AM_Training.Include(m => m.RS_Employee).Include(m => m.RS_Stations);
                return View();
            }
            catch (Exception ex)
            {
                
                return View();
            }
           // return View();
        }

        public ActionResult Close()
        {

            return RedirectToAction("Index", "Home");            
            
        }

        //public ActionResult Hide()
        //{

        //    //return RedirectToAction("Index", "Home");
        //    //div1.Visible = false;
            

        //}

        public ActionResult ProductionOrderSummary()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "ProductionOrderSummary";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Production Order Summary";
            globalData.contentFooter = "Production Order Summary";
            globalData.pageTitle = "Production Order Summary";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult MonthlyProductionPlan()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");

            Dictionary<int,string> year = new Dictionary<int,string>();
            year.Add(0,"select year"); year.Add(01,"2014"); year.Add(02,"2015"); year.Add(03,"2016"); year.Add(04,"2017"); year.Add(05,"2018"); year.Add(06,"2019"); year.Add(07,"2020"); year.Add(08,"2021"); year.Add(09,"2022"); year.Add(10,"2023"); year.Add(11,"2024"); year.Add(12,"2025");

            List<SelectListItem> yearlist = new List<SelectListItem>();
            yearlist.Add(new SelectListItem{Text="Select year",Value="0"});
            yearlist.Add(new SelectListItem { Text = "2014", Value = "01" });
            yearlist.Add(new SelectListItem { Text = "2015", Value = "02" });
            yearlist.Add(new SelectListItem { Text = "2016", Value = "03" });
            yearlist.Add(new SelectListItem { Text = "2017", Value = "04" });
            yearlist.Add(new SelectListItem { Text = "2018", Value = "05" });
            yearlist.Add(new SelectListItem { Text = "2019", Value = "06" });
            yearlist.Add(new SelectListItem { Text = "2020", Value = "07" });
            yearlist.Add(new SelectListItem { Text = "2021", Value = "08" });
            yearlist.Add(new SelectListItem { Text = "2022", Value = "09" });
            yearlist.Add(new SelectListItem { Text = "2023", Value = "10" });
            yearlist.Add(new SelectListItem { Text = "2024", Value = "11" });
            yearlist.Add(new SelectListItem { Text = "2025", Value = "12" });

            ViewBag.To_Date = new SelectList(yearlist, "value", "text", yearlist.Where(x => x.Text.ToString().ToLower() == DateTime.Now.Date.Year.ToString().ToLower()).FirstOrDefault().Value);
             ViewBag.From_Date=  new SelectList(new List<object> { new { value = "0", text = "-Select Month-" }, new { value = "1", text = "JAN" }, new { value = "2", text = "FEB" }, new { value = "3", text = "MAR" }, new { value = "4", text = "APRIL" }, new { value = "5", text = "MAY" }, new { value = "6", text = "JUNE" }, new { value = "7", text = "JULY" }, new { value = "8", text = "AUG" }, new { value = "9", text = "SEP" }, new { value = "10", text = "OCT" }, new { value = "11", text = "NOV" }, new { value = "12", text = "DEC" } }, "value", "text", DateTime.Now.Date.Month);
            globalData.actionName = "MonthlyProductionPlan";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Monthly Production Plan";
            globalData.contentFooter = "Monthly Production Plan";
            globalData.pageTitle = "Monthly Production Plan";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult PlanVSActual()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.actionName = "PlanVSActual";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Plan VS Actual";
            globalData.contentFooter = "Plan VS Actual";
            globalData.pageTitle = "Plan VS Actual";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult PlantProductionDashboard()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.actionName = "PlanProductionDashboard";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Plant Production Dashboard";
            globalData.contentFooter = "Plant Production Dashboard";
            globalData.pageTitle = "Plant Production Dashboard";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }
        public ActionResult LineStopReasons()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.actionName = "LineStopReasons";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Plant Line Stop Reasons";
            globalData.contentFooter = "Plant Line Stop Reasons";
            globalData.pageTitle = "Plant Line Stop Reasons";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }
        public ActionResult DailyOrderStartCount()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");

            globalData.actionName = "DailyOrderStartCount";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Daily Order Start Count";
            globalData.contentFooter = "Daily Order Start Count";
            globalData.pageTitle = "Daily Order Start Count";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult Paintorders()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.actionName = "Paintorders";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Paint Order List";
            globalData.contentFooter = "Paint Order List";
            globalData.pageTitle = "Paint Order List";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult WIPReport()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Plant_ID == plantId).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            //ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");

            globalData.actionName = "WIPReport";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "WIP Report";
            globalData.contentFooter = "WIP Report";
            globalData.pageTitle = "WIP Report";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult ProductionStatus()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");

            globalData.actionName = "ProductionStatus";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Production Status";
            globalData.contentFooter = "Production Status";
            globalData.pageTitle = "Production Status";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult EngineProductionDailyReport()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");

            globalData.actionName = "EngineProductionDailyReport";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Engine Production Daily Report";
            globalData.contentFooter = "Engine Production Daily Report";
            globalData.pageTitle = "Engine ProductionDaily Report";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult TransmissionProductionDailyReport()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");

            globalData.actionName = "TransmissionProductionDailyReport";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Transmission Production Daily Report";
            globalData.contentFooter = "Transmission Production Daily Report";
            globalData.pageTitle = "Transmission ProductionDaily Report";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult PlanProductionDashboard()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.actionName = "PlanProductionDashboard";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Plant Production Dashboard";
            globalData.contentFooter = "Plant Production Dashboard";
            globalData.pageTitle = "Plant Production Dashboard";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult PlanVsActualDashboard()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");

            globalData.actionName = "PlanVsActualDashboard";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Plan Vs Actual Dashboard";
            globalData.contentFooter = "Plan Vs Actual Dashboard";
            globalData.pageTitle = "Plan Vs Actual Dashboard";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult ActualTrend()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.actionName = "Actual Trend";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Actual Trend";
            globalData.contentFooter = "Actual Trend";
            globalData.pageTitle = "Actual Production";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult HistoryCardForTractor()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");

            globalData.actionName = "HistoryCardForTractor";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "History Card";
            globalData.contentFooter = "History Card";
            globalData.pageTitle = "History Card";
            ViewBag.SerialNo = new SelectList(db.RS_OM_Order_List.Where(x => x.Shop_ID == 4 && x.Partgroup_ID ==8).ToList(), "Serial_No", "Serial_No");
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();

        }

        public ActionResult EWT()
        {
            globalData.actionName = "EWT";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "EWT";
            globalData.contentFooter = "EWT";
            globalData.pageTitle = "EWT";
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();

        }

        public ActionResult DailyUses()
        {
            globalData.actionName = "DailyUses";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "DailyUses";
            globalData.contentFooter = "DailyUses";
            globalData.pageTitle = "DailyUses";
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();

        }

        public ActionResult SOPAvailableStatus()
        {
            globalData.actionName = "SOPAvailableStatus";
            globalData.controllerName = "Report";
            globalData.contentTitle = "SOP Available Status";
            globalData.contentFooter = "SOP Available Status";
            globalData.pageTitle = "SOP Available Status";
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();

        }

        public ActionResult GenerateReportProductionOrderSummary(string shopId, string lineId, string stationId, string orderStateId, string frmDate, string toDate)// string ModelId, string seriesId,
        {
            
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateReportMonthlyProductionPlan(string shopId, string lineId, string stationId, string frmDate, string toDate)
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLineByShopID(int shopId)
        {
            try
            {
                var st = from line in db.RS_Lines
                         where line.Shop_ID == shopId
                         orderby line.Line_Name
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStationByLineID(int lineId)
        {
            try
            {
                var st = from station in db.RS_Stations
                         where (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TractorDailyPlan()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "Tractor Daily Plan";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Tractor Daily Plan";
            globalData.contentFooter = "Tractor Daily Plan";
            globalData.pageTitle = "Tractor Daily Plan";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult EngineDailyPlan()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "EngineDailyPlan";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Engine Daily Plan";
            globalData.contentFooter = "Engine Daily Plan";
            globalData.pageTitle = "Engine Daily Plan";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult TransmissionDailyPlan()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "TransmissionDailyPlan";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Transmission Daily Plan";
            globalData.contentFooter = "Transmission Daily Plan";
            globalData.pageTitle = "Transmission Daily Plan";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult HydraulicDailyPlan()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "HydraulicDailyPlan";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Hydraulic Daily Plan";
            globalData.contentFooter = "Hydraulic Daily Plan";
            globalData.pageTitle = "Hydraulic Daily Plan";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        //Equipment Creation Status
        //Author: Jitendra Mahajan
        public ActionResult SAP_Equipment_Status()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "SAP_Equipment_Status";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "SAP Equipment Creation Status";
            globalData.contentFooter = "SAP Equipment Creation Status";
            globalData.pageTitle = "SAP Equipment Creation Status";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }


        //Paint shop summarry
        //Author: Jitendra Mahajan
        public ActionResult PaintShop_Summary()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "PaintShop_Summary";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Paint Shop Summary Details";
            globalData.contentFooter = "Paint Shop Summary Details";
            globalData.pageTitle = "Paint Shop Summary Details";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        //Colorwise Summary
        //Author: Jitendra Mahajan
        public ActionResult Colorwise_Summary()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "Colorwise_Summary";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Colorwise Summary Details";
            globalData.contentFooter = "Colorwise Summary Details";
            globalData.pageTitle = "Colorwise Summary Details";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        //Paint Shop Details Summary
        //Author: Jitendra Mahajan
        public ActionResult PaintShop_Details_Summary()
        {
            ViewBag.Station_ID = new SelectList(db.RS_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.RS_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "PaintShop_Details_Summary";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Paint Shop Details Summary";
            globalData.contentFooter = "Paint Shop Details Summary";
            globalData.pageTitle = "Paint Shop Details Summary";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }
       
        public ActionResult TraceabilityData()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            globalData.controllerName = "Reports";
            globalData.pageTitle = "Traceability Report";
            ViewBag.GlobalDataModel = globalData;
            return View();
        }
    }
}