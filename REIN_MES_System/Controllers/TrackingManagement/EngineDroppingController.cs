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
    public class EngineDroppingController : BaseController
    {

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();


        int plantId = 0, userId = 0, shopId = 0, lineId = 0, stationId = 0;
        bool stationcnt;
        string stationhost, userName, validatemsg, userIpAddress;


        // GET: EngineDropping
        public ActionResult Index()
        {


            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.EngineDropping;
            globalData.controllerName = "EngineDropping";
            globalData.actionName = ResourceGlobal.Create;
            ViewBag.GlobalDataModel = globalData;

            stationcnt = false;
            stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
            ViewBag.HostName = stationhost.Split('.')[0];
            string userhost = stationhost.Split('.')[0];

            var userId = ((FDSession)this.Session["FDSession"]).userId;
            var userName = ((FDSession)this.Session["FDSession"]).userName;


            var station = (from s in db.RS_Stations
                           where s.Station_Host_Name == userhost
                           select new
                           {
                               s.Station_Name
                           }).FirstOrDefault();


            if (station != null)
            {
                ViewBag.Station = station.Station_Name;
            }

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.ToList(), "Plant_ID", "Plant_Name", plantId);

            return View();
        }


        public ActionResult ScanData(string Scanvalue)
        {
            var Srexist = db.RS_OM_Order_List.Where(m => m.Serial_No == Scanvalue).ToList();

            if (Srexist.Count > 0)
            {
                var alScan = db.RS_EngineDropping.Where(m => m.EngineSrNo == Scanvalue && m.Is_EngineOk == true).ToList();
                if (alScan.Count > 0)
                {
                    stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
                    string userhost = stationhost.Split('.')[0];
                    var ModelCode = db.RS_OM_Order_List.Where(m => m.Serial_No == Scanvalue).Select(m => m.Model_Code).FirstOrDefault();

                    bool status = false;
                    bool IsEngineOk = false;

                    var TTTList = db.Partwise_Defect_Master.Where(m => m.BuyoffCode == 38 && m.BODYSRNO == Scanvalue).ToList();

                    bool IsTTTOk = false;

                    foreach (var item in TTTList)
                    {
                        if (item.DEFECT_STATUS == false)
                            IsTTTOk = true;
                        else if (item.REWORK_STATUS == true)
                            IsTTTOk = true;
                        else if (item.As_Is_Ok == true)
                            IsTTTOk = true;
                        else if (item.DEFECT_STATUS == true && (item.REWORK_STATUS != true
                            || item.As_Is_Ok != true))
                        {
                            IsTTTOk = false;
                            break;
                        }
                    }

                    var LBLTList = db.Partwise_Defect_Master.Where(m => m.BuyoffCode == 37 && m.BODYSRNO == Scanvalue).ToList();
                    bool IsLBLTOk = false;
                    foreach (var item in LBLTList)
                    {
                        if (item.DEFECT_STATUS == false)
                            IsLBLTOk = true;
                        else if (item.REWORK_STATUS == true)
                            IsLBLTOk = true;
                        else if (item.As_Is_Ok == true)
                            IsLBLTOk = true;
                        else if (item.DEFECT_STATUS == true && (item.REWORK_STATUS != true
                            || item.As_Is_Ok != true))
                        {
                            IsLBLTOk = false;
                            break;
                        }
                    }

                    if (IsTTTOk == true && IsLBLTOk == true)
                    {
                        status = true;
                        IsEngineOk = true;
                        validatemsg = "Engine OK at both TTT and Rework";
                    }
                    else if(IsTTTOk == true && IsLBLTOk == false)
                    {
                        status = false;
                        IsEngineOk = false;
                        validatemsg = "Engine OK at TTT but NOK at Rework";
                    }
                    else if(IsTTTOk == false && IsLBLTOk == true)
                    {
                        status = false;
                        IsEngineOk = false;
                        validatemsg = "Engine OK at Rework but NOK at TTT";
                    }
                    else
                    {
                        status = false;
                        IsEngineOk = false;
                        validatemsg = "Engine NOK at both Rework and TTT";
                    }

                    
                    if(LBLTList.Count == 0)
                    {
                        status = false;
                        validatemsg = "Engine Serial Number Already Scanned";
                    }

                    RS_EngineDropping obj = new RS_EngineDropping();
                    obj.EngineSrNo = Scanvalue.ToUpper();
                    obj.Model_Code = ModelCode;
                    obj.Scanning_Date = DateTime.Today;
                    obj.Inserted_Date = DateTime.Now;
                    obj.Inserted_Host = userhost;
                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    obj.Is_EngineOk = IsEngineOk;
                    db.RS_EngineDropping.Add(obj);
                    db.SaveChanges();

                    var StationID = db.RS_Stations.Where(m => m.Station_Host_Name == userhost).Select(m => m.Station_ID).FirstOrDefault();
                    RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == StationID).Single();

                    mmStationTrackingObj.Station_ID = StationID;
                    mmStationTrackingObj.Track_SerialNo = Scanvalue;
                    mmStationTrackingObj.Inserted_Date = DateTime.Now;
                    mmStationTrackingObj.Is_Edited = true;
                    db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                    db.SaveChanges();

                    //validatemsg = "Engine Number Scanned Successfully";

                    var date = DateTime.Today;
                    var result1 = (from Edr in db.RS_EngineDropping
                                   join
                                   model in db.RS_Model_Master
                                   on
                                   Edr.Model_Code equals model.Model_Code
                                   where Edr.Scanning_Date == date
                                   select new
                                   {

                                       sr_no = Edr.EngineDropingRow_ID,
                                       model_code = Edr.Model_Code,
                                       modedesc = model.Model_Description,
                                       Eng_Sr = Edr.EngineSrNo,
                                       ENGStatus = Edr.Is_EngineOk

                                   }).OrderByDescending(m => m.sr_no).Take(10).ToList();

                    return Json(new { Message = validatemsg, Status = status, Result = result1, LastSerialNo = Scanvalue, EngineOK = IsEngineOk }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
                    string userhost = stationhost.Split('.')[0];
                    var ModelCode = db.RS_OM_Order_List.Where(m => m.Serial_No == Scanvalue).Select(m => m.Model_Code).FirstOrDefault();
                    //var ModelDescription = db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Model_Description).FirstOrDefault();

                    var List = db.Partwise_Defect_Master.Where(m => m.BuyoffCode == 38 && m.BODYSRNO == Scanvalue).ToList();

                    bool IsEngineOk = false;
                    
                    foreach (var item in List)
                    {
                        if (item.DEFECT_STATUS == false)
                            IsEngineOk = true;
                        else if (item.REWORK_STATUS == true)
                            IsEngineOk = true;
                        else if (item.As_Is_Ok == true)
                            IsEngineOk = true;
                        else if (item.DEFECT_STATUS == true && (item.REWORK_STATUS != true
                            || item.As_Is_Ok != true))
                        {
                            IsEngineOk = false;
                            break;
                        }
                    }

                    bool status = false;
                    if(IsEngineOk == false)
                    {
                        status = false;
                        validatemsg = "Engine NOK at TTT";
                    }
                    else
                    {
                        status = true;
                        validatemsg = "Engine OK at TTT";
                    }
                    // select BODYSRNO from Partwise_Defect_Master where DEFECT_STATUS = 0 
                    //and BODYSRNO = 'UEK3H42132'and BuyoffCode = 38 and BODYSRNO Not IN 
                    //(select top(1) BODYSRNO from Partwise_Defect_Master where DEFECT_STATUS = 1 
                    //and BODYSRNO = 'UEK3H42132' and BuyoffCode = 38 )


                    RS_EngineDropping obj = new RS_EngineDropping();
                    obj.EngineSrNo = Scanvalue.ToUpper();
                    obj.Model_Code = ModelCode;
                    obj.Scanning_Date = DateTime.Today;
                    obj.Inserted_Date = DateTime.Now;
                    obj.Inserted_Host = userhost;
                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    obj.Is_EngineOk = IsEngineOk;
                    db.RS_EngineDropping.Add(obj);
                    db.SaveChanges();

                    var StationID = db.RS_Stations.Where(m => m.Station_Host_Name == userhost).Select(m => m.Station_ID).FirstOrDefault();
                    RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == StationID).Single();

                    mmStationTrackingObj.Station_ID = StationID;
                    mmStationTrackingObj.Track_SerialNo = Scanvalue;
                    mmStationTrackingObj.Inserted_Date = DateTime.Now;
                    mmStationTrackingObj.Is_Edited = true;
                    db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                    db.SaveChanges();

                    

                    var date = DateTime.Today;
                    var result1 = (from Edr in db.RS_EngineDropping
                                   join
                                   model in db.RS_Model_Master
                                   on
                                   Edr.Model_Code equals model.Model_Code
                                   where Edr.Scanning_Date == date
                                   select new
                                   {

                                       sr_no = Edr.EngineDropingRow_ID,
                                       model_code = Edr.Model_Code,
                                       modedesc = model.Model_Description,
                                       Eng_Sr = Edr.EngineSrNo,
                                       ENGStatus = Edr.Is_EngineOk

                                   }).OrderByDescending(m => m.sr_no).Take(10).ToList();

                    return Json(new { Message = validatemsg, Status = true, Result = result1, LastSerialNo = Scanvalue, EngineOK = IsEngineOk }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                validatemsg = "Invalid Serial Number";
                return Json(new { Message = validatemsg, Status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPresentdetailsOnLoad()
        {
            var date = DateTime.Today;
            var result2 = (from Edr in db.RS_EngineDropping
                           join
                           model in db.RS_Model_Master
                           on
                           Edr.Model_Code equals model.Model_Code
                           where Edr.Scanning_Date == date
                           select new
                           {

                               sr_no = Edr.EngineDropingRow_ID,
                               model_code = Edr.Model_Code,
                               modedesc = model.Model_Description,
                               Eng_Sr = Edr.EngineSrNo,
                               ENGStatus = Edr.Is_EngineOk

                           }).OrderByDescending(m => m.sr_no).Take(10).ToList();



            //   var  lastEng_Sr =bool.Parse (db.RS_EngineDropping.Where(m=>m.EngineSrNo).FirstOrDefault());

            var lastEng_Sr = db.RS_EngineDropping.FirstOrDefault();

            return Json(new { Status = true, Result = result2, LastSerialNo = lastEng_Sr }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDroppingCount()

        {
            //var count = (from count1 in db.MM_PDIToRFD
            //             where !(from o in db.RS_RFD
            //                     select o.Engine_No).Contains(count1.EngineSrNo)
            //             select count1.Row_ID
            //          ).Count();
            var date = DateTime.Today;
            var RecordCount = db.RS_EngineDropping.Count(a => a.Is_EngineOk == true && a.Scanning_Date == date);

            return Json(new { Tatal_Count = RecordCount }, JsonRequestBehavior.AllowGet);
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