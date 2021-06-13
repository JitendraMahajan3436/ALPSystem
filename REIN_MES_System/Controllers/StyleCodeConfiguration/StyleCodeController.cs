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

namespace REIN_MES_System.Controllers.StyleCodeConfiguration
{
    public class StyleCodeController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalObj = new General();
        // GET: StyleCode
        int plantId = 0;
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Style_Code_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StyleCodeController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Style_Code + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Style_Code + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            return View(db.RS_Style_Code.Where(m => m.Plant_ID == plantId).ToList());
        }

        // GET: StyleCode/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Style_Code RS_Style_Code = db.RS_Style_Code.Find(id);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Style_Code_Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "StyleCodeController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Style_Code + " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Style_Code + " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            if (RS_Style_Code == null)
            {
                return HttpNotFound();
            }
            return View(RS_Style_Code);
        }

        // GET: StyleCode/Create
        public ActionResult Create()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Style_Code_Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "StyleCodeController";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceModules.Style_Code + " " + ResourceGlobal.Create;
            globalData.contentFooter = ResourceModules.Style_Code + " " + ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            return View();
        }

        // POST: StyleCode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Style_Code RS_Style_Code)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (RS_Style_Code.RemoteStyleCode(RS_Style_Code.StyleCode_ID, RS_Style_Code.Style_Code))
                    {
                        RS_Style_Code.Inserted_Date = DateTime.Now;
                        RS_Style_Code.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Style_Code.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                        db.RS_Style_Code.Add(RS_Style_Code);
                        db.SaveChanges();

                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Style_Code;
                        globalData.messageDetail = ResourceModules.Style_Code + " " + ResourceMessages.Create_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Style_Code", ResourceValidation.Exist);
                    }

                }
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                globalData.pageTitle = ResourceModules.Style_Code_Config;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "StyleCodeController";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceModules.Style_Code + " " + ResourceGlobal.Create;
                globalData.contentFooter = ResourceModules.Style_Code + " " + ResourceGlobal.Create;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                return View(RS_Style_Code);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                        generalObj.addControllerException(dbex, "StyleCodeController", "Create(Post):" + validationError.ErrorMessage, ((FDSession)this.Session["FDSession"]).userId);
                    }
                }
                throw raise;
            }

        }

        // GET: StyleCode/Edit/5
        public ActionResult Edit(decimal id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                RS_Style_Code RS_Style_Code = db.RS_Style_Code.Find(id);
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                globalData.pageTitle = ResourceModules.Style_Code_Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "StyleCodeController";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceModules.Order_Type + " " + ResourceGlobal.Edit;
                globalData.contentFooter = ResourceModules.Order_Type + " " + ResourceGlobal.Edit;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                if (RS_Style_Code == null)
                {
                    return HttpNotFound();
                }
                return View(RS_Style_Code);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                        generalObj.addControllerException(dbex, "StyleCodeController", "Create(Post):" + validationError.ErrorMessage, ((FDSession)this.Session["FDSession"]).userId);
                    }
                }
                throw raise;
            }

        }

        // POST: StyleCode/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Style_Code RS_Style_Code)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RS_Style_Code obj_RS_Style_Code = db.RS_Style_Code.Find(RS_Style_Code.StyleCode_ID);
                    obj_RS_Style_Code.Style_Code = RS_Style_Code.Style_Code;
                    obj_RS_Style_Code.Style_Code_Description = RS_Style_Code.Style_Code_Description;
                    obj_RS_Style_Code.Style_Code_Number = RS_Style_Code.Style_Code_Number;
                    obj_RS_Style_Code.Updated_Date = DateTime.Now;
                    obj_RS_Style_Code.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    if (RS_Style_Code.RemoteStyleCode(obj_RS_Style_Code.StyleCode_ID, obj_RS_Style_Code.Style_Code))
                    {
                        db.Entry(obj_RS_Style_Code).State = EntityState.Modified;
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Style_Code;
                        globalData.messageDetail = ResourceModules.Style_Code + " " + ResourceMessages.Edit_Success;
                        TempData["globalData"] = globalData;
                        ////updating style code in model master table
                        var biwobj = db.RS_BIW_Part_Master.Where(m => m.Plant_ID == obj_RS_Style_Code.Plant_ID && m.StyleCode_ID == obj_RS_Style_Code.StyleCode_ID).ToList();
                        if (biwobj != null)
                        {
                            foreach(var biwitem in biwobj)
                            {
                                var modelObj = db.RS_Model_Master.Where(m => m.Plant_ID == biwitem.Plant_ID && m.Shop_ID == biwitem.Shop_ID && m.BIW_Part_No.ToLower() == biwitem.Variant_Code.Trim().ToLower()).ToList();
                                if (modelObj != null)
                                {
                                    foreach (var model in modelObj)
                                    {
                                        model.Style_Code = obj_RS_Style_Code.Style_Code; //vMStyleCodeMasterConfig.Style_Code;
                                        model.Is_Edited = true;
                                        model.Updated_Date = DateTime.Now;
                                        model.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                                        db.Entry(model).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }

                        ////

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Style_Code", ResourceValidation.Exist);
                    }

                }
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                plantId = ((FDSession)this.Session["FDSession"]).plantId;

                globalData.pageTitle = ResourceModules.Style_Code_Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "OrderTypeController";
                globalData.actionName = ResourceGlobal.Edit;
                globalData.contentTitle = ResourceModules.Style_Code + " " + ResourceGlobal.Edit;
                globalData.contentFooter = ResourceModules.Style_Code + " " + ResourceGlobal.Edit;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                return View(RS_Style_Code);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                        generalObj.addControllerException(dbex, "StyleCodeController", "Create(Post):" + validationError.ErrorMessage, ((FDSession)this.Session["FDSession"]).userId);
                    }
                }
                throw raise;
            }

        }

        // GET: StyleCode/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Style_Code RS_Style_Code = db.RS_Style_Code.Find(id);
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceModules.Style_Code_Config;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "OrderTypeController";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceModules.Style_Code + " " + ResourceGlobal.Delete;
            globalData.contentFooter = ResourceModules.Style_Code + " " + ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            if (RS_Style_Code == null)
            {
                return HttpNotFound();
            }
            return View(RS_Style_Code);
        }

        // POST: StyleCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_Style_Code RS_Style_Code = db.RS_Style_Code.Find(id);
                db.RS_Style_Code.Remove(RS_Style_Code);
                db.SaveChanges();

            }
            catch (DbUpdateException ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Style_Code;
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
