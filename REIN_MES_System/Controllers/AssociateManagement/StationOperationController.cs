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
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class StationOperationController : BaseController
    {
        int plantId = 0, lineId = 0, lineTypeId = 0, shopId = 0;
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_AM_Station_Operation mmStationsObj = new RS_AM_Station_Operation();
        public ActionResult getshopID(decimal plantId)
        {
            var shopObj = db.RS_Shops
              .Where(c => c.Plant_ID == plantId)
              .Select(c => new { c.Shop_ID, c.Shop_Name })
              .OrderBy(c => c.Shop_Name);
            return Json(shopObj, JsonRequestBehavior.AllowGet);
        }

        // GET: StationOperation

        public ActionResult getStationID(decimal lineId)
        {
            var stationObj = db.RS_Stations
              .Where(c => c.Line_ID == lineId)
              .Select(c => new { c.Station_ID, c.Station_Name })
              .OrderBy(c => c.Station_Name);
            return Json(stationObj, JsonRequestBehavior.AllowGet);
        }
        //get lines
        public ActionResult getLineID(decimal shopId)
        {
            var lineObj = db.RS_Lines   
              .Where(c => c.Shop_ID == shopId)
              .Select(c => new { c.Line_ID, c.Line_Name })
              .OrderBy(c => c.Line_Name);
            return Json(lineObj, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.Station_Operation;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "StationOperation";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            var RS_AM_Station_Operation = db.RS_AM_Station_Operation.Include(m => m.RS_AM_Operation_Master).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations);
            return View(RS_AM_Station_Operation.ToList());
        }

        // GET: StationOperation/Details/5
        public ActionResult Details(decimal id)
        {
            globalData.pageTitle = ResourceModules.Station_Operation;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "StationOperation";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Station_Operation + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Station_Operation + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Station_Operation RS_AM_Station_Operation = db.RS_AM_Station_Operation.Find(id);
            if (RS_AM_Station_Operation == null)
            {
                return HttpNotFound();
            }
            return View(RS_AM_Station_Operation);
        }

        // GET: StationOperation/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.Station_Operation;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StationOperation";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Opt_ID = new SelectList(db.RS_AM_Operation_Master, "Opt_ID", "Operation_Name");
           // ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
           
            return View();
        }

        // POST: StationOperation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Stn_Opt_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Opt_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_AM_Station_Operation RS_AM_Station_Operation)
        {
            if (ModelState.IsValid)
            {
                RS_AM_Station_Operation.Inserted_Date = DateTime.Now;
                RS_AM_Station_Operation.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                RS_AM_Station_Operation.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.RS_AM_Station_Operation.Add(RS_AM_Station_Operation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = ResourceModules.Station_Operation;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StationOperation";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Opt_ID = new SelectList(db.RS_AM_Operation_Master, "Opt_ID", "Operation_Name", RS_AM_Station_Operation.Opt_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Station_Operation.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Station_Operation.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Station_Operation.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_AM_Station_Operation.Station_ID);
            return View(RS_AM_Station_Operation);
        }

        // GET: StationOperation/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Station_Operation RS_AM_Station_Operation = db.RS_AM_Station_Operation.Find(id);
            if (RS_AM_Station_Operation == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Station_Operation;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "StationOperation";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Opt_ID = new SelectList(db.RS_AM_Operation_Master, "Opt_ID", "Operation_Name", RS_AM_Station_Operation.Opt_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Station_Operation.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Station_Operation.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Station_Operation.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_AM_Station_Operation.Station_ID);
            return View(RS_AM_Station_Operation);
        }

        // POST: StationOperation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Stn_Opt_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Opt_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_AM_Station_Operation RS_AM_Station_Operation)
        {
            if (ModelState.IsValid)
            {
                mmStationsObj = db.RS_AM_Station_Operation.Find(RS_AM_Station_Operation.Stn_Opt_ID);
                mmStationsObj.Plant_ID = RS_AM_Station_Operation.Plant_ID;
                mmStationsObj.Shop_ID = RS_AM_Station_Operation.Shop_ID;
                mmStationsObj.Line_ID = RS_AM_Station_Operation.Line_ID;
                mmStationsObj.Station_ID = RS_AM_Station_Operation.Station_ID;
                mmStationsObj.Is_Edited = true;
                mmStationsObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                mmStationsObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mmStationsObj.Updated_Date = DateTime.Now;
                mmStationsObj.Opt_ID = RS_AM_Station_Operation.Opt_ID;

                db.Entry(mmStationsObj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = ResourceModules.Station_Operation;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "StationOperation";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Opt_ID = new SelectList(db.RS_AM_Operation_Master, "Opt_ID", "Operation_Name", RS_AM_Station_Operation.Opt_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Station_Operation.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Station_Operation.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Station_Operation.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_AM_Station_Operation.Station_ID);
            return View(RS_AM_Station_Operation);
        }

        // GET: StationOperation/Delete/5
        public ActionResult Delete(decimal id)
        {
            globalData.pageTitle = ResourceModules.Station_Operation;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "StationOperation";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Station_Operation + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Station_Operation RS_AM_Station_Operation = db.RS_AM_Station_Operation.Find(id);
            if (RS_AM_Station_Operation == null)
            {
                return HttpNotFound();
            }
            return View(RS_AM_Station_Operation);
        }

        // POST: StationOperation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_AM_Station_Operation RS_AM_Station_Operation = db.RS_AM_Station_Operation.Find(id);
            db.RS_AM_Station_Operation.Remove(RS_AM_Station_Operation);
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
