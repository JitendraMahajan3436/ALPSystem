using ClosedXML.Excel;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.NewMaintenanceManagement
{
    public class TBMDashboardController : BaseController
    {
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();

        // GET: TBMDashboard
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            ViewBag.GlobalDataModel = globalData;

            decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
            decimal stationID = ((FDSession)this.Session["FDSession"]).stationId;
            //stationID = 4;
            ViewBag.MachineList = null;
            ViewBag.ShopTitle = "Machine TBM Dashboard";
            List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Station_ID == stationID).ToList();
            if (stationMachineList.Count() > 0)
            {
                ViewBag.MachineList = stationMachineList;
                var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                IEnumerable<MM_MT_Preventive_Equipment> eqpmentList = db.MM_MT_Preventive_Equipment.Where(a => machineIDList.Contains(a.Machine_ID)).OrderBy(a => a.Equipment_Name);

                return View(eqpmentList);
            }
            else
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "No Machines Assigned to this station !";
                globalData.messageDetail = "";
                TempData["globalData"] = globalData;
            }

            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getEquipmentLifeDetails(decimal machineId)
        {
            try
            {
                var eqpmentDetailsList = db.MM_MT_Preventive_Equipment.Where(a => a.Machine_ID == machineId)
                                           .Select(a => new { a.EQP_ID, a.Designated_Life, a.Life_Per_Cycle, a.Remaining_Life, a.Warning_At, a.Stop_At }).Distinct().ToList();
                return Json(eqpmentDetailsList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "TBMDashboardController", "getEquipmentLifeDetails(machineId : " + machineId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult getEquipmentData(decimal eqpId)
        {
            try
            {
                MM_MT_Preventive_Equipment eqpObj = db.MM_MT_Preventive_Equipment.Find(eqpId);
                ViewBag.equipmentObj = eqpObj;
                return PartialView("PVEquipmentDetails");
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "MinorStoppagesController", "getEquipmentData(equipmentId : " + eqpId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }


        public ActionResult getTBMResetHistory(string EQID)
        {
            try
            {
                int EQ = Convert.ToInt32(EQID);

                List<TBMResetHistoryData> data = new List<TBMResetHistoryData>();

                var Data = (from t in db.MM_TBM_Reset_History
                            where t.TBM_ID == EQ
                            select new
                            {
                                t.Inserted_Date,
                                t.Designated_Life,
                                t.Consumed_Life,
                                t.Remarks,
                                t.Inserted_UserName
                            }).ToList();

                foreach (var item in Data)
                {
                    TBMResetHistoryData obj = new TBMResetHistoryData(Convert.ToDateTime(item.Inserted_Date), Convert.ToInt32(item.Designated_Life), Convert.ToInt32(item.Consumed_Life), Convert.ToString(item.Remarks), item.Inserted_UserName);
                    data.Add(obj);
                }

                return PartialView("TBMResetHistoryData", data);
            }
            catch
            {
                return null;
            }
        }

        public ActionResult ShowMachineTBM(decimal id)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var plantName = db.MM_MTTUW_Plants.Where(m => m.Plant_ID == plantID).Select(m => m.Plant_Name).FirstOrDefault();
                var machineName = db.MM_MT_MTTUW_Machines.Where(m => m.Machine_ID == id).Select(m => m.Machine_Name).FirstOrDefault();
                globalData.plantName = plantName;
                globalData.pageTitle = "TBM Dashboard - " + machineName;
                //if (TempData["globalData"] != null)
                //{
                //    globalData = (GlobalData)TempData["globalData"];
                //}
                ViewBag.GlobalDataModel = globalData;
                
                ViewBag.MachineList = null;
               
                List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Machine_ID == id && a.IsActive == true).ToList();
                if (stationMachineList.Count() > 0)
                {
                    ViewBag.ShopTitle = stationMachineList.First().Machine_Name + " TBM Dashboard";
                    ViewBag.MachineList = stationMachineList;
                    var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                    IEnumerable<MM_MT_Preventive_Equipment> eqpmentList = db.MM_MT_Preventive_Equipment.Where(a => machineIDList.Contains(a.Machine_ID) && a.IsActive == true).OrderBy(a => a.Sequence_No);

                    return View(eqpmentList);
                }
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "No Machine found !";
                    globalData.messageDetail = "";
                    TempData["globalData"] = globalData;
                }
                return View();
            }
            catch (Exception exp)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Exception Occurred in Server!";
                globalData.messageDetail = exp.Message;
                TempData["globalData"] = globalData;
                generalHelper.addControllerException(exp, "TBMDashboardController", "ShowMachineTBM(machineId : " + id + ")", ((FDSession)this.Session["FDSession"]).userId);
                return View();
            }
        }

        public ActionResult PreviousTBMData(int tbmid)
        {
            try
            {

                List<string> allTimes = new List<string>();
                List<int?> DesignatedData = new List<int?>();
                List<int?> ConsumedData =new List<int?>();
                List<string> UserName = new List<string>();
                DateTime FromDate = DateTime.Now.AddDays(-30);
                DateTime ToDate = DateTime.Now;
                var obj = db.MM_TBM_Reset_History.Where(m => m.TBM_ID == tbmid).FirstOrDefault();
                var Parameter_Data = db.MM_TBM_Reset_History.Where(m => m.TBM_ID == tbmid && m.Inserted_Date > FromDate && m.Inserted_Date < ToDate).ToList();
                if (Parameter_Data.Count > 0)
                {
                    foreach (var item in Parameter_Data)
                    {
                        //var date = item.Inserted_Date.ToString().Split(' ')[1];
                        DateTime date1 = Convert.ToDateTime(item.Inserted_Date);
                        var date = date1.ToString("dd/MM/yy h:mm:ss");
                        var id = item.Inserted_User_ID;
                        var EmpNo = db.MM_MTTUW_Employee.Where(m => m.Employee_ID == id).Select(m => m.Employee_No).FirstOrDefault();

                        allTimes.Add(date + ',' + item.Inserted_UserName + ',' + EmpNo + ',' + item.Designated_Life + ',' + item.Consumed_Life);

                        //allTimes.Add(date);
                        DesignatedData.Add(item.Designated_Life);
                        ConsumedData.Add(item.Consumed_Life);
                        UserName.Add(item.Inserted_UserName);
                    }
                }
                var ParamName = (from t in db.MM_MT_Preventive_Equipment
                                 where t.EQP_ID == tbmid
                                 select new
                                 {
                                     t.Equipment_Name
                                 }).FirstOrDefault();

                var Parameter = ParamName.Equipment_Name;
                //var unit = obj.UOM;
                //var paramName = obj.Machine_Parameter;
                //return Json(new { allTimes, CBMdata,unit,paramName }, JsonRequestBehavior.AllowGet);
                //var unit = obj.UOM;
                //var paramName = obj.Machine_Parameter;
                FromDate = FromDate.Date;
                ToDate = ToDate.Date;
                var Startdate = FromDate.ToString("dd/MM/yyyy");
                var EndDate = ToDate.ToString("dd/MM/yyyy");
                var JSonResult = Json(new { allTimes, DesignatedData, ConsumedData, UserName, Parameter,Startdate,EndDate }, JsonRequestBehavior.AllowGet);
                JSonResult.MaxJsonLength = Int32.MaxValue;
                return JSonResult;
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult CustomTBMData(int tbmid, string fromDate, string toDate)
        {
            try
            {

                List<string> allTimes = new List<string>();
                List<int?> DesignatedData = new List<int?>();
                List<int?> ConsumedData = new List<int?>();
                List<string> UserName = new List<string>();
                DateTime StartDate = Convert.ToDateTime(fromDate);

                DateTime ToDate = Convert.ToDateTime(toDate);
                var time = TimeSpan.Parse("23:59:59.000");
                //var time1 = TimeSpan.Parse("06:29:00.000");
                //date = date.AddDays(-1);
                ToDate = (ToDate + time);
                //DateTime FromDate = DateTime.Now.AddDays(-30);
                //DateTime ToDate = DateTime.Now;
                var obj = db.MM_TBM_Reset_History.Where(m => m.TBM_ID == tbmid).FirstOrDefault();
                var Parameter_Data = db.MM_TBM_Reset_History.Where(m => m.TBM_ID == tbmid && m.Inserted_Date > StartDate && m.Inserted_Date <= ToDate).ToList();
                if (Parameter_Data.Count > 0)
                {
                    foreach (var item in Parameter_Data)
                    {
                        //var date = item.Inserted_Date.ToString().Split(' ')[1];
                        DateTime date1 = Convert.ToDateTime(item.Inserted_Date);
                        var date = date1.ToString("dd/MM/yy h:mm:ss");
                        var id = item.Inserted_User_ID;
                        var EmpNo = db.MM_MTTUW_Employee.Where(m => m.Employee_ID == id).Select(m => m.Employee_No).FirstOrDefault();

                        allTimes.Add(date + ',' + item.Inserted_UserName + ',' + EmpNo + ',' + item.Designated_Life + ',' + item.Consumed_Life);

                        //allTimes.Add(date);
                        DesignatedData.Add(item.Designated_Life);
                        ConsumedData.Add(item.Consumed_Life);
                        UserName.Add(item.Inserted_UserName);
                    }
                }
                var ParamName = (from t in db.MM_MT_Preventive_Equipment
                                 where t.EQP_ID == tbmid
                                 select new
                                 {
                                     t.Equipment_Name
                                 }).FirstOrDefault();

                var Parameter = ParamName.Equipment_Name;

                //var unit = obj.UOM;
                //var paramName = obj.Machine_Parameter;
                //return Json(new { allTimes, CBMdata,unit,paramName }, JsonRequestBehavior.AllowGet);
                //var unit = obj.UOM;
                //var paramName = obj.Machine_Parameter;
               var Startdate = StartDate.ToString("dd/MM/yyyy");
                var EndDate = ToDate.ToString("dd/MM/yyyy");
                var JSonResult = Json(new { allTimes, DesignatedData, ConsumedData, UserName, Parameter, Startdate, EndDate }, JsonRequestBehavior.AllowGet);
                JSonResult.MaxJsonLength = Int32.MaxValue;
                return JSonResult;
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult ExportData(int tbmid, string fromDate, string toDate)
        {
            try
            {
                DateTime FromDate = new DateTime();
                DateTime StartDate = new DateTime();
                DateTime ToDate = new DateTime();
                if (fromDate == "")
                {
                    FromDate = DateTime.Now.AddDays(-30); 
                    ToDate = DateTime.Now;

                }
                else
                {
                    ToDate = DateTime.Now;
                    var ToString = ToDate.ToString().Split(' ')[1];
                    FromDate = Convert.ToDateTime(fromDate);
                    //var time = TimeSpan.Parse(ToString);
                    //StartDate = (StartDate + time);
                    ToDate = Convert.ToDateTime(toDate);
                    var time = TimeSpan.Parse("23:59:59.000");
                    //var time1 = TimeSpan.Parse("06:29:00.000");
                    //date = date.AddDays(-1);
                    ToDate = (ToDate + time);
                }

                List<string> allTimes = new List<string>();
                List<double> CBMdata = new List<double>();
                List<TBMExcelData> exceldata = new List<TBMExcelData>();
               

                var obj = db.MM_TBM_Reset_History.Where(m => m.TBM_ID == tbmid).FirstOrDefault();
                var Parameter_Data = db.MM_TBM_Reset_History.Where(m => m.TBM_ID == tbmid && m.Inserted_Date > FromDate && m.Inserted_Date <= ToDate).ToList();
                // var datediff = System.Math.Round((ToDate - StartDate).TotalDays, 0);


                //var Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > StartDate && m.Inserted_Date < ToDate).ToList();
                //var minVal = Convert.ToDouble(obj.Green_Min_Val * obj.Scale_Denominator);
                //var maxVal = Convert.ToDouble(obj.Green_Max_Val * obj.Scale_Denominator);
               
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable("CMBData");
                dt.Columns.Add("Sr.No");
                dt.Columns.Add("Machine Name");
                dt.Columns.Add("Parameter Name");
                dt.Columns.Add("Date Time");
                dt.Columns.Add("Designated Life");
                dt.Columns.Add("Consumed Life");
                dt.Columns.Add("User Name");

                if (Parameter_Data.Count > 0  && obj!= null)
                {
                    foreach (var item in Parameter_Data)
                    {
                        //var date = item.Inserted_Date.ToString().Split(' ')[1];
                        DateTime date1 = Convert.ToDateTime(item.Inserted_Date);
                        var date = date1.ToString("dd/MM/yy h:mm:ss");
                        var DesignatedData = item.Designated_Life;
                        var ConsumedData = item.Consumed_Life;                
                        TBMExcelData export = new TBMExcelData(date1.ToString(),Convert.ToInt32(DesignatedData), Convert.ToInt32(ConsumedData), item.Inserted_UserName);
                        exceldata.Add(export);
                        //allTimes.Add(date);
                        //CBMdata.Add(Convert.ToDouble(Convert.ToDouble(item.Parameter_Value) / Convert.ToDouble(obj.Scale_Denominator)));

                    }
                    var MachineID = obj.Machine_ID;
                    var MachineName = (from m in db.MM_MT_MTTUW_Machines
                                       where m.Machine_ID == MachineID
                                       select new
                                       {
                                           m.Machine_Name,

                                       }).FirstOrDefault();

                    var ParamName = (from t in db.MM_MT_Preventive_Equipment
                                     where t.EQP_ID == tbmid
                                     select new
                                     {
                                         t.Equipment_Name
                                     }).FirstOrDefault();
                    int j = 1;

                    if (exceldata.Count() > 0)
                    {
                        for (int i = 0; i < exceldata.Count(); i++)
                        {
                            int sr = j;
                            var name = MachineName.Machine_Name;
                            var datetime = exceldata[i].datetime;
                            var desinated = exceldata[i].desginated_data;
                            var consumed = exceldata[i].consumed_data;

                            dt.Rows.Add(sr, name, ParamName.Equipment_Name, datetime, desinated, consumed, exceldata[i].User);
                            j++;
                        }
                    }

                }



                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("TBM_Reset_HistoryData");
                ws.Cell(1, 1).Value = "Sr.No";
                ws.Cell(1,2).Value = "Machine Name";
                ws.Cell(1, 3).Value = "Parameter Name";
                ws.Cell(1, 4).Value = "Date Time";
                ws.Cell(1, 5).Value = "Designated Life";
                ws.Cell(1, 6).Value = "Consumed Life";
                ws.Cell(1, 7).Value = "User Name";
                ws.Cell(2, 1).InsertData(dt.AsEnumerable());
                ws.Columns(1,3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Columns(5,7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(1).Style.Font.Bold = true;
                ws.Cells("A1:G1").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Columns().AdjustToContents();
                    
  
                 
                //    wb.Style.Font.FontColor = XLColor.Red;
                //    wb.Style.Fill.BackgroundColor = XLColor.Cyan;
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TBM_Reset_HistoryData.xlsx");
                }

                //da.Fill(dt);
                //DataSet ds = new DataSet();
                //ds.Tables.Add(dt);
                //var workbook = new XLWorkbook();
                //var ws = workbook.Worksheets.Add("TBM_Reset_HistoryData");


                //var ws = workbook.Worksheets.Add(ds);
                //using (XLWorkbook wb = new XLWorkbook())
                //{

                //    wb.Worksheets.Add(ds);
                //    wb.Style.Font.Bold = true;
                //    wb.Style.Font.FontColor = XLColor.Red;
                //    wb.Style.Fill.BackgroundColor = XLColor.Cyan;




                //    string excelFilePath = HttpContext.Server.MapPath("~").Replace("C:\\Excel\\", "/") + "PowerConsumptionReport.xlsx";

                //    wb.SaveAs(excelFilePath);
                //    wb.Dispose();
                //    using (MemoryStream stream = new MemoryStream())
                //    {
                //        wb.SaveAs(stream);
                //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TBM_Reset_HistoryData.xlsx");
                //    }
                //}

            }
            catch (Exception exp)
            {
                return RedirectToAction("Index");
            }
        }
    }
}