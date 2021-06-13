using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class Plant_ConsumeController : Controller
    {
        // GET: Plant_Consume
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();
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
                globalData.pageTitle = ResourceModules.Plant_Consume;

                var Plant = (from u in db.MM_Plant_Consume_Config join
                             p in db.MM_MTTUW_Plants on
                             u.Plant_ID equals p.Plant_ID join
                            f in db.MM_PlantFormula on
                            u.Formula_ID equals f.Formula_ID
                            where  p.Plant_ID == plantID
                            select new
                            {
                                u.Plant_ID,
                                u.Formula_ID,
                                f.Formula_Name,
                                p.Plant_Name
                            }).Distinct().ToList();
                var PlantwiseData = Plant.Select(c => new Metadata_Plant_Config
                {
                    Plant = Convert.ToInt16(c.Plant_ID),
                    Plant_Name = c.Plant_Name,
                    Formula_ID = Convert.ToInt16(c.Formula_ID),
                    Opertion = c.Formula_Name
                }).ToList();

                return View(PlantwiseData);
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
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.GlobalDataModel = globalData;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
            
                ViewBag.Plant_ID = plantID;
                var userObj = from ut in db.UtilityMainFeederMappings join
                              f in db.MM_Feeders on
                              ut.Feeder_ID equals f.Feeder_ID
                              where (ut.Parameter_ID == 1  && ut.Meter_ID ==1)
                              select new
                              {
                                  TagIndex = ut.TagIndex,
                                  FeederName = f.FeederName
                              };
                ViewBag.TagIndexadd = new SelectList(userObj, "TagIndex", "FeederName");


                ViewBag.TagIndexSub = new SelectList(userObj, "TagIndex", "FeederName");


                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s=>s.Plant_ID ==plantID).ToList(), "Shop_ID", "Shop_Name");
                ViewBag.Formula_ID = new SelectList(db.MM_PlantFormula.ToList(),"Formula_ID", "Formula_Name");
                ViewBag.ShopGroup_ID = new SelectList(db.MM_ShopsCategory.Where(s => s.Plant_ID == plantID).ToList(), "ShopsCat_ID", "ShopsCategory_Name");

                globalData.pageTitle = ResourceModules.Plant_Consume;

                TempData["GlobalDataModel"] = globalData;
                //ViewBag.Shop_ID = null;
                //ViewBag.TagIndex = null;
                return View();
            }
            catch
            {
                return RedirectToAction("Index");
            }


        }


        public ActionResult Addconsume(string [] add, string[] sub,string[] Shop, string[] ShopGroup, int formula)
        {
            string validatemsg;
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                string compName = ((FDSession)this.Session["FDSession"]).userHost;

                if (formula == 1)
                {
                    if (add != null)
                    {


                        foreach (var id in add)
                        {
                            MM_Plant_Consume_Config obj = new MM_Plant_Consume_Config();
                            obj.TagIndex = Convert.ToInt32(id);
                            obj.TagBoolean = true;
                            obj.Formula_ID = formula;
                            obj.Plant_ID = plantID;
                            obj.Inserted_User_ID = userID;
                            obj.Inserted_Date = System.DateTime.Now;
                            db.MM_Plant_Consume_Config.Add(obj);
                        }
                    }
                    if (sub != null)
                    {

                        foreach (var id in sub)
                        {
                            MM_Plant_Consume_Config obj = new MM_Plant_Consume_Config();
                            obj.TagIndex = Convert.ToInt32(id);
                            obj.TagBoolean = false;
                            obj.Formula_ID = formula;
                            obj.Plant_ID = plantID;
                            obj.Inserted_User_ID = userID;
                            obj.Inserted_Date = System.DateTime.Now;
                            obj.ShopGroup_ID = null;



                            db.MM_Plant_Consume_Config.Add(obj);
                        }

                    }
                    db.SaveChanges();
                }
                else if(formula == 2)
                {
                    foreach (var id in Shop)
                    {
                        MM_Plant_Consume_Config obj = new MM_Plant_Consume_Config();
                        obj.Shop_ID = Convert.ToInt32(id);
                        obj.Formula_ID = formula;
                        obj.Plant_ID = plantID;
                        obj.Inserted_User_ID = userID;
                        obj.Inserted_Date = System.DateTime.Now;
                        db.MM_Plant_Consume_Config.Add(obj);
                    }
                    db.SaveChanges();
                }
                else if(formula == 3)
                {
                    foreach (var id in ShopGroup)
                    {
                        MM_Plant_Consume_Config obj = new MM_Plant_Consume_Config();
                        obj.ShopGroup_ID = Convert.ToInt32(id);
                        obj.Formula_ID = formula;
                        obj.Plant_ID = plantID;
                        obj.Inserted_User_ID = userID;
                        obj.Inserted_Date = System.DateTime.Now;
                        db.MM_Plant_Consume_Config.Add(obj);
                    }
                    db.SaveChanges();
                }
                validatemsg = "Plant Consumption config added successfully .......!";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {

                validatemsg = "Try agin.....!";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }
           
        }


        public ActionResult CheckOpertion(int formula)
        {   
            var result = (from p in db.MM_Plant_Consume_Config  
                          select (p)).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id, int formula)
        {   
            try
            {

                int userID = ((FDSession)this.Session["FDSession"]).userId;
                globalData.pageTitle = ResourceMainFeederMapping.Title_Add_MainFeeder;
                ViewBag.GlobalDataModel = globalData;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plant_ID = plantID;
                
                var add = db.MM_Plant_Consume_Config.Where(x =>  x.TagBoolean == true && x.Plant_ID == plantID && x.Formula_ID ==1).Select(x => (x.TagIndex)).ToArray();
                var subtarct = db.MM_Plant_Consume_Config.Where(x => x.TagBoolean == false && x.Plant_ID == plantID && x.Formula_ID ==1).Select(x => (x.TagIndex)).ToArray();
                var shop = db.MM_Plant_Consume_Config.Where(x=> x.Plant_ID == plantID && x.Formula_ID == 2).Select(x => (x.Shop_ID)).ToArray();
                var shopgroup = db.MM_Plant_Consume_Config.Where(x=>x.Plant_ID == plantID && x.Formula_ID == 3).Select(x => (x.ShopGroup_ID)).ToArray();


                var Result = (from ut in db.UtilityMainFeederMappings
                              join
                              f in db.MM_Feeders on
                              ut.Feeder_ID equals f.Feeder_ID
                              where (ut.Parameter_ID == 1 || ut.Parameter_ID == 2) && ut.Meter_ID == 1
                              select new
                              {
                                  TagIndex = ut.TagIndex,
                                  FeederName = f.FeederName
                              }).ToList();
                var shoplist = (from u in db.MM_MTTUW_Shops
                                where u.Plant_ID == plantID
                                select new
                                {
                                    Shop_ID = u.Shop_ID,
                                    Shop_Name = u.Shop_Name
                                }).ToList();
                var shopgrouplist = (from u in db.MM_ShopsCategory
                                     where u.Plant_ID ==plantID
                                select new
                                {
                                    ShopsCat_ID = u.ShopsCat_ID,
                                    ShopsCategory_Name = u.ShopsCategory_Name
                                }).ToList();

                ViewBag.TagIndexadd = new MultiSelectList(Result, "TagIndex", "FeederName", add);
                ViewBag.TagIndexSub = new MultiSelectList(Result, "TagIndex", "FeederName", subtarct);
                ViewBag.Shop_ID = new MultiSelectList(shoplist, "Shop_ID", "Shop_Name",shop);
                ViewBag.Formula_ID = new SelectList(db.MM_PlantFormula.ToList(), "Formula_ID", "Formula_Name",formula);
                ViewBag.ShopGroup_ID = new MultiSelectList(shopgrouplist, "ShopsCat_ID", "ShopsCategory_Name",shopgroup);


                ViewBag.Plant_ID = plantID;
                globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

        }

        public ActionResult Editconsume(string[] add, string[] sub, string[] Shop, string[] ShopGroup, int formula)
         {
            string validatemsg;
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                string compName = ((FDSession)this.Session["FDSession"]).userHost;

                var Feeder = db.MM_Plant_Consume_Config.Where(x => x.Plant_ID == plantID && x.Formula_ID == formula).ToList();
                foreach (var item in Feeder)
                {
                    db.MM_Plant_Consume_Config.Remove(item);
                    db.SaveChanges();
                }
                if (formula == 1)
                {
                    if (add != null)
                    {


                        foreach (var id in add)
                        {
                            MM_Plant_Consume_Config obj = new MM_Plant_Consume_Config();
                            obj.TagIndex = Convert.ToInt32(id);
                            obj.TagBoolean = true;
                            obj.Formula_ID = formula;
                            obj.Plant_ID = plantID;
                            obj.Inserted_User_ID = userID;
                            obj.Inserted_Date = System.DateTime.Now;
                            db.MM_Plant_Consume_Config.Add(obj);
                        }
                    }
                    if (sub != null)
                    {

                        foreach (var id in sub)
                        {
                            MM_Plant_Consume_Config obj = new MM_Plant_Consume_Config();
                            obj.TagIndex = Convert.ToInt32(id);
                            obj.TagBoolean = false;
                            obj.Formula_ID = formula;
                            obj.Plant_ID = plantID;
                            obj.Inserted_User_ID = userID;
                            obj.Inserted_Date = System.DateTime.Now;
                            obj.ShopGroup_ID = null;



                            db.MM_Plant_Consume_Config.Add(obj);
                        }

                    }
                    db.SaveChanges();
                }
                else if (formula == 2)
                {
                    foreach (var id in Shop)
                    {
                        MM_Plant_Consume_Config obj = new MM_Plant_Consume_Config();
                        obj.Shop_ID = Convert.ToInt32(id);
                        obj.Formula_ID = formula;
                        obj.Plant_ID = plantID;
                        obj.Inserted_User_ID = userID;
                        obj.Inserted_Date = System.DateTime.Now;
                        db.MM_Plant_Consume_Config.Add(obj);
                    }
                    db.SaveChanges();
                }
                else if (formula == 3)
                {
                    foreach (var id in ShopGroup)
                    {
                        MM_Plant_Consume_Config obj = new MM_Plant_Consume_Config();
                        obj.ShopGroup_ID = Convert.ToInt32(id);
                        obj.Formula_ID = formula;
                        obj.Plant_ID = plantID;
                        obj.Inserted_User_ID = userID;
                        obj.Inserted_Date = System.DateTime.Now;
                        db.MM_Plant_Consume_Config.Add(obj);
                    }
                    db.SaveChanges();
                }

                db.SaveChanges();
                validatemsg = "Plant Consumption config edit successfully .......!";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                validatemsg = "Try agin.....!";
                return Json(new { Message = validatemsg }, JsonRequestBehavior.AllowGet);
            }
           
           

        }

        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int formula)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var Feeder = db.MM_Plant_Consume_Config.Where(x => x.Plant_ID == plantID && x.Formula_ID == formula).ToList();
                foreach (var item in Feeder)
                {
                    db.MM_Plant_Consume_Config.Remove(item);
                    db.SaveChanges();
                }
                //MM_Plant_Consume_Config obj = db.MM_Plant_Consume_Config.Find(id);
                //db.MM_Plant_Consume_Config.Remove(obj);
                //db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.pageTitle = ResourceModules.Plant_Consume;
                globalData.messageDetail = ResourceGlobal.Delete;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Plant_Consumption;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }

        }

    }
}