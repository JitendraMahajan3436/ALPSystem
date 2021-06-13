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
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.PlantConfiguration
{
    /* Controller Name            : StationController
    *  Description                : This controller is used to manage the station
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class StationController : BaseController
    {
        //private REIN_SOLUTION_MEntities MTdb = new REIN_SOLUTION_MEntities();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        General generalObj = new General();
        int plantId = 0, shopId = 0, stationId = 0, lineId = 0;
        String stationName = "";

        RS_Stations mmStationsObj = new RS_Stations();

        /* Action Name                : Index
        *  Description                : Action used to show the list station
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Station/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Station_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Station";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Station + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Station + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            //var RS_Stations = db.RS_Stations.Include(m => m.RS_Shops).Include(m => m.RS_Lines).Include(m => m.RS_Employee).Include(m => m.RS_Employee1);
            var RS_Stations = (from st in db.RS_Stations
                               join so in db.RS_Shops on st.Shop_ID equals so.Shop_ID
                               join li in db.RS_Lines on so.Shop_ID equals li.Shop_ID
                               where so.Plant_ID == plantId
                               select st).Distinct();
            return View(RS_Stations.ToList());
        }

        /* Action Name                : Details
        *  Description                : Action used to show the detail of station
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (station id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Station/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Stations RS_Stations = db.RS_Stations.Find(id);
            if (RS_Stations == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Station_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Station";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Station + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Station + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Stations);
        }

        /* Action Name                : Create
        *  Description                : Action used to show add station form
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Station/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Station_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Station";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Station + " " + ResourceGlobal.Form; ViewBag.GlobalDataModel = globalData;

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_user_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Station_Type_ID = new SelectList(db.RS_Station_Type, "Station_Type_ID", "Station_Type_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == shopId), "Line_ID", "Line_Name");
            return View();
        }

        /* Action Name                : Create
        *  Description                : Action used to add station
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Stations
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Station/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Stations RS_Stations)
        {
            if (ModelState.IsValid)
            {
                stationName = RS_Stations.Station_Name;
                plantId = Convert.ToInt16(RS_Stations.Plant_ID);
                shopId = Convert.ToInt16(RS_Stations.Shop_ID);
                lineId = Convert.ToInt16(RS_Stations.Line_ID);

                if (RS_Stations.isStationExists(stationName, plantId, shopId, lineId, 0))
                {
                    ModelState.AddModelError("Station_Name", ResourceValidation.Exist);
                }
                else
                {
                    RS_Stations.Inserted_Date = DateTime.Now;
                    RS_Stations.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Stations.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    db.RS_Stations.Add(RS_Stations);
                    db.SaveChanges();
                    //Add Station Into Station Tracking 
                    RS_Station_Tracking RS_Station_Tracking = new RS_Station_Tracking();
                    RS_Station_Tracking.Plant_ID = plantId;
                    RS_Station_Tracking.Shop_ID = shopId;
                    RS_Station_Tracking.Line_ID = lineId;
                    RS_Station_Tracking.Station_ID = RS_Stations.Station_ID;
                    RS_Station_Tracking.Inserted_Date = DateTime.Now;
                    db.RS_Station_Tracking.Add(RS_Station_Tracking);
                    db.SaveChanges();


                    ////Save records fro station in MTTUW db
                    //MTstation.Station_ID = RS_Stations.Station_ID;
                    //MTstation.Buffer_Size = RS_Stations.Buffer_Size;
                    //MTstation.Conveyor_Time = RS_Stations.Conveyor_Time;
                    //MTstation.Inserted_Date = RS_Stations.Inserted_Date;
                    //MTstation.Inserted_Host = RS_Stations.Inserted_Host;
                    //MTstation.Inserted_User_ID = RS_Stations.Inserted_User_ID;
                    //MTstation.Is_Buffer = RS_Stations.Is_Buffer;
                    //MTstation.Is_Conveyor = RS_Stations.Is_Conveyor;
                    //MTstation.Is_Critical_Station = RS_Stations.Is_Critical_Station;
                    //MTstation.Is_Edited = RS_Stations.Is_Edited;
                    //MTstation.Is_Purgeable = RS_Stations.Is_Purgeable;
                    //MTstation.Is_Transferred = RS_Stations.Is_Transferred;
                    //MTstation.Line_ID = RS_Stations.Line_ID;
                    //MTstation.Shop_ID = RS_Stations.Shop_ID;
                    //MTstation.Station_Description = RS_Stations.Station_Description;
                    //MTstation.Station_Host_Name = RS_Stations.Station_Host_Name;
                    //MTstation.Station_IP_Address = RS_Stations.Station_IP_Address;
                    //MTstation.Station_Name = RS_Stations.Station_Name;
                    //MTdb.MM_MTTUW_Stations.Add(MTstation);
                    //MTdb.SaveChanges();
                    ////end

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Station;
                    globalData.messageDetail = ResourceModules.Station + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Station_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Station";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = RS_Stations.Plant_ID;

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Stations.Shop_ID);
            ViewBag.Inserted_user_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Stations.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Stations.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Stations.Plant_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Stations.Shop_ID), "Line_ID", "Line_Name", RS_Stations.Line_ID);
            ViewBag.Station_Type_ID = new SelectList(db.RS_Station_Type, "Station_Type_ID", "Station_Type_Name",RS_Stations.Station_Type_ID);
            return View(RS_Stations);
        }

        /* Action Name                : Edit
        *  Description                : Action used to show the edit station form
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (station id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Station/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            RS_Stations RS_Stations = db.RS_Stations.Find(id);
            if (RS_Stations == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Station_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Station";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            RS_Shops shopObj = db.RS_Shops.Where(p => p.Shop_ID == RS_Stations.Shop_ID).SingleOrDefault();

            //RS_Stations stationObj = db.RS_Stations.Where(p => p.Shop_ID == RS_Stations.Shop_ID && p.Shop_ID==RS_Stations.Shop_ID).SingleOrDefault();
            plantId = Convert.ToInt16(shopObj.Plant_ID);
            RS_Stations.Plant_ID = plantId;

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Stations.Shop_ID);
            ViewBag.Inserted_user_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Stations.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Stations.Updated_User_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Stations.Plant_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Stations.Shop_ID), "Line_ID", "Line_Name", RS_Stations.Line_ID);
            ViewBag.Station_Type_ID = new SelectList(db.RS_Station_Type, "Station_Type_ID", "Station_Type_Name",RS_Stations.Station_Type_ID);
            return View(RS_Stations);
        }

        /* Action Name                : Edit
        *  Description                : Action used to update station
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : RS_Stations
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Station/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Station_ID,Station_Name,Station_Description,IsVin,IsEngine,Is_Error_Proofing,Tprint,Linemode,Reworkmode,Is_Buffer,Shop_ID,Station_IP_Address,Station_Host_Name,Is_Critical_Station,Is_Transferred,Is_Purgeable,Inserted_user_ID,Inserted_Date,Updated_User_ID,Updated_Date,Plant_ID,Line_ID,Sort_Order,Station_Type_ID")] RS_Stations RS_Stations)
        {
            if (ModelState.IsValid)
            {
                
                stationName = RS_Stations.Station_Name;
                plantId = Convert.ToInt16(RS_Stations.Plant_ID);
                shopId = Convert.ToInt16(RS_Stations.Shop_ID);
                stationId = Convert.ToInt16(RS_Stations.Station_ID);
                lineId = Convert.ToInt16(RS_Stations.Line_ID);
               
                //validation for line change if station is configured in route configuration under differnet line
                var isConfigured = db.RS_Route_Configurations.Where(m => (m.Line_ID != lineId || m.Plant_ID != plantId || m.Shop_ID != shopId) && m.Station_ID == stationId).FirstOrDefault();
                if (isConfigured != null)
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Station;
                    globalData.messageDetail = "Can not change shop or line, Because this Station is configured in route configuration";
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Edit", stationId);
                }

                ////

                if (RS_Stations.isStationExists(stationName, plantId, shopId, lineId, stationId))
                {
                    ModelState.AddModelError("Station_Name", ResourceValidation.Exist);
                }
                else
                {
                    mmStationsObj = db.RS_Stations.Find(RS_Stations.Station_ID);
                    mmStationsObj.Station_Name = RS_Stations.Station_Name;
                    mmStationsObj.Station_Description = RS_Stations.Station_Description;
                    mmStationsObj.Is_Buffer = RS_Stations.Is_Buffer;
                    mmStationsObj.Plant_ID = RS_Stations.Plant_ID;
                    mmStationsObj.Shop_ID = RS_Stations.Shop_ID;
                    mmStationsObj.Line_ID = RS_Stations.Line_ID;
                    mmStationsObj.Station_IP_Address = RS_Stations.Station_IP_Address;
                    mmStationsObj.Station_Host_Name = RS_Stations.Station_Host_Name;
                    mmStationsObj.IsVin = RS_Stations.IsVin;
                    mmStationsObj.IsEngine = RS_Stations.IsEngine;
                    mmStationsObj.Is_Error_Proofing = RS_Stations.Is_Error_Proofing;
                    mmStationsObj.Tprint = RS_Stations.Tprint;
                    mmStationsObj.Linemode = RS_Stations.Linemode;
                    mmStationsObj.Reworkmode = RS_Stations.Reworkmode;
                    mmStationsObj.Sort_Order = RS_Stations.Sort_Order;
                    mmStationsObj.Station_Type_ID = RS_Stations.Station_Type_ID;

                    mmStationsObj.Updated_Date = DateTime.Now;
                    mmStationsObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmStationsObj.Is_Edited = true;
                    db.Entry(mmStationsObj).State = EntityState.Modified;

                    //db.Entry(RS_Stations).State = EntityState.Modified;
                    db.SaveChanges();

                    ////Save records fro station in MTTUW db
                    //MTstation = MTdb.MM_MTTUW_Stations.Find(mmStationsObj.Station_ID);
                    //MTstation.Buffer_Size = mmStationsObj.Buffer_Size;
                    //MTstation.Conveyor_Time = mmStationsObj.Conveyor_Time;
                    //MTstation.Inserted_Date = mmStationsObj.Inserted_Date;
                    //MTstation.Inserted_Host = mmStationsObj.Inserted_Host;
                    //MTstation.Inserted_User_ID = mmStationsObj.Inserted_User_ID;
                    //MTstation.Is_Buffer = mmStationsObj.Is_Buffer;
                    //MTstation.Is_Conveyor = mmStationsObj.Is_Conveyor;
                    //MTstation.Is_Critical_Station = mmStationsObj.Is_Critical_Station;
                    //MTstation.Is_Edited = mmStationsObj.Is_Edited;
                    //MTstation.Is_Purgeable = mmStationsObj.Is_Purgeable;
                    //MTstation.Is_Transferred = mmStationsObj.Is_Transferred;
                    //MTstation.Line_ID = mmStationsObj.Line_ID;
                    //MTstation.Shop_ID = mmStationsObj.Shop_ID;
                    //MTstation.Station_Description = mmStationsObj.Station_Description;
                    //MTstation.Station_Host_Name = mmStationsObj.Station_Host_Name;
                    //MTstation.Station_IP_Address = mmStationsObj.Station_IP_Address;
                    //MTstation.Station_Name = mmStationsObj.Station_Name;
                    //MTdb.Entry(MTstation).State = EntityState.Modified;
                    //MTdb.SaveChanges();
                    ////end

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Station;
                    globalData.messageDetail = ResourceModules.Station + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Station_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Station";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            plantId = RS_Stations.Plant_ID;

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_Stations.Shop_ID);
            ViewBag.Inserted_user_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Stations.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Stations.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Stations.Plant_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Stations.Shop_ID), "Line_ID", "Line_Name", RS_Stations.Line_ID);
            ViewBag.Station_Type_ID = new SelectList(db.RS_Station_Type, "Station_Type_ID", "Station_Type_Name",RS_Stations.Station_Type_ID);
            return View(RS_Stations);
        }

        /* Action Name                : Delete
        *  Description                : Action used to show the delete station user confirnmation form
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (station id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Station/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Stations RS_Stations = db.RS_Stations.Find(id);
            if (RS_Stations == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Station_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Station";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Station + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Stations);
        }

        /* Action Name                : DeleteConfirmed
        *  Description                : Action used to delete station
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : id (station id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Station/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Stations RS_Stations = db.RS_Stations.Find(id);
            try
            {
                db.RS_Stations.Remove(RS_Stations);
                db.SaveChanges();
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Stations", "Station_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Station;
                globalData.messageDetail = ResourceModules.Station + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Station;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", id);

            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Station;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", id);
            }


        }

        /* Action Name                : Dispose
        *  Description                : Action used to dispose the station controller object
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : disposing
        *  Return Type                : void
        *  Revision                   : 1.0
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /* Action Name                : GetStationByShopIDForRoute
        *  Description                : Action used to return the list of station added under shop which is not configured on line
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : shopId (shop id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetStationByShopIDForRoute(int shopId)
        {
            try
            {
                var st = from station in db.RS_Stations
                         where station.Shop_ID == shopId
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                st = from station in db.RS_Stations
                     where station.Shop_ID == shopId && !(from routeConfig in db.RS_Route_Configurations select routeConfig.Station_ID).Contains(station.Station_ID)
                     select new
                     {
                         Id = station.Station_ID,
                         Value = station.Station_Name,
                     };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetStationByLineID
        *  Description                : Action used to return the list of station added under line in route configuration
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : lineId (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetStationByLineID(int lineId)
        {
            try
            {
                var st = from station in db.RS_Stations
                         where (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetCriticalStationByLineID
        *  Description                : Action used to get the critical station by line
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : lineId (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetCriticalStationByLineID(int lineId)
        {
            try
            {

                var st = from station in db.RS_Stations
                         where (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                         && station.Is_Critical_Station == true
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetNoCriticalStationByLineID
        *  Description                : Action used to show the non critical station by line
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : lineId (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetNoCriticalStationByLineID(int lineId)
        {
            try
            {

                var st = from station in db.RS_Stations
                         where (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                         && station.Is_Critical_Station == false
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetStationListByLineID
        *  Description                : Action used to get the list of station which is not added in route configuration
        *  Author, Timestamp          : Jitendra Mahajan :: 23 Oct 2015
        *  Input parameter            : lineId (line id)
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        public ActionResult GetStationListByLineID(int lineId)
        {
            try
            {

                var st = from station in db.RS_Stations
                         where station.Line_ID == lineId && !(from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetCriticalStationByShopID
       *  Description                : Action used to get the critical station by line
       *  Author, Timestamp          : Jitendra Mahajan
       *  Input parameter            : shopId (shop id)
       *  Return Type                : ActionResult
       *  Revision                   : 1.0
       */
        public ActionResult GetCriticalStationByShopID(int shopId)
        {
            try
            {

                var st = from station in db.RS_Stations
                         where (from routeConfig in db.RS_Route_Configurations where routeConfig.Shop_ID == shopId select routeConfig.Station_ID).Contains(station.Station_ID)
                         && station.Is_Critical_Station == true
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }



        /* Action Name                : GetNoCriticalStationByShopID
       *  Description                : Action used to show the non critical station by shop
       *  Author, Timestamp          : Jitendra Mahajan
       *  Input parameter            : shopId (shop id)
       *  Return Type                : ActionResult
       *  Revision                   : 1.0
       */
        public ActionResult GetNoCriticalStationByShopID(int shopId)
        {
            try
            {

                var st = from station in db.RS_Stations
                         where (from routeConfig in db.RS_Route_Configurations where routeConfig.Shop_ID == shopId select routeConfig.Station_ID).Contains(station.Station_ID)
                         && station.Is_Critical_Station == false
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
