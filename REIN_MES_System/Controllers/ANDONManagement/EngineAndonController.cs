using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.MaintenanceManagement
{
    public class EngineAndonController : BaseController
    {
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        REIN_SOLUTIONEntities db1 = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        // GET: EngineAndon
        public ActionResult Index()
        {
            try
            {
                var plan = db1.MM_OM_PPC_Daily_Plan.Where(x => x.Shop_ID == 1 && x.Plan_Date == DateTime.Today).Select(x => x.Planned_Qty).FirstOrDefault();
                ViewBag.plan = plan.ToString();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult TractorShop()
        {
            return View();
        }

        public ActionResult VTUShop()
        {
            return View();
        }
        public ActionResult TransmissionShop()
        {
            return View();
        }
        public class ShiftDetails
        {
            public TimeSpan ShiftStart { get; set; }
            public TimeSpan ShiftEnd { get; set; }
        }
        public ActionResult getshift(int shopid)
        {
            try
            {
                var shiftobj = db1.MM_Shift.Where(a => a.Shift_Start_Time != null && a.Shift_End_Time != null && a.Shop_ID == shopid).Select(a => new ShiftDetails { ShiftStart = a.Shift_Start_Time, ShiftEnd = a.Shift_End_Time }).FirstOrDefault();
                return Json(shiftobj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult getMESLineStopStation(decimal shopid, int lineid)
        {
            try
            {
                var mesLineStopsObj = db1.MM_History_LineStop.Where(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Time != null && a.Shop_ID == shopid && a.Line_ID == lineid)
                                        .Select(a => new { a.Station_ID, a.isLineStop, a.Stop_Reason, a.Stop_Time, a.Remarks }).ToList();
                return Json(mesLineStopsObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult getStationCalls(decimal shopid, int lineid)
        {
            try
            {
                using (new TransactionScope(TransactionScopeOption.Required,
                   new TransactionOptions
                   {
                       IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                   }))
                {
                    var stationAlarmsObj = (from a in db1.MM_Stations
                                            join b in db1.MM_Station_Alarms on a.Station_ID equals b.Station_ID
                                            where a.Shop_ID == shopid && a.Line_ID == lineid
                                            select new { b.Material_Call, b.Supervisor_Call, b.Maintenance_Call, b.Line_Stop, b.Emergency_Call, b.Station_ID }).ToList();

                    return Json(stationAlarmsObj, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public class myresult
        {
            public decimal Value { get; set; }
            public int? Count { get; set; }
        }
        public class MachineCycle
        {
            public int machineID { get; set; }
            public bool isHealthy { get; set; }
            public bool isIdle { get; set; }
            public bool isFaulty { get; set; }

        }

        public ActionResult TestBedAndon()
        {
            try
            {
                var plan = db1.MM_OM_PPC_Daily_Plan.Where(x => x.Shop_ID == 1 && x.Plan_Date == DateTime.Today).Select(x => x.Planned_Qty).FirstOrDefault();
                ViewBag.plan = plan.ToString();
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }

        public ActionResult TestBedCountForEngine()
        {
            try
            {
                var Machines = new string[] { "20", "21", "22", "23", "24", "25", "26", "27", "28", "29" };
                var res = (from x in db.MM_Ctrl_TBM_MachineEquipment
                           where Machines.Contains(x.Machine_ID.ToString())
                           && x.Inserted_Date.Value.Day == DateTime.Today.Day
                            && x.Inserted_Date.Value.Month == DateTime.Today.Month
                             && x.Inserted_Date.Value.Year == DateTime.Today.Year
                             && x.Is_Cycle_Complete == true
                           group x by x.Machine_ID into g
                           let count = g.Count()
                           orderby g.Key ascending
                           select new myresult { Value = g.Key, Count = count }).ToList();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult ActualProduction(int shopid, int stationId)
        {
            var res = db1.MM_SAP_Production_Booking.Where(om => om.Shop_ID == shopid && om.Station_ID == stationId && om.Inserted_Date.Value.Day == DateTime.Today.Day && om.Inserted_Date.Value.Month == DateTime.Today.Month && om.Inserted_Date.Value.Year == DateTime.Today.Year).Count();
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult askingRate(int shopid)
        {
            var res = 10;
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TestBedTotal()
        {
            try {
                var Machines = new string[] { "31","32","33","34","35" };
                var res = (from x in db.MM_Ctrl_TBM_MachineEquipment
                           where Machines.Contains(x.Machine_ID.ToString())
                           && x.Inserted_Date.Value.Day == DateTime.Today.Day
                            && x.Inserted_Date.Value.Month == DateTime.Today.Month
                             && x.Inserted_Date.Value.Year == DateTime.Today.Year
                             && x.Is_Cycle_Complete == true select x.Machine_ID).ToList().Count();
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult ConveyorStatusData()
        {
            try
            {
                var lineStopObj = db1.MM_Lines
                                  .Where(a => a.Is_Conveyor == true)
                                  .Select(a => new { a.Line_ID, a.Line_Name, a.LineStopStation_ID, a.isLineStop, a.LineStop_Time })
                                  .Distinct();

                return Json(lineStopObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult TestBedCycleData(String machineId)
        {
            try
            {
                var Machines = new string[] { "20", "21", "22", "23", "24", "25", "26", "27", "28", "29" };
                Machines = machineId.Split(',');
                var res = (from x in db1.MM_Ctrl_Equipment_Status
                           where Machines.Contains(x.Machine_ID.ToString())
                          && x.Inserted_Date.Value.Day == DateTime.Today.Day
                           && x.Inserted_Date.Value.Month == DateTime.Today.Month
                            && x.Inserted_Date.Value.Year == DateTime.Today.Year
                           group x by x.Machine_ID

                      into g

                           select g.OrderByDescending(p => p.Inserted_Date).Select(p => new { p.Machine_ID, p.isFaulty, p.isHealthy, p.isIdle, p.Heart_Bit }).FirstOrDefault()).ToList();



                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult ConveyorStatus()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var plantName = db1.MM_Plants.Where(m => m.Plant_ID == plantID).Select(m => m.Plant_Name).FirstOrDefault();
                globalData.plantName = plantName;
                var lineObj = db1.MM_Lines
                                  .Where(a => a.Is_Conveyor == true)
                                  .Distinct();
                var shops = (from l in db1.MM_Lines join s in db1.MM_Shops on l.Shop_ID equals s.Shop_ID where l.Is_Conveyor == true select new DistinctShop { Shop_ID = l.Shop_ID, Shop_Name = s.Shop_Name, Link = s.Tracking_Link }).Distinct();
                ViewBag.Shop = shops;
                var lines = (from l in db1.MM_Lines where l.Is_Conveyor == true select new { Id = l.Line_ID, Line_Name = l.Line_Name, Shop_Name = l.Shop_ID, Line_Stop_Time = l.LineStop_Time });
                ViewBag.Line = lines;

                globalData.pageTitle = ResourceModules.Conveyor_Status;
                ViewBag.GlobalDataModel = globalData;
                return View(lineObj);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
    }
}