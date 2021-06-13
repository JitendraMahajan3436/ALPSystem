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
    public class Shop_Power_Index_MappingController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();
        // GET: Shop_Power_Index_Mapping
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
                globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;

                var Shop = (from u in db.Shop_Index_Config
                            
                            select new
                            {
                                u.Shop_ID,
                                u.Shop_Name
                            }).Distinct().ToList();
                var ShopwiseData = Shop.Select(c => new Metadata_Shop_Config
                {
                    Shop_ID = Convert.ToInt16(c.Shop_ID),
                    Shop_Name = c.Shop_Name,                   
                }).ToList();

                return View(ShopwiseData);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
           
        }

        public ActionResult Edit1(int id,Boolean tag)
        {
            int plantID = ((FDSession)Session["FDSession"]).plantId;
            int userID = ((FDSession)this.Session["FDSession"]).userId;

            var abc = db.Shop_Index_Config.Where(x => x.Shop_ID == id && x.TagBoolean == tag).Select(x => (x.TagIndex)).ToArray();
            List<int> n = new List<int>();
            for (int i = 0; i < abc.Count(); i++)
            {
                int TagIndex = Convert.ToInt16(abc[i]);
                n.Add(TagIndex);
            }
            ViewBag.Feeder = new MultiSelectList(db.UtilityMainFeederMappings.Where(s=>s.Shop_ID==id), "TagIndex", "FeederName", abc);
            //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
            //                                   join
            //                                   u in db.MM_Plant_User on
            //                                   s.Plant_ID equals u.Plant_ID
            //                                   where u.User_ID == userID
            //                                   select (s)).ToList(), "Plant_ID", "Plant_Name", shop_Index_Config.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Shop_ID == id).ToList(), "Shop_ID", "Shop_Name", id);

            //ViewBag.Feeder = new SelectList(db.UtilityMainFeederMappings, "TagIndex", "FeederName", shop_Index_Config.TagIndex);
            ViewBag.ComFeederShopID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name", id);
            ViewBag.TagBoolean = tag;

            return View();
        }
        public ActionResult Index1()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            int userID = ((FDSession)this.Session["FDSession"]).userId;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            ViewBag.GlobalDataModel = globalData;
            globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;


            var Shop = (from u in db.Shop_Index_Config
                       
                        select new
                        {
                            u.Shop_ID,
                            u.TagBoolean,
                            u.Shop_Name
                        }).Distinct().ToList();
            //ViewData["ShopwiseData"] = Shop;
            var ShopwiseData = Shop.Select(c => new ShopWiseConsumption2
            {
               Shop_ID = Convert.ToInt16(c.Shop_ID),
               Shop_Name = c.Shop_Name,
               TagBoolean = c.TagBoolean
            }).ToList();
            return View(ShopwiseData);
        }

        // GET: Shop_Power_Index_Mapping/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop_Index_Config shop_Index_Config = db.Shop_Index_Config.Find(id);
            if (shop_Index_Config == null)
            {
                return HttpNotFound();
            }
            return View(shop_Index_Config);
        }

        // GET: Shop_Power_Index_Mapping/Create
        public ActionResult Create()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.GlobalDataModel = globalData;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userID
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name");
                //  int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                 ViewBag.Plant_ID = plantID;
                var userObj = from ut in db.UtilityMainFeederMappings
                              where ut.Parameter_ID == 1
                              select new
                              {
                                  TagIndex = ut.TagIndex,
                                  FeederName = ut.FeederName + "(" + ut.ParameterDesc + ")"
                              };
                ViewBag.TagIndex = new SelectList(userObj, "TagIndex", "FeederName");
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s=>s.Plant_ID==plantID), "Shop_ID", "Shop_Name");
                ViewBag.ComFeederShopID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");             
                globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
              
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

        public ActionResult ManualMeterdata()
        {
            var ManulMeter = (from u in db.UtilityMainFeederMappings
                              join
                              s in db.MM_MTTUW_Shops on
                              u.Shop_ID equals s.Shop_ID
                              join
                              f in db.MM_Feeders on
                              u.Feeder_ID equals f.Feeder_ID
                              where u.Parameter_ID == 1 && u.Active == true
                              select new
                              {
                                  u.Shop_ID,
                                  u.Feeder_ID,
                                  u.TagIndex,
                                  s.Shop_Name,
                                  f.FeederName,
                              }).ToList();
                        

                             
            //ViewData["FeederwiseData"] = ManulMeter;
            //ViewBag.pieData = JsonConvert.SerializeObject(Performace);
            return Json(ManulMeter,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Shopwiseconfig(int Shop_Id, int ParameterId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var Result = (from U in db.UtilityMainFeederMappings join
                              d in db.MM_Feeders on
                             U.Feeder_ID equals d.Feeder_ID join
                              p in db.MM_Parameter on
                              U.Parameter_ID equals p.Prameter_ID
                             where U.Shop_ID == Shop_Id && U.Plant_ID == plantID &&( U.Parameter_ID ==ParameterId || U.Parameter_ID ==2)
                              select new
                              {
                                  TagIndex= U.TagIndex,
                                  FeederName= d.FeederName + "(" + U.TagIndex + ")" 
                              }).ToList();
                return Json(Result);
            }
            catch
            {
                return RedirectToAction("Index","user");
            }
          

        }


        public ActionResult Shopwisemeter(int Shop_Id, int ParameterId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var Result = (from U in db.UtilityMainFeederMappings
                              join
                                d in db.MM_Feeders on
                               U.Feeder_ID equals d.Feeder_ID join p in db.MM_Parameter  on
                              U.Parameter_ID equals p.Prameter_ID
                              where U.Shop_ID == Shop_Id && U.Plant_ID == plantID && ( U.Parameter_ID == ParameterId || U.Parameter_ID ==2)
                              select new
                              {
                                  TagIndex = d.Feeder_ID,
                                  FeederName = d.FeederName + "(" + p.Prameter_Name +")"
                              }).ToList();
                return Json(Result);
            }
            catch
            {
                return RedirectToAction("Index", "user");
            }


        }
        // POST: Shop_Power_Index_Mapping/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Shop_Index_Config shop_Index_Config)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                DateTime today = DateTime.Now;
                decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                string compName = ((FDSession)this.Session["FDSession"]).userHost;
                var Consume = shop_Index_Config.Consume_Power;
              
                if (ModelState.IsValid)
                {
                    Boolean consider = Convert.ToBoolean(shop_Index_Config.TagBoolean);
                    int ShopId = Convert.ToInt16(shop_Index_Config.Shop_ID);
                    if (shop_Index_Config.IsFeederExists(plantID, ShopId,  0))
                    {
                        ModelState.AddModelError("Shop_ID", ResourceShopwise_PowerIndexMapping.Shop_Error_Shop_Name);
                    }
                    else
                    {

                 

                    foreach (decimal item in shop_Index_Config.IncomeraddFeeder)
                    {
                            var feedername = (from u in db.UtilityMainFeederMappings
                                              join
                             f in db.MM_Feeders on
                             u.Feeder_ID equals f.Feeder_ID
                                              where u.TagIndex == item
                                              select new { f.FeederName, f.Feeder_ID }).FirstOrDefault();




                            // var TagKWH = db.UtilityMainFeederMappings.Where(s => s.Feeder_ID == item && s.Parameter_ID == 1).Select(s => s.TagIndex).FirstOrDefault();
                           
                            var ShopName = (from s in db.MM_MTTUW_Shops
                                            where s.Plant_ID == shop_Index_Config.Plant_ID && s.Shop_ID == shop_Index_Config.Shop_ID
                                            select new { s.Shop_Name }).FirstOrDefault();
                        shop_Index_Config.Shop_Name = ShopName.Shop_Name;
                        shop_Index_Config.TagIndex = Convert.ToInt32(item);
                        shop_Index_Config.Inserted_Date = DateTime.Now;
                        shop_Index_Config.Inserted_Host = compName;
                        shop_Index_Config.Inserted_User_ID = insertedUserID;
                        shop_Index_Config.Feeder_ID = feedername.Feeder_ID;
                        shop_Index_Config.Consume_Power = true;
                        shop_Index_Config.TagBoolean = true;
                        shop_Index_Config.ConsumptionType = true;
                        db.Shop_Index_Config.Add(shop_Index_Config);
                        db.SaveChanges();
                            if (Consume == true)
                            {
                                shop_Index_Config.Consume_Power = false;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                            }
                            var kvahParaId = db.MM_Parameter.Where(s => s.Prameter_Name == "KVAH").Select(s => s.Prameter_ID).FirstOrDefault();
                            var kVAHtag = db.UtilityMainFeederMappings.Where(s => s.Parameter_ID == kvahParaId && s.Feeder_ID == feedername.Feeder_ID).Select(s => s.TagIndex).FirstOrDefault();

                            if (kVAHtag != null)
                            {
                                shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                shop_Index_Config.ConsumptionType = false;
                                shop_Index_Config.Consume_Power = true;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                                if (Consume == true)
                                {
                                    shop_Index_Config.Consume_Power = false;
                                
                                    shop_Index_Config.ConsumptionType = false;
                                    db.Shop_Index_Config.Add(shop_Index_Config);
                                    db.SaveChanges();
                                }

                            }
                        
                            db.SaveChanges();
                    }
                    foreach (decimal item in shop_Index_Config.IncomersubFeeder)
                        {
                            var feedername = (from u in db.UtilityMainFeederMappings
                                              join
                             f in db.MM_Feeders on
                             u.Feeder_ID equals f.Feeder_ID
                                              where u.TagIndex == item
                                              select new { f.FeederName, f.Feeder_ID }).FirstOrDefault();




                            //var TagKWH = db.UtilityMainFeederMappings.Where(s => s.Feeder_ID == item && s.Parameter_ID == 1).Select(s => s.TagIndex).FirstOrDefault();
                            shop_Index_Config.TagIndex = Convert.ToInt32(item); 

                           

                            var ShopName = (from s in db.MM_MTTUW_Shops
                                            where s.Plant_ID == shop_Index_Config.Plant_ID && s.Shop_ID == shop_Index_Config.Shop_ID
                                            select new { s.Shop_Name }).FirstOrDefault();

                            shop_Index_Config.Shop_Name = ShopName.Shop_Name;
                       
                            shop_Index_Config.Inserted_Date = DateTime.Now;
                            shop_Index_Config.Inserted_Host = compName;
                            shop_Index_Config.Inserted_User_ID = insertedUserID;
                            shop_Index_Config.Feeder_ID = feedername.Feeder_ID;
                           
                            shop_Index_Config.Consume_Power = true;
                            shop_Index_Config.TagBoolean = false;
                            shop_Index_Config.ConsumptionType = true;
                            db.Shop_Index_Config.Add(shop_Index_Config);
                            db.SaveChanges();

                           
                            if (Consume == true)
                            {
                                shop_Index_Config.Consume_Power = false;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                            }
                            var kvahParaId = db.MM_Parameter.Where(s => s.Prameter_Name == "KVAH").Select(s => s.Prameter_ID).FirstOrDefault();
                            var kVAHtag = db.UtilityMainFeederMappings.Where(s => s.Parameter_ID == kvahParaId && s.Feeder_ID == feedername.Feeder_ID).Select(s => s.TagIndex).FirstOrDefault();

                            if (kVAHtag != null)
                            {
                                shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                shop_Index_Config.ConsumptionType = false;
                                shop_Index_Config.Consume_Power = true;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                                if (Consume == true)
                                {
                                    shop_Index_Config.Consume_Power = false;
                                    shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                    shop_Index_Config.ConsumptionType = false;
                                    db.Shop_Index_Config.Add(shop_Index_Config);
                                    db.SaveChanges();
                                }

                            }
                            db.SaveChanges();
                        }

                    if(Consume==false)
                     {
                    foreach (decimal item in shop_Index_Config.ConsumeaddFeeder)
                     {
                                var feedername = (from u in db.UtilityMainFeederMappings
                                                  join
                                 f in db.MM_Feeders on
                                 u.Feeder_ID equals f.Feeder_ID
                                                  where u.TagIndex == item
                                                  select new { f.FeederName, f.Feeder_ID }).FirstOrDefault();



                               // var TagKWH = db.UtilityMainFeederMappings.Where(s => s.Feeder_ID == item && s.Parameter_ID == 1).Select(s => s.TagIndex).FirstOrDefault();
                                shop_Index_Config.TagIndex = Convert.ToInt32(item);



                                var ShopName = (from s in db.MM_MTTUW_Shops
                                            where s.Plant_ID == shop_Index_Config.Plant_ID && s.Shop_ID == shop_Index_Config.Shop_ID
                                            select new { s.Shop_Name }).FirstOrDefault();

                            shop_Index_Config.Shop_Name = ShopName.Shop_Name;
                          // shop_Index_Config.TagIndex = Convert.ToInt32(item);
                            shop_Index_Config.Inserted_Date = DateTime.Now;
                            shop_Index_Config.Inserted_Host = compName;
                            shop_Index_Config.Inserted_User_ID = insertedUserID;
                            shop_Index_Config.Feeder_ID = feedername.Feeder_ID;
                            shop_Index_Config.Consume_Power = false;
                            shop_Index_Config.TagBoolean = true;
                                shop_Index_Config.ConsumptionType = true;
                            db.Shop_Index_Config.Add(shop_Index_Config);
                            db.SaveChanges();
                                var kvahParaId = db.MM_Parameter.Where(s => s.Prameter_Name == "KVAH").Select(s => s.Prameter_ID).FirstOrDefault();
                                var kVAHtag = db.UtilityMainFeederMappings.Where(s => s.Parameter_ID == kvahParaId && s.Feeder_ID == feedername.Feeder_ID).Select(s => s.TagIndex).FirstOrDefault();

                                if (kVAHtag != null)
                                {
                                    shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                    shop_Index_Config.ConsumptionType = false;
                                    db.Shop_Index_Config.Add(shop_Index_Config);
                                    db.SaveChanges();
                                }
                            }
                    foreach (decimal item in shop_Index_Config.ConsumesubFeeder)
                        {
                                var feedername = (from u in db.UtilityMainFeederMappings
                                                  join
                                 f in db.MM_Feeders on
                                 u.Feeder_ID equals f.Feeder_ID
                                                  where u.TagIndex == item
                                                  select new { f.FeederName, f.Feeder_ID }).FirstOrDefault();




                                //var TagKWH = db.UtilityMainFeederMappings.Where(s => s.Feeder_ID == item && s.Parameter_ID == 1).Select(s => s.TagIndex).FirstOrDefault();
                                shop_Index_Config.TagIndex = Convert.ToInt32(item);



                                var ShopName = (from s in db.MM_MTTUW_Shops
                                            where s.Plant_ID == shop_Index_Config.Plant_ID && s.Shop_ID == shop_Index_Config.Shop_ID
                                            select new { s.Shop_Name }).FirstOrDefault();

                            shop_Index_Config.Shop_Name = ShopName.Shop_Name;
                            //shop_Index_Config.TagIndex = Convert.ToInt32(item);
                            shop_Index_Config.Inserted_Date = DateTime.Now;
                            shop_Index_Config.Inserted_Host = compName;
                            shop_Index_Config.Inserted_User_ID = insertedUserID;
                            shop_Index_Config.Feeder_ID = feedername.Feeder_ID;
                            shop_Index_Config.Consume_Power = false;
                            shop_Index_Config.TagBoolean = false;
                            shop_Index_Config.ConsumptionType = true;
                            db.Shop_Index_Config.Add(shop_Index_Config);
                            db.SaveChanges();
                                var kvahParaId = db.MM_Parameter.Where(s => s.Prameter_Name == "KVAH").Select(s => s.Prameter_ID).FirstOrDefault();
                                var kVAHtag = db.UtilityMainFeederMappings.Where(s => s.Parameter_ID == kvahParaId && s.Feeder_ID == feedername.Feeder_ID).Select(s => s.TagIndex).FirstOrDefault();

                                if (kVAHtag != null)
                                {
                                    shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                    shop_Index_Config.ConsumptionType = false;
                                    db.Shop_Index_Config.Add(shop_Index_Config);
                                    db.SaveChanges();
                                }
                            }
                    }
                       

                        globalData.isSuccessMessage = true;
                        globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                        globalData.messageDetail = ResourceShopwise_PowerIndexMapping.Sucess_Add;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                   
                }
                //ViewBag.TagIndex = new SelectList(db.UtilityMainFeederMappings, "TagIndex", "FeederName");
                //ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops, "Shop_ID", "Shop_Name");
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.ToList(), "Plant_ID", "Plant_Name",shop_Index_Config.Plant_ID);
          
                
                ViewBag.Shop_ID = new SelectList((from s in db.MM_MTTUW_Shops
                                                 
                                                  select (s)).ToList(), "Shop_ID", "Shop_Name",shop_Index_Config.Shop_ID);
                ViewBag.ComFeederShopID = new SelectList((from s in db.MM_MTTUW_Shops
                                                  
                                                  select (s)).ToList(), "Shop_ID", "Shop_Name", shop_Index_Config.ComFeederShopID);
                //ViewBag.feeder = new SelectList(db.UtilityMainFeederMappings, "TagIndex", "FeederName", shop_Index_Config.TagIndex);
                return View(shop_Index_Config);

            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
           
        }

        // GET: Shop_Power_Index_Mapping/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
            
                int userID = ((FDSession)this.Session["FDSession"]).userId;              
                globalData.pageTitle = ResourceMainFeederMapping.Title_Add_MainFeeder;
                ViewBag.GlobalDataModel = globalData;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plant_ID = plantID;
                var Inaddfeeder = db.Shop_Index_Config.Where(x => x.Shop_ID == id && x.TagBoolean ==true && x.Plant_ID == plantID && x.Consume_Power == true && x.ConsumptionType == true).Select(x => (x.TagIndex)).ToArray();
                var Insubfeeder = db.Shop_Index_Config.Where(x => x.Shop_ID == id && x.TagBoolean == false && x.Plant_ID == plantID && x.Consume_Power == true &&  x.ConsumptionType == true).Select(x => (x.TagIndex)).ToArray();
                var consumeddfeeder = db.Shop_Index_Config.Where(x => x.Shop_ID == id && x.TagBoolean == true && x.Plant_ID == plantID && x.Consume_Power == false && x.ConsumptionType == true).Select(x => (x.TagIndex)).ToArray();
                var consumesubfeeder = db.Shop_Index_Config.Where(x => x.Shop_ID == id && x.TagBoolean == false && x.Plant_ID == plantID && x.Consume_Power == false && x.ConsumptionType == true).Select(x => (x.TagIndex)).ToArray();

                List<int> comman = new List<int>();

                var Result = (from U in db.UtilityMainFeederMappings join
                              f in db.MM_Feeders on
                              U.Feeder_ID equals f.Feeder_ID  
                              where ( (U.Shop_ID == id && U.Plant_ID == plantID && (U.Parameter_ID ==1 ||U.Parameter_ID ==2)) ||(  (U.Parameter_ID ==1) && U.Active == true) )
                              select new
                              {
                                  TagIndex = U.TagIndex,
                                  FeederName = f.FeederName  +  "(" + U.TagIndex + ")"
                              }).ToList();

                
                ViewBag.IncomeraddFeeder = new MultiSelectList(Result, "TagIndex", "FeederName", Inaddfeeder);
                ViewBag.IncomersubFeeder = new MultiSelectList(Result, "TagIndex", "FeederName", Insubfeeder);
                ViewBag.ConsumeaddFeeder = new MultiSelectList(Result, "TagIndex", "FeederName", consumeddfeeder);
                ViewBag.ConsumesubFeeder = new MultiSelectList(Result, "TagIndex", "FeederName", consumesubfeeder);
               


                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Shop_ID == id).ToList(), "Shop_ID", "Shop_Name", id);
                ViewBag.ComFeederShopID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                //ViewBag.TagBoolean = tag;
                ViewBag.Plant_ID = plantID;
                globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                return View();
               
             
                //return View(shop_Index_Config);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
           
        }

        // POST: Shop_Power_Index_Mapping/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Shop_Index_Config shop_Index_Config)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var Consume = shop_Index_Config.Consume_Power;
                if (ModelState.IsValid)
                {
                    DateTime today = DateTime.Now;
                    decimal insertedUserID = ((FDSession)this.Session["FDSession"]).userId;
                    string compName = ((FDSession)this.Session["FDSession"]).userHost;

                    var Feeder = db.Shop_Index_Config.Where(x => x.Shop_ID == shop_Index_Config.Shop_ID && shop_Index_Config.Plant_ID == shop_Index_Config.Plant_ID).ToList();
                    foreach(var item in Feeder)
                    {
                        db.Shop_Index_Config.Remove(item);
                        db.SaveChanges();
                    }



                    foreach (decimal item in shop_Index_Config.IncomeraddFeeder)
                    {
                        var feedername = (from u in db.UtilityMainFeederMappings
                                          join
                         f in db.MM_Feeders on
                         u.Feeder_ID equals f.Feeder_ID
                                          where u.TagIndex == item
                                          select new { f.FeederName ,f.Feeder_ID}).FirstOrDefault();



                        var ShopName = (from s in db.MM_MTTUW_Shops
                                        where s.Plant_ID == shop_Index_Config.Plant_ID && s.Shop_ID == shop_Index_Config.Shop_ID
                                        select new { s.Shop_Name }).FirstOrDefault();

                        shop_Index_Config.Shop_Name = ShopName.Shop_Name;
                        shop_Index_Config.TagIndex = Convert.ToInt32(item);
                        shop_Index_Config.Inserted_Date = DateTime.Now;
                        shop_Index_Config.Inserted_Host = compName;
                        shop_Index_Config.Inserted_User_ID = insertedUserID;
                        shop_Index_Config.Feeder_ID = feedername.Feeder_ID;
                        shop_Index_Config.Consume_Power = true;
                        shop_Index_Config.TagBoolean = true;
                        shop_Index_Config.ConsumptionType = true;
                        db.Shop_Index_Config.Add(shop_Index_Config);
                        db.SaveChanges();
                        
                        if (Consume == true)
                        {
                            shop_Index_Config.Consume_Power = false;
                            db.Shop_Index_Config.Add(shop_Index_Config);
                            db.SaveChanges();
                        }
                        var kvahParaId = db.MM_Parameter.Where(s => s.Prameter_Name == "KVAH").Select(s => s.Prameter_ID).FirstOrDefault();
                        var kVAHtag = db.UtilityMainFeederMappings.Where(s => s.Parameter_ID == kvahParaId && s.Feeder_ID == feedername.Feeder_ID).Select(s => s.TagIndex).FirstOrDefault();

                        if (kVAHtag != null)
                        {
                            shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                            shop_Index_Config.Consume_Power = true;
                            shop_Index_Config.ConsumptionType = false;
                            db.Shop_Index_Config.Add(shop_Index_Config);
                            db.SaveChanges();
                            if (Consume == true)
                            {
                                shop_Index_Config.Consume_Power = false;
                                shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                shop_Index_Config.ConsumptionType = false;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                            }

                        }

                    }

                    if (shop_Index_Config.IncomersubFeeder != null)
                    {
                        foreach (decimal item in shop_Index_Config.IncomersubFeeder)
                        {
                            var feedername = (from u in db.UtilityMainFeederMappings
                                              join
                                              f in db.MM_Feeders on
                                              u.Feeder_ID equals f.Feeder_ID
                                              where u.TagIndex == item
                                              select new { f.FeederName, f.Feeder_ID }).FirstOrDefault();



                            var ShopName = (from s in db.MM_MTTUW_Shops
                                            where s.Plant_ID == shop_Index_Config.Plant_ID && s.Shop_ID == shop_Index_Config.Shop_ID
                                            select new { s.Shop_Name }).FirstOrDefault();

                            shop_Index_Config.Shop_Name = ShopName.Shop_Name;
                            shop_Index_Config.TagIndex = Convert.ToInt32(item);
                            shop_Index_Config.Inserted_Date = DateTime.Now;
                            shop_Index_Config.Inserted_Host = compName;
                            shop_Index_Config.Inserted_User_ID = insertedUserID;
                            shop_Index_Config.Feeder_ID = feedername.Feeder_ID;

                            shop_Index_Config.Consume_Power = true;
                            shop_Index_Config.TagBoolean = false;
                            shop_Index_Config.ConsumptionType  = true;
                            db.Shop_Index_Config.Add(shop_Index_Config);
                            db.SaveChanges();
                            if (Consume == true)
                            {
                                shop_Index_Config.Consume_Power = false;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                            }
                            var kvahParaId = db.MM_Parameter.Where(s => s.Prameter_Name == "KVAH").Select(s => s.Prameter_ID).FirstOrDefault();
                            var kVAHtag = db.UtilityMainFeederMappings.Where(s => s.Parameter_ID == kvahParaId && s.Feeder_ID == feedername.Feeder_ID).Select(s => s.TagIndex).FirstOrDefault();
                            if (kVAHtag != null)
                            {
                                shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                shop_Index_Config.ConsumptionType = false;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                                if (Consume == true)
                                {
                                    shop_Index_Config.Consume_Power = false;
                                    shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                    shop_Index_Config.ConsumptionType = false;
                                    db.Shop_Index_Config.Add(shop_Index_Config);
                                    db.SaveChanges();
                                }

                            }
                           
                            db.SaveChanges();
                        }
                    }
                    if (Consume == false)
                    {


                        foreach (decimal item in shop_Index_Config.ConsumeaddFeeder)
                        {
                            var feedername = (from u in db.UtilityMainFeederMappings
                                              join
                                              f in db.MM_Feeders on
                                              u.Feeder_ID equals f.Feeder_ID
                                              where u.TagIndex == item
                                              select new { f.FeederName, f.Feeder_ID }).FirstOrDefault();



                            var ShopName = (from s in db.MM_MTTUW_Shops
                                            where s.Plant_ID == shop_Index_Config.Plant_ID && s.Shop_ID == shop_Index_Config.Shop_ID
                                            select new { s.Shop_Name }).FirstOrDefault();

                            shop_Index_Config.Shop_Name = ShopName.Shop_Name;
                            shop_Index_Config.TagIndex = Convert.ToInt32(item);
                            shop_Index_Config.Inserted_Date = DateTime.Now;
                            shop_Index_Config.Inserted_Host = compName;
                            shop_Index_Config.Inserted_User_ID = insertedUserID;
                            shop_Index_Config.Feeder_ID = feedername.Feeder_ID;
                            shop_Index_Config.Consume_Power = false;
                            shop_Index_Config.TagBoolean = true;
                            shop_Index_Config.ConsumptionType = true;
                            db.Shop_Index_Config.Add(shop_Index_Config);
                            db.SaveChanges();
                            var kvahParaId = db.MM_Parameter.Where(s => s.Prameter_Name == "KVAH").Select(s => s.Prameter_ID).FirstOrDefault();
                            var kVAHtag = db.UtilityMainFeederMappings.Where(s => s.Parameter_ID == kvahParaId && s.Feeder_ID == feedername.Feeder_ID).Select(s => s.TagIndex).FirstOrDefault();
                            if (kVAHtag != null)
                            {
                                shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                shop_Index_Config.ConsumptionType = false;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                                

                            }
                        }
                        if (shop_Index_Config.ConsumesubFeeder != null)
                        {
                            foreach (decimal item in shop_Index_Config.ConsumesubFeeder)
                            {
                                var feedername = (from u in db.UtilityMainFeederMappings
                                                  join
                                                  f in db.MM_Feeders on
                                                  u.Feeder_ID equals f.Feeder_ID
                                                  where u.TagIndex == item
                                                  select new { f.FeederName, f.Feeder_ID }).FirstOrDefault();



                                var ShopName = (from s in db.MM_MTTUW_Shops
                                                where s.Plant_ID == shop_Index_Config.Plant_ID && s.Shop_ID == shop_Index_Config.Shop_ID
                                                select new { s.Shop_Name }).FirstOrDefault();

                                shop_Index_Config.Shop_Name = ShopName.Shop_Name;
                                shop_Index_Config.TagIndex = Convert.ToInt32(item);
                                shop_Index_Config.Inserted_Date = DateTime.Now;
                                shop_Index_Config.Inserted_Host = compName;
                                shop_Index_Config.Inserted_User_ID = insertedUserID;
                                shop_Index_Config.Feeder_ID = feedername.Feeder_ID;
                                shop_Index_Config.Consume_Power = false;
                                shop_Index_Config.TagBoolean = false;
                                shop_Index_Config.ConsumptionType = true;
                                db.Shop_Index_Config.Add(shop_Index_Config);
                                db.SaveChanges();
                                var kvahParaId = db.MM_Parameter.Where(s => s.Prameter_Name == "KVAH").Select(s => s.Prameter_ID).FirstOrDefault();
                                var kVAHtag = db.UtilityMainFeederMappings.Where(s => s.Parameter_ID == kvahParaId && s.Feeder_ID == feedername.Feeder_ID).Select(s => s.TagIndex).FirstOrDefault();
                                if (kVAHtag != null)
                                {
                                    shop_Index_Config.TagIndex = Convert.ToInt32(kVAHtag);
                                    shop_Index_Config.ConsumptionType = false;
                                    db.Shop_Index_Config.Add(shop_Index_Config);
                                    db.SaveChanges();


                                }
                            }
                        }
                    }
                    globalData.isSuccessMessage = true;
                    globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                    globalData.messageDetail = ResourceShopwise_PowerIndexMapping.Success_edit;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Shop_ID == shop_Index_Config.Shop_ID).ToList(), "Shop_ID", "Shop_Name", shop_Index_Config.Shop_ID);
                ViewBag.ComFeederShopID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
             
                return View(shop_Index_Config);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
          
        }

      
       
       
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, Boolean? tag)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var Feeder = db.Shop_Index_Config.Where(x => x.Shop_ID == id && x.Plant_ID == plantID).ToList();
                foreach (var item in Feeder)
                {
                    db.Shop_Index_Config.Remove(item);
                    db.SaveChanges();
                }
                Shop_Index_Config shop_Index_Config = db.Shop_Index_Config.Find(id);
                db.Shop_Index_Config.Remove(shop_Index_Config);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                globalData.messageDetail = ResourceShopwise_PowerIndexMapping.Success_Delete;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                globalData.messageDetail = ResourceShopwise_PowerIndexMapping.Delete_Dependency_Failure;
                TempData["globalData"] = globalData;
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
