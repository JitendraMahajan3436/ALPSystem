using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Helper;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper.IoT;
using System.Globalization;

namespace ZHB_AD.Controllers.OrderManagement
{
    public class DockOrderStartController : BaseController
    {
        private ZHB_ADEntities db = new ZHB_ADEntities();
        MM_OM_OrderRelease[] orderRelease;
        General generalHelper = new General();
        List<MM_MT_Clita> ClitaObjList = new List<MM_MT_Clita>();
        MetaShift objShift = new MetaShift();
        string modelStartTime = string.Empty;
        string modelEndTime = string.Empty;
        //int plantId = 1, shopId = 3, lineId = 0, stationId = 48;


        /*               Action Name               : Create
         *               Description               : Action used to show the list of order Create data.
         *               Author, Timestamp         : Rahul Varpe
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */

        // GET: DockOrderStart/Create
        public ActionResult Create()
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = ((FDSession)this.Session["FDSession"]).shopId;
            int lineId = ((FDSession)this.Session["FDSession"]).lineId;
            int stationId = ((FDSession)this.Session["FDSession"]).stationId;
            DateTime today = DateTime.Today;
            //sandip
            //string MachineName = string.Empty;
            //int flag = 0;
            //int shiftID = objShift.getShiftID();
            //using (MTTUWEntities MTTUW_db = new MTTUWEntities())
            //{
            //    var machine = MTTUW_db.MM_MT_MTTUW_Machines.Where(mac => mac.Shop_ID == shopId && mac.Station_ID == stationId).Select(mac => new { mac.Machine_ID, mac.Machine_Name });
            //    foreach (var item in machine)
            //    {
            //        List<Decimal> todaysPendingClitaID = MTTUW_db.MM_MT_Daily_Clita_Log.Where(a => DbFunctions.TruncateTime(a.Inserted_Date) == today && a.Shift_ID == shiftID).Select(a => a.Clita_ID).ToList();
            //        //var ClitaObjList1 = MTTUW_db.MM_MT_Clita.Where(a => a.MM_MT_MTTUW_Machines.Shop_ID == shopId && a.MM_MT_MTTUW_Machines.Station_ID == stationId && a.Machine_ID == item.Machine_ID).ToList();
            //        ClitaObjList = MTTUW_db.MM_MT_Clita.Where(a => !todaysPendingClitaID.Contains(a.Clita_ID) && a.MM_MT_MTTUW_Machines.Shop_ID == shopId && a.MM_MT_MTTUW_Machines.Station_ID == stationId && a.Machine_ID == item.Machine_ID).ToList();
            //        if (ClitaObjList.Count > 0)
            //        {
            //            flag++;
            //            MachineName += item.Machine_Name + ",";
            //        }
            //    }

            //}
            //TempData["MachineName"] = MachineName;
            //if (flag > 0)
            //{
            //    ViewBag.flag = 0;
            //}
            //else
            //{
            //    ViewBag.flag = 1;
            //}
            ////for 3rd shift bypass after 12 
            //decimal sID = Convert.ToDecimal(shiftID);
            //string shiftName = db.MM_Shift.Where(shift => shift.Shift_ID == sID).Select(shift => shift.Shift_Name).FirstOrDefault();
            //if (shiftName == "C")
            //{
            //    TimeSpan dt = DateTime.Now.TimeOfDay;
            //    int hours = dt.Hours;
            //    if (hours == 0 || (hours > 1 && hours < 7))
            //    {
            //        ViewBag.flag = 1;
            //    }
            //}


            MM_OM_OrderRelease mmOmRelease = new MM_OM_OrderRelease();
            //  orderRelease = mmOmRelease.getTodayReleasedOrderDetails(shopId);
            int userId = ((FDSession)this.Session["FDSession"]).userId;

            ViewBag.Plant_ID_User = plantId;
            ViewBag.Shop_ID_User = shopId;
            ViewBag.Line_ID_User = lineId;
            ViewBag.Station_ID_User = stationId;
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");

            // process to get the top three order number for RFD buffer
            //try
            //{
            //    String serialNumberList = "";
            //    //MM_OM_OrderRelease[] mmOrderRelease = db.MM_OM_OrderRelease.Where(p => p.Shop_ID == shopId && p.Line_ID == lineId).OrderBy(p => p.RSN).ToArray();
            //    MM_OM_OrderRelease[] mmOrderRelease = (from orderReleaseObj in db.MM_OM_OrderRelease
            //                                           where orderReleaseObj.Shop_ID == shopId && orderReleaseObj.Line_ID == lineId && !(from orderList in db.MM_OM_Order_List select orderList.Order_No).Contains(orderReleaseObj.Order_No)
            //                                           select orderReleaseObj).ToArray();
            //    for (int i = 0; i < mmOrderRelease.Count(); i++)
            //    {
            //        if (i > 3)
            //            break;
            //        if (serialNumberList == "")
            //        {
            //            serialNumberList = mmOrderRelease[i].Order_No;
            //        }
            //        serialNumberList = serialNumberList + "," + mmOrderRelease[i].Order_No;
            //    }
            //    ViewBag.OrderNumberForRequest = serialNumberList;
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.OrderNumberForRequest = "";
            //}

            ViewBag.OrderNumberForRequest = "";


            return View();
        }

        /*               Action Name               : ShowOrderListByShop
         *               Description               : Action used to show the order List.
         *               Author, Timestamp         : Rahul Varpe
         *               Input parameter           : orderListShopId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult ShowOrderListByShop(int orderListShopId)
        {

            var orderDetail = (from orderReleaseItem in db.MM_OM_OrderRelease
                               where orderReleaseItem.Shop_ID == orderListShopId && orderReleaseItem.Order_Status == "Release"
                               select orderReleaseItem).Take(10).ToList();
            return PartialView(orderDetail);
        }


        public ActionResult ShowOrderListByShopAndShop(int orderListShopId, int orderListLineId)
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

            IEnumerable<MM_OM_OrderRelease> orderDetail = null;
            MM_Partgroup partgroup;
            MM_Model_Master model_mast;

            partgroup = (from partgroupData in db.MM_Partgroup
                         where partgroupData.Line_ID == lineId
                         select partgroupData
                      ).FirstOrDefault();

            if (partgroup != null)
            {
                if (partgroup.Order_Create == true)
                {
                    //orderDetail = (from orderReleaseItem in db.MM_OM_OrderRelease
                    //               orderby orderReleaseItem.RSN
                    //               where orderReleaseItem.Shop_ID == orderListShopId && orderReleaseItem.Line_ID == orderListLineId && orderReleaseItem.Order_Status == "Release"
                    //               select orderReleaseItem).Take(20).ToList();

                    // orderDetail = db.MM_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == DateTime.Now.Year && or.Planned_Date.Value.Month == DateTime.Now.Month && or.Planned_Date.Value.Day == DateTime.Now.Day).Select(or => or).Take(20).ToList().OrderBy(or => or.RSN);
                    orderDetail = (from orderReleaseItem in db.MM_OM_OrderRelease
                                   orderby orderReleaseItem.RSN ascending
                                   where orderReleaseItem.Planned_Date == DateTime.Today
                                   && orderReleaseItem.Line_ID == orderListLineId
                                   && orderReleaseItem.Order_Status == "Release"
                                   select orderReleaseItem).Take(20).ToList();

                    //var orderOldSeriesData = new List<KeyValuePair<String, String>>();
                    foreach (var item in orderDetail)
                    {
                        String modelCode = item.Model_Code;

                        String orderNo = item.Order_No;
                        // process to get the old series
                        MM_Model_Master modelMasterObj = db.MM_Model_Master.Where(p => p.Model_Code == modelCode).FirstOrDefault();
                        //if (modelMasterObj != null)
                        //{
                        //    String oldSeriesCode = modelMasterObj.Old_Series_Code;
                        //    if (!String.IsNullOrEmpty(oldSeriesCode))
                        //    {
                        //        // proces to get old series
                        //        MM_Series seriesObj = db.MM_Series.Where(p => p.Series_Code == oldSeriesCode).FirstOrDefault();
                        //        if (seriesObj != null)
                        //        {
                        //            //orderOldSeriesData.Add(new KeyValuePair<string, int>(orderNo.ToString(), seriesObj.Series_Description.ToString()));
                        //            //orderOldSeriesData.Add(new KeyValuePair<string, String>(orderNo, seriesObj.Series_Description));
                        //            item.oldSeriesCode = seriesObj.Series_Description;
                        //        }
                        //    }
                        //}
                    }

                    //ViewBag.OldSeriesData = orderOldSeriesData;
                }
                else
                {
                    String[] omConfiguration = (from configur in db.MM_OM_Configuration
                                                where configur.Partgroup_ID == partgroup.Partgroup_ID
                                                select configur.OMconfig_ID).ToArray();

                    if (omConfiguration.Count() > 0)
                    {
                        String[] models = (from mm in db.MM_Model_Master
                                           where omConfiguration.Contains(mm.OMconfig_ID)
                                           select mm.Model_Code).ToArray();


                        orderDetail = (from or in db.MM_OM_OrderRelease.Where(or => models.Contains(or.partno) && or.Order_Status != "Hold")
                                       join ol in db.MM_OM_Order_List.Where(a => a.Line_ID == lineId)
                                       on or.Order_No equals ol.Order_No into orol
                                       from ol in orol.DefaultIfEmpty()
                                       where ol.Order_No == null
                                        && or.Inserted_Date.Year == DateTime.Now.Year
                                       && or.Inserted_Date.Month == DateTime.Now.Month
                                       && or.Inserted_Date.Day == DateTime.Now.Day
                                       orderby or.RSN, or.Inserted_Date ascending
                                       select or).Take(20).ToList();

                    }
                }
            }

            return PartialView(orderDetail);



        }


        /*               Action Name               : GetTopOrderDetails
         *               Description               : Action used to show the Order Release top orders details.
         *               Author, Timestamp         : Rahul Varpe
         *               Input parameter           : shopId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult GetTopOrderDetails(int shopId, int lineId)
        {

            try
            {
                MM_Partgroup partgroup;
                MM_Model_Master model_mast;

                partgroup = (from partgroupData in db.MM_Partgroup
                             where partgroupData.Line_ID == lineId
                             select partgroupData
                          ).FirstOrDefault();

                if (partgroup != null)
                {
                    if (partgroup.Order_Create == true)
                    {
                        var st = (from orderReleaseItem in db.MM_OM_OrderRelease
                                  where orderReleaseItem.Shop_ID == shopId && orderReleaseItem.Line_ID == lineId && orderReleaseItem.Order_Status == "Release" &&
                                  orderReleaseItem.Planned_Date == System.DateTime.Today
                                  orderby orderReleaseItem.RSN
                                  select new
                                  {
                                      Plant_OrderNo = orderReleaseItem.Plant_OrderNo,
                                      Model_Code = orderReleaseItem.Model_Code,
                                      Remarks = orderReleaseItem.Remarks,
                                      Part_no = orderReleaseItem.partno,
                                      Series = orderReleaseItem.Series_Code,
                                      Row_ID = orderReleaseItem.Row_ID,
                                      Order_No = orderReleaseItem.Order_No,
                                      Order_Type = orderReleaseItem.Order_Type
                                  }).Take(1).ToList();
                        return Json(st, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        String[] omConfiguration = (from configur in db.MM_OM_Configuration
                                                    where configur.Partgroup_ID == partgroup.Partgroup_ID
                                                    select configur.OMconfig_ID).ToArray();

                        if (omConfiguration.Count() > 0)
                        {
                            String[] models = (from mm in db.MM_Model_Master
                                               where omConfiguration.Contains(mm.OMconfig_ID)
                                               select mm.Model_Code).ToArray();



                            var st = (from or in db.MM_OM_OrderRelease
                                      from ol in db.MM_OM_Order_List
                                      .Where(pp1 => ((or.Order_No == pp1.Order_No) && (pp1.Line_ID == lineId)))
                                      .DefaultIfEmpty()
                                      orderby or.Inserted_Date ascending
                                      select or).Where(or => models.Contains(or.partno))
                                           .Select(orderReleaseItem => new
                                           {
                                               Plant_OrderNo = orderReleaseItem.Plant_OrderNo,
                                               Model_Code = orderReleaseItem.Model_Code,
                                               Remarks = orderReleaseItem.Remarks,
                                               Part_no = orderReleaseItem.partno,
                                               Series = orderReleaseItem.Series_Code,
                                               Row_ID = orderReleaseItem.Row_ID,
                                               Order_No = orderReleaseItem.Order_No,
                                               Order_Type = orderReleaseItem.Order_Type

                                           }).Take(1).ToList();
                            return Json(st, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }


        }

        /*               Action Name               : ShowStartOrderList
         *               Description               : Action used to show the started Order list. 
         *               Author, Timestamp         : Rahul Varpe
         *               Input parameter           : startOrderListShopId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult ShowStartOrderList(int startOrderListShopId, int startOrderListLineId)
        {
            var orderDetail = (from orderList in db.MM_OM_Order_List
                               where orderList.Shop_ID == startOrderListShopId && orderList.Line_ID == startOrderListLineId && (from orderRelease in db.MM_OM_OrderRelease where orderRelease.Order_Status == "Started" && orderRelease.Shop_ID == startOrderListShopId select orderRelease.Order_No).Contains(orderList.Order_No)
                               select orderList).OrderByDescending(p => p.Inserted_Date).Take(3).ToList();

            //var orderOldSeriesData = new List<KeyValuePair<String, String>>();
            foreach (var item in orderDetail)
            {
                String modelCode = item.Model_Code;

                String orderNo = item.Order_No;
                // process to get the old series
                MM_Model_Master modelMasterObj = db.MM_Model_Master.Where(p => p.Model_Code == modelCode).FirstOrDefault();
                //if (modelMasterObj != null)
                //{
                //    String oldSeriesCode = modelMasterObj.Old_Series_Code;
                //    if (!String.IsNullOrEmpty(oldSeriesCode))
                //    {
                //        // proces to get old series
                //        MM_Series seriesObj = db.MM_Series.Where(p => p.Series_Code == oldSeriesCode).FirstOrDefault();
                //        if (seriesObj != null)
                //        {
                //            //orderOldSeriesData.Add(new KeyValuePair<string, int>(orderNo.ToString(), seriesObj.Series_Description.ToString()));
                //            //orderOldSeriesData.Add(new KeyValuePair<string, String>(orderNo, seriesObj.Series_Description));
                //            item.oldSeriesCode = seriesObj.Series_Description;
                //        }
                //    }
                //}
            }

            return PartialView(orderDetail);
        }


        /*               Action Name               : StartOrder
         *               Description               : Action used to start the order. 
         *               Author, Timestamp         : Rahul Varpe
         *               Input parameter           : rowId,plantId,shopId,lineId,stationId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult StartOrder(int rowId, int plantId, int shopId, int lineId, int stationId, String[] serialNo, String[] partNo, string kitting_Serial_No)
        {
            try
            {
                bool isModelStart = checkIsModelStart(rowId, plantId);
                if (isModelStart)
                {
                    int newRSN = 0;
                    // process to check the order is already started or not
                    MM_OM_OrderRelease orderReleaseObj = db.MM_OM_OrderRelease.Where(p => p.Row_ID == rowId && p.Shop_ID == shopId).FirstOrDefault();
                    //if (orderReleaseObj.Order_Status == "Release")
                    {
                        // process to check for the same order number order is already started in order list or not
                        String orderNo = orderReleaseObj.Order_No;
                        MM_OM_Order_List orderListObj = db.MM_OM_Order_List.Where(p => p.Order_No == orderNo && p.Shop_ID == shopId && p.Line_ID == lineId).FirstOrDefault();
                        if (orderListObj != null)
                        {
                            if (orderReleaseObj.Order_Status == "Release")
                            {
                                MM_OM_Planned_Orders plannedOrdrObj = db.MM_OM_Planned_Orders.Where(a => a.Order_ID == rowId && a.Planned_Date == DateTime.Today).FirstOrDefault();
                                if (plannedOrdrObj != null)
                                {
                                    newRSN = Convert.ToInt16(plannedOrdrObj.RSN.GetValueOrDefault(0));
                                }
                                // it means order started, but status not updated
                                orderReleaseObj.Order_Status = "Started";
                                orderReleaseObj.Updated_Date = DateTime.Now;
                                orderReleaseObj.RSN = newRSN;
                                orderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                orderReleaseObj.Is_Edited = true;
                                db.Entry(orderReleaseObj).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            return Json(true, JsonRequestBehavior.AllowGet);
                        }
                    }

                    try
                    {
                        newRSN = generalHelper.updatePlannedRSN(rowId, ((FDSession)this.Session["FDSession"]).userId);
                    }
                    catch (Exception exp)
                    {
                        while (exp.InnerException != null)
                        {
                            exp = exp.InnerException;
                        }
                        generalHelper.addControllerException(exp, "DockOrderStartController", "new Logic For PPC Hold ORder(Order ID: " + rowId + ")", ((FDSession)this.Session["FDSession"]).userId);
                    }

                    string shop_name = "";
                    //find Shop Name
                    MM_Shops shop;
                    shop = (from shopName in db.MM_Shops
                            where shopName.Shop_ID == shopId
                            select shopName).FirstOrDefault();

                    if (shop != null)
                    {
                        shop_name = shop.Shop_Name;
                    }

                    // add record in order list table
                    MM_Partgroup partgroup;

                    partgroup = (from partgroupData in db.MM_Partgroup
                                 where partgroupData.Line_ID == lineId
                                 select partgroupData
                             ).FirstOrDefault();

                    MM_OM_OrderRelease mmOmOrderReleaseObj = db.MM_OM_OrderRelease.Find(rowId);

                    MM_OM_Order_List orderList = new MM_OM_Order_List();
                    orderList.Plant_ID = plantId;
                    orderList.Shop_ID = shopId;
                    orderList.Line_ID = lineId;
                    orderList.Station_ID = stationId;
                    orderList.Order_No = mmOmOrderReleaseObj.Order_No;
                    orderList.Model_Code = mmOmOrderReleaseObj.Model_Code;

                    String modelCode = mmOmOrderReleaseObj.Model_Code;

                    //MM_Partmaster partMasterObj = (from partMaster in db.MM_Partmaster
                    //                               where partMaster.Part_No == modelCode
                    //                               select partMaster).Single();

                    var partObj = (from modelMaster in db.MM_Model_Master
                                   where modelMaster.Model_Code == modelCode && modelMaster.Plant_ID == plantId
                                   select new
                                   {
                                       Part_No = modelMaster.Model_Code,
                                       series_code = modelMaster.Series_Code
                                   }).Union(from partMaster in db.MM_Partmaster
                                            where partMaster.Part_No == modelCode && partMaster.Plant_ID == plantId
                                            select new
                                            {
                                                Part_No = partMaster.Part_No,
                                                series_code = partMaster.Series_Code
                                            });


                    foreach (var item in partObj)
                    {

                        orderList.Series_Code = item.series_code;
                    }

                    Int64 psn;
                    // orderList.Series_Code = partMasterObj.Series_Code;
                    try
                    {
                        psn = Convert.ToInt64(db.MM_OM_Order_List.Where(p => p.Shop_ID == shopId && p.Line_ID == lineId && p.Station_ID == stationId).Max(item => item.PSN).ToString());
                    }
                    catch (Exception)
                    {
                        psn = 0;
                    }

                    orderList.PSN = psn + 1;
                    int dsn = orderList.getDSNByDateShop(shopId, lineId);

                    orderList.DSN = dsn + 1;
                    orderList.Entry_Date = DateTime.Now.Date;
                    orderList.Entry_Time = DateTime.Now.TimeOfDay;
                    orderList.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    orderList.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    orderList.Inserted_Date = DateTime.Now;
                    orderList.partno = mmOmOrderReleaseObj.partno;
                    orderList.Order_Status = "Started";
                    orderList.Partgroup_ID = partgroup.Partgroup_ID;
                    //duplicate series Logic 2019-01-01

                    checkduplicateAgain:
                    String serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, mmOmOrderReleaseObj.Order_No, mmOmOrderReleaseObj.Model_Code, mmOmOrderReleaseObj.partno, mmOmOrderReleaseObj.Series_Code, Convert.ToInt16(partgroup.Serial_Config_ID));
                    var duplicate = db.MM_OM_Order_List.Any(s => s.Serial_No == serialNumber && s.Shop_ID == shopId && s.Is_Rejected != true);
                    var IsModelBased = db.MM_Partgroup.Where(p => p.Partgroup_ID == partgroup.Partgroup_ID).Select(p => p.Is_Model_Based).FirstOrDefault();
                    if (duplicate)
                    {
                        if (IsModelBased == false)
                        {
                            try
                            {
                                MM_Partgroup mm_Partgroup = db.MM_Partgroup.Find(partgroup.Partgroup_ID);
                                mm_Partgroup.Running_Serial = mm_Partgroup.Running_Serial + 1;
                                db.Entry(mm_Partgroup).State = EntityState.Modified;
                                db.SaveChanges();
                                goto checkduplicateAgain;
                            }
                            catch (Exception ex)
                            {
                                generalHelper.addControllerException(ex, "DockStartController", "StartOrder(Duplicate check )", ((FDSession)this.Session["FDSession"]).userId);

                            }
                        }
                        else
                        {
                            try
                            {
                                MM_Serial_Number_Data mm_SerialNoData = db.MM_Serial_Number_Data.Where(s => s.Part_No == mmOmOrderReleaseObj.partno && s.Shop_ID == shopId).FirstOrDefault();
                                mm_SerialNoData.Running_Serial = mm_SerialNoData.Running_Serial + 1;
                                db.Entry(mm_SerialNoData).State = EntityState.Modified;
                                db.SaveChanges();
                                goto checkduplicateAgain;
                            }
                            catch (Exception ex)
                            {
                                generalHelper.addControllerException(ex, "DockStartController", "StartOrder(Duplicate check )", ((FDSession)this.Session["FDSession"]).userId);
                            }

                        }
                    }


                    orderList.Serial_No = serialNumber;
                    //update serial_No in kitting master & add kitting barcode in MM_OM_Order_List(Param_1) 
                    //sandip
                    orderList.Param_1 = kitting_Serial_No;
                    int Shift_ID = new MM_Shift().getCurrentRunningShiftByShopID(shopId);
                    orderList.Shift_ID = Shift_ID;
                    db.MM_OM_Order_List.Add(orderList);
                    db.SaveChanges();


                    if (partgroup.Is_Equipment_Create == true)
                    {
                        MM_OM_Order_List mmOmOrderListObj = db.MM_OM_Order_List.Where(p => p.Serial_No == serialNumber).Single();
                        MM_Quality_Captures mmQualityCapture = new MM_Quality_Captures();
                        if (!mmQualityCapture.isEquipmentCreationCompleted(serialNumber))
                        {
                            MM_SAP_Equipment_Creation mmSapEquipmentObj = new MM_SAP_Equipment_Creation();
                            mmSapEquipmentObj.Plant_ID = plantId;
                            mmSapEquipmentObj.Shop_ID = shopId;
                            mmSapEquipmentObj.Line_ID = lineId;
                            mmSapEquipmentObj.Station_ID = stationId;
                            mmSapEquipmentObj.Serial_No = serialNumber.ToUpper();
                            mmSapEquipmentObj.Model_Code = mmOmOrderListObj.Model_Code;
                            mmSapEquipmentObj.Inserted_Date = DateTime.Now;
                            mmSapEquipmentObj.Data_Status = "N";
                            mmSapEquipmentObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mmSapEquipmentObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            db.MM_SAP_Equipment_Creation.Add(mmSapEquipmentObj);
                            db.SaveChanges();
                        }

                    }

                    //save paint orders data for dispatch
                    var BomItemsList = db.MM_BOM_Item.Where(c => c.Model_Code == orderList.Model_Code).Select(c => c.Part_No).ToArray();
                    if (BomItemsList.Count() > 0)
                    {
                        var ModelsList = db.MM_Model_Master.Where(c => BomItemsList.Contains(c.Model_Code) && c.Shop_ID == 7).ToList();
                        foreach (var items in ModelsList)
                        {
                            MM_OM_Tractor_PAINT_PARTS parts = new MM_OM_Tractor_PAINT_PARTS();
                            //   parts.Line_ID = items.Line_ID;
                            parts.Shop_ID = items.Shop_ID;
                            parts.Plant_ID = items.Plant_ID;
                            parts.Line_ID = orderList.Line_ID;
                            parts.Station_ID = orderList.Station_ID;
                            parts.PartNo = items.Model_Code;
                            parts.Tractor_Order_No = orderList.Order_No;
                            parts.Tractor_Serial_No = orderList.Serial_No;
                            parts.Series_Code = Convert.ToDecimal(items.Series_Code);
                            parts.Tractor_Model_Code = orderList.Model_Code;
                            parts.Entry_Date = DateTime.Now.Date;
                            parts.Entry_Time = DateTime.Now.TimeOfDay;
                            parts.Inserted_User_ID = orderList.Inserted_User_ID;
                            parts.Inserted_Host = orderList.Inserted_Host;
                            parts.Inserted_Date = DateTime.Now;
                            parts.Order_Status = "Started";
                            db.MM_OM_Tractor_PAINT_PARTS.Add(parts);
                            db.SaveChanges();
                            //end
                        }
                    }

                    //MM_OM_Kitting_Barcode_Master objKitt = new MM_OM_Kitting_Barcode_Master();
                    //objKitt = (from kitt in db.MM_OM_Kitting_Barcode_Master where kitt.Barcode_String == kitting_Serial_No select kitt).FirstOrDefault();
                    //objKitt.Serial_No = serialNumber;
                    //objKitt.Is_Edited = true;
                    //db.Entry(objKitt).State = EntityState.Modified;
                    //db.SaveChanges();

                    // process to update the running serial number count
                    //if ((shopId == 1 || shopId == 2 || shopId == 3 || shopId == 4) && partgroup.Order_Create == true)
                    if (partgroup.Order_Create == true)
                    {
                        General genObj = new General();
                        int runningSerialCounter = genObj.getCurrentRunningSerial(shopId, mmOmOrderReleaseObj.partno, partgroup.Partgroup_ID);
                        MM_Serial_Number_Data serialNumberDataObj = db.MM_Serial_Number_Data.Where(a => a.Part_No.Trim() == modelCode.Trim()).FirstOrDefault();
                        if (serialNumberDataObj != null)
                        {
                            UpdateModel(serialNumberDataObj);
                            serialNumberDataObj.Running_Serial = runningSerialCounter;
                            serialNumberDataObj.Is_Edited = true;
                            db.Entry(serialNumberDataObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //Start 22 may 2017
                    var is_Print = (from pgroup in db.MM_Partgroup where pgroup.Partgroup_ID == partgroup.Partgroup_ID select pgroup.Is_Print).FirstOrDefault();
                    if (is_Print == true)
                    {

                        // process to add the record in PRN database
                        MM_PRN mmPRNObj = new MM_PRN();
                        mmPRNObj.Plant_ID = plantId;
                        mmPRNObj.Shop_ID = shopId;
                        mmPRNObj.Line_ID = lineId;
                        mmPRNObj.Station_ID = stationId;

                        string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/order_start.prn"));
                        prnFile = prnFile.Replace("012345678910", serialNumber.Trim().ToUpper());
                        prnFile = prnFile.Replace("serial_no", serialNumber.Trim().ToUpper());
                        prnFile = prnFile.Replace("shop_name", shop_name);
                        prnFile = prnFile.Replace("part_no", orderList.partno);


                        prnFile = prnFile.Replace("Production Start", partgroup.Partgrup_Desc);
                        //Series Description
                        try
                        {
                            MM_Series series = db.MM_Series.Where(p => p.Series_Code == orderList.Series_Code).Single();
                            if (series != null)
                            {
                                prnFile = prnFile.Replace("series", series.Series_Description);
                            }
                            else
                            {
                                prnFile = prnFile.Replace("series", " ");
                            }
                        }
                        catch (Exception)
                        {
                            prnFile = prnFile.Replace("series", " ");
                        }


                        prnFile = prnFile.Replace("re_printed", " ");
                        prnFile = prnFile.Replace("date", Convert.ToString(DateTime.Today.Date.ToString("dd-MM-yyyy")));

                        mmPRNObj.PRN_Text = prnFile;

                        mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        mmPRNObj.Inserted_Date = DateTime.Now;
                        mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.MM_PRN.Add(mmPRNObj);
                        db.SaveChanges();
                    }

                    try
                    {
                        Kepware kepwareObj = new Kepware();
                        kepwareObj.lineResume(shopId, Convert.ToInt32(lineId), Convert.ToInt32(stationId), "1");
                        kepwareObj.linePause(shopId, Convert.ToInt32(lineId), Convert.ToInt32(stationId), "0");
                        string sqlQuery = "UPDATE MM_History_LineStop SET Resume_Time = @p1 WHERE Station_ID = @p0 AND isLineStop = 1 AND PLC_Ack = 1 AND Resume_Time IS NULL AND Stop_Reason = 'Serial Not Scanned' AND Line_ID = @p2";
                        db.Database.ExecuteSqlCommand(sqlQuery, stationId, DateTime.Now, lineId);

                    }
                    catch (Exception exp)
                    {
                        while (exp.InnerException != null)
                        {
                            exp = exp.InnerException;
                        }
                        generalHelper.addControllerException(exp, "DockOrderStartController", "Updating History LineStop Table(Line ID : " + lineId + ")", ((FDSession)this.Session["FDSession"]).userId);
                    }
                    //------------------------------------------------------------------

                    MM_Model_Master model_mast;

                    partgroup = (from partgroupData in db.MM_Partgroup
                                 where partgroupData.Line_ID == lineId
                                 select partgroupData
                              ).FirstOrDefault();

                    try
                    {
                        if (partgroup != null)
                        {
                            if (partgroup.Order_Create == true)
                            {
                                // process to update the order release status
                                mmOmOrderReleaseObj = db.MM_OM_OrderRelease.Find(rowId);
                                mmOmOrderReleaseObj.Order_Status = "Started";
                                mmOmOrderReleaseObj.Updated_Date = DateTime.Now;
                                mmOmOrderReleaseObj.RSN = newRSN;
                                mmOmOrderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                mmOmOrderReleaseObj.Is_Edited = true;
                                db.Entry(mmOmOrderReleaseObj).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        while (exp.InnerException != null)
                        {
                            exp = exp.InnerException;
                        }
                        generalHelper.addControllerException(exp, "DockOrderStartController", "Updating Order Release Table(Order RowID:" + mmOmOrderReleaseObj.Row_ID + ",Order No:" + mmOmOrderReleaseObj.Order_No + ")", ((FDSession)this.Session["FDSession"]).userId);
                    }

                    MM_OM_Planned_Orders plannedOrdersObj = db.MM_OM_Planned_Orders.Where(a => a.Order_ID == rowId && a.Planned_Date == DateTime.Today).FirstOrDefault();
                    if (plannedOrdersObj != null)
                    {
                        plannedOrdersObj.Order_Status = "Started";
                        plannedOrdersObj.Last_Status_Change_Time = DateTime.Now;
                        plannedOrdersObj.Is_Edited = true;
                        db.Entry(plannedOrdersObj).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //-----------------------------------------------------------------
                    //Insert Data in Order_Status Table
                    MM_OM_Order_Status mmOrderStatus = new MM_OM_Order_Status();

                    mmOrderStatus.Plant_ID = plantId;
                    mmOrderStatus.Shop_ID = shopId;
                    mmOrderStatus.Line_ID = lineId;
                    mmOrderStatus.Station_ID = stationId;
                    mmOrderStatus.Order_No = mmOmOrderReleaseObj.Order_No;
                    mmOrderStatus.Serial_No = serialNumber;
                    mmOrderStatus.Action_Status = "Started";
                    mmOrderStatus.Entry_Date = DateTime.Now.Date;
                    mmOrderStatus.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mmOrderStatus.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmOrderStatus.Inserted_Date = DateTime.Now;

                    db.MM_OM_Order_Status.Add(mmOrderStatus);
                    db.SaveChanges();


                    //----------------------------Order List---------------------------
                    orderList = new MM_OM_Order_List();
                    orderList = db.MM_OM_Order_List.Where(p => p.Serial_No == serialNumber).Single();

                    // process to add the order in station tracking
                    MM_Station_Tracking mmStationTrackingObj = db.MM_Station_Tracking.Where(p => p.Station_ID == stationId).FirstOrDefault();
                    if (mmStationTrackingObj != null)
                    {
                        mmStationTrackingObj.Plant_ID = plantId;
                        mmStationTrackingObj.Shop_ID = shopId;
                        mmStationTrackingObj.Line_ID = lineId;
                        mmStationTrackingObj.Station_ID = stationId;
                        mmStationTrackingObj.SerialNo = serialNumber;
                        mmStationTrackingObj.Track_SerialNo = serialNumber;
                        mmStationTrackingObj.Inserted_Date = DateTime.Now;
                        mmStationTrackingObj.Is_Edited = true;
                        //db.MM_Station_Tracking.Add(mmStationTrackingObj);
                        db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    // process to update the quality ok order
                    partNo = partNo[0].Split(',');
                    serialNo = serialNo[0].Split(',');
                    for (int i = 0; i < serialNo.Count(); i++)
                    {
                        String geneologySerialNumber = serialNo[i];
                        MM_Quality_OK_Order mmQualityOkOrder = db.MM_Quality_OK_Order.Where(p => p.Serial_No == geneologySerialNumber).FirstOrDefault();
                        if (mmQualityOkOrder != null)
                        {
                            mmQualityOkOrder.Is_Pulled = true;
                            mmQualityOkOrder.Is_Edited = true;
                            db.Entry(mmQualityOkOrder).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        MM_OM_Order_List orderListObj = db.MM_OM_Order_List.Where(p => p.Serial_No == geneologySerialNumber).FirstOrDefault();
                        if (orderListObj != null)
                        {
                            orderListObj.Order_Status = "Closed";
                            orderListObj.Is_Edited = true;
                            orderListObj.Updated_Date = DateTime.Now;
                            db.Entry(orderListObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }

                    // process to update the retrieval table
                    for (int i = 0; i < serialNo.Count(); i++)
                    {
                        String geneologySerialNumber = serialNo[i];
                        MM_OM_Order_Retrieval mmOmOrderRetrievalObj = new MM_OM_Order_Retrieval();

                        mmOmOrderRetrievalObj.Plant_ID = plantId;
                        mmOmOrderRetrievalObj.Shop_ID = shopId;
                        mmOmOrderRetrievalObj.Line_ID = lineId;
                        mmOmOrderRetrievalObj.Station_ID = stationId;
                        mmOmOrderRetrievalObj.Main_Serial_No = serialNumber;
                        mmOmOrderRetrievalObj.Main_Model_Code = modelCode;
                        mmOmOrderRetrievalObj.Requested_Model_Code = partNo[i];
                        mmOmOrderRetrievalObj.Provided_Serial_No = serialNo[i];

                        mmOmOrderRetrievalObj.Provided_Model_Code = partNo[i];
                        mmOmOrderRetrievalObj.Is_Deleted = false;
                        mmOmOrderRetrievalObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        mmOmOrderRetrievalObj.Inserted_Date = DateTime.Now;
                        mmOmOrderRetrievalObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.MM_OM_Order_Retrieval.Add(mmOmOrderRetrievalObj);
                        db.SaveChanges();
                        // MM_OM_Order_List orderListObj = db.MM_OM_Order_List.Where(p => p.Serial_No == geneologySerialNumber).Single();
                        String errorProofingPartNumber = partNo[i].ToString();
                        MM_Partmaster partMasterObj = db.MM_Partmaster.Where(p => p.Part_No == errorProofingPartNumber).FirstOrDefault();
                        if (partMasterObj != null)
                        {
                            // process to add geneology items
                            MM_Geneaology mmGeneologyItemObj = new MM_Geneaology();
                            mmGeneologyItemObj.Plant_ID = plantId;
                            mmGeneologyItemObj.Shop_ID = shopId;
                            mmGeneologyItemObj.Line_ID = lineId;
                            mmGeneologyItemObj.Station_ID = stationId;
                            mmGeneologyItemObj.Main_Order_Serial_No = serialNumber;
                            mmGeneologyItemObj.Main_Model_Code = modelCode;
                            mmGeneologyItemObj.Child_Order_Serial_No = serialNo[i];
                            mmGeneologyItemObj.Child_Model_Code = partNo[i];
                            mmGeneologyItemObj.PartGroup_ID = partMasterObj.Partgroup_ID;
                            mmGeneologyItemObj.Inserted_Date = DateTime.Now;
                            mmGeneologyItemObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mmGeneologyItemObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            db.MM_Geneaology.Add(mmGeneologyItemObj);
                            db.SaveChanges();
                        }

                    }

                    // process to add the order in quality order queue

                    MM_Quality_Captures mmQualityCapturesObj = new MM_Quality_Captures();

                    mmQualityCapturesObj.insertOrderInQualityOrderQueue(serialNumber, mmOmOrderReleaseObj.Order_No, plantId, shopId, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);

                    return Json(orderList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                generalHelper.addControllerException(ex, "DockOrderStartController", "StartOrder", ((FDSession)this.Session["FDSession"]).userId);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        // POST: DockOrderStart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MM_OM_OrderRelease mM_OM_OrderRelease)
        {
            if (ModelState.IsValid)
            {
                db.MM_OM_OrderRelease.Add(mM_OM_OrderRelease);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_OM_OrderRelease.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_OM_OrderRelease.Shop_ID);
            return View(mM_OM_OrderRelease);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        class JSONData
        {
            public bool status { get; set; }
            public string type { get; set; }
            public string message { get; set; }
        }

        public ActionResult isOrderReadyToStart(int stationId, decimal rowId)
        {
            JSONData jsonData = new JSONData();
            try
            {
                MM_OM_OrderRelease orderRlsObj = db.MM_OM_OrderRelease.Find(rowId);
                String trackingSerialNo = db.MM_Station_Tracking.Where(p => p.Station_ID == stationId).Select(a => a.Track_SerialNo).FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(trackingSerialNo))
                {
                    jsonData.status = false;
                    jsonData.message = "Please Scan after current tractor moves to next station!";
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else if (orderRlsObj.Is_Blocked == true)
                {
                    jsonData.status = false;
                    jsonData.message = "This Order: " + orderRlsObj.Order_No + " is Blocked for sequencing right now.Please try after sometime.";
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    jsonData.status = true;
                    jsonData.message = "Success";
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                jsonData.status = false;
                jsonData.message = "Exception Occurred !";
                generalHelper.addControllerException(ex, "DockOrderStartController", "isOrderReadyToStart", ((FDSession)this.Session["FDSession"]).userId);
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult validateQualityOKOrder(String[] partNo, String[] serialNo, String modelCode, string kitting_Serialno)
        {
            try
            {
                int flag = 0;
                partNo = partNo[0].Split(',');
                serialNo = serialNo[0].Split(',');
                ErrorProofingRes[] errorProofingRes = new ErrorProofingRes[partNo.Count()];

                //if (db.MM_OM_Kitting_Barcode_Master.Where(kit => kit.Barcode_String == kitting_Serialno && kit.Is_Consumed == false).Count() > 0)
                {
                    String serialNumber = "";
                    String part_number = "";
                    for (int i = 0; i < serialNo.Count(); i++)
                    {
                        errorProofingRes[i] = new ErrorProofingRes();
                        errorProofingRes[i].partNo = partNo[i];
                        errorProofingRes[i].serialNo = serialNo[i];
                        errorProofingRes[i].modelCode = modelCode;
                        serialNumber = serialNo[i];
                        part_number = partNo[i];

                        try
                        {

                            //MM_Quality_OK_Order mmQualityOKOrderObj = (from mmQualityOkOrder in db.MM_Quality_OK_Order
                            //                                           where mmQualityOkOrder.Serial_No == serialNumber && mmQualityOkOrder.Is_Pulled != true && mmQualityOkOrder.Part_No == part_number
                            //                                           select mmQualityOkOrder).Single();

                            String partNumber = partNo[i];

                            MM_OM_Order_List orderListObj = (from orderList in db.MM_OM_Order_List
                                                             where orderList.Serial_No == serialNumber && orderList.partno == partNumber
                                                             select orderList).FirstOrDefault();

                            if (orderListObj == null)
                            {
                                errorProofingRes[i].isOK = false;
                                errorProofingRes[i].errorMessage = "Invalid serial number";
                                continue;
                            }

                            MM_Geneaology mmGenealogyObj = (from genealogy in db.MM_Geneaology
                                                            where genealogy.Child_Order_Serial_No == serialNumber
                                                            select genealogy).FirstOrDefault();
                            if (mmGenealogyObj == null)
                            {
                                flag++;
                                errorProofingRes[i].isOK = true;
                            }
                            else
                            {
                                errorProofingRes[i].isOK = false;
                                errorProofingRes[i].errorMessage = "Vehicle is already added to other tractor";
                            }

                            //if (mmQualityOKOrderObj != null)
                            //{
                            //    flag++;
                            //    errorProofingRes[i].isOK = true;
                            //}
                            //else
                            //{
                            //    errorProofingRes[i].isOK = false;
                            //    errorProofingRes[i].errorMessage = "No vehicle found with quality ok";
                            //}
                        }
                        catch (Exception ex1)
                        {
                            errorProofingRes[i].isOK = false;
                            errorProofingRes[i].errorMessage = "No vehicle found with quality ok";
                        }

                    }
                    //if (flag > 1)
                    //{
                    //    MM_OM_Kitting_Barcode_Master kit = new MM_OM_Kitting_Barcode_Master();
                    //    kit = (from kitt in db.MM_OM_Kitting_Barcode_Master where kitt.Barcode_String == kitting_Serialno select kitt).FirstOrDefault();
                    //    kit.Is_Consumed = true;
                    //    kit.Is_Edited = true;
                    //    db.Entry(kit).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //    errorProofingRes[0].isKitAvailable = true;
                    //}

                    return Json(errorProofingRes, JsonRequestBehavior.AllowGet);
                }
                //else
                //{
                //    errorProofingRes[0] = new ErrorProofingRes();
                //    errorProofingRes[0].isKitAvailable = false;
                //    errorProofingRes[0].errorMessage = "Kit not available";
                //    return Json(errorProofingRes, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "DockOrderStartController", "validateQualityOKOrder", ((FDSession)this.Session["FDSession"]).userId);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getErrorProofingGeneologyPartsByModelCode(String modelCode, int stationId)
        {
            try
            {
                //MM_Partmaster[] mmPartmasterObj = errorProofingObj.getStationPartList(stationId, modelCode);
                //var partMaster = (from mmPartMaster in db.MM_Partmaster
                //                  where (mmPartMaster.Genealogy == true || mmPartMaster.Error_Proofing == true) && mmPartMaster.Station_ID == stationId &&
                //                  (from mmBomItem in db.MM_BOM_Item where mmBomItem.Model_Code == modelCode select mmBomItem.Part_No).Contains(mmPartMaster.Part_No)
                //                  select new
                //                  {
                //                      Plant_ID = mmPartMaster.Plant_ID,
                //                      Shop_ID = mmPartMaster.Shop_ID,
                //                      Line_ID = mmPartMaster.Line_ID,
                //                      Station_ID = mmPartMaster.Station_ID,

                //                      Part_No = mmPartMaster.Part_No
                //                  });

                string query = "select * from MM_Partmaster where Station_ID = " + stationId + " and Part_No in(select part_no from MM_BOM_Item where model_code = '" + modelCode + "') and MM_Partmaster.Genealogy = 1 and MM_Partmaster.Error_Proofing = 1 order by PartGroup_ID ";
                var plan_order = db.Database.SqlQuery<MM_Partmaster>(query).ToList();
                var partMaster = from order in plan_order
                                 select new
                                 {
                                     Plant_ID = order.Plant_ID,
                                     Shop_ID = order.Shop_ID,
                                     Line_ID = order.Line_ID,
                                     Station_ID = order.Station_ID,

                                     Part_No = order.Part_No
                                 };

                return Json(partMaster.ToList(), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                General genObj = new General();
                genObj.addControllerException(ex, "DockOrderStart", "getErrorProofingGeneologyPartsByModelCode", 1);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult isReprint(string serial_number, int Shop_Id)
        {
            try
            {
                string shop_name = "";
                //find Shop Name
                MM_Shops shop;
                shop = (from shopName in db.MM_Shops
                        where shopName.Shop_ID == Shop_Id
                        select shopName).FirstOrDefault();

                if (shop != null)
                {
                    shop_name = shop.Shop_Name;
                }

                MM_OM_Order_List order_list = db.MM_OM_Order_List.Where(p => p.Shop_ID == Shop_Id && p.Serial_No == serial_number).FirstOrDefault();
                if (order_list.Serial_No != null)
                {

                    int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                    int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                    int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                    int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);


                    // process to add the record in PRN database
                    MM_PRN mmPRNObj = new MM_PRN();
                    mmPRNObj.Plant_ID = plantId;
                    mmPRNObj.Shop_ID = shopId;
                    mmPRNObj.Line_ID = lineId;
                    mmPRNObj.Station_ID = stationId;

                    // process to get Quality Ok PRN file
                    string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/order_start.prn"));
                    prnFile = prnFile.Replace("012345678910", serial_number.Trim().ToUpper());
                    prnFile = prnFile.Replace("serial_no", serial_number.Trim().ToUpper());
                    prnFile = prnFile.Replace("shop_name", shop_name);
                    prnFile = prnFile.Replace("part_no", order_list.partno);

                    //Series Description
                    try
                    {
                        MM_Series series = db.MM_Series.Where(p => p.Series_Code == order_list.Series_Code).Single();
                        if (series != null)
                        {
                            prnFile = prnFile.Replace("series", series.Series_Description);
                        }
                        else
                        {
                            prnFile = prnFile.Replace("series", " ");
                        }
                    }
                    catch (Exception)
                    {
                        prnFile = prnFile.Replace("series", " ");
                    }

                    prnFile = prnFile.Replace("re_printed", "Reprinted");
                    prnFile = prnFile.Replace("date", Convert.ToString(DateTime.Today.Date.ToString("dd-MM-yyyy")));

                    mmPRNObj.PRN_Text = prnFile;

                    mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmPRNObj.Inserted_Date = DateTime.Now;
                    mmPRNObj.Inserted_Host = "sample";
                    db.MM_PRN.Add(mmPRNObj);
                    db.SaveChanges();


                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "DockOrderStartController", "isReprint", ((FDSession)this.Session["FDSession"]).userId);
                return Json(false, JsonRequestBehavior.AllowGet);
            }


        }

        public bool checkIsModelStart(int rowId, int PlantId)
        {
            try
            {
                string m_code = db.MM_OM_OrderRelease.Where(r => r.Row_ID == rowId).Select(r => r.Model_Code).FirstOrDefault();
                var time = (from tbmc in db.MM_OM_TimeBased_ModelConfig
                            join mm in db.MM_Model_Master on tbmc.Model_ID equals mm.Model_ID
                            where mm.Model_Code == m_code && tbmc.Plant_ID == PlantId
                            select new { tbmc.StartTime, tbmc.EndTime }).ToList();
                if (time.Count > 0)
                {
                    foreach (var item in time)
                    {
                        modelStartTime = item.StartTime;
                        modelEndTime = item.EndTime;
                        DateTime start_dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                              CultureInfo.InvariantCulture);

                        DateTime end_dateTime = DateTime.ParseExact(item.EndTime, "HH:mm:ss",
                                             CultureInfo.InvariantCulture);

                        DateTime current_dateTime = DateTime.ParseExact(DateTime.Now.ToString("HH:mm:ss"), "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                        if ((start_dateTime < current_dateTime) && (end_dateTime > current_dateTime))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
