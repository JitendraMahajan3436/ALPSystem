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
    /* Class Name                 : AssignSupervisorToManagerController
   *  Description                : This class is used to add/save Supervisor assigned to Manager against particular shop selected
   *  Author, Timestamp          : Jitendra Mahajan      
   */
    public class AssignSupervisorToManagerController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0, managerId = 0;
        int operatorId = 0;

        /*	    Action Name		    : Index
        *		Description		    : To Display the supervisors under manager information in grid
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    :
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AssignSupervisorToManager
        public ActionResult Index()
        {
            var RS_Assign_SupervisorToManager = db.RS_Assign_SupervisorToManager.Include(m => m.RS_Employee).Include(m => m.RS_Plants).Include(m => m.RS_Shops);
            return View(RS_Assign_SupervisorToManager.ToList());
        }


        /*	    Action Name		    : Details
         *		Description		    : To show the supervisors under manager detailed information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : id  
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // GET: AssignSupervisorToManager/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Assign_SupervisorToManager RS_Assign_SupervisorToManager = db.RS_Assign_SupervisorToManager.Find(id);
            if (RS_Assign_SupervisorToManager == null)
            {
                return HttpNotFound();
            }
            return View(RS_Assign_SupervisorToManager);
        }


        /*	    Action Name		    : Create
        *		Description		    : To read the supervisor under manager info which is to be saved
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AssignSupervisorToManager/Create
        public ActionResult Create()
        {
            try
            {


                int plant_Id = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                    // ViewBag.Plant_ID = new SelectList("");
                    ViewBag.Shop_ID = new SelectList("");
                    ViewBag.Manager_ID = new SelectList("");
                    ViewBag.ListofSupervisor = new SelectList("");
                    ViewBag.selectedSupervisors = new SelectList("");
                    //ViewBag.Type = new SelectList("True");
                    //plantId = Convert.ToInt32(TempData["plantId"].ToString());
                    //shopId = Convert.ToInt32(TempData["shopId"].ToString());              
                    //managerId = Convert.ToInt32(TempData["managerId"].ToString());


                    //// line quality station
                    //var managerList = from supervisorToManager in db.RS_Assign_SupervisorToManager
                    //                  where (from manager in db.RS_AM_Shop_Manager_Mapping where manager.Employee_ID == managerId select manager.Employee_ID).Contains(supervisorToManager.Manager_ID)
                    //                  select new
                    //                  {
                    //                      Manager_ID = supervisorToManager.Manager_ID
                    //                      //Supervisor_Name = db.RS_Employee.
                    //                  };

                    //ViewBag.Manager_ID = new SelectList(managerList, "Manager_ID", "Manager_ID", managerId);

                    //// not selected supervisor
                    //var noSelectedsupervisors = from employee in db.RS_Employee
                    //                          where !(from selectmanagers in db.RS_Assign_SupervisorToManager where selectmanagers.Manager_ID == managerId select selectmanagers.Manager_ID).Contains(employee.Employee_ID) 

                    //                          select new
                    //                          {
                    //                              Employee_ID = employee.Employee_ID,
                    //                              Employee_Name = employee.Employee_Name
                    //                          };

                    //ViewBag.ListofSupervisor = new SelectList(noSelectedsupervisors, "Employee_ID", "Employee_Name");

                    //// load selected supervisors
                    //var selectedsupervisors = from employee in db.RS_Employee
                    //                          join supervisor in db.RS_Assign_SupervisorToManager on employee.Employee_ID equals supervisor.AssignedSupervisor_ID
                    //                          where supervisor.Manager_ID == managerId
                    //                        select new
                    //                        {
                    //                            Employee_ID = employee.Employee_ID,
                    //                            Employee_Name = employee.Employee_Name
                    //                        };
                    //ViewBag.selectedSupervisors = new SelectList(selectedsupervisors, "Employee_ID", "Employee_Name");

                }
                else
                {
                    ViewBag.Manager_ID = new SelectList(db.RS_AM_Shop_Manager_Mapping.Where(p => p.Employee_ID == managerId), "Employee_ID", "Employee_Name");
                    ViewBag.ListofSupervisor = new SelectList(db.RS_Assign_SupervisorToManager.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name");
                    ViewBag.AssignedSupervisor_ID = new SelectList(db.RS_Assign_SupervisorToManager.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name");
                    // ViewBag.Type = new SelectList("1");
                }

                globalData.pageTitle = ResourceSupervisorToManager.OperatorToSupervisor;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = ResourceSupervisorToManager.Site_Variable_OfficerToManager;
                globalData.actionName = ResourceSupervisorToManager.Site_Variable_Index;
                globalData.contentTitle = ResourceSupervisorToManager.OfficerToManager_Title_OfficerToManager_Lists;
                globalData.contentFooter = ResourceSupervisorToManager.OfficerToManager_Title_OfficerToManager_Lists;


                ViewBag.GlobalDataModel = globalData;
                ViewBag.STM_ID = new SelectList(db.RS_Assign_SupervisorToManager, "STM_ID", "STM_ID");
                ViewBag.STM_ID = new SelectList(db.RS_Assign_SupervisorToManager, "STM_ID", "STM_ID");
                ViewBag.Manager_ID = new SelectList(db.RS_Employee.Where(S => S.Employee_ID == 0), "Employee_ID", "Employee_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(shop => shop.Plant_ID == plant_Id).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
                // View.AssignedSupervisor_ID=
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }


        /*	    Action Name		    : Create
         *		Description		    : To save the supervisors under manager information
         *		Author, Timestamp	: Jitendra Mahajan
         *		Input parameter	    : RS_Assign_SupervisorToManager object 
         *		Return Type		    : ActionResult
         *		Revision		    :
         */
        // POST: AssignSupervisorToManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "STM_ID,Plant_ID,Shop_ID,Manager_ID,AssignedSupervisor_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,selectedSupervisors")] RS_Assign_SupervisorToManager RS_Assign_SupervisorToManager)
        {
            if (ModelState.IsValid)
            {
                plantId = 1;// Convert.ToInt32(RS_Assign_SupervisorToManager.Plant_ID);
                shopId = Convert.ToInt32(RS_Assign_SupervisorToManager.Shop_ID);
                managerId = Convert.ToInt32(RS_Assign_SupervisorToManager.Manager_ID);

                {
                    // process to delete all the defects added for the station
                    RS_Assign_SupervisorToManager.deleteOperator(shopId, managerId, ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);//plantId,
                }

                for (int i = 0; i < RS_Assign_SupervisorToManager.selectedSupervisors.Count(); i++)
                {
                    operatorId = Convert.ToInt16(RS_Assign_SupervisorToManager.selectedSupervisors[i]);

                    if (operatorId == 0)
                        continue;

                    //if (RS_Assign_OperatorToSupervisor.isDefectAddedToStation(operatorId, stationId, plantId, shopId, lineId))
                    //{
                    //    // defect is already added no need to the defect
                    //}
                    else
                    {
                        //    if(i>0)
                        //        db.Entry(RS_Assign_SupervisorToManager).State = EntityState.Modified;

                        RS_Assign_SupervisorToManager.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        RS_Assign_SupervisorToManager.Inserted_Date = DateTime.Now;
                        RS_Assign_SupervisorToManager.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        // RS_Assign_OperatorToSupervisor. = ((FDSession)this.Session["FDSession"]).userHost;

                        RS_Assign_SupervisorToManager.AssignedSupervisor_ID = RS_Assign_SupervisorToManager.selectedSupervisors[i];

                        RS_Assign_SupervisorToManager mmAssignSupervisorObj = new RS_Assign_SupervisorToManager();
                        mmAssignSupervisorObj = RS_Assign_SupervisorToManager;

                        db.Entry(mmAssignSupervisorObj).State = EntityState.Detached;
                        //db.RS_Assign_SupervisorToManager.ChangeObjectState(mmAssignSupervisorObj, EntityState.Unchanged);

                        db.RS_Assign_SupervisorToManager.Add(mmAssignSupervisorObj);

                        db.SaveChanges();
                    }

                }
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceSupervisorToManager.OperatorToSupervisor;
                globalData.messageDetail = ResourceSupervisorToManager.OfficerToManager_Success_OfficerToManager_Add_Success;
                TempData["globalData"] = globalData;
                TempData["plantId"] = globalData;
                TempData["shopId"] = globalData;
                TempData["managerId"] = globalData;
                //db.RS_Assign_SupervisorToManager.Add(RS_Assign_SupervisorToManager);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }


            globalData.pageTitle = ResourceSupervisorToManager.OperatorToSupervisor;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Officer to manager";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceSupervisorToManager.OfficerToManager_Title_Add_OfficerToManager;
            globalData.contentFooter = ResourceSupervisorToManager.OfficerToManager_Title_Add_OfficerToManager;
            ViewBag.GlobalDataModel = globalData;


            ViewBag.STM_ID = new SelectList(db.RS_Assign_SupervisorToManager, "STM_ID", "STM_ID", RS_Assign_SupervisorToManager.STM_ID);
            ViewBag.STM_ID = new SelectList(db.RS_Assign_SupervisorToManager, "STM_ID", "STM_ID", RS_Assign_SupervisorToManager.STM_ID);
            ViewBag.Manager_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Assign_SupervisorToManager.Manager_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Assign_SupervisorToManager.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Assign_SupervisorToManager.Shop_ID);
            return RedirectToAction("Create");
            // return View(RS_Assign_SupervisorToManager);
        }


        /*	    Action Name		    : Edit
        *		Description		    : To read the supervisors under manager information which is to be edited
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AssignSupervisorToManager/Edit/5
        public ActionResult Edit(decimal id)
        {
            try
            {


                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Assign_SupervisorToManager RS_Assign_SupervisorToManager = db.RS_Assign_SupervisorToManager.Find(id);
                if (RS_Assign_SupervisorToManager == null)
                {
                    return HttpNotFound();
                }
                ViewBag.STM_ID = new SelectList(db.RS_Assign_SupervisorToManager, "STM_ID", "STM_ID", RS_Assign_SupervisorToManager.STM_ID);
                ViewBag.STM_ID = new SelectList(db.RS_Assign_SupervisorToManager, "STM_ID", "STM_ID", RS_Assign_SupervisorToManager.STM_ID);
                ViewBag.Manager_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Assign_SupervisorToManager.Manager_ID);
                // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Assign_SupervisorToManager.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Assign_SupervisorToManager.Shop_ID);
                return View(RS_Assign_SupervisorToManager);
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }


        /*	    Action Name		    : Edit
        *		Description		    : To edit the supervisor under manager information
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : RS_Assign_SupervisorToManager object 
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // POST: AssignSupervisorToManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "STM_ID,Plant_ID,Shop_ID,Manager_ID,AssignedSupervisor_ID,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date")] RS_Assign_SupervisorToManager RS_Assign_SupervisorToManager)
        {
            if (ModelState.IsValid)
            {
                RS_Assign_SupervisorToManager.Is_Edited = true;
                db.Entry(RS_Assign_SupervisorToManager).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.STM_ID = new SelectList(db.RS_Assign_SupervisorToManager, "STM_ID", "STM_ID", RS_Assign_SupervisorToManager.STM_ID);
            ViewBag.STM_ID = new SelectList(db.RS_Assign_SupervisorToManager, "STM_ID", "STM_ID", RS_Assign_SupervisorToManager.STM_ID);
            ViewBag.Manager_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Assign_SupervisorToManager.Manager_ID);
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Assign_SupervisorToManager.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Assign_SupervisorToManager.Shop_ID);
            return View(RS_Assign_SupervisorToManager);
        }


        /*	    Action Name		    : Delete
        *		Description		    : To Display the supervisor under manager information which is to be deleted
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id to be deleted row  
        *		Return Type		    : ActionResult
        *		Revision		    :
        */
        // GET: AssignSupervisorToManager/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceSupervisorToManager.OperatorToSupervisor;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = ResourceSupervisorToManager.Site_Variable_OfficerToManager;
            globalData.actionName = ResourceSupervisorToManager.Site_Variable_Index;
            globalData.contentTitle = ResourceSupervisorToManager.OfficerToManager_Title_OfficerToManager_Lists;
            globalData.contentFooter = ResourceSupervisorToManager.OfficerToManager_Title_OfficerToManager_Lists;
            RS_Assign_SupervisorToManager RS_Assign_SupervisorToManager = db.RS_Assign_SupervisorToManager.Find(id);
            if (RS_Assign_SupervisorToManager == null)
            {
                return HttpNotFound();
            }
            return View(RS_Assign_SupervisorToManager);
        }



        /*	    Action Name		    : DeleteConfirmed
        *		Description		    : To delete the supervisor under manager record
        *		Author, Timestamp	: Jitendra Mahajan
        *		Input parameter	    : id 
        *		Return Type		    : redirected to the index page
        *		Revision		    :
        */
        // POST: AssignSupervisorToManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try {
                RS_Assign_SupervisorToManager RS_Assign_SupervisorToManager = db.RS_Assign_SupervisorToManager.Find(id);
                db.RS_Assign_SupervisorToManager.Remove(RS_Assign_SupervisorToManager);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceSupervisorToManager.OperatorToSupervisor;
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
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from employee in db.RS_Employee
                       where employee.Category_ID == 3
                       && (from shopManager in db.RS_AM_Shop_Manager_Mapping where shopManager.Shop_ID == shopId && employee.Is_Deleted == null select shopManager.Employee_ID).Contains(employee.Employee_ID)
                         orderby employee.Employee_Name
                         select new
                         {
                             Id = employee.Employee_ID,
                             Value = employee.Employee_Name + "(" + employee.Employee_No + ")",
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
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
        public ActionResult GetSupervisorsByManagerID(int managerId, int? shopId)//
        {
            try
            {
                decimal plant_Id = ((FDSession)this.Session["FDSession"]).plantId;

                //  var st = (from supervisor in db.RS_AM_Line_Supervisor_Mapping
                //            where !(from SM in db.RS_Assign_SupervisorToManager
                //                    where SM.Plant_ID == plant_Id //&& SM.Shop_ID == shopId
                //                    select SM.AssignedSupervisor_ID)
                //.Contains(supervisor.Employee_ID)
                //            join b in db.RS_Employee on supervisor.Employee_ID equals b.Employee_ID
                //            where supervisor.Plant_ID == plant_Id && b.Category_ID == 2 && b.Is_Deleted != true
                //            orderby b.Employee_Name
                //            select new
                //            {
                //                Id = supervisor.Employee_ID,
                //                Value = b.Employee_Name + "(" + b.Employee_No + ")"
                //            }).Distinct();


                var st = (from supervisor in db.RS_AM_Line_Supervisor_Mapping
                          where !(from SM in db.RS_Assign_SupervisorToManager
                                  where SM.Plant_ID == plant_Id
                                  select SM.AssignedSupervisor_ID)
              .Contains(supervisor.Employee_ID)
                          join b in db.RS_Employee on supervisor.Employee_ID equals b.Employee_ID
                          where supervisor.Plant_ID == plant_Id &&  b.Is_Deleted != true && supervisor.Shop_ID == shopId
                          orderby b.Employee_Name
                          select new
                          {
                              Id = supervisor.Employee_ID,
                              Value = b.Employee_Name + "(" + b.Employee_No + ")"
                          }).Distinct();


                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
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
        public ActionResult GetAssignedSupervisorsByManagerID(int managerId, int shopId)
        {

            try
            {
                decimal plant_Id = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from employee in db.RS_Employee
                         where (from managerToSupervisor in db.RS_Assign_SupervisorToManager
                                where managerToSupervisor.Manager_ID == managerId && managerToSupervisor.Shop_ID == shopId
                                && managerToSupervisor.Plant_ID == plant_Id
                                select managerToSupervisor.AssignedSupervisor_ID).Contains(employee.Employee_ID)
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
                    return Json(null, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult SaveAssignedSupervisor(string Stations, int managerId, int shopId)
        {
            try
            {


                RS_Assign_SupervisorToManager RS_Assign_SupervisorToManager = new RS_Assign_SupervisorToManager();

                RS_Assign_SupervisorToManager.deleteOperator(shopId, managerId, ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);  //plantId, 

                //var stationItem = from station in db.RS_Assign_SupervisorToManager
                //                  where (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                //                  select station;

                //foreach (var item in stationItem.ToList())
                //{
                //    stationObj = db.RS_Stations.Find(item.Station_ID);
                //    stationObj.Is_Critical_Station = false;
                //    db.Entry(stationObj).State = EntityState.Modified;
                //    db.SaveChanges();
                //}


                string[] words;
                words = Stations.Split(',');

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


                    // operatorId = Convert.ToInt16(RS_Assign_SupervisorToManager.value);                                      

                    RS_Assign_SupervisorToManager.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    RS_Assign_SupervisorToManager.Shop_ID = shopId;
                    RS_Assign_SupervisorToManager.Inserted_Date = DateTime.Now;
                    RS_Assign_SupervisorToManager.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Assign_SupervisorToManager.Manager_ID = managerId;
                    RS_Assign_SupervisorToManager.AssignedSupervisor_ID = i;

                    //RS_Assign_SupervisorToManager mmAssignSupervisorObj = new RS_Assign_SupervisorToManager();
                    //mmAssignSupervisorObj = RS_Assign_SupervisorToManager;

                    db.Entry(RS_Assign_SupervisorToManager).State = EntityState.Detached;
                    db.RS_Assign_SupervisorToManager.Add(RS_Assign_SupervisorToManager);
                    db.SaveChanges();

                    //stationObj = db.RS_Stations.Find(i);
                    //stationObj.Is_Critical_Station = true;
                    //db.Entry(stationObj).State = EntityState.Modified;
                    // db.SaveChanges();
                    i = 0;
                }
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
}
    }

}
