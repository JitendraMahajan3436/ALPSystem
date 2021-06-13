using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.NewMaintenanceManagement
{
    public class MinorStoppagesController : BaseController
    {
        REIN_SOLUTIONEntities db1 = new REIN_SOLUTIONEntities();
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();

        // GET: MinorStoppages
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            ViewBag.GlobalDataModel = globalData;

            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            decimal stationID = ((FDSession)this.Session["FDSession"]).stationId;
            //stationID = 4;
            ViewBag.MachineList = null;
            ViewBag.ShopTitle = "Machine Minor Stoppages Cycle";
            List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Station_ID == stationID).ToList();
            if (stationMachineList.Count() > 0)
            {
                ViewBag.MachineList = stationMachineList;
                var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                IEnumerable<MM_MT_MinorStoppageCycle> minorStoppageCycleObjList = db.MM_MT_MinorStoppageCycle.Where(a => machineIDList.Contains(a.Machine_ID)).OrderBy(a => a.CycleStep_Number);

                return View(minorStoppageCycleObjList);
            }
            else
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "No Machines Assigned to this station !";
                globalData.messageDetail = "";
                TempData["globalData"] = globalData;
            }

            return View();
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult checkForCycleSignals(decimal machineId)
        {
            try
            {
                
                var machineCycleSignals = db.MM_MT_MinorStoppageCycle.Where(a => a.Machine_ID == machineId)
                                   .Select(a => new { a.MS_CycleStep_ID, a.Current_Status }).ToList();
                return Json(machineCycleSignals, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppagesController", "checkForCycleSignals(machineId : " + machineId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getMachineIOList(int alarmID)
        {
            try
            {
                IEnumerable<MM_MT_Machine_IO_Alarm_Relation> mmMachineIOObj = db.MM_MT_Machine_IO_Alarm_Relation.Where(a => a.Alarm_ID == alarmID);
                return PartialView("PVCycleStepIOList", mmMachineIOObj);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppagesController", "getMachineIOList(Alarm ID : " + alarmID + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }
        public ActionResult ShowMachineMinorStoppage(decimal id)

        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var plantName = db.MM_MTTUW_Plants.Where(m => m.Plant_ID == plantID).Select(m => m.Plant_Name).FirstOrDefault();
                var machineName = db.MM_MT_MTTUW_Machines.Where(m => m.Machine_ID == id).Select(m => m.Machine_Name).FirstOrDefault();
                globalData.plantName = plantName;
                globalData.pageTitle = "Minor Stoppages" + " - " + machineName;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                ViewBag.GlobalDataModel = globalData;

                ViewBag.MachineList = null;
                List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Machine_ID == id).ToList();
                if (stationMachineList.Count() > 0)
                {
                    ViewBag.ShopTitle = stationMachineList.First().Machine_Name + " Minor Stoppages Cycle";
                    ViewBag.MachineList = stationMachineList;
                    var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                    IEnumerable<MM_MT_MinorStoppageCycle> minorStoppageCycleObjList = db.MM_MT_MinorStoppageCycle.Where(a => machineIDList.Contains(a.Machine_ID)).OrderBy(a => a.CycleStep_Number);

                    return View(minorStoppageCycleObjList);
                }
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "No Machines Assigned to this station !";
                    globalData.messageDetail = "";
                    TempData["globalData"] = globalData;
                }
                return View();
            }
            catch (Exception exp)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Exception Occurred in Server!";
                globalData.messageDetail = exp.Message;
                TempData["globalData"] = globalData;
                generalHelper.addControllerException(exp, "MinorStoppagesController", "ShowMachineMinorStoppage(machineId : " + id + ")", ((FDSession)this.Session["FDSession"]).userId);
                return View();
            }
        }


        public ActionResult getMinorStoppagesData(int machineId)
        {
            try
            {
                DateTime FromDate = DateTime.Now.AddDays(-30);
                DateTime ToDate = DateTime.Now;
                var start = Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy");
                var end = Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy"); ;
                ViewBag.fromDate = start;
                ViewBag.toDate = end;
                List<MinorStoppagesData> Minordata = new List<MinorStoppagesData>();
                //var obj = db.MM_TBM_Reset_History.Where(m => m.TBM_ID == tbmid).FirstOrDefault();
                var Parameter_Data = db.MM_MT_MinorStoppageCycle.Where(m => m.Machine_ID == machineId).ToList();

                

                foreach (var item in Parameter_Data)
                {
                    var data = db.MM_Ctrl_MinorStoppage_Cycle_Signals.Where(m => m.MS_CycleStep_ID == item.MS_CycleStep_ID && m.Inserted_Date > FromDate && m.Inserted_Date <= ToDate  && m.Is_Faulty == true).ToList();
                    foreach (var item1 in data)
                    {
                        MinorStoppagesData export = new MinorStoppagesData(item1.Inserted_Date, Convert.ToBoolean(item1.Is_Faulty), machineId);
                        Minordata.Add(export);
                    }
                }

                //var faultminorStoppage = Minordata.GroupBy(c => c.datetime).Select(c => new { Is_Faulty = c.Count(b => b.Is_Faulty), insertDate = c.Key, }).ToList();

                var MinorStoppages = Minordata.GroupBy(c => c.Machine).Select(c => new { Is_Faulty = c.Count(b => b.Is_Faulty), Machine = c.Key, }).FirstOrDefault();
                //var MinorStoppagesData = Parameter_Data.GroupBy(c=>c.Inserted_Date).Select(c=> new { TotalCount = c.Sum(b=>b.) })
                //var consumptionwise1 = consumptionwise3.GroupBy(c => c.ShopsCat_ID).Select(c => new { totalconsumption = c.Sum(b => b.totalconsumption), ShopsCat_ID = c.Key, }).ToList();
                //MM_MT_Preventive_Equipment eqpObj = db.MM_MT_Preventive_Equipment.Find(MachineID);
                //ViewBag.equipmentObj = eqpObj;
                ViewBag.faultminorStoppageDetail = MinorStoppages;
                var MachineName = (from m in db.MM_MT_MTTUW_Machines
                                   where m.Machine_ID == machineId
                                   select new
                                   {
                                       m.Machine_Name,

                                   }).FirstOrDefault();
                ViewBag.MachineName = MachineName.Machine_Name;

                ViewBag.Quanity = MinorStoppages != null ? MinorStoppages.Is_Faulty : 0;
                ViewBag.Machine = machineId;
                return PartialView("MinorStoppagestDetails");
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppagesController", "getMinorStoppagesData(MachineID : " + machineId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }

        public ActionResult getMachineCycle(string MachineID )
        {
            try
            {
                int Machine = Convert.ToInt32(MachineID);
                DateTime FromDate = DateTime.Now.AddDays(-30);
                DateTime ToDate = DateTime.Now;
                var start = Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy");
                var end = Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy"); ;
                ViewBag.fromDate = start;
                ViewBag.toDate = end;
                ViewData["cycleStepList"] = null;
                List<CycleStoppageData> Minordata = new List<CycleStoppageData>();
                var Parameter_Data = db.MM_MT_MinorStoppageCycle.Where(m => m.Machine_ID == Machine).ToList();
                foreach (var item in Parameter_Data)
                {
                    var data = db.MM_Ctrl_MinorStoppage_Cycle_Signals.Where(m => m.MS_CycleStep_ID == item.MS_CycleStep_ID && m.Inserted_Date > FromDate && m.Inserted_Date <= ToDate && m.Is_Faulty == true).ToList();
                    //Minordata.GroupBy(c => c.Machine).Select(c => new { Is_Faulty = c.Count(b => b.Is_Faulty), Machine = c.Key, }).FirstOrDefault();
                    var CycleCount = data.GroupBy(c => c.MS_CycleStep_ID).ToList();
                    var id = data.Count();
                    CycleStoppageData export = new CycleStoppageData(item.CycleStep_Name, id , Convert.ToInt32(item.MS_CycleStep_ID),Machine);
                    Minordata.Add(export);
                   
                }
                ViewData["cycleStepList"] = Minordata;
                var MachineName = (from m in db.MM_MT_MTTUW_Machines
                                   where m.Machine_ID == Machine
                                   select new
                                   {
                                       m.Machine_Name,

                                   }).FirstOrDefault();
                ViewBag.MachineName = MachineName.Machine_Name;

               
                ViewBag.Machine = Machine;
                return PartialView("MinorStoppagestCycleDetails",Minordata);

            }
            catch
            {
                return null;
            }
            
        }


        public ActionResult getminorstoppageHistory(string machine, string cycle)
        {
            try
            {
                int Machine = Convert.ToInt32(machine);
                int Cycle = Convert.ToInt32(cycle);
                DateTime FromDate = DateTime.Now.AddDays(-30);
                DateTime ToDate = DateTime.Now;
                var start = Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy");
                var end = Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy"); ;
                ViewBag.fromDate = start;
                ViewBag.toDate = end;
                ViewData["cycleStepList"] = null;
                List<MinorStoppagesHistorData> Minordata = new List<MinorStoppagesHistorData>();

                var cycleData = (from M in db.MM_MT_MinorStoppageCycle
                                 join
                                 mn in db.MM_MT_MTTUW_Machines
                                 on M.Machine_ID equals mn.Machine_ID
                                 join
                                 c in db.MM_Ctrl_MinorStoppage_Cycle_Signals
                                 on
                                 M.MS_CycleStep_ID equals c.MS_CycleStep_ID
                                 join
                                 a in db.MM_Ctrl_Machine_Alarm_Data                                   
                                 on
                                 c.Inserted_Date equals a.Inserted_Date
                                 join
                                 an in db.MM_Ctrl_Machine_Alarms_Master
                                 on
                                 a.Alarm_ID equals an.Alarm_ID
                                 where
                                 c.MS_CycleStep_ID == Cycle && c.Inserted_Date > FromDate && c.Inserted_Date <= ToDate && c.Is_Faulty == true
                                 && a.Machine_ID == Machine
                                 select new
                                 {
                                     mn.Machine_Name,
                                     M.CycleStep_Name,
                                     c.Inserted_Date,
                                     an.Alarm_Name

                                 }).ToList();

                foreach(var item in cycleData)
                {
                    MinorStoppagesHistorData obj = new MinorStoppagesHistorData(item.Machine_Name, item.CycleStep_Name, item.Inserted_Date, item.Alarm_Name);
                    Minordata.Add(obj);
                }

                return PartialView("MinorStoppagesDetailsHistory", Minordata);
            }
            catch
            {
                return null;
            }
        }

        public MM_Shift getCurrentRunningShiftByShopID(decimal shopId)
        {
            try
            {
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                MM_Shift mmShiftObj = (from shiftObj in db1.MM_Shift
                                       where shiftObj.Shop_ID == shopId
                                && TimeSpan.Compare(shiftObj.Shift_Start_Time, currDate) < 0
                                && TimeSpan.Compare(shiftObj.Shift_End_Time, currDate) > 0
                                       select shiftObj).Single();
                return mmShiftObj;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "MinorStoppages", "getCurrentRunningShiftByShopID() " + shopId, 1);
                return null;
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getMachineFaultyAlarms(int machineId)
        {
            try
            {
                var shifts = db1.MM_Machine_Shift_Data.Where(m => m.Machine_ID == machineId).Select(m => m.Shift_ID).ToList();
                var isPM = Convert.ToBoolean(db1.MM_Machine_Shift_Data.Where(m => m.Machine_ID == machineId).Select(m => m.Is_PM_Activity).FirstOrDefault());
                var shopID = db1.MM_MT_Machines.Where(m => m.Machine_ID == machineId).Select(m => m.Shop_ID).FirstOrDefault();
                var currentshift = getCurrentRunningShiftByShopID(shopID);

                List<MM_Ctrl_Machine_Alarms_Master> machineAlarmStatusListTemp = new List<MM_Ctrl_Machine_Alarms_Master>();
                List<MTTUFaultAlarmFields> faultyAlarmIDList = new List<MTTUFaultAlarmFields>();
                DateTime startDate = DateTime.Today;
                DateTime EndDAte = DateTime.Today;
                var starttime = TimeSpan.Parse("00:00:00.000");
                var endTime = TimeSpan.Parse("00:00:00.000");

                foreach (var shift in shifts)
                {
                    if(currentshift != null && shift == currentshift.Shift_ID && !isPM)
                    {
                        string query = "SELECT t1.Machine_ID, t1.Alarm_ID, t1.Status ,t1.Inserted_Date" +
                             " FROM   [MM_Ctrl_Machine_Alarm_Data] t1 " +
                             " INNER JOIN " +
                             "(" +
                             "    SELECT Max(Inserted_Date) date2, Alarm_ID" +
                             "    FROM   [MM_Ctrl_Machine_Alarm_Data] " +
                             "    GROUP BY Alarm_ID " +
                             ") AS t2 " +
                             "    ON t1.Alarm_ID = t2.Alarm_ID" +
                             "    AND t1.Inserted_Date = t2.date2" +
                             "	AND Machine_ID = @p0" +
                             " ORDER BY Alarm_ID";

                        faultyAlarmIDList = db.Database.SqlQuery<MTTUFaultAlarmFields>(query, machineId).Where(a => a.Status == true).ToList();
                        List<decimal> alarmIDList = faultyAlarmIDList.Select(b => b.Alarm_ID).ToList();
                        machineAlarmStatusListTemp = db.MM_Ctrl_Machine_Alarms_Master.Where(a => alarmIDList.Contains(a.Alarm_ID)).ToList();
                        startDate = (startDate + currentshift.Shift_Start_Time);
                        EndDAte = (EndDAte + currentshift.Shift_End_Time);
                    }
                }

                var machineAlarmStatusList = (from a in machineAlarmStatusListTemp
                                              join b in faultyAlarmIDList on a.Alarm_ID equals b.Alarm_ID
                                              where b.Inserted_Date >= startDate && b.Inserted_Date <= EndDAte
                                              select new { a.Alarm_ID, a.Alarm_Name, alarmDate = b.Inserted_Date.ToString(), b.Inserted_Date }).OrderByDescending(a => a.Inserted_Date);

                return Json(new { MachineList = machineAlarmStatusList,IsPM = isPM}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppagesController", "getMachineFaultyAlarms(machineId : " + machineId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }
    }
}