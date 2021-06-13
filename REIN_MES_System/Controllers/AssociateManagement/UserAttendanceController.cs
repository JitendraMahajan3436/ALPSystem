using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using System.Dynamic;
using System.Web.Routing;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /* Class Name                 : UserAttendanceController
    *  Description                : This class is used to show the report of attandance of operators to supervisor
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class UserAttendanceController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();


        /*	    Action Name		    : Index
        *		Description		    : To Display the supervisor attandance information in grid
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: UserAttendance
        public ActionResult Index()
        {
            var RS_User_Attendance_Sheet = db.RS_User_Attendance_Sheet.Include(m => m.RS_Employee);
            // var b = (from t in db.RS_User_Attendance_Sheet group t by new { t.Plant_ID, t.Shop_ID, t.Line_ID,t.Station_ID,t.User_ID } into attendance select new {plant=attendance.Key.Plant_ID,shop=attendance.Key.Shop_ID,line=attendance.Key.Line_ID, user=attendance.Key.User_ID });

            //var group = (from t in db.RS_User_Attendance_Sheet
            //             group t by new {  t.Shop_ID, t.Line_ID, t.Station_ID }
            //                 into attendance
            //                 select
            //                 new ExtensionClass()
            //                 {
            //                     plant_id =db.RS_Plants.Where(y=>y.Plant_ID==db.RS_Shops.Where(x=>x.Shop_ID==attendance.Key.Shop_ID).FirstOrDefault().Plant_ID).FirstOrDefault().Plant_Name,
            //                     shop_id = db.RS_Shops.Where(x=>x.Shop_ID==attendance.Key.Shop_ID).FirstOrDefault().Shop_Name,
            //                     line_id = db.RS_Lines.Where(x => x.Line_ID == attendance.Key.Line_ID).FirstOrDefault().Line_Name,
            //                    // station_id = db.RS_Stations.Where(x => x.Station_ID == attendance.Key.Station_ID).FirstOrDefault().Station_Name,
            //                     user_id = attendance.Count()

            //                 }
            //             ).ToList();

            var group = (from t in db.RS_User_Attendance_Sheet
                         group t by new {t.Plant_ID, t.Shop_ID, t.Line_ID, t.Station_ID }
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

            var one = group.GroupBy(x => x.plant_id);
            var two = group.GroupBy(x => x.shop_id);
            var three = group.GroupBy(x => x.line_id);
            var four = group.GroupBy(x => x.station_id);



            ViewBag.Group = group;
            // var r= b.Select(x => x.user).Count();
            //return View(RS_User_Attendance_Sheet.ToList());
            return View();
        }


        /*	    Action Name		    : Details
         *		Description		    : To show the supervisor attandance detailed information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : id of supervisor whose information is to be displayed 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: UserAttendance/Details/5
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


        /*	    Action Name		    : Create
        *		Description		    : To read the supervisor attandance info which is to be saved
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: UserAttendance/Create
        public ActionResult Create()
        {
            ViewBag.User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            return View();
        }


        /*	    Action Name		    : Create
         *		Description		    : To save the supervisor attandance information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : RS_Employee object 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // POST: UserAttendance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Attendance_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Shift_ID,User_ID,In_Time,Out_Time,Lunch_Time_Out,Lunch_Time_In,Attendance_Date,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_User_Attendance_Sheet RS_User_Attendance_Sheet)
        {
            if (ModelState.IsValid)
            {
                db.RS_User_Attendance_Sheet.Add(RS_User_Attendance_Sheet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_User_Attendance_Sheet.Employee_ID);
            return View(RS_User_Attendance_Sheet);
        }


        /*	    Action Name		    : Edit
        *		Description		    : To read the supervisor attaedance information which is to be edited
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: UserAttendance/Edit/5
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
            ViewBag.User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_User_Attendance_Sheet.Employee_ID);
            return View(RS_User_Attendance_Sheet);
        }



        /*	    Action Name		    : Edit
       *		Description		    : To edit the supervisor attandance information
       *		Author, Timestamp	: Jitendra Mahajan
       *		Input parameter	    : RS_Employee object 
       *		Return Type		    : ActionResult
       *		Revision		    :
       */
        // POST: UserAttendance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Attendance_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Shift_ID,User_ID,In_Time,Out_Time,Lunch_Time_Out,Lunch_Time_In,Attendance_Date,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_User_Attendance_Sheet RS_User_Attendance_Sheet)
        {
            if (ModelState.IsValid)
            {
                RS_User_Attendance_Sheet.Is_Edited = true;
                db.Entry(RS_User_Attendance_Sheet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_User_Attendance_Sheet.Employee_ID);
            return View(RS_User_Attendance_Sheet);
        }


        /*	    Action Name		    : Delete
        *		Description		    : To Display the supervisor attandance information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: UserAttendance/Delete/5
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
        *		Description		    : To delete the supervisor attandance record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: UserAttendance/Delete/5
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
