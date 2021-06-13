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
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.RoleManagement
{
    /* Controller Name            : UserRolesController
    *  Description                : This controller is used to add/edit/delete/list user role
    *  Author, Timestamp          : Ajay Wagh       
    */
    public class UserRolesController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        int roleId, userId;
        RS_User_Roles userRoles = new RS_User_Roles();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Index
        // Input Parameter      : None
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to show the list of user role
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: /UserRoles/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.UserRoles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "UserRoles";
            globalData.actionName = ResourceGlobal.Details;
           // globalData.contentTitle = ResourceUserRoles.UserRoles_Title_Details_User_Roles;
           // globalData.contentFooter = ResourceUserRoles.UserRoles_Title_Details_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            var RS_User_Roles = db.RS_User_Roles.Include(m => m.RS_Roles).Include(m => m.RS_Employee);
            return View(RS_User_Roles.ToList());
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Details
        // Input Parameter      : id
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to show detail of user role
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: /UserRoles/Details/5
        public ActionResult Details(decimal id)
        {
            globalData.pageTitle = ResourceModules.UserRoles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "UserRoles";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceUserRoles.UserRoles_Title_Details_User_Roles;
           // globalData.contentFooter = ResourceUserRoles.UserRoles_Title_Details_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            if (id < 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_User_Roles RS_User_Roles = db.RS_User_Roles.Find(id);
            if (RS_User_Roles == null)
            {
                return HttpNotFound();
            }
            return View(RS_User_Roles);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Create
        // Input Parameter      : None
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to show add user role form
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: /UserRoles/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.UserRoles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "UserRoles";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceUserRoles.UserRoles_Title_Add_User_Roles;
            //globalData.contentFooter = ResourceUserRoles.UserRoles_Title_Add_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            var userObj = from employee in db.RS_Employee
                          select new
                         {
                             Employee_ID = employee.Employee_ID,
                             Employee_Name = employee.Employee_Name + "(" + employee.Employee_No + ")"
                         };
            //ViewBag.Role_ID = new SelectList(db.RS_Roles, "Role_ID", "Role_Name");
            ViewBag.Employee_ID = new SelectList(userObj, "Employee_ID", "Employee_Name");

            return View();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : fillRolesDropDown
        // Input Parameter      : userID
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to show the list of Roles
        //////////////////////////////////////////////////////////////////////////////////////////////////
        [OutputCache(Duration = 0)]
        public ActionResult fillRolesDropDown(decimal userID)
        {
            var roleObj = from a in db.RS_Roles
                          where !(from b in db.RS_User_Roles where b.Employee_ID == userID select b.Role_ID).Contains(a.Role_ID)
                          select new
                           {
                               Role_ID = a.Role_ID,
                               Role_Name = a.Role_Name
                           };

            return Json(roleObj, JsonRequestBehavior.AllowGet);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Create
        // Input Parameter      : MM_User_Role
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to add the user role
        //////////////////////////////////////////////////////////////////////////////////////////////////

        // POST: /UserRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "User_Role_Key,Employee_ID,Role_ID,Description,Inserted_Date,Updated_Date,Roles")] RS_User_Roles RS_User_Roles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime today = DateTime.Now;
                    decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                    string compName = ((FDSession)this.Session["FDSession"]).userHost;

                    foreach (decimal roleID in RS_User_Roles.Roles)
                    {
                        RS_User_Roles.Role_ID = roleID;

                        RS_User_Roles.Inserted_Date = DateTime.Now;
                        RS_User_Roles.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_User_Roles.Inserted_Host = compName;
                        db.RS_User_Roles.Add(RS_User_Roles);
                        db.SaveChanges();
                    }

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.UserRoles;
                    globalData.messageDetail = ResourceMessages.UserRoles_Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
                var userObj = from employee in db.RS_Employee
                              where employee.Employee_ID == RS_User_Roles.Employee_ID
                              select new
                              {
                                  Employee_ID = employee.Employee_ID,
                                  Employee_Name = employee.Employee_Name + "(" + employee.Employee_No + ")"
                              };
                var roleObj = from a in db.RS_Roles
                              where !(from b in db.RS_User_Roles where b.Employee_ID == RS_User_Roles.Employee_ID select b.Role_ID).Contains(a.Role_ID)
                              select new
                              {
                                  Role_ID = a.Role_ID,
                                  Role_Name = a.Role_Name
                              };
                ViewBag.Roles = new SelectList(roleObj, "Role_ID", "Role_Name", RS_User_Roles.Role_ID);
                ViewBag.Employee_ID = new SelectList(userObj, "Employee_ID", "Employee_Name", RS_User_Roles.Employee_ID);
            }
            catch (Exception exp)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.UserRoles;
                globalData.messageDetail = ResourceMessages.UserRoles_Create_Exception;
                TempData["globalData"] = globalData;
                generalHelper.addControllerException(exp, "UserRolesController", "Create(post)", ((FDSession)this.Session["FDSession"]).userId);
            }
            return View(RS_User_Roles);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Edit
        // Input Parameter      : id
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to show edit user role form
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: /UserRoles/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_User_Roles RS_User_Roles = db.RS_User_Roles.Find(id);
            if (RS_User_Roles == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.UserRoles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "UserRoles";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceUserRoles.UserRoles_Title_Edit_User_Roles;
            //globalData.contentFooter = ResourceUserRoles.UserRoles_Title_Edit_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            var userObj = from employee in db.RS_Employee
                          where employee.Employee_ID == RS_User_Roles.Employee_ID
                          select new
                          {
                              Employee_ID = employee.Employee_ID,
                              Employee_Name = employee.Employee_Name + "(" + employee.Employee_No + ")"
                          };
            var roleObj = (from a in db.RS_Roles
                           join b in db.RS_User_Roles on a.Role_ID equals b.Role_ID
                           where b.Employee_ID == RS_User_Roles.Employee_ID
                           select a.Role_ID).ToList();
            ViewBag.Roles = new MultiSelectList(db.RS_Roles, "Role_ID", "Role_Name", roleObj);
            ViewBag.Employee_ID = new SelectList(userObj, "Employee_ID", "Employee_Name", RS_User_Roles.Employee_ID);
            return View(RS_User_Roles);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Edit
        // Input Parameter      : MM_User_Role
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to show update user role
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // POST: /UserRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "User_Role_Key,Employee_ID,Role_ID,Description,Inserted_Date,Updatedt_Date,Roles")] RS_User_Roles RS_User_Roles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RS_User_Roles []userRoles=db.RS_User_Roles.Where(a => a.Employee_ID == RS_User_Roles.Employee_ID && a.Is_Qdms != true).ToArray();
                    db.RS_User_Roles.RemoveRange(db.RS_User_Roles.Where(a => a.Employee_ID == RS_User_Roles.Employee_ID && a.Is_Qdms != true));
                    db.SaveChanges();

                    for (int i = 0; i < userRoles.Count();i++ )
                    {
                        generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_User_Roles", "User_Role_Key", userRoles[i].User_Role_Key.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                    }

                    DateTime today = DateTime.Now;
                    decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                    string compName = ((FDSession)this.Session["FDSession"]).userHost;

                    foreach (decimal roleID in RS_User_Roles.Roles)
                    {
                        RS_Roles robj = db.RS_Roles.Find(roleID);
                        if(robj.Is_Qdms != true)
                        {
                            RS_User_Roles.Role_ID = roleID;

                            RS_User_Roles.Updated_Date = DateTime.Now;
                            RS_User_Roles.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            RS_User_Roles.Updated_Host = compName;
                            db.RS_User_Roles.Add(RS_User_Roles);
                            db.SaveChanges();
                        }

                    }

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.UserRoles;
                    globalData.messageDetail = ResourceMessages.UserRoles_Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.UserRoles;
                globalData.messageDetail = ResourceMessages.UserRoles_Edit_Falure;
                TempData["globalData"] = globalData;
            }
            catch (Exception exp)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.UserRoles;
                globalData.messageDetail = ResourceMessages.UserRoles_Edit_Falure;
                TempData["globalData"] = globalData;
                generalHelper.addControllerException(exp, "UserRolesController", "Edit(post)", ((FDSession)this.Session["FDSession"]).userId);
            }
                var userObj = from employee in db.RS_Employee
                              where employee.Employee_ID == RS_User_Roles.Employee_ID
                              select new
                              {
                                  Employee_ID = employee.Employee_ID,
                                  Employee_Name = employee.Employee_Name + "(" + employee.Employee_No + ")"
                              };

                var roleObj = (from a in db.RS_Roles
                               join b in db.RS_User_Roles on a.Role_ID equals b.Role_ID
                               where b.Employee_ID == RS_User_Roles.Employee_ID
                               select a.Role_ID).ToList();
                ViewBag.Roles = new MultiSelectList(db.RS_Roles, "Role_ID", "Role_Name", roleObj);
                ViewBag.Employee_ID = new SelectList(userObj, "Employee_ID", "Employee_Name", RS_User_Roles.Employee_ID);
            
            return View(RS_User_Roles);

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : showRoleMenuList
        // Input Parameter      : roleID
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to show the Menus in the Role
        //////////////////////////////////////////////////////////////////////////////////////////////////
        [OutputCache(Duration = 0)]
        public ActionResult showRoleMenuList(decimal[] roleID)
        {
            try
            {
                var menusObj = db.RS_Menu_Role.Where(a => roleID.Contains(a.Role_ID)).Select(a => new
                {
                    Role_Name = a.RS_Roles.Role_Name,
                    Menu_ID = a.Menu_ID,
                    Menu_Name = a.RS_Menus.LinkName
                }).GroupBy(a => a.Role_Name).ToList();
                return Json(menusObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "UserRolesController", "showRoleMenuList(roleID)", ((FDSession)this.Session["FDSession"]).userId);
            }
            return null;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Delete
        // Input Parameter      : id
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh 
        // Description          : Action used to show the delete user role form
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // GET: /UserRoles/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_User_Roles RS_User_Roles = db.RS_User_Roles.Find(id);
            if (RS_User_Roles == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.UserRoles;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "UserRoles";
            globalData.actionName = ResourceGlobal.Delete;
           // globalData.contentTitle = ResourceUserRoles.UserRoles_Title_Details_User_Roles;
           // globalData.contentFooter = ResourceUserRoles.UserRoles_Title_Details_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_User_Roles);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : DeleteConfirmed
        // Input Parameter      : id
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to delete user role
        //////////////////////////////////////////////////////////////////////////////////////////////////
        // POST: /UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_User_Roles RS_User_Roles = db.RS_User_Roles.Find(id);
                db.RS_User_Roles.Remove(RS_User_Roles);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.UserRoles;
                globalData.messageDetail = ResourceMessages.UserRoles_Delete_Success;

            }
            catch (DbUpdateException exp)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.UserRoles;
                globalData.messageDetail = ResourceMessages.UserRoles_Delete_Dependency_Failure;
            }
            catch (Exception exp)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.UserRoles;
                globalData.messageDetail = ResourceMessages.UserRoles_Delete_Failure;
                generalHelper.addControllerException(exp, "UserRolesController", "DeleteConfirmed(id)", ((FDSession)this.Session["FDSession"]).userId);
            }
            TempData["globalData"] = globalData;
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
