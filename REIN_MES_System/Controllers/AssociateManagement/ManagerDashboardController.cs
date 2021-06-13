using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers
{
    public class ManagerDashboardController : Controller
    {
        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        GlobalData globalData = new GlobalData();
        RS_Route_Configurations routeConfigurationObj = new RS_Route_Configurations();
        RS_Route_Display routeDisplayObj = new RS_Route_Display();
        FDSession fdSession = new FDSession();
        int plantId = 0, shopId = 0, lineId = 0, Empid = 0, setupid = 0;
        // GET: ManagerDashboard
        // GET: ManagerDashboard
        public ActionResult Index()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = "Manager Dashboard";
            ViewBag.GlobalDataModel = globalData;

            int supervisorId = ((FDSession)this.Session["FDSession"]).userId;
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;


            var shopid = db.RS_AM_Shop_Manager_Mapping.Where(m => m.Employee_ID == supervisorId).Select(m => m.Shop_ID).FirstOrDefault();
            TimeSpan currDate = DateTime.Now.TimeOfDay;
            var shiftObj = (from shift in db.RS_Shift
                            where
                             TimeSpan.Compare(shift.Shift_Start_Time, currDate) < 0
                            && TimeSpan.Compare(shift.Shift_End_Time, currDate) > 0
                            select shift).ToList();
            var S_Id = shiftObj[0].Shift_ID;

            var lines = (from line in db.RS_Lines
                         where line.Shop_ID == shopid
                         select line).ToList().Distinct();

            //ViewBag.Line_ID1 = lines;
            List<getlist> list2 = new List<getlist>();

            List<List<int>> AllData = new List<List<int>>();
            foreach (var lin in lines)
            {
                List<int> Data = new List<int>();


                var set = (from setup in db.RS_Dashboard_Master where setup.Line_ID == lin.Line_ID && setup.Shop_ID == lin.Shop_ID select setup.Setup_ID).FirstOrDefault();




                var str4 = (from emp in db.RS_Employee
                            join operatorAllocation in db.RS_AM_Operator_Station_Allocation on emp.Employee_ID equals operatorAllocation.Employee_ID
                            join stat in db.RS_Stations on operatorAllocation.Station_ID equals stat.Station_ID
                            where operatorAllocation.Plant_ID == plant_ID && operatorAllocation.Shop_ID == shopid && operatorAllocation.Line_ID == lin.Line_ID && operatorAllocation.Setup_ID == set && operatorAllocation.Shift_ID == S_Id && stat.Is_Buffer != true && operatorAllocation.Allocation_Date == DateTime.Today
                            select new getlist
                            {

                                Employee_No = emp.Employee_No,
                                Employee_ID = emp.Employee_ID,
                                Employee_Name = emp.Employee_Name,
                                Station_ID = operatorAllocation.Station_ID,
                                Station_Name = operatorAllocation.RS_Stations.Station_Name,
                                Line_Name = operatorAllocation.RS_Lines.Line_Name,
                                Line_ID = operatorAllocation.RS_Lines.Line_ID,
                                IsOJT = operatorAllocation.Is_OJT_Operator,
                                Station_Type_ID = (operatorAllocation.Station_Type_ID).ToString(),
                                Absent = !(from att in db.RS_User_Attendance_Sheet
                                           where att.Entry_Date.Value.Year == DateTime.Now.Year
                                           && att.Entry_Date.Value.Month == DateTime.Now.Month
                                           && att.Entry_Date.Value.Day == DateTime.Now.Day /*&& att.Shift_ID == shiftid*/
                                           select att.Employee_No).Contains(emp.Employee_No) ? "ABSENT" : ""

                            }).Distinct();
                var str5 = str4.Select(x => new { x.Employee_ID, x.Employee_No, x.Employee_Name, x.Station_ID, x.IsOJT, x.Station_Name, x.Absent, x.Station_Type_ID, x.Line_Name, x.Line_ID }).Distinct().ToList();


                //var str1 = str5.Select(m => m.Station_ID).Count();
                //var str2 = str5.Select(m => m.Employee_ID).Count();
                var str3 = db.RS_Station_Setup_Mapping.Where(m => m.Setup_ID == set && m.RS_Stations.Station_Type_ID == 1).Select(m => m.Station_ID).Count();
                //var sttyp1 = str5.Select(m => m.Station_Type_ID).ToString();
                var str6 = str5.Where(m => m.Absent == "ABSENT").Select(m => m.Employee_ID).Count();
                var str1 = str5.Where(m => m.Station_Type_ID == "1" && m.Absent == "").Count();
                var str2 = str5.Where(m => m.Station_Type_ID == "2" && m.Absent == "").Count();
                var str9 = str5.Where(m => m.IsOJT == true).Count();
                var requireDirectCount = db.RS_ManPower_Required.Where(m => m.Line_ID == lin.Line_ID && m.Setup_ID == set).Select(m => m.Direct_MP_Quantity).FirstOrDefault().Value;

                var requireIndirectCount = db.RS_ManPower_Required.Where(m => m.Line_ID == lin.Line_ID && m.Setup_ID == set).Select(m => m.Indirect_MP_Quantity).FirstOrDefault().Value;





                var str7 = (from a in db.RS_Employee
                                //join b in db.RS_Assign_OperatorToSupervisor on a.Employee_ID equals b.AssignedOperator_ID
                            join c in db.RS_User_Attendance_Sheet on a.Employee_No equals c.Employee_No
                            join d in db.RS_AM_Operator_Station_Allocation on a.Employee_ID equals d.Employee_ID

                            where d.Line_ID == lin.Line_ID && d.Shift_ID == S_Id && d.Is_Buffer_Station == true && d.Allocation_Date == DateTime.Today && d.Setup_ID == set
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

                            }).Distinct().ToList();



                var str8 = str7.Select(x => new { x.Employee_ID, x.Employee_Name, x.Employee_No, x.Image_Content }).Distinct().Count();
                //var newList = GlobalStrings.Append(localStrings)
                //foreach(var ite in str4)
                //{
                //    list2.Add(ite);
                //}
                Data.Add(Convert.ToInt32(lin.Line_ID));
                Data.Add(str3);
                Data.Add(str1);
                Data.Add(str2);
                Data.Add(str9);
                Data.Add(str8);
                Data.Add(str6);
                Data.Add(requireDirectCount);
                Data.Add(requireIndirectCount);
                AllData.Add(Data);
            }










            //            ViewBag.Managerlist1 = list2;
            ViewBag.Managerlist = AllData;

            return View();
        }
    }
}