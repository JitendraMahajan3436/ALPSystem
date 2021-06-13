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

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class ParameterController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        MM_MT_Parameter Mparam = new MM_MT_Parameter();

        General generalObj = new General();

        // GET: Parameter
        public ActionResult Index()
        {
            globalData.pageTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            globalData.controllerName = App_LocalResources.ResourceModules.Parameter;
           
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.contentFooter = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;
            return View(db.MM_MT_Parameter.ToList());
        }

        // GET: Parameter/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Parameter mM_MT_Parameter = db.MM_MT_Parameter.Find(id);
            if (mM_MT_Parameter == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.subTitle = App_LocalResources.ResourceGlobal.Details;
            globalData.controllerName = App_LocalResources.ResourceModules.Parameter;

            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.contentFooter = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Parameter);
        }

        // GET: Parameter/Create
        public ActionResult Create()
        {
            globalData.pageTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.subTitle = App_LocalResources.ResourceGlobal.Create;
            globalData.controllerName = App_LocalResources.ResourceModules.Parameter;

            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.contentFooter = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        // POST: Parameter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Parameter_ID,Parameter_Name,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Parameter mM_MT_Parameter)
        {
            if (ModelState.IsValid)
            {
                mM_MT_Parameter.Inserted_Date = DateTime.Now;
                mM_MT_Parameter.Inserted_Host = Request.UserHostAddress;
                mM_MT_Parameter.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.MM_MT_Parameter.Add(mM_MT_Parameter);

                db.SaveChanges();
              
                return RedirectToAction("Index");
            }
            globalData.pageTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.subTitle = App_LocalResources.ResourceGlobal.Create;
            globalData.controllerName = App_LocalResources.ResourceModules.Parameter;

            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.contentFooter = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Parameter);
        }

        // GET: Parameter/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Parameter mM_MT_Parameter = db.MM_MT_Parameter.Find(id);
            if (mM_MT_Parameter == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.subTitle = App_LocalResources.ResourceGlobal.Edit;
            globalData.controllerName = App_LocalResources.ResourceModules.Parameter;

            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.contentFooter = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Parameter);
        }

        // POST: Parameter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Parameter_ID,Parameter_Name,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Parameter mM_MT_Parameter)
        {
            if (ModelState.IsValid)
            {
                Mparam = db.MM_MT_Parameter.Where(x => x.Parameter_ID == mM_MT_Parameter.Parameter_ID).FirstOrDefault();
                Mparam.Updated_Date = DateTime.Now;
                Mparam.Updated_Host = Request.UserHostAddress;
                Mparam.Parameter_Name = mM_MT_Parameter.Parameter_Name;
                Mparam.Is_Edited = true;
                db.Entry(Mparam).State = EntityState.Modified;
                
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            globalData.pageTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.subTitle = App_LocalResources.ResourceGlobal.Edit;
            globalData.controllerName = App_LocalResources.ResourceModules.Parameter;

            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.contentFooter = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Parameter);
        }

        // GET: Parameter/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Parameter mM_MT_Parameter = db.MM_MT_Parameter.Find(id);
            if (mM_MT_Parameter == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.subTitle = App_LocalResources.ResourceGlobal.Delete;
            globalData.controllerName = App_LocalResources.ResourceModules.Parameter;

            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.contentFooter = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_MT_Parameter);
        }

        // POST: Parameter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_MT_Parameter mM_MT_Parameter = db.MM_MT_Parameter.Find(id);
            db.MM_MT_Parameter.Remove(mM_MT_Parameter);
            db.SaveChanges();
            
            
            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_MT_Parameter", "[Parameter_ID]", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
            
            globalData.pageTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.subTitle = App_LocalResources.ResourceGlobal.Delete;
            globalData.controllerName = App_LocalResources.ResourceModules.Parameter;

            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
            globalData.contentFooter = App_LocalResources.ResourceModules.Parameter+" "+ResourceGlobal.Config;
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
