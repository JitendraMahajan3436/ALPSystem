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

namespace REIN_MES_System.Controllers.OrderManagement
{
    /* Class Name                 : ManagerAttendanceController
   *  Description                : This class is used to show the report of attandance of operators and supervisors to managers
   *  Author, Timestamp          : Jitendra Mahajan      
   */
    public class ManagerAttendanceController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /*	    Action Name		    : Index
        *		Description		    : To Display the Managers attendance in grid
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: ManagerAttendance
        public ActionResult Index()
        {
            var RS_User_Attendance_Sheet = db.RS_User_Attendance_Sheet.Include(m => m.RS_Employee).Include(m => m.RS_Shift);
            var group = (from t in db.RS_User_Attendance_Sheet
                         group t by new { t.Plant_ID, t.Shop_ID, t.Line_ID, t.Station_ID }
                             into attendance
                             select
                             new ExtensionClass()
                             {
                                 plant_id = db.RS_Plants.Where(y => y.Plant_ID == db.RS_Shops.Where(x => x.Shop_ID == attendance.Key.Shop_ID).FirstOrDefault().Plant_ID).FirstOrDefault().Plant_Name,
                                 shop_id = db.RS_Shops.Where(x => x.Shop_ID == attendance.Key.Shop_ID).FirstOrDefault().Shop_Name,
                                 line_id = db.RS_Lines.Where(x => x.Line_ID == attendance.Key.Line_ID).FirstOrDefault().Line_Name,
                                 station_id = db.RS_Stations.Where(x => x.Station_ID == attendance.Key.Station_ID).FirstOrDefault().Station_Name,
                                 user_id = attendance.Count()
                                 //supcount=attendance.Count(),
                                 //operatorcount=attendance.Count()

                             }
                       ).ToList();


            var cc = (from t in db.RS_User_Attendance_Sheet group t by t.Shop_ID into grou select grou.Key);



           //var Supervisor = (from r1 in db.RS_User_Attendance_Sheet.AsEnumerable()
           //   join r2 in db.RS_AM_Line_Supervisor_Mapping.AsEnumerable()
           //              on new {signal=r1.Employee_ID} equals new {signal=r2.Employee_ID} into prodGroup 
           //              from r3 in prodGroup select r3.Employee_ID).Distinct();



           //var Manager = (from r1 in db.RS_User_Attendance_Sheet.AsEnumerable()
           //               join r2 in db.RS_AM_Shop_Manager_Mapping.AsEnumerable()
           //                          on new { signal = r1.Employee_ID } equals new { signal = r2.Employee_ID } into prodGroup
           //               from r3 in prodGroup
           //               select r3.Employee_ID).Distinct();
          
            

            var one = group.GroupBy(x => x.plant_id);
            var two = group.GroupBy(x => x.shop_id);
            var three = group.GroupBy(x => x.line_id);
            var four = group.GroupBy(x => x.station_id);



            ViewBag.Group = group;
            return View();
            //return View(RS_User_Attendance_Sheet.ToList());
        }


        /*	    Action Name		    : Details
         *		Description		    : To show the managers attandance detailed information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : id  
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: ManagerAttendance/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_User_Attendance_Sheet RS_User_Attendance_Sheet = db.RS_User_Attendance_Sheet.Find(id);
            if (RS_User_Attendance_Sheet == null)
            {
                return HttpNotFound();
            }
            return View(RS_User_Attendance_Sheet);
        }

        // GET: ManagerAttendance/Create
        public ActionResult Create()
        {
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name");
            return View();
        }

        // POST: ManagerAttendance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Attendance_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Shift_ID,Employee_ID,In_Time,Out_Time,Lunch_Time_Out,Lunch_Time_In,Attendance_Date,Is_Transfered,Is_Purgeable,Inserted_Employee_ID,Inserted_Date,Updated_Employee_ID,Updated_Date")] RS_User_Attendance_Sheet RS_User_Attendance_Sheet)
        {
            if (ModelState.IsValid)
            {
                db.RS_User_Attendance_Sheet.Add(RS_User_Attendance_Sheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_User_Attendance_Sheet.Employee_ID);
            ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name", RS_User_Attendance_Sheet.Shift_ID);
            return View(RS_User_Attendance_Sheet);
        }

        // GET: ManagerAttendance/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_User_Attendance_Sheet RS_User_Attendance_Sheet = db.RS_User_Attendance_Sheet.Find(id);
            if (RS_User_Attendance_Sheet == null)
            {
                return HttpNotFound();
            }
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_User_Attendance_Sheet.Employee_ID);
            ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name", RS_User_Attendance_Sheet.Shift_ID);
            return View(RS_User_Attendance_Sheet);
        }

        // POST: ManagerAttendance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Attendance_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Shift_ID,Employee_ID,In_Time,Out_Time,Lunch_Time_Out,Lunch_Time_In,Attendance_Date,Is_Transfered,Is_Purgeable,Inserted_Employee_ID,Inserted_Date,Updated_Employee_ID,Updated_Date")] RS_User_Attendance_Sheet RS_User_Attendance_Sheet)
        {
            if (ModelState.IsValid)
            {
                RS_User_Attendance_Sheet.Is_Edited = true;
                db.Entry(RS_User_Attendance_Sheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_User_Attendance_Sheet.Employee_ID);
            ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name", RS_User_Attendance_Sheet.Shift_ID);
            return View(RS_User_Attendance_Sheet);
        }


        /*	    Action Name		    : Delete
        *		Description		    : To Display the users information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: ManagerAttendance/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_User_Attendance_Sheet RS_User_Attendance_Sheet = db.RS_User_Attendance_Sheet.Find(id);
            if (RS_User_Attendance_Sheet == null)
            {
                return HttpNotFound();
            }
            return View(RS_User_Attendance_Sheet);
        }


        /*	    Action Name		    : DeleteConfirmed
        *		Description		    : To delete the users record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: ManagerAttendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_User_Attendance_Sheet RS_User_Attendance_Sheet = db.RS_User_Attendance_Sheet.Find(id);
            db.RS_User_Attendance_Sheet.Remove(RS_User_Attendance_Sheet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        /*	    Action Name		    : Dispose
        *		Description		    : To clear the memory allocated by objects
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : disposing bool value 
        *		Return Type		    :
        *		Revision		    :
        */
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
