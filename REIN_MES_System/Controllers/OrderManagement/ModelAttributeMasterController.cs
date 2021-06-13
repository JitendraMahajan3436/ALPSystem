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

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class ModelAttributeMasterController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalObj = new General();
        // GET: ModelAttributeMaster
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.Model_Attribute;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "ModelAttributeMaster";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_Lists;
            globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_Lists;
            ViewBag.GlobalDataModel = globalData;
            var RS_Model_Attribute_Master = db.RS_Model_Attribute_Master.Include(m => m.RS_Major_Sub_Assembly).Include(m => m.RS_OM_Platform).Include(m => m.RS_Plants);
            return View(RS_Model_Attribute_Master.ToList());
        }

        // GET: ModelAttributeMaster/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Model_Attribute_Master RS_Model_Attribute_Master = db.RS_Model_Attribute_Master.Find(id);
            if (RS_Model_Attribute_Master == null)
            {
                return HttpNotFound();
            }
            globalData.pageTitle = ResourceModules.Model_Attribute;
            globalData.subTitle = ResourceAttributionParameters.AttributionParameters_Title_AttributionParameters_Detail;
            globalData.controllerName = "ModelAttributeMaster";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceAttributionParameters.AttributionParameters_Title_AttributionParameters_Detail;
            globalData.contentFooter = ResourceAttributionParameters.AttributionParameters_Title_AttributionParameters_Detail;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Model_Attribute_Master);
        }

        // GET: ModelAttributeMaster/Create
        public ActionResult Create()
        {
            try
            {
                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceModules.Model_Attribute;
                
                globalData.controllerName = "ModelAttributeMaster";
                globalData.actionName = ResourceGlobal.Create;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name");
                ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name");
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == 0), "Line_ID", "Line_Name");
                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
            
            return View();
        }

        // POST: ModelAttributeMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Model_Attribute_ID,Platform_ID,Sub_Assembly_ID,Attribution,Plant_ID,Shop_ID,Line_ID,Is_Transferred,Is_Purgeable,Is_Edited,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Model_Attribute_Master RS_Model_Attribute_Master)
        {
            try
            {
                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (ModelState.IsValid)
                {
                    var Attribute = RS_Model_Attribute_Master.Attribution;
                    var SubAssyID = Convert.ToDecimal(RS_Model_Attribute_Master.Sub_Assembly_ID);
                    var platformId = RS_Model_Attribute_Master.Platform_ID;
                    var ShopID = Convert.ToDecimal(RS_Model_Attribute_Master.Shop_ID);
                    var LineID = Convert.ToDecimal(RS_Model_Attribute_Master.Line_ID);
                    if (RS_Model_Attribute_Master.IsAttributeExists(Attribute,ShopID,LineID,platformId,0, plantId))
                    {
                        ModelState.AddModelError("Attribution", ResourceValidation.Exist);
                    }
                    else
                    {
                        RS_Model_Attribute_Master.Inserted_Date = DateTime.Now;
                        RS_Model_Attribute_Master.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        RS_Model_Attribute_Master.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.RS_Model_Attribute_Master.Add(RS_Model_Attribute_Master);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Model_Attribute;
                        globalData.messageDetail = ResourceMessages.Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                    
                }
                globalData.pageTitle = ResourceModules.Model_Attribute;
                globalData.controllerName = "ModelAttributeMaster";
                globalData.actionName = ResourceGlobal.Create;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name", RS_Model_Attribute_Master.Sub_Assembly_ID);
                ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name", RS_Model_Attribute_Master.Platform_ID);
                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Model_Attribute_Master.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name",RS_Model_Attribute_Master.Shop_ID);
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == 0), "Line_ID", "Line_Name",RS_Model_Attribute_Master.Line_ID);
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
           
            return View(RS_Model_Attribute_Master);
        }

        // GET: ModelAttributeMaster/Edit/5
        public ActionResult Edit(decimal id)
        {
            RS_Model_Attribute_Master RS_Model_Attribute_Master = db.RS_Model_Attribute_Master.Find(id);
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                
                if (RS_Model_Attribute_Master == null)
                {
                    return HttpNotFound();
                }
                globalData.pageTitle = ResourceModules.Model_Attribute;
                globalData.controllerName = "ModelAttributeMaster";
                globalData.actionName = ResourceGlobal.Create;
                ViewBag.GlobalDataModel = globalData;

                ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name", RS_Model_Attribute_Master.Sub_Assembly_ID);
                ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name", RS_Model_Attribute_Master.Platform_ID);
                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Model_Attribute_Master.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId), "Shop_ID", "Shop_Name",RS_Model_Attribute_Master.Shop_ID);
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_Model_Attribute_Master.Shop_ID), "Line_ID", "Line_Name",RS_Model_Attribute_Master.Line_ID);
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
            return View(RS_Model_Attribute_Master);

        }

        // POST: ModelAttributeMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Model_Attribute_ID,Platform_ID,Sub_Assembly_ID,Attribution,Plant_ID,Shop_ID,Line_ID,Is_Transferred,Is_Purgeable,Is_Edited,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Model_Attribute_Master RS_Model_Attribute_Master)
        {
            try
            {
                RS_Model_Attribute_Master obj = db.RS_Model_Attribute_Master.Find(RS_Model_Attribute_Master.Model_Attribute_ID);
                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (ModelState.IsValid)
                {
                    var Attribute = RS_Model_Attribute_Master.Attribution;
                    var SubAssyID = Convert.ToDecimal(RS_Model_Attribute_Master.Sub_Assembly_ID);
                    var ShopID = Convert.ToDecimal(RS_Model_Attribute_Master.Shop_ID);
                    var LineID = Convert.ToDecimal(RS_Model_Attribute_Master.Line_ID);
                    var platformId = RS_Model_Attribute_Master.Platform_ID;
                    var ID = RS_Model_Attribute_Master.Model_Attribute_ID;
                    if (RS_Model_Attribute_Master.IsAttributeExists(Attribute,ShopID,LineID, platformId, ID, plantId))
                    {
                        ModelState.AddModelError("Attribution", ResourceValidation.Exist);
                    }
                    else
                    {
                        obj.Sub_Assembly_ID = RS_Model_Attribute_Master.Sub_Assembly_ID;
                        obj.Platform_ID = RS_Model_Attribute_Master.Platform_ID;
                        obj.Shop_ID = RS_Model_Attribute_Master.Shop_ID;
                        obj.Line_ID = RS_Model_Attribute_Master.Line_ID;
                        obj.Attribution = RS_Model_Attribute_Master.Attribution;
                        obj.Is_Edited = true;
                        obj.Updated_Date = DateTime.Now;
                        obj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        obj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Model_Attribute;
                        globalData.messageDetail = ResourceModules.Model_Attribute + " " + ResourceMessages.Edit_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceAttributionParameters.AttributionParameters;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            ViewBag.Sub_Assembly_ID = new SelectList(db.RS_Major_Sub_Assembly, "Sub_Assembly_ID", "Sub_Assembly_Name", RS_Model_Attribute_Master.Sub_Assembly_ID);
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform, "Platform_ID", "Platform_Name", RS_Model_Attribute_Master.Platform_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Model_Attribute_Master.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == RS_Model_Attribute_Master.Plant_ID), "Shop_ID", "Shop_Name", RS_Model_Attribute_Master.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_Model_Attribute_Master.Shop_ID), "Line_ID", "Line_Name", RS_Model_Attribute_Master.Line_ID);
            globalData.pageTitle = ResourceModules.Model_Attribute;
            globalData.controllerName = "ModelAttributeMaster";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Model_Attribute_Master);
        }

        // GET: ModelAttributeMaster/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Model_Attribute_Master RS_Model_Attribute_Master = db.RS_Model_Attribute_Master.Find(id);
            if (RS_Model_Attribute_Master == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            globalData.pageTitle = ResourceModules.Model_Attribute;
            globalData.controllerName = "ModelAttributeMaster";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_Model_Attribute_Master);
        }

        // POST: ModelAttributeMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Model_Attribute_Master RS_Model_Attribute_Master = db.RS_Model_Attribute_Master.Find(id);
            try
            {
                
                db.RS_Model_Attribute_Master.Remove(RS_Model_Attribute_Master);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Model_Attribute;
                globalData.messageDetail = ResourceModules.Model_Attribute + " " + ResourceMessages.Delete_Success;

                TempData["globalData"] = globalData;
            }
            catch (DbUpdateException ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Attribution_Parameter;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceAttributionParameters.AttributionParameters;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", RS_Model_Attribute_Master);

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

        public ActionResult GetLineByShopID(int Shop_ID)
        {
            var line = (from s in db.RS_Lines
                        where s.Shop_ID == Shop_ID
                        select new
                        {
                            id = s.Line_ID,
                            value = s.Line_Name
                        }).ToList();

            return Json(line, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlatformByLine(decimal LineID)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var Platform = (from platform in db.RS_OM_Platform
                            join line in db.RS_Lines
                       on platform.Line_ID equals line.Line_ID
                            where platform.Line_ID == LineID && platform.Plant_ID == plantID
                            select new
                            {
                                Platform_ID = platform.Platform_ID,
                                Platform_Name = platform.Platform_Name
                            });

            return Json(Platform, JsonRequestBehavior.AllowGet);
        }
    }
}
