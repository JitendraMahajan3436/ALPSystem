//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using ZHB_AD.App_LocalResources;
//using ZHB_AD.Helper;
//using ZHB_AD.Models;
//using ZHB_AD.Controllers.BaseManagement;

//namespace ZHB_AD.Controllers.ZHB_AD
//{
//    public class BusinessController : BaseController
//    {
//        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
//        GlobalData globalData = new GlobalData();
//        FDSession adSession = new FDSession();
//        General generalObj = new General();
//        // GET: Business
//        public ActionResult Index()
//        {
//            try
//            {
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                int userID = ((FDSession)this.Session["FDSession"]).userId;
//                if (TempData["globalData"] != null)
//                {
//                    globalData = (GlobalData)TempData["globalData"];
//                }

//                ViewBag.GlobalDataModel = globalData;
//                globalData.pageTitle = App_LocalResources.ResourceModules.Business_Config;
//                return View(db.MM_Business.ToList());
//            }
//            catch (Exception ex)
//            {
//                return RedirectToAction("Index", "user");
//            }
//        }


//        // GET: Business/Create
//        public ActionResult Create()
//        {
//            try
//            {
//                ViewBag.GlobalDataModel = globalData;
//                int userID = ((FDSession)this.Session["FDSession"]).userId;

//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                globalData.actionName = ResourceGlobal.Create;
//                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Business + " " + ResourceGlobal.Form;
//                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Business + " " + ResourceGlobal.Form;
//                globalData.pageTitle = ResourceModules.Business_Config;
//                return View();
//            }
//            catch
//            {
//                return RedirectToAction("Index");
//            }
//        }

//        // POST: Business/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(MM_Business mM_Business)
//        {
//            try
//            {
//                int userID = ((FDSession)this.Session["FDSession"]).userId;
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;

//                if (ModelState.IsValid)
//                {

//                    var Business = mM_Business.Business_Name;


//                    if (mM_Business.IsBusinessExists(Business, plantID, 0))
//                    {
//                        ModelState.AddModelError("Business_Name", ResourceValidation.Exist);
//                    }
//                    else
//                    {

//                        mM_Business.Inserted_Date = DateTime.Now;
//                        mM_Business.Inserted_Host = Request.UserHostName;
//                        mM_Business.Inserted_User_ID = userID;
//                        mM_Business.Plant_ID = plantID;
//                        db.MM_Business.Add(mM_Business);
//                        db.SaveChanges();
//                        globalData.isSuccessMessage = true;
//                        globalData.messageTitle = ResourceModules.Area;
//                        globalData.messageDetail = ResourceModules.Area + " " + ResourceMessages.Add_Success;
//                        TempData["globalData"] = globalData;
//                        return RedirectToAction("Index");
//                    }
//                }

//                return View(mM_Business);
//            }
//            catch
//            {

//                return RedirectToAction("Index");
//            }
//        }

//        // GET: Business/Edit/5
//        public ActionResult Edit(decimal? id)
//        {
//            try
//            {
//                int userID = ((FDSession)this.Session["FDSession"]).userId;
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                if (id == null)
//                {
//                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//                }

//                MM_Business business = db.MM_Business.Find(id);

//                globalData.pageTitle = ResourceModules.Business_Config;
//                globalData.subTitle = ResourceGlobal.Edit;
//                globalData.controllerName = "Business";
//                globalData.actionName = ResourceGlobal.Edit;
//                globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Area + " " + ResourceGlobal.Form;
//                globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Area + " " + ResourceGlobal.Form;
//                ViewBag.GlobalDataModel = globalData;

//                if (business == null)
//                {
//                    return HttpNotFound();
//                }
//                return View(business);
//            }
//            catch
//            {
//                return RedirectToAction("Index");
//            }
//        }

//        // POST: Business/Edit/5

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(MM_Business mM_Business)
//        {
//            try
//            {
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                int userID = ((FDSession)this.Session["FDSession"]).userId;

//                if (ModelState.IsValid)
//                {
//                    var business = mM_Business.Business_Name;
//                    //int PlantId = Convert.ToInt32(mM_Category.Plant_ID);
//                    int busniessId = Convert.ToInt32(mM_Business.Business_Id);
//                    if (mM_Business.IsBusinessExists(business, plantID, busniessId))
//                    {
//                        ModelState.AddModelError("Business_Name", ResourceValidation.Exist);
//                    }
//                    else
//                    {


//                        mM_Business.Updated_Host = Request.UserHostName;
//                        mM_Business.Updated_Date = DateTime.Now;
//                        mM_Business.Plant_ID = plantID;
//                        mM_Business.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                        db.Entry(mM_Business).State = EntityState.Modified;
//                        db.SaveChanges();
//                        globalData.isSuccessMessage = true;
//                        globalData.messageTitle = ResourceModules.Business;
//                        globalData.messageDetail = ResourceModules.Business + " " + ResourceMessages.Edit_Success;
//                        TempData["globalData"] = globalData;
//                        return RedirectToAction("Index");
//                    }
//                }

//                return View(mM_Business);
//            }
//            catch (Exception ex)
//            {
//                generalObj.addControllerException(ex, "Business", "Edit", ((FDSession)this.Session["FDSession"]).userId);
//                globalData.isErrorMessage = true;
//                globalData.messageTitle = ResourceMasterCategory.pageTitle;
//                TempData["globalData"] = globalData;
//                return View(mM_Business);

//            }
//        }

//        // GET: Business/Delete/5
//        public ActionResult Delete(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_Business mM_Business = db.MM_Business.Find(id);
//            if (mM_Business == null)
//            {
//                return HttpNotFound();
//            }
//            return View(mM_Business);
//        }

       
      
//        public ActionResult DeleteConfirmed(decimal id)
//        {
//            MM_Business mM_Business = db.MM_Business.Find(id);
//            db.MM_Business.Remove(mM_Business);
//            db.SaveChanges();
//            globalData.isSuccessMessage = true;
//            globalData.messageTitle = ResourceModules.Business;
//            globalData.messageDetail = ResourceModules.Business+ " " + ResourceMessages.Delete_Success;
//            TempData["globalData"] = globalData;
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
