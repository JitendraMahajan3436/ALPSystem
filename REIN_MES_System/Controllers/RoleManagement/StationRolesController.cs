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
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.RoleManagement
{
    public class StationRolesController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_User_Roles userRoles = new RS_User_Roles();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        // GET: StationRoles
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.StationRoles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "StationRoles";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
            //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
            ViewBag.GlobalDataModel = globalData;

            var RS_Station_Roles = db.RS_Station_Roles.Include(m => m.RS_Roles).Include(m => m.RS_Stations);
            return View(RS_Station_Roles.ToList());
        }

        // GET: StationRoles/Details/5
        public ActionResult Details(decimal id)
        {
            globalData.pageTitle = ResourceModules.StationRoles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "StationRoles";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
            //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
            ViewBag.GlobalDataModel = globalData;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Station_Roles RS_Station_Roles = db.RS_Station_Roles.Find(id);
            if (RS_Station_Roles == null)
            {
                return HttpNotFound();
            }
            return View(RS_Station_Roles);
        }

        // GET: StationRoles/Create
        public ActionResult Create()
        {

            globalData.pageTitle = ResourceModules.StationRoles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "StationRoles";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
            //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
            ViewBag.GlobalDataModel = globalData;

            var stationObj = from station in db.RS_Stations
                             select new
                             {
                                 Station_ID = station.Station_ID,
                                 Station_Name = station.Station_Name + "(" + station.Station_IP_Address + ")"
                             };

            //ViewBag.Roles = new SelectList(db.RS_Roles, "Role_ID", "Role_Name");
            ViewBag.Station_ID = new SelectList(stationObj, "Station_ID", "Station_Name");
            return View();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : fillRolesDropDown
        // Input Parameter      : stationID
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh 
        // Description          : Action used to show add user role form
        //////////////////////////////////////////////////////////////////////////////////////////////////
        [OutputCache(Duration = 0)]
        public ActionResult fillRolesDropDown(decimal stationID)
        {
            var roleObj = from a in db.RS_Roles
                          where !(from b in db.RS_Station_Roles where b.Station_ID == stationID select b.Role_ID).Contains(a.Role_ID)
                          select new
                          {
                              Role_ID = a.Role_ID,
                              Role_Name = a.Role_Name
                          };

            return Json(roleObj, JsonRequestBehavior.AllowGet);
        }

        // POST: StationRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Station_Role_Key,Station_ID,Role_ID,Description,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Inserted_Host,Updated_User_ID,Updated_Date,Updated_Host,Roles")] RS_Station_Roles RS_Station_Roles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime today = DateTime.Now;
                    decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                    string compName = ((FDSession)this.Session["FDSession"]).userHost;

                    foreach (decimal roleID in RS_Station_Roles.Roles)
                    {
                        RS_Station_Roles.Role_ID = roleID;

                        RS_Station_Roles.Inserted_Date = DateTime.Now;
                        RS_Station_Roles.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Station_Roles.Inserted_Host = compName;
                        db.RS_Station_Roles.Add(RS_Station_Roles);
                        db.SaveChanges();
                    }

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceMessages.StationRolesTitle;
                    globalData.messageDetail = ResourceMessages.StationRoles_Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exp)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMessages.StationRolesTitle;
                globalData.messageDetail = ResourceMessages.StationRoles_Edit_Falure;
                TempData["globalData"] = globalData;
                generalHelper.addControllerException(exp, "UserRolesController", "Edit(post)", ((FDSession)this.Session["FDSession"]).userId);
            }
            var stationObj = from station in db.RS_Stations
                             where station.Station_ID == RS_Station_Roles.Station_ID
                             select new
                             {
                                 Station_ID = station.Station_ID,
                                 Station_Name = station.Station_Name + "(" + station.Station_IP_Address + ")"
                             };
            var roleObj = from a in db.RS_Roles
                          where !(from b in db.RS_Station_Roles where b.Station_ID == RS_Station_Roles.Station_ID select b.Role_ID).Contains(a.Role_ID)
                          select new
                          {
                              Role_ID = a.Role_ID,
                              Role_Name = a.Role_Name
                          };
            ViewBag.Roles = new SelectList(roleObj, "Role_ID", "Role_Name", RS_Station_Roles.Role_ID);
            ViewBag.Station_ID = new SelectList(stationObj, "Station_ID", "Station_Name", RS_Station_Roles.Station_ID);
            return View(RS_Station_Roles);
        }

        // GET: StationRoles/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Station_Roles RS_Station_Roles = db.RS_Station_Roles.Find(id);
            if (RS_Station_Roles == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.StationRoles;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "StationRoles";
            globalData.actionName = ResourceGlobal.Details;
            //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Edit_StationRoles;
            //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Edit_StationRoles;
            ViewBag.GlobalDataModel = globalData;

            var stationObj = from station in db.RS_Stations
                             where station.Station_ID == RS_Station_Roles.Station_ID
                             select new
                             {
                                 Station_ID = station.Station_ID,
                                 Station_Name = station.Station_Name + "(" + station.Station_IP_Address + ")"
                             };
            var roleObj = (from a in db.RS_Roles
                           join b in db.RS_Station_Roles on a.Role_ID equals b.Role_ID
                           where b.Station_ID == RS_Station_Roles.Station_ID
                           select a.Role_ID).ToList();

            ViewBag.Roles = new MultiSelectList(db.RS_Roles, "Role_ID", "Role_Name", roleObj);
            ViewBag.Station_ID = new SelectList(stationObj, "Station_ID", "Station_Name", RS_Station_Roles.Station_ID);
            return View(RS_Station_Roles);
        }

        // POST: StationRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Station_Role_Key,Station_ID,Role_ID,Description,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Inserted_Host,Updated_User_ID,Updated_Date,Updated_Host,Roles")] RS_Station_Roles RS_Station_Roles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RS_Station_Roles[] stationRoles = db.RS_Station_Roles.Where(a => a.Station_ID == RS_Station_Roles.Station_ID).ToArray();
                    db.RS_Station_Roles.RemoveRange(db.RS_Station_Roles.Where(a => a.Station_ID == RS_Station_Roles.Station_ID));
                    db.SaveChanges();
                    for(int i=0;i<stationRoles.Count();i++)
                    {
                        generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Station_Roles", "Station_Role_Key", stationRoles[i].Station_Role_Key.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                    }

                    DateTime today = DateTime.Now;
                    decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                    string compName = ((FDSession)this.Session["FDSession"]).userHost;

                    foreach (decimal roleID in RS_Station_Roles.Roles)
                    {
                        RS_Station_Roles.Role_ID = roleID;

                        RS_Station_Roles.Updated_Date = DateTime.Now;
                        RS_Station_Roles.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Station_Roles.Updated_Host = compName;
                        db.RS_Station_Roles.Add(RS_Station_Roles);
                        db.SaveChanges();
                    }

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceMessages.StationRolesTitle;
                    globalData.messageDetail = ResourceMessages.StationRoles_Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMessages.StationRolesTitle;
                globalData.messageDetail = ResourceMessages.StationRoles_Edit_Falure;
                TempData["globalData"] = globalData;
            }
            catch (Exception exp)
            {

                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMessages.StationRolesTitle;
                globalData.messageDetail = ResourceMessages.StationRoles_Edit_Falure;
                TempData["globalData"] = globalData;
                generalHelper.addControllerException(exp, "UserRolesController", "Edit(post)", ((FDSession)this.Session["FDSession"]).userId);
            }
            var stationObj = from station in db.RS_Stations
                             where station.Station_ID == RS_Station_Roles.Station_ID
                             select new
                             {
                                 Station_ID = station.Station_ID,
                                 Station_Name = station.Station_Name + "(" + station.Station_IP_Address + ")"
                             };
            var roleObj = (from a in db.RS_Roles
                           join b in db.RS_Station_Roles on a.Role_ID equals b.Role_ID
                           where b.Station_ID == RS_Station_Roles.Station_ID
                           select a.Role_ID).ToList();

            ViewBag.Roles = new MultiSelectList(db.RS_Roles, "Role_ID", "Role_Name", roleObj);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Station_Roles.Station_ID);

            return View(RS_Station_Roles);
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

            }
            return null;
        }

        // GET: StationRoles/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Station_Roles RS_Station_Roles = db.RS_Station_Roles.Find(id);
            if (RS_Station_Roles == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.StationRoles;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "StationRoles";
            globalData.actionName = ResourceGlobal.Delete;
            //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
            //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Station_Roles);
        }

        // POST: StationRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_Station_Roles RS_Station_Roles = db.RS_Station_Roles.Find(id);
                db.RS_Station_Roles.Remove(RS_Station_Roles);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceMessages.StationRolesTitle;
                globalData.messageDetail = ResourceMessages.StationRoles_Delete_Success;
            }
            catch (DbUpdateException exp)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMessages.StationRolesTitle;
                globalData.messageDetail = ResourceMessages.StationRoles_Delete_Dependency_Failure;
            }
            catch (Exception exp)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMessages.StationRolesTitle;
                globalData.messageDetail = ResourceMessages.StationRoles_Delete_Failure;
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
