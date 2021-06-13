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
    public class ColorLine1Controller : BaseController
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
        // GET: ColorLine1
        public ActionResult Index()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            stationId = ((FDSession)this.Session["FDSession"]).stationId;
            lineId = ((FDSession)this.Session["FDSession"]).lineId;
            shopId = ((FDSession)this.Session["FDSession"]).shopId;
            insertedHost = ((FDSession)this.Session["FDSession"]).userHost;
            insertedUserId = ((FDSession)this.Session["FDSession"]).userId;

            globalData.pageTitle = "Colour Line 1";
            ViewBag.GlobalDataModel = globalData;
            return View();

        }

 //       public ActionResult OnLoad()
 //       {
 //           if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
 //           {
 //               userIpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
 //           }
 //           else if (Request.UserHostAddress.Length != 0)
 //           {
 //               userIpAddress = Request.UserHostAddress;
 //           }
 ////           stationhost = System.Net.Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.ToString();

 //  //         String[] strArr = stationhost.Split(new char[] { '.' });

 //           string userhost = Environment.MachineName;

 //           stationcnt = Validated(userhost);

 //           if (stationcnt == true)
 //           {
 //               return Json(new { stationcnt, userhost, userIpAddress }, JsonRequestBehavior.AllowGet);
 //           }
 //           else
 //           {
 //               return Json(new { stationcnt, userhost, userIpAddress }, JsonRequestBehavior.AllowGet);
 //           }
 //       }
 //       public bool Validated(string StationHost)
 //       {
 //           var station = db.RS_Stations.Where(m => m.Station_Host_Name == StationHost).ToList();

 //           if (station.Count > 0)
 //               stationcnt = true;

 //           return stationcnt;
 //       }

        public ActionResult ValidateSerialNo(string SerialNo, bool WIP, bool CompletedRecord)
        {
            var SerialNumberExist = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNo).ToList();
            if (SerialNumberExist.Count == 0)
            {
                return Json(new { Result = false, Msg = "Invalid Serial No" },JsonRequestBehavior.AllowGet);
            }
            else
            {
                if(WIP== true)
                {
                    var currentStatus = db.RS_OM_Order_Status_Live.Where(m => m.Serial_No == SerialNo && (m.Action_Status == "Paint In")).ToList();
                    if (currentStatus.Count() > 0)
                        return Json(new { Result = true, Msg = "" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { Result = false, Msg = "Please Scan Body at Previous Station First" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var currentStatus = db.RS_OM_Order_Status_Live.Where(m => m.Serial_No == SerialNo && (m.Action_Status == "Colour Line 1")).ToList();
                    if (currentStatus.Count() > 0)
                        return Json(new { Result = true, Msg = "" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { Result = false, Msg = "Current Body Status is not Colour Line 1" }, JsonRequestBehavior.AllowGet);
                }
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
                    var result = (from s in db.RS_OM_Order_Status_Live.Where(m => m.Action_Status == "Paint In" && (m.Serial_No == (Serial_No == "" ? m.Serial_No : Serial_No) || m.Serial_No.Contains(Serial_No)))
                                  join c in db.RS_OM_Order_List on s.Serial_No equals c.Serial_No
                                  join d in db.RS_Model_Master on c.Model_Code equals d.Model_Code
                                  select new { BodySrno = c.Serial_No, Model_Code = c.Model_Code, Model_Description = d.Model_Description, Carrier_No = s.Carrier_No, Barcode = s.Barcode, Paint_Shop = "" }).ToList();

                    var result1 = db.RS_OM_Order_Status_Live.Where(m => m.Action_Status == "Colour Line 1").OrderByDescending(m => m.Updated_Date).Select(m => m.Serial_No).FirstOrDefault();

                    var CompletedCount = (from s in db.RS_OM_Order_Status_Live
                                          where s.Action_Status == "Colour Line 1"
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

                var result = (from s in db.RS_OM_Order_Status_Live.Where(m => m.Action_Status == "Colour Line 1")
                              join c in db.RS_OM_Order_List on s.Serial_No equals c.Serial_No
                              join d in db.RS_Model_Master on c.Model_Code equals d.Model_Code
                              //join e in db.RS_Shops on s.Paint_Shop_ID equals e.Shop_ID
                              select new { BodySrno = c.Serial_No, Model_Code = c.Model_Code, Model_Description = d.Model_Description, Carrier_No = s.Carrier_No, Skid_No = s.Skid_No, Hanger_No = s.Hanger_No, Barcode = s.Barcode, Paint_Shop = "" }).ToList();

                var result1 = db.RS_OM_Order_Status_Live.Where(m => m.Action_Status == "Colour Line 1").OrderByDescending(m => m.Updated_Date).Select(m => m.Serial_No).FirstOrDefault();




                var WIPCount = (from s in db.RS_OM_Order_Status_Live.Where(m => m.Action_Status == "Paint In")
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
                if(WIP == true)
                {
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
                            ExistingOrder.Action_Status = "Colour Line 1";
                            ExistingOrder.Station_ID = stationId;
                            ExistingOrder.Plant_ID = plantId;
                            ExistingOrder.Shop_ID = shopId;
                            ExistingOrder.Line_ID = lineId;
                            ExistingOrder.Serial_No = SerialNo;
                            db.Entry(ExistingOrder).State = EntityState.Modified;
                            db.SaveChanges();

                            ValidMsg = "Body ColorLine1 Successfully";
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
                                Action_Status = "Colour Line 1",
                                Order_No = orderNo,
                                Entry_Date = DateTime.Now

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
                            objOrderRelease.Order_Status = "Colour Line 1";
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
                        return Json(new { Result = false, Msg = "Error while changing Order Status to Colour Line 1" }, JsonRequestBehavior.AllowGet);
                    }
                }

                else
                {
                    var ExistingOrder = db.RS_OM_Order_Status_Live.Where(m => m.Serial_No == SerialNo).Select(m => m).FirstOrDefault();
                    var CurrentStatus = ExistingOrder.Action_Status;
                    if (ExistingOrder != null && CurrentStatus == "Colour Line 1")
                    {
                        var OrderStatusID = ExistingOrder.Order_Status_ID;
                        RS_OM_Order_Status_Live obj = db.RS_OM_Order_Status_Live.Find(OrderStatusID);

                        obj.Updated_Date = DateTime.Now;
                        obj.Updated_Host = insertedHost;
                        obj.Updated_User_ID = insertedUserId;
                        obj.Action_Status = "Paint In";
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();

                        try
                        {
                            if (!string.IsNullOrEmpty(orderNo))
                            {
                                RS_OM_OrderRelease objOrderRelease = db.RS_OM_OrderRelease.Where(m => m.Order_No == orderNo).FirstOrDefault();
                                objOrderRelease.Order_Status = "Paint In";
                                db.Entry(objOrderRelease).State = EntityState.Modified;
                                db.SaveChanges();

                                ValidMsg = "Body Recalled Successfully";
                            }
                            else
                            {
                                return Json(new { Result = false, Msg = "Body Serial Number Not found in Order List. Body Sr No :" + SerialNo }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new { Result = false, Msg = "Error while changing Order Status to Paint In" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                result = true;
                return Json(new { Result = result, Msg = ValidMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = result, Msg = ValidMsg }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}