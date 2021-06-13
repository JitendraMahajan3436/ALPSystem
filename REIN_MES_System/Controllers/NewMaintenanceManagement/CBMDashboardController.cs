using ClosedXML.Excel;
using iTextSharp.text;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using ZHB_AD.Helper.IoT;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using System.Data.Entity;

namespace ZHB_AD.Controllers.NewMaintenanceManagement
{
    public class CBMDashboardController : BaseController
    {
        REIN_SOLUTIONEntities db1 = new REIN_SOLUTIONEntities();
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        //CBMEntities CBMdb = new CBMEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        // GET: CBMDashboard
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
            ViewBag.ShopTitle = "Machine CBM Dashboard";
            List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Station_ID == stationID).ToList();
            if (stationMachineList.Count() > 0)
            {
                ViewBag.MachineList = stationMachineList;
                var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                IEnumerable<MM_MT_Conditional_Based_Maintenance> cbmList = db.MM_MT_Conditional_Based_Maintenance.Where(a => machineIDList.Contains(a.Machine_ID) && a.CBM_Check_Type != "Disable").OrderBy(a => a.Machine_Parameter);

                return View(cbmList);
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
        public ActionResult getCBMDetails(decimal machineId)
        {
            Kepware obj = new Kepware();
            try
            {
              
                List<string> IOTStrings = new List<string>();
                var cbmDetailsList = db.MM_MT_Conditional_Based_Maintenance.Where(a => a.Machine_ID == machineId && a.IsActive == true && a.CBM_Check_Type != "Disable")
                                           .Select(a => new { a.CBM_ID, a.IsActive, a.UOM, a.Min_Val, a.Max_Val, a.Green_Min_Val, a.Green_Max_Val, a.Warning_Min_Val, a.Warning_Max_Val, a.Current_Value, a.Scale_Denominator, a.Current_State, a.Parameter_Category, a.IOT_Tag_Name }).Distinct().ToList();
                foreach (var item in cbmDetailsList)
                {
                    if (item.IsActive == true)
                    {
                        IOTStrings.Add(item.IOT_Tag_Name);
                    }

                }
                ReadResults[] readData = obj.ReadCBMData(IOTStrings, cbmDetailsList[0].CBM_ID);
                if (readData != null)
                {
                    foreach (var v in readData)
                    {
                        MM_MT_Conditional_Based_Maintenance cbm = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.IOT_Tag_Name == v.id).FirstOrDefault();
                        cbm.Current_Value = v.v.ToString();
                        cbm.Is_Edited = true;
                        db.SaveChanges();
                    }
                }
                var cbmDetailsList1 = db.MM_MT_Conditional_Based_Maintenance.Where(a => a.Machine_ID == machineId && a.IsActive == true && a.CBM_Check_Type != "Disable")
                                           .Select(a => new { a.CBM_ID, a.UOM, a.Min_Val, a.Max_Val, a.Green_Min_Val, a.Green_Max_Val, a.Warning_Min_Val, a.Warning_Max_Val, a.Current_Value, a.Scale_Denominator, a.Current_State, a.Parameter_Category, a.IOT_Tag_Name, a.Image_ID }).Distinct().ToList();

                return Json(cbmDetailsList1, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "CBMDashboardController", "getCBMDetails(machineId : " + machineId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }

        //public FilePathResult exportPDf()
        //{
        //    var doc = new Document();
        //    var pdf = Server.MapPath("PDF/Chart.pdf");
        //    PdfWriter.GetInstance(doc, new FileStream(pdf, FileMode.Create));
        //    doc.Open();
        //    doc.Add(new Paragraph("History Data"));
        //    var image = Image.GetInstance(Chart());
        //    //var image = Image.GetInstance(IXLChart());
        //}


        public ActionResult getMachineStatus(int machineId)
        {
            var ConnectionType = db1.MM_MT_Machines.Where(m => m.Machine_ID == machineId).Select(m => m.Connection_TypeID).FirstOrDefault();

            if (ConnectionType == 2)
            {
                var status = db.Analytics_MCStatus.Where(m => m.Machine_id == machineId).OrderByDescending(m => m.Row_id).Select(m => m.Status_id).FirstOrDefault();
                return Json(new { Status = false, MStatus = false, MDBStatus = status, CType = ConnectionType }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var status = db1.MM_Ctrl_Equipment_Status
                .Where(c => c.Machine_ID == machineId)
                .GroupBy(c => new { c.Machine_ID })
                .Select(g => g.OrderByDescending(c => c.EQ_ID).FirstOrDefault())
                .Select(c => new { c.Machine_ID, c.isFaulty, c.isHealthy, c.isIdle, c.Heart_Bit });

                var MPLCStatus = db.MM_Ctrl_PLC_Status
                   .OrderByDescending(m => m.PS_ID)
                    .Select(m => m.Is_MasterPLC).FirstOrDefault();


                return Json(new { Status = status, MStatus = MPLCStatus, MDBStatus = 0, CType = ConnectionType }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowMachineCBM(int id)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var plantName = db1.MM_Plants.Where(m => m.Plant_ID == plantID).Select(m => m.Plant_Name).FirstOrDefault();
                var machineName = db.MM_MT_MTTUW_Machines.Where(m => m.Machine_ID == id).Select(m => m.Machine_Name).FirstOrDefault();
                var IsAnalytics = db1.MM_MT_Machines.Where(m => m.Machine_ID == id && m.IsActive == true).Select(m => m.Is_Analytics).FirstOrDefault();

                globalData.plantName = plantName;
                globalData.pageTitle = "CBM Dashboard" + " - " + machineName;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                ViewBag.IsAnalytics = IsAnalytics;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.MachineList = null;
                ViewBag.Machine_ID = Convert.ToInt32(id);
                List<MM_MT_MTTUW_Machines> stationMachineList = db.MM_MT_MTTUW_Machines.Where(a => a.Machine_ID == id && a.IsActive == true).ToList();

                var mname = stationMachineList.First().Machine_Name;
                ViewBag.MachineName = mname;
                List<MM_CBM_Group> CBMGroupList = db.MM_CBM_Group.Where(a => a.Machine_ID == id && a.MM_MT_MTTUW_Machines.IsActive == true).ToList();
                if (stationMachineList.Count() > 0)
                {
                    ViewBag.ShopTitle = stationMachineList.First().Machine_Name + " CBM Dashboard";
                    ViewBag.MachineList = stationMachineList;
                    ViewData["CBMGroupList"] = CBMGroupList;
                    ViewBag.CBMGroupList = CBMGroupList;
                    var machineIDList = stationMachineList.Select(a => a.Machine_ID).Distinct();

                    IEnumerable<MM_MT_Conditional_Based_Maintenance> cbmList = db.MM_MT_Conditional_Based_Maintenance.Where(a => machineIDList.Contains(a.Machine_ID) && a.CBM_Check_Type != "Disable" && a.IsActive == true).OrderBy(m => m.Sequence_No);

                    ViewBag.CBMCount = cbmList.Count();
                    return View(cbmList);
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
            catch (Exception exp)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = "Exception Occurred in Server!";
                globalData.messageDetail = exp.Message;
                TempData["globalData"] = globalData;
                generalHelper.addControllerException(exp, "CBMDashboardController", "ShowMachineCBM(machineId : " + id + ")", ((FDSession)this.Session["FDSession"]).userId);
                return View();
            }
        }

        public ActionResult demo()
        {
            return View();
        }

        public ActionResult getCBMID(decimal machineId)
        {
            var st = (from cbm in db.MM_MT_Conditional_Based_Maintenance
                      where cbm.Machine_ID == machineId && cbm.IsActive == true
                      select new
                      {
                          Id = cbm.CBM_ID,
                          Value = cbm.Machine_Parameter
                      }
                    ).ToList();
            //var st = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.Machine_ID == machineId && m.IsActive == true).ToList();
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getCBMGroupDetails(int machineId)
        {
            try
            {
                var Groups = db.MM_CBM_Group.Where(m => m.Machine_ID == machineId).Select(m => m.CBM_Group_ID).ToList();
                foreach (var group in Groups)
                {
                    MM_CBM_Group obj1 = db.MM_CBM_Group.Find(group);
                    obj1.Is_Yellow = false;
                    obj1.Is_Red = false;
                    obj1.Is_Green = false;
                    db.Entry(obj1).State = EntityState.Modified;
                    db.SaveChanges();
                    var CBMIds = db.MM_CBM_Group_Mapping.Where(m => m.CBM_Group_ID == group).Select(m => m.CBM_ID).ToList();
                    var GroupColor = "";
                    var color = "";
                    foreach (var CBMID in CBMIds)
                    {
                        var CBMColor = "";
                        var CBMData = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.CBM_ID == CBMID).FirstOrDefault();
                        var conversionFactor = CBMData.Is_Conversion_Factor;
                        decimal currentValue = 0;
                        var denominator = Convert.ToDecimal(CBMData.Scale_Denominator);
                        if (conversionFactor == 1 || conversionFactor == null)
                        {
                            currentValue = Convert.ToDecimal(CBMData.Current_Value) / denominator;
                        }
                        else
                            currentValue = Convert.ToDecimal(CBMData.Current_Value) * denominator;
                        var greenMax = CBMData.Green_Max_Val;
                        var greenMin = CBMData.Green_Min_Val;
                        var warningMax = CBMData.Warning_Max_Val;
                        var warningMin = CBMData.Warning_Min_Val;

                        if (currentValue <= greenMax && currentValue >= greenMin)
                        {
                            if (color == "")
                            {
                                color = "Green";
                            }
                            CBMColor = "Green";
                        }
                        else if (currentValue > greenMax)
                        {
                            if (currentValue > warningMax)
                            {
                                color = "Red";
                                CBMColor = "Red";
                            }
                            else
                            {
                                if (color == "" || color != "Red")
                                {
                                    color = "Yellow";
                                }
                                CBMColor = "Yellow";
                            }
                        }
                        else if (currentValue < greenMin)
                        {
                            if (currentValue < warningMin)
                            {
                                color = "Red";
                                CBMColor = "Red";
                            }
                            else
                            {
                                if (color == "" || color != "Red")
                                    color = "Yellow";
                                CBMColor = "Yellow";
                            }
                        }

                        var CGroupID = db.MM_CBM_Group_Mapping.Where(m => m.CBM_ID == CBMID && m.Machine_ID == machineId).Select(m => m.Group_ID).FirstOrDefault();
                        MM_CBM_Group_Mapping cgm = db.MM_CBM_Group_Mapping.Find(CGroupID);
                        cgm.ParameterColor = CBMColor;
                        db.Entry(cgm).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    GroupColor = color;
                    MM_CBM_Group obj = db.MM_CBM_Group.Find(group);
                    if (GroupColor == "Red")
                    {
                        obj.Is_Red = true;
                        obj.Is_Green = false;
                        obj.Is_Yellow = false;
                    }
                    else if (GroupColor == "Yellow")
                    {
                        if (obj.Is_Red == false)
                        {
                            obj.Is_Yellow = true;
                            obj.Is_Green = false;
                        }
                        if (obj.Is_Red == false && (obj.Is_Green == true || obj.Is_Green == false || obj.Is_Yellow == true || obj.Is_Yellow == false))
                        {
                            obj.Is_Yellow = true;
                            obj.Is_Green = false;
                        }
                    }
                    else if (GroupColor == "Green")
                    {
                        if (obj.Is_Red == false && obj.Is_Yellow == false)
                        {
                            obj.Is_Green = true;
                        }
                    }

                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();

                }
                List<string> paramDetails = new List<string>();
                var groupList = db.MM_CBM_Group.Where(m => m.Machine_ID == machineId).ToList();
                foreach (var g in groupList)
                {
                    MM_MT_Conditional_Based_Maintenance cbmobj = new MM_MT_Conditional_Based_Maintenance();
                    if (g.Is_Red == true || g.Is_Yellow == true)
                    {
                        var ID = g.CBM_Group_ID;
                        var CBMID = db.MM_CBM_Group_Mapping.Where(m => m.CBM_Group_ID == ID && (m.ParameterColor == "Red" || m.ParameterColor == "Yellow")).Select(m => m.CBM_ID).FirstOrDefault();
                        cbmobj = db.MM_MT_Conditional_Based_Maintenance.Find(CBMID);
                        paramDetails.Add(cbmobj.Machine_Parameter + " - " + cbmobj.Current_Value + " " + cbmobj.UOM);
                    }
                    else
                    {
                        paramDetails.Add("");
                    }
                }
                var CBMGroupList = groupList.Select(m => new { m.Machine_ID, m.CBM_Group_ID, m.Is_Green, m.Is_Red, m.Is_Yellow });
                return Json(new { List = CBMGroupList, PList = paramDetails }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "CBMDashboardController", "getCBMGroupDetails(machineId : " + machineId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }

        public ActionResult LiveCBMData(int cbmid)
        {
            try
            {

                List<string> allTimes = new List<string>();
                List<double> CBMdata = new List<double>();
                DateTime FromDate = DateTime.Now.AddDays(-3);
                DateTime ToDate = DateTime.Now;
                var obj = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.CBM_ID == cbmid).FirstOrDefault();
                var Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > FromDate && m.Inserted_Date < ToDate).ToList();
                if (Parameter_Data.Count > 0)
                {
                    foreach (var item in Parameter_Data)
                    {
                        //var date = item.Inserted_Date.ToString().Split(' ')[1];
                        DateTime date1 = Convert.ToDateTime(item.Inserted_Date);
                        //var date = date1.ToString("dd/MM/yy h:mm:ss");
                        allTimes.Add(date1.ToString());
                        var value = item.Parameter_Value / Convert.ToDouble(obj.Scale_Denominator);
                        if (value > 3000)
                            continue;
                        CBMdata.Add(Convert.ToDouble(value));
                    }
                }
                //var unit = obj.UOM;
                //var paramName = obj.Machine_Parameter;
                //return Json(new { allTimes, CBMdata,unit,paramName }, JsonRequestBehavior.AllowGet);
                var unit = obj.UOM;
                var paramName = obj.Machine_Parameter;
                var JSonResult = Json(new { allTimes, CBMdata, unit, paramName }, JsonRequestBehavior.AllowGet);
                JSonResult.MaxJsonLength = Int32.MaxValue;
                return JSonResult;
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult GetCBMAnalyticsParameterHealth(int machineId)
        {
            var CBMList = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.Machine_ID == machineId && m.IsActive == true).Select(m => m.CBM_ID).ToList();
            List<decimal> CBMID = new List<decimal>();
            List<string> ParameterHealth = new List<string>();
            List<Sp_Parameter_Health_Result> status = db.Sp_Parameter_Health(machineId).ToList();


            foreach (var item in status)
            {
                CBMID.Add(Convert.ToDecimal(item.CBMID));
                if (item.Parameter_Health != null)
                {
                    ParameterHealth.Add(item.Parameter_Health);
                }
                else
                {
                    ParameterHealth.Add(null);
                }
            }
            //foreach (var item in CBMList)
            //{
            //    // Chakan_MVML

            //    //var status = da.Analytics_Data.Where(m => m.CBM_ID == item).OrderByDescending(m => m.Row_Id).Select(m => m.Parameter_health).FirstOrDefault();

            //    DateTime todate = System.DateTime.Now;
            //    var date = todate.Date;

            //    //Nashik_Tool_Die
            //    var status = (from h in db.Analytics_Data_TD
            //                  where h.CBM_ID == item && h.Parameter_health != null && h.Inserted_datetime >= date
            //                  select new
            //                  {
            //                      h.Parameter_health
            //                  }).FirstOrDefault();

            //    //var status = db.Analytics_Data_TD.Where(m => m.CBM_ID == item).OrderByDescending(m => m.Row_Id).Select(m => m.Parameter_health).FirstOrDefault();
            //    CBMID.Add(item);
            //    if(status!=null)
            //    {
            //        ParameterHealth.Add(status.Parameter_health);
            //    }
            //    else
            //    {
            //        ParameterHealth.Add(null);
            //    }

            //}

            var jsonresult = Json(new { CBMID, ParameterHealth }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = Int32.MaxValue;
            return jsonresult;
        }

        public ActionResult GetCBMAnalyticsData(int machineId, string fromDate, string toDate)
        {
            List<List<string>> allTimes = new List<List<string>>();
            List<List<double>> CBMdata = new List<List<double>>();
            List<List<double>> Predicteddata = new List<List<double>>();
            List<List<double>> MaxValue = new List<List<double>>();
            List<List<double>> MinValue = new List<List<double>>();
            List<string> paramName = new List<string>();
            List<decimal> CBMID = new List<decimal>();
            List<string> unit = new List<string>();
            List<string> Times = new List<string>();
            DateTime ToDate = Convert.ToDateTime(toDate);
            DateTime StartDate = Convert.ToDateTime(fromDate);
            //for (DateTime date1 = StartDate, date2 = StartDate.AddSeconds(1); date2 <= ToDate; date1 = date2, date2 = date2.AddSeconds(1))
            //{
            //    string ddl = Convert.ToDateTime(date1).ToString("HH:mm");
            //    Times.Add(ddl);
            //    //allDates.Add(ddl);

            //}
            var CBMList = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.Machine_ID == machineId && m.IsActive == true).Select(m => m.CBM_ID).ToList();
            List<Sp_Analytics_Parameter_Data_Result> Parameter_Data = db.Sp_Analytics_Parameter_Data(Convert.ToInt32(machineId), StartDate, ToDate).ToList();
            foreach (var item in CBMList)
            {
                List<double> Analytics = new List<double>();

                Times = new List<string>();
                List<double> maxVal = new List<double>();
                List<double> minVal = new List<double>();
                var obj = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.CBM_ID == item).FirstOrDefault();
                if (Parameter_Data.Where(s => s.CBMID == item).Count() > 0)
                {
                    foreach (var item1 in Parameter_Data.Where(s => s.CBMID == item).OrderBy(s => s.InsertedDate))
                    {
                        var value = item1.ParameterValue;
                        var DynamicMaxValue = item1.MaxValue;
                        var DynamicMinValue = item1.MinValue;

                        value = Math.Round(Convert.ToDouble(value), 2);
                        DynamicMaxValue = Math.Round(Convert.ToDouble(DynamicMaxValue), 2);
                        DynamicMinValue = Math.Round(Convert.ToDouble(DynamicMinValue), 2);

                        DateTime date1 = Convert.ToDateTime(item1.InsertedDate);
                        Times.Add(date1.ToString());

                        Analytics.Add(Convert.ToDouble(value));
                        maxVal.Add(Convert.ToDouble(DynamicMaxValue));
                        minVal.Add(Convert.ToDouble(DynamicMinValue));
                    }
                }



                MaxValue.Add(maxVal);
                MinValue.Add(minVal);
                CBMID.Add(item);
                CBMdata.Add(Analytics);
                allTimes.Add(Times);
                paramName.Add(obj.Machine_Parameter);
                unit.Add(obj.UOM);
            }
            var jsonresult = Json(new { allTimes, CBMdata, MaxValue, MinValue, paramName, CBMID, unit }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = Int32.MaxValue;
            return jsonresult;
        }

        public ActionResult GetAnalyticsData(int cbmid, string fromDate, string toDate)
        {
            try
            {
                List<string> allTimes = new List<string>();
                List<double> CBMdata = new List<double>();
                List<double> MaxValue = new List<double>();
                List<double> MinValue = new List<double>();
                DateTime StartDate = Convert.ToDateTime(fromDate);
                DateTime ToDate = Convert.ToDateTime(toDate);

                var obj = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.CBM_ID == cbmid).FirstOrDefault();

                var Parameter_Data = new List<Analytics_Data>();
                if (obj.Parameter_Category == "Temperature")
                    Parameter_Data = db.Analytics_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_datetime > StartDate && m.Inserted_datetime < ToDate && m.Parameter_Value > 0).OrderBy(m => m.Inserted_datetime).ToList();
                else
                    Parameter_Data = db.Analytics_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_datetime > StartDate && m.Inserted_datetime < ToDate).OrderBy(m => m.Inserted_datetime).ToList();
                if (Parameter_Data.Count > 0)
                {
                    foreach (var item in Parameter_Data)
                    {
                        DateTime date1 = Convert.ToDateTime(item.Inserted_datetime);
                        allTimes.Add(date1.ToString());

                        CBMdata.Add(Convert.ToDouble(item.Parameter_Value));
                        MaxValue.Add(Convert.ToDouble(item.Max_Dynamic_Limit));
                        MinValue.Add(Convert.ToDouble(item.Min_Dynamic_Limit));
                    }
                }

                var paramName = obj.Machine_Parameter;
                var jsonresult = Json(new { allTimes, CBMdata, MaxValue, MinValue, paramName }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult CustomCBMData(decimal cbmid, string fromDate, string toDate, string HData)
        {
            var machineid = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.CBM_ID == cbmid).Select(m => m.Machine_ID).FirstOrDefault();
            try
            {
                var Date = DateTime.Now;
                DateTime ToDate = Convert.ToDateTime(toDate);
                var time = TimeSpan.Parse("00:00:00.000");
                //var ToString = ToDate.ToString().Split(' ')[1];
                DateTime StartDate = Convert.ToDateTime(fromDate);
                if (fromDate == "" && toDate == "" && HData == "ExceptionalData")
                {
                    StartDate = Date.AddDays(-3);
                    ToDate = Date;
                }

                else if (fromDate == "" && toDate == "" && HData == "AllData")
                {
                    StartDate = Date.AddDays(-3);
                    ToDate = Date;
                }

                else
                {
                    StartDate = Convert.ToDateTime(fromDate);
                    ToDate = Convert.ToDateTime(toDate);
                }


                //if (fromDate != "")
                //    StartDate = (StartDate + time);
                //if (toDate != "")
                //    ToDate = (ToDate + time);
                List<string> allTimes = new List<string>();
                List<double> CBMdata = new List<double>();
                List<double> MaxValue = new List<double>();
                List<double> MinValue = new List<double>();
                List<double> GMinVal = new List<double>();
                List<double> GMaxVal = new List<double>();
                List<double> YMinVal = new List<double>();
                List<double> YMaxVal = new List<double>();


                var datediff = System.Math.Round((ToDate - StartDate).TotalDays, 0);
                var obj = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.CBM_ID == cbmid).FirstOrDefault();

                //var Parameter_Data = new List<MM_Ctrl_CBM_Data>();
                //if (obj.Parameter_Category == "Temperature")
                //    Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > StartDate && m.Inserted_Date < ToDate && m.Parameter_Value > 0).OrderBy(m => m.Inserted_Date).ToList();
                //else
                //    Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > StartDate && m.Inserted_Date < ToDate).OrderBy(m => m.Inserted_Date).ToList();

                //List<Sp_Parameter_Graph_Data_Result> Parameter_Data = db.Sp_Parameter_Graph_Data(Convert.ToInt32(cbmid), StartDate, ToDate).ToList();

               List<Sp_CBM_Graph_Data_Result> Parameter_Data = db.Sp_CBM_Graph_Data(Convert.ToInt32(cbmid), StartDate, ToDate).ToList();
               // var Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > StartDate && m.Inserted_Date < ToDate).OrderBy(m => m.Inserted_Date).ToList();
                var minVal = Convert.ToDouble(obj.Green_Min_Val * obj.Scale_Denominator);
                var maxVal = Convert.ToDouble(obj.Green_Max_Val * obj.Scale_Denominator);
                if (obj.Is_Conversion_Factor == 2)
                {
                    minVal = Convert.ToDouble(obj.Green_Min_Val / obj.Scale_Denominator);
                    maxVal = Convert.ToDouble(obj.Green_Max_Val / obj.Scale_Denominator);
                }
                //if(HData == "ExceptionalData")
                //{
                //    Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > StartDate && m.Inserted_Date < ToDate && (m.Parameter_Value < minVal || m.Parameter_Value > maxVal) ).ToList();
                //}
                if (Parameter_Data.Count > 0)
                {
                    foreach (var item in Parameter_Data)
                    {
                        //var date = item.Inserted_Date.ToString().Split(' ')[1];
                        DateTime date1 = Convert.ToDateTime(item.Inserted_Date);
                        // var date = date1.ToString("dd/MM/yy h:mm:ss");
                        allTimes.Add(date1.ToString());
                        //var value = obj.Is_Conversion_Factor == 2 ? item.Parameter_Value * Convert.ToDouble(obj.Scale_Denominator) : item.Parameter_Value / Convert.ToDouble(obj.Scale_Denominator);
                        //if(value > 3000)
                        //{
                        //    continue;
                        //}
                        var value = Math.Round(Convert.ToDouble(item.Parameter_Value), 2);
                        CBMdata.Add(Convert.ToDouble(value));
                        MaxValue.Add(Convert.ToDouble(obj.Max_Val));
                        MinValue.Add(Convert.ToDouble(obj.Min_Val));
                        GMinVal.Add(Convert.ToDouble(obj.Green_Min_Val));
                        GMaxVal.Add(Convert.ToDouble(obj.Green_Max_Val));
                        YMinVal.Add(Convert.ToDouble(obj.Warning_Min_Val));
                        YMaxVal.Add(Convert.ToDouble(obj.Warning_Max_Val));
                    }
                }
                var unit = obj.UOM;
                var paramName = obj.Machine_Parameter;
                //return Json(new { allTimes, CBMdata, unit, paramName, datediff }, JsonRequestBehavior.AllowGet);
                var jsonresult = Json(new { allTimes, CBMdata, MaxValue, MinValue, GMaxVal, GMinVal, YMaxVal, YMinVal, unit, paramName, datediff }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;
            }
            catch(Exception ex)
            {
                return RedirectToAction("ShowMachineCBM("+machineid+")");
            }
        }

        public ActionResult ExportData(int cbmid, string fromDate, string todate, string HData)
        {
            try
            {
                DateTime FromDate = new DateTime();
                //DateTime StartDate = new DateTime();
                DateTime ToDate = new DateTime();
                if (fromDate == "" || todate == "")
                {
                    FromDate = DateTime.Now.AddMinutes(-10);
                    ToDate = DateTime.Now;

                }
                else
                {
                    var time = TimeSpan.Parse("00:00:00.000");
                    ToDate = Convert.ToDateTime(todate);
                    //var ToString = ToDate.ToString().Split(' ')[1];
                    FromDate = Convert.ToDateTime(fromDate);
                    //var time = TimeSpan.Parse(ToString);
                    // StartDate = (StartDate + time);
                    //ToDate = (ToDate + time);
                }

                List<string> allTimes = new List<string>();
                List<double> CBMdata = new List<double>();
                List<ExcelData> exceldata = new List<ExcelData>();


                var obj = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.CBM_ID == cbmid).FirstOrDefault();

                //var Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > FromDate && m.Inserted_Date < ToDate).ToList();
                // var datediff = System.Math.Round((ToDate - StartDate).TotalDays, 0);

                List<Sp_CBM_Graph_Data_Result> Parameter_Data = db.Sp_CBM_Graph_Data(Convert.ToInt32(cbmid), FromDate, ToDate).ToList();
                //var Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > StartDate && m.Inserted_Date < ToDate).ToList();
                var minVal = Convert.ToDouble(obj.Green_Min_Val * obj.Scale_Denominator);
                var maxVal = Convert.ToDouble(obj.Green_Max_Val * obj.Scale_Denominator);
                if (obj.Is_Conversion_Factor == 2)
                {
                    minVal = Convert.ToDouble(obj.Green_Min_Val / obj.Scale_Denominator);
                    maxVal = Convert.ToDouble(obj.Green_Max_Val / obj.Scale_Denominator);
                }
                //if (HData == "ExceptionalData")
                //{
                //    Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.CBM_ID == cbmid && m.Inserted_Date > FromDate && m.Inserted_Date < ToDate && (m.Parameter_Value < minVal || m.Parameter_Value > maxVal)).ToList();
                //}
                if (Parameter_Data.Count > 0)
                {
                    foreach (var item in Parameter_Data)
                    {
                        //var date = item.Inserted_Date.ToString().Split(' ')[1];
                        DateTime date1 = Convert.ToDateTime(item.Inserted_Date);
                        var date = date1.ToString("dd/MM/yy h:mm:ss");
                        double data = obj.Is_Conversion_Factor == 2 ? Convert.ToDouble(Convert.ToDouble(item.Parameter_Value) * Convert.ToDouble(obj.Scale_Denominator)) : Convert.ToDouble(Convert.ToDouble(item.Parameter_Value) / Convert.ToDouble(obj.Scale_Denominator));
                        ExcelData export = new ExcelData(date1.ToString(), data);
                        exceldata.Add(export);
                        //allTimes.Add(date);
                        //CBMdata.Add(Convert.ToDouble(Convert.ToDouble(item.Parameter_Value) / Convert.ToDouble(obj.Scale_Denominator)));

                    }
                }
                var unit = obj.UOM;
                var paramName = obj.Machine_Parameter;
                //return Json(new { allTimes, CBMdata, unit, paramName, datediff }, JsonRequestBehavior.AllowGet);
                //SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                //DataTable dt1 = new DataTable("Shop Consumption");
                //da1.Fill(dt1);
                //con.Close();
                //ds.Tables.Add(dt1);
                DataTable da = new DataTable();
                //DataSet ds = new DataSet();
                //ds.Tables.Add(da);
                //System.Data.DataTable dt = new System.Data.DataTable();
                //    DataColumn dc = new DataColumn("id");
                //    dt.Columns.Add(dc);
                //    dc = new DataColumn("name");
                //    dt.Columns.Add(dc)
                //    dt.Rows.Add("1", "a");
                //    dt.Rows.Add("2", "b");
                //    dt.Rows.Add("3", "c");
                da.Columns.Add("Sr.No");
                da.Columns.Add("Name");
                da.Columns.Add("Date");
                da.Columns.Add("CBMData");
                da.Columns.Add("Unit");
                int j = 1;
                for (int i = 0; i < exceldata.Count(); i++)
                {
                    int sr = j;
                    var name = paramName;
                    var datetime = exceldata[i].datetime;
                    var data = exceldata[i].cmbdata;
                    var DataUnit = unit;
                    da.Rows.Add(sr, name, datetime, data, DataUnit);
                    j++;
                }
                var workbook = new XLWorkbook();
                var ws = workbook.Worksheets.Add("CBM Data");
                ws.Cell(1, 1).Value = "Sr.No";
                ws.Cell(1, 2).Value = "Name";
                ws.Cell(1, 3).Value = "Date Time";
                ws.Cell(1, 4).Value = "CBMData";
                ws.Cell(1, 5).Value = "Unit";
                ws.Cell(2, 1).InsertData(da.AsEnumerable());
                ws.Columns(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Columns(3, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(1).Style.Font.Bold = true;
                ws.Cells("A1:E1").Style.Fill.BackgroundColor = XLColor.LightBlue;
                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CMBData.xlsx");
                }
                //using (XLWorkbook wb = new XLWorkbook())
                //{
                //    wb.Worksheets.Add(ds);
                //    wb.Style.Font.Bold = true;
                //    wb.Style.Font.FontColor = XLColor.Red;
                //    wb.Style.Fill.BackgroundColor = XLColor.Cyan;


                //    using (MemoryStream stream = new MemoryStream())
                //    {
                //        wb.SaveAs(stream);
                //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CMBData.xlsx");
                //    }
                //}

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult GroupData(decimal machineid, string toDate, string fromDate, decimal GroupId)
        {
            var cbmIds = db.MM_CBM_Group_Mapping.Where(m => m.Machine_ID == machineid && m.CBM_Group_ID == GroupId).Select(m => m.CBM_ID).ToList();


            DateTime FromDate = Convert.ToDateTime(fromDate);
            DateTime ToDate = Convert.ToDateTime(toDate);
            List<DateTime> Times = new List<DateTime>();
            List<DateTime> Times1 = new List<DateTime>();
            List<DateTime> Times2 = new List<DateTime>();
            List<string> allTimes = new List<string>();
            List<string> allTimes1 = new List<string>();
            List<string> allTimes2 = new List<string>();
            List<decimal?> CBMdata = new List<decimal?>();
            List<decimal?> CBMdata1 = new List<decimal?>();
            List<decimal?> CBMdata2 = new List<decimal?>();
            List<decimal?> CBMdata_Max = new List<decimal?>();
            List<decimal?> CBMdata_Min = new List<decimal?>();
            // IList<double?> CBMdata = listn
            List<List<decimal?>> CBMdatas = new List<List<decimal?>>();
            List<List<decimal?>> CBMdatas1 = new List<List<decimal?>>();
            List<List<decimal?>> CBMdatas2 = new List<List<decimal?>>();
            List<List<decimal?>> CBMdatasMax = new List<List<decimal?>>();
            List<List<decimal?>> CBMdatasMin = new List<List<decimal?>>();
            //var StartDate = 

            List<string> units = new List<string>();
            List<string> units1 = new List<string>();
            List<string> paramNames = new List<string>();
            List<string> paramNames1 = new List<string>();
            var datediff = 10;
            //var Satardate = ToDate;
            //var Enddate = ToDate.AddMinutes(10);
            var StartDate = FromDate;
            var Enddate = ToDate;
            for (DateTime date = Convert.ToDateTime(FromDate); date <= Convert.ToDateTime(ToDate); date = date.AddSeconds(1))
            {
                var date12 = Convert.ToDateTime(date);
                Times.Add(date12);
                var date1 = date12.ToString("dd/MMM HH:mm:ss");
                allTimes.Add(date1);
            }
            for (DateTime date = Convert.ToDateTime(StartDate); date <= Convert.ToDateTime(Enddate); date = date.AddSeconds(1))
            {
                var date12 = Convert.ToDateTime(date);
                Times1.Add(date12);
                var date1 = date12.ToString("dd/MMM HH:mm:ss");
                allTimes1.Add(date1);
            }

            var startdate = ToDate;
            var Enddate1 = ToDate.AddMinutes(10);
            for (DateTime date = Convert.ToDateTime(startdate); date <= Convert.ToDateTime(Enddate1); date = date.AddSeconds(1))
            {
                var date12 = Convert.ToDateTime(date);
                Times2.Add(date12);
                var date1 = date12.ToString("dd/MMM HH:mm:ss");
                allTimes2.Add(date1);
            }

            //foreach(var secondtimne in Times)
            var storeddata = 0;
            //var Parameter_Data = db.MM_Ctrl_CBM_Data.Where(m => m.Inserted_Date > FromDate && m.Inserted_Date < ToDate).Distinct().FirstOrDefault();
            List<List<Parameterview>> CBMView = new List<List<Parameterview>>();

            List<Sp_Analytics_Groupwise_Parameter_Data_Result> Parameter_Data1 = db.Sp_Analytics_Groupwise_Parameter_Data(Convert.ToInt32(machineid), Convert.ToInt32(GroupId), StartDate, Enddate).ToList();
            List<Sp_Analytics_Groupwise_Predict_Data_Result> Predict_Data = db.Sp_Analytics_Groupwise_Predict_Data(Convert.ToInt32(machineid), Convert.ToInt32(GroupId), StartDate, Enddate).ToList();
            List<Sp_Analytics_Groupwise_Predict_Data_Result> Predict_Data1 = db.Sp_Analytics_Groupwise_Predict_Data(Convert.ToInt32(machineid), Convert.ToInt32(GroupId), startdate, Enddate1).ToList();
            // Parameter_Data = null;
            foreach (var cbmId in cbmIds)
            {
                List<Parameterview> view = new List<Parameterview>();
                CBMdata = new List<decimal?>();
                CBMdata2 = new List<decimal?>();
                CBMdata1 = new List<decimal?>();
                CBMdata_Max = new List<decimal?>();
                var obj = db.MM_MT_Conditional_Based_Maintenance.Where(m => m.CBM_ID == cbmId).FirstOrDefault();
                var minVal = Convert.ToDouble(obj.Green_Min_Val * obj.Scale_Denominator);
                var maxVal = Convert.ToDouble(obj.Green_Max_Val * obj.Scale_Denominator);

                //foreach (var paraview in Parameter_Data1)
                //{
                //    Parameterview viewobj = new Parameterview(Convert.ToDateTime(paraview.InsertedDate), Convert.ToDouble(paraview.ParameterValue));
                //    view.Add(viewobj);
                //}

                foreach (var secondtimne in Times)
                {

                    //storeddata = 0;
                    if (Parameter_Data1.Where(n => n.CBMID == cbmId).Count() > 0)
                    {


                        var ParameterValue = Parameter_Data1.Where(n => n.CBMID == cbmId && n.InsertedDate == secondtimne).Select(s => s.ParameterValue).FirstOrDefault();
                        if (ParameterValue != null)
                        {


                            storeddata = 0;
                            var value = obj.Is_Conversion_Factor == 2 ? ParameterValue.Value * Convert.ToDouble(obj.Scale_Denominator) : ParameterValue.Value / Convert.ToDouble(obj.Scale_Denominator);
                            if (value > 3000)
                            {
                                continue;
                            }
                            storeddata = Convert.ToInt32(value);
                            CBMdata.Add(Convert.ToInt32(value));
                        }
                        else
                        {
                            CBMdata.Add(null);
                        }


                    }
                    else
                    {


                    }
                    if (Predict_Data.Where(n => n.CBMID == cbmId).Count() > 0)
                    {

                        var ParameterValue = Predict_Data.Where(n => n.CBMID == cbmId && n.InsertedDate == secondtimne).Select(s => s.ParameterValue).FirstOrDefault();
                        if (ParameterValue != null)
                        {


                            storeddata = 0;
                            var value = obj.Is_Conversion_Factor == 2 ? ParameterValue.Value * Convert.ToDouble(obj.Scale_Denominator) : ParameterValue.Value / Convert.ToDouble(obj.Scale_Denominator);
                            if (value > 3000)
                            {
                                continue;
                            }
                            storeddata = Convert.ToInt32(value);
                            CBMdata1.Add(Convert.ToInt32(value));
                        }
                        else
                        {
                            CBMdata1.Add(null);
                        }

                    }
                    else
                    {

                    }

                }

                foreach (var future in Times2)
                {
                    if (Predict_Data1.Where(n => n.CBMID == cbmId).Count() > 0)
                    {

                        var ParameterValue = Predict_Data.Where(n => n.CBMID == cbmId && n.InsertedDate == future).Select(s => s.ParameterValue).FirstOrDefault();
                        if (ParameterValue != null)
                        {


                            storeddata = 0;
                            var value = obj.Is_Conversion_Factor == 2 ? ParameterValue.Value * Convert.ToDouble(obj.Scale_Denominator) : ParameterValue.Value / Convert.ToDouble(obj.Scale_Denominator);
                            if (value > 3000)
                            {
                                continue;
                            }
                            storeddata = Convert.ToInt32(value);
                            CBMdata2.Add(Convert.ToInt32(value));
                        }
                        else
                        {
                            CBMdata2.Add(null);
                        }

                    }
                    else
                    {
                        break;

                    }
                }
                //storeddata = 0;
                //if (Parameter_Data1.Where(n=>n.CBMID ==cbmId).Count()> 0)
                //{
                //    CBMdata = new List<decimal?>();
                //    CBMdata_Max = new List<decimal?>();
                //    CBMdata_Min = new List<decimal?>();
                //    //Parameter_Data1 = Parameter_Data1.Distinct().to;
                //    foreach (var item in Parameter_Data1.Where(n => n.CBMID == cbmId).OrderBy(n=>n.InsertedDate))
                //    {
                //        storeddata = 0;
                //        //var value = obj.Is_Conversion_Factor == 2 ? item.Parameter_Value * Convert.ToDouble(obj.Scale_Denominator) : item.Parameter_Value / Convert.ToDouble(obj.Scale_Denominator);

                //        //if (value > 3000)
                //        //{
                //        //    continue;
                //        //}
                //        var value = item.ParameterValue;
                //        storeddata = Convert.ToInt32(value);
                //        CBMdata.Add(Math.Round(Convert.ToDecimal(value), 2));
                //        //CBMdata_Max.Add(Math.Round(Convert.ToDecimal(item.Max_Dynamic_Limit),2));
                //        //CBMdata_Min.Add(Math.Round(Convert.ToDecimal(item.Min_Dynamic_Limit),2));

                //    }

                //}
                //else
                //{
                //    CBMdata.Add(Convert.ToDecimal(0));
                //    //CBMdata_Max.Add(Convert.ToDecimal(0));
                //    //CBMdata_Min.Add(Convert.ToDecimal(0));
                //}


                //if (Predict_Data.Where(n => n.CBMID == cbmId).Count() > 0)
                //{
                //    CBMdata1 = new List<decimal?>();
                //    //CBMdata = new List<int?>();
                //    //Parameter_Data1 = Parameter_Data1.Distinct().to;
                //    foreach (var item in Predict_Data.Where(n => n.CBMID == cbmId).OrderBy(n => n.InsertedDate))
                //    {

                //        storeddata = 0;
                //        //var value = obj.Is_Conversion_Factor == 2 ? item.Predicted_value * Convert.ToDouble(obj.Scale_Denominator) : item.Predicted_value / Convert.ToDouble(obj.Scale_Denominator);
                //        //if (value > 3000)
                //        //{
                //        //    continue;
                //        //}
                //        var value = item.ParameterValue;
                //        storeddata = Convert.ToInt32(value);
                //        CBMdata1.Add(Math.Round(Convert.ToDecimal(value), 2));
                //        //CBMdata.Add(Convert.ToInt32(value));
                //        //    break;
                //        //}

                //    }

                //}
                //else
                //{
                //    CBMdata1.Add(Convert.ToDecimal(0));
                //    //CBMdata.Add(Convert.ToInt32(0));
                //}
                ////}
                //if (Predict_Data1.Where(n => n.CBMID == cbmId).Count() > 0)
                //{
                //    CBMdata2 = new List<decimal?>();
                //    //CBMdata = new List<int?>();
                //    //Parameter_Data1 = Parameter_Data1.Distinct().to;
                //    foreach (var item in Predict_Data1.Where(n => n.CBMID == cbmId).OrderBy(n => n.InsertedDate))
                //    {

                //        //i = i + 1;
                //        //if (secondtimne == item.Parameter_datetime)
                //        //{
                //        storeddata = 0;
                //        //var value = obj.Is_Conversion_Factor == 2 ? item.Predicted_value * Convert.ToDouble(obj.Scale_Denominator) : item.Predicted_value / Convert.ToDouble(obj.Scale_Denominator);
                //        //if (value > 3000)
                //        //{
                //        //    continue;
                //        //}
                //        var value = item.ParameterValue;
                //        storeddata = Convert.ToInt32(value);
                //        CBMdata2.Add(Math.Round(Convert.ToDecimal(value), 2));
                //        //CBMdata.Add(Convert.ToInt32(value));
                //        //    break;
                //        //}

                //    }

                //}
                //else
                //{
                //    CBMdata2.Add(0);

                //}

                CBMdatas1.Add(CBMdata1);
                CBMdatas.Add(CBMdata);
                CBMdatas2.Add(CBMdata2);
                //CBMView.Add(view);
                //CBMdatasMax.Add(CBMdata_Max);
                //CBMdatasMin.Add(CBMdata_Min);
                //CBMdatas.Add(CBMdata1);
                //allTimes.Add(Times);
                units.Add(obj.UOM);
                //units.Add(obj.UOM);
                paramNames.Add(obj.Machine_Parameter);


                //var perdict = "Prediction_" + obj.Machine_Parameter;
                //paramNames.Add(perdict);
            }

            var jsonresult = Json(new { allTimes, CBMdatas, units, paramNames, datediff, allTimes1, CBMdatas1, allTimes2, CBMdatas2 }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = Int32.MaxValue;
            return jsonresult;
        }
    }
}