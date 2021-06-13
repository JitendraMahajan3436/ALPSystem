using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class OperationMasterController : BaseController
    {
        int plantId = 0, lineId = 0, lineTypeId = 0, shopId = 0;
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_AM_Operation_Master mmStationsObj = new RS_AM_Operation_Master();
        // GET: MM_Operation_Master
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Operation_Master;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "OperationMaster";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(db.RS_AM_Operation_Master.ToList());
        }

        // GET: MM_Operation_Master/Details/5
        public ActionResult Details(decimal id)
        {
            globalData.pageTitle = ResourceModules.Operation_Master;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "OperationMaster";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Operation_Master + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Operation_Master + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Operation_Master mM_Operation_Master = db.RS_AM_Operation_Master.Find(id);
            if (mM_Operation_Master == null)
            {
                return HttpNotFound();
            }
            return View(mM_Operation_Master);
        }

        // GET: MM_Operation_Master/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Operation_Master;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "OperationMaster";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name",plantId);
            return View();
        }

        // POST: MM_Operation_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Opt_ID,Plant_ID,Operation_Name,Is_Deleted,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_AM_Operation_Master mM_Operation_Master)
        {
            if (ModelState.IsValid)
            {
                mM_Operation_Master.Inserted_Date = DateTime.Now;
                mM_Operation_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_Operation_Master.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.RS_AM_Operation_Master.Add(mM_Operation_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Operation_Master;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "OperationMaster";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            return View(mM_Operation_Master);
        }

        // GET: MM_Operation_Master/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Operation_Master mM_Operation_Master = db.RS_AM_Operation_Master.Find(id);
            if (mM_Operation_Master == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Operation_Master;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "OperationMaster";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            return View(mM_Operation_Master);
        }

        // POST: MM_Operation_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Opt_ID,Plant_ID,Operation_Name,Is_Deleted,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_AM_Operation_Master mM_Operation_Master)
        {
            if (ModelState.IsValid)
            {
                mmStationsObj = db.RS_AM_Operation_Master.Find(mM_Operation_Master.Opt_ID);
                mmStationsObj.Plant_ID= ((FDSession)this.Session["FDSession"]).plantId;
                mmStationsObj.Is_Edited = true;
                mmStationsObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                mmStationsObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mmStationsObj.Updated_Date = DateTime.Now;
                mmStationsObj.Operation_Name = mM_Operation_Master.Operation_Name;
               
                db.Entry(mmStationsObj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = ResourceModules.Operation_Master;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "OperationMaster";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            return View(mM_Operation_Master);
        }

        // GET: MM_Operation_Master/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Operation_Master mM_Operation_Master = db.RS_AM_Operation_Master.Find(id);
            if (mM_Operation_Master == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Operation_Master;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "OperationMaster";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Operation_Master + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(mM_Operation_Master);
        }

        // POST: MM_Operation_Master/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_AM_Operation_Master mM_Operation_Master = db.RS_AM_Operation_Master.Find(id);
            db.RS_AM_Operation_Master.Remove(mM_Operation_Master);
            db.SaveChanges();
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
