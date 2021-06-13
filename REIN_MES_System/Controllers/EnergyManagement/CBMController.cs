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
    public class CBMController : Controller
    {
        private ZHB_AD_MTTUWEntities db = new ZHB_AD_MTTUWEntities();
        //private AD_MTTUWEntities db1 = new  AD_MTTUWEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();

        // GET: CBM
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
                globalData.pageTitle = ResourceModules.CBM_Config;
                return View(db.MM_Area_CBM_Mapping.ToList());
            }
            catch(Exception ex)
            {
                generalObj.addControllerException(ex, "CBM", "Index", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMasterCategory.pageTitle;
                TempData["globalData"] = globalData;
               
                return RedirectToAction("Index", "user");
            }
           
        }

        // GET: CBM/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Area_CBM_Mapping mM_Area_CBM_Mapping = db.MM_Area_CBM_Mapping.Find(id);
            if (mM_Area_CBM_Mapping == null)
            {
                return HttpNotFound();
            }
            return View(mM_Area_CBM_Mapping);
        }

        // GET: CBM/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.GlobalDataModel = globalData;
                int userID = ((FDSession)this.Session["FDSession"]).userId;

                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Energy ==true), "Shop_ID", "Shop_Name");
                ViewBag.Area_ID = new SelectList(db.MM_Area.Where(s => s.Plant_ID == plantID), "Area_ID", "Area_Name");
                globalData.actionName = ResourceGlobal.Create;
                globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.CBM + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.CBM + " " + ResourceGlobal.Form;
                globalData.pageTitle = ResourceModules.CBM_Config;
                return View();
            }
            catch(Exception ex)
            {
                generalObj.addControllerException(ex, "CBM", "Create", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceMasterCategory.pageTitle;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
                
            }
     
        }



        // POST: CBM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_Area_CBM_Mapping mM_Area_CBM_Mapping)
        {
            if (ModelState.IsValid)
            {
                db.MM_Area_CBM_Mapping.Add(mM_Area_CBM_Mapping);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mM_Area_CBM_Mapping);
        }

        // GET: CBM/Edit/5
        public ActionResult Edit(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Area_CBM_Mapping mM_Area_CBM_Mapping = db.MM_Area_CBM_Mapping.Find(id);
            if (mM_Area_CBM_Mapping == null)
            {
                return HttpNotFound();
            }
            return View(mM_Area_CBM_Mapping);
        }

        // POST: CBM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MM_Area_CBM_Mapping mM_Area_CBM_Mapping)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mM_Area_CBM_Mapping).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mM_Area_CBM_Mapping);
        }

    
        public ActionResult ShopwiseMachine(int? Shop_Id)
        {
            var CbmShopId = db.MM_MTTUW_Shops.Where(s => s.Shop_ID == Shop_Id).Select(s => s.CBM_Shop_ID).FirstOrDefault();
            var Result = (from f in db.MM_MT_MTTUW_Machines
                          where f.Shop_ID == CbmShopId
                          select new
                          {
                              f.Machine_ID,
                              f.Machine_Name
                          }).ToList();
                
            return Json(Result);
        }

        public ActionResult MachinewiseCBM(int? Machine_Id)
        {
          
            var Result = (from f in db.MM_MT_Conditional_Based_Maintenance
                          where f.Machine_ID == Machine_Id
                          select new
                          {
                              f.CBM_ID,
                              f.Machine_Parameter
                          }).ToList();

            return Json(Result);
        }
    
        public ActionResult CheckexistCBM(int? CBM)
        {

            var Result = (from p in db.MM_Area_CBM_Mapping
                          where p.CBM_ID == CBM
                          select new
                          {
                              p.CBM_ID
                          }).ToList();


            return Json(Result);
        }

        public ActionResult AddCBM(List<CBMlist> Resultlist)
        {
            string validatemsg;
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                string compName = ((FDSession)this.Session["FDSession"]).userHost;
                foreach (var id in Resultlist)
                {
                    MM_Area_CBM_Mapping obj = new MM_Area_CBM_Mapping();
                    obj.Shop_ID = id.Shop;  
                    obj.Area_ID = id.Area;
                   

                    obj.Machine_ID= id.Machine;
                    obj.CBM_ID = id.CBM;
                    obj.Inserted_Date = System.DateTime.Now;
                 
                    if (id.CBM != 0)
                    {
                        var CBMdata = db.MM_MT_Conditional_Based_Maintenance.Where(s => s.CBM_ID == id.CBM).FirstOrDefault();
                        obj.CBM_Name = CBMdata.Machine_Parameter;
                        obj.UOM = CBMdata.UOM;
                        obj.Plant_ID = plantID;
                        obj.Inserted_Host = compName;
                        obj.Inserted_User_ID = userID;
                        db.MM_Area_CBM_Mapping.Add(obj);
                        db.SaveChanges();
                    }
                }

                validatemsg = "CBM (Machine Parameter) Mapping config is added successfully .......!";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                validatemsg = "Try agin.....!";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult AreawiseCBM(int? Area)
        {
           
            try
            {
               
                var Result = (from f in db.MM_Area_CBM_Mapping
                              where f.Area_ID == Area
                              select new
                              {
                                  f.CBM_ID, 
                                  f.CBM_Name
                              }).ToList();
                return Json(Result);
            }
            catch (Exception ex)
            {
               
                return Json(null);
            }
        }



        // GET: CBM/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Area_CBM_Mapping mM_Area_CBM_Mapping = db.MM_Area_CBM_Mapping.Find(id);
            if (mM_Area_CBM_Mapping == null)
            {
                return HttpNotFound();
            }
            return View(mM_Area_CBM_Mapping);
        }

        // POST: CBM/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_Area_CBM_Mapping mM_Area_CBM_Mapping = db.MM_Area_CBM_Mapping.Find(id);
            db.MM_Area_CBM_Mapping.Remove(mM_Area_CBM_Mapping);
            db.SaveChanges();
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
