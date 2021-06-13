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
using System.Data.OleDb;
using ZHB_AD.App_LocalResources;
using System.IO;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class MachineAlarmsController : BaseController
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0, lineId = 0, stationId = 0, alarmId = 0, machineId = 0;
        MM_Ctrl_Machine_Alarms machineAlarmObj = new MM_Ctrl_Machine_Alarms();

        General generalObj = new General();

        // GET: /MachineAlarms/
        public ActionResult Index()
        {
            TempData["globalData"] = null;
            var mm_ctrl_machine_alarms = db.MM_Ctrl_Machine_Alarms.Include(m => m.MM_Ctrl_Machine_Alarms_Master).Include(m => m.MM_MTTUW_Employee).Include(m => m.MM_MTTUW_Employee1).Include(m => m.MM_MTTUW_Lines).Include(m => m.MM_MT_MTTUW_Machines).Include(m => m.MM_MTTUW_Plants).Include(m => m.MM_MTTUW_Shops).Include(m => m.MM_MTTUW_Stations);
            return View(mm_ctrl_machine_alarms.ToList());
        }

        // GET: /MachineAlarms/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarms mm_ctrl_machine_alarms = db.MM_Ctrl_Machine_Alarms.Find(id);
            if (mm_ctrl_machine_alarms == null)
            {
                return HttpNotFound();
            }
            return View(mm_ctrl_machine_alarms);
        }

        // GET: /MachineAlarms/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                plantId = Convert.ToInt16(TempData["plantId"]);
                shopId = Convert.ToInt16(TempData["shopId"]);
                lineId = Convert.ToInt16(TempData["lineId"]);
                stationId = Convert.ToInt16(TempData["stationId"]);

                machineId = Convert.ToInt16(TempData["machineId"]);

                TempData["globalData"] = null;


                var notSelectedAlarms = from alarmsObj in db.MM_Ctrl_Machine_Alarms_Master
                                        where !(from machinesAlarmsObj in db.MM_Ctrl_Machine_Alarms where machinesAlarmsObj.Machine_ID == machineId select machinesAlarmsObj.Alarm_ID).Contains(alarmsObj.Alarm_ID)

                                        select new
                                        {
                                            Alarm_ID = alarmsObj.Alarm_ID,
                                            Alarm_Name = alarmsObj.Alarm_Name
                                        };
                ViewBag.Alarm_ID = new SelectList(notSelectedAlarms, "Alarm_ID", "Alarm_Name");


                var selectedAlarms = from alarmsObj in db.MM_Ctrl_Machine_Alarms_Master
                                     where (from machinesAlarmsObj in db.MM_Ctrl_Machine_Alarms where machinesAlarmsObj.Machine_ID == machineId select machinesAlarmsObj.Alarm_ID).Contains(alarmsObj.Alarm_ID)
                                     select new
                                     {
                                         Alarm_ID = alarmsObj.Alarm_ID,
                                         Alarm_Name = alarmsObj.Alarm_Name
                                     };
                ViewBag.selectedAlarms = new SelectList(selectedAlarms, "Alarm_ID", "Alarm_Name");



                ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines.Where(p => p.Shop_ID == shopId), "Line_ID", "Line_Name", lineId);
                ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(p => p.Station_ID == stationId), "Machine_ID", "Machine_Name", machineId);
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", shopId);
                ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations.Where(p => p.Line_ID == lineId), "Station_ID", "Station_Name", stationId);

                globalData = (GlobalData)TempData["globalData"];
            }
            else
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Alarm_ID = new SelectList(db.MM_Ctrl_Machine_Alarms_Master.Where(p => p.Plant_ID == 0), "Alarm_ID", "Alarm_Name");
                ViewBag.selectedAlarms = new SelectList(db.MM_Ctrl_Machine_Alarms_Master.Where(p => p.Plant_ID == 0), "Alarm_ID", "Alarm_Name");
                ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines.Where(p => p.Shop_ID == shopId), "Line_ID", "Line_Name");
                ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(p => p.Station_ID == stationId), "Machine_ID", "Machine_Name");
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations.Where(p => p.Line_ID == lineId), "Station_ID", "Station_Name");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", plantId);

            return View();
        }

        // POST: /MachineAlarms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Machine_Alarm_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Machine_ID,Alarm_ID,Status,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,selectedAlarms")] MM_Ctrl_Machine_Alarms mm_ctrl_machine_alarms)
        {
            if (ModelState.IsValid)
            {
                plantId = Convert.ToInt16(mm_ctrl_machine_alarms.Plant_ID);
                shopId = Convert.ToInt16(mm_ctrl_machine_alarms.Shop_ID);
                lineId = Convert.ToInt16(mm_ctrl_machine_alarms.Line_ID);
                stationId = Convert.ToInt16(mm_ctrl_machine_alarms.Station_ID);
                machineId = Convert.ToInt16(mm_ctrl_machine_alarms.Machine_ID);

                if (mm_ctrl_machine_alarms.selectedAlarms.Count() > 0)
                {


                    for (int i = 0; i < mm_ctrl_machine_alarms.selectedAlarms.Count(); i++)
                    {
                        alarmId = Convert.ToInt16(mm_ctrl_machine_alarms.selectedAlarms[i]);
                        if (alarmId == 0)
                        {
                            continue;
                        }

                        if (!mm_ctrl_machine_alarms.isAlarmAddedForMachine(alarmId, machineId))
                        {
                            // process to add record in database
                            mm_ctrl_machine_alarms.Alarm_ID = alarmId;
                            mm_ctrl_machine_alarms.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                            mm_ctrl_machine_alarms.Inserted_Date = DateTime.Now;
                            mm_ctrl_machine_alarms.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            db.MM_Ctrl_Machine_Alarms.Add(mm_ctrl_machine_alarms);
                            db.SaveChanges();
                        }


                    }


                    // process to remove the not selected alarms for the machine
                    mm_ctrl_machine_alarms.removeAddedAlarmsByMachine(mm_ctrl_machine_alarms.selectedAlarms, machineId, ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                }



                TempData["globalData"] = globalData;
                TempData["plantId"] = plantId;
                TempData["shopId"] = shopId;
                TempData["lineId"] = lineId;
                TempData["stationId"] = stationId;
                TempData["machineId"] = machineId;

                globalData.isSuccessMessage = true;
                globalData.messageTitle = "Machine Alarms";
                globalData.messageDetail = "Machine Alarm is added successufully.";
                TempData["globalData"] = globalData;
                return RedirectToAction("Create");
            }


            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            machineId = Convert.ToInt16(mm_ctrl_machine_alarms.Machine_ID);
            if (machineId == 0)
            {
                ViewBag.Alarm_ID = new SelectList(db.MM_Ctrl_Machine_Alarms_Master.Where(p => p.Plant_ID == plantId), "Alarm_ID", "Alarm_Name", mm_ctrl_machine_alarms.Alarm_ID);
                ViewBag.selectedAlarms = new SelectList(db.MM_Ctrl_Machine_Alarms_Master.Where(p => p.Plant_ID == 0), "Alarm_ID", "Alarm_Name", mm_ctrl_machine_alarms.Alarm_ID);
            }
            else
            {
                var notSelectedAlarms = from alarmsObj in db.MM_Ctrl_Machine_Alarms_Master
                                        where !(from machinesAlarmsObj in db.MM_Ctrl_Machine_Alarms where machinesAlarmsObj.Machine_ID == machineId select machinesAlarmsObj.Alarm_ID).Contains(alarmsObj.Alarm_ID)

                                        select new
                                        {
                                            Alarm_ID = alarmsObj.Alarm_ID,
                                            Alarm_Name = alarmsObj.Alarm_Name
                                        };
                ViewBag.Alarm_ID = new SelectList(notSelectedAlarms, "Alarm_ID", "Alarm_Name");


                var selectedAlarms = from alarmsObj in db.MM_Ctrl_Machine_Alarms_Master
                                     where (from machinesAlarmsObj in db.MM_Ctrl_Machine_Alarms where machinesAlarmsObj.Machine_ID == machineId select machinesAlarmsObj.Alarm_ID).Contains(alarmsObj.Alarm_ID)
                                     select new
                                     {
                                         Alarm_ID = alarmsObj.Alarm_ID,
                                         Alarm_Name = alarmsObj.Alarm_Name
                                     };
                ViewBag.selectedAlarms = new SelectList(selectedAlarms, "Alarm_ID", "Alarm_Name");
            }


            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarms.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarms.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines.Where(p => p.Shop_ID == mm_ctrl_machine_alarms.Shop_ID), "Line_ID", "Line_Name", mm_ctrl_machine_alarms.Line_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines.Where(p => p.Station_ID == mm_ctrl_machine_alarms.Station_ID), "Machine_ID", "Machine_Name", mm_ctrl_machine_alarms.Machine_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mm_ctrl_machine_alarms.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(p => p.Plant_ID == mm_ctrl_machine_alarms.Plant_ID), "Shop_ID", "Shop_Name", mm_ctrl_machine_alarms.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations.Where(p => p.Line_ID == mm_ctrl_machine_alarms.Line_ID), "Station_ID", "Station_Name", mm_ctrl_machine_alarms.Station_ID);
            return View(mm_ctrl_machine_alarms);
        }

        // GET: /MachineAlarms/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarms mm_ctrl_machine_alarms = db.MM_Ctrl_Machine_Alarms.Find(id);
            if (mm_ctrl_machine_alarms == null)
            {
                return HttpNotFound();
            }
            ViewBag.Alarm_ID = new SelectList(db.MM_Ctrl_Machine_Alarms_Master, "Alarm_ID", "Alarm_Name", mm_ctrl_machine_alarms.Alarm_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarms.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarms.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mm_ctrl_machine_alarms.Line_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Number", mm_ctrl_machine_alarms.Machine_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mm_ctrl_machine_alarms.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name", mm_ctrl_machine_alarms.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mm_ctrl_machine_alarms.Station_ID);
            return View(mm_ctrl_machine_alarms);
        }

        // POST: /MachineAlarms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Machine_Alarm_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Machine_ID,Alarm_ID,Status,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_Ctrl_Machine_Alarms mm_ctrl_machine_alarms)
        {
            if (ModelState.IsValid)
            {
                mm_ctrl_machine_alarms.Is_Edited = true;
                db.Entry(mm_ctrl_machine_alarms).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Alarm_ID = new SelectList(db.MM_Ctrl_Machine_Alarms_Master, "Alarm_ID", "Alarm_Name", mm_ctrl_machine_alarms.Alarm_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarms.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_MTTUW_Employee, "Employee_ID", "Employee_Name", mm_ctrl_machine_alarms.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", mm_ctrl_machine_alarms.Line_ID);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Number", mm_ctrl_machine_alarms.Machine_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name", mm_ctrl_machine_alarms.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name", mm_ctrl_machine_alarms.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", mm_ctrl_machine_alarms.Station_ID);
            return View(mm_ctrl_machine_alarms);
        }

        // GET: /MachineAlarms/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Ctrl_Machine_Alarms mm_ctrl_machine_alarms = db.MM_Ctrl_Machine_Alarms.Find(id);
            if (mm_ctrl_machine_alarms == null)
            {
                return HttpNotFound();
            }
            return View(mm_ctrl_machine_alarms);
        }

        // POST: /MachineAlarms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_Ctrl_Machine_Alarms mm_ctrl_machine_alarms = db.MM_Ctrl_Machine_Alarms.Find(id);
            db.MM_Ctrl_Machine_Alarms.Remove(mm_ctrl_machine_alarms);
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

        public ActionResult getMachineByStationId(int stationId)
        {
            try
            {
                var res = from machinesObj in db.MM_MT_MTTUW_Machines
                          where machinesObj.Station_ID == stationId

                          select new
                          {
                              Id = machinesObj.Machine_ID,
                              Value = machinesObj.Machine_Name
                          };

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getSelectedAlarms(int machineId, int stationId)
        {
            try
            {


                var res = from alarmsObj in db.MM_Ctrl_Machine_Alarms_Master
                          where (from machinesAlarmsObj in db.MM_Ctrl_Machine_Alarms where machinesAlarmsObj.Machine_ID == machineId && machinesAlarmsObj.Station_ID == stationId select machinesAlarmsObj.Alarm_ID).Contains(alarmsObj.Alarm_ID)
                          select new
                          {
                              Id = alarmsObj.Alarm_ID,
                              Value = alarmsObj.Alarm_Name
                          };

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getNotSelectedAlarms(int machineId)
        {
            try
            {
                var res = from alarmsObj in db.MM_Ctrl_Machine_Alarms_Master
                          where !(from machinesAlarmsObj in db.MM_Ctrl_Machine_Alarms where machinesAlarmsObj.Machine_ID == machineId select machinesAlarmsObj.Alarm_ID).Contains(alarmsObj.Alarm_ID)

                          select new
                          {
                              Id = alarmsObj.Alarm_ID,
                              Value = alarmsObj.Alarm_Name
                          };

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ExcelUpload()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            List<SelectListItem> listModel = new List<SelectListItem>();
            ViewBag.Line_ID = new SelectList(listModel);
            ViewBag.Station_ID = new SelectList(listModel);
            ViewBag.Machine_ID = new SelectList(listModel);

            if (TempData["OrderUploadRecords"] != null)
            {
                ViewBag.OrderUploadRecords = TempData["OrderUploadRecords"];
            }

            globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.Machine_Alarm + " " + ResourceGlobal.Form;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "MachineAlarms";
            globalData.actionName = ResourceGlobal.Upload;
            globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Machine_Alarm + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Machine_Alarm + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelUpload(ExcelMachineAlarms formData)
        {
            int plantId = 0, shopId = 0, lineId = 0, stationId = 0, machineId = 0;
            String createdOrders = "";
            if (ModelState.IsValid)
            {
                GlobalOperations globalOperations = new GlobalOperations();
                string fileName = Path.GetFileName(formData.Excel_File.FileName);
                string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);
                //String attributeId = formData.Attribute_ID;

                plantId = Convert.ToInt32(formData.Plant_ID);
                shopId = Convert.ToInt32(formData.Shop_ID);
                lineId = Convert.ToInt32(formData.Line_ID);
                stationId = Convert.ToInt32(formData.Station_ID);
                machineId = Convert.ToInt32(formData.Machine_ID);

                if (dt.Rows.Count > 0)
                {
                    MachineAlarmsUploadRecords[] orderUploadRecordsObj = new MachineAlarmsUploadRecords[dt.Rows.Count];
                    MM_Ctrl_Machine_Alarms MM_Ctrl_Machine_Alarms = new MM_Ctrl_Machine_Alarms();

                    int i = 0;
                    foreach (DataRow checkListRow in dt.Rows)
                    {
                        String alarmName = checkListRow[0].ToString().Trim();
                        String alarmMessage = checkListRow[1].ToString().Trim();
                        int qty = 0;
                        orderUploadRecordsObj[i] = new MachineAlarmsUploadRecords();
                        MachineAlarmsUploadRecords orderUploadObj = new MachineAlarmsUploadRecords();
                        orderUploadObj.AlarmName = alarmName;
                        orderUploadObj.AlarmMessage = alarmMessage;
                        MM_Ctrl_Machine_Alarms_Master MM_Ctrl_Machine_Alarms_Master = new MM_Ctrl_Machine_Alarms_Master();
                        if (!isMachineAlarmExist(plantId, shopId, lineId, stationId, machineId, alarmName))
                        {
                            if (!isAlarmExist(plantId, alarmName))
                            {
                                MM_Ctrl_Machine_Alarms_Master.Alarm_Name = alarmName;
                                MM_Ctrl_Machine_Alarms_Master.Alarm_Message = alarmMessage;
                                MM_Ctrl_Machine_Alarms_Master.Plant_ID = plantId;
                                MM_Ctrl_Machine_Alarms_Master.Inserted_Date = DateTime.Now;
                                MM_Ctrl_Machine_Alarms_Master.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                MM_Ctrl_Machine_Alarms_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                db.MM_Ctrl_Machine_Alarms_Master.Add(MM_Ctrl_Machine_Alarms_Master);
                                db.SaveChanges();

                                MM_Ctrl_Machine_Alarms.Plant_ID = plantId;
                                MM_Ctrl_Machine_Alarms.Shop_ID = shopId;
                                MM_Ctrl_Machine_Alarms.Line_ID = lineId;
                                MM_Ctrl_Machine_Alarms.Station_ID = stationId;
                                MM_Ctrl_Machine_Alarms.Machine_ID = machineId;
                                MM_Ctrl_Machine_Alarms.Alarm_ID = MM_Ctrl_Machine_Alarms_Master.Alarm_ID;
                                MM_Ctrl_Machine_Alarms.Inserted_Date = DateTime.Now;
                                MM_Ctrl_Machine_Alarms.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                MM_Ctrl_Machine_Alarms.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                db.MM_Ctrl_Machine_Alarms.Add(MM_Ctrl_Machine_Alarms);
                                db.SaveChanges();
                            }

                        }
                        orderUploadRecordsObj[i] = orderUploadObj;
                        i = i + 1;
                    }


                    TempData["OrderUploadRecords"] = orderUploadRecordsObj;
                    //TempData["ChecklistDataTable"] = dt;
                    ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                    //ViewBag.dt = qualityChecklistDt;
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.MaintenanceMachinePart;
                    globalData.messageDetail = ResourceGlobal.Order + " " + ResourceMessages.Upload_Success;
                    globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.MaintenanceMachinePart + " " + ResourceGlobal.Form;
                    globalData.subTitle = ResourceGlobal.Upload;
                    globalData.controllerName = "MaintenanceMachinePart";
                    globalData.actionName = ResourceGlobal.Upload;
                    globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.MaintenanceMachinePart + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.MaintenanceMachinePart + " " + ResourceGlobal.Form;
                    ViewBag.GlobalDataModel = globalData;

                    ViewBag.createdOrders = createdOrders;
                }
            }
            //return PartialView("QualityChecklistDetails");

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", shopId);
            ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines, "Line_ID", "Line_Name", lineId);
            ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations, "Station_ID", "Station_Name", stationId);
            ViewBag.Machine_ID = new SelectList(db.MM_MT_MTTUW_Machines, "Machine_ID", "Machine_Name", machineId);
            return View();
        }

        public bool isMachineAlarmExist(int plantId, int shopId, int lineId, int stationId, int machineId, string alarmName)
        {
            var result = db.MM_Ctrl_Machine_Alarms.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId && m.Station_ID == stationId && m.Machine_ID == machineId && m.MM_Ctrl_Machine_Alarms_Master.Alarm_Name == alarmName);
            if (result.Count() > 0)
                return true;
            else
                return false;
        }

        public bool isAlarmExist(int plantId, string alarmName)
        {
            var result = db.MM_Ctrl_Machine_Alarms_Master.Where(m => m.Plant_ID == plantId && m.Alarm_Name == alarmName);
            if (result.Count() > 0)
                return true;
            else
                return false;
        }

        public DataTable ExcelToDataTable(HttpPostedFileBase uploadFile, string fileLocation, string fileExtension)
        {



            DataTable dtExcelRecords = new DataTable();
            string connectionString = "";
            if (uploadFile.ContentLength > 0)
            {

                uploadFile.SaveAs(fileLocation);

                //Check whether file extension is xls or xslx

                if (fileExtension == ".xls")
                {
                    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                else if (fileExtension == ".xlsx")
                {
                    connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }

                //Create OleDB Connection and OleDb Command && Read data from excel and generate datatable 

                OleDbConnection con = new OleDbConnection(connectionString);
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;
                OleDbDataAdapter dAdapter = new OleDbDataAdapter(cmd);

                con.Open();
                DataTable dtExcelSheetName = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string getExcelSheetName = dtExcelSheetName.Rows[0]["Table_Name"].ToString();
                cmd.CommandText = "SELECT * FROM [" + getExcelSheetName + "]";
                dAdapter.SelectCommand = cmd;
                dAdapter.Fill(dtExcelRecords);
                con.Close();

            }
            return dtExcelRecords;
        }
    }
}
