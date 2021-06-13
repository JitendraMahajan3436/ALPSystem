using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.PaintShop
{
    public class PTEDOUTController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        string stationhost, userIpAddress; bool stationcnt;
        private decimal plantId;
        private decimal stationId;
        private decimal lineId;
        private decimal shopId;
        private string insertedHost;
        private decimal insertedUserId;
        // GET: PTEDOUT
        public ActionResult Index()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            stationId = ((FDSession)this.Session["FDSession"]).stationId;
            lineId = ((FDSession)this.Session["FDSession"]).lineId;
            shopId = ((FDSession)this.Session["FDSession"]).shopId;
            insertedHost = ((FDSession)this.Session["FDSession"]).userHost;
            insertedUserId = ((FDSession)this.Session["FDSession"]).userId;

            globalData.pageTitle = "Paint Out";
            ViewBag.GlobalDataModel = globalData;
            return View();

        }

//        public ActionResult OnLoad()
//        {
//            if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
//            {
//                userIpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
//            }
//            else if (Request.UserHostAddress.Length != 0)
//            {
//                userIpAddress = Request.UserHostAddress;
//            }
//            //stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();

////            String[] strArr = stationhost.Split(new char[] { '.' });

//            string userhost = System.Environment.MachineName; ;

//            stationcnt = Validated(userhost);

//            if (stationcnt == true)
//            {
//                return Json(new { stationcnt, userhost, userIpAddress }, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {
//                return Json(new { stationcnt, userhost, userIpAddress }, JsonRequestBehavior.AllowGet);
//            }
//        }
//        public bool Validated(string StationHost)
//        {
//            var station = db.RS_Stations.Where(m => m.Station_Host_Name == StationHost).ToList();

//            if (station.Count > 0)
//                stationcnt = true;

//            return stationcnt;
//        }

        public ActionResult ValidateSerialNo(string SerialNo)
        {
            var SerialNumberExist = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).ToList();
            if(SerialNumberExist.Count == 0)
            {
                return Json(new { Result = false, Msg = "Invalid Serial No" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var currentStatus = db.RS_OM_Order_Status_Live.Where(m => m.Serial_No == SerialNo && (m.Action_Status == "Colour Line 1" || m.Action_Status == "Colour Line 2")).ToList();
                if (currentStatus.Count() > 0)
                    return Json(new { Result = true, Msg = "" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Result = false, Msg = "Please Scan Body at Previous Station First" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetWIPData(string Serial_No = "")
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            stationId = ((FDSession)this.Session["FDSession"]).stationId;
            lineId = ((FDSession)this.Session["FDSession"]).lineId;
            shopId = ((FDSession)this.Session["FDSession"]).shopId;
            insertedHost = ((FDSession)this.Session["FDSession"]).userHost;
            insertedUserId = ((FDSession)this.Session["FDSession"]).userId;
            Serial_No = Serial_No.Trim();
            //var Date = DateTime.Today;
            try
            {
                if (Serial_No == "" || Serial_No.Length > 5)
                {
                    var result = (from s in db.RS_OM_Order_Status_Live.Where(m => (m.Action_Status == "Colour Line 1" || m.Action_Status == "Colour Line 2") && (m.Serial_No == (Serial_No == "" ? m.Serial_No : Serial_No) || m.Serial_No.Contains(Serial_No)))
                                  join c in db.RS_OM_Order_List on s.Serial_No equals c.Serial_No
                                  join d in db.RS_Model_Master on c.Model_Code equals d.Model_Code
                                  select new { BodySrno = c.Serial_No, Model_Code = c.Model_Code, Model_Description = d.Model_Description, Carrier_No = s.Carrier_No, Barcode = s.Barcode, Paint_Shop = "" }).ToList();

                    var result1 = db.RS_OM_Order_Status_Live.Where(m => m.Action_Status == "Paint Out").OrderByDescending(m => m.Updated_Date).Select(m => m.Serial_No).FirstOrDefault();
                    var TenDayDate = DateTime.Today.AddDays(-10);
                    var CompletedCount = (from s in db.RS_OM_Order_Status_Live
                                          where s.Action_Status == "Paint Out" && s.Updated_Date > TenDayDate
                                          select s).Count();
                    return Json(new { Result = result, Result1 = result1, CompletedCount = CompletedCount }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = "", Result1 = "", CompletedCount = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCompletedData()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            stationId = ((FDSession)this.Session["FDSession"]).stationId;
            lineId = ((FDSession)this.Session["FDSession"]).lineId;
            shopId = ((FDSession)this.Session["FDSession"]).shopId;
            insertedHost = ((FDSession)this.Session["FDSession"]).userHost;
            insertedUserId = ((FDSession)this.Session["FDSession"]).userId;
            var Date = DateTime.Today;
            try
            {
                var TenDayDate = Date.AddDays(-10);
                var result = (from s in db.RS_OM_Order_Status_Live.Where(m => m.Action_Status == "Paint Out")
                              join c in db.RS_OM_Order_List on s.Serial_No equals c.Serial_No
                              join d in db.RS_Model_Master on c.Model_Code equals d.Model_Code
                              //join e in db.RS_Shops on s.Paint_Shop_ID equals e.Shop_ID
                              where s.Updated_Date > TenDayDate
                              select new { BodySrno = c.Serial_No, Model_Code = c.Model_Code, Model_Description = d.Model_Description, Carrier_No = s.Carrier_No, Skid_No = s.Skid_No, Hanger_No = s.Hanger_No, Barcode = s.Barcode, Paint_Shop = "" }).ToList();

                var result1 = db.RS_OM_Order_Status_Live.Where(m => m.Action_Status == "Paint Out").OrderByDescending(m => m.Updated_Date).Select(m => m.Serial_No).FirstOrDefault();




                var WIPCount = (from s in db.RS_OM_Order_Status_Live.Where(m => (m.Action_Status == "Colour Line 1" || m.Action_Status == "Colour Line 2"))
                                join c in db.RS_OM_Order_List on s.Serial_No equals c.Serial_No
                                join d in db.RS_Model_Master on c.Model_Code equals d.Model_Code
                                select new { BodySrno = c.Serial_No, Model_Code = c.Model_Code, Model_Description = d.Model_Description }).Count();

                return Json(new { Result = result, Result1 = result1, WIPCount = WIPCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveData(string SerialNo, bool WIP, bool CompletedRecord, string Skid_No, string Hanger_No, string Barcode, int Shop_Id)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            stationId = ((FDSession)this.Session["FDSession"]).stationId;
            lineId = ((FDSession)this.Session["FDSession"]).lineId;
            shopId = ((FDSession)this.Session["FDSession"]).shopId;
            insertedHost = ((FDSession)this.Session["FDSession"]).userHost;
            insertedUserId = ((FDSession)this.Session["FDSession"]).userId;
            string ValidMsg = "";
            var result = false;
            SerialNo = SerialNo.ToUpper();
            //stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();
            //stationhost = Environment.MachineName;
            try
            {

                var orderNo = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).Select(m => m.Order_No).FirstOrDefault();
                var SerialStationID = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).Select(m => m.Station_ID).FirstOrDefault();
                if (!string.IsNullOrEmpty(orderNo))
                {
                    var ExistingOrder = db.RS_OM_Order_Status_Live.Where(m => m.Serial_No == SerialNo || m.Order_No == orderNo).Select(m => m).FirstOrDefault();

                    if (ExistingOrder != null)
                    {
                        ExistingOrder.Barcode = Barcode;
                        ExistingOrder.Skid_No = Skid_No;
                        ExistingOrder.Hanger_No = Hanger_No;
                        ExistingOrder.Order_No = orderNo;
                        ExistingOrder.Paint_Shop_ID = Shop_Id;
                        ExistingOrder.Updated_Date = DateTime.Now;
                        ExistingOrder.Updated_Host = insertedHost;
                        ExistingOrder.Updated_User_ID = insertedUserId;
                        ExistingOrder.Action_Status = "Paint Out";
                        ExistingOrder.Station_ID = stationId;
                        ExistingOrder.Plant_ID = plantId;
                        ExistingOrder.Shop_ID = shopId;
                        ExistingOrder.Line_ID = lineId;
                        ExistingOrder.Serial_No = SerialNo;
                        db.Entry(ExistingOrder).State = EntityState.Modified;
                        db.SaveChanges();

                        //Print Barcode Sticker
                        RS_PRN mmPRNObj = new RS_PRN();
                        mmPRNObj.Plant_ID = plantId;
                        mmPRNObj.Shop_ID = shopId;
                        mmPRNObj.Line_ID = lineId;
                        mmPRNObj.Station_ID = stationId;
                        String fileName = "";


                        var DefaultPrinter = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.Is_DefaultPrinter).FirstOrDefault();
                        if (DefaultPrinter == true && SerialStationID == 101)
                        {
                            fileName = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.DefaultPRNFileName).FirstOrDefault();
                        }
                        if (DefaultPrinter == true && SerialStationID != 101)
                        {
                            fileName = "Paint_Out_All.prn";
                        }
                        //else
                        //{
                        //    fileName = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.ExtraPRNFileName).FirstOrDefault();
                        //}
                        string prnFile = "";
                        if (fileName == "" || fileName == null)
                        {
                           // prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/OrderStart_Citizen.prn"));
                        }
                        else
                        {
                            prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/" + fileName));
                        }

                        var ModelColor = db.RS_OM_OrderRelease.Where(m => m.Order_No == orderNo).Select(m => m.Model_Color).FirstOrDefault();
                        var ColorDesc = db.RS_Colour.Where(m => m.Colour_ID == ModelColor).Select(m => m.Colour_Desc).FirstOrDefault();
                        var ModelCode = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).Select(m => m.Model_Code).FirstOrDefault();
                        RS_Model_Master mmModelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == ModelCode).Single();
                        var VINNumber = "MA1" + mmModelMasterObj.VIN_Code.Trim() + "00000000";
                        prnFile = prnFile.Replace("BODYSRNO", SerialNo.Trim().ToUpper());
                        prnFile = prnFile.Replace("COLORDESC", ColorDesc);
                        prnFile = prnFile.Replace("MODELCODE", mmModelMasterObj.Model_Code);
                        prnFile = prnFile.Replace("MODELDESC", mmModelMasterObj.Model_Description);
                        prnFile = prnFile.Replace("VINNo", VINNumber);
                       
                        prnFile = prnFile.Replace("PDateTime", Convert.ToString(DateTime.Now.ToString("dd-MMM-yy HH:mm:ss")));
                        
                        mmPRNObj.PRN_Text = prnFile;
                        mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmPRNObj.Inserted_Date = DateTime.Now;
                        mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.RS_PRN.Add(mmPRNObj);
                        db.SaveChanges();
                    }
                    else
                    {
                        RS_OM_Order_Status_Live objBodyOut = new RS_OM_Order_Status_Live
                        {
                            Barcode = Barcode,
                            Serial_No = SerialNo,
                            Inserted_Date = DateTime.Now,
                            Inserted_Host = insertedHost,
                            Inserted_User_ID = insertedUserId,
                            Station_ID = stationId,
                            Plant_ID = plantId,
                            Shop_ID = shopId,
                            Line_ID = lineId,
                            Skid_No = Skid_No,
                            Hanger_No = Hanger_No,
                            Paint_Shop_ID = Shop_Id,
                            Action_Status = "Paint Out",
                            Order_No = orderNo,
                            Entry_Date = DateTime.Now,
                            Updated_Date = DateTime.Now,
                            Updated_Host = insertedHost,
                            Updated_User_ID = insertedUserId
                    };
                        db.RS_OM_Order_Status_Live.Add(objBodyOut);
                        db.SaveChanges();


                    }
                }

                try
                {

                    if (!string.IsNullOrEmpty(orderNo))
                    {
                        RS_OM_OrderRelease objOrderRelease = db.RS_OM_OrderRelease.Where(m => m.Order_No == orderNo).FirstOrDefault();
                        objOrderRelease.Order_Status = "Paint Out";
                        db.Entry(objOrderRelease).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        return Json(new { Result = false, Msg = "Body Serial Number Not found in Order List. Body Sr No :" + SerialNo }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Result = false, Msg = "Error while changing Order Status to Paint Out" }, JsonRequestBehavior.AllowGet);
                }

                result = true;
                return Json(new { Result = result, Msg = ValidMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = result, Msg = ValidMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult IsReprint(string SerialNo)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            stationId = ((FDSession)this.Session["FDSession"]).stationId;
            lineId = ((FDSession)this.Session["FDSession"]).lineId;
            shopId = ((FDSession)this.Session["FDSession"]).shopId;
            insertedHost = ((FDSession)this.Session["FDSession"]).userHost;
            insertedUserId = ((FDSession)this.Session["FDSession"]).userId;
            var Result = false;
            var ValidMsg = "";
            var SerialNumberExist = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).ToList();
            if (SerialNumberExist.Count == 0)
            {
                return Json(new { Result = false, Msg = "Invalid Serial No" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var currentStatus = db.RS_OM_Order_Status_Live.Where(m => m.Serial_No == SerialNo && m.Action_Status == "Paint Out").ToList();
                var SerialStationID = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).Select(m => m.Station_ID).FirstOrDefault();
                var orderNo = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).Select(m => m.Order_No).FirstOrDefault();
                if (currentStatus.Count() > 0)
                {
                    RS_PRN mmPRNObj = new RS_PRN();
                    mmPRNObj.Plant_ID = plantId;
                    mmPRNObj.Shop_ID = shopId;
                    mmPRNObj.Line_ID = lineId;
                    mmPRNObj.Station_ID = stationId;
                    String fileName = "";


                    var DefaultPrinter = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.Is_DefaultPrinter).FirstOrDefault();
                    if (DefaultPrinter == true && SerialStationID == 101)
                    {
                        fileName = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.DefaultPRNFileName).FirstOrDefault();
                    }
                    if (DefaultPrinter == true && SerialStationID != 101)
                    {
                        fileName = "Paint_Out_All.prn";
                    }
                  
                    string prnFile = "";
                    if (fileName == "" || fileName == null)
                    {
                        // prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/OrderStart_Citizen.prn"));
                    }
                    else
                    {
                        prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/" + fileName));
                    }

                    var ModelColor = db.RS_OM_OrderRelease.Where(m => m.Order_No == orderNo).Select(m => m.Model_Color).FirstOrDefault();
                    var ColorDesc = db.RS_Colour.Where(m => m.Colour_ID == ModelColor).Select(m => m.Colour_Desc).FirstOrDefault();
                    var ModelCode = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).Select(m => m.Model_Code).FirstOrDefault();
                    RS_Model_Master mmModelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == ModelCode).Single();
                    var VINNumber = "MA1" + mmModelMasterObj.VIN_Code.Trim() + "00000000";
                    prnFile = prnFile.Replace("BODYSRNO", SerialNo.Trim().ToUpper());
                    prnFile = prnFile.Replace("COLORDESC", ColorDesc);
                    prnFile = prnFile.Replace("MODELCODE", mmModelMasterObj.Model_Code);
                    prnFile = prnFile.Replace("MODELDESC", mmModelMasterObj.Model_Description);
                    prnFile = prnFile.Replace("VINNo", VINNumber);

                    prnFile = prnFile.Replace("PDateTime", Convert.ToString(DateTime.Now.ToString("dd-MMM-yy HH:mm:ss")));

                    mmPRNObj.PRN_Text = prnFile;
                    mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmPRNObj.Inserted_Date = DateTime.Now;
                    mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_PRN.Add(mmPRNObj);
                    db.SaveChanges();

                    return Json(new { Result = true, Msg = "Print Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Result = false, Msg = "Body Current Status is not Paint Out" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Result = Result, Msg = ValidMsg }, JsonRequestBehavior.AllowGet);
        }
    }
}