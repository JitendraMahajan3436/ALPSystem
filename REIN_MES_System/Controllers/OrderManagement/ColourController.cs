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
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class ColourController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_Colour color = new RS_Colour();
        GlobalData globalData = new GlobalData();

        General generalObj = new General();

        // GET: Colour
        public ActionResult Index()
        {
            globalData.pageTitle = "Colour Configuration";
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "Colour";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = "Colour";
            globalData.contentFooter = "Colour";
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 1);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            return View(db.RS_Colour.ToList());
        }

        // GET: Colour/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Colour RS_Colour = db.RS_Colour.Find(id);
            if (RS_Colour == null)
            {
                return HttpNotFound();
            }
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 1);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            globalData.pageTitle = "Colour Configuration";
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "Colour";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = "Colour";
            globalData.contentFooter = "Colour";
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Colour);
        }

        // GET: Colour/Create
        public ActionResult Create()
        {
            globalData.pageTitle = "Colour Configuration";
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "Colour";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = "Colour";
            globalData.contentFooter = "Colour";
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 1);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            return View();
        }

        // POST: Colour/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Plant_ID,Colour_ID,Colour_Desc,Colour_Batch,Export_Colour,Active,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Colour RS_Colour)
        {
            if (ModelState.IsValid)
            {
                RS_Colour.Inserted_Date = DateTime.Now;
                RS_Colour.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                RS_Colour.Updated_Date = DateTime.Now;
                RS_Colour.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.RS_Colour.Add(RS_Colour);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = "Colour Configuration";
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "Colour";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = "Colour";
            globalData.contentFooter = "Colour";
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 1);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            return View(RS_Colour);
        }

        // GET: Colour/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Colour RS_Colour = db.RS_Colour.Find(id);
            if (RS_Colour == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = "Colour Configuration";
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "Colour";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = "Colour";
            globalData.contentFooter = "Colour";
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 1);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            return View(RS_Colour);
        }

        // POST: Colour/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Plant_ID,Colour_ID,Colour_Desc,Colour_Batch,Export_Colour,Active,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Colour RS_Colour)
        {
            if (ModelState.IsValid)
            {
                color = db.RS_Colour.Find(RS_Colour.Colour_ID);
                color.Updated_Date = DateTime.Now;
                color.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                color.Colour_Desc = RS_Colour.Colour_Desc;
                color.Colour_Batch = RS_Colour.Colour_Batch;
                color.Active = RS_Colour.Active;
                color.Export_Colour = RS_Colour.Export_Colour;
                color.Plant_ID = RS_Colour.Plant_ID;
                db.Entry(color).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = "Colour Configuration";
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "Colour";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = "Colour";
            globalData.contentFooter = "Colour";
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 1);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            return View(RS_Colour);
        }

        // GET: Colour/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Colour RS_Colour = db.RS_Colour.Find(id);
            if (RS_Colour == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = "Colour Configuration";
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "Colour";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = "Colour";
            globalData.contentFooter = "Colour";
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 1);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            return View(RS_Colour);
        }

        // POST: Colour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Colour RS_Colour = db.RS_Colour.Find(id);
            db.RS_Colour.Remove(RS_Colour);
            db.SaveChanges();

            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Colour", "Colour_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

            globalData.pageTitle = "Colour Configuration";
            // globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_Add_AttributionParameters;
            globalData.controllerName = "Colour";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = "Colour";
            globalData.contentFooter = "Colour";
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
