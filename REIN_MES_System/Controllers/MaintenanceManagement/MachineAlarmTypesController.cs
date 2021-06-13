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
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class MachineAlarmTypesController : BaseController
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        int plantId = 0, alarmTypeId = 0, userId = 0;

        String alarmTypeName = "";
        GlobalData globalData = new GlobalData();
        MM_Ctrl_Machine_Alarm_Types mmMachineAlarmTypes = new MM_Ctrl_Machine_Alarm_Types();

        General generalObj = new General();

        // GET: /MachineAlarmTypes/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Machine_Alarm_Type;
            globalData.subTitle = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            var mm_ctrl_machine_alarm_types = db.MM_Ctrl_Machine_Alarm_Types.Include(m => m.MM_MTTUW_Employee).Include(m => m.MM_MTTUW_Employee1).Include(m => m.MM_MTTUW_Plants);
            return View(mm_ctrl_machine_alarm_types.ToList());
        }

        // GET: /MachineAlarmTypes/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarm_Types mm_ctrl_machine_alarm_types = db.MM_Ctrl_Machine_Alarm_Types.Find(id);
            if (mm_ctrl_machine_alarm_types == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Machine_Alarm_Type;
            globalData.subTitle = ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarm_types);
        }

        // GET: /MachineAlarmTypes/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            userId = ((FDSession)this.Session["FDSession"]).userId;

            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", userId);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", plantId);

            globalData.pageTitle = ResourceModules.Machine_Alarm_Type;
            globalData.subTitle = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        // POST: /MachineAlarmTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Alarm_Type_ID,Alarm_Type_Name,Plant_ID,Fault_Background_Color,Fault_Forground_Color,Healthy_Background_Color,Healthy_Forground_Color,Inserted_Host,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_Ctrl_Machine_Alarm_Types mm_ctrl_machine_alarm_types)
        {
            if (ModelState.IsValid)
            {
                plantId = Convert.ToInt16(mm_ctrl_machine_alarm_types.Plant_ID);
                if (mm_ctrl_machine_alarm_types.isAlarmTypeExists(mm_ctrl_machine_alarm_types.Alarm_Type_Name, plantId))
                {
                    ModelState.AddModelError("Alarm_Type_Name", ResourceValidation.Exist);
                }
                else
                {
                    mm_ctrl_machine_alarm_types.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mm_ctrl_machine_alarm_types.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mm_ctrl_machine_alarm_types.Inserted_Date = DateTime.Now;
                    db.MM_Ctrl_Machine_Alarm_Types.Add(mm_ctrl_machine_alarm_types);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Machine_Alarm_Type;
                    globalData.messageDetail = ResourceModules.Machine_Alarm_Type+" "+ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            userId = ((FDSession)this.Session["FDSession"]).userId;

            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarm_types.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarm_types.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mm_ctrl_machine_alarm_types.Plant_ID);

            globalData.pageTitle = ResourceModules.Machine_Alarm_Type;
            globalData.subTitle = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            return View(mm_ctrl_machine_alarm_types);
        }

        // GET: /MachineAlarmTypes/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarm_Types mm_ctrl_machine_alarm_types = db.MM_Ctrl_Machine_Alarm_Types.Find(id);
            if (mm_ctrl_machine_alarm_types == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarm_types.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarm_types.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mm_ctrl_machine_alarm_types.Plant_ID);

            globalData.pageTitle = ResourceModules.Machine_Alarm_Type;
            globalData.subTitle = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarm_types);
        }

        // POST: /MachineAlarmTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Alarm_Type_ID,Alarm_Type_Name,Plant_ID,Fault_Background_Color,Fault_Forground_Color,Healthy_Background_Color,Healthy_Forground_Color,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_Ctrl_Machine_Alarm_Types mm_ctrl_machine_alarm_types)
        {
            if (ModelState.IsValid)
            {
                plantId = Convert.ToInt16(mm_ctrl_machine_alarm_types.Plant_ID);
                alarmTypeId = Convert.ToInt16(mm_ctrl_machine_alarm_types.Alarm_Type_ID);
                if (mm_ctrl_machine_alarm_types.isAlarmTypeExists(mm_ctrl_machine_alarm_types.Alarm_Type_Name, plantId, alarmTypeId))
                {
                    ModelState.AddModelError("Alarm_Type_Name", ResourceValidation.Exist);
                }
                else
                {
                    mm_ctrl_machine_alarm_types.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mm_ctrl_machine_alarm_types.Updated_Date = DateTime.Now;
                    mm_ctrl_machine_alarm_types.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mm_ctrl_machine_alarm_types.Is_Edited = true;
                    db.Entry(mm_ctrl_machine_alarm_types).State = EntityState.Modified;
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Machine_Alarm_Type;
                    globalData.messageDetail = ResourceModules.Machine_Alarm_Type+" "+ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarm_types.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarm_types.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mm_ctrl_machine_alarm_types.Plant_ID);

            globalData.pageTitle = ResourceModules.Machine_Alarm_Type;
            globalData.subTitle = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarm_types);
        }

        // GET: /MachineAlarmTypes/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarm_Types mm_ctrl_machine_alarm_types = db.MM_Ctrl_Machine_Alarm_Types.Find(id);
            if (mm_ctrl_machine_alarm_types == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Machine_Alarm_Type;
            globalData.subTitle = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarm_types);
        }

        // POST: /MachineAlarmTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_Ctrl_Machine_Alarm_Types mm_ctrl_machine_alarm_types = db.MM_Ctrl_Machine_Alarm_Types.Find(id);
            try
            {


                db.MM_Ctrl_Machine_Alarm_Types.Remove(mm_ctrl_machine_alarm_types);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_Ctrl_Machine_Alarm_Types", "Alarm_Type_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Machine_Alarm_Type;
                globalData.messageDetail = ResourceModules.Machine_Alarm_Type+" "+ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Machine_Alarm_Type;
                globalData.messageDetail = ex.InnerException.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", mm_ctrl_machine_alarm_types);
            }
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
