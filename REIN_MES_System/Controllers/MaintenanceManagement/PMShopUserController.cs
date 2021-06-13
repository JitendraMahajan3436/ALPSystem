using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using ZHB_AD.App_LocalResources;
namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class PMShopUserController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        MM_MT_Preventive_Maintenance_Log_New logObj = new MM_MT_Preventive_Maintenance_Log_New();
        // GET: PMShopUser
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.PMShopUser;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "PMShopUser";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var shopID = ((FDSession)this.Session["FDSession"]).shopId;
            // var MM_MT_Preventive_Maintenance_Log_New = db.MM_MT_Preventive_Maintenance_Log_New.Include(m => m.MM_Lines).Include(m => m.MM_MT_Machines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations);
            return View();
        }

        public ActionResult getdata(DateTime scheduleddate)
        {
            var shopID = ((FDSession)this.Session["FDSession"]).shopId;
            var machine = db.MM_MT_Preventive_Maintenance_Log_New.Where(a => a.Shop_ID == shopID && a.Scheduled_Date.Day == scheduleddate.Day && a.Scheduled_Date.Month == scheduleddate.Month && a.Scheduled_Date.Year == scheduleddate.Year && a.Is_Acknowledge!=true);
            return PartialView(machine);
        }


        // GET: PMShopUser/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Preventive_Maintenance_Log_New MM_MT_Preventive_Maintenance_Log_New = db.MM_MT_Preventive_Maintenance_Log_New.Find(id);
            if (MM_MT_Preventive_Maintenance_Log_New == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.PMShopUser;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PMShopUser";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", MM_MT_Preventive_Maintenance_Log_New.Line_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Number", MM_MT_Preventive_Maintenance_Log_New.Machine_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", MM_MT_Preventive_Maintenance_Log_New.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", MM_MT_Preventive_Maintenance_Log_New.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", MM_MT_Preventive_Maintenance_Log_New.Station_ID);
            return View(MM_MT_Preventive_Maintenance_Log_New);
        }

        // POST: PMShopUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PM_Log_ID,Plant_ID,Station_ID,Shop_ID,Line_ID,Machine_ID,Is_Acknowledge,Scheduled_Date,Last_Done_Date,Confirmed_Date,Acknowledge_User_ID,Acknowledge_Date,Confirmed_User_ID,Postpone_Date,Postpone_Reason,Postpone_User_ID,Is_Confirmed,Is_Transferred,Is_Purgeable,Is_Edited,Is_Deleted,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Preventive_Maintenance_Log_New MM_MT_Preventive_Maintenance_Log_New)
        {
            if (ModelState.IsValid)
            {
                logObj = db.MM_MT_Preventive_Maintenance_Log_New.Find(MM_MT_Preventive_Maintenance_Log_New.PM_Log_ID);
                logObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                logObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                logObj.Updated_Date = DateTime.Now;
                logObj.Is_Acknowledge = MM_MT_Preventive_Maintenance_Log_New.Is_Acknowledge;
                logObj.Confirmed_Date = MM_MT_Preventive_Maintenance_Log_New.Confirmed_Date;
                logObj.Acknowledge_User_ID = MM_MT_Preventive_Maintenance_Log_New.Acknowledge_User_ID;
                db.Entry(logObj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = ResourceModules.PMShopUser;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PMShopUser";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", MM_MT_Preventive_Maintenance_Log_New.Line_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Number", MM_MT_Preventive_Maintenance_Log_New.Machine_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", MM_MT_Preventive_Maintenance_Log_New.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", MM_MT_Preventive_Maintenance_Log_New.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", MM_MT_Preventive_Maintenance_Log_New.Station_ID);
            return View(MM_MT_Preventive_Maintenance_Log_New);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
