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
using ZHB_AD.App_LocalResources;
using ZHB_AD.Controllers.BaseManagement;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class BreakDownTargetController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        // GET: /BreakDownTarget/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.BreakDownTarget;
            globalData.subTitle = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var mm_mt_breakdown_target = db.MM_MT_BreakDown_Target.Include(m => m.MM_Shops);
            return View(mm_mt_breakdown_target.ToList());
        }

        // GET: /BreakDownTarget/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_BreakDown_Target mm_mt_breakdown_target = db.MM_MT_BreakDown_Target.Find(id);
            if (mm_mt_breakdown_target == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.BreakDownTarget;
            globalData.subTitle = ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            return View(mm_mt_breakdown_target);
        }

        // GET: /BreakDownTarget/Create
        public ActionResult Create()
        {
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.MM_Plants.Find(plantID);
            globalData.pageTitle = ResourceModules.BreakDownTarget;
            globalData.subTitle = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            return View();
        }
        //
        public class months
        {
            public decimal? monthNo { get; set; }
            public string MonthName { get; set; }
        }
        public ActionResult FillMonths(string year, int shopid, string module, string valuetype, string type)
        {
            var result = db.MM_MT_BreakDown_Target.Where(mm => mm.Year == year && mm.Shop_ID == shopid && mm.Module == module && mm.Value_Type == valuetype && mm.Type == type).Select(mm => new months { monthNo = mm.Month }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: /BreakDownTarget/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_MT_BreakDown_Target mm_BDtarget)
        {

            string duplicateMonths = "";
            int validCnt = 0;
            int invalidCount = 0;
            if (ModelState.IsValid)
            {
                foreach (string months in mm_BDtarget.Months)
                {

                    if (!String.IsNullOrWhiteSpace(months))
                    {
                        decimal monthno = Convert.ToDecimal(months);

                        if (!(db.MM_MT_BreakDown_Target.Any(a => a.Shop_ID == mm_BDtarget.Shop_ID && a.Month == monthno && a.Type == mm_BDtarget.Type && a.Value_Type == mm_BDtarget.Value_Type && a.Year == mm_BDtarget.Year)))
                        {
                            validCnt++;
                            mm_BDtarget.Month = monthno;
                            mm_BDtarget.Inserted_Date = DateTime.Now;
                            mm_BDtarget.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mm_BDtarget.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            db.MM_MT_BreakDown_Target.Add(mm_BDtarget);
                            db.SaveChanges();
                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceModules.BreakDownTarget;
                            globalData.messageDetail = ResourceModules.BreakDownTarget + " " + ResourceMessages.Add_Success;
                            TempData["globalData"] = globalData;

                        }
                        else
                        {
                            invalidCount++;
                            duplicateMonths += months + ",";
                        }
                    }
                }

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.BreakDownTarget;
                globalData.messageDetail = ResourceModules.BreakDownTarget + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var zb_plantObj = db.MM_Plants.Find(plantID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants.Where(a => a.Plant_ID == plantID), "Plant_ID", "Plant_Name", zb_plantObj.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_BDtarget.Shop_ID);
            globalData.pageTitle = ResourceModules.BreakDownTarget;
            globalData.subTitle = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            return View(mm_BDtarget);
        }
        public class monthList
        {
            public int? MonthNo { get; set; }
            public string MonthName { get; set; }
            public bool Checked { get; set; }
        }

        public ActionResult getMonthCheckBoxList()
        {
            List<monthList> modelList = new List<monthList>();
            IEnumerable<MM_Month> monthObj = db.MM_Month.Where(a => a.Identifier_ID == 1).OrderBy(a => a.Month_No);
            foreach (MM_Month obj in monthObj)
            {
                modelList.Add(new monthList { MonthNo = obj.Month_No, MonthName = obj.Month_Name });
            }
            return Json(modelList, JsonRequestBehavior.AllowGet);
        }

        // GET: /BreakDownTarget/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_BreakDown_Target mm_mt_breakdown_target = db.MM_MT_BreakDown_Target.Find(id);
            if (mm_mt_breakdown_target == null)
            {
                return HttpNotFound();
            }
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_mt_breakdown_target.Shop_ID);
            globalData.pageTitle = ResourceModules.BreakDownTarget;
            globalData.subTitle = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            return View(mm_mt_breakdown_target);
        }

        // POST: /BreakDownTarget/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Target_ID,Type,Plant_ID,Shop_ID,Year,Month,Value_Type,Module,Target_Value,Is_Transferred,Is_Purgeable,Is_Edited,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_MT_BreakDown_Target mm_mt_breakdown_target)
        {
            if (ModelState.IsValid)
            {
                mm_mt_breakdown_target.Updated_Date = DateTime.Now;
                mm_mt_breakdown_target.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mm_mt_breakdown_target.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                mm_mt_breakdown_target.Is_Edited = true;
                db.Entry(mm_mt_breakdown_target).State = EntityState.Modified;
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.BreakDownTarget;
                globalData.messageDetail = ResourceModules.BreakDownTarget + " " + ResourceMessages.Edit_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_mt_breakdown_target.Shop_ID);
            globalData.pageTitle = ResourceModules.BreakDownTarget;
            globalData.subTitle = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            return View(mm_mt_breakdown_target);
        }

        // GET: /BreakDownTarget/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_BreakDown_Target mm_mt_breakdown_target = db.MM_MT_BreakDown_Target.Find(id);
            if (mm_mt_breakdown_target == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.BreakDownTarget;
            globalData.subTitle = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            return View(mm_mt_breakdown_target);
        }

        // POST: /BreakDownTarget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_MT_BreakDown_Target mm_mt_breakdown_target = db.MM_MT_BreakDown_Target.Find(id);
            db.MM_MT_BreakDown_Target.Remove(mm_mt_breakdown_target);
            db.SaveChanges();
            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.BreakDownTarget;
            globalData.messageDetail = ResourceModules.BreakDownTarget + " " + ResourceMessages.Delete_Success;
            TempData["globalData"] = globalData;
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
