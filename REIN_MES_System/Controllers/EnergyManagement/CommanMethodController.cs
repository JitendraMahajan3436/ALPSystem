//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using ZHB_AD.Models;
//using ZHB_AD.Helper;

//namespace ZHB_AD.Controllers.ZHB_AD
//{
//    public class CommanMethodController : Controller
//    {

//        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
//        GlobalData globalData = new GlobalData();
//        FDSession adSession = new FDSession();
//        General generalHelper = new General();
//        //Plantwise Shop Configure 
//        public ActionResult PlantwiseShopconfig(int? Plant_ID)
//        {
//           // int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//            var Result = (from U in db.MM_MTTUW_Shops
//                          where U.Plant_ID == Plant_ID
//                          select new
//                          {
//                              U.Shop_ID,
//                              U.Shop_Name
//                          }).ToList();
//            return Json(Result);
//        }

//        public ActionResult PlantwiseShopGroupconfig(int? Plant_ID)
//        {
//            // int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//            var Result = (from U in db.MM_ShopsCategory
//                          where U.Plant_ID == Plant_ID
//                          select new
//                          {
//                              U.ShopsCat_ID,
//                              U.ShopsCategory_Name
//                          }).ToList();
//            return Json(Result);
//        }

//        public ActionResult PlantwiseCategoryconfig(int? Plant_ID)
//        {
//            var Result = (from U in db.MM_Category
//                          where U.Plant_ID == Plant_ID
//                          select new
//                          {
//                              U.Category_Id,
//                              U.Category_Name
//                          }).ToList();
//            return Json(Result);
//        }

//        public ActionResult PlantwiseParameterconfig(int? Plant_ID)
//        {
//            var Result = (from U in db.MM_Parameter
//                          where U.Plant_ID == Plant_ID
//                          select new
//                          {
//                              U.Prameter_ID,
//                              U.Prameter_Name
//                          }).ToList();
//            return Json(Result);
//        }

//        public ActionResult SharedFeederwiseconfig(int Shop_Id, int ParameterId)
//        {
//            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//            var Result = (from U in db.UtilityMainFeederMappings join
//                          F in db.MM_Feeders on
//                          U.Feeder_ID equals F.Feeder_ID join p in db.MM_Parameter on
//                          U.Parameter_ID equals p.Prameter_ID
//                          where U.Shop_ID == Shop_Id && U.Plant_ID == plantID
//                          && U.Active == true &&(U.Parameter_ID == ParameterId)
//                          select new
//                          {
//                              U.TagIndex,
//                              FeederName = F.FeederName + "(" + U.TagIndex + ")"
//                          }).ToList();
//            return Json(Result);
//        }
//        public ActionResult Feederwiseconfig(int Shop_Id)
//        {
//            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//            var Result = (from U in db.UtilityMainFeederMappings join
//                          f in db.MM_Feeders on
//                          U.Feeder_ID equals f.Feeder_ID
//                          where U.Shop_ID == Shop_Id && U.Plant_ID == plantID && U.Parameter_ID ==1
                        
//                          select new
//                          {
//                              U.TagIndex,
//                              f.FeederName
//                          }).ToList();
//            return Json(Result);
//        }
//        public ActionResult Manualmeterconfig(int Shop_Id)
//        {
//            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
//            var Result = (from U in db.UtilityMainFeederMappings join
//                          f in db.MM_Feeders on
//                          U.Feeder_ID equals f.Feeder_ID
//                          where U.Shop_ID == Shop_Id && U.Plant_ID == plantID
//                          && U.ManualMeter == true
//                          select new
//                          {
//                              U.TagIndex,
//                              f.FeederName
//                          }).ToList();
//            return Json(Result);
//        }

//        public ActionResult AutoMailChart(int? plantID)
//        {

//            try
//            {
//                plantID = 1;
//                // yesterday datetime 
//                DateTime fromdate = DateTime.Now;
//                DateTime toDate = DateTime.Now;
//                DateTime date = DateTime.Now.Date;
//                DateTime dtn = DateTime.Now.Date;
//                var time = TimeSpan.Parse("06:30:00.000");
//                var time1 = TimeSpan.Parse("06:29:00.000");
//                date = date.AddDays(-1);
//                fromdate = (date + time);
//                toDate = (date.AddDays(1) + time1);

//                // Today datetime 

//                DateTime starttodaydate = System.DateTime.Now;
//                DateTime endtodaydate = System.DateTime.Now;
//                DateTime date1 = DateTime.Now.Date;
//                starttodaydate = (date1.AddDays(0) + time);
             


//                // current  finance year
//                int CurrentYear = DateTime.Today.Year;
//                int PreviousYear = DateTime.Today.Year - 1;
//                int NextYear = DateTime.Today.Year + 1;
//                string PreYear = PreviousYear.ToString();
//                string NexYear = NextYear.ToString();
//                string CurYear = CurrentYear.ToString();
//                string FinYear = null;

//                if (DateTime.Today.Month > 3)
//                {

//                    FinYear = CurYear + "-" + NexYear;
//                }
//                else
//                {
//                    CurYear = CurrentYear.ToString("yy");
//                    FinYear = PreYear + "-" + CurYear;
//                }

//                // current month 
//                string month = DateTime.Now.ToString("MMMM");


//                // target list 
//                var Target1 = (from t in db.MM_PowerTarget
//                               join
//   s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
//                               where t.Year == FinYear && t.Month == month
//                               select new
//                               {
//                                   t.Target,
//                                   s.Shop_Name,
//                                   s.Shop_ID
//                               }).ToList();




//                // yesterday consumption  list
//                List<Sp_AllShopConsumption_Result> consumptionwise = db.Sp_AllShopConsumption(fromdate, toDate, plantID).ToList();
//                // yesterday production  list
//                List<sp_getdailyProductionCount_Result> Productionwise = db.sp_getdailyProductionCount(fromdate, toDate, Convert.ToInt32(plantID)).ToList();


              

//                List<double?> Yesterdaydata = new List<double?>();
            
//                List<double?> Target = new List<double?>();

//                List<string> ShopName = new List<string>();
//                // DateTime date = DateTime.Now;



//                //consumptionwise = consumptionwise.OrderByDescending(s => s.totalconsumption).ToList();
//                //int rCount = consumptionwise.Rows.Count;

//                // yesterday data 
//                for (int i = 0; i < consumptionwise.Count(); i++)
//                {
//                    for (int j = 0; j < Productionwise.Count(); j++)
//                    {


//                        if (consumptionwise[i].Shop_ID == Productionwise[j].ShopId)
//                        {
//                            // double Consumption = 0;
//                            if (Productionwise[j].totalproduction == 0)
//                            {
//                                Yesterdaydata.Add(0);
//                                ShopName.Add((consumptionwise[i].Shop_Name));
//                                //Consumption = (Math.Round(Convert.ToDouble(C.totalconsumption), 0));
//                                break;
//                            }
//                            else
//                            {
//                                int count = Convert.ToInt32(Productionwise[j].totalproduction);

//                                Yesterdaydata.Add(System.Math.Round((Convert.ToDouble(consumptionwise[i].totalconsumption) / count), 0));
//                                ShopName.Add((consumptionwise[i].Shop_Name));
//                                break;
//                            }


//                        }

//                    }

//                }

               
                    
               
//                for (int i = 0; i < Productionwise.Count(); i++)
//                {
//                    for (int k = 0; k < Target1.Count(); k++)
//                    {

//                        if (Productionwise[i].ShopId == Target1[k].Shop_ID)
//                        {
//                            if (Target1[k].Target == 0)
//                            {
//                                Target.Add(0);
//                            }
//                            else
//                            {
//                                Target.Add(System.Math.Round(Convert.ToDouble(Target1[k].Target), 0));
//                                break;

//                            }

//                        }

//                    }
//                }

           


//                ViewData["yesturdaydata"] = Yesterdaydata;
//                ViewData["Target"] = Target;
//                ViewData["shopName"] = ShopName;


//                return View();
//            }
//            catch
//            {
//                return RedirectToAction("Index");
//            }


//        }

//        public ActionResult ShopwiseFeeder(int? Shop_Id)
//        {
//            var Result = (from f in db.MM_Feeders
//                          where f.Shop_ID == Shop_Id
//                          select new
//                          {
//                              f.Feeder_ID,
//                              f.FeederName
//                          }).ToList();

//            return Json(Result);
//        }

//        public ActionResult ShopwiseArea(int? Shop_Id)
//        {
//            var Result = (from f in db.MM_Area
//                          where f.Shop_ID == Shop_Id
//                          select new
//                          {
//                              f.Area_Id,
//                              f.Area_Name
//                          }).ToList();

//            return Json(Result);
//        }

//        public ActionResult Feederwisedesc(int? Feeder_ID)
//        {
//            var Result = (from f in db.MM_Feeders
//                          where f.Feeder_ID == Feeder_ID
//                          select new
//                          {

//                              f.FeederDesc
//                          }).FirstOrDefault();

//            return Json(Result.FeederDesc);
//        }

//        public ActionResult ParameterwiseUnit(int? Parameter_ID)
//        {
//            try
//            {


          
//            var unit = (from p in db.MM_Parameter
//                        where p.Prameter_ID == Parameter_ID
//                        select new
//                        {
//                            p.Unit
//                        }).FirstOrDefault();
//            var Result = (from f in db.MM_Unit_Measurement
//                         where f.Display_unit == unit.Unit
//                          select new
//                         {
//                             f.Unit_ID,
//                             f.Data_Unit
//                         }).ToList();

//            return Json(Result);
//            }
//            catch(Exception ex)
//            {
//                return Json(null);
//            }
       
//        }


//        public ActionResult ShopTagIndexconfig(int? Tag_Index)
//        {

//            var Result = (from p in db.UtilityMainFeederMappings
//                        where p.TagIndex == Tag_Index
//                        select new
//                        {
//                            p.TagIndex
//                        }).ToList();
          

//            return Json(Result);
//        }
//    }
//}