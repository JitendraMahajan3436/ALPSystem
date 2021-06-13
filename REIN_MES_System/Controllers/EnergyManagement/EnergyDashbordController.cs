using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using Newtonsoft.Json;
using ZHB_AD.App_LocalResources;

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class EnergyDashbordController : Controller
    {
        // GET: EnergyDashbord

        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();
        Boolean Is_Production = true;
        public ActionResult Index()

        {
            try
            {

                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                List<Performance_Indices> Performace = new List<Performance_Indices>();
                List<Performance_Indices> ShopwisePerformace = new List<Performance_Indices>();
                List<ShopWisespec_Data> Process = new List<ShopWisespec_Data>();
                List<ShopWisespec_Data> busniess = new List<ShopWisespec_Data>();
                List<Barchart> CategoryData = new List<Barchart>();
                List<Barchart> Category_Load = new List<Barchart>();
                List<double?> Target = new List<double?>();

                Session["PageTitle"] = "Energey Dashbord ";
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                globalData.pageTitle = ResourceEneregyDashboard.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                var today = System.DateTime.Today;
                var enddate1 = today.Date;

                today = today.AddDays(-1);
                var startdate1 = today.Date;


                var Perform = (from p in db.MM_Performance_Indices_Energy
                               where p.DateandTime == today && p.ConsumptionType == true
                               select new
                               {
                                   p.Shop_ID,
                                   p.Consumption,
                                   p.Best,
                                   p.Average,
                                   p.Efficiency,
                                   p.Production,
                                   p.TotalConsumption,
                                   p.DateandTime
                               }).OrderBy(s => s.Shop_ID).ToList();


                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;

                if (DateTime.Today.Month > 3)
                {

                    FinYear = CurYear + "-" + NexYear;
                }
                else
                {
                    CurYear = CurrentYear.ToString();
                    FinYear = PreYear + "-" + CurYear;
                }

                // current month 
                string month = DateTime.Now.ToString("MMMM");
                // target list 
                var Target1 = (from t in db.MM_PowerTarget
                               join
                                s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                               where t.Year == FinYear && t.Month == month && s.Spec_Unit_ID != 5 && s.Energy == true
                               select new
                               {
                                   t.Target,
                                   s.Shop_Name,
                                   s.Shop_ID
                               }).ToList();

                List<ShopWiseConsumption> consumptionwise3 = new List<ShopWiseConsumption>();

                foreach (var id in Perform.OrderByDescending(s => s.TotalConsumption))
                {
                    string ShopName;
                    Boolean Generation = true;
                    if (id.Shop_ID == 0)
                    {
                        ShopName = "Plant";
                        Generation = true;
                    }
                    else
                    {
                        var ShopName1 = (from s in db.MM_MTTUW_Shops
                                         join
                   g in db.MM_ShopsCategory on
                   s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == id.Shop_ID
                                         select new
                                         {
                                             s.Shop_Name,
                                             g.Is_Consumption,
                                             s.Spec_Unit_ID
                                         }).FirstOrDefault();
                        ShopName = ShopName1.Shop_Name;
                        Generation = Convert.ToBoolean(ShopName1.Is_Consumption);
                        Is_Production = true;
                        if (ShopName1.Spec_Unit_ID == 5)
                        {
                            Is_Production = false;
                        }
                    }

                    double Consumptionengine = Convert.ToDouble(id.Consumption);
                    double Best = Convert.ToDouble(id.Best);
                    double Avg = Convert.ToDouble(id.Average);
                    double Effiency = Convert.ToDouble(id.Efficiency);
                    double Consumption = Convert.ToDouble(id.TotalConsumption);
                    int Production = Convert.ToInt32(id.Production);
                    int ShopID = Convert.ToInt32(id.Shop_ID);

                    Performance_Indices obj = new Performance_Indices(ShopName, Consumptionengine, Best, Avg, Effiency, Consumption, Production, ShopID, Generation, Is_Production);

                    if (id.Shop_ID != 0)
                    {


                        ShopwisePerformace.Add(obj);
                        var shop = (from s in db.MM_MTTUW_Shops
                                    join
             g in db.MM_ShopsCategory on
             s.ShopsCat_ID equals g.ShopsCat_ID
                                    where s.Shop_ID == id.Shop_ID
                                    select new
                                    {
                                        s.Shop_Name,
                                        s.ShopsCat_ID,
                                        g.ShopsCategory_Name,
                                        s.Business_ID,

                                    }).FirstOrDefault();
                        int groupId = Convert.ToInt32(shop.ShopsCat_ID);
                        int busniesId = Convert.ToInt32(shop.Business_ID);
                        var shopName = shop.Shop_Name;
                        var groupName = shop.ShopsCategory_Name;
                        var total = Convert.ToDouble(id.TotalConsumption);
                        var pro_consum = Convert.ToDouble(id.Consumption);
                        int pro_Production = Convert.ToInt32(id.Production);

                        ShopWiseConsumption obj1 = new ShopWiseConsumption(groupId, Convert.ToInt32(id.Shop_ID), shopName, groupName, total);
                        ShopWisespec_Data Pro_Data = new ShopWisespec_Data(groupId, groupName, total, pro_Production, pro_consum);
                        ShopWisespec_Data bus_Data = new ShopWisespec_Data(busniesId, shopName, total, pro_Production, pro_consum);
                        consumptionwise3.Add(obj1);
                        Process.Add(Pro_Data);
                        busniess.Add(bus_Data);
                    }
                    Performace.Add(obj);


                }

                var consumptionwise1 = consumptionwise3.GroupBy(c => c.ShopsCat_ID).Select(c => new { totalconsumption = c.Sum(b => b.totalconsumption), ShopsCat_ID = c.Key, }).ToList();
                var Shopgroups = (from shop in db.MM_MTTUW_Shops
                                  join shopgroup in db.MM_ShopsCategory
                                  on shop.ShopsCat_ID equals shopgroup.ShopsCat_ID
                                  //join consum in consumptionwise1
                                  //on shopgroup.ShopsCat_ID equals consum.ShopsCat_ID
                                  where shop.Plant_ID == plantID && shop.Energy == true
                                  select shopgroup
                                     ).ToList().Distinct();
                var res = Shopgroups.OrderBy(s => s.Sort_Order).Select(c => new ShopWiseConsumption1
                {

                    ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),

                    ShopsCategory_Name = c.ShopsCategory_Name,

                    totalconsumption = Convert.ToDouble(consumptionwise3.Where(dc => dc.ShopsCat_ID == c.ShopsCat_ID).Sum(dc => dc.totalconsumption))
                }).Distinct().ToList();

                List<ShopWisespec_Data> Process_data = new List<ShopWisespec_Data>();
                foreach (var id in Shopgroups)
                {
                    double Consumption = 0.0;
                    var Total = Convert.ToDouble(Process.Where(dc => dc.dataId == id.ShopsCat_ID).Sum(dc => dc.totalconsumption));
                    var Production = Convert.ToInt32(Process.Where(dc => dc.dataId == id.ShopsCat_ID).Sum(dc => dc.Production));
                    if (Production != 0)
                        Consumption = Math.Round((Total / Production), 0);
                    ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(id.ShopsCat_ID), id.ShopsCategory_Name, Total, Production, Consumption);
                    Process_data.Add(Pro_Data);
                }
                List<ShopWisespec_Data> Business_data = new List<ShopWisespec_Data>();
                foreach (var id in db.MM_Business)
                {
                    double Consumption = 0.0;
                    var Total = Convert.ToDouble(busniess.Where(dc => dc.dataId == id.Business_Id).Sum(dc => dc.totalconsumption));
                    var Production = Convert.ToInt32(busniess.Where(dc => dc.dataId == id.Business_Id).Sum(dc => dc.Production));
                    if (Production != 0)
                        Consumption = Math.Round((Total / Production), 0);
                    ShopWisespec_Data bus_Data = new ShopWisespec_Data(Convert.ToInt32(id.Business_Id), id.Business_Name, Total, Production, Consumption);
                    Business_data.Add(bus_Data);
                }


                ViewData["Processwise_Data"] = Process_data.OrderByDescending(s => s.totalconsumption);
                ViewData["Performance_Shopwise"] = ShopwisePerformace;
                ViewBag.pieData = JsonConvert.SerializeObject(Performace);
                ViewBag.MinuteRange = new SelectList(db.MM_MinutesRange.Where(s => s.Minute == 60).ToList(), "Minute", "unit", 60);
                ViewBag.Shiftwise = new SelectList(db.MM_MTTUW_Shift.Where(s=>s.Shop_ID==30).ToList(), "Shift_ID", "Shift_Name");

                if (Target1.Count() == 0)
                {
                    for (int i = 0; i < ShopwisePerformace.Count(); i++)
                    {
                        Target.Add(0);
                    }
                }
                else
                {
                    for (int i = 0; i < ShopwisePerformace.Count(); i++)
                    {
                        int j = 0;
                        for (int k = 0; k < Target1.Count(); k++)
                        {
                            j = k;
                            if (ShopwisePerformace[i].ShopId == Target1[k].Shop_ID)
                            {
                                if (Target1[k].Target == 0)
                                {
                                    Target.Add(null);
                                    break;
                                }
                                else
                                {
                                    Target.Add(System.Math.Round(Convert.ToDouble(Target1[k].Target), 0));
                                    break;

                                }

                            }

                        }
                        if (j == (Target1.Count() - 1))
                        {
                            Target.Add(null);

                        }

                    }
                }

                ViewData["Target"] = Target;
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange, "DateID", "DateName", 2);

                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shop_ID ==30
                             select
                             new
                             {
                                 s.Shift_ID,
                                 s.Shift_Start_Time,
                                 s.Shift_Name
                             }).OrderBy(s => s.Shift_ID).ToList();
                var starttime = TimeSpan.Parse(shift[0].Shift_Start_Time.ToString());

                var TOd = db.MM_Tod.ToList();

                //List<TODheader> todobj = new List<TODheader>();
                //foreach(var id in TOd)
                //{
                //    var start = id.Tod_Start_Time.ToString("hh:mm");
                //    var End = id.Tod_End_Time.ToString(hh: mm""");

                //}
                ViewBag.ShiftA = shift[0].Shift_Name;
                ViewBag.ShiftB = shift[1].Shift_Name;
                ViewBag.ShiftC = shift[2].Shift_Name;

                ViewData["TOD"] = TOd;

                startdate1 = (startdate1 + starttime);
                enddate1 = (enddate1 + starttime);
                var PlantsConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                         where b.Shop_ID == 0 &&
                                        b.DateandTime > startdate1 && b.DateandTime <= enddate1 && b.ConsumptionType == true
                                         select new
                                         {
                                             b.Consumption,
                                             b.Best,
                                             b.Average,
                                             b.Inserted_Date,
                                             b.Time,
                                             b.TotalConsumption,
                                             b.Production,
                                             b.DateandTime
                                         }).ToList().OrderBy(m => m.Inserted_Date);
                List<string> allDates = new List<string>();
                double best = 0;
                int bestprod = 0;
                var bestdate = "";
                double bestpower = 0.0;
                double maxpower = 0.0;
                var maxdate = "";
                List<double?> cumulative = new List<double?>();
                List<double?> Averge = new List<double?>();
                double avgdata = 0.0;
                double cumulativedata = 0.0;
                int avg = 1;


                foreach (var id in PlantsConsumption)
                {
                    string ddl = id.Time;
                    allDates.Add(ddl);
                    if (id.Consumption != 0)
                    {
                        if (best == 0)
                        {
                            best = Math.Round(Convert.ToDouble(id.Consumption), 0);
                            bestdate = ddl;
                            bestprod = Convert.ToInt32(id.Production);
                            bestpower = Math.Round(Convert.ToDouble(id.TotalConsumption), 0);

                        }
                        else if (id.Consumption < best)
                        {
                            best = Math.Round(Convert.ToDouble(id.Consumption), 0);
                            bestdate = ddl;
                            bestprod = Convert.ToInt32(id.Production);
                            bestpower = Math.Round(Convert.ToDouble(id.TotalConsumption), 0);
                        }


                    }
                    if (maxpower == 0)
                    {
                        maxpower = Math.Round(Convert.ToDouble(id.TotalConsumption), 0);
                        maxdate = ddl;
                    }
                    else if (id.TotalConsumption > maxpower)
                    {
                        maxpower = Math.Round(Convert.ToDouble(id.TotalConsumption), 0);
                        maxdate = ddl;
                    }
                    cumulativedata += (Convert.ToDouble(id.TotalConsumption));
                    avgdata = Math.Round(Convert.ToDouble(cumulativedata / avg), 0);
                    cumulative.Add(cumulativedata);
                    Averge.Add(Math.Round(avgdata, 0));
                    avg += 1;
                }

                var totalconsume = PlantsConsumption
                    .Sum(s => s.TotalConsumption);
                var totalavg = PlantsConsumption.Average(s => s.TotalConsumption);
                var totalprod = PlantsConsumption.Sum(s => s.Production);
                var totalavgsec = PlantsConsumption.Average(s => s.Consumption);

                ViewBag.totalconsume = Math.Round(Convert.ToDouble(totalconsume), 0);
                ViewBag.totalavg = Math.Round(Convert.ToDouble(totalavg), 0);
                ViewBag.totalprod = totalprod;
                ViewBag.totalavgsec = Math.Round(Convert.ToDouble(totalavgsec), 0);
                ViewBag.maxdate = maxdate;
                ViewBag.maxpower = Math.Round(Convert.ToDouble(maxpower), 0);
                ViewBag.best = Math.Round(Convert.ToDouble(best), 0);
                ViewBag.bestdate = bestdate;
                ViewBag.bestprod = bestprod;
                ViewBag.bestpower = Math.Round(Convert.ToDouble(bestpower), 0);




                var time = TimeSpan.Parse(shift[0].Shift_Start_Time.ToString());
                var time1 = TimeSpan.Parse(shift[0].Shift_Start_Time.ToString());

                DateTime fromdate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                DateTime date = DateTime.Now.Date;
                date = date.AddDays(-1);
                fromdate = (date + time);
                toDate = (date.AddDays(1) + time1);



                List<Sp_PlantCategorywiseConsumption_Result> Category = db.Sp_PlantCategorywiseConsumption(fromdate, toDate, "", "", plantID, null, true).ToList();
                foreach (var id in Category)
                {
                    Barchart obj3 = new Barchart(id.Category_Name, Convert.ToDouble(id.Comsumtionvalues), "Add", 1);
                    CategoryData.Add(obj3);
                }
                ViewData["CategoryData"] = CategoryData;
                ViewBag.CategoryData = JsonConvert.SerializeObject(CategoryData);
                ViewData["Business_Wise"] = Business_data.OrderByDescending(s => s.totalconsumption);
                ViewData["Plantwise_Data"] = PlantsConsumption;
                ViewData["cumulative"] = cumulative;
                ViewData["Averge"] = Averge;

                ViewData["Performance_Indices"] = PlantsConsumption;
                ViewData["Plant_timeseries"] = allDates;

                DateTime now = DateTime.Now;
                var month1stDate = new DateTime(now.Year, now.Month, 1);
                var firstmonthd = month1stDate.AddMonths(-1);
                var lastmonthd = month1stDate.AddDays(-1);
                //DateTime mondayOfLastWeek = date.AddDays(-(int)date.DayOfWeek - 6);   
                DateTime LastWeek = DateTime.Now.AddDays(-7);
                var endDate = now;
                var perivousdate = now.AddDays(-1);

                perivousdate = perivousdate.Date;
                month1stDate = firstmonthd.Date;
                LastWeek = LastWeek.Date;

                endDate = endDate.Date;



                var Performdata = (from p in db.MM_Performance_Indices_Energy
                                   where (p.DateandTime) >= firstmonthd && (p.DateandTime) <= lastmonthd &&
                                   p.Shop_ID == 0
                                   select new
                                   {
                                       p.Shop_ID,
                                       p.Consumption,
                                       p.Best,
                                       p.Average,
                                       p.Efficiency,
                                       p.Production,
                                       p.TotalConsumption,
                                       p.DateandTime,
                                       p.ConsumptionType

                                   }).OrderBy(s => s.DateandTime).ToList();
                var Data = (from p in db.MM_Performance_Indices_Energy
                            where (p.DateandTime) >= LastWeek && (p.DateandTime) <= endDate &&
                            p.Shop_ID == 0
                            select new
                            {
                                p.Shop_ID,
                                p.Consumption,
                                p.Best,
                                p.Average,
                                p.Efficiency,
                                p.Production,
                                p.TotalConsumption,
                                p.DateandTime,
                                p.ConsumptionType
                            }).OrderBy(s => s.DateandTime).ToList();

                var YesterdayData = Data.Where(s => s.DateandTime == perivousdate && s.ConsumptionType == true).FirstOrDefault();


                if (YesterdayData == null)
                {
                    ViewBag.Yesterday = 0;
                    ViewBag.yesterdayPro = 0;
                    ViewBag.Yesterdaycons = 0;
                }
                else
                {
                    ViewBag.Yesterday = YesterdayData.TotalConsumption;
                    ViewBag.yesterdayPro = YesterdayData.Production;
                    ViewBag.Yesterdaycons = YesterdayData.Consumption;
                }

                var perivousweekdata = Data.Where(s => s.DateandTime >= LastWeek && s.DateandTime <= endDate && s.ConsumptionType == true).
                     Sum(s => s.TotalConsumption);
                var perivousweekProdution = Data.Where(s => s.DateandTime >= LastWeek && s.DateandTime <= endDate && s.ConsumptionType == true).
                Sum(s => s.Production);
                perivousweekdata = Convert.ToDouble(perivousweekdata);
                perivousweekProdution = Convert.ToInt32(perivousweekProdution);
                var perivousspecfic = 0.0;
                if (perivousweekProdution > 0)
                {
                    perivousspecfic = Math.Round(Convert.ToDouble(perivousweekdata / perivousweekProdution), 0);
                }
                else
                {
                    perivousspecfic = 0;
                }
                ViewBag.weekdata = perivousweekdata;
                ViewBag.weekPro = perivousweekProdution;
                ViewBag.weekcons = perivousspecfic;

                var thismonthdata = Performdata.Where(s => s.DateandTime >= firstmonthd && s.DateandTime <= lastmonthd && s.ConsumptionType == true).
                    Sum(s => s.TotalConsumption);
                var thismonthProduction = Performdata.Where(s => s.DateandTime >= firstmonthd && s.DateandTime <= lastmonthd && s.ConsumptionType == true).
                     Sum(s => s.Production);
                var thisMonthspefic = 0.0;
                if (thismonthProduction > 0)
                {
                    thisMonthspefic = Math.Round(Convert.ToDouble(thismonthdata / thismonthProduction), 0);
                }
                else
                {
                    thisMonthspefic = 0;
                }
                ViewBag.Monthdata = thismonthdata;
                ViewBag.MonthPro = thismonthProduction;
                ViewBag.Monthcons = thisMonthspefic;

                var dayshift = (from s in db.MM_MTTUW_Shift
                                where s.Shift_ID == 25
                                select new
                                {
                                    s.Shift_Start_Time
                                }).FirstOrDefault();
                DateTime startm = DateTime.Parse(firstmonthd.Date.ToString());
                DateTime endm = DateTime.Parse(lastmonthd.Date.AddDays(1).ToString());
                DateTime startw = DateTime.Parse(LastWeek.Date.ToString());
                DateTime endw = DateTime.Parse(endDate.Date.ToString());
                DateTime starty = DateTime.Parse(perivousdate.Date.ToString());
                DateTime endy = DateTime.Parse(endDate.Date.ToString());
                startm = (startm + starttime);
                endm = (endm + starttime);
                startw = (startw + starttime);
                endw = (endw + starttime);
                starty = (starty + starttime);
                endy = (endy + starttime);
                ViewBag.startm = startm.ToString("dd-MMM hh:mm tt");
                ViewBag.endm = endm.ToString("dd-MMM hh:mm tt");
                ViewBag.startw = startw.ToString("dd-MMM hh:mm tt");
                ViewBag.endw = endw.ToString("dd-MMM hh:mm tt");
                ViewBag.starty = starty.ToString("dd-MMM hh:mm tt");
                ViewBag.endy = endy.ToString("dd-MMM hh:mm tt");
                var todaydata = (from p in db.MM_Shopwise_TimeConsumption
                                 where p.DateandTime > endy && p.DateandTime <= now && p.Shop_ID == 0 && p.ConsumptionType == true
                                 select p).ToList();
                var livedata = todaydata.Sum(s => s.TotalConsumption);
                var livePro = todaydata.Sum(s => s.Production);
                var livespec = 0.0;
                if (livePro > 0)
                {
                    livespec = Math.Round(Convert.ToDouble(livedata / livePro), 0);
                }
                else
                {
                    livespec = 0;

                }
                ViewBag.todayData = livedata;
                ViewBag.todayPro = livePro; ;
                ViewBag.Todaycons = livespec;
                ViewBag.startt = endy.ToString("dd-MMM hh:mm tt");
                ViewBag.endt = now.ToString("dd-MMM hh:mm tt");

                var CategoryRatedLoad = (from r in db.MM_RatedLoad
                                         where r.Shop_ID == 0
                                         select new
                                         {
                                             r.Category_ID,
                                             r.Percentage
                                         }).ToList();
                foreach (var id in CategoryRatedLoad.OrderByDescending(s => s.Percentage))
                {
                    var Name = db.MM_Category.Where(s => s.Category_Id == id.Category_ID).Select(s => s.Category_Name).FirstOrDefault();
                    Barchart obj3 = new Barchart(Name, Convert.ToDouble(id.Percentage), "Add", 1);
                    Category_Load.Add(obj3);
                }
                ViewData["CategoryRatedLoad"] = Category_Load;
                ViewBag.Reason_ID = new SelectList(db.MM_Holiday_Reason, "Reason_ID", "Reason_Name");
                var Consumeunit = db.MM_Parameter.Where(s => s.Prameter_ID == 1).Select(s => s.Unit).FirstOrDefault();
                ViewBag.Consumeunit = Consumeunit;

                ViewBag.Type = new SelectList(db.MM_ConsumptionType, "Consumption_ID", "ConsumptionName");




                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }





        }

        public ActionResult ALLParameter()
        {
            try
            {




                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange, "DateID", "DateName", 1);

                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Energy == true), "Shop_ID", "Shop_Name");
                ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name");
                ViewBag.Feeder_ID = new SelectList(db.MM_Feeders, "Feeder_Id", "FeederName");
                globalData.pageTitle = "Analytics";
                ViewBag.GlobalDataModel = globalData;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
        }

        public ActionResult Parameterwiseview(string StartDate, string EndDate, string Shop, string ParameterID, string feeder)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            int ShopId = Convert.ToInt32(Shop);
            string[] ParameterArray = { };
            if (ParameterID == "null")
            {
            }
            else
            {
                ParameterArray = ParameterID.Split(',').ToArray();
            }

            DateTime startdate1 = Convert.ToDateTime(StartDate);
            DateTime enddate1 = Convert.ToDateTime(EndDate);
            List<string> Names = new List<string>();
            List<string> allDates = new List<string>();
            List<Sp_ParameterWiseAnalaytics_Result> Parameterwiseobj = null;


            List<double?> ParameterConsumption = new List<double?>();
            List<string> Units = new List<string>();
            List<List<double?>> AllConsumption = new List<List<double?>>();

            foreach (var item in ParameterArray)
            {
                List<double?> Consumption = new List<double?>();
                //List<double?> Final_Consumption = new List<double?>();

                int ShopID = Convert.ToInt32(Shop);
                int ParameterId = Convert.ToInt32(item);
                int Feederid = Convert.ToInt32(feeder);

                var feeder1 = (from U in db.UtilityMainFeederMappings
                               join
f in db.MM_Feeders on
U.Feeder_ID equals f.Feeder_ID
                               join
un in db.MM_Unit_Measurement on
U.Unit_ID equals un.Unit_ID
                               where U.Feeder_ID == Feederid
                               select new
                               {
                                   U.TagIndex,
                                   un.Display_unit,
                                   f.Feeder_ID,
                                   f.FeederName
                               }).FirstOrDefault();

                Parameterwiseobj = db.Sp_ParameterWiseAnalaytics(ShopID, startdate1, enddate1, plantID, ParameterId, null, null, Convert.ToInt32(feeder1.TagIndex)).ToList();
                if (Parameterwiseobj.Count() > 0)
                {
                    foreach (var id in Parameterwiseobj)
                    {
                        Consumption.Add(id.Consumption);
                        allDates.Add(id.ConsumptionDate.ToString());
                        //string ddl = Convert.ToDateTime(id.ConsumptionDate).ToString("HH:mm");
                        //allDates.Add(ddl);



                    }
                }
                else
                {
                    Consumption.Add(0);
                    allDates.Add(null);
                }
                {


                    AllConsumption.Add(Consumption);
                    Units.Add(feeder1.Display_unit);
                    var name = feeder1.FeederName + "_" + feeder1.Display_unit;
                    Names.Add(name);
                }

            }

            return Json(new { AllConsumption, Names, allDates, Units }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult showchart()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                // yesterday datetime 
                DateTime fromdate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                DateTime date = DateTime.Now.Date;
                DateTime dtn = DateTime.Now.Date;
                var time = TimeSpan.Parse("06:30:00.000");
                var time1 = TimeSpan.Parse("06:29:00.000");
                date = date.AddDays(-1);
                fromdate = (date + time);
                toDate = (date.AddDays(1) + time1);

                // Today datetime 

                DateTime starttodaydate = System.DateTime.Now;
                DateTime endtodaydate = System.DateTime.Now;
                DateTime date1 = DateTime.Now.Date;
                starttodaydate = (date1.AddDays(0) + time);
                // List<SP_LiveShopConsumption_Result> todayConsumption = db.SP_LiveShopConsumption(plantID, starttodaydate, endtodaydate).ToList();


                // current  finance year
                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;

                if (DateTime.Today.Month > 3)
                {

                    FinYear = CurYear + "-" + NexYear;
                }
                else
                {
                    CurYear = CurrentYear.ToString();
                    FinYear = PreYear + "-" + CurYear;
                }

                // current month 
                string month = DateTime.Now.ToString("MMMM");


                // target list 

                var Target1 = (from t in db.MM_PowerTarget
                               join
   s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                               where t.Year == FinYear && t.Month == month && s.Spec_Unit_ID != 5 && s.Energy == true
                               select new
                               {
                                   t.Target,
                                   s.Shop_Name,
                                   s.Shop_ID
                               }).ToList();



                // yesterday consumption  list
                List<Sp_AllShopConsumption_Result> consumptionwise = db.Sp_AllShopConsumption(fromdate, toDate, Convert.ToInt32(plantID)).ToList();
                // yesterday production  list
                List<sp_getdailyProductionCount_Result> Productionwise = db.sp_getdailyProductionCount(fromdate, toDate, Convert.ToInt32(plantID)).ToList();


                // today List 
                //List<sp_getdailyProductionCount_Result> todayProduction = db.sp_getdailyProductionCount(starttodaydate, endtodaydate, Convert.ToInt32(plantID)).ToList();


                List<double?> Yesterdaydata = new List<double?>();
                List<double?> TodayData = new List<double?>();
                List<double?> Target = new List<double?>();

                List<string> ShopName = new List<string>();
                List<double?> PrivousKWHdata = new List<double?>();
                List<double?> TodayKWHData = new List<double?>();
                List<string> KWHShopName = new List<string>();

                //  KWH Graph

                //Today KWH Consumption
                //foreach (var id in todayConsumption)
                //{
                //    TodayKWHData.Add(id.totalconsumption);

                //}
                var today = System.DateTime.Today;
                today = today.AddDays(-1);
                var Perform = (from p in db.MM_Performance_Indices_Energy
                               where p.DateandTime == today && p.Shop_ID != 0
                               select new
                               {
                                   p.Shop_ID,
                                   p.Consumption,
                                   p.Best,
                                   p.Average,
                                   p.Efficiency,
                                   p.Production,
                                   p.TotalConsumption
                               }).OrderBy(s => s.Shop_ID).ToList();
                // Yesterday kWh Consumption
                foreach (var id in consumptionwise.OrderByDescending(s => s.totalconsumption))
                {
                    PrivousKWHdata.Add(id.totalconsumption);
                    KWHShopName.Add(id.Shop_Name);
                }




                // yesterday data 
                for (int i = 0; i < consumptionwise.Count(); i++)
                {
                    for (int j = 0; j < Productionwise.Count(); j++)
                    {


                        if (consumptionwise[i].Shop_ID == Productionwise[j].ShopId)
                        {
                            // double Consumption = 0;
                            if (Productionwise[j].totalproduction == 0)
                            {
                                Yesterdaydata.Add(0);
                                ShopName.Add((consumptionwise[i].Shop_Name));
                                //Consumption = (Math.Round(Convert.ToDouble(C.totalconsumption), 0));
                                break;
                            }
                            else
                            {
                                int count = Convert.ToInt32(Productionwise[j].totalproduction);

                                Yesterdaydata.Add(System.Math.Round((Convert.ToDouble(consumptionwise[i].totalconsumption) / count), 1));
                                ShopName.Add((consumptionwise[i].Shop_Name));
                                break;
                            }


                        }

                    }

                }
                //foreach (var id in Perform)
                //{
                //    Yesterdaydata.Add(id.Best);
                //    Yesterdaydata.Add(id.Average);
                //}
                // Today Data 
                //for (int i = 0; i < todayConsumption.Count(); i++)
                //{
                //    for (int j = 0; j < todayProduction.Count(); j++)
                //    {


                //        if (todayConsumption[i].Shop_ID == todayProduction[j].ShopId)
                //        {
                //            // double Consumption = 0;
                //            if (todayProduction[j].totalproduction == 0)
                //            {
                //                //if(todayProduction[i].ShopId == 4  || todayProduction[i].ShopId ==1)
                //                //{
                //                TodayData.Add(0);
                //                break;
                //                //}
                //                //Consumption = (Math.Round(Convert.ToDouble(C.totalconsumption), 0));


                //                //break;
                //            }
                //            else
                //            {
                //                int count = Convert.ToInt32(todayProduction[j].totalproduction);

                //                if (todayConsumption[i].totalconsumption == 0)
                //                {

                //                    TodayData.Add(0);
                //                    break;
                //                }
                //                else
                //                {
                //                    TodayData.Add(System.Math.Round((Convert.ToDouble(todayConsumption[i].totalconsumption) / count), 0));
                //                    break;
                //                }


                //            }


                //        }

                //    }

                //}


                for (int i = 0; i < Productionwise.Count(); i++)
                {
                    for (int k = 0; k < Target1.Count(); k++)
                    {

                        if (Productionwise[i].ShopId == Target1[k].Shop_ID)
                        {
                            if (Target1[k].Target == 0)
                            {
                                Target.Add(0);
                            }
                            else
                            {
                                Target.Add(System.Math.Round(Convert.ToDouble(Target1[k].Target), 0));
                                break;

                            }

                        }

                    }
                }
                if (Target1.Count() == 0)
                {
                    for (int i = 0; i < Productionwise.Count(); i++)
                    {
                        Target.Add(0);
                    }
                }



                return Json(new { Yesterdaydata, Target, ShopName, PrivousKWHdata, KWHShopName }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }


        }

        public ActionResult PerformceceEnergy(string Formdate, string todate, string ddlformat, int? Shiftwise, int? Minute, string ConsumptionId, string Noworking, Boolean? Similar)
        {


            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                decimal groupId;
                //int Shop = 0;
                string shopName = null;
                string groupName = null;
                string starts = "";
                string ends = "";
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }

                double total = 0;
                DateTime start_m = DateTime.Parse(Formdate);
                DateTime end_m = DateTime.Parse(todate);
                var hours = (end_m - start_m).TotalHours;


                DateTime startdate1 = DateTime.Parse(Formdate);
                DateTime enddate1 = DateTime.Parse(todate);

                DateTime endg = DateTime.Parse(Formdate);
                DateTime startg = DateTime.Parse(Formdate);

                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;
                //Object Perform =null;
                var PlantsConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                         where b.Shop_ID == 0 &&
                                        b.DateandTime >= startdate1 && b.DateandTime <= enddate1 && b.ConsumptionType == type
                                         select new
                                         {
                                             b.Consumption,
                                             b.Best,
                                             b.Average,
                                             b.Inserted_Date,
                                             b.Time,
                                             b.TotalConsumption,
                                             b.Production
                                         }).ToList().OrderBy(m => m.Inserted_Date);
                var Perform = (from p in db.MM_Performance_Indices_Energy
                               where (p.Inserted_Date) >= startdate1 && (p.Inserted_Date) <= enddate1 && p.ConsumptionType == type
                               select new
                               {
                                   p.Shop_ID,
                                   p.Consumption,
                                   p.Best,
                                   p.Average,
                                   p.Efficiency,
                                   p.Production,
                                   p.TotalConsumption,
                                   p.DateandTime
                               }).OrderBy(s => s.DateandTime).ToList();

                var ShopConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                       where b.ConsumptionType == type &&
                                      b.DateandTime == startdate1 && b.Shop_ID == 0
                                       select new
                                       {
                                           b.Shop_ID,
                                           b.Consumption,
                                           b.Best,
                                           b.Average,
                                           b.Inserted_Date,
                                           b.Time,
                                           b.TotalConsumption,
                                           b.Production
                                       }).ToList().OrderBy(m => m.Inserted_Date);

                var shiftPerform = db.MM_Shiftwise_Consume_Power.Where(s => s.ConsumptionType == type && s.Income_Power == true && s.Dateandtime >= startdate1 && s.Dateandtime <= enddate1).ToList();

                List<ShopWiseConsumption> consumptionwise3 = new List<ShopWiseConsumption>();
                List<SP_LiveShopConsumption_Result> consumptionwise = new List<SP_LiveShopConsumption_Result>();
                List<sp_getdailyProductionCount_Result> Productionwise = new List<sp_getdailyProductionCount_Result>();
                List<Performance_Indices> Performace = new List<Performance_Indices>();
                List<Performance_Indices> ShopwisePerformace = new List<Performance_Indices>();
                List<Sp_Shift_AreawiseConsumption_Result> areaobj = new List<Sp_Shift_AreawiseConsumption_Result>();
                List<Sp_Shift_BusinesswiseConsumption_Result> businsobj = new List<Sp_Shift_BusinesswiseConsumption_Result>();
                List<ShopWisespec_Data> Process = new List<ShopWisespec_Data>();
                List<ShopWisespec_Data> busniess = new List<ShopWisespec_Data>();
                List<ShopWisespec_Data> Shopwise = new List<ShopWisespec_Data>();
                List<Sp_PlantCategorywiseConsumption_Result> Category = new List<Sp_PlantCategorywiseConsumption_Result>();
                List<SP_Live_BusinesswiseConsumption_Result> Livebusinsobj = new List<SP_Live_BusinesswiseConsumption_Result>();

                List<double?> BestData = new List<double?>();


                List<Barchart> CategoryData = new List<Barchart>();

                List<string> allDates = new List<string>();

                List<double?> Target = new List<double?>();
                List<double?> PlantTarget = new List<double?>();

                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID ==25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).First();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());

                enddate1 = enddate1.AddDays(1);

                enddate1 = (enddate1.Date + starttime);
                startdate1 = (startdate1.Date + starttime);

                var starttime1 = "";
                var endtime1 = "";
                var endmintime = "";
                int shiftId = 0;
                if (Shiftwise != null)
                {
                    shiftId = Convert.ToInt32(Shiftwise);
                    var shiftobj = (from s in db.MM_MTTUW_Shift
                                    where s.Shift_ID == shiftId
                                    select
                                    new
                                    {
                                        s.Shift_Start_Time,
                                        s.Shift_End_Time
                                    }).FirstOrDefault();
                    starttime1 = shiftobj.Shift_Start_Time.ToString();
                    endtime1 = shiftobj.Shift_Start_Time.ToString();
                    endmintime = shiftobj.Shift_End_Time.ToString();
                    DateTime startd = DateTime.Parse(Formdate);
                    DateTime endd = DateTime.Parse(todate);
                    var startt = TimeSpan.Parse(starttime1);
                    var Endt = TimeSpan.Parse(endmintime);
                    startg = DateTime.Parse(Formdate);
                    startg = (startd.Date + startt);
                    //endg = endg.AddDays(1);
                    if (shiftId == 3)
                    {
                        endg = endg.AddDays(1);

                    }
                    endg = (endg.Date + Endt);

                    startd = (startd.Date + startt);
                    endd = (endd.Date + Endt);

                    starts = startd.ToString("dd/MMM hh:mm tt");
                    ends = endd.ToString("dd/MMM hh:mm tt");

                }
                else
                {
                    DateTime startd = DateTime.Parse(Formdate);
                    DateTime endd = DateTime.Parse(todate);
                    endd = endd.AddDays(1);
                    startg = (startd.Date + starttime);
                    endg = (endd.Date + starttime);
                    starts = startg.ToString("dd/MMM hh:mm tt");
                    ends = endg.ToString("dd/MMM hh:mm tt");
                }

                DateTime plantstart = DateTime.Parse(Formdate);
                DateTime plantend = DateTime.Parse(todate);
                if (ddlformat == "5")
                {
                    plantstart = plantstart.Date;
                    plantend = plantend.Date;
                }
                if (Minute != null && (ddlformat == "2" || ddlformat == "5" || ddlformat == "1"))
                {
                    var todday1 = System.DateTime.Now;
                    if (ddlformat == "1")
                    {
                        var today = System.DateTime.Now;
                        PlantsConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                             where b.Shop_ID == 0 && b.ConsumptionType == type && 
                                            b.DateandTime > startdate1 && b.DateandTime <= today
                                             select new
                                             {
                                                 b.Consumption,
                                                 b.Best,
                                                 b.Average,
                                                 b.Inserted_Date,
                                                 b.Time,
                                                 b.TotalConsumption,
                                                 b.Production
                                             }).ToList().OrderBy(m => m.Inserted_Date);
                        foreach (var id in PlantsConsumption)
                        {
                            string ddl = id.Time;
                            allDates.Add(ddl);
                        }
                        ShopConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                           where b.ConsumptionType == type &&
                                          b.DateandTime >= startdate1 && b.DateandTime <= today
                                           select new
                                           {
                                               b.Shop_ID,
                                               b.Consumption,
                                               b.Best,
                                               b.Average,
                                               b.Inserted_Date,
                                               b.Time,
                                               b.TotalConsumption,
                                               b.Production
                                           }).ToList().OrderBy(m => m.Inserted_Date);
                        //Livebusinsobj = db.SP_Live_BusinesswiseConsumption(startdate1, today, "", "", plantID,Shiftwise,type).ToList();

                        //consumptionwise = db.SP_LiveShopConsumption(plantID, startdate1, today,Shiftwise,type).ToList();
                        //Productionwise = db.sp_getdailyProductionCount(startdate1, today, plantID).ToList();
                        Category = db.Sp_PlantCategorywiseConsumption(startdate1, enddate1, "", "", plantID, Shiftwise, type).ToList();
                        foreach (var id in ShopConsumption.Where(s => s.Shop_ID != 0))
                        {




                            var shop = (from s in db.MM_MTTUW_Shops
                                        join
                                        g in db.MM_ShopsCategory on
                                        s.ShopsCat_ID equals g.ShopsCat_ID
                                        where s.Shop_ID == id.Shop_ID && s.Energy ==true
                                        select new
                                        {
                                            s.Shop_Name,
                                            s.ShopsCat_ID,
                                            g.ShopsCategory_Name,
                                            s.Business_ID,

                                        }).FirstOrDefault();

                            if (shop != null)
                            {
                                groupId = Convert.ToInt32(shop.ShopsCat_ID);
                                int busniesId = Convert.ToInt32(shop.Business_ID);
                                shopName = shop.Shop_Name;
                                groupName = shop.ShopsCategory_Name;
                                total = Convert.ToDouble(id.TotalConsumption);
                                var pro_consum = Convert.ToDouble(id.Consumption);
                                int pro_Production = Convert.ToInt32(id.Production);

                                ShopWiseConsumption obj1 = new ShopWiseConsumption(groupId, Convert.ToInt32(id.Shop_ID), shopName, groupName, total);
                                ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(groupId), groupName, total, pro_Production, pro_consum);
                                ShopWisespec_Data bus_Data = new ShopWisespec_Data(busniesId, shopName, total, pro_Production, pro_consum);
                                consumptionwise3.Add(obj1);
                                Process.Add(Pro_Data);
                                busniess.Add(bus_Data);
                            }

                        }

                    }
                    else if (ddlformat == "5")
                    {
                        PlantsConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                             where b.Shop_ID == 0 && b.ConsumptionType == type &&
                                            b.DateandTime > start_m && b.DateandTime <= end_m
                                             select new
                                             {
                                                 b.Consumption,
                                                 b.Best,
                                                 b.Average,
                                                 b.Inserted_Date,
                                                 b.Time,
                                                 b.TotalConsumption,
                                                 b.Production
                                             }).ToList().OrderBy(m => m.Inserted_Date);
                        foreach (var id in PlantsConsumption)
                        {
                            string ddl = id.Time;
                            allDates.Add(ddl);
                        }
                        ShopConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                           where b.ConsumptionType == type &&
                                          b.DateandTime >= start_m && b.DateandTime <= end_m
                                           select new
                                           {
                                               b.Shop_ID,
                                               b.Consumption,
                                               b.Best,
                                               b.Average,
                                               b.Inserted_Date,
                                               b.Time,
                                               b.TotalConsumption,
                                               b.Production
                                           }).ToList().OrderBy(m => m.Inserted_Date);
                        //Livebusinsobj = db.SP_Live_BusinesswiseConsumption(startdate1, today, "", "", plantID,Shiftwise,type).ToList();

                        //consumptionwise = db.SP_LiveShopConsumption(plantID, startdate1, today,Shiftwise,type).ToList();
                        //Productionwise = db.sp_getdailyProductionCount(startdate1, today, plantID).ToList();
                        Category = db.Sp_PlantCategorywiseConsumption(start_m, end_m, "", "", plantID, Shiftwise, type).ToList();
                        foreach (var id in ShopConsumption.Where(s => s.Shop_ID != 0))
                        {




                            var shop = (from s in db.MM_MTTUW_Shops
                                        join
                                        g in db.MM_ShopsCategory on
                                        s.ShopsCat_ID equals g.ShopsCat_ID
                                        where s.Shop_ID == id.Shop_ID && s.Energy ==true
                                        select new
                                        {
                                            s.Shop_Name,
                                            s.ShopsCat_ID,
                                            g.ShopsCategory_Name,
                                            s.Business_ID,

                                        }).FirstOrDefault();

                            if (shop != null)
                            {
                                groupId = Convert.ToInt32(shop.ShopsCat_ID);
                                int busniesId = Convert.ToInt32(shop.Business_ID);
                                shopName = shop.Shop_Name;
                                groupName = shop.ShopsCategory_Name;
                                total = Convert.ToDouble(id.TotalConsumption);
                                var pro_consum = Convert.ToDouble(id.Consumption);
                                int pro_Production = Convert.ToInt32(id.Production);

                                ShopWiseConsumption obj1 = new ShopWiseConsumption(groupId, Convert.ToInt32(id.Shop_ID), shopName, groupName, total);
                                ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(groupId), groupName, total, pro_Production, pro_consum);
                                ShopWisespec_Data bus_Data = new ShopWisespec_Data(busniesId, shopName, total, pro_Production, pro_consum);
                                consumptionwise3.Add(obj1);
                                Process.Add(Pro_Data);
                                busniess.Add(bus_Data);
                            }

                        }
                    }
                    else
                    {


                        Perform = (from p in db.MM_Performance_Indices_Energy
                                   where (p.DateandTime) >= plantstart && (p.DateandTime) <= plantend && p.ConsumptionType == type
                                   select new
                                   {
                                       p.Shop_ID,
                                       p.Consumption,
                                       p.Best,
                                       p.Average,
                                       p.Efficiency,
                                       p.Production,
                                       p.TotalConsumption,
                                       p.DateandTime
                                   }).OrderBy(s => s.DateandTime).ToList();
                        businsobj = db.Sp_Shift_BusinesswiseConsumption(startdate1, enddate1, "", "", plantID, Shiftwise, type).ToList();
                        if (Shiftwise != null)
                        {


                            shiftPerform = db.MM_Shiftwise_Consume_Power.Where(s => s.ConsumptionType == type && s.Income_Power == true &&
                                                                       s.Dateandtime >= startdate1 && s.Dateandtime < enddate1 && s.Shift_ID == shiftId).ToList();
                        }
                        PlantsConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                             where b.Shop_ID == 0 &&
                                            b.DateandTime > startg && b.DateandTime <= endg && b.ConsumptionType == type
                                             select new
                                             {
                                                 b.Consumption,
                                                 b.Best,
                                                 b.Average,
                                                 b.Inserted_Date,
                                                 b.Time,
                                                 b.TotalConsumption,
                                                 b.Production
                                             }).ToList().OrderBy(m => m.Inserted_Date);
                        foreach (var id in PlantsConsumption)
                        {
                            string ddl = id.Time;
                            allDates.Add(ddl);
                        }
                        Category = db.Sp_PlantCategorywiseConsumption(startdate1, enddate1, "", "", plantID, Shiftwise, type).ToList();
                    }
                }
                else
                {
                    if (ddlformat == "2")
                    {
                        var today = System.DateTime.Today;

                        Perform = (from p in db.MM_Performance_Indices_Energy
                                   where (p.DateandTime) >= startdate1 && (p.DateandTime) <= enddate1 && p.ConsumptionType == type
                                   select new
                                   {
                                       p.Shop_ID,
                                       p.Consumption,
                                       p.Best,
                                       p.Average,
                                       p.Efficiency,
                                       p.Production,
                                       p.TotalConsumption,
                                       p.DateandTime
                                   }).OrderBy(s => s.DateandTime).ToList();

                    }
                    else
                    {
                        if (startdate1 == enddate1)
                        {


                            Perform = (from p in db.MM_Performance_Indices_Energy
                                       where (p.DateandTime) >= startdate1 && (p.DateandTime) <= enddate1
                                       select new
                                       {
                                           p.Shop_ID,
                                           p.Consumption,
                                           p.Best,
                                           p.Average,
                                           p.Efficiency,
                                           p.Production,
                                           p.TotalConsumption,
                                           p.DateandTime
                                       }).OrderBy(s => s.Shop_ID).ToList();

                            PlantsConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                                 where b.Shop_ID == 0 &&
                                                b.DateandTime >= startdate1 && b.DateandTime <= enddate1
                                                 select new
                                                 {
                                                     b.Consumption,
                                                     b.Best,
                                                     b.Average,
                                                     b.Inserted_Date,
                                                     b.Time,
                                                     b.TotalConsumption,
                                                     b.Production
                                                 }).ToList().OrderBy(m => m.Inserted_Date);
                            foreach (var id in PlantsConsumption)
                            {
                                string ddl = id.Time;
                                allDates.Add(ddl);
                            }
                        }
                        else
                        {


                            Perform = (from p in db.MM_Performance_Indices_Energy
                                       where (p.DateandTime) >= plantstart && (p.DateandTime) <= plantend && p.ConsumptionType == type
                                       select new
                                       {
                                           p.Shop_ID,
                                           p.Consumption,
                                           p.Best,
                                           p.Average,
                                           p.Efficiency,
                                           p.Production,
                                           p.TotalConsumption,
                                           p.DateandTime
                                       }).OrderBy(s => s.DateandTime).ToList();

                            businsobj = db.Sp_Shift_BusinesswiseConsumption(startdate1, enddate1, "", "", plantID, Shiftwise, type).ToList();
                            Category = db.Sp_PlantCategorywiseConsumption(startdate1, enddate1, "", "", plantID, Shiftwise, type).ToList();
                            if (Shiftwise != null)
                            {


                                shiftPerform = db.MM_Shiftwise_Consume_Power.Where(s => s.ConsumptionType == type && s.Income_Power == true &&
                                                                           s.Dateandtime >= startdate1 && s.Dateandtime < enddate1 && s.Shift_ID == shiftId).ToList();
                            }
                        }

                    }
                }

                //object PlantResult;
                List<ShiftPlantData> PlantResult = new List<ShiftPlantData>();

                List<ShopWisespec_Data> Process_data = new List<ShopWisespec_Data>();
                if (startdate1.Month > 3)
                {
                    CurYear = startdate1.Year.ToString();
                    var next = startdate1.AddYears(1);
                    NexYear = next.Year.ToString();

                    FinYear = CurYear + "-" + NexYear;
                }
                else
                {
                    CurYear = startdate1.Year.ToString();
                    var pre = startdate1.AddYears(-1);
                    PreYear = pre.Year.ToString();
                    //CurYear = CurrentYear.ToString();
                    FinYear = PreYear + "-" + CurYear;
                }
                // current month 
                string month = startdate1.ToString("MMMM");
                // target list 
                var Target1 = (from t in db.MM_PowerTarget
                               join
                                s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                               where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && s.Spec_Unit_ID != 5 && s.Energy ==true
                               select new
                               {
                                   t.Target,
                                   s.Shop_Name,
                                   s.Shop_ID
                               }).ToList();
                var PlantTarget1 = (from t in db.MM_PlantTarget
                                    where t.Year == FinYear && t.Month == month
                                    select new
                                    {
                                        t.Target
                                    }).FirstOrDefault();

                if (Target1.Count() == 0)
                {
                    CurYear = startdate1.Year.ToString();
                    var pre = startdate1.AddYears(-1);

                    PreYear = pre.Year.ToString();
                    //CurYear = CurrentYear.ToString();
                    FinYear = PreYear + "-" + CurYear;
                    Target1 = (from t in db.MM_PowerTarget
                               join
                                s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                               where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && s.Spec_Unit_ID != 5 && s.Energy ==true
                               select new
                               {
                                   t.Target,
                                   s.Shop_Name,
                                   s.Shop_ID
                               }).ToList();
                }
                string[] HolidayAarry = { };

                if (Noworking != "")
                {
                    HolidayAarry = Noworking.Split(',').ToArray();
                    List<Performance_Indices> ShopResult = new List<Performance_Indices>();
                    foreach (var item in HolidayAarry)
                    {

                        int Reason_ID = Convert.ToInt32(item);

                        var noworkingday = (from n in db.MM_No_Working_Day
                                            where n.Reason_ID == Reason_ID && n.Day_Date >= startdate1 && n.Day_Date <= enddate1
                                            select new
                                            {
                                                n.Day_Date
                                            }).ToList();
                        if (noworkingday.Count() > 0)

                        {

                            foreach (var id in noworkingday.OrderBy(n => n.Day_Date))
                            {
                                if (Shiftwise != null)
                                {
                                    var noworking = shiftPerform.Where(x => x.Shop_ID == 0 && Convert.ToDateTime(x.Dateandtime).Date == (id.Day_Date).Date).FirstOrDefault();

                                    if (noworking != null)
                                    {
                                        ShiftPlantData obj = new ShiftPlantData(Convert.ToDouble(noworking.SpecConsumption),
                                          Convert.ToDouble(noworking.Consumption), Convert.ToInt32(noworking.Production), 0.0, id.Day_Date.ToString("dd/MMM/yy"));
                                        PlantResult.Add(obj);
                                        string ddl = Convert.ToDateTime(noworking.Dateandtime).ToString("dd/MMM/yy");
                                        allDates.Add(ddl);

                                    }
                                    foreach (var id1 in shiftPerform.Where(x => x.Shop_ID != 0 && Convert.ToDateTime(x.Dateandtime).Date == (id.Day_Date).Date))
                                    {



                                        Boolean Is_Consumption = true;
                                        Is_Production = true;

                                        var shop = (from s in db.MM_MTTUW_Shops
                                                    join
                                                    g in db.MM_ShopsCategory on
                                                    s.ShopsCat_ID equals g.ShopsCat_ID
                                                    where s.Shop_ID == id1.Shop_ID && s.Energy ==true
                                                    select new
                                                    {
                                                        s.Shop_Name,
                                                        s.ShopsCat_ID,
                                                        g.ShopsCategory_Name,
                                                        s.Business_ID,
                                                        g.Is_Consumption,
                                                        s.Spec_Unit_ID

                                                    }).FirstOrDefault();

                                        if (shop != null)
                                        {
                                            groupId = Convert.ToInt32(shop.ShopsCat_ID);
                                            int busniesId = Convert.ToInt32(shop.Business_ID);
                                            shopName = shop.Shop_Name;
                                            groupName = shop.ShopsCategory_Name;
                                            total = Convert.ToDouble(id1.Consumption);
                                            var pro_consum = Convert.ToDouble(id1.SpecConsumption);
                                            int pro_Production = Convert.ToInt32(id1.Production);
                                            Is_Consumption = Convert.ToBoolean(shop.Is_Consumption);
                                            if (shop.Spec_Unit_ID == 5)
                                            {
                                                Is_Production = false;
                                            }

                                            ShopWiseConsumption obj1 = new ShopWiseConsumption(groupId, Convert.ToInt32(id1.Shop_ID), shopName, groupName, total);
                                            ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(groupId), groupName, total, pro_Production, pro_consum);
                                            ShopWisespec_Data bus_Data = new ShopWisespec_Data(busniesId, shopName, total, pro_Production, pro_consum);
                                            Performance_Indices obj = new Performance_Indices(shop.Shop_Name, pro_consum, 0.0, 0.0, 0.0, total, pro_Production, Convert.ToInt32(id1.Shop_ID), Is_Consumption, Is_Production);
                                            ShopResult.Add(obj);
                                            consumptionwise3.Add(obj1);
                                            Process.Add(Pro_Data);
                                            busniess.Add(bus_Data);
                                        }
                                    }

                                }
                                else
                                {
                                    var noworking = Perform.Where(x => x.Shop_ID == 0 && x.DateandTime == id.Day_Date).FirstOrDefault();
                                    if (noworking != null)
                                    {
                                        ShiftPlantData obj = new ShiftPlantData(Convert.ToDouble(noworking.Consumption),
                                           Convert.ToDouble(noworking.TotalConsumption), Convert.ToInt32(noworking.Production),
                                            Convert.ToDouble(noworking.Best), id.Day_Date.ToString("dd/MMM/yy"));
                                        PlantResult.Add(obj);
                                        string ddl = Convert.ToDateTime(noworking.DateandTime).ToString("dd/MMM/yy");
                                        allDates.Add(ddl);

                                    }
                                    foreach (var id1 in Perform.Where(s => s.Shop_ID != 0 && s.DateandTime == id.Day_Date))
                                    {



                                        Boolean Is_Consumption = true;
                                        Is_Production = true;

                                        var shop = (from s in db.MM_MTTUW_Shops
                                                    join
                                                    g in db.MM_ShopsCategory on
                                                    s.ShopsCat_ID equals g.ShopsCat_ID
                                                    where s.Shop_ID == id1.Shop_ID && s.Energy ==true
                                                    select new
                                                    {
                                                        s.Shop_Name,
                                                        s.ShopsCat_ID,
                                                        g.ShopsCategory_Name,
                                                        s.Business_ID,
                                                        g.Is_Consumption,
                                                        s.Spec_Unit_ID
                                                    }).FirstOrDefault();

                                        if (shop != null)
                                        {
                                            groupId = Convert.ToInt32(shop.ShopsCat_ID);
                                            int busniesId = Convert.ToInt32(shop.Business_ID);
                                            shopName = shop.Shop_Name;
                                            groupName = shop.ShopsCategory_Name;
                                            total = Convert.ToDouble(id1.TotalConsumption);
                                            var pro_consum = Convert.ToDouble(id1.Consumption);
                                            int pro_Production = Convert.ToInt32(id1.Production);
                                            var Best = Convert.ToDouble(id1.Best);
                                            var Avg = Convert.ToDouble(id1.Average);
                                            var Effiency = Convert.ToDouble(id1.Efficiency);
                                            Is_Consumption = Convert.ToBoolean(shop.Is_Consumption);
                                            if (shop.Spec_Unit_ID == 5)
                                            {
                                                Is_Production = false;
                                            }

                                            ShopWiseConsumption obj1 = new ShopWiseConsumption(groupId, Convert.ToInt32(id1.Shop_ID), shopName, groupName, total);
                                            ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(groupId), groupName, total, pro_Production, pro_consum);
                                            ShopWisespec_Data bus_Data = new ShopWisespec_Data(busniesId, shopName, total, pro_Production, pro_consum);
                                            Performance_Indices obj = new Performance_Indices(shop.Shop_Name, pro_consum, Best, Avg, Effiency, total, pro_Production, Convert.ToInt32(id1.Shop_ID), Is_Consumption, Is_Production);
                                            ShopResult.Add(obj);
                                            consumptionwise3.Add(obj1);
                                            Process.Add(Pro_Data);
                                            busniess.Add(bus_Data);
                                        }
                                    }

                                }



                            }


                            var Shopnwise1 = ShopResult.GroupBy(c => c.ShopId).Select(c => new { totalconsumption = c.Sum(b => b.TotalConsumtion), Production = c.Sum(b => b.Production), Shop_Id = c.Key, }).ToList();
                            foreach (var id2 in Shopnwise1.Where(s => s.Shop_Id != 0).OrderByDescending(s => s.totalconsumption))
                            {
                                var ShopName1 = (from s in db.MM_MTTUW_Shops
                                                 join
                           g in db.MM_ShopsCategory on
                           s.ShopsCat_ID equals g.ShopsCat_ID

                                                 where s.Shop_ID == id2.Shop_Id && s.Energy ==true
                                                 select new
                                                 {
                                                     s.Shop_Name,
                                                     g.Is_Consumption,
                                                     s.Spec_Unit_ID
                                                 }).FirstOrDefault();
                                Boolean Is_Consumption = true;
                                Is_Production = true;
                                double Consumptionengine = 0.0;
                                double Best = 0.0;
                                double Avg = 0.0;
                                double Effiency = 0.0;
                                double Consumption = Convert.ToDouble(id2.totalconsumption);
                                int Production = Convert.ToInt32(id2.Production);
                                int ShopID = Convert.ToInt32(id2.Shop_Id);
                                if (Production != 0) Consumptionengine = Math.Round((Consumption) / Production, 0);
                                Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                                if (ShopName1.Spec_Unit_ID == 5)
                                {
                                    Is_Production = false;
                                }
                                Performance_Indices obj = new Performance_Indices(ShopName1.Shop_Name, Consumptionengine, Best, Avg, Effiency, Consumption, Production, ShopID, Is_Consumption, Is_Production);
                                ShopwisePerformace.Add(obj);
                            }
                            var consumptionwise1 = consumptionwise3.GroupBy(c => c.ShopsCat_ID).Select(c => new { totalconsumption = c.Sum(b => b.totalconsumption), ShopsCat_ID = c.Key, }).ToList();
                            var Shopgroups = (from shop in db.MM_MTTUW_Shops
                                              join shopgroup in db.MM_ShopsCategory
                                              on shop.ShopsCat_ID equals shopgroup.ShopsCat_ID
                                              //join consum in consumptionwise1
                                              //on shopgroup.ShopsCat_ID equals consum.ShopsCat_ID
                                              where shop.Plant_ID == plantID && shop.Energy ==true
                                              select shopgroup
                                                 ).ToList().Distinct();
                            foreach (var id3 in Shopgroups)
                            {
                                double Consumption = 0.0;
                                var Total = Convert.ToDouble(Process.Where(dc => dc.dataId == id3.ShopsCat_ID).Sum(dc => dc.totalconsumption));
                                var Production = Convert.ToInt32(Process.Where(dc => dc.dataId == id3.ShopsCat_ID).Sum(dc => dc.Production));
                                if (Production != 0)
                                    Consumption = Math.Round((Total / Production), 0);
                                ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(id3.ShopsCat_ID), id3.ShopsCategory_Name, Total, Production, Consumption);
                                Process_data.Add(Pro_Data);
                            }



                        }
                    }
                }
                else
                {
                    if (Minute != null)
                    {
                        foreach (var data in PlantsConsumption)
                        {
                            ShiftPlantData obj = new ShiftPlantData(
                                  Convert.ToDouble(data.Consumption),
                                  Convert.ToDouble(data.TotalConsumption),
                                 Convert.ToInt32(data.Production), Convert.ToDouble(data.Best), data.Time);

                            PlantResult.Add(obj);

                        }

                    }
                    else
                    {
                        if (Shiftwise != null)
                        {
                            //PlantResult = shiftPerform.Where(s => s.Shop_ID == 0).ToList();

                            //PlantResult = shiftPerform.Where(x => x.Shop_ID == 0)
                            //  .Select(x => new ShiftPlantData
                            //  {
                            //      TotalConsumption = Convert.ToDouble(x.Consumption),
                            //      Production = Convert.ToInt32(x.Production),
                            //      Consumption = Convert.ToDouble(x.SpecConsumption)
                            //  }).ToList();
                            foreach (var data in shiftPerform.Where(s => s.Shop_ID == 0))
                            {
                                double bestdata = 0;
                                string ddl = Convert.ToDateTime(data.Dateandtime).ToString("dd/MMM/yy");
                                ShiftPlantData obj = new ShiftPlantData(
                                    Convert.ToDouble(data.SpecConsumption),
                                    Convert.ToDouble(data.Consumption),
                                   Convert.ToInt32(data.Production), bestdata, ddl);

                                PlantResult.Add(obj);
                            }

                            foreach (var id in shiftPerform.Where(s => s.Shop_ID == 0))
                            {
                                string ddl = Convert.ToDateTime(id.Dateandtime).ToString("dd/MMM/yy");
                                allDates.Add(ddl);
                            }
                        }
                        else
                        {
                            //PlantResult = Perform.Where(s => s.Shop_ID == 0).ToList();
                            foreach (var data in Perform.Where(s => s.Shop_ID == 0))
                            {
                                double bestdata = Convert.ToDouble(data.Best);
                                string ddl = Convert.ToDateTime(data.DateandTime).ToString("dd/MMM/yy");
                                ShiftPlantData obj = new ShiftPlantData(
                                    Convert.ToDouble(data.Consumption),
                                    Convert.ToDouble(data.TotalConsumption),
                                   Convert.ToInt32(data.Production), bestdata, ddl);

                                PlantResult.Add(obj);
                            }
                            foreach (var id in Perform.Where(s => s.Shop_ID == 0))
                            {
                                string ddl = Convert.ToDateTime(id.DateandTime).ToString("dd/MMM/yy");
                                allDates.Add(ddl);
                            }
                        }


                    }

                    var today = System.DateTime.Now;
                    if (ddlformat != "1" && (startdate1.Date != today.Date))
                    {

                        if (Shiftwise != null)
                        {
                            foreach (var id in shiftPerform.Where(s => s.Shop_ID != 0))
                            {




                                var shop = (from s in db.MM_MTTUW_Shops
                                            join
                                            g in db.MM_ShopsCategory on
                                            s.ShopsCat_ID equals g.ShopsCat_ID
                                            where s.Shop_ID == id.Shop_ID && s.Energy ==true
                                            select new
                                            {
                                                s.Shop_Name,
                                                s.ShopsCat_ID,
                                                g.ShopsCategory_Name,
                                                s.Business_ID,

                                            }).FirstOrDefault();

                                if (shop != null)
                                {
                                    groupId = Convert.ToInt32(shop.ShopsCat_ID);
                                    int busniesId = Convert.ToInt32(shop.Business_ID);
                                    shopName = shop.Shop_Name;
                                    groupName = shop.ShopsCategory_Name;
                                    total = Convert.ToDouble(id.Consumption);
                                    var pro_consum = Convert.ToDouble(id.SpecConsumption);
                                    int pro_Production = Convert.ToInt32(id.Production);

                                    ShopWiseConsumption obj1 = new ShopWiseConsumption(groupId, Convert.ToInt32(id.Shop_ID), shopName, groupName, total);
                                    ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(groupId), groupName, total, pro_Production, pro_consum);
                                    ShopWisespec_Data bus_Data = new ShopWisespec_Data(busniesId, shopName, total, pro_Production, pro_consum);
                                    consumptionwise3.Add(obj1);
                                    Process.Add(Pro_Data);
                                    busniess.Add(bus_Data);
                                }

                            }
                        }
                        else
                        {
                            foreach (var id in Perform.Where(s => s.Shop_ID != 0))
                            {




                                var shop = (from s in db.MM_MTTUW_Shops
                                            join
                                            g in db.MM_ShopsCategory on
                                            s.ShopsCat_ID equals g.ShopsCat_ID
                                            where s.Shop_ID == id.Shop_ID && s.Energy == true
                                            select new
                                            {
                                                s.Shop_Name,
                                                s.ShopsCat_ID,
                                                g.ShopsCategory_Name,
                                                s.Business_ID,

                                            }).FirstOrDefault();

                                if (shop != null)
                                {
                                    groupId = Convert.ToInt32(shop.ShopsCat_ID);
                                    int busniesId = Convert.ToInt32(shop.Business_ID);
                                    shopName = shop.Shop_Name;
                                    groupName = shop.ShopsCategory_Name;
                                    total = Convert.ToDouble(id.TotalConsumption);
                                    var pro_consum = Convert.ToDouble(id.Consumption);
                                    int pro_Production = Convert.ToInt32(id.Production);

                                    ShopWiseConsumption obj1 = new ShopWiseConsumption(groupId, Convert.ToInt32(id.Shop_ID), shopName, groupName, total);
                                    ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(groupId), groupName, total, pro_Production, pro_consum);
                                    ShopWisespec_Data bus_Data = new ShopWisespec_Data(busniesId, shopName, total, pro_Production, pro_consum);
                                    consumptionwise3.Add(obj1);
                                    Process.Add(Pro_Data);
                                    busniess.Add(bus_Data);
                                }

                            }
                        }

                    }
                    var consumptionwise1 = consumptionwise3.GroupBy(c => c.ShopsCat_ID).Select(c => new { totalconsumption = c.Sum(b => b.totalconsumption), ShopsCat_ID = c.Key, }).ToList();
                    var Shopgroups = (from shop in db.MM_MTTUW_Shops
                                      join shopgroup in db.MM_ShopsCategory
                                      on shop.ShopsCat_ID equals shopgroup.ShopsCat_ID
                                      //join consum in consumptionwise1
                                      //on shopgroup.ShopsCat_ID equals consum.ShopsCat_ID
                                      where shop.Plant_ID == plantID && shop.Energy == true
                                      select shopgroup
                                         ).ToList().Distinct();
                    var res = Shopgroups.OrderBy(s => s.Sort_Order).Select(c => new ShopWiseConsumption1
                    {

                        ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),

                        ShopsCategory_Name = c.ShopsCategory_Name,
                        totalconsumption = Convert.ToDouble(consumptionwise3.Where(dc => dc.ShopsCat_ID == c.ShopsCat_ID).Sum(dc => dc.totalconsumption))
                    }).Distinct().ToList();








                    if (ddlformat == "1" || (ddlformat == "5" && startdate1.Date == today.Date))
                    {
                        var Shopnwise1 = ShopConsumption.GroupBy(c => c.Shop_ID).Select(c => new { totalconsumption = c.Sum(b => b.TotalConsumption), Production = c.Sum(b => b.Production), Shop_Id = c.Key, }).ToList();
                        foreach (var id in Shopnwise1.Where(s => s.Shop_Id != 0).OrderByDescending(s => s.totalconsumption))
                        {
                            Boolean Is_Consumption = true;
                            Is_Production = true;
                            var ShopName1 = (from s in db.MM_MTTUW_Shops
                                             join
                       g in db.MM_ShopsCategory on
                       s.ShopsCat_ID equals g.ShopsCat_ID
                                             where s.Shop_ID == id.Shop_Id && s.Energy == true
                                             select new
                                             {
                                                 s.Shop_Name,
                                                 g.Is_Consumption,
                                                 s.Spec_Unit_ID
                                             }).FirstOrDefault();

                            double Consumptionengine = 0.0;
                            double Best = 0.0;
                            double Avg = 0.0;
                            double Effiency = 0.0;
                            double Consumption = Convert.ToDouble(id.totalconsumption);
                            int Production = Convert.ToInt32(id.Production);
                            int ShopID = Convert.ToInt32(id.Shop_Id);
                            if (Production != 0) Consumptionengine = Math.Round((Consumption) / Production, 0);
                            Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                            if (ShopName1.Spec_Unit_ID == 5)
                            {
                                Is_Production = false;
                            }
                            Performance_Indices obj = new Performance_Indices(ShopName1.Shop_Name, Consumptionengine, Best, Avg, Effiency, Consumption, Production, ShopID, Is_Consumption, Is_Production);
                            ShopwisePerformace.Add(obj);
                        }
                    }
                    else
                    {
                        if (Shiftwise != null)
                        {
                            var Shopnwise1 = shiftPerform.GroupBy(c => c.Shop_ID).Select(c => new { totalconsumption = c.Sum(b => b.Consumption), Production = c.Sum(b => b.Production), Shop_Id = c.Key, }).ToList();
                            foreach (var id in Shopnwise1.Where(s => s.Shop_Id != 0).OrderByDescending(s => s.totalconsumption))
                            {
                                Boolean Is_Consumption = true;
                                Is_Production = true;
                                var ShopName1 = (from s in db.MM_MTTUW_Shops
                                                 join
                                                 g in db.MM_ShopsCategory on
                                                 s.ShopsCat_ID equals g.ShopsCat_ID
                                                 where s.Shop_ID == id.Shop_Id && s.Energy == true
                                                 select new
                                                 {
                                                     s.Shop_Name,
                                                     g.Is_Consumption,
                                                     s.Spec_Unit_ID
                                                 }).FirstOrDefault();

                                double Consumptionengine = 0.0;
                                double Best = 0.0;
                                double Avg = 0.0;
                                double Effiency = 0.0;
                                double Consumption = Convert.ToDouble(id.totalconsumption);
                                int Production = Convert.ToInt32(id.Production);
                                int ShopID = Convert.ToInt32(id.Shop_Id);
                                if (Production != 0) Consumptionengine = Math.Round((Consumption) / Production, 0);
                                Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                                if (ShopName1.Spec_Unit_ID == 5)
                                {
                                    Is_Production = false;
                                }
                                Performance_Indices obj = new Performance_Indices(ShopName1.Shop_Name, Consumptionengine, Best, Avg, Effiency, Consumption, Production, ShopID, Is_Consumption, Is_Production);
                                ShopwisePerformace.Add(obj);
                            }
                        }
                        else
                        {
                            var Shopnwise1 = Perform.GroupBy(c => c.Shop_ID).Select(c => new { totalconsumption = c.Sum(b => b.TotalConsumption), Production = c.Sum(b => b.Production), Shop_Id = c.Key, }).ToList();
                            foreach (var id in Shopnwise1.Where(s => s.Shop_Id != 0).OrderByDescending(s => s.totalconsumption))
                            {
                                Boolean Is_Consumption = true;
                                Is_Production = true;
                                var ShopName1 = (from s in db.MM_MTTUW_Shops
                                                 join
                                                 g in db.MM_ShopsCategory on
                                                 s.ShopsCat_ID equals g.ShopsCat_ID
                                                 where s.Shop_ID == id.Shop_Id && s.Energy == true
                                                 select new
                                                 {
                                                     s.Shop_Name,
                                                     g.Is_Consumption,
                                                     s.Spec_Unit_ID
                                                 }).FirstOrDefault();

                                double Consumptionengine = 0.0;
                                double Best = 0.0;
                                double Avg = 0.0;
                                double Effiency = 0.0;
                                double Consumption = Convert.ToDouble(id.totalconsumption);
                                int Production = Convert.ToInt32(id.Production);
                                int ShopID = Convert.ToInt32(id.Shop_Id);
                                if (Consumption > 0)
                                {
                                    if (Production != 0) Consumptionengine = Math.Round((Consumption) / Production, 0);
                                }
                                Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                                if (ShopName1.Spec_Unit_ID == 5)
                                {
                                    Is_Production = false;
                                }
                                Performance_Indices obj = new Performance_Indices(ShopName1.Shop_Name, Consumptionengine, Best, Avg, Effiency, Consumption, Production, ShopID, Is_Consumption, Is_Production);
                                ShopwisePerformace.Add(obj);
                            }
                        }

                    }
                    foreach (var id3 in Shopgroups)
                    {
                        double Consumption = 0.0;
                        var Total = Convert.ToDouble(Process.Where(dc => dc.dataId == id3.ShopsCat_ID).Sum(dc => dc.totalconsumption));
                        var Production = Convert.ToInt32(Process.Where(dc => dc.dataId == id3.ShopsCat_ID).Sum(dc => dc.Production));
                        if (Production != 0)
                            Consumption = Math.Round((Total / Production), 0);
                        ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(id3.ShopsCat_ID), id3.ShopsCategory_Name, Total, Production, Consumption);
                        Process_data.Add(Pro_Data);
                    }
                }

                if (Target1.Count() == 0)
                {
                    for (int i = 0; i < ShopwisePerformace.Count(); i++)
                    {
                        Target.Add(0);
                    }
                }
                else
                {
                    for (int i = 0; i < ShopwisePerformace.Count(); i++)
                    {
                        int j = 0;
                        for (int k = 0; k < Target1.Count(); k++)
                        {
                            j = k;
                            if (ShopwisePerformace[i].ShopId == Target1[k].Shop_ID)
                            {
                                if (Target1[k].Target == 0)
                                {
                                    Target.Add(0);
                                    break;
                                }
                                else
                                {
                                    Target.Add(System.Math.Round(Convert.ToDouble(Target1[k].Target), 0));
                                    break;

                                }

                            }

                        }
                        if (j == (Target1.Count() - 1))
                        {
                            Target.Add(0);
                        }

                    }
                }




                List<ShopWisespec_Data> Business_data = new List<ShopWisespec_Data>();
                foreach (var id in db.MM_Business)
                {
                    double Consumption = 0.0;
                    var Total = Convert.ToDouble(busniess.Where(dc => dc.dataId == id.Business_Id).Sum(dc => dc.totalconsumption));
                    var Production = Convert.ToInt32(busniess.Where(dc => dc.dataId == id.Business_Id).Sum(dc => dc.Production));
                    if (Production != 0)
                        Consumption = Math.Round((Total / Production), 0);
                    ShopWisespec_Data bus_Data = new ShopWisespec_Data(Convert.ToInt32(id.Business_Id), id.Business_Name, Total, Production, Consumption);
                    Business_data.Add(bus_Data);
                }
                foreach (var id in allDates)
                {
                    if (PlantTarget1 == null)
                    {
                        PlantTarget.Add(null);
                    }
                    else
                    {
                        PlantTarget.Add(PlantTarget1.Target);
                    }

                }
                foreach (var id in Category)
                {
                    Barchart obj3 = new Barchart(id.Category_Name, Convert.ToDouble(id.Comsumtionvalues), "Add", 1);
                    CategoryData.Add(obj3);
                }

                var Consumeunit = "";


                if (type == true)
                {
                    Consumeunit = db.MM_Parameter.Where(s => s.Prameter_ID == 1).Select(s => s.Unit).FirstOrDefault();

                }
                else
                {
                    Consumeunit = "KVAH";
                }


                List<Summary> PlantSummary = new List<Summary>();
                List<double?> Cumulative = new List<double?>();
                List<double?> Averge = new List<double?>();
                double best = 0;
                double cumulativedata = 0;
                double avgdata = 0.0;
                int avg = 1;
                int bestprod = 0;
                var bestdate = "";
                double bestpower = 0.0;
                double maxpower = 0.0;
                var maxdate = "";
                if (Similar == true)
                {
                    var SmilierPlantResult = PlantResult;


                    var PlantSimilercount = PlantResult.GroupBy(s => s.Production).OrderBy(g => g.Count()).
                                           Select(g => new { Name = g.Key, Count = g.Count() > 1 }).ToList();

                    var Similercount = PlantSimilercount.Where(s => s.Count == true && s.Name != 0).ToList();
                    PlantResult = new List<ShiftPlantData>();
                    allDates = new List<string>();
                    foreach (var id in Similercount)
                    {
                        var data = SmilierPlantResult.Where(s => s.Production == id.Name).ToList();


                        foreach (var samedata in data)
                        {
                            if (Minute != null)
                            {
                                ShiftPlantData obj = new ShiftPlantData(
                                  Convert.ToDouble(samedata.Consumption),
                                  Convert.ToDouble(samedata.TotalConsumption),
                                 Convert.ToInt32(samedata.Production), Convert.ToDouble(samedata.Best), samedata.date);

                                PlantResult.Add(obj);
                                allDates.Add(samedata.date);

                            }
                            else
                            {
                                ShiftPlantData obj = new ShiftPlantData(
                                    Convert.ToDouble(samedata.Consumption),
                                    Convert.ToDouble(samedata.TotalConsumption),
                                   Convert.ToInt32(samedata.Production), Convert.ToDouble(samedata.Best), samedata.date);

                                PlantResult.Add(obj);
                                allDates.Add(samedata.date);

                            }
                        }
                    }

                }


                var best1Data = 0.0;
                foreach (var id in PlantResult)
                {
                    string ddl = id.date;
                    //allDates.Add(ddl);
                    if (best1Data == id.Best)
                    {
                        BestData.Add(null);
                    }
                    else
                    {
                        best1Data = Convert.ToDouble(id.Best);
                        BestData.Add(best1Data);
                    }
                    if (id.Consumption != 0)
                    {
                        if (best == 0)
                        {
                            best = Convert.ToDouble(id.Best);
                            bestdate = ddl;
                            bestprod = Convert.ToInt32(id.Production);
                            bestpower = Convert.ToDouble(id.TotalConsumption);

                        }
                        else if (id.Best < best)
                        {
                            best = Convert.ToDouble(id.Best);
                            bestdate = ddl;
                            bestprod = Convert.ToInt32(id.Production);
                            bestpower = Convert.ToDouble(id.TotalConsumption);
                        }


                    }
                    if (maxpower == 0)
                    {
                        maxpower = Convert.ToDouble(id.TotalConsumption);
                        maxdate = ddl;
                    }
                    else if (id.TotalConsumption > maxpower)
                    {
                        maxpower = Convert.ToDouble(id.TotalConsumption);
                        maxdate = ddl;
                    }
                    cumulativedata += id.TotalConsumption;

                    avgdata = Math.Round(Convert.ToDouble(cumulativedata / avg), 0);
                    Cumulative.Add(cumulativedata);
                    Averge.Add(avgdata);
                    avg += 1;


                }
                var totalconsume = Math.Round(PlantResult.Sum(s => s.TotalConsumption), 0);
                if (PlantResult.Count() > 0)
                {


                    var totalavg = Math.Round(PlantResult.Average(s => s.TotalConsumption), 0);
                    var totalprod = PlantResult.Sum(s => s.Production);
                    var totalavgsec = Math.Round(PlantResult.Average(s => s.Consumption), 0);
                    Summary summaryobj = new Summary(totalconsume, totalavg, totalavgsec, totalprod, best, bestpower, bestprod, maxpower, bestdate, maxdate);
                    PlantSummary.Add(summaryobj);
                }



                List<Shopwise_SEC> SEC = new List<Shopwise_SEC>();
                List<List<Shopwise_SEC>> Shopwise_SEC = new List<List<Shopwise_SEC>>();
                if (Formdate != todate)
                {
                    var shops = db.MM_MTTUW_Shops.Where(s => s.Spec_Unit_ID != 5 && s.Spec_Unit_ID != 4 && s.Energy == true).ToList();
                    var unit = "kVAh/SEC";

                    if (type)
                    {
                        unit = "kWh/SEC";
                    }
                    if (Shiftwise != null)
                    {
                        foreach (var shop in shops)
                        {
                            SEC = new List<Shopwise_SEC>();
                            foreach (var id in shiftPerform.Where(s => s.Shop_ID == shop.Shop_ID).OrderBy(s => s.Dateandtime))
                            {
                                var date = Convert.ToDateTime(id.Dateandtime).ToString("dd/MMM/yy");
                                Shopwise_SEC obj = new Shopwise_SEC(shop.Shop_Name, date, Convert.ToDouble(id.Consumption), Convert.ToInt32(id.Shop_ID), unit);
                                SEC.Add(obj);
                            }
                            Shopwise_SEC.Add(SEC);

                        }
                    }
                    else
                    {

                        foreach (var shop in shops)
                        {
                            SEC = new List<Shopwise_SEC>();
                            foreach (var id in Perform.Where(s => s.Shop_ID == shop.Shop_ID).OrderBy(s => s.DateandTime))
                            {
                                var date = Convert.ToDateTime(id.DateandTime).ToString("dd/MMM/yy");
                                Shopwise_SEC obj = new Shopwise_SEC(shop.Shop_Name, date, Convert.ToDouble(id.Consumption), Convert.ToInt32(id.Shop_ID), unit);
                                SEC.Add(obj);
                            }
                            Shopwise_SEC.Add(SEC);

                        }

                    }
                }



                return Json(new { Business_data, Process_data, PlantResult, Target, ShopwisePerformace, allDates, ddlformat, PlantTarget, CategoryData, Consumeunit, PlantSummary, starts, ends, Cumulative, Averge, BestData, Shopwise_SEC }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult SubShopwisedata(string startdate, string ConsumptionId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID ==25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).First();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());


                List<Performance_Indices> ShopwisePerformace = new List<Performance_Indices>();
                List<Barchart> CategoryData = new List<Barchart>();
                List<ShopWisespec_Data> Business_data = new List<ShopWisespec_Data>();
                List<ShopWisespec_Data> Process_data = new List<ShopWisespec_Data>();

                List<ShopWisespec_Data> Process = new List<ShopWisespec_Data>();
                List<ShopWisespec_Data> busniess = new List<ShopWisespec_Data>();

                List<string> allDates = new List<string>();
                DateTime startdate1 = DateTime.Parse(startdate);
                DateTime enddate1 = DateTime.Parse(startdate);

                enddate1 = enddate1.AddDays(1);
                List<ShopWiseConsumption> consumptionwise3 = new List<ShopWiseConsumption>();
                var Perform = (from p in db.MM_Performance_Indices_Energy
                               where p.DateandTime == startdate1 && p.ConsumptionType == type && p.Shop_ID != 0
                               select new
                               {
                                   p.Shop_ID,
                                   p.Consumption,
                                   p.Best,
                                   p.Average,
                                   p.Efficiency,
                                   p.Production,
                                   p.TotalConsumption,
                                   p.DateandTime
                               }).OrderBy(s => s.Shop_ID).ToList();
                foreach (var id in Perform.OrderByDescending(s => s.TotalConsumption))
                {
                    string ShopName;
                    Boolean Generation = true;
                    if (id.Shop_ID == 0)
                    {
                        ShopName = "Plant";
                        Generation = true;
                    }
                    else
                    {
                        var ShopName1 = (from s in db.MM_MTTUW_Shops
                                         join
                                         g in db.MM_ShopsCategory on
                                         s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == id.Shop_ID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_Name,
                                             g.Is_Consumption,
                                             s.Spec_Unit_ID
                                         }).FirstOrDefault();
                        ShopName = ShopName1.Shop_Name;
                        Generation = Convert.ToBoolean(ShopName1.Is_Consumption);
                        Is_Production = true;
                        if (ShopName1.Spec_Unit_ID == 5)
                        {
                            Is_Production = false;
                        }
                    }

                    double Consumptionengine = Convert.ToDouble(id.Consumption);
                    double Best = Convert.ToDouble(id.Best);
                    double Avg = Convert.ToDouble(id.Average);
                    double Effiency = Convert.ToDouble(id.Efficiency);
                    double Consumption = Convert.ToDouble(id.TotalConsumption);
                    int Production = Convert.ToInt32(id.Production);
                    int ShopID = Convert.ToInt32(id.Shop_ID);

                    Performance_Indices obj = new Performance_Indices(ShopName, Consumptionengine, Best, Avg, Effiency, Consumption, Production, ShopID, Generation, Is_Production);

                    if (id.Shop_ID != 0)
                    {


                        ShopwisePerformace.Add(obj);
                        var shop = (from s in db.MM_MTTUW_Shops
                                    join
                                     g in db.MM_ShopsCategory on
                                    s.ShopsCat_ID equals g.ShopsCat_ID
                                    where s.Shop_ID == id.Shop_ID && s.Energy == true
                                    select new
                                    {
                                        s.Shop_Name,
                                        s.ShopsCat_ID,
                                        g.ShopsCategory_Name,
                                        s.Business_ID,

                                    }).FirstOrDefault();
                        int groupId = Convert.ToInt32(shop.ShopsCat_ID);
                        int busniesId = Convert.ToInt32(shop.Business_ID);
                        var shopName = shop.Shop_Name;
                        var groupName = shop.ShopsCategory_Name;
                        var total = Convert.ToDouble(id.TotalConsumption);
                        var pro_consum = Convert.ToDouble(id.Consumption);
                        int pro_Production = Convert.ToInt32(id.Production);

                        ShopWiseConsumption obj1 = new ShopWiseConsumption(groupId, Convert.ToInt32(id.Shop_ID), shopName, groupName, total);
                        ShopWisespec_Data Pro_Data = new ShopWisespec_Data(groupId, groupName, total, pro_Production, pro_consum);
                        ShopWisespec_Data bus_Data = new ShopWisespec_Data(busniesId, shopName, total, pro_Production, pro_consum);
                        consumptionwise3.Add(obj1);
                        Process.Add(Pro_Data);
                        busniess.Add(bus_Data);
                    }



                }

                var consumptionwise1 = consumptionwise3.GroupBy(c => c.ShopsCat_ID).Select(c => new { totalconsumption = c.Sum(b => b.totalconsumption), ShopsCat_ID = c.Key, }).ToList();
                var Shopgroups = (from shop in db.MM_MTTUW_Shops
                                  join shopgroup in db.MM_ShopsCategory
                                  on shop.ShopsCat_ID equals shopgroup.ShopsCat_ID
                                  //join consum in consumptionwise1
                                  //on shopgroup.ShopsCat_ID equals consum.ShopsCat_ID
                                  where shop.Plant_ID == plantID && shop.Energy == true
                                  select shopgroup
                                     ).ToList().Distinct();
                var res = Shopgroups.OrderBy(s => s.Sort_Order).Select(c => new ShopWiseConsumption1
                {

                    ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),

                    ShopsCategory_Name = c.ShopsCategory_Name,

                    totalconsumption = Convert.ToDouble(consumptionwise3.Where(dc => dc.ShopsCat_ID == c.ShopsCat_ID).Sum(dc => dc.totalconsumption))
                }).Distinct().ToList();

                foreach (var id in Shopgroups)
                {
                    double Consumption = 0.0;
                    var Total = Convert.ToDouble(Process.Where(dc => dc.dataId == id.ShopsCat_ID).Sum(dc => dc.totalconsumption));
                    var Production = Convert.ToInt32(Process.Where(dc => dc.dataId == id.ShopsCat_ID).Sum(dc => dc.Production));
                    if (Production != 0)
                        Consumption = Math.Round((Total / Production), 0);
                    ShopWisespec_Data Pro_Data = new ShopWisespec_Data(Convert.ToInt32(id.ShopsCat_ID), id.ShopsCategory_Name, Total, Production, Consumption);
                    Process_data.Add(Pro_Data);
                }

                foreach (var id in db.MM_Business)
                {
                    double Consumption = 0.0;
                    var Total = Convert.ToDouble(busniess.Where(dc => dc.dataId == id.Business_Id).Sum(dc => dc.totalconsumption));
                    var Production = Convert.ToInt32(busniess.Where(dc => dc.dataId == id.Business_Id).Sum(dc => dc.Production));
                    if (Production != 0)
                        Consumption = Math.Round((Total / Production), 0);
                    ShopWisespec_Data bus_Data = new ShopWisespec_Data(Convert.ToInt32(id.Business_Id), id.Business_Name, Total, Production, Consumption);
                    Business_data.Add(bus_Data);
                }
                DateTime fromdate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                DateTime date = startdate1.Date;
                // date = date.AddDays(-1);
                fromdate = (date + starttime);
                toDate = (date.AddDays(1) + starttime);
                List<Sp_PlantCategorywiseConsumption_Result> Category = db.Sp_PlantCategorywiseConsumption(fromdate, toDate, "", "", plantID, null, true).ToList();
                foreach (var id in Category)
                {
                    Barchart obj3 = new Barchart(id.Category_Name, Convert.ToDouble(id.Comsumtionvalues), "Add", 1);
                    CategoryData.Add(obj3);
                }

                var Consumeunit = "";
                if (type == true)
                {
                    Consumeunit = db.MM_Parameter.Where(s => s.Prameter_ID == 1).Select(s => s.Unit).FirstOrDefault();

                }
                else
                {
                    Consumeunit = "kVAh";
                }
                return Json(new { Business_data, Process_data, ShopwisePerformace, allDates, CategoryData, Consumeunit }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult PerformceceEnergyfeederwise(string Formdate, string todate, string ddlformat, string Shop)
        {
            List<Performance_Indices> Performace = new List<Performance_Indices>();

            DateTime startdate1 = DateTime.Parse(Formdate);
            DateTime enddate1 = DateTime.Parse(todate);
            int ShopId = Convert.ToInt32(Shop);

            if (ddlformat == "2")
            {
                var Perform = (dynamic)null;
                if (ShopId == 0)
                {
                    Perform = (from p in db.MM_Feederwise_Daily_Analytics
                               where (p.DateandTime) >= startdate1 && (p.DateandTime) <= enddate1
                               select new
                               {
                                   p.Shop_ID,
                                   p.Consumption,
                                   p.Best,
                                   p.Average,
                                   p.Efficiency,
                                   p.TagIndex,
                                   p.DateandTime
                               }).OrderBy(s => s.Shop_ID).ToList();
                }
                else
                {
                    Perform = (from p in db.MM_Feederwise_Daily_Analytics
                               join
s in db.Shop_Index_Config on
p.TagIndex equals s.TagIndex

                               where (p.DateandTime) >= startdate1 && (p.DateandTime) <= enddate1
                               && p.Shop_ID == ShopId
                               select new
                               {
                                   p.Shop_ID,
                                   p.Consumption,
                                   p.Best,
                                   p.Average,
                                   p.Efficiency,
                                   p.TagIndex,
                                   p.DateandTime
                               }).OrderBy(s => s.Shop_ID).ToList();
                }

                foreach (var id in Perform)
                {
                    int TagIndex = Convert.ToInt32(id.TagIndex);
                    var feeder1 = (from f in db.UtilityMainFeederMappings
                                   where f.TagIndex == TagIndex
                                   select new
                                   {
                                       f.FeederName
                                   }).FirstOrDefault();
                    double Consumptionengine = Convert.ToDouble(id.Consumption);
                    double Best = Convert.ToDouble(id.Best);
                    double Avg = Convert.ToDouble(id.Average);
                    double Effiency = Convert.ToDouble(id.Efficiency);
                    Boolean Is_Consumption = true;
                    Is_Production = true;
                    int ShopID = Convert.ToInt32(id.Shop_ID);
                    var ShopName1 = (from s in db.MM_MTTUW_Shops
                                     join
                                     g in db.MM_ShopsCategory on
                                     s.ShopsCat_ID equals g.ShopsCat_ID
                                     where s.Shop_ID == ShopID && s.Energy == true
                                     select new
                                     {
                                         s.Shop_Name,
                                         g.Is_Consumption,
                                         s.Spec_Unit_ID
                                     }).FirstOrDefault();
                    Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                    if (ShopName1.Spec_Unit_ID == 5)
                    {
                        Is_Production = false;
                    }
                    Performance_Indices obj = new Performance_Indices(feeder1.FeederName, 0, Best, Avg, Effiency, Consumptionengine, 0, ShopID, Is_Consumption, Is_Production);

                    Performace.Add(obj);
                }
            }
            else
            {

                if (startdate1 == enddate1)
                {
                    var Perform = (dynamic)null;
                    if (ShopId == 0)
                    {
                        Perform = (from p in db.MM_Feederwise_Daily_Analytics
                                   where (p.DateandTime) >= startdate1 && (p.DateandTime) <= enddate1
                                   select new
                                   {
                                       p.Shop_ID,
                                       p.Consumption,
                                       p.Best,
                                       p.Average,
                                       p.Efficiency,
                                       p.TagIndex,
                                       p.DateandTime
                                   }).OrderBy(s => s.Shop_ID).ToList();
                    }
                    else
                    {
                        Perform = (from p in db.MM_Feederwise_Daily_Analytics
                                   where (p.DateandTime) >= startdate1 && (p.DateandTime) <= enddate1
                                   && p.Shop_ID == ShopId
                                   select new
                                   {
                                       p.Shop_ID,
                                       p.Consumption,
                                       p.Best,
                                       p.Average,
                                       p.Efficiency,
                                       p.TagIndex,
                                       p.DateandTime
                                   }).OrderBy(s => s.Shop_ID).ToList();
                    }

                    foreach (var id in Perform)
                    {
                        int TagIndex = Convert.ToInt32(id.TagIndex);
                        var feeder1 = (from f in db.UtilityMainFeederMappings
                                       where f.TagIndex == TagIndex
                                       select new
                                       {
                                           f.FeederName
                                       }).FirstOrDefault();
                        double Consumptionengine = Convert.ToDouble(id.Consumption);
                        double Best = Convert.ToDouble(id.Best);
                        double Avg = Convert.ToDouble(id.Average);
                        double Effiency = Convert.ToDouble(id.Efficiency);

                        int ShopID = Convert.ToInt32(id.Shop_ID);
                        Boolean Is_Consumption = true;
                        Is_Production = true;
                        var ShopName1 = (from s in db.MM_MTTUW_Shops
                                         join
                                         g in db.MM_ShopsCategory on
                                         s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == ShopID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_Name,
                                             g.Is_Consumption,
                                             s.Spec_Unit_ID
                                         }).FirstOrDefault();
                        Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                        if (ShopName1.Spec_Unit_ID == 5)
                        {
                            Is_Production = false;
                        }
                        Performance_Indices obj = new Performance_Indices(feeder1.FeederName, 0, Best, Avg, Effiency, Consumptionengine, 0, ShopID, Is_Consumption, Is_Production);

                        Performace.Add(obj);
                    }
                }
                else
                {


                    List<Sp_Daywise_Performance_Indices_Energy_Feederwise_Result> consumptionwise = db.Sp_Daywise_Performance_Indices_Energy_Feederwise(startdate1, enddate1, ShopId).ToList();

                    foreach (var id in consumptionwise)
                    {

                        int TagIndex = Convert.ToInt32(id.TagIndex);
                        var feeder1 = (from f in db.UtilityMainFeederMappings
                                       where f.TagIndex == TagIndex
                                       select new
                                       {
                                           f.FeederName
                                       }).FirstOrDefault();


                        double Best = Convert.ToDouble(id.Best);
                        double Avg = Convert.ToDouble(id.Average);
                        double Effiency = Convert.ToDouble(id.Efficiency);
                        double Consumptionengine = Convert.ToDouble(id.totalconsumption);

                        int ShopID = Convert.ToInt32(id.Shop);
                        Boolean Is_Consumption = true;
                        Is_Production = true;
                        var ShopName1 = (from s in db.MM_MTTUW_Shops
                                         join
                                         g in db.MM_ShopsCategory on
                                         s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == ShopID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_Name,
                                             g.Is_Consumption,
                                             s.Spec_Unit_ID
                                         }).FirstOrDefault();
                        Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                        if (ShopName1.Spec_Unit_ID == 5)
                        {
                            Is_Production = false;
                        }
                        Performance_Indices obj = new Performance_Indices(feeder1.FeederName, 0, Best, Avg, Effiency, Consumptionengine, 0, ShopID, Is_Consumption, Is_Production);

                        Performace.Add(obj);

                        //}

                    }
                }
            }

            ViewData["Performance_Indices"] = Performace;
            ViewBag.pieData = JsonConvert.SerializeObject(Performace);
            return PartialView("FeederwisePerformceceEnergy");
        }



        public ActionResult SubProcess(string Formdate, string todate, string ddlformat, int? Shiftwise, int? Minute, string ConsumptionId, string holiday, string Procces)
        {
            try
            {
                List<Processchart> ProcessData = new List<Processchart>();
                List<Sp_ProcesswiseConsumption_Result> SubProcess = new List<Sp_ProcesswiseConsumption_Result>();
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var GroupID = db.MM_ShopsCategory.Where(s => s.ShopsCategory_Name == Procces).Select(s => s.ShopsCat_ID).FirstOrDefault();
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID == 25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).FirstOrDefault();
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }


                DateTime startdate1 = DateTime.Parse(Formdate);
                DateTime enddate1 = DateTime.Parse(todate);

                DateTime endg = DateTime.Parse(Formdate);
                DateTime startg = DateTime.Parse(Formdate);
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                var livedate = System.DateTime.Now;
                enddate1 = enddate1.AddDays(1);
                enddate1 = (enddate1.Date + starttime);
                startdate1 = (startdate1.Date + starttime);

                if (holiday != "")
                {
                    List<Processchart> ProcessNoworkingData = new List<Processchart>();
                    string[] holidayArray = { };
                    holidayArray = holiday.Split(',').ToArray();

                    foreach (var item in holidayArray)
                    {
                        int Reason_ID = Convert.ToInt32(item);

                        var noworkingday = (from n in db.MM_No_Working_Day
                                            where n.Reason_ID == Reason_ID && n.Day_Date >= startdate1 && n.Day_Date <= enddate1
                                            select new
                                            {
                                                n.Day_Date
                                            }).ToList();
                        if (noworkingday.Count() > 0)

                        {

                            foreach (var id in noworkingday.OrderBy(n => n.Day_Date))
                            {
                                var enddate = id.Day_Date.AddDays(1);
                                SubProcess = db.Sp_ProcesswiseConsumption(Convert.ToInt32(GroupID), id.Day_Date, enddate, "", "", plantID, Shiftwise, type).ToList();
                                foreach (var id1 in SubProcess)
                                {
                                    Processchart obj3 = new Processchart(id1.Shop_Name, Convert.ToDouble(id1.Consumption), Convert.ToInt32(id1.Shop_ID));
                                    ProcessNoworkingData.Add(obj3);
                                }
                            }
                            foreach (var id1 in ProcessNoworkingData.Select(s => s.ShopId).Distinct().ToList())
                            {
                                var Name = ProcessNoworkingData.Where(s => s.ShopId == id1).Select(s => s.Label).FirstOrDefault();
                                var total = ProcessNoworkingData.Where(s => s.ShopId == id1).Sum(s => s.Y);
                                Processchart obj3 = new Processchart(Name, Convert.ToDouble(total), Convert.ToInt32(id1));
                                ProcessData.Add(obj3);
                            }

                        }
                    }
                }
                else
                {
                    if (ddlformat != "1" && (startdate1.Date != livedate.Date))
                    {
                        SubProcess = db.Sp_ProcesswiseConsumption(Convert.ToInt32(GroupID), startdate1, enddate1, "", "", plantID, Shiftwise, type).ToList();
                    }
                    else
                    {


                        SubProcess = db.Sp_ProcesswiseConsumption(Convert.ToInt32(GroupID), startdate1, livedate, "", "", plantID, Shiftwise, type).ToList();
                    }
                    foreach (var id in SubProcess)
                    {
                        Processchart obj3 = new Processchart(id.Shop_Name, Convert.ToDouble(id.Consumption), Convert.ToInt32(id.Shop_ID));
                        ProcessData.Add(obj3);
                    }
                }



                return Json(ProcessData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return View();
            }

        }


        public ActionResult Analaysis(string ConsumptionId)
        {

            try
            {


                DateTime now = DateTime.Now;
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }


                List<Double?> LastMth = new List<double?>();
                List<Double?> AvgtMth = new List<double?>();
                List<Double?> EstimatedMth = new List<double?>();

                List<Double?> cumulative = new List<double?>();
                List<Double?> cumulativeMth = new List<double?>();

                List<string> allDates = new List<string>();

                Double performAvg = 0.0;
                string ddl = "";
                int Daycount = 0;
                Double cumulativedata = 0.0;
                Double maxedata = 0.0;

                // This Month date
                var month1stDate = new DateTime(now.Year, now.Month, 1);
                var endmonthd = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);
                // Last Month Date
                var firstmonthd = month1stDate.AddMonths(-1);
                var lastmonthd = month1stDate.AddDays(-1);
                //finial year startdate
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;
                var Startmonth = month > 9 ? new DateTime(year, 10, 1) : new DateTime(year - 1, 10, 1);
                int fyStartMonth = 4;
                var dte = new DateTime(DateTime.Today.Year, fyStartMonth, 1);
                if (month <= fyStartMonth)
                {
                    dte = dte.AddYears(-1);
                }


                var financial_avg = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime >= dte && s.DateandTime <= lastmonthd && s.Shop_ID == 0 && s.ConsumptionType == type).Average(s => s.TotalConsumption);
                var LastMonthToatl = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime >= firstmonthd && s.DateandTime <= lastmonthd && s.Shop_ID == 0 && s.ConsumptionType == type).Sum(s => s.TotalConsumption);

                var Perform = (from p in db.MM_Performance_Indices_Energy
                               where p.DateandTime == now && p.ConsumptionType == type && p.Shop_ID == 0
                               select new
                               {
                                   p.Consumption,
                                   p.Production,
                                   p.TotalConsumption,
                                   p.DateandTime
                               }).OrderBy(s => s.DateandTime).ToList();
                //now = now.AddDays(-1);
                if (month1stDate.Date == now.Date)
                {

                }
                else
                {
                    Perform = (from p in db.MM_Performance_Indices_Energy
                               where (p.DateandTime) >= month1stDate && (p.DateandTime) <= endmonthd && p.ConsumptionType == type && p.Shop_ID == 0
                               select new
                               {
                                   p.Consumption,
                                   p.Production,
                                   p.TotalConsumption,
                                   p.DateandTime
                               }).OrderBy(s => s.DateandTime).ToList();
                    now = now.AddDays(-1);
                    Daycount = (endmonthd - now).Days;
                    performAvg = Convert.ToDouble(Perform.Average(s => s.TotalConsumption));
                    foreach (var id in Perform.OrderBy(s => s.DateandTime))
                    {
                        cumulativedata += Convert.ToDouble(id.TotalConsumption);
                        cumulative.Add(Math.Round(cumulativedata));
                        cumulativeMth.Add(Math.Round(cumulativedata));
                        EstimatedMth.Add(0);
                        LastMth.Add(0);
                        AvgtMth.Add(0);
                        ddl = Convert.ToDateTime(id.DateandTime).ToString("dd/MMM/yy");
                        allDates.Add(ddl);

                    }
                    maxedata = cumulativeMth.Max().Value;
                    cumulativedata = 0.0;

                    for (int i = 0; i <= Daycount; i++)
                    {
                        if (i == 0)
                        {
                            cumulativedata = Math.Round(maxedata + performAvg);
                            EstimatedMth.Add(Math.Round(cumulativedata));
                        }
                        else
                        {
                            cumulativedata += performAvg;
                            EstimatedMth.Add(Math.Round(cumulativedata));
                        }
                        now = now.AddDays(1);
                        ddl = Convert.ToDateTime(now).ToString("dd/MMM/yy");
                        allDates.Add(ddl);

                        LastMth.Add(0);
                        AvgtMth.Add(0);


                    }

                    performAvg = EstimatedMth.Average().Value;
                    LastMth.Add(LastMonthToatl);
                    allDates.Add("Last Mth");
                    allDates.Add("Avg Mth");
                    AvgtMth.Add(0);



                    // if month is october or later, the FY started 10-1 of this year
                    // else it started 10-1 of last year
                    var finavg = Math.Round(Convert.ToDouble(financial_avg), 0);
                    AvgtMth.Add(finavg);
                }
                return Json(new { Perform, LastMth, AvgtMth, EstimatedMth, cumulative, cumulativeMth, allDates }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TOD(string ConsumptionId)
        {
            try
            {

                List<toddata> todobj = new List<toddata>();
                List<todName> TOD_Name = new List<todName>();
                List<double?> TOD_Total = new List<double?>();

                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }
                // This Month date
                DateTime now = DateTime.Now.Date;
                var yesturday = now.AddDays(-1);
                var month1stDate = new DateTime(now.Year, now.Month, 1);
                var endmonthd = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);
                // Last Month Date
                var firstmonthd = month1stDate.AddMonths(-1);
                var lastmonthd = month1stDate.AddDays(-1);
                double shiftA, total, shiftA_avg = 0.0;
                double totalA = 0.0;
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shop_ID == 30 select new { s.Shift_ID }).ToList();

                //yesturday data
                var yesturday_data = (from y in db.Todwise_ShopConsumption
                                      where y.Shop_ID == 0 && y.ConsumptionType == type && (y.Dateandtime >= yesturday.Date && y.Dateandtime < now)
                                      select new
                                      {
                                          y.Tod_ID,
                                          y.Consumption
                                      }).OrderBy(s => s.Tod_ID).ToList();
                string yesturday_date = Convert.ToDateTime(yesturday).ToString("dd.MM.yyyy");
                total = Convert.ToDouble(yesturday_data.Sum(s => s.Consumption));
                todName objName = new todName(1, "For Yesterday", total);
                TOD_Name.Add(objName);
                var Percentage = 0.0;
                string percent = "";
                //shiftA = Convert.ToDouble(yesturday_data[0].Consumption);
                //ShiftB = Convert.ToDouble(yesturday_data[1].Consumption);
                //ShiftC = Convert.ToDouble(yesturday_data[2].Consumption);
                foreach (var id1 in db.MM_Tod.OrderBy(s => s.Tod_ID))
                {

                    shiftA = Convert.ToDouble(yesturday_data.Where(s => s.Tod_ID == id1.Tod_ID).Sum(s => s.Consumption));
                    Percentage = (shiftA / total) * 100;
                    percent = Percentage.ToString() + '%';
                    toddata obj = new toddata(1, yesturday_date, yesturday_date, Convert.ToDouble(shiftA), percent);
                    todobj.Add(obj);
                }


                int Daycount = (endmonthd - yesturday).Days;

                // For the Month Data
                var thismonth_data = (from y in db.Todwise_ShopConsumption
                                      where y.Shop_ID == 0 && y.ConsumptionType == type && (y.Dateandtime >= month1stDate &&
                                  y.Dateandtime < now.Date)
                                      select new
                                      {
                                          y.Tod_ID,
                                          y.Consumption,
                                          y.Dateandtime

                                      }).OrderBy(s => s.Dateandtime).ToList();
                total = Convert.ToDouble(thismonth_data.Sum(s => s.Consumption));

                todName objName1 = new todName(2, "For This Month", total);
                TOD_Name.Add(objName1);

                var firstMonthdath = month1stDate.ToString("dd.MM.yyyy");
                var endMonthdath = endmonthd.ToString("dd.MM.yyyy");
                foreach (var id in db.MM_Tod.OrderBy(s => s.Tod_ID))
                {
                    shiftA = Convert.ToDouble(thismonth_data.Where(s => s.Tod_ID == id.Tod_ID).Sum(s => s.Consumption));
                    Percentage = (shiftA / total) * 100;
                    percent = Percentage.ToString() + '%';
                    shiftA_avg = Convert.ToDouble(thismonth_data.Where(s => s.Tod_ID == id.Tod_ID).Average(s => s.Consumption));
                    shiftA_avg = (shiftA_avg * Daycount);
                    totalA = Math.Round(shiftA + shiftA_avg, 0);
                    toddata obj = new toddata(2, firstMonthdath, yesturday_date, shiftA, percent);
                    todobj.Add(obj);
                    //Percentage = (shiftA / total) * 100;
                    //percent = Percentage.ToString() + '%';
                    obj = new toddata(3, firstMonthdath, endMonthdath, totalA, percent);
                    todobj.Add(obj);

                }




                //toddata obj1 = new toddata("For This Month", firstMonthdath, yesturday_date, total, shiftA, ShiftB, ShiftC);
                //todobj.Add(obj1);

                // for the Estimate Month Data
                total = todobj.Where(s => s.todId == 3).Sum(s => s.Consumption);
                TOD_Total.Add(total);
                todName objName2 = new todName(3, "Estimatation For this Month", total);
                TOD_Name.Add(objName2);
                //toddata obj2 = new toddata("Estimatation For this Month", firstMonthdath, endMonthdath, total, totalA, totalB, totalC);
                //todobj.Add(obj2);

                return Json(new { todobj, TOD_Name }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Performance(string ConsumptionId)
        {
            try
            {
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }
                DateTime today = DateTime.Now.Date;
                today = today.AddDays(-1);
                var startdate1 = today.Date;
                var month1stDate = new DateTime(today.Year, today.Month, 1);
                var endmonthd = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);


                List<Performance_Indices> Performace = new List<Performance_Indices>();

                var Perform = (from p in db.MM_Performance_Indices_Energy
                               where p.DateandTime == today && p.ConsumptionType == type
                               select new
                               {
                                   p.Shop_ID,
                                   p.Consumption,
                                   p.Best,
                                   p.Average,
                                   p.Efficiency,
                                   p.Production,
                                   p.TotalConsumption,
                                   p.DateandTime
                               }).OrderBy(s => s.Shop_ID).ToList();
                List<ShopWiseConsumption> consumptionwise3 = new List<ShopWiseConsumption>();

                foreach (var id in Perform.OrderByDescending(s => s.TotalConsumption))
                {
                    string ShopName = "";
                    Boolean Is_Consumption = true;
                    Is_Production = true;
                    if (id.Shop_ID == 0)
                    {
                        ShopName = "Plant";

                    }
                    else
                    {

                        var ShopName1 = (from s in db.MM_MTTUW_Shops
                                         join
                                         g in db.MM_ShopsCategory on
                                         s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == id.Shop_ID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_Name,
                                             g.Is_Consumption,
                                             s.Spec_Unit_ID
                                         }).FirstOrDefault();
                        Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                        if (ShopName1 != null)
                        {
                            ShopName = ShopName1.Shop_Name;
                            Is_Consumption = Convert.ToBoolean(ShopName1.Is_Consumption);
                            if (ShopName1.Spec_Unit_ID == 5)
                            {
                                Is_Production = false;
                            }
                        }

                    }
                    double Consumptionengine = Convert.ToDouble(id.Consumption);


                    double Best = Convert.ToDouble(id.Best);
                    double Avg = Convert.ToDouble(id.Average);
                    var avg1 = db.MM_Performance_Indices_Energy.Where(s => s.Shop_ID == id.Shop_ID && s.DateandTime >= month1stDate && s.DateandTime <= endmonthd && s.Production != 0 && s.ConsumptionType == type).Average(s => s.Consumption);
                    var Effiency = Math.Round(Convert.ToDouble(Best / Consumptionengine * 100), 2);
                    if (Consumptionengine == 0)
                    {
                        Effiency = 0;
                    }
                    double Consumption = Convert.ToDouble(id.TotalConsumption);
                    int Production = Convert.ToInt32(id.Production);
                    int ShopID = Convert.ToInt32(id.Shop_ID);
                    if (ShopName != "")
                    {
                        Performance_Indices obj = new Performance_Indices(ShopName, Consumptionengine, Best, Avg, Effiency, Consumption, Production, ShopID, Is_Consumption, Is_Production);
                        Performace.Add(obj);
                    }
                }

                return Json(new { Performace }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public class Summary
        {
            public double totalpower { get; set; }
            public double totalavg { get; set; }
            public double totalavgsec { get; set; }
            public int totalprod { get; set; }

            public double best { get; set; }
            public double bestpower { get; set; }
            public int bestprod { get; set; }

            public double maxpower { get; set; }

            public string maxdate { get; set; }
            public string bestdate { get; set; }
            public Summary(double totalpower, double totalavg, double totalavgsec, int totalprod, double best, double bestpower, int bestprod, double maxpower, string bestdate, string maxdate)
            {
                this.totalpower = totalpower;
                this.totalavg = totalavg;
                this.totalavgsec = totalavgsec;
                this.totalprod = totalprod;
                this.best = best;
                this.bestpower = bestpower;
                this.bestprod = bestprod;
                this.maxpower = maxpower;
                this.bestdate = bestdate;
                this.maxdate = maxdate;

            }

        }

        public class toddata
        {
            public int todId { get; set; }
            public string startdate { get; set; }
            public string enddate { get; set; }
            public double Consumption { get; set; }

            public string percent { get; set; }



            public toddata(int todId, string startdate, string enddate, double Consumption, string percent)
            {

                this.todId = todId;
                this.startdate = startdate;
                this.enddate = enddate;

                this.Consumption = Consumption;
                this.percent = percent;

            }

        }
        public class todName
        {
            public int todId { get; set; }
            public string Name { get; set; }
            public double Consumption { get; set; }


            public todName(int todId, string Name, double totl)
            {

                this.todId = todId;
                this.Name = Name;
                this.Consumption = totl;

            }

        }

        public class Areawise_Consumption
        {
            public string AreaName { get; set; }
            public string date { get; set; }
            public Double TotalConsumtion { get; set; }
            public int Area { get; set; }


            public Areawise_Consumption(string AreaName, string date, double Consumption, int area)
            {
                this.AreaName = AreaName;
                this.date = date;
                this.TotalConsumtion = Consumption;
                this.Area = area;

            }
        }


        public class ShopWisespec_Data
        {

            public int dataId { get; set; }
            public String DataName { get; set; }
            public double? totalconsumption { get; set; }
            public int? Production { get; set; }
            public double? specconsumption { get; set; }



            public ShopWisespec_Data(int dataId, String DataName, double totalconsumption, int Production, double specconsumption)
            {

                this.dataId = dataId;
                this.DataName = DataName;
                this.totalconsumption = totalconsumption;
                this.Production = Production;
                this.specconsumption = specconsumption;


            }

        }

        public class ShiftPlantData
        {

            public double Consumption { get; set; }
            public double TotalConsumption { get; set; }
            public int Production { get; set; }

            public double Best { get; set; }
            public string date { get; set; }
            public ShiftPlantData(double Consumption, double TotalConsumption, int Production, double Best, string date)
            {
                this.Consumption = Consumption;
                this.TotalConsumption = TotalConsumption;
                this.Production = Production;
                this.Best = Best;
                this.date = date;
            }

        }

        public class Shopwise_SEC
        {
            public string Name { get; set; }
            public string date { get; set; }
            public Double Value { get; set; }
            public int ID { get; set; }

            public string unit { get; set; }


            public Shopwise_SEC(string Name, string date, double Value, int ID, string unit)
            {
                this.Name = Name;
                this.date = date;
                this.Value = Value;
                this.ID = ID;
                this.unit = unit;

            }
        }


    }

}
