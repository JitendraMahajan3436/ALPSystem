using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Helper;
using System.Data.Entity.Infrastructure;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class MachineAlarmDashboardController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        private REIN_SOLUTION_MEntities mttuwDB = new REIN_SOLUTION_MEntities();
        int plantId = 0;

        // GET: /MachineAlarmDashboard/
        public ActionResult Index()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.ShopTitle = "Machine Dashboard";

            // process to get the shop id of login user form line supervisor mapping
            int userId = ((FDSession)this.Session["FDSession"]).userId;
            MM_AM_Line_Supervisor_Mapping[] mmAMLineSupervisorMappingObj = db.MM_AM_Line_Supervisor_Mapping.Where(p => p.Employee_ID == userId).ToArray();
            if (mmAMLineSupervisorMappingObj.Count() > 0)
            {
                ViewBag.User_Shop_ID = mmAMLineSupervisorMappingObj[0].Shop_ID;
            }

            var mm_mt_machines = db.MM_MT_Machines.Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations);
            return View(mm_mt_machines.ToList());
        }

        public ActionResult GetMachineDetails(int shopId)
        {
            var mm_mt_machines = db.MM_MT_Machines.Where(p => p.Shop_ID == shopId && p.Is_Status_Machine == true).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations);
            return PartialView(mm_mt_machines.ToList());
        }


        // GET: /MachineAlarmDashboard/Create
        public ActionResult Create()
        {
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            return View();
        }

        // POST: /MachineAlarmDashboard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Machine_ID,Machine_Number,Machine_Name,Machine_Description,Plant_ID,Shop_ID,Line_ID,Station_ID,FMEA_Document,Family,Machine_Category_ID,Manufaturing_Year,Is_Status_Machine,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Machines mm_mt_machines)
        {
            if (ModelState.IsValid)
            {
                db.MM_MT_Machines.Add(mm_mt_machines);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_mt_machines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_mt_machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mm_mt_machines.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mm_mt_machines.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_mt_machines.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mm_mt_machines.Station_ID);
            return View(mm_mt_machines);
        }

        // GET: /MachineAlarmDashboard/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Machines mm_mt_machines = db.MM_MT_Machines.Find(id);
            if (mm_mt_machines == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_mt_machines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_mt_machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mm_mt_machines.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mm_mt_machines.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_mt_machines.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mm_mt_machines.Station_ID);
            return View(mm_mt_machines);
        }

        // POST: /MachineAlarmDashboard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Machine_ID,Machine_Number,Machine_Name,Machine_Description,Plant_ID,Shop_ID,Line_ID,Station_ID,FMEA_Document,Family,Machine_Category_ID,Manufaturing_Year,Is_Status_Machine,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_Machines mm_mt_machines)
        {
            if (ModelState.IsValid)
            {
                mm_mt_machines.Is_Edited = true;
                db.Entry(mm_mt_machines).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_mt_machines.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_mt_machines.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mm_mt_machines.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mm_mt_machines.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_mt_machines.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mm_mt_machines.Station_ID);
            return View(mm_mt_machines);
        }

        // GET: /MachineAlarmDashboard/Delete/5
        //public ActionResult Delete(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MM_MT_Machines mm_mt_machines = db.MM_MT_Machines.Find(id);
        //    if (mm_mt_machines == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mm_mt_machines);
        //}

        //// POST: /MachineAlarmDashboard/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(decimal id)
        //{
        //    MM_MT_Machines mm_mt_machines = db.MM_MT_Machines.Find(id);
        //    db.MM_MT_Machines.Remove(mm_mt_machines);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetMachineAlarmStatus(int shopId)
        {
            try
            {
                var res = (from machineAlarmStatusObj in mttuwDB.MM_Ctrl_Machine_Alarms
                           where machineAlarmStatusObj.Shop_ID == shopId && machineAlarmStatusObj.Status == true
                           select new
                           {
                               Machine_ID = machineAlarmStatusObj.Machine_ID
                           }).ToList();

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMachineAlarmDetails(int machineId)
        {
            try
            {
                DateTime date = DateTime.Now.Date;
                var res = from machineAlarmHistory in mttuwDB.MM_Ctrl_Machine_Alarm_Data_History
                          where machineAlarmHistory.Alarm_Start_Time >= date && machineAlarmHistory.Machine_ID == machineId
                          select new
                          {
                              Alarm_Name = (from alarmsMasterObj in mttuwDB.MM_Ctrl_Machine_Alarms_Master where alarmsMasterObj.Alarm_ID == machineAlarmHistory.Alarm_ID select alarmsMasterObj.Alarm_Name),
                              Start_Time = machineAlarmHistory.Alarm_Start_Time.ToString(),
                              Stop_Time = machineAlarmHistory.Alarm_Stop_Time.ToString(),
                              Total_Time = machineAlarmHistory.Alarm_Time.ToString()
                          };


                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult getStatusByShop(int shopId)
        {
            var status = db.MM_Ctrl_Equipment_Status
                // .Where(x=>x.Inserted_Date.Value.Year==DateTime.Now.Year && x.Inserted_Date.Value.Month==DateTime.Now.Month && x.Inserted_Date.Value.Day==DateTime.Now.Day)
                .Where(p => p.Shop_ID == shopId)
                .GroupBy(c => c.Machine_ID)
                .Select(g => g.OrderByDescending(c => c.EQ_ID).FirstOrDefault())
                .Select(c => new { c.Machine_ID, c.isFaulty, c.isHealthy, c.isIdle, c.Heart_Bit });
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMachineDownTimeByShop(int shopId)
        {
            try
            {
                String SQLQuery = "";
                SQLQuery = "select Machine_ID, DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', down_time)), '00:00:00.000') as DownTime, Type='Idle' ";
                SQLQuery = SQLQuery + "from MM_Ctrl_Equipment_Status_History ";
                SQLQuery = SQLQuery + "where CONVERT(DATE,inserted_date)=CONVERT(DATE, GETDATE()) ";
                SQLQuery = SQLQuery + "and isidle=1 ";
                SQLQuery = SQLQuery + "group by machine_id ";

                SQLQuery = SQLQuery + " union ";

                SQLQuery = SQLQuery + "select Machine_ID, DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', down_time)), '00:00:00.000') as DownTime, Type='Faulty' ";
                SQLQuery = SQLQuery + "from MM_Ctrl_Equipment_Status_History ";
                SQLQuery = SQLQuery + "where CONVERT(DATE,inserted_date)=CONVERT(DATE, GETDATE()) ";
                SQLQuery = SQLQuery + "and isFaulty=1 ";
                SQLQuery = SQLQuery + "group by machine_id ";

                SQLQuery = SQLQuery + "union ";

                SQLQuery = SQLQuery + "select Machine_ID, DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', down_time)), '00:00:00.000') as DownTime, Type='Healthy' ";
                SQLQuery = SQLQuery + "from MM_Ctrl_Equipment_Status_History ";
                SQLQuery = SQLQuery + "where CONVERT(DATE,inserted_date)=CONVERT(DATE, GETDATE()) ";
                SQLQuery = SQLQuery + "and isHealthy=1 ";
                SQLQuery = SQLQuery + "group by machine_id ";

                var objectContext = ((IObjectContextAdapter)db).ObjectContext;
                List<object> listobj = new List<object>();
                List<MachineAlarmDownTime> data = objectContext.ExecuteStoreQuery<MachineAlarmDownTime>(SQLQuery).AsQueryable().ToList();

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
