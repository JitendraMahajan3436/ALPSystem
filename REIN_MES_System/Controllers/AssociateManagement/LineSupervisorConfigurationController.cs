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
    /* Class Name                 : LineSupervisorConfigurationController
   *  Description                : This class is used to perform the basic operations like insert, update, edit and delete Supervisor added for line 
   *  Author, Timestamp          : Jitendra Mahajan      
   */
    public class LineSupervisorConfigurationController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalObj = new General();

        /*	    Action Name		    : Index
        *		Description		    : To Display the Supervisor information in grid
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: LineSupervisorConfiguration
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = ResourceModules.Officer;
            globalData.actionName = ResourceModules.Officer + " " + ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Officer + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Officer + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            var RS_AM_Line_Supervisor_Mapping = db.RS_AM_Line_Supervisor_Mapping.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Lines);
            return View(RS_AM_Line_Supervisor_Mapping.ToList());
        }


        /*	    Action Name		    : Details
         *		Description		    : To show the supervisor detailed information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : id 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: LineSupervisorConfiguration/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = db.RS_AM_Line_Supervisor_Mapping.Find(id);
            if (RS_AM_Line_Supervisor_Mapping == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Supervisors";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Officer + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Officer + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Line_Supervisor_Mapping);
        }


        /*	    Action Name		    : Create
        *		Description		    : To read the supervisor info which is to be saved
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: LineSupervisorConfiguration/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = ResourceModules.Officer;
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.Officer + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Officer + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");



            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(m => m.Employee_ID == 0), "Employee_ID", "Employee_Name");
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name",0);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            return View();
        }


        /*	    Action Name		    : Create
         *		Description		    : To save the supervisor information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : RS_AM_Line_Supervisor_Mapping object 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // POST: LineSupervisorConfiguration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SMM_ID,Plant_ID,Shop_ID,Employee_ID,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Line_ID,checkBox")] RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping)
        {
            if (ModelState.IsValid)
            {
                if (RS_AM_Line_Supervisor_Mapping.checkBox.Value == false)
                {
                    decimal shopId = RS_AM_Line_Supervisor_Mapping.Shop_ID;
                    var st = from line in db.RS_Lines
                             where line.Shop_ID == shopId
                             select line;
                    int supervisor = Convert.ToInt32(RS_AM_Line_Supervisor_Mapping.Employee_ID);
                    bool isValid = true;
                    foreach (var item in st.ToList())
                    {
                        int line = Convert.ToInt32(item.Line_ID);

                        //check line officer present or nt
                        if (RS_AM_Line_Supervisor_Mapping.IsSupervisorExists(line, supervisor))
                        {
                            isValid = false;
                            ModelState.AddModelError("Line_ID", ResourceValidation.Exist);
                            //<a href="@Url.Action("Delete", "LineSupervisorConfiguration", new { id = item.SMM_ID })"><i class="fa fa-trash"></i></a>

                        }

                    }

                    if (isValid)
                    {
                        //process to assign officer to all shop lines
                        foreach (var item in st.ToList())
                        {
                            RS_AM_Line_Supervisor_Mapping.Line_ID = item.Line_ID;
                            RS_AM_Line_Supervisor_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            RS_AM_Line_Supervisor_Mapping.Inserted_Date = DateTime.Now;
                            RS_AM_Line_Supervisor_Mapping.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            RS_AM_Line_Supervisor_Mapping.Plant_ID = 1;
                            db.RS_AM_Line_Supervisor_Mapping.Add(RS_AM_Line_Supervisor_Mapping);
                            db.SaveChanges();
                        }

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Officer;
                        globalData.messageDetail = ResourceModules.Officer+" "+ResourceMessages.Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }

                }
                else
                {

                    // check officer is added to selected line

                    int line = Convert.ToInt32(RS_AM_Line_Supervisor_Mapping.Line_ID);
                    int supervisor = Convert.ToInt32(RS_AM_Line_Supervisor_Mapping.Employee_ID);
                    bool isValid = true;

                    //check line is null or not
                    if (line == null)
                    {
                        ModelState.AddModelError("Employee_ID", ResourceValidation.Exist);
                    }

                    if (RS_AM_Line_Supervisor_Mapping.IsSupervisorExists(line, supervisor))
                    {
                        isValid = false;
                        //ModelState.AddModelError("Employee_ID", ResourceSupervisorConfig.LineSupervisor_Error_LineSupervisor_Name_Exists);

                        TempData["mmObj"] = RS_AM_Line_Supervisor_Mapping;
                        return RedirectToAction("OverrideLineSupervisor");
                    }

                    if (isValid)
                    {
                        // process to assign officer to selected line
                        //RS_AM_Line_Supervisor_Mapping.Line_ID = item.Line_ID;
                        RS_AM_Line_Supervisor_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_AM_Line_Supervisor_Mapping.Inserted_Date = DateTime.Now;
                        RS_AM_Line_Supervisor_Mapping.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        RS_AM_Line_Supervisor_Mapping.Plant_ID = 1;
                        db.RS_AM_Line_Supervisor_Mapping.Add(RS_AM_Line_Supervisor_Mapping);
                        db.SaveChanges();

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Officer;
                        globalData.messageDetail = ResourceModules.Officer+" "+ResourceMessages.Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }


            }

            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Supervisors";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Updated_User_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Employee_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Line_Supervisor_Mapping.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Line_Supervisor_Mapping.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Line_Supervisor_Mapping.Line_ID);
            return View(RS_AM_Line_Supervisor_Mapping);
        }


        /*	    Action Name		    : Edit
        *		Description		    : To read the supervisor information which is to be edited
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: LineSupervisorConfiguration/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = db.RS_AM_Line_Supervisor_Mapping.Find(id);
            if (RS_AM_Line_Supervisor_Mapping == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Supervisors";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Updated_User_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Employee_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Line_Supervisor_Mapping.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Line_Supervisor_Mapping.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Line_Supervisor_Mapping.Line_ID);
            return View(RS_AM_Line_Supervisor_Mapping);
        }


        /*	    Action Name		    : Edit
        *		Description		    : To edit the supervisor information
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : RS_AM_Line_Supervisor_Mapping object 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // POST: LineSupervisorConfiguration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SMM_ID,Plant_ID,Shop_ID,Employee_ID,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Line_ID")] RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping)
        {
            RS_AM_Line_Supervisor_Mapping mmSupervisorObj = new RS_AM_Line_Supervisor_Mapping();
            if (ModelState.IsValid)
            {
                mmSupervisorObj = db.RS_AM_Line_Supervisor_Mapping.Find(RS_AM_Line_Supervisor_Mapping.SRS_ID);
                mmSupervisorObj.Shop_ID = RS_AM_Line_Supervisor_Mapping.Shop_ID;
                mmSupervisorObj.Plant_ID = RS_AM_Line_Supervisor_Mapping.Plant_ID;
                mmSupervisorObj.Line_ID = RS_AM_Line_Supervisor_Mapping.Line_ID;
                mmSupervisorObj.Employee_ID = RS_AM_Line_Supervisor_Mapping.Employee_ID;
                mmSupervisorObj.Inserted_Date = db.RS_AM_Line_Supervisor_Mapping.Find(RS_AM_Line_Supervisor_Mapping.SRS_ID).Inserted_Date;
                mmSupervisorObj.Inserted_User_ID = db.RS_AM_Line_Supervisor_Mapping.Find(RS_AM_Line_Supervisor_Mapping.SRS_ID).Inserted_User_ID;
                mmSupervisorObj.Inserted_Host = db.RS_AM_Line_Supervisor_Mapping.Find(RS_AM_Line_Supervisor_Mapping.SRS_ID).Inserted_Host;
                mmSupervisorObj.Is_Edited = true;
                mmSupervisorObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mmSupervisorObj.Updated_Date = DateTime.Now;
                mmSupervisorObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Officer;
                globalData.messageDetail = ResourceModules.Officer + " " + ResourceMessages.Edit_Success;
                TempData["globalData"] = globalData;


                db.Entry(mmSupervisorObj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Supervisors";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Updated_User_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Employee_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Line_Supervisor_Mapping.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Line_Supervisor_Mapping.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Line_Supervisor_Mapping.Line_ID);
            return View(RS_AM_Line_Supervisor_Mapping);
        }


        /*	    Action Name		    : Delete
        *		Description		    : To Display the supervisor information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: LineSupervisorConfiguration/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = db.RS_AM_Line_Supervisor_Mapping.Find(id);
            if (RS_AM_Line_Supervisor_Mapping == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Supervisors";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Line_Supervisor_Mapping);
        }



        /*	    Action Name		    : DeleteConfirmed
        *		Description		    : To delete the supervisor record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id of user whose record is to be deleted
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: LineSupervisorConfiguration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation = new RS_AM_Operator_Station_Allocation();
                var ot = from operatorAllocation in db.RS_AM_Operator_Station_Allocation
                         where (from operatorToSupervisor in db.RS_Assign_OperatorToSupervisor where operatorToSupervisor.Supervisor_ID == id select operatorToSupervisor.Supervisor_ID).Contains(operatorAllocation.Employee_ID)
                         select operatorAllocation;
                foreach (var item in ot.ToList())
                {
                    //if (RS_AM_Operator_Station_Allocation != null)
                    //{
                    RS_AM_Operator_Station_Allocation = db.RS_AM_Operator_Station_Allocation.Find(item.OSM_ID);
                    db.RS_AM_Operator_Station_Allocation.Remove(RS_AM_Operator_Station_Allocation);
                    db.SaveChanges();


                    generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_AM_Operator_Station_Allocation", "OSM_ID", item.OSM_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                    
                    // }
                }
                RS_Assign_OperatorToSupervisor mM_AM_Assign_OperatorToSupervisor = new RS_Assign_OperatorToSupervisor();
                var st = from operatorToSupervisor in db.RS_Assign_OperatorToSupervisor
                         where operatorToSupervisor.Supervisor_ID == id
                         select operatorToSupervisor;
                foreach (var item in st.ToList())
                {

                    //if (mM_AM_Assign_OperatorToSupervisor != null)
                    //{
                    //int line = Convert.ToInt32(item.Line_ID);  
                    mM_AM_Assign_OperatorToSupervisor = db.RS_Assign_OperatorToSupervisor.Find(item.OTS_ID);
                    db.RS_Assign_OperatorToSupervisor.Remove(mM_AM_Assign_OperatorToSupervisor);
                    db.SaveChanges();

                    generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Assign_OperatorToSupervisor", "OTS_ID", item.OTS_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                    // }
                }

                RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = db.RS_AM_Line_Supervisor_Mapping.Find(id);
                if (RS_AM_Line_Supervisor_Mapping != null)
                {
                    db.RS_AM_Line_Supervisor_Mapping.Remove(RS_AM_Line_Supervisor_Mapping);
                    db.SaveChanges();

                    generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_AM_Line_Supervisor_Mapping", "SMM_ID", RS_AM_Line_Supervisor_Mapping.SRS_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                }

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Officer;
                globalData.messageDetail = ResourceModules.Officer + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Manager;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;

                return RedirectToAction("Delete");

            }


        }



        public ActionResult OverrideLineSupervisor()
        {
            //int lineID = Convert.ToInt16(TempData["SMM_ID"]);
            RS_AM_Line_Supervisor_Mapping objdata = (RS_AM_Line_Supervisor_Mapping)(TempData["mmObj"]);
            // decimal lineId = Convert.ToInt32(objdata.Line_ID);
            // decimal supervisorId = objdata.Employee_ID;

            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", objdata.Employee_ID);
            // ViewBag.Employee_name = objdata.Employee_ID;
            // ViewBag.Shop_Name = objdata.Shop_ID;
            //ViewBag.Line_Name = objdata.Line_ID;
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Line_Supervisor_Mapping.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", objdata.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", objdata.Line_ID);

            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping = db.RS_AM_Line_Supervisor_Mapping.Find(id);
            //if (RS_AM_Line_Supervisor_Mapping == null)
            //{
            //    return HttpNotFound();
            //}
            //if (TempData["globalData"] != null)
            //{
            //    globalData = (GlobalData)TempData["globalData"];
            //}

            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Override;
            globalData.controllerName = "Supervisors";
            globalData.actionName = ResourceGlobal.Override;
            globalData.contentTitle = ResourceGlobal.Override + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Override + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View();
            //return View(RS_AM_Line_Supervisor_Mapping);
        }

        [HttpPost]
        public ActionResult OverrideLineSupervisor([Bind(Include = "SMM_ID,Plant_ID,Shop_ID,Employee_ID,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Line_ID,checkBox")] RS_AM_Line_Supervisor_Mapping RS_AM_Line_Supervisor_Mapping)
        {
            RS_AM_Line_Supervisor_Mapping mmSupervisorObj = new RS_AM_Line_Supervisor_Mapping();
            //if (ModelState.IsValid)
            // {
            mmSupervisorObj = db.RS_AM_Line_Supervisor_Mapping.Where(p => p.Line_ID == RS_AM_Line_Supervisor_Mapping.Line_ID).Single();

            mmSupervisorObj.Employee_ID = RS_AM_Line_Supervisor_Mapping.Employee_ID;
            mmSupervisorObj.Is_Edited = true;
            mmSupervisorObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
            mmSupervisorObj.Updated_Date = DateTime.Now;
            mmSupervisorObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
            db.Entry(mmSupervisorObj).State = EntityState.Modified;
            db.SaveChanges();

            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.Officer;
            globalData.messageDetail = ResourceModules.Officer + " " + ResourceMessages.Override_Success;
            TempData["globalData"] = globalData;


            //db.Entry(mmSupervisorObj).State = EntityState.Modified;
            //db.SaveChanges();
            return RedirectToAction("Index");
            // }
            globalData.pageTitle = ResourceModules.Officer;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Supervisors";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Officer + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Updated_User_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Line_Supervisor_Mapping.Employee_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Line_Supervisor_Mapping.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Line_Supervisor_Mapping.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_AM_Line_Supervisor_Mapping.Line_ID);
            return View(RS_AM_Line_Supervisor_Mapping);
            return View();
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
    }
}
