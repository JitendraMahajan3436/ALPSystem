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
//    public class AreaController : BaseController
//    {
//        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
//        GlobalData globalData = new GlobalData();
//        FDSession adSession = new FDSession();
//        General generalObj = new General();
//        // GET: Area
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
//                globalData.pageTitle = ResourceModules.Area_Config;
//                return View(db.MM_Area.ToList());
//            }
//            catch(Exception ex)
//            {
//                return RedirectToAction("Index", "user");
//            }
           
//        }



//        // GET: Area/Create
//        public ActionResult Create()
//        {
//            try
//            {
//                ViewBag.GlobalDataModel = globalData;
//                int userID = ((FDSession)this.Session["FDSession"]).userId;

//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                globalData.actionName = ResourceGlobal.Create;
//                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Area + " " + ResourceGlobal.Form;
//                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Area + " " + ResourceGlobal.Form;
//                globalData.pageTitle = ResourceModules.Area_Config;
//                return View();
//            }
//            catch
//            {
//                return RedirectToAction("Index");
//            }
//        }

    
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create( MM_Area mM_Area)
//        {
//             try
//            {
//                int userID = ((FDSession)this.Session["FDSession"]).userId;
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;

//                if (ModelState.IsValid)
//                {

//                    var Area = mM_Area.Area_Name;


//                    if (mM_Area.IsAreaExists(Area, plantID, 0))
//                    {
//                        ModelState.AddModelError("Area_Name", ResourceValidation.Exist);
//                    }
//                    else
//                    {

//                        mM_Area.Inserted_Date = DateTime.Now;
//                        mM_Area.Inserted_Host = Request.UserHostName;
//                        mM_Area.Inserted_User_ID = userID;
//                        mM_Area.Plant_ID = plantID;
//                        db.MM_Area.Add(mM_Area);
//                        db.SaveChanges();
//                        globalData.isSuccessMessage = true;
//                        globalData.messageTitle = ResourceModules.Area;
//                        globalData.messageDetail = ResourceModules.Area + " " + ResourceMessages.Add_Success;
//                        TempData["globalData"] = globalData;
//                        return RedirectToAction("Index");
//                    }
//                }

//                return View(mM_Area);
//            }
//            catch
//            {

//                return RedirectToAction("Index");
//            }
//        }

//        // GET: Area/Edit/5
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

//                MM_Area area = db.MM_Area.Find(id);

//                globalData.pageTitle = ResourceModules.Area_Config;
//                globalData.subTitle = ResourceGlobal.Edit;
//                globalData.controllerName = "Area";
//                globalData.actionName = ResourceGlobal.Edit;
//                globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Area + " " + ResourceGlobal.Form;
//                globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Area + " " + ResourceGlobal.Form;
//                ViewBag.GlobalDataModel = globalData;

//                if (area == null)
//                {
//                    return HttpNotFound();
//                }
//                return View(area);
//            }
//            catch
//            {
//                return RedirectToAction("Index");
//            }
//        }

//        // POST: Area/Edit/5

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(MM_Area mM_Area)
//        {
//            try
//            {
//                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//                int userID = ((FDSession)this.Session["FDSession"]).userId;

//                if (ModelState.IsValid)
//                {
//                    var Area = mM_Area.Area_Name;
//                    //int PlantId = Convert.ToInt32(mM_Category.Plant_ID);
//                    int areaId = Convert.ToInt32(mM_Area.Area_Id);
//                    if (mM_Area.IsAreaExists(Area, plantID, areaId))
//                    {
//                        ModelState.AddModelError("Area_Name", ResourceValidation.Exist);
//                    }
//                    else
//                    {


//                        mM_Area.Updated_Host = Request.UserHostName;
//                        mM_Area.Updated_Date = DateTime.Now;
//                        mM_Area.Plant_ID = plantID;
//                        mM_Area.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                        db.Entry(mM_Area).State = EntityState.Modified;
//                        db.SaveChanges();
//                        globalData.isSuccessMessage = true;
//                        globalData.messageTitle = ResourceModules.Area;
//                        globalData.messageDetail = ResourceModules.Area + " " + ResourceMessages.Edit_Success;
//                        TempData["globalData"] = globalData;
//                        return RedirectToAction("Index");
//                    }
//                }

//                return View(mM_Area);
//            }
//            catch (Exception ex)
//            {
//                generalObj.addControllerException(ex, "Area", "Edit", ((FDSession)this.Session["FDSession"]).userId);
//                globalData.isErrorMessage = true;
//                globalData.messageTitle = ResourceMasterCategory.pageTitle;
//                TempData["globalData"] = globalData;
//                return View(mM_Area);

//            }
//        }

 

//        // POST: Area/Delete/5
       
//        public ActionResult DeleteConfirmed(decimal id)
//        {
//            MM_Area mM_Area = db.MM_Area.Find(id);
//            db.MM_Area.Remove(mM_Area);
//            db.SaveChanges();
//            globalData.isSuccessMessage = true;
//            globalData.messageTitle = ResourceModules.Area;
//            globalData.messageDetail = ResourceModules.Area + " " + ResourceMessages.Delete_Success;
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
