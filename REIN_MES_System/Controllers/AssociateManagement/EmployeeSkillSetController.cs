using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.App_LocalResources;
using System.IO;
using System.Data.OleDb;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class EmployeeSkillSetController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_AM_Employee_SkillSet mmEmpSkillSetObj = new RS_AM_Employee_SkillSet();
        decimal empId = 0;
        decimal stationId = 0;
        decimal shopId = 0;
        FDSession fdSession = new FDSession();
        General generalObj = new General();
        // GET: EmployeeSkillSet
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = ResourceModules.Employee_Skill_Set;
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            var RS_AM_Employee_SkillSet = db.RS_AM_Employee_SkillSet.Include(m => m.RS_AM_Skill_Set).Include(m => m.RS_Employee).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations).Where(m => m.Plant_ID == plant_ID);
            return View(RS_AM_Employee_SkillSet.ToList());
        }

        // GET: EmployeeSkillSet/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Employee_SkillSet RS_AM_Employee_SkillSet = db.RS_AM_Employee_SkillSet.Find(id);
            if (RS_AM_Employee_SkillSet == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "SkillSet Allocation";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Employee_Skill_Set;
            globalData.contentFooter = ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Details; ;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Employee_SkillSet);
        }

        // GET: EmployeeSkillSet/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = ResourceModules.Employee_Skill_Set;
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set.Where(p => p.Plant_ID == plant_ID), "Skill_ID", "Skill_Set");
            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plant_ID), "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", 0);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(m => m.Station_ID == 0), "Station_ID", "Station_Name");
            return View();
        }

        // POST: EmployeeSkillSet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_AM_Employee_SkillSet RS_AM_Employee_SkillSet)
        {
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                stationId = RS_AM_Employee_SkillSet.Station_ID;
                empId = RS_AM_Employee_SkillSet.Employee_ID;
                if (RS_AM_Employee_SkillSet.IsEmpNoExists(empId, stationId))
                {
                    ModelState.AddModelError("Employee_ID", ResourceValidation.Exist);
                }
                else
                {
                    RS_AM_Employee_SkillSet.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    RS_AM_Employee_SkillSet.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_AM_Employee_SkillSet.Inserted_Date = DateTime.Now;
                    RS_AM_Employee_SkillSet.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_AM_Employee_SkillSet.Add(RS_AM_Employee_SkillSet);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "SkillSet Allocation";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set.Where(p => p.Plant_ID == plant_ID), "Skill_ID", "Skill_Set", RS_AM_Employee_SkillSet.Skill_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", RS_AM_Employee_SkillSet.Employee_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_AM_Employee_SkillSet.Shop_ID), "Line_ID", "Line_Name", RS_AM_Employee_SkillSet.Line_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Employee_SkillSet.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", RS_AM_Employee_SkillSet.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == RS_AM_Employee_SkillSet.Line_ID), "Station_ID", "Station_Name", RS_AM_Employee_SkillSet.Station_ID);
            return View(RS_AM_Employee_SkillSet);
        }

        // GET: EmployeeSkillSet/Edit/5
        public ActionResult Edit(decimal id)
        {
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Employee_SkillSet RS_AM_Employee_SkillSet = db.RS_AM_Employee_SkillSet.Find(id);
            if (RS_AM_Employee_SkillSet == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "SkillSet Allocation";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set.Where(p => p.Plant_ID == plant_ID), "Skill_ID", "Skill_Set", RS_AM_Employee_SkillSet.Skill_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", RS_AM_Employee_SkillSet.Employee_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_AM_Employee_SkillSet.Shop_ID), "Line_ID", "Line_Name", RS_AM_Employee_SkillSet.Line_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", RS_AM_Employee_SkillSet.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == RS_AM_Employee_SkillSet.Line_ID), "Station_ID", "Station_Name", RS_AM_Employee_SkillSet.Station_ID);
            return View(RS_AM_Employee_SkillSet);
        }

        // POST: EmployeeSkillSet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeSkill_Id,Plant_ID,Shop_ID,Line_ID,Station_ID,Employee_ID,Skill_ID,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_AM_Employee_SkillSet RS_AM_Employee_SkillSet)
        {
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            mmEmpSkillSetObj = new RS_AM_Employee_SkillSet();
            if (ModelState.IsValid)
            {
                stationId = RS_AM_Employee_SkillSet.Station_ID;
                empId = RS_AM_Employee_SkillSet.Employee_ID;
                if (RS_AM_Employee_SkillSet.IsEmpNoExists(empId, stationId))
                {
                    ModelState.AddModelError("Employee_ID", ResourceValidation.Exist);
                }
                else
                {
                    mmEmpSkillSetObj = db.RS_AM_Employee_SkillSet.Find(RS_AM_Employee_SkillSet.EmployeeSkill_Id);
                    mmEmpSkillSetObj.Shop_ID = RS_AM_Employee_SkillSet.Shop_ID;
                    mmEmpSkillSetObj.Line_ID = RS_AM_Employee_SkillSet.Line_ID;
                    mmEmpSkillSetObj.Station_ID = RS_AM_Employee_SkillSet.Station_ID;
                    mmEmpSkillSetObj.Plant_ID = 1;// RS_AM_Employee_SkillSet.Plant_ID;
                    mmEmpSkillSetObj.Employee_ID = RS_AM_Employee_SkillSet.Employee_ID;
                    mmEmpSkillSetObj.Skill_ID = RS_AM_Employee_SkillSet.Skill_ID;
                    mmEmpSkillSetObj.Inserted_Date = db.RS_AM_Employee_SkillSet.Find(RS_AM_Employee_SkillSet.EmployeeSkill_Id).Inserted_Date;
                    mmEmpSkillSetObj.Inserted_User_ID = db.RS_AM_Employee_SkillSet.Find(RS_AM_Employee_SkillSet.EmployeeSkill_Id).Inserted_User_ID;
                    mmEmpSkillSetObj.Inserted_Host = db.RS_AM_Employee_SkillSet.Find(RS_AM_Employee_SkillSet.EmployeeSkill_Id).Inserted_Host;
                    mmEmpSkillSetObj.Is_Edited = true;
                    mmEmpSkillSetObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmEmpSkillSetObj.Updated_Date = DateTime.Now;
                    mmEmpSkillSetObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Manager;
                    globalData.messageDetail = ResourceModules.Manager + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;


                    db.Entry(mmEmpSkillSetObj).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "SkillSet Allocation";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            //ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set, "Skill_ID", "Skill_Set", RS_AM_Employee_SkillSet.Skill_ID);
            //ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Employee_SkillSet.Employee_ID);
            //ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Employee_SkillSet.Line_ID);
            //// ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Employee_SkillSet.Plant_ID);
            //ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Employee_SkillSet.Shop_ID);
            //ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_AM_Employee_SkillSet.Station_ID);

            ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set.Where(p => p.Plant_ID == plant_ID), "Skill_ID", "Skill_Set", RS_AM_Employee_SkillSet.Skill_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", RS_AM_Employee_SkillSet.Employee_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_AM_Employee_SkillSet.Shop_ID), "Line_ID", "Line_Name", RS_AM_Employee_SkillSet.Line_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_ID), "Shop_ID", "Shop_Name", RS_AM_Employee_SkillSet.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == RS_AM_Employee_SkillSet.Line_ID), "Station_ID", "Station_Name", RS_AM_Employee_SkillSet.Station_ID);
            return View(RS_AM_Employee_SkillSet);
        }

        // GET: EmployeeSkillSet/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Employee_SkillSet RS_AM_Employee_SkillSet = db.RS_AM_Employee_SkillSet.Find(id);
            if (RS_AM_Employee_SkillSet == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "SkillSet Allocation";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Employee_SkillSet);
        }

        // POST: EmployeeSkillSet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_AM_Employee_SkillSet RS_AM_Employee_SkillSet = db.RS_AM_Employee_SkillSet.Find(id);
                db.RS_AM_Employee_SkillSet.Remove(RS_AM_Employee_SkillSet);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_AM_Employee_SkillSet", "EmployeeSkill_Id", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Skill_Set_Allocation;
                globalData.messageDetail = ResourceModules.Employee_Skill_Set + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Skill_Set_Allocation;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete");
            }

        }

        #region Skill Set Allocation
        public ActionResult SkillSetAllocation()
        {
            int userid = ((FDSession)this.Session["FDSession"]).userId;
            //userid = 11808;
            int plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Line_ID = new SelectList((from line in db.RS_Lines
                                              join superline in db.RS_AM_Line_Supervisor_Mapping
                                              on
                                              line.Line_ID equals superline.Line_ID
                                              where superline.Employee_ID == userid && superline.Plant_ID == plant_ID
                                              select line).ToList(), "Line_ID", "Line_Name");

            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "EmployeeSkillSet";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }
        [HttpPost]
        public ActionResult CreateSkillSetAllocationTable(decimal lineid)
        {
            string table = "<table id='myTable' class='table table-bordered table-striped datatable_completes'><thead><tr>";
            try
            {
                int userid = ((FDSession)this.Session["FDSession"]).userId;
                //  decimal lineid=0;
                var Employees = (from emp in db.RS_Employee
                                 join cellmember in db.RS_Assign_OperatorToSupervisor
                                 on
                                 emp.Employee_ID equals cellmember.AssignedOperator_ID
                                 where cellmember.Supervisor_ID == userid && cellmember.Line_ID == lineid
                                 && emp.Is_Deleted == null
                                 select emp).Distinct().ToList();

                List<RS_Stations> stations = (from station in db.RS_Stations
                                              where station.Line_ID == lineid
                                              select station).Distinct().ToList();

                table += "<th>Employees/Stations</th>";
                for (int i = 0; i < stations.Count(); i++)
                {
                    table += "<th data-id=" + stations[i].Station_Name + ">" + stations[i].Station_Name + "</th>";
                }
                table += "</tr></thead><tbody>";
                for (int i = 0; i < Employees.Count(); i++)
                {
                    table += "<tr><td>" + Employees[i].Employee_Name + "</td>";
                    for (int j = 0; j < stations.Count(); j++)
                    {

                        decimal station = stations[j].Station_ID;
                        decimal emp = Employees[i].Employee_ID;
                        if (db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == emp && x.Station_ID == station && x.Line_ID == lineid).Count() > 0)
                        {
                            if (db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == emp && x.Station_ID == station && x.Line_ID == lineid).FirstOrDefault().Skill_ID == 1)
                            {
                                table += "<td><div class=''><div class='skill-block'><div class='white-font col-md-5 radio-square quarter1' name=" + Employees[i].Employee_ID
                              + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                              + "_" + stations[j].Station_ID + "_" + j + "_1 value='1' data-id=divchk style='background:rgb(61, 185, 0)'"
                              + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_1  onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,1)'>2</div><div class='white-font col-md-5 radio-square quarter2' name=" + Employees[i].Employee_ID
                              + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                              + "_" + stations[j].Station_ID + "_" + j + "_2 value='2' data-id=divchk style='background:red'"
                              + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_2 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,2)' >4</div></div><div class='skill-block'><div class='white-font col-md-5 radio-square quarter3' name=" + Employees[i].Employee_ID
                              + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                              + "_" + stations[j].Station_ID + "_" + j + "_4 value='4' data-id=divchk style='background:red'"
                              + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_4 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,4)'>8</div><div class='white-font col-md-5 radio-square quarter4' name=" + Employees[i].Employee_ID
                              + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                              + "_" + stations[j].Station_ID + "_" + j + "_3 value='3' data-id=divchk style='background:red'"
                              + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_3 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,3)'>6</div></div><div class='skill-refresh'  title='Reset Skill Set' onclick='resetSkillset(" + stations[j].Station_ID + "," + Employees[i].Employee_ID + ",this)'><i class='fa fa-refresh'></i></div></div></td>";
                            }
                            else if (db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == emp && x.Station_ID == station && x.Line_ID == lineid).FirstOrDefault().Skill_ID == 2)
                            {
                                table += "<td><div class=''><div class='skill-block'><div class='white-font col-md-5 radio-square quarter1' name=" + Employees[i].Employee_ID
                             + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                             + "_" + stations[j].Station_ID + "_" + j + "_1 value='1' data-id=divchk style='background:rgb(61, 185, 0)'"
                             + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_1  onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,1)'>2</div><div class='white-font col-md-5 radio-square quarter2' name=" + Employees[i].Employee_ID
                             + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                             + "_" + stations[j].Station_ID + "_" + j + "_2 value='2' data-id=divchk  style='background:rgb(61, 185, 0)'"
                             + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_2 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,2)' >4</div></div><div class='skill-block'><div class='white-font col-md-5 radio-square quarter3' name=" + Employees[i].Employee_ID
                             + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                             + "_" + stations[j].Station_ID + "_" + j + "_4 value='4' data-id=divchk style='background:red'"
                             + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_4 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,4)'>8</div><div class='white-font col-md-5 radio-square quarter4' name=" + Employees[i].Employee_ID
                             + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                             + "_" + stations[j].Station_ID + "_" + j + "_3 value='3' data-id=divchk style='background:red'"
                             + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_3 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,3)'>6</div></div><div class='skill-refresh' title='Reset Skill Set' onclick='resetSkillset(" + stations[j].Station_ID + "," + Employees[i].Employee_ID + ",this)'><i class='fa fa-refresh'></i></div></div></td>";
                            }
                            else if (db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == emp && x.Station_ID == station && x.Line_ID == lineid).FirstOrDefault().Skill_ID == 3)
                            {
                                table += "<td><div class=''><div class='skill-block'><div class='white-font col-md-5 radio-square quarter1' name=" + Employees[i].Employee_ID
                            + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                            + "_" + stations[j].Station_ID + "_" + j + "_1 value='1' data-id=divchk style='background:rgb(61, 185, 0)'"
                            + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_1  onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,1)'>2</div><div class='white-font col-md-5 radio-square quarter2' name=" + Employees[i].Employee_ID
                            + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                            + "_" + stations[j].Station_ID + "_" + j + "_2 value='2' data-id=divchk style='background:rgb(61, 185, 0)'"
                            + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_2 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,2)' >4</div></div><div class='skill-block'><div class='white-font col-md-5 radio-square quarter3' name=" + Employees[i].Employee_ID
                            + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                            + "_" + stations[j].Station_ID + "_" + j + "_4 value='4' data-id=divchk style='background:red'"
                            + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_4 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,4)'>8</div><div class='white-font col-md-5 radio-square quarter4' name=" + Employees[i].Employee_ID
                            + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                            + "_" + stations[j].Station_ID + "_" + j + "_3 value='3' data-id=divchk style='background:rgb(61, 185, 0)'"
                            + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_3 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,3)'>6</div></div><div class='skill-refresh'  title='Reset Skill Set' onclick='resetSkillset(" + stations[j].Station_ID + "," + Employees[i].Employee_ID + ",this)'><i class='fa fa-refresh'></i></div></div></td>";
                            }
                            else if (db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == emp && x.Station_ID == station && x.Line_ID == lineid).FirstOrDefault().Skill_ID == 4)
                            {
                                table += "<td><div class=''><div class='skill-block'><div class='white-font col-md-5 radio-square quarter1' name=" + Employees[i].Employee_ID
                           + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                           + "_" + stations[j].Station_ID + "_" + j + "_1 value='1' data-id=divchk style='background:rgb(61, 185, 0)'"
                           + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_1  onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,1)'>2</div><div class='white-font col-md-5 radio-square quarter2' name=" + Employees[i].Employee_ID
                           + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                           + "_" + stations[j].Station_ID + "_" + j + "_2 value='2' data-id=divchk style='background:rgb(61, 185, 0)'"
                           + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_2 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,2)' >4</div></div><div class='skill-block'><div class='white-font col-md-5 radio-square quarter3' name=" + Employees[i].Employee_ID
                           + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                           + "_" + stations[j].Station_ID + "_" + j + "_4 value='4' data-id=divchk style='background:rgb(61, 185, 0)'"
                           + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_4 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,4)'>8</div><div class='white-font col-md-5 radio-square quarter4' name=" + Employees[i].Employee_ID
                           + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                           + "_" + stations[j].Station_ID + "_" + j + "_3 value='3' data-id=divchk style='background:rgb(61, 185, 0)'"
                           + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_3 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,3)'>6</div></div><div class='skill-refresh'  title='Reset Skill Set' onclick='resetSkillset(" + stations[j].Station_ID + "," + Employees[i].Employee_ID + ",this)'><i class='fa fa-refresh'></i></div></div></td>";
                            }
                        }
                        else
                        {
                            table += "<td><div class=''><div class='skill-block'><div class='black-font col-md-5 radio-square quarter1' name=" + Employees[i].Employee_ID
                           + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                           + "_" + stations[j].Station_ID + "_" + j + "_1 value='1' data-id=divchk"
                           + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_1  onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,1)'>2</div><div class='black-font col-md-5 radio-square quarter2' name=" + Employees[i].Employee_ID
                           + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                           + "_" + stations[j].Station_ID + "_" + j + "_2 value='2' data-id=divchk"
                           + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_2 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,2)' >4</div></div><div class='skill-block'><div class='black-font col-md-5 radio-square quarter3' name=" + Employees[i].Employee_ID
                           + "" + stations[j].Station_ID + j + "  id=" + Employees[i].Employee_ID
                           + "_" + stations[j].Station_ID + "_" + j + "_4 value='4' data-id=divchk"
                           + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_4 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,4)'>8</div><div class='black-font col-md-5 radio-square quarter4' name=" + Employees[i].Employee_ID
                           + "" + stations[j].Station_ID + j + " id=" + Employees[i].Employee_ID
                           + "_" + stations[j].Station_ID + "_" + j + "_3 value='3' data-id=divchk"
                           + Employees[i].Employee_ID + "_" + stations[j].Station_ID + "_3 onclick='SaveSkillSet(" + Employees[i].Employee_ID + "," + stations[j].Station_ID + ",this,3)'>6</div></div></div></td>";
                        }
                    }
                    table += "</tr>";
                }
                table += "</tbody></table>";
            }
            catch (Exception ex)
            {

            }
            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "EmployeeSkillSet";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            var jsonresult = Json(table, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = Int32.MaxValue;
            return jsonresult;
        }

        public ActionResult SaveEmployeeSkillSet(string controlid, decimal empid, decimal stationid, int skilllevel, decimal lineid)
        {
            string success = "";

            if (db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == empid && x.Station_ID == stationid && x.Line_ID == lineid).Count() > 0)
            {
                RS_AM_Employee_SkillSet empskill = new RS_AM_Employee_SkillSet();
                empskill = db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == empid && x.Station_ID == stationid && x.Line_ID == lineid).FirstOrDefault();
                empskill.Skill_ID = skilllevel;
                empskill.Updated_Date = DateTime.Now;
                empskill.Updated_Host = HttpContext.Request.UserHostAddress;
                empskill.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                empskill.Is_Edited = true;
                db.Entry(empskill).State = EntityState.Modified;
                db.SaveChanges();
                success += "Employee Skill Set Edited Sucessfully.!";
                var jsonresult = Json(CreateSkillSetAllocationTable(lineid), JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;

            }
            else
            {
                RS_AM_Employee_SkillSet empSkill = new RS_AM_Employee_SkillSet();
                empSkill.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                empSkill.Shop_ID = db.RS_Lines.Where(x => x.Line_ID == lineid).Select(x => x.Shop_ID).FirstOrDefault();
                empSkill.Line_ID = lineid;
                empSkill.Station_ID = stationid;
                empSkill.Employee_ID = empid;
                empSkill.Skill_ID = skilllevel;
                empSkill.Inserted_Date = DateTime.Now;
                empSkill.Inserted_Host = HttpContext.Request.UserHostAddress;
                empSkill.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                db.RS_AM_Employee_SkillSet.Add(empSkill);
                db.SaveChanges();
                success += "Employee Skill Set Created Sucessfully.!";
                var jsonresult = Json(CreateSkillSetAllocationTable(lineid), JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;
            }
            // return Json(success, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetEmployeeSkillSet(decimal empid, decimal stationid, decimal lineid)
        {
            RS_AM_Employee_SkillSet empskill = new RS_AM_Employee_SkillSet();
            empskill = db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == empid && x.Station_ID == stationid && x.Line_ID == lineid).FirstOrDefault();
            db.RS_AM_Employee_SkillSet.Remove(empskill);
            db.SaveChanges();
            var jsonresult = Json(CreateSkillSetAllocationTable(lineid), JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = Int32.MaxValue;
            return jsonresult;
        }

        public ActionResult Upload()
        {
            if (TempData["SSRecords"] != null)
            {
                ViewBag.SSRecords = TempData["SSRecords"];
                ViewBag.SSDataTable = TempData["SSDataTable"];
            }
            globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "EmployeeSkillSet";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }
        [HttpPost]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(HttpPostedFileBase files)
        {
            GlobalOperations globalOperations = new GlobalOperations();
            string fileName = Path.GetFileName(files.FileName);
            string fileExtension = Path.GetExtension(files.FileName);
            string fileLocation = Server.MapPath("~/App_Data/" + fileName);
            DataTable dt = globalOperations.ExcelToDataTable(files, fileLocation, fileExtension);
            int J = 0;

            ExcelATSkillSetRecords[] SSRecords = new ExcelATSkillSetRecords[dt.Rows.Count];
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow drss in dt.Rows)
                {
                    RS_AM_Employee_SkillSet mm_empSkill = new RS_AM_Employee_SkillSet();

                    #region Assigned Skill Set to Employees
                    ExcelATSkillSetRecords ssmsg = new ExcelATSkillSetRecords();

                    string EmployeeNo = drss["Token"].ToString();
                    string StationName = drss["Station_Name"].ToString();
                    string skilllevel = drss["Skill_Level"].ToString();
                    string Line_Name = drss["Line_Name"].ToString();
                    decimal empid = db.RS_Employee.Where(x => x.Employee_No.ToString().ToLower() == EmployeeNo.ToLower().Trim()).FirstOrDefault().Employee_ID;
                    decimal stationid = db.RS_Stations.Where(x => x.Station_Name.ToLower() == StationName.ToLower().Trim()).FirstOrDefault().Station_ID;
                    decimal lineid = db.RS_Lines.Where(x => x.Line_Name.ToLower() == Line_Name.ToLower().Trim()).FirstOrDefault().Line_ID;
                    ssmsg.SkillLevel = Convert.ToInt32(skilllevel);
                    ssmsg.EmployeNo = EmployeeNo;
                    ssmsg.EmployeeName = db.RS_Employee.Where(x => x.Employee_No.ToLower() == EmployeeNo.ToLower().Trim()).FirstOrDefault().Employee_Name;
                    ssmsg.StationName = StationName;

                    if (db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == empid && x.Station_ID == stationid && x.Line_ID == lineid).Count() > 0)
                    {
                        RS_AM_Employee_SkillSet empskill = new RS_AM_Employee_SkillSet();
                        empskill = db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == empid && x.Station_ID == stationid && x.Line_ID == lineid).FirstOrDefault();
                        empskill.Skill_ID = Convert.ToDecimal(skilllevel);
                        empskill.Updated_Date = DateTime.Now;
                        empskill.Is_Edited = true;
                        empskill.Updated_Host = HttpContext.Request.UserHostAddress;
                        empskill.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.Entry(empskill).State = EntityState.Modified;
                        db.SaveChanges();
                        ssmsg.SS_Error_Sucess = "Employee Skill Set Edited Sucessfully";
                    }
                    else
                    {
                        RS_AM_Employee_SkillSet empSkill = new RS_AM_Employee_SkillSet();
                        empSkill.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        empSkill.Shop_ID = db.RS_Lines.Where(x => x.Line_ID == lineid).Select(x => x.Shop_ID).FirstOrDefault();
                        empSkill.Line_ID = lineid;
                        empSkill.Station_ID = stationid;
                        empSkill.Employee_ID = empid;
                        empSkill.Skill_ID = Convert.ToDecimal(skilllevel);
                        empSkill.Inserted_Date = DateTime.Now;
                        empSkill.Inserted_Host = HttpContext.Request.UserHostAddress;
                        empSkill.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.RS_AM_Employee_SkillSet.Add(empSkill);
                        db.SaveChanges();
                        ssmsg.SS_Error_Sucess = "Employee Skill Set Added Sucessfully";
                    }
                    SSRecords[J] = ssmsg;
                    J = J + 1;
                    #endregion
                }
                TempData["SSRecords"] = SSRecords;
                TempData["SSDataTable"] = dt;
                ViewBag.SSRecords = SSRecords;
                ViewBag.SSDataTable = dt;

                globalData.pageTitle = ResourceModules.Skill_Set_Allocation;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "EmployeeSkillSet";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
            }

            return View();
        }

        #endregion

        public ActionResult GetStationListByLineID(int lineId)
        {
            try
            {

                var st = from station in db.RS_Stations
                         where station.Line_ID == lineId && (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult ExcelUpload()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            List<SelectListItem> listModel = new List<SelectListItem>();
            ViewBag.Line_ID = new SelectList(listModel);

            if (TempData["OrderUploadRecords"] != null)
            {
                ViewBag.OrderUploadRecords = TempData["OrderUploadRecords"];
            }

            globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.subTitle = ResourceGlobal.Upload;
            globalData.controllerName = "EmployeeSkillSet";
            globalData.actionName = ResourceGlobal.Upload;
            globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelUpload(ExcelSkillSetAllocation formData)
        {
            int plantId = 0, shopId = 0, lineId = 0;
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

                if (dt.Rows.Count > 0)
                {
                    EmployeeSkillSetUploadRecords[] orderUploadRecordsObj = new EmployeeSkillSetUploadRecords[dt.Rows.Count];
                    RS_Maintenance_Machine_Part mmOrderCreationObj = new RS_Maintenance_Machine_Part();

                    int i = 0;
                    foreach (DataRow checkListRow in dt.Rows)
                    {
                        String stationName = checkListRow[0].ToString() != null ? checkListRow[0].ToString().Trim() : null;
                        String OpratorTokenNumber = checkListRow[1].ToString() != null ? checkListRow[1].ToString().Trim() : null;
                        int SkillSet = checkListRow[2].ToString() != "" ? Convert.ToInt32(checkListRow[2].ToString().Trim()) : 0;
                        SkillSet = SkillSet == 0 ? 0 : ((SkillSet / 2) == 0 ? 1 : (SkillSet / 2));
                        orderUploadRecordsObj[i] = new EmployeeSkillSetUploadRecords();
                        EmployeeSkillSetUploadRecords orderUploadObj = new EmployeeSkillSetUploadRecords();
                        RS_AM_Employee_SkillSet obj = new RS_AM_Employee_SkillSet();
                        orderUploadObj.StationName = stationName;
                        orderUploadObj.OperatorTokenNumber = OpratorTokenNumber;
                        orderUploadObj.SkillSet = Convert.ToInt32(SkillSet);

                        var station = db.RS_Stations.Any(m => m.Station_Name == stationName && m.Line_ID == lineId);
                        if (station)
                        {
                            var stationId = db.RS_Stations.Where(m => m.Station_Name == stationName && m.Line_ID == lineId).Select(m => m.Station_ID).FirstOrDefault();
                            var userId = ((FDSession)this.Session["FDSession"]).userId;
                            var supervisorId = db.RS_AM_Line_Supervisor_Mapping.Where(m => m.Line_ID == lineId && m.Employee_ID == userId).Select(m => m.Employee_ID).FirstOrDefault();
                            var operatorIds = db.RS_Assign_OperatorToSupervisor.Where(m => m.Line_ID == lineId && m.Supervisor_ID == supervisorId).Select(m => m.AssignedOperator_ID).ToArray();
                            string[] EmpNos = db.RS_Employee.Where(c => operatorIds.Contains(c.Employee_ID)).Select(c => c.Employee_No).ToArray();
                            var results = Array.FindAll(EmpNos, s => s.Equals(OpratorTokenNumber));
                            if (results.Count() > 0)
                            {
                                var EmpId = db.RS_Employee.Where(m => m.Employee_No == OpratorTokenNumber).Select(m => m.Employee_ID).FirstOrDefault();
                                if (SkillSet > 0 && SkillSet < 5)
                                {
                                    if (db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == EmpId && x.Station_ID == stationId && x.Line_ID == lineId).Count() > 0)
                                    {
                                        RS_AM_Employee_SkillSet empskill = new RS_AM_Employee_SkillSet();
                                        empskill = db.RS_AM_Employee_SkillSet.Where(x => x.Employee_ID == EmpId && x.Station_ID == stationId && x.Line_ID == lineId).FirstOrDefault();
                                        empskill.Skill_ID = Convert.ToDecimal(SkillSet);
                                        empskill.Updated_Date = DateTime.Now;
                                        empskill.Is_Edited = true;
                                        empskill.Updated_Host = HttpContext.Request.UserHostAddress;
                                        empskill.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        db.Entry(empskill).State = EntityState.Modified;
                                        db.SaveChanges();
                                        orderUploadObj.SS_Error_Sucess = "Employee Skill Set Edited Sucessfully";
                                    }
                                    else
                                    {
                                        obj.Employee_ID = EmpId;
                                        obj.Line_ID = lineId;
                                        obj.Plant_ID = plantId;
                                        obj.Shop_ID = shopId;
                                        obj.Skill_ID = SkillSet;
                                        obj.Station_ID = stationId;
                                        obj.Inserted_Date = DateTime.Now;
                                        obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                        db.RS_AM_Employee_SkillSet.Add(obj);
                                        db.SaveChanges();
                                        orderUploadObj.SS_Error_Sucess = "Employee Skill Set Added Sucessfully";
                                    }
                                }
                                else
                                {
                                    orderUploadObj.SS_Error_Sucess = "Error:Enter correct skillset";
                                }
                            }
                            else
                            {
                                orderUploadObj.SS_Error_Sucess = "Error:Operator is not available";
                            }
                        }
                        else
                        {
                            orderUploadObj.SS_Error_Sucess = "Error:Station is not available";
                        }

                        orderUploadRecordsObj[i] = orderUploadObj;
                        i = i + 1;
                    }


                    TempData["OrderUploadRecords"] = orderUploadRecordsObj;
                    //TempData["ChecklistDataTable"] = dt;
                    ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                    //ViewBag.dt = qualityChecklistDt;
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Employee_Skill_Set;
                    globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceMessages.Upload_Success;
                    globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
                    globalData.subTitle = ResourceGlobal.Upload;
                    globalData.controllerName = "EmployeeSkillSet";
                    globalData.actionName = ResourceGlobal.Upload;
                    globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Employee_Skill_Set + " " + ResourceGlobal.Form;
                    ViewBag.GlobalDataModel = globalData;

                    ViewBag.createdOrders = createdOrders;
                }
            }
            //return PartialView("QualityChecklistDetails");

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", shopId);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", lineId);

            return View();
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
