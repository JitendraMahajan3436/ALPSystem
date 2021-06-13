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
    public class PMSoftUserController : BaseController
    {
        private ZHB_ADEntities db = new ZHB_ADEntities();
        GlobalData globalData = new GlobalData();
        MM_MT_Preventive_Maintenance_Log_New pmlogObj = new MM_MT_Preventive_Maintenance_Log_New();
        // GET: PMSoftUser
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourcePreventiveMaintenanceNew.PMSoftUser;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "PMSoftUser";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var shopID = ((FDSession)this.Session["FDSession"]).shopId;
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name");

            //var MM_MT_Preventive_Maintenance_Log_New = db.MM_MT_Preventive_Maintenance_Log_New.Include(m => m.MM_Lines).Include(m => m.MM_MT_Machines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations);
            return View();
        }
        public ActionResult getdata(DateTime scheduleddate, int shopID)
        {
            //var shopID = ((FDSession)this.Session["FDSession"]).shopId;
            var machine = db.MM_MT_Preventive_Maintenance_Log_New.Where(a => a.Shop_ID == shopID && a.Scheduled_Date == scheduleddate);

            return PartialView(machine);
        }


        // GET: PMSoftUser/Edit/5
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
            globalData.pageTitle = ResourcePreventiveMaintenanceNew.PMSoftUser;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PMSoftUser";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", MM_MT_Preventive_Maintenance_Log_New.Line_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines, "Machine_ID", "Machine_Number", MM_MT_Preventive_Maintenance_Log_New.Machine_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", MM_MT_Preventive_Maintenance_Log_New.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", MM_MT_Preventive_Maintenance_Log_New.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", MM_MT_Preventive_Maintenance_Log_New.Station_ID);
            return View(MM_MT_Preventive_Maintenance_Log_New);
        }

        // POST: PMSoftUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PM_Log_ID,Plant_ID,Station_ID,Shop_ID,Line_ID,Machine_ID,Is_Acknowledge,Scheduled_Date,Last_Done_Date,Confirmed_Date,Acknowledge_User_ID,Acknowledge_Date,Confirmed_User_ID,Postpone_Date,Postpone_Reason,Postpone_User_ID,Is_Confirmed,Is_Transferred,Is_Purgeable,Is_Edited,Is_Deleted,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Is_Postponed")] MM_MT_Preventive_Maintenance_Log_New MM_MT_Preventive_Maintenance_Log_New)
        {
            if (ModelState.IsValid)
            {
                pmlogObj = db.MM_MT_Preventive_Maintenance_Log_New.Find(MM_MT_Preventive_Maintenance_Log_New.PM_Log_ID);
                pmlogObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                pmlogObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                pmlogObj.Updated_Date = DateTime.Now;
                pmlogObj.Confirmed_Date = MM_MT_Preventive_Maintenance_Log_New.Confirmed_Date;
                pmlogObj.Acknowledge_User_ID = MM_MT_Preventive_Maintenance_Log_New.Acknowledge_User_ID;
                pmlogObj.Postpone_Date = MM_MT_Preventive_Maintenance_Log_New.Postpone_Date;
                pmlogObj.Postpone_Reason = MM_MT_Preventive_Maintenance_Log_New.Postpone_Reason;
                db.Entry(pmlogObj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = ResourcePreventiveMaintenanceNew.PMSoftUser;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PMSoftUser";
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
