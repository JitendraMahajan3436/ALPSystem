using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using System.Transactions;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System.Globalization;
using System.IO;

namespace REIN_MES_System.Controllers.TrackingManagement
{
    public class TrackingController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        // GET: Tracking
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EngineShop()
        {
            return View();
        }

        public ActionResult MachineShop()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.Tracking_MachineShop;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult MDURA()
        {
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 1).Select(m => m.Shop_Name).FirstOrDefault();
            var buttonshopName1 = db.RS_Shops.Where(m => m.Shop_ID == 1).Select(m => m.Shop_Name).FirstOrDefault();
            ViewBag.ButtonTitle2 = buttonshopName1;



            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.Tracking_MachineShop;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }
        public ActionResult MDURA1()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 1).Select(m => m.Shop_Name).FirstOrDefault();
            var buttonshopName = db.RS_Shops.Where(m => m.Shop_ID == 3).Select(m => m.Shop_Name).FirstOrDefault();
            ViewBag.ButtonTitle = buttonshopName;


            globalData.pageTitle = shopName ;
            globalData.pageTitle = shopName ;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult JeetoBody()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 13).Select(m => m.Shop_Name).FirstOrDefault();
            
            globalData.pageTitle = shopName;
            globalData.pageTitle = shopName;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult LCVBody()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 12).Select(m => m.Shop_Name).FirstOrDefault();

            globalData.pageTitle = shopName;
            globalData.pageTitle = shopName;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult UVBody()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 14).Select(m => m.Shop_Name).FirstOrDefault();

            globalData.pageTitle = shopName;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult ALFABody()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 35).Select(m => m.Shop_Name).FirstOrDefault();

            globalData.pageTitle = shopName;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult PaintShop()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 29).Select(m => m.Shop_Name).FirstOrDefault();

            globalData.pageTitle = shopName;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult PDI()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            //var shopName = db.RS_Shops.Where(m => m.Shop_ID == 29).Select(m => m.Shop_Name).FirstOrDefault();

            globalData.pageTitle = "PDI";
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult LCVTCF()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 19).Select(m => m.Shop_Name).FirstOrDefault();

            globalData.pageTitle = shopName;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult UVTCF()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var shopName = db.RS_Shops.Where(m => m.Shop_ID == 11).Select(m => m.Shop_Name).FirstOrDefault();

            globalData.pageTitle = shopName;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        public ActionResult JEETOALFATCF()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            //var shopName = db.RS_Shops.Where(m => m.Shop_ID == 17).Select(m => m.Shop_Name).FirstOrDefault();

            globalData.pageTitle = "JEETO ALFA TCF";
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Tracking";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Shop + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult checkIfLineStop(decimal shopid)
        {
            //var lineStopObj = (from a in db.RS_Lines
            //                  join b in db.MM_ on a.Station_ID equals b.Station_ID
            //                  join c in db.RS_OM_Order_List on b.SerialNo equals c.Serial_No into bc
            //                  from c in bc.DefaultIfEmpty()
            //                  where a.Station_ID == stationid
            //                  select new { 

            var lineStopObj = db.RS_Lines
                              .Where(a => a.isLineStop == true && a.Shop_ID == shopid)
                              .Select(a => new { a.Line_ID, a.LineStopStation_ID })
                              .Distinct();

            return Json(lineStopObj, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getMESLineStopStation(decimal shopid)
        {
            var mesLineStopsObj = db.RS_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Time != null && a.Shop_ID == shopid)
                                    .Select(a => new { a.Station_ID, a.isLineStop, a.Stop_Reason, a.Stop_Time, a.Remarks }).ToList();
            return Json(mesLineStopsObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult trackOrder(decimal shopid)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted

                }))
            {
                var orderTrackingObj = (from a in db.RS_Stations
                                        join b in db.RS_Station_Tracking on a.Station_ID equals b.Station_ID
                                        join c in db.RS_OM_Order_List on b.Track_SerialNo equals c.Serial_No into bc
                                        from c in bc.DefaultIfEmpty()
                                        where a.Shop_ID == shopid && a.Is_Buffer != true
                                        select new
                                        {
                                            Station_ID = a.Station_ID,
                                            SerialNo = b.Track_SerialNo.Trim().ToUpper(),
                                            Series_Description = c.RS_Series.Series_Description,
                                            Is_Empty = b.isEmptyPitch == null ? false : b.isEmptyPitch == false ? false : true
                                        })
                                           .OrderBy(a => a.Station_ID);
                //db.RS_Station_Tracking.Where(a => a.Shop_ID == shopid)
                //                   .Select(a => new { a.Station_ID, a.SerialNo })
                //                   .OrderBy(a => a.Station_ID);

                return Json(orderTrackingObj, JsonRequestBehavior.AllowGet);
            }
        }


        public IEnumerable<RS_OM_OrderRelease> getReleasedOrdersList(decimal shopID, decimal lineID)
        {
            //int plannedQty = db.RS_OM_PPC_Daily_Plan.Where(a => a.Shop_ID == shopID && a.Plan_Date == DateTime.Today).Select(a => a.Planned_Qty).FirstOrDefault();
            //return db.RS_OM_OrderRelease.Where(or => or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release")
            //                .AsEnumerable().OrderBy(or => or.RSN).ToList();

            return (from orderReleaseItem in db.RS_OM_OrderRelease
                    orderby orderReleaseItem.RSN
                    where orderReleaseItem.Shop_ID == shopID && orderReleaseItem.Line_ID == lineID && orderReleaseItem.Order_Status == "Release" && orderReleaseItem.Planned_Date == DateTime.Today
                    select orderReleaseItem).ToList();
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getReleasedOrders(decimal shopID, decimal lineID)
        {
            IEnumerable<RS_OM_OrderRelease> OrderReleased = getReleasedOrdersList(shopID, lineID);
            return PartialView(OrderReleased);
        }
        public ActionResult DownloadAvixFile(string documentPath)
        {

            string fullpath = Server.MapPath("~/Content/AvixFiles/" + documentPath);
            string filename = HttpUtility.UrlEncode(Path.GetFileName(fullpath));
            if (!System.IO.File.Exists(fullpath))
            {
                return Content("This file is not available on server............! File Name  : " + documentPath);
            }
            else
            {
                Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
                return File(fullpath, MimeMapping.GetMimeMapping(filename));
            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getStationData(decimal shopid, decimal stationid)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                }))
            {
                var stationObj = (from a in db.RS_Stations
                                  join b in db.RS_Station_Tracking on a.Station_ID equals b.Station_ID into ab
                                  from b in ab.DefaultIfEmpty()
                                  join c in db.RS_OM_Order_List on b.Track_SerialNo equals c.Serial_No into bc
                                  from c in bc.DefaultIfEmpty()
                                  where a.Station_ID == stationid
                                  select new { Station_ID = a.Station_ID, Station_Name = a.Station_Name, DSN = c.DSN, SerialNo = b.Track_SerialNo.Trim().ToUpper(), partno = c.partno, Series_Description = c.RS_Series.Series_Description }).FirstOrDefault();

                return Json(stationObj, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getStationAssociates(decimal shopid, decimal stationid)
        {
            try
            {

                RS_Quality_Captures cap = new RS_Quality_Captures();
                var currentshift = cap.getCurrentRunningShiftByShopID(Convert.ToInt32(shopid));
                var day = DateTime.Today.DayOfWeek;
                var cult = CultureInfo.CurrentCulture;
                int weekNo = cult.Calendar.GetWeekOfYear(new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);

                var associatesObj = (from a in db.RS_AM_Operator_Station_Allocation
                                     join b in db.RS_Employee on a.Employee_ID equals b.Employee_ID
                                     join c in db.RS_AM_Employee_SkillSet on b.Employee_ID equals c.Employee_ID //into bc
                                     where a.Allocation_Date.Year == DateTime.Now.Year
                                     && a.Allocation_Date.Month == DateTime.Now.Month
                                     && a.Allocation_Date.Day == DateTime.Now.Day
                                      && a.Station_ID == stationid && c.Station_ID == stationid && a.Shift_ID == currentshift.Shift_ID
                                     select new PresentAbsentEmployee { Station_ID = a.Station_ID, Employee_ID = b.Employee_ID, Employee_Name = b.Employee_Name, Employee_No = b.Employee_No, Skill_ID = c.Skill_ID }).Distinct().ToList();
                List<PresentAbsentEmployee> PresentObj = new List<PresentAbsentEmployee>();
                foreach (var items in associatesObj)
                {
                    PresentAbsentEmployee presentEMp = new PresentAbsentEmployee();
                    var res = (from att in db.RS_User_Attendance_Sheet
                               where att.Entry_Date.Value.Year == DateTime.Now.Year
                               && att.Entry_Date.Value.Month == DateTime.Now.Month
                               && att.Entry_Date.Value.Day == DateTime.Now.Day
                               && att.Employee_ID == items.Employee_ID
                               orderby att.Attendance_ID descending
                               select att).FirstOrDefault();
                    if (res != null)
                    {
                        if (res.Status == "CMS81")
                            PresentObj.Add(items);
                    }
                }
                var IsAssocaitesPrsent = associatesObj.Count() == 0 ? 0 : 1;

                return Json(new { PresentObj = PresentObj, IsAssocaitesPrsent = IsAssocaitesPrsent == 0 ? "No Associates assigned ! " : "Assigned Employee Absent!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getMachineDetails(decimal shopid)
        {
            try
            {
                var machineObj = db.RS_MT_Machines.Where(m => m.Shop_ID == shopid).Select(m => m.Station_ID).ToList();

                return Json(machineObj, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                return null;
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getStationMachines(decimal shopid, decimal stationid)
        {
            try
            {
                var machineObj = (from a in db.RS_MT_Machines
                                  where a.Station_ID == stationid
                                  select new { Station_ID = a.Station_ID, Machine_ID = a.Machine_ID, a.Machine_Number, a.Machine_Name }).ToList();

                return Json(machineObj, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                return null;
            }
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getAjaxBufferData(decimal stationID)
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            //DateTime thisDate = DateTime.Today.AddDays(1);
            //DateTime today = DateTime.Today;
            //IEnumerable<RS_OM_Order_List> bufferObj =
            //    (from a in db.RS_Line_Complete_Buffer
            //     join b in db.RS_OM_Order_List on a.SerialNo equals b.Serial_No
            //     where a.Station_ID == stationID && a.isConsumed == false && b.Order_Status != "Closed"
            //     && DbFunctions.TruncateTime(b.Inserted_Date) == today
            //     select b).AsEnumerable().OrderBy(b => b.Inserted_Date).ToList();
            string sqlString = "SELECT Model_Code,c.Series_Description as Series,COUNT(Model_Code) as Qty from RS_Line_Complete_Buffer a" +
                              " JOIN  RS_OM_Order_List b on a.SerialNo = b.Serial_No" +
                              " JOIN RS_Series c ON b.Series_Code = c.Series_Code" +
                              " WHERE a.Station_ID = @p0 AND a.isConsumed = 0 AND b.Order_Status != 'Closed'" +
                              " AND CONVERT(DATE,b.Inserted_Date) >= @p1" +
                              " GROUP BY Model_Code,Series_Description";
            List<CummulativeFields> bufferModelsList = db.Database.SqlQuery<CummulativeFields>(sqlString, stationID, yesterday).ToList();
            ViewBag.BufferData = bufferModelsList;
            ViewBag.TotalQty = bufferModelsList.Sum(a => a.Qty);
            ViewBag.StationName = db.RS_Stations.Find(stationID).Station_Name;

            return PartialView("AjaxShowBuffer");
        }

        public ActionResult GetBerakLunchTime(int ShopID)
        {
            RS_Quality_Captures cap = new RS_Quality_Captures();
            string str = "";
            var shift = cap.getCurrentRunningShiftByShopID(ShopID);
            if (DateTime.Now.TimeOfDay >= shift.Break1_Time && DateTime.Now.TimeOfDay <= shift.Break1_End_Time)
            {
                var B1 = new DateTime(shift.Break1_Time.Value.Ticks); // Date part is 01-01-0001
                var formattedTimeB1 = B1.ToString("h:mm tt", CultureInfo.InvariantCulture);
                var B1End = new DateTime(shift.Break1_End_Time.Value.Ticks); // Date part is 01-01-0001
                var formattedTimeB1End = B1End.ToString("h:mm tt", CultureInfo.InvariantCulture);
                str = "Break Time 1 Between :" + formattedTimeB1 + " TO " + formattedTimeB1End;
            }
            else if (DateTime.Now.TimeOfDay >= shift.Break2_Time && DateTime.Now.TimeOfDay <= shift.Break2_End_Time)
            {
                var B2 = new DateTime(shift.Break2_Time.Value.Ticks); // Date part is 01-01-0001
                var formattedTimeB2 = B2.ToString("h:mm tt", CultureInfo.InvariantCulture);
                var B2End = new DateTime(shift.Break2_End_Time.Value.Ticks); // Date part is 01-01-0001
                var formattedTimeB2End = B2End.ToString("h:mm tt", CultureInfo.InvariantCulture);
                str = "Break Time 2 Between :" + formattedTimeB2 + " TO " + formattedTimeB2End;
            }
            else if (DateTime.Now.TimeOfDay >= shift.Lunch_Time && DateTime.Now.TimeOfDay <= shift.Lunch_End_Time)
            {
                var L1 = new DateTime(shift.Lunch_Time.Value.Ticks); // Date part is 01-01-0001
                var formattedTimeB2 = L1.ToString("h:mm tt", CultureInfo.InvariantCulture);
                var L1End = new DateTime(shift.Lunch_End_Time.Value.Ticks); // Date part is 01-01-0001
                var formattedTimeB1End = L1End.ToString("h:mm tt", CultureInfo.InvariantCulture);
                str = "Lunch Time Between :" + shift.Lunch_Time + " TO " + shift.Lunch_End_Time;
            }
            return Json(str, JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        //public ActionResult getStationDefects(decimal shopid, decimal stationid, string serialno)
        //{
        //    var stationObj = db.RS_Quality_Captures.Where(a => a.Serial_No == serialno && a.Is_Clear == false).OrderByDescending(a => a.Inserted_Date)
        //                                           .Select(a => new { a.MM_Quality_Checklist.Checklist_Name, a.MM_Quality_Checkpoint.Checkpoint_Name, a.Is_Value_Based, a.User_Checkpoint_Value, a.RS_Quality_Defect.Defect_Name, a.Is_Clear, a.RS_Quality_Corrective_Actions.Corrective_Action_Name });

        //    return Json(stationObj, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getSignalsSummary(int shopID)
        {
            IEnumerable<RS_Ctrl_Alarm_Signals_History> resultObj = db.RS_Ctrl_Alarm_Signals_History.Where(a => a.Shop_ID == shopID && (a.Work_Delay != true || a.Work_Delay == false))
                                                             .OrderByDescending(a => a.Start_Time).Take(5);
            //return Json(resultObj, JsonRequestBehavior.AllowGet);

            ViewBag.CallSummary = resultObj;
            return PartialView("PVAlarmSummary");
            //string sqlqry = " SELECT COUNT(Line_Stop) as Line_Stop,COUNT(Material_Call) as Material_Call,COUNT(Supervisor_Call) as Supervisor_Call,COUNT(Maintenance_Call) as Maintenance_Call,COUNT(Emergency_Call) as Emergency_Call,COUNT(Work_Delay) as Work_Delay" +
            //                " FROM MM_Ctrl_Alarm_Signals" +
            //                " WHERE (Line_Stop = 1 OR Material_Call = 1 OR Supervisor_Call = 1 OR Maintenance_Call = 1 OR Emergency_Call = 1 OR Work_Delay = 1) "+
            //                " AND CONVERT(DATE,Inserted_Date) = CONVERT(DATE,GETDATE())";

            //IEnumerable<SequenceData> full_sequence = db.Database.SqlQuery<>(sqlqry, triggershopid, lastVin, partgroupid, triggerid);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getStationCalls(decimal shopid)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
               new TransactionOptions
               {
                   IsolationLevel = IsolationLevel.ReadUncommitted
               }))
            {
                var stationAlarmsObj = (from a in db.RS_Stations
                                        join b in db.RS_Station_Alarms on a.Station_ID equals b.Station_ID
                                        where a.Shop_ID == shopid
                                        select new { b.Material_Call, b.Supervisor_Call, b.Maintenance_Call, b.Line_Stop, b.Emergency_Call, b.Work_Delay, b.Station_ID }).ToList();

                return Json(stationAlarmsObj, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getStationSignals(decimal shopid, decimal stationid)
        {
            var stationAlarmsObj = (from a in db.RS_Stations
                                    join b in db.RS_Station_Alarms on a.Station_ID equals b.Station_ID
                                    where a.Station_ID == stationid && a.Shop_ID == shopid
                                    select new { b.Material_Call, b.Supervisor_Call, b.Maintenance_Call, b.Line_Stop, b.Emergency_Call, b.Work_Delay, b.Station_ID }).FirstOrDefault();

            return Json(stationAlarmsObj, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getMachinStatus(decimal shopid)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
               new TransactionOptions
               {
                   IsolationLevel = IsolationLevel.ReadUncommitted
               }))
            {
                var status = (from a in db.RS_Ctrl_Equipment_Status
                              join b in db.RS_MT_Machines on a.Machine_ID equals b.Machine_ID
                              join c in db.RS_Stations on b.Station_ID equals c.Station_ID
                              where c.Shop_ID == shopid && b.Station_ID != null
                              select new
                              {
                                  a.EQ_ID,
                                  a.Machine_ID,
                                  b.Station_ID,
                                  a.isFaulty,
                                  a.Heart_Bit,
                                  a.isHealthy,
                                  a.isIdle,
                                  a.Inserted_Date
                              })
                    .GroupBy(c => c.Machine_ID)
                    .Select(g => g.OrderByDescending(c => c.EQ_ID).FirstOrDefault())
                    .Where(a => a.isFaulty == true)
                    .Select(c => new { c.Station_ID, c.Machine_ID, c.isFaulty, c.isHealthy, c.isIdle, c.Heart_Bit });

                return Json(status, JsonRequestBehavior.AllowGet);
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getTakeInOutData(decimal shopid)
        {
            var takeInOutObj = db.RS_Quality_Take_In_Take_Out.Where(a => a.Rework_Status == "Not Good" || a.Rework_Status == "Good")
                                           .Select(a => new { a.Serial_No, a.Rework_Status }).ToList();

            return Json(takeInOutObj, JsonRequestBehavior.AllowGet);
        }

        public int getRunningShiftByShopID(int shopId)
        {
            try
            {
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                RS_Shift mmShiftObj = (from shiftObj in db.RS_Shift
                                       where shiftObj.Shop_ID == shopId
                                && TimeSpan.Compare(shiftObj.Shift_Start_Time, currDate) < 0
                                && TimeSpan.Compare(shiftObj.Shift_End_Time, currDate) > 0
                                       select shiftObj).FirstOrDefault();
                return Convert.ToInt16(mmShiftObj.Shift_ID);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public ActionResult getStationAllocation(int shopId, string stationStr)
        {
            try
            {
                int shiftId = getRunningShiftByShopID(shopId);
                String[] stnId = stationStr.Split(',');
                // List stationList = new List();
                List<decimal> stationList = new List<decimal>();
                for (int i = 0; i < stnId.Count(); i++)
                {
                    stationList.Add(Convert.ToDecimal(stnId[i]));
                }
                RS_Stations[] mmStation = db.RS_Stations.Where(p => p.Shop_ID == shopId && stationList.Contains(p.Line_ID)).ToArray();
                StationOperator[] stationOperator = new StationOperator[mmStation.Count()];
                int plantId = 2;
                var day = DateTime.Today.DayOfWeek.ToString();
                var cult = CultureInfo.CurrentCulture;
                int weekNo = cult.Calendar.GetWeekOfYear(new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);

                for (int i = 0; i < mmStation.Count(); i++)
                {
                    stationOperator[i] = new StationOperator();
                    int stationId = Convert.ToInt16(mmStation[i].Station_ID);
                    decimal lineId = mmStation[i].Line_ID;
                    stationOperator[i].Station_ID = stationId;
                    // process to get number of employee assigned for shift
                    RS_Employee[] employeeList = (from emp in db.RS_Employee
                                                  join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
                                                  where operatorAllocation.Shift_ID == shiftId && operatorAllocation.Line_ID == lineId && operatorAllocation.Station_ID == stationId && emp.Is_Deleted == null && emp.Plant_ID == plantId
                                                  //&& DbFunctions.TruncateTime(operatorAllocation.Allocation_Date) == DbFunctions.TruncateTime(DateTime.Now) 
                                                  && operatorAllocation.Week_Day == day && operatorAllocation.Week_Number == weekNo
                                                  select emp
                                ).Distinct().ToArray();
                    int totalEmployee = employeeList.Count();
                    if (totalEmployee == 0)
                    {
                        stationOperator[i].IsUnAllocated = true;
                        continue;
                    }
                    else
                    {
                        int presentCount = 0;
                        // process to check employee is present or not
                        for (int j = 0; j < employeeList.Count(); j++)
                        {
                            decimal employeeId = employeeList[j].Employee_ID;
                            String userAttendanceSheet = db.RS_User_Attendance_Sheet.Where(c => c.Entry_Date.Value.Day == DateTime.Now.Day
                                                                       && c.Entry_Date.Value.Year == DateTime.Now.Year
                                                                       && c.Entry_Date.Value.Month == DateTime.Now.Month
                                                                       && c.Employee_ID == employeeId
                                                                       ).OrderByDescending(c => c.Attendance_ID).Select(c => c.Status).FirstOrDefault() == "CMS81" ? "" : "ABSENT";
                            if (userAttendanceSheet == "")
                            {
                                // present
                                presentCount++;
                            }
                        }

                        if (presentCount == 0)
                        {
                            stationOperator[i].IsFree = true;
                        }
                        else if (presentCount < employeeList.Count())
                        {
                            stationOperator[i].IsPartial = true;
                        }
                        else
                        {
                            stationOperator[i].IsAllocated = true;
                        }

                    }
                }
                return Json(stationOperator, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getMachines(decimal machineid)
        {
            try
            {
                var machineObj = (from a in db.RS_MT_Machines
                                  where a.Machine_ID == machineid && a.IsActive == true
                                  select new { Machine_ID = a.Machine_ID, a.Machine_Number, a.Machine_Name }).ToList();

                return Json(machineObj, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                return null;
            }
        }
        public ActionResult getmachineimage(int machineid)

        {
            var imagadata = (from image in db.RS_Machine_Image
                             where image.Machine_ID == machineid
                             select new
                             {
                                 Imagedata = image.Image_Content,
                                 ImageId = image.Image_ID,
                                 ImageType = image.Image_Type
                             }


                           ).Distinct().ToList();
            var jsonresult = Json(imagadata, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = Int32.MaxValue;
            return jsonresult;
            
        }

    }
}
public class StationOperator
{
    public int Station_ID { get; set; }
    public bool IsAllocated { get; set; }
    public bool IsFree { get; set; }
    public bool IsPartial { get; set; }
    public bool IsUnAllocated { get; set; }
}