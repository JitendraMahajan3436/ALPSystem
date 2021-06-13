using ZHB_AD.Helper;
using ZHB_AD.Helper.IoT;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.NewMaintenanceManagement
{
    public class PCMClitaController : Controller
    {
        // GET: PCMClita
        MTTUWEntities db = new MTTUWEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();

        // GET: CLITADailyCheck
        public ActionResult Index()
        {
            try
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
                ViewBag.ShopTitle = "Daily CLITA";
                List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Station_ID == stationID).ToList();
                if (stationMachineList.Count() > 0)
                {
                    ViewBag.MachineList = stationMachineList;
                    var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                    IEnumerable<MM_MTTUW_PCM_Clita> mMClitaItems = db.MM_MTTUW_PCM_Clita.Where(a => machineIDList.Contains(a.Machine_ID));

                    return View(mMClitaItems);
                }
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "No Machines Assigned to this station !";
                    globalData.messageDetail = "";
                    TempData["globalData"] = globalData;
                }
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "CLITADailyCheckController", "Index", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Operation Failed";
                globalData.messageDetail = "Exception - " + exp.Message;
                TempData["globalData"] = globalData;
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult logCLITAData(decimal clitaID, string value, Boolean status)
        {
            try
            {
                MetaShift objShift = new MetaShift();
                MM_MTTUW_PCM_Clita clitaObj = db.MM_MTTUW_PCM_Clita.Find(clitaID);
                int shiftID = objShift.getShiftID();
                //Added on 8-3-2018 for macine stopagges
                if (clitaObj != null)
                {
                    if (clitaObj.Is_Critical == true)
                    {

                    }
                }
                if (!db.MM_MTTUW_PCM_Clita_Log.Any(a => a.Clita_ID == clitaID && DbFunctions.TruncateTime(a.Inserted_Date) == DateTime.Today && a.Status == true && a.Shift_ID == shiftID))
                {
                    MM_MTTUW_PCM_Clita_Log clitaLog = new MM_MTTUW_PCM_Clita_Log();
                    clitaLog.Clita_ID = clitaID;
                    clitaLog.Input_Value = value;
                    clitaLog.Status = status;
                    clitaLog.Shift_ID = shiftID;
                    clitaLog.Inserted_Date = DateTime.Now;
                    clitaLog.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    clitaLog.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.MM_MTTUW_PCM_Clita_Log.Add(clitaLog);
                    db.SaveChanges();
                }
                decimal machineID = clitaObj.Machine_ID;
                List<decimal> clitaList = db.MM_MTTUW_PCM_Clita.Where(a => a.Machine_ID == machineID).Select(a => a.Clita_ID).ToList();
                int totalClitaforMachine = clitaList.Count();
                int totalClitaDoneToday = db.MM_MTTUW_PCM_Clita_Log.Where(a => DbFunctions.TruncateTime(a.Inserted_Date) == DateTime.Today && clitaList.Contains(a.Clita_ID)).Count();
                if (totalClitaDoneToday >= totalClitaforMachine)
                {
                    Kepware kepwareObj = new Kepware();
                    //kepwareObj.machineResume(machineID, "1");
                    //kepwareObj.machinePause(machineID, "0");
                }

                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "CLITADailyCheckController", "logCLITAData(clitaID : " + clitaID + ")", ((FDSession)this.Session["FDSession"]).userId);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowMachineDailyCLITA(decimal id)
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                ViewBag.GlobalDataModel = globalData;

                ViewBag.MachineList = null;
                List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Machine_ID == id).ToList();
                if (stationMachineList.Count() > 0)
                {
                    ViewBag.ShopTitle = stationMachineList.First().Machine_Name + " Daily CLITA";
                    ViewBag.MachineList = stationMachineList;
                    var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                    IEnumerable<MM_MTTUW_PCM_Clita> mMClitaItems = db.MM_MTTUW_PCM_Clita.Where(a => machineIDList.Contains(a.Machine_ID) && a.Is_Deleted != true);

                    return View(mMClitaItems);
                }
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "No Machines found with this ID !";
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
                generalHelper.addControllerException(exp, "CLITADailyCheckDashboardController", "ShowMachineDailyCLITA(machineId : " + id + ")", ((FDSession)this.Session["FDSession"]).userId);
                return View();
            }
        }
    }
}