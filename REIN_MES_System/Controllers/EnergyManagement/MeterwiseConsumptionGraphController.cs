using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using Newtonsoft.Json;
using ClosedXML.Excel;
using System.IO;

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class MeterwiseConsumptionGraphController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();
       

 
        // Analytics ALL Parameter wise 

        public ActionResult analytics()
        {
            try
            {

            
           

            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
             ViewBag.ddlDateRange = new SelectList(db.MM_DateRange, "DateID", "DateName", 1);
            //ViewBag.ddlDateRange = new SelectList(db.MM_DateRange.ToList(), "DateID", "DateName", 1);
            //ViewBag.ddlDateRange = new SelectList(db.MM_DateRange.Where(s => s.DateID == 1), "DateID", "DateName", 1);
            ViewBag.ShopName = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
            ViewBag.Parameter = new SelectList(db.MM_Parameter, "Prameter_ID", "Prameter_Name");
            globalData.pageTitle = "Analytics";
            ViewBag.GlobalDataModel = globalData;
            return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }

        }


        public ActionResult Analyicsview(string StartDate, string EndDate, string Shop, string ddlformate)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            List<List<double?>> AllConsumption = new List<List<double?>>();
            List<string> Names = new List<string>();
            List<double?> Consumption = new List<double?>();
            List<int> Production = new List<int>();
            List<double?> bestConsumption = new List<double?>();
            List<double?> MinConsumption = new List<double?>();
            List<double?> MaxConsumption = new List<double?>();
            List<double?> differenceConsumption = new List<double?>();
            List<double?> AvergeConsumption = new List<double?>();
            List<double?> TotalConsumption = new List<double?>();
            //List<string> allTimes = new List<string>();
            //List<string> allTimes = new List<string>();
            List<string> allDates = new List<string>();
            DateTime fromdate = DateTime.Now;
            DateTime toDate = DateTime.Now;
            DateTime date = DateTime.Now.Date;
            DateTime dtn = DateTime.Now.Date;
            var ShiftTime = (from s in db.MM_MTTUW_Shift
                             where s.Shop_ID == 30
                             select new
                             {
                                 s.Shift_Start_Time,
                                 s.Shift_End_Time
                             }).FirstOrDefault();
            var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
            var End = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());

            //var time = TimeSpan.Parse("06:30:00.000");

            //var time1 = TimeSpan.Parse("06:40:00.000");
            date = date.AddDays(-1);
            string starttime = start.ToString();
            string endtime = End.ToString();
            DateTime startdate1 = DateTime.Parse(StartDate);
            DateTime enddate1 = DateTime.Parse(EndDate);

         
           
                enddate1 = enddate1.AddDays(1);
                starttime = start.ToString();
                endtime = start.ToString();

            int ShopId = Convert.ToInt32(Shop);
           
    
            List<Sp_ShopwiseSummary_Result> obj3 = null;
            obj3 = db.Sp_ShopwiseSummary(ShopId, startdate1, enddate1, plantID, starttime, endtime).ToList();
            for (DateTime date1 = Convert.ToDateTime(startdate1), date2 = Convert.ToDateTime(startdate1).AddDays(1); date2 <= Convert.ToDateTime(enddate1); date1 = date1.AddDays(1), date2 = date2.AddDays(1))
            {
                string ddl = Convert.ToDateTime(date1).ToString("dd/MMM");
                allDates.Add(ddl);

                var cons = db.Sp_AllShopConsumption(date1, date2, plantID).Where(m => m.Shop_ID == ShopId).FirstOrDefault();
                var prod = db.sp_getdailyProductionCount(date1, date2, plantID).Where(m => m.ShopId == ShopId).FirstOrDefault();
                double cons1 = 0;
                if ((prod.totalproduction == 0||prod.totalproduction ==null) ||(cons.totalconsumption == 0 ||cons.totalconsumption == null ))
                {

                    Consumption.Add(0);
                    Production.Add(0);
                }
                else
                {

                    cons1 = (Convert.ToDouble(cons.totalconsumption / prod.totalproduction));
                    Consumption.Add(System.Math.Round(cons1, 2));
                    bestConsumption.Add(System.Math.Round(cons1, 2));
                    Production.Add(Convert.ToInt32(prod.totalproduction));
                    TotalConsumption.Add(Convert.ToDouble(cons.totalconsumption));
                }

            }
            var Min = System.Math.Round(Convert.ToDouble(bestConsumption.Min()), 2);
            var Max = System.Math.Round(Convert.ToDouble(Consumption.Max()), 2);
            var ToatlComsumption = TotalConsumption.Sum();
            var TotalProduction = Production.Sum();
            var Avarage = System.Math.Round(Convert.ToDouble(ToatlComsumption / TotalProduction), 2);

            //var Avarage = System.Math.Round(Convert.ToDouble(Consumption.Average()),0);


            foreach (var consum in Production)
            {
            
                if (consum == 0)
                {                
                    AvergeConsumption.Add(0);
                    MinConsumption.Add(0);
                    MaxConsumption.Add(0);
                    differenceConsumption.Add(0);


                }
                else
                {
                   var Minval = System.Math.Round((Min * consum),2);
                    var Avarageval = System.Math.Round((Avarage * consum),2);
                    var Maxval = System.Math.Round((Max * consum),0);
                    var cons1val = System.Math.Round(Convert.ToDouble(Maxval - Minval),2);
                    AvergeConsumption.Add(System.Math.Round(Convert.ToDouble(Avarageval), 2));
                    MinConsumption.Add(System.Math.Round(Convert.ToDouble(Minval), 2));
                    MaxConsumption.Add(System.Math.Round(Convert.ToDouble(Maxval), 2));
                    //cons1 = (Convert.ToDouble(diff - Min));
                    differenceConsumption.Add(System.Math.Round(cons1val, 2));
                }
            }

            //foreach(var diff in Consumption)
            //{
            //    double cons1 = 0;
            //    if (diff == 0)
            //    {
            //        AvergeConsumption.Add(System.Math.Round(Convert.ToDouble(Avarage), 0));
            //        MinConsumption.Add(System.Math.Round(Convert.ToDouble(Min), 0));
            //        MaxConsumption.Add(System.Math.Round(Convert.ToDouble(Max), 0));
            //        differenceConsumption.Add(0);

                   
            //    }
            //   else
            //    {                  
            //        AvergeConsumption.Add(System.Math.Round(Convert.ToDouble(Avarage), 0));
            //        MinConsumption.Add(System.Math.Round(Convert.ToDouble(Min), 0));
            //        MaxConsumption.Add(System.Math.Round(Convert.ToDouble(Max), 0));
            //        cons1 = (Convert.ToDouble(diff - Min));
            //        differenceConsumption.Add(System.Math.Round(cons1, 0));
            //    }
            //}
            //AllConsumption.Add(Consumption);
            AllConsumption.Add(AvergeConsumption);
            AllConsumption.Add(MinConsumption);
            AllConsumption.Add(MaxConsumption);
            AllConsumption.Add(differenceConsumption);
            //Names.Add("Actual");          
            Names.Add("Avarage");
            Names.Add("Min");
            Names.Add("Max");
            Names.Add("Difference");
            return Json(new { AllConsumption, Names, allDates }, JsonRequestBehavior.AllowGet);



            //return View();
        }

        public ActionResult Analyicstimeview(string StartDate, string EndDate, string Shop, string ddlformate, string feeder, string fromTime, string toTime)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            List<List<double?>> AllConsumption = new List<List<double?>>();
            List<string> Names = new List<string>();
            //List<double?> LiveConsumption = new List<double?>();
            //List<double?> AverageConsumption = new List<double?>();
            //List<double?> bestConsumption = new List<double?>();
            List<string> allDates = new List<string>();

            DateTime EndDate1 = System.DateTime.Now;
            DateTime StartDate1 = EndDate1.Date;
            var ShiftTime = (from s in db.MM_MTTUW_Shift
                             where s.Shop_ID == 30
                             select new
                             {
                                 s.Shift_Start_Time,
                                 s.Shift_End_Time
                             }).FirstOrDefault();

            var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
            var End = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
            //var time = TimeSpan.Parse("07:00:00.000");
            StartDate1 = (StartDate1 + start);
            int ShopId = Convert.ToInt32(Shop);
            
            string[] FeederArray = { };
            if (feeder == "null")
            {


            }
            else
            {
                FeederArray = feeder.Split(',').ToArray();
            }
            if (fromTime != "")
            {
                var starttime = TimeSpan.Parse(fromTime);
                var endtime = TimeSpan.Parse(toTime);
                DateTime startdate1 = DateTime.Parse(StartDate);
                DateTime enddate1 = DateTime.Parse(StartDate);

                startdate1 = (startdate1 + starttime);
                enddate1 = (enddate1 + endtime);
                for (DateTime date1 = Convert.ToDateTime(startdate1), date2 = Convert.ToDateTime(startdate1); date2 <= Convert.ToDateTime(enddate1); date1 = date2)
                {

                    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                    allDates.Add(ddl);
                    //allDates.Add(ddl);
                    date2 = date2.AddMinutes(15);
                }
                foreach (var item in FeederArray)
                {
                    int feederId = feeder == "" ? 0 : Convert.ToInt32(item);
                    List<double?> LiveConsumption = new List<double?>();
                    List<double?> AverageConsumption = new List<double?>();
                    List<double?> bestConsumption = new List<double?>();
                    var TimePerforamce = (from b in db.MM_Feederwise_TimeConsumption
                                      where b.Shop_ID == ShopId && b.TagIndex == feederId &&
                                     b.DateandTime >= startdate1 && b.DateandTime <= enddate1
                                          select new
                                      {
                                          b.Consumption,
                                          b.Updated_Date,
                                          b.Average,
                                          b.Best
                                      }).ToList().OrderBy(m => m.Updated_Date);
                    //foreach (var id in Avg)
                    //{
                    //    AverageConsumption.Add(id.Consumption);
                    //}
                    foreach (var id in TimePerforamce)
                    {
                        LiveConsumption.Add(id.Consumption);
                        AverageConsumption.Add(id.Average);
                        bestConsumption.Add(id.Best);
                    }

                    AllConsumption.Add(LiveConsumption);
                    AllConsumption.Add(AverageConsumption);
                    AllConsumption.Add(bestConsumption);
                    int tagindex1 = Convert.ToInt32(feederId);
                    var feeder1 = (from f in db.UtilityMainFeederMappings
                                   where f.TagIndex == tagindex1
                                   select new
                                   {
                                       f.FeederName
                                   }).FirstOrDefault();
                    // var FeederName = obj[0].FeederName;
                    var FeederName = feeder1 != null ? feeder1.FeederName : "";
                    Names.Add(FeederName + " - Live ");
                    Names.Add(FeederName + " - Average");
                    Names.Add(FeederName + " - Best");
                }
            }
            else
            {
                if (ddlformate == "1")
                {
                    for (DateTime date1 = StartDate1, date2 = StartDate1; date2 <= EndDate1; date1 = date2)
                    {



                        string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                        allDates.Add(ddl);
                        date2 = date2.AddMinutes(15);
                    }
                }
                else
                {
                    for (DateTime date1 = Convert.ToDateTime(StartDate), date2 = Convert.ToDateTime(StartDate); date2 <= Convert.ToDateTime(EndDate); date1 = date2)
                    {

                        string ddl = Convert.ToDateTime(date1).ToString("dd/MMM");
                        allDates.Add(ddl);
                        date2 = date2.AddDays(1);
                    }
                }
                foreach (var item in FeederArray)
                {
                    int feederId = feeder == "" ? 0 : Convert.ToInt32(item);



                    List<double?> LiveConsumption = new List<double?>();
                    List<double?> AverageConsumption = new List<double?>();
                    List<double?> bestConsumption = new List<double?>();
                    if (ddlformate == "1")
                    {


                        //obj = db.SP_LiveFeederwise_TimeConsumption(ShopId, StartDate1, EndDate1, plantID, "", "", feederId).ToList();
                        //foreach (var id in obj)
                        //{
                        //    LiveConsumption.Add(id.Consumption);
                        //}
                        //var Avg = (from b in db.MM_Feederwise_Time_Averge_Consumption
                        //           where b.Shop_ID == ShopId && b.TagIndex == feederId &&
                        //          b.Updated_Date >= StartDate1
                        //           select new
                        //           {
                        //               b.Consumption,
                        //               b.Updated_Date
                        //           }).ToList().OrderBy(m => m.Updated_Date);

                        var Perforamce = (from b in db.MM_Feederwise_TimeConsumption
                                          where b.Shop_ID == ShopId && b.TagIndex == feederId &&
                                         b.DateandTime >= StartDate1
                                          select new
                                          {
                                              b.Consumption,
                                              b.Updated_Date,
                                              b.Average,
                                              b.Best
                                          }).ToList();
                        //foreach (var id in Avg)
                        //{
                        //    AverageConsumption.Add(id.Consumption);
                        //}
                        foreach (var id in Perforamce)
                        {
                            LiveConsumption.Add(id.Consumption);
                            AverageConsumption.Add(id.Average);
                            bestConsumption.Add(id.Best);
                        }

                        AllConsumption.Add(LiveConsumption);
                        AllConsumption.Add(AverageConsumption);
                        AllConsumption.Add(bestConsumption);
                    }
                    else if (ddlformate == "2")
                    {
                        DateTime today = DateTime.Now.Date;
                        var startTime = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString()); ;
                        var EndTime = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString()); ;
                        DateTime End1 = System.DateTime.Now;
                        DateTime Start = End1.Date;
                        //var time = TimeSpan.Parse("06:30:00.000");
                        //Start = (Start + time);

                        today = (Start + EndTime);
                        DateTime startDate1 = DateTime.Parse(StartDate);
                        startDate1 = (startDate1 + startTime);

                        allDates.Clear();
                        for (DateTime date1 = Convert.ToDateTime(startDate1), date2 = Convert.ToDateTime(startDate1); date2 <= Convert.ToDateTime(today); date1 = date2)
                        {

                            string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                            allDates.Add(ddl);
                            //allDates.Add(ddl);
                            date2 = date2.AddMinutes(15);
                        }

                        var Perforamce = (from b in db.MM_Feederwise_TimeConsumption
                                          where b.Shop_ID == ShopId && b.TagIndex == feederId &&
                                         b.DateandTime >= startDate1 && b.DateandTime <= today
                                          select new
                                          {
                                              b.Consumption,
                                              b.Updated_Date,
                                              b.Average,
                                              b.Best
                                          }).ToList().OrderBy(m => m.Updated_Date);
                        //foreach (var id in Avg)
                        //{
                        //    AverageConsumption.Add(id.Consumption);
                        //}
                        foreach (var id in Perforamce)
                        {
                            LiveConsumption.Add(id.Consumption);
                            AverageConsumption.Add(id.Average);
                            bestConsumption.Add(id.Best);
                        }

                        AllConsumption.Add(LiveConsumption);
                        AllConsumption.Add(AverageConsumption);
                        AllConsumption.Add(bestConsumption);
                    }
                    else
                    {
                        DateTime startdate1 = DateTime.Parse(StartDate);
                        DateTime enddate1 = DateTime.Parse(EndDate);

                        var Perform = (from p in db.MM_Feederwise_Daily_Analytics
                                       where (p.DateandTime) >= startdate1 && (p.DateandTime) <= enddate1
                                       && p.TagIndex == feederId
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
                        //var Avg = (from b in db.MM_Feederwise_Time_Averge_Consumption
                        //           where b.Shop_ID == ShopId && b.TagIndex == feederId &&
                        //          b.Updated_Date >= StartDate1
                        //           select new
                        //           {
                        //               b.Consumption,
                        //               b.Updated_Date
                        //           }).ToList().OrderBy(m => m.Updated_Date);

                        //var best = (from b in db.MM_Feederwise_TimeConsumption
                        //            where b.Shop_ID == ShopId && b.TagIndex == feederId &&
                        //           b.Updated_Date >= StartDate1
                        //            select new
                        //            {
                        //                b.Consumption,
                        //                b.Updated_Date
                        //            }).ToList().OrderBy(m => m.Updated_Date);
                        foreach (var id in Perform)
                        {
                            LiveConsumption.Add(id.Consumption);
                            AverageConsumption.Add(id.Average);
                            bestConsumption.Add(id.Best);
                        }


                        AllConsumption.Add(LiveConsumption);
                        AllConsumption.Add(AverageConsumption);
                        AllConsumption.Add(bestConsumption);
                    }
                    int tagindex1 = Convert.ToInt32(feederId);
                    var feeder1 = (from f in db.UtilityMainFeederMappings
                                   where f.TagIndex == tagindex1
                                   select new
                                   {
                                       f.FeederName
                                   }).FirstOrDefault();
                    // var FeederName = obj[0].FeederName;
                    var FeederName = feeder1 != null ? feeder1.FeederName : "";
                    Names.Add(FeederName + " - Live ");
                    Names.Add(FeederName + " - Average");
                    Names.Add(FeederName + " - Best");


                }
            }
            object unit = null;
            unit = "kwh";
            return Json(new { AllConsumption, Names, allDates, unit }, JsonRequestBehavior.AllowGet);



            //return View();
        }

        public ActionResult Parameterwiseview(string StartDate, string EndDate, string Shop, string ddlformate, string ParameterID, string fromTime, string toTime, string feeder)
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            int ShopId = Convert.ToInt32(Shop);
            object unit = null;
            int ParameterId = Convert.ToInt32(ParameterID);
            List<List<double?>> AllConsumption = new List<List<double?>>();
            List<string> Names = new List<string>();
            List<double?> LiveConsumption = new List<double?>();
            List<double?> AverageConsumption = new List<double?>();
            List<double?> bestConsumption = new List<double?>();
            List<string> allDates = new List<string>();
           
           
            List<Sp_ParameterWiseAnalaytics_Result> Parameterwiseobj = null;
            List<double?> ParameterConsumption = new List<double?>();
            DateTime EndDate1 = System.DateTime.Now;
            DateTime StartDate1 = EndDate1.Date;
            var time = TimeSpan.Parse("07:00:00.000");
            StartDate1 = (StartDate1 + time);
            var ShiftTime = (from s in db.MM_MTTUW_Shift
                             where s.Shop_ID == 1
                             select new
                             {
                                 s.Shift_Start_Time,
                                 s.Shift_End_Time
                             }).FirstOrDefault();
        


            // var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
            //TimeSpan fromTime1 = fromTime != "" ? TimeSpan.Parse(fromTime) : TimeSpan.Parse("06:30:00.000");
            //TimeSpan toTime1 = toTime != "" ? TimeSpan.Parse(toTime) : TimeSpan.Parse("06:30:00.000");
            TimeSpan fromTime1 = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
            TimeSpan toTime1 = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
            if (fromTime != "")
            {
                if (ParameterID == "1")
                {


                    var starttime = TimeSpan.Parse(fromTime);
                    var endtime = TimeSpan.Parse(toTime);
                    DateTime startdate1 = DateTime.Parse(StartDate);
                    DateTime enddate1 = DateTime.Parse(StartDate);

                    startdate1 = (startdate1 + starttime);
                    enddate1 = (enddate1 + endtime);
                    for (DateTime date1 = Convert.ToDateTime(startdate1), date2 = Convert.ToDateTime(startdate1); date2 <= Convert.ToDateTime(enddate1); date1 = date2)
                    {

                        string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                        allDates.Add(ddl);
                        //allDates.Add(ddl);
                        date2 = date2.AddMinutes(15);
                    }
                    var TimeConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                           where b.Shop_ID == ShopId &&
                                          b.Inserted_Date >= startdate1 && b.Inserted_Date <= enddate1
                                           select new
                                           {
                                               b.Consumption,
                                               b.Best,
                                               b.Average,
                                               b.Inserted_Date
                                           }).ToList().OrderBy(m => m.Inserted_Date);
                    foreach (var id in TimeConsumption)
                    {
                        LiveConsumption.Add(id.Consumption);
                        bestConsumption.Add(id.Best);
                        AverageConsumption.Add(id.Average);

                    }
                    AllConsumption.Add(LiveConsumption);
                    AllConsumption.Add(bestConsumption);
                    AllConsumption.Add(AverageConsumption);
                }
                else
                {
                    var starttime = TimeSpan.Parse(fromTime);
                    var endtime = TimeSpan.Parse(toTime);
                    DateTime startdate1 = DateTime.Parse(StartDate);
                    DateTime enddate1 = DateTime.Parse(StartDate);

                    startdate1 = (startdate1 + starttime);
                    enddate1 = (enddate1 + endtime);
                    string[] FeederArray = { };
                   
                    if (feeder == "null")
                    {
                    }
                    else
                    {
                        FeederArray = feeder.Split(',').ToArray();
                    }
                    foreach (var item in FeederArray)
                    {
                        List<double?> Consumption = new List<double?>();
                        //List<double?> Final_Consumption = new List<double?>();

                        int ShopID = Convert.ToInt32(Shop);
                        int feederId = Convert.ToInt32(item);
                        // Parameterwiseobj = null;
                        Parameterwiseobj = db.Sp_ParameterWiseAnalaytics(ShopID, startdate1, enddate1, plantID, ParameterId, starttime.ToString(), endtime.ToString(), feederId).ToList();
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

                        if (feederId == -1)
                        {
                            var shop = (from s in db.MM_MTTUW_Shops
                                        where s.Shop_ID == ShopId
                                        select new { s.Shop_Name }).FirstOrDefault();

                            AllConsumption.Add(ParameterConsumption);
                            Names.Add(shop.Shop_Name);
                        }
                        else
                        {
                            var feeder1 = (from f in db.UtilityMainFeederMappings
                                           where f.TagIndex == feederId
                                           select new
                                           {
                                               f.FeederName,
                                               f.Unit
                                           }).FirstOrDefault();
                            // var FeederName = obj[0].FeederName;
                            var FeederName = feeder1 != null ? feeder1.FeederName : "";

                            unit = feeder1.Unit;

                            AllConsumption.Add(Consumption);
                            Names.Add(FeederName);
                        }

                    }

                    if (FeederArray.Count() == 1)
                    {

                    }
                    else
                    {
                        allDates = new List<string>();

                        for (DateTime date1 = startdate1, date2 = date1.AddMinutes(6); date2 <= enddate1; date1 = date2, date2 = date2.AddMinutes(6))
                        {
                            string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                            allDates.Add(ddl);

                        }

                    }
                }
            }
            else
            {
              if (ParameterID == "1")
                {
                if (ddlformate == "1")
                {

                    //obj = db.SP_LiveShopwise_TimeConsumptionperVechicle(ShopId, StartDate1, EndDate1, plantID, "", "").ToList();
                    //foreach (var id in obj)
                    //{
                    //    LiveConsumption.Add(id.Consumption);
                    //}

                    for (DateTime date1 = Convert.ToDateTime(StartDate1), date2 = Convert.ToDateTime(StartDate1); date2 <= Convert.ToDateTime(EndDate1); date1 = date2)
                    {

                        string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                        allDates.Add(ddl);
                        //allDates.Add(ddl);
                        date2 = date2.AddMinutes(15);
                    }

                    //for (DateTime date1 = StartDate1, date2 = StartDate1.AddMinutes(15); date2 <= EndDate1; date1 = date2, date2 = date2.AddMinutes(15))
                    //{

                    //    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                    //    allDates.Add(ddl);

                    //}

                    //var Avg = (from b in db.MM_Shopwise_Time_Average_Consumption
                    //           where b.Shop_ID == ShopId &&
                    //          b.Updated_Date >= StartDate1
                    //           select new
                    //           {
                    //               b.Consumption,
                    //               b.Updated_Date
                    //           }).ToList().OrderBy(m => m.Updated_Date);

                    var TimeConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                           where b.Shop_ID == ShopId &&
                                          b.Inserted_Date >= StartDate1
                                           select new
                                           {
                                               b.Consumption,
                                               b.Best,
                                               b.Average,
                                               b.Inserted_Date
                                           }).ToList().OrderBy(m => m.Inserted_Date);
                    foreach (var id in TimeConsumption)
                    {
                        LiveConsumption.Add(id.Consumption);
                        bestConsumption.Add(id.Best);
                        AverageConsumption.Add(id.Average);

                    }
                    //foreach (var id in Avg)
                    //{
                    //    AverageConsumption.Add(id.Consumption);
                    //}


                    AllConsumption.Add(LiveConsumption);
                    AllConsumption.Add(bestConsumption);
                    AllConsumption.Add(AverageConsumption);
               
                   
                
                }
                else if (ddlformate == "2")
                {
                    DateTime today = DateTime.Now.Date;
                    var startTime = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString()); ;
                    var EndTime = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString()); ;
                    DateTime End = System.DateTime.Now;
                    DateTime Start = End.Date;
                    //var time = TimeSpan.Parse("06:30:00.000");
                    //Start = (Start + time);

                    today = (Start + time);
                    DateTime startDate1 = DateTime.Parse(StartDate);
                    startDate1 = (startDate1 + startTime);
                    //for (DateTime date1 = startDate1, date2 = startDate1.AddMinutes(15); date2 <= today; date1 = date2, date2 = date2.AddMinutes(15))
                    //{

                    //    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                    //    allDates.Add(ddl);

                    //}
                    for (DateTime date1 = Convert.ToDateTime(startDate1), date2 = Convert.ToDateTime(startDate1); date2 <= Convert.ToDateTime(today); date1 = date2)
                    {

                        string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                        allDates.Add(ddl);
                        //allDates.Add(ddl);
                        date2 = date2.AddMinutes(15);
                    }
                    var TimeConsumption = (from b in db.MM_Shopwise_TimeConsumption
                                           where b.Shop_ID == ShopId &&
                                          b.Inserted_Date >= startDate1 && b.Inserted_Date <= today
                                           select new
                                           {
                                               b.Consumption,
                                               b.Best,
                                               b.Average,
                                               b.Inserted_Date
                                           }).ToList().OrderBy(m => m.Inserted_Date);
                    foreach (var id in TimeConsumption)
                    {
                        LiveConsumption.Add(id.Consumption);
                        bestConsumption.Add(id.Best);
                        AverageConsumption.Add(id.Average);

                    }
                    //foreach (var id in Avg)
                    //{
                    //    AverageConsumption.Add(id.Consumption);
                    //}


                    AllConsumption.Add(LiveConsumption);
                    AllConsumption.Add(bestConsumption);
                    AllConsumption.Add(AverageConsumption);
                  
                }
                else
                {
                    //DateTime custFDate = DateTime.Parse(StartDate);
                    //DateTime custTDate = DateTime.Parse(EndDate);
                    //custFDate = (custFDate + fromTime1);
                    //custTDate = (custTDate + toTime1);
                    for (DateTime date1 = Convert.ToDateTime(StartDate), date2 = Convert.ToDateTime(StartDate); date2 <= Convert.ToDateTime(EndDate); date1 = date2)
                    {

                        string ddl = Convert.ToDateTime(date1).ToString("dd/MMM");
                        allDates.Add(ddl);
                        date2 = date2.AddDays(1);
                    }
                    DateTime startdate1 = DateTime.Parse(StartDate);
                    DateTime enddate1 = DateTime.Parse(EndDate);
                    var Perform = (from p in db.MM_Performance_Indices_Energy
                                   where (p.DateandTime) >= startdate1 && (p.DateandTime) <= enddate1
                                       && p.Shop_ID == ShopId
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

                    allDates.Clear();
                    foreach (var id in Perform)
                    {

                        LiveConsumption.Add(id.Consumption);
                        AverageConsumption.Add(id.Average);
                        bestConsumption.Add(id.Best);
                        string ddl = Convert.ToDateTime(id.DateandTime).ToString("dd/MMM");
                        allDates.Add(ddl);

                    }


                    AllConsumption.Add(LiveConsumption);
                    AllConsumption.Add(bestConsumption);
                    AllConsumption.Add(AverageConsumption);
             
                    //Names.Add("Live");
                    //Names.Add("Average");
                    //Names.Add("Best");
                }
                }
              else
                {
                    fromTime = fromTime != "" ? fromTime + ":00.000" : "";
                    toTime = toTime != "" ? toTime + ":00.000" : "";
                    string[] FeederArray = { };
                    DateTime  Todate, FormDate;
                    if (feeder == "null")
                    {
                    }
                    else
                    {
                        FeederArray = feeder.Split(',').ToArray();
                    }
                    if(ddlformate == "1")
                    {
                        DateTime startdate1 = DateTime.Parse(StartDate);
                        FormDate = (startdate1 + time);
                        Todate = DateTime.Now;
                    }
                    else if (ddlformate == "2")
                    {
                        DateTime startdate1 = DateTime.Parse(StartDate).AddDays(-1);
                        DateTime enddate1 = DateTime.Parse(EndDate);
                        FormDate = (startdate1 + time);
                        Todate = (enddate1 + time);
                    }
                    else
                    {
                        DateTime startdate1 = DateTime.Parse(StartDate);
                        DateTime enddate1 = DateTime.Parse(EndDate);
                        FormDate = (startdate1 + time);
                        Todate = (enddate1 + time);
                    }
                    foreach (var item in FeederArray)
                    {
                        List<double?> Consumption = new List<double?>();
                        //List<double?> Final_Consumption = new List<double?>();

                        int ShopID = Convert.ToInt32(Shop);
                        int feederId = Convert.ToInt32(item);
                       // Parameterwiseobj = null;
                        Parameterwiseobj = db.Sp_ParameterWiseAnalaytics(ShopID, FormDate, Todate, plantID, ParameterId, null, null, feederId).ToList();
                        if(Parameterwiseobj.Count() > 0)
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

                        if (feederId == -1)
                        {
                            var shop = (from s in db.MM_MTTUW_Shops
                                        where s.Shop_ID == ShopId
                                        select new { s.Shop_Name }).FirstOrDefault();

                            AllConsumption.Add(ParameterConsumption);
                            Names.Add(shop.Shop_Name);
                        }
                        else
                        {
                            var feeder1 = (from f in db.UtilityMainFeederMappings
                                           where f.TagIndex == feederId
                                           select new
                                           {
                                               f.FeederName,
                                               f.Unit
                                           }).FirstOrDefault();
                            // var FeederName = obj[0].FeederName;
                            var FeederName = feeder1 != null ? feeder1.FeederName : "";

                            unit = feeder1.Unit;

                            AllConsumption.Add(Consumption);
                            Names.Add(FeederName);
                        }

                    }

                    if (FeederArray.Count() == 1)
                    {

                    }
                    else
                    {
                        allDates = new List<string>();

                        for (DateTime date1 = FormDate, date2 = date1.AddMinutes(6); date2 <= Todate; date1 = date2, date2 = date2.AddMinutes(6))
                        {
                            string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                            allDates.Add(ddl);

                        }

                    }
                }
            }
            if (ParameterID == "1")
            {
                Names.Add("Live");
                Names.Add("Best");
                Names.Add("Average");
                unit = "kwh/Vechicle";
            }
            
            return Json(new { AllConsumption, Names, allDates, unit }, JsonRequestBehavior.AllowGet);
        }












        // Feeder wise Reports Graphs 

        public ActionResult Index(int Shop_ID)
        {
            var shopName = db.MM_MTTUW_Shops.Where(m => m.Shop_ID == Shop_ID).Select(m => m.Shop_Name).FirstOrDefault();
            try
            {

                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                int userId = ((FDSession)this.Session["FDSession"]).userId;
                //globalData.pageTitle = ResourceFeederwiseChart.FeederwiseChartTitle;
                globalData.pageTitle = "Report : " + shopName;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.ShopID = Shop_ID;
                ViewBag.Plant_ID = new SelectList((from s in db.MM_MTTUW_Plants
                                                   where s.Plant_ID == plantID
                                                   select (s)).ToList(), "Plant_ID", "Plant_Name", plantID);
                ViewBag.Shift = new SelectList(db.MM_MTTUW_Shift.Where(s=>s.Shop_ID ==30), "Shift_ID", "Shift_Name");
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange, "DateID", "DateName", 2);
                //ViewBag.ddlDateRange = new SelectList(db.MM_DateRange.Where(x=>x.DateID ==1 || x.DateID  == 3 || x.DateID == 4), "DateID", "DateName", 1);
                ViewBag.ShopName = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                return View();
            }
            catch
            {
                globalData.pageTitle = "Report : " + shopName;
                ViewBag.GlobalDataModel = globalData;
                return RedirectToAction("Index", "user");
            }

        }

        public ActionResult ConsumptionGraph()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceFeederwiseChart.FeederwiseChartTitle;
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
                ViewBag.TagIndex = new SelectList(db.UtilityMainFeederMappings, "TagIndex", "FeederName");
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                ViewBag.ComFeederShopID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                //globalData.pageTitle = ResourceShopwise_PowerIndexMapping.PageTitle;
                globalData.pageTitle = ResourceFeederwiseChart.FeederwiseChartTitle;

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
     
        public ActionResult Datewise(string StartDate, string EndDate, string Shop, string Shift, string ddlformate, string buttonType)
        {
            try
            {
                List<string> allDates = new List<string>();

                for (DateTime date = Convert.ToDateTime(StartDate); date <= Convert.ToDateTime(EndDate); date = date.AddDays(1))
                {
                    string ddl = Convert.ToDateTime(date).ToString("dd/MMM");
                    allDates.Add(ddl);
                }
                //***************************************** Added by Mukesh  04/01/2019
                if (ddlformate == "3" || ddlformate == "5" || ddlformate == "7")
                {
                    double totalConsPerVehicle = 0;
                    var Shop_ID = Shop != "" ? Convert.ToInt32(Shop) : 0;
                    var ShiftTime = (from s in db.MM_MTTUW_Shift
                                     where s.Shop_ID == 30
                                     select new
                                     {
                                         s.Shift_Start_Time,
                                         s.Shift_End_Time
                                     }).FirstOrDefault();

                    var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                    var End = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                    //var time = TimeSpan.Parse("06:30:00.000");
                    var fromdate = Convert.ToDateTime(StartDate) + start;
                    var toDate = Convert.ToDateTime(EndDate).AddDays(1) + start;
                    var plantID = ((FDSession)this.Session["FDSession"]).plantId;
                    List<Sp_DailyShopwiseConsumption_Result> obj1 = db.Sp_DailyShopwiseConsumption(Shop_ID, fromdate, toDate, plantID).ToList();


                    List<Sp_ProductionCount_Result> prod1 = db.Sp_ProductionCount(Shop_ID, fromdate, toDate, plantID).ToList();
                    if (obj1.Count() > 0)
                    {
                        if (obj1[0].Comsumtionvalues != null)
                        {
                            int production = Convert.ToInt32(prod1[0].Production);
                            if(buttonType == "1")
                            {
                                totalConsPerVehicle = (Math.Round((Convert.ToDouble(obj1[0].Comsumtionvalues) ), 2));

                            }
                            else if(buttonType == "3")
                            {
                                totalConsPerVehicle = (Math.Round((Convert.ToDouble(obj1[0].Comsumtionvalues) / production), 2));

                            }
                            //totalConsPerVehicle = (Math.Round((Convert.ToDouble(obj1[0].Comsumtionvalues) / production), 0));
                        }

                    }
                    var unit = "kWh/Vehicle";
                    return Json(new { allDates, totalConsPerVehicle, unit }, JsonRequestBehavior.AllowGet);
                }
                //***************************************** Added by Mukesh  04/01/2019      

                return Json(allDates, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }
       
        public ActionResult LiveDatewise(string StartDate, string EndDate, string Shop, string Shift, string ddlformate)
        {
            try
            {
                List<string> allTimes = new List<string>();
                var ShiftTime = (from s in db.MM_MTTUW_Shift
                                 where s.Shop_ID == 30
                                 select new
                                 {
                                     s.Shift_Start_Time,
                                     s.Shift_End_Time
                                 }).FirstOrDefault();

                var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                var End = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                if (ddlformate == "1")
                {
                    DateTime EndDate1 = System.DateTime.Now;
                    DateTime StartDate1 = EndDate1.Date;
                   
                    //var time = TimeSpan.Parse("06:30:00.000");
                    StartDate1 = (StartDate1 + start);


                    for (DateTime date = Convert.ToDateTime(StartDate1); date <= Convert.ToDateTime(EndDate1); date = date.AddMinutes(5))
                    {
                        string ddl = Convert.ToDateTime(date).ToString("HH:mm");
                        allTimes.Add(ddl);
                    }

                }
                else
                {
                    DateTime StartDate1 = System.DateTime.Now;
                    DateTime EndDate1 = StartDate1.Date;
                    //var time = TimeSpan.Parse("06:30:00.000");
                    EndDate1 = (EndDate1 + start);
                    DateTime from = DateTime.Parse(StartDate);
                    from = (from + start);


                    for (DateTime date = Convert.ToDateTime(from); date <= Convert.ToDateTime(EndDate1); date = date.AddMinutes(5))
                    {
                        string ddl = Convert.ToDateTime(date).ToString("HH:mm");
                        allTimes.Add(ddl);
                    }
                }


                 return Json(allTimes, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        public ActionResult showchart(string StartDate, string EndDate, string Shop, string Shift)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;



                DateTime fromdate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                DateTime date = DateTime.Now.Date;
                DateTime dtn = DateTime.Now.Date;
                var ShiftTime = (from s in db.MM_MTTUW_Shift
                                 where s.Shop_ID == 30
                                 select new
                                 {
                                     s.Shift_Start_Time,
                                     s.Shift_End_Time
                                 }).FirstOrDefault();

                var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                //var End = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                //var time = TimeSpan.Parse("06:30:00.000");
                //var time1 = TimeSpan.Parse("06:29:00.000");
                date = date.AddDays(-1);
                //fromdate = (date + time);
                //toDate = (date.AddDays(1) + time1);

                //startdate = (startdate + time);
                //enddate = (enddate + time1);
                // production Data 
                DateTime startdate1 = DateTime.Parse("2018-05-05");
                DateTime enddate1 = DateTime.Parse("2018-05-06");


                startdate1 = (startdate1 + start);
                enddate1 = (enddate1 + start);
                var Target1 = (from t in db.MM_PowerTarget
                               join
   s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                               where t.Year == "2018-19" && t.Month == "August"
                               select new
                               {
                                   t.Target,
                                   s.Shop_Name
                               }).ToList();
                List<Sp_AllShopConsumption_Result> consumptionwise = db.Sp_AllShopConsumption(startdate1, enddate1, plantID).ToList();
                List<sp_getdailyProductionCount_Result> Productionwise = db.sp_getdailyProductionCount(startdate1, enddate1, plantID).ToList();
                List<double?> Yesterdaydata = new List<double?>();
                List<double?> TodayData = new List<double?>();
                List<double?> Target = new List<double?>();

                List<string> ShopName = new List<string>();
                // DateTime date = DateTime.Now;




                // consumptionwise = consumptionwise.OrderByDescending(s => s.totalconsumption).ToList();
                //int rCount = consumptionwise.Rows.Count;

                // yesterday data 
                for (int i = 0; i < consumptionwise.Count(); i++)
                {
                    for (int j = 0; j < Productionwise.Count(); j++)
                    {


                        if (consumptionwise[i].Shop_Name == Productionwise[j].Shop_Name)
                        {
                            // double Consumption = 0;
                            if (Productionwise[j].totalproduction == 0)
                            {
                                //Consumption = (Math.Round(Convert.ToDouble(C.totalconsumption), 0));
                                break;
                            }
                            else
                            {
                                int count = Convert.ToInt32(Productionwise[j].totalproduction);

                                Yesterdaydata.Add(System.Math.Round((Convert.ToDouble(consumptionwise[i].totalconsumption) / count), 2));
                                ShopName.Add((consumptionwise[i].Shop_Name));
                            }


                        }

                    }

                }

                // Today Data 
                for (int i = 0; i < consumptionwise.Count(); i++)
                {
                    for (int j = 0; j < Productionwise.Count(); j++)
                    {


                        if (consumptionwise[i].Shop_Name == Productionwise[j].Shop_Name)
                        {
                            // double Consumption = 0;
                            if (Productionwise[j].totalproduction == 0)
                            {
                                //Consumption = (Math.Round(Convert.ToDouble(C.totalconsumption), 0));
                                break;
                            }
                            else
                            {
                                int count = Convert.ToInt32(Productionwise[j].totalproduction);

                                TodayData.Add(System.Math.Round((Convert.ToDouble(consumptionwise[i].totalconsumption) / count), 2));

                            }


                        }

                    }

                }

                // Target data 
                for (int i = 0; i < consumptionwise.Count(); i++)
                {
                    for (int k = 0; k < Target1.Count(); k++)
                    {

                        if (consumptionwise[i].Shop_Name == Target1[k].Shop_Name)
                        {


                            Target.Add(System.Math.Round(Convert.ToDouble(Target1[k].Target), 2));
                            break;

                        }

                    }
                }
                //    TodayData.Add(System.Math.Round(Convert.ToDouble(consumptionwise[i].totalconsumption), 0));




                return Json(new { Yesterdaydata, TodayData, Target, ShopName }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return RedirectToAction("Index", "ShiftWiseReports");
            }
        }

        /************************************** Added by Ajay 22-12-2018 */
        public ActionResult ThisMonthConsumption(string StartDate, string EndDate, string Shop, string ddlformate, string feederId)
        {

            try
            {
                List<string> Names = new List<string>();
                List<double?> ShopTarget = new List<double?>();
                List<List<double?>> AllConsumption = new List<List<double?>>();
                var Shop_ID = Convert.ToInt32(Shop);
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
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
                               where t.Plant_ID == plantID && t.Shop_ID == Shop_ID &&
                                t.Year == FinYear && t.Month == month
                               select new
                               {
                                   t.Target
                               }
                               ).FirstOrDefault();
                DateTime fromdate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                DateTime date = DateTime.Now.Date;
                DateTime dtn = DateTime.Now.Date;
                var ShiftTime = (from s in db.MM_MTTUW_Shift
                                 where s.Shop_ID == 1
                                 select new
                                 {
                                     s.Shift_Start_Time,
                                     s.Shift_End_Time
                                 }).FirstOrDefault();

                var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                var EndTime = (from s in db.MM_MTTUW_Shift
                               where s.Shift_ID ==2
                                 select new
                                 {
                                     s.Shift_Start_Time,
                                     s.Shift_End_Time
                                 }).FirstOrDefault();  
                var End = TimeSpan.Parse((EndTime.Shift_End_Time).ToString());
                //var time = TimeSpan.Parse("06:30:00.000");
                //var time1 = TimeSpan.Parse("23:59:00.000");
                DateTime startdate1 = DateTime.Parse(StartDate);
                DateTime enddate1 = DateTime.Parse(EndDate).AddDays(1);
                date = date.AddDays(-1);
                fromdate = (startdate1 + start);
                toDate = (enddate1 + End);
                List<string> allDates = new List<string>();
                List<double?> Consumption = new List<double?>();
                //double cummCountAvg = 0;
                double cummCount = 0;
                //List<Sp_AllShopConsumption_Result> consumptionwise = null;
                //List<sp_getdailyProductionCount_Result> Productionwise = null;
                for (DateTime date1 = Convert.ToDateTime(fromdate), date2 = Convert.ToDateTime(fromdate).AddDays(1); date2 <= Convert.ToDateTime(toDate); date1 = date1.AddDays(1), date2 = date2.AddDays(1))
                {
                    string ddl = Convert.ToDateTime(date1).ToString("dd/MMM");
                    allDates.Add(ddl);
                    var cons = db.Sp_AllShopConsumption(date1, date2, plantID).Where(m => m.Shop_ID == Shop_ID).FirstOrDefault();
                    var prod = db.sp_getdailyProductionCount(date1, date2, plantID).Where(m => m.ShopId == Shop_ID).FirstOrDefault();
                    double cons1 = 0;
                    if (cons.totalconsumption == null || cons.totalconsumption == 0 || prod.totalproduction == 0)
                    {
                        cummCount += 0;
                        Consumption.Add(0);
                        ShopTarget.Add(System.Math.Round(Convert.ToDouble(Target1.Target), 2));
                    }
                     
                    else
                    {
                        cons1 = Convert.ToDouble(cons.totalconsumption / prod.totalproduction);
                        cummCount += cons1;
                        Consumption.Add(System.Math.Round(cons1, 2));
                        ShopTarget.Add(System.Math.Round(Convert.ToDouble(Target1.Target), 2));
                    }
                        
                    //consumptionwise.Add(db.Sp_AllShopConsumption(date1, date2, plantID).Where(m => m.Shop_ID == Shop_ID).FirstOrDefault());
                    // Productionwise.Add(db.sp_getdailyProductionCount(date1, date2, plantID).Where(m => m.ShopId == Shop_ID).FirstOrDefault());

                }
                AllConsumption.Add(Consumption);
                AllConsumption.Add(ShopTarget);
                Names.Add("Actual");
                Names.Add("Target");
                //cummCountAvg = Math.Round(cummCount / allDates.Count,2);

                return Json(new { AllConsumption, allDates, Names }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "HomeEnergy");
            }


        }
        /************************************** Added by Ajay 22-12-2018 */

        
        public ActionResult Jsonshowchart(string StartDate, string EndDate, string Shop, string ddlformate, string temp, string buttonType, string fromTime, string toTime)
        {
            try
            {
                var ShiftTime = (from s in db.MM_MTTUW_Shift
                                 where s.Shop_ID == 30
                                 select new
                                 {
                                     s.Shift_Start_Time,
                                     s.Shift_End_Time
                                 }).FirstOrDefault();

                List<double?> ShopTarget = new List<double?>();
                if (fromTime != "")
                {
                    List<List<double?>> AllConsumption = new List<List<double?>>();

                    List<string> Names = new List<string>();
                    List<double?> Consumption = new List<double?>();
                    List<string> allTimes = new List<string>();
                    List<string> allDates = new List<string>();
                    double cummCountAvg = 0;
                    double cummCount = 0;
                    var isAllFeader = false;
                    string[] FeederArray = { };
                    if (temp == "null")
                    {


                    }
                    else
                    {
                        FeederArray = temp.Split(',').ToArray();
                    }


                    foreach (var item in FeederArray)
                    {
                        int feederId = temp == "" ? 0 : Convert.ToInt32(item);
                        int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                        DateTime date = DateTime.Now.Date;

                        var time = TimeSpan.Parse(fromTime);
                        var time2 = TimeSpan.Parse(toTime);
                        DateTime EndDate1 = DateTime.Now;
                        var enddate11 = Convert.ToDateTime(EndDate1).ToString("dd/MM/yyyy HH: mm:ss");

                        var time1 = Convert.ToDateTime(enddate11).ToString("HH:mm:ss");
                        date = date.AddDays(0);

                        var endtime1 = TimeSpan.Parse(time1.ToString());
                        int ShopID = Convert.ToInt32(Shop);


                        string starttime = time.ToString();
                        string endtime = time2.ToString();

                        DateTime startdate1 = DateTime.Parse(StartDate);
                        DateTime enddate1 = DateTime.Parse(EndDate).AddDays(0);

                        startdate1 = (startdate1 + time);
                        enddate1 = (enddate1 + time2);
                        List<double?> Final_Consumption = new List<double?>();


                     
                        if (buttonType == "1")
                        {
                            allTimes = new List<string>();
                            allDates = new List<string>();
                            Consumption = new List<double?>();
                            //List<double?> ShopTarget = new List<double?>();
                            List<Sp_LiveFeederwiseConsumption_Result> obj = null;
                            if (feederId == 0)
                            {
                                //List<SP_LiveShopwiseConsumption_Result> obj1 = db.SP_LiveShopwiseConsumption(plantID, startdate1, enddate1, ShopID).ToList();
                                //if (obj1[0].totalconsumption == null || obj1[0].totalconsumption == 0)
                                //{
                                //    Final_Consumption.Add(0);
                                //}
                                //else
                                //{
                                //    Final_Consumption.Add(obj1[0].totalconsumption);
                                //}
                                for (DateTime date1 = startdate1, date2 = startdate1.AddHours(1); date2 <= enddate1; date1 = date2, date2 = date2.AddHours(1))
                                {


                                    List<SP_LiveShopwiseConsumption_Result> obj1 = db.SP_LiveShopwiseConsumption(plantID, date1, date2, ShopID).ToList();


                                    if (obj1[0].totalconsumption == null || obj1[0].totalconsumption == 0)
                                    {
                                        Final_Consumption.Add(0);
                                    }
                                    else
                                    {
                                        Final_Consumption.Add(obj1[0].totalconsumption);
                                    }
                                    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                                    allDates.Add(ddl);

                                }
                            }
                            else
                            {
                                obj = db.Sp_LiveFeederwiseConsumption(ShopID, startdate1, enddate1, plantID, starttime, endtime, feederId).ToList();
                                //if (obj[0].Consumption == null || obj[0].Consumption == 0)
                                //{
                                //    Final_Consumption.Add(0);
                                //}
                                //else
                                //{
                                //    Final_Consumption.Add(obj[0].Consumption);
                                //}
                                foreach (var id in obj)
                                {
                                    Final_Consumption.Add(id.Consumption);
                                }

                            }
                            if (feederId != -1)
                            {
                                for (DateTime date1 = startdate1, date2 = startdate1.AddMinutes(6); date2 <= enddate1; date1 = date2, date2 = date2.AddMinutes(6))
                                {
                                    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                                    allTimes.Add(ddl);
                                    allDates.Add(ddl);

                                }
                            }
                            if (feederId == -1)
                            {
                                for (int k = 0; k < Final_Consumption.Count(); k++)
                                {
                                    double difference_first = 0.0;
                                    difference_first = Math.Round(Convert.ToDouble(Final_Consumption[k]), 2);
                                    Consumption.Add(difference_first);
                                    cummCount += difference_first;
                                }
                            }
                            else
                            {
                                for (int k = 0; k < Final_Consumption.Count(); k++)
                                {

                                    if (k == 1)
                                    {
                                        double difference_first = 0.0;
                                        double privous = Convert.ToDouble(Final_Consumption[k - 1]);
                                        double current = Convert.ToDouble(Final_Consumption[k]);
                                        if (current > privous)
                                        {
                                            if (privous != 0)
                                            {
                                                difference_first = Math.Round(Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]), 2);
                                                Consumption.Add(difference_first);
                                                cummCount += difference_first;
                                            }
                                            else
                                            {
                                                Consumption.Add(0);
                                                cummCount += difference_first;
                                            }

                                        }
                                        else
                                        {
                                            // difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                                            Consumption.Add(0);
                                            cummCount += difference_first;
                                        }

                                    }
                                    else if (k != 0)
                                    {
                                        double difference_first = 0.0;
                                        double privous = Convert.ToDouble(Final_Consumption[k - 1]);
                                        double current = Convert.ToDouble(Final_Consumption[k]);
                                        if (current > privous)
                                        {
                                            if (privous != 0)
                                            {
                                                difference_first = Math.Round(Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]), 2);
                                                Consumption.Add(difference_first);
                                                cummCount += difference_first;
                                            }
                                            else
                                            {
                                                Consumption.Add(0);
                                                cummCount += difference_first;
                                            }
                                        }
                                        else
                                        {
                                            // difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                                            Consumption.Add(0);
                                            cummCount += difference_first;
                                        }
                                    }
                                    else
                                    {
                                        Consumption.Add(0);
                                    }
                                }
                            }

                        }

                        if (buttonType == "2")
                        {


                            DateTime SStartDate = Convert.ToDateTime(StartDate);

                            TimeSpan ATime = TimeSpan.Parse(fromTime);
                            //TimeSpan BTime = TimeSpan.Parse("15:10:00.000");
                            TimeSpan CTime = TimeSpan.Parse(toTime);
                            DateTime shiftADate = SStartDate + ATime;
                            //DateTime shiftBDate = SStartDate + BTime;
                            DateTime shiftCDate = SStartDate + CTime;
                            var startshift = shiftADate.ToString();
                            var EndShift = shiftCDate.ToString();
                            //DateTime shiftEndDate = SEndDate + ATime;

                            var shiftdate = startshift + " To  " + EndShift;

                            var prodA = db.Sp_ProductionCount(ShopID, shiftADate, shiftCDate, plantID).ToList();
                            if (prodA[0].Production == 0)
                            {
                                Consumption.Add(0);
                                allTimes.Add("shiftdate");
                            }
                            else
                            {
                                double cons1 = Convert.ToDouble(prodA[0].Production);
                                cummCount += cons1;
                                Consumption.Add(cons1);

                                allDates.Add(shiftdate);
                            }



                        }
                        if (buttonType == "3")
                        {
                            DateTime SStartDate = Convert.ToDateTime(StartDate);
                            var livedate = DateTime.Now.ToString();
                            TimeSpan ATime = TimeSpan.Parse(fromTime);
                            TimeSpan BTime = TimeSpan.Parse("15:10:00.000");
                            TimeSpan CTime = TimeSpan.Parse(toTime);
                            DateTime shiftADate = SStartDate + ATime;
                            DateTime shiftBDate = SStartDate + BTime;
                            DateTime shiftCDate = SStartDate + CTime;
                            //DateTime shiftEndDate = SEndDate + ATime;
                            //if (EndDate1 <= shiftBDate)
                            //{
                            //    shiftBDate = EndDate1;
                            //}
                            var consA = db.SP_LiveShopwiseConsumption(plantID, shiftADate, shiftCDate, ShopID).ToList();
                            var prodA = db.Sp_ProductionCount(ShopID, shiftADate, shiftCDate, plantID).ToList();
                            double cons1 = Math.Round(Convert.ToDouble(consA[0].totalconsumption / prodA[0].Production), 2);
                            cummCount += cons1;
                            Consumption.Add(cons1);
                            var startshift = shiftADate.ToString();
                            var EndShift = shiftCDate.ToString();
                            //DateTime shiftEndDate = SEndDate + ATime;

                            var shiftdate = startshift + " To  " + EndShift;
                            allDates.Add(shiftdate);



                        }

                        DateTime End = System.DateTime.Now;
                        DateTime Start = End.Date;
                        //var time = TimeSpan.Parse("06:30:00.000");
                        Start = (Start + time);


                        //for (DateTime date1 = Convert.ToDateTime(startdate1); date1 <= Convert.ToDateTime(EndDate1); date1 = date1.AddMinutes(5))
                        //{
                        //    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                        //    allTimes.Add(ddl);
                        //}

                        int tagindex = Convert.ToInt32(feederId);
                        var feeder = (from f in db.UtilityMainFeederMappings
                                      where f.TagIndex == tagindex
                                      select new
                                      {
                                          f.FeederName
                                      }).FirstOrDefault();
                        // var FeederName = obj[0].FeederName;
                        var FeederName = feeder != null ? feeder.FeederName : "";

                        Names.Add(FeederName);
                        AllConsumption.Add(Consumption);
                    }

                    if (AllConsumption.Count() == 1 && AllConsumption[0].Count > 0)
                    {
                        //if(buttonType == "1")
                        //{
                        //    foreach (var cumm in AllConsumption[0])
                        //    {
                        //        //cummCount += (double)cumm;
                        //    }
                        //}

                        cummCountAvg = Math.Round((Convert.ToDouble(cummCount / AllConsumption[0].Count())), 2);

                    }
                    return Json(new { AllConsumption, Names, buttonType, cummCountAvg, isAllFeader, allDates }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    fromTime = fromTime != "" ? fromTime + ":00.000" : "";
                    toTime = toTime != "" ? toTime + ":00.000" : "";
                    TimeSpan fromTime1 = fromTime != "" ? TimeSpan.Parse(fromTime) : TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                    TimeSpan toTime1 = toTime != "" ? TimeSpan.Parse(toTime) : TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                    double cummCountAvg = 0;
                    double cummCount = 0;

                    var isAllFeader = false;
                    List<List<double?>> AllConsumption = new List<List<double?>>();
                    List<string> Names = new List<string>();
                    int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                    int ShopID = Convert.ToInt32(Shop);
                    string[] FeederArray = { };
                    if (temp == "null")
                    {

                    }
                    else
                    {
                        FeederArray = temp.Split(',').ToArray();
                    }

                    List<string> allDates = new List<string>();
                    List<string> allTimes = new List<string>();

                    foreach (var item in FeederArray)
                    {
                        allDates = new List<string>();
                        int feederId = item != "" ? Convert.ToInt32(item) : -1;
                        if (ddlformate == "2")
                        {
                            DateTime today = DateTime.Now.Date;
                            var startTime = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString()); ;
                            var EndTime = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString()); ;

                            DateTime from = DateTime.Parse(StartDate);

                            //  List<Sp_LiveFeederwiseConsumption_Result> obj1 =
                            //db.Sp_LiveFeederwiseConsumption(ShopID, from, today, plantID, startTime.ToString(), EndTime.ToString(), feederId).ToList();

                            List<double?> Consumption = new List<double?>();
                            List<double?> Final_Consumption = new List<double?>();



                            DateTime End = System.DateTime.Now;
                            DateTime Start = End.Date;
                            //var time = TimeSpan.Parse("06:30:00.000");
                            //Start = (Start + time);

                            today = (today + EndTime);
                            DateTime startDate1 = DateTime.Parse(StartDate);
                            startDate1 = (startDate1 + startTime);
                            if (buttonType == "1")
                            {

                                allDates = new List<string>();
                                Consumption = new List<double?>();
                                if (feederId == -1)
                                {
                                    //List<SP_LiveShopwiseConsumption_Result> obj = db.SP_LiveShopwiseConsumption(plantID, startDate1, today, ShopID).ToList();
                                    //if (obj[0].totalconsumption == null || obj[0].totalconsumption == 0)
                                    //{
                                    //    Final_Consumption.Add(0);
                                    //}
                                    //else
                                    //{
                                    //    Final_Consumption.Add(obj[0].totalconsumption);
                                    //}
                                    //foreach (var id in obj)
                                    //{
                                    //    Final_Consumption.Add(id.totalconsumption);
                                    //}
                                    allTimes = new List<string>();
                                    //for (DateTime date1 = Convert.ToDateTime(startDate1), date2 = Convert.ToDateTime(startDate1); date2 <= Convert.ToDateTime(today); date1 = date2)
                                    //{
                                    //    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                                    //    allDates.Add(ddl);
                                    //    //allDates.Add(ddl);
                                    //    date2 = date2.AddHours(1);
                                    //    List<SP_LiveShopwiseConsumption_Result> obj = db.SP_LiveShopwiseConsumption(plantID, date1, date2, ShopID).ToList();


                                    //    if (obj[0].totalconsumption == null || obj[0].totalconsumption == 0)
                                    //    {
                                    //        Final_Consumption.Add(0);
                                    //    }
                                    //    else
                                    //    {
                                    //        Final_Consumption.Add(obj[0].totalconsumption);
                                    //    }


                                    //}

                                    for (DateTime date1 = startDate1, date2 = startDate1.AddHours(1); date2 <= today; date1 = date2, date2 = date2.AddHours(1))
                                    {


                                        List<SP_LiveShopwiseConsumption_Result> obj = db.SP_LiveShopwiseConsumption(plantID, date1, date2, ShopID).ToList();


                                        if (obj[0].totalconsumption == null || obj[0].totalconsumption == 0)
                                        {
                                            Final_Consumption.Add(0);
                                        }
                                        else
                                        {
                                            Final_Consumption.Add(obj[0].totalconsumption);
                                        }
                                        string ddl = Convert.ToDateTime(date2).ToString("HH:mm");
                                        allDates.Add(ddl);

                                    }

                                }
                                else
                                {
                                    List<Sp_LiveFeederwiseConsumption_Result> obj = db.Sp_LiveFeederwiseConsumption(ShopID, startDate1, today, plantID, "", "", feederId).ToList();

                                    //if (obj[0].Consumption == null || obj[0].Consumption == 0)
                                    //{
                                    //    Final_Consumption.Add(0);
                                    //}
                                    //else
                                    //{
                                    //    Final_Consumption.Add(obj[0].Consumption);
                                    //}

                                    if (FeederArray.Count() == 1)
                                    {
                                        allDates = new List<string>();
                                        foreach (var id in obj)
                                        {
                                            Final_Consumption.Add(id.Consumption);

                                            string ddl = Convert.ToDateTime(id.ConsumptionDate).ToString("HH:mm");
                                            allDates.Add(ddl);
                                        }
                                    }
                                    else
                                    {


                                        foreach (var id in obj)
                                        {
                                            Final_Consumption.Add(id.Consumption);
                                        }
                                    }

                                }

                                if (feederId != -1)
                                {
                                    //List<Sp_LiveFeederwiseConsumption_Result> obj = null;
                                    if (FeederArray.Count() == 1)
                                    {
                                    }
                                    else
                                    {
                                        for (DateTime date1 = startDate1, date2 = date1.AddMinutes(6); date2 <= today; date1 = date2, date2 = date2.AddMinutes(6))
                                        {
                                            string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                                            allDates.Add(ddl);

                                        }
                                    }

                                }
                                if (feederId == -1)
                                {
                                    for (int k = 0; k < Final_Consumption.Count(); k++)
                                    {
                                        double difference_first = 0.0;
                                        difference_first = Math.Round(Convert.ToDouble(Final_Consumption[k]), 2);
                                        Consumption.Add(difference_first);
                                        cummCount += difference_first;
                                    }
                                }
                                else
                                {


                                    for (int k = 0; k < Final_Consumption.Count(); k++)
                                    {

                                        if (k == 1)
                                        {
                                            double difference_first = 0.0;
                                            double privous = Convert.ToDouble(Final_Consumption[k - 1]);
                                            double current = Convert.ToDouble(Final_Consumption[k]);
                                            if (current > privous)
                                            {
                                                if (privous != 0)
                                                {
                                                    difference_first = Math.Round(Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]), 2);
                                                    Consumption.Add(difference_first);
                                                    cummCount += difference_first;
                                                }
                                                else
                                                {
                                                    Consumption.Add(0);
                                                    cummCount += difference_first;
                                                }

                                            }
                                            else
                                            {
                                                // difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                                                Consumption.Add(0);
                                                cummCount += difference_first;
                                            }

                                        }
                                        else if (k != 0)
                                        {
                                            double difference_first = 0.0;
                                            double privous = Convert.ToDouble(Final_Consumption[k - 1]);
                                            double current = Convert.ToDouble(Final_Consumption[k]);
                                            if (current > privous)
                                            {
                                                if (privous != 0)
                                                {
                                                    difference_first = Math.Round(Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]), 2);
                                                    Consumption.Add(difference_first);
                                                    cummCount += difference_first;
                                                }
                                                else
                                                {
                                                    Consumption.Add(0);
                                                    cummCount += difference_first;
                                                }
                                            }
                                            else
                                            {
                                                // difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                                                Consumption.Add(0);
                                                cummCount += difference_first;
                                            }
                                        }
                                        else
                                        {
                                            Consumption.Add(0);
                                        }
                                    }
                                }

                            }

                            if (buttonType == "2")
                            {

                                DateTime SStartDate = Convert.ToDateTime(StartDate);
                                DateTime SEndDate = Convert.ToDateTime(EndDate).AddDays(1);
                                DateTime shiftADate = DateTime.Today;
                                DateTime shiftBDate = DateTime.Today;
                                DateTime shiftCDate = DateTime.Today;
                                DateTime shiftEndDate = DateTime.Today;
                                var shiftTimeing = (from s in db.MM_MTTUW_Shift
                                                    where s.Shop_ID == 30
                                                    select new
                                                    {
                                                        s.Shift_Name,
                                                        s.Shift_ID,
                                                        s.Shift_Start_Time,
                                                        s.Shift_End_Time
                                                    }).ToList();
                                foreach (var id in shiftTimeing)
                                {
                                    if (id.Shift_ID == 10021)
                                    {
                                        TimeSpan ATime = id.Shift_Start_Time;
                                        shiftADate = SStartDate + ATime;
                                         shiftEndDate = SEndDate + ATime;
                                    }
                                    else if (id.Shift_ID == 10022)
                                    {

                                        TimeSpan BTime = id.Shift_Start_Time;
                                        shiftBDate = SStartDate + BTime;

                                        TimeSpan CTime = id.Shift_End_Time;
                                        shiftCDate = SStartDate + CTime;

                                    }
                                }
                                //TimeSpan ATime = TimeSpan.Parse("06:30:00.000");
                                //TimeSpan BTime = TimeSpan.Parse("15:10:00.000");
                                //TimeSpan CTime = TimeSpan.Parse("23:50:00.000");
                                //DateTime shiftADate = SStartDate + ATime;
                                //DateTime shiftBDate = SStartDate + BTime;
                                //DateTime shiftCDate = SStartDate + CTime;
                              

                                var prodA = db.Sp_ProductionCount(ShopID, shiftADate, shiftBDate, plantID).ToList();
                                if (prodA[0].Production == 0 || prodA[0].Production == null)
                                {
                                    Consumption.Add(0);
                                    allDates.Add("Shift-A");
                                }
                                else
                                {
                                    double cons1 = Convert.ToDouble(prodA[0].Production);
                                    Consumption.Add(cons1);
                                    cummCount += cons1;
                                    allDates.Add("Shift-A");
                                }

                                var prodB = db.Sp_ProductionCount(ShopID, shiftBDate, shiftCDate, plantID).ToList();
                                if (prodB[0].Production == 0 || prodB[0].Production == null)
                                {
                                    Consumption.Add(0);
                                    allDates.Add("Shift-B");
                                }
                                else
                                {
                                    double cons1 = Convert.ToDouble(prodB[0].Production);
                                    Consumption.Add(cons1);
                                    cummCount += cons1;
                                    allDates.Add("Shift-B");
                                }

                                var prodC = db.Sp_ProductionCount(ShopID, shiftCDate, shiftEndDate, plantID).ToList();
                                if (prodC[0].Production == 0 || prodC[0].Production == null)
                                {
                                    Consumption.Add(0);
                                    allDates.Add("Shift-C");
                                }
                                else
                                {
                                    double cons1 = Convert.ToDouble(prodC[0].Production);
                                    Consumption.Add(cons1);
                                    cummCount += cons1;
                                    allDates.Add("Shift-C");
                                }

                            }
                            if (buttonType == "3")
                            {
                                
                                DateTime SStartDate = Convert.ToDateTime(StartDate);
                                DateTime SEndDate = Convert.ToDateTime(EndDate).AddDays(1);
                                DateTime shiftADate = DateTime.Today;
                                DateTime shiftBDate = DateTime.Today;
                                DateTime shiftCDate = DateTime.Today;
                                DateTime shiftEndDate = DateTime.Today;
                                var shiftTimeing = (from s in db.MM_MTTUW_Shift
                                                    where s.Shop_ID == 30
                                                    select new
                                                    {
                                                        s.Shift_Name,
                                                        s.Shift_ID,
                                                        s.Shift_Start_Time,
                                                        s.Shift_End_Time
                                                    }).ToList();
                                foreach (var id in shiftTimeing)
                                {
                                    if (id.Shift_ID == 10021)
                                    {
                                        TimeSpan ATime = id.Shift_Start_Time;
                                        shiftADate = SStartDate + ATime;
                                        shiftEndDate = SEndDate + ATime;
                                    }
                                    else if (id.Shift_ID == 10022)
                                    {

                                        TimeSpan BTime = id.Shift_Start_Time;
                                        shiftBDate = SStartDate + BTime;

                                        TimeSpan CTime = id.Shift_End_Time;
                                        shiftCDate = SStartDate + CTime;

                                    }
                                }
                                //TimeSpan ATime = TimeSpan.Parse("06:30:00.000");
                                //TimeSpan BTime = TimeSpan.Parse("15:10:00.000");
                                //TimeSpan CTime = TimeSpan.Parse("23:50:00.000");
                                //DateTime shiftADate = SStartDate + ATime;
                                //DateTime shiftBDate = SStartDate + BTime;
                                //DateTime shiftCDate = SStartDate + CTime;
                                //DateTime shiftEndDate = SEndDate + ATime;

                                var consA = db.SP_ShopwiseConsumption_New(ShopID, shiftADate, shiftBDate, plantID).ToList();
                                var prodA = db.Sp_ProductionCount(ShopID, shiftADate, shiftBDate, plantID).ToList();

                                if (consA.Count() <= 0 || prodA.Count() <= 0)
                                {
                                    Consumption.Add(0);
                                    allDates.Add("Shift-A");
                                }
                                else
                                {
                                    if (consA[0].Comsumtionvalues == 0 || consA[0].Comsumtionvalues == null || prodA[0].Production == 0)
                                    {
                                        Consumption.Add(0);
                                        allDates.Add("Shift-A");
                                    }
                                    else
                                    {
                                        double cons1 = Math.Round(Convert.ToDouble(consA[0].Comsumtionvalues / prodA[0].Production), 2);
                                        cummCount += cons1;
                                        Consumption.Add(cons1);
                                        allDates.Add("Shift-A");
                                    }
                                }

                                var consB = db.SP_ShopwiseConsumption_New(ShopID, shiftBDate, shiftCDate, plantID).ToList();
                                var prodB = db.Sp_ProductionCount(ShopID, shiftBDate, shiftCDate, plantID).ToList();
                                if (consB.Count() == 0 || prodB.Count() == 0)
                                {
                                    Consumption.Add(0);
                                    allDates.Add("Shift-B");
                                }
                                else
                                {
                                    if (consB[0].Comsumtionvalues == 0 || consB[0].Comsumtionvalues == null || prodB[0].Production == 0)
                                    {
                                        Consumption.Add(0);
                                        allDates.Add("Shift-B");
                                    }
                                    else
                                    {
                                        double cons1 = Math.Round(Convert.ToDouble(consB[0].Comsumtionvalues / prodB[0].Production), 2);
                                        cummCount += cons1;
                                        Consumption.Add(cons1);
                                        allDates.Add("Shift-B");
                                    }
                                }

                                var consC = db.SP_ShopwiseConsumption_New(ShopID, shiftCDate, shiftEndDate, plantID).ToList();
                                var prodC = db.Sp_ProductionCount(ShopID, shiftCDate, shiftEndDate, plantID).ToList();
                                if (consC.Count() == 0 || prodC.Count() == 0)
                                {
                                    Consumption.Add(0);
                                    allDates.Add("Shift-C");
                                }
                                else
                                {


                                    if (consC.Count > 0)
                                    {


                                        if (consC[0].Comsumtionvalues == 0 || consC[0].Comsumtionvalues == null || prodC[0].Production == 0)
                                        {
                                            Consumption.Add(0);
                                            allDates.Add("Shift-C");
                                        }
                                        else
                                        {
                                            double cons1 = Math.Round(Convert.ToDouble(consC[0].Comsumtionvalues / prodC[0].Production), 2);
                                            cummCount += cons1;
                                            Consumption.Add(cons1);
                                            allDates.Add("Shift-C");
                                        }
                                    }
                                    else
                                    {
                                        Consumption.Add(0);
                                        allDates.Add("Shift-C");
                                    }
                                }
                            }

                           
                            int tagindex1 = Convert.ToInt32(feederId);
                            if (feederId == -1)
                            {
                                var shop = (from s in db.MM_MTTUW_Shops
                                            where s.Shop_ID == ShopID
                                            select new { s.Shop_Name }).FirstOrDefault();

                                AllConsumption.Add(Consumption);
                                Names.Add(shop.Shop_Name);
                            }
                            else
                            {
                                var feeder1 = (from f in db.UtilityMainFeederMappings
                                               where f.TagIndex == tagindex1
                                               select new
                                               {
                                                   f.FeederName
                                               }).FirstOrDefault();
                                // var FeederName = obj[0].FeederName;
                                var FeederName = feeder1 != null ? feeder1.FeederName : "";


                                AllConsumption.Add(Consumption);
                                Names.Add(FeederName);
                            }



                        }
                        else
                        {
                            DateTime fromdate = DateTime.Now;
                            DateTime toDate = DateTime.Now;
                            DateTime date = DateTime.Now.Date;
                            DateTime dtn = DateTime.Now.Date;
                            //var time = TimeSpan.Parse("06:30:00.000");
                            //var time1 = TimeSpan.Parse("06:40:00.000");
                            var start = TimeSpan.Parse(ShiftTime.Shift_Start_Time.ToString());
                            date = date.AddDays(-1);
                            string starttime = start.ToString();
                            string endtime = start.ToString();
                            DateTime startdate1 = DateTime.Parse(StartDate);
                            DateTime enddate1 = DateTime.Parse(EndDate);
                            if (ddlformate == "3" || ddlformate == "4" || ddlformate == "5")
                            {
                                enddate1 = enddate1.AddDays(1);
                                starttime = start.ToString();
                                endtime = start.ToString();
                            }
                            List<Sp_dailyFeederwiseConsumption_Result> obj = null;
                            List<Sp_dailyFeederwiseConsumption_New_Result> obj1 = null;
                            //List<Sp_dailyFeederwiseConsumption_New_Result> obj2 = null;
                            List<Sp_ShopwiseSummary_Result> obj3 = null;
                            List<double?> Consumption = new List<double?>();
                            if (ddlformate == "5" || ddlformate == "3" || ddlformate == "4")
                            {
                                if (buttonType == "1" && feederId == -1)
                                {
                                    DateTime todaydate = System.DateTime.Now;
                                    if(enddate1.Date >= todaydate.Date)
                                    {
                                        enddate1 = todaydate;
                                    }

                                    obj3 = db.Sp_ShopwiseSummary(ShopID, startdate1, enddate1, plantID, starttime, endtime).ToList();
                                }
                                else if (buttonType == "1" && feederId != -1)
                                {
                                    if (fromTime != "" && toTime != "")
                                    {
                                        DateTime startdate2 = DateTime.Parse(StartDate);
                                        DateTime enddate2 = DateTime.Parse(StartDate);
                                        var stime = TimeSpan.Parse(fromTime.ToString());
                                        var etime = TimeSpan.Parse(toTime.ToString());
                                        startdate2 = startdate2 + stime;
                                        enddate2 = enddate2 + etime;
                                        for (DateTime date1 = Convert.ToDateTime(startdate2), date2 = date1.AddMinutes(5); date2 <= enddate2; date1 = date2, date2 = date2.AddMinutes(5))
                                        {
                                            var cons = db.SP_FeederwiseConsumption_New(ShopID, date1, date2, plantID, feederId).ToList();
                                            string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                                            allDates.Add(ddl);
                                            if (cons.Count == 0)
                                                Consumption.Add(0);
                                            else
                                            {
                                                cummCount += Convert.ToDouble(cons[0].Consumption);
                                                Consumption.Add(Math.Round(Convert.ToDouble(cons[0].Consumption), 2));
                                            }

                                        }
                                    }
                                    else
                                    {
                                        
                                        obj1 = db.Sp_dailyFeederwiseConsumption_New(ShopID, startdate1, enddate1, plantID, " ", "", feederId).ToList();
                                    }
                                }
                            }
                            else
                            {

                                obj = db.Sp_dailyFeederwiseConsumption(ShopID, startdate1, enddate1, plantID, starttime, endtime, feederId).ToList();


                            }
                            if (buttonType == "2" || buttonType == "3")

                            {
                                if (ddlformate == "5" && fromTime != "" && toTime != "")
                                {
                                    DateTime custFDate = DateTime.Parse(StartDate);
                                    DateTime custTDate = DateTime.Parse(EndDate);
                                    custFDate = (custFDate + fromTime1);
                                    custTDate = (custTDate + toTime1);
                                    for (DateTime date1 = Convert.ToDateTime(custFDate), date2 = Convert.ToDateTime(custFDate).AddMinutes(10); date2 <= Convert.ToDateTime(custTDate); date1 = date1.AddMinutes(10), date2 = date2.AddMinutes(10))
                                    {

                                        string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                                        allDates.Add(ddl);
                                        if (buttonType == "2")
                                        {
                                            var prod = db.sp_getdailyProductionCount(date1, date2, plantID).Where(m => m.ShopId == ShopID).FirstOrDefault();
                                            if (prod.totalproduction == 0 || prod.totalproduction == null)
                                                Consumption.Add(0);
                                            else
                                                Consumption.Add(prod.totalproduction);
                                        }
                                        if (buttonType == "3")
                                        {

                                            var cons = db.Sp_AllShopConsumption(date1, date2, plantID).Where(m => m.Shop_ID == ShopID).FirstOrDefault();
                                            var prod = db.sp_getdailyProductionCount(date1, date2, plantID).Where(m => m.ShopId == ShopID).FirstOrDefault();
                                            double cons1 = 0;
                                            if (cons.totalconsumption == null || cons.totalconsumption == 0 || prod.totalproduction == 0)
                                            {
                                                cummCount += 0;
                                                Consumption.Add(0);
                                            }

                                            else
                                            {
                                                cons1 = (Convert.ToDouble(cons.totalconsumption / prod.totalproduction));
                                                cummCount += cons1;
                                                Consumption.Add(System.Math.Round(cons1, 2));
                                            }


                                        }

                                    }
                                }
                                else
                                {
                                        if (buttonType == "2")
                                        {
                                        var Perfrom = (from p in db.MM_Performance_Indices_Energy
                                                       where p.DateandTime >= startdate1 && p.DateandTime <= enddate1
                                                        &&
                                                       p.Shop_ID == ShopID
                                                       select new
                                                       {
                                                           p.DateandTime,
                                                           p.Production
                                                       }).ToList();
                                        foreach (var id in Perfrom)
                                        {
                                            Consumption.Add(id.Production);
                                            string ddl = Convert.ToDateTime(id.DateandTime).ToString("dd/MMM");
                                            allDates.Add(ddl);
                                            
                                        }
                                    }
                                        if (buttonType == "3")
                                        {
                                            int CurrentYear = DateTime.Today.Year;
                                            int PreviousYear = DateTime.Today.Year - 1;
                                            int NextYear = DateTime.Today.Year + 1;
                                            string PreYear = PreviousYear.ToString();
                                            string NexYear = NextYear.ToString();
                                            string CurYear = CurrentYear.ToString();
                                            string FinYear = null;

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
                                            var Target1 = (from t in db.MM_PowerTarget
                                                           where t.Plant_ID == plantID && t.Shop_ID == ShopID &&
                                                            t.Year == FinYear && t.Month == month
                                                           select new
                                                           {
                                                               t.Target
                                                           }
                                                           ).FirstOrDefault();
                                        var Perfrom = (from p in db.MM_Performance_Indices_Energy
                                                       where p.DateandTime >= startdate1 && p.DateandTime <= enddate1 &&
                                                       p.Shop_ID  == ShopID
                                                       select new
                                                       {
                                                           p.DateandTime,
                                                           p.Consumption
                                                       }).ToList();
                                        foreach(var id in Perfrom)
                                        {
                                            Consumption.Add(id.Consumption);
                                            string ddl = Convert.ToDateTime(id.DateandTime).ToString("dd/MMM");
                                            allDates.Add(ddl);
                                            if (Target1 == null)
                                            {

                                                    ShopTarget.Add(0);
                                            }
                                            else
                                             {
                                                //ShopTarget.Add(0);
                                                 ShopTarget.Add(System.Math.Round(Convert.ToDouble(Target1.Target), 2));
                                             }
                                    }
                                       
                                    }

                                  
                                }
                            }

                            startdate1 = (startdate1 + start);
                            enddate1 = (enddate1 + start);
                            if (buttonType == "1")
                            {
                                if (fromTime != "" && toTime != "")
                                {
                                    //for (TimeSpan ctime1 = TimeSpan.Parse(starttime), ctime2 = TimeSpan.Parse(starttime).Add(TimeSpan.Parse("00:05:00")); ctime2 <= TimeSpan.Parse(endtime); ctime1 = ctime2, ctime2 = ctime2.Add(TimeSpan.Parse("00:05:00")))
                                    //{
                                    //    string ddl = ctime1.Hours + ":" + ctime1.Minutes;
                                    //    allDates.Add(ddl);
                                    //}
                                }
                                else
                                {
                                    if (feederId!= -1)
                                    {
                                        for (DateTime date1 = Convert.ToDateTime(StartDate); date1 <= Convert.ToDateTime(EndDate); date1 = date1.AddDays(1))
                                        {
                                            string ddl = Convert.ToDateTime(date1).ToString("dd/MMM");
                                            allDates.Add(ddl);
                                        }
                                    }
                                    else
                                    {
                                        foreach(var id in obj3)
                                        {
                                            string ddl = Convert.ToDateTime(id.ConsumptionDate).ToString("dd/MMM");
                                            allDates.Add(ddl);
                                        }
                                    }
                                   
                                }

                            }

                            int tagindex = Convert.ToInt32(feederId);
                            var feeder = (from f in db.UtilityMainFeederMappings
                                          where f.TagIndex == tagindex
                                          select new
                                          {
                                              f.FeederName
                                          }).FirstOrDefault();
                            int j = 0;
                            var FeederName = feeder != null ? feeder.FeederName : "All Feeder";
                            if (ddlformate == "3" || ddlformate == "4" || ddlformate == "5")
                            {
                                if (buttonType == "1" && feederId != -1)
                                {
                                    if (fromTime != "" && endtime != "")
                                    {

                                    }

                                    {
                                        //for (int i = 0; i < allDates.Count(); i++)
                                        //{

                                            for (j = 0; j < obj1.Count(); j++)
                                            {
                                                //if (allDates[i] == Convert.ToDateTime(obj1[j].ConsumptionDate).ToString("dd/MMM"))
                                                //{
                                                  var cons1 = obj1[j].Consumption;
                                                    if (cons1 == null)
                                                        cons1 = 0;
                                                    cummCount += Convert.ToDouble(cons1);
                                                    Consumption.Add(System.Math.Round((Convert.ToDouble(obj1[j].Consumption)), 2));
                                                    //break;
                                                //}


                                            }
                                            //if (j == obj1.Count())
                                            //{
                                            //    Consumption.Add(0);
                                            //}

                                        //}
                                    }

                                }
                                else if (buttonType == "1" && feederId == -1)
                                {
                                    var FeederCount = db.UtilityMainFeederMappings.Where(m => m.Shop_ID == ShopID

                                        ).ToList().Count;



                                    for (int i = 0; i < allDates.Count(); i++)
                                    {
                                        double TotalConsumption = 0;
                                        for (j = 0; j < obj3.Count(); j++)
                                        {


                                            if (allDates[i] == Convert.ToDateTime(obj3[j].ConsumptionDate).ToString("dd/MMM"))
                                            {
                                                var cons1 = obj3[j].totalconsumption;
                                                if (cons1 == null)
                                                    cons1 = 0;
                                                cummCount += Convert.ToDouble(cons1);
                                                TotalConsumption = Convert.ToDouble(obj3[j].totalconsumption);
                                                Consumption.Add(TotalConsumption);
                                            }


                                        }
                                        //Consumption.Add(TotalConsumption);
                                        //if (j == obj2.Count())
                                        //{
                                        //    Consumption.Add(0);
                                        //}

                                    }
                                }

                            }
                            else
                            {
                                for (int i = 0; i < allDates.Count(); i++)
                                {
                                    for (j = 0; j < obj.Count(); j++)
                                    {
                                        if (allDates[i] == Convert.ToDateTime(obj[j].ConsumptionDate).ToString("dd/MMM"))
                                        {

                                            Consumption.Add(System.Math.Round((Convert.ToDouble(obj[j].Consumption)), 2));
                                            break;
                                        }


                                    }
                                    if (j == obj.Count())
                                    {
                                        Consumption.Add(0);
                                    }
                                    else
                                    {

                                    }


                                }

                            }
                            AllConsumption.Add(Consumption);
                            if (buttonType == "3")
                            {
                                AllConsumption.Add(ShopTarget);
                                Names.Add("Actual ");
                                Names.Add("Target");
                            }
                            else
                            {
                                Names.Add(FeederName);
                            }



                        }

                    }

                    if (AllConsumption.Count() == 1)
                    {
                        if(cummCount ==0)
                        {
                            cummCountAvg = 0;
                        }
                        else
                        {
                            cummCountAvg = Math.Round((Convert.ToDouble(cummCount / AllConsumption[0].Count())), 2);
                        }

                    }
                    return Json(new { AllConsumption, Names, buttonType, cummCountAvg, isAllFeader, allDates }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("PlantList", "user");
            }

        }


        public ActionResult Livehowchart(string StartDate, string EndDate, string Shop, string Shift, string temp, string buttonType)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int ShopID = Convert.ToInt32(Shop);
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
                               where t.Plant_ID == plantID && t.Shop_ID == ShopID &&
                                t.Year == FinYear && t.Month == month
                               select new
                               {
                                   t.Target
                               }
                               ).FirstOrDefault();

                List<List<double?>> AllConsumption = new List<List<double?>>();
                List<string> Names = new List<string>();
                List<double?> Total_Consumption = new List<double?>();
                List<double?> Consumption = new List<double?>();
                List<string> allTimes = new List<string>();
                List<double?> Target = new List<double?>();
                double cummCountAvg = 0;
                double cummCount = 0;
                string[] FeederArray = { };

              
                //FeederArray = temp.Split(',').ToArray();
                if (temp == "null")
                {


                }
                else
                {
                    FeederArray = temp.Split(',').ToArray();
                }
                foreach (var item in FeederArray)
                {
                    int feederId = item != "" ? Convert.ToInt32(item) : -1;
                    // int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                    DateTime date = DateTime.Now.Date;
                    var ShiftTime = (from s in db.MM_MTTUW_Shift
                                     where s.Shop_ID == 30
                                     select new
                                     {
                                         s.Shift_Start_Time,
                                         s.Shift_End_Time
                                     }).FirstOrDefault();

                    var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                    //var time = TimeSpan.Parse("06:30:00.000");
                    //var time2 = TimeSpan.Parse("06:30:00.000");
                    DateTime EndDate1 = DateTime.Now;
                    var enddate11 = Convert.ToDateTime(EndDate1).ToString("dd/MM/yyyy HH: mm:ss");

                    var time1 = Convert.ToDateTime(enddate11).ToString("HH:mm:ss");
                    date = date.AddDays(-1);

                    var endtime1 = TimeSpan.Parse(time1.ToString());



                    string starttime = start.ToString();
                    string endtime = start.ToString();

                    DateTime startdate1 = DateTime.Parse(StartDate);
                    DateTime enddate1 = DateTime.Parse(EndDate).AddDays(1);
                    startdate1 = (startdate1 + start);
                    enddate1 = (enddate1 + start);
                    List<double?> Final_Consumption = new List<double?>();


                   
                    if (buttonType == "1")
                    {
                        allTimes = new List<string>();
                        Consumption = new List<double?>();
                        Total_Consumption = new List<double?>();
                        List<Sp_LiveFeederwiseConsumption_Result> obj = null;
                        List<SP_LiveShopwiseConsumption_Result> obj1 = null;
                        for (DateTime date1 = startdate1, date2 = startdate1.AddMinutes(6); date2 <= EndDate1; date1 = date2, date2 = date2.AddMinutes(6))
                        {
                            //if (feederId == -1)
                            //{
                            //    obj1 = db.SP_LiveShopwiseConsumption(plantID, date1, date2, ShopID).ToList();

                            //    if (obj1[0].totalconsumption == null || obj1[0].totalconsumption == 0)
                            //    {
                            //        Final_Consumption.Add(0);
                            //    }
                            //    else
                            //    {
                            //        Final_Consumption.Add(obj1[0].totalconsumption);
                            //    }
                            //}
                            //else
                            //{
                            //   obj = db.Sp_LiveFeederwiseConsumption(ShopID, date1, date2, plantID, starttime, endtime, feederId).ToList();
                            //    if (obj.Count > 0)
                            //    {
                            //        if (obj[0].Consumption == null || obj[0].Consumption == 0)
                            //        {
                            //            Final_Consumption.Add(0);
                            //        }
                            //        else
                            //        {
                            //            Final_Consumption.Add(obj[0].Consumption);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        Final_Consumption.Add(0);
                            //    }
                            //}
                            string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                            allTimes.Add(ddl);
                            //obj = db.Sp_LiveFeederwiseConsumption(ShopID, date1, date2, plantID, starttime, endtime, feederId).ToList();
                            //if(obj[0].Consumption == null || obj[0].Consumption == 0)
                            //{
                            //    Final_Consumption.Add(0);
                            //}
                            //else
                            //{
                            //    Final_Consumption.Add(obj[0].Consumption);
                            //}
                        }
                        if (feederId == -1)
                        {


                            allTimes = new List<string>();


                            for (DateTime date1 = startdate1, date2 = startdate1.AddHours(1); date2 <= EndDate1; date1 = date2, date2 = date2.AddHours(1))
                            {


                                obj1 = db.SP_LiveShopwiseConsumption(plantID, date1, date2, ShopID).ToList();


                                if (obj1[0].totalconsumption == null || obj1[0].totalconsumption == 0)
                                {
                                    Final_Consumption.Add(0);
                                }
                                else
                                {
                                    Final_Consumption.Add(obj1[0].totalconsumption);
                                }
                                string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                                allTimes.Add(ddl);

                            }


                            //if (obj1[0].totalconsumption == null || obj1[0].totalconsumption == 0)
                            //{
                            //    Final_Consumption.Add(0);
                            //}
                            //else
                            //{
                            //    Final_Consumption.Add(obj1[0].totalconsumption);
                            //}
                            //foreach (var id in obj1)
                            //{
                            //    Final_Consumption.Add(id.totalconsumption);
                            //}
                        }
                        else
                        {
                            //obj = db.Sp_LiveFeederwiseConsumption(ShopID, startdate1, EndDate1, plantID, starttime, endtime, feederId).ToList();
                            obj = db.Sp_LiveFeederwiseConsumption(ShopID, startdate1, EndDate1, plantID, "", "", feederId).ToList();
                            //if (obj.Count > 0)
                            //{
                            //    if (obj[0].Consumption == null || obj[0].Consumption == 0)
                            //    {
                            //        Final_Consumption.Add(0);
                            //    }
                            //    else
                            //    {
                            //        Final_Consumption.Add(obj[0].Consumption);
                            //    }
                            //}
                            //else
                            //{
                            //    Final_Consumption.Add(0);
                            //}
                            if (FeederArray.Count() == 1)
                            {
                                allTimes = new List<string>();
                                foreach (var id in obj)
                                {
                                    Final_Consumption.Add(id.Consumption);

                                    string ddl = Convert.ToDateTime(id.ConsumptionDate).ToString("HH:mm");
                                    allTimes.Add(ddl);
                                }
                            }
                            else
                            {
                                foreach (var id in obj)
                                {
                                    Final_Consumption.Add(id.Consumption);


                                }
                            }

                        }
                        //string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                        //allTimes.Add(ddl);
                        //obj = db.Sp_LiveFeederwiseConsumption(ShopID, date1, date2, plantID, starttime, endtime, feederId).ToList();
                        //if(obj[0].Consumption == null || obj[0].Consumption == 0)
                        //{
                        //    Final_Consumption.Add(0);
                        //}
                        //else
                        //{
                        //    Final_Consumption.Add(obj[0].Consumption);
                        //}
                        if (feederId == -1)
                        {
                            for (int k = 0; k < Final_Consumption.Count(); k++)
                            {
                                double difference_first = 0.0;
                                difference_first = Convert.ToDouble(Final_Consumption[k]);
                                Consumption.Add(difference_first);
                                cummCount += difference_first;
                            }
                        }
                        else
                        {

                            double privous_Total = 0.0;
                            double Total = 0.0;
                            for (int k = 0; k < Final_Consumption.Count(); k++)
                            {

                                if (k == 1)
                                {
                                    double difference_first = 0.0;


                                    double privous = Convert.ToDouble(Final_Consumption[k - 1]);
                                    double current = Convert.ToDouble(Final_Consumption[k]);
                                    if (current > privous)
                                    {
                                        if (privous != 0)
                                        {
                                            difference_first = Math.Round(Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]), 2);
                                            Total = Math.Round(Convert.ToDouble(Final_Consumption[k] - privous_Total), 2);
                                            Consumption.Add(difference_first);
                                            Total_Consumption.Add(Total);
                                            cummCount += difference_first;
                                        }
                                        else
                                        {
                                            Total_Consumption.Add(Total);
                                            Consumption.Add(0);
                                            cummCount += difference_first;
                                        }

                                    }
                                    else
                                    {
                                        // difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                                        Consumption.Add(0);
                                        Total_Consumption.Add(Total);
                                        cummCount += difference_first;
                                    }

                                }
                                else if (k != 0)
                                {
                                    double difference_first = 0.0;
                                    double privous = 0.0;
                                    double current = 0.0;
                                    privous = Convert.ToDouble(Final_Consumption[k - 1]);
                                    current = Convert.ToDouble(Final_Consumption[k]);


                                    if (current > privous)
                                    {
                                        if (privous != 0)
                                        {
                                            difference_first = Math.Round(Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]), 2);
                                            Total = Math.Round(Convert.ToDouble(Final_Consumption[k] - privous_Total), 2);
                                            Consumption.Add(difference_first);
                                            Total_Consumption.Add(Total);
                                            cummCount += difference_first;
                                        }
                                        else
                                        {
                                            Total_Consumption.Add(Total);
                                            Consumption.Add(0);
                                            cummCount += difference_first;
                                        }
                                    }
                                    else
                                    {
                                        // difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                                        Consumption.Add(0);
                                        Total_Consumption.Add(Total);
                                        cummCount += difference_first;
                                    }
                                }
                                else
                                {

                                    privous_Total = Convert.ToDouble(Final_Consumption[k]);

                                    Consumption.Add(0);
                                    Total_Consumption.Add(0);
                                }
                            }

                            //for (int k = 0; k < Final_Consumption.Count(); k++)
                            //{

                            //    if (k == 1)
                            //    {
                            //        double difference_first = 0.0;
                            //        difference_first = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                            //        Consumption.Add(difference_first);
                            //        cummCount += difference_first;
                            //    }
                            //    else if (k != 0)
                            //    {
                            //        double difference = 0.0;
                            //        difference = Convert.ToDouble(Final_Consumption[k] - Final_Consumption[k - 1]);
                            //        Consumption.Add(difference);
                            //        cummCount += difference;
                            //    }
                            //    else
                            //    {
                            //        Consumption.Add(0);
                            //    }
                            //}
                        }

                        //for (j = 0; j < obj.Count(); j++)
                        //{

                        //    Final_Consumption.Add(System.Math.Round((Convert.ToDouble(obj[j].Consumption)), 0));


                        //}
                    }

                    if (buttonType == "2")
                    {
                        //TimeSpan vtime = TimeSpan.Parse("06:40:00.000");
                        //DateTime vStartTime = DateTime.Parse(StartDate);
                        //vStartTime = vStartTime + vtime;
                        //DateTime vEndTime = DateTime.Parse(EndDate).AddDays(1);
                        //vEndTime = vEndTime + vtime;
                        DateTime SStartDate = Convert.ToDateTime(StartDate);
                        //TimeSpan ATime = TimeSpan.Parse("06:30:00.000");
                        //TimeSpan BTime = TimeSpan.Parse("15:10:00.000");
                        //TimeSpan CTime = TimeSpan.Parse("23:50:00.000");
                        DateTime shiftADate = DateTime.Today;
                        DateTime shiftBDate = DateTime.Today;
                        DateTime shiftCDate = DateTime.Today;
                        var shiftTimeing = (from s in db.MM_MTTUW_Shift
                                            where s.Shop_ID ==30
                                         select new
                                         {  s.Shift_ID,
                                             s.Shift_Start_Time,
                                             s.Shift_End_Time
                                         }).ToList();
                        foreach(var  id in shiftTimeing)
                        {
                            if(id.Shift_ID == 1)
                            {
                                TimeSpan ATime = id.Shift_Start_Time;
                                shiftADate = SStartDate + ATime; 
                            }
                            else if(id.Shift_ID == 2)
                            {
                           
                                TimeSpan BTime = id.Shift_Start_Time;
                                shiftBDate = SStartDate + BTime;

                              TimeSpan CTime = id.Shift_End_Time;
                                shiftCDate = SStartDate + CTime;

                            }
                        }
                       
                        //DateTime shiftEndDate = SEndDate + ATime;
                        if (EndDate1 <= shiftBDate)
                        {
                            shiftBDate = EndDate1;
                        }
                        if (shiftBDate <= EndDate1)
                        {
                            //var consA = db.SP_ShopwiseConsumption_New(ShopID, shiftADate, shiftBDate, plantID).ToList();
                            var prodA = db.Sp_ProductionCount(ShopID, shiftADate, shiftBDate, plantID).ToList();
                            if (prodA[0].Production == 0)
                            {
                                Consumption.Add(0);
                                allTimes.Add("Shift-A");
                            }
                            else
                            {
                                double cons1 = Convert.ToDouble(prodA[0].Production);
                                cummCount += cons1;
                                Consumption.Add(cons1);
                                allTimes.Add("Shift-A");
                            }
                        }



                        if (EndDate1 <= shiftCDate)
                        {
                            shiftCDate = EndDate1;
                        }
                        if (shiftCDate <= EndDate1)
                        {
                            //var consB = db.SP_ShopwiseConsumption_New(ShopID, shiftBDate, shiftCDate, plantID).ToList();
                            var prodB = db.Sp_ProductionCount(ShopID, shiftBDate, shiftCDate, plantID).ToList();
                            if (prodB[0].Production == 0)
                            {
                                Consumption.Add(0);
                                allTimes.Add("Shift-B");
                            }
                            else
                            {
                                double cons1 = Convert.ToDouble(prodB[0].Production);
                                cummCount += cons1;
                                Consumption.Add(cons1);
                                allTimes.Add("Shift-B");
                            }
                        }








                        //for (DateTime date1 = Convert.ToDateTime(startdate1), date2 = Convert.ToDateTime(startdate1).AddMinutes(5); date2 <= Convert.ToDateTime(EndDate1); date1 = date1.AddMinutes(5), date2 = date2.AddMinutes(5))
                        //{
                        //    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                        //    var prod = db.Sp_ProductionCount(ShopID, date1, date2, plantID).ToList();
                        //    if (prod[0].Production == 0 || prod[0].Production == null)
                        //    {
                        //        continue;
                        //    }  
                        //    else
                        //    {
                        //        allTimes.Add(ddl);
                        //        Consumption.Add(prod[0].Production);
                        //    }
                        //}
                    }
                    if (buttonType == "3")
                    {
                        DateTime SStartDate = Convert.ToDateTime(StartDate);
                        var livedate = DateTime.Now.ToString();

                        //TimeSpan ATime = TimeSpan.Parse("06:30:00.000");
                        //TimeSpan BTime = TimeSpan.Parse("15:10:00.000");
                        //TimeSpan CTime = TimeSpan.Parse("23:50:00.000");
                        //DateTime shiftADate = SStartDate + ATime;
                        //DateTime shiftBDate = SStartDate + BTime;
                        //DateTime shiftCDate = SStartDate + CTime;
                        //DateTime shiftEndDate = SEndDate + ATime;
                        DateTime shiftADate = DateTime.Today;
                        DateTime shiftBDate = DateTime.Today;
                        DateTime shiftCDate = DateTime.Today;
                        var shiftTimeing = (from s in db.MM_MTTUW_Shift
                                            where s.Shop_ID ==30
                                            select new
                                            {
                                                s.Shift_ID,
                                                s.Shift_Start_Time,
                                                s.Shift_End_Time
                                            }).ToList();
                        foreach (var id in shiftTimeing)
                        {
                            if (id.Shift_ID == 1)
                            {
                                TimeSpan ATime = id.Shift_Start_Time;
                                shiftADate = SStartDate + ATime;
                            }
                            else if (id.Shift_ID == 2)
                            {

                                TimeSpan BTime = id.Shift_Start_Time;
                                shiftBDate = SStartDate + BTime;

                                TimeSpan CTime = id.Shift_End_Time;
                                shiftCDate = SStartDate + CTime;

                            }
                        }
                        if (EndDate1 <= shiftBDate)
                        {
                            shiftBDate = EndDate1;
                        }
                        var consA = db.SP_LiveShopwiseConsumption(plantID, shiftADate, shiftCDate, ShopID).FirstOrDefault();
                        var prodA = db.Sp_ProductionCount(ShopID, shiftADate, shiftCDate, plantID).FirstOrDefault();
                        if (consA == null || consA.totalconsumption == 0 || prodA.Production == 0)
                        {
                            double cons1 = 0;
                            cummCount += cons1;
                            Consumption.Add(cons1);
                        }
                        else
                        {          
                                double cons1 = Math.Round(Convert.ToDouble(consA.totalconsumption / prodA.Production), 2);
                                cummCount += cons1;
                                Consumption.Add(cons1);
                        }
                     
                      


                        allTimes.Add(livedate);
                        //if (shiftBDate <= EndDate1)
                        //{
                        //    var consA = db.SP_ShopwiseConsumption_New(ShopID, shiftADate, shiftBDate, plantID).ToList();
                        //    var prodA = db.Sp_ProductionCount(ShopID, shiftADate, shiftBDate, plantID).ToList();
                        //    if (consA[0].Comsumtionvalues == 0 || consA[0].Comsumtionvalues == null || prodA[0].Production == 0)
                        //    {
                        //        Consumption.Add(0);
                        //        allTimes.Add("Shift-A");
                        //    }
                        //    else
                        //    {
                        //        double cons1 = Math.Round(Convert.ToDouble(consA[0].Comsumtionvalues / prodA[0].Production), 2);
                        //        cummCount += cons1;
                        //        Consumption.Add(cons1);
                        //        allTimes.Add("Shift-A");
                        //    }
                        //}



                        //if(EndDate1 <= shiftCDate)
                        //{
                        //    shiftCDate = EndDate1;
                        //}
                        //if(shiftCDate <= EndDate1)
                        //{
                        //    var consB = db.SP_ShopwiseConsumption_New(ShopID, shiftBDate, shiftCDate, plantID).ToList();
                        //    var prodB = db.Sp_ProductionCount(ShopID, shiftBDate, shiftCDate, plantID).ToList();
                        //    if (consB[0].Comsumtionvalues == 0 || consB[0].Comsumtionvalues == null || prodB[0].Production == 0)
                        //    {
                        //        Consumption.Add(0);
                        //        allTimes.Add("Shift-B");
                        //    }
                        //    else
                        //    {
                        //        double cons1 = Math.Round(Convert.ToDouble(consB[0].Comsumtionvalues / prodB[0].Production), 2);
                        //        cummCount += cons1;
                        //        Consumption.Add(cons1);
                        //        allTimes.Add("Shift-B");
                        //    }
                        //}


                        //for (DateTime date1 = Convert.ToDateTime(startdate1), date2 = Convert.ToDateTime(startdate1).AddMinutes(5); date2 <= Convert.ToDateTime(EndDate1); date1 = date1.AddMinutes(5), date2 = date2.AddMinutes(5))
                        //{
                        //    //List<SP_LiveShopwiseConsumption_Result> obj2 = db.SP_LiveShopwiseConsumption(plantID, fromdate, toDate, Shop_ID).ToList();
                        //    ////List<Sp_DailyShopwiseConsumption_Result> obj2 = db.Sp_DailyShopwiseConsumption(Shop_ID, fromdate, toDate, plantID).ToList();
                        //    //List<Sp_ProductionCount_Result> prod2 = db.Sp_ProductionCount(Shop_ID, fromdate, toDate, plantID).ToList();
                        //    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                        //    allTimes.Add(ddl);
                        //    var cons = db.SP_LiveShopwiseConsumption(plantID, date1, date2, ShopID).ToList();
                        //    var prod = db.Sp_ProductionCount(ShopID, date1, date2, plantID).ToList();
                        //    double cons1 = 0; 
                        //    if (cons[0].totalconsumption == null || cons[0].totalconsumption == 0 || prod[0].Production == 0 || prod[0].Production == null)
                        //    {
                        //        cummCount += 0;
                        //        Consumption.Add(0);
                        //    }

                        //    else
                        //    {
                        //        cons1 = (Convert.ToDouble(cons[0].totalconsumption / prod[0].Production));
                        //        cummCount += cons1;
                        //        Consumption.Add(System.Math.Round(cons1, 0));
                        //    }

                        //}

                    }

                    //DateTime Enddate = System.DateTime.Now;
                    //DateTime Startdate = Enddate.Date;
                    ////var time = TimeSpan.Parse("06:30:00.000");
                    //Startdate = (Startdate + start);


                    //for (DateTime date1 = Convert.ToDateTime(startdate1); date1 <= Convert.ToDateTime(EndDate1); date1 = date1.AddMinutes(5))
                    //{
                    //    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
                    //    allTimes.Add(ddl);
                    //}

                    int tagindex = Convert.ToInt32(feederId);
                    var feeder = (from f in db.UtilityMainFeederMappings
                                  where f.TagIndex == tagindex
                                  select new
                                  {
                                      f.FeederName
                                  }).FirstOrDefault();
                    // var FeederName = obj[0].FeederName;
                    var FeederName = feeder != null ? feeder.FeederName : "";
                    if (buttonType == "3")
                    {   if(Target1 == null)
                        {
                            Target.Add(0);
                        }
                         else
                        {
                            Target.Add(Math.Round(Convert.ToDouble(Target1.Target),2));
                        }
                        AllConsumption.Add(Consumption);
                        AllConsumption.Add(Target);
                        Names.Add("Actual");
                        Names.Add("Target");

                    }
                    else
                    {
                        Names.Add(FeederName);
                        var Total_Name = "Total_" + FeederName;
                        //Names.Add(Total_Name);

                        AllConsumption.Add(Consumption);
                        //AllConsumption.Add(Total_Consumption);
                    }

                }

                if (AllConsumption.Count() == 1 && AllConsumption[0].Count > 0)
                {
                    //if(buttonType == "1")
                    //{
                    //    foreach (var cumm in AllConsumption[0])
                    //    {
                    //        //cummCount += (double)cumm;
                    //    }
                    //}

                    cummCountAvg = Math.Round((Convert.ToDouble(cummCount / AllConsumption[0].Count())), 2);

                }
                return Json(new { AllConsumption, Names, buttonType, cummCountAvg, allTimes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }

        }
        

        public ActionResult ExportData(string StartDate, string EndDate, string Shop, string Shift)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int shop = Convert.ToInt32(Shop);
                var starttime = "";
                var endtime = "";
                if (Shift == null || Shift == "")
                {
                    starttime = null;
                    endtime = null;

                }
                else
                {


                    int shiftID = Convert.ToInt32(Shift);
                    var ShiftTime = (from s in db.MM_MTTUW_Shift
                                     where s.Shift_ID == shiftID
                                     select new
                                     {
                                         s.Shift_Start_Time,
                                         s.Shift_End_Time
                                     }).FirstOrDefault();

                    starttime = Convert.ToString(ShiftTime.Shift_Start_Time);
                    endtime = Convert.ToString(ShiftTime.Shift_End_Time);
                }
                DateTime startdate = DateTime.Parse(StartDate);
                DateTime enddate = DateTime.Parse(EndDate).AddDays(1);
                var ShiftTime1 = (from s in db.MM_MTTUW_Shift
                                  where s.Shift_ID == 1
                                 select new
                                 {
                                     s.Shift_Start_Time,
                                     s.Shift_End_Time
                                 }).FirstOrDefault();

                var start = TimeSpan.Parse((ShiftTime1.Shift_Start_Time).ToString());
                var End = TimeSpan.Parse((ShiftTime1.Shift_Start_Time).ToString());
                //var time = TimeSpan.Parse("06:30:00.000");
                //var time1 = TimeSpan.Parse("06:30:00.000");
                startdate = (startdate + start);
                enddate = (enddate + End);
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                DataSet ds = new DataSet();
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_ShopwiseSummary", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Shop", shop);
                    cmd.Parameters.AddWithValue("@STARTDATE", startdate);
                    cmd.Parameters.AddWithValue("@ENDDATE", enddate);
                    cmd.Parameters.AddWithValue("@Plant", plantID);
                    if (starttime == null)
                    {
                        cmd.Parameters.AddWithValue("@StartTime", DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndTime", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@StartTime", starttime);
                        cmd.Parameters.AddWithValue("@EndTime", endtime);
                    }

                    SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                    DataTable dt1 = new DataTable("Shop Consumption");
                   
                    da1.Fill(dt1);
                    con.Close();
                    ds.Tables.Add(dt1);
                    dt1.Columns["Plant_Name"].ColumnName = "Plant Name";
                    dt1.Columns["Shop_Name"].ColumnName = "Shop Name";
                    //dt1.Columns["shiftName"].ColumnName = "Shift Name";
                    dt1.Columns["ConsumptionDate"].ColumnName = "Consumption Date";
                    dt1.Columns["totalconsumption"].ColumnName = "Total Consumption";
                    
                }
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_FeederwiseConsumption", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Shop", shop);
                    cmd.Parameters.AddWithValue("@STARTDATE", startdate);
                    cmd.Parameters.AddWithValue("@ENDDATE", enddate);
                    cmd.Parameters.AddWithValue("@Plant", plantID);
                    if (starttime == null)
                    {
                        cmd.Parameters.AddWithValue("@StartTime", DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndTime", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@StartTime", starttime);
                        cmd.Parameters.AddWithValue("@EndTime", endtime);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Feederwise Consumption");
                    da.Fill(dt);
                    con.Close();
                    ds.Tables.Add(dt);
                }



                // ds.Tables.Add(dt);
                //  ds.Tables.Add(dt2);

                using (XLWorkbook wb = new XLWorkbook())
                {
                   
                    wb.Worksheets.Add(ds);
                    wb.Style.Font.Bold = true;
                    wb.Style.Font.FontColor = XLColor.Red;
                    wb.Style.Fill.BackgroundColor = XLColor.Cyan;
                    
                    //string excelFilePath = HttpContext.Server.MapPath("~").Replace("C:\\Excel\\", "/") + "PowerConsumptionReport.xlsx";

                    //wb.SaveAs(excelFilePath);
                    //wb.Dispose();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "feederWise Consumption.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }






        }
    }
}