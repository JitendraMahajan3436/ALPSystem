using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Models;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class ChildPartSubAssemblyController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalObj = new General();
        // GET: RS_Major_Sub_Assembly
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
            globalData.controllerName = "Child Part Sub Assembly";
            globalData.actionName = ResourceGlobal.Lists;
            
            ViewBag.GlobalDataModel = globalData;

            return View(db.RS_Major_Sub_Assembly.ToList());
        }

        // GET: RS_Major_Sub_Assembly/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Major_Sub_Assembly RS_Major_Sub_Assembly = db.RS_Major_Sub_Assembly.Find(id);
            if (RS_Major_Sub_Assembly == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
            globalData.controllerName = "Child Part Sub Assembly";
            globalData.actionName = ResourceGlobal.Lists;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Major_Sub_Assembly);
        }

        // GET: RS_Major_Sub_Assembly/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
            globalData.controllerName = "Child Part Sub Assembly";
            globalData.actionName = ResourceGlobal.Lists;

            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        // POST: RS_Major_Sub_Assembly/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sub_Assembly_ID,Sub_Assembly_Name,Is_Transferred,Is_Purgeable,Is_Edited,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Major_Sub_Assembly RS_Major_Sub_Assembly)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isvalid = true;
                    if (RS_Major_Sub_Assembly.IsSubAssemblyExists(RS_Major_Sub_Assembly.Sub_Assembly_Name,RS_Major_Sub_Assembly.Sub_Assembly_ID))
                    {
                        ModelState.AddModelError("Sub_Assembly_Name", ResourceValidation.Exist);
                        isvalid = false;
                    }

                    if(isvalid == true)
                    {
                        RS_Major_Sub_Assembly.Inserted_Date = DateTime.Now;
                        RS_Major_Sub_Assembly.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        RS_Major_Sub_Assembly.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.RS_Major_Sub_Assembly.Add(RS_Major_Sub_Assembly);
                        db.SaveChanges();
                        globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
                        globalData.controllerName = "Child Part Sub Assembly";
                        globalData.actionName = ResourceGlobal.Create;

                        ViewBag.GlobalDataModel = globalData;
                        return RedirectToAction("Index");
                    }
                }
                catch(Exception ex)
                {
                    globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
                    globalData.controllerName = "Child Part Sub Assembly";
                    globalData.actionName = ResourceGlobal.Create;

                    ViewBag.GlobalDataModel = globalData;
                }
            }

            return View(RS_Major_Sub_Assembly);
        }

        // GET: RS_Major_Sub_Assembly/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Major_Sub_Assembly RS_Major_Sub_Assembly = db.RS_Major_Sub_Assembly.Find(id);
            if (RS_Major_Sub_Assembly == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
            globalData.controllerName = "Child Part Sub Assembly";
            globalData.actionName = ResourceGlobal.Lists;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Major_Sub_Assembly);
        }

        // POST: RS_Major_Sub_Assembly/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sub_Assembly_ID,Sub_Assembly_Name,Is_Transferred,Is_Purgeable,Is_Edited,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Major_Sub_Assembly RS_Major_Sub_Assembly)
        {
            if (ModelState.IsValid)
            {
                bool isValid = true;
                try
                {
                    if(RS_Major_Sub_Assembly.IsSubAssemblyExists(RS_Major_Sub_Assembly.Sub_Assembly_Name,RS_Major_Sub_Assembly.Sub_Assembly_ID))
                    {
                        ModelState.AddModelError("Sub_Assembly_Name", ResourceValidation.Exist);
                        isValid = false;
                    }

                    if(isValid == true)
                    {
                        db.Entry(RS_Major_Sub_Assembly).State = EntityState.Modified;
                        db.SaveChanges();
                        globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
                        globalData.controllerName = "Child Part Sub Assembly";
                        globalData.actionName = ResourceGlobal.Lists;

                        ViewBag.GlobalDataModel = globalData;
                        return RedirectToAction("Index");
                    }
                    
                }
                catch(Exception ex)
                {
                    globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
                    globalData.controllerName = "Child Part Sub Assembly";
                    globalData.actionName = ResourceGlobal.Lists;

                    ViewBag.GlobalDataModel = globalData;
                }
            }
            return View(RS_Major_Sub_Assembly);
        }

        // GET: RS_Major_Sub_Assembly/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Major_Sub_Assembly RS_Major_Sub_Assembly = db.RS_Major_Sub_Assembly.Find(id);
            if (RS_Major_Sub_Assembly == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
            globalData.controllerName = "Child Part Sub Assembly";
            globalData.actionName = ResourceGlobal.Lists;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Major_Sub_Assembly);
        }

        // POST: RS_Major_Sub_Assembly/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_Major_Sub_Assembly RS_Major_Sub_Assembly = db.RS_Major_Sub_Assembly.Find(id);
                db.RS_Major_Sub_Assembly.Remove(RS_Major_Sub_Assembly);
                db.SaveChanges();
                globalData.pageTitle = ResourceModules.Major_Sub_Assembly + " " + ResourceGlobal.Config;
                globalData.controllerName = "Child Part Sub Assembly";
                globalData.actionName = ResourceGlobal.Lists;

                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                generalObj.addControllerException(ex, "ChildPartSubAssembly", "DeleteConfirmed", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Setup";
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", id);
            }
           
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
