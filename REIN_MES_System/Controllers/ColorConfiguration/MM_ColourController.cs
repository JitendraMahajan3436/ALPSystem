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
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.ColorConfiguration
{
    public class RS_ColourController :  BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
       

        General generalObj = new General();

        
        int plantId = 0;
        // GET: RS_Colour
        public ActionResult Index()
        {
            var plantId =((FDSession)this.Session["FDSession"]).plantId;
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceModules.Color_Config;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "RS_Colour";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.Plant + " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.Plant + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;
                var RS_Colour = db.RS_Colour.Where(c => c.Plant_ID == plantId).Include(m => m.RS_Plants);
                return View(RS_Colour.ToList());
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Plant;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return View();
            }
        }

        // GET: RS_Colour/Details/5
        public ActionResult Details(string id)
        {
            globalData.pageTitle = ResourceModules.Color_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "RS_Colour";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Details + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Details + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //RS_Colour RS_Colour = db.RS_Colour.Find(id);
            RS_Colour RS_Colour = db.RS_Colour.Find(Convert.ToInt32(id));
            if (RS_Colour == null)
            {
                return HttpNotFound();
            }

            return View(RS_Colour);
        }

        // GET: RS_Colour/Create
        public ActionResult Create()
        {
            var plantID = ((FDSession)(this.Session["FDSession"])).plantId;
            globalData.pageTitle = ResourceModules.Color_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "RS_Colour";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p=> p.Plant_ID== plantID) , "Plant_ID", "Plant_Name");
            return View();
        }

        // POST: RS_Colour/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( RS_Colour RS_Colour)
        {
            if (ModelState.IsValid)
            {
                if (RS_Colour.RemoteColorCode(RS_Colour.Colour_ID, RS_Colour.Row_ID))
                {
                    RS_Colour.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Colour.Inserted_Date = DateTime.Now;
                    db.RS_Colour.Add(RS_Colour);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Color_Config;
                    globalData.messageDetail = ResourceModules.Color_Config + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Colour_ID",ResourceValidation.Exist);
                }
            }
            globalData.pageTitle = ResourceModules.Color_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "RS_Colour";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Colour.Plant_ID);
            return View(RS_Colour);
        }

        // GET: RS_Colour/Edit/5
        public ActionResult Edit(string id)
        {
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Colour RS_Colour = db.RS_Colour.Find(Convert.ToInt32(id));
            if (RS_Colour == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Color_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "RS_Colour";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantID), "Plant_ID", "Plant_Name", RS_Colour.Plant_ID);
            return View(RS_Colour);
        }

        // POST: RS_Colour/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Colour RS_Colour)
        {
            if (ModelState.IsValid)
            {
                RS_Colour Obj_RS_Colour = db.RS_Colour.Find(RS_Colour.Row_ID);
                //RS_Colour Obj_RS_Colour = db.RS_Colour.Find(Convert.ToInt32(id));
                if (Obj_RS_Colour.RemoteColorCode(RS_Colour.Colour_ID, RS_Colour.Row_ID))
                {
                    Obj_RS_Colour.Colour_ID = RS_Colour.Colour_ID;
                    Obj_RS_Colour.Active = RS_Colour.Active;
                    Obj_RS_Colour.Colour_Batch = RS_Colour.Colour_Batch;
                    Obj_RS_Colour.Colour_Desc = RS_Colour.Colour_Desc;
                    Obj_RS_Colour.Export_Colour = RS_Colour.Export_Colour;
                    Obj_RS_Colour.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    Obj_RS_Colour.Updated_Date = DateTime.Now;
                    db.Entry(Obj_RS_Colour).State = EntityState.Modified;
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Color_Config;
                    globalData.messageDetail = ResourceModules.Color_Config + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Colour_ID", ResourceValidation.Exist);
                }
            }
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Colour.Plant_ID);
            globalData.pageTitle = ResourceModules.Color_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "RS_Colour";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Colour);
        }

        // GET: RS_Colour/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //RS_Colour RS_Colour = db.RS_Colour.Find(id);
            RS_Colour RS_Colour = db.RS_Colour.Find(Convert.ToInt32(id));
            globalData.pageTitle = ResourceModules.Color_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "RS_Colour";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Delete + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Delete + " " + ResourceModules.Color_Config + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            if (RS_Colour == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            return View(RS_Colour);
        }

        // POST: RS_Colour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try { 
            //RS_Colour RS_Colour = db.RS_Colour.Find(id);
            RS_Colour RS_Colour = db.RS_Colour.Find(Convert.ToInt32(id));
            db.RS_Colour.Remove(RS_Colour);
            db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Colour";
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

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
