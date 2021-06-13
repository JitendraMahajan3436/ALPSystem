using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class DailyMISController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        MM_Daily_MIS mm_daily_Mes = new MM_Daily_MIS();
        // GET: DailyMIS
        public ActionResult Index()
        {
            var shopID=((FDSession)this.Session["FDSession"]).shopId;
            var mM_Daily_MIS = db.MM_Daily_MIS.Where(x => x.Shop_ID == shopID).Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Where(x => x.Inserted_Date.Year == DateTime.Now.Year && x.Inserted_Date.Month == DateTime.Now.Month && x.Inserted_Date.Day == DateTime.Now.Day);
            
            return View(mM_Daily_MIS.ToList());
        }

        // GET: DailyMIS/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Daily_MIS mM_Daily_MIS = db.MM_Daily_MIS.Find(id);
            if (mM_Daily_MIS == null)
            {
                return HttpNotFound();
            }
            return View(mM_Daily_MIS);
        }

        // GET: DailyMIS/Create
        public ActionResult Create()
        {
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            return View();
        }

        // POST: DailyMIS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Row_ID,Plant_ID,Shop_ID,Log_Date,From_Time,To_Time,Total_Time,Owner,Reason,Remark,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Is_Active,Is_Deleted")] MM_Daily_MIS mM_Daily_MIS)
        {
            if (ModelState.IsValid)
            {
                mM_Daily_MIS.Inserted_Date = DateTime.Now;
                mM_Daily_MIS.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_Daily_MIS.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                mM_Daily_MIS.Shop_ID = ((FDSession)this.Session["FDSession"]).shopId;
               mM_Daily_MIS.Log_Date = DateTime.Now;
          
                db.MM_Daily_MIS.Add(mM_Daily_MIS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Daily_MIS.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Daily_MIS.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Daily_MIS.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_Daily_MIS.Shop_ID);
            return View(mM_Daily_MIS);
        }

        // GET: DailyMIS/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Daily_MIS mM_Daily_MIS = db.MM_Daily_MIS.Find(id);
            if (mM_Daily_MIS == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Daily_MIS.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Daily_MIS.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Daily_MIS.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_Daily_MIS.Shop_ID);
            return View(mM_Daily_MIS);
        }

        // POST: DailyMIS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Row_ID,Plant_ID,Shop_ID,Log_Date,From_Time,To_Time,Total_Time,Owner,Reason,Remark,Is_Transfered,Is_Purgeable,Is_Edited,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Is_Active,Is_Deleted")] MM_Daily_MIS mM_Daily_MIS)
        {
            if (ModelState.IsValid)
            {
                mm_daily_Mes = db.MM_Daily_MIS.Find(mM_Daily_MIS.Row_ID);
                mm_daily_Mes.Updated_Date = DateTime.Now;
                mm_daily_Mes.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                //mM_Daily_MIS.Inserted_Date = DateTime.Now;
                //mM_Daily_MIS.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mm_daily_Mes.Log_Date = DateTime.Now;
                mm_daily_Mes.From_Time = mM_Daily_MIS.From_Time;
                mm_daily_Mes.To_Time = mM_Daily_MIS.To_Time;
                mm_daily_Mes.Total_Time = mM_Daily_MIS.Total_Time;
                mm_daily_Mes.Owner = mM_Daily_MIS.Owner;
                mm_daily_Mes.Reason = mM_Daily_MIS.Reason;
                mm_daily_Mes.Remark = mM_Daily_MIS.Remark;
                mm_daily_Mes.Is_Edited = true;
                //mm_daily_Mes.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                // mM_Daily_MIS.Shop_ID = ((FDSession)this.Session["FDSession"]).shopId;
                db.Entry(mm_daily_Mes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Daily_MIS.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_Daily_MIS.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Daily_MIS.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_Daily_MIS.Shop_ID);
            return View(mM_Daily_MIS);
        }

        // GET: DailyMIS/Delete/5
        //public ActionResult Delete(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MM_Daily_MIS mM_Daily_MIS = db.MM_Daily_MIS.Find(id);
        //    if (mM_Daily_MIS == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mM_Daily_MIS);
        //}

        //// POST: DailyMIS/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(decimal id)
        //{
        //    MM_Daily_MIS mM_Daily_MIS = db.MM_Daily_MIS.Find(id);
        //    db.MM_Daily_MIS.Remove(mM_Daily_MIS);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public JsonResult isTimeValid(TimeSpan? From_Time, TimeSpan? To_Time)
        {
            if (From_Time != null && To_Time != null)
            {
                if (From_Time >= To_Time)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else 
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    
    
    
    
    }

}
