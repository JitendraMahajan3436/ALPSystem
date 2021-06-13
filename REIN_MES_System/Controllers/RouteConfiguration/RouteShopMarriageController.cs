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

namespace REIN_MES_System.Controllers.RouteConfiguration
{
    public class RouteShopMarriageController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        General generalObj = new General();

        // GET: /RouteShopMarriage/
        public ActionResult Index()
        {
            var RS_Route_Marriage_Shop = db.RS_Route_Marriage_Shop.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Lines1).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Shops1).Include(m => m.RS_Stations).Include(m => m.RS_Stations1);
            return View(RS_Route_Marriage_Shop.ToList());
        }

        // GET: /RouteShopMarriage/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Marriage_Shop RS_Route_Marriage_Shop = db.RS_Route_Marriage_Shop.Find(id);
            if (RS_Route_Marriage_Shop == null)
            {
                return HttpNotFound();
            }
            return View(RS_Route_Marriage_Shop);
        }

        // GET: /RouteShopMarriage/Create
        public ActionResult Create()
        {
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Marriage_Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Sub_Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            return View();
        }

        // POST: /RouteShopMarriage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Route_Marriage_Shop_Station,Plant_ID,Sub_Shop_ID,Sub_Line_ID,Sub_Line_Station_ID,Marriage_Shop_ID,Marriage_Line_ID,Marriage_Station_ID,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Marriage_Shop RS_Route_Marriage_Shop)
        {
            if (ModelState.IsValid)
            {
                db.RS_Route_Marriage_Shop.Add(RS_Route_Marriage_Shop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Shop.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Shop.Updated_User_ID);
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Shop.Marriage_Line_ID);
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Shop.Sub_Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Marriage_Shop.Plant_ID);
            ViewBag.Marriage_Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Shop.Marriage_Shop_ID);
            ViewBag.Sub_Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Shop.Sub_Shop_ID);
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Shop.Marriage_Station_ID);
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Shop.Sub_Line_Station_ID);
            return View(RS_Route_Marriage_Shop);
        }

        // GET: /RouteShopMarriage/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Marriage_Shop RS_Route_Marriage_Shop = db.RS_Route_Marriage_Shop.Find(id);
            if (RS_Route_Marriage_Shop == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Shop.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Shop.Updated_User_ID);
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Shop.Marriage_Line_ID);
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Shop.Sub_Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Marriage_Shop.Plant_ID);
            ViewBag.Marriage_Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Shop.Marriage_Shop_ID);
            ViewBag.Sub_Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Shop.Sub_Shop_ID);
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Shop.Marriage_Station_ID);
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Shop.Sub_Line_Station_ID);
            return View(RS_Route_Marriage_Shop);
        }

        // POST: /RouteShopMarriage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Route_Marriage_Shop_Station,Plant_ID,Sub_Shop_ID,Sub_Line_ID,Sub_Line_Station_ID,Marriage_Shop_ID,Marriage_Line_ID,Marriage_Station_ID,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Marriage_Shop RS_Route_Marriage_Shop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(RS_Route_Marriage_Shop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Shop.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Shop.Updated_User_ID);
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Shop.Marriage_Line_ID);
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Shop.Sub_Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Marriage_Shop.Plant_ID);
            ViewBag.Marriage_Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Shop.Marriage_Shop_ID);
            ViewBag.Sub_Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Shop.Sub_Shop_ID);
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Shop.Marriage_Station_ID);
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Shop.Sub_Line_Station_ID);
            return View(RS_Route_Marriage_Shop);
        }

        // GET: /RouteShopMarriage/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Marriage_Shop RS_Route_Marriage_Shop = db.RS_Route_Marriage_Shop.Find(id);
            if (RS_Route_Marriage_Shop == null)
            {
                return HttpNotFound();
            }
            return View(RS_Route_Marriage_Shop);
        }

        // POST: /RouteShopMarriage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Route_Marriage_Shop RS_Route_Marriage_Shop = db.RS_Route_Marriage_Shop.Find(id);
            db.RS_Route_Marriage_Shop.Remove(RS_Route_Marriage_Shop);
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
