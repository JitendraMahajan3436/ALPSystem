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
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;

namespace ZHB_AD.Controllers.AssociateManagement
{
    public class escalationConfigurationController : BaseController
    {
        private ZHB_ADEntities db = new ZHB_ADEntities();
        GlobalData globalData = new GlobalData();
        MM_Notification_Modules mm_notification = new MM_Notification_Modules();
        int plantId = 0, shopId = 0;
        #region Notification_Modules

        // GET: escalationConfiguration
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.escalationConfiguration;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.escalationConfiguration + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.escalationConfiguration + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View(db.MM_Notification_Modules.ToList());
        }

        // GET: escalationConfiguration/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Notification_Modules mM_Notification_Modules = db.MM_Notification_Modules.Find(id);
            if (mM_Notification_Modules == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.escalationConfiguration;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.escalationConfiguration + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.escalationConfiguration + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Notification_Modules);
        }

        // GET: escalationConfiguration/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.escalationConfiguration;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.escalationConfiguration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.escalationConfiguration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            MM_Notification_Modules mM_Notification_Modules = new MM_Notification_Modules();
            mM_Notification_Modules.Is_Shift = true;
            mM_Notification_Modules.Is_Email = true;
            return View(mM_Notification_Modules);
        }

        // POST: escalationConfiguration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_Notification_Modules mM_Notification_Modules)
        {
            if (ModelState.IsValid)
            {
                mM_Notification_Modules.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_Notification_Modules.Inserted_Date = DateTime.Now;
                mM_Notification_Modules.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                if (mM_Notification_Modules.Scheduler_Type == "on")
                {
                    mM_Notification_Modules.Scheduler_Type = "1";
                }
                else
                {
                    mM_Notification_Modules.Scheduler_Type = "0";
                }

                db.MM_Notification_Modules.Add(mM_Notification_Modules);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.escalationConfiguration;
                globalData.messageDetail = ResourceModules.escalationConfiguration + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }

            return View(mM_Notification_Modules);
        }

        // GET: escalationConfiguration/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Notification_Modules mM_Notification_Modules = db.MM_Notification_Modules.Find(id);
            if (mM_Notification_Modules == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.escalationConfiguration;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.escalationConfiguration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.escalationConfiguration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Notification_Modules);
        }

        // POST: escalationConfiguration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_Notification_Modules mM_Notification_Modules)
        {
            if (ModelState.IsValid)
            {
                //if (mM_Notification_Modules.isModelNameExists(mM_Notification_Modules.Module_Name))
                //{
                //    ModelState.AddModelError("Module_Name", ResourceValidation.Exist);
                //}
                mm_notification = db.MM_Notification_Modules.Find(mM_Notification_Modules.Module_ID);
                mm_notification.Module_Name = mM_Notification_Modules.Module_Name;
                mm_notification.Is_Email = mM_Notification_Modules.Is_Email;
                mm_notification.Is_intervel = mM_Notification_Modules.Is_intervel;
                mm_notification.intervel_Time = mM_Notification_Modules.intervel_Time;
                mm_notification.Is_Shift = mM_Notification_Modules.Is_Shift;
                mm_notification.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mm_notification.Updated_Date = DateTime.Now;
                mm_notification.Is_Edited = true;
                mm_notification.escalation_Time = mM_Notification_Modules.escalation_Time;
                if (mM_Notification_Modules.Scheduler_Type == "on")
                {
                    mm_notification.Scheduler_Type = "1";
                }
                else
                {
                    mm_notification.Scheduler_Type = "0";
                }
                db.Entry(mm_notification).State = EntityState.Modified;
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.escalationConfiguration;
                globalData.messageDetail = ResourceModules.escalationConfiguration + " " + ResourceMessages.Edit_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }

            globalData.pageTitle = ResourceModules.escalationConfiguration;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.escalationConfiguration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.escalationConfiguration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Notification_Modules);
        }

        // GET: escalationConfiguration/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Notification_Modules mM_Notification_Modules = db.MM_Notification_Modules.Find(id);
            if (mM_Notification_Modules == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.escalationConfiguration;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.escalationConfiguration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.escalationConfiguration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Notification_Modules);
        }

        // POST: escalationConfiguration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MM_Notification_Modules mM_Notification_Modules = db.MM_Notification_Modules.Find(id);
            db.MM_Notification_Modules.Remove(mM_Notification_Modules);
            db.SaveChanges();
            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.escalationConfiguration;
            globalData.messageDetail = ResourceModules.escalationConfiguration + " " + ResourceMessages.Delete_Success;
            TempData["globalData"] = globalData;
            return RedirectToAction("Index");
        }

        #endregion

        #region Notification_Employee_Configuration

        public ActionResult employee()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Module_ID = new SelectList(db.MM_Notification_Modules, "Module_ID", "Module_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Shift_ID = new SelectList(db.MM_Shift.Where(p => p.Plant_ID == plantId), "Shift_ID", "Shift_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(p => p.Shop_ID == shopId), "Line_ID", "Line_Name");
            globalData.pageTitle = ResourceModules.EmployeeConfiguration;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.EmployeeConfiguration + " " + ResourceGlobal.Create;
            globalData.contentFooter = ResourceModules.EmployeeConfiguration + " " + ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult employee(MM_Notification_Config mM_Notification_Config)
        {
            if (ModelState.IsValid)
            {
                mM_Notification_Config.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_Notification_Config.Inserted_Date = DateTime.Now;
                mM_Notification_Config.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.MM_Notification_Config.Add(mM_Notification_Config);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.EmployeeConfiguration;
                globalData.messageDetail = ResourceModules.EmployeeConfiguration + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("employeeList");
            }

            return View(mM_Notification_Config);
        }

        public ActionResult employeeList()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.EmployeeConfiguration;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.EmployeeConfiguration + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.EmployeeConfiguration + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View(db.MM_Notification_Config.ToList());
        }

        public ActionResult deleteEmployee(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Notification_Config mM_Notification_Config = db.MM_Notification_Config.Find(id);
            if (mM_Notification_Config == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.EmployeeConfiguration;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "escalationConfiguration";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.EmployeeConfiguration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.EmployeeConfiguration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(mM_Notification_Config);
        }

        [HttpPost, ActionName("deleteEmployee")]
        public ActionResult deleteEmployeeConfirmed(long? id)
        {
            MM_Notification_Config mM_Notification_Config = db.MM_Notification_Config.Find(id);
            db.MM_Notification_Config.Remove(mM_Notification_Config);
            db.SaveChanges();
            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.EmployeeConfiguration;
            globalData.messageDetail = ResourceModules.EmployeeConfiguration + " " + ResourceMessages.Delete_Success;
            TempData["globalData"] = globalData;
            return RedirectToAction("employeeList");
        }

        #endregion
















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
