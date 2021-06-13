using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class OrderTypeController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        General generalObj = new General();
        int plantId = 0;
        // GET: OrderType
        public ActionResult Index()
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                globalData.pageTitle = ResourceModules.Order_Type_Config;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "OrderTypeController";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.Order_Type + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.Order_Type + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;
                return View(db.RS_OM_Order_Type.ToList());
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
            
        }

        // GET: OrderType/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Order_Type RS_OM_Order_Type = db.RS_OM_Order_Type.Find(id);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Order_Type;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "OrderTypeController";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Order_Type + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Order_Type + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            if (RS_OM_Order_Type == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_Order_Type);
        }

        // GET: OrderType/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Order_Type;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "OrderTypeController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Order_Type + " " + ResourceGlobal.Create;
            globalData.contentFooter = ResourceModules.Order_Type + " " + ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", plantId);
            return View();
        }

        // POST: OrderType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_OM_Order_Type RS_OM_Order_Type)
        {
            if (ModelState.IsValid)
            {
                RS_OM_Order_Type.Inserted_Date = DateTime.Now;
                RS_OM_Order_Type.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.RS_OM_Order_Type.Add(RS_OM_Order_Type);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Order_Type;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "OrderTypeController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Order_Type + " " + ResourceGlobal.Create;
            globalData.contentFooter = ResourceModules.Order_Type + " " + ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", plantId);
            return View(RS_OM_Order_Type);
        }

        // GET: OrderType/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Order_Type RS_OM_Order_Type = db.RS_OM_Order_Type.Find(id);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Order_Type;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "OrderTypeController";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceModules.Order_Type + " " + ResourceGlobal.Edit;
            globalData.contentFooter = ResourceModules.Order_Type + " " + ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", plantId);
            if (RS_OM_Order_Type == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_Order_Type);
        }

        // POST: OrderType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_OM_Order_Type RS_OM_Order_Type)
        {
            if (ModelState.IsValid)
            {
                RS_OM_Order_Type obj_RS_OM_Order_Type = db.RS_OM_Order_Type.Find(RS_OM_Order_Type.Order_Type_ID);
                obj_RS_OM_Order_Type.Is_Build_Sheet_Print = RS_OM_Order_Type.Is_Build_Sheet_Print;
                obj_RS_OM_Order_Type.Is_Production_Booking = RS_OM_Order_Type.Is_Production_Booking;
                obj_RS_OM_Order_Type.Is_Serial_No_Generation = RS_OM_Order_Type.Is_Serial_No_Generation;
                obj_RS_OM_Order_Type.Is_Spare = RS_OM_Order_Type.Is_Spare;
                obj_RS_OM_Order_Type.Order_Type_Code = RS_OM_Order_Type.Order_Type_Code;
                obj_RS_OM_Order_Type.Order_Type_Name = RS_OM_Order_Type.Order_Type_Name;
                obj_RS_OM_Order_Type.Is_Authorize_By_Supervisor = RS_OM_Order_Type.Is_Authorize_By_Supervisor;
                obj_RS_OM_Order_Type.Shop_ID = RS_OM_Order_Type.Shop_ID;
                db.Entry(obj_RS_OM_Order_Type).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Order_Type;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "OrderTypeController";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceModules.Order_Type + " " + ResourceGlobal.Edit;
            globalData.contentFooter = ResourceModules.Order_Type + " " + ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", plantId);
            return View(RS_OM_Order_Type);
        }

        // GET: OrderType/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Order_Type RS_OM_Order_Type = db.RS_OM_Order_Type.Find(id);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Order_Type;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "OrderTypeController";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceModules.Order_Type + " " + ResourceGlobal.Delete;
            globalData.contentFooter = ResourceModules.Order_Type + " " + ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", plantId);
            if (RS_OM_Order_Type == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_Order_Type);
        }

        // POST: OrderType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_OM_Order_Type RS_OM_Order_Type = db.RS_OM_Order_Type.Find(id);
            db.RS_OM_Order_Type.Remove(RS_OM_Order_Type);
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
