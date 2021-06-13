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
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /* Class Name                 : AssignOperatorToSupervisorController
   *  Description                : This class is used to add/save Operators assigned to supervisor against particular line selected
   *  Author, Timestamp          : Jitendra Mahajan      
   */
    public class AssignOperatorToSupervisorController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0, supervisorId = 0;
        int operatorId = 0;


        /*	    Action Name		    : Index
        *		Description		    : To Display the operators under supervisor information in grid
        *		Author, Timestamp	: Ajay Wagh
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AssignOperatorToSupervisor
        public ActionResult Index()
        {
            var RS_Assign_OperatorToSupervisor = db.RS_Assign_OperatorToSupervisor.Include(m => m.RS_Employee).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops);
            return View(RS_Assign_OperatorToSupervisor.ToList());
        }


        /*	    Action Name		    : Details
         *		Description		    : To show the operators under supervisor detailed information
         *		Author, Timestamp	: Ajay Wagh
         *		Input parameter	    : id of operators whose information is to be displayed 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: AssignOperatorToSupervisor/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Assign_OperatorToSupervisor RS_Assign_OperatorToSupervisor = db.RS_Assign_OperatorToSupervisor.Find(id);
            if (RS_Assign_OperatorToSupervisor == null)
            {
                return HttpNotFound();
            }
            return View(RS_Assign_OperatorToSupervisor);
        }


        /*	    Action Name		    : Create
        *		Description		    : To read the operators under supervisor info which is to be saved
        *		Author, Timestamp	: Ajay Wagh
        *		Input parameter	    : 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AssignOperatorToSupervisor/Create
        public ActionResult Create()
        {
            try
            {


                int plant_Id = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];

                    ViewBag.Shop_ID = new SelectList("");
                    ViewBag.Line_ID = new SelectList("");
                    ViewBag.Supervisor_ID = new SelectList("");
                    ViewBag.ListofOperator = new SelectList("");
                    ViewBag.AssignedOperator_ID = new SelectList("");
                    //plantId = 1;//Convert.ToInt32(TempData["plantId"].ToString());
                    //shopId = Convert.ToInt32(TempData["shopId"].ToString());
                    //lineId = Convert.ToInt32(TempData["lineId"].ToString());
                    //supervisorId = Convert.ToInt32(TempData["supervisorId"].ToString());
                    // ViewBag.Type = new SelectList("False");                

                    // line quality station
                    var stationList = from supervisor in db.RS_Assign_OperatorToSupervisor
                                      where (from employee in db.RS_Employee where employee.Employee_ID == supervisorId select employee.Employee_ID).Contains(supervisor.Supervisor_ID)
                                      select new
                                      {
                                          Supervisor_ID = supervisor.Supervisor_ID
                                          //Supervisor_Name = db.RS_Employee.
                                      };

                    ViewBag.Station_ID = new SelectList(stationList, "Station_ID", "Station_Name", stationId);

                    // not selected defect
                    var noSelectedOperators = from employee in db.RS_Employee
                                              where !(from selecteoperators in db.RS_Assign_OperatorToSupervisor where selecteoperators.Supervisor_ID == supervisorId select selecteoperators.AssignedOperator_ID).Contains(employee.Employee_ID) &&
                                           employee.Plant_ID == plantId && employee.Is_Deleted != true
                                              select new
                                              {
                                                  AssignedOperator_ID = employee.Employee_ID,
                                                  AssignedOperator_Name = employee.Employee_Name
                                              };

                    ViewBag.AssignedOperator_ID = new SelectList(noSelectedOperators, "AssignedOperator_ID", "AssignedOperator_Name");

                    // load selected defect
                    var selectedOperators = from selecteoperators in db.RS_Assign_OperatorToSupervisor
                                            join operators in db.RS_Employee on selecteoperators.AssignedOperator_ID equals operators.Employee_ID
                                            where selecteoperators.Supervisor_ID == supervisorId && operators.Is_Deleted != true
                                            select new
                                            {
                                                AssignedOperator_ID = operators.Employee_ID,
                                                AssignedOperator_Name = operators.Employee_Name
                                            };
                    ViewBag.selectedOperators = new SelectList(selectedOperators, "AssignedOperator_ID", "AssignedOperator_Name");

                }
                else
                {
                    //ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_Id).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
                    ViewBag.Supervisor_ID = new SelectList((db.RS_Employee.Where(p => p.Employee_ID == supervisorId)).OrderBy(c => c.Employee_Name), "Employee_ID", "Employee_Name");
                    ViewBag.ListofOperator = new SelectList(db.RS_Assign_OperatorToSupervisor.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name");
                    ViewBag.AssignedOperator_ID = new SelectList(db.RS_Assign_OperatorToSupervisor.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name");
                    // ViewBag.Type = new SelectList("0");
                }
                globalData.pageTitle = ResourceOperatorToSupervisor.OperatorToSupervisor;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = ResourceOperatorToSupervisor.Site_Variable_OperatorToSupervisor;
                globalData.actionName = ResourceOperatorToSupervisor.Site_Variable_Index;
                globalData.contentTitle = ResourceOperatorToSupervisor.OperatorToSupervisor_Title_OperatorToSupervisor_Lists;
                globalData.contentFooter = ResourceOperatorToSupervisor.OperatorToSupervisor_Title_OperatorToSupervisor_Lists;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(S => S.Employee_ID == 0), "Employee_ID", "Employee_Name");
                ViewBag.Supervisor_ID = new SelectList((db.RS_Employee.Where(S => S.Employee_ID == 0)).OrderBy(c => c.Employee_Name), "Employee_ID", "Employee_Name");
                ViewBag.Line_ID = new SelectList((db.RS_Lines.Where(p => p.Line_ID == 0)).OrderBy(c => c.Line_Name), "Line_ID", "Line_Name");
                //ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_Id).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
                return View();
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }



        /*	    Action Name		    : Create
         *		Description		    : To save the operators under supervisor information
         *		Author, Timestamp	: Ajay Wagh
         *		Input parameter	    : RS_Assign_OperatorToSupervisor object 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // POST: AssignOperatorToSupervisor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Assign_OperatorToSupervisor RS_Assign_OperatorToSupervisor)
        {
            if (ModelState.IsValid)
            {
                plantId = 1; //Convert.ToInt32(RS_Assign_OperatorToSupervisor.Plant_ID);
                shopId = Convert.ToInt32(RS_Assign_OperatorToSupervisor.Shop_ID);
                lineId = Convert.ToInt32(RS_Assign_OperatorToSupervisor.Line_ID);
                supervisorId = Convert.ToInt32(RS_Assign_OperatorToSupervisor.Supervisor_ID);


                {
                    // process to delete all the defects added for the station
                    RS_Assign_OperatorToSupervisor.deleteOperator(shopId, lineId, supervisorId, ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);//stationId, plantId,
                }
                for (int i = 0; i < RS_Assign_OperatorToSupervisor.selectedOperators.Count(); i++)
                {
                    operatorId = Convert.ToInt16(RS_Assign_OperatorToSupervisor.selectedOperators[i]);

                    if (operatorId == 0)
                        continue;

                    //if (RS_Assign_OperatorToSupervisor.isDefectAddedToStation(operatorId, stationId, plantId, shopId, lineId))
                    //{
                    //    // defect is already added no need to the defect
                    //}
                    else
                    {
                        RS_Assign_OperatorToSupervisor.Plant_ID = 1;
                        RS_Assign_OperatorToSupervisor.Inserted_Date = DateTime.Now;
                        RS_Assign_OperatorToSupervisor.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        // RS_Assign_OperatorToSupervisor. = ((FDSession)this.Session["FDSession"]).userHost;

                        RS_Assign_OperatorToSupervisor.AssignedOperator_ID = RS_Assign_OperatorToSupervisor.selectedOperators[i];
                        db.RS_Assign_OperatorToSupervisor.Add(RS_Assign_OperatorToSupervisor);
                        db.SaveChanges();
                    }

                }
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceOperatorToSupervisor.OperatorToSupervisor;
                globalData.messageDetail = ResourceOperatorToSupervisor.OperatorToSupervisor_Success_OperatorToSupervisor_Add_Success;
                TempData["globalData"] = globalData;
                // db.RS_Assign_OperatorToSupervisor.Add(RS_Assign_OperatorToSupervisor);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }

            globalData.pageTitle = ResourceOperatorToSupervisor.OperatorToSupervisor;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "OperatorToSupervisor";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceOperatorToSupervisor.OperatorToSupervisor_Title_Add_OperatorToSupervisor;
            globalData.contentFooter = ResourceOperatorToSupervisor.OperatorToSupervisor_Title_Add_OperatorToSupervisor;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Supervisor_ID = new SelectList((db.RS_Employee.Where(S => S.Employee_ID == 0)).OrderBy(c => c.Employee_Name), "Employee_ID", "Employee_Name", RS_Assign_OperatorToSupervisor.Supervisor_ID);
            ViewBag.Line_ID = new SelectList((db.RS_Lines.Where(p => p.Line_ID == 0)).OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Assign_OperatorToSupervisor.Line_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", 0);
            // ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", 0);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Assign_OperatorToSupervisor.Shop_ID);
            return RedirectToAction("Create");
            //return View(RS_Assign_OperatorToSupervisor);
        }


        /*	    Action Name		    : Edit
        *		Description		    : To read the operators under supervisor information which is to be edited
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AssignOperatorToSupervisor/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Assign_OperatorToSupervisor RS_Assign_OperatorToSupervisor = db.RS_Assign_OperatorToSupervisor.Find(id);
            if (RS_Assign_OperatorToSupervisor == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supervisor_ID = new SelectList(db.RS_Employee.OrderBy(c => c.Employee_Name), "Employee_ID", "Employee_Name", RS_Assign_OperatorToSupervisor.Supervisor_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Assign_OperatorToSupervisor.Line_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Assign_OperatorToSupervisor.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Assign_OperatorToSupervisor.Shop_ID);
            return View(RS_Assign_OperatorToSupervisor);
        }


        /*	    Action Name		    : Edit
        *		Description		    : To edit the operators under supervisor information
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : RS_Assign_OperatorToSupervisor object 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // POST: AssignOperatorToSupervisor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OTS_ID,Plant_ID,Shop_ID,Line_ID,Supervisor_ID,AssignedOperator_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Assign_OperatorToSupervisor RS_Assign_OperatorToSupervisor)
        {
            if (ModelState.IsValid)
            {
                RS_Assign_OperatorToSupervisor.Is_Edited = true;
                db.Entry(RS_Assign_OperatorToSupervisor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Supervisor_ID = new SelectList(db.RS_Employee.OrderBy(c => c.Employee_Name), "Employee_ID", "Employee_Name", RS_Assign_OperatorToSupervisor.Supervisor_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Assign_OperatorToSupervisor.Line_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Assign_OperatorToSupervisor.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Assign_OperatorToSupervisor.Shop_ID);
            return View(RS_Assign_OperatorToSupervisor);
        }



        /*	    Action Name		    : Delete
        *		Description		    : To Display the operators under supervisor information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AssignOperatorToSupervisor/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Assign_OperatorToSupervisor RS_Assign_OperatorToSupervisor = db.RS_Assign_OperatorToSupervisor.Find(id);
            if (RS_Assign_OperatorToSupervisor == null)
            {
                return HttpNotFound();
            }
            return View(RS_Assign_OperatorToSupervisor);
        }


        /*	    Action Name		    : DeleteConfirmed
        *		Description		    : To delete the operators under supervisor record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: AssignOperatorToSupervisor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Assign_OperatorToSupervisor RS_Assign_OperatorToSupervisor = db.RS_Assign_OperatorToSupervisor.Find(id);
            db.RS_Assign_OperatorToSupervisor.Remove(RS_Assign_OperatorToSupervisor);
            db.SaveChanges();
            return RedirectToAction("Index");
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

        /* Action Name                : GetSupervisorByLineID
   *  Description                : Action used to get the Supervisor list by line
   *  Author, Timestamp          : Jitendra Mahajan
   *  Input parameter            : lineId (line id)
   *  Return Type                : ActionResult
   *  Revision                   : 1.0
   */
        public ActionResult GetSupervisorByLineID(int lineId)
        {
            try
            {
                decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from employee in db.RS_Employee
                         join lineSupervisor in db.RS_AM_Line_Supervisor_Mapping on employee.Employee_ID equals lineSupervisor.Employee_ID
                         where lineSupervisor.Line_ID == lineId && employee.Is_Deleted != true
                         //&& employee.Category_ID == 2
                         && employee.Plant_ID == plantID
                         orderby employee.Employee_Name
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }


        /* Action Name                : GetoperatorsBySupervisorID
  *  Description                : Action used to get the operator list by supervisor Id
  *  Author, Timestamp          : Jitendra Mahajan
  *  Input parameter            : supervisorId 
  *  Return Type                : ActionResult
  *  Revision                   : 1.0
  */

        public ActionResult GetoperatorsBySupervisorID(int supervisorId)
        {
            try
            {
                decimal plant_Id = ((FDSession)this.Session["FDSession"]).plantId;

                var st = (from emp in db.RS_Employee
                          where !(from cm in db.RS_Assign_OperatorToSupervisor
                                  where cm.Plant_ID == plant_Id
                                  select cm.AssignedOperator_ID).Contains(emp.Employee_ID)
                                   //&&
                                   // (from cm in db.RS_AM_Employee_SkillSet
                                   //  where cm.Plant_ID == plant_Id
                                   //  select cm.Employee_ID).Contains(emp.Employee_ID)
                                   //&& emp.Category_ID == 1
                                   && emp.Is_Deleted == null
                                   && emp.Plant_ID == plant_Id
                          orderby emp.Employee_Name
                          select new
                          {
                              Id = emp.Employee_ID,
                              Value = emp.Employee_Name + "(" + emp.Employee_No + ")"
                          }).Distinct();


                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }


        public ActionResult GetoperatorsBySupervisorID_Old(int supervisorId)
        {//RS_Assign_OperatorToSupervisor
            try
            {
                decimal plant_Id = ((FDSession)this.Session["FDSession"]).plantId;
                //var st = from a in db.RS_Employee
                //         join b in db.RS_AM_Shop_Manager_Mapping on a.Employee_ID equals b.Employee_ID into b_join
                //         from b in b_join.DefaultIfEmpty()
                //         join c in db.RS_Assign_OperatorToSupervisor on new { Employee_ID = a.Employee_ID } equals new { Employee_ID = c.AssignedOperator_ID } into c_join
                //         from c in c_join.DefaultIfEmpty()
                //         join d in db.RS_AM_Line_Supervisor_Mapping on a.Employee_ID equals d.Employee_ID into d_join
                //         from d in d_join.DefaultIfEmpty()
                //         where
                //           a.Category_ID == 1
                //            && a.Is_Deleted != true
                //            && a.Plant_ID == plant_Id
                //            && !(from SM in db.RS_Assign_OperatorToSupervisor
                //                 where SM.Plant_ID == plant_Id
                //                 select SM.AssignedOperator_ID).Contains(a.Employee_ID)
                //         orderby a.Employee_Name

                var st = from a in db.RS_Employee
                         join b in db.RS_AM_Shop_Manager_Mapping on a.Employee_ID equals b.Employee_ID into b_join
                         from b in b_join.DefaultIfEmpty()
                         join c in db.RS_Assign_OperatorToSupervisor on new { Employee_ID = a.Employee_ID } equals new { Employee_ID = c.AssignedOperator_ID } into c_join
                         from c in c_join.DefaultIfEmpty()
                         join d in db.RS_AM_Line_Supervisor_Mapping on a.Employee_ID equals d.Employee_ID into d_join
                         from d in d_join.DefaultIfEmpty()
                         where
                           //a.Category_ID == 1  &&
                             a.Is_Deleted != true
                            && a.Plant_ID == plant_Id
                         orderby a.Employee_Name


                         //where !(from OperatorToSupervisor in db.RS_Assign_OperatorToSupervisor 
                         //        where OperatorToSupervisor.Supervisor_ID == supervisorId select OperatorToSupervisor.AssignedOperator_ID)
                         //.Contains(employee.Employee_ID)
                         // && employee.Is_Critical_Station == true
                         select new
                         {
                             Id = a.Employee_ID,
                             Value = a.Employee_Name + "(" + a.Employee_No + ")"
                         };

                return Json(st.Distinct(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
        }


        /* Action Name                : GetAssignedSupervisorsByManagerID
*  Description                : Action used to get the Assigned Supervisor list by Manager Id
*  Author, Timestamp          : Jitendra Mahajan
*  Input parameter            : managerId 
*  Return Type                : ActionResult
*  Revision                   : 1.0
*/
        public ActionResult GetAssignedOperatorsBySupervisorID(int supervisorId)
        {

            try
            {
                decimal plant_Id = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from employee in db.RS_Employee
                         where (from supervisorToOperator in db.RS_Assign_OperatorToSupervisor where supervisorToOperator.Supervisor_ID == supervisorId select supervisorToOperator.AssignedOperator_ID).Contains(employee.Employee_ID)
                        && employee.Is_Deleted != true
                        && employee.Plant_ID == plant_Id
                         orderby employee.Employee_Name
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }

        }

        public ActionResult GetAssignedOperatorsByLineIDSupervisorID(int lineId, int supervisorId)
        {

            try
            {
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from employee in db.RS_Employee
                         where (from supervisorToOperator in db.RS_Assign_OperatorToSupervisor where supervisorToOperator.Supervisor_ID == supervisorId && supervisorToOperator.Line_ID == lineId select supervisorToOperator.AssignedOperator_ID).Contains(employee.Employee_ID)
                         && employee.Is_Deleted != true && employee.Plant_ID == plant_ID
                         orderby employee.Employee_Name
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }

        }

        public ActionResult SaveAssignedOperators(string Stations, int supervisorId, int shopId, int lineId)
        {
            try
            {


                RS_Assign_OperatorToSupervisor RS_Assign_OperatorToSupervisor = new RS_Assign_OperatorToSupervisor();

                RS_Assign_OperatorToSupervisor.deleteOperator(shopId, lineId, supervisorId, ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);//stationId, plantId, 

                string[] words;
                words = Stations.Split(',');
                int flag = 0;
                foreach (string value in words)
                {
                    int i = 0;

                    if (value == "")
                    {
                        i = 0;
                        flag = 0;

                    }
                    else
                    {
                        i = Convert.ToInt32(value);
                        flag = 1;
                    }
                    if (i == 0)
                        continue;

                    RS_Assign_OperatorToSupervisor.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    RS_Assign_OperatorToSupervisor.Shop_ID = shopId;
                    RS_Assign_OperatorToSupervisor.Line_ID = lineId;
                    RS_Assign_OperatorToSupervisor.Inserted_Date = DateTime.Now;
                    RS_Assign_OperatorToSupervisor.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Assign_OperatorToSupervisor.Supervisor_ID = supervisorId;
                    RS_Assign_OperatorToSupervisor.AssignedOperator_ID = i;

                    db.Entry(RS_Assign_OperatorToSupervisor).State = EntityState.Detached;
                    db.RS_Assign_OperatorToSupervisor.Add(RS_Assign_OperatorToSupervisor);
                    db.SaveChanges();
                    i = 0;
                }

                //var opertor_list = db.RS_Assign_OperatorToSupervisor.Where(sup => sup.Supervisor_ID == supervisorId && sup.Line_ID==lineId).Select(sup => sup.AssignedOperator_ID);
                //foreach (var emp_ID in opertor_list)
                //{
                //    if (!db.RS_AM_Operator_Station_Allocation.Any(op => op.Employee_ID == emp_ID))
                //    {
                //        string demo = "delete";
                //    }
                //}

                var opertor_list = db.RS_AM_Operator_Station_Allocation.Where(sup => sup.Inserted_User_ID == supervisorId && sup.Line_ID == lineId).Select(sup => sup.Employee_ID).Distinct();
                foreach (var emp_ID in opertor_list)
                {
                    if (!db.RS_Assign_OperatorToSupervisor.Any(op => op.AssignedOperator_ID == emp_ID))
                    {
                        var operators = db.RS_AM_Operator_Station_Allocation.Where(op => op.Employee_ID == emp_ID && DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(DateTime.Now)).Select(op => op.Employee_ID).ToList();
                        foreach (var item in operators)
                        {
                            using (REIN_SOLUTIONEntities fdDb = new REIN_SOLUTIONEntities())
                            {
                                decimal empID = Convert.ToDecimal(item);
                                RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation = fdDb.RS_AM_Operator_Station_Allocation.Where(op => op.Employee_ID == empID && DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(DateTime.Now)).FirstOrDefault();
                                fdDb.RS_AM_Operator_Station_Allocation.Remove(RS_AM_Operator_Station_Allocation);
                                fdDb.SaveChanges();
                            }
                        }

                        var operators_hist = db.RS_AM_Operator_Station_Allocation_History.Where(op => op.Employee_ID == emp_ID && DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(DateTime.Now)).Select(op => op.Employee_ID).ToList();
                        foreach (var item in operators_hist)
                        {
                            using (REIN_SOLUTIONEntities fdDb1 = new REIN_SOLUTIONEntities())
                            {
                                decimal empID = Convert.ToDecimal(item);
                                RS_AM_Operator_Station_Allocation_History RS_AM_Operator_Station_Allocation_History = fdDb1.RS_AM_Operator_Station_Allocation_History.Where(op => op.Employee_ID == empID && DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(DateTime.Now)).FirstOrDefault();
                                fdDb1.RS_AM_Operator_Station_Allocation_History.Remove(RS_AM_Operator_Station_Allocation_History);
                                fdDb1.SaveChanges();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
            }
            return Json(1, JsonRequestBehavior.AllowGet);
            }
            
            

    }
}
