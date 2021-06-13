using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Helper;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /* Class Name                 : ShopManagerConfigurationController
    *  Description                : This class is used to perform the basic operations of insert, update, edit and delete of managers of shop for manager configuration
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class ShopManagerConfigurationController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_AM_Shop_Manager_Mapping mmManagerObj = new RS_AM_Shop_Manager_Mapping();
        decimal empId = 0;
        decimal shopId = 0;
        FDSession fdSession = new FDSession();
        General generalObj = new General();

        /*	    Action Name		    : Index
        *		Description		    : To Display the managers information in grid
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: ShopManagerConfiguration
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Manager;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = ResourceModules.Manager;
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceGlobal.Managers + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceGlobal.Managers + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            var RS_AM_Shop_Manager_Mapping = db.RS_AM_Shop_Manager_Mapping.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Employee2).Include(m => m.RS_Plants).Include(m => m.RS_Shops);
            return View(RS_AM_Shop_Manager_Mapping.ToList());
        }


        /*	    Action Name		    : Details
         *		Description		    : To show the managers detailed information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : id of manager whose information is to be displayed 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: ShopManagerConfiguration/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping = db.RS_AM_Shop_Manager_Mapping.Find(id);
            if (RS_AM_Shop_Manager_Mapping == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Manager;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Managers";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Manager + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Manager + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Shop_Manager_Mapping);
        }


        /*	    Action Name		    : Create
        *		Description		    : To read the manager info which is to be saved
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: ShopManagerConfiguration/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Manager;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = ResourceModules.Manager;
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceGlobal.Managers + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceGlobal.Managers + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;




            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");

            //var employeeName = from managerName in db.RS_AM_Shop_Manager_Mapping
            //                   join employee in db.RS_Employee on managerName.Employee_ID equals employee.Employee_ID
            //                   where managerName.Shop_ID == shopId
            //                   select new
            //                   {
            //                       Employee_ID = employee.Employee_ID,
            //                       Employee_Name = employee.Employee_Name + "(" + employee.Employee_ID + ")"
            //                   };
            ViewBag.Employee_ID = new SelectList(db.RS_Employee.Where(m => m.Employee_ID == 0), "Employee_ID", "Employee_Name");

            //ViewBag.Employee_ID = new SelectList(db.RS_AM_Shop_Manager_Mapping, "Employee_ID", "Employee_ID");
            //ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name",0);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", 0);
            return View();
        }



        /*	    Action Name		    : Create
         *		Description		    : To save the managers information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : RS_AM_Shop_Manager_Mapping object 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // POST: ShopManagerConfiguration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SRS_ID,Plant_ID,Shop_ID,Employee_ID,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping)
        {
            if (ModelState.IsValid)
            {
                empId = RS_AM_Shop_Manager_Mapping.Employee_ID;
                shopId = RS_AM_Shop_Manager_Mapping.Shop_ID;
                RS_AM_Shop_Manager_Mapping.Plant_ID = 1;
                if (RS_AM_Shop_Manager_Mapping.IsManagerExists(empId, shopId))
                {
                    ModelState.AddModelError("Employee_Name", ResourceValidation.Exist);
                }
                else
                {
                    RS_AM_Shop_Manager_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_AM_Shop_Manager_Mapping.Inserted_Date = DateTime.Now;
                    RS_AM_Shop_Manager_Mapping.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_AM_Shop_Manager_Mapping.Add(RS_AM_Shop_Manager_Mapping);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Manager;
                    globalData.messageDetail = ResourceModules.Manager + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
            }


            globalData.pageTitle = ResourceModules.Manager;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Managers";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Manager + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Manager + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Updated_User_ID);
            // ViewBag.Manager_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Employee_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Employee_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Shop_Manager_Mapping.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Shop_Manager_Mapping.Shop_ID);
            return View(RS_AM_Shop_Manager_Mapping);
        }


        /*	    Action Name		    : Edit
        *		Description		    : To read the managers information which is to be edited
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: ShopManagerConfiguration/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping = db.RS_AM_Shop_Manager_Mapping.Find(id);
            if (RS_AM_Shop_Manager_Mapping == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Manager;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Manager";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Manager + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Manager + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Updated_User_ID);
            //ViewBag.Manager_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Employee_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Employee_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Shop_Manager_Mapping.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Shop_Manager_Mapping.Shop_ID);
            return View(RS_AM_Shop_Manager_Mapping);

        }



        /*	    Action Name		    : Edit
        *		Description		    : To edit the manager information
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : RS_AM_Shop_Manager_Mapping object 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // POST: ShopManagerConfiguration/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SRS_ID,Plant_ID,Shop_ID,Employee_ID,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping)
        {
            mmManagerObj = new RS_AM_Shop_Manager_Mapping();
            if (ModelState.IsValid)
            {
                empId = RS_AM_Shop_Manager_Mapping.Employee_ID;
                shopId = RS_AM_Shop_Manager_Mapping.Shop_ID;
                if (RS_AM_Shop_Manager_Mapping.IsManagerExists(empId, shopId))
                {
                    ModelState.AddModelError("Employee_ID", ResourceValidation.Exist);
                }
                else
                {
                    mmManagerObj = db.RS_AM_Shop_Manager_Mapping.Find(RS_AM_Shop_Manager_Mapping.SRS_ID);
                    mmManagerObj.Shop_ID = RS_AM_Shop_Manager_Mapping.Shop_ID;
                    mmManagerObj.Plant_ID = 1;// RS_AM_Shop_Manager_Mapping.Plant_ID;
                    mmManagerObj.Employee_ID = RS_AM_Shop_Manager_Mapping.Employee_ID;
                    mmManagerObj.Inserted_Date = db.RS_AM_Shop_Manager_Mapping.Find(RS_AM_Shop_Manager_Mapping.SRS_ID).Inserted_Date;
                    mmManagerObj.Inserted_User_ID = db.RS_AM_Shop_Manager_Mapping.Find(RS_AM_Shop_Manager_Mapping.SRS_ID).Inserted_User_ID;
                    mmManagerObj.Inserted_Host = db.RS_AM_Shop_Manager_Mapping.Find(RS_AM_Shop_Manager_Mapping.SRS_ID).Inserted_Host;
                    mmManagerObj.Is_Edited = true;
                    mmManagerObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmManagerObj.Updated_Date = DateTime.Now;
                    mmManagerObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Manager;
                    globalData.messageDetail = ResourceModules.Manager+" "+ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;


                    db.Entry(mmManagerObj).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Manager;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Managers";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Manager + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Manager + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;


            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Updated_User_ID);
            ViewBag.Employee_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_AM_Shop_Manager_Mapping.Employee_ID);
            //ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Shop_Manager_Mapping.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_AM_Shop_Manager_Mapping.Shop_ID);
            return View(RS_AM_Shop_Manager_Mapping);
        }


        /*	    Action Name		    : Delete
        *		Description		    : To Display the manager information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: ShopManagerConfiguration/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping = db.RS_AM_Shop_Manager_Mapping.Find(id);
            if (RS_AM_Shop_Manager_Mapping == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Manager;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Managerss";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Manager + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Manager + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Shop_Manager_Mapping);
        }


        /*	    Action Name		    : DeleteConfirmed
        *		Description		    : To delete the manager record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id of manager whose record is to be deleted
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: ShopManagerConfiguration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_AM_Shop_Manager_Mapping RS_AM_Shop_Manager_Mapping = db.RS_AM_Shop_Manager_Mapping.Find(id);
                db.RS_AM_Shop_Manager_Mapping.Remove(RS_AM_Shop_Manager_Mapping);
                db.SaveChanges();
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_AM_Shop_Manager_Mapping", "SRS_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Manager;
                globalData.messageDetail = ResourceModules.Manager + " " + ResourceMessages.Delete_Success;
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
        public ActionResult GetOperatorByStationID(int stationId)
        {
            try
            {
                int userId = 71;// ((FDSession)this.Session["FDSession"]).userId; mk patil=689, kureshi=71, vijay anand=451
                var st = from employee in db.RS_Employee
                         where (from operators in db.RS_Assign_OperatorToSupervisor where operators.Supervisor_ID == userId select operators.AssignedOperator_ID).Contains(employee.Employee_ID)
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")",
                         };

                //var st = from employee in db.RS_Employee
                //         where !(from shopManager in db.RS_AM_Shop_Manager_Mapping  select shopManager.Employee_ID).Contains(employee.Employee_ID)
                //         && !(from lineSupervisor in db.RS_AM_Line_Supervisor_Mapping  select lineSupervisor.Employee_ID).Contains(employee.Employee_ID)
                //         && !(from assignOperator in db.RS_Assign_OperatorToSupervisor select assignOperator.AssignedOperator_ID).Contains(employee.Employee_ID)
                //         select new
                //         {
                //             Id = employee.Employee_ID,
                //             Value = employee.Employee_Name + "(" + employee.Employee_ID + ")",
                //         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetOperatorSkillSetByOperatorID(int operatorId)
        {
            try
            {              
                var st = from skillSet in db.RS_AM_Skill_Set
                         where (from skilledoperators in db.RS_AM_Employee_SkillSet where skilledoperators.Employee_ID == operatorId select skilledoperators.Skill_ID).Contains(skillSet.Skill_ID)                        
                         select new
                         {
                             Id = skillSet.Skill_ID,
                             Value = skillSet.Skill_Set + "(" + skillSet.Skill_ID + ")",
                         };              

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSkilledOperatorByStationID(int stationId)
        {
            try
            {
               // int userId = 1;// ((FDSession)this.Session["FDSession"]).userId;
                var st = from employee in db.RS_Employee
                         where (from skilledoperators in db.RS_AM_Employee_SkillSet where skilledoperators.Station_ID == stationId select skilledoperators.Employee_ID).Contains(employee.Employee_ID)
                        // && where (from skilledOperators in db.RS_AM_Employee_SkillSet where skilledOperators.Station_ID==stationId && )
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")",
                         };

                //var st = from employee in db.RS_Employee
                //         where !(from shopManager in db.RS_AM_Shop_Manager_Mapping  select shopManager.Employee_ID).Contains(employee.Employee_ID)
                //         && !(from lineSupervisor in db.RS_AM_Line_Supervisor_Mapping  select lineSupervisor.Employee_ID).Contains(employee.Employee_ID)
                //         && !(from assignOperator in db.RS_Assign_OperatorToSupervisor select assignOperator.AssignedOperator_ID).Contains(employee.Employee_ID)
                //         select new
                //         {
                //             Id = employee.Employee_ID,
                //             Value = employee.Employee_Name + "(" + employee.Employee_ID + ")",
                //         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
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
                // lineId = 14;
                var st = from employee in db.RS_Employee
                         join lineSupervisor in db.RS_AM_Line_Supervisor_Mapping on employee.Employee_ID equals lineSupervisor.Employee_ID
                         where lineSupervisor.Line_ID == lineId
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /* Action Name                : GetSupervisorByLineID
    *  Description                : Action used to get the Supervisor list by line
    *  Author, Timestamp          : Jitendra Mahajan
    *  Input parameter            : lineId (line id)
    *  Return Type                : ActionResult
    *  Revision                   : 1.0
    */
        public ActionResult GetSupervisorByShopID(int shopId)
        {
            try
            {
                //shopId = 14;
                var st = from employee in db.RS_Employee
                         join shopSupervisor in db.RS_AM_Line_Supervisor_Mapping on employee.Employee_ID equals shopSupervisor.Employee_ID
                         where shopSupervisor.Shop_ID == shopId
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /* Action Name                : GetManagerByShopID
   *  Description                : Action used to get the Manager list by line
   *  Author, Timestamp          : Jitendra Mahajan
   *  Input parameter            : shopId (shopId id)
   *  Return Type                : ActionResult
   *  Revision                   : 1.0
   */
        public ActionResult GetManagerByShopID(int shopId)
        {
            try
            {
                //shopId = 1;
                //var st = from employee in db.RS_Employee
                //         join shopManager in db.RS_AM_Shop_Manager_Mapping on employee.Employee_ID equals shopManager.Employee_ID
                //         join lineSupervisor in db.RS_AM_Line_Supervisor_Mapping on employee.Employee_ID equals lineSupervisor.Employee_ID
                //         join assignOperator in db.RS_Assign_OperatorToSupervisor on employee.Employee_ID equals assignOperator.AssignedOperator_ID
                //        // where shopManager.Shop_ID == shopId
                //         select new
                //         {
                //             Id = employee.Employee_ID,
                //             Value = employee.Employee_Name + "(" + employee.Employee_ID + ")",
                //         };

                var st = from employee in db.RS_Employee
                         where !(from shopManager in db.RS_AM_Shop_Manager_Mapping where shopManager.Shop_ID == shopId select shopManager.Employee_ID).Contains(employee.Employee_ID)
                         && !(from lineSupervisor in db.RS_AM_Line_Supervisor_Mapping where lineSupervisor.Shop_ID == shopId select lineSupervisor.Employee_ID).Contains(employee.Employee_ID)
                         && !(from assignOperator in db.RS_Assign_OperatorToSupervisor select assignOperator.AssignedOperator_ID).Contains(employee.Employee_ID)
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")",
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }



        /* Action Name                : GetManagerListByShopID
  *  Description                : Action used to get the Manager list by line
  *  Author, Timestamp          : Jitendra Mahajan
  *  Input parameter            : shopId (shopId id)
  *  Return Type                : ActionResult
  *  Revision                   : 1.0
  */
        public ActionResult GetManagerListByShopID(int shopId)
        {
            try
            {
                var st = from employee in db.RS_Employee
                          where employee.Category_ID == 3 &&

                         (from shopManager in db.RS_AM_Shop_Manager_Mapping where shopManager.Shop_ID == shopId select shopManager.Employee_ID).Contains(employee.Employee_ID)
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")",
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /* Action Name                : GetSupervisorsByManagerID
    *  Description                : Action used to get the Supervisor list by Manager Id
    *  Author, Timestamp          : Jitendra Mahajan
    *  Input parameter            : managerId 
    *  Return Type                : ActionResult
    *  Revision                   : 1.0
    */
        public ActionResult GetSupervisorsByManagerID(int managerId)//, int shopId
        {
            try
            {
                var st = (from supervisor in db.RS_AM_Line_Supervisor_Mapping
                         where !(from SM in db.RS_Assign_SupervisorToManager
                                 select SM.AssignedSupervisor_ID)
             .Contains(supervisor.Employee_ID)
                         join b in db.RS_Employee on supervisor.Employee_ID equals b.Employee_ID 
                         select new
                         {
                             Id = supervisor.Employee_ID,
                             Value = b.Employee_Name + "(" + b.Employee_No + ")"
                         }).Distinct();


                //var st = from a in db.RS_Employee
                //         join b in db.RS_AM_Shop_Manager_Mapping on a.Employee_ID equals b.Employee_ID into b_join
                //         from b in b_join.DefaultIfEmpty()
                //         join c in db.RS_Assign_SupervisorToManager on new { Employee_ID = a.Employee_ID } equals new { Employee_ID = c.AssignedSupervisor_ID } into c_join
                //         from c in c_join.DefaultIfEmpty()
                //         join d in db.RS_AM_Shop_Manager_Mapping on a.Employee_ID equals d.Employee_ID into d_join
                //         from d in d_join.DefaultIfEmpty()
                //         where
                //           b.Employee_ID == null &&
                //           c.AssignedSupervisor_ID == null &&
                //           d.Employee_ID == null
                //where !(from OperatorToSupervisor in db.RS_Assign_OperatorToSupervisor 
                //        where OperatorToSupervisor.Supervisor_ID == supervisorId select OperatorToSupervisor.AssignedOperator_ID)
                //.Contains(employee.Employee_ID)
                // && employee.Is_Critical_Station == true

                //select new
                //         {
                //             Id = supervisor.Employee_ID,
                //             Value = supervisor.Employee_Name + "(" + a.Employee_No + ")"
                //         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /* Action Name                : GetAssignedSupervisorsByManagerID
  *  Description                : Action used to get the Assigned Supervisor list by Manager Id
  *  Author, Timestamp          : Jitendra Mahajan
  *  Input parameter            : managerId 
  *  Return Type                : ActionResult
  *  Revision                   : 1.0
  */
        public ActionResult GetAssignedSupervisorsByManagerID(int managerId)
        {

            try
            {
                var st = from employee in db.RS_Employee
                         where (from managerToSupervisor in db.RS_Assign_SupervisorToManager where managerToSupervisor.Manager_ID == managerId select managerToSupervisor.AssignedSupervisor_ID).Contains(employee.Employee_ID)
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
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
                var st = from employee in db.RS_Employee
                         where (from supervisorToOperator in db.RS_Assign_OperatorToSupervisor where supervisorToOperator.Supervisor_ID == supervisorId select supervisorToOperator.AssignedOperator_ID).Contains(employee.Employee_ID)
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }


        /* Action Name                : GetEmployeesBySessionID
   *  Description                : Action used to get the Employee list by Session Id
   *  Author, Timestamp          : Jitendra Mahajan
   *  Input parameter            : sessionId 
   *  Return Type                : ActionResult
   *  Revision                   : 1.0
   */
        public ActionResult GetEmployeesBySessionID(int sessionId)
        {
            try
            {
                var st = from a in db.RS_Employee
                         //  join b in db.RS_AM_Shop_Manager_Mapping on a.Employee_ID equals b.Employee_ID into b_join
                         // from b in b_join.DefaultIfEmpty()
                         join c in db.RS_Training_Users on new { Employee_ID = a.Employee_ID } equals new { Employee_ID = c.Employee_ID } into c_join
                         from c in c_join.DefaultIfEmpty()
                         //  join d in db.RS_AM_Shop_Manager_Mapping on a.Employee_ID equals d.Employee_ID into d_join
                         // from d in d_join.DefaultIfEmpty()
                         where
                             // b.Employee_ID == null &&
                           c.Employee_ID == null //&&
                         // d.Employee_ID == null                         
                         select new
                         {
                             Id = a.Employee_ID,
                             Value = a.Employee_Name + "(" + a.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /* Action Name                : GetAssignedEmployeesBySessionID
  *  Description                : Action used to get the Assigned Employee list by Session Id
  *  Author, Timestamp          : Jitendra Mahajan
  *  Input parameter            : sessionId 
  *  Return Type                : ActionResult
  *  Revision                   : 1.0
  */
        public ActionResult GetAssignedEmployeesBySessionID(int sessionId)
        {

            try
            {
                var st = from employee in db.RS_Employee
                         where (from AssignedEmployee in db.RS_Training_Users where AssignedEmployee.Training_Session_ID == sessionId select AssignedEmployee.Employee_ID).Contains(employee.Employee_ID)
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
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
                var st = from a in db.RS_Employee
                         join b in db.RS_AM_Shop_Manager_Mapping on a.Employee_ID equals b.Employee_ID into b_join
                         from b in b_join.DefaultIfEmpty()
                         join c in db.RS_Assign_OperatorToSupervisor on new { Employee_ID = a.Employee_ID } equals new { Employee_ID = c.AssignedOperator_ID } into c_join
                         from c in c_join.DefaultIfEmpty()
                         join d in db.RS_AM_Line_Supervisor_Mapping on a.Employee_ID equals d.Employee_ID into d_join
                         from d in d_join.DefaultIfEmpty()
                         where
                           b.Employee_ID == null &&
                           c.AssignedOperator_ID == null &&
                           d.Employee_ID == null
                         // && a.Category_ID==1
                         //where !(from OperatorToSupervisor in db.RS_Assign_OperatorToSupervisor 
                         //        where OperatorToSupervisor.Supervisor_ID == supervisorId select OperatorToSupervisor.AssignedOperator_ID)
                         //.Contains(employee.Employee_ID)
                         // && employee.Is_Critical_Station == true
                         select new
                         {
                             Id = a.Employee_ID,
                             Value = a.Employee_Name + "(" + a.Employee_No + ")"
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }



        /* Action Name                : GetoperatorsBySupervisorID
   *  Description                : Action used to get the operator list by supervisor Id
   *  Author, Timestamp          : Jitendra Mahajan
   *  Input parameter            : supervisorId 
   *  Return Type                : ActionResult
   *  Revision                   : 1.0
   */
        public ActionResult GetTrainingsByStationID(int stationId)
        {
            try
            {
                var st = from Training in db.RS_Production_Training
                         where Training.Station_ID == stationId
                         select new
                         {
                             Id = Training.Training_ID,
                             Value = Training.Training_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetSessionsByTrainingId
  *  Description                : Action used to get the Sessions list by training Id
  *  Author, Timestamp          : Jitendra Mahajan
  *  Input parameter            : trainingId 
  *  Return Type                : ActionResult
  *  Revision                   : 1.0
  */
        public ActionResult GetSessionsByTrainingId(int trainingId)
        {
            try
            {
                var st = from Sessions in db.RS_Training_Sessions
                         where Sessions.Training_ID == trainingId
                         select new
                         {
                             Id = Sessions.Training_Session_ID,
                             Value = Sessions.Training_Session_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /* Action Name                : GetTrainingByAssignedEmployeeId
 *  Description                : Action used to get the Sessions list by training Id
 *  Author, Timestamp          : Jitendra Mahajan
 *  Input parameter            : trainingId 
 *  Return Type                : ActionResult
 *  Revision                   : 1.0
 */
        public ActionResult GetTrainingByAssignedEmployeeId(int AssignedEmployee)
        {
            try
            {
                var st = from skillset in db.RS_AM_Skill_Set
                         // where skillset.Skill_ID == AssignedEmployee
                         select new
                         {
                             Id = skillset.Skill_ID,
                             Value = skillset.Skill_Set,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
