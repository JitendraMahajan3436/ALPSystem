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

namespace REIN_MES_System.Controllers
{
    public class OrderRetrievalController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        General generalObj = new General();

        // GET: OrderRetrieval
        public ActionResult Index()
        {
            var RS_OM_Order_Retrieval = db.RS_OM_Order_Retrieval.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations);
            return View(RS_OM_Order_Retrieval.ToList());
        }

        // GET: OrderRetrieval/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Order_Retrieval RS_OM_Order_Retrieval = db.RS_OM_Order_Retrieval.Find(id);
            if (RS_OM_Order_Retrieval == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_Order_Retrieval);
        }

        // GET: OrderRetrieval/Create
        public ActionResult Create()
        {
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            return View();
        }

        // POST: OrderRetrieval/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Order_Retrieval_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Main_Serial_No,Main_Model_Code,Requested_Model_Code,Provided_Model_Code,Provided_Serial_No,Is_Transfered,Is_Purgeable,Is_Deleted,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_OM_Order_Retrieval RS_OM_Order_Retrieval)
        {
            if (ModelState.IsValid)
            {
                db.RS_OM_Order_Retrieval.Add(RS_OM_Order_Retrieval);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_OM_Order_Retrieval.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_OM_Order_Retrieval.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_OM_Order_Retrieval.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_Order_Retrieval.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_OM_Order_Retrieval.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_OM_Order_Retrieval.Station_ID);
            return View(RS_OM_Order_Retrieval);
        }

        // GET: OrderRetrieval/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Order_Retrieval RS_OM_Order_Retrieval = db.RS_OM_Order_Retrieval.Find(id);
            if (RS_OM_Order_Retrieval == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_OM_Order_Retrieval.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_OM_Order_Retrieval.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_OM_Order_Retrieval.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_Order_Retrieval.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_OM_Order_Retrieval.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_OM_Order_Retrieval.Station_ID);
            return View(RS_OM_Order_Retrieval);
        }

        // POST: OrderRetrieval/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Order_Retrieval_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Main_Serial_No,Main_Model_Code,Requested_Model_Code,Provided_Model_Code,Provided_Serial_No,Is_Transfered,Is_Purgeable,Is_Deleted,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_OM_Order_Retrieval RS_OM_Order_Retrieval)
        {
            if (ModelState.IsValid)
            {
                db.Entry(RS_OM_Order_Retrieval).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_OM_Order_Retrieval.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_OM_Order_Retrieval.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_OM_Order_Retrieval.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_Order_Retrieval.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_OM_Order_Retrieval.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_OM_Order_Retrieval.Station_ID);
            return View(RS_OM_Order_Retrieval);
        }

        // GET: OrderRetrieval/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Order_Retrieval RS_OM_Order_Retrieval = db.RS_OM_Order_Retrieval.Find(id);
            if (RS_OM_Order_Retrieval == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_Order_Retrieval);
        }

        // POST: OrderRetrieval/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_OM_Order_Retrieval RS_OM_Order_Retrieval = db.RS_OM_Order_Retrieval.Find(id);
            db.RS_OM_Order_Retrieval.Remove(RS_OM_Order_Retrieval);
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
