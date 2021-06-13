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

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class PartgroupItemController : Controller
    {
        string partno;
        int partgroupItemId;
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        decimal partgroupId;
        General generalObj = new General();
        
        // GET: PartgroupItem
        public ActionResult Index()
        {
            var RS_PartgroupItem = db.RS_PartgroupItem.Include(m => m.RS_Partgroup);
            return View(RS_PartgroupItem.ToList());
        }

        // GET: PartgroupItem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PartgroupItem RS_PartgroupItem = db.RS_PartgroupItem.Find(id);
            if (RS_PartgroupItem == null)
            {
                return HttpNotFound();
            }
            return View(RS_PartgroupItem);
        }

        // GET: PartgroupItem/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.PartGroup_Item;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "PartgroupItem";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.PartGroup_Item;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.PartGroup_Item;
            ViewBag.GlobalDataModel = globalData;

            RS_PartgroupItem partgroup_item = new RS_PartgroupItem();

            string partgroupitem_id = partgroup_item.GetLastPartGroupItemNumber();

            ViewBag.PartGroupItem_Id = partgroupitem_id;
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup, "Partgroup_ID", "Partgrup_Desc");
            return View();
        }

        // POST: PartgroupItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Row_Id,PartgroupItem_ID,Partgroup_ID,Part_No,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_PartgroupItem RS_PartgroupItem)
        {
            if (ModelState.IsValid)
            {
                partgroupItemId = RS_PartgroupItem.PartgroupItem_ID;
                partgroupId = RS_PartgroupItem.Partgroup_ID;
                partno = RS_PartgroupItem.Part_No;

                RS_PartgroupItem.Inserted_Date = DateTime.Now;
                RS_PartgroupItem.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                RS_PartgroupItem.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                RS_PartgroupItem.Updated_Date = DateTime.Now;

                db.RS_PartgroupItem.Add(RS_PartgroupItem);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.PartGroup_Item;
                globalData.messageDetail = ResourceGlobal.Add + " " + ResourceModules.PartGroup_Item;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }

            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup, "Partgroup_ID", "Partgrup_Desc", RS_PartgroupItem.Partgroup_ID);
            return View(RS_PartgroupItem);
        }

        // GET: PartgroupItem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PartgroupItem RS_PartgroupItem = db.RS_PartgroupItem.Find(id);
            if (RS_PartgroupItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup, "Partgroup_ID", "Partgrup_Desc", RS_PartgroupItem.Partgroup_ID);
            return View(RS_PartgroupItem);
        }

        // POST: PartgroupItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Row_Id,PartgroupItem_ID,Partgroup_ID,Part_No,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_PartgroupItem RS_PartgroupItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(RS_PartgroupItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Partgroup_ID = new SelectList(db.RS_Partgroup, "Partgroup_ID", "Partgrup_Desc", RS_PartgroupItem.Partgroup_ID);
            return View(RS_PartgroupItem);
        }

        // GET: PartgroupItem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PartgroupItem RS_PartgroupItem = db.RS_PartgroupItem.Find(id);
            if (RS_PartgroupItem == null)
            {
                return HttpNotFound();
            }
            return View(RS_PartgroupItem);
        }

        // POST: PartgroupItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RS_PartgroupItem RS_PartgroupItem = db.RS_PartgroupItem.Find(id);
            db.RS_PartgroupItem.Remove(RS_PartgroupItem);
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
