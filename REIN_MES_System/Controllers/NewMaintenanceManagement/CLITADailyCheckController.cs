using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using ZHB_AD.Helper.IoT;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using ClosedXML.Excel;

namespace ZHB_AD.Controllers.NewMaintenanceManagement
{
    public class CLITADailyCheckController : BaseController
    {
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();

        // GET: CLITADailyCheck
        public ActionResult Index(decimal lineID = 0, decimal stationID = 0)
        {
            try
            {
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                ViewBag.GlobalDataModel = globalData;

                decimal plantID = ((FDSession)this.Session["FDSession"]).plantId;
                decimal shopID = ((FDSession)this.Session["FDSession"]).shopId;
                if (stationID == 0)
                {
                    stationID = ((FDSession)this.Session["FDSession"]).stationId;
                }

                if (lineID == 0)
                {
                    lineID = ((FDSession)this.Session["FDSession"]).lineId;
                }

                //stationID = 4;
                ViewBag.MachineList = null;
                ViewBag.ShopTitle = "Daily CLITA";
                List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Station_ID == stationID).ToList();
                ViewBag.Line_ID = new SelectList(db.MM_MTTUW_Lines.Where(p => p.Shop_ID == shopID), "Line_ID", "Line_Name", lineID);
                ViewBag.Station_ID = new SelectList(db.MM_MTTUW_Stations.Where(p => p.Line_ID == lineID), "Station_ID", "Station_Name", stationID);
                if (stationMachineList.Count() > 0)
                {
                    ViewBag.MachineList = stationMachineList;
                    var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                    IEnumerable<MM_MT_Clita> mMClitaItems = db.MM_MT_Clita.Where(a => machineIDList.Contains(a.Machine_ID));

                    return View(mMClitaItems);
                }
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "No Machines Assigned to this station !";
                    globalData.messageDetail = "";
                    TempData["globalData"] = globalData;
                }

            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "CLITADailyCheckController", "Index", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Operation Failed";
                globalData.messageDetail = "Exception - " + exp.Message;
                TempData["globalData"] = globalData;
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]

        public ActionResult logCLITAData(decimal clitaID, string value, Boolean status, string remarkss)
        {
            try
            {
                MetaShift objShift = new MetaShift();
                MM_MT_Clita clitaObj = db.MM_MT_Clita.Find(clitaID);
                int shiftID = objShift.getShiftID();
                //Added on 8-3-2018 for macine stopagges
                if (clitaObj != null)
                {
                    if (clitaObj.Is_Critical == true)
                    {

                    }
                }

                //var frequency = Convert.ToInt32(db.MM_MT_Clita.Where(m => m.Clita_ID == clitaID).Select(m => m.Cycle).FirstOrDefault());
                //var date = DateTime.Now;
                //var clitadate = date.AddDays(-frequency);

                //if (!db.MM_MT_Daily_Clita_Log.Any(a => a.Clita_ID == clitaID && DbFunctions.TruncateTime(a.Inserted_Date) > clitadate && DbFunctions.TruncateTime(a.Inserted_Date) <= date && a.Status == true && a.Shift_ID == shiftID))

                if (!db.MM_MT_Daily_Clita_Log.Any(a => a.Clita_ID == clitaID && DbFunctions.TruncateTime(a.Inserted_Date) == DateTime.Today && a.Status == true && a.Shift_ID == shiftID))

                {
                    MM_MT_Daily_Clita_Log clitaLog = new MM_MT_Daily_Clita_Log();
                    clitaLog.Clita_ID = clitaID;
                    clitaLog.Input_Value = value;

                    clitaLog.Status = status;
                    clitaLog.Remarks = remarkss;
                    clitaLog.Shift_ID = shiftID;
                    clitaLog.Station_ID = ((FDSession)this.Session["FDSession"]).stationId;
                    clitaLog.Inserted_Date = DateTime.Now;
                    clitaLog.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    clitaLog.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.MM_MT_Daily_Clita_Log.Add(clitaLog);
                    db.SaveChanges();
                }
                decimal machineID = clitaObj.Machine_ID;
                List<decimal> clitaList = db.MM_MT_Clita.Where(a => a.Machine_ID == machineID).Select(a => a.Clita_ID).ToList();
                int totalClitaforMachine = clitaList.Count();
                int totalClitaDoneToday = db.MM_MT_Daily_Clita_Log.Where(a => DbFunctions.TruncateTime(a.Inserted_Date) == DateTime.Today && clitaList.Contains(a.Clita_ID)).Count();
                if (totalClitaDoneToday >= totalClitaforMachine)
                {
                    Kepware kepwareObj = new Kepware();
                    //kepwareObj.machineResume(machineID, "1");
                    //kepwareObj.machinePause(machineID, "0");
                }

                return Json("true", JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "CLITADailyCheckController", "logCLITAData(clitaID : " + clitaID + ")", ((FDSession)this.Session["FDSession"]).userId);
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowMachineDailyCLITA(decimal id)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var plantName = db.MM_MTTUW_Plants.Where(m => m.Plant_ID == plantID).Select(m => m.Plant_Name).FirstOrDefault();
                var machineName = db.MM_MT_MTTUW_Machines.Where(m => m.Machine_ID == id).Select(m => m.Machine_Name).FirstOrDefault();
                globalData.plantName = plantName;
                globalData.pageTitle = "CLITA Daily Check" + " - " + machineName;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                ViewBag.GlobalDataModel = globalData;

                ViewBag.MachineList = null;
                List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Machine_ID == id).ToList();
                var mname = db.MM_MT_MTTUW_Machines.Where(a => a.Machine_ID == id).Select(m => m.Machine_Name).FirstOrDefault(); ;
                ViewBag.MachineName = machineName;
                if (stationMachineList.Count() > 0)
                {
                    ViewBag.ShopTitle = stationMachineList.First().Machine_Name + " Daily CLITA";
                    ViewBag.MachineList = stationMachineList;
                    var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                    IEnumerable<MM_MT_Clita> mMClitaItems = db.MM_MT_Clita.Where(a => machineIDList.Contains(a.Machine_ID) && a.Is_Deleted != true && a.IsActive == true).OrderBy(a => a.Sequence_No);

                    return View(mMClitaItems);
                }
                else
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = "No Machines found with this ID !";
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
                generalHelper.addControllerException(exp, "CLITADailyCheckDashboardController", "ShowMachineDailyCLITA(machineId : " + id + ")", ((FDSession)this.Session["FDSession"]).userId);
                return View();
            }
        }

        public ActionResult ExportData(string fromDate, string todate, string machineID)
        {
            try
            {
                int machineId = Convert.ToInt32(machineID);
                //DateTime FromDate = new DateTime();
                //DateTime ToDate = new DateTime();
                //if (fromDate == "" || todate == "")
                //{
                //    FromDate = DateTime.Now.AddDays(-30);
                //    ToDate = DateTime.Today.AddDays(1);

                //}
                //else
                //{
                //    var time = TimeSpan.Parse("00:00:00.000");
                //    ToDate = Convert.ToDateTime(todate).AddDays(1);
                    
                //    FromDate = Convert.ToDateTime(fromDate);
                   
                //}


                //var obj = (from clita in db.MM_MT_Clita
                //           join clitaData in db.MM_MT_Daily_Clita_Log
                //           on clita.Clita_ID equals clitaData.Clita_ID
                //           where clita.Machine_ID == machineId && clitaData.Inserted_Date >= FromDate && clitaData.Inserted_Date <= ToDate
                //           orderby clitaData.CLITA_DailyCheck_ID descending
                //           select new
                //           {
                //               clitaData.Clita_ID,
                //               clita.Clita_Item,
                //               clita.MM_MT_MTTUW_Machines.Machine_Name,
                //               clitaData.Inserted_Date,
                //               clitaData.Status,
                //               clitaData.Remarks,

                //           }).ToList();
                //var obj = obj2.GroupBy( i => i.Clita_ID, (key, group) => group.First()).ToList();
                DateTime StartDate = Convert.ToDateTime(fromDate);
                DateTime ToDate = Convert.ToDateTime(todate);
                var time = TimeSpan.Parse("23:59:59.000");

                ToDate = (ToDate + time);
                List<string> allTimes = new List<string>();

                List<decimal> IsOK = new List<decimal>();
                List<decimal> IsNOK = new List<decimal>();
                List<decimal> IsPending = new List<decimal>();

                int IsOKCount = 0;
                int IsNOKCount = 0;
                int IsPendingCount = 0;
                DateTime startDate = DateTime.Today;
                DateTime endDate = DateTime.Now;
               
                DateTime FromDate = startDate;
                

                var time1 = TimeSpan.Parse("00:00:00.000");

                DataTable da = new DataTable();

                da.Columns.Add("Sr.No");
                da.Columns.Add("Machine Name");
                da.Columns.Add("CLITA Parameter");
                da.Columns.Add("Date Time");
                da.Columns.Add("Status");
                da.Columns.Add("Remark");
                da.Columns.Add("Parameter value");

                int j = 1;

                FromDate = (FromDate + time1);
                var machineName = db.MM_MT_MTTUW_Machines.Where(m=>m.Machine_ID == machineId).Select(m=>m.Machine_Name).FirstOrDefault();
                var name = "";
                var datetime = "";
                var status = "";
                var Remark = "";
                var ParameterValue = "";
                var clitas = db.MM_MT_Clita.Where(m => m.Machine_ID == machineId).Select(m => m).OrderBy(m=>m.Sequence_No).ToList();
                for (startDate = StartDate; startDate <= ToDate; startDate = startDate.AddDays(1))
                {
                    //IsPendingCount = 0;
                    //IsOKCount = 0;
                    //IsNOKCount = 0;
                    
                    foreach (var item in clitas)
                    {
                        status = "";
                        Remark = "";
                        ParameterValue = "";
                        int sr = j;
                        name = db.MM_MT_Clita.Where(m => m.Clita_ID == item.Clita_ID).Select(m => m.Clita_Item).FirstOrDefault();
                        int freq = Convert.ToInt32(db.MM_MT_Clita.Where(m => m.Clita_ID == item.Clita_ID).Select(m => m.Cycle).FirstOrDefault());
                        if (freq == 1)
                        {
                            var EndDate = startDate.AddDays(1);
                            var clita = db.MM_MT_Daily_Clita_Log.Where(m => m.Inserted_Date > startDate && m.Inserted_Date <= EndDate && m.Clita_ID == item.Clita_ID).OrderByDescending(m => m.CLITA_DailyCheck_ID).FirstOrDefault();
                            if (clita != null)
                            {
                                if (clita.Status == true)
                                {
                                    //IsOKCount += 1;
                                    status = "OK";
                                    ParameterValue = clita.Input_Value;
                                }

                                if (clita.Status == false)
                                {
                                    //IsNOKCount += 1;
                                    status = "NOK";
                                    Remark = clita.Remarks;
                                    ParameterValue = clita.Input_Value;
                                }
                            }
                            else
                            {
                                //IsPendingCount += 1;
                                status = "Pending";
                            }
                        }
                        else if (freq > 1)
                        {
                            DateTime clitaStartDate = Convert.ToDateTime(db.MM_MT_Clita.Where(m => m.Clita_ID == item.Clita_ID).Select(m => m.Start_Date).FirstOrDefault());
                            var datediff = System.Math.Round((startDate - clitaStartDate).TotalDays, 0);
                            var rem = datediff % freq;
                            if (rem == 0)
                            {
                                var EndDate = startDate.AddDays(1);
                                var clita = db.MM_MT_Daily_Clita_Log.Where(m => m.Inserted_Date > startDate && m.Inserted_Date <= EndDate && m.Clita_ID == item.Clita_ID).OrderByDescending(m => m.CLITA_DailyCheck_ID).FirstOrDefault();
                                if (clita != null)
                                {
                                    if (clita.Status == true)
                                    {
                                        //IsOKCount += 1;
                                        status = "OK";
                                        ParameterValue = clita.Input_Value;
                                    }

                                    if (clita.Status == false)
                                    {
                                        //IsNOKCount += 1;
                                        status = "NOK";
                                        Remark = clita.Remarks;
                                        ParameterValue = clita.Input_Value;
                                    }
                                }
                                else
                                {
                                    //IsPendingCount += 1;
                                    status = "Pending";
                                }
                            }
                        }
                        datetime = startDate.ToString("dd/MM/yyyy");
                        if (status != "")
                        {
                            da.Rows.Add(sr, machineName, name, datetime, status, Remark, ParameterValue);
                            j++;
                        }
                    }

                }
                
                
                //for (int i = 0; i < obj.Count(); i++)
                //{
                    
                //    var name = obj[i].Clita_Item;
                //    var datetime = obj[i].Inserted_Date;
                //    var status = obj[i].Status == true ? "OK" : "NOK";
                //    var Remark = obj[i].Remarks;
                //    var pendingValue = obj[i].

                //    da.Rows.Add(sr, machineName, name, datetime, status, Remark);
                //    j++;
                //}
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("Clita Data");
                ws.Cell(1, 1).Value = "Sr.No";
                ws.Cell(1, 2).Value = "Machine Name";
                ws.Cell(1, 3).Value = "CLITA Parameter";
                ws.Cell(1, 4).Value = "Date Time";
                ws.Cell(1, 5).Value = "Status";
                ws.Cell(1, 6).Value = "Remark";
                ws.Cell(1, 7).Value = "Parameter Value";
                ws.Cell(2, 1).InsertData(da.AsEnumerable());
                ws.Columns(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Columns(3, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(1).Style.Font.Bold = true;
                ws.Cells("A1:G1").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ClitaData.xlsx");
                }

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult PreviousClitaData(int machine)
        {
            try
            {
                DateTime FromDate = DateTime.Now.AddDays(-30);
                DateTime ToDate = DateTime.Now;
                List<string> allTimes = new List<string>();
                List<decimal> ConsumedTime = new List<decimal>();
                var Clita = db.MM_MT_Daily_Clita_ConsumTime.Where(m => m.Machine_ID == machine && m.DateandTime > FromDate && m.DateandTime <= ToDate).ToList();
                if (Clita.Count > 0)
                {
                    foreach (var item in Clita)
                    {
                        DateTime date1 = Convert.ToDateTime(item.DateandTime);
                        var date = date1.ToString("dd/MM/yy h:mm:ss");
                        allTimes.Add(date);

                        ConsumedTime.Add((Convert.ToDecimal(item.ConsumTime)));
                    }
                }
                var MachineName = (from t in db.MM_MT_MTTUW_Machines
                                   where t.Machine_ID == machine
                                   select new
                                   {
                                       t.Machine_Name
                                   }).FirstOrDefault();


                //var unit = obj.UOM;
                //var paramName = obj.Machine_Parameter;
                //return Json(new { allTimes, CBMdata,unit,paramName }, JsonRequestBehavior.AllowGet);
                //var unit = obj.UOM;
                var Machine = MachineName.Machine_Name;
                FromDate = FromDate.Date;
                ToDate = ToDate.Date;
                var Startdate = FromDate.ToString("dd/MM/yyyy");
                var EndDate = ToDate.ToString("dd/MM/yyyy");
                var JSonResult = Json(new { allTimes, ConsumedTime, Machine, Startdate, EndDate }, JsonRequestBehavior.AllowGet);
                JSonResult.MaxJsonLength = Int32.MaxValue;
                return JSonResult;
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult CustomClitaData(int machine, string fromDate, string toDate)
        {
            try
            {
                DateTime StartDate = Convert.ToDateTime(fromDate);
                DateTime ToDate = Convert.ToDateTime(toDate);
                var time = TimeSpan.Parse("23:59:59.000");

                ToDate = (ToDate + time);
                List<string> allTimes = new List<string>();
               
                List<decimal> IsOK = new List<decimal>();
                List<decimal> IsNOK = new List<decimal>();
                List<decimal> IsPending = new List<decimal>();

                int IsOKCount = 0;
                int IsNOKCount = 0;
                int IsPendingCount = 0;
                DateTime startDate = DateTime.Today;
                DateTime endDate = DateTime.Now;
                //var Frequencies = db.MM_MT_Clita.Where(m => m.Machine_ID == machine).Select(m => m.Cycle).Distinct();
               // int MinFreq = Convert.ToInt32(Frequencies.Min());
                //var Totalbar = 15;
               // var TotalDays = 15 * MinFreq;
                //DateTime FromDate = startDate.AddDays(-TotalDays);
                DateTime FromDate = startDate;
                //DateTime ClitaStartDate = Convert.ToDateTime(db.MM_MT_Clita.Where(m => m.Machine_ID == machine).Select(m => m.Start_Date).Min());
                //if (FromDate < ClitaStartDate)
                //    FromDate = ClitaStartDate;

                var time1 = TimeSpan.Parse("00:00:00.000");

                FromDate = (FromDate + time1);
                var clitas = db.MM_MT_Clita.Where(m => m.Machine_ID == machine).Select(m => m.Clita_ID).ToList();
                //var counter = 0;
                for (startDate = StartDate; startDate <= ToDate; startDate = startDate.AddDays(1))
                {
                    IsPendingCount = 0;
                    IsOKCount = 0;
                    IsNOKCount = 0;
                    foreach (var item in clitas)
                    {
                        int freq = Convert.ToInt32(db.MM_MT_Clita.Where(m => m.Clita_ID == item).Select(m => m.Cycle).FirstOrDefault());
                        if(freq == 1)
                        {
                            var EndDate = startDate.AddDays(1);
                            var clita = db.MM_MT_Daily_Clita_Log.Where(m => m.Inserted_Date > startDate && m.Inserted_Date <= EndDate && m.Clita_ID == item).OrderByDescending(m => m.CLITA_DailyCheck_ID).FirstOrDefault();
                            if (clita != null)
                            {
                                if (clita.Status == true)
                                {
                                    IsOKCount += 1;

                                }

                                if (clita.Status == false)
                                    IsNOKCount += 1;
                            }
                            else
                            {
                                IsPendingCount += 1;
                            }
                        }
                        else if(freq > 1)
                        {
                            DateTime clitaStartDate = Convert.ToDateTime(db.MM_MT_Clita.Where(m => m.Clita_ID == item).Select(m => m.Start_Date).FirstOrDefault());
                            var datediff = System.Math.Round((startDate - clitaStartDate).TotalDays, 0);
                            var rem = datediff % freq;
                            if(rem == 0)
                            {
                                var EndDate = startDate.AddDays(1);
                                var clita = db.MM_MT_Daily_Clita_Log.Where(m => m.Inserted_Date > startDate && m.Inserted_Date <= EndDate && m.Clita_ID == item).OrderByDescending(m => m.CLITA_DailyCheck_ID).FirstOrDefault();
                                if (clita != null)
                                {
                                    if (clita.Status == true)
                                    {
                                        IsOKCount += 1;

                                    }

                                    if (clita.Status == false)
                                        IsNOKCount += 1;
                                }
                                else
                                {
                                    IsPendingCount += 1;
                                }
                            }
                        }
                        
                    }


                    IsOK.Add(IsOKCount);
                    IsNOK.Add(IsNOKCount);
                    IsPending.Add(IsPendingCount);
                    allTimes.Add(startDate.ToString("dd/MM/yyyy"));
                }
                //foreach (var frequency in Frequencies)
                //{
                //    if (counter < 15)
                //    {
                //        int cycle = Convert.ToInt32(frequency);
                //        for (startDate = FromDate, endDate = startDate.AddDays(cycle); endDate <= ToDate; startDate = startDate.AddDays(cycle), endDate = endDate.AddDays(cycle))
                //        {
                //            IsPendingCount = 0;
                //            IsOKCount = 0;
                //            IsNOKCount = 0;
                //            foreach (var item in clitas)
                //            {
                //                var clita = db.MM_MT_Daily_Clita_Log.Where(m => m.Inserted_Date > startDate && m.Inserted_Date <= endDate && m.Clita_ID == item).OrderByDescending(m => m.CLITA_DailyCheck_ID).FirstOrDefault();
                //                if (clita != null)
                //                {
                //                    if (clita.Status == true)
                //                    {
                //                        IsOKCount += 1;

                //                    }

                //                    if (clita.Status == false)
                //                        IsNOKCount += 1;
                //                }
                //                else
                //                {
                //                    IsPendingCount += 1;
                //                }
                //            }


                //            IsOK.Add(IsOKCount);
                //            IsNOK.Add(IsNOKCount);
                //            IsPending.Add(IsPendingCount);
                //            allTimes.Add(startDate.ToString("dd/MM/yyyy"));
                //            counter = counter + 1;
                //        }
                //    }
                //}

                //var JSonResult = Json(new { allTimes, ConsumedTime, Machine, Startdate, EndDate }, JsonRequestBehavior.AllowGet);
                var JSonResult = Json(new { allTimes, IsOK, IsNOK, IsPending }, JsonRequestBehavior.AllowGet);
                JSonResult.MaxJsonLength = Int32.MaxValue;
                return JSonResult;
            }
            catch
            {
                return RedirectToAction("Index");

            }
        }


        public ActionResult CheckPendingClita(int machine, int clitaID, int value)
        {
            var lowerLimit = db.MM_MT_Clita.Where(m => m.Clita_ID == clitaID).Select(m => m.Lower_Limit).FirstOrDefault();
            var upperlimit = db.MM_MT_Clita.Where(m => m.Clita_ID == clitaID).Select(m => m.Upper_Limit).FirstOrDefault();
            var status = false;
            if (value >= lowerLimit && value <= upperlimit)
                status = true;

            MetaShift objShift = new MetaShift();
            int shiftID = objShift.getShiftID();

            MM_MT_Daily_Clita_Log clitaLog = new MM_MT_Daily_Clita_Log();
            clitaLog.Clita_ID = clitaID;
            clitaLog.Input_Value = Convert.ToString(value);

            clitaLog.Status = status;
            //clitaLog.Remarks = remarkss;
            clitaLog.Shift_ID = shiftID;
            clitaLog.Station_ID = ((FDSession)this.Session["FDSession"]).stationId;
            clitaLog.Inserted_Date = DateTime.Now;
            clitaLog.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
            clitaLog.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
            db.MM_MT_Daily_Clita_Log.Add(clitaLog);
            db.SaveChanges();
            return Json(status, JsonRequestBehavior.AllowGet);
        }
       
    }
}