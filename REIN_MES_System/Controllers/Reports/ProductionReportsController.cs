using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.Reports
{
    public class ProductionReportsController : BaseController //Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        // GET: ProductionReports
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductionDashboard()
        {
            return View();
        }
        public ActionResult PlanVsActual()
        {
            return View();
        }
        public ActionResult ProductionWithStatus()
        {
            return View();
        }
        public ActionResult HourlyProduction()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Plant_ID == plantId).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(x => x.Platform_ID == 0).OrderBy(c => c.Platform_Name), "Platform_ID", "Platform_Name", 0);


            globalData.actionName = "HourlyProduction";
            globalData.controllerName = "ProductionReports";
            globalData.contentTitle = "BIW Hourly Production Report";
            globalData.contentFooter = "BIW Hourly Production Report";
            globalData.pageTitle = "BIW Hourly Production Report";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }
        public ActionResult ShopProduction()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Plant_ID == plantId).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(x => x.Platform_ID == 0).OrderBy(c => c.Platform_Name), "Platform_ID", "Platform_Name", 0);


            globalData.actionName = "ShopProductionReport";
            globalData.controllerName = "ProductionReports";
            globalData.contentTitle = "Shop Production Report";
            globalData.contentFooter = "Shop Production Report";
            globalData.pageTitle = "Shop Production Report";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult ShopWIPStatusReport()
        {
            var Shop = (from status in db.RS_Status
                        join shops in db.RS_Shops
                        on status.Shop_ID equals shops.Shop_ID
                        select new
                        {
                            Text = shops.Shop_Name,
                            Value = status.Shop_ID
                        }).Distinct();

            ViewBag.Shop_ID = new SelectList(Shop, "Value", "Text");
            ViewBag.FromStage = new SelectList(db.RS_Status.Where(a => a.Shop_ID == 0), "Row_ID", "Status").Distinct();
            ViewBag.ToStage = new SelectList(db.RS_Status.Where(a => a.Shop_ID == 0), "Row_ID", "Status").Distinct();
            //ViewBag.FromStage = new SelectList(db.RS_Status.Select(a=>a.Status), "Row_ID", "Status").ToList().Distinct().OrderBy(m=>m.;

            globalData.actionName = "ShopStatus";
            globalData.controllerName = "ShopStatus";
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult GetFromStageByShop(int shopid)
        {

            var FromStage = db.RS_Status.Where(c => (c.Shop_ID == shopid) && (c.Is_Active == true)).Select(a => new { a.Sort_Order, a.Status });
            return Json(FromStage, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetToStageByShop(int shopid, string stage)
        {
            int sortOrder = Convert.ToInt32(db.RS_Status.Where(m => m.Shop_ID == shopid && m.Status == stage).Select(m => m.Sort_Order).FirstOrDefault());
            var ToStage = db.RS_Status.Where(c => (c.Shop_ID == shopid) && (c.Is_Active == true) && (c.Sort_Order > sortOrder)).Select(a => new { a.Sort_Order, a.Status });
            return Json(ToStage, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statuswisedata()
        {
            try
            {

                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Plant_ID == plantId).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
                ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(x => x.Platform_ID == 0).OrderBy(c => c.Platform_Name), "Platform_ID", "Platform_Name", 0);
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(x => x.Plant_ID == plantId).OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", 0);
            }
            catch (Exception ex)
            {
                throw;
            }
            return View();

        }

        public ActionResult GetStatusListByLinewise(int shopId, int LineId, DateTime Date)
        {
            var data = (from Ostatus in db.RS_OM_OrderRelease
                        join st in db.RS_Status on Ostatus.Order_Status equals st.Status
                        where Ostatus.Shop_ID == shopId && Ostatus.Line_ID == LineId && st.Is_Active == true && Ostatus.Planned_Date == Date
                        group Ostatus by Ostatus.Order_Status into g
                        select new
                        {
                            Order_Status = g.Key,
                            ProductionCount = g.Count()
                        });


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StagewiseProductionReport()
        {
            try
            {

                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(x => x.Plant_ID == plantId).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
                ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(x => x.Shop_ID == 0).OrderBy(c => c.Platform_Name), "Platform_ID", "Platform_Name", 0);
                globalData.pageTitle = "Stage wise Production Report";
                ViewBag.GlobalDataModel = globalData;
            }
            catch (Exception ex)
            {
                throw;
            }
            return View();

        }

        public ActionResult VehicleStatusReport()
        {
            try
            {
                globalData.pageTitle = "Vehicle Status Report";
                ViewBag.GlobalDataModel = globalData;
            }
            catch (Exception ex)
            {
                throw;
            }
            return View();
        }

        public ActionResult GetPlatform(decimal Shop_Id)
        {
            var platformDetail = (from platform in db.RS_OM_Platform
                                  join shop in db.RS_Shops on platform.Shop_ID equals shop.Shop_ID
                                  where platform.Shop_ID == Shop_Id
                                  select new
                                  {
                                      platform.Platform_Name,
                                      platform.Platform_ID
                                  }).Distinct().ToList();
            return Json(platformDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlatformID(int Shop_Id)
        {
            var platformDetail = (from platform in db.RS_OM_Platform
                                  join partgroup in db.RS_Partgroup on platform.Line_ID equals partgroup.Line_ID
                                  where platform.Shop_ID == Shop_Id && partgroup.Order_Create == true
                                  select new
                                  {
                                      platform.Platform_Name,
                                      platform.Platform_ID
                                  }).Distinct().ToList();
            return Json(platformDetail, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetShiftByShop(int Shop_Id)
        {
            var ShiftDetail = (from shift in db.RS_Shift
                                  where shift.Shop_ID == Shop_Id
                                  select new
                                  {
                                      shift.Shift_Name,
                                      shift.Shift_ID,
                                      shift.Shift_Start_Time,
                                      shift.Shift_End_Time
                                  }).Distinct().ToList();
            return Json(ShiftDetail, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetFirstShiftTime(int Shop_Id)
        //{
        //    var ShiftDetail = db.RS_Shift.Where(m => m.Shop_ID == Shop_Id).Select(m=>m.Shift_Start_Time).FirstOrDefault();
        //    if (ShiftDetail == null)
        //    {
        //        return Json("07:00 AM", JsonRequestBehavior.AllowGet);
        //    }
        //    var ampm = ShiftDetail.Hours >= 12 ? "PM" : "AM";
        //    var hour = ShiftDetail.Hours < 10 ? "0" + ShiftDetail.Hours : ShiftDetail.Hours.ToString();
        //    var minute = ShiftDetail.Minutes < 10 ? "0" + ShiftDetail.Minutes : ShiftDetail.Minutes.ToString();



        //    var shiftStartTime = hour +":"+ minute +" "+ ampm;
        //    return Json(shiftStartTime, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetFirstShiftTime(int Shop_Id)
        {
            var ShiftDetail = db.RS_Shift.Where(m => m.Shop_ID == Shop_Id).Select(m => m.Shift_Start_Time).FirstOrDefault();
            if (ShiftDetail == null)
            {
                return Json("07:00", JsonRequestBehavior.AllowGet);
            }
            //var ampm = ShiftDetail.Hours >= 12 ? "PM" : "AM";
            var hour = ShiftDetail.Hours < 10 ? "0" + ShiftDetail.Hours : ShiftDetail.Hours.ToString();
            var minute = ShiftDetail.Minutes < 10 ? "0" + ShiftDetail.Minutes : ShiftDetail.Minutes.ToString();



            var shiftStartTime = hour + ":" + minute;
            return Json(shiftStartTime, JsonRequestBehavior.AllowGet);
        }
    }
}