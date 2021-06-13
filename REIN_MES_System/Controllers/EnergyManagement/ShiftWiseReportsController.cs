using System;
using System.Linq;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ClosedXML.Excel;
using System.IO;
using DocumentFormat.OpenXml;
using System.Collections.Generic;

namespace ZHB_AD.Controllers.Reports
{
    public class ShiftWiseReportsController : Controller
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();
        // GET: ShiftWiseReports
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
                globalData.pageTitle = ResourceShiftWiseReports.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.ToList(), "Plant_ID", "Plant_Name", plantID);
                ViewBag.Shift = new SelectList(db.MM_MTTUW_Shift.Where(s=>s.Shop_ID==30), "Shift_ID", "Shift_Name");
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange.Where(s=>s.DateID!=1), "DateID", "DateName");
                ViewBag.ShopName = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "user");
            }
           
        }

        public ActionResult Reading()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                int userId = ((FDSession)this.Session["FDSession"]).userId;
                globalData.pageTitle = ResourceShiftWiseReports.pageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = new SelectList(db.MM_MTTUW_Plants.ToList(), "Plant_ID", "Plant_Name", plantID);
                ViewBag.Shift = new SelectList(db.MM_MTTUW_Shift.Where(s=>s.Shop_ID ==30), "Shift_ID", "Shift_Name");
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange, "DateID", "DateName");
                ViewBag.ShopName = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "user");
            }

        }


        public ActionResult ExportReadingData(string StartDate, string EndDate, string Shop, string ddlFormat)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int shop;
                if (Shop == "")
                {
                   

                }
                else
                {
                     shop = Convert.ToInt32(Shop);
                }
               
                {


                    //int shiftID = Convert.ToInt32(Shift);
                    //var ShiftTime = (from s in db.MM
                    //                 where s.Shift_ID == shiftID
                    //                 select new
                    //                 {
                    //                     s.Shift_Start_Time,
                    //                     s.Shift_End_Time
                    //                 }).FirstOrDefault();

                    //starttime = Convert.ToString(ShiftTime.Shift_Start_Time);
                    //endtime = Convert.ToString(ShiftTime.Shift_End_Time);
                }
                DateTime startdate = DateTime.Parse(StartDate);
                DateTime enddate = DateTime.Parse(EndDate).AddDays(1);
                var ShiftTime = (from s in db.MM_MTTUW_Shift
                                 where s.Shift_ID ==25
                                 select new
                                 {
                                     s.Shift_Start_Time,
                                     s.Shift_End_Time
                                 }).FirstOrDefault();

                var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                var End = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                startdate = (startdate + start);
                if(ddlFormat == "2")
                {
                    enddate = (enddate).AddDays(1);
                    enddate = (enddate + End);
                }
                enddate = (enddate + End);
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                DataSet ds = new DataSet();
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_Feederwise_Reading", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if(Shop=="")
                    {
                        cmd.Parameters.AddWithValue("@Shop", DBNull.Value);
                    }
                    else
                     {
                        cmd.Parameters.AddWithValue("@Shop", Convert.ToInt32(Shop));
                    }
                    cmd.Parameters.AddWithValue("@STARTDATE", startdate);
                    cmd.Parameters.AddWithValue("@ENDDATE", enddate);
                    //cmd.Parameters.AddWithValue("@Plant", plantID);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                    DataTable dt1 = new DataTable("Feederwise Reading");
                    da1.Fill(dt1);
                    con.Close();
                    ds.Tables.Add(dt1);
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
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "feederWise Reading.xlsx");
                    }
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }






        }

        public ActionResult ExportshopwiseReadingData(string StartDate, string EndDate, string Shop )
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int shop;
                if (Shop == "")
                {


                }
                else
                {
                    shop = Convert.ToInt32(Shop);
                }

                DateTime startdate =Convert.ToDateTime(StartDate);
                DateTime enddate = Convert.ToDateTime(EndDate);
              
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                DataSet ds = new DataSet();
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_Feederwise_Reading", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (Shop == "")
                    {
                        cmd.Parameters.AddWithValue("@Shop", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Shop", Convert.ToInt32(Shop));
                    }
                    cmd.Parameters.AddWithValue("@STARTDATE", startdate);
                    cmd.Parameters.AddWithValue("@ENDDATE", enddate);
                    //cmd.Parameters.AddWithValue("@Plant", plantID);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                    DataTable dt1 = new DataTable("Feederwise Reading");
                    da1.Fill(dt1);
                    con.Close();
                    ds.Tables.Add(dt1);
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
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FeederWise_Reading.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }






        }

        public ActionResult ExportshopLiveData( string Shop)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int shop;
                if (Shop == "")
                {


                }
                else
                {
                    shop = Convert.ToInt32(Shop);
                }
                var Livedate = System.DateTime.Now;
                var Today = Livedate.AddMinutes(-30);
                //List<Sp_Feederwise_Live_Reading_Result> LiveData = db.Sp_Feederwise_Live_Reading(Shop, Today, Livedate).ToList();
                DateTime startdate = Convert.ToDateTime(Today);
                DateTime enddate = Convert.ToDateTime(Livedate);

                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                DataSet ds = new DataSet();
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_Feederwise_Live_Reading", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (Shop == "")
                    {
                        cmd.Parameters.AddWithValue("@Shop", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Shop", Convert.ToInt32(Shop));
                    }
                    cmd.Parameters.AddWithValue("@STARTDATE", startdate);
                    cmd.Parameters.AddWithValue("@ENDDATE", enddate);
                    //cmd.Parameters.AddWithValue("@Plant", plantID);
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                    DataTable dt1 = new DataTable("Feederwise_LiveReading");
                    da1.Fill(dt1);
                    con.Close();
                    ds.Tables.Add(dt1);
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
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FeederWise_LiveReading.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }






        }

        public ActionResult ExportFeederData(string StartDate, string EndDate, string Shop, string Feeder,int? Parameter)
        {
            try
            {

                
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                DataSet ds = new DataSet();
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("Sp_Feederwise_allParameter_Reading", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (Shop == "")
                    {
                        cmd.Parameters.AddWithValue("@Shop", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Shop", Convert.ToInt32(Shop));
                    }
                   
                    cmd.Parameters.AddWithValue("@STARTDATE", Convert.ToDateTime(StartDate));
                    cmd.Parameters.AddWithValue("@ENDDATE", Convert.ToDateTime(EndDate));
                    cmd.Parameters.AddWithValue("@Feeder", Convert.ToInt32(Feeder));
                    if (Parameter == null)
                    {
                        cmd.Parameters.AddWithValue("@Parameter", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Parameter", Parameter);
                    }
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                    DataTable dt1 = new DataTable("Feederwise Details Report");
                    da1.Fill(dt1);
                    con.Close();
                    ds.Tables.Add(dt1);
                }

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
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "feederWise Details Report.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");

            }






        }

        public ActionResult ExportData(string StartDate, string EndDate, string Shop,string Shift)
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


                    //int shiftID = Convert.ToInt32(Shift);
                    //var ShiftTime = (from s in db.MM_MTTUW_Shift
                    //                 where s.Shift_ID == shiftID
                    //                 select new
                    //                 {
                    //                     s.Shift_Start_Time,
                    //                     s.Shift_End_Time
                    //                 }).FirstOrDefault();

                    //starttime = Convert.ToString(ShiftTime.Shift_Start_Time);
                    //endtime = Convert.ToString(ShiftTime.Shift_End_Time);
                }
                DateTime startdate = DateTime.Parse(StartDate);
                DateTime enddate = DateTime.Parse(EndDate).AddDays(1);
                var ShiftTime = (from s in db.MM_MTTUW_Shift
                                 where s.Shift_ID == 25
                                 select new
                                 {
                                     s.Shift_Start_Time,
                                     s.Shift_End_Time
                                 }).FirstOrDefault();
              
                var start = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
                var End = TimeSpan.Parse((ShiftTime.Shift_Start_Time).ToString());
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
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            } 
        }

        // GET: ShiftWiseReports/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ShiftWiseReports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShiftWiseReports/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ShiftWiseReports/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ShiftWiseReports/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ShiftWiseReports/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ShiftWiseReports/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
