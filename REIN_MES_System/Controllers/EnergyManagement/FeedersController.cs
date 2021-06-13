using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;

namespace ZHB_AD.Controllers.MasterManagement
{
    public class FeedersController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();
        // GET: Feeders
        public ActionResult Index()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                globalData.pageTitle = ResourceModules.Feeder_Config;
                ViewBag.GlobalDataModel = globalData;
                var mM_Feeders = db.MM_Feeders.Include(m => m.MM_MTTUW_Shops);
                return View(mM_Feeders.ToList());
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }

        }

        // GET: Feeders/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Feeders mM_Feeders = db.MM_Feeders.Find(id);
            if (mM_Feeders == null)
            {
                return HttpNotFound();
            }
            return View(mM_Feeders);
        }

        // GET: Feeders/Create
        public ActionResult Create()
        { 
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;

                ViewBag.Plant_ID = plantID;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name");
                globalData.pageTitle = ResourceModules.Feeder_Config;
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Feeder";
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Feeder + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Feeder + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
           
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( MM_Feeders mM_Feeders)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                var host = ((FDSession)this.Session["FDSession"]).userHost;
                int Shop = Convert.ToInt16(mM_Feeders.Shop_ID);
                var Feeder = mM_Feeders.FeederName;
               
                if (ModelState.IsValid)
                {
                                     
                    if (mM_Feeders.IsFeedersExists(plantID,Shop, Feeder,0))
                    { 
                        ModelState.AddModelError("FeederName", ResourceValidation.Exist);
                    }
                    else
                    {

                    mM_Feeders.Inserted_Date = System.DateTime.Now;
                    mM_Feeders.Inserted_Host = host;
                    mM_Feeders.Inserted_User_ID = userID;
                    db.MM_Feeders.Add(mM_Feeders);
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Feeder;
                    globalData.messageDetail = ResourceModules.Feeder + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                   }

                }

                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name", mM_Feeders.Shop_ID);
                return View(mM_Feeders);
            }
            catch
            {
                return RedirectToAction("Index");
            }
           
        }

        // GET: Feeders/Edit/5
        public ActionResult Edit(decimal? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MM_Feeders mM_Feeders = db.MM_Feeders.Find(id);
                if (mM_Feeders == null)
                {
                    return HttpNotFound();
                }
                globalData.pageTitle = ResourceModules.Feeder_Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "Feeder";
                globalData.actionName = ResourceGlobal.Edit;
                globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Feeder + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Feeder + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = mM_Feeders.Plant_ID;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name", mM_Feeders.Shop_ID);
                return View(mM_Feeders);
            }
            catch
            {
                return RedirectToAction("Index");
            }
          
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_Feeders mM_Feeders)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                var host = ((FDSession)this.Session["FDSession"]).userHost;
                int Shop = Convert.ToInt16(mM_Feeders.Shop_ID);
                var Feeder = mM_Feeders.FeederName;
                if (ModelState.IsValid)
                {
                    if (mM_Feeders.IsFeedersExists(plantID, Shop, Feeder, Convert.ToInt32(mM_Feeders.Feeder_ID)))
                    {
                        ModelState.AddModelError("FeederName", ResourceValidation.Exist);
                    }
                    else
                    {
                        mM_Feeders.Updated_Date = System.DateTime.Now;
                        mM_Feeders.Updated_Host = host;
                        mM_Feeders.Updated_User_ID = userID;
                        db.Entry(mM_Feeders).State = EntityState.Modified;
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Feeder;
                        globalData.messageDetail = ResourceModules.Feeder + " " + ResourceMessages.Edit_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name", mM_Feeders.Shop_ID);
                return View(mM_Feeders);
            }
            catch
            {
                return RedirectToAction("Index");
            }
          
        }

        // GET: Feeders/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Feeders mM_Feeders = db.MM_Feeders.Find(id);
            if (mM_Feeders == null)
            {
                return HttpNotFound();
            }
            return View(mM_Feeders);
        }

        // POST: Feeders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
           
            try
            {
                MM_Feeders mM_Feeders = db.MM_Feeders.Find(id);
                db.MM_Feeders.Remove(mM_Feeders);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Feeder;
                globalData.messageDetail = ResourceModules.Feeder + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                generalObj.addControllerException(ex, "Feeders", "delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Feeder;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                //ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Category.Plant_ID);
                return RedirectToAction("Index");

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
    }
}
