using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Helper;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.ManifestSopManagement
{
    
    public class KittingBarcodeMasterController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0;
        General generalObj = new General();
            // GET: /KittingBarcodeMaster/
        public ActionResult Index()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            return View(db.RS_Kitt_Barcode_Master.Where(m=>m.Plant_ID==plantId).ToList());
        }

        // GET: /KittingBarcodeMaster/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Kitt_Barcode_Master RS_Kitt_Barcode_Master = db.RS_Kitt_Barcode_Master.Find(id);
            if (RS_Kitt_Barcode_Master == null)
            {
                return HttpNotFound();
            }
            return View(RS_Kitt_Barcode_Master);
        }

        // GET: /KittingBarcodeMaster/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.Kitt_Barcode_Config;
            globalData.subTitle = ResourceGlobal.Create;
            //globalData.subTitle = "Create";
            globalData.controllerName = "KittingBarcodeMaster";
            globalData.actionName = ResourceGlobal.Create;
            //globalData.contentTitle = ResourcePartGroup.PartGroup_Title_Add_PartGroup;
            //globalData.contentFooter = ResourcePartGroup.PartGroup_Title_Add_PartGroup;

            ViewBag.GlobalDataModel = globalData;
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m=>m.Plant_ID== plantId), "Plant_ID", "Plant_Name");
            //ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m=>m.Plant_ID== plantId), "Shop_ID", "Shop_Name");
            return View();
        }
        // POST: /KittingBarcodeMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Kitt_Barcode_Master RS_Kitt_Barcode_Master)
        {
            if (ModelState.IsValid)
            {
                plantId = Convert.ToInt16( RS_Kitt_Barcode_Master.Plant_ID);
                shopId =Convert.ToInt16( RS_Kitt_Barcode_Master.Shop_ID);
                lineId = Convert.ToInt16( RS_Kitt_Barcode_Master.Line_ID);
                stationId = Convert.ToInt16( RS_Kitt_Barcode_Master.Station_ID);
                //bool result = RS_Kitt_Barcode_Master.IsBarcodeExist(plantId, shopId, lineId, stationId, RS_Kitt_Barcode_Master.Barcode_String);
                //if (result== true)
                //{
                    //ModelState.AddModelError("Barcode_String", ResourceValidation.Barcode_String);
                //}
                //else
                //{
                    RS_Kitt_Barcode_Master.Inserted_Date = DateTime.Now;
                    RS_Kitt_Barcode_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Kitt_Barcode_Master.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Kitt_Barcode_Master.Updated_Date = DateTime.Now;


                    db.RS_Kitt_Barcode_Master.Add(RS_Kitt_Barcode_Master);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                //}
                
            }

            globalData.pageTitle = ResourceModules.Kitt_Barcode_Config;
            globalData.subTitle = ResourceGlobal.Create;
            //globalData.subTitle = "Create";
            globalData.controllerName = "KittingBarcodeMaster";
            globalData.actionName = ResourceGlobal.Create;
            //globalData.contentTitle = ResourcePartGroup.PartGroup_Title_Add_PartGroup;
            //globalData.contentFooter = ResourcePartGroup.PartGroup_Title_Add_PartGroup;

            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Line_Type_Id = new SelectList(db.RS_Line_Types.Where(p => p.Plant_ID == plantId), "Line_Type_ID", "Type_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");

            return View(RS_Kitt_Barcode_Master);
        }

        // GET: /KittingBarcodeMaster/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Kitt_Barcode_Master RS_Kitt_Barcode_Master = db.RS_Kitt_Barcode_Master.Find(id);
            if (RS_Kitt_Barcode_Master == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = "kitting Master Config";
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "KittingBarcodeMasterController";
            globalData.actionName = ResourceGlobal.Edit;

            ViewBag.GlobalDataModel = globalData;
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m=>m.Plant_ID==plantId), "Plant_ID", "Plant_Name", RS_Kitt_Barcode_Master.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m=>m.Plant_ID==plantId), "Shop_ID", "Shop_Name", RS_Kitt_Barcode_Master.Shop_ID);
            //ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Kitt_Barcode_Master.Line_ID);
            
            //added by ketan
            var lineDetail = (from line in db.RS_Lines
                              where line.Shop_ID == RS_Kitt_Barcode_Master.Shop_ID
                              orderby line.Line_Name ascending
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).ToList();
            ViewBag.Line_ID = new SelectList(lineDetail, "Line_ID", "Line_Name", RS_Kitt_Barcode_Master.Line_ID);
            var StationDetail = (from line in db.RS_Stations
                                 where line.Line_ID == RS_Kitt_Barcode_Master.Line_ID
                                 orderby line.Station_Name ascending
                                 select new
                                 {
                                     line.Station_Name,
                                     line.Station_ID
                                 }).ToList();
            ViewBag.Station_ID = new SelectList(StationDetail, "Station_ID", "Station_Name", RS_Kitt_Barcode_Master.Station_ID);



            return View(RS_Kitt_Barcode_Master);
        }

        // POST: /KittingBarcodeMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Kitt_Barcode_Master RS_Kitt_Barcode_Master)
        {
            if (ModelState.IsValid)
            {
                int stationid;
                plantId = Convert.ToInt16(RS_Kitt_Barcode_Master.Plant_ID);
                shopId = Convert.ToInt16(RS_Kitt_Barcode_Master.Shop_ID);
                lineId = Convert.ToInt16(RS_Kitt_Barcode_Master.Line_ID);
                stationid = Convert.ToInt16(RS_Kitt_Barcode_Master.Station_ID);
                //bool result = RS_Kitt_Barcode_Master.IsBarcodeExist(plantId, shopId, lineId, stationid, RS_Kitt_Barcode_Master.Barcode_String);
                //if (result == true)
                //{
                //    ModelState.AddModelError("Barcode_String", ResourceValidation.Exist);
                //}
                //else
                //{
                    RS_Kitt_Barcode_Master objRS_Kitt_Barcode_Master = new RS_Kitt_Barcode_Master();
                    objRS_Kitt_Barcode_Master.Plant_ID = plantId;
                    objRS_Kitt_Barcode_Master.Shop_ID = shopId;
                    objRS_Kitt_Barcode_Master.Line_ID = lineId;
                    objRS_Kitt_Barcode_Master.Station_ID = stationid;
                    objRS_Kitt_Barcode_Master.Barcode_String = RS_Kitt_Barcode_Master.Barcode_String;

                    objRS_Kitt_Barcode_Master.Updated_Date = DateTime.Now;
                    objRS_Kitt_Barcode_Master.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                    objRS_Kitt_Barcode_Master.Inserted_Date = RS_Kitt_Barcode_Master.Inserted_Date;
                    objRS_Kitt_Barcode_Master.Inserted_User_ID = RS_Kitt_Barcode_Master.Inserted_User_ID;

                    objRS_Kitt_Barcode_Master.Kitting_Barcode_ID = RS_Kitt_Barcode_Master.Kitting_Barcode_ID;

                    objRS_Kitt_Barcode_Master.Is_Edited = true;
                    db.Entry(objRS_Kitt_Barcode_Master).State = EntityState.Modified;
                    db.SaveChanges();


                    globalData.isSuccessMessage = true;
                    //globalData.messageTitle = ResourcePartGroup.PartGroup;
                    //globalData.messageDetail = ResourcePartGroup.PartGroup_Success_Edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                //}
            }
            globalData.pageTitle = ResourceModules.Kitt_Barcode_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "KittingBarcodeMasterController";
            globalData.actionName = ResourceGlobal.Edit;

            globalData.contentTitle = ResourcePartGroup.PartGroup_Title_Edit_PartGroup;
            globalData.contentFooter = ResourcePartGroup.PartGroup_Title_Edit_PartGroup;

            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m=>m.Plant_ID==plantId), "Plant_ID", "Plant_Name", RS_Kitt_Barcode_Master.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Kitt_Barcode_Master.Shop_ID);
             //added by ketan
            var lineDetail = (from line in db.RS_Lines
                              where line.Shop_ID == RS_Kitt_Barcode_Master.Shop_ID
                              orderby line.Line_Name ascending
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).ToList();
            ViewBag.Line_ID = new SelectList(lineDetail, "Line_ID", "Line_Name", RS_Kitt_Barcode_Master.Line_ID);
            var StationDetail = (from line in db.RS_Stations
                                 where line.Line_ID == RS_Kitt_Barcode_Master.Line_ID
                                 orderby line.Station_Name ascending
                                 select new
                                 {
                                     line.Station_Name,
                                     line.Station_ID
                                 }).ToList();
            ViewBag.Station_ID = new SelectList(StationDetail, "Station_ID", "Station_Name", RS_Kitt_Barcode_Master.Station_ID);
            return View(RS_Kitt_Barcode_Master);
        }

        // GET: /KittingBarcodeMaster/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Kitt_Barcode_Master RS_Kitt_Barcode_Master = db.RS_Kitt_Barcode_Master.Find(id);

            if (RS_Kitt_Barcode_Master == null)
            {
                return HttpNotFound();
            }
            return View(RS_Kitt_Barcode_Master);
        }

        // POST: /KittingBarcodeMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Kitt_Barcode_Master RS_Kitt_Barcode_Master = db.RS_Kitt_Barcode_Master.Find(id);
            db.RS_Kitt_Barcode_Master.Remove(RS_Kitt_Barcode_Master);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult GetShop(int PlantID)
        {
            var lineDetail = (from shop in db.RS_Shops
                              where shop.Plant_ID == PlantID
                              orderby shop.Shop_Name ascending
                              select new
                              {
                                  shop.Shop_Name,
                                  shop.Shop_ID
                              }).ToList();
            return Json(lineDetail, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLineID(int Shopid)
        {
            var lineDetail = (from line in db.RS_Lines
                              where line.Shop_ID == Shopid
                              orderby line.Line_Name ascending
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).ToList();
            return Json(lineDetail, JsonRequestBehavior.AllowGet);
        }
        /*               Action Name               : GetStationID
       *               Description               : Action used to return the list of station ID for Part Group
       *               Author, Timestamp         : Ketan Dhanuka
       *               Input parameter           : Lineid
       *               Return Type               : ActionResult
       *               Revision                  : 1
      */
        //Find Line
        public ActionResult GetStationID(int Lineid)
        {
            var lineDetail = (from line in db.RS_Stations
                              where line.Line_ID == Lineid
                              orderby line.Station_Name ascending
                              select new
                              {
                                  line.Station_Name,
                                  line.Station_ID
                              }).ToList();
            return Json(lineDetail, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IsBarcodeExist(int Shopid, int LineID, int StationID, string barcodeString)
        {
            IQueryable<RS_Kitt_Barcode_Master> result;

            result = db.RS_Kitt_Barcode_Master.Where(p => p.Shop_ID == Shopid && p.Line_ID == LineID && p.Station_ID == StationID && p.Barcode_String == barcodeString);
            if (result.Count()>0)
            {
               ModelState.AddModelError("Barcode_String", ResourceValidation.Exist);
            }
            return Json(result.Count(), JsonRequestBehavior.AllowGet);
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
