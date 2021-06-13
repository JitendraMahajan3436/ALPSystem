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

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class MM_Category_MasterController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();
     
        // GET: MM_Category_Master
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
                //Session["PageTitle"] = "Main Feeder (Meter) Configuration";
                // globalData.pageTitle = ResourceMainFeederMapping.MainFeeder_List;

                //globalData.controllerName = "StationRoles";
                //globalData.actionName = ResourceGlobal.Details;
                //globalData.contentTitle = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
                //globalData.contentFooter = ResourceUserRoles.StationRoles_Title_Details_StationRoles;
                ViewBag.GlobalDataModel = globalData;
                globalData.pageTitle = ResourceMasterCategory.pageTitle;              
                return View((from s in db.MM_Category
                          
                            select (s)).ToList());

            }
            catch
            {
                return RedirectToAction("Index", "user");
            }
            
        }

        // GET: MM_Category_Master/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Category mM_Category = db.MM_Category.Find(id);
            if (mM_Category == null)
            {
                return HttpNotFound();
            }
            return View(mM_Category);
        }

        // GET: MM_Category_Master/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.GlobalDataModel = globalData;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
  
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceMasterCategory.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                //ViewBag.Plant_ID = plantID;
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name");


                return View();
            }
            catch
            {
                return RedirectToAction("Index");
            }
            //globalData.pageTitle = ResourceMainFeederMapping.Title_Add_MainFeeder;
            
        }

        // POST: MM_Category_Master/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_Category mM_Category)
        {
            try
            {
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;

                if (ModelState.IsValid)
                {

                    var Catergory = mM_Category.Category_Name;
                   

                    if (mM_Category.IsCategoryExists(Catergory, plantID, 0))
                    {
                        ModelState.AddModelError("Category_Name", ResourceValidation.Exist);
                    }
                    else
                    {

                        mM_Category.Inserted_Date = DateTime.Now;
                        mM_Category.Inserted_Host = Request.UserHostName;
                        mM_Category.Inserted_User_ID = userID;
                        mM_Category.Plant_ID = plantID;
                        db.MM_Category.Add(mM_Category);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceMasterCategory.pageTitle;
                        globalData.messageDetail = ResourceMasterCategory.MM_Category_Success_MM_Category_Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name");
                return View(mM_Category);
            }
            catch
            {

                return RedirectToAction("Index");
            }
            
        }

        // GET: MM_Category_Master/Edit/5
        public ActionResult Edit(decimal? id)
        {
            try
            {
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
              
                MM_Category mM_Category = db.MM_Category.Find(id);
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name", mM_Category.Plant_ID);
                globalData.pageTitle = ResourceMasterCategory.pageTitle;
                ViewBag.GlobalDataModel = globalData;

                if (mM_Category == null)
                {
                    return HttpNotFound();
                }
                return View(mM_Category);
            }
            catch
            {
                return RedirectToAction("Index");
            }
           
        }

        // POST: MM_Category_Master/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_Category mM_Category)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                //bool isValid = true;
                //if (mM_Category.IsCategoryExists(mM_Category.Category_Name, Convert.ToDecimal(mM_Category.Plant_ID)))
                //{
                //    ModelState.AddModelError("Category_Name", ResourceValidation.Exist);
                //    isValid = false;
                //}
                if (ModelState.IsValid)
                {
                    var Catergory = mM_Category.Category_Name;
                    //int PlantId = Convert.ToInt32(mM_Category.Plant_ID);
                    int CategoryId = Convert.ToInt32(mM_Category.Category_Id);
                    if(mM_Category.IsCategoryExists(Catergory, plantID, CategoryId))
                    {
                        ModelState.AddModelError("Category_Name", ResourceValidation.Exist);
                    }
                    else
                    {

                  
                    mM_Category.Updated_Host = Request.UserHostName;
                    mM_Category.Updated_Date = DateTime.Now;
                        mM_Category.Plant_ID = plantID;
                    mM_Category.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.Entry(mM_Category).State = EntityState.Modified;
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceMasterCategory.pageTitle;
                    globalData.messageDetail = ResourceMasterCategory.MM_Category_Success_MM_Category_edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                 }
                }
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name",mM_Category.Plant_ID);
                return View(mM_Category);
            }
            catch(Exception ex)
            {
                generalObj.addControllerException(ex, "MM_Category_Master", "Edit", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMasterCategory.pageTitle;
                globalData.messageDetail = ResourceMasterCategory.MM_Category_Edit_DbUpdateException;
                TempData["globalData"] = globalData;                
                return View(mM_Category);
               
            }
          
        }

        // GET: MM_Category_Master/Delete/5
        public ActionResult Delete(decimal id)
        {
            try
            {

            }
            catch
            {

            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Category mM_Category = db.MM_Category.Find(id);
            if (mM_Category == null)
            {
                return HttpNotFound();
            }
            return View(mM_Category);
        }

        // POST: MM_Category_Master/Delete/5
        
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            try
            {
                MM_Category mM_Category = db.MM_Category.Find(id);
                db.MM_Category.Remove(mM_Category);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceMasterCategory.pageTitle;
                globalData.messageDetail = ResourceMasterCategory.MM_Category_Success_MM_Category_delete_Success;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                generalObj.addControllerException(ex, "MM_Category_Master", "delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMasterCategory.pageTitle;
                globalData.messageDetail = ResourceMasterCategory.MM_Category_Delete_Dependency_Failure;
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
