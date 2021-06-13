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
    public class RouteMarriageStationController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        General generalObj = new General();

        // GET: /RouteMarriageStation/
        public ActionResult Index()
        {
            var RS_Route_Marriage_Station = db.RS_Route_Marriage_Station.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Lines1).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations).Include(m => m.RS_Stations1);
            return View(RS_Route_Marriage_Station.ToList());
        }

        // GET: /RouteMarriageStation/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Marriage_Station RS_Route_Marriage_Station = db.RS_Route_Marriage_Station.Find(id);
            if (RS_Route_Marriage_Station == null)
            {
                return HttpNotFound();
            }
            return View(RS_Route_Marriage_Station);
        }

        // GET: /RouteMarriageStation/Create
        public ActionResult Create()
        {
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            return View();
        }

        // POST: /RouteMarriageStation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Route_Marriage_Station,Plant_ID,Shop_ID,Sub_Line_ID,Sub_Line_Station_ID,Marriage_Line_ID,Marriage_Station_ID,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Marriage_Station RS_Route_Marriage_Station)
        {
            if (ModelState.IsValid)
            {
                db.RS_Route_Marriage_Station.Add(RS_Route_Marriage_Station);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Station.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Station.Updated_User_ID);
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Station.Marriage_Line_ID);
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Station.Sub_Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Marriage_Station.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Station.Shop_ID);
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Station.Marriage_Station_ID);
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Station.Sub_Line_Station_ID);
            return View(RS_Route_Marriage_Station);
        }

        // GET: /RouteMarriageStation/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Marriage_Station RS_Route_Marriage_Station = db.RS_Route_Marriage_Station.Find(id);
            if (RS_Route_Marriage_Station == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Station.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Station.Updated_User_ID);
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Station.Marriage_Line_ID);
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Station.Sub_Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Marriage_Station.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Station.Shop_ID);
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Station.Marriage_Station_ID);
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Station.Sub_Line_Station_ID);
            return View(RS_Route_Marriage_Station);
        }

        // POST: /RouteMarriageStation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Route_Marriage_Station,Plant_ID,Shop_ID,Sub_Line_ID,Sub_Line_Station_ID,Marriage_Line_ID,Marriage_Station_ID,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Marriage_Station RS_Route_Marriage_Station)
        {
            if (ModelState.IsValid)
            {
                db.Entry(RS_Route_Marriage_Station).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Station.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Station.Updated_User_ID);
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Station.Marriage_Line_ID);
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Marriage_Station.Sub_Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Marriage_Station.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Marriage_Station.Shop_ID);
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Station.Marriage_Station_ID);
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Marriage_Station.Sub_Line_Station_ID);
            return View(RS_Route_Marriage_Station);
        }

        // GET: /RouteMarriageStation/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Marriage_Station RS_Route_Marriage_Station = db.RS_Route_Marriage_Station.Find(id);
            if (RS_Route_Marriage_Station == null)
            {
                return HttpNotFound();
            }
            return View(RS_Route_Marriage_Station);
        }

        // POST: /RouteMarriageStation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Route_Marriage_Station RS_Route_Marriage_Station = db.RS_Route_Marriage_Station.Find(id);
            db.RS_Route_Marriage_Station.Remove(RS_Route_Marriage_Station);
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
