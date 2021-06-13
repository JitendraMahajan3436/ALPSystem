using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.RoleManagement
{
    public class MenusController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        FDSession fdSession = new FDSession();


        int menuId;
        String menuName;

        RS_Menus menus = new RS_Menus();

        General generalObj = new General();
        // GET: Menus
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Menus;
            //globalData.subTitle = ResourceGlobal.Add;
            globalData.controllerName = "Roles";
            globalData.actionName = ResourceGlobal.Index;
           
            ViewBag.GlobalDataModel = globalData;
            
            return View(db.RS_Menus.ToList());
        }

        // GET: Menus/Details/5
        public ActionResult Details(decimal? id)
        {
            globalData.pageTitle = ResourceModules.Menus;
            //globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Menus";
            globalData.actionName = ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Menus RS_Menus = db.RS_Menus.Find(id);
            if (RS_Menus == null)
            {
                return HttpNotFound();
            }
            return View(RS_Menus);
        }

        // GET: Menus/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.Menus;
            //globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Menus";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            return View();
        }

        // POST: Menus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Menu_ID,LinkName,ActionName,ControllerName,Sort_Order,CSSClass,Documents_Name,Technical_Name,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Is_Transfered,Is_Purgeable,Is_Edited")] RS_Menus RS_Menus)
        {
            if (ModelState.IsValid)
            {
                menuName = RS_Menus.LinkName;


                if (RS_Menus.isMenuExists(menuName, 0))
                {
                    ModelState.AddModelError("LinkName", ResourceMessages.Allready_Exit);
                }
                else
                {
                    RS_Menus.CSSClass = "fa fa-dashboard";
                    RS_Menus.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Menus.Inserted_Date = DateTime.Now;
                    RS_Menus.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Menus.Add(RS_Menus);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.MenusTitle;
                    globalData.messageDetail = ResourceModules.MenusTitle + " " +ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = ResourceModules.Menus;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Menus";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Menus.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Menus.Updated_User_ID);
            return View(RS_Menus);
        }

        // GET: Menus/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Menus RS_Menus = db.RS_Menus.Find(id);
            if (RS_Menus == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceModules.Menus;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Menus";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;
            
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Menus.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Menus.Updated_User_ID);
            return View(RS_Menus);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Menu_ID,LinkName,ActionName,ControllerName,Sort_Order,CSSClass,Documents_Name,Technical_Name,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date,Is_Transfered,Is_Purgeable,Is_Edited")] RS_Menus RS_Menus)
        {
            if (ModelState.IsValid)
            {
                menuId = Convert.ToInt32(RS_Menus.Menu_ID);
                menuName = RS_Menus.LinkName;

                menus = db.RS_Menus.Find(menuId);
                menus.LinkName = menuName;
                menus.Sort_Order = RS_Menus.Sort_Order;
                menus.ControllerName = RS_Menus.ControllerName;
                menus.ActionName = RS_Menus.ActionName;
                menus.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                menus.Updated_Date = DateTime.Now;
                menus.Is_Edited = true;
                db.Entry(menus).State = EntityState.Modified;
                db.SaveChanges();

                RS_Menu_Role mmMenuRole = new RS_Menu_Role();
                var menuRoleId = db.RS_Menu_Role.Where(m => m.Menu_ID == menuId).Select(m => m.Menu_Role_ID).FirstOrDefault();
                if(menuRoleId > 0)
                {
                    mmMenuRole = db.RS_Menu_Role.Find(menuRoleId);
                    mmMenuRole.Sort_Order = menus.Sort_Order;
                    db.Entry(menus).State = EntityState.Modified;
                    db.SaveChanges();
                }
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.MenusTitle;
                globalData.messageDetail = ResourceModules.MenusTitle + " " + ResourceMessages.Edit_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

                // }
            }

            globalData.pageTitle = ResourceModules.Menus;
            //globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Menus";
            globalData.actionName = ResourceGlobal.Edit;
            
            ViewBag.GlobalDataModel = globalData;
            
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Menus.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Menus.Updated_User_ID);
            return View(RS_Menus);
        }

        // GET: Menus/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Menus RS_Menus = db.RS_Menus.Find(id);
            if (RS_Menus == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Menus;
            //globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Menus";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Menus);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                var menurole = db.RS_Menu_Role.Where(x => x.Menu_ID == id).ToList();
                db.RS_Menu_Role.RemoveRange(menurole);

                RS_Menus RS_Menus = db.RS_Menus.Find(id);
                db.RS_Menus.Remove(RS_Menus);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.MenusTitle;
                globalData.messageDetail = ResourceModules.MenusTitle + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
            }
            catch (DbUpdateException ex)
            {
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Menus", "Menu_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                
                globalData.isAlertMessage = true;
                globalData.messageTitle = ResourceModules.MenusTitle;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");

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
