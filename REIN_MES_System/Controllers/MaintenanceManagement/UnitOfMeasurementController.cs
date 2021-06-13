using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class UnitOfMeasurementController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        MM_MT_Unit_Of_Measurement cbm = new MM_MT_Unit_Of_Measurement();


        // GET: UnitOfMeasurement
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }


            globalData.pageTitle = ResourceModules.UOM_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "UnitOfMeasurement";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var MM_MT_Unit_Of_Measurement = db.MM_MT_Unit_Of_Measurement;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;

            return View(MM_MT_Unit_Of_Measurement.Where(m => m.Plant_ID == plantId).ToList());
        }

        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Unit_Of_Measurement MM_MT_Unit_Of_Measurement = db.MM_MT_Unit_Of_Measurement.Find(id);
            if (MM_MT_Unit_Of_Measurement == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.UOM_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "UnitOfMeasurement";
            globalData.actionName = ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            //TempData["globalData"] = globalData;

            return View(MM_MT_Unit_Of_Measurement);
        }

        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.UOM_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "UnitOfMeasurement";
            globalData.actionName = ResourceGlobal.Create;

            ViewBag.GlobalDataModel = globalData;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantId), "Plant_ID", "Plant_Name");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "M_ID,Measurement_Name,Plant_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Unit_Of_Measurement MM_MT_Unit_Of_Measurement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var plantId = ((FDSession)this.Session["FDSession"]).plantId;

                    MM_MT_Unit_Of_Measurement.Plant_ID = plantId;
                    MM_MT_Unit_Of_Measurement.Inserted_Date = DateTime.Now;
                    MM_MT_Unit_Of_Measurement.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    MM_MT_Unit_Of_Measurement.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.MM_MT_Unit_Of_Measurement.Add(MM_MT_Unit_Of_Measurement);
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.UOM_Config;
                    globalData.messageDetail = ResourceModules.UOM_Config + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.UOM_Config;
                globalData.messageDetail = ex.Message.ToString();
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
            }


            globalData.pageTitle = ResourceModules.UOM_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "UnitOfMeasurement";
            globalData.actionName = ResourceGlobal.Create;

            ViewBag.GlobalDataModel = globalData;


            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", MM_MT_Unit_Of_Measurement.Plant_ID);

            return View(MM_MT_Unit_Of_Measurement);
        }

        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Unit_Of_Measurement MM_MT_Unit_Of_Measurement = db.MM_MT_Unit_Of_Measurement.Find(id);
            if (MM_MT_Unit_Of_Measurement == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.UOM_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "UnitOfMeasurement";
            globalData.actionName = ResourceGlobal.Edit;

            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", MM_MT_Unit_Of_Measurement.Plant_ID);

            return View(MM_MT_Unit_Of_Measurement);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "M_ID,Plant_ID,Measurement_Name,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Unit_Of_Measurement MM_MT_Unit_Of_Measurement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cbm = db.MM_MT_Unit_Of_Measurement.Find(MM_MT_Unit_Of_Measurement.M_ID);
                    cbm.Measurement_Name = MM_MT_Unit_Of_Measurement.Measurement_Name;

                    cbm.Plant_ID = MM_MT_Unit_Of_Measurement.Plant_ID;
                    cbm.Updated_Date = DateTime.Now;
                    cbm.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    cbm.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    cbm.Is_Edited = true;
                    db.Entry(cbm).State = EntityState.Modified;
                    db.SaveChanges();


                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.UOM_Config;
                    globalData.messageDetail = ResourceModules.UOM_Config + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    ViewBag.GlobalDataModel = globalData;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exp)
            {

                generalHelper.addControllerException(exp, "UnitOfMeasurementController", "Edit(Post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.UOM_Config;
                globalData.messageDetail = exp.Message;
                this.Session["globalData"] = globalData;
            }

            globalData.pageTitle = ResourceModules.UOM_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "UnitOfMeasurement";
            globalData.actionName = ResourceGlobal.Edit;

            TempData["globalData"] = globalData;
            ViewBag.GlobalDataModel = globalData;


            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", MM_MT_Unit_Of_Measurement.Plant_ID);

            return View(MM_MT_Unit_Of_Measurement);
        }



        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Unit_Of_Measurement MM_MT_Unit_Of_Measurement = db.MM_MT_Unit_Of_Measurement.Find(id);
            if (MM_MT_Unit_Of_Measurement == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.UOM_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "UnitOfMeasurement";
            globalData.actionName = ResourceGlobal.Delete;

            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(MM_MT_Unit_Of_Measurement);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {

                MM_MT_Unit_Of_Measurement MM_MT_Unit_Of_Measurement = db.MM_MT_Unit_Of_Measurement.Find(id);
                db.MM_MT_Unit_Of_Measurement.Remove(MM_MT_Unit_Of_Measurement);
                db.SaveChanges();


                globalData.pageTitle = ResourceModules.UOM_Config;
                globalData.subTitle = ResourceGlobal.Delete;
                globalData.controllerName = "UnitOfMeasurement";
                globalData.actionName = ResourceGlobal.Delete;


                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.UOM_Config;
                globalData.messageDetail = ResourceModules.UOM_Config + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.UOM_Config;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                ViewBag.GlobalDataModel = globalData;
            }
            globalData.pageTitle = "Unit Of Measurement";
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "UnitOfMeasurement";
            globalData.actionName = ResourceGlobal.Delete;

            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
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