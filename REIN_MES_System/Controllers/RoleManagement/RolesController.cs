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
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.RoleManagement
{
    /* Controller Name            : RolesController
    *  Description                : This controller is used to manage user roles
    *  Author, Timestamp          : Ajay Wagh       
    */
    public class RolesController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        FDSession fdSession = new FDSession();

        int roleId;
        String roleName;

        RS_Roles userRoles = new RS_Roles();

        General generalObj = new General();

        /* Action Name                : Index
        *  Description                : Action used to show the list user role
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Roles/REIN_MES_System
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Roles;
            globalData.subTitle = ResourceGlobal.Add;
            globalData.controllerName = "Roles";
            globalData.actionName = ResourceGlobal.Index;
            //globalData.contentTitle = ResourceRoles.Roles_Title_Add_User_Roles;
            //globalData.contentFooter = ResourceRoles.Roles_Title_Add_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            var RS_Roles = db.RS_Roles.Include(m => m.RS_Employee);
            return View(RS_Roles.ToList());
        }

        /* Action Name                : Details
        *  Description                : Action used to show the detail of user role
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : id
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Roles/Details/5
        public ActionResult Details(decimal id)
        {
            globalData.pageTitle = ResourceModules.Roles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Roles";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceRoles.Roles_Title_Details_User_Roles;
            //globalData.contentFooter = ResourceRoles.Roles_Title_Details_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Roles RS_Roles = db.RS_Roles.Find(id);
            if (RS_Roles == null)
            {
                return HttpNotFound();
            }
            return View(RS_Roles);
        }

        /* Action Name                : Create
        *  Description                : Action used to show add user role form
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : None
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Roles/Create
        public ActionResult Create()
        {

            globalData.pageTitle = ResourceModules.Roles;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Roles";
            globalData.actionName = ResourceGlobal.Create;
            //globalData.contentTitle = ResourceRoles.Roles_Title_Add_User_Roles;
            //globalData.contentFooter = ResourceRoles.Roles_Title_Add_User_Roles;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Menu_Roles = new SelectList(db.RS_Menus, "Menu_ID", "LinkName");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            return View();
        }

        /* Action Name                : Create
        *  Description                : Action used to add user role
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : RS_Roles
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Role_ID,Role_Name,Role_Description,Sort_Order,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Menu_ID")] RS_Roles RS_Roles)
        {
            if (ModelState.IsValid)
            {
                roleName = RS_Roles.Role_Name;


                if (RS_Roles.isRoleExists(roleName, 0))
                {
                    ModelState.AddModelError("Role_Name", ResourceMessages.Roles_Error_Role_Name_Exists);
                }
                else
                {
                    RS_Roles.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Roles.Inserted_Date = DateTime.Now;
                    db.RS_Roles.Add(RS_Roles);
                    db.SaveChanges();

                    foreach (var item in RS_Roles.Menu_ID)
                    {
                        RS_Menus obj = new RS_Menus();
                        obj = db.RS_Menus.Find(item);
                        RS_Menu_Role RS_Menu_Role = new RS_Menu_Role();
                        RS_Menu_Role.Menu_ID = (long)(item);
                        RS_Menu_Role.Role_ID = Convert.ToDecimal(RS_Roles.Role_ID);
                        RS_Menu_Role.Sort_Order = obj.Sort_Order;
                        RS_Menu_Role.Inserted_Date = DateTime.Now;
                        RS_Menu_Role.Inserted_Host = Request.UserHostName;
                        RS_Menu_Role.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.RS_Menu_Role.Add(RS_Menu_Role);
                        db.SaveChanges();
                    }

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Roles;
                    globalData.messageDetail = ResourceMessages.Roles_Success_User_Role_Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Roles;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Roles";
            globalData.actionName = ResourceGlobal.Create;
            //globalData.contentTitle = ResourceRoles.Roles_Title_Add_User_Roles;
           // globalData.contentFooter = ResourceRoles.Roles_Title_Add_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Roles.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Roles.Updated_User_ID);
            return View(RS_Roles);
        }

        /* Action Name                : Edit
        *  Description                : Action used to show edit user role form
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : id
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Roles/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Roles RS_Roles = db.RS_Roles.Find(id);
            if (RS_Roles == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Roles;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Roles";
            globalData.actionName = ResourceGlobal.Edit;
            //globalData.contentTitle = ResourceRoles.Roles_Title_Edit_User_Roles;
            //globalData.contentFooter = ResourceRoles.Roles_Title_Edit_User_Roles;
            ViewBag.GlobalDataModel = globalData;
            var abc = db.RS_Menu_Role.Where(x => x.Role_ID == id).Select(x => (x.Menu_ID)).ToArray();
            List<decimal> n = new List<decimal>();
            for (int i = 0; i < abc.Count(); i++)
            {
                n.Add(abc[i]);
            }
            ViewBag.Menu_ID = new MultiSelectList(db.RS_Menus, "Menu_ID", "LinkName", abc);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Roles.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Roles.Updated_User_ID);
            return View(RS_Roles);
        }

        /* Action Name                : Edit
        *  Description                : Action used to show edit user role form
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : RS_Roles
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Role_ID,Role_Name,Role_Description,Sort_Order,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Menu_ID")] RS_Roles RS_Roles)
        {
            if (ModelState.IsValid)
            {
                roleId = Convert.ToInt32(RS_Roles.Role_ID);
                roleName = RS_Roles.Role_Name;

                RS_Roles mmObj = new RS_Roles();
                mmObj = db.RS_Roles.Find(roleId);
                mmObj.Sort_Order = RS_Roles.Sort_Order;
                mmObj.Menu_ID = RS_Roles.Menu_ID;
                mmObj.Updated_Date = DateTime.Now;
                mmObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.Entry(mmObj).State = EntityState.Modified;
                db.SaveChanges();

                userRoles = db.RS_Roles.Find(roleId);
                userRoles.Role_Name = roleName;
                userRoles.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                userRoles.Updated_Date = DateTime.Now;
                userRoles.Menu_ID = RS_Roles.Menu_ID;
                userRoles.Is_Edited = true;
                db.Entry(userRoles).State = EntityState.Modified;
                db.SaveChanges();



                var menurole = db.RS_Menu_Role.Where(x => x.Role_ID == roleId).ToList();
                RS_Menu_Role[] menuroleArray = db.RS_Menu_Role.Where(x => x.Role_ID == roleId).ToArray();
                db.RS_Menu_Role.RemoveRange(menurole);

                for (int i = 0; i < menuroleArray.Count(); i++)
                {
                    generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Menu_Role", "Menu_Role_ID", menuroleArray[i].Menu_Role_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                }

                foreach (var item in RS_Roles.Menu_ID)
                {
                    RS_Menus obj = new RS_Menus();
                    obj = db.RS_Menus.Find(item);
                    RS_Menu_Role RS_Menu_Role = new RS_Menu_Role();
                    RS_Menu_Role.Menu_ID = (long)(item);
                    RS_Menu_Role.Role_ID = Convert.ToDecimal(RS_Roles.Role_ID);
                    RS_Menu_Role.Sort_Order = obj.Sort_Order;
                    RS_Menu_Role.Updated_Date = DateTime.Now;
                    RS_Menu_Role.Updated_Host = Request.UserHostName;
                    RS_Menu_Role.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.RS_Menu_Role.Add(RS_Menu_Role);
                    db.SaveChanges();
                }


                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Roles;
                globalData.messageDetail = ResourceMessages.Roles_Success_User_Role_Edit_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

                // }
            }

            globalData.pageTitle = ResourceModules.Roles;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Roles";
            globalData.actionName = ResourceGlobal.Edit;
           // globalData.contentTitle = ResourceRoles.Roles_Title_Edit_User_Roles;
            //globalData.contentFooter = ResourceRoles.Roles_Title_Edit_User_Roles;
            ViewBag.GlobalDataModel = globalData;
            //  ViewBag.Menu_ID = new SelectList(db.RS_Menus, "Menu_ID", "LinkName", db.RS_Menu_Role.Where(x => x.Role_ID == id).Select(x => x.Menu_ID));
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Roles.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Roles.Updated_User_ID);
            return View(RS_Roles);
        }

        /* Action Name                : Delete
        *  Description                : Action used to show delete user role for user confirnmation
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : id
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // GET: /Roles/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Roles RS_Roles = db.RS_Roles.Find(id);
            if (RS_Roles == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Roles;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Roles";
            globalData.actionName = ResourceGlobal.Delete;
            //globalData.contentTitle = ResourceRoles.Roles_Title_Delete_User_Roles;
            //globalData.contentFooter = ResourceRoles.Roles_Title_Delete_User_Roles;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Roles);
        }

        /* Action Name                : DeleteConfirmed
        *  Description                : Action used to delete user role
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : id
        *  Return Type                : ActionResult
        *  Revision                   : 1.0
        */
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {


                var menurole = db.RS_Menu_Role.Where(x => x.Role_ID == id).ToList();
                db.RS_Menu_Role.RemoveRange(menurole);

                RS_Roles RS_Roles = db.RS_Roles.Find(id);
                db.RS_Roles.Remove(RS_Roles);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Roles;
                globalData.messageDetail = ResourceMessages.Roles_Success_User_Role_Delete_Success;
                TempData["globalData"] = globalData;
            }
            catch (DbUpdateException ex)
            {
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Roles", "Role_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                //globalData.dbUpdateExceptionDetail = ex.InnerException.InnerException.Message.ToString();

                globalData.isAlertMessage = true;
                globalData.messageTitle = ResourceModules.Roles;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");
        }

        /* Action Name                : Dispose
        *  Description                : Action used to dispose the role controller object
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : disposing
        *  Return Type                : void
        *  Revision                   : 1.0
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
