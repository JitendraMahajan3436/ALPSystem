using ZHB_AD.App_LocalResources;
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
    public class MinorStoppageCycleController : BaseController
    {
        //private REIN_SOLUTIONEntities db1 = new REIN_SOLUTIONEntities();
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        int plantId = 0, shopId = 0, stationId = 0, lineId = 0;
        String stationName = "";
        // GET: MinorStoppageCycle
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceDisplayName.Minor_Stoppage_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "MinorStoppageCycle";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.MM_MTTUW_Plants.Find(plantID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            var shops = (from shop in db.MM_MTTUW_Shops
                         join cr in db.MM_MT_MTTUW_Machines
                         on shop.Shop_ID equals cr.Shop_ID
                         select shop).Distinct();
            //ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(a => a.Plant_ID == zb_plantObj.Plant_ID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name");
            ViewBag.Shop_ID = new SelectList(shops, "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines.Where(p => p.Shop_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(m => m.Station_ID == 0), "Machine_ID", "Machine_Name");

            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]


        public ActionResult getLineNameList(decimal shopId)
        {
            try
            {
                var LineNameList = (from line in db.MM_MTTUW_Lines
                                    where line.Shop_ID == shopId /*&& machine.IsActive == true*/
                                    select new
                                    {
                                        Id = line.Line_ID,
                                        Value = line.Line_Name
                                    }).Distinct();
                //var machineList = db.MM_MT_MTTUW_Machines.Where(a => a.Shop_ID == shopId && a.IsActive == true)
                //                 .Select(a => new { a.Machine_ID, a.Machine_Name }).Distinct();
                return Json(LineNameList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppageCycleController", "getLineNameList(shopID : " + shopId + " )", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }

        public ActionResult getMachineList(decimal lineId)
        {
            try
            {
                var machineList = (from machine in db.MM_MT_MTTUW_Machines
                                   where machine.Line_ID == lineId /*&& machine.IsActive == true*/
                                   select new
                                   {
                                       Id = machine.Machine_ID,
                                       Value = machine.Machine_Name
                                   }).Distinct();
                //var machineList = db.MM_MT_MTTUW_Machines.Where(a => a.Shop_ID == shopId && a.IsActive == true)
                //                 .Select(a => new { a.Machine_ID, a.Machine_Name }).Distinct();
                return Json(machineList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppageCycleController", "getMachineList(lineId : " + lineId + " )", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }



        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getCycleList(decimal machineId)
        {
            try
            {
                var minorStoppagesList = db.MM_MT_MinorStoppageCycle.Where(a => a.Machine_ID == machineId)
                                           .Select(a => new { a.MS_CycleStep_ID, a.CycleStep_Name, a.CycleStep_Number, a.Machine_ID, a.Data_Retention_Period })
                                           .OrderBy(a => a.CycleStep_Number);
                return Json(minorStoppagesList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppageCycleController", "getCycleList(machineId : " + machineId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }

        [HttpPost]
        public JsonResult addCycle(decimal machineId, string cycleName, int? DataRetationPeriod)
        {
            try
            {
                MM_MT_MinorStoppageCycle minorStoppageObj = new MM_MT_MinorStoppageCycle();
                minorStoppageObj.CycleStep_Name = cycleName.Trim().ToUpper();
                List<MM_MT_MinorStoppageCycle> minorStoppageList = db.MM_MT_MinorStoppageCycle.Where(a => a.Machine_ID == machineId).ToList();
                if (minorStoppageList.Count > 0)
                {
                    minorStoppageObj.CycleStep_Number = minorStoppageList.Max(a => a.CycleStep_Number) + 1;
                }
                else
                {
                    minorStoppageObj.CycleStep_Number = 1;
                }
                minorStoppageObj.Current_Status = "Idle";
                minorStoppageObj.Machine_ID = machineId;
                minorStoppageObj.Data_Retention_Period = DataRetationPeriod;
                // minorStoppageObj.Line_ID = Lineid;
                minorStoppageObj.Inserted_Date = DateTime.Now;
                minorStoppageObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                minorStoppageObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.MM_MT_MinorStoppageCycle.Add(minorStoppageObj);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppageCycleController", "addCycle(Machine Id : " + machineId + ",Cycle Name = " + cycleName + " )", ((FDSession)this.Session["FDSession"]).userId);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult editCycle(Int64 cycleId, String cycleName)
        {
            try
            {
                MM_MT_MinorStoppageCycle minorStoppagecycleObj = db.MM_MT_MinorStoppageCycle.Find(cycleId);
                minorStoppagecycleObj.CycleStep_Name = cycleName;
                minorStoppagecycleObj.Is_Edited = true;
                db.Entry(minorStoppagecycleObj).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = true, message = "Successfully Edited Cycle Step Name" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "MinorStoppageCycleController", "editCycle(cycleId : " + cycleId + ",cycleName : " + cycleName + ")", ((FDSession)this.Session["FDSession"]).userId);
                return Json(new { status = false, message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult removeCycle(Int64 cycleId)
        {
            try
            {
                MM_MT_MinorStoppageCycle minorStoppageObj = db.MM_MT_MinorStoppageCycle.Find(cycleId);
                if (minorStoppageObj != null)
                {
                    db.MM_MT_MinorStoppageCycle.Remove(minorStoppageObj);
                    db.SaveChanges();
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "MinorStoppageCycleController", "removeCycle(Cycle Id : " + cycleId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult saveNewCycleList(List<Int64> cycleList, int DataRetationPeriod)
        {
            try
            {
                int cycleCntr = 0;
                foreach (Int64 cycleID in cycleList)
                {
                    cycleCntr++;
                    MM_MT_MinorStoppageCycle minorStoppageObj = db.MM_MT_MinorStoppageCycle.Find(cycleID);
                    if (minorStoppageObj != null)
                    {
                        minorStoppageObj.CycleStep_Number = cycleCntr;
                        minorStoppageObj.Data_Retention_Period = DataRetationPeriod;
                        minorStoppageObj.Is_Edited = true;
                        db.Entry(minorStoppageObj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return Json(new { status = true, message = "Cycle Steps Saved Successfully.." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "MinorStoppageCycleController", "saveNewCycleList()", ((FDSession)this.Session["FDSession"]).userId);
                return Json(new { status = false, message = exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
