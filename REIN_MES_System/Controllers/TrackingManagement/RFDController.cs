using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.TrackingManagement
{
    public class RFDController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0;
        string str, mPartID, mPartNo, mVendor, mDate, mShift, mSerialNo, bodysrno, mModel, mStncode, validatemsg, stationhost, status, mdate2, userIpAddress;
        bool stationcnt;
        // GET: RFD
        public ActionResult Index()
        {
            // plantId = ((FDSession)this.Session["FDSession"]).plantId;

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];

            }
            globalData.pageTitle = ResourceModules.RFD;
            globalData.controllerName = "RFD";
            globalData.actionName = ResourceGlobal.Create;


            ViewBag.GlobalDataModel = globalData;
            stationcnt = false;
            stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
            //stationhost = System.Environment.MachineName;
            ViewBag.HostName = stationhost.Split('.')[0];
            string userhost = stationhost.Split('.')[0];

            //var userId = ((FDSession)this.Session["FDSession"]).userId;
            //var userName = ((FDSession)this.Session["FDSession"]).userName;
            var station = (from s in db.RS_Stations
                           where s.Station_Host_Name == userhost
                           select new
                           {
                               s.Linemode,
                               s.Station_Name
                           }).FirstOrDefault();


            if (station != null)
            {
                ViewBag.Station = station.Station_Name;
            }
            else
            {
                ViewBag.Linemode = true;
            }
            /// plantId = ((FDSession)this.Session["FDSession"]).plantId;
            // ViewBag.Plant_ID = new SelectList(db.RS_Plants.ToList(), "Plant_ID", "Plant_Name", plantId);
            return View();
        }

        public ActionResult GetCount()

        {
            var date = DateTime.Today;
            var RecordCount = db.RS_RFD.Count(a => a.Scanning_Date == date);
            var PlanCount = db.RS_OM_PPC_Daily_Plan.Where(m => m.Plan_Date == DateTime.Today.Date && m.Shop_ID == 1).Select(m => m.Planned_Qty).FirstOrDefault();

            return Json(new { Total_Count = RecordCount, Plan = PlanCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetModelwiseCount()
        {
            var date = DateTime.Today;
            var result1 = (from RFD in db.RS_RFD
                           join model in db.RS_Model_Master
                           on RFD.Model_Code equals model.Model_Code
                           where RFD.Scanning_Date == date
                           select new
                           {
                               Model_Code = RFD.Model_Code,
                               Model_Desc = model.Model_Description
                           }
                           ).GroupBy(m=>m.Model_Code).ToList();
            return Json(new {Result = result1}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPresentdetailsOnLoad()
        {
            var date = DateTime.Today;
            //var startDate = Convert.ToDateTime("2019-10-26");
            var startDate = Convert.ToDateTime("2020-10-22");



            var result2 = (from pb in db.RS_SAP_Production_Booking
                           join rfd in db.RS_RFD
                           on pb.Serial_No equals rfd.Engine_No into oral
                           from rfd in oral.DefaultIfEmpty()
                           where rfd.Engine_No == null
                           join model in db.RS_Model_Master
                           on pb.BIW_Part_No equals model.Model_Code
                           where pb.Inserted_Date > startDate
                           select new
                           {
                               sr_no = pb.Row_Id,
                               model_code = model.Model_Code,
                               modedesc = model.Model_Description,
                               Eng_Sr = pb.Serial_No,
                               IsSAP = pb.Data_Status,
                               RFDDate = DateTime.Now,
                               SAPDate = pb.Inserted_Date
                           }
                           ).OrderBy(m=>m.sr_no).ToList();

            //var result2 = (from Edr in db.RS_RFD
            //               join pb in db.RS_SAP_Production_Booking
            //               on Edr.Engine_No equals pb.Serial_No  into orol
            //               from pb in orol.DefaultIfEmpty()
            //               join
            //               model in db.RS_Model_Master
            //               on
            //               Edr.Model_Code equals model.Model_Code
            //               where Edr.Scanning_Date == date
            //               select new
            //               {

            //                   sr_no = Edr.R_ID,
            //                   model_code = Edr.Model_Code,
            //                   modedesc = model.Model_Description,
            //                   Eng_Sr = Edr.Engine_No ,
            //                   IsSAP = pb.Data_Status,
            //                                          RFDDate = Edr.Inserted_Date,
            //                                          SAPDate = pb.Inserted_Date
            //               }).OrderByDescending(m => m.sr_no).Take(10).ToList();


            List<string> Ages = new List<string>();
            foreach (var item in result2)
            {
                
                DateTime d1 = Convert.ToDateTime(item.SAPDate);
                DateTime d2 = Convert.ToDateTime(item.RFDDate);
                TimeSpan d = d2 - d1;
                long durationTicks = Math.Abs(d.Ticks / TimeSpan.TicksPerMillisecond);
                long hours = durationTicks / (1000 * 60 * 60);
                hours = hours.ToString().Length == 1 ? Convert.ToInt64("0" + hours) : hours;
                long minutes = (durationTicks - (hours * 60 * 60 * 1000)) / (1000 * 60);
                minutes = minutes.ToString().Length == 1 ? Convert.ToInt64("0" + minutes) : minutes;
                var age = hours.ToString() + ":" + minutes.ToString();
                Ages.Add(age);
            }

            //   var  lastEng_Sr =bool.Parse (db.RS_EngineDropping.Where(m=>m.EngineSrNo).FirstOrDefault());
            var PlanCount = db.RS_OM_PPC_Daily_Plan.Where(m => m.Inserted_Date == DateTime.Today.Date && m.Shop_ID == 1).Select(m=>m.Planned_Qty).FirstOrDefault();

           var lastEng_Sr = (from rfd in db.RS_RFD select new { rfd.Engine_No, rfd.R_ID }).OrderByDescending(m => m.R_ID).FirstOrDefault();
         
            var RecordCount = db.RS_RFD.Count(a => a.Scanning_Date == date);
            var JsonResult = Json(new { Status = true, Result = result2, LastSerialNo = lastEng_Sr, Total_Count = RecordCount, Age = Ages, Plan = PlanCount }, JsonRequestBehavior.AllowGet);
            JsonResult.MaxJsonLength = Int32.MaxValue;
            return JsonResult;
        }

        public ActionResult ScanData(string Scanvalue)
        {
            //var startDate = Convert.ToDateTime("2019-10-26");
            var startDate = Convert.ToDateTime("2020-10-22");
            var Srexist = db.RS_OM_Order_List.Where(m => m.Serial_No == Scanvalue).ToList();
            if (Srexist.Count > 0)
            {
                var alScan = db.RS_RFD.Where(m => m.Engine_No == Scanvalue).ToList();
                if (alScan.Count > 0)
                {
                    validatemsg = "Engine Serial Number Already Scanned";
                    return Json(new { Message = validatemsg, Status = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
                    string userhost = stationhost.Split('.')[0];
                    var ModelCode = db.RS_OM_Order_List.Where(m => m.Serial_No == Scanvalue).Select(m => m.Model_Code).FirstOrDefault();

                    var result2 = (from pb in db.RS_SAP_Production_Booking
                                   join rfd in db.RS_RFD
                                   on pb.Serial_No equals rfd.Engine_No into oral
                                   from rfd in oral.DefaultIfEmpty()
                                   where rfd.Engine_No == null
                                   join model in db.RS_Model_Master
                                   on pb.BIW_Part_No equals model.Model_Code
                                   where pb.Inserted_Date > startDate
                                   select new
                                   {
                                       sr_no = pb.Row_Id,
                                       model_code = model.Model_Code,
                                       modedesc = model.Model_Description,
                                       Eng_Sr = pb.Serial_No,
                                       IsSAP = pb.Data_Status,
                                       RFDDate = DateTime.Now,
                                       SAPDate = pb.Inserted_Date
                                   }
                         ).OrderBy(m => m.sr_no).ToList();
                    var EngineTobeCompare = result2.Count > 0 ? result2[0].Eng_Sr : Scanvalue;

                    RS_RFD obj = new RS_RFD();
                    obj.Engine_No = Scanvalue;
                    obj.Model_Code = ModelCode;
                    obj.Scanning_Date = DateTime.Today;
                    obj.Inserted_Date = DateTime.Now;
                    obj.Inserted_Host = userhost;
                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.RS_RFD.Add(obj);
                    db.SaveChanges();

                    var StationID = db.RS_Stations.Where(m => m.Station_Host_Name == userhost).Select(m => m.Station_ID).FirstOrDefault();
                    RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == StationID).Single();

                    mmStationTrackingObj.Station_ID = StationID;
                    mmStationTrackingObj.Track_SerialNo = Scanvalue;
                    mmStationTrackingObj.Inserted_Date = DateTime.Now;
                    mmStationTrackingObj.Is_Edited = true;
                    db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                    db.SaveChanges();

                    bool isFIFO = true;
                    if(EngineTobeCompare.CompareTo(Scanvalue) == 0)
                    {
                        isFIFO = true;
                        validatemsg = "FIFO followed and Engine dispatched Successfully";
                    }
                    else
                    {
                        isFIFO = false;
                        validatemsg = "FIFO not followed but Engine dispatched Successfully";
                    }

                    var date = DateTime.Today;
                    //var result1 = (from RFD in db.RS_RFD
                    //               join pb in db.RS_SAP_Production_Booking
                    //               on RFD.Engine_No equals pb.Serial_No into orol
                    //               from pb in orol.DefaultIfEmpty()
                    //               join model in db.RS_Model_Master
                    //               on RFD.Model_Code equals model.Model_Code
                    //               where RFD.Scanning_Date == date
                    //               select new
                    //               {
                    //                   Row_ID = RFD.R_ID,
                    //                   Model_Code = RFD.Model_Code,
                    //                   Model_Desc = model.Model_Description,
                    //                   Er_No = RFD.Engine_No ,
                    //                   IsSAP = pb.Data_Status,
                    //                   RFDDate = RFD.Inserted_Date,
                    //                   SAPDate = pb.Inserted_Date
                    //               }
                    //               ).OrderByDescending(m => m.Row_ID).Take(10).ToList();

                    
                    var result1 = (from pb in db.RS_SAP_Production_Booking
                                   join rfd in db.RS_RFD
                                   on pb.Serial_No equals rfd.Engine_No into oral
                                   from rfd in oral.DefaultIfEmpty()
                                   where rfd.Engine_No == null
                                   join model in db.RS_Model_Master
                                   on pb.BIW_Part_No equals model.Model_Code
                                   where pb.Inserted_Date > startDate
                                   select new
                                   {
                                       sr_no = pb.Row_Id,
                                       model_code = model.Model_Code,
                                       modedesc = model.Model_Description,
                                       Eng_Sr = pb.Serial_No,
                                       IsSAP = pb.Data_Status,
                                       RFDDate = DateTime.Now,
                                       SAPDate = pb.Inserted_Date
                                   }
                          ).OrderBy(m => m.sr_no).ToList();

                    List<string> Ages = new List<string>();
                    foreach (var item in result1)
                    {

                        DateTime d1 = Convert.ToDateTime(item.SAPDate);
                        DateTime d2 = Convert.ToDateTime(item.RFDDate);
                        TimeSpan d = d2 - d1;
                        long durationTicks = Math.Abs(d.Ticks / TimeSpan.TicksPerMillisecond);
                        long hours = durationTicks / (1000 * 60 * 60);
                        long minutes = (durationTicks - (hours * 60 * 60 * 1000)) / (1000 * 60);
                        var age = hours.ToString() + ":" + minutes.ToString();
                        Ages.Add(age);
                    }
                    //var result = db.RS_RFD.Where(m => m.Scanning_Date == date).Select(m => m).OrderByDescending(m => m.R_ID).Take(10).ToList();
                    
                    var Jsonresult=    Json(new { Message = validatemsg, Status = true, Result = result1, LastSerialNo = Scanvalue,Age = Ages,Fifo = isFIFO }, JsonRequestBehavior.AllowGet);
                    Jsonresult.MaxJsonLength = Int32.MaxValue;
                    return Jsonresult;
                }

            }
            else
            {
                validatemsg = "Invalid Serial Number";
                return Json(new { Message = validatemsg, Status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OnLoad()
        {
            if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                userIpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else if (Request.UserHostAddress.Length != 0)
            {
                userIpAddress = Request.UserHostAddress;
            }
            stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
            String[] strArr = stationhost.Split(new char[] { '.' });

            //stationhost = System.Environment.MachineName;


            string userhost = strArr[0];
            stationcnt = Validated(userhost);

            if (stationcnt == true)
            {
                return Json(new { stationcnt, userhost, userIpAddress }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { stationcnt, userhost, userIpAddress }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool Validated(string StationHost)
        {
            var station = db.RS_Stations.Where(m => m.Station_Host_Name == StationHost).ToList();

            if (station.Count > 0)
                stationcnt = true;
            return stationcnt;
        }
    }
}