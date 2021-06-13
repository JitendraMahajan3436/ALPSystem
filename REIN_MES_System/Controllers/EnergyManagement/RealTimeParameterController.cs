using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class RealTimeParameterController : Controller
    {
        // GET: RealTimeParameter
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();

        public ActionResult Index(int? Shop_ID)

        {
            try
            {
                ViewBag.Shop_ID = Shop_ID;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                ViewBag.TodaySpecificConsumption = 0;
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                ViewBag.GlobalDataModel = globalData;
                var ShopId = (from s in db.MM_MTTUW_Shops
                              join
        u in db.MM_Specific_Cosume_Unit on
        s.Spec_Unit_ID equals u.Spec_Unit_ID
                              where s.Shop_ID == Shop_ID && s.Energy == true
                              select new
                              {
                                  u.Spec_Unit_ID,
                                  u.Unit_Name,
                                  s.Shop_Name
                              }).FirstOrDefault();


                globalData.pageTitle = ShopId.Shop_Name;
                ViewBag.ShopID = Shop_ID;
                DateTime fromdate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                DateTime date = DateTime.Now.Date;
                DateTime dtn = DateTime.Now.Date;
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID == 25
                             select new
                             {
                                 s.Shift_Start_Time
                             }).FirstOrDefault();

                var time = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                var time1 = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                List<Performance_Indices> Performace = new List<Performance_Indices>();
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange.ToList(), "DateID", "DateName", 2);
                ViewBag.ddlDateRange1 = new SelectList(db.MM_DateRange.ToList(), "DateID", "DateName", 2);
                ViewBag.Shiftwise = new SelectList(db.MM_MTTUW_Shift.Where(s=>s.Shop_ID==30).ToList(), "Shift_ID", "Shift_Name");
                ViewBag.Shiftwise1 = new SelectList(db.MM_MTTUW_Shift.Where(s => s.Shop_ID == 30).ToList(), "Shift_ID", "Shift_Name");
                ViewBag.MinuteRange = new SelectList(db.MM_MinutesRange.ToList(), "Minute", "unit", 60);
                ViewBag.MinuteRange1 = new SelectList(db.MM_MinutesRange.ToList(), "Minute", "unit", 60);

                List<SP_Shopwise_Timewise_Consumption_Result> obj12 = new List<SP_Shopwise_Timewise_Consumption_Result>();

                var shopwie_Data = db.MM_Shopwise_TimeConsumption.Where(s => s.Shop_ID == Shop_ID && s.DateandTime >= dtn).ToList();

                // Yesturday Data
                {
                    date = date.AddDays(-1);
                    fromdate = (date + time);
                    toDate = (date.AddDays(1) + time1);
                    ViewBag.starty = fromdate.ToString("dd-MMM hh:mm tt");
                    ViewBag.endy = toDate.ToString("dd-MMM hh:mm tt");
                    var yesturday = (from p in db.MM_Performance_Indices_Energy
                                     where p.Shop_ID == Shop_ID && p.DateandTime == date
                                     select new
                                     {
                                         p.TotalConsumption,
                                         p.Consumption,
                                         p.Production,
                                         p.ConsumptionType
                                     }).ToList();

                    var Shop = ShopId;
                    ViewBag.yesturdayProduction = 0;
                    ViewBag.yesturdayspecificConsumption = 0;
                    ViewBag.yesturdayConsumption = 0;
                    ViewBag.yesturdayPKVAH = 0;
                    ViewBag.yesturdaySKVAH = 0;
                    ViewBag.yesturdayCKVAH = 0;

                    foreach (var id in yesturday)
                    {
                        if (id.ConsumptionType == true)
                        {
                            ViewBag.yesturdayProduction = id.Production;
                            ViewBag.yesturdayspecificConsumption = id.Consumption;
                            ViewBag.yesturdayConsumption = id.TotalConsumption;
                        }
                        else
                        {
                            ViewBag.yesturdayPKVAH = id.Production;
                            ViewBag.yesturdaySKVAH = id.Consumption;
                            ViewBag.yesturdayCKVAH = id.TotalConsumption;

                        }

                    }




                }
                // Last 1 week Data
                {


                    DateTime LastWeek = DateTime.Now.AddDays(-7);
                    var endDate = DateTime.Now;
                    endDate = endDate.Date;
                    LastWeek = LastWeek.Date;
                    LastWeek = LastWeek.Date;
                    var Data = (from p in db.MM_Performance_Indices_Energy
                                where (p.DateandTime) >= LastWeek && (p.DateandTime) <= endDate &&
                                p.Shop_ID == Shop_ID
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

                    // Kwh Last 1 week data

                    var perivousweekdata = Data.Where(s => s.DateandTime >= LastWeek && s.DateandTime <= endDate && s.ConsumptionType == true).Sum(s => s.TotalConsumption);
                    var perivousweekProdution = Data.Where(s => s.DateandTime >= LastWeek && s.DateandTime <= endDate && s.ConsumptionType == true).Sum(s => s.Production);

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

                    // KVAH Last 1 week data
                    perivousweekdata = Data.Where(s => s.DateandTime >= LastWeek && s.DateandTime <= endDate && s.ConsumptionType == false).Sum(s => s.TotalConsumption);
                    perivousweekProdution = Data.Where(s => s.DateandTime >= LastWeek && s.DateandTime <= endDate && s.ConsumptionType == false).Sum(s => s.Production);

                    perivousweekdata = Convert.ToDouble(perivousweekdata);
                    perivousweekProdution = Convert.ToInt32(perivousweekProdution);
                    perivousspecfic = 0.0;
                    if (perivousweekProdution > 0)
                    {
                        perivousspecfic = Math.Round(Convert.ToDouble(perivousweekdata / perivousweekProdution), 0);
                    }
                    else
                    {
                        perivousspecfic = 0;
                    }


                    ViewBag.weekdatakvah = perivousweekdata;
                    ViewBag.weekProkvah = perivousweekProdution;
                    ViewBag.weekconskvah = perivousspecfic;

                    DateTime startw = DateTime.Parse(LastWeek.Date.ToString());
                    DateTime endw = DateTime.Parse(endDate.Date.ToString());
                    startw = (startw + time);
                    endw = (endw + time);
                    ViewBag.startw = startw.ToString("dd-MMM hh:mm tt");
                    ViewBag.endw = endw.ToString("dd-MMM hh:mm tt");
                }


                // Today's Consumption
                {


                    // fromdate = (date + time);
                    // var startdate = System.DateTime.Now.Date;
                    DateTime todaydate = DateTime.Now.Date;
                    fromdate = (todaydate + time);
                    toDate = System.DateTime.Now;
                    ViewBag.startt = fromdate.ToString("dd-MMM hh:mm tt");
                    ViewBag.endt = toDate.ToString("dd-MMM hh:mm tt");

                    var Todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime > fromdate && s.DateandTime <= toDate && s.Shop_ID == Shop_ID).ToList();

                    ViewBag.TodayConsumption = 0.0;
                    ViewBag.TodayProduction = 0;
                    ViewBag.TodaySpecificConsumption = 0.0;

                    ViewBag.TodayC = 0.0;
                    ViewBag.TodayP = 0;
                    ViewBag.TodayS = 0.0;
                    var Production = Todayresult.Where(s => s.ConsumptionType == true).Sum(s => s.Production);
                    var total = Todayresult.Where(s => s.ConsumptionType == true).Sum(s => s.TotalConsumption);
                    if (Todayresult.Where(s => s.ConsumptionType == true).Count() > 0)
                    {



                        if (Production != null && Production != 0)
                        {
                            ViewBag.TodaySpecificConsumption = Math.Round(Convert.ToDouble(total / Production), 0);
                            ViewBag.TodayConsumption = total;
                            ViewBag.TodayProduction = Production;
                        }
                        ViewBag.TodayConsumption = total;
                    }
                    if (Todayresult.Where(s => s.ConsumptionType == false).Count() > 0)
                    {
                        Production = Todayresult.Where(s => s.ConsumptionType == false).Sum(s => s.Production);
                        total = Todayresult.Where(s => s.ConsumptionType == false).Sum(s => s.TotalConsumption);
                        if (Production != null || Production != 0)
                        {
                            ViewBag.TodayS = Math.Round(Convert.ToDouble(total / Production), 0);
                            ViewBag.TodayC = total;
                            ViewBag.TodayP = Production;
                        }
                        ViewBag.TodayC = total;
                    }

                    //List<SP_LiveShopwiseConsumption_Result> obj2 = db.SP_LiveShopwiseConsumption(plantID, fromdate, toDate, Shop_ID).ToList();
                    //List<Sp_ProductionCount_Result> prod2 = db.Sp_ProductionCount(Shop_ID, fromdate, toDate, plantID).ToList();
                    //var Liveprod = prod2[0].Production > 0 ? prod2[0].Production : 0;

                    //if (obj2.Count() > 0)
                    //{
                    //    if (prod2[0].Production == 0)
                    //    {
                    //        ViewBag.TodaySpecificConsumption = 0;

                    //    }
                    //    else
                    //    {
                    //        int production = Convert.ToInt32(prod2[0].Production);
                    //        ViewBag.TodayProduction = production;
                    //        ViewBag.TodaySpecificConsumption = (Math.Round((Convert.ToDouble(obj2[0].totalconsumption) / production), 2));
                    //    }

                    //    ViewBag.TodayConsumption = Math.Round(Convert.ToDouble(obj2[0].totalconsumption), 2);

                    //}



                    // Monthly Consumption 

                    date = new DateTime(dtn.Year, dtn.Month, 1);
                    var firstmonthd = date.AddMonths(-1);
                    var lastmonthd = date.AddDays(-1);


                    var Monthresult = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime >= firstmonthd && s.DateandTime <= lastmonthd && s.Shop_ID == Shop_ID).ToList();
                    ViewBag.Monthlyspecificconsumption = 0;
                    ViewBag.Monthconsumption = 0.0;
                    ViewBag.MonthlyProduction = 0;
                    ViewBag.Monthlys = 0;
                    ViewBag.Monthc = 0.0;
                    ViewBag.MonthlyP = 0;

                    // var monthtotal = Monthresult.Where(s => s.ConsumptionType == true).Sum(s => s.Production);
                    if (Monthresult.Where(s => s.ConsumptionType == true).Count() > 0)
                    {

                        Production = Monthresult.Where(s => s.ConsumptionType == true).Sum(s => s.Production);
                        total = Monthresult.Where(s => s.ConsumptionType == true).Sum(s => s.TotalConsumption);

                        if (Production != 0)
                        {

                            ViewBag.Monthlyspecificconsumption = Math.Round(Convert.ToDouble(total / Production), 0);
                            ViewBag.Monthconsumption = total;
                            ViewBag.MonthlyProduction = Production;
                        }
                        ViewBag.Monthconsumption = total;
                        // var monthtotal = Monthresult.Where(s => s.ConsumptionType == true).Sum(s => s.Production);
                    }

                    if (Monthresult.Where(s => s.ConsumptionType == true).Count() > 0)
                    {
                        Production = Monthresult.Where(s => s.ConsumptionType == false).Sum(s => s.Production);
                        total = Monthresult.Where(s => s.ConsumptionType == false).Sum(s => s.TotalConsumption);
                        if (Production != 0)
                        {

                            ViewBag.Monthlys = Math.Round(Convert.ToDouble(total / Production), 0);
                            ViewBag.Monthc = total;
                            ViewBag.MonthlyP = Production;
                        }
                        ViewBag.Monthc = total;
                    }
                    //fromdate = (firstmonthd + time);
                    //toDate = System.DateTime.Now;
                    //if(fromdate.Date == toDate.Date)
                    //{
                    //    if (obj2.Count() > 0)
                    //    {
                    //        if (prod2[0].Production == 0)
                    //        {
                    //            ViewBag.Monthlyspecificconsumption = 0;
                    //            ViewBag.MonthlyProduction = 0;

                    //        }
                    //        else
                    //        {
                    //            int production = Convert.ToInt32(prod2[0].Production);
                    //            ViewBag.MonthlyProduction = production;
                    //            ViewBag.Monthlyspecificconsumption = (Math.Round((Convert.ToDouble(obj2[0].totalconsumption) / production), 2));
                    //        }

                    //        ViewBag.Monthconsumption = Math.Round(Convert.ToDouble(obj2[0].totalconsumption), 2);

                    //    }

                    //}
                    //else
                    {



                        //toDate = (toDate.Date + time);
                        //ViewBag.endm = toDate.ToString("dd MMM yy hh:mm:tt");
                        //List<Sp_DailyShopwiseConsumption_Result> obj1 = db.Sp_DailyShopwiseConsumption(Shop_ID, firstmonthd, lastmonthd, plantID).ToList();
                        //List<Sp_ProductionCount_Result> prod1 = db.Sp_ProductionCount(Shop_ID, firstmonthd, lastmonthd, plantID).ToList();

                        //if (obj1.Count() > 0)
                        //{

                        //    if (prod1[0].Production == 0)
                        //    {
                        //        ViewBag.Monthlyspecificconsumption = 0;
                        //        ViewBag.MonthlyProduction = 0;

                        //    }
                        //    else
                        //    {
                        //        int production = Convert.ToInt32(prod1[0].Production);
                        //        ViewBag.MonthlyProduction = production;
                        //        ViewBag.Monthlyspecificconsumption = (Math.Round((Convert.ToDouble(obj1[0].Comsumtionvalues) / production), 2));
                        //    }
                        //}


                    }
                    DateTime startm = DateTime.Parse(firstmonthd.Date.ToString());
                    DateTime endm = DateTime.Parse(lastmonthd.Date.AddDays(1).ToString());
                    startm = (startm + time);
                    endm = (endm + time);
                    ViewBag.startm = startm.ToString("dd-MMM hh:mm tt");
                    ViewBag.endm = endm.ToString("dd-MMM hh:mm tt");

                }

                // Shop Wise Data
                {
                    List<string> allDates = new List<string>();
                    List<double?> Consumption = new List<double?>();
                    List<double?> SpficConsumption = new List<double?>();
                    List<SP_Shopwise_TimeConsumptionperVechicle_Result> specfic = new List<SP_Shopwise_TimeConsumptionperVechicle_Result>();
                    var S_startd = fromdate.AddDays(-1);
                    var S_endd = fromdate;

                    //obj12 = db.SP_Shopwise_Timewise_Consumption(plantID, S_startd, S_endd, Shop_ID, Convert.ToInt32(60),true ).ToList();
                    shopwie_Data = db.MM_Shopwise_TimeConsumption.Where(s => s.Shop_ID == Shop_ID && s.DateandTime > S_startd && s.DateandTime <= S_endd && s.ConsumptionType == true).ToList();
                    //if (ShopId.Spec_Unit_ID == 5)
                    //{

                    //}
                    //else
                    //{
                    //    specfic = db.SP_Shopwise_TimeConsumptionperVechicle(plantID, S_startd, S_endd, Shop_ID, Convert.ToInt32(60),true).ToList();

                    //}


                    List<double?> cumulative = new List<double?>();
                    List<double?> Averge = new List<double?>();
                    List<int?> Production = new List<int?>();
                    double cumulativedata = 0.0;
                    int avg = 1;
                    double avgdata = 0.0;

                    foreach (var id in shopwie_Data)
                    {
                        Consumption.Add(id.TotalConsumption);
                        string ddl = Convert.ToDateTime(id.DateandTime).ToString("hh:mm tt");
                        cumulativedata += Math.Round(Convert.ToDouble(id.TotalConsumption), 0);
                        avgdata = Math.Round(Convert.ToDouble(cumulativedata / avg), 0);
                        cumulative.Add(cumulativedata);
                        Averge.Add(avgdata);
                        avg += 1;
                        allDates.Add(ddl);
                        if (ShopId.Spec_Unit_ID == 5)
                        {
                            SpficConsumption.Add(id.Consumption);
                            Production.Add(id.Production);
                        }
                        else
                        {
                            SpficConsumption.Add(id.Consumption);
                            Production.Add(id.Production);
                        }
                    }
                    //if (ShopId.Spec_Unit_ID != 5)
                    //{
                    //    foreach (var id in specfic)
                    //    {
                    //        SpficConsumption.Add(id.Consumption);


                    //    }
                    //}



                    ViewData["Shopwise_Data"] = Consumption.ToList();
                    ViewData["Shopwise_Date"] = allDates.ToList();
                    ViewData["Shopwise_specfic_Data"] = SpficConsumption.ToList();
                    ViewData["cumulative"] = cumulative.ToList();
                    ViewData["Averge"] = Averge.ToList();
                    ViewData["Production"] = Production.ToList();
                }



                //  Feederwise Consumption Pie chart   
                {
                    IFormatProvider culture = new CultureInfo("en-US", true);


                    var Fstart = fromdate.AddDays(-1);
                    var Fend = fromdate;


                    string Month = Fstart.ToString("MMM-yyyy ", CultureInfo.InvariantCulture);
                    ViewBag.Month = Month;
                    string sp_fromDate = Fstart.ToString("dd/MMM hh:mm tt", CultureInfo.InvariantCulture);
                    string sp_toDate = Fend.ToString("dd/MMM hh:mm tt", CultureInfo.InvariantCulture);
                    ViewBag.fromdate = sp_fromDate;
                    ViewBag.todate = sp_toDate;
                    List<Sp_CategorywiseConsumption_Result> Category = db.Sp_CategorywiseConsumption(Shop_ID, Fstart, Fend, "", "", plantID, null, true).ToList();
                    List<Sp_DailyShopwiseConsumption_New_Result> pie = db.Sp_DailyShopwiseConsumption_New(Shop_ID, Fstart, Fend, "", "", plantID, null, true).ToList();

                    //IncomerArea

                    List<Sp_Shift_AreawiseShop_Result> InArea = db.Sp_Shift_AreawiseShop(Fstart, Fend, "", "", plantID, null, Shop_ID, true, true).ToList();
                    //ConsumeArea
                    //List<Sp_Shift_AreawiseShop_Result> ConsumeArea = db.Sp_Shift_AreawiseShop(Fstart, Fend, "", "", plantID,null, Shop_ID, false, true).ToList();

                    List<Barchart> pieData = new List<Barchart>();
                    List<Barchart> CategoryData = new List<Barchart>();
                    List<Barchart> FeederData = new List<Barchart>();
                    List<Barchart> SubFeederData = new List<Barchart>();
                    List<Areachart> AreaINData = new List<Areachart>();

                    var otherconsume = (from m in db.MM_Shiftwise_Consume_Power
                                        where m.Dateandtime >= Fstart && m.Dateandtime < Fend && m.Shop_ID == Shop_ID
                                        && m.Income_Power == false && m.ConsumptionType == true
                                        select
                                        new { m.Consumption }).Sum(s => s.Consumption);

                    //List<Barchart> AreaconsumData = new List<Barchart>();
                    //var otherconsume = (from m in db.MM_Shiftwise_Consume_Power
                    //                    where m.Dateandtime >= fromdate && m.Dateandtime <= toDate && m.Shop_ID == Shop_ID
                    //                    && m.Income_Power == false
                    //                    select
                    //                    new { m.Consumption }).Sum(s => s.Consumption);


                    double FeederTotal = 0.0;
                    if (pie.Count() > 0)
                    {
                        foreach (var id in pie)
                        {

                            string function = id.TagBoolean == true ? "Add" : "Subtract";
                            Double Y = Math.Round(Convert.ToDouble(id.Comsumtionvalues), 2);
                            Barchart obj1 = new Barchart(id.FeederName, Y, function, 1);
                            if (id.TagBoolean == true)
                            {
                                pieData.Add(obj1);
                                FeederData.Add(obj1);
                            }
                            else
                            {
                                SubFeederData.Add(obj1);
                            }

                        }
                        FeederTotal = Convert.ToDouble(FeederData.Sum(s => s.Y));
                        if (otherconsume != null)
                        {
                            Barchart obj1 = new Barchart("Other", otherconsume.Value, "Add", 0);
                            pieData.Add(obj1);
                            FeederData.Add(obj1);
                        }
                        ViewData["pieData1"] = pieData;
                        ViewData["ParetoData"] = pieData.OrderByDescending(s => s.Y);
                        ViewBag.pieData = JsonConvert.SerializeObject(pieData);

                        ViewData["feederData1"] = FeederData;
                        ViewData["SubFeederData"] = SubFeederData;

                    }
                    foreach (var id in Category)
                    {
                        Barchart obj3 = new Barchart(id.Category_Name, Convert.ToDouble(id.Comsumtionvalues), "Add", 1);
                        CategoryData.Add(obj3);
                    }
                    foreach (var id in InArea)
                    {
                        Areachart obj4 = new Areachart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), Convert.ToInt32(id.AreaId));
                        AreaINData.Add(obj4);
                    }
                    Double Other = FeederTotal - Convert.ToDouble(AreaINData.Sum(s => s.Y));
                    Areachart obj5 = new Areachart("Others", Convert.ToDouble(Other), Convert.ToInt32(0));
                    AreaINData.Add(obj5);
                    //foreach (var id in ConsumeArea)
                    //{
                    //    Barchart obj5 = new Barchart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), "Add");
                    //    AreaconsumData.Add(obj5);
                    //}
                    ViewData["AreaINData"] = AreaINData;
                    //ViewData["AreaconsumData"] = AreaINData;
                    ViewData["CategoryData"] = CategoryData;
                    //ViewBag.CategoryData1 = JsonConvert.SerializeObject(CategoryData);
                }




                // Live Reading Data 

                {

                    List<LastUpdatedate> updatedate = new List<LastUpdatedate>();
                    var Livedate = System.DateTime.Now;
                    var Today = Livedate.AddMinutes(-1410);
                    List<Sp_Feederwise_Live_Reading_Result> LiveData = db.Sp_Feederwise_Live_Reading(Shop_ID, Today, Livedate).ToList();
                    var status = db.MM_Mailalert_status.Where(s => s.alert_Id == 4 && s.Shop_ID == Shop_ID && s.Feeder_ID != null && s.DateandTime == Livedate.Date).Select(s => s.Feeder_ID).Distinct().ToList();
                    ViewBag.mailstatus = status;
                    // ViewBag.Parameter = Parameter;
                    ViewBag.FeederList = LiveData;
                    foreach (var id in LiveData)
                    {
                        var datetime = LiveData.Where(s => s.Feeder_ID == id.Feeder_ID).Max(s => s.Dateandtime);
                        //var date = Convert.ToDateTime(datetime).ToString("dd MMM hh:mm");
                        if (datetime != null)
                        {


                            string ddl = Convert.ToDateTime(datetime).ToString("dd MMM hh:mm tt");

                            LastUpdatedate objfeeder = new LastUpdatedate(Convert.ToInt32(id.Feeder_ID), ddl);
                            updatedate.Add(objfeeder);
                        }
                    }

                    ViewBag.Feederdate = updatedate;
                    ViewBag.FeederName = (from f in db.MM_Feeders
                                          join
                  s in db.UtilityMainFeederMappings on
                  f.Feeder_ID equals s.Feeder_ID
                                          where f.Shop_ID == Shop_ID && (s.Parameter_ID == 1 && s.ManualMeter != true)
                                          select f).ToList().OrderBy(s => s.SortOrder);

                    var unit = ShopId.Unit_Name.Replace("\r\n", "");
                    ViewBag.specUnit = unit;

                    var Parameter = db.MM_Parameter.ToList();
                    var Parametercounts = Parameter.GroupBy(s => s.Description).OrderBy(g => g.Count()).
                                             Select(g => new { Name = g.Key, Count = g.Count() }).ToList();
                    var Paracount = Parameter.Count();
                    ViewBag.Paracount = Paracount;
                    var Parametersubcounts = Parameter.GroupBy(s => s.Description).OrderBy(g => g.Count()).
                                           Select(g => new { Name = g.Key, Count = g.Count() > 1 }).ToList();

                    var subcount = Parametersubcounts.Where(s => s.Count == true).ToList();



                    ViewBag.Parameter = Parameter.ToList();

                    var Parameterlist = Parameter.Select(s => s.Description).Distinct().ToList();
                    ViewBag.Parameterlist = Parameterlist;
                    List<string> SubParameter = new List<string>();
                    List<ParameterSub> subobj = new List<ParameterSub>();
                    foreach (var id in Parameterlist)
                    {
                        foreach (var id1 in Parametercounts)
                        {
                            if (id == id1.Name)
                            {


                                ParameterSub countobj = new ParameterSub(id1.Name, id1.Count);
                                subobj.Add(countobj);
                            }
                        }
                    }
                    ViewBag.Parameterdesc = subobj;

                    foreach (var id2 in Parameterlist)
                    {
                        foreach (var id in subcount)
                        {


                            if (id2 == id.Name)
                            {


                                var sub = Parameter.Where(s => s.Description == id.Name).ToList();
                                foreach (var id1 in sub)
                                {
                                    SubParameter.Add(id1.Prameter_Name);
                                }
                            }


                        }
                    }

                    ViewBag.SubParameter = SubParameter;
                }
                ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name");
                ViewBag.ParameterID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name");
                ViewBag.ParameterID1 = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name");
                ViewBag.Reason_ID = new SelectList(db.MM_Holiday_Reason, "Reason_ID", "Reason_Name");
                ViewBag.Reason_ID1 = new SelectList(db.MM_Holiday_Reason, "Reason_ID", "Reason_Name");
                ViewBag.Feeder = new SelectList(db.MM_Feeders.Where(s => s.Shop_ID == Shop_ID), "Feeder_ID", "FeederName");
                ViewBag.Area = new SelectList(db.MM_Area.Where(s => s.Shop_ID == Shop_ID), "Area_ID", "Area_Name");



                // CategoryRatedLoad Data
                {
                    var CategoryRatedLoad = (from r in db.MM_RatedLoad
                                             where r.Shop_ID == Shop_ID
                                             select new
                                             {
                                                 r.Category_ID,
                                                 r.Percentage
                                             }).ToList();
                    List<Barchart> Category_Load = new List<Barchart>();
                    foreach (var id in CategoryRatedLoad.OrderByDescending(s => s.Percentage))
                    {
                        var Name = db.MM_Category.Where(s => s.Category_Id == id.Category_ID).Select(s => s.Category_Name).FirstOrDefault();
                        Barchart obj3 = new Barchart(Name, Convert.ToDouble(id.Percentage), "Add", 1);
                        Category_Load.Add(obj3);
                    }
                    ViewData["CategoryRatedLoad"] = Category_Load;
                }

                // Summary
                {

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


                    foreach (var id in shopwie_Data)
                    {
                        string ddl = Convert.ToDateTime(id.DateandTime).ToString("hh:mm tt");

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

                    var totalconsume = shopwie_Data
                        .Sum(s => s.TotalConsumption);
                    var totalavg = shopwie_Data.Average(s => s.TotalConsumption);
                    var totalprod = shopwie_Data.Sum(s => s.Production);
                    var totalavgsec = shopwie_Data.Average(s => s.Consumption);

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
                }


                var consumetype = db.MM_Parameter.Where(s => s.Prameter_ID == 1).Select(s => s.Unit).FirstOrDefault();
                ViewBag.ConsumeType = consumetype;
                ViewBag.Type = new SelectList(db.MM_ConsumptionType, "Consumption_ID", "ConsumptionName");

                //Generation
                var data = (from s in db.MM_MTTUW_Shops
                            join
                            g in db.MM_ShopsCategory on
                            s.ShopsCat_ID equals g.ShopsCat_ID
                            where s.Shop_ID == Shop_ID && s.Energy==true
                            select new
                            {
                                g.Is_Consumption
                            }).FirstOrDefault();
                var Generation = "Consumption";
                if (data != null)
                {
                    if (data.Is_Consumption == false)
                    {
                        Generation = "Generation";
                    }

                }
                ViewBag.Generation = Generation;

                return View("RealTimeParameterDashborad");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
        }

        public class Mailalert
        {
            int Feeder { get; set; }
            public Mailalert(int feeder)
            {
                this.Feeder = feeder;

            }
        }
        public class ParameterSub
        {
            public string Name { get; set; }
            public int Subcount { get; set; }

            public ParameterSub(string name, int count)
            {
                this.Name = name;
                this.Subcount = count;
            }
        }

        public class LastUpdatedate
        {
            int Feeder { get; set; }
            string lastdate { get; set; }
            public LastUpdatedate(int feeder, string date)
            {
                this.Feeder = feeder;
                this.lastdate = date;

            }
        }


        public ActionResult CommanMethod(int Shop_ID)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;


            DateTime fromdate = DateTime.Now;
            DateTime toDate = DateTime.Now;
            DateTime date = DateTime.Now.Date;
            DateTime dtn = DateTime.Now.Date;
            var time = TimeSpan.Parse("06:30:00.000");
            var time1 = TimeSpan.Parse("06:29:00.000");
            date = date.AddDays(-1);
            fromdate = (date + time);
            toDate = (date.AddDays(1) + time1);


            //DateTime startdate1 = DateTime.Parse("2018-05-05");
            //DateTime enddate1 = DateTime.Parse("2018-05-06");
            //// yesturday Consumption 
            //DateTime startdate = DateTime.Parse("2018-05-03");
            //DateTime enddate = DateTime.Parse("2018-05-04");
            //var time = TimeSpan.Parse("06:45:00.000");
            //var time1 = TimeSpan.Parse("06:44:00.000");
            //startdate = (startdate + time);
            //enddate = (enddate + time1);
            //ViewBag.yesterdaydate = startdate;
            //double office = 0;
            //double street = 0;
            // Pie chart

            List<Sp_CategorywiseConsumption_Result> pie = db.Sp_CategorywiseConsumption(Shop_ID, fromdate, toDate, "", "", plantID, 1, true).ToList();

            // List<PieSeriesData> pieData = new List<PieSeriesData>();
            List<Barchart> pieData = new List<Barchart>();

            JsonConvert.SerializeObject(pie);
            return Json(pie, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Feederconsumption(string Formdate, string todate, string ddlformat, int Shop, int? Shiftwise, string Minute, int? Reason, string ConsumptionId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }
                List<Sp_DailyShopwiseConsumption_New_Result> pie = new List<Sp_DailyShopwiseConsumption_New_Result>();
                List<Sp_No_WorkingDay_Data_Result> pie_NoWorkingData = new List<Sp_No_WorkingDay_Data_Result>();
                List<Sp_CategorywiseConsumption_Result> Category = new List<Sp_CategorywiseConsumption_Result>();
                List<Sp_Shift_AreawiseShop_Result> AreaIncomer = new List<Sp_Shift_AreawiseShop_Result>();
                List<Barchart> SubFeederData = new List<Barchart>();
                List<Sp_Categorywise_No_Working_Data_Result> Category_NoWorkingData = new List<Sp_Categorywise_No_Working_Data_Result>();
                List<Sp_Areawise_No_Working_Data_Result> AreaIncomer_NoWorkingData = new List<Sp_Areawise_No_Working_Data_Result>();
                List<Sp_Areawise_No_Working_Data_Result> Areaconsume_NoWorkingData = new List<Sp_Areawise_No_Working_Data_Result>();
                List<SP_LiveAreawiseConsumption_Result> AreaIncomerlive = new List<SP_LiveAreawiseConsumption_Result>();
                List<SP_LiveFeederwise_TimeConsumption_Result> pie1 = new List<SP_LiveFeederwise_TimeConsumption_Result>();
                List<Sp_ShiftwiseShopwiseConsumption_Result> Perform = new List<Sp_ShiftwiseShopwiseConsumption_Result>();
                List<SP_Shopwise_Timewise_Consumption_Result> Grain = new List<SP_Shopwise_Timewise_Consumption_Result>();

                List<SP_Shopwise_Timewise_Consumption_Result> Grainhour = new List<SP_Shopwise_Timewise_Consumption_Result>();
                List<Sp_ShiftwiseShopwise_Specific_Cons_Result> Specfic = new List<Sp_ShiftwiseShopwise_Specific_Cons_Result>();
                List<SP_Shopwise_TimeConsumptionperVechicle_Result> Grain_specfic = new List<SP_Shopwise_TimeConsumptionperVechicle_Result>();

                List<ShiftPlantData> ShopResult = new List<ShiftPlantData>();
                List<double?> BestData = new List<double?>();
                List<Summary> ShopSummary = new List<Summary>();


                List<string> allDates = new List<string>();
                List<MM_Performance_Indices_Energy> Best = new List<MM_Performance_Indices_Energy>();
                List<double?> cumulative = new List<double?>();
                List<double?> Averge = new List<double?>();
                double cumulativedata = 0.0;
                double avgdata = 0.0;
                int avg = 1;

                DateTime startdate1 = DateTime.Parse(Formdate);
                DateTime enddate1 = DateTime.Parse(todate);

                DateTime endg = DateTime.Parse(Formdate);
                DateTime startg = DateTime.Parse(Formdate);
                DateTime todaydate = DateTime.Now.Date;

                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;

                List<Barchart> pieData = new List<Barchart>();
                List<Barchart> CategoryData = new List<Barchart>();
                var ShopId = (from s in db.MM_MTTUW_Shops
                              join
                               u in db.MM_Specific_Cosume_Unit on
                               s.Spec_Unit_ID equals u.Spec_Unit_ID
                              where s.Shop_ID == Shop && s.Energy == true
                              select new
                              {
                                  u.Spec_Unit_ID,
                                  u.Unit_Name,
                                  s.Shop_Name
                              }).FirstOrDefault();
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID == 25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).FirstOrDefault();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                enddate1 = enddate1.AddDays(1);
                enddate1 = (enddate1 + starttime);
                startdate1 = (startdate1 + starttime);


                var starttime1 = "";
                var endtime1 = "";
                var endmintime = "";
                //var shiftPerform = db.MM_Shiftwise_Consume_Power.Where(s => s.ConsumptionType == type && s.Income_Power == type && s.Dateandtime >= startdate1 && s.Dateandtime <= enddate1).ToList();
                //var  ShopGrain = db.MM_Shopwise_TimeConsumption.Where(s=>s.Shop_ID ==0)

                if (Shiftwise != null)
                {
                    var shiftId = Convert.ToInt32(Shiftwise);
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
                    startg = (startd + startt);
                    //endg = endg.AddDays(1);
                    if (shiftId == 3)
                    {
                        endg = endg.AddDays(1);

                    }
                    endg = (endg + Endt);

                    startd = (startd + startt);
                    endd = (endd + Endt);

                    ViewData["start"] = startd.ToString("dd/MMM hh:mm: tt");
                    ViewData["end"] = endd.ToString("dd/MMM hh:mm tt");

                }
                else
                {
                    DateTime startd = DateTime.Parse(Formdate);
                    DateTime endd = DateTime.Parse(todate);
                    startd = (startd + starttime);
                    endd = (endd + starttime);
                    startg = DateTime.Parse(Formdate);
                    startg = (startg + starttime);
                    endg = endg.AddDays(1);
                    endg = (endg + starttime);
                    ViewData["start"] = startd.ToString("dd/MMM hh:mm tt");
                    ViewData["end"] = endd.ToString("dd/MMM hh:mm tt");
                }



                if (Minute != "" && (ddlformat == "2" || ddlformat == "5" || ddlformat == "1"))
                {

                    if (ddlformat == "1")
                    {
                        var Today = System.DateTime.Now;



                        Category = db.Sp_CategorywiseConsumption(Shop, startdate1, Today, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                        pie1 = db.SP_LiveFeederwise_TimeConsumption(Shop, startdate1, Today, starttime1, endmintime, plantID, Shiftwise, type).ToList();
                        AreaIncomerlive = db.SP_LiveAreawiseConsumption(startdate1, Today, starttime1, endtime1, plantID, Shiftwise, Shop, true, type).ToList();

                        Grain = db.SP_Shopwise_Timewise_Consumption(plantID, startg, Today, Shop, Convert.ToInt32(Minute), type).ToList();
                        // Grain=  db.MM_Shopwise_TimeConsumption.Where(s => s.Shop_ID == Shop && s.DateandTime > startg && s.DateandTime <= Today && s.ConsumptionType == type).ToList();
                        if (ShopId.Spec_Unit_ID != 5)
                        {
                            Grain_specfic = db.SP_Shopwise_TimeConsumptionperVechicle(plantID, startg, Today, Shop, Convert.ToInt32(Minute), type).ToList();
                        }
                        else if (Minute != "60")
                        {
                            Grainhour = db.SP_Shopwise_Timewise_Consumption(plantID, startg, Today, Shop, Convert.ToInt32(60), type).ToList();
                        }




                    }
                    else
                    {

                        Category = db.Sp_CategorywiseConsumption(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                        pie = db.Sp_DailyShopwiseConsumption_New(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                        AreaIncomer = db.Sp_Shift_AreawiseShop(startg, endg, starttime1, endtime1, plantID, Shiftwise, Shop, true, type).ToList();

                        Grain = db.SP_Shopwise_Timewise_Consumption(plantID, startg, endg, Shop, Convert.ToInt32(Minute), type).ToList();

                        //Grain = db.MM_Shopwise_TimeConsumption.Where(s => s.Shop_ID == Shop && s.DateandTime > startg && s.DateandTime <= endg && s.ConsumptionType == type).ToList();


                        //AreaConsumer = db.Sp_Shift_AreawiseShop(startg, endg, starttime1, endtime1, plantID, Shiftwise, Shop, false, type).ToList();

                        if (ShopId.Spec_Unit_ID != 5)
                        {
                            Grain_specfic = db.SP_Shopwise_TimeConsumptionperVechicle(plantID, startg, endg, Shop, Convert.ToInt32(Minute), type).ToList();
                        }
                        else if (Minute != "60")
                        {
                            Grainhour = db.SP_Shopwise_Timewise_Consumption(plantID, startg, endg, Shop, Convert.ToInt32(60), type).ToList();
                        }


                    }
                }
                else
                {

                    Perform = db.Sp_ShiftwiseShopwiseConsumption(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                    Specfic = db.Sp_ShiftwiseShopwise_Specific_Cons(Shop, startdate1, enddate1, starttime1, endtime1, plantID).ToList();
                    pie = db.Sp_DailyShopwiseConsumption_New(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                    Category = db.Sp_CategorywiseConsumption(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                    AreaIncomer = db.Sp_Shift_AreawiseShop(startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, Shop, true, type).ToList();
                    //AreaConsumer = db.Sp_Shift_AreawiseShop(startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, Shop, false, type).ToList();
                    if (Shiftwise == null)
                    {
                        Best = (from p in db.MM_Performance_Indices_Energy
                                where p.Shop_ID == Shop && p.DateandTime >= startdate1.Date && p.DateandTime < enddate1.Date && p.ConsumptionType == type
                                select
                                p).ToList();
                    }
                    else
                    {


                    }
                }


                var otherconsume = (from m in db.MM_Shiftwise_Consume_Power
                                    where m.Dateandtime >= startdate1 && m.Dateandtime <= enddate1 && m.Shop_ID == Shop
                                    && m.Income_Power == false && m.ConsumptionType == type
                                    select
                                    new { m.Consumption }).Sum(s => s.Consumption);

                if (Shiftwise != null && Minute != null)
                {


                    otherconsume = (from m in db.MM_Shiftwise_Consume_Power
                                    where m.Dateandtime >= startg && m.Dateandtime <= startg && m.Shop_ID == Shop
                                    && m.Income_Power == false
                                    select
                                    new { m.Consumption }).Sum(s => s.Consumption);
                }
                else if (Shiftwise != null)
                {
                    otherconsume = (from m in db.MM_Shiftwise_Consume_Power
                                    where m.Dateandtime >= startdate1 && m.Dateandtime <= enddate1 && m.Shop_ID == Shop && m.Shift_ID == Shiftwise
                                    && m.Income_Power == false && m.ConsumptionType == type
                                    select
                                    new { m.Consumption }).Sum(s => s.Consumption);
                }
                double FeederTotal = 0.0;
                Double Others = 0.0;
                if (pie.Count() > 0)
                {

                    foreach (var id in pie)
                    {
                        string function = id.TagBoolean == true ? "Add" : "Subtract";
                        //string function = "Add";
                        Double Y = Math.Round(Convert.ToDouble(id.Comsumtionvalues), 2);
                        Barchart obj1 = new Barchart(id.FeederName, Y, function, 1);
                        if (id.TagBoolean == true)
                        {
                            pieData.Add(obj1);
                        }
                        else
                        {
                            SubFeederData.Add(obj1);
                        }

                    }
                    FeederTotal = Convert.ToDouble(pieData.Sum(s => s.Y));
                    if (otherconsume != null)
                    {
                        Barchart obj1 = new Barchart("Other", otherconsume.Value, "Add", 0);
                        pieData.Add(obj1);
                    }
                    ViewData["pieData1"] = pieData;
                    ViewData["ParetoData"] = pieData.OrderByDescending(s => s.Y);
                    ViewBag.pieData = JsonConvert.SerializeObject(pieData);
                    // ViewData["feederData1"] = FeederData;
                    ViewData["SubFeederData"] = SubFeederData;
                }
                if (pie1.Count() > 0)
                {

                    foreach (var id in pie1)
                    {
                        string function = id.TagBoolean == true ? "Add" : "Subtract";
                        //string function = "Add";
                        Double Y = Math.Round(Convert.ToDouble(id.Consumption), 2);
                        Barchart obj1 = new Barchart(id.FeederName, Y, function, 1);
                        if (id.TagBoolean == true)
                        {
                            pieData.Add(obj1);
                        }
                        else
                        {
                            SubFeederData.Add(obj1);
                        }

                    }
                    FeederTotal = Convert.ToDouble(pieData.Sum(s => s.Y));
                    var todayshop = Grain.Sum(s => s.totalconsumption);
                    otherconsume = pie1.Sum(s => s.Consumption);
                    otherconsume = Math.Round(Convert.ToDouble(todayshop - otherconsume), 0);
                    if (otherconsume != null)
                    {
                        Barchart obj1 = new Barchart("Other", otherconsume.Value, "Add", 0);
                        pieData.Add(obj1);
                    }
                    ViewData["pieData1"] = pieData;
                    ViewData["ParetoData"] = pieData.OrderByDescending(s => s.Y);
                    ViewBag.pieData = JsonConvert.SerializeObject(pieData);
                    ViewData["SubFeederData"] = SubFeederData;
                }

                foreach (var id in Category)
                {
                    Barchart obj3 = new Barchart(id.Category_Name, Convert.ToDouble(id.Comsumtionvalues), "Add", 1);
                    CategoryData.Add(obj3);
                }

                //  ViewBag.CategoryData1 = JsonConvert.SerializeObject(CategoryData);

                ViewData["CategoryData"] = CategoryData;
                List<Areachart> AreaInomerData = new List<Areachart>();
                List<Barchart> AreaConsumeData = new List<Barchart>();

                if (AreaIncomer.Count() > 0)
                {
                    foreach (var id in AreaIncomer)
                    {
                        Areachart obj3 = new Areachart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), Convert.ToInt32(id.AreaId));
                        AreaInomerData.Add(obj3);
                    }
                    Others = (FeederTotal - Convert.ToDouble(AreaInomerData.Sum(s => s.Y)));
                    Areachart obj5 = new Areachart("Others", Convert.ToDouble(Others), Convert.ToInt32(0));
                    AreaInomerData.Add(obj5);
                    ViewData["AreaINData"] = AreaInomerData;
                }
                else
                {
                    foreach (var id in AreaIncomerlive)
                    {
                        Areachart obj3 = new Areachart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), Convert.ToInt32(id.AreaId));
                        AreaInomerData.Add(obj3);
                    }
                    Others = Math.Round(FeederTotal - Convert.ToDouble(AreaInomerData.Sum(s => s.Y)), 1);
                    Areachart obj5 = new Areachart("Others", Convert.ToDouble(Others), Convert.ToInt32(0));
                    AreaInomerData.Add(obj5);
                    ViewData["AreaINData"] = AreaInomerData;
                }



                if (Minute != "" && (ddlformat == "2" || ddlformat == "5" || ddlformat == "1"))
                {
                    ViewData["Shopwise_Data"] = Grain;
                    foreach (var id in Grain.OrderBy(s => s.ConsumptionDate))
                    {
                        string ddl = Convert.ToDateTime(id.ConsumptionDate).ToString("hh:mm tt");
                        cumulativedata += Math.Round(Convert.ToDouble(id.totalconsumption), 0);
                        avgdata = Math.Round(Convert.ToDouble(cumulativedata / avg), 0);
                        cumulative.Add(cumulativedata);
                        allDates.Add(ddl);
                        Averge.Add(avgdata);
                        avg += 1;



                    }

                    ViewData["Shopwise_Data"] = Grain;
                    ViewData["Shopwise_Date"] = allDates;
                    ViewData["cumulative"] = cumulative;
                    ViewData["Averge"] = Averge;
                    if (ShopId.Spec_Unit_ID == 5 && Minute != "60")
                    {
                        List<double?> totalconsumption = new List<double?>();
                        foreach (var id in Grain)
                        {
                            var consumption = Grainhour.Where(s => s.ConsumptionDate == id.ConsumptionDate).FirstOrDefault();
                            if (consumption != null)
                            {
                                totalconsumption.Add(Convert.ToDouble(consumption.totalconsumption));
                            }
                            else
                            {


                                totalconsumption.Add(null);
                            }
                        }
                        ViewData["Shopwise_specfic_Data"] = totalconsumption;
                    }
                    else if (ShopId.Spec_Unit_ID == 5)
                    {

                        ViewData["Shopwise_specfic_Data"] = Grain.ToList();
                    }
                    else
                    {
                        ViewData["Shopwise_specfic_Data"] = Grain_specfic.ToList();
                    }

                }
                else
                {
                    avg = 1;
                    foreach (var id in Perform)
                    {
                        string ddl = Convert.ToDateTime(id.ConsumptionDate).ToString("dd/MMM/yy");
                        allDates.Add(ddl);
                        cumulativedata += Math.Round(Convert.ToDouble(id.totalconsumption), 0);
                        avgdata = Math.Round(Convert.ToDouble(cumulativedata / avg), 0);

                        cumulative.Add(cumulativedata);
                        Averge.Add(avgdata);
                        avg += 1;



                    }
                    if (Shiftwise != null)
                    {
                        ViewData["Shopwise_Data"] = Perform;
                        ViewData["Shopwise_Date"] = allDates;
                        ViewData["Shopwise_specfic_Data"] = Specfic.ToList();
                        ViewData["cumulative"] = cumulative.ToList();
                        ViewData["Averge"] = Averge.ToList();
                    }
                    else
                    {
                        cumulativedata = 0.0;
                        avg = 1;
                        cumulative = new List<double?>();
                        Averge = new List<double?>();
                        BestData = new List<double?>();
                        var best1Data = 0.0;

                        foreach (var id in Best)
                        {
                            double bestdata = 0;
                            if (best1Data == id.Best)
                            {
                                BestData.Add(null);
                            }
                            else
                            {
                                best1Data = Convert.ToDouble(id.Best);
                                BestData.Add(best1Data);
                            }
                            cumulativedata += Math.Round(Convert.ToDouble(id.TotalConsumption), 0);
                            avgdata = Math.Round(Convert.ToDouble(cumulativedata / avg), 0);
                            cumulative.Add(cumulativedata);
                            Averge.Add(avgdata);
                            avg += 1;
                            ShiftPlantData obj = new ShiftPlantData(
                                   Convert.ToDouble(id.Consumption),
                                   Convert.ToDouble(id.TotalConsumption),
                                  Convert.ToInt32(id.Production), bestdata, id.DateandTime.ToString());

                            ShopResult.Add(obj);
                        }

                        ViewData["Shopwise_Data"] = Best;
                        ViewData["Shopwise_best"] = BestData;
                        ViewData["Shopwise_Date"] = allDates;
                        ViewData["Shopwise_specfic_Data"] = Specfic.ToList();
                        ViewData["cumulative"] = cumulative.ToList();
                        ViewData["Averge"] = Averge.ToList();
                    }

                }

                var unit = ShopId.Unit_Name.Replace("\r\n", "");
                ViewBag.specUnit = unit;
                ViewBag.UnitId = ShopId.Spec_Unit_ID;
                ViewBag.Minute = Minute;
                ViewBag.Shiftwise = Shiftwise;
                ViewBag.ddlforamte = ddlformat;

                // Target
                List<double?> Target = new List<double?>();
                string month = startdate1.ToString("MMMM");
                // target list 
                var Target1 = (from t in db.MM_PowerTarget
                               join
                                s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                               where t.Year == FinYear && t.Month == month && t.TargetType == 1
                               select new
                               {
                                   t.Target,
                                   s.Shop_Name,
                                   s.Shop_ID
                               }).FirstOrDefault();
                if (Target1 == null)
                {
                    CurYear = startdate1.Year.ToString();
                    var pre = startdate1.AddYears(-1);

                    PreYear = pre.Year.ToString();
                    //CurYear = CurrentYear.ToString();
                    FinYear = PreYear + "-" + CurYear;
                    Target1 = (from t in db.MM_PowerTarget
                               join
                                s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                               where t.Year == FinYear && t.Month == month && t.TargetType == 1
                               select new
                               {
                                   t.Target,
                                   s.Shop_Name,
                                   s.Shop_ID
                               }).FirstOrDefault();
                }

                if (Reason != null)
                {
                    List<Noworkingdata> NoworkingDayobj = new List<Noworkingdata>();
                    List<Noworkingdata> NoworkingDayspec = new List<Noworkingdata>();
                    List<Noworkingdata> Other = new List<Noworkingdata>();
                    List<Areachart> AreaInomerNoworkingdata = new List<Areachart>();
                    // List<Barchart> AreaConsumeNoworkingdata = new List<Barchart>();
                    cumulativedata = 0.0;
                    avg = 1;
                    cumulative = new List<double?>();
                    Averge = new List<double?>();



                    Category_NoWorkingData = db.Sp_Categorywise_No_Working_Data(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Reason, type).ToList();
                    AreaIncomer_NoWorkingData = db.Sp_Areawise_No_Working_Data(startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, Shop, true, type, Reason).ToList();
                    Areaconsume_NoWorkingData = db.Sp_Areawise_No_Working_Data(startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, Shop, false, type, Reason).ToList();

                    pie_NoWorkingData = db.Sp_No_WorkingDay_Data(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Reason, type).ToList();
                    allDates = new List<string>();
                    pieData = new List<Barchart>();
                    SubFeederData = new List<Barchart>();

                    CategoryData = new List<Barchart>();
                    var noworkingday = (from n in db.MM_No_Working_Day
                                        where n.Reason_ID == Reason && n.Day_Date >= startdate1 && n.Day_Date <= enddate1
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
                                var noworking = Perform.Where(s => s.ConsumptionDate == id.Day_Date).Select(c => c.totalconsumption).FirstOrDefault();
                                if (noworking != null)
                                {
                                    Noworkingdata obj = new Noworkingdata(0, Convert.ToDouble(noworking.Value));
                                    NoworkingDayobj.Add(obj);
                                }
                                else
                                {
                                    Noworkingdata obj = new Noworkingdata(0, 0);
                                    NoworkingDayobj.Add(obj);
                                }
                                cumulativedata += Math.Round(Convert.ToDouble(noworking.Value), 0);
                                avgdata = Math.Round(Convert.ToDouble(cumulativedata / avg), 0);
                                cumulative.Add(cumulativedata);
                                Averge.Add(avgdata);
                                avg += 1;
                            }
                            else
                            {

                                var Consumption = Best.Where(s => s.DateandTime == id.Day_Date).Select(s => s.Consumption).FirstOrDefault();
                                var Production = Best.Where(s => s.DateandTime == id.Day_Date).Select(s => s.Production).FirstOrDefault();
                                var BOB = Best.Where(s => s.DateandTime == id.Day_Date).Select(s => s.Best).FirstOrDefault();
                                var Total = Best.Where(s => s.DateandTime == id.Day_Date).Select(s => s.TotalConsumption).FirstOrDefault();
                                if (Consumption != null)
                                {
                                    Noworkingdata obj = new Noworkingdata(Consumption.Value, Production.Value, BOB.Value, Total.Value);
                                    NoworkingDayobj.Add(obj);
                                }
                                else
                                {
                                    Noworkingdata obj = new Noworkingdata(0, 0);
                                    NoworkingDayobj.Add(obj);
                                }
                                cumulativedata += Math.Round(Convert.ToDouble(Total.Value), 0);
                                avgdata = Math.Round(Convert.ToDouble(cumulativedata / avg), 0);
                                cumulative.Add(cumulativedata);
                                Averge.Add(avgdata);
                                avg += 1;

                            }
                            allDates.Add(id.Day_Date.ToString("dd/MMM/yy"));
                            var SpecficData = Specfic.Where(s => s.ConsumptionDate == id.Day_Date).Select(s => s.Consumption).FirstOrDefault();
                            if (SpecficData != null)
                            {
                                Noworkingdata obj = new Noworkingdata(SpecficData.Value, SpecficData.Value);
                                NoworkingDayspec.Add(obj);
                            }
                            var otherdata = (from m in db.MM_Shiftwise_Consume_Power
                                             where m.Dateandtime >= startdate1 && m.Dateandtime <= enddate1 && m.Shop_ID == Shop
                                             && m.Income_Power == false && m.ConsumptionType == type
                                             select
                                             new { m.Consumption, m.Dateandtime }).ToList();
                            //otherdata = ManualConsumtion.OrderByDescending(s => s.ConsumptionDate).ToList();

                            otherdata = otherdata.Where(s => Convert.ToDateTime(s.Dateandtime).Year == id.Day_Date.Year
                                                                       && Convert.ToDateTime(s.Dateandtime).Month == id.Day_Date.Month
                                                                       && Convert.ToDateTime(s.Dateandtime).Day == id.Day_Date.Day).ToList();
                            var otherdata1 = otherdata.Select(s => s.Consumption).Sum(s => s.Value);
                            Noworkingdata obj1 = new Noworkingdata(Convert.ToDouble(otherdata1), Convert.ToDouble(otherdata1));

                            Other.Add(obj1);

                        }

                    }
                    if (pie_NoWorkingData.Count() > 0)
                    {

                        foreach (var id in pie_NoWorkingData.Where(s => s.TagBoolean == true))
                        {
                            string function = id.TagBoolean == true ? "Add" : "Subtract";
                            Double Y = Math.Round(Convert.ToDouble(id.Comsumtionvalues), 2);
                            Barchart obj1 = new Barchart(id.FeederName, Y, function, 1);
                            if (id.TagBoolean == true)
                            {
                                pieData.Add(obj1);
                            }
                            else
                            {
                                SubFeederData.Add(obj1);
                            }

                        }
                        FeederTotal = Convert.ToDouble(pieData.Sum(s => s.Y));
                        if (Other.Count() > 0)
                        {
                            var otherdata = Other.Sum(s => s.totalconsumption);
                            Barchart obj1 = new Barchart("Other", otherdata, "Add", 0);
                            pieData.Add(obj1);
                        }

                        ViewData["pieData1"] = pieData;
                        ViewData["ParetoData"] = pieData.OrderByDescending(s => s.Y);
                        ViewBag.pieData = JsonConvert.SerializeObject(pieData);
                        ViewData["SubFeederData"] = SubFeederData;
                    }
                    else
                    {
                        ViewData["pieData1"] = pieData.OrderByDescending(s => s.Consumption);
                        ViewBag.pieData = JsonConvert.SerializeObject(pieData);
                        ViewData["SubFeederData"] = SubFeederData;
                    }
                    foreach (var id in Category_NoWorkingData)
                    {
                        Barchart obj3 = new Barchart(id.Category_Name, Convert.ToDouble(id.Comsumtionvalues), "Add", 1);
                        CategoryData.Add(obj3);
                    }
                    foreach (var id in AreaIncomer_NoWorkingData)
                    {
                        Areachart obj3 = new Areachart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), Convert.ToInt32(id.Area_Id));
                        AreaInomerNoworkingdata.Add(obj3);
                    }
                    Others = FeederTotal - Convert.ToDouble(AreaInomerNoworkingdata.Sum(s => s.Y));
                    Areachart obj5 = new Areachart("Others", Convert.ToDouble(Others), Convert.ToInt32(0));
                    AreaInomerNoworkingdata.Add(obj5);
                    //foreach (var id in Areaconsume_NoWorkingData)
                    //{
                    //    Barchart obj3= new Barchart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), "Add");
                    //    AreaConsumeNoworkingdata.Add(obj3);
                    //}
                    //ViewBag.CategoryData1 = JsonConvert.SerializeObject(CategoryData);
                    ViewData["CategoryData"] = CategoryData;
                    ViewData["Shopwise_Data"] = NoworkingDayobj;
                    ViewData["Shopwise_Date"] = allDates;
                    ViewData["Shopwise_specfic_Data"] = NoworkingDayspec;
                    ViewData["AreaINData"] = AreaInomerNoworkingdata;
                    ViewData["cumulative"] = cumulative;
                    ViewData["Averge"] = Averge;
                    // ViewData["AreaconsumData"] = AreaConsumeNoworkingdata;

                }

                foreach (var id in allDates)
                {
                    if (Target1 != null)
                    {
                        Target.Add(Math.Round(Convert.ToDouble(Target1.Target), 0));
                    }
                    else
                    {
                        Target.Add(null);
                    }


                }
                ViewData["Target"] = Target.ToList();
                if (type == true)
                {
                    var consumetype = db.MM_Parameter.Where(s => s.Prameter_ID == 1).Select(s => s.Unit).FirstOrDefault();
                    ViewBag.ConsumeType = consumetype;
                }
                else
                {

                    ViewBag.ConsumeType = "KVAH";

                }

                //Shop Summary
                {
                    List<double?> Cumulative = new List<double?>();
                    var best1Data = 0.0;
                    double best = 0;
                    cumulativedata = 0;
                    avgdata = 0.0;
                    avg = 1;
                    int bestprod = 0;
                    var bestdate = "";
                    double bestpower = 0.0;
                    double maxpower = 0.0;
                    var maxdate = "";
                    foreach (var id in ShopResult)
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
                    var totalconsume = Math.Round(ShopResult.Sum(s => s.TotalConsumption), 0);
                    if (ShopResult.Count() > 0)
                    {


                        var totalavg = Math.Round(ShopResult.Average(s => s.TotalConsumption), 0);
                        var totalprod = ShopResult.Sum(s => s.Production);
                        var totalavgsec = Math.Round(ShopResult.Average(s => s.Consumption), 0);
                        Summary summaryobj = new Summary(totalconsume, totalavg, totalavgsec, totalprod, best, bestpower, bestprod, maxpower, bestdate, maxdate);
                        ShopSummary.Add(summaryobj);
                    }
                }

                return PartialView("_FeederwiseConsumption");
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult ShopwiseSummay(string Formdate, string todate, string ddlformat, int Shop, int? Shiftwise, string Minute, string Noworking, string ConsumptionId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }
                double total = 0;
                DateTime startdate1 = DateTime.Parse(Formdate);
                DateTime enddate1 = DateTime.Parse(todate);

                DateTime start_m = DateTime.Parse(Formdate);
                DateTime end_m = DateTime.Parse(todate);
                var hours = (end_m - start_m).TotalHours;

                DateTime endg = DateTime.Parse(Formdate);
                DateTime startg = DateTime.Parse(Formdate);

                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;
                var Perform = (from p in db.MM_Performance_Indices_Energy
                               where (p.Inserted_Date) >= startdate1 && (p.Inserted_Date) <= enddate1 && p.ConsumptionType == type && p.Shop_ID == Shop
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
                var ShopGrin = (from b in db.MM_Shopwise_TimeConsumption
                                where b.ConsumptionType == type &&
                               b.DateandTime == startdate1 && b.Shop_ID == Shop
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

                var shiftPerform = db.MM_Shiftwise_Consume_Power.Where(s => s.ConsumptionType == type && s.Income_Power == true && s.Dateandtime >= startdate1 && s.Dateandtime <= enddate1 && s.Shop_ID == Shop).ToList();
                List<Sp_DailyShopwiseConsumption_New_Result> pie = new List<Sp_DailyShopwiseConsumption_New_Result>();
                List<Sp_No_WorkingDay_Data_Result> pie_NoWorkingData = new List<Sp_No_WorkingDay_Data_Result>();
                List<Sp_CategorywiseConsumption_Result> Category = new List<Sp_CategorywiseConsumption_Result>();
                List<Sp_Shift_AreawiseShop_Result> AreaIncomer = new List<Sp_Shift_AreawiseShop_Result>();

                List<Sp_Categorywise_No_Working_Data_Result> Category_NoWorkingData = new List<Sp_Categorywise_No_Working_Data_Result>();
                List<Sp_Areawise_No_Working_Data_Result> AreaIncomer_NoWorkingData = new List<Sp_Areawise_No_Working_Data_Result>();
                List<Sp_Areawise_No_Working_Data_Result> Areaconsume_NoWorkingData = new List<Sp_Areawise_No_Working_Data_Result>();
                List<SP_LiveAreawiseConsumption_Result> AreaIncomerlive = new List<SP_LiveAreawiseConsumption_Result>();
                List<SP_LiveFeederwise_TimeConsumption_Result> pie1 = new List<SP_LiveFeederwise_TimeConsumption_Result>();


                List<double?> BestData = new List<double?>();
                List<Barchart> CategoryData = new List<Barchart>();
                List<string> allDates = new List<string>();
                List<double?> Target = new List<double?>();
                List<double?> PlantTarget = new List<double?>();
                List<Barchart> SubFeederData = new List<Barchart>();
                List<Barchart> AddFeederData = new List<Barchart>();
                List<Barchart> FeederParetoData = new List<Barchart>();
                List<Areachart> AreaInomerData = new List<Areachart>();
                List<Barchart> AreaConsumeData = new List<Barchart>();


                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID==25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).First();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                enddate1 = enddate1.Date.AddDays(1);
                enddate1 = (enddate1.Date + starttime);
                startdate1 = (startdate1.Date + starttime);
                var starttime1 = "";
                var endtime1 = "";
                var endmintime = "";
                int shiftId = 0;
                string starts = "";
                string ends = "";
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
                        endg = endg.Date.AddDays(1);

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
                    endd = endd.Date.AddDays(1);
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

                string month = startdate1.ToString("MMMM");

                var ShopId = (from s in db.MM_MTTUW_Shops
                              join
                               u in db.MM_Specific_Cosume_Unit on
                               s.Spec_Unit_ID equals u.Spec_Unit_ID
                              where s.Shop_ID == Shop && s.Energy == true
                              select new
                              {
                                  u.Spec_Unit_ID,
                                  u.Unit_Name,
                                  s.Shop_Name
                              }).FirstOrDefault();

                var unit = ShopId.Unit_Name.Replace("\r\n", "");
                ViewBag.specUnit = unit;
                ViewBag.UnitId = ShopId.Spec_Unit_ID;



                if (Minute != null && (ddlformat == "2" || ddlformat == "1" || (ddlformat == "5" && hours <= 24)))
                {
                    var todday1 = System.DateTime.Now;
                    if (ddlformat == "1")
                    {
                        var today = System.DateTime.Now;
                        ShopGrin = (from b in db.MM_Shopwise_TimeConsumption
                                    where b.Shop_ID == Shop && b.ConsumptionType == type &&
                                   b.DateandTime > startdate1 && b.DateandTime <= today
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
                        foreach (var id in ShopGrin)
                        {
                            string ddl = id.Time;
                            allDates.Add(ddl);
                        }
                        Category = db.Sp_CategorywiseConsumption(Shop, startdate1, today, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                        pie1 = db.SP_LiveFeederwise_TimeConsumption(Shop, startdate1, today, starttime1, endmintime, plantID, Shiftwise, type).ToList();
                        AreaIncomerlive = db.SP_LiveAreawiseConsumption(startdate1, today, starttime1, endtime1, plantID, Shiftwise, Shop, true, type).ToList();
                    }
                    else if (ddlformat == "5" && hours <= 24)
                    {
                        ShopGrin = (from b in db.MM_Shopwise_TimeConsumption
                                    where b.Shop_ID == Shop && b.ConsumptionType == type &&
                                   b.DateandTime > start_m && b.DateandTime <= end_m
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
                        foreach (var id in ShopGrin)
                        {
                            string ddl = id.Time;
                            allDates.Add(ddl);
                        }
                        Category = db.Sp_CategorywiseConsumption(Shop, start_m, end_m, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                        pie1 = db.SP_LiveFeederwise_TimeConsumption(Shop, start_m, end_m, starttime1, endmintime, plantID, Shiftwise, type).ToList();
                        AreaIncomerlive = db.SP_LiveAreawiseConsumption(start_m, end_m, starttime1, endtime1, plantID, Shiftwise, Shop, true, type).ToList();
                    }
                    else
                    {
                        ShopGrin = (from b in db.MM_Shopwise_TimeConsumption
                                    where b.Shop_ID == Shop && b.ConsumptionType == type &&
                                   b.DateandTime > startdate1 && b.DateandTime <= endg
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
                        foreach (var id in ShopGrin)
                        {
                            string ddl = id.Time;
                            allDates.Add(ddl);
                        }
                        Category = db.Sp_CategorywiseConsumption(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                        pie = db.Sp_DailyShopwiseConsumption_New(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                        AreaIncomer = db.Sp_Shift_AreawiseShop(startg, endg, starttime1, endtime1, plantID, Shiftwise, Shop, true, type).ToList();
                    }
                }
                else
                {
                    Perform = (from p in db.MM_Performance_Indices_Energy
                               where (p.DateandTime) >= plantstart && (p.DateandTime) <= plantend && p.ConsumptionType == type && p.Shop_ID == Shop
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

                    if (Shiftwise != null)
                    {


                        shiftPerform = db.MM_Shiftwise_Consume_Power.Where(s => s.ConsumptionType == type && s.Income_Power == true &&
                                                                   s.Dateandtime >= startdate1 && s.Dateandtime < enddate1 && s.Shift_ID == shiftId && s.Shop_ID == Shop).ToList();
                    }

                    pie = db.Sp_DailyShopwiseConsumption_New(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                    Category = db.Sp_CategorywiseConsumption(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, type).ToList();
                    AreaIncomer = db.Sp_Shift_AreawiseShop(startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, Shop, true, type).ToList();
                }

                List<ShiftPlantData> ShopResult = new List<ShiftPlantData>();

                //Target
                FinYear = PreYear + "-" + CurYear;
                var ShopTarget = (from t in db.MM_PowerTarget
                                  where t.Year == FinYear && t.Month == month && t.Shop_ID == Shop && t.TargetType == 1 && t.Category == 2
                                  select new
                                  {
                                      t.Target
                                  }).FirstOrDefault();
                {

                    if (ShopTarget == null)
                    {
                        CurYear = startdate1.Year.ToString();
                        var pre = startdate1.AddYears(-1);
                        PreYear = pre.Year.ToString();
                        //CurYear = CurrentYear.ToString();
                        FinYear = PreYear + "-" + CurYear;
                        ShopTarget = (from t in db.MM_PowerTarget
                                      where t.Year == FinYear && t.Month == month && t.Shop_ID == Shop && t.TargetType == 1 && t.Category == 2
                                      select new
                                      {
                                          t.Target
                                      }).FirstOrDefault();
                    }
                }

                // Other Consumption
                var otherconsume = (from m in db.MM_Shiftwise_Consume_Power
                                    where m.Dateandtime >= startdate1 && m.Dateandtime < enddate1 && m.Shop_ID == Shop
                                    && m.Income_Power == false && m.ConsumptionType == type
                                    select
                                    new { m.Consumption }).Sum(s => s.Consumption);

                if (Shiftwise != null && Minute != null)
                {


                    otherconsume = (from m in db.MM_Shiftwise_Consume_Power
                                    where m.Dateandtime >= startg && m.Dateandtime <= startg && m.Shop_ID == Shop
                                    && m.Income_Power == false
                                    select
                                    new { m.Consumption }).Sum(s => s.Consumption);
                }
                else if (Shiftwise != null)
                {
                    otherconsume = (from m in db.MM_Shiftwise_Consume_Power
                                    where m.Dateandtime >= startdate1 && m.Dateandtime < enddate1 && m.Shop_ID == Shop && m.Shift_ID == Shiftwise
                                    && m.Income_Power == false && m.ConsumptionType == type
                                    select
                                    new { m.Consumption }).Sum(s => s.Consumption);
                }
                double FeederTotal = 0.0;
                Double Others = 0.0;

                //Noworking / Shiftwise/ Dayawise / hourlywise
                string[] HolidayAarry = { };
                if (Noworking != "")
                {
                    HolidayAarry = Noworking.Split(',').ToArray();
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
                                    var noworking = shiftPerform.Where(x => x.Shop_ID == Shop && Convert.ToDateTime(x.Dateandtime).Date == (id.Day_Date).Date).FirstOrDefault();
                                    if (noworking != null)
                                    {
                                        ShiftPlantData obj = new ShiftPlantData(Convert.ToDouble(noworking.SpecConsumption),
                                          Convert.ToDouble(noworking.Consumption), Convert.ToInt32(noworking.Production), 0.0, id.Day_Date.ToString("dd/MMM/yy"));
                                        ShopResult.Add(obj);
                                        string ddl = Convert.ToDateTime(noworking.Dateandtime).ToString("dd/MMM/yy");
                                        allDates.Add(ddl);

                                    }
                                }
                                else
                                {
                                    var noworking = Perform.Where(x => x.Shop_ID == Shop && x.DateandTime == id.Day_Date).FirstOrDefault();
                                    if (noworking != null)
                                    {
                                        ShiftPlantData obj = new ShiftPlantData(Convert.ToDouble(noworking.Consumption),
                                           Convert.ToDouble(noworking.TotalConsumption), Convert.ToInt32(noworking.Production),
                                            Convert.ToDouble(noworking.Best), id.Day_Date.ToString("dd/MMM/yy"));
                                        ShopResult.Add(obj);
                                        string ddl = Convert.ToDateTime(noworking.DateandTime).ToString("dd/MMM/yy");
                                        allDates.Add(ddl);

                                    }
                                }
                            }

                            // Category
                            Category_NoWorkingData = db.Sp_Categorywise_No_Working_Data(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Reason_ID, type).ToList();

                            // Feeder
                            pie_NoWorkingData = db.Sp_No_WorkingDay_Data(Shop, startdate1, enddate1, starttime1, endtime1, plantID, Reason_ID, type).ToList();

                        }


                        // Category Data
                        foreach (var id in Category_NoWorkingData)
                        {
                            Barchart obj3 = new Barchart(id.Category_Name, Convert.ToDouble(id.Comsumtionvalues), "Add", 1);
                            CategoryData.Add(obj3);
                        }

                        // Feeder Data
                        if (pie_NoWorkingData.Count() > 0)
                        {

                            foreach (var id in pie_NoWorkingData.Where(s => s.TagBoolean == true))
                            {
                                string function = id.TagBoolean == true ? "Add" : "Subtract";
                                Double Y = Math.Round(Convert.ToDouble(id.Comsumtionvalues), 2);
                                Barchart obj1 = new Barchart(id.FeederName, Y, function, 1);
                                if (id.TagBoolean == true)
                                {
                                    AddFeederData.Add(obj1);
                                }
                                else
                                {
                                    SubFeederData.Add(obj1);
                                }

                            }
                            FeederTotal = Convert.ToDouble(AddFeederData.Sum(s => s.Y));
                            //if (Other.Count() > 0)
                            //{
                            //    var otherdata = Other.Sum(s => s.totalconsumption);
                            //    Barchart obj1 = new Barchart("Other", otherdata, "Add", 0);
                            //    AddFeederData.Add(obj1);
                            //}


                        }

                        //Area  Wise Data 
                        AreaIncomer_NoWorkingData = db.Sp_Areawise_No_Working_Data(startdate1, enddate1, starttime1, endtime1, plantID, Shiftwise, Shop, true, type, Reason_ID).ToList();
                        if (AreaIncomer_NoWorkingData.Count() > 0)
                        {
                            foreach (var id in AreaIncomer_NoWorkingData)
                            {
                                Areachart obj3 = new Areachart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), Convert.ToInt32(id.Area_Id));
                                AreaInomerData.Add(obj3);
                            }
                            Others = FeederTotal - Convert.ToDouble(AreaInomerData.Sum(s => s.Y));
                            Areachart obj5 = new Areachart("Others", Convert.ToDouble(Others), Convert.ToInt32(0));
                            AreaInomerData.Add(obj5);
                        }
                    }

                }
                else
                {
                    if (Minute != "")
                    {
                        foreach (var data in ShopGrin)
                        {
                            ShiftPlantData obj = new ShiftPlantData(
                                  Convert.ToDouble(data.Consumption),
                                  Convert.ToDouble(data.TotalConsumption),
                                 Convert.ToInt32(data.Production), Convert.ToDouble(data.Best), data.Time);

                            ShopResult.Add(obj);

                        }

                    }
                    else
                    {
                        if (Shiftwise != null)
                        {
                            foreach (var data in shiftPerform.Where(s => s.Shop_ID == Shop))
                            {
                                double bestdata = 0;
                                string ddl = Convert.ToDateTime(data.Dateandtime).ToString("dd/MMM/yy");
                                ShiftPlantData obj = new ShiftPlantData(
                                    Convert.ToDouble(data.SpecConsumption),
                                    Convert.ToDouble(data.Consumption),
                                   Convert.ToInt32(data.Production), bestdata, ddl);

                                ShopResult.Add(obj);
                                allDates.Add(ddl);
                            }
                        }
                        else
                        {
                            foreach (var data in Perform.Where(s => s.Shop_ID == Shop))
                            {
                                double bestdata = Convert.ToDouble(data.Best);
                                string ddl = Convert.ToDateTime(data.DateandTime).ToString("dd/MMM/yy");
                                ShiftPlantData obj = new ShiftPlantData(
                                    Convert.ToDouble(data.Consumption),
                                    Convert.ToDouble(data.TotalConsumption),
                                   Convert.ToInt32(data.Production), bestdata, ddl);

                                ShopResult.Add(obj);
                                allDates.Add(ddl);
                                if (ShopTarget != null)
                                {
                                    var TARGETDATA = Math.Round(Convert.ToDouble(ShopTarget.Target), 1);
                                    Target.Add(TARGETDATA);
                                }
                                else
                                {
                                    Target.Add(0);
                                }
                            }
                        }
                    }

                    //Category
                    foreach (var id in Category)
                    {
                        Barchart obj3 = new Barchart(id.Category_Name, Convert.ToDouble(id.Comsumtionvalues), "Add", 1);
                        CategoryData.Add(obj3);
                    }

                    // Feeder Data
                    if (pie.Count() > 0)
                    {

                        foreach (var id in pie)
                        {
                            string function = id.TagBoolean == true ? "Add" : "Subtract";
                            //string function = "Add";
                            Double Y = Math.Round(Convert.ToDouble(id.Comsumtionvalues), 2);
                            Barchart obj1 = new Barchart(id.FeederName, Y, function, 1);
                            if (id.TagBoolean == true)
                            {
                                AddFeederData.Add(obj1);
                            }
                            else
                            {
                                SubFeederData.Add(obj1);
                            }

                        }
                        foreach (var id in AddFeederData.OrderByDescending(s => s.Y))
                        {
                            Double Y = Math.Round(Convert.ToDouble(id.Y), 2);
                            Barchart obj1 = new Barchart(id.Label, Y, "ADD", 1);
                            FeederParetoData.Add(obj1);
                        }
                        FeederTotal = Convert.ToDouble(AddFeederData.Sum(s => s.Y));
                        if (otherconsume != null)
                        {
                            Barchart obj1 = new Barchart("Other", otherconsume.Value, "Add", 0);
                            AddFeederData.Add(obj1);
                        }

                    }

                    // Feeder grinulity
                    if (pie1.Count() > 0)
                    {

                        foreach (var id in pie1)
                        {
                            string function = id.TagBoolean == true ? "Add" : "Subtract";
                            //string function = "Add";
                            Double Y = Math.Round(Convert.ToDouble(id.Consumption), 2);
                            Barchart obj1 = new Barchart(id.FeederName, Y, function, 1);
                            if (id.TagBoolean == true)
                            {
                                AddFeederData.Add(obj1);
                            }
                            else
                            {
                                SubFeederData.Add(obj1);
                            }

                        }
                        FeederTotal = Convert.ToDouble(AddFeederData.Sum(s => s.Y));
                        foreach (var id in AddFeederData.OrderByDescending(s => s.Y))
                        {
                            Double Y = Math.Round(Convert.ToDouble(id.Y), 2);
                            Barchart obj1 = new Barchart(id.Shopname, Y, "ADD", 1);
                            FeederParetoData.Add(obj1);
                        }
                        var todayshop = ShopGrin.Sum(s => s.TotalConsumption);
                        otherconsume = pie1.Sum(s => s.Consumption);
                        otherconsume = Math.Round(Convert.ToDouble(todayshop - otherconsume), 0);
                        if (otherconsume != null)
                        {
                            Barchart obj1 = new Barchart("Other", otherconsume.Value, "Add", 0);
                            AddFeederData.Add(obj1);
                        }
                    }

                    // Area Wise Data
                    if (AreaIncomer.Count() > 0)
                    {
                        foreach (var id in AreaIncomer)
                        {
                            Areachart obj3 = new Areachart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), Convert.ToInt32(id.AreaId));
                            AreaInomerData.Add(obj3);
                        }
                        Others = (FeederTotal - Convert.ToDouble(AreaInomerData.Sum(s => s.Y)));
                        Areachart obj5 = new Areachart("Others", Convert.ToDouble(Others), Convert.ToInt32(0));
                        AreaInomerData.Add(obj5);

                    }
                    else
                    {
                        foreach (var id in AreaIncomerlive)
                        {
                            Areachart obj3 = new Areachart(id.Area_Name, Convert.ToDouble(id.Comsumtionvalues), Convert.ToInt32(id.AreaId));
                            AreaInomerData.Add(obj3);
                        }
                        Others = Math.Round(FeederTotal - Convert.ToDouble(AreaInomerData.Sum(s => s.Y)), 1);
                        Areachart obj5 = new Areachart("Others", Convert.ToDouble(Others), Convert.ToInt32(0));
                        AreaInomerData.Add(obj5);

                    }

                }
                //Summary
                List<Summary> ShopSummary = new List<Summary>();
                List<double?> Cumulative = new List<double?>();
                List<double?> Averge = new List<double?>();
                {

                    double best = 0;
                    double cumulativedata = 0;
                    double avgdata = 0.0;
                    int avg = 1;
                    int bestprod = 0;
                    var bestdate = "";
                    double bestpower = 0.0;
                    double maxpower = 0.0;
                    var maxdate = "";
                    var best1Data = 0.0;
                    foreach (var id in ShopResult)
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
                    var totalconsume = Math.Round(ShopResult.Sum(s => s.TotalConsumption), 0);
                    if (ShopResult.Count() > 0)
                    {


                        var totalavg = Math.Round(ShopResult.Average(s => s.TotalConsumption), 0);
                        var totalprod = ShopResult.Sum(s => s.Production);
                        var totalavgsec = Math.Round(ShopResult.Average(s => s.Consumption), 0);
                        Summary summaryobj = new Summary(totalconsume, totalavg, totalavgsec, totalprod, best, bestpower, bestprod, maxpower, bestdate, maxdate);
                        ShopSummary.Add(summaryobj);
                    }
                }


                if (type == true)
                {
                    var consumetype = db.MM_Parameter.Where(s => s.Prameter_ID == 1).Select(s => s.Unit).FirstOrDefault();
                    ViewBag.ConsumeType = consumetype;
                }
                else
                {

                    ViewBag.ConsumeType = "KVAH";

                }

                return Json(new
                {
                    ShopResult,
                    Target,
                    ShopSummary,
                    Cumulative,
                    Averge,
                    allDates,
                    starts,
                    ends,
                    BestData,
                    CategoryData,
                    AreaInomerData,
                    FeederParetoData,
                    AddFeederData,
                    SubFeederData,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }


        public ActionResult ParamterLivedata(int Shop)
        {
            List<FeederDatetime> Feederobj = new List<FeederDatetime>();
            var Livedate = System.DateTime.Now;
            var Today = Livedate.AddMinutes(-1440);
            var feeder = (from f in db.MM_Feeders
                          where f.Shop_ID == Shop
                          select new
                          {
                              f.Feeder_ID
                          }).ToList();
            List<Sp_Feederwise_Live_Reading_Result> LiveData = db.Sp_Feederwise_Live_Reading(Shop, Today, Livedate).ToList();
            foreach (var id in feeder)
            {
                var datetime = LiveData.Where(s => s.Feeder_ID == id.Feeder_ID).Max(s => s.Dateandtime);
                //var date = Convert.ToDateTime(datetime).ToString("dd MMM hh:mm");
                if (datetime != null)
                {


                    string ddl = Convert.ToDateTime(datetime).ToString("dd MMM hh:mm tt");

                    FeederDatetime objfeeder = new FeederDatetime(Convert.ToInt32(id.Feeder_ID), ddl);
                    Feederobj.Add(objfeeder);
                }
            }
            return Json(new { LiveData, Feederobj }, JsonRequestBehavior.AllowGet);
        }
        public class FeederDatetime
        {
            public int feeder { get; set; }
            public string datetime { get; set; }
            public FeederDatetime(int feeder, string datetime)
            {
                this.feeder = feeder;
                this.datetime = datetime;
            }
        }

        public class Noworkingdata
        {
            public Double Consumption { get; set; }
            public int Production { get; set; }
            public Double Best { get; set; }
            public Double TotalConsumption { get; set; }
            public Double totalconsumption { get; set; }
            public Noworkingdata(Double Consumption, int Production, Double Best, Double TotalConsumption)
            {
                this.Consumption = Consumption;
                this.Production = Production;
                this.Best = Best;
                this.TotalConsumption = TotalConsumption;
            }
            public Noworkingdata(Double Consumption, Double totalConsumption)
            {
                this.Consumption = Consumption;
                this.totalconsumption = totalConsumption;
            }


        }

        public ActionResult Feederwisedetails(string Fromdate, string todate, string Shop, int Feeder, int? Parameter)
        {
            List<Sp_Feederwise_allParameter_Reading_Result> Performace = new List<Sp_Feederwise_allParameter_Reading_Result>();



            Performace = db.Sp_Feederwise_allParameter_Reading(Convert.ToInt32(Shop), Convert.ToDateTime(Fromdate), Convert.ToDateTime(todate), Feeder, Parameter).ToList();

            ViewData["FeederwiseData"] = Performace;
            ViewBag.pieData = JsonConvert.SerializeObject(Performace);
            return PartialView("_FeederwiseDetails");
        }

        public ActionResult alertdata(int Shop)
        {



            var Today = System.DateTime.Now.Date;

            var Performace = (from m in db.MM_Mailalert_status
                              join
                               a in db.MM_Alert_Mail on
                               m.alert_Id equals a.alert_Id
                              join
f in db.MM_Feeders on
m.Feeder_ID equals f.Feeder_ID
                              where m.Shop_ID == Shop && m.DateandTime == Today && (m.alert_Id == 4 || m.alert_Id == 5)
                              select new
                              {
                                  a.alert_Name,
                                  f.FeederName,
                                  m.TagIndex,
                                  m.Pervious_Value,
                                  Pervious_Time = (m.Pervious_Time).ToString(),
                                  m.Latest_Value,
                                  Latest_Time = (m.Latest_Time).ToString()
                              }).ToList();

            var ConstantData = (from m in db.MM_Mailalert_status
                                join
                                 a in db.MM_Alert_Mail on
                                 m.alert_Id equals a.alert_Id
                                join
                                f in db.MM_Feeders on
                                  m.Feeder_ID equals f.Feeder_ID
                                where m.Shop_ID == Shop && m.DateandTime == Today && (m.alert_Id == 2)
                                select new
                                {
                                    a.alert_Name,
                                    f.FeederName,
                                    m.TagIndex,
                                    m.Pervious_Value,
                                    Pervious_Time = (m.Pervious_Time).ToString(),
                                    m.Latest_Value,
                                    m.Value_Logged,
                                    Latest_Time = (m.Latest_Time).ToString()
                                }).ToList();



            ViewData["FeederwiseAlert"] = Performace;
            ViewBag.FeederwiseAlert1 = Performace;
            ViewBag.pieData = JsonConvert.SerializeObject(Performace);
            return Json(new { Performace, ConstantData }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FeederwiseTrend(string Fromdate, string todate, string Shop, int feeder, string temp)
        {
            // var isAllParameter = false;
            List<List<double?>> AllConsumption = new List<List<double?>>();
            List<List<double?>> cumulative = new List<List<double?>>();
            List<string> Names = new List<string>();
            List<string> Units = new List<string>();
            List<string> allDates = new List<string>();
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            int ShopID = Convert.ToInt32(Shop);
            string[] ParameterArray = { };
            var startdate = Convert.ToDateTime(Fromdate);
            var enddate = Convert.ToDateTime(todate);
            if (temp == "null")
            {

            }
            else
            {
                ParameterArray = temp.Split(',').ToArray();
            }
            foreach (var item in ParameterArray)
            {
                List<double?> Consumption = new List<double?>();
                List<double?> cumulativeConsume = new List<double?>();
                double cumulativedata = 0.0;
                int Parameter_Id = Convert.ToInt32(item);
                var Tagindex = (from t in db.UtilityMainFeederMappings
                                where t.Shop_ID == ShopID && t.Feeder_ID == feeder && t.Parameter_ID == Parameter_Id
                                select new
                                {
                                    t.Parameter_ID,
                                    t.Feeder_ID,
                                    t.TagIndex,
                                    t.Unit_ID
                                }).FirstOrDefault();
                var Parametertype = db.MM_Parameter.Where(p => p.Prameter_ID == Parameter_Id).Select(p => p.Consumption).FirstOrDefault();
                if (Parametertype.Value == true)
                {

                    List<Sp_LiveFeederwiseConsumption_Result> objconsumption = db.Sp_LiveFeederwiseConsumption(ShopID, startdate, enddate, plantID, "", "", Convert.ToInt32(Tagindex.TagIndex)).ToList();

                    for (int k = 0; k < objconsumption.Count(); k++)
                    {

                        if (k == 1)
                        {
                            double difference_first = 0.0;
                            double privous = Convert.ToDouble(objconsumption[k - 1].Consumption);
                            double current = Convert.ToDouble(objconsumption[k].Consumption);
                            if (current > privous)
                            {
                                if (privous != 0)
                                {
                                    difference_first = Math.Round(Convert.ToDouble(objconsumption[k].Consumption - objconsumption[k - 1].Consumption), 0);
                                    Consumption.Add(difference_first);

                                }
                                else
                                {
                                    Consumption.Add(0);

                                }

                            }
                            else
                            {
                                // difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                                Consumption.Add(0);

                            }

                        }
                        else if (k != 0)
                        {
                            double difference_first = 0.0;
                            double privous = Convert.ToDouble(objconsumption[k - 1].Consumption);
                            double current = Convert.ToDouble(objconsumption[k].Consumption);
                            if (current > privous)
                            {
                                if (privous != 0)
                                {
                                    difference_first = Math.Round(Convert.ToDouble(objconsumption[k].Consumption - objconsumption[k - 1].Consumption), 0);
                                    Consumption.Add(difference_first);

                                }
                                else
                                {
                                    Consumption.Add(0);

                                }
                            }
                            else
                            {
                                // difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                                Consumption.Add(0);

                            }
                        }
                        else
                        {
                            Consumption.Add(0);
                        }

                        string ddl = Convert.ToDateTime(objconsumption[k].ConsumptionDate).ToString("hh:mm tt");
                        allDates.Add(ddl);

                    }

                    foreach (var id in Consumption)
                    {
                        cumulativedata += Convert.ToDouble(id);
                        cumulativeConsume.Add(cumulativedata);
                    }
                    cumulative.Add(cumulativeConsume);
                }
                else
                {
                    List<Sp_ParameterWiseAnalaytics_Result> Parameterwiseobj = null;
                    Parameterwiseobj = db.Sp_ParameterWiseAnalaytics(ShopID, startdate, enddate, plantID, Parameter_Id, null, null, Convert.ToInt32(Tagindex.TagIndex)).ToList();
                    if (Parameterwiseobj.Count() > 0)
                    {
                        allDates = new List<string>();
                        foreach (var id in Parameterwiseobj)
                        {
                            Consumption.Add(id.Consumption);
                            //var consumedate = Convert.ToDateTime(id.ConsumptionDate).ToString("dd/mm hh:mm");
                            ////var date = (id.ConsumptionDate).ToString();
                            //allDates.Add(consumedate);
                            string ddl = Convert.ToDateTime(id.ConsumptionDate).ToString("hh:mm tt");
                            allDates.Add(ddl);

                        }
                    }
                    else
                    {
                        Consumption.Add(0);
                        // allDates.Add(null);
                    }
                }

                AllConsumption.Add(Consumption);
                var name = (from p in db.MM_Parameter
                            where p.Prameter_ID == Parameter_Id
                            select new { p.Prameter_Name }).FirstOrDefault();
                string unit = "";
                if (Tagindex != null)
                {
                    var units = (from u in db.MM_Unit_Measurement
                                 where u.Unit_ID == Tagindex.Unit_ID
                                 select new
                                 {
                                     u.Data_Unit
                                 }).FirstOrDefault();
                    unit = units.Data_Unit;
                }
                {

                }

                Names.Add(name.Prameter_Name);
                Units.Add(unit);



            }
            var FeederName = (from f in db.MM_Feeders
                              where f.Feeder_ID == feeder && f.Shop_ID == ShopID
                              select new { f.FeederName }).FirstOrDefault();
            return Json(new { AllConsumption, Names, Units, allDates, FeederName.FeederName, cumulative }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ComparisonData(string Fromdate, string todate, string ddlformat, string Shop, string temp, string Reason, string Parameter, string shiftwise, string Minute)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int ShopID = Convert.ToInt32(Shop);
                int Parameter_Id = Convert.ToInt32(Parameter);
                var Unit1 = (from u in db.MM_Parameter
                             where u.Prameter_ID == Parameter_Id
                             select new
                             {
                                 u.Unit
                             }).FirstOrDefault();
                var Unit = Unit1.Unit;

                DateTime startdate1 = DateTime.Parse(Fromdate);
                DateTime enddate1 = DateTime.Parse(todate);

                DateTime endg = DateTime.Parse(Fromdate);
                DateTime startg = DateTime.Parse(Fromdate);
                DateTime todaydate = DateTime.Now.Date;
                var ShopId = (from s in db.MM_MTTUW_Shops
                              join
                               u in db.MM_Specific_Cosume_Unit on
                               s.Spec_Unit_ID equals u.Spec_Unit_ID
                              where s.Shop_ID == ShopID && s.Energy == true
                              select new
                              {
                                  u.Spec_Unit_ID,
                                  u.Unit_Name,
                                  s.Shop_Name
                              }).FirstOrDefault();
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID == 25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).FirstOrDefault();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                enddate1 = enddate1.AddDays(1);
                enddate1 = (enddate1 + starttime);
                startdate1 = (startdate1 + starttime);
                var starttime1 = "";
                var endtime1 = "";
                var endmintime = "";
                if (shiftwise != "")
                {
                    var shiftId = Convert.ToInt32(shiftwise);
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
                    DateTime startd = DateTime.Parse(Fromdate);
                    DateTime endd = DateTime.Parse(todate);
                    var startt = TimeSpan.Parse(starttime1);
                    var Endt = TimeSpan.Parse(endmintime);
                    startg = DateTime.Parse(Fromdate);
                    startg = (startd + startt);
                    //endg = endg.AddDays(1);
                    if (shiftId == 3)
                    {
                        endg = endg.AddDays(1);

                    }
                    endg = (endg + Endt);
                    startd = (startd + startt);
                    endd = (endd + Endt);
                    ViewData["start"] = startd.ToString("dd/MMM hh:mm:ss tt");
                    ViewData["end"] = endd.ToString("dd/MMM hh:mm:ss tt");

                }
                else
                {
                    DateTime startd = DateTime.Parse(Fromdate);
                    DateTime endd = DateTime.Parse(todate);
                    startd = (startd + starttime);
                    endd = (endd + starttime);
                    startg = DateTime.Parse(Fromdate);
                    startg = (startg + starttime);
                    endg = endg.AddDays(1);
                    endg = (endg + starttime);
                    ViewData["start"] = startd.ToString("dd/MMM hh:mm:ss tt");
                    ViewData["end"] = endd.ToString("dd/MMM hh:mm:ss tt");
                }
                string[] FeederArray = { };
                if (temp == "null")
                {


                }
                else
                {
                    FeederArray = temp.Split(',').ToArray();
                }

                List<List<double?>> AllConsumption = new List<List<double?>>();
                List<List<double?>> Ideal = new List<List<double?>>();
                List<List<double?>> cumulative = new List<List<double?>>();

                List<string> Names = new List<string>();
                List<string> allDates = new List<string>();
                List<Sp_ParameterWiseAnalaytics_Result> Parameterwiseobj = null;
                List<Sp_dailyFeederwiseConsumption_New_Result> ConsumeFeeder = new List<Sp_dailyFeederwiseConsumption_New_Result>();
                List<SP_Feeder_TimeWise_Consumption_Result> Grana = new List<SP_Feeder_TimeWise_Consumption_Result>();
                List<SP_Feeder_Parameterwise_Data_Result> Parameterdata = new List<SP_Feeder_Parameterwise_Data_Result>();
                //List<Sp_DailyShopwiseConsumption_New_Result> pie = new List<Sp_DailyShopwiseConsumption_New_Result>();
                // Sp_dailyFeederwiseConsumption_New(ShopID, startdate1, enddate1, plantID, starttime, endtime, feederId).ToList()
                var Parametertype = db.MM_Parameter.Where(p => p.Prameter_ID == Parameter_Id).Select(p => p.Consumption).FirstOrDefault();

                var minmum = 100000000.12;
                if (Parametertype.Value == true)
                {

                    {
                        foreach (var item in FeederArray)
                        {
                            // allDates = new List<string>();
                            double cumulativedata = 0.0;
                            allDates = new List<string>();
                            List<double?> Consumption = new List<double?>();
                            List<double?> cumulativeconsume = new List<double?>();
                            int feederId = item != "" ? Convert.ToInt32(item) : -1;
                            int ParameterId = Convert.ToInt32(Parameter);
                            var TagIndex = (from u in db.UtilityMainFeederMappings
                                            where u.Feeder_ID == feederId && u.Parameter_ID == ParameterId
                                            select new
                                            {
                                                u.TagIndex
                                            }).FirstOrDefault();
                            if (Minute != "" && (ddlformat == "2" || ddlformat == "5" || ddlformat == "1"))
                            {

                                if (ddlformat == "1")
                                {
                                    var Today = System.DateTime.Now;
                                    Grana = db.SP_Feeder_TimeWise_Consumption(plantID, startdate1, Today, ShopID, Convert.ToInt32(Minute), Convert.ToInt32(TagIndex.TagIndex), ParameterId).ToList();
                                }
                                else
                                {
                                    Grana = db.SP_Feeder_TimeWise_Consumption(plantID, startg, endg, ShopID, Convert.ToInt32(Minute), Convert.ToInt32(TagIndex.TagIndex), ParameterId).ToList();
                                }
                            }
                            else
                            {



                                {

                                    ConsumeFeeder = db.Sp_dailyFeederwiseConsumption_New(ShopID, startdate1, enddate1, plantID, starttime1, starttime1, Convert.ToInt32(TagIndex.TagIndex)).ToList();
                                }
                            }
                            if (Reason != "")
                            {
                                int ReasonID = Convert.ToInt32(Reason);
                                var noworkingday = (from n in db.MM_No_Working_Day
                                                    where n.Reason_ID == ReasonID && n.Day_Date >= startdate1 && n.Day_Date <= enddate1
                                                    select new
                                                    {
                                                        n.Day_Date
                                                    }).ToList();

                                if (noworkingday.Count() > 0)
                                {


                                    foreach (var id in noworkingday.OrderBy(n => n.Day_Date))
                                    {
                                        var ConsumeData = ConsumeFeeder.Where(s => s.ConsumptionDate == id.Day_Date).Select(s => s.Consumption).FirstOrDefault();
                                        Consumption.Add(ConsumeData.Value);
                                        if (ConsumeData.Value > 0)
                                        {
                                            if (minmum > ConsumeData.Value)
                                            {
                                                minmum = Convert.ToDouble(ConsumeData.Value);
                                            }
                                        }
                                        allDates.Add(id.Day_Date.ToString("dd/MM/yy"));
                                    }

                                }

                            }
                            else
                            {
                                if (Minute != "")
                                {
                                    foreach (var id in Grana)
                                    {
                                        Consumption.Add(id.Consumption);

                                        if (id.Consumption > 0)
                                        {
                                            if (minmum > id.Consumption)
                                            {
                                                minmum = Convert.ToDouble(id.Consumption);
                                            }
                                        }

                                        var consumedate = Convert.ToDateTime(id.dateandtime).ToString("hh:mm tt");
                                        allDates.Add(consumedate.ToString());
                                    }
                                }
                                else
                                {
                                    foreach (var id in ConsumeFeeder)
                                    {
                                        Consumption.Add(id.Consumption);
                                        if (id.Consumption > 0)
                                        {
                                            if (minmum > id.Consumption)
                                            {
                                                minmum = Convert.ToDouble(id.Consumption);
                                            }
                                        }
                                        var consumedate = Convert.ToDateTime(id.ConsumptionDate).ToString("dd/MM/yy");
                                        allDates.Add(consumedate.ToString());
                                    }
                                }

                            }
                            foreach (var id in Consumption)
                            {
                                cumulativedata += Math.Round(Convert.ToDouble(id), 0);
                                cumulativeconsume.Add(cumulativedata);
                            }
                            AllConsumption.Add(Consumption);
                            cumulative.Add(cumulativeconsume);
                            var FeederName = (from f in db.MM_Feeders
                                              where f.Feeder_ID == feederId
                                              select
                                              new
                                              {
                                                  f.FeederName

                                              }).FirstOrDefault();
                            Names.Add(FeederName.FeederName);


                        }
                    }
                    if (Parameter_Id == 1)
                    {

                        foreach (var item in FeederArray)
                        {
                            // allDates = new List<string>();
                            //allDates = new List<string>();
                            List<double?> Consumption = new List<double?>();
                            int feederId = item != "" ? Convert.ToInt32(item) : -1;
                            int ParameterId = Convert.ToInt32(Parameter);
                            var TagIndex = (from u in db.UtilityMainFeederMappings
                                            where u.Feeder_ID == feederId && u.Parameter_ID == ParameterId
                                            select new
                                            {
                                                u.TagIndex,
                                                u.Ratedload
                                            }).FirstOrDefault();
                            if (Minute != "")
                            {
                                foreach (var id in allDates)
                                {
                                    Consumption.Add(TagIndex.Ratedload);
                                }
                            }
                            else
                            {
                                if (TagIndex != null)
                                {
                                    var load = Math.Round(Convert.ToDouble(TagIndex.Ratedload * 24), 0);
                                    foreach (var id in allDates)
                                    {
                                        Consumption.Add(load);
                                    }
                                }


                            }

                            Ideal.Add(Consumption);
                        }
                    }
                }
                else
                {

                    foreach (var item in FeederArray)
                    {
                        List<double?> Consumption = new List<double?>();
                        //List<double?> Final_Consumption = new List<double?>();
                        allDates = new List<string>();


                        int feederId1 = item != "" ? Convert.ToInt32(item) : -1;
                        int ParameterId1 = Convert.ToInt32(Parameter);
                        var TagIndex = (from u in db.UtilityMainFeederMappings
                                        where u.Feeder_ID == feederId1 && u.Parameter_ID == ParameterId1
                                        select new
                                        {
                                            u.TagIndex
                                        }).FirstOrDefault();
                        // Parameterwiseobj = null;
                        Parameterwiseobj = db.Sp_ParameterWiseAnalaytics(ShopID, startdate1, enddate1, plantID, Parameter_Id, null, null, Convert.ToInt32(TagIndex.TagIndex)).ToList();
                        if (Parameterwiseobj.Count() > 0)
                        {
                            foreach (var id in Parameterwiseobj)
                            {
                                Consumption.Add(id.Consumption);
                                if (id.Consumption > 0)
                                {
                                    if (minmum > id.Consumption)
                                    {
                                        minmum = Convert.ToDouble(id.Consumption);
                                    }
                                }
                                var consumedate = Convert.ToDateTime(id.ConsumptionDate).ToString("dd/MMM hh:mm tt");
                                //var date = (id.ConsumptionDate).ToString();
                                allDates.Add(consumedate);
                                //string ddl = Convert.ToDateTime(id.ConsumptionDate).ToString("HH:mm");
                                //allDates.Add(ddl);

                            }
                        }
                        else
                        {
                            Consumption.Add(0);
                            allDates.Add(null);
                        }

                        var FeederName = (from f in db.MM_Feeders
                                          where f.Feeder_ID == feederId1
                                          select
                                          new
                                          {
                                              f.FeederName

                                          }).FirstOrDefault();
                        Names.Add(FeederName.FeederName);
                        //if (feederId == -1)
                        //{
                        //    //var shop = (from s in db.MM_MTTUW_Shops
                        //    //            where s.Shop_ID == Shop
                        //    //            select new { s.Shop_Name }).FirstOrDefault();

                        AllConsumption.Add(Consumption);
                        //    //Names.Add(shop.Shop_Name);
                        //}
                        //else
                        //{
                        //    //var feeder1 = (from f in db.MM_Parameter
                        //    //               where f.TagIndex == feederId
                        //    //               select new
                        //    //               {
                        //    //                   f.FeederName,
                        //    //                   f.Unit
                        //    //               }).FirstOrDefault();
                        //    //// var FeederName = obj[0].FeederName;
                        //    //var FeederName = feeder1 != null ? feeder1.FeederName : "";

                        //    //Unit = feeder1.Unit;

                        //    AllConsumption.Add(Consumption);
                        //    //Names.Add(FeederName);
                        //}

                    }



                }





                return Json(new { AllConsumption, Names, allDates, Unit, Ideal, cumulative, minmum }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult SubArea(string Formdate, string todate, string ddlformat, int? Shiftwise, int? Minute, string ConsumptionId, int? holiday, int Area, int Shop)
        {
            try
            {
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }
                List<Processchart> ProcessData = new List<Processchart>();
                List<Barchart> piechartData = new List<Barchart>();
                List<Sp_SubAreawiseConsumption_Result> SubAreaFeeder = new List<Sp_SubAreawiseConsumption_Result>();
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var AreaName = db.MM_Area.Where(s => s.Area_Id == Area).Select(s => s.Area_Name).FirstOrDefault();
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID ==25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).FirstOrDefault();
                DateTime startdate1 = DateTime.Parse(Formdate);
                DateTime enddate1 = DateTime.Parse(todate);

                DateTime endg = DateTime.Parse(Formdate);
                DateTime startg = DateTime.Parse(Formdate);
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                var livedate = System.DateTime.Now;
                enddate1 = enddate1.Date.AddDays(1);
                enddate1 = (enddate1.Date + starttime);
                startdate1 = (startdate1.Date + starttime);

                DateTime start_m = DateTime.Parse(Formdate);
                DateTime end_m = DateTime.Parse(todate);
                var hours = (end_m - start_m).TotalHours;

                if (holiday != null)
                {
                    List<Processchart> ProcessNoworkingData = new List<Processchart>();
                    var noworkingday = (from n in db.MM_No_Working_Day
                                        where n.Reason_ID == holiday && n.Day_Date >= startdate1 && n.Day_Date <= enddate1
                                        select new
                                        {
                                            n.Day_Date
                                        }).ToList();
                    if (noworkingday.Count() > 0)

                    {

                        foreach (var id in noworkingday.OrderBy(n => n.Day_Date))
                        {
                            var enddate = id.Day_Date.AddDays(1);
                            SubAreaFeeder = db.Sp_SubAreawiseConsumption(Convert.ToInt32(Area), id.Day_Date, enddate, "", "", plantID, Shop, Shiftwise, type).ToList();
                            foreach (var id1 in SubAreaFeeder)
                            {
                                Processchart obj3 = new Processchart(id1.Feeder_Name, Convert.ToDouble(id1.Consumption), Convert.ToInt32(id1.TagIndex));
                                ProcessNoworkingData.Add(obj3);
                            }
                        }
                        foreach (var id1 in ProcessNoworkingData.Select(s => s.ShopId).Distinct().ToList())
                        {
                            var Name = ProcessNoworkingData.Where(s => s.ShopId == id1).Select(s => s.Label).FirstOrDefault();
                            var total = ProcessNoworkingData.Where(s => s.ShopId == id1).Sum(s => s.Y);
                            Processchart obj3 = new Processchart(Name, Convert.ToDouble(total), Convert.ToInt32(id1));
                            ProcessData.Add(obj3);
                            Barchart obj4 = new Barchart(Name, Convert.ToDouble(total), "Add", 1);
                            piechartData.Add(obj4);
                        }

                    }
                }
                else
                {
                    if (ddlformat == "5" && hours <= 24)
                    {

                        SubAreaFeeder = db.Sp_SubAreawiseConsumption(Convert.ToInt32(Area), start_m, end_m, "", "", plantID, Shop, Shiftwise, type).ToList();
                    }
                    else if (ddlformat != "1" && (startdate1.Date != livedate.Date))
                    {
                        SubAreaFeeder = db.Sp_SubAreawiseConsumption(Convert.ToInt32(Area), startdate1, enddate1, "", "", plantID, Shop, Shiftwise, type).ToList();
                    }
                    else
                    {


                        SubAreaFeeder = db.Sp_SubAreawiseConsumption(Convert.ToInt32(Area), startdate1, livedate, "", "", plantID, Shop, Shiftwise, type).ToList();
                    }
                    foreach (var id in SubAreaFeeder)
                    {
                        Processchart obj3 = new Processchart(id.Feeder_Name, Convert.ToDouble(id.Consumption), Convert.ToInt32(id.TagIndex));
                        ProcessData.Add(obj3);
                        Barchart obj4 = new Barchart(id.Feeder_Name, Convert.ToDouble(id.Consumption), "Add", 1);
                        piechartData.Add(obj4);
                    }
                }

                var CBM = (from f in db.MM_Area_CBM_Mapping
                           where f.Area_ID == Area
                           select new
                           {
                               f.CBM_ID,
                               f.CBM_Name
                           }).ToList();


                return Json(new { ProcessData, AreaName, piechartData, CBM }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public ActionResult FeederData(string Fromdate, string todate, string ddlformat, string Shop, int TagIndex, string Reason, string shiftwise, string Minute)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int ShopID = Convert.ToInt32(Shop);
                Double avg = 0.0;
                var Unit1 = (from u in db.UtilityMainFeederMappings
                             where u.TagIndex == TagIndex
                             select new
                             {
                                 u.Unit_ID,
                                 u.Parameter_ID
                             }).FirstOrDefault();
                var Unit = db.MM_Parameter.Where(s => s.Prameter_ID == Unit1.Parameter_ID).Select(s => s.Unit);
                DateTime startdate1 = DateTime.Parse(Fromdate);
                DateTime enddate1 = DateTime.Parse(todate);

                DateTime endg = DateTime.Parse(Fromdate);
                DateTime startg = DateTime.Parse(Fromdate);
                DateTime todaydate = DateTime.Now.Date;
                var ShopId = (from s in db.MM_MTTUW_Shops
                              join
                               u in db.MM_Specific_Cosume_Unit on
                               s.Spec_Unit_ID equals u.Spec_Unit_ID
                              where s.Shop_ID == ShopID && s.Energy == true
                              select new
                              {
                                  u.Spec_Unit_ID,
                                  u.Unit_Name,
                                  s.Shop_Name
                              }).FirstOrDefault();
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID == 25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).FirstOrDefault();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                enddate1 = enddate1.Date.AddDays(1);
                enddate1 = (enddate1.Date + starttime);
                startdate1 = (startdate1.Date + starttime);
                var starttime1 = "";
                var endtime1 = "";
                var endmintime = "";
                if (shiftwise != "")
                {
                    var shiftId = Convert.ToInt32(shiftwise);
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
                    DateTime startd = DateTime.Parse(Fromdate);
                    DateTime endd = DateTime.Parse(todate);
                    var startt = TimeSpan.Parse(starttime1);
                    var Endt = TimeSpan.Parse(endmintime);
                    startg = DateTime.Parse(Fromdate);
                    startg = (startd.Date + startt);
                    //endg = endg.AddDays(1);
                    if (shiftId == 3)
                    {
                        endg = endg.Date.AddDays(1);

                    }
                    endg = (endg.Date + Endt);
                    startd = (startd.Date + startt);
                    endd = (endd.Date + Endt);
                    ViewData["start"] = startd.ToString("dd/MMM hh:mm:ss tt");
                    ViewData["end"] = endd.ToString("dd/MMM hh:mm:ss tt");

                }
                else
                {
                    DateTime startd = DateTime.Parse(Fromdate);
                    DateTime endd = DateTime.Parse(todate);
                    startd = (startd + starttime);
                    endd = (endd + starttime);
                    startg = DateTime.Parse(Fromdate);
                    startg = (startg.Date + starttime);
                    endg = endg.AddDays(1);
                    endg = (endg.Date + starttime);
                    ViewData["start"] = startd.ToString("dd/MMM hh:mm:ss tt");
                    ViewData["end"] = endd.ToString("dd/MMM hh:mm:ss tt");
                }


                DateTime start_m = DateTime.Parse(Fromdate);
                DateTime end_m = DateTime.Parse(todate);
                var hours = (end_m - start_m).TotalHours;

                string[] FeederArray = { };

                List<List<double?>> AllConsumption = new List<List<double?>>();
                List<List<double?>> Ideal = new List<List<double?>>();
                List<List<double?>> cumulative = new List<List<double?>>();

                List<List<double?>> Average = new List<List<double?>>();
                List<string> Names = new List<string>();
                List<string> allDates = new List<string>();
                //List<Sp_ParameterWiseAnalaytics_Result> Parameterwiseobj = null;
                List<Sp_dailyFeederwiseConsumption_New_Result> ConsumeFeeder = new List<Sp_dailyFeederwiseConsumption_New_Result>();
                List<SP_Feeder_TimeWise_Consumption_Result> Grana = new List<SP_Feeder_TimeWise_Consumption_Result>();
                List<SP_Feeder_Parameterwise_Data_Result> Parameterdata = new List<SP_Feeder_Parameterwise_Data_Result>();
                //List<Sp_DailyShopwiseConsumption_New_Result> pie = new List<Sp_DailyShopwiseConsumption_New_Result>();
                // Sp_dailyFeederwiseConsumption_New(ShopID, startdate1, enddate1, plantID, starttime, endtime, feederId).ToList()
                var Parametertype = db.MM_Parameter.Where(p => p.Prameter_ID == Unit1.Parameter_ID).Select(p => p.Consumption).FirstOrDefault();


                //if (Parametertype.Value == true)
                //{

                {

                    // allDates = new List<string>();
                    double cumulativedata = 0.0;
                    allDates = new List<string>();
                    List<double?> Consumption = new List<double?>();
                    List<double?> cumulativeconsume = new List<double?>();

                    if (Minute != "" && (ddlformat == "2" || (ddlformat == "5" && hours <= 24) || ddlformat == "1"))
                    {

                        if (ddlformat == "1")
                        {
                            var Today = System.DateTime.Now;
                            Grana = db.SP_Feeder_TimeWise_Consumption(plantID, startdate1, Today, ShopID, Convert.ToInt32(Minute), Convert.ToInt32(TagIndex), Convert.ToInt32(Unit1.Parameter_ID)).ToList();
                        }
                        else if (ddlformat == "5" && hours <= 24)
                        {
                            Grana = db.SP_Feeder_TimeWise_Consumption(plantID, start_m, end_m, ShopID, Convert.ToInt32(Minute), Convert.ToInt32(TagIndex), Convert.ToInt32(Unit1.Parameter_ID)).ToList();
                        }
                        else
                        {
                            Grana = db.SP_Feeder_TimeWise_Consumption(plantID, startg, endg, ShopID, Convert.ToInt32(Minute), Convert.ToInt32(TagIndex), Convert.ToInt32(Unit1.Parameter_ID)).ToList();
                        }
                    }
                    else
                    {



                        {

                            ConsumeFeeder = db.Sp_dailyFeederwiseConsumption_New(ShopID, startdate1, enddate1, plantID, starttime1, starttime1, Convert.ToInt32(TagIndex)).ToList();
                        }
                    }
                    if (Reason != "")
                    {
                        int ReasonID = Convert.ToInt32(Reason);
                        var noworkingday = (from n in db.MM_No_Working_Day
                                            where n.Reason_ID == ReasonID && n.Day_Date >= startdate1 && n.Day_Date <= enddate1
                                            select new
                                            {
                                                n.Day_Date
                                            }).ToList();

                        if (noworkingday.Count() > 0)
                        {


                            foreach (var id in noworkingday.OrderBy(n => n.Day_Date))
                            {
                                var ConsumeData = ConsumeFeeder.Where(s => s.ConsumptionDate == id.Day_Date).Select(s => s.Consumption).FirstOrDefault();
                                Consumption.Add(ConsumeData.Value);
                                allDates.Add(id.Day_Date.ToString("dd/MM/yy"));
                            }

                        }

                    }
                    else
                    {
                        if (Minute != "")
                        {
                            foreach (var id in Grana)
                            {
                                Consumption.Add(id.Consumption);
                                var consumedate = Convert.ToDateTime(id.dateandtime).ToString("HH:mm tt");
                                allDates.Add(consumedate.ToString());
                            }
                        }
                        else
                        {
                            foreach (var id in ConsumeFeeder)
                            {
                                Consumption.Add(id.Consumption);
                                var consumedate = Convert.ToDateTime(id.ConsumptionDate).ToString("dd/MM/yy");
                                allDates.Add(consumedate.ToString());
                            }
                        }

                    }
                    foreach (var id in Consumption)
                    {
                        cumulativedata += Math.Round(Convert.ToDouble(id), 1);
                        cumulativeconsume.Add(Math.Round(cumulativedata, 1));
                    }
                    AllConsumption.Add(Consumption);
                    cumulative.Add(cumulativeconsume);
                    var FeederName = (from u in db.UtilityMainFeederMappings
                                      join
                                      f in db.MM_Feeders on
                                      u.Feeder_ID equals f.Feeder_ID
                                      where u.TagIndex == TagIndex
                                      select
                                      new
                                      {
                                          f.FeederName

                                      }).FirstOrDefault();
                    Names.Add(FeederName.FeederName);



                }
                //if (Unit1.Parameter_ID == 1)
                //{


           

















                // allDates = new List<string>();
                //allDates = new List<string>();
                List<double?> ConsumptionLoad = new List<double?>();
                List<double?> Avglist = new List<double?>();
                var Feeder = (from u in db.UtilityMainFeederMappings
                              where u.TagIndex == TagIndex
                              select new
                              {

                                  u.Ratedload
                              }).FirstOrDefault();
                if (Minute != "")
                {
                    foreach (var id in allDates)
                    {
                        ConsumptionLoad.Add(Feeder.Ratedload);
                    }
                    foreach (var id in Grana)
                    {
                        //Avglist.Add(id.Average);
                        Avglist.Add(id.Average);
                    }

                }
                else
                {
                    if (Feeder != null)
                    {
                        var load = Math.Round(Convert.ToDouble(Feeder.Ratedload * 24), 0);
                        avg = Math.Round(Convert.ToDouble(ConsumeFeeder.Average(s => s.Consumption)), 0);
                        foreach (var id in allDates)
                        {
                            ConsumptionLoad.Add(load);
                            Avglist.Add(avg);
                        }
                    }

                }
                Ideal.Add(ConsumptionLoad);
                Average.Add(Avglist);
                //}

                //}


                return Json(new { AllConsumption, Names, allDates, Unit, cumulative, Ideal, Average }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CBMFeederData(string CBMdate, string Shop, int TagIndex, string type, string Area, string CBM)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int ShopID = Convert.ToInt32(Shop);
                Boolean types = true;

                var livedate = System.DateTime.Now;
                List<Processchart> FeederwiseData = new List<Processchart>();
                List<Sp_SubAreawiseConsumption_Result> SubAreaFeeder = new List<Sp_SubAreawiseConsumption_Result>();
                if (type != "1")
                {
                    types = false;
                }
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID ==25
                             select 
                             new
                             {
                                 s.Shift_Start_Time
                             }).First();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                DateTime startdate1 = DateTime.Parse(CBMdate);
                DateTime enddate1 = DateTime.Parse(CBMdate);
                enddate1 = enddate1.AddDays(1);
                enddate1 = (enddate1 + starttime);
                startdate1 = (startdate1 + starttime);
                List<string> Units = new List<string>();
                Double avg = 0.0;
                var Unit1 = (from u in db.UtilityMainFeederMappings
                             where u.TagIndex == TagIndex
                             select new
                             {
                                 u.Unit_ID,
                                 u.Parameter_ID
                             }).FirstOrDefault();
                var Unit = db.MM_Parameter.Where(s => s.Prameter_ID == Unit1.Parameter_ID).Select(s => s.Unit).FirstOrDefault();

                Units.Add(Unit);
                var ShopId = (from s in db.MM_MTTUW_Shops
                              join
                               u in db.MM_Specific_Cosume_Unit on
                               s.Spec_Unit_ID equals u.Spec_Unit_ID
                              where s.Shop_ID == ShopID && s.Energy == true
                              select new
                              {
                                  u.Spec_Unit_ID,
                                  u.Unit_Name,
                                  s.Shop_Name
                              }).FirstOrDefault();
                string[] FeederArray = { };
                List<List<double?>> CBMData = new List<List<double?>>();
                List<List<double?>> AllConsumption = new List<List<double?>>();
                List<List<double?>> Ideal = new List<List<double?>>();
                List<List<double?>> cumulative = new List<List<double?>>();
                List<List<double?>> Average = new List<List<double?>>();
                List<string> Names = new List<string>();
                List<string> allDates = new List<string>();
                List<Sp_dailyFeederwiseConsumption_New_Result> ConsumeFeeder = new List<Sp_dailyFeederwiseConsumption_New_Result>();
                List<SP_Feeder_TimeWise_Consumption_Result> Grana = new List<SP_Feeder_TimeWise_Consumption_Result>();
                List<SP_Feeder_Parameterwise_Data_Result> Parameterdata = new List<SP_Feeder_Parameterwise_Data_Result>();
                var Parametertype = db.MM_Parameter.Where(p => p.Prameter_ID == Unit1.Parameter_ID).Select(p => p.Consumption).FirstOrDefault();


                // Feeder wise Granulity,Idel, cumulative,Average
                {

                    // allDates = new List<string>();
                    double cumulativedata = 0.0;
                    allDates = new List<string>();
                    List<double?> Consumption = new List<double?>();
                    List<double?> cumulativeconsume = new List<double?>();
                    Grana = db.SP_Feeder_TimeWise_Consumption(plantID, startdate1, enddate1, ShopID, Convert.ToInt32(60), Convert.ToInt32(TagIndex), Convert.ToInt32(Unit1.Parameter_ID)).ToList();
                    foreach (var id in Grana)
                    {
                        Consumption.Add(id.Consumption);
                        var consumedate = Convert.ToDateTime(id.dateandtime).ToString("HH:mm tt");
                        allDates.Add(consumedate.ToString());
                    }
                    foreach (var id in Consumption)
                    {
                        cumulativedata += Math.Round(Convert.ToDouble(id), 1);
                        cumulativeconsume.Add(Math.Round(cumulativedata, 1));
                    }

                    AllConsumption.Add(Consumption);
                    cumulative.Add(cumulativeconsume);
                    var FeederName = (from u in db.UtilityMainFeederMappings
                                      join
                                      f in db.MM_Feeders on
                                      u.Feeder_ID equals f.Feeder_ID
                                      where u.TagIndex == TagIndex
                                      select
                                      new
                                      {
                                          f.FeederName

                                      }).FirstOrDefault();
                    Names.Add(FeederName.FeederName);





                    List<double?> ConsumptionLoad = new List<double?>();
                    List<double?> Avglist = new List<double?>();
                    var Feeder = (from u in db.UtilityMainFeederMappings
                                  where u.TagIndex == TagIndex
                                  select new
                                  {
                                      u.Ratedload
                                  }).FirstOrDefault();

                    foreach (var id in allDates)
                    {
                        ConsumptionLoad.Add(Feeder.Ratedload);
                    }
                    foreach (var id in Grana)
                    {
                        //Avglist.Add(id.A);
                       Avglist.Add(id.Average);
                    }
                    Ideal.Add(ConsumptionLoad);
                    Average.Add(Avglist);
                }
                // CBM Parameter Data 
                {
                    string[] CBMArray = { };
                    if (CBM == "null")
                    {


                    }
                    else
                    {
                        CBMArray = CBM.Split(',').ToArray();
                    }


                    foreach (var item in CBMArray)
                    {
                        List<double?> CBMValue = new List<double?>();
                        int CBMID = item != "" ? Convert.ToInt32(item) : -1;
                        var CBMData1 = db.MM_Ctrl_CBM_Hourly_Data.Where(s => s.DateandTime > startdate1 && s.DateandTime <= enddate1 && s.CBM_ID == CBMID).ToList();
                        var unit = db.MM_MT_Conditional_Based_Maintenance.Where(s => s.CBM_ID == CBMID).Select(s => s.UOM).FirstOrDefault().ToString();
                        var Name = db.MM_MT_Conditional_Based_Maintenance.Where(s => s.CBM_ID == CBMID).Select(s => s.Machine_Parameter).FirstOrDefault().ToString();
                        foreach (var id in CBMData1)
                        {
                            //var areadate = Convert.ToDateTime(id.DateandTime).ToString("hh:mm tt");
                            //unit = db1.MM_MT_Conditional_Based_Maintenance.Where(s => s.CBM_ID == id.CBM_ID).Select(s => s.UOM).FirstOrDefault().ToString();

                            //Areawise_CBM obj = new Areawise_CBM(Name, areadate, Convert.ToDouble(id.Parameter_Value), Convert.ToInt32(id.CBM_ID), unit);
                            //Areaobj.Add(obj);
                            CBMValue.Add(Convert.ToDouble(id.Parameter_Value));
                        }
                        CBMData.Add(CBMValue);
                        Units.Add(unit);
                        Names.Add(Name);
                    }
                }

                // Feederwise Table Data 
                {
                    if (startdate1.Date == livedate.Date)
                    {
                        SubAreaFeeder = db.Sp_SubAreawiseConsumption(Convert.ToInt32(Area), startdate1, livedate, "", "", plantID, ShopID, null, types).ToList();
                    }
                    else
                    {
                        SubAreaFeeder = db.Sp_SubAreawiseConsumption(Convert.ToInt32(Area), startdate1, enddate1, "", "", plantID, ShopID, null, types).ToList();
                    }

                    foreach (var id in SubAreaFeeder)
                    {
                        Processchart obj3 = new Processchart(id.Feeder_Name, Convert.ToDouble(id.Consumption), Convert.ToInt32(id.TagIndex));
                        FeederwiseData.Add(obj3);

                    }
                }


                return Json(new { AllConsumption, Names, allDates, Units, cumulative, Ideal, Average, CBMData, FeederwiseData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }



        public ActionResult AreawiseTrend(string Formdate, string todate, string ddlformat, int Shop, int? Shiftwise, string Minute, int? Reason, string ConsumptionId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }
                List<Sp_Shift_AreawiseConsumption_Result> shiftarea = new List<Sp_Shift_AreawiseConsumption_Result>();
                List<Sp_Shift_Areawise_TimeConsumption_Result> hourlyArea = new List<Sp_Shift_Areawise_TimeConsumption_Result>();
                List<Areawise_Consumption> Area = new List<Areawise_Consumption>();
                List<List<Areawise_Consumption>> Areawise = new List<List<Areawise_Consumption>>();
                List<Areawise_Consumption> AreaWisePareto = new List<Areawise_Consumption>();

                DateTime startdate1 = DateTime.Parse(Formdate);
                DateTime enddate1 = DateTime.Parse(todate);

                DateTime start_m = DateTime.Parse(Formdate);
                DateTime end_m = DateTime.Parse(todate);
                var hours = (end_m - start_m).TotalHours;

                DateTime endg = DateTime.Parse(Formdate);
                DateTime startg = DateTime.Parse(Formdate);
                DateTime todaydate = DateTime.Now.Date;
                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID ==25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).First();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                enddate1 = enddate1.Date.AddDays(1);
                enddate1 = (enddate1.Date + starttime);
                startdate1 = (startdate1.Date + starttime);


                var starttime1 = "";
                var endtime1 = "";
                var endmintime = "";
                if (Shiftwise != null)
                {
                    var shiftId = Convert.ToInt32(Shiftwise);
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
                        endg = endg.Date.AddDays(1);

                    }
                    endg = (endg.Date + Endt);

                    startd = (startd.Date + startt);
                    endd = (endd.Date + Endt);
                }
                else
                {
                    DateTime startd = DateTime.Parse(Formdate);
                    DateTime endd = DateTime.Parse(todate);
                    startd = (startd.Date + starttime);
                    endd = (endd.Date + starttime);
                    startg = DateTime.Parse(Formdate);
                    startg = (startg.Date + starttime);
                    endg = endg.Date.AddDays(1);
                    endg = (endg.Date + starttime);

                }

                if (Minute != "" && (ddlformat == "2" || ddlformat == "1"))
                {
                    hourlyArea = db.Sp_Shift_Areawise_TimeConsumption(startg, endg, "", "", plantID, Shop, Convert.ToInt32(Minute), Shiftwise, type).ToList();
                }
                else if (ddlformat == "5" && hours <= 24)
                {
                    hourlyArea = db.Sp_Shift_Areawise_TimeConsumption(start_m, end_m, "", "", plantID, Shop, Convert.ToInt32(Minute), Shiftwise, type).ToList();
                }
                else
                {
                    shiftarea = db.Sp_Shift_AreawiseConsumption(startdate1, enddate1, "", "", plantID, Shop, Shiftwise, type).ToList();
                }

                foreach (var area in db.MM_Area.Where(s => s.Shop_ID == Shop).Select(s => s.Area_Id).ToList())
                {
                    Area = new List<Areawise_Consumption>();
                    double total = 0.0;
                    var Name = "";
                    if (Minute != "")
                    {


                        foreach (var id in hourlyArea.Where(s => s.Area_Id == area).OrderBy(s => s.DateandTime))
                        {
                            var areadate = Convert.ToDateTime(id.DateandTime).ToString("hh:mm tt");
                            Name = id.Area_Name;
                            Areawise_Consumption obj = new Areawise_Consumption(id.Area_Name, areadate, Convert.ToDouble(id.Consumption), Convert.ToInt32(id.Area_Id));
                            total += Convert.ToDouble(id.Consumption);
                            Area.Add(obj);

                        }
                        Areawise_Consumption objPareto = new Areawise_Consumption(Name, "", total, Convert.ToInt32(area));
                        AreaWisePareto.Add(objPareto);
                    }

                    else
                    {


                        foreach (var id in shiftarea.Where(s => s.Area == area).OrderBy(s => s.ConsumptionDate))
                        {
                            var areadate = Convert.ToDateTime(id.ConsumptionDate).ToString("dd/MMM");
                            Name = id.Area_Name;

                            Areawise_Consumption obj = new Areawise_Consumption(id.Area_Name, areadate, Convert.ToDouble(id.Consumption), Convert.ToInt32(id.Area));
                            total += Convert.ToDouble(id.Consumption);
                            Area.Add(obj);
                        }
                        Areawise_Consumption objPareto = new Areawise_Consumption(Name, "", total, Convert.ToInt32(area));
                        AreaWisePareto.Add(objPareto);
                    }

                    Areawise.Add(Area);

                }
                var Pareto = AreaWisePareto.OrderByDescending(s => s.TotalConsumtion).ToList();

                return Json(new { Areawise, Pareto }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult AreawiseCBM(string Date, int Shop, string ConsumptionId, int? Area, string CBM)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int type = 1;
                Boolean typs = true;

                string unit = "kWh";
                if (ConsumptionId != "1")
                {
                    type = 2;
                    unit = "kvah";
                    typs = false;

                }

                List<Areawise_CBM> Areaobj = new List<Areawise_CBM>();
                List<List<Areawise_CBM>> Areawise = new List<List<Areawise_CBM>>();
                //List<Areawise_CBM> AreaWisePareto = new List<Areawise_CBM>();
                List<Areawise_Consumption> AreaWisePareto = new List<Areawise_Consumption>();
                DateTime startdate1 = DateTime.Parse(Date);
                DateTime enddate1 = DateTime.Parse(Date);

                var shift = (from s in db.MM_MTTUW_Shift
                             where s.Shift_ID==25
                             select
                             new
                             {
                                 s.Shift_Start_Time
                             }).First();
                var starttime = TimeSpan.Parse(shift.Shift_Start_Time.ToString());
                enddate1 = enddate1.AddDays(1);
                enddate1 = (enddate1 + starttime);
                startdate1 = (startdate1 + starttime);

                var hourlyArea = db.MM_AreawiseHourlyConsumption.Where(s => s.DateandTime >= startdate1 && s.DateandTime <= enddate1 && s.Area_ID == Area && s.Shop_ID == Shop && s.ConsumptionType == type).ToList();

                var Name = "";
                foreach (var id in hourlyArea.Where(s => s.Area_ID == Area).OrderBy(s => s.DateandTime))
                {
                    var areadate = Convert.ToDateTime(id.DateandTime).ToString("hh:mm tt");

                    Name = db.MM_Area.Where(s => s.Area_Id == Area).Select(s => s.Area_Name).FirstOrDefault().ToString();
                    Areawise_CBM obj = new Areawise_CBM(Name, areadate, Convert.ToDouble(id.TotalConsumption), Convert.ToInt32(id.Area_ID), unit);
                    Areaobj.Add(obj);
                }


                Areawise.Add(Areaobj);

                string[] CBMArray = { };
                if (CBM == "null")
                {


                }
                else
                {
                    CBMArray = CBM.Split(',').ToArray();
                }
                foreach (var item in CBMArray)
                {
                    Areaobj = new List<Areawise_CBM>();
                    int CBMID = item != "" ? Convert.ToInt32(item) : -1;
                    var CBMData = db.MM_Ctrl_CBM_Hourly_Data.Where(s => s.DateandTime >= startdate1 && s.DateandTime <= enddate1 && s.CBM_ID == CBMID).ToList();
                    foreach (var id in CBMData)
                    {
                        var areadate = Convert.ToDateTime(id.DateandTime).ToString("hh:mm tt");
                        unit = db.MM_MT_Conditional_Based_Maintenance.Where(s => s.CBM_ID == id.CBM_ID).Select(s => s.UOM).FirstOrDefault().ToString();
                        Name = db.MM_MT_Conditional_Based_Maintenance.Where(s => s.CBM_ID == id.CBM_ID).Select(s => s.Machine_Parameter).FirstOrDefault().ToString();
                        Areawise_CBM obj = new Areawise_CBM(Name, areadate, Convert.ToDouble(id.Parameter_Value), Convert.ToInt32(id.CBM_ID), unit);
                        Areaobj.Add(obj);
                    }
                    Areawise.Add(Areaobj);
                }

                var Today = System.DateTime.Now;

                List<Sp_Shift_AreawiseConsumption_Result> shiftareas = new List<Sp_Shift_AreawiseConsumption_Result>();
                List<Sp_Shift_Areawise_TimeConsumption_Result> hourlyAreas = new List<Sp_Shift_Areawise_TimeConsumption_Result>();

                if (Today.Date == startdate1.Date)
                {
                    hourlyAreas = db.Sp_Shift_Areawise_TimeConsumption(startdate1, enddate1, "", "", plantID, Shop, Convert.ToInt32(60), null, typs).ToList();
                }
                else
                {
                    shiftareas = db.Sp_Shift_AreawiseConsumption(startdate1, enddate1, "", "", plantID, Shop, null, typs).ToList();
                }
                foreach (var area in db.MM_Area.Where(s => s.Shop_ID == Shop).Select(s => s.Area_Id).ToList())
                {

                    double total = 0.0;
                    Name = "";

                    if (Today.Date == startdate1.Date)
                    {

                        foreach (var id in hourlyAreas.Where(s => s.Area_Id == area).OrderBy(s => s.DateandTime))
                        {
                            var areadate = Convert.ToDateTime(id.DateandTime).ToString("hh:mm tt");
                            Name = id.Area_Name;
                            total += Convert.ToDouble(id.Consumption);
                        }
                        Areawise_Consumption objPareto = new Areawise_Consumption(Name, "", total, Convert.ToInt32(area));
                        AreaWisePareto.Add(objPareto);
                    }
                    else
                    {
                        foreach (var id in shiftareas.Where(s => s.Area == area).OrderBy(s => s.ConsumptionDate))
                        {
                            var areadate = Convert.ToDateTime(id.ConsumptionDate).ToString("hh:mm tt");
                            Name = id.Area_Name;
                            total += Convert.ToDouble(id.Consumption);
                        }
                        Areawise_Consumption objPareto = new Areawise_Consumption(Name, "", total, Convert.ToInt32(area));
                        AreaWisePareto.Add(objPareto);
                    }
                }
                var Pareto = AreaWisePareto.OrderByDescending(s => s.TotalConsumtion).ToList();
                return Json(new { Areawise, Pareto }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ForeCast(string ConsumptionId, int? Shop)
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


                var financial_avg = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime >= dte && s.DateandTime <= lastmonthd && s.Shop_ID == Shop && s.ConsumptionType == type).Average(s => s.TotalConsumption);
                var LastMonthToatl = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime >= firstmonthd && s.DateandTime <= lastmonthd && s.Shop_ID == Shop && s.ConsumptionType == type).Sum(s => s.TotalConsumption);

                var Perform = (from p in db.MM_Performance_Indices_Energy
                               where p.DateandTime == now && p.ConsumptionType == type && p.Shop_ID == Shop
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
                               where (p.DateandTime) >= month1stDate && (p.DateandTime) <= endmonthd && p.ConsumptionType == type && p.Shop_ID == Shop
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

        public class Areawise_CBM
        {
            public string Name { get; set; }
            public string date { get; set; }
            public Double Value { get; set; }
            public int ID { get; set; }

            public string unit { get; set; }


            public Areawise_CBM(string Name, string date, double Value, int ID, string unit)
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

