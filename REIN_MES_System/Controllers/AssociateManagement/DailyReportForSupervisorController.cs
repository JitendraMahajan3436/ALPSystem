using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Globalization;
using System.IO;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using ClosedXML.Excel;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class DailyReportForSupervisorController : BaseController
    {
        //  private REIN_SOLUTIONEntities  db = new REIN_SOLUTIONEntities ();
        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        int plantId = 0, shopId = 0, lineId = 0, subShopId = 0, subLineId = 0, subStationId = 0, marriageShopId = 0, marriageLineId = 0, marriageStationId = 0;
        bool isStartStationAdded, isEndStationAdded;
        int startStationId = 0, endStationId = 0;

        String routeConfiguration = "";
        String routeConfigurationDisplay = "";
        GlobalData globalData = new GlobalData();
        RS_Route_Configurations routeConfigurationObj = new RS_Route_Configurations();
        RS_Route_Display routeDisplayObj = new RS_Route_Display();
        FDSession fdSession = new FDSession();

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Index
        // Input Parameter      : None
        // Return Type          : ActionResult
        // Author & Time Stamp  : Ajay Wagh
        // Description          : Action used to show the line, allow the user to configure line
        //////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // GET: /RouteConfiguration/
        public ActionResult Index()
        {
            try
            {
                int plant_Id = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                //  ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_Id), "Shop_ID", "Shop_Name", 0);
                ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == 0).ToList(), "Line_ID", "Line_Name");
                ViewBag.Setup_ID = new SelectList(db.RS_Setup, "Setup_ID", "Setup_Name");
                ViewBag.New_Line_ID = new SelectList(db.RS_Lines.Where(m => m.Shop_ID == shopId && m.Line_ID != lineId), "Line_ID", "Line_Name").ToList();
                var stationItem = from station in db.RS_Stations
                                  where !(from routeConfig in db.RS_Route_Configurations select routeConfig.Station_ID).Contains(station.Station_ID)
                                  select station;

                ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Shop_ID == 0).ToList(), "Station_ID", "Station_Name");




                Dictionary<string, string> abcentemp = new Dictionary<string, string>();

                var abscentEmployee = from employee in db.RS_Employee
                                      join Attandance in db.RS_User_Attendance_Sheet on employee.Employee_ID equals Attandance.Employee_ID
                                      where Attandance.Is_Present == false && Attandance.Entry_Date.Value.Year == DateTime.Now.Year
                                      && employee.Is_Deleted == false
                                        && Attandance.Entry_Date.Value.Month == DateTime.Now.Month
                                        && Attandance.Entry_Date.Value.Day == DateTime.Now.Day
                                      select new AM_EmployeeData()
                                      {
                                          Employee_Name = employee.Employee_Name,
                                          Employee_No = Attandance.Employee_No
                                      };



                //  abcentemp = ()abscentEmployee;
                //var abscentEmployee = from Attandance in db.RS_User_Attendance_Sheet
                //                       where Attandance.Is_Prescent == false
                //                       select Attandance;
                ViewBag.PEmp = null;// prescentEmployee;
                ViewBag.AEmp = abscentEmployee;
                // ViewBag.Cshift = shiftName;


                //Week
                List<int> lstWeek = new List<int>();
                var cult = CultureInfo.CurrentCulture;
                var weekNo = cult.Calendar.GetWeekOfYear(new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);
                int currentWeek = Convert.ToInt32(weekNo);
                lstWeek.Add(currentWeek);
                if (weekNo == 1)
                {
                    //lstWeek.Add(currentWeek);
                }
                else if (weekNo == 2)
                {
                    lstWeek.Add(currentWeek - 1);
                }
                else
                {
                    lstWeek.Add(currentWeek - 2);
                    lstWeek.Add(currentWeek - 1);
                }


                //if (weekNo == 52)
                //{

                //}
                //else if (weekNo == 51)
                //{
                //    lstWeek.Add(currentWeek + 1);
                //}
                //else if(weekNo<=50 )
                //{
                //    lstWeek.Add(currentWeek + 1);
                //    lstWeek.Add(currentWeek + 2);
                //}

                //  ViewBag.weeks = lstWeek;
                List<int> lstAWeek = new List<int>();
                lstAWeek.Add(currentWeek);
                if (weekNo == 52)
                {

                }
                else if (weekNo == 51)
                {
                    lstAWeek.Add(currentWeek + 1);
                }
                else
                {
                    lstAWeek.Add(currentWeek + 1);
                    lstAWeek.Add(currentWeek + 2);
                }

                ViewBag.Shift1 = new SelectList(db.RS_Shift.Where(shift => shift.Plant_ID == plant_Id), "Shift_ID", "Shift_Name");
                ViewBag.Shift2 = new SelectList(db.RS_Shift.Where(shift => shift.Plant_ID == plant_Id), "Shift_ID", "Shift_Name");
                ViewBag.Shift3 = new SelectList(db.RS_Shift.Where(shift => shift.Plant_ID == plant_Id), "Shift_ID", "Shift_Name", weekNo);

                ViewBag.weeks2 = new SelectList(lstWeek, weekNo);
                ViewBag.weeks3 = new SelectList(lstAWeek);
                ViewBag.weeks = new SelectList(lstWeek);

                //---------------------

                DateTime dates = FirstDateOfWeek(DateTime.Now.Year, weekNo);
                DateTime weekStartDate = Convert.ToDateTime(dates);
                ViewBag.CurrentDay = weekStartDate.DayOfWeek;
                //ViewBag.CurrentDay = "Sunday";

                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                //globalData.pageTitle = ResourceRouteConfiguration.Route_Configuration;
                globalData.pageTitle = "Daily Allocation";
                globalData.subTitle = ResourceGlobal.Index;
                globalData.controllerName = "Line";
                globalData.actionName = ResourceGlobal.Index;
                globalData.contentTitle = ResourceRouteConfiguration.Route_Configuration_Title_Configure_Route;
                globalData.contentFooter = ResourceRouteConfiguration.Route_Configuration_Title_Configure_Route;
                ViewBag.GlobalDataModel = globalData;

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }


        public ActionResult AllocationDashboard()
        {
            try
            {



                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }

                globalData.pageTitle = ResourceOperatorAllocation.Operator;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = ResourceOperatorAllocation.Site_Variable_Operator;
                globalData.actionName = ResourceOperatorAllocation.Site_Variable_Index;
                globalData.contentTitle = ResourceOperatorAllocation.Operator_Title_Operator_Lists;
                globalData.contentFooter = ResourceOperatorAllocation.Operator_Title_Operator_Lists;
                ViewBag.GlobalDataModel = globalData;
                // decimal shopID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                //decimal stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                //int shopId=((FDSession)this.Session["FDSession"]).shopId;
                //int stationId = ((FDSession)this.Session["FDSession"]).stationId;//2  
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                //get shop id of user
                var res = db.RS_AM_Line_Supervisor_Mapping.FirstOrDefault(p => p.Employee_ID == userId).Shop_ID;
                int shopID = Convert.ToInt32(res);

                var str4 = (from emp in db.RS_Employee
                            join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
                            join Attendance in db.RS_User_Attendance_Sheet on emp.Employee_No equals Attendance.Employee_No
                            where (Attendance.Is_Present == true && operatorAllocation.Shop_ID == shopID && (Attendance.Entry_Date.Value.Year == DateTime.Now.Year
                                        && Attendance.Entry_Date.Value.Month == DateTime.Now.Month
                                        && Attendance.Entry_Date.Value.Day == DateTime.Now.Day))
                            orderby emp.Employee_No

                            select new AllocationDashboard
                            {
                                Employee_ID = emp.Employee_ID,
                                Employee_Token = emp.Employee_No,
                                Employee_Name = emp.Employee_Name,
                                Station_ID = operatorAllocation.Station_ID,
                                Line_ID = operatorAllocation.Line_ID,
                                Shop_ID = operatorAllocation.Shop_ID,
                                Shop_Name = db.RS_Shops.Where(x => x.Shop_ID == operatorAllocation.Shop_ID).FirstOrDefault().Shop_Name,
                                Line_Name = db.RS_Lines.Where(x => x.Line_ID == operatorAllocation.Line_ID).FirstOrDefault().Line_Name,
                                Station_Name = db.RS_Stations.Where(x => x.Station_ID == operatorAllocation.Station_ID).FirstOrDefault().Station_Name

                            }).Distinct();
                // var RS_AM_Operator_Station_Allocation = db.RS_AM_Operator_Station_Allocation.Include(m => m.RS_AM_Skill_Set).Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations).Include(m => m.RS_Employee);
                // return View(RS_AM_Operator_Station_Allocation.ToList());
                return View(str4);
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }

        public ActionResult getDailyAllocation()
        {
            decimal shopID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            try
            {
                var str4 = (from emp in db.RS_Employee
                            join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
                            join Attendance in db.RS_User_Attendance_Sheet on emp.Employee_ID equals Attendance.Employee_ID
                            where (Attendance.Is_Present == true && operatorAllocation.Shop_ID == shopID && (Attendance.Entry_Date.Value.Year == DateTime.Now.Year
                                        && Attendance.Entry_Date.Value.Month == DateTime.Now.Month
                                        && Attendance.Entry_Date.Value.Day == DateTime.Now.Day))
                            orderby emp.Employee_No
                            select new AllocationDashboard
                            {
                                Employee_ID = emp.Employee_ID,
                                Employee_Token = emp.Employee_No,
                                Employee_Name = emp.Employee_Name,
                                Station_ID = operatorAllocation.Station_ID,
                                Line_ID = operatorAllocation.Line_ID,
                                Shop_ID = operatorAllocation.Shop_ID,
                                Shop_Name = db.RS_Shops.Where(x => x.Shop_ID == operatorAllocation.Shop_ID).FirstOrDefault().Shop_Name,
                                Line_Name = db.RS_Lines.Where(x => x.Line_ID == operatorAllocation.Line_ID).FirstOrDefault().Line_Name,
                                Station_Name = db.RS_Stations.Where(x => x.Station_ID == operatorAllocation.Station_ID).FirstOrDefault().Station_Name

                            }).Distinct();
                return Json(str4, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                    return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetCurrentShift(int shopId)
        {
            try
            {
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                //var shiftObj = db.RS_Shift.AsEnumerable()
                //                 .Where(a => a.Shop_ID == shopId && TimeSpan.Compare(a.Shift_Start_Time, currDate) < 0 && TimeSpan.Compare(a.Shift_End_Time, currDate) > 0)
                //                 .FirstOrDefault();

                var shiftObj = (from shift in db.RS_Shift
                                where
                               shift.Shop_ID == shopId
                                //&& TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                                //&& TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                                select new
                                {
                                    shift.Shift_ID,
                                    shift.Shift_Name
                                });

                return Json(shiftObj);
            }
            catch (Exception dbEx)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult saveAssociateAllocation(string employeeNo, string prevStationId, string stationId, int shopId, int lineId, int shift, int FromDay, int ToDay, bool IsOJT, int SetupID)
        {
            try
            {
                string[] values = prevStationId.Split('_');
                prevStationId = values.Length > 1 ? values[2] : prevStationId;
                int flag = 0;
                var day = DateTime.Today.DayOfWeek;
                var cult = CultureInfo.CurrentCulture;
                var weekNo = cult.Calendar.GetWeekOfYear(new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);
                DateTime dates = FirstDateOfWeek(DateTime.Now.Year, weekNo);
                DateTime weekStartDate = dates;
                DateTime today = DateTime.Today;
                int currentDayOfWeek = (int)today.DayOfWeek;

                //decimal pStationId = Convert.ToDecimal(prevStationId);
                //decimal ChangeStationId = Convert.ToDecimal(stationId);
                //decimal employeeId = db.RS_Employee.Where(e => e.Employee_No == employeeNo).Select(e => e.Employee_ID).FirstOrDefault();
                //decimal ExistRecord = db.RS_AM_Operator_Station_Allocation.Where(c => c.Employee_ID == employeeId && c.Station_ID== pStationId && c.Allocation_Date == today && c.Shift_ID== shift && c.Shop_ID== shopId).Select(c=>c.OSM_ID).FirstOrDefault();

                //RS_AM_Operator_Station_Allocation obj = new RS_AM_Operator_Station_Allocation();
                //obj.Station_ID = ChangeStationId;



                if (FromDay > ToDay || currentDayOfWeek > ToDay)
                {
                    return Json(new { result = "dateerror" }, JsonRequestBehavior.AllowGet);
                }

                int Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                int droppedStationId = 0;
                if (stationId == "extra_operator_list")
                {
                    var station_Id = db.RS_Stations.Where(m => m.Shop_ID == shopId && m.Line_ID == lineId && m.Is_Buffer == true).Select(m => m.Station_ID).FirstOrDefault();
                    droppedStationId = Convert.ToInt32(station_Id);
                    //bufferStation = true;
                }
                else
                {
                    string[] stat = stationId.Split('_');
                    droppedStationId = Convert.ToInt16(stat[1]);
                }
                string[] empno = employeeNo.Split('_');
                employeeNo = empno.Length > 1 ? empno[3] : empno[0];

                var st = db.RS_Employee.Where(a => a.Employee_No == employeeNo && a.Plant_ID == Plant_ID && a.Is_Deleted == null).FirstOrDefault();
                decimal employeeId = st.Employee_ID;
                decimal shiftid = shift;

                //weekStartDate = weekStartDate.AddDays(FromDay - 1);

                //if (mmAllocationObj.IsOperatorToOneStationExists(employeeId, droppedStationId, shiftid, shopId, weekStartDate.Date))
                //{
                //    return Json(new { result = "Allready" }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                int fromD = 0;
                currentDayOfWeek = (int)today.DayOfWeek;
                weekStartDate = Convert.ToDateTime(dates);
                if (FromDay >= currentDayOfWeek)
                {
                    fromD = FromDay;
                }
                else
                {
                    fromD = currentDayOfWeek;
                }
                if (fromD != 1)
                {
                    weekStartDate = weekStartDate.AddDays(fromD - 1);
                }

                if (empno.Length > 1)
                {
                    fromD = currentDayOfWeek;
                    ToDay = currentDayOfWeek;
                }

                for (int i = fromD; i <= ToDay; i++)
                {
                    var isEmpExistCount = db.RS_AM_Operator_Station_Allocation.Where(op => DbFunctions.TruncateTime(op.Allocation_Date) == DbFunctions.TruncateTime(weekStartDate) && op.Employee_ID == employeeId && op.Shift_ID == shiftid && op.Station_ID == droppedStationId && op.Setup_ID == SetupID).Select(op => op.Week_Day).ToList().Count();
                    if (isEmpExistCount == 0)
                    {
                        if (empno.Length > 1)
                        {
                            var pStationId = Convert.ToDecimal(prevStationId);
                            //var ChangeStationId = Convert.ToDecimal(droppedStationId);
                            decimal employee_Id = db.RS_Employee.Where(e => e.Employee_No == employeeNo).Select(e => e.Employee_ID).FirstOrDefault();
                            decimal ExistRecord = db.RS_AM_Operator_Station_Allocation.Where(c => c.Employee_ID == employeeId && c.Station_ID == pStationId && c.Allocation_Date == today && c.Shift_ID == shift && c.Shop_ID == shopId && c.Setup_ID == SetupID).Select(c => c.OSM_ID).FirstOrDefault();

                            RS_AM_Operator_Station_Allocation obj = new RS_AM_Operator_Station_Allocation();
                            RS_AM_Operator_Station_Allocation_History obj1 = new RS_AM_Operator_Station_Allocation_History();
                            obj = db.RS_AM_Operator_Station_Allocation.Find(ExistRecord);
                            obj.Station_ID = droppedStationId;
                            obj.Week_Day = day.ToString();
                            obj.Week_Number = weekNo;
                            if (stationId == "extra_operator_list")
                            {
                                obj.Is_Buffer_Station = true;
                                obj.Station_Type_ID = null;
                                obj.Is_OJT_Operator = false;
                            }
                            // obj.OSM_ID = ExistRecord;
                            db.Entry(obj).State = EntityState.Modified;
                            db.SaveChanges();

                            RS_External_Training_Data isExtObj = db.RS_External_Training_Data.Where(m => m.Station_ID == pStationId && m.Setup_ID == SetupID && m.Training_Date == today && m.Employee_ID == employeeId).Select(m => m).FirstOrDefault();
                            if (isExtObj != null)
                            {
                                db.RS_External_Training_Data.Remove(isExtObj);
                                db.SaveChanges();
                            }

                            RS_On_Job_Training_Data isOJTExistObj = db.RS_On_Job_Training_Data.Where(m => m.Station_ID == pStationId && m.Setup_ID == SetupID && m.OJT_Date == today && m.Employee_ID == employeeId).Select(m => m).FirstOrDefault();
                            if (isOJTExistObj != null)
                            {
                                db.RS_On_Job_Training_Data.Remove(isOJTExistObj);
                                db.SaveChanges();
                            }
                            if (IsOJT == true)
                            {
                                RS_On_Job_Training_Data mojt = new RS_On_Job_Training_Data();
                                var ExistOJTRecord = db.RS_On_Job_Training_Data.Where(c => c.Employee_ID == employeeId && c.Station_ID == pStationId && c.OJT_Date == today && c.Shift_ID == shift && c.Shop_ID == shopId && c.Setup_ID == SetupID).Select(c => c.OJT_ID).FirstOrDefault();
                                mojt = db.RS_On_Job_Training_Data.Find(ExistOJTRecord);
                                mojt.Station_ID = droppedStationId;
                                db.Entry(mojt).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            obj1.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            obj1.Shop_ID = shopId;
                            obj1.Line_ID = lineId;
                            obj1.Shift_ID = shiftid;
                            obj1.Station_ID = droppedStationId;
                            obj1.Setup_ID = obj.Setup_ID;
                            obj1.Is_Transferred = false;
                            obj1.Is_Purgeable = false;
                            obj1.Employee_ID = employeeId;
                            obj1.Allocation_Date = today;
                            obj1.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            obj1.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

                            obj1.Inserted_Date = DateTime.Now;
                            obj1.Is_Transfer_Operator = true;
                            obj1.Is_Buffer_Station = obj.Is_Buffer_Station;
                            obj1.Is_OJT_Operator = obj.Is_OJT_Operator;
                            obj1.Prev_Line_ID = obj.Line_ID;
                            obj1.Prev_Shop_ID = obj.Shop_ID;
                            obj1.Prev_Station_ID = pStationId;
                            obj1.Week_Number = weekNo;
                            obj1.Week_Day = day.ToString();
                            db.RS_AM_Operator_Station_Allocation_History.Add(obj1);
                            db.SaveChanges();
                            flag = 1;
                            //GetCurrentShiftOperatorAgainstStationListByLineID(lineId,Convert.ToDecimal( shift), day.ToString());
                        }
                        else
                        {
                            saveAssociateAllocationDetails(shopId, lineId, droppedStationId, shiftid, employeeId, weekNo, weekStartDate.DayOfWeek.ToString(), weekStartDate.Date, IsOJT, SetupID);
                            weekStartDate = weekStartDate.AddDays(1);
                            flag = 1;
                        }

                    }
                    else
                    {
                        flag = 2;
                        weekStartDate = weekStartDate.AddDays(1);
                    }
                }
                if (flag == 1)
                {
                    //var stationID = Convert.ToDecimal(prevStationId);
                    //var emp = db.RS_AM_Operator_Station_Allocation.FirstOrDefault(s => s.Shop_ID == shopId && s.Line_ID == lineId && s.Shift_ID == shiftid && s.Employee_ID == employeeId && s.Station_ID == stationID && s.Allocation_Date == DateTime.Today);
                    //if (emp != null)
                    //{
                    //    db.RS_AM_Operator_Station_Allocation.Remove(emp);
                    //    db.SaveChanges();
                    //}
                    return Json(new { result = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else if (flag == 2)
                {
                    return Json(new { result = "Allready" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "failed" }, JsonRequestBehavior.AllowGet);
                }
                //}
            }
            catch (Exception ex)
            {
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult DeleteOperator(string Id, string employeeNo, string stationId, int shift, int line_ID, string fromDay, string toDay, int setupID)
        {
            try
            {
                decimal Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                string[] stat = employeeNo.Split('_');
                int droppedStationId = Convert.ToInt16(stat[2]);
                string droppedEmployeeNo = stat[3];
                var cult = CultureInfo.CurrentCulture;
                var weekNo = cult.Calendar.GetWeekOfYear(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);
                DateTime dates = FirstDateOfWeek(DateTime.Now.Year, weekNo);
                DateTime StartDate = dates;
                DateTime EndDate = dates;
                if (fromDay == "Tuesday")
                {
                    StartDate = StartDate.Date.AddDays(1);
                }
                else if (fromDay == "Wednesday")
                {
                    StartDate = StartDate.Date.AddDays(2);
                }
                else if (fromDay == "Thursday")
                {
                    StartDate = StartDate.Date.AddDays(3);
                }

                if (fromDay == "Friday")
                {
                    StartDate = StartDate.Date.AddDays(4);
                }

                else if (fromDay == "Saturday")
                {
                    StartDate = StartDate.Date.AddDays(5);
                }
                else if (fromDay == "Sunday")
                {
                    StartDate = StartDate.Date.AddDays(6);
                }

                //End Date

                if (toDay == "Tuesday")
                {
                    EndDate = EndDate.Date.AddDays(1);
                }
                else if (toDay == "Wednesday")
                {
                    EndDate = EndDate.Date.AddDays(2);
                }
                else if (toDay == "Thursday")
                {
                    EndDate = EndDate.Date.AddDays(3);
                }
                if (toDay == "Friday")
                {
                    EndDate = EndDate.Date.AddDays(4);
                }
                else if (toDay == "Saturday")
                {
                    EndDate = EndDate.Date.AddDays(5);
                }
                else if (toDay == "Sunday")
                {
                    EndDate = EndDate.Date.AddDays(6);
                }

                if (StartDate.Year <= DateTime.Now.Year && StartDate.Month <= DateTime.Now.Month && StartDate.Day <= DateTime.Now.Day)
                {
                    StartDate = DateTime.Now;
                }
                if (EndDate.Year <= DateTime.Now.Year && EndDate.Month <= DateTime.Now.Month && EndDate.Day < DateTime.Now.Day)
                {
                    return Json(new { msg = "Previous" }, JsonRequestBehavior.AllowGet);
                }

                var st = db.RS_Employee.Where(a => a.Employee_No == droppedEmployeeNo && a.Plant_ID == Plant_ID && a.Is_Deleted == null).FirstOrDefault();
                decimal droppedEmployeeId = st.Employee_ID;
                try
                {
                    int flag = 0;
                    if (Id == "btnThis")
                    {
                        IEnumerable<RS_AM_Operator_Station_Allocation> deleteOp = (from op in db.RS_AM_Operator_Station_Allocation
                                                                                   where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                                                                                   && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                                                                                   && op.Setup_ID == setupID && op.Shift_ID == shift && op.Employee_ID == droppedEmployeeId && op.Is_Buffer_Station == true
                                                                                   select op).ToList();
                        var sId = db.RS_Stations.Where(m => m.Is_Buffer == true && m.Line_ID == line_ID).Select(m => m.Station_ID).FirstOrDefault();
                        foreach (var item in deleteOp)
                        {
                            item.Station_ID = sId;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        IEnumerable<RS_AM_Operator_Station_Allocation> operatop = (from op in db.RS_AM_Operator_Station_Allocation
                                                                                   where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                                                                                   && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                                                                                   && op.Setup_ID == setupID && op.Shift_ID == shift && op.Employee_ID == droppedEmployeeId && op.Is_Buffer_Station != true
                                                                                   select op).ToList();
                        db.RS_AM_Operator_Station_Allocation.RemoveRange(operatop);
                        db.SaveChanges();

                        //IEnumerable<RS_AM_Operator_Station_Allocation_History> deleteopH = (from op in db.RS_AM_Operator_Station_Allocation_History
                        //                                                                    where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                        //                                                                    && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                        //                                                                    && op.Shift_ID == shift && op.Employee_ID == droppedEmployeeId && op.Is_Buffer_Station == true
                        //                                                                    select op).ToList();

                        //foreach(var item in deleteopH)
                        //{
                        //    item.Station_ID = sId;
                        //    db.Entry(item).State = EntityState.Modified;
                        //    db.SaveChanges();
                        //}
                        IEnumerable<RS_AM_Operator_Station_Allocation_History> operatopH = (from op in db.RS_AM_Operator_Station_Allocation_History
                                                                                            where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                                                                                            && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                                                                                            && op.Setup_ID == setupID && op.Shift_ID == shift && op.Employee_ID == droppedEmployeeId && op.Is_Buffer_Station != true
                                                                                            select op).ToList();
                        //db.RS_AM_Operator_Station_Allocation_History.RemoveRange(operatopH);
                        foreach (var item in operatopH)
                        {
                            item.Is_Deleted = true;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //db.SaveChanges();
                    }
                    else
                    {
                        //IEnumerable<RS_AM_Operator_Station_Allocation> deleteop = (from op in db.RS_AM_Operator_Station_Allocation
                        //                                                           where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                        //                                                           && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                        //                                                           && op.Shift_ID == shift && op.Line_ID == line_ID && op.Is_Buffer_Station == true
                        //                                                           select op).ToList();
                        //var sId = db.RS_Stations.Where(m => m.Is_Buffer_Station == true && m.Line_ID == line_ID).Select(m => m.Station_ID).FirstOrDefault();
                        //foreach (var item in deleteop)
                        //{
                        //    item.Station_ID = sId;
                        //    db.Entry(item).State = EntityState.Modified;
                        //    db.SaveChanges();
                        //}

                        IEnumerable<RS_AM_Operator_Station_Allocation> operatop = (from op in db.RS_AM_Operator_Station_Allocation
                                                                                   where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                                                                                   && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                                                                                   && op.Setup_ID == setupID && op.Shift_ID == shift && op.Line_ID == line_ID /*&& op.Is_Buffer_Station != true*/
                                                                                   select op).ToList();
                        db.RS_AM_Operator_Station_Allocation.RemoveRange(operatop);
                        db.SaveChanges();

                        //IEnumerable<RS_AM_Operator_Station_Allocation_History> deletepH = (from op in db.RS_AM_Operator_Station_Allocation_History
                        //                                                                    where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                        //                                                                    && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                        //                                                                    && op.Shift_ID == shift && op.Line_ID == line_ID && op.Is_Buffer_Station != true
                        //                                                                    select op).ToList();
                        //foreach (var item in deletepH)
                        //{
                        //    item.Station_ID = sId;
                        //    db.Entry(item).State = EntityState.Modified;
                        //    db.SaveChanges();
                        //}

                        IEnumerable<RS_AM_Operator_Station_Allocation_History> operatopH = (from op in db.RS_AM_Operator_Station_Allocation_History
                                                                                            where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                                                                                            && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                                                                                            && op.Setup_ID == setupID && op.Shift_ID == shift && op.Line_ID == line_ID /*&& op.Is_Buffer_Station != true*/
                                                                                            select op).ToList();
                        //db.RS_AM_Operator_Station_Allocation_History.RemoveRange(operatopH);
                        foreach (var item in operatopH)
                        {
                            item.Is_Deleted = true;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    //if (flag == 1)
                    //{
                    return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { msg = "Error" }, JsonRequestBehavior.AllowGet);                    }
                }
                catch (Exception ex)
                {
                   
                        return Json(new { msg = "Error" }, JsonRequestBehavior.AllowGet);
                }

                // RS_AM_Operator_Station_Allocation mmAllocationObj = db.RS_AM_Operator_Station_Allocation.Find(aremove);
                // db.RS_AM_Operator_Station_Allocation.Remove(aremove);
                //db.SaveChanges();

                //return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult setupfreeze(string lineID, string setupID)
        {
            try
            {
                var lineId = Convert.ToInt32(lineID);
                var setupId = Convert.ToInt32(setupID);
                var date = DateTime.Today;
                var allocationData = db.RS_AM_Operator_Station_Allocation.Where(m => m.Allocation_Date == date && m.Line_ID == lineId && m.Setup_ID == setupId).ToList();
                foreach (var item in allocationData)
                {
                    item.Is_Setup_Freeze = true;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var historyData = db.RS_AM_Operator_Station_Allocation_History.Where(m => m.Allocation_Date == date && m.Line_ID == lineId && m.Setup_ID == setupId).ToList();
                foreach (var item in historyData)
                {
                    item.Is_Setup_Freeze = true;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /* Action Name                : GetStationListByLineID
       *  Description                : Action used to get the list of station which is added in route configuration
       *  Author, Timestamp          : Ajay Wagh
       *  Input parameter            : lineId (line id)
       *  Return Type                : ActionResult
       *  Revision                   : 1.0
       */
        public ActionResult GetStationListByLineID(int lineId, int shiftID, int setupID)
        {
            try
            {

                var st1 = (from str1 in db.RS_Stations
                           join str in db.RS_Station_Setup_Mapping
                           on str1.Station_ID equals str.Station_ID
                           where str1.Line_ID == lineId && str1.Is_Buffer != true && str.Setup_ID == setupID && str1.Station_Type_ID == 1
                           select new
                           {
                               Id = str1.Station_ID,
                               Value = str1.Station_Name,
                               Type = str1.RS_Station_Type.Station_Type_Name,
                               TypeId = str1.RS_Station_Type.Station_Type_ID,
                               SortOrder = str1.Sort_Order
                           }).Distinct();
                var st2 = (from str1 in db.RS_Stations
                           join str in db.RS_Station_Setup_Mapping
                           on str1.Station_ID equals str.Station_ID
                           where str1.Line_ID == lineId && str1.Is_Buffer != true && str1.Station_Type_ID != 1 && str.Setup_ID == setupID
                           select new
                           {
                               Id = str1.Station_ID,
                               Value = str1.Station_Name,
                               Type = str1.RS_Station_Type.Station_Type_Name,
                               TypeId = str1.RS_Station_Type.Station_Type_ID,
                               SortOrder = str1.Sort_Order
                           }).Distinct();
                var st = st2!= null ? st1.Union(st2).OrderBy(m => m.SortOrder) : st1;

                return Json(new { st = st, count = st1.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetCurrentShiftOperatorAgainstStationListByLineID(int lineId, decimal? shiftID, int setupID, string Day)

        {
            try
            {
                int supervisorId = ((FDSession)this.Session["FDSession"]).userId;
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                decimal shopId = db.RS_Lines.Find(lineId).Shop_ID;

                var day = DateTime.Today.DayOfWeek;
                var cult = CultureInfo.CurrentCulture;
                int weekNo = cult.Calendar.GetWeekOfYear(new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);


                decimal shiftid = shiftID.Value;

                var str4 = (from emp in db.RS_Employee
                            join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
                            join station in db.RS_Station_Setup_Mapping on operatorAllocation.Station_ID equals station.Station_ID
                            where operatorAllocation.Shift_ID == shiftid && operatorAllocation.Is_Buffer_Station != true && operatorAllocation.Line_ID == lineId && emp.Is_Deleted == null && emp.Plant_ID == plant_ID && operatorAllocation.Setup_ID == setupID
                            //&& DbFunctions.TruncateTime(operatorAllocation.Allocation_Date) == DbFunctions.TruncateTime(DateTime.Now) 
                            && operatorAllocation.Allocation_Date == DateTime.Today
                            //&& operatorAllocation.Week_Number == weekNo
                            select new
                            {
                                Employee_ID = emp.Employee_ID,
                                Employee_Token = emp.Employee_No,
                                Employee_Name = emp.Employee_Name,
                                Image_Content = emp.Image_Content,
                                Station_ID = operatorAllocation.Station_ID,
                                Station_Name = operatorAllocation.RS_Stations.Station_Name,
                                IsOJT = operatorAllocation.Is_OJT_Operator,
                                Station_Type_ID = operatorAllocation.Station_Type_ID,
                                Absent = !(from att in db.RS_User_Attendance_Sheet
                                           where att.Entry_Date.Value.Year == DateTime.Now.Year
                                           && att.Entry_Date.Value.Month == DateTime.Now.Month
                                           && att.Entry_Date.Value.Day == DateTime.Now.Day /*&& att.Shift_ID == shiftid*/
                                           select att.Employee_No).Contains(emp.Employee_No) ? "ABSENT" : ""
                            }).Distinct();

                var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_Token, x.Employee_Name, x.Station_ID, x.Absent, x.Image_Content, x.IsOJT, x.Station_Type_ID, x.Station_Name }).Distinct();
                var str1 = str5.Where(m => m.Station_Type_ID == 1);
                var str2 = str5.Where(m => m.Station_Type_ID == 2);
                var str3 = str5.Where(m => m.IsOJT == true);
                // var ids = 
                var stationIds = str5.Where(m => m.Absent == "ABSENT").Select(m => m.Station_ID).ToArray();
                var idss = from str in str5
                           where str.Absent == "ABSENT"
                           group str by str.Station_ID into str12
                           select new
                           {
                               Name = str12.Key,
                               Count = str12.Count()
                           };

                List<string> stationList = new List<string>();
                List<int> opCount = new List<int>();
                foreach (var id in idss)
                {
                    //var count = stationIds
                    var stationName = db.RS_Stations.Where(m => m.Station_ID == id.Name).Select(m => m.Station_Name).FirstOrDefault();
                    stationList.Add(stationName);
                    opCount.Add(id.Count);
                }
                var requireDirectCount = db.RS_ManPower_Required.Where(m => m.Line_ID == lineId && m.Setup_ID == setupID).Select(m => m.Direct_MP_Quantity).FirstOrDefault();
                var requireIndirectCount = db.RS_ManPower_Required.Where(m => m.Line_ID == lineId && m.Setup_ID == setupID).Select(m => m.Indirect_MP_Quantity).FirstOrDefault();
                return Json(new { str5 = str5.Distinct(), DirectCount = str1.Count(), IndirectCount = str2.Count(), OJTCount = str3.Count(), Direct = requireDirectCount, InDirect = requireIndirectCount, list = stationList, OpCount = opCount }, JsonRequestBehavior.AllowGet);

                // return Json(str4, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        //All Employee
        //if employee on leave display on red color 
        //Don't take RS_User_Attendance_Sheet table reference
        //public ActionResult GetPresentSkilledEmployeeAgainstSelectedStation(int stationis,int? ShiftID)
        //{
        //    try
        //    {
        //        int supervisorId = ((FDSession)this.Session["FDSession"]).userId;
        //        decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

        //        //   var shift = GetCurrentShift(shopId);
        //        decimal shiftid = Convert.ToDecimal(ShiftID);
        //        var str4 = (from a in db.RS_Employee
        //                    //join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
        //                    join c in db.RS_User_Attendance_Sheet on a.Employee_No equals c.Employee_No
        //                    join d in db.RS_AM_Employee_SkillSet on a.Employee_ID equals d.Employee_ID
        //                    where  a.Is_Deleted == null && a.Shift_ID == shiftid
        //                            && c.Is_Present == true && (c.Entry_Date.Value.Year == DateTime.Now.Year
        //                                    && c.Entry_Date.Value.Month == DateTime.Now.Month
        //                                    && c.Entry_Date.Value.Day == DateTime.Now.Day)
        //                            && d.Station_ID == stationis  // 33
        //                                                          //  && !(from l in db.MM_AM_LeaveManagement where l.Plant_ID == plant_ID select l.Employee_ID).Contains(a.Employee_ID)
        //                    orderby d.Skill_ID descending
        //                    select new
        //                    {
        //                        a.Employee_ID,
        //                        a.Employee_Name,
        //                        a.Employee_No,
        //                        d.Skill_ID,
        //                        a.Image_Content
        //                        //a.Line_ID,
        //                        //d.Line_Name,
        //                        //a.Station_Name
        //                    }).Distinct().ToList();
        //        var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_Name, x.Employee_No,x.Image_Content }).Distinct();
        //        //ViewBag.PresentDeployedEmp = str5.Count();
        //        // return PartialView();
        //        return Json(new { str5 =   str5.Distinct(), count = str5.Count() } , JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult GetAbsCoverageEmployeeUnderLogedinSupervisor(int shopId, int? ShiftID, int Line_ID, int Setup_ID)
        {
            try
            {
                int supervisorId = ((FDSession)this.Session["FDSession"]).userId;
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;



                TimeSpan currDate = DateTime.Now.TimeOfDay;
                //var shiftObj = (from shift in db.RS_Shift
                //                where
                //                  shift.Shop_ID == shopId &&
                //                 TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                //                && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                //                select shift);
                decimal shiftid = Convert.ToDecimal(ShiftID); //shiftObj.FirstOrDefault().Shift_ID;
                var stationId = db.RS_Stations.Where(m => m.Shop_ID == shopId && m.Line_ID == Line_ID && m.Is_Buffer == true).Select(m => m.Station_ID).FirstOrDefault();
                var str4 = (from a in db.RS_Employee
                                //join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
                            join c in db.RS_User_Attendance_Sheet on a.Employee_No equals c.Employee_No
                            join d in db.RS_AM_Operator_Station_Allocation on a.Employee_ID equals d.Employee_ID

                            where d.Line_ID == Line_ID && d.Shift_ID == shiftid && d.Is_Buffer_Station == true && d.Allocation_Date == DateTime.Today && d.Setup_ID == Setup_ID
                                    && c.Is_Present == true && (c.Entry_Date.Value.Year == DateTime.Now.Year
                                            && c.Entry_Date.Value.Month == DateTime.Now.Month
                                            && c.Entry_Date.Value.Day == DateTime.Now.Day)

                            //  && !(from l in db.MM_AM_LeaveManagement where l.Plant_ID == plant_ID select l.Employee_ID).Contains(a.Employee_ID)
                            //orderby d.Skill_ID descending
                            select new
                            {
                                Employee_ID = a.Employee_ID,
                                Employee_Name = a.Employee_Name,
                                Employee_No = a.Employee_No,
                                Image_Content = a.Image_Content,
                                StationId = stationId
                                //d.Skill_ID
                                //a.Line_ID,
                                //d.Line_Name,
                                //a.Station_Name
                            }).Distinct().ToList();



                var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_Name, x.Employee_No, x.Image_Content, x.StationId }).Distinct();

                //str4 = str5;
                return Json(new { str5 = str5.Distinct(), count = str5.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetAbscentEmployeeUnderLogedinSupervisor(int shopId, int? ShiftID, int Line_ID, int SetupID)  //int stationis
        {
            try
            {
                int supervisorId = ((FDSession)this.Session["FDSession"]).userId;
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;



                TimeSpan currDate = DateTime.Now.TimeOfDay;
                //var shiftObj = (from shift in db.RS_Shift
                //                where
                //                  shift.Shop_ID == shopId &&
                //                 TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                //                && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                //                select shift);
                decimal shiftid = Convert.ToDecimal(ShiftID); //shiftObj.FirstOrDefault().Shift_ID;

                var str2 = (from a in db.RS_Employee
                                //join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
                            join c in db.RS_AM_Operator_Station_Allocation on a.Employee_ID equals c.Employee_ID
                            //join d in db.RS_Station_Setup_Mapping on c.Station_ID equals d.Station_ID
                            //join d in db.RS_AM_Employee_SkillSet on a.Employee_ID equals d.Employee_ID
                            // b.Supervisor_ID == supervisorId &&
                            where c.Shift_ID == shiftid && c.Allocation_Date == DateTime.Today && c.Setup_ID == SetupID
                             && a.Is_Deleted == null && c.Line_ID == Line_ID
                                && !(from att in db.RS_User_Attendance_Sheet
                                     where att.Entry_Date.Value.Year == DateTime.Now.Year
                                     && att.Entry_Date.Value.Month == DateTime.Now.Month
                                     && att.Entry_Date.Value.Day == DateTime.Now.Day
                                     select att.Employee_No).Contains(a.Employee_No)
                            //&& b.Line_ID== Line_ID// 33
                            // && (from l in db.MM_AM_LeaveManagement where l.Plant_ID == plant_ID select l.Employee_ID).Contains(a.Employee_ID)
                            //orderby d.Skill_ID descending
                            select new
                            {
                                a.Employee_ID,
                                a.Employee_Name,
                                a.Employee_No,
                                a.Image_Content
                                //d.Skill_ID
                                //a.Line_ID,
                                //d.Line_Name,
                                //a.Station_Name
                            }).Distinct().ToList();

                var str3 = (from a in db.RS_Employee
                            join c in db.RS_AM_Operator_Station_Allocation on a.Employee_ID equals c.Employee_ID

                            where c.Shift_ID == shiftid && c.Allocation_Date == DateTime.Today && c.Is_Buffer_Station == true && c.Setup_ID == SetupID
                             && a.Is_Deleted == null && c.Line_ID == Line_ID
                                && !(from att in db.RS_User_Attendance_Sheet
                                     where att.Entry_Date.Value.Year == DateTime.Now.Year
                                     && att.Entry_Date.Value.Month == DateTime.Now.Month
                                     && att.Entry_Date.Value.Day == DateTime.Now.Day
                                     select att.Employee_No).Contains(a.Employee_No)

                            select new
                            {
                                a.Employee_ID,
                                a.Employee_Name,
                                a.Employee_No,
                                a.Image_Content
                            }).Distinct().ToList();
                var str4 = str2.Union(str3);
                var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_Name, x.Employee_No, x.Image_Content }).Distinct();

                //str4 = str5;
                return Json(new { str5 = str5.Distinct(), count = str5.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOJTHistory(string empNumber)
        {
            var EmpName = db.RS_Employee.Where(m => m.Employee_No == empNumber).Select(m => m.Employee_Name).FirstOrDefault();
            EmpName = EmpName.Split(',')[0];
            var str1 = (from ojt in db.RS_On_Job_Training_Data
                        join emp in db.RS_Employee on ojt.Employee_ID equals emp.Employee_ID
                        where emp.Employee_No == empNumber && ojt.OJT_Date < DateTime.Today
                        group ojt by ojt.RS_Stations.Station_Name into data
                        select new
                        {
                            EmpToken = empNumber,
                            stationName = data.Key,
                            Duration = data.Count()
                        }
                        );
            return Json(new { str = str1, Name = EmpName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLineByShopID(int shopId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int supervisorID = ((FDSession)this.Session["FDSession"]).userId;
                var lineID = from sup in db.RS_AM_Line_Supervisor_Mapping
                             where sup.Employee_ID == supervisorID && sup.Plant_ID == plantID
                             select sup.Line_ID;

                var st = from line in db.RS_Lines
                         where line.Shop_ID == shopId && (lineID).Contains(line.Line_ID)
                         orderby line.Line_Name
                         select new
                         {
                             line.Line_ID,
                             line.Line_Name,
                         };
                return Json(st);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Getsetupbylineid(int lineId)
        {
            try
            {
                var list = db.RS_AM_Operator_Station_Allocation.Where(m => m.Allocation_Date == DateTime.Today && m.Line_ID == lineId && m.Is_Setup_Freeze == true).ToList();
                var st = from setup in db.RS_Setup
                         where setup.Line_ID == lineId
                         orderby setup.Setup_Name
                         select new
                         {
                             Id = setup.Setup_ID,
                             Value = setup.Setup_Name,
                         };
                if (list.Count > 0)
                {
                    var setupId = db.RS_AM_Operator_Station_Allocation.Where(m => m.Allocation_Date == DateTime.Today && m.Line_ID == lineId && m.Is_Setup_Freeze == true).Select(m => m.Setup_ID).FirstOrDefault();
                    st = from setup in db.RS_Setup
                         where setup.Line_ID == lineId && setup.Setup_ID == setupId
                         orderby setup.Setup_Name
                         select new
                         {
                             Id = setup.Setup_ID,
                             Value = setup.Setup_Name,
                         };
                }
                return Json(st);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetWeekandShiftWiseAssignedOperatorDetails(int lineId, int shopID, int week, int Shift)
        {
            var operators = (from emp in db.RS_Employee
                             join ope in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals ope.Employee_ID
                             join ope_History in db.RS_AM_Operator_Station_Allocation_History on emp.Employee_ID equals ope_History.Employee_ID
                             join station in db.RS_Stations on ope_History.Station_ID equals station.Station_ID
                             where ope_History.Line_ID == lineId && ope_History.Shop_ID == shopID && ope_History.Week_Number == week && ope_History.Shift_ID == Shift
                             select new
                             {
                                 Employee_Name = emp.Employee_Name + "_" + emp.Employee_No,
                                 Station = station.Station_Name,
                                 Day = ope_History.Week_Day,
                                 Shift = ope_History.RS_Shift.Shift_Name,
                                 ope_History.Week_Day,
                                 ope_History.Shift_ID,
                                 ope_History.Line_ID,
                                 ope_History.Shop_ID,
                                 ope_History.Station_ID,
                                 ope_History.Employee_ID,
                             }).ToList().Distinct().OrderBy(emp => emp.Day);

            //var results = (from op in operators select op).GroupBy(p => p.Day).ToList();

            // List<RS_AM_Operator_Station_Allocation_History> obj = (from op in operators select new { op.Employee_ID, op.Week_Day, op.Shift_ID, op.Line_ID, op.Station_ID }).ToList();
            TempData["operators"] = operators;

            return Json(operators, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AssignOperatorToNextWeek_OLD(int copy_Week, DateTime fromDate, DateTime toDate, int Shift, string replace)
        //{
        //    try
        //    {
        //        // string msg = string.Empty;
        //        int plantID = ((FDSession)this.Session["FDSession"]).plantId;
        //        int supervisorID = ((FDSession)this.Session["FDSession"]).userId;
        //        string userHost = ((FDSession)this.Session["FDSession"]).userHost;
        //        DateTime dates = FirstDateOfWeek(DateTime.Now.Year, weeks);
        //        DateTime weekStartDate = Convert.ToDateTime(dates);
        //        RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation;
        //        RS_AM_Operator_Station_Allocation_History RS_AM_Operator_Station_Allocation_History;
        //        IEnumerable<object> obj = TempData.ContainsKey("operators") ? TempData["operators"] as IEnumerable<object> : null;
        //        TempData.Keep("operators");
        //        string prevDay = string.Empty;
        //        string currDay = string.Empty;
        //        var data = obj;
        //        // int i = 0;

        //        int count = db.RS_AM_Operator_Station_Allocation_History.Where(os => os.Week_Number == weeks && os.Shift_ID == Shift).Count();

        //        if (obj != null && (count <= 0 || replace == "YES"))
        //        {
        //            if (replace == "YES")
        //            {
        //                //remove allready exist weekly allocation
        //                IEnumerable<RS_AM_Operator_Station_Allocation> operatop = (from op in db.RS_AM_Operator_Station_Allocation
        //                                                                           where op.Week_Number == weeks && op.Shift_ID == Shift
        //                                                                           select op).ToList();
        //                db.RS_AM_Operator_Station_Allocation.RemoveRange(operatop);
        //                db.SaveChanges();

        //                IEnumerable<RS_AM_Operator_Station_Allocation_History> operatop_history = (from op in db.RS_AM_Operator_Station_Allocation_History
        //                                                                                           where op.Week_Number == weeks && op.Shift_ID == Shift
        //                                                                                           select op).ToList();
        //                db.RS_AM_Operator_Station_Allocation_History.RemoveRange(operatop_history);
        //                db.SaveChanges();

        //            }
        //            int flag = 0;
        //            DateTime startDay = DateTime.MinValue;
        //            foreach (dynamic item in data)
        //            {
        //                RS_AM_Operator_Station_Allocation = new RS_AM_Operator_Station_Allocation();
        //                RS_AM_Operator_Station_Allocation_History = new RS_AM_Operator_Station_Allocation_History();
        //                //if (!item.Week_Day.Contains("Sunday"))
        //                startDay = weekStartDate;
        //                currDay = weekStartDate.DayOfWeek.ToString();

        //                if (item.Week_Day == "Tuesday")
        //                {
        //                    weekStartDate = weekStartDate.Date.AddDays(1);
        //                }
        //                else if (item.Week_Day == "Wednesday")
        //                {
        //                    weekStartDate = weekStartDate.Date.AddDays(2);
        //                }
        //                else if (item.Week_Day == "Thursday")
        //                {
        //                    weekStartDate = weekStartDate.Date.AddDays(3);
        //                }

        //                if (item.Week_Day == "Friday")
        //                {
        //                    weekStartDate = weekStartDate.Date.AddDays(4);
        //                }

        //                else if (item.Week_Day == "Saturday")
        //                {
        //                    weekStartDate = weekStartDate.Date.AddDays(5);
        //                }

        //                RS_AM_Operator_Station_Allocation.Allocation_Date = weekStartDate.Date;
        //                RS_AM_Operator_Station_Allocation.Plant_ID = plantID;
        //                RS_AM_Operator_Station_Allocation.Shop_ID = item.Shop_ID;
        //                RS_AM_Operator_Station_Allocation.Line_ID = item.Line_ID;
        //                RS_AM_Operator_Station_Allocation.Station_ID = item.Station_ID;
        //                RS_AM_Operator_Station_Allocation.Employee_ID = item.Employee_ID;
        //                RS_AM_Operator_Station_Allocation.Shift_ID = Shift;
        //                RS_AM_Operator_Station_Allocation.Inserted_Date = DateTime.Now;
        //                RS_AM_Operator_Station_Allocation.Inserted_User_ID = supervisorID;
        //                RS_AM_Operator_Station_Allocation.Inserted_Host = userHost;
        //                RS_AM_Operator_Station_Allocation.Week_Day = weekStartDate.DayOfWeek.ToString();
        //                RS_AM_Operator_Station_Allocation.Week_Number = weeks;
        //                RS_AM_Operator_Station_Allocation.Copy_Week_Number = copy_Week;
        //                db.RS_AM_Operator_Station_Allocation.Add(RS_AM_Operator_Station_Allocation);
        //                db.SaveChanges();
        //                //History
        //                RS_AM_Operator_Station_Allocation_History.Allocation_Date = weekStartDate.Date;
        //                RS_AM_Operator_Station_Allocation_History.Plant_ID = plantID;
        //                RS_AM_Operator_Station_Allocation_History.Shop_ID = item.Shop_ID;
        //                RS_AM_Operator_Station_Allocation_History.Line_ID = item.Line_ID;
        //                RS_AM_Operator_Station_Allocation_History.Station_ID = item.Station_ID;
        //                RS_AM_Operator_Station_Allocation_History.Employee_ID = item.Employee_ID;
        //                RS_AM_Operator_Station_Allocation_History.Shift_ID = Shift;
        //                RS_AM_Operator_Station_Allocation_History.Inserted_Date = DateTime.Now;
        //                RS_AM_Operator_Station_Allocation_History.Inserted_User_ID = supervisorID;
        //                RS_AM_Operator_Station_Allocation_History.Week_Day = weekStartDate.DayOfWeek.ToString();
        //                RS_AM_Operator_Station_Allocation_History.Week_Number = weeks;
        //                RS_AM_Operator_Station_Allocation_History.Inserted_Host = userHost;
        //                RS_AM_Operator_Station_Allocation_History.Copy_Week_Number = copy_Week;
        //                db.RS_AM_Operator_Station_Allocation_History.Add(RS_AM_Operator_Station_Allocation_History);
        //                db.SaveChanges();

        //                //Set to week starting date(monday date)
        //                weekStartDate = startDay;
        //            }
        //            return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);

        //        }
        //        else
        //        {
        //            return Json(new { msg = "Allready" }, JsonRequestBehavior.AllowGet);
        //            // Employee Assign to selected week
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;

        //        return Json(new { msg = "Error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult AssignOperatorToNextWeek(int? shopID, int? lineID, int copy_Week, DateTime fromDate, DateTime toDate, int Shift, string replace)
        {
            try
            {
                // string msg = string.Empty;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int supervisorID = ((FDSession)this.Session["FDSession"]).userId;
                string userHost = ((FDSession)this.Session["FDSession"]).userHost;
                RS_AM_Operator_Station_Allocation RS_AM_Operator_Station_Allocation = new RS_AM_Operator_Station_Allocation();
                RS_AM_Operator_Station_Allocation_History RS_AM_Operator_Station_Allocation_History = new RS_AM_Operator_Station_Allocation_History();
                DateTime weekStartDate = Convert.ToDateTime(fromDate);
                IEnumerable<object> obj = TempData.ContainsKey("operators") ? TempData["operators"] as IEnumerable<object> : null;
                TempData.Keep("operators");
                var data = obj;
                var cult = CultureInfo.CurrentCulture;
                var weekNo = cult.Calendar.GetWeekOfYear(new DateTime(fromDate.Year, fromDate.Month, fromDate.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);
                var preweekNo = weekNo - 1;
                int is_Emp_Available = db.RS_AM_Operator_Station_Allocation.Where(op => op.Shop_ID == shopID && op.Line_ID == lineID && op.Shift_ID == Shift && DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(toDate)).ToList().Count();
                int CheckPrevAllocation = db.RS_AM_Operator_Station_Allocation.Where(op => op.Shop_ID == shopID && op.Line_ID == lineID && op.Week_Number == preweekNo && op.Shift_ID == Shift).ToList().Count();
                TimeSpan day = toDate.Subtract(fromDate);
                double totalDays = day.TotalDays;

                if (obj != null && (is_Emp_Available == 0 || replace == "YES"))
                {
                    if (CheckPrevAllocation == 0)
                    {
                        return Json(new { msg = "Previous" }, JsonRequestBehavior.AllowGet);
                    }
                    if (replace == "YES")
                    {
                        ////remove allready exist weekly allocation
                        IEnumerable<RS_AM_Operator_Station_Allocation> operatop = (from op in db.RS_AM_Operator_Station_Allocation
                                                                                   where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(fromDate)
                                                                                   && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(toDate)
                                                                                   && op.Shift_ID == Shift
                                                                                   select op).ToList();
                        db.RS_AM_Operator_Station_Allocation.RemoveRange(operatop);
                        db.SaveChanges();

                        IEnumerable<RS_AM_Operator_Station_Allocation_History> operatop_history = (from op in db.RS_AM_Operator_Station_Allocation_History
                                                                                                   where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(fromDate)
                                                                                                   && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(toDate)
                                                                                                   && op.Shift_ID == Shift
                                                                                                   select op).ToList();
                        db.RS_AM_Operator_Station_Allocation_History.RemoveRange(operatop_history);
                        db.SaveChanges();
                    }

                    DateTime startDay = DateTime.MinValue;
                    foreach (dynamic item in data)
                    {
                        RS_AM_Operator_Station_Allocation = new RS_AM_Operator_Station_Allocation();
                        RS_AM_Operator_Station_Allocation_History = new RS_AM_Operator_Station_Allocation_History();
                        startDay = weekStartDate;
                        weekNo = cult.Calendar.GetWeekOfYear(new DateTime(weekStartDate.Year, weekStartDate.Month, weekStartDate.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);
                        if (item.Week_Day == "Tuesday")
                        {
                            weekStartDate = weekStartDate.Date.AddDays(1);
                        }
                        else if (item.Week_Day == "Wednesday")
                        {
                            weekStartDate = weekStartDate.Date.AddDays(2);
                        }
                        else if (item.Week_Day == "Thursday")
                        {
                            weekStartDate = weekStartDate.Date.AddDays(3);
                        }

                        if (item.Week_Day == "Friday")
                        {
                            weekStartDate = weekStartDate.Date.AddDays(4);
                        }

                        else if (item.Week_Day == "Saturday")
                        {
                            weekStartDate = weekStartDate.Date.AddDays(5);
                        }
                        else if (item.Week_Day == "Sunday")
                        {
                            weekStartDate = weekStartDate.Date.AddDays(6);
                        }
                        for (int i = 0; i < totalDays; i++)
                        {
                            if (toDate.Year >= weekStartDate.Year && toDate.Month >= weekStartDate.Month && toDate.Day >= weekStartDate.Day)
                            {
                                RS_AM_Operator_Station_Allocation.Allocation_Date = weekStartDate.Date;
                                RS_AM_Operator_Station_Allocation.Plant_ID = plantID;
                                RS_AM_Operator_Station_Allocation.Shop_ID = item.Shop_ID;
                                RS_AM_Operator_Station_Allocation.Line_ID = item.Line_ID;
                                RS_AM_Operator_Station_Allocation.Station_ID = item.Station_ID;
                                RS_AM_Operator_Station_Allocation.Employee_ID = item.Employee_ID;
                                RS_AM_Operator_Station_Allocation.Shift_ID = Shift;
                                RS_AM_Operator_Station_Allocation.Inserted_Date = DateTime.Now;
                                RS_AM_Operator_Station_Allocation.Inserted_User_ID = supervisorID;
                                RS_AM_Operator_Station_Allocation.Inserted_Host = userHost;
                                RS_AM_Operator_Station_Allocation.Week_Day = weekStartDate.DayOfWeek.ToString();
                                RS_AM_Operator_Station_Allocation.Week_Number = weekNo;
                                RS_AM_Operator_Station_Allocation.Copy_Week_Number = copy_Week;
                                db.RS_AM_Operator_Station_Allocation.Add(RS_AM_Operator_Station_Allocation);
                                db.SaveChanges();
                                //History
                                RS_AM_Operator_Station_Allocation_History.Allocation_Date = weekStartDate.Date;
                                RS_AM_Operator_Station_Allocation_History.Plant_ID = plantID;
                                RS_AM_Operator_Station_Allocation_History.Shop_ID = item.Shop_ID;
                                RS_AM_Operator_Station_Allocation_History.Line_ID = item.Line_ID;
                                RS_AM_Operator_Station_Allocation_History.Station_ID = item.Station_ID;
                                RS_AM_Operator_Station_Allocation_History.Employee_ID = item.Employee_ID;
                                RS_AM_Operator_Station_Allocation_History.Shift_ID = Shift;
                                RS_AM_Operator_Station_Allocation_History.Inserted_Date = DateTime.Now;
                                RS_AM_Operator_Station_Allocation_History.Inserted_User_ID = supervisorID;
                                RS_AM_Operator_Station_Allocation_History.Week_Day = weekStartDate.DayOfWeek.ToString();
                                RS_AM_Operator_Station_Allocation_History.Week_Number = weekNo;
                                RS_AM_Operator_Station_Allocation_History.Inserted_Host = userHost;
                                RS_AM_Operator_Station_Allocation_History.Copy_Week_Number = copy_Week;
                                db.RS_AM_Operator_Station_Allocation_History.Add(RS_AM_Operator_Station_Allocation_History);
                                db.SaveChanges();
                                weekStartDate = weekStartDate.Date.AddDays(7);
                            }
                            else
                            {
                                break;
                            }
                        }

                        //Set to week starting date(monday date)
                        weekStartDate = startDay;
                    }
                    return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { msg = "Allready" }, JsonRequestBehavior.AllowGet);
                    // Employee Assign to selected week
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(new { msg = "Error" }, JsonRequestBehavior.AllowGet);
            }


        }


        public void saveAssociateAllocationDetails(int shopId, int lineId, int droppedStationId, decimal shiftid, decimal employeeId, int weekNo, string day, DateTime allocation_Date, bool isOJT, int SetupID)
        {

            var pStationId = db.RS_Stations.Where(m => m.Is_Buffer == true && m.Line_ID == lineId).Select(m => m.Station_ID).FirstOrDefault();
            //var ChangeStationId = Convert.ToDecimal(droppedStationId);
            //decimal employee_Id = db.RS_Employee.Where(e => e.Employee_No == employeeNo).Select(e => e.Employee_ID).FirstOrDefault();
            decimal ExistRecord = db.RS_AM_Operator_Station_Allocation.Where(c => c.Employee_ID == employeeId && c.Station_ID == pStationId && c.Allocation_Date == allocation_Date && c.Shift_ID == shiftid && c.Shop_ID == shopId && c.Setup_ID == SetupID).Select(c => c.OSM_ID).FirstOrDefault();

            RS_AM_Operator_Station_Allocation mmAllocationObj = new RS_AM_Operator_Station_Allocation();
            RS_AM_Operator_Station_Allocation_History mmAllocationHistoryObj = new RS_AM_Operator_Station_Allocation_History();
            var stationTypeID = db.RS_Stations.Where(m => m.Station_ID == droppedStationId && m.Line_ID == lineId).Select(m => m.Station_Type_ID).FirstOrDefault();
            mmAllocationObj = db.RS_AM_Operator_Station_Allocation.Find(ExistRecord);
            mmAllocationObj.Station_ID = droppedStationId;
            mmAllocationObj.Week_Day = day.ToString();
            mmAllocationObj.Week_Number = weekNo;
            mmAllocationObj.Is_OJT_Operator = isOJT;
            mmAllocationObj.Is_Buffer_Station = false;
            if (isOJT != true)
                mmAllocationObj.Station_Type_ID = stationTypeID;
            // obj.OSM_ID = ExistRecord;
            db.Entry(mmAllocationObj).State = EntityState.Modified;
            db.SaveChanges();

            if (isOJT == true)
            {
                RS_On_Job_Training_Data obj = new RS_On_Job_Training_Data();
                obj.Plant_ID = mmAllocationObj.Plant_ID;
                obj.Shop_ID = shopId;
                obj.Line_ID = lineId;
                obj.Station_ID = droppedStationId;
                obj.Setup_ID = SetupID;
                obj.Shift_ID = shiftid;
                obj.Employee_ID = employeeId;
                obj.OJT_Date = allocation_Date;
                obj.Inserted_Date = DateTime.Now;
                obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.RS_On_Job_Training_Data.Add(obj);
                db.SaveChanges();
            }


            mmAllocationHistoryObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            mmAllocationHistoryObj.Shop_ID = shopId;
            mmAllocationHistoryObj.Line_ID = lineId;
            mmAllocationHistoryObj.Shift_ID = shiftid;
            mmAllocationHistoryObj.Station_ID = droppedStationId;
            mmAllocationHistoryObj.Setup_ID = mmAllocationObj.Setup_ID;
            mmAllocationHistoryObj.Is_Transferred = false;
            mmAllocationHistoryObj.Is_Purgeable = false;
            mmAllocationHistoryObj.Employee_ID = employeeId;
            mmAllocationHistoryObj.Allocation_Date = allocation_Date;
            mmAllocationHistoryObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
            mmAllocationHistoryObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

            mmAllocationHistoryObj.Inserted_Date = DateTime.Now;
            mmAllocationHistoryObj.Prev_Line_ID = lineId;
            mmAllocationHistoryObj.Prev_Shop_ID = shopId;
            mmAllocationHistoryObj.Prev_Station_ID = pStationId;
            mmAllocationHistoryObj.Is_Transfer_Operator = true;
            mmAllocationHistoryObj.Is_OJT_Operator = isOJT;
            mmAllocationHistoryObj.Week_Number = weekNo;
            mmAllocationHistoryObj.Week_Day = day.ToString();
            if (isOJT != true)
                mmAllocationHistoryObj.Station_Type_ID = stationTypeID;
            db.RS_AM_Operator_Station_Allocation_History.Add(mmAllocationHistoryObj);
            db.SaveChanges();
        }


        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        public ActionResult CheckIsValidSkillForCriticalStation(string Station_Id, string Employee_No, int Shop_Id, int Line_Id)
        {
            var bufferStation = false;
            try
            {
                int droppedStationId = 0;

                if (Station_Id == "extra_operator_list")
                {
                    var stationId = db.RS_Stations.Where(m => m.Shop_ID == Shop_Id && m.Line_ID == Line_Id && m.Is_Buffer == true).Select(m => m.Station_ID).FirstOrDefault();
                    droppedStationId = Convert.ToInt32(stationId);
                    bufferStation = true;
                }
                else
                {
                    string[] stat = Station_Id.Split('_');
                    droppedStationId = Convert.ToInt16(stat[1]);
                }

                string[] empno = Employee_No.Split('_');
                Employee_No = empno.Length > 2 ? empno[3] : empno[0];

                var Employee_Id = db.RS_Employee.Where(c => c.Employee_No == Employee_No).Select(c => c.Employee_ID).FirstOrDefault();
                var isCriticalStation = db.RS_Stations.Where(c => c.Station_ID == droppedStationId).Select(c => c.Is_Critical_Station).FirstOrDefault();
                if (isCriticalStation == true)
                {
                    var IsValidSkillSet = db.RS_AM_Employee_SkillSet.Where(c => c.Employee_ID == Employee_Id).Select(c => c.Skill_ID).FirstOrDefault();
                    if (IsValidSkillSet >= 3)
                    {
                        return Json(new { bs = bufferStation, msg = true, success = "True" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { bs = bufferStation, msg = false, success = "True" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new { bs = bufferStation, msg = true, success = "True" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { bs = bufferStation, msg = false, success = "False" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveExternalTrainingData(int ShopId, int LineId, int StationId, int SetupId, int ShiftId, string EmpNo, string Name, string Desc)
        {
            try
            {
                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                var empId = db.RS_Employee.Where(m => m.Employee_No == EmpNo).Select(m => m.Employee_ID).FirstOrDefault();
                RS_External_Training_Data extObj = new RS_External_Training_Data();
                var ID = db.RS_External_Training_Data.Where(m => m.Employee_ID == empId && m.Station_ID == StationId && m.Setup_ID == SetupId && m.Training_Date == DateTime.Today).Select(m => m.ET_ID).FirstOrDefault();
                if (ID > 0)
                {
                    extObj = db.RS_External_Training_Data.Find(ID);
                    extObj.Training_Name = Name;
                    extObj.Training_Description = Desc;
                    db.Entry(extObj).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    extObj.Plant_ID = plantId;
                    extObj.Shop_ID = ShopId;
                    extObj.Line_ID = LineId;
                    extObj.Station_ID = StationId;
                    extObj.Setup_ID = SetupId;
                    extObj.Shift_ID = ShiftId;
                    extObj.Employee_ID = empId;
                    extObj.Training_Name = Name;
                    extObj.Training_Description = Desc;
                    extObj.Training_Date = DateTime.Today;
                    extObj.Inserted_Date = DateTime.Now;
                    extObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    extObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.RS_External_Training_Data.Add(extObj);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExcelUpload()
        {
            try
            {

                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");

                List<SelectListItem> listModel = new List<SelectListItem>();
                ViewBag.Line_ID = new SelectList(listModel);
                ViewBag.Shift_ID = new SelectList(db.RS_Shift.Where(m => m.Plant_ID == plantId), "Shift_ID", "Shift_Name");
                if (TempData["OrderUploadRecords"] != null)
                {
                    ViewBag.OrderUploadRecords = TempData["OrderUploadRecords"];
                }

                globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.Station_Allocation + " " + ResourceGlobal.Form;
                globalData.subTitle = ResourceGlobal.Upload;
                globalData.controllerName = "DailyReportForSupervisor";
                globalData.actionName = ResourceGlobal.Upload;
                globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Station_Allocation + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Station_Allocation + " " + ResourceGlobal.Form;
                ViewBag.GlobalDataModel = globalData;

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelUpload(ExcelStationAllocation formData)
        {
            try
            {


                int plantId = 0, shopId = 0, lineId = 0, shiftId = 0;
                String createdOrders = "";
                DateTime FromDate, ToDate;
                if (ModelState.IsValid)
                {
                    GlobalOperations globalOperations = new GlobalOperations();
                    string fileName = Path.GetFileName(formData.Excel_File.FileName);
                    string fileExtension = Path.GetExtension(formData.Excel_File.FileName);
                    string fileLocation = Server.MapPath("~/App_Data/" + fileName);
                    DataTable dt = ExcelToDataTable(formData.Excel_File, fileLocation, fileExtension);
                    //String attributeId = formData.Attribute_ID;

                    plantId = Convert.ToInt32(formData.Plant_ID);
                    shopId = Convert.ToInt32(formData.Shop_ID);
                    lineId = Convert.ToInt32(formData.Line_ID);
                    FromDate = Convert.ToDateTime(formData.From_Date);
                    ToDate = Convert.ToDateTime(formData.To_Date);
                    shiftId = Convert.ToInt32(formData.Shift_ID);
                    if (dt.Rows.Count > 0)
                    {
                        StationAllocationUploadRecords[] orderUploadRecordsObj = new StationAllocationUploadRecords[dt.Rows.Count];

                        int i = 0;
                        foreach (DataRow checkListRow in dt.Rows)
                        {
                            String stationName = checkListRow[0].ToString() != null ? checkListRow[0].ToString().Trim() : null;
                            String OpratorTokenNumber = checkListRow[1].ToString() != null ? checkListRow[1].ToString().Trim() : null;
                            if (!string.IsNullOrEmpty(stationName) && !string.IsNullOrEmpty(OpratorTokenNumber))
                            {
                                orderUploadRecordsObj[i] = new StationAllocationUploadRecords();
                                StationAllocationUploadRecords orderUploadObj = new StationAllocationUploadRecords();

                                RS_AM_Operator_Station_Allocation obj = new RS_AM_Operator_Station_Allocation();
                                orderUploadObj.StationName = stationName;
                                orderUploadObj.OperatorTokenNumber = OpratorTokenNumber;

                                var station = db.RS_Stations.Any(m => m.Station_Name == stationName && m.Line_ID == lineId);
                                if (station)
                                {
                                    var stationId = db.RS_Stations.Where(m => m.Station_Name == stationName && m.Line_ID == lineId).Select(m => m.Station_ID).FirstOrDefault();
                                    var userId = ((FDSession)this.Session["FDSession"]).userId;
                                    var supervisorId = db.RS_AM_Line_Supervisor_Mapping.Where(m => m.Line_ID == lineId && m.Employee_ID == userId).Select(m => m.Employee_ID).FirstOrDefault();
                                    var operatorIds = db.RS_Assign_OperatorToSupervisor.Where(m => m.Line_ID == lineId && m.Supervisor_ID == supervisorId).Select(m => m.AssignedOperator_ID).ToArray();
                                    string[] EmpNos = db.RS_Employee.Where(c => operatorIds.Contains(c.Employee_ID)).Select(c => c.Employee_No).ToArray();
                                    var results = Array.FindAll(EmpNos, s => s.Equals(OpratorTokenNumber));
                                    if (results.Count() > 0)
                                    {
                                        var EmpId = db.RS_Employee.Where(m => m.Employee_No == OpratorTokenNumber).Select(m => m.Employee_ID).FirstOrDefault();
                                        var datediff = (ToDate - FromDate).TotalDays;
                                        if (FromDate <= DateTime.Today && ToDate >= DateTime.Today)
                                        {
                                            for (int j = 0; j <= datediff; j++)
                                            {
                                                var date = FromDate.AddDays(j);
                                                var isAllocate = db.RS_AM_Operator_Station_Allocation.Where(m => m.Station_ID == stationId && m.Shift_ID == shiftId && m.Allocation_Date == date && m.Employee_ID == EmpId);
                                                if (isAllocate.Count() == 0)
                                                {
                                                    obj.Plant_ID = plantId;
                                                    obj.Line_ID = lineId;
                                                    obj.Shop_ID = shopId;
                                                    obj.Station_ID = stationId;
                                                    obj.Shift_ID = shiftId;
                                                    obj.Employee_ID = EmpId;
                                                    obj.Allocation_Date = date;
                                                    obj.Inserted_Date = DateTime.Now;
                                                    obj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                                    //obj.Week_Day =Convert.ToString(date.DayOfWeek);

                                                    db.RS_AM_Operator_Station_Allocation.Add(obj);
                                                    db.SaveChanges();
                                                    orderUploadObj.SS_Error_Sucess = "Record saved sucessfully..!";
                                                }
                                                else
                                                {
                                                    orderUploadObj.SS_Error_Sucess = "Error:Operator is already allocated to station";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        orderUploadObj.SS_Error_Sucess = "Error:Operator is not available";
                                    }
                                }
                                else
                                {
                                    orderUploadObj.SS_Error_Sucess = "Error:Station is not available";
                                }

                                orderUploadRecordsObj[i] = orderUploadObj;
                                i = i + 1;
                            }
                        }


                        TempData["OrderUploadRecords"] = orderUploadRecordsObj;
                        //TempData["ChecklistDataTable"] = dt;
                        ViewBag.OrderUploadRecords = orderUploadRecordsObj;
                        //ViewBag.dt = qualityChecklistDt;
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceModules.Station_Allocation;
                        globalData.messageDetail = ResourceGlobal.Excel + " " + ResourceMessages.Upload_Success;
                        globalData.pageTitle = ResourceGlobal.Excel + " " + ResourceModules.Station_Allocation + " " + ResourceGlobal.Form;
                        globalData.subTitle = ResourceGlobal.Upload;
                        globalData.controllerName = "DailyReportForSupervisor";
                        globalData.actionName = ResourceGlobal.Upload;
                        globalData.contentTitle = ResourceGlobal.Excel + " " + ResourceModules.Station_Allocation + " " + ResourceGlobal.Form;
                        globalData.contentFooter = ResourceGlobal.Excel + " " + ResourceModules.Station_Allocation + " " + ResourceGlobal.Form;
                        ViewBag.GlobalDataModel = globalData;

                        ViewBag.createdOrders = createdOrders;
                    }
                }
                //return PartialView("QualityChecklistDetails");

                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", shopId);
                ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", lineId);
                ViewBag.Shift_ID = new SelectList(db.RS_Shift.Where(m => m.Plant_ID == plantId), "Shift_ID", "Shift_Name");

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }

        public DataTable ExcelToDataTable(HttpPostedFileBase uploadFile, string fileLocation, string fileExtension)
        {
            DataTable dtExcelRecords = new DataTable();
            string connectionString = "";
            if (uploadFile.ContentLength > 0)
            {
                uploadFile.SaveAs(fileLocation);

                //Check whether file extension is xls or xslx

                if (fileExtension == ".xls")
                {
                    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                else if (fileExtension == ".xlsx")
                {
                    connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }

                //Create OleDB Connection and OleDb Command && Read data from excel and generate datatable 

                OleDbConnection con = new OleDbConnection(connectionString);
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = con;
                OleDbDataAdapter dAdapter = new OleDbDataAdapter(cmd);

                con.Open();
                DataTable dtExcelSheetName = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string getExcelSheetName = dtExcelSheetName.Rows[0]["Table_Name"].ToString();
                cmd.CommandText = "SELECT * FROM [" + getExcelSheetName + "]";
                dAdapter.SelectCommand = cmd;
                dAdapter.Fill(dtExcelRecords);
                con.Close();

            }
            return dtExcelRecords;
        }


        [HttpPost]
        public ActionResult publishstations(int Lineid, int Shopid, int Shiftid, int setupid)
        {
            try
            {
                var setupID = setupid;
                var dashid = (from dash in db.RS_Dashboard_Master
                              where dash.Line_ID == Lineid && dash.Shop_ID == Shopid && dash.Shift_ID == Shiftid
                              select dash.Dash_ID).Distinct();


                RS_Dashboard_Master dashobj1 = new RS_Dashboard_Master();
                foreach (var dash in dashid)
                {
                    dashobj1 = db.RS_Dashboard_Master.Find(dash);
                    dashobj1.Shop_ID = Shopid;
                    dashobj1.Line_ID = Lineid;
                    dashobj1.Setup_ID = setupID;


                    dashobj1.Shift_ID = Shiftid;

                    db.Entry(dashobj1).State = EntityState.Modified;

                }

                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportData(int ShopId, int? LineId, int? SetupId)
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                DataSet ds = new DataSet();
                {
                    var Shop_Name = db.RS_Shops.Where(m => m.Shop_ID == ShopId).Select(m => m.Shop_Name).FirstOrDefault();
                    var Line_Name = db.RS_Lines.Where(m => m.Line_ID == LineId).Select(m => m.Line_Name).FirstOrDefault();
                    var Setup_Name = db.RS_Setup.Where(m => m.Setup_ID == SetupId).Select(m => m.Setup_Name).FirstOrDefault();


                    con.Open();
                    SqlCommand cmd = new SqlCommand("SP_ExportData_Allocation", con);
                    if (Shop_Name != "" && (Line_Name == "" || Line_Name == null) && (Setup_Name == null || Setup_Name == ""))
                    {
                        cmd = new SqlCommand("SP_ExportData_Allocation", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Shop_Name", Shop_Name);
                        cmd.Parameters.AddWithValue("@Line_Name", null);
                        cmd.Parameters.AddWithValue("@Setup_Name", null);
                    }
                    else if (Shop_Name != "" && Line_Name != "" && (Setup_Name == null || Setup_Name == ""))
                    {
                        cmd = new SqlCommand("SP_ExportData_Allocationbyshopline", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Shop_Name", Shop_Name);
                        cmd.Parameters.AddWithValue("@Line_Name", Line_Name);
                        cmd.Parameters.AddWithValue("@Setup_Name", null);
                    }
                    else if (ShopId > 0 && LineId > 0 && SetupId > 0)
                    {
                        cmd = new SqlCommand("SP_ExportData_Allocation_Shop_Line_Setup1", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Shop_Name", Shop_Name);
                        cmd.Parameters.AddWithValue("@Line_Name", Line_Name);
                        cmd.Parameters.AddWithValue("@Setup_Name", Setup_Name);
                    }
                    //cmd.Parameters.AddWithValue("@Plant", plantID);


                    SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                    DataTable dt1 = new DataTable("All Configuration");

                    da1.Fill(dt1);
                    con.Close();
                    ds.Tables.Add(dt1);
                    //dt1.Columns["Plant_Name"].ColumnName = "Plant Name";
                    //dt1.Columns["Shop_Name"].ColumnName = "Shop Name";
                    //dt1.Columns["shiftName"].ColumnName = "Shift Name";
                    //dt1.Columns["ConsumptionDate"].ColumnName = "Consumption Date";
                    //dt1.Columns["totalconsumption"].ColumnName = "Total Consumption";

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
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RS_All_Configuration.xlsx");
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return RedirectToAction("Index");
            }
        }

        public ActionResult ExportAbsEmployee(int ShopId, int LineId)
        {
            var line1 = (from line in db.RS_Lines
                         where line.Shop_ID == ShopId && line.Line_ID != LineId
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name
                         }
                         ).ToList();
            return Json(line1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAbsOperatorByShiftID(int shiftId, int? shopId, int? lineId, int? setupId)//
        {
            try
            {
                int plant_Id = ((FDSession)this.Session["FDSession"]).plantId;


                //var st = (from emp in db.RS_AM_Operator_Station_Allocation
                //          join emp1 in db.RS_Employee on emp.Employee_ID equals emp1.Employee_ID
                //          join stn in db.RS_Stations on emp.Station_ID equals stn.Station_ID
                //          join line in db.RS_Lines on emp.Line_ID equals line.Line_ID
                //          join emp2 in db.RS_User_Attendance_Sheet on emp1.Employee_No equals emp2.Employee_No
                //          where emp1.Plant_ID == plant_Id && emp1.Line_ID == lineId && emp1.Shop_ID == shopId && emp1.Shift_ID == shiftId && emp.Allocation_Date == DateTime.Today && stn.Is_Buffer_Station == true && emp2.Entry_Date == DateTime.Today && emp.Setup_ID==setupId





                //          select new
                //          {
                //              Id = emp.Employee_ID,
                //              Value = emp.RS_Employee.Employee_Name
                //          }).Distinct().ToList();

                var str4 = (from a in db.RS_Employee
                                //join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
                            join c in db.RS_User_Attendance_Sheet on a.Employee_No equals c.Employee_No
                            join d in db.RS_AM_Operator_Station_Allocation on a.Employee_ID equals d.Employee_ID

                            where d.Line_ID == lineId && d.Shift_ID == shiftId && d.Setup_ID == setupId && d.Is_Buffer_Station == true && d.Allocation_Date == DateTime.Today
                                    && c.Is_Present == true && (c.Entry_Date.Value.Year == DateTime.Now.Year
                                            && c.Entry_Date.Value.Month == DateTime.Now.Month
                                            && c.Entry_Date.Value.Day == DateTime.Now.Day)

                            //  && !(from l in db.MM_AM_LeaveManagement where l.Plant_ID == plant_ID select l.Employee_ID).Contains(a.Employee_ID)
                            //orderby d.Skill_ID descending
                            select new
                            {
                                Id = a.Employee_ID,
                                Value = a.Employee_Name,
                                Employee_No = a.Employee_No,
                                //Image_Content = a.Image_Content,
                                //StationId = stationId
                                //d.Skill_ID
                                //a.Line_ID,
                                //d.Line_Name,
                                //a.Station_Name
                            }).Distinct().ToList();



                var str5 = str4.Select(x => new { x.Id, x.Value, x.Employee_No }).Distinct();







                return Json(str5, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                    return RedirectToAction("Index", "User");
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveTranserAbsOperators(string Operators, int shiftId, int shopId, int lineId, int nlineId)
        {
            try
            {

                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                RS_Abs_Operator_Transfer_Allocation RS_Abs_Operator_Transfer_Allocation = new RS_Abs_Operator_Transfer_Allocation();

                //plantId, 


                string[] words;
                words = Operators.Split(',');

                foreach (string value in words)
                {
                    int i = 0;
                    if (value == "")
                    {
                        i = 0;
                    }
                    else
                    {
                        i = Convert.ToInt32(value);
                    }
                    if (i == 0)
                        continue;



                    var allocationDate = DateTime.Today;
                    var prevstationId = db.RS_Stations.Where(m => m.Line_ID == lineId && m.Is_Buffer == true).Select(m => m.Station_ID).FirstOrDefault();
                    var newstationId = db.RS_Stations.Where(m => m.Line_ID == nlineId && m.Is_Buffer == true).Select(m => m.Station_ID).FirstOrDefault();



                    var operatorlist = db.RS_AM_Operator_Station_Allocation.Where(p => p.Shift_ID == shiftId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Plant_ID == plantId && p.Station_ID == prevstationId && p.Employee_ID == i && p.Allocation_Date == DateTime.Today);//&& p.plant_id == plantid
                    foreach (var item in operatorlist.ToList())

                    {
                        db.RS_AM_Operator_Station_Allocation.Remove(item);
                        db.SaveChanges();
                    }
                    var operatorlist1 = db.RS_AM_Operator_Station_Allocation_History.Where(p => p.Shift_ID == shiftId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Plant_ID == plantId && p.Station_ID == prevstationId && p.Employee_ID == i && p.Allocation_Date == DateTime.Today);//&& p.plant_id == plantid
                    foreach (var item1 in operatorlist1.ToList())

                    {
                        db.RS_AM_Operator_Station_Allocation_History.Remove(item1);
                        db.SaveChanges();
                    }



                    RS_AM_Operator_Station_Allocation ops = new RS_AM_Operator_Station_Allocation();
                    RS_AM_Operator_Station_Allocation_History mm_OPH = new RS_AM_Operator_Station_Allocation_History();
                    var setupids = db.RS_Setup.Where(m => m.Line_ID == nlineId).Select(m => m.Setup_ID).ToArray();

                    foreach (var item in setupids)
                    {
                        var IfExist = db.RS_AM_Operator_Station_Allocation.Where(m => m.Line_ID == nlineId && m.Shop_ID == shopId && m.Setup_ID == item && m.Allocation_Date == DateTime.Today && m.Employee_ID == i).Select(m => m.OSM_ID).FirstOrDefault();




                        if (IfExist == 0)


                        {
                            ops.Plant_ID = plantId;
                            ops.Shop_ID = shopId;
                            ops.Line_ID = nlineId;
                            ops.Shift_ID = shiftId;
                            ops.Employee_ID = i;
                            ops.Setup_ID = item;
                            ops.Station_ID = newstationId;
                            ops.Allocation_Date = DateTime.Today;
                            ops.Is_Buffer_Station = true;
                            ops.Is_Purgeable = false;
                            ops.Is_Transferred = false;
                            ops.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            ops.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            ops.Inserted_Date = DateTime.Today;
                            db.RS_AM_Operator_Station_Allocation.Add(ops);
                            db.SaveChanges();


                        }
                        else
                        {
                            ops = db.RS_AM_Operator_Station_Allocation.Find(IfExist);
                            ops.Plant_ID = plantId;
                            ops.Shop_ID = shopId;
                            ops.Line_ID = nlineId;
                            ops.Shift_ID = shiftId;
                            ops.Employee_ID = i;
                            ops.Setup_ID = item;
                            ops.Station_ID = newstationId;
                            ops.Allocation_Date = DateTime.Today;
                            ops.Is_Buffer_Station = true;
                            ops.Is_Purgeable = false;
                            ops.Is_Transferred = false;
                            ops.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            ops.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            ops.Updated_Date = DateTime.Today;
                            db.Entry(ops).State = EntityState.Modified;
                            db.SaveChanges();
                            //Operator_Station_History_Alloacation






                        }
                    }
                    foreach (var item2 in setupids)
                    {
                        var IfExist1 = db.RS_AM_Operator_Station_Allocation_History.Where(m => m.Line_ID == nlineId && m.Shop_ID == shopId && m.Setup_ID == item2 && m.Allocation_Date == DateTime.Today && m.Employee_ID == i).Select(m => m.Row_ID).FirstOrDefault();
                        //var IfExist1 = db.RS_AM_Operator_Station_Allocation_History.Where(m => m.Line_ID == nlineId && m.Shop_ID == shopId && m.Setup_ID == item && m.Allocation_Date == DateTime.Today && m.Employee_ID == i).Select(m => m.Row_ID).ToList();
                        if (IfExist1 == 0)
                        {


                            mm_OPH.Plant_ID = plantId;
                            mm_OPH.Shop_ID = shopId;
                            mm_OPH.Line_ID = nlineId;
                            mm_OPH.Shift_ID = shiftId;
                            mm_OPH.Employee_ID = i;
                            mm_OPH.Setup_ID = item2;
                            mm_OPH.Station_ID = newstationId;
                            mm_OPH.Allocation_Date = DateTime.Today;
                            mm_OPH.Is_Buffer_Station = true;
                            mm_OPH.Is_Purgeable = false;
                            mm_OPH.Is_Transferred = false;
                            mm_OPH.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            mm_OPH.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mm_OPH.Inserted_Date = DateTime.Today;
                            db.RS_AM_Operator_Station_Allocation_History.Add(mm_OPH);
                            db.SaveChanges();


                        }
                        else
                        {

                            mm_OPH = db.RS_AM_Operator_Station_Allocation_History.Find((IfExist1));

                            mm_OPH.Plant_ID = plantId;
                            mm_OPH.Shop_ID = shopId;
                            mm_OPH.Line_ID = nlineId;
                            mm_OPH.Shift_ID = shiftId;
                            mm_OPH.Employee_ID = i;
                            mm_OPH.Setup_ID = item2;
                            mm_OPH.Station_ID = newstationId;
                            mm_OPH.Allocation_Date = DateTime.Today;
                            mm_OPH.Is_Buffer_Station = true;
                            mm_OPH.Is_Purgeable = false;
                            mm_OPH.Is_Transferred = false;
                            mm_OPH.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            mm_OPH.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mm_OPH.Updated_Date = DateTime.Today;
                            db.Entry(mm_OPH).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }


                    var transferExitst = db.RS_Abs_Operator_Transfer_Allocation.Where(m => m.New_Line_ID == nlineId && m.Allocation_Date == DateTime.Today && m.New_Shop_ID == shopId && m.Employee_ID == i).Select(m => m.Row_ID).FirstOrDefault();
                    if (transferExitst > 0)
                    {
                        RS_Abs_Operator_Transfer_Allocation = db.RS_Abs_Operator_Transfer_Allocation.Find(transferExitst);


                        RS_Abs_Operator_Transfer_Allocation.Plant_ID = plantId;
                        RS_Abs_Operator_Transfer_Allocation.Old_Shop_ID = shopId;
                        RS_Abs_Operator_Transfer_Allocation.Old_Line_ID = lineId;
                        RS_Abs_Operator_Transfer_Allocation.New_Shop_ID = shopId;
                        RS_Abs_Operator_Transfer_Allocation.New_Line_ID = nlineId;
                        RS_Abs_Operator_Transfer_Allocation.Inserted_Date = DateTime.Now;
                        RS_Abs_Operator_Transfer_Allocation.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Abs_Operator_Transfer_Allocation.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        RS_Abs_Operator_Transfer_Allocation.Shift_ID = shiftId;
                        RS_Abs_Operator_Transfer_Allocation.Employee_ID = i;
                        RS_Abs_Operator_Transfer_Allocation.Allocation_Date = allocationDate;

                        db.Entry(RS_Abs_Operator_Transfer_Allocation).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    else
                    {
                        RS_Abs_Operator_Transfer_Allocation.Plant_ID = plantId;
                        RS_Abs_Operator_Transfer_Allocation.Old_Shop_ID = shopId;
                        RS_Abs_Operator_Transfer_Allocation.Old_Line_ID = lineId;
                        RS_Abs_Operator_Transfer_Allocation.New_Shop_ID = shopId;
                        RS_Abs_Operator_Transfer_Allocation.New_Line_ID = nlineId;
                        RS_Abs_Operator_Transfer_Allocation.Inserted_Date = DateTime.Now;
                        RS_Abs_Operator_Transfer_Allocation.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        RS_Abs_Operator_Transfer_Allocation.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        RS_Abs_Operator_Transfer_Allocation.Shift_ID = shiftId;
                        RS_Abs_Operator_Transfer_Allocation.Employee_ID = i;
                        RS_Abs_Operator_Transfer_Allocation.Allocation_Date = allocationDate;


                        db.RS_Abs_Operator_Transfer_Allocation.Add(RS_Abs_Operator_Transfer_Allocation);
                        db.SaveChanges();


                    }



                    i = 0;
                }
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
    }
}