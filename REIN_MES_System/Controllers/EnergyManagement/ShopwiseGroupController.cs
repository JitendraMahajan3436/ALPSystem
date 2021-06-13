using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Helper;
using ZHB_AD.App_LocalResources;

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class ShopwiseGroupController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();
        // GET: ShopwiseGroup
        public ActionResult Index()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                int userId = ((FDSession)this.Session["FDSession"]).userId;
                globalData.pageTitle = ShopwiseGroupResource.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                var ShopGroup = (from s in db.MM_ShopsCategory
                            select (s)).ToList();
                return View(ShopGroup);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
            //var mM_ShopsCategory = db.MM_ShopsCategory.Include(m => m.MM_Plants);
            //return View(mM_ShopsCategory.ToList());
        }

        // GET: ShopwiseGroup/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_ShopsCategory mM_ShopsCategory = db.MM_ShopsCategory.Find(id);
            if (mM_ShopsCategory == null)
            {
                return HttpNotFound();
            }
            return View(mM_ShopsCategory);
        }

        // GET: ShopwiseGroup/Create
        public ActionResult Create()
        {
            try
            {
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ShopwiseGroupResource.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = plantID;
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "user");
            }
           
        }

        // POST: ShopwiseGroup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_ShopsCategory mM_ShopsCategory)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                    int userId = ((FDSession)this.Session["FDSession"]).userId;
                    if (mM_ShopsCategory.IsShopGroupExists(mM_ShopsCategory.ShopsCategory_Name, plantID, 0))
                    {
                        ModelState.AddModelError("Shop_Name", ResourceValidation.Exist);
                    }
                    else
                    {

                        mM_ShopsCategory.Inserted_User_ID = userId;
                        mM_ShopsCategory.Plant_ID = plantID;
                        mM_ShopsCategory.Inserted_Date = System.DateTime.Now;
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ShopwiseGroupResource.PageTitle;
                        globalData.messageDetail = ResourceMM_Shops.MM_Shops_Success_MM_Shops_Add_Success;
                        TempData["globalData"] = globalData;
                        db.MM_ShopsCategory.Add(mM_ShopsCategory);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                // TODO: Add insert logic here
                ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants, "Plant_ID", "Plant_Name");
                ViewBag.ShopsCat_ID = new SelectList(db.MM_ShopsCategory, "ShopsCat_ID", "ShopsCategory_Name");
                return View();
            }
            catch (Exception ex)
            {
                generalObj.addControllerException(ex, "ShopwiseGroup", "Create", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ShopwiseGroupResource.PageTitle;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return View();
            }
        }

        // GET: ShopwiseGroup/Edit/5
        public ActionResult Edit(decimal id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MM_ShopsCategory obj = db.MM_ShopsCategory.Find(id);
                if (obj == null)
                {
                    return HttpNotFound();
                }
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plant_ID = plantId;             
                globalData.pageTitle = ShopwiseGroupResource.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                return View(obj);
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        // POST: ShopwiseGroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_ShopsCategory mM_ShopsCategory)
        {
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (ModelState.IsValid)
                {
                    var ShopGroup = mM_ShopsCategory.ShopsCategory_Name;
                    

                    int shopGroupId = Convert.ToInt16(mM_ShopsCategory.ShopsCat_ID);

                    if (mM_ShopsCategory.IsShopGroupExists(ShopGroup, plantId, shopGroupId))
                    {
                        ModelState.AddModelError("ShopsCategory_Name", ResourceValidation.Exist);
                    }
                    else
                    {
                       mM_ShopsCategory.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mM_ShopsCategory.Updated_Date = System.DateTime.Now;
                        //MM_Shops obj = db.MM_Shops.Find(id);
                        //obj.Shop_Name = mm_Shops.Shop_Name;
                        //obj.Shop_SAP = mm_Shops.Shop_SAP;
                        //obj.ShopsCat_ID = mm_Shops.ShopsCat_ID;
                        //obj.Updated_Date = System.DateTime.Now;
                        //obj.Inserted_Host = mm_Shops.Inserted_Host;
                        //obj.Updated_Host = Request.UserHostName;
                        //obj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.Entry(mM_ShopsCategory).State = EntityState.Modified;
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceMM_Shops.pagetitle;
                        globalData.messageDetail = ResourceMM_Shops.MM_Shops_Success_MM_Shops_EDIT_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }
                ViewBag.Plant_ID = plantId;
      
                return View(mM_ShopsCategory);

            }
            catch (Exception e)
            {
                generalObj.addControllerException(e, "Energy_Shop_Master", "Create", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = MasterPlantsResource.pageTitle;
                globalData.messageDetail = MasterPlantsResource.Plant_Title_Plant_Detail;
                TempData["globalData"] = globalData;
                return View(mM_ShopsCategory);
            }
        }

        // GET: ShopwiseGroup/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_ShopsCategory mM_ShopsCategory = db.MM_ShopsCategory.Find(id);
            if (mM_ShopsCategory == null)
            {
                return HttpNotFound();
            }
            try
            {
               
                db.MM_ShopsCategory.Remove(mM_ShopsCategory);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ShopwiseGroupResource.PageTitle;
                globalData.messageDetail = ShopwiseGroupResource.ShopGroup_delete_Msg;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                generalObj.addControllerException(ex, "ShopwiseGroup", "delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ShopwiseGroupResource.PageTitle;
                globalData.messageDetail = ShopwiseGroupResource.ShopGroup__Delete_Dependency_Failure;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
          
          
            //return View(mM_ShopsCategory);
        }

        // POST: ShopwiseGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(decimal id)
        //{
        //    MM_ShopsCategory mM_ShopsCategory = db.MM_ShopsCategory.Find(id);
        //    db.MM_ShopsCategory.Remove(mM_ShopsCategory);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
