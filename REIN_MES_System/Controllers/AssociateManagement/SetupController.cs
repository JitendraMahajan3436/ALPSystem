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
    public class SetupController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalObj = new General();

        String setupName = "";
        int setupId = 0;
        // GET: Setup
        public ActionResult Index()
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = "Setup Configuration";
                globalData.controllerName = "Setup";
                ViewBag.GlobalDataModel = globalData;

                var RS_Setups = db.RS_Setup;
                return View(RS_Setups.ToList());
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Setup";
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return View();
            }
        }

        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Setup RS_Setup = db.RS_Setup.Find(id);
            if (RS_Setup == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = "View Setup";
            globalData.controllerName = "Setup";
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Setup);
        }

        public ActionResult Create()
        {
            globalData.pageTitle = "Create Setup";
            globalData.controllerName = "Setup";
            ViewBag.GlobalDataModel = globalData;

            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = plantID;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Setup RS_Setup)
        {
            if (ModelState.IsValid)
            {
                setupName = RS_Setup.Setup_Name;
                int lineId = Convert.ToInt32(RS_Setup.Line_ID);
                int shopId = Convert.ToInt32(RS_Setup.Shop_ID);
                if (RS_Setup.isSetupExists(setupName,shopId,lineId, 0))
                {
                    ModelState.AddModelError("Setup_Name", ResourceValidation.Exist);
                }
                else
                {
                    RS_Setup.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Setup.Inserted_Date = DateTime.Now;
                    RS_Setup.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Setup.Add(RS_Setup);
                    db.SaveChanges();
                    
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Setup";
                    globalData.messageDetail = "Setup" + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = "Setup Configuration";
            globalData.controllerName = "Setup";
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Setup.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == RS_Setup.Plant_ID), "Shop_ID", "Shop_Name", RS_Setup.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_Setup.Shop_ID), "Line_ID", "Line_Name", RS_Setup.Line_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Setup.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Setup.Updated_User_ID);
            return View(RS_Setup);
        }

        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Setup RS_Setup = db.RS_Setup.Find(id);
            if (RS_Setup == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = "Edit Setup";
            globalData.controllerName = "Setup";
           
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Setup.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == RS_Setup.Plant_ID), "Shop_ID", "Shop_Name", RS_Setup.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_Setup.Shop_ID), "Line_ID", "Line_Name", RS_Setup.Line_ID);

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Setup.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Setup.Updated_User_ID);
            return View(RS_Setup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_Setup RS_Setup)
        {
            if (ModelState.IsValid)
            {
                setupName = RS_Setup.Setup_Name;
                setupId = Convert.ToInt32(RS_Setup.Setup_ID);
                int lineId = Convert.ToInt32(RS_Setup.Line_ID);
                int shopId = Convert.ToInt32(RS_Setup.Shop_ID);
                if (RS_Setup.isSetupExists(setupName, shopId,lineId, setupId))
                {
                    ModelState.AddModelError("Setup_Name", ResourceValidation.Exist);
                }
                else
                {
                    RS_Setup obj = new RS_Setup();
                    obj = db.RS_Setup.Find(setupId);
                    obj.Setup_Name = RS_Setup.Setup_Name;
                    obj.Plant_ID = RS_Setup.Plant_ID;
                    obj.Shop_ID = RS_Setup.Shop_ID;
                    obj.Line_ID = RS_Setup.Line_ID;
                    obj.Setup_Description = RS_Setup.Setup_Description;
                    obj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    obj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    obj.Updated_Date = DateTime.Now;
                    obj.Is_Edited = true;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = "Setup";
                    globalData.messageDetail = "Setup" + " " + ResourceMessages.Edit_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("Index");
                }
            }

            globalData.pageTitle = "Setup Configuration";
            globalData.controllerName = "Setup";
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Setup.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == RS_Setup.Plant_ID), "Shop_ID", "Shop_Name", RS_Setup.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == RS_Setup.Shop_ID), "Line_ID", "Line_Name", RS_Setup.Line_ID);

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Setup.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "User_ID", "User_Name", RS_Setup.Updated_User_ID);
            return View(RS_Setup);
        }

        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Setup RS_Setup = db.RS_Setup.Find(id);
            if (RS_Setup == null)
            {
                return HttpNotFound();
            }

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = "Delete Setup";
            globalData.controllerName = "Setup";
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Setup);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                RS_Setup RS_Setup = db.RS_Setup.Find(id);
                db.RS_Setup.Remove(RS_Setup);
                db.SaveChanges();


               // generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Plants", "Plant_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = "Setup";
                globalData.messageDetail = "Setup" + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                generalObj.addControllerException(ex, "Setup", "DeleteConfirmed", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Setup";
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

        public ActionResult GetSetupByLineID(int lineId)
        {
            try
            {
                var st = from setup in db.RS_Setup
                         where setup.Line_ID == lineId
                         select new
                         {
                             Id = setup.Setup_ID,
                             Value = setup.Setup_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLineByShopID(int shopId)
        {
            try
            {
                var st = from line in db.RS_Lines
                         where line.Shop_ID == shopId
                         orderby line.Line_Name
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name,
                         };
                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}