using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.OrderManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers
{
    public class TrackingAllocationController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        // GET: TrakingAllocation
        public ActionResult Index()
        {
            return View();
        }

        /*     Action Name                : getStationData
         *     Description                : Gets the Order Tracking Data of a particular Station.Called by Ajax request from Client Side.
         *     Author, Timestamp          : Jitendra Mahajan Nair
         *     Input parameter            : shopid,stationid
         *     Return Type                : ActionResult
         *     Revision                   : 1                            
         */
        public ActionResult getStationData(decimal shopid, decimal stationid)
        {
            var stationObj = (from a in db.RS_Stations
                              join b in db.RS_Station_Tracking on a.Station_ID equals b.Station_ID
                              join c in db.RS_OM_Order_List on b.SerialNo equals c.Serial_No into bc
                              from c in bc.DefaultIfEmpty()
                              where a.Station_ID == stationid
                              select new { a.Station_ID, a.Station_Name, c.DSN, b.SerialNo, c.partno, c.RS_Series.Series_Description }).FirstOrDefault();

            var AllocatedEmployee = (from Allocation in db.RS_AM_Operator_Station_Allocation
                                    join Attandance in db.RS_User_Attendance_Sheet on Allocation.Employee_ID equals Attandance.Employee_ID
                                    join employee in db.RS_Employee on Allocation.Employee_ID equals employee.Employee_ID
                                    join station in db.RS_Stations on Allocation.Station_ID equals station.Station_ID
                                    join shift in db.RS_Shift on Allocation.Shift_ID equals shift.Shift_ID
                                    where Attandance.Is_Present == true 
                                    && (Attandance.Entry_Date.Value.Year == DateTime.Now.Year 
                                    && Attandance.Entry_Date.Value.Month == DateTime.Now.Month 
                                    && Attandance.Entry_Date.Value.Day == DateTime.Now.Day) 
                                    && Allocation.Station_ID==stationid
                                    select new EmployeeAllocationData()
                                    {
                                        Employee_Name = employee.Employee_Name,
                                        Employee_No = employee.Employee_No,
                                        Employee_ID = employee.Employee_ID,
                                        Station_ID = Allocation.Station_ID,
                                        Station_Name = station.Station_Name
                                    }).Distinct();
            return Json(AllocatedEmployee, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getAjaxBufferData(decimal stationID)
        {
            IEnumerable<RS_OM_Order_List> bufferObj =
                (from a in db.RS_Station_Tracking
                 join b in db.RS_OM_Order_List on a.SerialNo equals b.Serial_No
                 where a.Station_ID == stationID && a.isConsumed == false
                 select b).AsEnumerable().OrderBy(a => a.Inserted_Date).ToList();

            ViewBag.BufferData = bufferObj;
            ViewBag.StationName = db.RS_Stations.Find(stationID).Station_Name;

            return PartialView("AjaxShowBuffer");
        }

        public ActionResult getStationCalls(decimal shopid)
        {
            var stationAlarmsObj = (from a in db.RS_Stations
                                    join b in db.RS_Station_Alarms on a.Station_ID equals b.Station_ID
                                    where a.Shop_ID == shopid
                                    select new { b.Material_Call, b.Supervisor_Call, b.Maintenance_Call, b.Station_ID }).ToList();

            return Json(stationAlarmsObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getTakeInOutData(decimal shopid)
        {
            var takeInOutObj = db.RS_Quality_Take_In_Take_Out.Where(a => a.Rework_Status == "Not Good" || a.Rework_Status == "Good")
                                           .Select(a => new { a.Serial_No, a.Rework_Status }).ToList();

            return Json(takeInOutObj, JsonRequestBehavior.AllowGet);
        }

        /*     Action Name                : checkIfLineStop
         *     Description                : Gets all lines which are Line Stop.Called by Ajax request from Client Side.
         *     Author, Timestamp          : Jitendra Mahajan Nair
         *     Input parameter            : shopid
         *     Return Type                : ActionResult
         *     Revision                   : 1                            
         */
        public ActionResult checkIfLineStop(decimal shopid)
        {
            //var lineStopObj = (from a in db.RS_Lines
            //                  join b in db.MM_ on a.Station_ID equals b.Station_ID
            //                  join c in db.RS_OM_Order_List on b.SerialNo equals c.Serial_No into bc
            //                  from c in bc.DefaultIfEmpty()
            //                  where a.Station_ID == stationid
            //                  select new { 

            var lineStopObj = db.RS_Lines
                              .Where(a => a.isLineStop == true && a.Shop_ID == shopid)
                              .Select(a => new { a.Line_ID, a.LineStopStation_ID })
                              .Distinct();

            return Json(lineStopObj, JsonRequestBehavior.AllowGet);
        }
        /*     Action Name                : trackOrder
         *     Description                : Gets the Station wise Order Tracking Data for a particular Shop.Called by Ajax request from Client Side.
         *     Author, Timestamp          : Jitendra Mahajan Nair
         *     Input parameter            : shopid
         *     Return Type                : ActionResult
         *     Revision                   : 1                            
         */
        public ActionResult trackOrder(decimal shopid)
        {
            var orderTrackingObj = (from a in db.RS_Stations
                                    join b in db.RS_Station_Tracking on a.Station_ID equals b.Station_ID
                                    join c in db.RS_OM_Order_List on b.SerialNo equals c.Serial_No
                                    where a.Shop_ID == shopid && a.Is_Buffer != true
                                    select new { a.Station_ID, b.SerialNo, c.Series_Code })
                                   .OrderBy(a => a.Station_ID);
            //db.RS_Station_Tracking.Where(a => a.Shop_ID == shopid)
            //                   .Select(a => new { a.Station_ID, a.SerialNo })
            //                   .OrderBy(a => a.Station_ID);

            return Json(orderTrackingObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrackingScreen()
        {
            int userID = ((FDSession)this.Session["FDSession"]).userId;
            //if (userID == 36 || userID == 23100023 || userID == 5)
            //{
            //    return RedirectToAction("EngineShop");
            //}
            //else if (userID == 33)
            //{
            //    return RedirectToAction("TransmissionShop");
            //}
            //else if (userID == 52)
            //{
            //    return RedirectToAction("TractorShop");
            //}
            //else
            //{
            int stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            // int stationID = 52;
            var getShopObj = db.RS_Stations.Find(stationID);
            int ShopID = Decimal.ToInt32(getShopObj.Shop_ID);
            if (ShopID == 1)
            {
                return RedirectToAction("EngineShop");
            }
            else if (ShopID == 2)
            {
                return RedirectToAction("TransmissionShop");
            }
            else if (ShopID == 3)
            {
                return RedirectToAction("VTUShop");
            }
            else if (ShopID == 4)
            {
                return RedirectToAction("TractorShop");
            }
            else
            {
                return RedirectToAction("CVShop");
            }
            //}
        }
        /*     Action Name                : EngineShop
         *     Description                : Loads the Engine Shop Tracking Screen
         *     Author, Timestamp          : Jitendra Mahajan Nair
         *     Input parameter            : None
         *     Return Type                : ActionResult
         *     Revision                   : 1
         */
        public ActionResult EngineShop()
        {
            globalData.ShopTitle = ResourceGlobal.Engine_Shop;
            IEnumerable<RS_OM_OrderRelease> OrderReleased = db.RS_OM_OrderRelease
                                .Where(a => a.Shop_ID == 1 && a.Line_ID == 1 && a.Order_Status == "Release")
                                .Distinct().ToList();

            ViewData["GlobalDataModel"] = globalData;

            return View(OrderReleased);
        }

        /*     Action Name                : TractorShop
         *     Description                : Loads the Tractor Shop Tracking Screen
         *     Author, Timestamp          : Jitendra Mahajan Nair
         *     Input parameter            : None
         *     Return Type                : ActionResult
         *     Revision                   : 1
         */
        public ActionResult TractorShop()
        {
            globalData.ShopTitle = ResourceGlobal.Tractor_Shop;
            IEnumerable<RS_OM_OrderRelease> OrderReleased = db.RS_OM_OrderRelease
                                .Where(a => a.Shop_ID == 4 && a.Line_ID == 10002 && a.Order_Status == "Release")
                                .Distinct().ToList();
            ViewData["GlobalDataModel"] = globalData;
            return View(OrderReleased);
        }

        /*     Action Name                : TransmissionShop
         *     Description                : Loads the Transmission Shop Tracking Screen
         *     Author, Timestamp          : Jitendra Mahajan Nair
         *     Input parameter            : None
         *     Return Type                : ActionResult
         *     Revision                   : 1
         */
        public ActionResult TransmissionShop()
        {
            globalData.ShopTitle = ResourceGlobal.Transmission_Shop;
            IEnumerable<RS_OM_OrderRelease> OrderReleased = db.RS_OM_OrderRelease
                                .Where(a => a.Shop_ID == 2 && a.Line_ID == 17 && a.Order_Status == "Release")
                                .Distinct().ToList();
            ViewData["GlobalDataModel"] = globalData;
            return View(OrderReleased);
        }

        /*     Action Name                : VTUShop
         *     Description                : Loads the VTU Shop Tracking Screen
         *     Author, Timestamp          : Jitendra Mahajan Nair
         *     Input parameter            : None
         *     Return Type                : ActionResult
         *     Revision                   : 1
         */
        public ActionResult VTUShop()
        {
            globalData.ShopTitle = ResourceGlobal.VTU_Shop;
            IEnumerable<RS_OM_OrderRelease> OrderReleased = db.RS_OM_OrderRelease
                                .Where(a => a.Shop_ID == 3 && a.Line_ID == 10020 && a.Order_Status == "Release")
                                .Distinct().ToList();
            ViewData["GlobalDataModel"] = globalData;
            return View(OrderReleased);
        }

        /*     Action Name                : CVShop
         *     Description                : Loads the CV Shop Tracking Screen
         *     Author, Timestamp          : Jitendra Mahajan Nair
         *     Input parameter            : None
         *     Return Type                : ActionResult
         *     Revision                   : 1
         */
        public ActionResult CVShop()
        {
            globalData.ShopTitle = ResourceGlobal.CV_Shop;
            IEnumerable<RS_OM_OrderRelease> OrderReleased = db.RS_OM_OrderRelease
                                .Where(a => a.Shop_ID == 7 && a.Line_ID == 10025 && a.Order_Status == "Release")
                                .Distinct().ToList();
            ViewData["GlobalDataModel"] = globalData;
            return View(OrderReleased);
        }
        public ActionResult checkLineEmployees(decimal shopid)
        {

            var lines = db.RS_Lines.Where(x => x.Shop_ID == shopid).ToList();
            List<RS_Stations> Stations = new List<RS_Stations>();

            foreach (var line in lines)
            {
                List<RS_Stations> _stations = db.RS_Stations.Where(x => x.Line_ID == line.Line_ID).ToList();
                Stations.AddRange(_stations);    
            }
            //stationid,employeeid,employeeno,employee_Name
            List<Tuple<decimal, decimal, string, string>> AllocatedStaions = new List<Tuple<decimal, decimal, string, string>>();
            List<Tuple<decimal, decimal, string, string>> UnallocatedStations = new List<Tuple<decimal, decimal, string, string>>();
            var AllocatedEmployee = from Allocation in db.RS_AM_Operator_Station_Allocation
                                    join Attandance in db.RS_User_Attendance_Sheet on Allocation.Employee_ID equals Attandance.Employee_ID
                                    join employee in db.RS_Employee on Allocation.Employee_ID equals employee.Employee_ID
                                    join shift in db.RS_Shift on Allocation.Shift_ID equals shift.Shift_ID
                                    where Attandance.Is_Present == true && (Attandance.Entry_Date.Value.Year == DateTime.Now.Year && Attandance.Entry_Date.Value.Month == DateTime.Now.Month && Attandance.Entry_Date.Value.Day == DateTime.Now.Day)
                                    select new EmployeeAllocationData()
                                    {
                                        Employee_Name = employee.Employee_Name,
                                        Employee_No = employee.Employee_No,
                                        Employee_ID = employee.Employee_ID,
                                        Station_ID = Allocation.Station_ID,
                                        Line_ID = Allocation.Line_ID
                                    };

           var unstations= (from t in Stations join b in AllocatedEmployee on t.Station_ID equals b.Station_ID select t).ToList();
           var unallocated=Stations.Except(unstations);
           var wholelines = new { allocation = AllocatedEmployee.Select(x=>x.Station_ID), unallocation = unallocated.Select(x=>x.Station_ID) };
           return Json(wholelines, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TrackingSignalR()
        {

            return View();
        }
    }
}