using ZHB_AD.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.App_LocalResources;

namespace ZHB_AD.Controllers.ZHB_AD
{
    public class HomeEnergyController : Controller
    {

        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();
        // GET: HomeEnergy
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
                globalData.pageTitle = ResourceEnergyHome.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                //ViewBag.Plant_ID = new SelectList((from s in db.MM_MTTUW_Plants
                //                                   join
                //                                   u in db.MM_Plant_User on
                //                                   s.Plant_ID equals u.Plant_ID
                //                                   where u.User_ID == userId
                //                                   select (s)).ToList(), "Plant_ID", "Plant_Name", plantID);
                ViewBag.Shift = new SelectList(db.MM_MTTUW_Shift.Where(s=>s.Shop_ID==30), "Shift_ID", "Shift_Name");

                ViewBag.Parameter = new SelectList(db.MM_Parameter, "Prameter_ID", "Prameter_Name");
                ViewBag.Reason_ID = new SelectList(db.MM_Holiday_Reason, "Reason_ID", "Reason_Name");
                //var This_Month = DateTime.Now.ToString("MMM yyyy");
                //var Previous_Month = DateTime.Now.AddMonths(-1).ToString("MMM yyyy");
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange.Where(s => s.DateID == 1 || s.DateID == 2 || s.DateID == 3 || s.DateID == 4 || s.DateID == 5), "DateID", "DateName", 2);



                ViewBag.ShopName = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Energy ==true), "Shop_ID", "Shop_Name");
                ViewBag.Parameter = new SelectList(db.MM_ConsumptionType, "Consumption_ID", "ConsumptionName");

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "user");
            }

            // return View();
        }


        public ActionResult Gethomedashoard(string StartDate, string EndDate, int ddlDateRange, string ConsumptionId, int? shift, int? holiday)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }

                DateTime fromdate = DateTime.Now;
                DateTime toDate = DateTime.Now;

                DateTime date = DateTime.Now.Date;

                var shiftwise = (from s in db.MM_MTTUW_Shift
                                 where s.Shift_ID ==25
                                 select
                                 new
                                 {
                                     s.Shift_Start_Time
                                 }).FirstOrDefault();
                var starttime = TimeSpan.Parse(shiftwise.Shift_Start_Time.ToString());
                DateTime startdate1 = DateTime.Parse(StartDate);
                DateTime enddate1 = DateTime.Parse(EndDate);

                DateTime start_m = DateTime.Parse(StartDate);
                DateTime end_m = DateTime.Parse(EndDate);
                var hours = (end_m - start_m).TotalHours;


                int Shop = 0;
                string shopName = null;
                string groupName = null;
                double total = 0;
                double PlantToatal = 0;

                List<ShopWiseConsumption> consumptionwise3 = new List<ShopWiseConsumption>();
                var result = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime == date && s.ConsumptionType == type).ToList();
                var todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime == date && s.ConsumptionType == type).ToList();

                var startd = Convert.ToDateTime(StartDate);
                var endd = Convert.ToDateTime(EndDate);

                if (shift != null)
                {
                    if (ddlDateRange == 5)
                    {

                    }
                    else
                    {
                        startd = (startd.Date + starttime);
                        endd = (endd.Date.AddDays(1) + starttime);
                    }


                    var ShiftResult = db.MM_Shiftwise_Consume_Power.Where(s => s.Dateandtime >= startd && s.Dateandtime < endd && s.ConsumptionType == type && s.Shift_ID == shift && s.Income_Power == true).ToList();
                    if (ShiftResult.Count() > 0)
                    {
                        foreach (var id in ShiftResult)
                        {
                            var Group = (from s in db.MM_MTTUW_Shops
                                         join g in db.MM_ShopsCategory on
                                                       s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == id.Shop_ID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_ID,
                                             s.Shop_Name,
                                             g.ShopsCategory_Name,
                                             g.ShopsCat_ID

                                         }).FirstOrDefault();
                            if (Group != null)
                            {
                                Shop = Convert.ToInt16(id.Shop_ID);

                                shopName = Group.Shop_Name;
                                groupName = Group.ShopsCategory_Name;
                                total = Convert.ToDouble(id.Consumption);
                                ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                consumptionwise3.Add(obj);
                            }


                        }
                        PlantToatal = ShiftResult.Where(s => s.Shop_ID == 0).Sum(s => s.Consumption).Value;
                    }
                    else
                    {
                        startd = Convert.ToDateTime(StartDate);
                        endd = Convert.ToDateTime(EndDate);

                        //var shiftwise = (from s in db.MM_MTTUW_Shift
                        //                 where s.Shift_ID == 1
                        //                 select
                        //                 new
                        //                 {
                        //                     s.Shift_Start_Time
                        //                 }).FirstOrDefault();
                        //var starttime = TimeSpan.Parse(shiftwise.Shift_Start_Time.ToString());
                        //date = date.AddDays(-1);
                        fromdate = (startd.Date + starttime);
                        toDate = (endd.Date.AddDays(1) + starttime);
                        todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime >= fromdate && s.DateandTime <= toDate && s.ConsumptionType == type).ToList();


                        foreach (var id in todayresult)
                        {
                            var Group = (from s in db.MM_MTTUW_Shops
                                         join g in db.MM_ShopsCategory on
                                                       s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == id.Shop_ID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_ID,
                                             s.Shop_Name,
                                             g.ShopsCategory_Name,
                                             g.ShopsCat_ID

                                         }).FirstOrDefault();
                            if (Group != null)
                            {
                                Shop = Convert.ToInt16(id.Shop_ID);

                                shopName = Group.Shop_Name;
                                groupName = Group.ShopsCategory_Name;
                                total = Convert.ToDouble(id.TotalConsumption);
                                ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                consumptionwise3.Add(obj);
                            }


                        }
                        PlantToatal = todayresult.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                        //}
                    }
                }
                else
                {

                    if (ddlDateRange == 1 || (startd.Date == date.Date))
                    {

                        //date = date.AddDays(-1);
                        fromdate = (startdate1.Date + starttime);
                        toDate = System.DateTime.Now;
                        todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime > fromdate && s.DateandTime <= toDate && s.ConsumptionType == type).ToList();


                        foreach (var id in todayresult)
                        {
                            var Group = (from s in db.MM_MTTUW_Shops
                                         join g in db.MM_ShopsCategory on
                                                       s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == id.Shop_ID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_ID,
                                             s.Shop_Name,
                                             g.ShopsCategory_Name,
                                             g.ShopsCat_ID

                                         }).FirstOrDefault();
                            if (Group != null)
                            {
                                Shop = Convert.ToInt16(id.Shop_ID);

                                shopName = Group.Shop_Name;
                                groupName = Group.ShopsCategory_Name;
                                total = Convert.ToDouble(id.TotalConsumption);
                                ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                consumptionwise3.Add(obj);
                            }


                        }
                        PlantToatal = todayresult.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                    }
                    else if (ddlDateRange == 5 && hours <= 24)
                    {
                        todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime >= start_m && s.DateandTime <= end_m && s.ConsumptionType == type).ToList();


                        foreach (var id in todayresult)
                        {
                            var Group = (from s in db.MM_MTTUW_Shops
                                         join g in db.MM_ShopsCategory on
                                                       s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == id.Shop_ID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_ID,
                                             s.Shop_Name,
                                             g.ShopsCategory_Name,
                                             g.ShopsCat_ID

                                         }).FirstOrDefault();
                            if (Group != null)
                            {
                                Shop = Convert.ToInt16(id.Shop_ID);

                                shopName = Group.Shop_Name;
                                groupName = Group.ShopsCategory_Name;
                                total = Convert.ToDouble(id.TotalConsumption);
                                ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                consumptionwise3.Add(obj);
                            }


                        }
                        PlantToatal = todayresult.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                    }
                    else
                    {

                        {
                            result = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime >= startd.Date && s.DateandTime <= endd.Date && s.ConsumptionType == type).ToList();
                            if (result.Count() > 0)
                            {
                                foreach (var id in result)
                                {
                                    var Group = (from s in db.MM_MTTUW_Shops
                                                 join g in db.MM_ShopsCategory on
                                                               s.ShopsCat_ID equals g.ShopsCat_ID
                                                 where s.Shop_ID == id.Shop_ID && s.Energy == true
                                                 select new
                                                 {
                                                     s.Shop_ID,
                                                     s.Shop_Name,
                                                     g.ShopsCategory_Name,
                                                     g.ShopsCat_ID

                                                 }).FirstOrDefault();
                                    if (Group != null)
                                    {
                                        Shop = Convert.ToInt16(id.Shop_ID);

                                        shopName = Group.Shop_Name;
                                        groupName = Group.ShopsCategory_Name;
                                        total = Convert.ToDouble(id.TotalConsumption);
                                        ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                        consumptionwise3.Add(obj);
                                    }


                                }
                                PlantToatal = result.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                            }
                            else
                            {
                                startd = Convert.ToDateTime(StartDate);
                                endd = Convert.ToDateTime(EndDate);
                                //if (ddlDateRange == 1 || (startd.Date == date.Date))
                                //{
                                //var shiftwise = (from s in db.MM_MTTUW_Shift
                                //                 where s.Shift_ID == 1
                                //                 select
                                //                 new
                                //                 {
                                //                     s.Shift_Start_Time
                                //                 }).FirstOrDefault();
                                //var starttime = TimeSpan.Parse(shiftwise.Shift_Start_Time.ToString());
                                //date = date.AddDays(-1);
                                fromdate = (startd + starttime);
                                toDate = (endd.AddDays(1) + starttime);
                                todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime >= fromdate && s.DateandTime <= toDate && s.ConsumptionType == type).ToList();


                                foreach (var id in todayresult)
                                {
                                    var Group = (from s in db.MM_MTTUW_Shops
                                                 join g in db.MM_ShopsCategory on
                                                               s.ShopsCat_ID equals g.ShopsCat_ID
                                                 where s.Shop_ID == id.Shop_ID && s.Energy == true
                                                 select new
                                                 {
                                                     s.Shop_ID,
                                                     s.Shop_Name,
                                                     g.ShopsCategory_Name,
                                                     g.ShopsCat_ID

                                                 }).FirstOrDefault();
                                    if (Group != null)
                                    {
                                        Shop = Convert.ToInt16(id.Shop_ID);

                                        shopName = Group.Shop_Name;
                                        groupName = Group.ShopsCategory_Name;
                                        total = Convert.ToDouble(id.TotalConsumption);
                                        ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                        consumptionwise3.Add(obj);
                                    }


                                }
                                PlantToatal = todayresult.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                                //}
                            }
                        }
                    }



                }



                var consumptionwise1 = consumptionwise3.GroupBy(c => c.ShopsCat_ID).Select(c => new { totalconsumption = c.Sum(b => b.totalconsumption), ShopsCat_ID = c.Key, }).ToList();
                var Shopgroups = (from shop in db.MM_MTTUW_Shops
                                  join shopgroup in db.MM_ShopsCategory
                                  on shop.ShopsCat_ID equals shopgroup.ShopsCat_ID
                                  where shop.Plant_ID == plantID && shop.Energy ==true
                                  select shopgroup
                                 ).ToList().Distinct();
                var Shoplist = (from shop in db.MM_MTTUW_Shops
                                join shopgroup in db.MM_ShopsCategory
                                on shop.ShopsCat_ID equals shopgroup.ShopsCat_ID
                                where shop.Plant_ID == plantID && shop.Energy == true
                                select new
                                {
                                    shop.Shop_ID,
                                    shop.Shop_Name,
                                    shop.ShopsCat_ID,
                                    shop.Spec_Unit_ID,
                                    shopgroup.ShopsCategory_Name,
                                    shop.Sort_Order
                                }
                                   ).ToList().Distinct().OrderBy(s => s.Sort_Order);


                var shopwisedata = Shoplist.OrderBy(s => s.Sort_Order).Select(c => new ShopWiseConsumption1
                {
                    ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                    ShopId = Convert.ToInt32(c.Shop_ID),
                    ShopName = c.Shop_Name,
                    ShopsCategory_Name = c.ShopsCategory_Name,
                    totalconsumption = Convert.ToDouble(consumptionwise3.Where(dc => dc.ShopId == c.Shop_ID).Sum(dc => dc.totalconsumption))
                }).Distinct().ToList();

                var res = Shopgroups.OrderBy(s => s.Sort_Order).Select(c => new ShopWiseConsumption1
                {

                    ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                    ShopId = Shop,
                    ShopName = shopName,
                    ShopsCategory_Name = c.ShopsCategory_Name,
                    totalconsumption = Convert.ToDouble(consumptionwise3.Where(dc => dc.ShopsCat_ID == c.ShopsCat_ID).Sum(dc => dc.totalconsumption))
                }).Distinct().ToList();


                // Target
                object Targetdata;
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
                    // target list 
                    var Target = (from t in db.MM_PowerTarget
                                  join
                                  s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                                  where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 1 && t.Category == 1
                                  select new
                                  {
                                      t.Target,
                                      s.Shop_Name,
                                      t.Shop_ID,
                                      s.ShopsCat_ID
                                  }).ToList();
                    if (Target.Count() == 0)
                    {
                        CurYear = startdate1.Year.ToString();
                        var pre = startdate1.AddYears(-1);

                        PreYear = pre.Year.ToString();
                        //CurYear = CurrentYear.ToString();
                        FinYear = PreYear + "-" + CurYear;
                        Target = (from t in db.MM_PowerTarget
                                  join
                                  s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                                  where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 1 && t.Category == 1
                                  select new
                                  {
                                      t.Target,
                                      s.Shop_Name,
                                      t.Shop_ID,
                                      s.ShopsCat_ID
                                  }).ToList();
                    }
                    if (shift != null)
                    {
                        Target = (from t in db.MM_PowerTarget
                                  join
                                  s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                                  where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 2 && t.Category == 1 && t.Shift_ID == shift
                                  select new
                                  {
                                      t.Target,
                                      s.Shop_Name,
                                      t.Shop_ID,
                                      s.ShopsCat_ID
                                  }).ToList();
                        if (Target.Count() == 0)
                        {
                            CurYear = startdate1.Year.ToString();
                            var pre = startdate1.AddYears(-1);

                            PreYear = pre.Year.ToString();
                            //CurYear = CurrentYear.ToString();
                            FinYear = PreYear + "-" + CurYear;
                            Target = (from t in db.MM_PowerTarget
                                      join
                                      s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                                      where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 2 && t.Category == 1 && t.Shift_ID == shift
                                      select new
                                      {
                                          t.Target,
                                          s.Shop_Name,
                                          t.Shop_ID,
                                          s.ShopsCat_ID
                                      }).ToList();
                        }
                    }

                    if (Target.Count() > 0)
                    {

                        List<ShopWiseConsumption> Targetlist = new List<ShopWiseConsumption>();
                        foreach (var id in Shoplist)
                        {
                            if (Target.Where(s => s.Shop_ID == id.Shop_ID).FirstOrDefault() != null)
                            {
                                var totalconsumption = Target.Where(s => s.Shop_ID == id.Shop_ID).Select(s => s.Target).FirstOrDefault();

                                ShopWiseConsumption obj = new ShopWiseConsumption(Convert.ToDecimal(id.ShopsCat_ID), Convert.ToInt32(id.Shop_ID), id.Shop_Name, id.ShopsCategory_Name, Math.Round(Convert.ToDouble(totalconsumption), 2));
                                Targetlist.Add(obj);
                            }
                            else
                            {
                                ShopWiseConsumption obj = new ShopWiseConsumption(Convert.ToDecimal(id.ShopsCat_ID), Convert.ToInt32(id.Shop_ID), id.Shop_Name, id.ShopsCategory_Name, 0);
                                Targetlist.Add(obj);
                            }
                        
                        }
                        Targetdata = Targetlist.Select(c => new ShopWiseConsumption1
                        {
                            ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                            ShopId = Convert.ToInt32(c.ShopId),
                            ShopName = c.ShopName,
                            ShopsCategory_Name = null,
                            totalconsumption = (c.totalconsumption)

                        }).ToList();
                    }
                    else
                    {
                        Targetdata = Shoplist.Select(c => new ShopWiseConsumption1
                        {
                            ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                            ShopId = Convert.ToInt32(c.Shop_ID),
                            ShopName = c.Shop_Name,
                            ShopsCategory_Name = c.ShopsCategory_Name,
                            totalconsumption = 0

                        }).Distinct().ToList();
                    }
                }



                var Incomer = (from i in db.MM_Plant_Consume_Config
                               where i.Plant_ID == plantID
                               select new
                               {
                                   i.TagIndex,
                                   i.Shop_ID,
                                   i.Formula_ID,
                                   i.ShopGroup_ID
                               }).ToList();



                var plantName = (from p in db.MM_MTTUW_Plants
                                 where p.Plant_ID == plantID
                                 select new { p.Plant_Name }).FirstOrDefault();

                ViewData["ShopwiseData"] = shopwisedata;
                ViewBag.PlantToatal = PlantToatal;
                ViewData["Targetdata"] = Targetdata;
                ViewBag.PlantName = plantName.Plant_Name;


                if (type)
                {
                    ViewBag.Tag = "kWh";
                }
                else
                {
                    ViewBag.Tag = "kVAh";
                }


                return PartialView("_getHomedashboard", res);



            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetkWhvechicle(string StartDate, string EndDate, int ddlDateRange, string ConsumptionId, int? shift, int? holiday)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                Boolean type = true;
                if (ConsumptionId != "1")
                {
                    type = false;
                }

                double PlantTotal = 0;
                DateTime fromdate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                DateTime date = DateTime.Now.Date;
                DateTime dtn = DateTime.Now.Date;
                //var time = TimeSpan.Parse("06:30:00.000");
                //var time1 = TimeSpan.Parse("23:59:00.000");
                DateTime startdate1 = DateTime.Parse(StartDate);
                DateTime enddate1 = DateTime.Parse(EndDate);
                DateTime start_m = DateTime.Parse(StartDate);
                DateTime end_m = DateTime.Parse(EndDate);
                var hours = (end_m - start_m).TotalHours;

                int Shop = 0;
                string shopName = null;
                string groupName = null;
                double total = 0;
                //double PlantConsume = 0;
                List<ShopWiseConsumption> consumptionwise3 = new List<ShopWiseConsumption>();
                List<ShopWiseConsumption> ProductionData = new List<ShopWiseConsumption>();
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
                // target list 
                var Target = (from t in db.MM_PowerTarget
                              join
                              s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                              where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 1 && t.Category == 2
                              select new
                              {
                                  t.Target,
                                  s.Shop_Name,
                                  t.Shop_ID,
                                  s.ShopsCat_ID
                              }).ToList();
                if (Target.Count() == 0)
                {
                    CurYear = startdate1.Year.ToString();
                    var pre = startdate1.AddYears(-1);

                    PreYear = pre.Year.ToString();
                    //CurYear = CurrentYear.ToString();
                    FinYear = PreYear + "-" + CurYear;
                    Target = (from t in db.MM_PowerTarget
                              join
                              s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                              where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 1
                              select new
                              {
                                  t.Target,
                                  s.Shop_Name,
                                  t.Shop_ID,
                                  s.ShopsCat_ID
                              }).ToList();
                }

                var Incomer = (from i in db.MM_Plant_Consume_Config
                               where i.Plant_ID == plantID
                               select new
                               {
                                   i.TagIndex,
                                   i.Shop_ID,
                                   i.Formula_ID,
                                   i.ShopGroup_ID
                               }).ToList();
                var PlantProd = (from p in db.MM_Plant_Production
                                 where p.Plant_ID == plantID
                                 select new
                                 {
                                     p.Shop_ID
                                 }).ToList();
                var result = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime == date && s.ConsumptionType == type).ToList();
                var todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime == date && s.ConsumptionType == type).ToList();
                var PlantToatal = 0.0;
                var PlantPro = 0;
                var Plantconsum = 0.0;
                var NoWorkingday = db.MM_No_Working_Day.Where(s => s.Day_Date == date.Date).FirstOrDefault();
                var startd = Convert.ToDateTime(StartDate);
                var endd = Convert.ToDateTime(EndDate);
                var shiftwise = (from s in db.MM_MTTUW_Shift
                                 where s.Shift_ID == 25
                                 select
                                 new
                                 {
                                     s.Shift_Start_Time
                                 }).FirstOrDefault();
                var starttime = TimeSpan.Parse(shiftwise.Shift_Start_Time.ToString());

                if (shift != null)
                {
                    Target = (from t in db.MM_PowerTarget
                              join
                              s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                              where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 2 && t.Category == 2 && t.Shift_ID == shift
                              select new
                              {
                                  t.Target,
                                  s.Shop_Name,
                                  t.Shop_ID,
                                  s.ShopsCat_ID
                              }).ToList();
                    if (Target.Count() == 0)
                    {
                        CurYear = startdate1.Year.ToString();
                        var pre = startdate1.AddYears(-1);

                        PreYear = pre.Year.ToString();
                        //CurYear = CurrentYear.ToString();
                        FinYear = PreYear + "-" + CurYear;
                        Target = (from t in db.MM_PowerTarget
                                  join
                                  s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                                  where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 3 && t.Category == 2 && t.Shift_ID == shift
                                  select new
                                  {
                                      t.Target,
                                      s.Shop_Name,
                                      t.Shop_ID,
                                      s.ShopsCat_ID
                                  }).ToList();
                    }
                    startd = (startd.Date + starttime);
                    endd = (endd.Date.AddDays(1) + starttime);
                    var shiftresult = db.MM_Shiftwise_Consume_Power.Where(s => s.Dateandtime >= startd && s.Dateandtime < endd && s.ConsumptionType == type && s.Income_Power == true && s.Shift_ID == shift).ToList();


                    if (StartDate == EndDate)
                    {
                        NoWorkingday = db.MM_No_Working_Day.Where(s => s.Day_Date == startd.Date).FirstOrDefault();

                        if (NoWorkingday != null)
                        {
                            Target = (from t in db.MM_PowerTarget
                                      join
                                      s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                                      where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 3 && t.Category == 2
                                      select new
                                      {
                                          t.Target,
                                          s.Shop_Name,
                                          t.Shop_ID,
                                          s.ShopsCat_ID
                                      }).ToList();
                        }
                    }
                    foreach (var id in shiftresult)
                    {
                        var Group = (from s in db.MM_MTTUW_Shops
                                     join g in db.MM_ShopsCategory on
                                                   s.ShopsCat_ID equals g.ShopsCat_ID
                                     where s.Shop_ID == id.Shop_ID && s.Energy == true
                                     select new
                                     {
                                         s.Shop_ID,
                                         s.Shop_Name,
                                         g.ShopsCategory_Name,
                                         g.ShopsCat_ID

                                     }).FirstOrDefault();
                        if (Group != null)
                        {
                            Shop = Convert.ToInt16(id.Shop_ID);

                            shopName = Group.Shop_Name;
                            groupName = Group.ShopsCategory_Name;
                            if (StartDate == EndDate)
                            {
                                total = Convert.ToDouble(id.SpecConsumption);
                                if (NoWorkingday != null)
                                {
                                    total = Convert.ToDouble(id.Consumption);
                                    total = Math.Round(total / 8, 2);
                                }
                            }
                            else
                            {
                                total = Convert.ToDouble(id.Consumption);
                            }

                            ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                            ShopWiseConsumption obj1 = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, Convert.ToDouble(id.Production));
                            consumptionwise3.Add(obj);
                            ProductionData.Add(obj1);
                        }


                    }
                    PlantPro = result.Where(s => s.Shop_ID == 0).Sum(s => s.Production).Value;
                    Plantconsum = result.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                    if (PlantPro != 0 || PlantProd != null)
                    {
                        PlantToatal = Math.Round(Plantconsum / PlantPro, 0);
                    }
                    else
                    {
                        PlantToatal = 0;
                    }
                }
                else
                {

                    if (ddlDateRange == 1)
                    {

                        fromdate = (startdate1 + starttime);
                        toDate = System.DateTime.Now;
                        todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime >= fromdate && s.DateandTime <= toDate && s.ConsumptionType == type).ToList();

                        foreach (var shop in db.MM_MTTUW_Shops.ToList())
                        {

                            var Group = (from s in db.MM_MTTUW_Shops
                                         join g in db.MM_ShopsCategory on
                                                       s.ShopsCat_ID equals g.ShopsCat_ID
                                         where s.Shop_ID == shop.Shop_ID && s.Energy == true
                                         select new
                                         {
                                             s.Shop_ID,
                                             s.Shop_Name,
                                             g.ShopsCategory_Name,
                                             g.ShopsCat_ID

                                         }).FirstOrDefault();
                            if (Group != null)
                            {
                                Shop = Convert.ToInt16(shop.Shop_ID);

                                shopName = Group.Shop_Name;
                                groupName = Group.ShopsCategory_Name;
                                var totalconsumption = Convert.ToDouble(todayresult.Where(dc => dc.Shop_ID == shop.Shop_ID).Sum(dc => dc.TotalConsumption));
                                var Procount = Convert.ToDouble(todayresult.Where(dc => dc.Shop_ID == shop.Shop_ID).Sum(dc => dc.Production));
                                if (Procount > 0)
                                {
                                    total = Math.Round(totalconsumption / Procount, 2);
                                }
                                else
                                {
                                    total = 0;
                                }

                                ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                consumptionwise3.Add(obj);
                            }


                        }



                        PlantPro = todayresult.Where(s => s.Shop_ID == 0).Sum(s => s.Production).Value;
                        Plantconsum = todayresult.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                        if (PlantPro != 0 || PlantProd != null)
                        {
                            PlantToatal = Math.Round(Plantconsum / PlantPro, 0);
                        }
                        else
                        {
                            PlantToatal = 0;
                        }
                    }
                    else
                    {

                        if (ddlDateRange == 5 && hours <= 24)
                        {
                            todayresult = db.MM_Shopwise_TimeConsumption.Where(s => s.DateandTime >= start_m && s.DateandTime <= end_m && s.ConsumptionType == type).ToList();
                            foreach (var shop in db.MM_MTTUW_Shops.ToList())
                            {

                                var Group = (from s in db.MM_MTTUW_Shops
                                             join g in db.MM_ShopsCategory on
                                                           s.ShopsCat_ID equals g.ShopsCat_ID
                                             where s.Shop_ID == shop.Shop_ID && s.Energy == true
                                             select new
                                             {
                                                 s.Shop_ID,
                                                 s.Shop_Name,
                                                 g.ShopsCategory_Name,
                                                 g.ShopsCat_ID

                                             }).FirstOrDefault();
                                if (Group != null)
                                {
                                    Shop = Convert.ToInt16(shop.Shop_ID);

                                    shopName = Group.Shop_Name;
                                    groupName = Group.ShopsCategory_Name;
                                    var totalconsumption = Convert.ToDouble(todayresult.Where(dc => dc.Shop_ID == shop.Shop_ID).Sum(dc => dc.TotalConsumption));
                                    var Procount = Convert.ToDouble(todayresult.Where(dc => dc.Shop_ID == shop.Shop_ID).Sum(dc => dc.Production));
                                    if (Procount > 0)
                                    {
                                        total = Math.Round(totalconsumption / Procount, 2);
                                    }
                                    else
                                    {
                                        total = 0;
                                    }

                                    ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                    consumptionwise3.Add(obj);
                                }


                            }



                            PlantPro = todayresult.Where(s => s.Shop_ID == 0).Sum(s => s.Production).Value;
                            Plantconsum = todayresult.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                            if (PlantPro != 0 || PlantProd != null)
                            {
                                PlantToatal = Math.Round(Plantconsum / PlantPro, 0);
                            }
                            else
                            {
                                PlantToatal = 0;
                            }
                        }
                        else
                        {
                            result = db.MM_Performance_Indices_Energy.Where(s => s.DateandTime >= startd.Date && s.DateandTime <= endd.Date && s.ConsumptionType == type).ToList();


                            if (StartDate == EndDate)
                            {
                                NoWorkingday = db.MM_No_Working_Day.Where(s => s.Day_Date == startd.Date).FirstOrDefault();

                                if (NoWorkingday != null)
                                {
                                    Target = (from t in db.MM_PowerTarget
                                              join
                                              s in db.MM_MTTUW_Shops on t.Shop_ID equals s.Shop_ID
                                              where t.Year == FinYear && t.Month == month && t.ConsumptionType == type && t.TargetType == 3
                                              select new
                                              {
                                                  t.Target,
                                                  s.Shop_Name,
                                                  t.Shop_ID,
                                                  s.ShopsCat_ID
                                              }).ToList();
                                }
                            }
                            foreach (var id in result)
                            {
                                var Group = (from s in db.MM_MTTUW_Shops
                                             join g in db.MM_ShopsCategory on
                                                           s.ShopsCat_ID equals g.ShopsCat_ID
                                             where s.Shop_ID == id.Shop_ID && s.Energy == true
                                             select new
                                             {
                                                 s.Shop_ID,
                                                 s.Shop_Name,
                                                 g.ShopsCategory_Name,
                                                 g.ShopsCat_ID

                                             }).FirstOrDefault();
                                if (Group != null)
                                {
                                    Shop = Convert.ToInt16(id.Shop_ID);

                                    shopName = Group.Shop_Name;
                                    groupName = Group.ShopsCategory_Name;
                                    if (StartDate == EndDate)
                                    {
                                        total = Convert.ToDouble(id.Consumption);
                                        if (NoWorkingday != null)
                                        {
                                            total = Convert.ToDouble(id.TotalConsumption);
                                            total = Math.Round(total / 24, 2);
                                        }
                                    }
                                    else
                                    {
                                        total = Convert.ToDouble(id.TotalConsumption);
                                    }

                                    ShopWiseConsumption obj = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, total);
                                    ShopWiseConsumption obj1 = new ShopWiseConsumption(Group.ShopsCat_ID, Shop, shopName, groupName, Convert.ToDouble(id.Production));
                                    consumptionwise3.Add(obj);
                                    ProductionData.Add(obj1);
                                }


                            }
                            PlantPro = result.Where(s => s.Shop_ID == 0).Sum(s => s.Production).Value;
                            Plantconsum = result.Where(s => s.Shop_ID == 0).Sum(s => s.TotalConsumption).Value;
                            if (PlantPro != 0 || PlantProd != null)
                            {
                                PlantToatal = Math.Round(Plantconsum / PlantPro, 0);
                            }
                            else
                            {
                                PlantToatal = 0;
                            }

                        }
                    }



                }

                var consumptionwise1 = consumptionwise3.GroupBy(c => c.ShopsCat_ID).Select(c => new { totalconsumption = c.Sum(b => b.totalconsumption), ShopsCat_ID = c.Key, }).ToList();
                var Shopgroups = (from shop in db.MM_MTTUW_Shops
                                  join shopgroup in db.MM_ShopsCategory
                                  on shop.ShopsCat_ID equals shopgroup.ShopsCat_ID
                                  where shop.Plant_ID == plantID && shop.Energy == true
                                  select shopgroup
                                 ).ToList().Distinct();


                var Shoplist = (from shop in db.MM_MTTUW_Shops
                                join shopgroup in db.MM_ShopsCategory
                                on shop.ShopsCat_ID equals shopgroup.ShopsCat_ID
                                where shop.Plant_ID == plantID && shop.Energy == true
                                select new {
                                    shop.Shop_ID,
                                shop.Shop_Name,
                                shop.Spec_Unit_ID,
                                shop.Sort_Order,
                                shop.ShopsCat_ID,
                                shopgroup.ShopsCategory_Name}
                                   ).ToList().Distinct();
                if (StartDate == EndDate || hours <= 24)
                {

                    var shopwisedata = Shoplist.OrderBy(s => s.Sort_Order).Select(c => new ShopWiseConsumption1
                    {
                        ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                        ShopId = Convert.ToInt32(c.Shop_ID),
                        ShopName = c.Shop_Name,
                        ShopsCategory_Name = c.ShopsCategory_Name,
                        totalconsumption = Convert.ToDouble(consumptionwise3.Where(dc => dc.ShopId == c.Shop_ID).Sum(dc => dc.totalconsumption))
                    }).Distinct().ToList();
                    ViewData["ShopwiseData"] = shopwisedata;
                }
                else
                {
                    List<ShopWiseConsumption> shopwiselist = new List<ShopWiseConsumption>();
                    double spec = 0.0;
                    foreach (var id in Shoplist)
                    {
                        var totalconsumption = Convert.ToDouble(consumptionwise3.Where(dc => dc.ShopId == id.Shop_ID).Sum(dc => dc.totalconsumption));
                        var Procount = Convert.ToDouble(ProductionData.Where(dc => dc.ShopId == id.Shop_ID).Sum(dc => dc.totalconsumption));
                        if (Procount > 0)
                        {
                            spec = Math.Round(totalconsumption / Procount, 2);
                        }
                        else
                        {
                            spec = 0;
                        }

                        ShopWiseConsumption obj = new ShopWiseConsumption(Convert.ToDecimal(id.ShopsCat_ID), Convert.ToInt32(id.Shop_ID), id.Shop_Name, id.ShopsCategory_Name, spec);
                        shopwiselist.Add(obj);
                    }
                    var shopwisedata = shopwiselist.Select(c => new ShopWiseConsumption1
                    {
                        ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                        ShopId = Convert.ToInt32(c.ShopId),
                        ShopName = c.ShopName,
                        ShopsCategory_Name = c.ShopsCategory_Name,
                        totalconsumption = c.totalconsumption
                    }).Distinct().ToList();
                    ViewData["ShopwiseData"] = shopwisedata;


                }



                object Targetdata;

                if (Target.Count() > 0)
                {

                    List<ShopWiseConsumption> Targetlist = new List<ShopWiseConsumption>();
                    foreach (var id in Shoplist)
                    {
                        if (Target.Where(s => s.Shop_ID == id.Shop_ID).FirstOrDefault() != null)
                        {
                            var totalconsumption = Target.Where(s => s.Shop_ID == id.Shop_ID).Select(s => s.Target).FirstOrDefault();

                            ShopWiseConsumption obj = new ShopWiseConsumption(Convert.ToDecimal(id.ShopsCat_ID), Convert.ToInt32(id.Shop_ID), id.Shop_Name, id.ShopsCategory_Name, Math.Round(Convert.ToDouble(totalconsumption), 2));
                            Targetlist.Add(obj);
                        }
                        else
                        {
                            ShopWiseConsumption obj = new ShopWiseConsumption(Convert.ToDecimal(id.ShopsCat_ID), Convert.ToInt32(id.Shop_ID), id.Shop_Name, id.ShopsCategory_Name, 0);
                            Targetlist.Add(obj);
                        }

                    }
                    Targetdata = Targetlist.Select(c => new ShopWiseConsumption1
                    {
                        ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                        ShopId = Convert.ToInt32(c.ShopId),
                        ShopName = c.ShopName,
                        ShopsCategory_Name = null,
                        totalconsumption = (c.totalconsumption)

                    }).ToList();
                }
                else
                {
                    Targetdata = Shoplist.Select(c => new ShopWiseConsumption1
                    {
                        ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                        ShopId = Convert.ToInt32(c.Shop_ID),
                        ShopName = c.Shop_Name,
                        ShopsCategory_Name = c.ShopsCategory_Name,
                        totalconsumption = 0

                    }).Distinct().ToList();
                }



                var res = Shopgroups.OrderBy(s => s.Sort_Order).Select(c => new ShopWiseConsumption1
                {

                    ShopsCat_ID = Convert.ToDecimal(c.ShopsCat_ID),
                    ShopId = Shop,
                    ShopName = shopName,
                    ShopsCategory_Name = c.ShopsCategory_Name,
                    totalconsumption = Convert.ToDouble(consumptionwise3.Where(dc => dc.ShopsCat_ID == c.ShopsCat_ID).Sum(dc => dc.totalconsumption))
                }).Distinct().ToList();








                var plantName = (from p in db.MM_MTTUW_Plants
                                 where p.Plant_ID == plantID
                                 select new { p.Plant_Name }).FirstOrDefault();

                //ViewData["ShopwiseData"] = shopwisedata;
                ViewData["Targetdata"] = Targetdata;
                ViewBag.PlantToatal = PlantTotal;
                ViewBag.PlantName = plantName.Plant_Name;
                if (type)
                {


                    ViewBag.Tag = "SEC(Actual)";
                    if (NoWorkingday != null)
                    {
                        ViewBag.Tag = "kWh/hr (Actual)";
                    }
                }
                else
                {

                    ViewBag.Tag = "SEC(Actual)";
                    if (NoWorkingday != null)
                    {
                        ViewBag.Tag = "kVAh/hr (Actual)";
                    }
                }

                //ViewBag.Tag = "kWh/Vehicle";
                return PartialView("_getkwhpervechicle", res);


            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}