using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using System.Data.Entity.SqlServer;
using ZHB_AD.Controllers.BaseManagement;

namespace ZHB_AD.Controllers
{
    public class HistoryLineStopController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        private MM_History_LineStop hislin;

        public ActionResult Index()
        {
            var shopID = ((FDSession)this.Session["FDSession"]).shopId;
            var lineID = new SelectList(db.MM_Lines.Where(a => a.Shop_ID == shopID && a.isPLC == true && a.Is_Conveyor == true).OrderBy(a => a.Line_Name), "Line_ID", "Line_Name");
            ViewBag.Line_ID = lineID;
            return View();

        }

        public ActionResult getdata(decimal lineid)
        {
            var shopID = ((FDSession)this.Session["FDSession"]).shopId;
            MM_Shift shiftObj = db.MM_Shift.Where(a => a.Shop_ID == shopID && a.Lunch_Time != null && a.Lunch_End_Time != null).FirstOrDefault();
            var todayDate = DateTime.Today;
            TimeSpan? startTime = shiftObj.Shift_Start_Time;
            TimeSpan? endTime = shiftObj.Shift_End_Time;
            TimeSpan? lunchTimeStart = shiftObj.Lunch_Time;
            TimeSpan? lunchTimeEnd = shiftObj.Lunch_End_Time;
            TimeSpan? breakTimeStart = shiftObj.Break1_Time;
            TimeSpan? breakTimeEnd = shiftObj.Break1_End_Time;

            var mM_History_LineStop = db.MM_History_LineStop.Where(x => x.Shop_ID == shopID && x.isEmergencyCall == true && x.Line_ID == lineid
                     && (x.Stop_Time >= DbFunctions.AddMilliseconds(DbFunctions.TruncateTime(x.Stop_Time), DbFunctions.DiffMilliseconds(TimeSpan.Zero, startTime))
                     && x.Stop_Time < DbFunctions.AddMilliseconds(DbFunctions.TruncateTime(x.Stop_Time), DbFunctions.DiffMilliseconds(TimeSpan.Zero, endTime)))
                     && !(x.Stop_Time >= DbFunctions.AddMilliseconds(DbFunctions.TruncateTime(x.Stop_Time), DbFunctions.DiffMilliseconds(TimeSpan.Zero, lunchTimeStart))
                          && x.Stop_Time <= DbFunctions.AddMilliseconds(DbFunctions.TruncateTime(x.Stop_Time), DbFunctions.DiffMilliseconds(TimeSpan.Zero, lunchTimeEnd)))
                     && !(x.Stop_Time >= DbFunctions.AddMilliseconds(DbFunctions.TruncateTime(x.Resume_Time), DbFunctions.DiffMilliseconds(TimeSpan.Zero, breakTimeStart))
                          && x.Stop_Time <= DbFunctions.AddMilliseconds(DbFunctions.TruncateTime(x.Resume_Time), DbFunctions.DiffMilliseconds(TimeSpan.Zero, breakTimeEnd)))
                     && DbFunctions.TruncateTime(x.Inserted_Date) == DateTime.Today);

            //var Line_Stoppage_Reason = new SelectList(db.MM_LineStoppage_Reasons, "Reason_id", "Reason");
            //ViewBag.Stop_Reason = Line_Stoppage_Reason;
            return PartialView(mM_History_LineStop);
        }

        //public ActionResult GetSecOwnerData(int primaryOwner_id)
        //{
        //    var secondaryOwner_Data = db.MM_HL_Secondary_Owner
        //        .Where(c => c.PrimaryOwner_Id == primaryOwner_id)
        //        .Select(c => new { c.Secondary_Owner })
        //        .Distinct()
        //        .OrderBy(c => c.Secondary_Owner);
        //    return Json(secondaryOwner_Data, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult GetSecOwnerData(int primaryOwner_id)
        {
            try
            {
                var st = from secondaryowner in db.MM_HL_Secondary_Owner
                         where secondaryowner.PrimaryOwner_id == primaryOwner_id
                         orderby secondaryowner.Secondary_Owner
                         select new
                         {
                             Id = secondaryowner.SecondaryOwner_id,
                             Value = secondaryowner.Secondary_Owner,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }



        // GET: HistoryLineStop/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_History_LineStop mM_History_LineStop = db.MM_History_LineStop.Find(id);
            if (mM_History_LineStop == null)
            {
                return HttpNotFound();
            }
            ViewBag.EFPartGroup_ID = new SelectList(db.MM_Partgroup, "Partgroup_ID", "Partgrup_Desc", mM_History_LineStop.EFPartGroup_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_History_LineStop.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_History_LineStop.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_History_LineStop.Shop_ID);
            ViewBag.Stop_Reason = new SelectList(db.MM_LineStoppage_Reasons, "Reason", "Reason", mM_History_LineStop.Stop_Reason);
            ViewBag.PrimaryOwner_Id = new SelectList(db.MM_HL_Primary_Owner, "PrimaryOwner_Id", "Primary_Owner", mM_History_LineStop.PrimaryOwner_Id);
            //ViewBag.Secondary_Owner = new SelectList(db.MM_HL_Secondary_Owner, "SecondaryOwner_Id", "Secondary_Owner", mM_History_LineStop.Secondary_Owner);
            ViewBag.SecondaryOwner_Id = new SelectList(db.MM_HL_Secondary_Owner, "SecondaryOwner_Id", "Secondary_Owner", mM_History_LineStop.SecondaryOwner_Id);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_History_LineStop.Station_ID);
            ViewBag.Total_Time = mM_History_LineStop.Resume_Time - mM_History_LineStop.Stop_Time;
            return View(mM_History_LineStop);
        }

        // POST: HistoryLineStop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Row_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Stop_Time,Stop_Reason,PrimaryOwner_Id,Line_Stop_By,SecondaryOwner_Id,Status,Resume_Time,Remarks,isLineStop,isEmergencyCall,isHeartBit,EFPartGroup_ID,PLC_Ack,Inserted_Date,Is_Transfered,Is_Purgeable,Is_Edited")] MM_History_LineStop mM_History_LineStop)
        {
            if (ModelState.IsValid)
            {
                hislin = new MM_History_LineStop();
                hislin = db.MM_History_LineStop.Find(mM_History_LineStop.Row_ID);
               // hislin.Primary_Owner = mM_History_LineStop.Primary_Owner;
                hislin.PrimaryOwner_Id = mM_History_LineStop.PrimaryOwner_Id;
                hislin.SecondaryOwner_Id = mM_History_LineStop.SecondaryOwner_Id;
                hislin.Line_Stop_By = mM_History_LineStop.Line_Stop_By;
                hislin.Status = mM_History_LineStop.Status;
                hislin.Stop_Reason = mM_History_LineStop.Stop_Reason;
                hislin.Remarks = mM_History_LineStop.Remarks;
                hislin.Is_Edited = true;
                db.Entry(hislin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //var primaryOwner = new SelectList(db.MM_HL_Primary_Owner, "PrimaryOwner_Id", "Primary_Owner");
            ViewBag.EFPartGroup_ID = new SelectList(db.MM_Partgroup, "Partgroup_ID", "Partgrup_Desc", mM_History_LineStop.EFPartGroup_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_History_LineStop.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_History_LineStop.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_History_LineStop.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_History_LineStop.Station_ID);
            ViewBag.Stop_Reason = new SelectList(db.MM_LineStoppage_Reasons, "Reason", "Reason", mM_History_LineStop.Stop_Reason);
            ViewBag.PrimaryOwner_Id = new SelectList(db.MM_HL_Primary_Owner, "PrimaryOwner_Id", "Primary_Owner", mM_History_LineStop.PrimaryOwner_Id);
            //ViewBag.Secondary_Owner = new SelectList(db.MM_HL_Secondary_Owner, "SecondaryOwner_Id", "Secondary_Owner", mM_History_LineStop.Secondary_Owner);
            ViewBag.SecondaryOwner_Id = new SelectList(db.MM_HL_Secondary_Owner.OrderBy(c => c.Secondary_Owner), "SecondaryOwner_Id", "Secondary_Owner", mM_History_LineStop.SecondaryOwner_Id);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_History_LineStop.Station_ID);
            ViewBag.Total_Time = mM_History_LineStop.Resume_Time - mM_History_LineStop.Stop_Time;
            return View(mM_History_LineStop);
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
