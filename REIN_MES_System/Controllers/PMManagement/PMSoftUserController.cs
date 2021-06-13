using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.App_LocalResources;
using System.Data.Entity.Validation;

namespace REIN_MES_System.Controllers.PMManagement
{
    public class PMSoftUserController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        int partId = 0;



        RS_MT_Preventive_Maintenance_Log_New pmlogObj = new RS_MT_Preventive_Maintenance_Log_New();
        // GET: PMSoftUser
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.PMSoftUser;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "PMSoftUser";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var UserId = ((FDSession)this.Session["FDSession"]).userId;
            var res = db.RS_PM_Activity_Log.Where(a => a.Is_Confirmed != true && a.RS_PM_Activity.Activity_Owner_ID == UserId && a.Plant_ID==PlantID).ToList();
            return View(res);
        }

        public ActionResult ShopOpenActivity()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            decimal stationId = ((FDSession)this.Session["FDSession"]).stationId;
            globalData.pageTitle = ResourceModules.PMSoftUser;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "PMSoftUser";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var UserId = ((FDSession)this.Session["FDSession"]).userId;
            var res = db.RS_PM_Activity_Log.Where(a => a.Is_Confirmed != true && a.RS_PM_Activity.Station_ID == stationId && a.Plant_ID == PlantID).ToList();
            return View(res);
        }


        //public class partList
        //{
        //    public decimal Part_ID { get; set; }
        //    public string Part_Name { get; set; }

        //}
        /* Action Name                : Edit
    *  Description                : Show the edit line form
    *  Author, Timestamp          : Jitendra Mahajan
    *  Input parameter            : id (line id)
    *  Return Type                : ActionResult
    *  Revision                   : 1.0
    */
        // GET: /Line/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PM_Activity_Log mm_Log = db.RS_PM_Activity_Log.Find(id);
            if (mm_Log == null)
            {
                return HttpNotFound();
            }
            var res = (from m in db.RS_Maintenance_Part
                       join n in db.RS_PM_Activity_Part on m.Maintenance_Part_ID equals n.Maintenance_Part_ID
                       where n.Activity_ID == mm_Log.Activity_ID
                       select new partList { Part_ID = m.Maintenance_Part_ID, Part_Name = m.Part_Name }).ToList();

            mm_Log.partsData = res;
            ViewBag.assigned_Part = res;
            globalData.pageTitle = ResourceModules.Open_Activity;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PMSoftUser";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Open_Activity + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Activity_ID = new SelectList(db.RS_PM_Activity.Where(a=>a.Activity_ID==mm_Log.Activity_ID), "Activity_ID", "Activity_NAme", mm_Log.Activity_ID);
            ViewBag.Plant_ID = mm_Log.RS_Plants.Plant_Name;
            ViewBag.Shop_ID = mm_Log.RS_Shops.Shop_Name;
            ViewBag.Line_ID = mm_Log.RS_Lines.Line_Name;
            ViewBag.Station_ID =mm_Log.RS_Stations.Station_Name;
            ViewBag.Machine_ID = mm_Log.RS_MT_Machines.Machine_Name;
            ViewBag.Activity_Owner_ID = mm_Log.RS_PM_Activity.RS_Employee.Employee_Name;
            ViewBag.dueDate =mm_Log.Due_Date.ToString("yyyy-MM-dd");
            ViewBag.EQP_ID =mm_Log.RS_PM_Activity.RS_PM_Equipment.Equipment_Name;
            return View(mm_Log);
        }
        public ActionResult SaveRecord(string values)
        {
            //foreach(string i in values)
            //{
            //    ViewBag.ii = i.ID;
            //}

            return RedirectToAction("Index");

        }
        // POST: /Line/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        /* Action Name                : Edit
        *  Description                : Action is used to edit the line
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Lines
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(RS_PM_Activity_Log mm_Log)
        {
           
            RS_PM_Activity_Part_Log Obj = new RS_PM_Activity_Part_Log();
          
            RS_PM_Activity_Log ActivityLog = db.RS_PM_Activity_Log.Find(mm_Log.PM_Activity_Log_ID);
            if (mm_Log == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                try {
                    if (mm_Log.partsData != null)
                    {
                        foreach (var item in mm_Log.partsData)
                        {
                         //   string[] words = item.Part_ID;

                            Obj.Maintenance_Part_ID = item.Part_ID;//Convert.ToDecimal(words[0]);
                            Obj.Qty = item.part_Quantity;

                            Obj.Plant_ID = ActivityLog.Plant_ID;
                            Obj.Shop_ID = ActivityLog.Shop_ID;
                            Obj.Line_ID = ActivityLog.Line_ID;
                            Obj.Station_ID = ActivityLog.Station_ID;
                            Obj.Machine_ID = ActivityLog.Machine_ID;
                            Obj.PM_Activity_Log_ID = mm_Log.PM_Activity_Log_ID;
                            Obj.Inserted_Date = DateTime.Now;
                            Obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            Obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.RS_PM_Activity_Part_Log.Add(Obj);
                            db.SaveChanges();
                        }
                    }

                    //if (mm_Log.months != null)
                    //{
                    //    foreach (string item in mm_Log.months)
                    //    {
                    //        string[] words = item.Split('_');

                    //        Obj.Maintenance_Part_ID = Convert.ToDecimal(words[0]);
                    //        //Obj.Qty = Convert.ToInt32(words[1]);

                    //        Obj.Plant_ID = ActivityLog.Plant_ID;
                    //        Obj.Shop_ID = ActivityLog.Shop_ID;
                    //        Obj.Line_ID = ActivityLog.Line_ID;
                    //        Obj.Station_ID = ActivityLog.Station_ID;
                    //        Obj.Machine_ID = ActivityLog.Machine_ID;
                    //        Obj.PM_Activity_Log_ID = mm_Log.PM_Activity_Log_ID;
                    //        Obj.Inserted_Date = DateTime.Now;
                    //        Obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    //        Obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //        db.RS_PM_Activity_Part_Log.Add(Obj);
                    //        db.SaveChanges();
                    //    }
                    //}
                    ActivityLog.Observation_Before_PM = mm_Log.Observation_Before_PM;
                    ActivityLog.Observation_After_PM = mm_Log.Observation_After_PM;
                    ActivityLog.Is_Confirmed = true;
                    ActivityLog.Confirmed_Date = DateTime.Now;
                    ActivityLog.Is_Edited = true;
                    db.Entry(ActivityLog).State = EntityState.Modified;
                    db.SaveChanges();
                    RS_PM_Activity objActivity = db.RS_PM_Activity.Find(ActivityLog.Activity_ID);
                    objActivity.Last_Date = DateTime.Today;
                    decimal?[] prices = {0};
                    objActivity.Maintenance_Part_ID = prices;
                    objActivity.Is_Edited = true;
                    objActivity.Updated_Date = DateTime.Now;
                    objActivity.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    objActivity.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.Entry(objActivity).State = EntityState.Modified;
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Open_Activity;
                    globalData.messageDetail = ResourceMessages.close_Ticket;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    var ress = (from m in db.RS_Maintenance_Part
                               join n in db.RS_PM_Activity_Part on m.Maintenance_Part_ID equals n.Maintenance_Part_ID
                               where n.Activity_ID == mm_Log.Activity_ID
                               select new partList { Part_ID = m.Maintenance_Part_ID, Part_Name = m.Part_Name }).ToList();


                    ViewBag.assigned_Part = ress;
                    globalData.pageTitle = ResourceModules.Open_Activity;
                    globalData.subTitle = ResourceGlobal.Edit;
                    globalData.controllerName = "PMSoftUser";
                    globalData.actionName = ResourceGlobal.Edit;
                    globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Open_Activity + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
                    ViewBag.GlobalDataModel = globalData;
                    ViewBag.GlobalDataModel = globalData;
                    ViewBag.Activity_ID = new SelectList((db.RS_PM_Activity), "Activity_ID", "Activity_Name", mm_Log.RS_PM_Activity);
                    ViewBag.Plant_ID = new SelectList((db.RS_Plants), "Plant_ID", "Plant_Name", mm_Log.Plant_ID);
                    ViewBag.Shop_ID = new SelectList((db.RS_Shops), "Shop_ID", "Shop_Name", mm_Log.Shop_ID);
                    ViewBag.Line_ID = new SelectList((db.RS_Lines), "Line_ID", "Line_Name", mm_Log.Line_ID);
                    ViewBag.Station_ID = new SelectList((db.RS_Stations), "Station_ID", "Station_Name", mm_Log.Station_ID);
                    ViewBag.Machine_ID = new SelectList((db.RS_MT_Machines), "Machine_ID", "Machine_Name", mm_Log.Machine_ID);
                    ViewBag.Activity_Owner_ID = mm_Log.RS_PM_Activity.RS_Employee.Employee_Name;
                    ViewBag.EQP_ID = new SelectList((db.RS_PM_Equipment), "EQP_ID", "Equipment_Name", mm_Log.RS_PM_Activity.EQP_ID);
                    ViewBag.dueDate = mm_Log.Due_Date.ToString("yyyy-MM-dd");
                    return View(mm_Log);
                }
            }
            var res = (from m in db.RS_Maintenance_Part
                       join n in db.RS_PM_Activity_Part on m.Maintenance_Part_ID equals n.Maintenance_Part_ID
                       where n.Activity_ID == mm_Log.Activity_ID
                       select new partList { Part_ID = m.Maintenance_Part_ID, Part_Name = m.Part_Name }).ToList();


            ViewBag.assigned_Part = res;
            globalData.pageTitle = ResourceModules.Open_Activity;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PMSoftUser";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Open_Activity + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Line + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Activity_ID = new SelectList((db.RS_PM_Activity), "Activity_ID", "Activity_Name", mm_Log.RS_PM_Activity);
            ViewBag.Plant_ID = new SelectList((db.RS_Plants), "Plant_ID", "Plant_Name", mm_Log.Plant_ID);
            ViewBag.Shop_ID = new SelectList((db.RS_Shops), "Shop_ID", "Shop_Name", mm_Log.Shop_ID);
            ViewBag.Line_ID = new SelectList((db.RS_Lines), "Line_ID", "Line_Name", mm_Log.Line_ID);
            ViewBag.Station_ID = new SelectList((db.RS_Stations), "Station_ID", "Station_Name", mm_Log.Station_ID);
            ViewBag.Machine_ID = new SelectList((db.RS_MT_Machines), "Machine_ID", "Machine_Name", mm_Log.Machine_ID);
            ViewBag.Activity_Owner_ID = mm_Log.RS_PM_Activity.RS_Employee.Employee_Name;
            ViewBag.EQP_ID = new SelectList((db.RS_PM_Equipment), "EQP_ID", "Equipment_Name", mm_Log.RS_PM_Activity.EQP_ID);
            ViewBag.dueDate = mm_Log.Due_Date.ToString("yyyy-MM-dd");
            return View(mm_Log);
        }

        public ActionResult getdata(DateTime scheduleddate, int shopID)
        {
            //var machines = db.RS_MT_Preventive_Maintenance_Log_New.Where(a => a.Shop_ID == shopID && DbFunctions.TruncateTime(a.Scheduled_Date) == DbFunctions.TruncateTime(scheduleddate));

            var activity = db.RS_PM_Activity_Log.Where(a => a.Shop_ID == shopID && DbFunctions.TruncateTime(a.Due_Date) == DbFunctions.TruncateTime(scheduleddate) && a.Is_Confirmed != true);

            return PartialView(activity);
        }

        public ActionResult ClosedPM(string id)
        {
            RS_PM_Activity_Log RS_PM_Activity_Log = db.RS_PM_Activity_Log.Find(Convert.ToInt32(id));
            RS_PM_Activity_Log.Confirmed_Date = DateTime.Now;
            RS_PM_Activity_Log.Is_Confirmed = true;
            RS_PM_Activity_Log.Confirmed_User_ID = Convert.ToDecimal(((FDSession)this.Session["FDSession"]).userId.ToString());
            db.SaveChanges();
            string result = "PM Close Successfully";
            return Json(result);
        }

        // GET: PMSoftUser/Edit/5



    }
}
