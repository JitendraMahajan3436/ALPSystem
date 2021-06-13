using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System.Net;

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class Common_Feeder_Consumption_FactorController : Controller
    {
        // GET: Common_Feeder_Consumption_Factor
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();
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
                globalData.pageTitle = ResourceCommon_Feeder_Consumption_Factor.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                var Factor = (from u in db.MM_Comman_Feeder_Consumption_Factor                            
                              select (u)).ToList();

              return View(Factor);
            }
            catch
            {
                return RedirectToAction("Index", "user");
            }

        }
        public ActionResult Create()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                ViewBag.Plant_ID = plantID;
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                ViewBag.Category_ID = new SelectList(db.MM_Category.Where(s => s.Plant_ID == plantID), "Category_Id", "Category_Name");
                globalData.pageTitle = ResourceCommon_Feeder_Consumption_Factor.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                return View();
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        //[HttpPost]
        //public ActionResult Create(MM_Comman_Feeder_Consumption_Factor obj)
        //{
        //    try
        //    {
        //        int plantID = ((FDSession)this.Session["FDSession"]).plantId;
        //        int userID = ((FDSession)this.Session["FDSession"]).userId;
        //        if (ModelState.IsValid)
        //        {
        //            var feeder = obj.Factor;
        //            int shopId = Convert.ToInt16(obj.Shop_Id);

        //            int TagIndex = Convert.ToInt16(obj.TagIndex);

        //            //if (obj.Shop_Id(TagIndex, plantID, 0))
        //            //{
        //            //    ModelState.AddModelError("TagIndex", ResourceValidation.Exist);
        //            //}
        //            //else if (utilityMainFeederMapping.IsFeederExists(feeder, plantID, shopId, CatergoryId, 0))
        //            //{
        //            //    ModelState.AddModelError("FeederName", ResourceValidation.Exist);
        //            //}
        //        }
        //        else
        //        {
        //            //OBJ.Plant_ID = utilityMainFeederMapping.Plant_ID;
        //            //OBJ.Shop_ID = utilityMainFeederMapping.Shop_ID;
        //            //OBJ.FeederName = utilityMainFeederMapping.FeederName;
        //            //OBJ.Unit = utilityMainFeederMapping.Unit;
        //            //OBJ.TagIndex = utilityMainFeederMapping.TagIndex;
        //            obj.Inserted_Date = DateTime.Now;
        //            obj.Inserted_Host = Request.UserHostName;
        //            obj.Inserted_User_ID = userID;
                   

        //            db.MM_Comman_Feeder_Consumption_Factor.Add(obj);
        //            db.SaveChanges();
        //            globalData.isSuccessMessage = true;
        //            globalData.messageTitle = ResourceCommon_Feeder_Consumption_Factor.Consumption_Factor_Add_Success;
        //            globalData.messageDetail = ResourceCommon_Feeder_Consumption_Factor.Consumption_Factor_Add_Success;
        //            TempData["globalData"] = globalData;
        //            return RedirectToAction("Index");
        //        }
                

        //        //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
        //        //                                   join
        //        //                                   u in db.MM_Plant_User on
        //        //                                   s.Plant_ID equals u.Plant_ID
        //        //                                   where u.User_ID == userID
        //        //                                   select (s)).ToList(), "Plant_ID", "Plant_Name");
        //        ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
        //        ViewBag.Category_ID = new SelectList(db.MM_Category.Where(s => s.Plant_ID == plantID), "Category_Id", "Category_Name");
               
        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        generalObj.addControllerException(ex, "Comman_Feeder_Consumption_Factor", "Create", ((FDSession)this.Session["FDSession"]).userId);
        //        globalData.isErrorMessage = true;
        //        globalData.messageTitle = ResourceCommon_Feeder_Consumption_Factor.Consumption_Factor_Add_Success;
        //        globalData.messageDetail = ResourceCommon_Feeder_Consumption_Factor.Consumption_Factor_Add_Success;
        //        TempData["globalData"] = globalData;
        //        return RedirectToAction("Index");

        //    }


           
        //}
    }
}