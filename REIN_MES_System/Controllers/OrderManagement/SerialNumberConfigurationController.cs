using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class SerialNumberConfigurationController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        General generalObj = new General();
        int plantId = 0;
        // GET: SerialNumberConfiguration
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "SerialNumberConfiguration";
            ViewBag.GlobalDataModel = globalData;
            return View(db.RS_Serial_Number_Configuration.ToList());
        }

        // GET: SerialNumberConfiguration/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Serial_Number_Configuration RS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Find(id);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "SerialNumberConfiguration";

            ViewBag.GlobalDataModel = globalData;
            if (RS_Serial_Number_Configuration == null)
            {
                return HttpNotFound();
            }

            return View(RS_Serial_Number_Configuration);
        }

        // GET: SerialNumberConfiguration/Create
        public ActionResult Create()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            //ViewBag.Part_No = new SelectList(db.RS_Model_Master.Where(p => p.Plant_ID == plantID), "Model_Code", "Model_Code");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantID), "Shop_ID", "Shop_Name");
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(p => p.Shop_ID == 0), "Platform_ID", "Platform_Name");

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Create;

            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        // POST: SerialNumberConfiguration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Config_ID,Plant_ID,Shop_ID,Platform_ID,Display_Name,Serial_Logic,Series_Count,Month_Identifier,Year_Identifier,Is_Transfered,Is_Purgeable,Is_Edited,Running_Serial_Number,Plant_Code")] RS_Serial_Number_Configuration RS_Serial_Number_Configuration)
        {
            RS_Serial_Number_Configuration.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

            if (ModelState.IsValid)
            {
                RS_Serial_Number_Configuration.Inserted_Date = DateTime.Now;
                RS_Serial_Number_Configuration.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.RS_Serial_Number_Configuration.Add(RS_Serial_Number_Configuration);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageDetail = "Serial Number Configured Sucessfully";
                globalData.messageTitle = "Serial Number Configuration";
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Serial_Number_Configuration.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Serial_Number_Configuration.Shop_ID);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name", RS_Serial_Number_Configuration.Platform_ID);

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Create;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Serial_Number_Configuration);
        }

        // GET: SerialNumberConfiguration/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Serial_Number_Configuration RS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Find(id);
            if (RS_Serial_Number_Configuration == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Part_No = new SelectList(db.RS_Model_Master, "Model_Code", "Model_Code", RS_Serial_Number_Data.Part_No);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Serial_Number_Configuration.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Serial_Number_Configuration.Shop_ID);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name", RS_Serial_Number_Configuration.Platform_ID);

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "SerialNumberConfiguration";
            globalData.actionName = ResourceGlobal.Edit;

            ViewBag.GlobalDataModel = globalData;
            return View(RS_Serial_Number_Configuration);
        }

        // POST: SerialNumberConfiguration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Config_ID,Plant_ID,Shop_ID,Platform_ID,Display_Name,Serial_Logic,Series_Count,Month_Identifier,Year_Identifier,Is_Transfered,Is_Purgeable,Is_Edited,Running_Serial_Number,Plant_Code")] RS_Serial_Number_Configuration RS_Serial_Number_Configuration)
        {
            if (ModelState.IsValid)
            {
                RS_Serial_Number_Configuration.Is_Edited = true;
                RS_Serial_Number_Configuration.Updated_Date = DateTime.Now;
                RS_Serial_Number_Configuration.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.Entry(RS_Serial_Number_Configuration).State = EntityState.Modified;
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageDetail = "Serial Number Edited Sucessfully";
                globalData.messageTitle = "Serial Number Configuration";
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "SerialNumberData";
            globalData.actionName = ResourceGlobal.Edit;

            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Serial_Number_Configuration.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Serial_Number_Configuration.Shop_ID);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name", RS_Serial_Number_Configuration.Platform_ID);
            return View(RS_Serial_Number_Configuration);
        }

        // GET: SerialNumberConfiguration/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Serial_Number_Configuration RS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Find(id);
            if (RS_Serial_Number_Configuration == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = "Serial Number Configuration";
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "SerialNumberConfiguration";
            globalData.actionName = ResourceGlobal.Delete;

            ViewBag.GlobalDataModel = globalData;

            return View(RS_Serial_Number_Configuration);
        }

        // POST: SerialNumberConfiguration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            RS_Serial_Number_Configuration RS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Find(id);
            db.RS_Serial_Number_Configuration.Remove(RS_Serial_Number_Configuration);
            db.SaveChanges();
            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Serial_Number_Configuration", "Row_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
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


        public ActionResult GetPlatformID(int Shop_id)
        {

            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var platformDetail = (from platform in db.RS_OM_Platform
                                  join partgroup in db.RS_Partgroup on platform.Line_ID equals partgroup.Line_ID
                                  where platform.Shop_ID == Shop_id && partgroup.Order_Create == true && platform.Plant_ID == plantId
                                  select new
                                  {
                                      platform.Platform_Name,
                                      platform.Platform_ID
                                  }).Distinct().ToList();
            return Json(platformDetail, JsonRequestBehavior.AllowGet);
        }
    }
}
