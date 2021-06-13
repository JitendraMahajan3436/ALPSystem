using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class OperatorStationAllocationController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_AM_Operator_Station_Allocation mmOperatorAllocationObj = new RS_AM_Operator_Station_Allocation();
        decimal operatorId = 0;
        decimal stationid = 0;
        decimal shiftid = 0;
        General generalObj = new General();

        /*	    Action Name		    : Index
        *		Description		    : To Display the operators allocation information in grid
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: OperatorStationAllocation
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceOperatorAllocation.Operator;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = ResourceOperatorAllocation.Site_Variable_Operator;
            globalData.actionName = ResourceOperatorAllocation.Site_Variable_Index;
            globalData.contentTitle = ResourceOperatorAllocation.Operator_Title_Operator_Lists;
            globalData.contentFooter = ResourceOperatorAllocation.Operator_Title_Operator_Lists;
            ViewBag.GlobalDataModel = globalData;

            var RS_AM_Operator_Station_Allocation = db.RS_AM_Operator_Station_Allocation.Include(m => m.RS_AM_Skill_Set).Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations).Include(m => m.RS_Employee);
            return View(RS_AM_Operator_Station_Allocation.ToList());
        }


        /*	    Action Name		    : Details
         *		Description		    : To show the operator allocation detailed information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : id of employee whose information is to be displayed 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: OperatorStationAllocation/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation = db.RS_AM_Operator_Station_Allocation.Find(id);
            if (RS_AM_Operator_Station_Allocation == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceOperatorAllocation.Operator;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Operators";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceOperatorAllocation.Operator_Title_Operator_Detail;
            globalData.contentFooter = ResourceOperatorAllocation.Operator_Title_Operator_Detail;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Operator_Station_Allocation);
        }


        /*	    Action Name		    : Create
        *		Description		    : To read the operator alloction info which is to be saved
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: OperatorStationAllocation/Create
        public ActionResult Create()
        {

            globalData.pageTitle = ResourceOperatorAllocation.Operator;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = ResourceOperatorAllocation.Site_Variable_Operator;
            globalData.actionName = ResourceOperatorAllocation.Site_Variable_Index;
            globalData.contentTitle = ResourceOperatorAllocation.Operator_Title_Operator_Lists;
            globalData.contentFooter = ResourceOperatorAllocation.Operator_Title_Operator_Lists;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name");
            ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set, "Skill_ID", "Skill_Set");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            //  ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name",0);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", 0);
            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(m => m.Station_ID == 0), "Station_ID", "Station_Name");
            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(m => m.Employee_ID == 0 && m.Is_Deleted != true), "Employee_ID", "Employee_Name");
            ViewBag.Operator_ID = new SelectList(db.RS_Employee.Where(m => m.Employee_ID == 0 && m.Is_Deleted != true), "Employee_ID", "Employee_Name");

            return View();
        }


        /*	    Action Name		    : Create
         *		Description		    : To save the operators allocation information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : RS_AM_Operator_Station_Allocation object 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // POST: OperatorStationAllocation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OSM_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Employee_ID,Skill_ID,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Shift_ID,Allocation_Date")] RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //operatorId = RS_AM_Operator_Station_Allocation.Employee_ID;
                    // stationid = RS_AM_Operator_Station_Allocation.Station_ID;
                    //shiftid = Convert.ToDecimal(RS_AM_Operator_Station_Allocation.Shift_ID);
                    //if (RS_AM_Operator_Station_Allocation.IsOperatorToOneStationExists(operatorId, stationid, shiftid))
                    //{
                    //    globalData.isErrorMessage = true;
                    //    globalData.messageTitle = "Already Exist";
                    //    globalData.messageDetail = (ResourceAddUsers.Operator_Error_Operator_Name_Exists);


                    //    // ModelState.AddModelError("Employee_Name", ResourceAddUsers.Operator_Error_Operator_Name_Exists);
                    //}
                    //else
                    // {

                    RS_AM_Operator_Station_Allocation.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_AM_Operator_Station_Allocation.Plant_ID = 1;
                    RS_AM_Operator_Station_Allocation.Inserted_Date = DateTime.Now;
                    RS_AM_Operator_Station_Allocation.Allocation_Date = DateTime.Now;
                    RS_AM_Operator_Station_Allocation.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    db.RS_AM_Operator_Station_Allocation.Add(RS_AM_Operator_Station_Allocation);
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceOperatorAllocation.Operator;
                    globalData.messageDetail = ResourceOperatorAllocation.Operator_Success_Operator_Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                    //}
                }

                globalData.pageTitle = ResourceOperatorAllocation.Operator;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Operators";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceOperatorAllocation.Operator_Title_Add_Operator;
                globalData.contentFooter = ResourceOperatorAllocation.Operator_Title_Add_Operator;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set, "Skill_ID", "Skill_Set", RS_AM_Operator_Station_Allocation.Skill_ID);
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Inserted_User_ID);
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Updated_User_ID);
                ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Operator_Station_Allocation.Line_ID);
                //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Operator_Station_Allocation.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Operator_Station_Allocation.Shop_ID);
                ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_AM_Operator_Station_Allocation.Station_ID);
                ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name", RS_AM_Operator_Station_Allocation.Shift_ID);
                ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(m => m.Is_Deleted != true), "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Employee_ID);
                ViewBag.Operator_ID = new SelectList(db.RS_Employee.Where(m => m.Is_Deleted != true), "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Employee_ID);
                return View(RS_AM_Operator_Station_Allocation);
            }
            catch (Exception ex)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceOperatorAllocation.Operator;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }

        }


        /*	    Action Name		    : Edit
        *		Description		    : To read the operator allocation information which is to be edited
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: OperatorStationAllocation/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation = db.RS_AM_Operator_Station_Allocation.Find(id);
            if (RS_AM_Operator_Station_Allocation == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceOperatorAllocation.Operator;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Operator";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceOperatorAllocation.Operator_Title_Edit_Operator;
            globalData.contentFooter = ResourceOperatorAllocation.Operator_Title_Edit_Operator;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set, "Skill_ID", "Skill_Set", RS_AM_Operator_Station_Allocation.Skill_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Operator_Station_Allocation.Line_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Operator_Station_Allocation.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Operator_Station_Allocation.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_AM_Operator_Station_Allocation.Station_ID);
            ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name", RS_AM_Operator_Station_Allocation.Shift_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(m => m.Is_Deleted != true), "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Employee_ID);
            ViewBag.Operator_ID = new SelectList(db.RS_Employee.Where(m => m.Is_Deleted != true), "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Employee_ID);
            return View(RS_AM_Operator_Station_Allocation);

        }


        /*	    Action Name		    : Edit
        *		Description		    : To edit the operator allocation information
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : RS_AM_Operator_Station_Allocation object 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // POST: OperatorStationAllocation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OSM_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Employee_ID,Skill_ID,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Shift_ID")] RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation)
        {
            mmOperatorAllocationObj = new RS_AM_Operator_Station_Allocation();
            if (ModelState.IsValid)
            {
                operatorId = RS_AM_Operator_Station_Allocation.Employee_ID;
                stationid = RS_AM_Operator_Station_Allocation.Station_ID;
                shiftid = Convert.ToDecimal(RS_AM_Operator_Station_Allocation.Shift_ID);
                //if (RS_AM_Operator_Station_Allocation.IsOperatorToOneStationExists(operatorId, stationid, shiftid))
                //{
                // globalData.isErrorMessage = true;
                // globalData.messageTitle = "Already Exist";
                // globalData.messageDetail = (ResourceAddUsers.Operator_Error_Operator_Name_Exists);


                // ModelState.AddModelError("Employee_Name", ResourceAddUsers.Operator_Error_Operator_Name_Exists);
                // }
                //else
                //{

                mmOperatorAllocationObj = db.RS_AM_Operator_Station_Allocation.Find(RS_AM_Operator_Station_Allocation.OSM_ID);
                mmOperatorAllocationObj.Shop_ID = RS_AM_Operator_Station_Allocation.Shop_ID;
                mmOperatorAllocationObj.Plant_ID = RS_AM_Operator_Station_Allocation.Plant_ID;
                mmOperatorAllocationObj.Employee_ID = RS_AM_Operator_Station_Allocation.Employee_ID;
                mmOperatorAllocationObj.Shift_ID = RS_AM_Operator_Station_Allocation.Shift_ID;
                mmOperatorAllocationObj.Inserted_Date = db.RS_AM_Operator_Station_Allocation.Find(RS_AM_Operator_Station_Allocation.OSM_ID).Inserted_Date;
                mmOperatorAllocationObj.Inserted_User_ID = db.RS_AM_Operator_Station_Allocation.Find(RS_AM_Operator_Station_Allocation.OSM_ID).Inserted_User_ID;
                mmOperatorAllocationObj.Inserted_Host = db.RS_AM_Operator_Station_Allocation.Find(RS_AM_Operator_Station_Allocation.OSM_ID).Inserted_Host;

                mmOperatorAllocationObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mmOperatorAllocationObj.Updated_Date = DateTime.Now;
                mmOperatorAllocationObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                mmOperatorAllocationObj.Is_Edited = true;
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceOperatorAllocation.Operator;
                globalData.messageDetail = ResourceOperatorAllocation.Operator_Success_Operator_Edit_Success;
                TempData["globalData"] = globalData;
                db.Entry(mmOperatorAllocationObj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
                //}
            }


            globalData.pageTitle = ResourceOperatorAllocation.Operator;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Operators";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceOperatorAllocation.Operator_Title_Edit_Operator;
            globalData.contentFooter = ResourceOperatorAllocation.Operator_Title_Edit_Operator;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Skill_ID = new SelectList(db.RS_AM_Skill_Set, "Skill_ID", "Skill_Set", RS_AM_Operator_Station_Allocation.Skill_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Updated_User_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Operator_Station_Allocation.Line_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Operator_Station_Allocation.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Operator_Station_Allocation.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_AM_Operator_Station_Allocation.Station_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(m => m.Is_Deleted != true), "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Employee_ID);
            ViewBag.Operator_ID = new SelectList(db.RS_Employee.Where(m => m.Is_Deleted != true), "Employee_ID", "Employee_Name", RS_AM_Operator_Station_Allocation.Employee_ID);
            ViewBag.Shift_ID = new SelectList(db.RS_Shift, "Shift_ID", "Shift_Name", RS_AM_Operator_Station_Allocation.Shift_ID);
            return View(RS_AM_Operator_Station_Allocation);
        }



        /*	    Action Name		    : Delete
        *		Description		    : To Display the operator allocation information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: OperatorStationAllocation/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation = db.RS_AM_Operator_Station_Allocation.Find(id);
            if (RS_AM_Operator_Station_Allocation == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceOperatorAllocation.Operator;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Operators";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceOperatorAllocation.Operator_Title_Delete_Operator;
            globalData.contentFooter = ResourceOperatorAllocation.Operator_Title_Delete_Operator;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Operator_Station_Allocation);
        }


        /*	    Action Name		    : DeleteConfirmed
        *		Description		    : To delete the operator allocation record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id of user whose record is to be deleted
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: OperatorStationAllocation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation = db.RS_AM_Operator_Station_Allocation.Find(id);
                db.RS_AM_Operator_Station_Allocation.Remove(RS_AM_Operator_Station_Allocation);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_AM_Operator_Station_Allocation", "OSM_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceOperatorAllocation.Operator;
                globalData.messageDetail = ResourceOperatorAllocation.Operator_Success_Operator_Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceOperatorAllocation.Operator;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete");

            }


        }


        /*	    Action Name		    : Dispose
        *		Description		    : To clear the memory allocated by objects
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : disposing bool value 
        *		Return Type		    :
        *		Revision		    :
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Opearator Allocation To Station
        public ActionResult OperatorAllocationToLine()
        {

            decimal userid = ((FDSession)this.Session["FDSession"]).userId;

            #region Present Buffer Employee's
            var prescentBufferEmployee = (from employee in db.RS_Employee
                                          join Attendance in db.RS_User_Attendance_Sheet on employee.Employee_ID equals Attendance.Employee_ID
                                          join cellmembr in db.RS_Assign_OperatorToSupervisor.Where(x => x.Supervisor_ID == userid) on employee.Employee_ID equals cellmembr.AssignedOperator_ID
                                          where Attendance.Is_Present == true && (Attendance.Entry_Date.Value.Year == DateTime.Now.Year && Attendance.Entry_Date.Value.Month == DateTime.Now.Month && Attendance.Entry_Date.Value.Day == DateTime.Now.Day) && !db.RS_AM_Operator_Station_Allocation.Any(x => x.Employee_ID == employee.Employee_ID)
                                          select new EmployeeAllocationData()
                                          {
                                              Employee_Name = employee.Employee_Name,
                                              Employee_No = Attendance.Employee_No,
                                              Employee_ID = employee.Employee_ID

                                          }).Distinct();
            #endregion

            #region Absent Employee's

            var abscentEmployee = (from employee in db.RS_Employee
                                   join Attandance in db.RS_User_Attendance_Sheet on employee.Employee_ID equals Attandance.Employee_ID
                                   join allocation in db.RS_AM_Operator_Station_Allocation on Attandance.Employee_ID equals allocation.Employee_ID
                                   join skill in db.RS_AM_Employee_SkillSet on Attandance.Employee_ID equals skill.Employee_ID
                                   join cellmembr in db.RS_Assign_OperatorToSupervisor.Where(x => x.Supervisor_ID == userid) on employee.Employee_ID equals cellmembr.AssignedOperator_ID
                                   where Attandance.Is_Present == false
                                   && (Attandance.Entry_Date.Value.Year == DateTime.Now.Year
                                   && Attandance.Entry_Date.Value.Month == DateTime.Now.Month
                                   && Attandance.Entry_Date.Value.Day == DateTime.Now.Day)
                                   select new EmployeeAllocationData()
                                   {
                                       Employee_Name = employee.Employee_Name,
                                       Employee_No = Attandance.Employee_No,
                                       Employee_ID = employee.Employee_ID

                                   }).Distinct();

            #endregion

            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.AEmp = abscentEmployee;
            ViewBag.PBEmp = prescentBufferEmployee;
            return View();
        }

        public ActionResult GetEmployeeDetails(decimal stationid)
        {
            decimal userid = ((FDSession)this.Session["FDSession"]).userId;

            #region Present Employee
            var prescentEmployee = (from employee in db.RS_Employee
                                    join operatr in db.RS_AM_Operator_Station_Allocation on employee.Employee_ID equals operatr.Employee_ID
                                    join attendance in db.RS_User_Attendance_Sheet on operatr.Employee_ID equals attendance.Employee_ID
                                    join skill in db.RS_AM_Employee_SkillSet on attendance.Employee_ID equals skill.Employee_ID
                                    where attendance.Is_Present == true
                                    && (attendance.Entry_Date.Value.Year == DateTime.Now.Year
                                    && attendance.Entry_Date.Value.Month == DateTime.Now.Month
                                    && attendance.Entry_Date.Value.Day == DateTime.Now.Day)
                                    && skill.Station_ID == stationid
                                    orderby skill.Skill_ID descending
                                    select new EmployeeAllocationData
                                    {
                                        Employee_Name = employee.Employee_Name,
                                        Employee_No = attendance.Employee_No,
                                        Employee_ID = employee.Employee_ID
                                    }).Distinct();

            #endregion

            ViewBag.PEmp = prescentEmployee;

            return Json(prescentEmployee, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLineByShopID(decimal shopid)
        {
            var lines = db.RS_Lines.Where(x => x.Shop_ID == shopid).Select(x => new { x.Line_ID, x.Line_Name }).ToList();
            return Json(lines, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateOperatorAllocationToLineTable(decimal lineid)
        {
            decimal userid = ((FDSession)this.Session["FDSession"]).userId;
            db.Configuration.ProxyCreationEnabled = false;
            var Lineroute = db.RS_Route_Display.Where(x => x.Line_ID == lineid).Distinct().ToList();
            // var Employee = db.RS_AM_Operator_Station_Allocation.Where(x=>x.Line_ID==lineid).ToList();
            int shopid = Convert.ToInt32(db.RS_Lines.Where(x => x.Line_ID == lineid).FirstOrDefault().Shop_ID);
            var shiftname = GetCurrentShift(shopid);


            var AllocatedEmployee = from Allocation in db.RS_AM_Operator_Station_Allocation
                                    join Attandance in db.RS_User_Attendance_Sheet on Allocation.Employee_ID equals Attandance.Employee_ID
                                    join employee in db.RS_Employee on Allocation.Employee_ID equals employee.Employee_ID
                                    join shift in db.RS_Shift on Allocation.Shift_ID equals shift.Shift_ID
                                    where Attandance.Is_Present == true
                                    && employee.Is_Deleted != true
                                    && (Attandance.Entry_Date.Value.Year == DateTime.Now.Year
                                    && Attandance.Entry_Date.Value.Month == DateTime.Now.Month
                                    && Attandance.Entry_Date.Value.Day == DateTime.Now.Day)
                                    && !db.RS_User_Attendance_Sheet.Any(x => x.Employee_ID == Allocation.Employee_ID && x.Is_Present == false)
                                    select new EmployeeAllocationData()
                                    {
                                        Employee_Name = employee.Employee_Name,
                                        Employee_No = employee.Employee_No,
                                        Employee_ID = employee.Employee_ID,
                                        Station_ID = Allocation.Station_ID,
                                        Line_ID = Allocation.Line_ID
                                    };

            var data = new { route = Lineroute, employee = AllocatedEmployee };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCurrentShift(int shopId)
        {
            try
            {
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                //var shiftObj = db.RS_Shift.AsEnumerable()
                //                 .Where(a => a.Shop_ID == shopId && TimeSpan.Compare(a.Shift_Start_Time, currDate) < 0 && TimeSpan.Compare(a.Shift_End_Time, currDate) > 0)
                //                 .FirstOrDefault();

                var shiftObj = (from shift in db.RS_Shift
                                where
                               shift.Shop_ID == shopId
                                && TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                                && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                                select new
                                {
                                    // Id = shift.Shift_ID,
                                    Value = shift.Shift_Name
                                });

                return Json(shiftObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception dbEx)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult saveAssociateAllocation(string employeeNo, string stationId, int shopId, int lineId)
        {
            try
            {
                string[] stat = stationId.Split('_');
                int droppedStationId = Convert.ToInt16(stat[1]);
                RS_AM_Operator_Station_Allocation mmAllocationObj = new RS_AM_Operator_Station_Allocation();
                var st = db.RS_Employee.Where(a => a.Employee_No == employeeNo).FirstOrDefault();
                decimal employeeId = st.Employee_ID;
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                var shiftObj = (from shift in db.RS_Shift
                                where
                               shift.Shop_ID == shopId
                                && TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                                && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                                select shift);
                //new
                //{
                //    Id = shift.Shift_ID,
                //    Value = shift.Shift_Name
                //});
                decimal shiftid = shiftObj.FirstOrDefault().Shift_ID;
                // var st1 = db.RS_AM_Operator_Station_Allocation.Where(a => a.Employee_ID == employeeId && a.Station_ID == droppedStationId).FirstOrDefault();
                // decimal shiftid = st1.Shift_ID;
                if (mmAllocationObj.IsOperatorToOneStationExists(employeeId, droppedStationId, shiftid, shopId,DateTime.Now))
                {

                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    mmAllocationObj.Plant_ID = 1;
                    mmAllocationObj.Shop_ID = shopId;
                    mmAllocationObj.Line_ID = lineId;
                    mmAllocationObj.Shift_ID = shiftid;
                    mmAllocationObj.Station_ID = droppedStationId;

                    mmAllocationObj.Employee_ID = st.Employee_ID;
                    mmAllocationObj.Allocation_Date = DateTime.Now;
                    mmAllocationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId; ;
                    mmAllocationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    mmAllocationObj.Inserted_Date = DateTime.Now;
                    db.RS_AM_Operator_Station_Allocation.Add(mmAllocationObj);
                    db.SaveChanges();

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception dbEx)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult DeleteOperator(string employeeNo, string stationId)
        {
            try
            {
                string[] stat = employeeNo.Split('_');
                int droppedStationId = Convert.ToInt16(stat[2]);
                string droppedEmployeeNo = stat[3];
                var st = db.RS_Employee.Where(a => a.Employee_No == droppedEmployeeNo).FirstOrDefault();
                decimal droppedEmployeeId = st.Employee_ID;
                // RS_AM_Operator_Station_Allocation mmAllocationObj = new RS_AM_Operator_Station_Allocation();

                var remove = from aremove in db.RS_AM_Operator_Station_Allocation
                             where aremove.Employee_ID == droppedEmployeeId && aremove.Station_ID == droppedStationId
                             select aremove;
                foreach (var detail in remove)
                {
                    db.RS_AM_Operator_Station_Allocation.Remove(detail);
                    generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_AM_Operator_Station_Allocation", "OSM_ID", detail.OSM_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                }
                try
                {
                    db.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    // Provide for exceptions.
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                // RS_AM_Operator_Station_Allocation mmAllocationObj = db.RS_AM_Operator_Station_Allocation.Find(aremove);
                // db.RS_AM_Operator_Station_Allocation.Remove(aremove);
                //db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception dbEx)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult checkAllocatedEmployees(decimal stationid)
        {
            var Employee = db.RS_AM_Operator_Station_Allocation.Where(x => x.Station_ID == stationid).ToList();
            return Json(Employee, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Supervisor Monitoring
        //public ActionResult SupervisorMonitoring()
        //{
        //    decimal userid = ((FDSession)this.Session["FDSession"]).userId;
        //    if (db.RS_Employee.Where(x => x.Employee_ID == userid).FirstOrDefault())
        //    {
        //        if (db.RS_AM_Line_Supervisor_Mapping.Where(x => x.Employee_ID == userid).FirstOrDefault().Shop_ID == 1)
        //        {
        //            return RedirectToAction("EngineShop", "TrackingAllocation");
        //        }
        //        else if (db.RS_AM_Line_Supervisor_Mapping.Where(x => x.Employee_ID == userid).FirstOrDefault().Shop_ID == 2)
        //        {
        //            return RedirectToAction("TransmissionShop", "TrackingAllocation");
        //        }
        //        else if (db.RS_AM_Line_Supervisor_Mapping.Where(x => x.Employee_ID == userid).FirstOrDefault().Shop_ID == 4)
        //        {
        //            return RedirectToAction("TractorShop", "TrackingAllocation");
        //        }
        //        else if (db.RS_AM_Line_Supervisor_Mapping.Where(x => x.Employee_ID == userid).FirstOrDefault().Shop_ID == 3)
        //        {
        //            return RedirectToAction("HydraulicShop", "TrackingAllocation");
        //        }
        //        else if (db.RS_AM_Line_Supervisor_Mapping.Where(x => x.Employee_ID == userid).FirstOrDefault().Shop_ID == 7)
        //        {
        //            return RedirectToAction("CVShop", "TrackingAllocation");
        //        }
        //        else if (db.RS_AM_Line_Supervisor_Mapping.Where(x => x.Employee_ID == userid).FirstOrDefault().Shop_ID == 6)
        //        {
        //            return RedirectToAction("PaintShop", "TrackingAllocation");
        //        }
        //    }

        //    return View();
        //}
        #endregion
    }
}
