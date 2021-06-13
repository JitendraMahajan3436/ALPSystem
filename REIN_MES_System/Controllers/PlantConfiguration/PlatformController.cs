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
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.PlantConfiguration
{
    public class PlatformController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();

        General generalObj = new General();

        String plantName = "";
        int plantId = 0;

        // GET: Platform
        public ActionResult Index()
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                globalData.pageTitle = ResourceModules.Platform_Config;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "Platform";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.Platform+ " " + ResourceGlobal.Lists;
                globalData.contentFooter = ResourceModules.Platform + " " + ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;
                var RS_OM_Platform = db.RS_OM_Platform.Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Lines).Where(p => p.Plant_ID == plantId);

                return View(RS_OM_Platform.ToList());
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Platform;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return View();
            }
            //return View(db.RS_OM_Platform.ToList());
        }

        // GET: Platform/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Platform RS_OM_Platform = db.RS_OM_Platform.Find(id);
            if (RS_OM_Platform == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Platform_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Platform";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Platform + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Platform + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_OM_Platform);
        }

        // GET: Platform/Create
        public ActionResult Create()
        {
            globalData.pageTitle = ResourceModules.Platform_Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Platform";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Platform + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Platform + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID= new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", plantId);
            ViewBag.LIne_ID= new SelectList(db.RS_Lines, "Line_ID", "Line_Name");

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            return View();
        }

        // POST: Platform/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_OM_Platform RS_OM_Platform)
        {
            if (ModelState.IsValid)
            {
                if (RS_OM_Platform.IsAlreadyExistPlatformName(RS_OM_Platform.Platform_ID, RS_OM_Platform.Shop_ID, RS_OM_Platform.Line_ID, RS_OM_Platform.Platform_Name))
                {
                    RS_OM_Platform.Inserted_Date = DateTime.Now;
                    RS_OM_Platform.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.RS_OM_Platform.Add(RS_OM_Platform);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Platform;
                    globalData.messageDetail = ResourceModules.Platform + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Platform_Name", ResourceValidation.Exist);
                }
            }
            globalData.pageTitle = ResourceModules.Platform_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Platform";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Platform + " " + ResourceGlobal.Create;
            globalData.contentFooter = ResourceModules.Platform + " " + ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;

            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_OM_Platform.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_OM_Platform.Shop_ID);
            ViewBag.LIne_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_OM_Platform.Shop_ID), "Line_ID", "Line_Name", RS_OM_Platform.Line_ID);
            return View(RS_OM_Platform);
        }

        // GET: Platform/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Platform RS_OM_Platform = db.RS_OM_Platform.Find(id);
            if (RS_OM_Platform == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Platform_Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Platform";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceModules.Platform + " " + ResourceGlobal.Edit;
            globalData.contentFooter = ResourceModules.Platform + " " + ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_OM_Platform.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_OM_Platform.Shop_ID);
            ViewBag.LIne_ID = new SelectList(db.RS_Lines.Where(m=>m.Shop_ID== RS_OM_Platform.Shop_ID), "Line_ID", "Line_Name",RS_OM_Platform.Line_ID);
                        return View(RS_OM_Platform);
        }

        // POST: Platform/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_OM_Platform RS_OM_Platform)
        {
            if (ModelState.IsValid)
            {
                RS_OM_Platform objRS_OM_Platform = db.RS_OM_Platform.Find(RS_OM_Platform.Platform_ID);
                if (RS_OM_Platform.IsAlreadyExistPlatformName(RS_OM_Platform.Platform_ID, RS_OM_Platform.Shop_ID, RS_OM_Platform.Line_ID, RS_OM_Platform.Platform_Name))
                {
                    objRS_OM_Platform.Line_ID = RS_OM_Platform.Line_ID;
                    objRS_OM_Platform.Plant_ID = RS_OM_Platform.Plant_ID;
                    objRS_OM_Platform.Platform_Name = RS_OM_Platform.Platform_Name;

                    objRS_OM_Platform.Serial_No_Code = RS_OM_Platform.Serial_No_Code;
                    objRS_OM_Platform.Shop_ID = RS_OM_Platform.Shop_ID;
                    objRS_OM_Platform.Updated_Date = DateTime.Now;
                    objRS_OM_Platform.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                    db.Entry(objRS_OM_Platform).State = EntityState.Modified;
                    db.SaveChanges();
                   

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Platform;
                    globalData.messageDetail = ResourceModules.Platform + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Platform_Name", ResourceValidation.Exist);
                }

                globalData.pageTitle = ResourceModules.Platform_Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "Platform";
                globalData.actionName = ResourceGlobal.Edit;
                globalData.contentTitle = ResourceModules.Platform + " " + ResourceGlobal.Edit;
                globalData.contentFooter = ResourceModules.Platform + " " + ResourceGlobal.Edit;
                ViewBag.GlobalDataModel = globalData;

                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_OM_Platform.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_OM_Platform.Shop_ID);
                ViewBag.LIne_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_OM_Platform.Shop_ID), "Line_ID", "Line_Name", RS_OM_Platform.Line_ID);

            }
            return View(RS_OM_Platform);
        }

        // GET: Platform/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_Platform RS_OM_Platform = db.RS_OM_Platform.Find(id);
            if (RS_OM_Platform == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Platform_Config;
            
            ViewBag.GlobalDataModel = globalData;
            return View(RS_OM_Platform);
        }

        // POST: Platform/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try { 
            RS_OM_Platform RS_OM_Platform = db.RS_OM_Platform.Find(id);
            db.RS_OM_Platform.Remove(RS_OM_Platform);
            db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Platform;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
            }

            return RedirectToAction("Index");
        }
        public ActionResult GetLineID(int Shopid)
        {
            var lineDetail = (from line in db.RS_Lines
                              where line.Shop_ID == Shopid
                              orderby line.Line_Name ascending
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).ToList();
            return Json(lineDetail, JsonRequestBehavior.AllowGet);
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
