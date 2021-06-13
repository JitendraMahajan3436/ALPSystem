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
    /* Controller Name            : RouteController
    *  Description                : This controller is used to define the route
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class RouteController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        General generalObj = new General();

        // GET: /Route/
        public ActionResult Index()
        {
            var RS_Route_Configurations = db.RS_Route_Configurations.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations).Include(m => m.RS_Stations1).Include(m => m.RS_Stations2);
            return View(RS_Route_Configurations.ToList());
        }

        // GET: /Route/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Configurations RS_Route_Configurations = db.RS_Route_Configurations.Find(id);
            if (RS_Route_Configurations == null)
            {
                return HttpNotFound();
            }
            return View(RS_Route_Configurations);
        }

        // GET: /Route/Create
        public ActionResult Create()
        {
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            ViewBag.Next_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            ViewBag.Prev_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            return View();
        }

        // POST: /Route/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Route_Configuration_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Prev_Station_ID,Next_Station_ID,Is_Start_Station,Is_End_Station,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Configurations RS_Route_Configurations)
        {
            if (ModelState.IsValid)
            {
                db.RS_Route_Configurations.Add(RS_Route_Configurations);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Configurations.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Configurations.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Configurations.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Station_ID);
            ViewBag.Next_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Next_Station_ID);
            ViewBag.Prev_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Prev_Station_ID);
            return View(RS_Route_Configurations);
        }

        // GET: /Route/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Configurations RS_Route_Configurations = db.RS_Route_Configurations.Find(id);
            if (RS_Route_Configurations == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Configurations.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Configurations.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Configurations.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Station_ID);
            ViewBag.Next_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Next_Station_ID);
            ViewBag.Prev_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Prev_Station_ID);
            return View(RS_Route_Configurations);
        }

        // POST: /Route/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Route_Configuration_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Prev_Station_ID,Next_Station_ID,Is_Start_Station,Is_End_Station,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Configurations RS_Route_Configurations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(RS_Route_Configurations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Configurations.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Route_Configurations.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Configurations.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Route_Configurations.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Station_ID);
            ViewBag.Next_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Next_Station_ID);
            ViewBag.Prev_Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Route_Configurations.Prev_Station_ID);
            return View(RS_Route_Configurations);
        }

        // GET: /Route/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Route_Configurations RS_Route_Configurations = db.RS_Route_Configurations.Find(id);
            if (RS_Route_Configurations == null)
            {
                return HttpNotFound();
            }
            return View(RS_Route_Configurations);
        }

        // POST: /Route/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Route_Configurations RS_Route_Configurations = db.RS_Route_Configurations.Find(id);
            db.RS_Route_Configurations.Remove(RS_Route_Configurations);
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
