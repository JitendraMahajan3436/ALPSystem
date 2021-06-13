using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class RequireManPowerConfigurationController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalObj = new General();
        // GET: RequireManPowerConfiguration
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = "ManPower Configuration";
            globalData.controllerName = "RequireManPowerConfiguration";
            ViewBag.GlobalDataModel = globalData;

            var mm_MPR = db.RS_ManPower_Required.Where(m => m.Plant_ID == plantId);
            return View(mm_MPR.ToList());
        }

        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_ManPower_Required obj = db.RS_ManPower_Required.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = "View ManPower Configuration";
            globalData.controllerName = "RequireManPowerConfiguration";
            globalData.actionName = ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;

            return View(obj);
        }

        public ActionResult Create()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            
            globalData.pageTitle = "Create ManPower Configuration";
            globalData.controllerName = "RequireManPowerConfiguration";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Inserted_user_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(m => m.Line_ID == 0), "Setup_ID", "Setup_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == 0), "Line_ID", "Line_Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_ManPower_Required obj)
        {
            var plantId = Convert.ToInt32(obj.Plant_ID);
            var shopId = Convert.ToInt32(obj.Shop_ID);
            var lineId = Convert.ToInt32(obj.Line_ID);
            var setupId = Convert.ToInt32(obj.Setup_ID);
            if (ModelState.IsValid)
            {
               
                if (isRecordExists(plantId, shopId, lineId,setupId, 0))
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "ManPower Configuration";
                    globalData.messageDetail = "Record already exist against these plant,shop,line and setup. Configure another record";
                    TempData["globalData"] = globalData;
                }
                else
                {
                    obj.Inserted_Date = DateTime.Now;
                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                    db.RS_ManPower_Required.Add(obj);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "ManPower Configuration";
                    globalData.messageDetail = "ManPower Configuration "  + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = "ManPower Configuration";
            globalData.controllerName = "RequireManPowerConfiguration";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;

            plantId = Convert.ToInt32(obj.Plant_ID);

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", obj.Shop_ID);
            ViewBag.Inserted_user_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", obj.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", obj.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", obj.Plant_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == obj.Shop_ID), "Line_ID", "Line_Name", obj.Line_ID);
            ViewBag.Setup_ID = new SelectList(db.RS_Setup, "Setup_ID", "Setup_Name", obj.Setup_ID);
            return View(obj);
        }

        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_ManPower_Required obj = db.RS_ManPower_Required.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }


            globalData.pageTitle = "Edit ManPower Configuration";
            globalData.controllerName = "RequireManPowerConfiguration";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            var plantId = Convert.ToInt32(obj.Plant_ID);

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", obj.Shop_ID);
            ViewBag.Inserted_user_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", obj.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", obj.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == obj.Shop_ID), "Line_ID", "Line_Name", obj.Line_ID);
            ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(p => p.Line_ID == obj.Line_ID), "Setup_ID", "Setup_Name", obj.Setup_ID);
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_ManPower_Required obj)
        {
            var plantId = Convert.ToInt32(obj.Plant_ID);
            var shopId = Convert.ToInt32(obj.Shop_ID);
            var setupId = Convert.ToInt32(obj.Setup_ID);
            var lineId = Convert.ToInt32(obj.Line_ID);
            var rowId = Convert.ToInt32(obj.MPR_ID);
            if (ModelState.IsValid)
            {
                if (isRecordExists(plantId, shopId, lineId, setupId,rowId))
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "ManPower Configuration";
                    globalData.messageDetail = "Record already exist against these plant,shop,line and setup. Configure another record";
                    TempData["globalData"] = globalData;
                }
                else
                {
                    RS_ManPower_Required obj1 = db.RS_ManPower_Required.Find(obj.MPR_ID);
                    
                    obj1.Plant_ID = obj.Plant_ID;
                    obj1.Shop_ID = obj.Shop_ID;
                    obj1.Line_ID = obj.Line_ID;
                    obj1.Setup_ID = obj.Setup_ID;
                    obj1.Direct_MP_Quantity = obj.Direct_MP_Quantity;
                    obj1.Indirect_MP_Quantity = obj.Indirect_MP_Quantity;
                    obj1.Updated_Date = DateTime.Now;
                    obj1.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    obj1.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    obj1.Is_Edited = true;
                    db.Entry(obj1).State = EntityState.Modified;
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "ManPower Configuration";
                    globalData.messageDetail = "ManPower Configuration " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = "ManPower Configuration";
            globalData.controllerName = "RequireManPowerConfiguration";
            globalData.actionName = ResourceGlobal.Edit;
            ViewBag.GlobalDataModel = globalData;

            //plantId = RS_Stations.Plant_ID;

            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", obj.Shop_ID);
            ViewBag.Inserted_user_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", obj.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", obj.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", obj.Plant_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == obj.Shop_ID), "Line_ID", "Line_Name", obj.Line_ID);
            ViewBag.Setup_ID = new SelectList(db.RS_Setup, "Setup_ID", "Setup_Name", obj.Setup_ID);
            return View(obj);
        }

        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_ManPower_Required obj = db.RS_ManPower_Required.Find(id);
            if (obj == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = "Delete ManPower Configuration";
            globalData.controllerName = "RequireManPowerConfiguration";
            globalData.actionName = ResourceGlobal.Delete;
            ViewBag.GlobalDataModel = globalData;

            return View(obj);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_ManPower_Required obj = db.RS_ManPower_Required.Find(id);
                db.RS_ManPower_Required.Remove(obj);
                db.SaveChanges();

                //generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Stations", "Station_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                globalData.isSuccessMessage = true;
                globalData.messageTitle = "ManPower Configuration";
                globalData.messageDetail = "ManPower Configuration " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "ManPower Configuration";
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", id);
            }


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public bool isRecordExists(int plantId, int shopId, int lineId, int setupId,int rowId)
        {
            try
            {
                IQueryable<RS_ManPower_Required> result;
                //var res = null;
                if (rowId == 0)
                {
                    result = db.RS_ManPower_Required.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Setup_ID == setupId);
                }
                else
                {
                    result = db.RS_ManPower_Required.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Setup_ID == setupId && p.MPR_ID != rowId);
                }

                if (result.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}