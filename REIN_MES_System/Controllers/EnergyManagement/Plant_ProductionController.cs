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
    public class Plant_ProductionController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();

        // GET: Plant_Production
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

                ViewBag.GlobalDataModel = globalData;
                globalData.pageTitle = ResourceModules.Plant_Production;
                var mM_Plant_Production = db.MM_Plant_Production.Include(m => m.MM_MTTUW_Shops);
                return View(mM_Plant_Production.ToList());
    
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
           
        }

   

        public ActionResult Create()
        {
            try
            {
                ViewBag.GlobalDataModel = globalData;
                int userID = ((FDSession)this.Session["FDSession"]).userId;

                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Plant_Production + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Plant_Production + " " + ResourceGlobal.Form;
                ViewBag.Shop = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name");
                ViewBag.Plant_ID = plantID;
                globalData.pageTitle = ResourceModules.Plant_Production;
                return View();
            }
            catch
            {
                return RedirectToAction("Index");
            }
    
       
        }

        // POST: Plant_Production/Create
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( MM_Plant_Production mM_Plant_Production)
        {
            try
            {
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                string compName = ((FDSession)this.Session["FDSession"]).userHost;
                if (ModelState.IsValid)
                {
                    //if (mM_Plant_Production.IsShopExists(plantID, ShopId, consider, 0))
                    //{
                    //    ModelState.AddModelError("Shop_ID", ResourceShopwise_PowerIndexMapping.Shop_Error_Shop_Name);
                    //}

                    //else
                    //{
                        foreach (var item in mM_Plant_Production.Shop)
                        {
                            MM_Plant_Production obj = new MM_Plant_Production();


                            obj.Shop_ID = item;
                            obj.Plant_ID = plantID;
                            obj.Inserted_Date = System.DateTime.Now;
                            obj.Inserted_User_ID = userID;
                            obj.Inserted_Host = compName;
                            db.MM_Plant_Production.Add(obj);
                            db.SaveChanges();
                    }
                     
                        globalData.isSuccessMessage = true;
                        globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                        globalData.messageDetail = ResourceShopwise_PowerIndexMapping.Sucess_Add;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                    //}
                }
                return View();
            }
            catch(Exception ex)
            {

                return RedirectToAction("Index");
            }
           
        }

        // GET: Plant_Production/Edit/5
        public ActionResult Edit(decimal? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //MM_Plant_Production mM_Plant_Production = db.MM_Plant_Production.Find(id);
            //if (mM_Plant_Production == null)
            //{
            //    return HttpNotFound();
            //}
            int userID = ((FDSession)this.Session["FDSession"]).userId;
            globalData.pageTitle = ResourceModules.Plant_Production;
            ViewBag.GlobalDataModel = globalData;
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = plantID;
            var abc = db.MM_Plant_Production.Where(x=> x.Plant_ID == plantID).Select(x => (x.Shop_ID)).ToArray();
            ViewBag.Shop = new MultiSelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name", abc);
            return View();
        }

        // POST: Plant_Production/Edit/5
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( MM_Plant_Production mM_Plant_Production)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (ModelState.IsValid)
                {
                    DateTime today = DateTime.Now;
                    decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                    string compName = ((FDSession)this.Session["FDSession"]).userHost;

                    var Shop = db.MM_Plant_Production.Where(x => x.Plant_ID == plantID).ToList();
                    foreach (var item in Shop)
                    {
                        db.MM_Plant_Production.Remove(item);
                        db.SaveChanges();
                    }



                    foreach (decimal item in mM_Plant_Production.Shop)
                    {
                        MM_Plant_Production obj = new MM_Plant_Production();


                        obj.Shop_ID = item;
                        obj.Plant_ID = plantID;
                        obj.Inserted_Date = System.DateTime.Now;
                        obj.Inserted_User_ID = insertedUserID;
                        obj.Inserted_Host = compName;
                        db.MM_Plant_Production.Add(obj);

                    }
                    db.SaveChanges();
                    globalData.isSuccessMessage = true;
                    globalData.pageTitle = ResourceModules.Plant_Production;
                    globalData.messageDetail = ResourceGlobal.Edit;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                ViewBag.Shop = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name", mM_Plant_Production.Shop);
          
                return View(mM_Plant_Production);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

       
        

      
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_Plant_Production mM_Plant_Production = db.MM_Plant_Production.Find(id);
            db.MM_Plant_Production.Remove(mM_Plant_Production);
            db.SaveChanges();           
            globalData.isSuccessMessage = true;
            globalData.pageTitle = ResourceModules.Plant_Production;
            globalData.messageDetail = ResourceGlobal.Delete;
            TempData["globalData"] = globalData;
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
