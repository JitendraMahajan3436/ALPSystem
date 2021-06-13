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
    public class MachineAlarmMasterController : BaseController
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        int plantId = 0, alarmId = 0, userId = 0, alarmTypeId = 0, machineId = 0;

        String alarmName = "";
        GlobalData globalData = new GlobalData();
        General generalObj = new General();
        General generalHelper = new General();
        // GET: /MachineAlarmMaster/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Machine_Alarm_Master;
            globalData.subTitle = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var mm_ctrl_machine_alarms_master = db.MM_Ctrl_Machine_Alarms_Master.Include(m => m.MM_Ctrl_Machine_Alarm_Types).Include(m => m.MM_MTTUW_Plants).Include(m => m.MM_MT_MTTUW_Machines);
            return View(mm_ctrl_machine_alarms_master.Where(m => m.Plant_ID == plantId).ToList());
        }

        // GET: /MachineAlarmMaster/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarms_Master mm_ctrl_machine_alarms_master = db.MM_Ctrl_Machine_Alarms_Master.Find(id);
            if (mm_ctrl_machine_alarms_master == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Machine_Alarm_Master;
            globalData.subTitle = ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarms_master);
        }

        // GET: /MachineAlarmMaster/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            userId = ((FDSession)this.Session["FDSession"]).userId;

            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", userId);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Alarm_Type_ID = new SelectList(db.MM_Ctrl_Machine_Alarm_Types.Where(p => p.Plant_ID == plantId), "Alarm_Type_ID", "Alarm_Type_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(a => a.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(a => a.Plant_ID == plantId).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name");
            ViewBag.AlarmIOList = new SelectList(db.MM_MT_Machine_IO, "MS_IO_ID", "IO_Name");
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")", Machine_ID = a.Machine_ID })
            //    , "Machine_ID", "Machine_Name");
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name");

            globalData.pageTitle = ResourceModules.Machine_Alarm_Master;
            globalData.subTitle = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        // POST: /MachineAlarmMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Alarm_ID,Machine_ID,Plant_ID,Alarm_Type_ID,Alarm_Name,Shop_ID,Alarm_Message,Is_MTTR,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Is_Major,AlarmIOList")] MM_Ctrl_Machine_Alarms_Master mm_ctrl_machine_alarms_master)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    plantId = Convert.ToInt16(mm_ctrl_machine_alarms_master.Plant_ID);
                    alarmTypeId = Convert.ToInt16(mm_ctrl_machine_alarms_master.Alarm_Type_ID);
                    machineId = Convert.ToInt32(mm_ctrl_machine_alarms_master.Machine_ID);
                    if (mm_ctrl_machine_alarms_master.isAlarmExists(mm_ctrl_machine_alarms_master.Alarm_Name, plantId, machineId))
                    {
                        ModelState.AddModelError("Alarm_Name", ResourceValidation.Exist);
                    }
                    else
                    {
                        mm_ctrl_machine_alarms_master.Inserted_Date = DateTime.Now;
                        mm_ctrl_machine_alarms_master.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.MM_Ctrl_Machine_Alarms_Master.Add(mm_ctrl_machine_alarms_master);
                        db.SaveChanges();

                        if (mm_ctrl_machine_alarms_master.AlarmIOList != null)
                        {
                            foreach (Int64 ioID in mm_ctrl_machine_alarms_master.AlarmIOList)
                            {
                                MM_MT_Machine_IO_Alarm_Relation alarmRelationObj = new MM_MT_Machine_IO_Alarm_Relation();
                                alarmRelationObj.Alarm_ID = mm_ctrl_machine_alarms_master.Alarm_ID;
                                alarmRelationObj.MS_IO_ID = ioID;
                                db.MM_MT_Machine_IO_Alarm_Relation.Add(alarmRelationObj);
                                db.SaveChanges();
                            }
                        }

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Machine_Alarm_Master;
                        globalData.messageDetail = ResourceGlobal.Alarm + " " + ResourceMessages.Add_Success;
                        TempData["globalData"] = globalData;
                        
                        ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(a => a.Plant_ID == plantId).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name",mm_ctrl_machine_alarms_master.Shop_ID);
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception exp)
            {
                generalObj.addControllerException(exp, "MachineAlarmMasterController", "Create(post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Machine_Alarm_Master;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceGlobal.Alarm + " " + ResourceGlobal.Create;
                this.Session["globalData"] = globalData;
            }
            ViewBag.Alarm_Type_ID = new SelectList(db.MM_Ctrl_Machine_Alarm_Types, "Alarm_Type_ID", "Alarm_Type_Name", mm_ctrl_machine_alarms_master.Alarm_Type_ID);
            ViewBag.Alarm_ID = new SelectList(db.MM_Ctrl_Machine_Alarms_Master, "Alarm_ID", "Alarm_Name", mm_ctrl_machine_alarms_master.Alarm_ID);
            //ViewBag.Alarm_ID = new SelectList(db.MM_Ctrl_Machine_Alarms_Master, "Alarm_ID", "Alarm_Name", mm_ctrl_machine_alarms_master.Alarm_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(a => a.Plant_ID == plantId), "Plant_ID", "Plant_Name", mm_ctrl_machine_alarms_master.Plant_ID);
            var ioList = mm_ctrl_machine_alarms_master.MM_MT_Machine_IO_Alarm_Relation.Select(a => a.MS_IO_ID);
            ViewBag.AlarmIOList = new MultiSelectList(db.MM_MT_Machine_IO, "MS_IO_ID", "IO_Name", ioList);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(a => a.Plant_ID == plantId).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", mm_ctrl_machine_alarms_master.Shop_ID);
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")", Machine_ID = a.Machine_ID }),
            //    "Machine_ID", "Machine_Name", mm_ctrl_machine_alarms_master.Machine_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", mm_ctrl_machine_alarms_master.Machine_ID);
            globalData.pageTitle = ResourceModules.Machine_Alarm_Master;
            globalData.subTitle = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarms_master);
        }

        // GET: /MachineAlarmMaster/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarms_Master mm_ctrl_machine_alarms_master = db.MM_Ctrl_Machine_Alarms_Master.Find(id);
            if (mm_ctrl_machine_alarms_master == null)
            {
                return HttpNotFound();
            }
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mm_ctrl_machine_alarms_master.Plant_ID);
            ViewBag.Alarm_Type_ID = new SelectList(db.MM_Ctrl_Machine_Alarm_Types, "Alarm_Type_ID", "Alarm_Type_Name", mm_ctrl_machine_alarms_master.Alarm_Type_ID);
            ViewBag.Alarm_ID = new SelectList(db.MM_Ctrl_Machine_Alarms_Master, "Alarm_ID", "Alarm_Name", mm_ctrl_machine_alarms_master.Alarm_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(a => a.Plant_ID == plantID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", mm_ctrl_machine_alarms_master.Shop_ID);
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")", Machine_ID = a.Machine_ID })
            //    , "Machine_ID", "Machine_Name",mm_ctrl_machine_alarms_master.Machine_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", mm_ctrl_machine_alarms_master.Machine_ID);
            var ioList = mm_ctrl_machine_alarms_master.MM_MT_Machine_IO_Alarm_Relation.Select(a => a.MS_IO_ID);
            ViewBag.AlarmIOList = new MultiSelectList(db.MM_MT_Machine_IO, "MS_IO_ID", "IO_Name", ioList);
            globalData.pageTitle = ResourceModules.Machine_Alarm_Master;
            globalData.subTitle = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarms_master);
        }

        // POST: /MachineAlarmMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Alarm_ID,Machine_ID,Plant_ID,Alarm_Type_ID,Shop_ID,Alarm_Name,Alarm_Message,Is_MTTR,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,AlarmIOList")] MM_Ctrl_Machine_Alarms_Master mm_ctrl_machine_alarms_master)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    plantId = Convert.ToInt16(mm_ctrl_machine_alarms_master.Plant_ID);
                    alarmTypeId = Convert.ToInt16(mm_ctrl_machine_alarms_master.Alarm_Type_ID);

                    alarmId = Convert.ToInt16(mm_ctrl_machine_alarms_master.Alarm_ID);
                    machineId = Convert.ToInt32(mm_ctrl_machine_alarms_master.Machine_ID);
                    if (mm_ctrl_machine_alarms_master.isAlarmExists(mm_ctrl_machine_alarms_master.Alarm_Name, plantId, machineId, alarmId))
                    {
                        ModelState.AddModelError("Alarm_Name", ResourceValidation.Exist);
                    }
                    else
                    {
                        List<MM_MT_Machine_IO_Alarm_Relation> machineIOAlarmReleationList = db.MM_MT_Machine_IO_Alarm_Relation.Where(a => a.Alarm_ID == mm_ctrl_machine_alarms_master.Alarm_ID).ToList();
                        if (machineIOAlarmReleationList.Count > 0)
                        {
                            db.MM_MT_Machine_IO_Alarm_Relation.RemoveRange(machineIOAlarmReleationList);
                            db.SaveChanges();
                        }
                        //mm_ctrl_machine_alarms_master.AlarmIOList.ToList().RemoveRange()
                        if (mm_ctrl_machine_alarms_master.AlarmIOList != null)
                        {
                            foreach (Int64 ioID in mm_ctrl_machine_alarms_master.AlarmIOList)
                            {
                                MM_MT_Machine_IO_Alarm_Relation alarmRelationObj = new MM_MT_Machine_IO_Alarm_Relation();
                                alarmRelationObj.Alarm_ID = mm_ctrl_machine_alarms_master.Alarm_ID;
                                alarmRelationObj.MS_IO_ID = ioID;
                                db.MM_MT_Machine_IO_Alarm_Relation.Add(alarmRelationObj);
                                db.SaveChanges();
                            }
                        }
                        mm_ctrl_machine_alarms_master.Shop_ID = mm_ctrl_machine_alarms_master.Shop_ID;
                        mm_ctrl_machine_alarms_master.Machine_ID = mm_ctrl_machine_alarms_master.Machine_ID;
                        mm_ctrl_machine_alarms_master.Updated_Date = DateTime.Now;
                        mm_ctrl_machine_alarms_master.Is_Edited = true;
                        mm_ctrl_machine_alarms_master.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.Entry(mm_ctrl_machine_alarms_master).State = EntityState.Modified;
                        db.SaveChanges();

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Machine_Alarm_Master;
                        globalData.messageDetail = ResourceGlobal.Alarm + " " + ResourceMessages.Edit_Success; ;
                        TempData["globalData"] = globalData;

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception exp)
            {
                generalObj.addControllerException(exp, "MachineAlarmMasterController", "Edit(post)", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Machine_Alarm_Master;
                globalData.messageDetail = ResourceValidation.Error_In + " " + ResourceGlobal.Alarm + " " + ResourceGlobal.Edit;
                this.Session["globalData"] = globalData;
            }
            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", mm_ctrl_machine_alarms_master.Machine_ID);
            //ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines
            //    .Select(a => new { Machine_Name = a.Machine_Name + "(" + a.Machine_Number + ")", Machine_ID = a.Machine_ID }),
            //    "Machine_ID", "Machine_Name", mm_ctrl_machine_alarms_master.Machine_ID);
            ViewBag.Alarm_Type_ID = new SelectList(db.MM_Ctrl_Machine_Alarm_Types, "Alarm_Type_ID", "Alarm_Type_Name", mm_ctrl_machine_alarms_master.Alarm_Type_ID);
            ViewBag.Alarm_ID = new SelectList(db.MM_Ctrl_Machine_Alarms_Master, "Alarm_ID", "Alarm_Name", mm_ctrl_machine_alarms_master.Alarm_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mm_ctrl_machine_alarms_master.Plant_ID);
            //ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(a => a.Plant_ID == plantID).OrderBy(a => a.Shop_Name), "Shop_ID", "Shop_Name", mm_ctrl_machine_alarms_master.Shop_ID);
            var ioList = mm_ctrl_machine_alarms_master.MM_MT_Machine_IO_Alarm_Relation.Select(a => a.MS_IO_ID);
            ViewBag.AlarmIOList = new MultiSelectList(db.MM_MT_Machine_IO, "MS_IO_ID", "IO_Name", ioList);

            globalData.pageTitle = ResourceModules.Machine_Alarm_Master;
            globalData.subTitle = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarms_master);
        }

        // GET: /MachineAlarmMaster/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarms_Master mm_ctrl_machine_alarms_master = db.MM_Ctrl_Machine_Alarms_Master.Find(id);
            if (mm_ctrl_machine_alarms_master == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle =  ResourceModules.Machine_Alarm_Master;
            globalData.subTitle = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;

            return View(mm_ctrl_machine_alarms_master);
        }

        // POST: /MachineAlarmMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_Ctrl_Machine_Alarms_Master mm_ctrl_machine_alarms_master = db.MM_Ctrl_Machine_Alarms_Master.Find(id);
            try
            {
                List<MM_Ctrl_Machine_Alarm_Data> alarmDataList = db.MM_Ctrl_Machine_Alarm_Data.Where(a => a.Alarm_ID == mm_ctrl_machine_alarms_master.Alarm_ID).ToList();
                if (alarmDataList.Count > 0)
                {
                    db.MM_Ctrl_Machine_Alarm_Data.RemoveRange(alarmDataList);
                    db.SaveChanges();
                }
                List<MM_MT_Machine_IO_Alarm_Relation> machineIOAlarmReleationList = db.MM_MT_Machine_IO_Alarm_Relation.Where(a => a.Alarm_ID == mm_ctrl_machine_alarms_master.Alarm_ID).ToList();
                if (machineIOAlarmReleationList.Count > 0)
                {
                    db.MM_MT_Machine_IO_Alarm_Relation.RemoveRange(machineIOAlarmReleationList);
                    db.SaveChanges();
                }
                db.MM_Ctrl_Machine_Alarms_Master.Remove(mm_ctrl_machine_alarms_master);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_Ctrl_Machine_Alarms_Master", "Alarm_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle =  ResourceModules.Machine_Alarm_Master;
                globalData.messageDetail = ResourceGlobal.Alarm + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle =  ResourceModules.Machine_Alarm_Master;
                globalData.messageDetail = ex.InnerException.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete", mm_ctrl_machine_alarms_master);
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

        public ActionResult getMachineList(decimal shopId)
        {
            try
            {
                var machineList = (from machine in db.MM_MT_MTTUW_Machines
                                   where machine.Shop_ID == shopId /*&& machine.IsActive == true*/
                                   select new
                                   {
                                       Id = machine.Machine_ID,
                                       Value = machine.Machine_Name
                                   }).Distinct();
                
                return Json(machineList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppageCycleController", "getMachineList(shopID : " + shopId + " )", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }
    }
}
