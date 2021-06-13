using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;

namespace ZHB_AD.Controllers
{
    public class EmailTimeController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        MM_MT_Email_Time_Configuration emailConfig = new MM_MT_Email_Time_Configuration();

        General generalObj = new General();

        // GET: EmailTime
        public ActionResult Index()
        {
            globalData.pageTitle = "Email Time Config";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            globalData.controllerName = "EmailTime";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
           
            return View(db.MM_MT_Email_Time_Configuration.ToList());
        }

        // GET: EmailTime/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Email_Time_Configuration mM_MT_Email_Time_Configuration = db.MM_MT_Email_Time_Configuration.Find(id);
            if (mM_MT_Email_Time_Configuration == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = "Email Time Config";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Details;
            globalData.controllerName = "EmailTime";
            globalData.actionName = ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Email_Time_Configuration);
        }

        // GET: EmailTime/Create
        public ActionResult Create()
        {
            globalData.pageTitle = "Email Time Config";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Create;
            globalData.controllerName = "EmailTime";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.M_ID = new SelectList(db.MM_MT_Maintenance_Type, "M_ID", "Maintenance_Type");
            return View();
        }

        // POST: EmailTime/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ET_ID,M_ID,Day_Frequency,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Email_Time_Configuration mM_MT_Email_Time_Configuration)
        {
            if (ModelState.IsValid)
            {
                mM_MT_Email_Time_Configuration.Inserted_Date = DateTime.Now;
                mM_MT_Email_Time_Configuration.Inserted_Host = Request.UserHostName;
                mM_MT_Email_Time_Configuration.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.MM_MT_Email_Time_Configuration.Add(mM_MT_Email_Time_Configuration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = "Email Time Config";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Create;
            globalData.controllerName = "EmailTime";
            globalData.actionName = ResourceGlobal.Create;
            globalData.isSuccessMessage = true;
            globalData.messageDetail = "Email Time Configuration Added Sucessfully";
            globalData.messageTitle = "Email Time Config";
            ViewBag.GlobalDataModel = globalData;
            ViewBag.M_ID = new SelectList(db.MM_MT_Maintenance_Type, "M_ID", "Maintenance_Type",mM_MT_Email_Time_Configuration.M_ID);
            return View(mM_MT_Email_Time_Configuration);
        }

        // GET: EmailTime/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Email_Time_Configuration mM_MT_Email_Time_Configuration = db.MM_MT_Email_Time_Configuration.Find(id);
            if (mM_MT_Email_Time_Configuration == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = "Email Time Config";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Edit;
            globalData.controllerName = "EmailTime";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.M_ID = new SelectList(db.MM_MT_Maintenance_Type, "M_ID", "Maintenance_Type",mM_MT_Email_Time_Configuration.M_ID);
            return View(mM_MT_Email_Time_Configuration);
        }

        // POST: EmailTime/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ET_ID,M_ID,Day_Frequency,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Email_Time_Configuration mM_MT_Email_Time_Configuration)
        {
           
            if (ModelState.IsValid)
            {
                emailConfig = db.MM_MT_Email_Time_Configuration.Where(x => x.ET_ID == mM_MT_Email_Time_Configuration.ET_ID).FirstOrDefault();
                emailConfig.Day_Frequency = mM_MT_Email_Time_Configuration.Day_Frequency;
                emailConfig.M_ID = mM_MT_Email_Time_Configuration.M_ID;
                emailConfig.Updated_Date = DateTime.Now;
                emailConfig.Updated_Host = Request.UserHostName;
                emailConfig.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                emailConfig.Is_Edited = true;
                db.Entry(emailConfig).State = EntityState.Modified;
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageDetail = "Email Time Configuration Edited Sucessfully";
                globalData.messageTitle = "Email Time Config";
                return RedirectToAction("Index");
            }
            globalData.pageTitle = "Email Time Config";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Edit;
            globalData.controllerName = "EmailTime";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.M_ID = new SelectList(db.MM_MT_Maintenance_Type, "M_ID", "Maintenance_Type",mM_MT_Email_Time_Configuration.M_ID);
            return View(mM_MT_Email_Time_Configuration);
        }

        // GET: EmailTime/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Email_Time_Configuration mM_MT_Email_Time_Configuration = db.MM_MT_Email_Time_Configuration.Find(id);
            if (mM_MT_Email_Time_Configuration == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = "Email Time Config";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Delete;
            globalData.controllerName = "EmailTime";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Email_Time_Configuration);
        }

        // POST: EmailTime/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_MT_Email_Time_Configuration mM_MT_Email_Time_Configuration = db.MM_MT_Email_Time_Configuration.Find(id);
            db.MM_MT_Email_Time_Configuration.Remove(mM_MT_Email_Time_Configuration);
            db.SaveChanges();
            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_MT_Email_Time_Configuration", "ET_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
            
            globalData.pageTitle = "Email Time Config";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Delete;
            globalData.controllerName = "EmailTime";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            return RedirectToAction("Index");
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
