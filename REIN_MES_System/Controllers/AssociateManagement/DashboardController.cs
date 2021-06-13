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
using System.Web.Routing;
using System.Data.OleDb;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class DashboardController : BaseController
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
        // Author & Time Stamp  : Jitendra Mahajan
        // Description          : Action used to show the line, allow the user to configure line
        //////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // GET: /RouteConfiguration/
        public ActionResult Index()

      {

           // string hostAddress;
           // string hostName1 = System.Net.Dns.GetHostEntry(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]).HostName;
           //// string hostName2 = System.Net.Dns.GetHostEntry(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_USER"]).HostName;
            //hostAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            //string entry = Dns.GetHostEntry(hostAddress).HostName;
            //string[] entry1 = entry.Split(new char[] { '.' });
            // string hostname = entry1[0].ToLower();
            //string[] computer_name = System.Net.Dns.GetHostEntry(System.Web.HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
            //string hostname = computer_name[0].ToString();
             /// string ip_address="";
             //hostAddress = System.Web.HttpContext.Current.Request.UserHostName;
             //string entry = Dns.GetHostEntry(hostAddress).HostName;
             //string[] entry1 = entry.Split(new char[] { '.' });
             //string hostname = entry1[0].ToLower();
             //hostAddress = System.Web.HttpContext.Current.Request.UserHostName;
             // string entry = Dns.GetHostEntry(hostAddress).AddressList;

             string ip_address ="";
            if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"]!=null)
            {
                ip_address = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                ip_address = Request.UserHostAddress;
            }

            //hostAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            //string entry = Dns.GetHostEntry(hostAddress).HostName;
            //string[] entry1 = entry.Split(new char[] { '.' });
            //string hostname123 = entry1[0].ToLower();


            ViewBag.ip_address = ip_address;   
            //ViewBag.host_name = hostName1;         
            //ViewBag.hostName2 = entry;
            //  string ip_address1=

            TimeSpan currDate = DateTime.Now.TimeOfDay;
            var shiftObj = (from shift in db.RS_Shift
                            where
                             TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                            && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                            select shift).ToList();
            var S_Id = shiftObj[0].Shift_ID;
            //S_Id = 3;
            //string hostName = Dns.GetHostName();
            //string myIp = Dns.GetHostByName(hostName).AddressList[0].ToString();

            //string ipAddress = Dns.GetHostAddresses(Dns.GetHostName()).First(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();

            var dashbord = (from dash in db.RS_Dashboard_Master
                            where dash.Host_Name == ip_address && dash.Shift_ID == S_Id
                            select new
                            {
                                dash.Host_Name,
                                dash.Line_ID,
                                dash.Shop_ID,
                                dash.Shift_ID,
                                dash.Plant_ID,
                                dash.Setup_ID
                            }).ToList();

            
            


            //RS_Dashboard_Master Machine = new RS_Dashboard_Master();
            RS_Dashboard_Master Machine = new RS_Dashboard_Master();
            if (dashbord.Count == 0)
            {
                RedirectToAction("Home");
                globalData.isErrorMessage = true;
                //globalData.messageTitle = ResourceModules.Host_Name;
                globalData.messageDetail = ResourceGlobal.Host_Name;
            }
            else
            {

                foreach (var id1 in dashbord)
                {
                    if (S_Id == id1.Shift_ID)
                    {
                        Machine.Shop_ID = id1.Shop_ID;
                        Machine.Line_ID = id1.Line_ID;
                        Machine.Shift_ID = id1.Shift_ID;
                        Machine.Host_Name = id1.Host_Name;
                        Machine.Plant_ID = id1.Plant_ID;
                        ViewBag.Line_ID11 = id1.Line_ID;
                        Machine.Setup_ID = id1.Setup_ID;
                    }

                }
                
                int plant_Id = ((FDSession)this.Session["FDSession"]).plantId; ;
                ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
                //  ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
                //ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plant_Id), "Shop_ID", "Shop_Name", 0);
              //  ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == 0).ToList(), "Line_ID", "Line_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Shop_ID == Machine.Shop_ID), "Shop_ID", "Shop_Name", Machine.Shop_ID);
                //ViewBag.Line_ID = Machine.Line_ID;
                //ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == 1), "Line_ID", "Line_Name");
                if (dashbord.Count == 1)
                    ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Line_ID == Machine.Line_ID).ToList(), "Line_ID", "Line_Name", Machine.Line_ID);
                else
                    ViewBag.Line_ID = new SelectList((from d in db.RS_Dashboard_Master
                                                      join l in db.RS_Lines
                                                    on d.Line_ID equals l.Line_ID
                                                      where d.Host_Name == ip_address && d.Shift_ID == S_Id
                                                      select new
                                                      {
                                                           l.Line_ID,
                                                          l.Line_Name
                                                      }).ToList(), "Line_ID", "Line_Name");
                var stationItem = from station in db.RS_Stations
                                  where !(from routeConfig in db.RS_Route_Configurations select routeConfig.Station_ID).Contains(station.Station_ID)
                                  select station;

                ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Shop_ID == 0).ToList(), "Station_ID", "Station_Name");
                ViewBag.Setup_ID = new SelectList(db.RS_Setup, "Setup_ID", "Setup_Name", Machine.Setup_ID);
                List<getlist> list1 = new List<getlist>();
                foreach(var line in dashbord)
                {
                    var str4 = (from emp in db.RS_Employee
                                join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
                                join station in db.RS_Station_Setup_Mapping on operatorAllocation.Station_ID equals station.Station_ID
                                where operatorAllocation.Shift_ID == Machine.Shift_ID && operatorAllocation.Is_Buffer_Station != true && operatorAllocation.Line_ID == line.Line_ID && emp.Is_Deleted == null && emp.Plant_ID == line.Plant_ID && operatorAllocation.Setup_ID == line.Setup_ID
                                //&& DbFunctions.TruncateTime(operatorAllocation.Allocation_Date) == DbFunctions.TruncateTime(DateTime.Now) 
                                && operatorAllocation.Allocation_Date == DateTime.Today && (from att in db.RS_User_Attendance_Sheet
                                                                                            where att.Entry_Date.Value.Year == DateTime.Now.Year
                                                                                            && att.Entry_Date.Value.Month == DateTime.Now.Month
                                                                                            && att.Entry_Date.Value.Day == DateTime.Now.Day /*&& att.Shift_ID == shiftid*/
                                                                                            select att.Employee_No).Contains(emp.Employee_No)
                                //&& operatorAllocation.Week_Number == weekNo
                                select new getlist
                                {
                                    
                                    Employee_No = emp.Employee_No,
                                    Employee_Name = emp.Employee_Name,
                                    Image_Content = emp.Image_Content,
                                    Line_ID=operatorAllocation.Line_ID,
                                    Line_Name = operatorAllocation.RS_Lines.Line_Name,
                                    //Station_ID = operatorAllocation.Station_ID,
                                    Station_Name = operatorAllocation.RS_Stations.Station_Name,
                                    IsOJT = operatorAllocation.Is_OJT_Operator,
                                    Station_Type_ID = operatorAllocation.Station_Type_ID.ToString()
                                   
                                }).Distinct().ToList().OrderBy(m => m.Station_Type_ID);

                    foreach(var item in str4)
                    {
                        list1.Add(item);
                    }
                }

                ViewBag.Name = list1;
                 var Line_ID1 = list1.Select(m => m.Line_Name).Distinct().ToList();
                ViewBag.Setup1 = Line_ID1;
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
                
                //ViewBag.Shift1 = new SelectList(db.RS_Shift.Where(shift => shift.Plant_ID == plant_Id), "Shift_ID", "Shift_Name");
                ViewBag.Shift1 = new SelectList(db.RS_Shift.Where(shift => shift.Shift_ID == S_Id), "Shift_ID", "Shift_Name", S_Id);
                //ViewBag.Shift2 = new SelectList(db.RS_Shift.Where(shift => shift.Plant_ID == plant_Id), "Shift_ID", "Shift_Name");
                //ViewBag.Shift2 =myIp;
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

                //TempData["IP"] = myIp;
                TempData["globalData"] = globalData;
                globalData.pageTitle = "Dashboard Allocation";
                globalData.subTitle = ResourceGlobal.Index;
                globalData.controllerName = "Line";
                globalData.actionName = ResourceGlobal.Index;
                globalData.contentTitle = ResourceRouteConfiguration.Route_Configuration_Title_Configure_Route;
                globalData.contentFooter = ResourceRouteConfiguration.Route_Configuration_Title_Configure_Route;
                ViewBag.GlobalDataModel = globalData;
                return View();
            }
            
            TempData["globalData"] = globalData;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Shop_ID == Machine.Shop_ID), "Shop_ID", "Shop_Name", Machine.Shop_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == Machine.Shop_ID).ToList(), "Line_ID", "Line_Name",Machine.Line_ID);
            ViewBag.Shift1 = new SelectList(db.RS_Shift.Where(shift => shift.Shift_ID == S_Id), "Shift_ID", "Shift_Name", S_Id);
            ViewBag.GlobalDataModel = globalData;
            return Redirect("~/DashboardMaster/Index");
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
            //try
            //{
            //    var str4 = (from emp in db.RS_Employee
            //                join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
            //                join Attendance in db.RS_User_Attendance_Sheet on emp.Employee_ID equals Attendance.Employee_ID
            //                where  (Attendance.Is_Present == true && (Attendance.Entry_Date.Value.Year == DateTime.Now.Year
            //                            && Attendance.Entry_Date.Value.Month == DateTime.Now.Month
            //                            && Attendance.Entry_Date.Value.Day == DateTime.Now.Day))
            //                orderby  emp.Employee_No
            //                select new AllocationDashboard
            //                {
            //                    Employee_ID = emp.Employee_ID,
            //                    Employee_Token = emp.Employee_No,
            //                    Employee_Name = emp.Employee_Name,
            //                    Station_ID = operatorAllocation.Station_ID,
            //                    Line_ID = operatorAllocation.Line_ID,
            //                    Shop_ID = operatorAllocation.Shop_ID,
            //                    Shop_Name = db.RS_Shops.Where(x => x.Shop_ID == operatorAllocation.Shop_ID).FirstOrDefault().Shop_Name,
            //                    Line_Name = db.RS_Lines.Where(x => x.Line_ID == operatorAllocation.Line_ID).FirstOrDefault().Line_Name,
            //                    Station_Name = db.RS_Stations.Where(x => x.Station_ID == operatorAllocation.Station_ID).FirstOrDefault().Station_Name

            //                }).Distinct();
            //    return Json(str4, JsonRequestBehavior.AllowGet);
            //}
            //catch(Exception ex)
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}

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

        public ActionResult saveAssociateAllocation(string employeeNo, string stationId, int shopId, int lineId, int shift, int FromDay, int ToDay)
        {
            try
            {

                int flag = 0;
                var day = DateTime.Today.DayOfWeek;
                var cult = CultureInfo.CurrentCulture;
                var weekNo = cult.Calendar.GetWeekOfYear(new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day), cult.DateTimeFormat.CalendarWeekRule, cult.DateTimeFormat.FirstDayOfWeek);
                DateTime dates = FirstDateOfWeek(DateTime.Now.Year, weekNo);
                DateTime weekStartDate = dates;
                DateTime today = DateTime.Today;
                int currentDayOfWeek = (int)today.DayOfWeek;
                if (FromDay > ToDay || currentDayOfWeek > ToDay)
                {
                    return Json(new { result = "dateerror" }, JsonRequestBehavior.AllowGet);
                }

                int Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                string[] stat = stationId.Split('_');
                int droppedStationId = Convert.ToInt16(stat[1]);
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
                for (int i = fromD; i <= ToDay; i++)
                {
                    var isEmpExistCount = db.RS_AM_Operator_Station_Allocation.Where(op => DbFunctions.TruncateTime(op.Allocation_Date) == DbFunctions.TruncateTime(weekStartDate) && op.Employee_ID == employeeId && op.Shift_ID == shiftid && op.Station_ID == droppedStationId).Select(op => op.Week_Day).ToList().Count();
                    if (isEmpExistCount == 0)
                    {
                        saveAssociateAllocationDetails(shopId, lineId, droppedStationId, shiftid, employeeId, weekNo, weekStartDate.DayOfWeek.ToString(), weekStartDate.Date);
                        weekStartDate = weekStartDate.AddDays(1);
                        flag = 1;
                    }
                    else
                    {
                        flag = 2;
                        weekStartDate = weekStartDate.AddDays(1);
                    }
                }
                if (flag == 1)
                {
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
            catch (Exception dbEx)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult DeleteOperator(string employeeNo, string stationId, int shift, int line_ID, string fromDay, string toDay)
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
                    IEnumerable<RS_AM_Operator_Station_Allocation> operatop = (from op in db.RS_AM_Operator_Station_Allocation
                                                                               where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                                                                               && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                                                                               && op.Shift_ID == shift
                                                                               select op).ToList();
                    db.RS_AM_Operator_Station_Allocation.RemoveRange(operatop);
                    db.SaveChanges();
                    IEnumerable<RS_AM_Operator_Station_Allocation_History> operatopH = (from op in db.RS_AM_Operator_Station_Allocation_History
                                                                                        where DbFunctions.TruncateTime(op.Allocation_Date) >= DbFunctions.TruncateTime(StartDate)
                                                                                        && DbFunctions.TruncateTime(op.Allocation_Date) <= DbFunctions.TruncateTime(EndDate)
                                                                                        && op.Shift_ID == shift
                                                                                        select op).ToList();
                    db.RS_AM_Operator_Station_Allocation_History.RemoveRange(operatopH);
                    db.SaveChanges();
                    //if (flag == 1)
                    //{
                    return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { msg = "Error" }, JsonRequestBehavior.AllowGet);                    }
                }
                catch (Exception e)
                {
                    // Provide for exceptions.
                    return Json(new { msg = "Error" }, JsonRequestBehavior.AllowGet);
                }

                // RS_AM_Operator_Station_Allocation mmAllocationObj = db.RS_AM_Operator_Station_Allocation.Find(aremove);
                // db.RS_AM_Operator_Station_Allocation.Remove(aremove);
                //db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception dbEx)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /* Action Name                : GetStationListByLineID
       *  Description                : Action used to get the list of station which is added in route configuration
       *  Author, Timestamp          : Jitendra Mahajan
       *  Input parameter            : lineId (line id)
       *  Return Type                : ActionResult
       *  Revision                   : 1.0
       */
        public ActionResult GetStationListByLineID(string lineId,int setupId, int shiftId)
        {
            try
            {
                int ID = Convert.ToInt32(lineId);
                var setupID = Convert.ToInt32(setupId);
                var date = DateTime.Today;
                var st = from station in db.RS_AM_Operator_Station_Allocation
                         where station.Line_ID == ID && station.Shift_ID == shiftId && station.Setup_ID == setupID  && station.Allocation_Date == date && station.Is_Buffer_Station != true  //&& (from routeConfig in db.RS_Route_Configurations where routeConfig.Line_ID == lineId select routeConfig.Station_ID).Contains(station.Station_ID)
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.RS_Stations.Station_Name,
                         };
                //var st = (from str1 in db.RS_Stations
                // join str in db.RS_Station_Setup_Mapping
                // on str1.Station_ID equals str.Station_ID
                // where str1.Line_ID == ID && str1.Is_Buffer_Station != true && str.Setup_ID == 13
                //          select new
                // {
                //     Id = str1.Station_ID,
                //     Value = str1.Station_Name
                // }).Distinct();

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetCurrentShiftOperatorAgainstStationListByLineID(int lineId, decimal? shiftID, string Day)
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

                //var str4 = (from emp in db.RS_Employee
                //            join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
                //            join station in db.RS_Stations on operatorAllocation.Station_ID equals station.Station_ID
                //            where operatorAllocation.Shift_ID == shiftid && station.Is_Buffer_Station != true && operatorAllocation.Line_ID == lineId && emp.Is_Deleted == null && emp.Plant_ID == plant_ID
                //            //&& DbFunctions.TruncateTime(operatorAllocation.Allocation_Date) == DbFunctions.TruncateTime(DateTime.Now) 
                //            && operatorAllocation.Allocation_Date == DateTime.Today
                //            //&& operatorAllocation.Week_Number == weekNo
                //            select new
                //            {
                //                Employee_ID = emp.Employee_ID,
                //                Employee_Token = emp.Employee_No,
                //                Employee_Name = emp.Employee_Name,
                //                Image_Content = emp.Image_Content,
                //                Station_ID = operatorAllocation.Station_ID,
                //                IsOJT = operatorAllocation.Is_OJT_Operator,
                //                Absent = !(from att in db.RS_User_Attendance_Sheet
                //                           where att.Entry_Date.Value.Year == DateTime.Now.Year
                //                           && att.Entry_Date.Value.Month == DateTime.Now.Month
                //                           && att.Entry_Date.Value.Day == DateTime.Now.Day /*&& att.Shift_ID == shiftid*/
                //                           select att.Employee_No).Contains(emp.Employee_No) ? "ABSENT" : ""
                //            }).Distinct();
                var str4 = (from emp in db.RS_Employee
                 join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
                 join station in db.RS_Station_Setup_Mapping on operatorAllocation.Station_ID equals station.Station_ID
                 where operatorAllocation.Shift_ID == shiftid && operatorAllocation.Is_Buffer_Station != true && operatorAllocation.Line_ID == lineId && emp.Is_Deleted == null && emp.Plant_ID == plant_ID /*&& operatorAllocation.Setup_ID == 13*/
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
                var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_Token, x.Employee_Name, x.Station_ID, x.Absent, x.Image_Content,x.IsOJT }).Distinct();
                return Json(new { str5 = str5.Distinct(), count = str5.Count() }, JsonRequestBehavior.AllowGet);

                // return Json(str4, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        //All Employee
        //if employee on leave display on red color 
        //Don't take RS_User_Attendance_Sheet table reference
        //public ActionResult GetPresentSkilledEmployeeAgainstSelectedStation(int stationis, int? ShiftID)
        //{
        //    try
        //    {
        //        int supervisorId = ((FDSession)this.Session["FDSession"]).userId;
        //        decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

        //        //   var shift = GetCurrentShift(shopId);
        //        decimal shiftid = Convert.ToDecimal(ShiftID);
        //        var str4 = (from a in db.RS_Employee
        //                    join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
        //                    join c in db.RS_User_Attendance_Sheet on a.Employee_No equals c.Employee_No
        //                    join d in db.RS_AM_Employee_SkillSet on a.Employee_ID equals d.Employee_ID
        //                    where b.Supervisor_ID == supervisorId && a.Is_Deleted == null && a.Shift_ID == shiftid
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
        //                        d.Skill_ID
        //                        //a.Line_ID,
        //                        //d.Line_Name,
        //                        //a.Station_Name
        //                    }).Distinct().ToList();
        //        var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_Name, x.Employee_No }).Distinct();
        //        ViewBag.PEmp = str5;
        //        // return PartialView();
        //        return Json(str5.Distinct(), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult GetAbsCoverageEmployeeUnderLogedinSupervisor(int shopId, int? ShiftID, int Line_ID)
        //{
        //    try
        //    {
        //        int supervisorId = ((FDSession)this.Session["FDSession"]).userId;
        //        decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;



        //        TimeSpan currDate = DateTime.Now.TimeOfDay;
        //        //var shiftObj = (from shift in db.RS_Shift
        //        //                where
        //        //                  shift.Shop_ID == shopId &&
        //        //                 TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
        //        //                && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
        //        //                select shift);
        //        decimal shiftid = Convert.ToDecimal(ShiftID); //shiftObj.FirstOrDefault().Shift_ID;

        //        var str4 = (from a in db.RS_Employee
        //                    join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
        //                    join c in db.RS_User_Attendance_Sheet on a.Employee_No equals c.Employee_No
        //                    join d in db.RS_AM_Employee_SkillSet on a.Employee_ID equals d.Employee_ID
        //                    where a.Shift_ID == shiftid && !(from e in db.RS_AM_Operator_Station_Allocation select e.Employee_ID).Contains(a.Employee_ID)
        //                            && c.Is_Present == true && (c.Entry_Date.Value.Year == DateTime.Now.Year
        //                                    && c.Entry_Date.Value.Month == DateTime.Now.Month
        //                                    && c.Entry_Date.Value.Day == DateTime.Now.Day)

        //                    //  && !(from l in db.MM_AM_LeaveManagement where l.Plant_ID == plant_ID select l.Employee_ID).Contains(a.Employee_ID)
        //                    orderby d.Skill_ID descending
        //                    select new
        //                    {
        //                        a.Employee_ID,
        //                        a.Employee_Name,
        //                        a.Employee_No,
        //                        d.Skill_ID
        //                        //a.Line_ID,
        //                        //d.Line_Name,
        //                        //a.Station_Name
        //                    }).Distinct().ToList();



        //        var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_Name, x.Employee_No }).Distinct();



        //        ViewBag.PEmp = str5;
        //        //str4 = str5;
        //        return Json(str5.Distinct(), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult GetAbscentEmployeeUnderLogedinSupervisor(int shopId, int? ShiftID, int Line_ID)  //int stationis
        //{
        //    try
        //    {
        //        int supervisorId = ((FDSession)this.Session["FDSession"]).userId;
        //        decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;



        //        TimeSpan currDate = DateTime.Now.TimeOfDay;
        //        //var shiftObj = (from shift in db.RS_Shift
        //        //                where
        //        //                  shift.Shop_ID == shopId &&
        //        //                 TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
        //        //                && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
        //        //                select shift);
        //        decimal shiftid = Convert.ToDecimal(ShiftID); //shiftObj.FirstOrDefault().Shift_ID;

        //        var str4 = (from a in db.RS_Employee
        //                    join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
        //                    join d in db.RS_AM_Employee_SkillSet on a.Employee_ID equals d.Employee_ID
        //                    where b.Supervisor_ID == supervisorId && a.Shift_ID == shiftid
        //                    && a.Plant_ID == plant_ID && a.Is_Deleted == null && d.Line_ID == Line_ID
        //                        && !(from att in db.RS_User_Attendance_Sheet
        //                             where att.Entry_Date.Value.Year == DateTime.Now.Year
        //                             && att.Entry_Date.Value.Month == DateTime.Now.Month
        //                             && att.Entry_Date.Value.Day == DateTime.Now.Day
        //                             select att.Employee_No).Contains(a.Employee_No)
        //                    //&& b.Line_ID== Line_ID// 33
        //                    // && (from l in db.MM_AM_LeaveManagement where l.Plant_ID == plant_ID select l.Employee_ID).Contains(a.Employee_ID)
        //                    orderby d.Skill_ID descending
        //                    select new
        //                    {
        //                        a.Employee_ID,
        //                        a.Employee_Name,
        //                        a.Employee_No,
        //                        d.Skill_ID
        //                        //a.Line_ID,
        //                        //d.Line_Name,
        //                        //a.Station_Name
        //                    }).Distinct().ToList();

        //        var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_Name, x.Employee_No }).Distinct();


        //        ViewBag.PEmp = str5;
        //        //str4 = str5;
        //        return Json(str5.Distinct(), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //}

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
        public JsonResult AssignOperatorToNextWeek(int? shopID, int? lineID, int copy_Week, DateTime fromDate, DateTime toDate, int Shift, string replace)
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
                return Json(new { msg = "Error" }, JsonRequestBehavior.AllowGet);
            }


        }


        public void saveAssociateAllocationDetails(int shopId, int lineId, int droppedStationId, decimal shiftid, decimal employeeId, int weekNo, string day, DateTime allocation_Date)
        {
            RS_AM_Operator_Station_Allocation mmAllocationObj = new RS_AM_Operator_Station_Allocation();
            RS_AM_Operator_Station_Allocation_History mmAllocationHistoryObj = new RS_AM_Operator_Station_Allocation_History();
            mmAllocationObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            mmAllocationObj.Shop_ID = shopId;
            mmAllocationObj.Line_ID = lineId;
            mmAllocationObj.Shift_ID = shiftid;
            mmAllocationObj.Station_ID = droppedStationId;
            mmAllocationObj.Employee_ID = employeeId;
            mmAllocationObj.Allocation_Date = allocation_Date;
            mmAllocationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
            mmAllocationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
            mmAllocationObj.Week_Number = weekNo;
            mmAllocationObj.Week_Day = day.ToString();
            mmAllocationObj.Inserted_Date = DateTime.Now;
            db.RS_AM_Operator_Station_Allocation.Add(mmAllocationObj);
            db.SaveChanges();

            mmAllocationHistoryObj.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            mmAllocationHistoryObj.Shop_ID = shopId;
            mmAllocationHistoryObj.Line_ID = lineId;
            mmAllocationHistoryObj.Shift_ID = shiftid;
            mmAllocationHistoryObj.Station_ID = droppedStationId;

            mmAllocationHistoryObj.Employee_ID = employeeId;
            mmAllocationHistoryObj.Allocation_Date = allocation_Date;
            mmAllocationHistoryObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
            mmAllocationHistoryObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;

            mmAllocationHistoryObj.Inserted_Date = DateTime.Now;

            mmAllocationHistoryObj.Week_Number = weekNo;
            mmAllocationHistoryObj.Week_Day = day.ToString();
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

        public ActionResult CheckIsValidSkillForCriticalStation(string Station_Id, string Employee_No)
        {
            try
            {
                string[] stat = Station_Id.Split('_');
                int droppedStationId = Convert.ToInt16(stat[1]);

                var Employee_Id = db.RS_Employee.Where(c => c.Employee_No == Employee_No).Select(c => c.Employee_ID).FirstOrDefault();
                var isCriticalStation = db.RS_Stations.Where(c => c.Station_ID == droppedStationId).Select(c => c.Is_Critical_Station).FirstOrDefault();
                if (isCriticalStation == true)
                {
                    var IsValidSkillSet = db.RS_AM_Employee_SkillSet.Where(c => c.Employee_ID == Employee_Id).Select(c => c.Skill_ID).FirstOrDefault();
                    if (IsValidSkillSet >= 3)
                    {
                        return Json(new { msg = true, success = "True" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        return Json(new { msg = false, success = "True" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new { msg = true, success = "True" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, success = "False" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExcelUpload()
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

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelUpload(ExcelStationAllocation formData)
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
    }
}