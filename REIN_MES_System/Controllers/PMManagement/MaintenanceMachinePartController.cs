using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.MaintenanceManagement
{
    public class MaintenanceMachinePartController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_Maintenance_Machine_Part mmtobj = new RS_Maintenance_Machine_Part();
        int machineId = 0, partId = 0;
        int operatorId = 0;
        // GET: MaintenanceMachinePart
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.MaintenanceMachinePart;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "MaintenanceMachinePart";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var RS_Maintenance_Machine_Part = db.RS_Maintenance_Machine_Part.Where(p => p.Plant_ID == PlantID).Include(m => m.RS_Employee).Include(m => m.RS_Plants).Include(m => m.RS_Shops);
            return View(RS_Maintenance_Machine_Part.ToList());
        }

        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Maintenance_Machine_Part RS_Maintenance_Machine_Part = db.RS_Maintenance_Machine_Part.Find(id);
            if (RS_Maintenance_Machine_Part == null)
            {
                return HttpNotFound();
            }
            return View(RS_Maintenance_Machine_Part);
        }

        public ActionResult Create()
        {
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
                ViewBag.Machine_ID = new SelectList("");
                ViewBag.ListofPart = new SelectList("");
                ViewBag.selectedParts = new SelectList("");

            }
            else
            {
                ViewBag.ListofPart = new SelectList(db.RS_Maintenance_Machine_Part.Where(p => p.Maintenance_Part_ID == partId), "Maintenance_Part_ID", "Part_Name");
                ViewBag.Part_ID = new SelectList(db.RS_Maintenance_Machine_Part.Where(p => p.Maintenance_Part_ID == partId), "Maintenance_Part_ID", "Part_Name");
            }

            globalData.pageTitle = ResourceModules.Part_To_Machine;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "MaintenanceMachinePart";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.Part_To_Machine + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Part_To_Machine + " " + ResourceGlobal.Lists;

            List<SelectListItem> listModel = new List<SelectListItem>();

            ViewBag.GlobalDataModel = globalData;
            //ViewBag.Part_ID = new SelectList(db.MM_MT_Machine_Part, "Part_ID", "Part_ID");
            ViewBag.Machine_Part_ID = new SelectList(db.RS_Maintenance_Machine_Part, "Machine_Part_ID", "Machine_Part_ID");
            ViewBag.Machine_Part_ID = new SelectList(db.RS_Maintenance_Machine_Part, "Machine_Part_ID", "Machine_Part_ID");
            ViewBag.Part_ID = new SelectList(db.RS_Maintenance_Part.OrderBy(c => c.Part_Name), "Maintenance_Part_ID", "Part_Name");

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", PlantID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(listModel);
            ViewBag.Station_ID = new SelectList(listModel);
            ViewBag.Machine_ID = new SelectList(listModel);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Maintenance_Machine_Part RS_Maintenance_Machine_Part)
        {
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                partId = 1;
                for (int i = 0; i < RS_Maintenance_Machine_Part.selectedParts.Count(); i++)
                {
                    operatorId = Convert.ToInt16(RS_Maintenance_Machine_Part.selectedParts[i]);

                    if (operatorId == 0)
                        continue;
                    else
                    {
                        RS_Maintenance_Machine_Part.Plant_ID = PlantID;
                        RS_Maintenance_Machine_Part.Inserted_Date = DateTime.Now;
                        RS_Maintenance_Machine_Part.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                        RS_Maintenance_Machine_Part.Maintenance_Part_ID = RS_Maintenance_Machine_Part.selectedParts[i];

                        RS_Maintenance_Machine_Part mmAssignPartObj = new RS_Maintenance_Machine_Part();
                        mmAssignPartObj = RS_Maintenance_Machine_Part;

                        db.Entry(mmAssignPartObj).State = EntityState.Detached;

                        db.RS_Maintenance_Machine_Part.Add(mmAssignPartObj);

                        db.SaveChanges();
                    }

                }
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Part_To_Machine;
                globalData.messageDetail = ResourceModules.Part_To_Machine + " " + ResourceMessages.Add_Success;
                TempData["globalData"] = globalData;
                TempData["plantId"] = globalData;
                TempData["shopId"] = globalData;
                TempData["managerId"] = globalData;
            }


            globalData.pageTitle = ResourceModules.Part_To_Machine;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "MaintenanceMachinePart";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Part_To_Machine + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Part_To_Machine + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Machine_Part_ID = new SelectList(db.RS_Maintenance_Machine_Part, "Machine_Part_ID", "Machine_Part_ID", RS_Maintenance_Machine_Part.Machine_Part_ID);
            ViewBag.Machine_Part_ID = new SelectList(db.RS_Maintenance_Machine_Part, "Machine_Part_ID", "Machine_Part_ID", RS_Maintenance_Machine_Part.Machine_Part_ID);

            ViewBag.Machine_ID = new SelectList(db.RS_MT_Machines.OrderBy(c => c.Machine_Name), "Machine_ID", "Machine_Name", RS_Maintenance_Machine_Part.Machine_ID);
            return View(RS_Maintenance_Machine_Part);
        }

        //public ActionResult GetMachineByShopID(int shopid)
        //{
        //    var Machines = db.RS_MT_Machines.Where(c => c.Shop_ID == shopid).Select(a => new { a.Machine_ID, a.Machine_Name }).OrderBy(x => x.Machine_Name);
        //    return Json(Machines, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetShopByPlantID(int plantid)
        //{
        //    var Shops = db.RS_Shops.Where(c => c.Plant_ID == plantid).Select(a => new { a.Shop_ID, a.Shop_Name }).OrderBy(x => x.Shop_Name);
        //    return Json(Shops, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetLineByShopID(int shopid)
        //{
        //    var Lines = db.RS_Lines.Where(c => c.Shop_ID == shopid).Select(a => new { a.Line_ID, a.Line_Name }).OrderBy(x => x.Line_Name);
        //    return Json(Lines, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetStationByLineID(int lineid)
        //{
        //    var Stations = db.RS_Stations.Where(c => c.Line_ID == lineid).Select(a => new { a.Station_ID, a.Station_Name }).OrderBy(x => x.Station_Name);
        //    return Json(Stations, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Maintenance_Machine_Part RS_Maintenance_Machine_Part = db.RS_Maintenance_Machine_Part.Find(id);
            if (RS_Maintenance_Machine_Part == null)
            {
                return HttpNotFound();
            }
            ViewBag.Machine_Part_ID = new SelectList(db.RS_Maintenance_Machine_Part, "Machine_Part_ID", "Machine_Part_ID", RS_Maintenance_Machine_Part.Machine_Part_ID);
            ViewBag.Machine_Part_ID = new SelectList(db.RS_Maintenance_Machine_Part, "Machine_Part_ID", "Machine_Part_ID", RS_Maintenance_Machine_Part.Machine_Part_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", RS_Maintenance_Machine_Part.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Maintenance_Machine_Part.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Maintenance_Machine_Part.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Maintenance_Machine_Part.Station_ID);
            ViewBag.Machine_ID = new SelectList(db.RS_MT_Machines, "Machine_ID", "Machine_Name", RS_Maintenance_Machine_Part.Machine_ID);
            return View(RS_Maintenance_Machine_Part);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Machine_Part_ID,Maintenance_Part_ID,Machine_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Is_Deleted,Is_Transfered,Is_Purgeable")] RS_Maintenance_Machine_Part RS_Maintenance_Machine_Part)
        {
            if (ModelState.IsValid)
            {
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[RS_Maintenance_Machine_Part] ON");
                RS_Maintenance_Machine_Part = db.RS_Maintenance_Machine_Part.Find(RS_Maintenance_Machine_Part.Machine_Part_ID);


                RS_Maintenance_Machine_Part.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                RS_Maintenance_Machine_Part.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                RS_Maintenance_Machine_Part.Updated_Date = DateTime.Now;
                RS_Maintenance_Machine_Part.Is_Edited = true;
                db.Entry(RS_Maintenance_Machine_Part).State = EntityState.Modified;
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[RS_Maintenance_Machine_Part] OFF");
                return RedirectToAction("Index");
            }
            ViewBag.Machine_Part_ID = new SelectList(db.RS_Maintenance_Machine_Part, "Machine_Part_ID", "Machine_Part_ID", RS_Maintenance_Machine_Part.Machine_Part_ID);
            ViewBag.Machine_Part_ID = new SelectList(db.RS_Maintenance_Machine_Part, "Machine_Part_ID", "Machine_Part_ID", RS_Maintenance_Machine_Part.Machine_Part_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", RS_Maintenance_Machine_Part.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Maintenance_Machine_Part.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Maintenance_Machine_Part.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Maintenance_Machine_Part.Station_ID);

            ViewBag.Machine_ID = new SelectList(db.RS_MT_Machines, "Machine_ID", "Machine_Name", RS_Maintenance_Machine_Part.Machine_ID);
            return View(RS_Maintenance_Machine_Part);
        }

        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Maintenance_Machine_Part RS_Maintenance_Machine_Part = db.RS_Maintenance_Machine_Part.Find(id);
            if (RS_Maintenance_Machine_Part == null)
            {
                return HttpNotFound();
            }
            return View(RS_Maintenance_Machine_Part);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Maintenance_Machine_Part RS_Maintenance_Machine_Part = db.RS_Maintenance_Machine_Part.Find(id);
            db.RS_Maintenance_Machine_Part.Remove(RS_Maintenance_Machine_Part);
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

        public ActionResult GetMachineByShopId(string Shop_ID)
        {
            var shopid = Convert.ToInt32(Shop_ID);
            var MachineList = (from machine in db.RS_MT_Machines
                               where machine.Shop_ID == shopid && machine.IsActive == true
                               select new
                               {
                                   Id = machine.Machine_ID,
                                   Value = machine.Machine_Name
                               });
            return Json(MachineList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssignedPartsByMachineID(int machineId)
        {

            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from part in db.RS_Maintenance_Machine_Part
                         where part.Machine_ID == machineId
                         select new
                         {
                             Id = part.Maintenance_Part_ID,
                             Value = part.RS_Maintenance_Part.Part_Name
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetPartsByMachineID(int machineId)
        {
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                db.Configuration.ProxyCreationEnabled = false;
                var freepart = db.RS_PM_Activity_Part.Select(m => m.RS_Maintenance_Part).ToList();
                var st = (from part in db.RS_Maintenance_Part
                          where part.Plant_ID == plantId &&
                          !(from sm in db.RS_Maintenance_Machine_Part where sm.Machine_ID == machineId select sm.Maintenance_Part_ID).Contains(part.Maintenance_Part_ID)
                          select new
                          {
                              Id = part.Maintenance_Part_ID,
                              Value = part.Part_Name
                          });

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveAssignedPart(string Parts, int machineId, int plantId, int shopId, int lineId,int stationId)
        {
            try
            {
                RS_Maintenance_Machine_Part RS_Maintenance_Machine_Part = new RS_Maintenance_Machine_Part();

                RS_Maintenance_Machine_Part.deleteOperator(machineId, ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);  //plantId, 
                var AssignedPart = (from pm in db.RS_PM_Activity_Part
                                    join MM in db.RS_Maintenance_Machine_Part
                                    on pm.Maintenance_Part_ID equals MM.Maintenance_Part_ID
                                    where MM.Machine_ID == machineId
                                    select MM.RS_Maintenance_Part.Part_Name).Distinct().ToList();
                string notDeletedPart = "";

                foreach (var item in AssignedPart)
                {
                    notDeletedPart += item + " ";
                }


                var result = new { Status = true, Message = notDeletedPart };
                string[] words;
                words = Parts.Split(',');

                foreach (string value in words)
                {
                    int i = 0;
                    if (value == "")
                    {
                        i = 0;
                    }
                    else
                    {
                        i = Convert.ToInt32(value);
                    }
                    if (i == 0)
                        continue;

                    RS_Maintenance_Machine_Part.Plant_ID = plantId;
                    RS_Maintenance_Machine_Part.Shop_ID = shopId;
                    RS_Maintenance_Machine_Part.Line_ID = lineId;
                    RS_Maintenance_Machine_Part.Station_ID = stationId;
                    RS_Maintenance_Machine_Part.Inserted_Date = DateTime.Now;
                    RS_Maintenance_Machine_Part.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Maintenance_Machine_Part.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    RS_Maintenance_Machine_Part.Machine_ID = machineId;
                    RS_Maintenance_Machine_Part.Maintenance_Part_ID = i;

                    var IsExist = db.RS_Maintenance_Machine_Part.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId && m.Station_ID == stationId && m.Machine_ID == machineId && m.Maintenance_Part_ID == i).ToList();
                    if (IsExist.Count == 0)
                    {
                        db.Entry(RS_Maintenance_Machine_Part).State = EntityState.Detached;
                        db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[RS_Maintenance_Machine_Part] ON");
                        db.RS_Maintenance_Machine_Part.Add(RS_Maintenance_Machine_Part);
                        db.SaveChanges();
                        db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[RS_Maintenance_Machine_Part] OFF");
                    }
                    i = 0;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException e)
            {
                var result = new { Status = false, Message = "Exception !!!!!" };
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
           
        }

        public ActionResult ExcelUpload()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            List<SelectListItem> listModel = new List<SelectListItem>();
            ViewBag.Line_ID = new SelectList(listModel);
            ViewBag.Station_ID = new SelectList(listModel);
            ViewBag.Machine_ID = new SelectList(listModel);

            if (TempData["OrderUploadRecords"] != null)
            {
                ViewBag.OrderUploadRecords = TempData["OrderUploadRecords"];
            }

            globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.MaintenanceMachinePart + " " + ResourceGlobal.Form;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "MaintenanceMachinePart";
            globalData.actionName = ResourceGlobal.Upload;
            globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.MaintenanceMachinePart + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.MaintenanceMachinePart + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelUpload(ExcelMachineToPart formData)
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
                    PartToMachineUploadRecords[] orderUploadRecordsObj = new PartToMachineUploadRecords[dt.Rows.Count];
                    RS_Maintenance_Machine_Part mmOrderCreationObj = new RS_Maintenance_Machine_Part();

                    int i = 0;
                    foreach (DataRow checkListRow in dt.Rows)
                    {

                        String partName = checkListRow[0].ToString().Trim();
                        String partDescription = checkListRow[1].ToString().Trim();
                        int qty = 0;
                        orderUploadRecordsObj[i] = new PartToMachineUploadRecords();
                        PartToMachineUploadRecords orderUploadObj = new PartToMachineUploadRecords();
                        orderUploadObj.PartName = partName;
                        orderUploadObj.PartDescription = partDescription;
                        try {
                            RS_Maintenance_Part RS_Maintenance_Part = new RS_Maintenance_Part();

                            var result = db.RS_Maintenance_Part.Where(m => m.Part_Name == partName);
                            if (result.Count() > 0)
                            {
                                var id = db.RS_Maintenance_Part.Where(m => m.Part_Name == partName).Select(m => m.Maintenance_Part_ID).FirstOrDefault();
                                if (!isAlreadyExist(plantId, shopId, lineId, stationId, machineId, partName))
                                {
                                    mmOrderCreationObj.Plant_ID = plantId;
                                    mmOrderCreationObj.Shop_ID = shopId;
                                    mmOrderCreationObj.Line_ID = lineId;
                                    mmOrderCreationObj.Station_ID = stationId;
                                    mmOrderCreationObj.Inserted_Date = DateTime.Now;
                                    mmOrderCreationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    mmOrderCreationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    mmOrderCreationObj.Machine_ID = machineId;
                                    mmOrderCreationObj.Maintenance_Part_ID = id;
                                    db.RS_Maintenance_Machine_Part.Add(mmOrderCreationObj);
                                    db.SaveChanges();
                                    orderUploadObj.SS_Error_Success = "Record Added Successfully";
                                }
                                else
                                {
                                    orderUploadObj.SS_Error_Success = "Record Already Added";
                                }
                            }
                            else
                            {
                                RS_Maintenance_Part.Part_Name = partName;
                                RS_Maintenance_Part.Part_Desscription = partDescription;
                                RS_Maintenance_Part.Plant_ID = plantId;
                                RS_Maintenance_Part.Inserted_Date = DateTime.Now;
                                RS_Maintenance_Part.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                RS_Maintenance_Part.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                db.RS_Maintenance_Part.Add(RS_Maintenance_Part);
                                db.SaveChanges();

                                mmOrderCreationObj.Plant_ID = plantId;
                                mmOrderCreationObj.Shop_ID = shopId;
                                mmOrderCreationObj.Line_ID = lineId;
                                mmOrderCreationObj.Station_ID = stationId;
                                mmOrderCreationObj.Inserted_Date = DateTime.Now;
                                mmOrderCreationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                mmOrderCreationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                mmOrderCreationObj.Machine_ID = machineId;
                                mmOrderCreationObj.Maintenance_Part_ID = RS_Maintenance_Part.Maintenance_Part_ID;
                                db.Entry(mmOrderCreationObj).State = EntityState.Detached;
                                db.RS_Maintenance_Machine_Part.Add(mmOrderCreationObj);
                                db.SaveChanges();
                                orderUploadObj.SS_Error_Success = "Record Added Successfuly";
                            }

                            orderUploadRecordsObj[i] = orderUploadObj;
                            i = i + 1;
                        }
                        catch(Exception ex)
                        {
                            orderUploadObj.SS_Error_Success = ex.Message;
                        }

                        }



                    TempData["OrderUploadRecords"] = orderUploadRecordsObj;
                    //TempData["ChecklistDataTable"] = dt;
                    ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                    //ViewBag.dt = qualityChecklistDt;
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.MaintenanceMachinePart;
                    globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceMessages.Upload_Success;
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
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", shopId);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", lineId);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", stationId);
            ViewBag.Machine_ID = new SelectList(db.RS_MT_Machines, "Machine_ID", "Machine_Name", machineId);
            return View();
        }
        public bool isAlreadyExist(int plantId, int shopId, int lineId, int stationId, int machineId, string partName)
        {
            var result = db.RS_Maintenance_Machine_Part.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId && m.Station_ID == stationId && m.Machine_ID == machineId && m.RS_Maintenance_Part.Part_Name == partName);
            if (result.Count() > 0)
                return true;
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