using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.MaintenanceManagement
{
    public class PMActivityController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        
        GlobalData globalData = new GlobalData();
        RS_PM_Activity mmtobj = new RS_PM_Activity();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0;
        // GET: PMActivity
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            decimal PlantID=((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.PMActivity;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "PMActivity";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var RS_PM_Activity = db.RS_PM_Activity.Where(p=>p.Plant_ID==PlantID);
            return View(RS_PM_Activity.ToList());
        }

        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PM_Activity RS_PM_Activity = db.RS_PM_Activity.Find(id);
            if (RS_PM_Activity == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.PMActivity;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "PMActivity";
            globalData.actionName = ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_PM_Activity);
        }

        public ActionResult Create()
        {
            decimal PlantID=((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.PMActivity;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "PMActivity";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            List<SelectListItem> listModel = new List<SelectListItem>();
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", PlantID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(listModel);
            ViewBag.Station_ID = new SelectList(listModel);
            ViewBag.Machine_ID = new SelectList(db.RS_MT_Machines,"Machine_ID","Machine_Name");
            ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p=>p.Plant_ID==PlantID), "Employee_ID", "Employee_Name");
            var assignedPart = db.RS_PM_Activity_Part.Select(p=>p.Maintenance_Part_ID).ToList();
            ViewBag.Maintenance_Part_ID = new SelectList(db.RS_Maintenance_Part.Where(p => p.Plant_ID == PlantID && !assignedPart.Contains(p.Maintenance_Part_ID)), "Maintenance_Part_ID", "Part_Name");
            ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment, "EQP_ID", "Equipment_Name");
            ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_PM_Activity RS_PM_Activity)
        {
            try
            {
                var assignedPart = db.RS_PM_Activity_Part.Select(p => p.Maintenance_Part_ID).ToList();
                decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
                var MachinesList = getMachineByShopId(Convert.ToInt32(RS_PM_Activity.Shop_ID));

                if (ModelState.IsValid)
                {

                    if (RS_PM_Activity.Is_Value_Based == true && (RS_PM_Activity.Lower_Limit == null || RS_PM_Activity.Upper_Limit == null))
                    {
                        if (RS_PM_Activity.Lower_Limit == null)
                        {
                            ModelState.AddModelError("Lower_Limit", ResourceValidation.Required);
                        }
                        if (RS_PM_Activity.Upper_Limit == null)
                        {
                            ModelState.AddModelError("Upper_Limit", ResourceValidation.Required);
                        }
                        if (RS_PM_Activity.M_ID == null)
                        {
                            ModelState.AddModelError("M_ID", ResourceValidation.Required);
                        }
                        globalData.messageTitle = ResourceModules.PMActivity;
                        globalData.messageDetail = ResourceModules.PMActivity + " " + ResourceValidation.Upper_Limit + " OR " + ResourceValidation.Lower_Limit;
                        globalData.isErrorMessage = true;

                        TempData["globalData"] = globalData;
                        globalData.pageTitle = ResourceModules.PMActivity;
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "PMActivity";
                        globalData.actionName = ResourceGlobal.Create;
                        ViewBag.GlobalDataModel = globalData;
                        ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", PlantID);
                        ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
                        ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_PM_Activity.Line_ID);
                        ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_PM_Activity.Station_ID);
                        ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID);
                        ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name");
                        ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);

                        ViewBag.Maintenance_Part_ID = new SelectList(db.RS_Maintenance_Part.Where(p => p.Plant_ID == PlantID && !assignedPart.Contains(p.Maintenance_Part_ID)), "Maintenance_Part_ID", "Part_Name");
                        ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment, "EQP_ID", "Equipment_Name");

                        ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
                        ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Inserted_User_ID);
                        ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Updated_User_ID);

                        return View(RS_PM_Activity);
                    }
                    if (RS_PM_Activity.Is_Value_Based == true && ((RS_PM_Activity.Lower_Limit >= RS_PM_Activity.Upper_Limit)))
                    {
                        ModelState.AddModelError("Lower_Limit", "Please Check value Range");
                        ModelState.AddModelError("Upper_Limit", "Please Check value Range");
                        globalData.messageTitle = ResourceModules.PMActivity;
                        globalData.messageDetail = ResourceModules.PMActivity + " Please check upper and lower limit";
                        globalData.isErrorMessage = true;

                        TempData["globalData"] = globalData;
                        globalData.pageTitle = ResourceModules.PMActivity;
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "PMActivity";
                        globalData.actionName = ResourceGlobal.Create;
                        ViewBag.GlobalDataModel = globalData;
                        ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", PlantID);
                        ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
                        ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
                        ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
                        MachinesList = getMachineByShopId(Convert.ToInt32(RS_PM_Activity.Shop_ID));
                        ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID); ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name");
                        // var assignedPart = db.RS_PM_Activity_Part.Select(p => p.Activity_Part_ID).ToList();
                        ViewBag.Maintenance_Part_ID = new MultiSelectList(db.RS_Maintenance_Part.Where(p => p.Plant_ID == PlantID && !assignedPart.Contains(p.Maintenance_Part_ID)), "Maintenance_Part_ID", "Part_Name", RS_PM_Activity.Maintenance_Part_ID);
                        ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment.Where(m=>m.Is_PM_Equipment == true), "EQP_ID", "Equipment_Name");
                        ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);

                        ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
                        ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Inserted_User_ID);
                        ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Updated_User_ID);

                        return View(RS_PM_Activity);

                    }
                    if (RS_PM_Activity.Start_Date > RS_PM_Activity.Last_Date)
                    {
                        ModelState.AddModelError("Start_Date", ResourceValidation.Chk_Date);
                        ModelState.AddModelError("Last_Date", ResourceValidation.Chk_Last_date);
                        globalData.messageTitle = ResourceModules.PMActivity;
                        globalData.messageDetail = ResourceModules.PMActivity + " " + ResourceValidation.Date_diff;
                        globalData.isErrorMessage = true;

                        TempData["globalData"] = globalData;
                        globalData.pageTitle = ResourceModules.PMActivity;
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "PMActivity";
                        globalData.actionName = ResourceGlobal.Create;
                        ViewBag.GlobalDataModel = globalData;
                        ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", PlantID);
                        ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == PlantID), "Shop_ID", "Shop_Name");
                        ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
                        ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
                        MachinesList = getMachineByShopId(Convert.ToInt32(RS_PM_Activity.Shop_ID));
                        ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID);
                        ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name");
                        //var assignedPart = db.RS_PM_Activity_Part.Select(p => p.Activity_Part_ID).ToList();
                        ViewBag.Maintenance_Part_ID = new SelectList(db.RS_Maintenance_Part.Where(p => p.Plant_ID == PlantID && !assignedPart.Contains(p.Maintenance_Part_ID)), "Maintenance_Part_ID", "Part_Name", RS_PM_Activity.Maintenance_Part_ID);
                        ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment.Where(m=>m.Is_PM_Equipment == true), "EQP_ID", "Equipment_Name");
                        ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);

                        ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
                        ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Inserted_User_ID);
                        ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Updated_User_ID);

                        return View(RS_PM_Activity);
                    }
                    else
                    {
                        if (RS_PM_Activity.Activity_Name == RS_PM_Activity.Activity_Description)
                        {
                            ModelState.AddModelError("Activity_Description", "Activity Description can not be same as Activity Name");
                        }
                        else
                        {
                            if (RS_PM_Activity.Is_Value_Based == null)
                            {
                                RS_PM_Activity.Is_Value_Based = false;
                            }

                            if (RS_PM_Activity.Is_User_Value_Based == null)
                            {
                                RS_PM_Activity.Is_User_Value_Based = false;
                            }
                            // RS_PM_Activity.Plant_ID = PlantID;
                            RS_PM_Activity.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            RS_PM_Activity.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            RS_PM_Activity.Inserted_Date = DateTime.Now;
                            var empid = RS_PM_Activity.Activity_Owner_ID;
                            RS_PM_Activity.RS_Employee = db.RS_Employee.Find(empid);
                            db.RS_PM_Activity.Add(RS_PM_Activity);
                            db.SaveChanges();
                            RS_PM_Activity_Part mmpmapobj = new RS_PM_Activity_Part();
                            foreach (var item in RS_PM_Activity.Maintenance_Part_ID)
                            {
                                mmpmapobj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                mmpmapobj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                mmpmapobj.Inserted_Date = DateTime.Now;
                                mmpmapobj.Plant_ID = PlantID;
                                mmpmapobj.Shop_ID = RS_PM_Activity.Shop_ID;
                                mmpmapobj.Line_ID = RS_PM_Activity.Line_ID;
                                mmpmapobj.Station_ID = RS_PM_Activity.Station_ID;
                                mmpmapobj.Activity_ID = RS_PM_Activity.Activity_ID;
                                mmpmapobj.Machine_ID = RS_PM_Activity.Machine_ID;
                                mmpmapobj.Maintenance_Part_ID = Convert.ToDecimal(item);
                                db.RS_PM_Activity_Part.Add(mmpmapobj);
                                db.SaveChanges();
                            }

                            globalData.isSuccessMessage = true;
                            globalData.messageTitle = ResourceModules.PMActivity;
                            globalData.messageDetail = ResourceModules.PMActivity + " " + ResourceMessages.Add_Success;
                            TempData["globalData"] = globalData;
                            return RedirectToAction("Index");
                        }
                    }
                }

               // globalData.isSuccessMessage = true;
                globalData.isErrorMessage = true;

                TempData["globalData"] = globalData;
                globalData.pageTitle = ResourceModules.PMActivity;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "PMActivity";
                globalData.actionName = ResourceGlobal.Create;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", PlantID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m=>m.Plant_ID==PlantID), "Shop_ID", "Shop_Name");
                ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
                ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
                MachinesList = getMachineByShopId(Convert.ToInt32(RS_PM_Activity.Shop_ID));
                ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID);
                ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name");
               // var assignedPart = db.RS_PM_Activity_Part.Select(p => p.Activity_Part_ID).ToList();
                ViewBag.Maintenance_Part_ID = new SelectList(db.RS_Maintenance_Part.Where(p => p.Plant_ID == PlantID && !assignedPart.Contains(p.Maintenance_Part_ID)), "Maintenance_Part_ID", "Part_Name", RS_PM_Activity.Maintenance_Part_ID);
                ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment.Where(m=>m.Is_PM_Equipment == true), "EQP_ID", "Equipment_Name");
                ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);

                ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Updated_User_ID);

                return View(RS_PM_Activity);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

        }

        //public ActionResult GetMachineByShopID(int shopid)
        //{
        //    var Machines = db.RS_MT_Machines.Where(c => c.Shop_ID == shopid && c.IsActive == true).Select(a => new { a.Machine_ID, a.Machine_Name }).OrderBy(x => x.Machine_Name);
        //    return Json(Machines, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetMachinePartByMachineID(int machineid)
        {
            //get here only part against machine id 
            //which configured in machine_part mapping and which is not assigned to any other activity
            var allPart = db.RS_Maintenance_Machine_Part.Where(m => m.Machine_ID == machineid).Select(m => m.Maintenance_Part_ID).ToList();

            var assigned_Part = db.RS_PM_Activity_Part.Where(p=>p.Machine_ID==machineid).Select(a => a.Maintenance_Part_ID).Distinct().ToList();//allready assigned part list
            var res=allPart.Except(assigned_Part).ToList();
            //[RS_Maintenance_Part] part master 
            var Parts = db.RS_Maintenance_Part.Where(c => res.Contains(c.Maintenance_Part_ID)).Select(c => new { c.Maintenance_Part_ID, c.Part_Name }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMachinePartByMachineIDEdit(int machineid,int Activity_Id)
        {
            var allPart = db.RS_Maintenance_Machine_Part.Where(m => m.Machine_ID == machineid).Select(m => m.Maintenance_Part_ID).ToList();

            var assigned_Part = db.RS_PM_Activity_Part.Where(p => p.Machine_ID == machineid && p.Activity_ID!=Activity_Id).Select(a => a.Maintenance_Part_ID).Distinct().ToList();//allready assigned part list
            var res = allPart.Except(assigned_Part).ToList();
            //[RS_Maintenance_Part] part master 
            var Parts = db.RS_Maintenance_Part.Where(c => res.Contains(c.Maintenance_Part_ID)).Select(c => new { c.Maintenance_Part_ID, c.Part_Name }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEquipmentPartByMachineID(int machineid)
        {
            var Parts = db.RS_PM_Equipment.Where(c => c.Machine_ID == machineid).Select(a => new { a.EQP_ID, a.Equipment_Name }).OrderBy(x => x.EQP_ID).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMachineByShopID(string shopid)
        {
            int Shop_ID = Convert.ToInt32(shopid);
            int PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            var result = mmtobj.GetMachineByShopId(PlantID, Shop_ID);
            return Json(result.Select(c => new { Id = c.Machine_ID, Value = c.Machine_Name + "(" + c.Machine_ID + ")" }), JsonRequestBehavior.AllowGet);
        }

        //public List<RS_MT_Machines> getMachineByStationId(int Station_ID)
        //{
        //    int PlantID = ((FDSession)this.Session["FDSession"]).plantId;
        //    var result = mmtobj.GetMachineByShopId(PlantID, Station_ID);
        //    return result;
        //}

        public List<RS_MT_Machines> getMachineByShopId(int Shop_ID)
        {
            int PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            var result = mmtobj.GetMachineByShopId(PlantID, Shop_ID);
            return result;
        }

        public ActionResult Edit(decimal id)
        {
            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PM_Activity RS_PM_Activity = db.RS_PM_Activity.Find(id);
            if (RS_PM_Activity == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.PMActivity;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PMActivity";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            var allPart = db.RS_Maintenance_Machine_Part.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID).Select(m => m.Maintenance_Part_ID).ToList();
            var roleObj1 = (from a in db.RS_PM_Activity_Part
                           join b in db.RS_PM_Activity on a.Activity_ID equals b.Activity_ID
                           where b.Activity_ID == RS_PM_Activity.Activity_ID
                           select a.RS_Maintenance_Part.Maintenance_Part_ID).ToList();
            var assigned_Part1 = db.RS_PM_Activity_Part.Where(p => p.Machine_ID == RS_PM_Activity.Machine_ID && p.Activity_ID != id).Select(a => a.Maintenance_Part_ID).Distinct().ToList();//allready assigned part list
            var res = allPart.Except(assigned_Part1).ToList();
            //[RS_Maintenance_Part] part master 
            var Parts1 = db.RS_Maintenance_Part.Where(c => res.Contains(c.Maintenance_Part_ID)).Select(c => new { c.Maintenance_Part_ID, c.Part_Name }).ToList();

            ViewBag.Maintenance_Part_ID = new MultiSelectList(Parts1, "Maintenance_Part_ID", "Part_Name", roleObj1);

           
            ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", RS_PM_Activity.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p=>p.Plant_ID==PlantID), "Shop_ID", "Shop_Name", RS_PM_Activity.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_PM_Activity.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_PM_Activity.Station_ID);
            var MachinesList = getMachineByShopId(Convert.ToInt32(RS_PM_Activity.Shop_ID));
            ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID);
            ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Activity_Owner_ID);
            ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment.Where(m=>m.Machine_ID==RS_PM_Activity.Machine_ID && m.Is_PM_Equipment == true), "EQP_ID", "Equipment_Name", RS_PM_Activity.EQP_ID);
            ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);


            return View(RS_PM_Activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_PM_Activity RS_PM_Activity)
        {

            decimal PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            var MachinesList = getMachineByShopId(Convert.ToInt32(RS_PM_Activity.Shop_ID));

            if (ModelState.IsValid)
            {
                plantId = Convert.ToInt16(RS_PM_Activity.Plant_ID);
                shopId = Convert.ToInt32(RS_PM_Activity.Shop_ID);
                lineId = Convert.ToInt32(RS_PM_Activity.Line_ID);
                stationId = Convert.ToInt32(RS_PM_Activity.Station_ID);
                if (RS_PM_Activity.Is_Value_Based == true && (RS_PM_Activity.Lower_Limit > RS_PM_Activity.Upper_Limit))
                {
                    ModelState.AddModelError("Lower_Limit", ResourceValidation.Lower_Limit);
                    ModelState.AddModelError("Upper_Limit", ResourceValidation.Upper_Limit);
                    var allPart = db.RS_Maintenance_Machine_Part.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID).Select(m => m.Maintenance_Part_ID).ToList();
                    var roleObj1 = (from a in db.RS_PM_Activity_Part
                                    join b in db.RS_PM_Activity on a.Activity_ID equals b.Activity_ID
                                    where b.Activity_ID == RS_PM_Activity.Activity_ID
                                    select a.RS_Maintenance_Part.Maintenance_Part_ID).ToList();
                    var assigned_Part1 = db.RS_PM_Activity_Part.Where(p => p.Machine_ID == RS_PM_Activity.Machine_ID && p.Activity_ID != RS_PM_Activity.Activity_ID).Select(a => a.Maintenance_Part_ID).Distinct().ToList();//allready assigned part list
                    var res = allPart.Except(assigned_Part1).ToList();
                    //[RS_Maintenance_Part] part master 
                    var Parts1 = db.RS_Maintenance_Part.Where(c => res.Contains(c.Maintenance_Part_ID)).Select(c => new { c.Maintenance_Part_ID, c.Part_Name }).ToList();

                    ViewBag.Maintenance_Part_ID = new MultiSelectList(Parts1, "Maintenance_Part_ID", "Part_Name", roleObj1);
                    ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", RS_PM_Activity.Plant_ID);
                    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name", RS_PM_Activity.Shop_ID);
                    ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_PM_Activity.Line_ID);
                    ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_PM_Activity.Station_ID);
                    ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID);
                    ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Activity_Owner_ID);
                    ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID && m.Is_PM_Equipment == true), "EQP_ID", "Equipment_Name", RS_PM_Activity.EQP_ID);
                    ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);


                    ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
                    globalData.isSuccessMessage = true;
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.PMActivity;
                    globalData.messageDetail = ResourceValidation.Upper_Limit + " " + ResourceValidation.Lower_Limit;
                    TempData["globalData"] = globalData;
                    return View(RS_PM_Activity);
                }


                if (RS_PM_Activity.Is_Value_Based == true && (RS_PM_Activity.Lower_Limit == null || RS_PM_Activity.Upper_Limit == null))
                {
                    if (RS_PM_Activity.Lower_Limit == null)
                    {
                        ModelState.AddModelError("Lower_Limit", ResourceValidation.Required);
                    }
                    if (RS_PM_Activity.Upper_Limit == null)
                    {
                        ModelState.AddModelError("Upper_Limit", ResourceValidation.Required);
                    }
                    if (RS_PM_Activity.M_ID == null)
                    {
                        ModelState.AddModelError("M_ID", ResourceValidation.Required);
                    }
                    var allPart = db.RS_Maintenance_Machine_Part.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID).Select(m => m.Maintenance_Part_ID).ToList();
                    var roleObj1 = (from a in db.RS_PM_Activity_Part
                                    join b in db.RS_PM_Activity on a.Activity_ID equals b.Activity_ID
                                    where b.Activity_ID == RS_PM_Activity.Activity_ID
                                    select a.RS_Maintenance_Part.Maintenance_Part_ID).ToList();
                    var assigned_Part1 = db.RS_PM_Activity_Part.Where(p => p.Machine_ID == RS_PM_Activity.Machine_ID && p.Activity_ID != RS_PM_Activity.Activity_ID).Select(a => a.Maintenance_Part_ID).Distinct().ToList();//allready assigned part list
                    var res = allPart.Except(assigned_Part1).ToList();
                    //[RS_Maintenance_Part] part master 
                    var Parts1 = db.RS_Maintenance_Part.Where(c => res.Contains(c.Maintenance_Part_ID)).Select(c => new { c.Maintenance_Part_ID, c.Part_Name }).ToList();
                    ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);

                    ViewBag.Maintenance_Part_ID = new MultiSelectList(Parts1, "Maintenance_Part_ID", "Part_Name", roleObj1);
                    ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", RS_PM_Activity.Plant_ID);
                    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name", RS_PM_Activity.Shop_ID);
                    ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_PM_Activity.Line_ID);
                    ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_PM_Activity.Station_ID);
                    ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID);
                    ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Activity_Owner_ID);
                    ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID && m.Is_PM_Equipment == true), "EQP_ID", "Equipment_Name", RS_PM_Activity.EQP_ID);


                    ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
                    globalData.isSuccessMessage = true;
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.PMActivity;
                    globalData.messageDetail = "Lower_Limit & Upper_Limit " + " " + ResourceValidation.Required;
                    TempData["globalData"] = globalData;
                    return View(RS_PM_Activity);
                }
                if (RS_PM_Activity.Start_Date > RS_PM_Activity.Last_Date)
                {
                    ModelState.AddModelError("Start_Date", ResourceValidation.Chk_Date);
                    ModelState.AddModelError("Last_Date", ResourceValidation.Chk_Last_date);
                    var allPart = db.RS_Maintenance_Machine_Part.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID).Select(m => m.Maintenance_Part_ID).ToList();
                    var roleObj1 = (from a in db.RS_PM_Activity_Part
                                    join b in db.RS_PM_Activity on a.Activity_ID equals b.Activity_ID
                                    where b.Activity_ID == RS_PM_Activity.Activity_ID
                                    select a.RS_Maintenance_Part.Maintenance_Part_ID).ToList();
                    var assigned_Part1 = db.RS_PM_Activity_Part.Where(p => p.Machine_ID == RS_PM_Activity.Machine_ID && p.Activity_ID != RS_PM_Activity.Activity_ID).Select(a => a.Maintenance_Part_ID).Distinct().ToList();//allready assigned part list
                    var res = allPart.Except(assigned_Part1).ToList();
                    //[RS_Maintenance_Part] part master 
                    var Parts1 = db.RS_Maintenance_Part.Where(c => res.Contains(c.Maintenance_Part_ID)).Select(c => new { c.Maintenance_Part_ID, c.Part_Name }).ToList();
                    ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);

                    ViewBag.Maintenance_Part_ID = new MultiSelectList(Parts1, "Maintenance_Part_ID", "Part_Name", roleObj1);
                    ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", RS_PM_Activity.Plant_ID);
                    ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name", RS_PM_Activity.Shop_ID);
                    ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_PM_Activity.Line_ID);
                    ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_PM_Activity.Station_ID);
                    ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID);
                    ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Activity_Owner_ID);
                    ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID && m.Is_PM_Equipment == true), "EQP_ID", "Equipment_Name", RS_PM_Activity.EQP_ID);
                    ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
                    globalData.isSuccessMessage = true;
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.PMActivity;
                    globalData.messageDetail = ResourceValidation.Chk_Last_date + " " + ResourceValidation.Chk_Date;
                    TempData["globalData"] = globalData;
                    return View(RS_PM_Activity);

                }
                else
                {
                    if (RS_PM_Activity.Activity_Name == RS_PM_Activity.Activity_Description)
                    {
                        ModelState.AddModelError("Activity_Description", "Activity Description can not be same as Activity Name");
                    }
                    else
                    {
                        RS_PM_Activity mmtobj = db.RS_PM_Activity.Find(RS_PM_Activity.Activity_ID);
                        mmtobj.Maintenance_Part_ID = RS_PM_Activity.Maintenance_Part_ID;
                        mmtobj.Activity_Name = RS_PM_Activity.Activity_Name;
                        mmtobj.Activity_Description = RS_PM_Activity.Activity_Description;
                        mmtobj.Frequency = RS_PM_Activity.Frequency;
                        mmtobj.Start_Date = RS_PM_Activity.Start_Date;
                        mmtobj.Last_Date = RS_PM_Activity.Last_Date;
                        // mmtobj.Plant_ID = PlantID;
                        mmtobj.Shop_ID = RS_PM_Activity.Shop_ID;
                        mmtobj.Line_ID = RS_PM_Activity.Line_ID;
                        mmtobj.Station_ID = RS_PM_Activity.Station_ID;
                        mmtobj.EQP_ID = RS_PM_Activity.EQP_ID;

                        mmtobj.Is_Value_Based = RS_PM_Activity.Is_Value_Based;
                        if (RS_PM_Activity.Is_Value_Based == true)
                        {
                            mmtobj.Lower_Limit = RS_PM_Activity.Lower_Limit;
                            mmtobj.Upper_Limit = RS_PM_Activity.Upper_Limit;
                            mmtobj.M_ID = RS_PM_Activity.M_ID;
                        }
                        else
                        {
                            mmtobj.Lower_Limit = null;
                            mmtobj.Upper_Limit = null;
                            mmtobj.M_ID = null;
                            mmtobj.Is_Value_Based = false;
                        }

                        if (RS_PM_Activity.Is_User_Value_Based == true)
                        {
                            mmtobj.Lower_Limit = null;
                            mmtobj.Upper_Limit = null;
                            mmtobj.M_ID = null;
                            mmtobj.Is_Value_Based = false;
                            mmtobj.Is_User_Value_Based = true;
                        }
                        else
                        {
                            mmtobj.Is_User_Value_Based = false;
                        }

                        mmtobj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        mmtobj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmtobj.Updated_Date = DateTime.Now;
                        mmtobj.Is_Edited = true;
                        db.Entry(mmtobj).State = EntityState.Modified;
                        db.SaveChanges();

                        //update Due date in log table taking only latest log record against that activity
                        var Log_ID = db.RS_PM_Activity_Log.Where(a => a.Activity_ID == RS_PM_Activity.Activity_ID && a.Is_Confirmed != true).OrderByDescending(a => a.Due_Date).FirstOrDefault();
                        if (Log_ID != null)
                        {
                            RS_PM_Activity_Log Log = db.RS_PM_Activity_Log.Find(Log_ID.PM_Activity_Log_ID);
                            Log.Due_Date = Convert.ToDateTime(RS_PM_Activity.Last_Date);
                            Log.Updated_Date = DateTime.Now;
                            Log.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            Log.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            Log.Is_Edited = true;
                            db.Entry(Log).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //done 
                        //delete all assigned part from mm_activity part table
                        RS_PM_Activity_Part[] userRoles = db.RS_PM_Activity_Part.Where(a => a.Activity_ID == RS_PM_Activity.Activity_ID).ToArray();
                        db.RS_PM_Activity_Part.RemoveRange(db.RS_PM_Activity_Part.Where(a => a.Activity_ID == RS_PM_Activity.Activity_ID));
                        db.SaveChanges();
                        //fresh assign part to activity
                        RS_PM_Activity_Part mmpmapobj = new RS_PM_Activity_Part();
                        foreach (var item in RS_PM_Activity.Maintenance_Part_ID)
                        {

                            var AP = db.RS_PM_Activity_Part.Where(a => a.Maintenance_Part_ID == item);
                            mmpmapobj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            mmpmapobj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mmpmapobj.Inserted_Date = DateTime.Now;
                            mmpmapobj.Plant_ID = RS_PM_Activity.Plant_ID;
                            mmpmapobj.Shop_ID = RS_PM_Activity.Shop_ID;
                            mmpmapobj.Line_ID = RS_PM_Activity.Line_ID;
                            mmpmapobj.Station_ID = RS_PM_Activity.Station_ID;
                            mmpmapobj.Activity_ID = RS_PM_Activity.Activity_ID;
                            mmpmapobj.Machine_ID = RS_PM_Activity.Machine_ID;
                            mmpmapobj.Maintenance_Part_ID = Convert.ToDecimal(item);
                            db.RS_PM_Activity_Part.Add(mmpmapobj);
                            db.SaveChanges();

                        }

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.PMActivity;
                        globalData.messageDetail = ResourceModules.PMActivity + " " + ResourceMessages.Edit_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }
            }
            globalData.pageTitle = ResourceModules.PMActivity;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "PMActivity";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            var assigned_Part = db.RS_PM_Activity_Part.Where(a => a.Activity_ID != RS_PM_Activity.Activity_ID).Select(a => a.Maintenance_Part_ID).ToList();//allready assigned part list

            var allPart1 = db.RS_Maintenance_Machine_Part.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID).Select(m => m.Maintenance_Part_ID).ToList();
            var roleObj11 = (from a in db.RS_PM_Activity_Part
                             join b in db.RS_PM_Activity on a.Activity_ID equals b.Activity_ID
                             where b.Activity_ID == RS_PM_Activity.Activity_ID
                             select a.RS_Maintenance_Part.Maintenance_Part_ID).ToList();
            var assigned_Part11 = db.RS_PM_Activity_Part.Where(p => p.Machine_ID == RS_PM_Activity.Machine_ID && p.Activity_ID != RS_PM_Activity.Activity_ID).Select(a => a.Maintenance_Part_ID).Distinct().ToList();//allready assigned part list
            var res1 = allPart1.Except(assigned_Part11).ToList();
            //[RS_Maintenance_Part] part master 
            var Parts11 = db.RS_Maintenance_Part.Where(c => res1.Contains(c.Maintenance_Part_ID)).Select(c => new { c.Maintenance_Part_ID, c.Part_Name }).ToList();
            ViewBag.M_ID = new SelectList(db.RS_MT_Unit_Of_Measurement.OrderBy(c => c.Measurement_Name), "M_ID", "Measurement_Name", RS_PM_Activity.M_ID);

            ViewBag.Maintenance_Part_ID = new MultiSelectList(Parts11, "Maintenance_Part_ID", "Part_Name", roleObj11);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == PlantID).OrderBy(p => p.Plant_Name), "Plant_ID", "Plant_Name", RS_PM_Activity.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == PlantID), "Shop_ID", "Shop_Name", RS_PM_Activity.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_PM_Activity.Line_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_PM_Activity.Station_ID);
            ViewBag.Machine_ID = new SelectList(MachinesList, "Machine_ID", "Machine_Name", RS_PM_Activity.Machine_ID);
            ViewBag.Activity_Owner_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == PlantID), "Employee_ID", "Employee_Name", RS_PM_Activity.Activity_Owner_ID);
            ViewBag.EQP_ID = new SelectList(db.RS_PM_Equipment.Where(m => m.Machine_ID == RS_PM_Activity.Machine_ID && m.Is_PM_Equipment == true), "EQP_ID", "Equipment_Name", RS_PM_Activity.EQP_ID);
            ViewBag.PMActivity_Is_Value_Based = RS_PM_Activity.Is_Value_Based;
            return View(RS_PM_Activity);
        }
           
        

        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_PM_Activity RS_PM_Activity = db.RS_PM_Activity.Find(id);
            if (RS_PM_Activity == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.PMActivity;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "PMActivity";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_PM_Activity);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_PM_Activity RS_PM_Activity = db.RS_PM_Activity.Find(id);

            if (RS_PM_Activity.RS_PM_Activity_Part.Count > 0)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.PMActivity;
                globalData.messageDetail = ResourceMessages.CanNotDelete;
                TempData["globalData"] = globalData;
            }
            else
            {
                RS_PM_Activity_Part[] ActivityPart = db.RS_PM_Activity_Part.Where(a => a.Activity_ID ==id).ToArray();
                db.RS_PM_Activity_Part.RemoveRange(db.RS_PM_Activity_Part.Where(a => a.Activity_ID == id));
                db.SaveChanges();
                db.RS_PM_Activity.Remove(RS_PM_Activity);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.PMActivity;
                globalData.messageDetail = ResourceModules.PMActivity + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
            }
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
