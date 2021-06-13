using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Reporting.WebForms;
using System.IO;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using REIN_MES_System.Helper.IoT;
using System.Xml;
using System.Xml.Linq;

namespace REIN_MES_System.Controllers.OrderManagement
{
    /*               Controller Name           : OrderStartController
     *               Description               : Controller used to start the order as per order configuration one or multiple shop.
     *               Author, Timestamp         : Jitendra Mahajan
     */
    public class OrderStartController : BaseController
    {

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_OM_OrderRelease[] orderRelease;
        General generalHelper = new General();
        //List<RS_MT_Clita> ClitaObjList = new List<RS_MT_Clita>();
        MetaShift objShift = new MetaShift();
        string modelStartTime = string.Empty;
        string modelEndTime = string.Empty;
        /*               Action Name               : Create
         *               Description               : Action used to show the list of order Create data.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        // GET: OrderStart/Create
        public ActionResult Create()
        {
            try
            {
                DateTime today = DateTime.Today;
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                ViewBag.MakeDetails = new SelectList(db.RS_Model_Tyre_Make, "Tyre_Make_ID", "Make_Name", "Tyre_Make_ID");
                //return RedirectToAction("Create", "DockOrderStart");
                //if (stationId == 101)
                //{
                //    //RedirectToAction("Create","DockOrderStart");
                //    return RedirectToAction("Create", "DockOrderStart");
                //}
                string MachineName = string.Empty;
                int flag = 0;

                int shiftID = objShift.getShiftID();
                //using (MTTUWEntities MTTUW_db = new MTTUWEntities())
                //{
                //    var machine = MTTUW_db.MM_MT_MTTUW_Machines.Where(mac => mac.Shop_ID == shopId && mac.Station_ID == stationId).Select(mac => new { mac.Machine_ID, mac.Machine_Name });
                //    foreach (var item in machine)
                //    {
                //        List<Decimal> todaysPendingClitaID = MTTUW_db.MM_MT_Daily_Clita_Log.Where(a => DbFunctions.TruncateTime(a.Inserted_Date) == today && a.Shift_ID == shiftID).Select(a => a.Clita_ID).ToList();
                //        //var ClitaObjList1 = MTTUW_db.RS_MT_Clita.Where(a => a.MM_MT_MTTUW_Machines.Shop_ID == shopId && a.MM_MT_MTTUW_Machines.Station_ID == stationId && a.Machine_ID == item.Machine_ID).ToList();
                //        ClitaObjList = MTTUW_db.RS_MT_Clita.Where(a => !todaysPendingClitaID.Contains(a.Clita_ID) && a.MM_MT_MTTUW_Machines.Shop_ID == shopId && a.MM_MT_MTTUW_Machines.Station_ID == stationId && a.Machine_ID == item.Machine_ID).ToList();
                //        if (ClitaObjList.Count > 0)
                //        {
                //            flag++;
                //            MachineName += item.Machine_Name + ",";
                //        }
                //    }

                //}
                //TempData["MachineName"] = MachineName;
                if (flag > 0)
                {
                    ViewBag.flag = 0;
                }
                else
                {
                    ViewBag.flag = 1;
                }
                //for 3rd shift bypass after 12 
                decimal sID = Convert.ToDecimal(shiftID);
                string shiftName = db.RS_Shift.Where(shift => shift.Shift_ID == sID).Select(shift => shift.Shift_Name).FirstOrDefault();
                if (shiftName == "C")
                {
                    TimeSpan dt = DateTime.Now.TimeOfDay;
                    int hours = dt.Hours;
                    if (hours == 0 || (hours > 1 && hours < 7))
                    {
                        ViewBag.flag = 1;
                    }
                }

                RS_OM_OrderRelease mmOmRelease = new RS_OM_OrderRelease();
                orderRelease = mmOmRelease.getTodayReleasedOrderDetails(shopId);

                ViewBag.Plant_ID_User = plantId;
                ViewBag.Shop_ID_User = shopId;
                ViewBag.Line_ID_User = lineId;
                ViewBag.Station_ID_User = stationId;
                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
                //added by ketan to show ppc kitting order start
                RS_Partgroup partgroup;
                partgroup = (from partgroupData in db.RS_Partgroup
                             where partgroupData.Line_ID == lineId && partgroupData.Plant_ID == plantId
                             select partgroupData
                          ).FirstOrDefault();
                if (partgroup != null && partgroup.Is_PPC_Kitting_Order != null)
                {
                    ViewBag.Is_PPC_Kitting_Order = partgroup.Is_PPC_Kitting_Order;
                }
                ViewBag.Total_Genealogy_Count = partgroup.Total_Genealogy_Count;
                ViewBag.Is_Kitting = partgroup.Is_Kitting;

               // ViewBag.Is_FIFO_Order_Start = partgroup.Is_FIFO_Order_Start;
                if (partgroup.Is_Batch_Order_Start == true)
                {
                    return RedirectToAction("BatchOrderCreate", "OrderStart");
                }
                else
                if (partgroup.Is_Bulk_Order_Start == true)
                {
                    return RedirectToAction("BulkOrderCreate", "OrderStart");
                }
                //Added by ketan to show spare order list
                var orderType = "Spare";
                var orderStatus = "Release";
                //if (db.RS_OM_OrderRelease.Any(m => DbFunctions.TruncateTime(m.Planned_Date) == DbFunctions.TruncateTime(DateTime.Now)
                //&& m.Order_Type.Trim().ToLower() == orderType.Trim().ToLower() && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId))
                //{
                //    ViewBag.Is_Spare = true;
                //}


                ///added by mukesh
                string stationName = ((FDSession)this.Session["FDSession"]).stationName;
                //string platformName =
                //var t = db.RS_OM_Platform.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId && m.Platform_Name == stationName).Select(p => p.Platform_ID).FirstOrDefault();
                //if (db.RS_OM_OrderRelease.Any(m => DbFunctions.TruncateTime(m.Planned_Date) == DbFunctions.TruncateTime(DateTime.Now)
                //&& m.Order_Type.Trim().ToLower() == orderType.Trim().ToLower() && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId))

                //RS_OM_OrderRelease orderRelease = new RS_OM_OrderRelease(); 
                var orderReleaseSpare = db.RS_OM_OrderRelease.Where(m => DbFunctions.TruncateTime(m.Planned_Date) == DbFunctions.TruncateTime(DateTime.Now)
                && m.Order_Type.Trim().ToLower() == orderType.Trim().ToLower() && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId && m.Order_Status.Trim().ToLower() == orderStatus.Trim().ToLower()).ToList();
                ViewBag.Is_Spare = false;
                //foreach (var row in orderReleaseSpare)
                //{
                //    //if (db.RS_Model_Master.Any(c => c.Platform_Id == t && row.Model_Code == c.Model_Code))
                //    if (db.RS_Model_Master.Any(c => row.Model_Code == c.Model_Code))
                //    {
                //        ViewBag.Is_Spare = true;
                //        break;
                //    }
                //}
                //////




                ViewBag.ToTal = db.RS_OM_OrderRelease.Where(m => DbFunctions.TruncateTime(m.Planned_Date) == DbFunctions.TruncateTime(DateTime.Now) && m.Line_ID == lineId).Select(m => m.Row_ID).Count();
                ViewBag.Completed = db.RS_OM_OrderRelease.Where(m => DbFunctions.TruncateTime(m.Planned_Date) == DbFunctions.TruncateTime(DateTime.Now) && m.Order_Status == "Started" && m.Line_ID == lineId).Select(m => m.Row_ID).Count();
                ViewBag.Balance = db.RS_OM_OrderRelease.Where(m => DbFunctions.TruncateTime(m.Planned_Date) == DbFunctions.TruncateTime(DateTime.Now) && m.Order_Status == "Release" && m.Line_ID == lineId).Select(m => m.Row_ID).Count();
                //ViewBag.CurrentModel = db.RS_OM_Order_List.Select(m => m.Model_Code).ToString();


                ViewBag.CurrentEngine = "";
                ViewBag.CurrentModel = "";

                
                
                // ViewBag.CurrentModel = db.RS_OM_Order_List.Select(m => m.Model_Code).ToString();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }


            return View();
        }
        public ActionResult GenerateXML(string OrderNo, string Modelcode, string serialNo,string typename)
        {
            try
            {
                int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);

                RS_Stations station = db.RS_Stations.Find(stationId);
                RS_Model_Master model = db.RS_Model_Master.Where(c=>c.Model_Code==Modelcode).FirstOrDefault();
                string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                path = path + "\\App_Data\\XMLFiles\\"+ OrderNo+"_"+ typename + ".xml";

                RS_Quality_Captures shift = new RS_Quality_Captures();
                var shiftdata = shift.getCurrentRunningShiftByShopID(shopId);
                string shiftname = "";
                if(shiftdata!=null)
                {
                    shiftname = shiftdata.Shift_Name;
                }
                using (XmlWriter writer = XmlWriter.Create(path))
                {
                    if (typename == "build")
                    {
                        // start output  buildinfo
                        writer.WriteStartElement("output");
                        writer.WriteAttributeString("type", typename);


                        writer.WriteStartElement("buildinfo");  // start job buildinfo
                        writer.WriteAttributeString("workorder", OrderNo);
                        writer.WriteAttributeString("status", "Started");
                        writer.WriteAttributeString("timestamp", System.DateTime.Now.ToString());
                        writer.WriteAttributeString("stationname", station.Station_Name.ToString());


                        writer.WriteStartElement("jobinfo"); // start job info
                        writer.WriteAttributeString("model", "---");
                        writer.WriteEndElement(); // end job info

                        writer.WriteStartElement("partinfo"); // start part info
                        writer.WriteAttributeString("partnumber", model.Model_Code);
                        writer.WriteAttributeString("partdesc", model.Model_Description);
                        writer.WriteEndElement(); // end part info

                        writer.WriteEndElement();// end job buildinfo


                        writer.WriteEndElement();// end output  buildinfo
                        writer.Flush();
                    }
                    else if (typename == "pass")
                    {
                        // start output  buildinfo
                        writer.WriteStartElement("output");
                        writer.WriteAttributeString("type", typename);


                        writer.WriteStartElement("buildinfo");  // start job buildinfo
                        writer.WriteAttributeString("workorder", OrderNo);
                        writer.WriteAttributeString("status", "Started");
                        writer.WriteAttributeString("timestamp", System.DateTime.Now.ToString());
                        writer.WriteAttributeString("stationname", station.Station_Name.ToString());


                        writer.WriteStartElement("jobinfo"); // start job info
                        writer.WriteAttributeString("partid", "");
                        writer.WriteAttributeString("vendorid", "");
                        writer.WriteAttributeString("timestamp", System.DateTime.Now.ToString());
                        writer.WriteAttributeString("shift", shiftname);


                        writer.WriteEndElement(); // end job info

                        writer.WriteStartElement("partinfo"); // start part info
                        writer.WriteAttributeString("partnumber", model.Model_Code);
                        writer.WriteAttributeString("partdesc", model.Model_Description);
                        writer.WriteEndElement(); // end part info

                        writer.WriteEndElement();// end job buildinfo


                        writer.WriteEndElement();// end output  buildinfo
                        writer.Flush();
                    }

                 }
            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
       
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BulkOrderCreate()
        {
            DateTime today = DateTime.Today;
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

            int shiftID = objShift.getShiftID();

            RS_OM_OrderRelease mmOmRelease = new RS_OM_OrderRelease();

            ViewBag.Plant_ID_User = plantId;
            ViewBag.Shop_ID_User = shopId;
            ViewBag.Line_ID_User = lineId;
            ViewBag.Station_ID_User = stationId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
           // ViewBag.Remark_ID = new SelectList(db.MM_OM_Hold_Remark, "Remark_ID", "Remark");
            try
            {
                
                RS_Partgroup partgroup;
                partgroup = (from partgroupData in db.RS_Partgroup
                             where partgroupData.Line_ID == lineId && partgroupData.Plant_ID == plantId && partgroupData.Station_ID == stationId
                             select partgroupData
                          ).FirstOrDefault();
                ViewBag.Total_Genealogy_Count = partgroup.Total_Genealogy_Count;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "OrderStart", "BulkOrderCreate");
            }

            return View();
        }

        public ActionResult ShowBulkOrderList(int orderListShopId, int orderListLineId)
        {
            try
            {
                
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int shopId = orderListShopId;
                int lineId = orderListLineId;
                int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

                IEnumerable<RS_OM_OrderRelease> PlannedOrderObj = null;
                RS_OM_PPC_Daily_Plan ppcDailyPlanObj = db.RS_OM_PPC_Daily_Plan.Where(p => p.Line_ID == lineId && p.Shop_ID == shopId).OrderByDescending(p => p.Plan_Date).FirstOrDefault();
                if (ppcDailyPlanObj != null)
                {
                    PlannedOrderObj = (from orderReleaseItem in db.RS_OM_OrderRelease
                                       orderby orderReleaseItem.RSN ascending
                                       where orderReleaseItem.Planned_Date == ppcDailyPlanObj.Plan_Date && orderReleaseItem.Line_ID == orderListLineId && orderReleaseItem.Order_Status == "Release"
                                       select orderReleaseItem).ToList();
                }
                else
                {
                    PlannedOrderObj = (from orderReleaseItem in db.RS_OM_OrderRelease
                                       orderby orderReleaseItem.RSN ascending
                                       where orderReleaseItem.Planned_Date == DateTime.Today && orderReleaseItem.Line_ID == orderListLineId && orderReleaseItem.Order_Status == "Release"
                                       select orderReleaseItem).ToList();
                }

                var ModelsList = db.RS_Model_Master.Where(p => p.Shop_ID == orderListShopId).ToList();
                var result = PlannedOrderObj.Select(c => new BulkOrderList
                {
                    part_no = c.partno,
                    part_description = (from model in ModelsList
                                        where model.Model_Code == c.partno
                                        select model.Model_Description).FirstOrDefault(),
                    Color = (from orderRelease in db.RS_OM_OrderRelease
                             where orderRelease.Shop_ID == orderListShopId && orderRelease.Line_ID == orderListLineId && orderRelease.Planned_Date == DateTime.Today && orderRelease.partno == c.partno
                             select orderRelease.Model_Color).FirstOrDefault(),
                    order_no = c.Order_No,
                    row_id = c.Row_ID,

                    planned_qty = (from orderRelease in db.RS_OM_OrderRelease
                                   where orderRelease.Shop_ID == orderListShopId && orderRelease.Line_ID == orderListLineId && orderRelease.Planned_Date == DateTime.Today && orderRelease.partno == c.partno
                                   select orderRelease).ToList().Count(),

                    started_qty = (from orderRelease in db.RS_OM_OrderRelease
                                   where orderRelease.Shop_ID == orderListShopId && orderRelease.Line_ID == orderListLineId && orderRelease.Order_Status == "Started" && orderRelease.Planned_Date == DateTime.Today && orderRelease.partno == c.partno
                                   select orderRelease).Count(),


                    pending_qty = (from orderRelease in db.RS_OM_OrderRelease
                                   where orderRelease.Shop_ID == orderListShopId && orderRelease.Line_ID == orderListLineId && orderRelease.Order_Status == "Release" && orderRelease.Planned_Date == DateTime.Today
                                   && orderRelease.partno == c.partno
                                   select orderRelease).Count(),

                }).ToList();
                return PartialView(result);
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "OrderStart", "ShowBulkOrderList");
                return PartialView();
            }


        }

        public ActionResult BatchOrderCreate()
        {
            DateTime today = DateTime.Today;
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

            int shiftID = objShift.getShiftID();

            RS_OM_OrderRelease mmOmRelease = new RS_OM_OrderRelease();

            ViewBag.Plant_ID_User = plantId;
            ViewBag.Shop_ID_User = shopId;
            ViewBag.Line_ID_User = lineId;
            ViewBag.Station_ID_User = stationId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            //ViewBag.Remark_ID = new SelectList(db.MM_OM_Hold_Remark, "Remark_ID", "Remark");


            return View();
        }

        //public ActionResult ShowBatchOrderList(int orderListShopId, int orderListLineId)
        //{
        //    try
        //    {
        //        DataTable dt = GetTotalBatchOrder(orderListShopId, orderListLineId);
        //        var result = dt.AsEnumerable().Select(c => new BulkOrderList
        //        {
        //            part_no = c["part_no"].ToString(),
        //            part_description = c["Model_Description"].ToString(),
        //            order_no = "",
        //            row_id = Convert.ToInt32(c["Row_ID"]),
        //            planned_qty = Convert.ToInt16(c["planned_qty"]),
        //            started_qty = Convert.ToInt16(c["started_qty"]),
        //            pending_qty = Convert.ToInt16(c["pending_qty"]),

        //        }).ToList();
        //        return PartialView(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        generalHelper.addControllerException(ex, "OrderStart", "ShowBatchOrderList");
        //        return PartialView();
        //    }
        //}

        //public DataTable GetTotalBatchOrder(int shopId, int lineId)
        //{
        //    DataTable dt = new DataTable();
        //    try
        //    {

        //        string conStr = ConfigurationManager.ConnectionStrings["FDEntities_SP"].ToString();        // GET: Maintanance
        //        using (SqlConnection con = new SqlConnection(conStr))
        //        {
        //            con.Open();
        //            using (SqlCommand cmd = new SqlCommand("PROC_BATCH_ORDER", con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@Line_Id", lineId);
        //                cmd.Parameters.AddWithValue("@Shop_Id", shopId);
        //                SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                da.Fill(dt);
        //                con.Close();
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        generalHelper.addControllerException(ex, "OrderStart", "GetTotalBatchOrder");
        //    }
        //    return dt;
        //}


        public ActionResult GetCurrentDetails(int orderListShopId)
        {


            ViewBag.flag = 1;

            //var CompletedOrder = (from orderReleaseItem in db.RS_OM_OrderRelease
            //                   where orderReleaseItem.Shop_ID == orderListShopId && orderReleaseItem
            //                   select orderReleaseItem
            //          ).Count();
            // ViewBag.CompletedOrder = CompletedOrder;

            // return PartialView(CompletedOrder);

            //ViewBag.Plant_ID = new SelectList(db.MM_Family.Where(m => m.Plant_ID == plantID), "Plant_ID", "Plant_Name");

            //var Total = db..Where(m => m.Plant_ID == plantID);
            //return View(mm_modl.ToList());

            return PartialView();
        }




        /*               Action Name               : ShowOrderListByShop
         *               Description               : Action used to show the order List.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : orderListShopId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult ShowOrderListByShop(int orderListShopId)
        {


            ViewBag.flag = 1;
            var orderDetail = (from orderReleaseItem in db.RS_OM_OrderRelease
                               orderby orderReleaseItem.RSN ascending
                               where orderReleaseItem.Shop_ID == orderListShopId && orderReleaseItem.Order_Status == "Release"
                               select orderReleaseItem).Take(5).ToList();
            return PartialView(orderDetail);
        }

        public DateTime getPlannedDate()
        {
            DateTime plannedDateTime = DateTime.Now;
            DateTime plannedDate;
            TimeSpan currentTime = plannedDateTime.TimeOfDay;

            TimeSpan firstShiftStartTime = new TimeSpan(7, 0, 0);
            if (currentTime < firstShiftStartTime)
            {
                plannedDate = plannedDateTime.Date.AddDays(-1);
            }
            else
            {
                plannedDate = plannedDateTime.Date;
            }
            return plannedDate;
        }
        public ActionResult ShowOrderListByShopAndLine(int orderListShopId, int orderListLineId)
        {

            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            var plannedDate = getPlannedDate();
            IEnumerable<RS_OM_OrderRelease> previousOrderObj = null;
            IEnumerable<RS_OM_OrderRelease> TodayPlannedOrders = null;
            IEnumerable<RS_OM_OrderRelease> TodayAllPlannedOrders = null;
            IEnumerable<RS_OM_OrderRelease> TotalOrder = null;
            IEnumerable<RS_OM_OrderRelease> orderDetail = null;
            RS_Partgroup partgroup;
            RS_Model_Master model_mast;


            partgroup = (from partgroupData in db.RS_Partgroup
                         where partgroupData.Line_ID == lineId && partgroupData.Plant_ID == plantId
                         select partgroupData
                      ).FirstOrDefault();
            ViewBag.PartGroup = partgroup;
            if (partgroup != null)
            {
                if (partgroup.Order_Create == false)
                {
                    if (partgroup.Is_Kitting == true)
                    {
                        TempData["Is_Kitting"] = 1;
                    }
                    else
                    {
                        TempData["Is_Kitting"] = 0;
                    }
                }
                else
                {
                    TempData["Is_Kitting"] = 0;
                }
            }
            var platformName = "";
            platformName = db.RS_OM_Platform.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId).Select(m => m.Platform_Name).FirstOrDefault();

            if (partgroup != null)
            {
                if (partgroup.Order_Create == true)
                {
                    //    orderDetail = (from orderReleaseItem in db.RS_OM_OrderRelease
                    //                   orderby orderReleaseItem.RSN ascending
                    //                   where orderReleaseItem.Shop_ID == orderListShopId && orderReleaseItem.Line_ID == orderListLineId && orderReleaseItem.Order_Status == "Release" && orderReleaseItem.Plant_ID == plantId
                    //                  // && (orderReleaseItem.Planned_Date.Value.Date >= DateTime.Now.Date && orderReleaseItem.Planned_Date.Value.Date <= DateTime.Now)
                    //                  && DbFunctions.TruncateTime(orderReleaseItem.Planned_Date.Value)== DateTime.Now.Date
                    //                   select orderReleaseItem).Take(20).ToList().OrderByDescending(m => m.Order_No);
                    //orderDetail = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == DateTime.Now.Year && or.Planned_Date.Value.Month == DateTime.Now.Month && or.Planned_Date.Value.Day == DateTime.Now.Day).Select(or => or).Take(20).ToList().OrderByDescending(m => m.Order_No);
                    //Shiftwise production
                    //finding current shift
                    DateTime date = DateTime.Now;
                    TimeSpan currentTime = date.TimeOfDay;

                    var shift = db.RS_Shift.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Shift_Start_Time <= currentTime && currentTime <= m.Shift_End_Time).Select(m => m).FirstOrDefault();
                    var plannedQty = 0;
                    plannedQty = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == shopId && m.Line_ID == lineId && m.Plan_Date == plannedDate.Date && m.Shift_ID == shift.Shift_ID).Select(m => m.Planned_Qty).FirstOrDefault();
                    var startedOrderCount = 0;
                    startedOrderCount = db.RS_OM_Order_List.Where(m => m.Shop_ID == orderListShopId && m.Line_ID == orderListLineId && m.Entry_Date >= plannedDate.Date && m.Entry_Time >= shift.Shift_Start_Time && m.Entry_Time <= shift.Shift_End_Time).Select(m => m).Count();


                    //if (platformName.Equals("U321", StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    if (isTactSheetPrepared(orderListShopId, platformName, orderListLineId, shift.Shift_ID, plannedDate))
                    //    {
                    //        orderDetail = (from u321 in db.RS_OM_U321_Tactsheet_Orders.Where(m => m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day)
                    //                       join or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantId && or.Shop_ID == shopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == shift.Shift_ID &&
                    //                       or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day)
                    //                      on u321.Order_No equals or.Order_No
                    //                       orderby u321.RSN ascending
                    //                       select or).ToList();
                    //    }
                    //    else
                    //    {
                    //        orderDetail = (from u321 in db.RS_OM_U321_Tactsheet_Orders
                    //                       join or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantId && or.Shop_ID == shopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Order_Start == false)
                    //                      on u321.Order_No equals or.Order_No
                    //                       orderby u321.RSN ascending
                    //                       select or).ToList();
                    //    }
                    //}
                    //else if (platformName.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
                    //{

                    ////if (isTactSheetPrepared(orderListShopId, platformName, orderListLineId, shift.Shift_ID, plannedDate))
                    ////{
                    ////    orderDetail = 
                    ////    //(from u321 in db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day)
                    ////    //               join
                    ////                   (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantId && or.Shop_ID == shopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == shift.Shift_ID &&
                    ////                   or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day)
                    ////                 // on u321.Order_No equals or.Order_No
                    ////                   //orderby u321.RSN ascending
                    ////                   select or).ToList();
                    ////}
                    ////else
                    {
                        orderDetail =
                                       //(from u321 in db.RS_OM_S201_Tactsheet_Orders
                                       //               join or 
                                       (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantId && or.Shop_ID == shopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Date != null && or.Planned_Shift_ID > 0)
                                            // on u321.Order_No equals or.Order_No
                                        orderby or.RSN ascending
                                        select or).ToList();
                    }
                    // }
                    //else if (platformName.Equals("XYLO", StringComparison.CurrentCultureIgnoreCase))
                    //{

                    //    if (isTactSheetPrepared(orderListShopId, platformName, orderListLineId, shift.Shift_ID, plannedDate))
                    //    {
                    //        orderDetail = (from u321 in db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Where(m => m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day)
                    //                       join or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantId && or.Shop_ID == shopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == shift.Shift_ID &&
                    //                       or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day)
                    //                      on u321.Order_No equals or.Order_No
                    //                       orderby u321.RSN ascending
                    //                       select or).ToList();
                    //    }
                    //    else
                    //    {
                    //        orderDetail = (from u321 in db.RS_OM_XYLO_Tactsheet_Orders_Sequence
                    //                       join or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantId && or.Shop_ID == shopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Order_Start == false)
                    //                      on u321.Order_No equals or.Order_No
                    //                       orderby u321.RSN ascending
                    //                       select or).ToList();
                    //    }
                    //}



                    //previousOrderObj = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date < plannedDate.Date
                    //              && or.Order_Type != "Spare" && or.Planned_Shift_ID != null).Select(or => or).ToList().OrderBy(m => m.RSN);

                    //backup

                    //TodayPlannedOrders = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day
                    //           && or.Order_Type != "Spare" && or.Planned_Shift_ID != null && or.Planned_Shift_ID <= shift.Shift_ID).Select(or => or).ToList().OrderBy(m => m.RSN);
                    //orderDetail = previousOrderObj.Union(TodayPlannedOrders);
                    //FOR SHOWING MAIN ORDER COUNT AND TOTAL COUNT

                    //TodayAllPlannedOrders = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day
                    //              && or.Order_Type != "Spare" && or.Planned_Shift_ID != null && or.Planned_Shift_ID <= shift.Shift_ID).Select(or => or).ToList().OrderBy(m => m.RSN);
                    //ViewBag.allordercount = (previousOrderObj.Union(TodayAllPlannedOrders)).Count();

                    //TotalOrder = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day
                    //              && ((or.Planned_Shift_ID != null && or.Planned_Shift_ID <= shift.Shift_ID) || or.Order_Type.ToLower() == "spare")).Select(or => or).ToList();
                    ViewBag.totalordercount = 0;
                    //ViewBag.totalordercount = (previousOrderObj.Union(TotalOrder)).Count();

                }
                else
                {
                    String[] omConfiguration = (from configur in db.RS_OM_Configuration
                                                where configur.Partgroup_ID == partgroup.Partgroup_ID
                                                select configur.OMconfig_ID).ToArray();

                    if (omConfiguration.Count() > 0)
                    {
                        String[] models = (from mm in db.RS_Model_Master
                                           where omConfiguration.Contains(mm.OMconfig_ID)
                                           select mm.Model_Code).ToArray();


                        orderDetail = (from or in db.RS_OM_OrderRelease.Where(or => models.Contains(or.partno) && or.Order_Status == "Started" && or.Plant_ID == plantId)
                                       join ol in db.RS_OM_Order_List.Where(a => a.Line_ID == lineId && a.Plant_ID == plantId)
                                       on or.Order_No equals ol.Order_No into orol
                                       from ol in orol.DefaultIfEmpty()
                                       where ol.Order_No == null
                                        && or.Plant_ID == plantId && or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day
                                       orderby or.Updated_Date ascending
                                       select or).ToList().OrderByDescending(m => m.Order_No);

                        foreach (var item in orderDetail)
                        {
                            String modelCode = item.partno;

                            String orderNo = item.Order_No;
                            // process to get the old series
                            RS_Model_Master modelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == modelCode).FirstOrDefault();
                            //if (modelMasterObj != null)
                            //{
                            //    String oldSeriesCode = modelMasterObj.Old_Series_Code;
                            //    if (!String.IsNullOrEmpty(oldSeriesCode))
                            //    {
                            //        // proces to get old series
                            //        RS_Series seriesObj = db.RS_Series.Where(p => p.Series_Code == oldSeriesCode).FirstOrDefault();
                            //        if (seriesObj != null)
                            //        {
                            //            //orderOldSeriesData.Add(new KeyValuePair<string, int>(orderNo.ToString(), seriesObj.Series_Description.ToString()));
                            //            //orderOldSeriesData.Add(new KeyValuePair<string, String>(orderNo, seriesObj.Series_Description));
                            //            item.oldSeriesCode = seriesObj.Series_Description;
                            //        }
                            //    }
                            //}
                        }
                    }
                }
            }
            if (orderDetail != null)
            {
                return PartialView(orderDetail.Take(20));

            }
            return PartialView(orderDetail);


        }
        public Boolean isTactSheetPrepared(int orderListShopId, string platformName, int orderListLineId, decimal Shift_ID, DateTime plannedDate)
        {
            var is_tactsheetPrepared = false;
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            decimal platformId = 0;
            platformId = db.RS_OM_Platform.Where(m => m.Line_ID == orderListLineId && m.Shop_ID == orderListShopId && m.Plant_ID == plantId).Select(m => m.Platform_ID).FirstOrDefault();

            if (platformName.Equals("U321", StringComparison.CurrentCultureIgnoreCase))
            {
                is_tactsheetPrepared = db.RS_OM_U321_Tactsheet_Orders.Any(m => m.Plant_ID == plantId && m.Shop_ID == orderListShopId && m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day && m.Shift_ID == Shift_ID && m.Platform_ID == platformId);
            }
            else if (platformName.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
            {
                is_tactsheetPrepared = db.RS_OM_S201_Tactsheet_Orders.Any(m => m.Plant_ID == plantId && m.Shop_ID == orderListShopId && m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day && m.Shift_ID == Shift_ID && m.Platform_ID == platformId);


            }
            else if (platformName.Equals("XYLO", StringComparison.CurrentCultureIgnoreCase))
            {

                is_tactsheetPrepared = db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(m => m.Plant_ID == plantId && m.Shop_ID == orderListShopId && m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day && m.Shift_ID == Shift_ID && m.Platform_ID == platformId);

            }


            return is_tactsheetPrepared;
        }
        //public ActionResult ShowOrderListByShopAndLine(int orderListShopId, int orderListLineId)
        //{

        //    int plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
        //    int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
        //    int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
        //    var plannedDate = getPlannedDate();
        //    IEnumerable<RS_OM_OrderRelease> previousOrderObj = null;
        //    IEnumerable<RS_OM_OrderRelease> TodayPlannedOrders = null;
        //    IEnumerable<RS_OM_OrderRelease> TodayAllPlannedOrders = null;
        //    IEnumerable<RS_OM_OrderRelease> TotalOrder = null;
        //    IEnumerable<RS_OM_OrderRelease> orderDetail = null;
        //    RS_Partgroup partgroup;
        //    RS_Model_Master model_mast;

        //    partgroup = (from partgroupData in db.RS_Partgroup
        //                 where partgroupData.Line_ID == lineId && partgroupData.Plant_ID == plantId
        //                 select partgroupData
        //              ).FirstOrDefault();
        //    ViewBag.PartGroup = partgroup;
        //    if (partgroup != null)
        //    {
        //        if (partgroup.Order_Create == false)
        //        {
        //            if (partgroup.Is_Kitting == true)
        //            {
        //                TempData["Is_Kitting"] = 1;
        //            }
        //            else
        //            {
        //                TempData["Is_Kitting"] = 0;
        //            }
        //        }
        //        else
        //        {
        //            TempData["Is_Kitting"] = 0;
        //        }
        //    }

        //    if (partgroup != null)
        //    {
        //        if (partgroup.Order_Create == true)
        //        {
        //            //    orderDetail = (from orderReleaseItem in db.RS_OM_OrderRelease
        //            //                   orderby orderReleaseItem.RSN ascending
        //            //                   where orderReleaseItem.Shop_ID == orderListShopId && orderReleaseItem.Line_ID == orderListLineId && orderReleaseItem.Order_Status == "Release" && orderReleaseItem.Plant_ID == plantId
        //            //                  // && (orderReleaseItem.Planned_Date.Value.Date >= DateTime.Now.Date && orderReleaseItem.Planned_Date.Value.Date <= DateTime.Now)
        //            //                  && DbFunctions.TruncateTime(orderReleaseItem.Planned_Date.Value)== DateTime.Now.Date
        //            //                   select orderReleaseItem).Take(20).ToList().OrderByDescending(m => m.Order_No);
        //            //orderDetail = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == DateTime.Now.Year && or.Planned_Date.Value.Month == DateTime.Now.Month && or.Planned_Date.Value.Day == DateTime.Now.Day).Select(or => or).Take(20).ToList().OrderByDescending(m => m.Order_No);
        //            //Shiftwise production
        //            //finding current shift
        //            DateTime date = DateTime.Now;
        //            TimeSpan currentTime = date.TimeOfDay;

        //            var shift = db.RS_Shift.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Shift_Start_Time <= currentTime && currentTime <= m.Shift_End_Time).Select(m => m).FirstOrDefault();
        //            var plannedQty = 0;
        //            plannedQty = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == shopId && m.Line_ID == lineId && m.Plan_Date == plannedDate.Date && m.Shift_ID == shift.Shift_ID).Select(m => m.Planned_Qty).FirstOrDefault();
        //            var startedOrderCount = 0;
        //            startedOrderCount = db.RS_OM_Order_List.Where(m => m.Shop_ID == orderListShopId && m.Line_ID == orderListLineId && m.Entry_Date >= plannedDate.Date && m.Entry_Time >= shift.Shift_Start_Time && m.Entry_Time <= shift.Shift_End_Time).Select(m => m).Count();


        //            previousOrderObj = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date < plannedDate.Date
        //                          && or.Order_Type != "Spare" && or.Planned_Shift_ID != null).Select(or => or).ToList().OrderBy(m => m.RSN);

        //            //backup

        //            TodayPlannedOrders = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day
        //                       && or.Order_Type != "Spare" && or.Planned_Shift_ID != null && or.Planned_Shift_ID <= shift.Shift_ID).Select(or => or).ToList().OrderBy(m => m.RSN);
        //           orderDetail = previousOrderObj.Union(TodayPlannedOrders);
        //            //FOR SHOWING MAIN ORDER COUNT AND TOTAL COUNT

        //            TodayAllPlannedOrders = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day
        //                          && or.Order_Type != "Spare" && or.Planned_Shift_ID != null && or.Planned_Shift_ID <= shift.Shift_ID).Select(or => or).ToList().OrderBy(m => m.RSN);
        //            ViewBag.allordercount = (previousOrderObj.Union(TodayAllPlannedOrders)).Count();

        //            TotalOrder = db.RS_OM_OrderRelease.Where(or => or.Shop_ID == orderListShopId && or.Line_ID == orderListLineId && or.Order_Status == "Release" && or.Plant_ID == plantId && or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day
        //                          && ((or.Planned_Shift_ID != null && or.Planned_Shift_ID <= shift.Shift_ID) || or.Order_Type.ToLower() == "spare")).Select(or => or).ToList();
        //            ViewBag.totalordercount = 0;
        //            ViewBag.totalordercount = (previousOrderObj.Union(TotalOrder)).Count();

        //        }
        //        else
        //        {
        //            String[] omConfiguration = (from configur in db.RS_OM_Configuration
        //                                        where configur.Partgroup_ID == partgroup.Partgroup_ID
        //                                        select configur.OMconfig_ID).ToArray();

        //            if (omConfiguration.Count() > 0)
        //            {
        //                String[] models = (from mm in db.RS_Model_Master
        //                                   where omConfiguration.Contains(mm.OMconfig_ID)
        //                                   select mm.Model_Code).ToArray();


        //                orderDetail = (from or in db.RS_OM_OrderRelease.Where(or => models.Contains(or.partno) && or.Order_Status == "Started" && or.Plant_ID == plantId)
        //                               join ol in db.RS_OM_Order_List.Where(a => a.Line_ID == lineId && a.Plant_ID == plantId)
        //                               on or.Order_No equals ol.Order_No into orol
        //                               from ol in orol.DefaultIfEmpty()
        //                               where ol.Order_No == null
        //                                && or.Plant_ID == plantId && or.Planned_Date.Value.Year == plannedDate.Year && or.Planned_Date.Value.Month == plannedDate.Month && or.Planned_Date.Value.Day == plannedDate.Day
        //                               orderby or.Updated_Date ascending
        //                               select or).ToList().OrderByDescending(m => m.Order_No);

        //                foreach (var item in orderDetail)
        //                {
        //                    String modelCode = item.partno;

        //                    String orderNo = item.Order_No;
        //                    // process to get the old series
        //                    RS_Model_Master modelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == modelCode).FirstOrDefault();
        //                    //if (modelMasterObj != null)
        //                    //{
        //                    //    String oldSeriesCode = modelMasterObj.Old_Series_Code;
        //                    //    if (!String.IsNullOrEmpty(oldSeriesCode))
        //                    //    {
        //                    //        // proces to get old series
        //                    //        RS_Series seriesObj = db.RS_Series.Where(p => p.Series_Code == oldSeriesCode).FirstOrDefault();
        //                    //        if (seriesObj != null)
        //                    //        {
        //                    //            //orderOldSeriesData.Add(new KeyValuePair<string, int>(orderNo.ToString(), seriesObj.Series_Description.ToString()));
        //                    //            //orderOldSeriesData.Add(new KeyValuePair<string, String>(orderNo, seriesObj.Series_Description));
        //                    //            item.oldSeriesCode = seriesObj.Series_Description;
        //                    //        }
        //                    //    }
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //    if (orderDetail != null)
        //    {
        //        return PartialView(orderDetail.Take(20));

        //    }
        //    return PartialView(orderDetail);


        //}

        public ActionResult PPCKittingOrderList(int? orderListShopId, int? orderListLineId)
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

            IEnumerable<RS_OM_OrderRelease> kittingorderDetail = null;
            RS_Partgroup partgroup;

            partgroup = (from partgroupData in db.RS_Partgroup
                         where partgroupData.Line_ID == lineId && partgroupData.Plant_ID == plantId
                         select partgroupData
                      ).FirstOrDefault();
            ViewBag.PartGroup = partgroup;
            if (partgroup.Order_Create == false)
            {
                if (partgroup.Is_Kitting == true)
                {
                    TempData["Is_Kitting"] = 1;
                }
                else
                {
                    TempData["Is_Kitting"] = 0;
                }
            }
            else
            {
                TempData["Is_Kitting"] = 0;
            }
            //added by ketan to display orders as ppc plan display on daily bassis.
            //Date 14-jul-17

            if (partgroup != null)
            {
                if (partgroup.Is_PPC_Kitting_Order == true)
                {
                    //kittingorderDetail = (from ppcplannedOrder in db.RS_OM_Planned_Orders
                    //                      join kittingOrder in db.RS_OM_Kitting_Order_list
                    //                      on ppcplannedOrder.Order_No equals kittingOrder.Order_No
                    //                      into order
                    //                      from kittingOrder in order.DefaultIfEmpty()
                    //                      orderby ppcplannedOrder.Planned_Date ascending
                    //                      where ppcplannedOrder.Shop_ID == shopId &&
                    //                      ppcplannedOrder.Order_Status == "Release" && ppcplannedOrder.Plant_ID == plantId &&
                    //                      !ppcplannedOrder.Order_No.Contains(kittingOrderNo) && kittingOrder.Station_ID == stationId
                    //                      select ppcplannedOrder).Take(20).ToList();

                    kittingorderDetail = (from orderRelease in db.RS_OM_OrderRelease
                                          join ppcplannedOrder in db.RS_OM_Planned_Orders
                                          on orderRelease.Order_No equals ppcplannedOrder.Order_No
                                          join kittingOrder in db.RS_OM_Kitting_Order_list
                                          on ppcplannedOrder.Order_No equals kittingOrder.Order_No
                                          into order
                                          from kittingOrder in order.DefaultIfEmpty()
                                          orderby ppcplannedOrder.Planned_Date ascending
                                          where ppcplannedOrder.Shop_ID == shopId &&
                                          ppcplannedOrder.Order_Status == "Release" && ppcplannedOrder.Plant_ID == plantId &&
                                          ppcplannedOrder.Planned_Date == System.DateTime.Today &&
                                          !(from planeOrder in db.RS_OM_Kitting_Order_list where planeOrder.Station_ID == stationId select planeOrder.Order_No).
                                          Contains(ppcplannedOrder.Order_No)
                                          select orderRelease).Take(20).ToList();
                }
                //var noSelectedDefect = from defect in db.MM_Quality_Checklist
                //                       where defect.Shop_ID == shopId && !(from defectStation in db.MM_Quality_Station_Checklist where defectStation.Station_ID == stationId select defectStation.Checklist_ID).Contains(defect.Checklist_ID) &&
                //                       defect.Plant_ID == plantId && defect.Attribute_ID == attributeId
                //                       select new
                //                       {
                //                           Checklist_ID = defect.Checklist_ID,
                //                           Checklist_Name = defect.Checklist_Name
                //                       };
            }

            return PartialView("PPCKittingOrderList", kittingorderDetail);
        }
        /*               Action Name               : GetTopOrderDetails
         *               Description               : Action used to show the Order Release top orders details.
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : shopId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult GetTopOrderDetails(int shopId, int lineId)
        {
            try
            {
                RS_Partgroup partgroup;
                RS_Model_Master model_mast;

                partgroup = (from partgroupData in db.RS_Partgroup
                             where partgroupData.Line_ID == lineId
                             select partgroupData
                          ).FirstOrDefault();

                if (partgroup != null)
                {
                    if (partgroup.Order_Create == true)
                    {
                        var st = (from orderReleaseItem in db.RS_OM_OrderRelease
                                  where orderReleaseItem.Shop_ID == shopId && orderReleaseItem.Line_ID == lineId && orderReleaseItem.Order_Status == "Release"
                                  orderby orderReleaseItem.RSN ascending
                                  select new
                                  {
                                      Plant_OrderNo = orderReleaseItem.Plant_OrderNo,
                                      Model_Code = orderReleaseItem.Model_Code,
                                      Remarks = orderReleaseItem.Remarks,
                                      Part_no = orderReleaseItem.partno,
                                      Series = orderReleaseItem.Series_Code,
                                      Row_ID = orderReleaseItem.Row_ID,
                                      Order_No = orderReleaseItem.Order_No,
                                      Order_Type = orderReleaseItem.Order_Type,
                                      RSN = orderReleaseItem.RSN
                                  }).Take(1).ToList();
                        return Json(st, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        String[] omConfiguration = (from configur in db.RS_OM_Configuration
                                                    where configur.Partgroup_ID == partgroup.Partgroup_ID
                                                    select configur.OMconfig_ID).ToArray();

                        if (omConfiguration.Count() > 0)
                        {
                            String[] models = (from mm in db.RS_Model_Master
                                               where omConfiguration.Contains(mm.OMconfig_ID)
                                               select mm.Model_Code).ToArray();


                            var st = (from or in db.RS_OM_OrderRelease.Where(or => models.Contains(or.partno) && or.Order_Status != "Hold" && or.Order_Status == "Started")
                                      join ol in db.RS_OM_Order_List.Where(a => a.Line_ID == lineId)
                                      on or.Order_No equals ol.Order_No into orol
                                      from ol in orol.DefaultIfEmpty()
                                      where ol.Order_No == null
                                      orderby or.Updated_Date ascending
                                      select new
                                      {
                                          Plant_OrderNo = or.Plant_OrderNo,
                                          Model_Code = or.Model_Code,
                                          Remarks = or.Remarks,
                                          Part_no = or.partno,
                                          Series = or.Series_Code,
                                          Row_ID = or.Row_ID,
                                          Order_No = or.Order_No,
                                          Order_Type = or.Order_Type,
                                          RSN = or.RSN,
                                          or.Updated_Date
                                      }).Take(1).ToList();

                            return Json(st, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "OrderStartController", "GetTopOrderDetails", ((FDSession)this.Session["FDSession"]).userId);
                return Json(null, JsonRequestBehavior.AllowGet);
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

                            //RS_Quality_OK_Order mmQualityOKOrderObj = (from mmQualityOkOrder in db.RS_Quality_OK_Order
                            //                                           where mmQualityOkOrder.Serial_No == serialNumber && mmQualityOkOrder.Is_Pulled != true && mmQualityOkOrder.Part_No == part_number
                            //                                           select mmQualityOkOrder).Single();

                            String partNumber = partNo[i];

                            RS_OM_Order_List orderListObj = (from orderList in db.RS_OM_Order_List
                                                             where orderList.Serial_No == serialNumber && orderList.partno == partNumber
                                                             select orderList).FirstOrDefault();

                            if (orderListObj == null)
                            {
                                errorProofingRes[i].isOK = false;
                                errorProofingRes[i].errorMessage = "Invalid serial number";
                                continue;
                            }

                            RS_Geneaology mmGenealogyObj = (from genealogy in db.RS_Geneaology
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


        /*               Action Name               : ShowStartOrderList
         *               Description               : Action used to show the started Order list. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : startOrderListShopId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult ShowStartOrderList(int startOrderListShopId, int startOrderListLineId)
        {
            //var orderDetail = (from orderList in db.RS_OM_Order_List
            //                   where orderList.Shop_ID == startOrderListShopId && orderList.Line_ID == startOrderListLineId && (from orderRelease in db.RS_OM_OrderRelease where orderRelease.Order_Status == "Started" && orderRelease.Shop_ID == startOrderListShopId select orderRelease.Order_No).Contains(orderList.Order_No)
            //                   select orderList).OrderByDescending(p => p.Inserted_Date).Take(3).ToList();
            List<RS_OM_Order_List> orderDetail = new List<RS_OM_Order_List>();
            try
            {
                orderDetail = (from orderList in db.RS_OM_Order_List
                               where orderList.Shop_ID == startOrderListShopId && orderList.Line_ID == startOrderListLineId && orderList.Order_Status == "Started"
                               orderby orderList.PSN descending
                               select orderList).Take(3).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }


            foreach (var item in orderDetail)
            {
                String modelCode = item.partno;

                String orderNo = item.Order_No;
                // process to get the old series
                RS_Model_Master modelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == modelCode).FirstOrDefault();
                //if (modelMasterObj != null)
                //{
                //    String oldSeriesCode = modelMasterObj.Old_Series_Code;
                //    if (!String.IsNullOrEmpty(oldSeriesCode))
                //    {
                //        // proces to get old series
                //        RS_Series seriesObj = db.RS_Series.Where(p => p.Series_Code == oldSeriesCode).FirstOrDefault();
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
        class JSONBuildsheetData
        {
            public bool status { get; set; }
            public string orderNo { get; set; }

        }
        class JSONData
        {
            public bool status { get; set; }
            public string type { get; set; }
            public string message { get; set; }
            public string kit_message { get; set; }

            public int Total { get; set; }
            public int Completed { get; set; }

            public int Balance { get; set; }

            public string CurrentModel { get; set; }

            public string CurrentEngine { get; set; }
            public string ModelCode { get; set; }
            public string OrderNo { get; set; }
        }

        /*               Action Name               : StartOrder
         *               Description               : Action used to start the order. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : rowId,plantId,shopId,lineId,stationId
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult StartOrder_Old(int rowId, string Tyre_Size = "", string Model_Code = "", int tyreMakeId = 0, string kitting_Barcode = "", string Is_PPC_Kitting_Order = "")
        {
            JSONData jsonData = new JSONData();
            string codePosition = "";
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                //for current shift
                DateTime date = DateTime.Now;
                TimeSpan currentTime = date.TimeOfDay;

                var shift = db.RS_Shift.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Shift_Start_Time <= currentTime && currentTime <= m.Shift_End_Time).Select(m => m).FirstOrDefault();
                ////
                string orderno = "";
                string modelcode = "";
                int rsn = 0;
                if (kitting_Barcode != "")
                {
                    if (db.RS_Kitt_Barcode_Master.Where(kit => kit.Barcode_String == kitting_Barcode && kit.Is_Consumed == false).Count() == 0)
                    {
                        jsonData.kit_message = "This kitt: " + kitting_Barcode + " is not available.";
                        jsonData.status = false;
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    }
                }

                //Added By ketan to save PPC kitting order plan
                //Date 18-jul-17
                if (Is_PPC_Kitting_Order != null && Is_PPC_Kitting_Order != "")
                {
                    RS_OM_OrderRelease objRS_OM_OrderRelease = db.RS_OM_OrderRelease.Find(rowId);
                    RS_OM_Kitting_Order_list objRS_OM_Kitting_Order_list = new RS_OM_Kitting_Order_list();
                    objRS_OM_Kitting_Order_list.Order_ID = objRS_OM_OrderRelease.Row_ID;
                    objRS_OM_Kitting_Order_list.Line_ID = lineId;
                    objRS_OM_Kitting_Order_list.Model_Code = objRS_OM_OrderRelease.Model_Code;
                    objRS_OM_Kitting_Order_list.Order_No = objRS_OM_OrderRelease.Order_No;
                    objRS_OM_Kitting_Order_list.Order_Status = "Started";
                    objRS_OM_Kitting_Order_list.Parent_Model_Code = objRS_OM_OrderRelease.partno;
                    objRS_OM_Kitting_Order_list.Planned_Date = System.DateTime.Now;
                    objRS_OM_Kitting_Order_list.Plant_ID = plantId;
                    objRS_OM_Kitting_Order_list.Shop_ID = shopId;
                    objRS_OM_Kitting_Order_list.Station_ID = stationId;
                    objRS_OM_Kitting_Order_list.Updated_Date = DateTime.Now;
                    objRS_OM_Kitting_Order_list.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.RS_OM_Kitting_Order_list.Add(objRS_OM_Kitting_Order_list);
                    db.SaveChanges();
                }

                RS_OM_OrderRelease ordReleaseObj = db.RS_OM_OrderRelease.Find(rowId);
                orderno = ordReleaseObj.Order_No;
                modelcode = ordReleaseObj.Model_Code;
                rsn = Convert.ToInt32(ordReleaseObj.RSN);
                //try
                //{
                //    if (ordReleaseObj != null)
                //    {
                //        orderno = ordReleaseObj.Order_No;
                //        modelcode = ordReleaseObj.Model_Code;
                //        rsn = Convert.ToInt32(ordReleaseObj.RSN);
                //        var isHolded = db.RS_OM_ShopFloor_Holded_RSN.Where(a => a.Order_ID == rowId).FirstOrDefault();
                //        if (isHolded != null)
                //        {
                //            rsn = Convert.ToInt32(db.RS_OM_Planned_Orders.Where(a => a.Order_ID == isHolded.Latch_Order_ID && a.Planned_Date == DateTime.Today).FirstOrDefault().RSN.GetValueOrDefault(0));
                //            ordReleaseObj.RSN = rsn;
                //            db.Entry(ordReleaseObj).State = EntityState.Modified;
                //            db.SaveChanges();
                //            isHolded.Is_Started = true;
                //            isHolded.Updated_Date = DateTime.Now;
                //            db.Entry(isHolded).State = EntityState.Modified;
                //            db.SaveChanges();
                //        }
                //        generalHelper.updatePlannedRSN(rowId, ((FDSession)this.Session["FDSession"]).userId);
                //    }
                //}
                //catch (Exception exp)
                //{
                //    while (exp.InnerException != null)
                //    {
                //        exp = exp.InnerException;
                //    }
                //    generalHelper.addControllerException(exp, "OrderStartController", "new Logic For PPC Hold ORder(Order ID: " + rowId + ")", ((FDSession)this.Session["FDSession"]).userId);
                //}

                string shop_name = "";

                //find Shop Name
                RS_Shops shop = db.RS_Shops.Find(shopId);

                if (shop != null)
                {
                    shop_name = shop.Shop_Name;
                }

                String serialNumber = "";
                RS_OM_Order_List orderList = new RS_OM_Order_List();
                orderList.Plant_ID = plantId;
                orderList.Shop_ID = shopId;
                orderList.Line_ID = lineId;
                orderList.Station_ID = stationId;

                if (orderList.Hold_Order_Check(rowId, shopId, lineId, orderno, modelcode, rsn) == true)
                {
                    jsonData.status = false;
                    jsonData.message = "This Order: " + orderno + " is Holded right now .";
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else if ((db.RS_OM_OrderRelease.Any(a => a.Row_ID == rowId && a.Is_Blocked == true)) == true)
                {
                    jsonData.status = false;
                    jsonData.message = "This Order: " + orderno + " is Blocked for sequencing right now .";
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    RS_Partgroup partgroup;
                    RS_OM_OrderRelease mmOmOrderReleaseObj;

                    partgroup = (from partgroupData in db.RS_Partgroup
                                 where partgroupData.Line_ID == lineId
                                 select partgroupData
                              ).FirstOrDefault();

                    if (partgroup != null)
                    {
                        if (partgroup.Order_Create == true)
                        {
                            int newRSN = 0;
                            //CODE BLOCK FOR Updating the RSN in PlannedOrder Table AND RESOLVING ORDER Missing in sub assembly issue
                            try
                            {
                                // ---------------- CODE FOR ELIMINATING DUPLICATE ORDER ID (SUB ASSEMBLY MISSING ORDERS ISSUE)------------------
                                // process to check the order is already started or not
                                RS_OM_OrderRelease orderReleaseObj = db.RS_OM_OrderRelease.Where(p => p.Row_ID == rowId && p.Shop_ID == shopId).FirstOrDefault();
                                //if (orderReleaseObj.Order_Status == "Release")
                                {
                                    // process to check for the same order number order is already started in order list or not
                                    String orderNo = orderReleaseObj.Order_No;
                                    RS_OM_Order_List orderListObj = db.RS_OM_Order_List.Where(p => p.Order_No == orderNo && p.Shop_ID == shopId && p.Line_ID == lineId).FirstOrDefault();
                                    if (orderListObj != null)
                                    {
                                        if (orderReleaseObj.Order_Status == "Release")
                                        {
                                            RS_OM_Planned_Orders plannedOrdrObj = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == rowId && a.Planned_Date == DateTime.Today).FirstOrDefault();
                                            if (plannedOrdrObj != null)
                                            {
                                                newRSN = Convert.ToInt32(plannedOrdrObj.RSN.GetValueOrDefault(0));
                                            }
                                            // it means order started, but status not updated
                                            orderReleaseObj.Order_Status = "Started";
                                            orderReleaseObj.Updated_Date = DateTime.Now;
                                            orderReleaseObj.RSN = newRSN;
                                            orderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                            codePosition = "Position 1";
                                            db.Entry(orderReleaseObj).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        return Json(true, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                //----------------------------------------------------------------

                                newRSN = generalHelper.updatePlannedRSN(rowId, ((FDSession)this.Session["FDSession"]).userId);
                            }
                            catch (Exception exp)
                            {
                                while (exp.InnerException != null)
                                {
                                    exp = exp.InnerException;
                                }
                                generalHelper.addControllerException(exp, "OrderStartController", "INSIDE NEW CODE BLOCK", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
                            }
                            //----------------------------------------------------------------

                            // add record in order list table
                            mmOmOrderReleaseObj = (from orderRelease in db.RS_OM_OrderRelease
                                                   where orderRelease.Row_ID == rowId
                                                   select orderRelease).Single();

                            string modelCode = mmOmOrderReleaseObj.partno;
                            orderList.Order_No = mmOmOrderReleaseObj.Order_No;
                            orderList.Model_Code = modelCode;
                            orderList.Series_Code = mmOmOrderReleaseObj.Series_Code;

                            Int64 psn;
                            // orderList.Series_Code = partMasterObj.Series_Code;
                            try
                            {
                                psn = Convert.ToInt64(db.RS_OM_Order_List.Where(p => p.Shop_ID == shopId && p.Line_ID == lineId && p.Station_ID == stationId).Max(item => item.PSN).ToString());
                            }
                            catch (Exception)
                            {
                                psn = 0;
                            }

                            //int psn = orderList.getPSNByDate(shopId,lineId);
                            orderList.PSN = psn + 1;
                            int dsn = orderList.getDSNByDateShop(shopId, lineId);

                            orderList.DSN = dsn + 1;
                            orderList.Entry_Date = DateTime.Now.Date;
                            orderList.Entry_Time = DateTime.Now.TimeOfDay;
                            orderList.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            orderList.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            orderList.Inserted_Date = DateTime.Now;
                            orderList.partno = mmOmOrderReleaseObj.partno;
                            orderList.Started_Shift_ID = shift.Shift_ID;
                            orderList.Order_Status = "Started";
                            orderList.Partgroup_ID = partgroup.Partgroup_ID;

                            //added by ketan Date 02/09/2017
                            bool is_SerialNumberGenration = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Serial_No_Generation).FirstOrDefault();
                            if (is_SerialNumberGenration)
                            //if (mmOmOrderReleaseObj.Order_Type.Trim().ToLower() == Order_Type.Trim().ToLower())
                            {
                                var modelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode && m.Plant_ID == plantId && m.Shop_ID == shopId).FirstOrDefault();
                                RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == modelMaster.Config_ID).FirstOrDefault();
                                objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                TempData["updateRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                //serialNumber = orderList.getSerialNumber(orderList.Shop_ID, orderList.Line_ID, orderList.Order_No,mmOmOrderReleaseObj.Model_Code,mmOmOrderReleaseObj.partno, modelMaster.Config_ID);
                                serialNumber = orderList.getSerialNumberAD(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);
                            }
                            bool trialSerialNo = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Trial_Body_Serial_No).FirstOrDefault();
                            if (trialSerialNo)
                            {
                                RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Serial_Logic == "TR").FirstOrDefault();
                                objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                TempData["updateTrialBodyRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                serialNumber = orderList.getTrialBodySerialNumber(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);
                            }
                            //comment by ketan Date 02/09/2017
                            // serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, mmOmOrderReleaseObj.Order_No, mmOmOrderReleaseObj.Model_Code, mmOmOrderReleaseObj.partno, mmOmOrderReleaseObj.Series_Code,partgroup.Partgrup_Desc,partgroup.Serial_Config_ID);
                            //serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, mmOmOrderReleaseObj.Order_No, mmOmOrderReleaseObj.Model_Code, mmOmOrderReleaseObj.partno, mmOmOrderReleaseObj.Series_Code, Convert.ToInt16(partgroup.Serial_Config_ID));
                            orderList.Serial_No = serialNumber;
                            //added by ketan Date 21-07-2016
                            //use to in SaveOrderPartsDetails method to save part details. 
                            if (serialNumber != null)
                            {
                                TempData["SerialNo"] = serialNumber;
                            }
                            codePosition = "Position 2";
                            //sandip
                            if (kitting_Barcode != "")
                            {
                                orderList.Param_1 = kitting_Barcode;
                            }
                            db.RS_OM_Order_List.Add(orderList);
                            db.SaveChanges();
                            if (kitting_Barcode != "")
                            {
                                RS_Kitt_Barcode_Master objKitt = new RS_Kitt_Barcode_Master();
                                objKitt = (from kitt in db.RS_Kitt_Barcode_Master where kitt.Barcode_String == kitting_Barcode select kitt).FirstOrDefault();
                                objKitt.Serial_No = serialNumber;
                                objKitt.Is_Consumed = true;
                                db.Entry(objKitt).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            //Start 22 may 2017
                            var is_Print = (from pgroup in db.RS_Partgroup where pgroup.Partgroup_ID == partgroup.Partgroup_ID select pgroup.Is_Print).FirstOrDefault();
                            if (is_Print == true)
                            {

                                // process to add the record in PRN database
                                RS_PRN mmPRNObj = new RS_PRN();
                                mmPRNObj.Plant_ID = plantId;
                                mmPRNObj.Shop_ID = shopId;
                                mmPRNObj.Line_ID = lineId;
                                mmPRNObj.Station_ID = stationId;
                                // process to get Quality Ok PRN file
                                string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/order_start.prn"));
                                prnFile = prnFile.Replace("012345678910", serialNumber.Trim().ToUpper());
                                prnFile = prnFile.Replace("serial_no", serialNumber.Trim().ToUpper());
                                prnFile = prnFile.Replace("shop_name", shop_name);
                                prnFile = prnFile.Replace("part_no", orderList.partno);
                                prnFile = prnFile.Replace("Production Start", "Production");
                                //prnFile = prnFile.Replace()

                                try
                                {
                                    RS_Model_Master mmModelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == orderList.partno).Single();
                                    RS_Series series = db.RS_Series.Where(p => p.Series_Code == mmModelMasterObj.Series_Code).Single();
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
                                codePosition = "Position 3";
                                mmPRNObj.PRN_Text = prnFile;
                                //code for enter bit in RS_PRN file is_orderstartr
                                mmPRNObj.Is_OrderStart = true;


                                mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                mmPRNObj.Inserted_Date = DateTime.Now;
                                mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                db.RS_PRN.Add(mmPRNObj);
                                db.SaveChanges();
                                //End
                            }
                            if (partgroup.Order_Create == true)
                            {
                                General genObj = new General();
                                int runningSerialCounter = genObj.getCurrentRunningSerial(mmOmOrderReleaseObj.partno, ((FDSession)this.Session["FDSession"]).userId);
                                RS_Serial_Number_Data serialNumberDataObj = db.RS_Serial_Number_Data.Where(a => a.Part_No.Trim() == modelCode.Trim()).FirstOrDefault();
                                if (serialNumberDataObj != null)
                                {
                                    codePosition = "Position 4";
                                    UpdateModel(serialNumberDataObj);
                                    serialNumberDataObj.Running_Serial = runningSerialCounter;
                                    serialNumberDataObj.Is_Edited = true;
                                    db.Entry(serialNumberDataObj).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }


                            //---------------------Release-------------------------------------//
                            try
                            {
                                mmOmOrderReleaseObj = db.RS_OM_OrderRelease.Find(rowId);
                                mmOmOrderReleaseObj.Order_Status = "Started";
                                mmOmOrderReleaseObj.RSN = newRSN;
                                mmOmOrderReleaseObj.Order_Start = true;
                                mmOmOrderReleaseObj.Updated_Date = DateTime.Now;
                                mmOmOrderReleaseObj.Is_Edited = true;
                                mmOmOrderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                codePosition = "Position 5";
                                db.Entry(mmOmOrderReleaseObj).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception exp)
                            {
                                while (exp.InnerException != null)
                                {
                                    exp = exp.InnerException;
                                }
                                generalHelper.addControllerException(exp, "OrderStartController", "Updating Order Release Table(Line ID:" + mmOmOrderReleaseObj.Line_ID + ", Order RowID:" + mmOmOrderReleaseObj.Row_ID + ", Order No:" + mmOmOrderReleaseObj.Order_No + ")", ((FDSession)this.Session["FDSession"]).userId);
                            }
                            //------------------------------------------------------------------
                            RS_OM_Planned_Orders plannedOrdersObj = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == rowId && a.Planned_Date == DateTime.Today).FirstOrDefault();
                            if (plannedOrdersObj != null)
                            {
                                plannedOrdersObj.Order_Status = "Started";
                                plannedOrdersObj.Last_Status_Change_Time = DateTime.Now;
                                db.Entry(plannedOrdersObj).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            //Insert Data in Order_Status Table
                            RS_OM_Order_Status mmOrderStatus = new RS_OM_Order_Status();

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
                            codePosition = "Position 6";
                            db.RS_OM_Order_Status.Add(mmOrderStatus);
                            db.SaveChanges();

                            orderList = new RS_OM_Order_List();
                            orderList = db.RS_OM_Order_List.Where(p => p.Serial_No == serialNumber).Single();

                            //check Condition
                            int next_lineId;
                            RS_Lines line;
                            line = db.RS_Lines.Where(p => p.Line_ID == lineId).Single();
                            if (line.Is_Conveyor == false)
                            {
                                var route_marriage_station = (from route_marriage in db.RS_Route_Marriage_Station
                                                              where route_marriage.Sub_Line_ID == lineId
                                                              select route_marriage).ToList();

                                foreach (var item in route_marriage_station)
                                {
                                    next_lineId = Convert.ToInt32(item.Marriage_Line_ID);

                                    RS_Stations station;
                                    station = db.RS_Stations.Where(p => p.Station_ID == item.Marriage_Station_ID).Single();
                                    if (station.Is_Buffer == true)
                                    {
                                        RS_Line_Complete_Buffer line_complete_buffer = new RS_Line_Complete_Buffer();
                                        line_complete_buffer.Plant_ID = plantId;
                                        line_complete_buffer.Shop_ID = shopId;
                                        line_complete_buffer.Line_ID = next_lineId;
                                        line_complete_buffer.Station_ID = item.Marriage_Station_ID;
                                        line_complete_buffer.SerialNo = serialNumber;
                                        line_complete_buffer.Inserted_Time = DateTime.Now;
                                        codePosition = "Position 7";
                                        db.RS_Line_Complete_Buffer.Add(line_complete_buffer);
                                        db.SaveChanges();
                                    }
                                }

                                //Staion_tracking
                                // process to add the order in station tracking
                                RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                                mmStationTrackingObj.Plant_ID = plantId;
                                mmStationTrackingObj.Shop_ID = shopId;
                                mmStationTrackingObj.Line_ID = lineId;
                                mmStationTrackingObj.Station_ID = stationId;
                                mmStationTrackingObj.SerialNo = serialNumber;
                                mmStationTrackingObj.Track_SerialNo = serialNumber;
                                mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                mmStationTrackingObj.Is_Edited = true;
                                //db.RS_Station_Tracking.Add(mmStationTrackingObj);
                                codePosition = "Position 8";
                                db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            else
                            {
                                // process to add the order in station tracking
                                RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                                mmStationTrackingObj.Plant_ID = plantId;
                                mmStationTrackingObj.Shop_ID = shopId;
                                mmStationTrackingObj.Line_ID = lineId;
                                mmStationTrackingObj.Station_ID = stationId;
                                mmStationTrackingObj.Is_Edited = true;
                                mmStationTrackingObj.SerialNo = serialNumber;
                                mmStationTrackingObj.Track_SerialNo = serialNumber;
                                mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                //db.RS_Station_Tracking.Add(mmStationTrackingObj);
                                codePosition = "Position 9";
                                db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            // process to add the order in quality order queue

                            RS_Quality_Captures mmQualityCapturesObj = new RS_Quality_Captures();
                            mmQualityCapturesObj.insertOrderInQualityOrderQueue(serialNumber, orderno, plantId, shopId, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);


                        }
                        else
                        {
                            mmOmOrderReleaseObj = db.RS_OM_OrderRelease.Find(rowId);
                            String modelCode = mmOmOrderReleaseObj.partno;

                            orderList.Order_No = mmOmOrderReleaseObj.Order_No;
                            orderList.Model_Code = modelCode;

                            decimal? series_code;
                            series_code = mmOmOrderReleaseObj.Series_Code;
                            orderList.Series_Code = series_code;

                            // orderList.Series_Code = partMasterObj.Series_Code;
                            // int psn = orderList.getPSNByDate(shopId,lineId);

                            for (int totalPartGroupCount = 0; totalPartGroupCount < partgroup.Qty; totalPartGroupCount++)
                            {
                                Int64 psn;
                                // orderList.Series_Code = partMasterObj.Series_Code;
                                try
                                {
                                    psn = Convert.ToInt64(db.RS_OM_Order_List.Where(p => p.Shop_ID == shopId && p.Line_ID == lineId && p.Station_ID == stationId).Max(item => item.PSN).ToString());
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
                                orderList.partno = modelCode;
                                orderList.Started_Shift_ID = shift.Shift_ID;
                                orderList.Order_Status = "Started";
                                orderList.Partgroup_ID = partgroup.Partgroup_ID;

                                String myOrderNumber = orderno;
                                if (partgroup.Qty > 1)
                                {
                                    myOrderNumber = orderno + "" + (totalPartGroupCount + 1);
                                }
                                //added by ketan Date 02/09/2017
                                bool is_SerialNumberGenration = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Serial_No_Generation).FirstOrDefault();
                                if (is_SerialNumberGenration)
                                //if (mmOmOrderReleaseObj.Order_Type.Trim().ToLower() == Order_Type.Trim().ToLower())
                                {
                                    var modelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode && m.Plant_ID == plantId && m.Shop_ID == shopId).FirstOrDefault();
                                    RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == modelMaster.Config_ID).FirstOrDefault();
                                    objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                    TempData["updateRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                    serialNumber = orderList.getSerialNumberAD(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);
                                }
                                bool trialSerialNo = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Trial_Body_Serial_No).FirstOrDefault();
                                if (trialSerialNo)
                                {
                                    RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Serial_Logic == "TR").FirstOrDefault();
                                    objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                    TempData["updateTrialBodyRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                    serialNumber = orderList.getTrialBodySerialNumber(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);
                                }

                                //serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, myOrderNumber, modelcode, modelCode, series_code, Convert.ToInt16(partgroup.Serial_Config_ID));
                                orderList.Serial_No = serialNumber;
                                //added by ketan Date 21-07-2016
                                //use to in SaveOrderPartsDetails method to save part details. 
                                if (serialNumber != null)
                                {
                                    TempData["SerialNo"] = serialNumber;
                                }

                                codePosition = "Position 10";
                                if (kitting_Barcode != "")
                                {
                                    orderList.Param_1 = kitting_Barcode;
                                }
                                db.RS_OM_Order_List.Add(orderList);
                                db.SaveChanges();
                                if (kitting_Barcode != "")
                                {
                                    RS_Kitt_Barcode_Master objKitt = new RS_Kitt_Barcode_Master();
                                    objKitt = (from kitt in db.RS_Kitt_Barcode_Master where kitt.Barcode_String == kitting_Barcode select kitt).FirstOrDefault();
                                    objKitt.Serial_No = serialNumber;
                                    objKitt.Is_Consumed = true;
                                    db.Entry(objKitt).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                // string Tyre_Size = "", string Model_Code = ""

                                string user_Host = Convert.ToString(((FDSession)this.Session["FDSession"]).shopId);
                                //if (stationId == 228 || stationId == 158)
                                //{
                                //    var Partno = db.RS_OM_OrderRelease.Where(or => or.Row_ID == rowId).Select(pno => pno.partno).FirstOrDefault();
                                //    MM_OM_Make_Tyre_Details mm_make = new MM_OM_Make_Tyre_Details()
                                //    {
                                //        Plant_ID = plantId,
                                //        Station_ID = stationId,
                                //        Shop_ID = shopId,
                                //        Line_ID = lineId,
                                //        Make_ID = tyreMakeId,
                                //        Row_ID = rowId,
                                //        // Model_Code = db.RS_Model_Master.Where(p => p.Model_Code == Partno).Select(model_code => model_code.Model_Code).FirstOrDefault(),
                                //        Model_Code = orderList.Model_Code,
                                //        Tyre_Size = Tyre_Size,
                                //        PartGroup_ID = orderList.Partgroup_ID, //db.RS_OM_Order_List.Where(p => p.Model_Code == Partno).Select(partId => partId.Partgroup_ID).FirstOrDefault(),
                                //        Inserted_Host = user_Host,
                                //        Inserted_Date = DateTime.Now, //Convert.ToDateTime(((FDSession)this.Session["FDSession"]).insertedDate),
                                //        Serial_No = orderList.Serial_No,
                                //        Inserted_User = Convert.ToString(((FDSession)this.Session["FDSession"]).userId),
                                //    };
                                //    db.MM_OM_Make_Tyre_Details.Add(mm_make);
                                //    db.SaveChanges();
                                //}

                                //End
                                var is_Print = (from pgroup in db.RS_Partgroup where pgroup.Partgroup_ID == partgroup.Partgroup_ID select pgroup.Is_Print).FirstOrDefault();
                                if (is_Print == true)
                                {
                                    // process to add the record in PRN database
                                    RS_PRN mmPRNObj = new RS_PRN();
                                    mmPRNObj.Plant_ID = plantId;
                                    mmPRNObj.Shop_ID = shopId;
                                    mmPRNObj.Line_ID = lineId;
                                    mmPRNObj.Station_ID = stationId;
                                    //Jitendra Mahajan Date 20-05-17

                                    // process to get Quality Ok PRN file
                                    string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/order_start.prn"));
                                    prnFile = prnFile.Replace("012345678910", serialNumber.Trim().ToUpper());
                                    if (partgroup.Qty > 1)
                                    {
                                        if (totalPartGroupCount == 0)
                                        {
                                            String rplsStr = serialNumber.Trim().ToUpper() + " LH";
                                            prnFile = prnFile.Replace("serial_no", rplsStr);
                                        }
                                        else
                                        {
                                            String rplsStr = serialNumber.Trim().ToUpper() + " RH";
                                            prnFile = prnFile.Replace("serial_no", rplsStr);
                                        }
                                    }
                                    else
                                    {
                                        prnFile = prnFile.Replace("serial_no", serialNumber.Trim().ToUpper());
                                    }
                                    prnFile = prnFile.Replace("shop_name", shop_name);
                                    prnFile = prnFile.Replace("part_no", orderList.partno);
                                    prnFile = prnFile.Replace("Production Start", partgroup.Partgrup_Desc);
                                    //Series Description
                                    try
                                    {
                                        RS_Model_Master mmModelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == orderList.partno).Single();

                                        RS_Series series = db.RS_Series.Where(p => p.Series_Code == mmModelMasterObj.Series_Code).Single();
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
                                    //code of set qok in RS_PRN
                                    mmPRNObj.Is_OrderStart = true;

                                    mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    mmPRNObj.Inserted_Date = DateTime.Now;
                                    mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    codePosition = "Position 11";
                                    db.RS_PRN.Add(mmPRNObj);
                                    db.SaveChanges();
                                }

                                //------------------------------------------------------------------
                                mmOmOrderReleaseObj = db.RS_OM_OrderRelease.Find(rowId);
                                mmOmOrderReleaseObj.Is_Edited = true;
                                mmOmOrderReleaseObj.Order_Start = true;
                                // mmOmOrderReleaseObj.Updated_Date = DateTime.Now;
                                //mmOmOrderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                codePosition = "Position 12";
                                db.Entry(mmOmOrderReleaseObj).State = EntityState.Modified;
                                db.SaveChanges();
                                //------------------------------------------------------------------

                                //Insert Data in Order_Status Table
                                RS_OM_Order_Status mmOrderStatus = new RS_OM_Order_Status();

                                mmOrderStatus.Plant_ID = plantId;
                                mmOrderStatus.Shop_ID = shopId;
                                mmOrderStatus.Line_ID = lineId;
                                mmOrderStatus.Station_ID = stationId;
                                mmOrderStatus.Order_No = orderno;
                                mmOrderStatus.Serial_No = serialNumber;
                                mmOrderStatus.Action_Status = "Started";
                                mmOrderStatus.Entry_Date = DateTime.Now.Date;
                                mmOrderStatus.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                mmOrderStatus.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                mmOrderStatus.Inserted_Date = DateTime.Now;
                                codePosition = "Position 13";
                                db.RS_OM_Order_Status.Add(mmOrderStatus);
                                db.SaveChanges();

                                orderList = new RS_OM_Order_List();
                                orderList = db.RS_OM_Order_List.Where(p => p.Serial_No == serialNumber).Single();

                                //check Condition
                                int next_lineId;
                                RS_Lines line = db.RS_Lines.Where(p => p.Line_ID == lineId).Single();
                                if (line.Is_Conveyor == false)
                                {
                                    //IF LINE IS NOT CONVEYOR THEN PUT THE STARTED ORDER IN THE STATION AND ALSO IN LINE COMPLETE BUFFER
                                    var route_marriage_station = (from route_marriage in db.RS_Route_Marriage_Station
                                                                  join marriageStn in db.RS_Stations on route_marriage.Marriage_Station_ID equals marriageStn.Station_ID
                                                                  where route_marriage.Sub_Line_ID == lineId && marriageStn.Is_Buffer == true
                                                                  select route_marriage).ToList();

                                    foreach (var item in route_marriage_station)
                                    {
                                        next_lineId = Convert.ToInt32(item.Marriage_Line_ID);

                                        RS_Line_Complete_Buffer line_complete_buffer = new RS_Line_Complete_Buffer();
                                        line_complete_buffer.Plant_ID = plantId;
                                        line_complete_buffer.Shop_ID = shopId;
                                        line_complete_buffer.Line_ID = next_lineId;
                                        line_complete_buffer.Station_ID = item.Marriage_Station_ID;
                                        line_complete_buffer.SerialNo = serialNumber;
                                        line_complete_buffer.Inserted_Time = DateTime.Now;
                                        codePosition = "Position 14";
                                        db.RS_Line_Complete_Buffer.Add(line_complete_buffer);
                                        db.SaveChanges();
                                    }

                                    //Staion_tracking
                                    // process to add the order in station tracking
                                    RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                                    mmStationTrackingObj.Plant_ID = plantId;
                                    mmStationTrackingObj.Shop_ID = shopId;
                                    mmStationTrackingObj.Line_ID = lineId;
                                    mmStationTrackingObj.Station_ID = stationId;
                                    mmStationTrackingObj.SerialNo = serialNumber;
                                    mmStationTrackingObj.Track_SerialNo = serialNumber;
                                    mmStationTrackingObj.Is_Edited = true;
                                    mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                    //db.RS_Station_Tracking.Add(mmStationTrackingObj);
                                    codePosition = "Position 15";
                                    db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    //IF LINE IS CONVEYOR THEN PUT THE STARTED ORDER IN THE STATION ONLY
                                    // process to add the order in station tracking
                                    RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                                    mmStationTrackingObj.Plant_ID = plantId;
                                    mmStationTrackingObj.Shop_ID = shopId;
                                    mmStationTrackingObj.Is_Edited = true;
                                    mmStationTrackingObj.Line_ID = lineId;
                                    mmStationTrackingObj.Station_ID = stationId;
                                    mmStationTrackingObj.SerialNo = serialNumber;
                                    mmStationTrackingObj.Track_SerialNo = serialNumber;
                                    mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                    //db.RS_Station_Tracking.Add(mmStationTrackingObj);
                                    codePosition = "Position 16";
                                    db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }

                jsonData.status = true;
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                jsonData.status = false;
                generalHelper.addControllerException(ex, "OrderStartController", "StartOrder(" + codePosition + ")", ((FDSession)this.Session["FDSession"]).userId);
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getErrorProofingGeneologyPartsByModelCode(String modelCode, int stationId)
        {
            try
            {
                string query = "select * from RS_Partmaster where Part_No in(select part_no from RS_BOM_Item where model_code = '" + modelCode + "') and RS_Partmaster.Genealogy = 1 and RS_Partmaster.Error_Proofing = 1 and RS_Partmaster.Station_ID = " + stationId + " order by PartGroup_ID ";
                var plan_order = db.Database.SqlQuery<RS_Partmaster>(query).ToList();
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
                genObj.addControllerException(ex, "OrderStart", "getErrorProofingGeneologyPartsByModelCode", 1);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StartOrder(int rowId, string Tyre_Size = "", string Model_Code = "", int tyreMakeId = 0, string kitting_Barcode = "", string Is_PPC_Kitting_Order = "", string genealogyStr = "")
        {

            JSONData jsonData = new JSONData();
            string codePosition = "";
            var CurrentModel = "";
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                string platformName = string.Empty;
                //for current shift
                DateTime date = DateTime.Now;
                TimeSpan currentTime = date.TimeOfDay;

                var shift = db.RS_Shift.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Shift_Start_Time <= currentTime && currentTime <= m.Shift_End_Time).Select(m => m).FirstOrDefault();
                ////
                string orderNo = string.Empty;
                string orderno = "";
                string modelcode = "";
                int rsn = 0;
                bool isModelStart = true;// checkIsModelStart(rowId, plantId);

                if (isModelStart)
                {
                    //if (kitting_Barcode != "")
                    //{
                    //    if (db.RS_Kitt_Barcode_Master.Where(kit => kit.Barcode_String == kitting_Barcode && kit.Is_Consumed == false).Count() == 0)
                    //    {
                    //        jsonData.kit_message = "This kitt: " + kitting_Barcode + " is not available.";
                    //        jsonData.status = false;
                    //        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    RS_OM_OrderRelease ordReleaseObj = db.RS_OM_OrderRelease.Find(rowId);
                    RS_MAST_VIN_LINE vin_Line;

                    orderno = ordReleaseObj.Order_No;
                    modelcode = ordReleaseObj.Model_Code;
                    rsn = Convert.ToInt32(ordReleaseObj.RSN);

                    CurrentModel = modelcode;
                    string shop_name = "";

                    //find Shop Name
                    RS_Shops shop = db.RS_Shops.Find(shopId);

                    if (shop != null)
                    {
                        shop_name = shop.Shop_Name;
                    }

                    String serialNumber = "";
                    String VinNumber = "";
                    RS_OM_Order_List orderList = new RS_OM_Order_List();
                    orderList.Plant_ID = plantId;
                    orderList.Shop_ID = shopId;
                    orderList.Line_ID = lineId;
                    orderList.Station_ID = stationId;

                    if (orderList.Hold_Order_Check(rowId, shopId, lineId, orderno, modelcode, rsn) == true)
                    {
                        jsonData.status = false;
                        jsonData.message = "This Order: " + orderno + " is Holded right now .";
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    }
                    else if ((db.RS_OM_OrderRelease.Any(a => a.Row_ID == rowId && a.Is_Blocked == true)) == true)
                    {
                        jsonData.status = false;
                        jsonData.message = "This Order: " + orderno + " is Blocked for sequencing right now .";
                        return Json(jsonData, JsonRequestBehavior.AllowGet);
                    }

                    else
                    {
                        RS_Partgroup partgroup;
                        RS_OM_OrderRelease mmOmOrderReleaseObj;


                        partgroup = (from partgroupData in db.RS_Partgroup
                                     where partgroupData.Line_ID == lineId
                                     select partgroupData
                                  ).FirstOrDefault();

                        if (partgroup != null)
                        {
                            if (partgroup.Order_Create == true)
                            {
                                int newRSN = 0;
                                //CODE BLOCK FOR Updating the RSN in PlannedOrder Table AND RESOLVING ORDER Missing in sub assembly issue
                                try
                                {
                                    // ---------------- CODE FOR ELIMINATING DUPLICATE ORDER ID (SUB ASSEMBLY MISSING ORDERS ISSUE)------------------
                                    // process to check the order is already started or not
                                    RS_OM_OrderRelease orderReleaseObj = db.RS_OM_OrderRelease.Where(p => p.Row_ID == rowId && p.Shop_ID == shopId && p.Is_Rejected != true).FirstOrDefault();
                                    //if (orderReleaseObj.Order_Status == "Release")
                                    {
                                        // process to check for the same order number order is already started in order list or not
                                        orderNo = orderReleaseObj.Order_No;
                                        //{returning null because RS_OM_Order_List does not have entry for released orders due to this cant start order 
                                        RS_OM_Order_List orderListObj = db.RS_OM_Order_List.Where(p => p.Order_No == orderNo && p.Shop_ID == shopId && p.Line_ID == lineId && p.Is_Rejected != true).FirstOrDefault();
                                        //}
                                        //RS_OM_Order_List orderListObj = db.RS_OM_Order_List.Where(p => p.Order_No == orderNo && p.Shop_ID == shopId && p.Line_ID == lineId && p.Is_Rejected != true).FirstOrDefault();

                                        if (orderListObj != null)
                                        {
                                            if (orderReleaseObj.Order_Status == "Release")
                                            {
                                                RS_OM_Planned_Orders plannedOrdrObj = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == rowId && a.Planned_Date == DateTime.Today).FirstOrDefault();
                                                if (plannedOrdrObj != null)
                                                {
                                                    newRSN = Convert.ToInt32(plannedOrdrObj.RSN.GetValueOrDefault(0));
                                                }
                                                // it means order started, but status not updated
                                                orderReleaseObj.Order_Status = "Started";
                                                orderReleaseObj.Updated_Date = DateTime.Now;
                                                orderReleaseObj.RSN = newRSN;
                                                orderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                                codePosition = "Position 1";
                                                db.Entry(orderReleaseObj).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                            return Json(true, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    //----------------------------------------------------------------

                                    newRSN = generalHelper.updatePlannedRSN(rowId, ((FDSession)this.Session["FDSession"]).userId);
                                }
                                catch (Exception exp)
                                {
                                    while (exp.InnerException != null)
                                    {
                                        exp = exp.InnerException;
                                    }
                                    generalHelper.addControllerException(exp, "OrderStartController", "INSIDE NEW CODE BLOCK", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
                                }
                                //----------------------------------------------------------------

                                // add record in order list table
                                mmOmOrderReleaseObj = (from orderRelease in db.RS_OM_OrderRelease
                                                       where orderRelease.Row_ID == rowId
                                                       select orderRelease).Single();

                                string modelCode = mmOmOrderReleaseObj.partno;
                                decimal lineIden = Convert.ToDecimal(mmOmOrderReleaseObj.Line_ID);
                                orderList.Order_No = mmOmOrderReleaseObj.Order_No;
                                orderList.Model_Code = modelCode;
                                orderList.Series_Code = mmOmOrderReleaseObj.Series_Code;

                                Int64 psn;
                                // orderList.Series_Code = partMasterObj.Series_Code;
                                try
                                {
                                    psn = Convert.ToInt64(db.RS_OM_Order_List.Where(p => p.Shop_ID == shopId && p.Line_ID == lineId && p.Station_ID == stationId).Max(item => item.PSN).ToString());
                                }
                                catch (Exception)
                                {
                                    psn = 0;
                                }

                                //int psn = orderList.getPSNByDate(shopId,lineId);
                                orderList.PSN = psn + 1;
                                int dsn = orderList.getDSNByDateShop(shopId, lineId);

                                orderList.DSN = dsn + 1;
                                orderList.Entry_Date = DateTime.Now.Date;
                                orderList.Entry_Time = DateTime.Now.TimeOfDay;
                                orderList.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                orderList.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                orderList.Inserted_Date = DateTime.Now;
                                orderList.partno = mmOmOrderReleaseObj.partno;
                                orderList.Started_Shift_ID = shift.Shift_ID;
                                orderList.Order_Status = "Started";
                                orderList.Partgroup_ID = partgroup.Partgroup_ID;
                                // serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, mmOmOrderReleaseObj.Order_No, mmOmOrderReleaseObj.Model_Code, mmOmOrderReleaseObj.partno, mmOmOrderReleaseObj.Series_Code,partgroup.Partgrup_Desc,partgroup.Serial_Config_ID);

                                int count = db.RS_OM_OrderRelease.Where(p => p.Shop_ID == shopId && p.Order_No == orderNo).Count();
                                if (count == 1)
                                {
                                    //added by ketan Date 02/09/2017
                                    //string Order_Type = "Regular Production";
                                    bool is_SerialNumberGenration = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim() && m.Shop_ID == shopId).Select(m => m.Is_Serial_No_Generation).FirstOrDefault();
                                    if (is_SerialNumberGenration)
                                    //if (mmOmOrderReleaseObj.Order_Type.Trim().ToLower() == Order_Type.Trim().ToLower())
                                    {
                                        var modelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode && m.Plant_ID == plantId && m.Shop_ID == shopId).FirstOrDefault();
                                        try
                                        {
                                            platformName = db.RS_OM_Platform.Find(modelMaster.Platform_Id).Platform_Name;

                                        }
                                        catch (Exception)
                                        {

                                            platformName = "";
                                        }
                                        RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == modelMaster.Config_ID).FirstOrDefault();
                                        //var test = objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == modelMaster.Config_ID).FirstOrDefault();
                                        objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number < 99999 ? objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1 :1;
                                        TempData["updateRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                        //serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, mmOmOrderReleaseObj.Order_No, mmOmOrderReleaseObj.Model_Code, mmOmOrderReleaseObj.partno, mmOmOrderReleaseObj.Series_Code, Convert.ToInt16(partgroup.Serial_Config_ID));
                                        serialNumber = orderList.getSerialNumberAD(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);

                                        var IsVIN = db.RS_Stations.Where(m => m.Station_ID == stationId).Select(m => m.IsVin).FirstOrDefault();
                                        if (IsVIN == true)
                                        {
                                            VinNumber = orderList.getVinNumberGenration(modelcode, lineIden, mmOmOrderReleaseObj.Order_No);
                                        }

                                    }
                                    bool trialSerialNo = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Trial_Body_Serial_No).FirstOrDefault();
                                    if (trialSerialNo)
                                    {
                                        RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Display_Name == "TR").FirstOrDefault();
                                        objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                        TempData["updateTrialBodyRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                        serialNumber = orderList.getTrialBodySerialNumber(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);
                                        if (lineIden == 5)
                                        {
                                            VinNumber = orderList.getVinNumberGenration(modelcode, lineIden, mmOmOrderReleaseObj.Order_No);
                                        }
                                    }
                                    ////added by mukesh
                                    {
                                        bool tearDownSerialNo = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Tear_Down_Body_Serial_No).FirstOrDefault();
                                        if (tearDownSerialNo)
                                        {
                                            RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Display_Name == "TD").FirstOrDefault();
                                            objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                            TempData["updateTearDownBodyRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                            serialNumber = orderList.getTearDownBodySerialNumber(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);
                                            if (lineIden == 5)
                                            {
                                                VinNumber = orderList.getVinNumberGenration(modelcode, lineIden, mmOmOrderReleaseObj.Order_No);
                                            }
                                        }
                                    }////

                                    bool is_VendorSerialNumberGenration = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim() && m.Shop_ID == shopId).Select(m => m.Is_Vendor_Serial_No).FirstOrDefault();
                                    if (is_VendorSerialNumberGenration)
                                    //if (mmOmOrderReleaseObj.Order_Type.Trim().ToLower() == Order_Type.Trim().ToLower())
                                    {
                                        var modelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode && m.Plant_ID == plantId && m.Shop_ID == shopId).FirstOrDefault();
                                        if (genealogyStr != "")
                                        {
                                            String[] str = genealogyStr.Split('#');
                                            var VendorNo = str[0];
                                            serialNumber = orderList.getVendorSerialNumberAD(modelCode, VendorNo, mmOmOrderReleaseObj.Order_No);
                                        }
                                    }
                                    //comment by ketan change in serial logic as per Nashik AD
                                    //serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, mmOmOrderReleaseObj.Order_No, mmOmOrderReleaseObj.Model_Code, mmOmOrderReleaseObj.partno, mmOmOrderReleaseObj.Series_Code, Convert.ToInt16(partgroup.Serial_Config_ID));
                                }
                                else
                                {
                                    //we reject only main order 
                                    serialNumber = db.RS_OM_Order_List.Where(ol => ol.Shop_ID == shopId && ol.Order_No == orderNo && ol.Is_Rejected == true).Select(ol => ol.Serial_No).FirstOrDefault();
                                }

                                orderList.Serial_No = serialNumber;
                                orderList.VIN_Number = VinNumber;
                                //added by ketan Date 21-07-2016
                                //use to in SaveOrderPartsDetails method to save part details. 
                                //orderList.Serial_No = serialNumber;
                                if (serialNumber != null)
                                {
                                    TempData["SerialNo"] = serialNumber;
                                }

                                if (VinNumber != null)
                                {
                                    TempData["VINNo"] = VinNumber;
                                }
                                codePosition = "Position 2";
                                //sandip
                                if (kitting_Barcode != "")
                                {
                                    orderList.Param_1 = kitting_Barcode;
                                }

                                if (genealogyStr != "")
                                {
                                    String[] str = genealogyStr.Split('#');
                                    for (int j = 0; j < str.Length; j++)
                                    {
                                        if (j == 0) { orderList.Param_1 = str[j].ToUpper(); }
                                        if (j == 1) { orderList.Param_2 = str[j].ToUpper(); }
                                        if (j == 2) { orderList.Param_3 = str[j].ToUpper(); }
                                        if (j == 3) { orderList.Param_4 = str[j].ToUpper(); }
                                        if (j == 4) { orderList.Param_5 = str[j].ToUpper(); }
                                    }
                                }
                                db.RS_OM_Order_List.Add(orderList);
                                db.SaveChanges();


                                jsonData.CurrentEngine = orderList.Serial_No;
                                jsonData.ModelCode = orderList.Model_Code;
                                jsonData.OrderNo = orderList.Order_No;

                                //if (kitting_Barcode != "")
                                //{
                                //    RS_Kitt_Barcode_Master objKitt = new RS_Kitt_Barcode_Master();
                                //    objKitt = (from kitt in db.RS_Kitt_Barcode_Master where kitt.Barcode_String == kitting_Barcode select kitt).FirstOrDefault();
                                //    objKitt.Serial_No = serialNumber;
                                //    objKitt.Is_Consumed = true;
                                //    db.Entry(objKitt).State = EntityState.Modified;
                                //    db.SaveChanges();
                                //}

                                //Start 22 may 2017

                                var is_Print = (from pgroup in db.RS_Partgroup where pgroup.Partgroup_ID == partgroup.Partgroup_ID select pgroup.Is_Print).FirstOrDefault();
                                var is_PRN = db.RS_Lines.Where(m => m.Line_ID == lineIden).Select(m => m.Is_PRN).FirstOrDefault();
                                if (is_Print == true && is_PRN == true)
                                {
                                    // process to add the record in PRN database
                                    RS_PRN mmPRNObj = new RS_PRN();
                                    mmPRNObj.Plant_ID = plantId;
                                    mmPRNObj.Shop_ID = shopId;
                                    mmPRNObj.Line_ID = lineId;
                                    mmPRNObj.Station_ID = stationId;
                                    // process to get Quality Ok PRN file
                                    var Barcode2D = orderList.partno + ":" + serialNumber.Trim().ToUpper();
                                    string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/order_start.prn"));
                                    prnFile = prnFile.Replace("012345678910", serialNumber.Trim().ToUpper());
                                    prnFile = prnFile.Replace("serial_no", serialNumber.Trim().ToUpper());
                                    prnFile = prnFile.Replace("shop_name", shop_name);
                                    prnFile = prnFile.Replace("part_no", orderList.partno);
                                    prnFile = prnFile.Replace("Production Start", "Production");
                                    prnFile = prnFile.Replace("Platform_name", platformName);
                                    prnFile = prnFile.Replace("2DBarcode", Barcode2D);

                                    try
                                    {
                                        RS_Model_Master mmModelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == orderList.partno).Single();
                                        RS_Series series = db.RS_Series.Where(p => p.Series_Code == mmModelMasterObj.Series_Code).Single();//-----------------need to addd color Code Logic
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
                                    codePosition = "Position 3";
                                    mmPRNObj.PRN_Text = prnFile;
                                    //code for enter bit in RS_PRN file is_orderstartr
                                    mmPRNObj.Is_OrderStart = true;

                                    mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    mmPRNObj.Inserted_Date = DateTime.Now;
                                    mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    db.RS_PRN.Add(mmPRNObj);
                                    db.SaveChanges();
                                    //End
                                }

                                var is_buildsheet = db.RS_Lines.Where(m => m.Line_ID == lineIden).Select(m => m.Is_Buildsheet).FirstOrDefault();
                                if(is_Print == true && is_buildsheet == true)
                                {
                                    BuildSheetPrint(serialNumber);

                                }

                                //if (partgroup.Order_Create == true)
                                //{
                                //    General genObj = new General();
                                //    int runningSerialCounter = genObj.getCurrentRunningSerial(mmOmOrderReleaseObj.partno);
                                //    RS_Serial_Number_Data serialNumberDataObj = db.RS_Serial_Number_Data.Where(a => a.Part_No.Trim() == modelCode.Trim()).FirstOrDefault();
                                //    if (serialNumberDataObj != null)
                                //    {
                                //        codePosition = "Position 4";
                                //        UpdateModel(serialNumberDataObj);
                                //        serialNumberDataObj.Running_Serial = runningSerialCounter;
                                //        serialNumberDataObj.Is_Edited = true;
                                //        db.Entry(serialNumberDataObj).State = EntityState.Modified;
                                //        db.SaveChanges();
                                //    }
                                //}


                                //---------------------Release-------------------------------------//
                                try
                                {
                                    mmOmOrderReleaseObj = db.RS_OM_OrderRelease.Find(rowId);
                                    mmOmOrderReleaseObj.Order_Status = "Started";
                                    mmOmOrderReleaseObj.RSN = newRSN;
                                    mmOmOrderReleaseObj.Order_Start = true;
                                    mmOmOrderReleaseObj.Updated_Date = DateTime.Now;
                                    mmOmOrderReleaseObj.Is_Edited = true;
                                    mmOmOrderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    codePosition = "Position 5";
                                    db.Entry(mmOmOrderReleaseObj).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                catch (Exception exp)
                                {
                                    while (exp.InnerException != null)
                                    {
                                        exp = exp.InnerException;
                                    }
                                    generalHelper.addControllerException(exp, "OrderStartController", "Updating Order Release Table(Line ID:" + mmOmOrderReleaseObj.Line_ID + ", Order RowID:" + mmOmOrderReleaseObj.Row_ID + ", Order No:" + mmOmOrderReleaseObj.Order_No + ")", ((FDSession)this.Session["FDSession"]).userId);
                                }
                                //------------------------------------------------------------------
                                RS_OM_Planned_Orders plannedOrdersObj = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == rowId && a.Planned_Date == DateTime.Today).FirstOrDefault();
                                if (plannedOrdersObj != null)
                                {
                                    plannedOrdersObj.Order_Status = "Started";
                                    plannedOrdersObj.Last_Status_Change_Time = DateTime.Now;
                                    db.Entry(plannedOrdersObj).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                //Insert Data in Order_Status Table
                                RS_OM_Order_Status mmOrderStatus = new RS_OM_Order_Status();

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
                                codePosition = "Position 6";
                                db.RS_OM_Order_Status.Add(mmOrderStatus);
                                db.SaveChanges();

                                //Insert Data in Order Status Live Table
                                var ActionStatus = db.RS_Status.Where(m => m.Sort_Order == 2 && m.Shop_ID == shopId).Select(m => m.Status).FirstOrDefault();
                                if(ActionStatus != null)
                                {
                                    RS_OM_Order_Status_Live RS_OM_Order_Status_Live = new RS_OM_Order_Status_Live();
                                    RS_OM_Order_Status_Live.Plant_ID = plantId;
                                    RS_OM_Order_Status_Live.Shop_ID = shopId;
                                    RS_OM_Order_Status_Live.Line_ID = lineId;
                                    RS_OM_Order_Status_Live.Station_ID = stationId;
                                    RS_OM_Order_Status_Live.Order_No = mmOmOrderReleaseObj.Order_No;
                                    RS_OM_Order_Status_Live.Serial_No = serialNumber;
                                    RS_OM_Order_Status_Live.Action_Status = ActionStatus;
                                    RS_OM_Order_Status_Live.Entry_Date = DateTime.Now.Date;
                                    RS_OM_Order_Status_Live.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    RS_OM_Order_Status_Live.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    RS_OM_Order_Status_Live.Inserted_Date = DateTime.Now;
                                    RS_OM_Order_Status_Live.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    RS_OM_Order_Status_Live.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    RS_OM_Order_Status_Live.Updated_Date = DateTime.Now;
                                    codePosition = "Position 6.1";
                                    db.RS_OM_Order_Status_Live.Add(RS_OM_Order_Status_Live);
                                    db.SaveChanges();
                                }
                                

                                orderList = new RS_OM_Order_List();
                                if (!string.IsNullOrWhiteSpace(serialNumber))
                                {
                                    orderList = db.RS_OM_Order_List.Where(p => p.Serial_No == serialNumber).Single();
                                }


                                //check Condition
                                int next_lineId;
                                RS_Lines line;
                                line = db.RS_Lines.Where(p => p.Line_ID == lineId).Single();
                                if (line.Is_Conveyor == false)
                                {
                                    var route_marriage_station = (from route_marriage in db.RS_Route_Marriage_Station
                                                                  where route_marriage.Sub_Line_ID == lineId
                                                                  select route_marriage).ToList();

                                    foreach (var item in route_marriage_station)
                                    {
                                        next_lineId = Convert.ToInt32(item.Marriage_Line_ID);

                                        RS_Stations station;
                                        station = db.RS_Stations.Where(p => p.Station_ID == item.Marriage_Station_ID).Single();
                                        if (station.Is_Buffer == true)
                                        {
                                            RS_Line_Complete_Buffer line_complete_buffer = new RS_Line_Complete_Buffer();
                                            line_complete_buffer.Plant_ID = plantId;
                                            line_complete_buffer.Shop_ID = shopId;
                                            line_complete_buffer.Line_ID = next_lineId;
                                            line_complete_buffer.Station_ID = item.Marriage_Station_ID;
                                            line_complete_buffer.SerialNo = serialNumber;
                                            line_complete_buffer.Inserted_Time = DateTime.Now;
                                            codePosition = "Position 7";
                                            db.RS_Line_Complete_Buffer.Add(line_complete_buffer);
                                            db.SaveChanges();
                                        }
                                    }

                                    //Station_tracking
                                    // process to add the order in station tracking
                                    RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                                    mmStationTrackingObj.Plant_ID = plantId;
                                    mmStationTrackingObj.Shop_ID = shopId;
                                    mmStationTrackingObj.Line_ID = lineId;
                                    mmStationTrackingObj.Station_ID = stationId;
                                    mmStationTrackingObj.SerialNo = serialNumber;
                                    mmStationTrackingObj.Track_SerialNo = serialNumber;
                                    mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                    mmStationTrackingObj.Is_Edited = true;
                                    //db.RS_Station_Tracking.Add(mmStationTrackingObj);
                                    codePosition = "Position 8";
                                    db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                    db.SaveChanges();

                                }
                                else
                                {
                                    // process to add the order in station tracking
                                    RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                                    mmStationTrackingObj.Plant_ID = plantId;
                                    mmStationTrackingObj.Shop_ID = shopId;
                                    mmStationTrackingObj.Line_ID = lineId;
                                    mmStationTrackingObj.Station_ID = stationId;
                                    mmStationTrackingObj.Is_Edited = true;
                                    mmStationTrackingObj.SerialNo = serialNumber;
                                    mmStationTrackingObj.Track_SerialNo = serialNumber;
                                    mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                    //db.RS_Station_Tracking.Add(mmStationTrackingObj);
                                    codePosition = "Position 9";
                                    db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                // process to add the order in quality order queue

                                RS_Quality_Captures mmQualityCapturesObj = new RS_Quality_Captures();
                                mmQualityCapturesObj.insertOrderInQualityOrderQueue(serialNumber, orderno, plantId, shopId, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);

                                //code to update running serial number in serial number.
                                if (TempData["updateRunningSerialNumber"] != null)
                                {
                                    var modelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode).FirstOrDefault();
                                    RS_Serial_Number_Configuration obj_RS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == modelMaster.Config_ID).FirstOrDefault();
                                    obj_RS_Serial_Number_Configuration.Running_Serial_Number = Convert.ToInt32(TempData["updateRunningSerialNumber"]);
                                    db.Entry(obj_RS_Serial_Number_Configuration).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                if (TempData["updateTrialBodyRunningSerialNumber"] != null)
                                {
                                    var modelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode).FirstOrDefault();
                                    RS_Serial_Number_Configuration obj_RS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Display_Name == "TR").FirstOrDefault();
                                    obj_RS_Serial_Number_Configuration.Running_Serial_Number = Convert.ToInt32(TempData["updateTrialBodyRunningSerialNumber"]);
                                    db.Entry(obj_RS_Serial_Number_Configuration).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                mmOmOrderReleaseObj = db.RS_OM_OrderRelease.Find(rowId);
                                String modelCode = mmOmOrderReleaseObj.partno;


                                orderList.Order_No = mmOmOrderReleaseObj.Order_No;
                                orderList.Model_Code = modelCode;

                                decimal? series_code;
                                series_code = mmOmOrderReleaseObj.Series_Code;
                                orderList.Series_Code = series_code;

                                // orderList.Series_Code = partMasterObj.Series_Code;
                                // int psn = orderList.getPSNByDate(shopId,lineId);

                                for (int totalPartGroupCount = 0; totalPartGroupCount < partgroup.Qty; totalPartGroupCount++)
                                {
                                    Int64 psn;
                                    // orderList.Series_Code = partMasterObj.Series_Code;
                                    try
                                    {
                                        psn = Convert.ToInt64(db.RS_OM_Order_List.Where(p => p.Shop_ID == shopId && p.Line_ID == lineId && p.Station_ID == stationId).Max(item => item.PSN).ToString());
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
                                    orderList.partno = modelCode;
                                    orderList.Started_Shift_ID = shift.Shift_ID;
                                    orderList.Order_Status = "Started";
                                    orderList.Partgroup_ID = partgroup.Partgroup_ID;

                                    String myOrderNumber = orderno;
                                    if (partgroup.Qty > 1)
                                    {
                                        myOrderNumber = orderno + "" + (totalPartGroupCount + 1);
                                    }
                                    //added by ketan Date 02/09/2017
                                    bool is_SerialNumberGenration = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Serial_No_Generation).FirstOrDefault();
                                    if (is_SerialNumberGenration)
                                    //if (mmOmOrderReleaseObj.Order_Type.Trim().ToLower() == Order_Type.Trim().ToLower())
                                    {
                                        var modelMaster = db.RS_Model_Master.Where(m => m.Model_Code == modelCode && m.Plant_ID == plantId && m.Shop_ID == shopId).FirstOrDefault();
                                        RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == modelMaster.Config_ID).FirstOrDefault();
                                        objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                        TempData["updateRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                        serialNumber = orderList.getSerialNumberAD(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);

                                    }
                                    bool trialSerialNo = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Trial_Body_Serial_No).FirstOrDefault();
                                    if (trialSerialNo)
                                    {
                                        RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Serial_Logic == "TR").FirstOrDefault();
                                        objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                        TempData["updateTrialBodyRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                        serialNumber = orderList.getTrialBodySerialNumber(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);
                                    }
                                    //serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, myOrderNumber, modelcode, modelCode, series_code, Convert.ToInt16(partgroup.Serial_Config_ID));
                                    orderList.Serial_No = serialNumber;
                                    //added by mukesh check
                                    bool tearDownSerialNo = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim() == mmOmOrderReleaseObj.Order_Type.Trim()).Select(m => m.Is_Tear_Down_Body_Serial_No).FirstOrDefault();
                                    if (tearDownSerialNo)
                                    {
                                        RS_Serial_Number_Configuration objRS_Serial_Number_Configuration = db.RS_Serial_Number_Configuration.Where(m => m.Serial_Logic == "TD").FirstOrDefault();
                                        objRS_Serial_Number_Configuration.Running_Serial_Number = objRS_Serial_Number_Configuration.Running_Serial_Number.GetValueOrDefault(0) + 1;
                                        TempData["updateTearDownBodyRunningSerialNumber"] = objRS_Serial_Number_Configuration.Running_Serial_Number;
                                        serialNumber = orderList.getTearDownBodySerialNumber(modelCode, objRS_Serial_Number_Configuration.Running_Serial_Number, mmOmOrderReleaseObj.Order_No);
                                    }
                                    //serialNumber = orderList.getSerialNumber(shopId, lineId, dsn + 1, myOrderNumber, modelcode, modelCode, series_code, Convert.ToInt16(partgroup.Serial_Config_ID));
                                    orderList.Serial_No = serialNumber;
                                    ////

                                    //added by ketan Date 21-07-2016
                                    //use to in SaveOrderPartsDetails method to save part details. 
                                    if (serialNumber != null)
                                    {
                                        TempData["SerialNo"] = serialNumber;
                                    }

                                    codePosition = "Position 10";
                                    if (kitting_Barcode != "")
                                    {
                                        orderList.Param_1 = kitting_Barcode;
                                    }
                                    db.RS_OM_Order_List.Add(orderList);
                                    db.SaveChanges();
                                    if (kitting_Barcode != "")
                                    {
                                        RS_Kitt_Barcode_Master objKitt = new RS_Kitt_Barcode_Master();
                                        objKitt = (from kitt in db.RS_Kitt_Barcode_Master where kitt.Barcode_String == kitting_Barcode select kitt).FirstOrDefault();
                                        objKitt.Serial_No = serialNumber;
                                        objKitt.Is_Consumed = true;
                                        db.Entry(objKitt).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    //Added By ketan to save PPC kitting order plan
                                    //Date 18-jul-17
                                    if (Is_PPC_Kitting_Order != null && Is_PPC_Kitting_Order != "")
                                    {
                                        SaveKittingorderMaster(rowId, plantId, shopId, lineId, stationId);
                                    }

                                    // string Tyre_Size = "", string Model_Code = ""

                                    string user_Host = Convert.ToString(((FDSession)this.Session["FDSession"]).shopId);
                                    //if (stationId == 228 || stationId == 158)
                                    //{
                                    //    var Partno = db.RS_OM_OrderRelease.Where(or => or.Row_ID == rowId).Select(pno => pno.partno).FirstOrDefault();
                                    //    MM_OM_Make_Tyre_Details mm_make = new MM_OM_Make_Tyre_Details()
                                    //    {
                                    //        Plant_ID = plantId,
                                    //        Station_ID = stationId,
                                    //        Shop_ID = shopId,
                                    //        Line_ID = lineId,
                                    //        Make_ID = tyreMakeId,
                                    //        Row_ID = rowId,
                                    //        // Model_Code = db.RS_Model_Master.Where(p => p.Model_Code == Partno).Select(model_code => model_code.Model_Code).FirstOrDefault(),
                                    //        Model_Code = orderList.Model_Code,
                                    //        Tyre_Size = Tyre_Size,
                                    //        PartGroup_ID = orderList.Partgroup_ID, //db.RS_OM_Order_List.Where(p => p.Model_Code == Partno).Select(partId => partId.Partgroup_ID).FirstOrDefault(),
                                    //        Inserted_Host = user_Host,
                                    //        Inserted_Date = DateTime.Now, //Convert.ToDateTime(((FDSession)this.Session["FDSession"]).insertedDate),
                                    //        Serial_No = orderList.Serial_No,
                                    //        Inserted_User = Convert.ToString(((FDSession)this.Session["FDSession"]).userId),
                                    //    };
                                    //    db.MM_OM_Make_Tyre_Details.Add(mm_make);
                                    //    db.SaveChanges();
                                    //}

                                    //End
                                    var is_Print = (from pgroup in db.RS_Partgroup where pgroup.Partgroup_ID == partgroup.Partgroup_ID select pgroup.Is_Print).FirstOrDefault();
                                    var LineID = db.RS_OM_OrderRelease.Where(m => m.Order_No == orderno).Select(m => m.Line_ID).FirstOrDefault();
                                    var is_PRN = db.RS_Lines.Where(m => m.Line_ID == LineID).Select(m => m.Is_PRN).FirstOrDefault();
                                    if (is_Print == true && is_PRN == true)
                                    {
                                        // process to add the record in PRN database
                                        RS_PRN mmPRNObj = new RS_PRN();
                                        mmPRNObj.Plant_ID = plantId;
                                        mmPRNObj.Shop_ID = shopId;
                                        mmPRNObj.Line_ID = lineId;
                                        mmPRNObj.Station_ID = stationId;
                                        //Jitendra Mahajan Date 20-05-17

                                        // process to get Quality Ok PRN file
                                        string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/order_start.prn"));
                                        prnFile = prnFile.Replace("012345678910", serialNumber.Trim().ToUpper());
                                        if (partgroup.Qty > 1)
                                        {
                                            if (totalPartGroupCount == 0)
                                            {
                                                String rplsStr = serialNumber.Trim().ToUpper() + " LH";
                                                prnFile = prnFile.Replace("serial_no", rplsStr);
                                            }
                                            else
                                            {
                                                String rplsStr = serialNumber.Trim().ToUpper() + " RH";
                                                prnFile = prnFile.Replace("serial_no", rplsStr);
                                            }
                                        }
                                        else
                                        {
                                            prnFile = prnFile.Replace("serial_no", serialNumber.Trim().ToUpper());
                                        }
                                        var Barcode2D = orderList.partno + ":" + serialNumber.Trim().ToUpper();
                                        prnFile = prnFile.Replace("shop_name", shop_name);
                                        prnFile = prnFile.Replace("part_no", orderList.partno);
                                        prnFile = prnFile.Replace("Production Start", "Production");
                                        prnFile = prnFile.Replace("Platform_name", platformName);
                                        prnFile = prnFile.Replace("2DBarcode", Barcode2D);

                                        //Series Description
                                        try
                                        {
                                            RS_Model_Master mmModelMasterObj = db.RS_Model_Master.Where(p => p.Model_Code == orderList.partno).Single();

                                            RS_Series series = db.RS_Series.Where(p => p.Series_Code == mmModelMasterObj.Series_Code).Single();
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
                                        //code of set qok in RS_PRN
                                        mmPRNObj.Is_OrderStart = true;

                                        mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                        mmPRNObj.Inserted_Date = DateTime.Now;
                                        mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                        codePosition = "Position 11";
                                        db.RS_PRN.Add(mmPRNObj);
                                        db.SaveChanges();
                                    }

                                    //------------------------------------------------------------------
                                    mmOmOrderReleaseObj = db.RS_OM_OrderRelease.Find(rowId);
                                    mmOmOrderReleaseObj.Is_Edited = true;
                                    mmOmOrderReleaseObj.Order_Start = true;
                                    // mmOmOrderReleaseObj.Updated_Date = DateTime.Now;
                                    //mmOmOrderReleaseObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    codePosition = "Position 12";
                                    db.Entry(mmOmOrderReleaseObj).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //------------------------------------------------------------------

                                    //Insert Data in Order_Status Table
                                    RS_OM_Order_Status mmOrderStatus = new RS_OM_Order_Status();

                                    mmOrderStatus.Plant_ID = plantId;
                                    mmOrderStatus.Shop_ID = shopId;
                                    mmOrderStatus.Line_ID = lineId;
                                    mmOrderStatus.Station_ID = stationId;
                                    mmOrderStatus.Order_No = orderno;
                                    mmOrderStatus.Serial_No = serialNumber;
                                    mmOrderStatus.Action_Status = "Started";
                                    mmOrderStatus.Entry_Date = DateTime.Now.Date;
                                    mmOrderStatus.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    mmOrderStatus.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    mmOrderStatus.Inserted_Date = DateTime.Now;
                                    codePosition = "Position 13";
                                    db.RS_OM_Order_Status.Add(mmOrderStatus);
                                    db.SaveChanges();

                                    orderList = new RS_OM_Order_List();
                                    orderList = db.RS_OM_Order_List.Where(p => p.Serial_No == serialNumber).Single();

                                    //check Condition
                                    int next_lineId;
                                    RS_Lines line = db.RS_Lines.Where(p => p.Line_ID == lineId).Single();
                                    if (line.Is_Conveyor == false)
                                    {
                                        //IF LINE IS NOT CONVEYOR THEN PUT THE STARTED ORDER IN THE STATION AND ALSO IN LINE COMPLETE BUFFER
                                        var route_marriage_station = (from route_marriage in db.RS_Route_Marriage_Station
                                                                      join marriageStn in db.RS_Stations on route_marriage.Marriage_Station_ID equals marriageStn.Station_ID
                                                                      where route_marriage.Sub_Line_ID == lineId && marriageStn.Is_Buffer == true
                                                                      select route_marriage).ToList();

                                        foreach (var item in route_marriage_station)
                                        {
                                            next_lineId = Convert.ToInt32(item.Marriage_Line_ID);

                                            RS_Line_Complete_Buffer line_complete_buffer = new RS_Line_Complete_Buffer();
                                            line_complete_buffer.Plant_ID = plantId;
                                            line_complete_buffer.Shop_ID = shopId;
                                            line_complete_buffer.Line_ID = next_lineId;
                                            line_complete_buffer.Station_ID = item.Marriage_Station_ID;
                                            line_complete_buffer.SerialNo = serialNumber;
                                            line_complete_buffer.Inserted_Time = DateTime.Now;
                                            codePosition = "Position 14";
                                            db.RS_Line_Complete_Buffer.Add(line_complete_buffer);
                                            db.SaveChanges();
                                        }

                                        //Staion_tracking
                                        // process to add the order in station tracking
                                        RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                                        mmStationTrackingObj.Plant_ID = plantId;
                                        mmStationTrackingObj.Shop_ID = shopId;
                                        mmStationTrackingObj.Line_ID = lineId;
                                        mmStationTrackingObj.Station_ID = stationId;
                                        mmStationTrackingObj.SerialNo = serialNumber;
                                        mmStationTrackingObj.Track_SerialNo = serialNumber;
                                        mmStationTrackingObj.Is_Edited = true;
                                        mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                        //db.RS_Station_Tracking.Add(mmStationTrackingObj);
                                        codePosition = "Position 15";
                                        db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        //IF LINE IS CONVEYOR THEN PUT THE STARTED ORDER IN THE STATION ONLY
                                        // process to add the order in station tracking
                                        RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                                        mmStationTrackingObj.Plant_ID = plantId;
                                        mmStationTrackingObj.Shop_ID = shopId;
                                        mmStationTrackingObj.Is_Edited = true;
                                        mmStationTrackingObj.Line_ID = lineId;
                                        mmStationTrackingObj.Station_ID = stationId;
                                        mmStationTrackingObj.SerialNo = serialNumber;
                                        mmStationTrackingObj.Track_SerialNo = serialNumber;
                                        mmStationTrackingObj.Inserted_Date = DateTime.Now;
                                        //db.RS_Station_Tracking.Add(mmStationTrackingObj);
                                        codePosition = "Position 16";
                                        db.Entry(mmStationTrackingObj).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    //to Generate build Sheet
                    var PlannedDate = DateTime.Today;
                    var Total = db.RS_OM_OrderRelease.Where(m => m.Planned_Date == PlannedDate && m.Line_ID == lineId).Select(m => m.Row_ID).Count();
                    var Completed = db.RS_OM_OrderRelease.Where(m => m.Planned_Date == PlannedDate && m.Order_Status == "Started" && m.Line_ID == lineId).Count();
                    var Balance = db.RS_OM_OrderRelease.Where(m => m.Planned_Date == PlannedDate && m.Order_Status == "Release" && m.Line_ID == lineId).Count();
                    var ModelCode = db.RS_OM_OrderRelease.Where(m => m.Row_ID == rowId).Select(m => m.Model_Code).FirstOrDefault();
                    var ModelDesc = db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Model_Description).FirstOrDefault();
                    jsonData.Total = Total;
                    jsonData.Completed = Completed;
                    jsonData.Balance = Balance;
                    jsonData.CurrentModel = ModelDesc;
                    jsonData.CurrentEngine = serialNumber;
                    jsonData.status = true;
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    jsonData.status = false;
                    jsonData.kit_message = "You can start this order between " + modelStartTime + " and " + modelEndTime;
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                jsonData.status = false;
                generalHelper.addControllerException(ex, "OrderStartController", "StartOrder(" + codePosition + ")", ((FDSession)this.Session["FDSession"]).userId);
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult StartSpareOrder(int rowId)
        {
            JSONData jsonData = new JSONData();
            try
            {

                RS_OM_Creation obj_RS_OM_Creation = new RS_OM_Creation();
                obj_RS_OM_Creation = db.RS_OM_Creation.Find(rowId);
                RS_OM_OrderRelease obj_RS_OM_OrderRelease = new RS_OM_OrderRelease();
                obj_RS_OM_OrderRelease = db.RS_OM_OrderRelease.Where(m => m.Plant_OrderNo == obj_RS_OM_Creation.Plant_OrderNo && m.Order_Status == "Release").OrderByDescending(m => m.Order_No).FirstOrDefault();
                if (obj_RS_OM_OrderRelease != null)
                {
                    RS_OM_OrderRelease obj_OrderRelease = new RS_OM_OrderRelease();
                    //obj_OrderRelease.Order_Status = "Started";
                    obj_RS_OM_OrderRelease.Order_Status = "Started";
                    obj_RS_OM_OrderRelease.Updated_Date = DateTime.Now;
                    //obj_OrderRelease.RSN = newRSN;
                    obj_RS_OM_OrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.Entry(obj_RS_OM_OrderRelease).State = EntityState.Modified;
                    db.SaveChanges();
                }

                jsonData.status = true;
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        raise = new InvalidOperationException(Message, raise);
                    }
                }
                throw raise;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                jsonData.status = false;
                generalHelper.addControllerException(ex, "OrderStartController", "StartOrder(Spare Order)", ((FDSession)this.Session["FDSession"]).userId);
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }
        // POST: OrderStart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_OM_OrderRelease RS_OM_OrderRelease)
        {
            if (ModelState.IsValid)
            {
                db.RS_OM_OrderRelease.Add(RS_OM_OrderRelease);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_OrderRelease.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_OM_OrderRelease.Shop_ID);
            return View(RS_OM_OrderRelease);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetGenealogyFields(decimal stationId)
        {
            // List<string> Fields = new List<string>();
            var Fields = (from item in db.RS_OM_Order_Genealogy_Fields
                          where item.Station_ID == stationId
                          select new { item.Display_Name, item.Field_No }).ToList();

            return Json(Fields, JsonRequestBehavior.AllowGet);
        }

        public ActionResult isGenealogyOK(int shopId, string inputStr, string modelCode)
        {
            try
            {
                String[] genealogyData = inputStr.Split('#');
                bool isRecordExists = false;
                String[] returnData = inputStr.Split('#');
                String[] returnMsg = inputStr.Split('#');
                var StationId = ((FDSession)this.Session["FDSession"]).stationId;
                for (int i = 0; i < genealogyData.Length; i++)
                {
                    var IsUnique = db.RS_OM_Order_Genealogy_Fields.Where(m => m.Field_No == i + 1 && m.Station_ID == StationId).Select(m => m.Is_Unique).FirstOrDefault();
                    var IsCompare = db.RS_OM_Order_Genealogy_Fields.Where(m => m.Field_No == i + 1 && m.Station_ID == StationId).Select(m => m.Is_Compare).FirstOrDefault();
                    String serialNo = genealogyData[i].ToUpper().Trim();

                    if (IsUnique == true)
                    {
                        var record = db.RS_OM_Order_List.Where(p => p.Param_1 == serialNo || p.Param_2 == serialNo || p.Param_3 == serialNo || p.Param_4 == serialNo || p.Param_5 == serialNo).ToList();
                        if (record.Count > 0)
                        {
                            returnData[i] = "1";
                            returnMsg[i] = "This value is already exists";
                        }
                        else
                        {
                            returnData[i] = "0";
                            returnMsg[i] = "";
                        }
                    }
                    else if (IsCompare == true)
                    {
                        var CompareValue = db.RS_OM_Genealogy_Compare_Fields.Where(m => m.Model_Code == modelCode && m.Field_No == i + 1 && m.Field_Value.ToLower() == serialNo.ToLower()).ToList();

                        if (CompareValue.Count() > 0)
                        {
                            returnData[i] = "0";
                            returnMsg[i] = "";
                        }
                        else
                        {
                            returnData[i] = "1";
                            returnMsg[i] = "Invalid Value";
                        }
                    }
                    var length = db.RS_OM_Order_Genealogy_Fields.Where(m => m.Field_No == i + 1 && m.Station_ID == StationId).Select(m => m.Field_Length).FirstOrDefault();
                    if (length != null)
                    {
                        if (serialNo.Trim().Length != length)
                        {
                            returnData[i] = "1";
                            returnMsg[i] = "Field length Should be " + length;
                        }
                    }
                    var FirstCharacter = db.RS_OM_Order_Genealogy_Fields.Where(m => m.Field_No == i + 1 && m.Station_ID == StationId).Select(m => m.Is_Check_First_Character).FirstOrDefault();
                    if(FirstCharacter != null)
                    {
                        var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year).Select(m => m.Year_Code).FirstOrDefault();
                        var FirstCharacterValue = serialNo.Substring(0, 1);
                        if(year != FirstCharacterValue)
                        {
                            returnData[i] = "1";
                            returnMsg[i] = "Invalid First Character : " + FirstCharacterValue; 
                        }
                    }
                }
                return Json(new { returnData, returnMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult isOrderReadyToStart(int stationId)
        {
            try
            {
                RS_Station_Tracking mmStationTrackingObj = db.RS_Station_Tracking.Where(p => p.Station_ID == stationId).Single();
                if (String.IsNullOrWhiteSpace(mmStationTrackingObj.Track_SerialNo))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "OrderStartController", "isOrderReadyToStart", ((FDSession)this.Session["FDSession"]).userId);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult isErrorProofing(int stationId)
        {
            // var latestOrder = db.RS_OM_Order_List.Where(m => m.Station_ID == stationId).Select(m => m.Serial_No).OrderByDescending(m => m.Order_ID).FirstOrDefault();
            var isErrorProofing = db.RS_Stations.Where(m => m.Station_ID == stationId).Select(m => m.Is_Error_Proofing).FirstOrDefault();
            if (isErrorProofing == true)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult isWorkstationCompleted(decimal stationId)
        {
            return Json(true, JsonRequestBehavior.AllowGet);
            //var Orderlist = db.RS_OM_Order_List.Where(m => m.Station_ID == stationId).Select(m => m).OrderByDescending(m=>m.Order_ID).FirstOrDefault();
            //if(Orderlist== null)
            //{
            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    var IsWorkstationExist = db.RS_Order_Queue.Where(m => m.Station_ID == stationId && m.Order_No == Orderlist.Order_No).FirstOrDefault();
            //    if (IsWorkstationExist != null)
            //        return Json(true, JsonRequestBehavior.AllowGet);
            //    else
            //        return Json(false, JsonRequestBehavior.AllowGet);
            //}
        }

        public ActionResult isReprint(string serial_number)
        {
            try
            {
                JSONBuildsheetData jsonData = new JSONBuildsheetData();
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
                //find Shop Name
                RS_Shops shop = db.RS_Shops.Find(shopId);
                string shop_name = "";
                if (shop != null)
                {
                    shop_name = shop.Shop_Name;
                }

                RS_OM_Order_List order_list = db.RS_OM_Order_List.Where(p => p.Shop_ID == shopId && p.Serial_No == serial_number).FirstOrDefault();
                if (order_list.Serial_No != null)
                {
                    jsonData.status = true;
                    jsonData.orderNo = order_list.Order_No;

                    var Is_PRN = db.RS_Lines.Where(m => m.Line_ID == lineId).Select(m => m.Is_PRN).FirstOrDefault();
                    if (Is_PRN == true)
                    {
                        // process to add the record in PRN database
                        RS_PRN mmPRNObj = new RS_PRN();
                        mmPRNObj.Plant_ID = plantId;
                        mmPRNObj.Shop_ID = shopId;
                        mmPRNObj.Line_ID = lineId;
                        mmPRNObj.Station_ID = stationId;

                        var modelcode = order_list.Model_Code;
                        var modelID = db.RS_Model_Master.Where(m => m.Model_Code == modelcode).Select(m => m.Model_ID).FirstOrDefault();
                        RS_Model_Master mm_model = db.RS_Model_Master.Find(modelID);
                        var ModelCode = mm_model.Model_Code;
                        //var platformID = mm_model != null ? mm_model.Platform_Id : 0;
                        //var platformName = db.RS_OM_Platform.Where(m => m.Platform_ID == platformID).Select(m => m.Platform_Name).FirstOrDefault();


                        // var PartID = db.RS_Partmaster.Where(m => m.Part_No == modelcode).Select(m => m.PartID).FirstOrDefault();
                        // RS_Partmaster mmPartObj = db.RS_Partmaster.Find(PartID);
                        //var PartGroupID = mmPartObj.Partgroup_ID;
                        // RS_Partgroup mmPartGroupObj = db.RS_Partgroup.Find(PartGroupID);
                        // var partGroupDesc = mmPartGroupObj.Partgrup_Desc;
                        // process to get Quality Ok PRN file
                        //string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/order_start.prn"));

                        // process to add the record in PRN database
                        var printQuantity = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.PrintQuantity).FirstOrDefault();
                        for (int i = 0; i < printQuantity; i++)
                        {
                            //RS_PRN mmPRNObj = new RS_PRN();

                            // process to get Quality Ok PRN file
                            var Barcode2D = ModelCode.Trim().ToUpper() + ":" + serial_number.Trim().ToUpper();
                            var barcodeTest = serial_number.Trim().ToUpper() + " " + ModelCode.Trim().ToUpper();
                            String filename = "";
                            var DefaultPrinter = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.Is_DefaultPrinter).FirstOrDefault();
                            if (DefaultPrinter == true)
                            {
                                filename = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.DefaultPRNFileName).FirstOrDefault();
                            }
                            else
                            {
                                filename = db.RS_PrinterConfig.Where(m => m.Station_ID == stationId).Select(m => m.ExtraPRNFileName).FirstOrDefault();
                            }

                            string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/" + filename));

                            prnFile = prnFile.Replace("012345678910", serial_number.Trim().ToUpper());
                            prnFile = prnFile.Replace("serial_no", serial_number.Trim().ToUpper());

                            prnFile = prnFile.Replace("model_code", ModelCode);
                            prnFile = prnFile.Replace("part_no", ModelCode);
                            prnFile = prnFile.Replace("Production Start", "Production");
                            prnFile = prnFile.Replace("name", "");
                            prnFile = prnFile.Replace("2DTrace", Barcode2D);
                            prnFile = prnFile.Replace("2DTest", barcodeTest);
                            prnFile = prnFile.Replace("Model_Desc", mm_model.Model_Description);

                            //prnFile = prnFile.Replace("re_printed", " ");
                            prnFile = prnFile.Replace("date", Convert.ToString(DateTime.Today.Date.ToString("dd-MM-yyyy")));

                            mmPRNObj.PRN_Text = prnFile;
                            mmPRNObj.Is_OrderStart = true;

                            mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mmPRNObj.Inserted_Date = DateTime.Now;
                            mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            db.RS_PRN.Add(mmPRNObj);
                            db.SaveChanges();
                            //End
                        }
                    }

                    var Is_Buildsheet = db.RS_Lines.Where(m => m.Line_ID == lineId).Select(m => m.Is_Buildsheet).FirstOrDefault();
                    if (Is_Buildsheet == true)
                    {
                        BuildSheetReprint(serial_number);
                    }
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    jsonData.status = false;
                    jsonData.orderNo = "";
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "OrderStartController", "isReprint", ((FDSession)this.Session["FDSession"]).userId);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult loadAllReleasedOrders()
        {
            try
            {
                int Line_Id = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                int Shop_Id = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                IEnumerable<RS_OM_OrderRelease> orderDetail = null;
                RS_Partgroup partgroup;

                partgroup = (from partgroupData in db.RS_Partgroup
                             where partgroupData.Line_ID == Line_Id
                             select partgroupData).FirstOrDefault();

                if (partgroup != null)
                {
                    if (partgroup.Order_Create == true)
                    {
                        orderDetail = (from orderReleaseItem in db.RS_OM_OrderRelease
                                       orderby orderReleaseItem.RSN
                                       where orderReleaseItem.Shop_ID == Shop_Id && orderReleaseItem.Line_ID == Line_Id && orderReleaseItem.Order_Status == "Release"
                                       select orderReleaseItem).Take(20).ToList();
                    }
                    else
                    {
                        String[] omConfiguration = (from configur in db.RS_OM_Configuration
                                                    where configur.Partgroup_ID == partgroup.Partgroup_ID
                                                    select configur.OMconfig_ID).ToArray();

                        if (omConfiguration.Count() > 0)
                        {
                            String[] models = (from mm in db.RS_Model_Master
                                               where omConfiguration.Contains(mm.OMconfig_ID)
                                               select mm.Model_Code).ToArray();


                            orderDetail = (from or in db.RS_OM_OrderRelease.Where(or => models.Contains(or.partno) && or.Order_Status != "Hold")
                                           join ol in db.RS_OM_Order_List.Where(a => a.Line_ID == Line_Id)
                                           on or.Order_No equals ol.Order_No into orol
                                           from ol in orol.DefaultIfEmpty()
                                           where ol.Order_No == null
                                           orderby or.RSN ascending
                                           select or).Take(20).ToList();

                        }
                    }
                }
                return PartialView("PVReleasedOrders", orderDetail);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "OrderStartController", "loadAllReleasedOrders", ((FDSession)this.Session["FDSession"]).userId);
                return null;
            }
        }

        public ActionResult isOrderStartOK()
        {
            try
            {
                int stationId = ((FDSession)this.Session["FDSession"]).stationId;
                RS_OM_Order_List[] omOrderListObj = db.RS_OM_Order_List.Where(p => p.Station_ID == stationId).OrderByDescending(p => p.Inserted_Date).ToArray();
                DateTime dateObj = DateTime.Now;

                TimeSpan timeObj = dateObj - omOrderListObj[0].Inserted_Date;
                if (timeObj > new TimeSpan(0, 2, 0))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult isTableLock(int lineId, String tableName)
        {
            try
            {
                General generalObj = new General();
                bool isTableLock = generalObj.isTableLock(lineId, tableName);
                return Json(isTableLock, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //25 March 

        /*               Action Name               : StartOrder
        *               Description               : Action used to get make details from model master. 
        *               Author, Timestamp         : Sandip,Amol,Jitendra Mahajan
        *               Input parameter           : rowId
        *               Return Type               : ActionResult
        *               Revision                  : 1
       */

        public ActionResult GetTyreMakeDeailsByMoelCode(int rowId)
        {
            int stationId = ((FDSession)this.Session["FDSession"]).stationId;
            var Partno = db.RS_OM_OrderRelease.Where(or => or.Row_ID == rowId).Select(pno => pno.partno).FirstOrDefault();
            var ismakeNull = from ismake in db.RS_Model_Master where ismake.Model_Code == Partno select ismake.Tyre_Make_ID;


            var TyreMakeDetails = from mt in db.RS_Model_Master
                                  where mt.Model_Code == Partno
                                  select new
                                  {
                                      mt.Tyre_Make_Size_Front,
                                      mt.Tyre_Make_Size_Rear,
                                      stationID = stationId,
                                      mt.Tyre_Make_ID,
                                      mt.Model_Code,
                                      Row_ID = rowId
                                  };
            ViewBag.MakeDetails = new SelectList(db.RS_Model_Tyre_Make, "Tyre_Make_ID", "Make_Name", "Tyre_Make_ID");
            return Json(TyreMakeDetails, JsonRequestBehavior.AllowGet);

        }



        //public ActionResult saveOrderAndMakeData(int rowId, int tyreMakeId, string tyreSizeFront, string tyreSizeRear)
        //{
        //    try
        //    {

        //        int stationId = ((FDSession)this.Session["FDSession"]).stationId;
        //        var Partno = db.RS_OM_OrderRelease.Where(or => or.Row_ID == rowId).Select(pno => pno.partno).FirstOrDefault();
        //        MM_Model_Make_Size_Order MM_Make_Order = new MM_Model_Make_Size_Order();
        //        MM_Make_Order.Row_ID = rowId;
        //        MM_Make_Order.Part_NO = Partno;
        //        MM_Make_Order.Tyre_Make_ID = tyreMakeId;
        //        MM_Make_Order.Tyre_Make_Size_front = tyreSizeFront;
        //        MM_Make_Order.Tyre_Make_Size_Rear = tyreSizeRear;

        //        db.MM_Model_Make_Size_Order.Add(MM_Make_Order);
        //        db.SaveChanges();

        //        var saveData = true;
        //        //added by Jitendra Mahajan

        //        return Json(saveData, JsonRequestBehavior.AllowGet);
        //    }
        //    catch
        //    {
        //        var saveData = false;
        //        return Json(saveData, JsonRequestBehavior.AllowGet);
        //    }



        //}

        public ActionResult IsKitAvailabele(string kitting_Barcode)
        {
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);
            if (db.RS_Kitt_Barcode_Master.Where(kit => kit.Barcode_String == kitting_Barcode && kit.Is_Consumed == false && kit.Station_ID == stationId).Count() == 0)
            {
                return Json(new { msg = "This kitt: " + kitting_Barcode + " is not available." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { msg = true }, JsonRequestBehavior.AllowGet);
            }

        }

        //Added by Ketan
        public ActionResult GetPartConfiguredInBOMItemAndKittingZone(string ModelCode)
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
            int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

            var st = (from kzp in db.RS_Kitting_Zone_Part
                      join boi in db.RS_BOM_Item.Where(a => a.Model_Code == ModelCode)
                      on new { Code = kzp.Model_Code, PartNo = kzp.Part_No } equals
                        new { Code = boi.Model_Code, PartNo = boi.Part_No }
                        //on boi.Part_No equals kzp.Part_No
                        //from kz in orol.DefaultIfEmpty()
                      where kzp.Model_Code == ModelCode && kzp.Station_ID == stationId
                      select new
                      {
                          Part_No = kzp.Part_No,
                          Quantity = boi.Qty,

                      }).Distinct().ToList();

            return Json(st, JsonRequestBehavior.AllowGet);
        }
        public class KittingOrder_PartsDetails
        {
            public string Part_No { get; set; }
            public int Quantity { get; set; }

        }
        public ActionResult SaveOrderPartsDetails(List<string> dataModel, string barcodeString, string Model_Code, string OrderID)
        {
            JSONData JSONDataStatus = new JSONData();
            try
            {
                if (barcodeString != null && OrderID != null && Model_Code != null)
                {
                    int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                    int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                    int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
                    int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);


                    List<KittingOrder_PartsDetails> objJSONData = (List<KittingOrder_PartsDetails>)JsonConvert.DeserializeObject(dataModel[0], typeof(List<KittingOrder_PartsDetails>));

                    //RS_OM_Order_List Serial_No = db.RS_OM_Order_List.Where(p => p.Model_Code == Model_Code && p.Order_No == OrderID && p.Order_Status== "Started").FirstOrDefault();
                    string serialNo = null;

                    if (TempData["SerialNo"] != null)
                    {
                        serialNo = TempData["SerialNo"].ToString();
                    }
                    else
                    {

                    }
                    RS_OM_KittingOrder_Parts objKittingOrderParts = new RS_OM_KittingOrder_Parts();
                    foreach (KittingOrder_PartsDetails item in objJSONData)
                    {
                        objKittingOrderParts.Part_No = item.Part_No;
                        objKittingOrderParts.Quantity = item.Quantity;
                        objKittingOrderParts.Plant_ID = plantId;
                        objKittingOrderParts.Shop_ID = shopId;
                        objKittingOrderParts.Line_ID = lineId;
                        objKittingOrderParts.Station_ID = stationId;
                        objKittingOrderParts.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        objKittingOrderParts.Inserted_Date = DateTime.Now;
                        objKittingOrderParts.Kitting_Barcode_Number = barcodeString;
                        objKittingOrderParts.Model_Code = Model_Code;
                        objKittingOrderParts.Order_ID = OrderID;
                        objKittingOrderParts.Serial_No = serialNo;

                        db.RS_OM_KittingOrder_Parts.Add(objKittingOrderParts);
                        db.SaveChanges();
                    }


                }
                else
                {

                }
                JSONDataStatus.status = true;
                return Json(JSONDataStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                JSONDataStatus.status = false;
                generalHelper.addControllerException(ex, "OrderStartController", "SaveOrderPartsDetails", ((FDSession)this.Session["FDSession"]).userId);
                return Json(JSONDataStatus, JsonRequestBehavior.AllowGet);
            }
        }

        public void SaveKittingorderMaster(int rowId, decimal plantId, decimal shopId, decimal lineId, decimal stationId)
        {
            RS_OM_OrderRelease objRS_OM_OrderRelease = db.RS_OM_OrderRelease.Find(rowId);
            RS_OM_Kitting_Order_list objRS_OM_Kitting_Order_list = new RS_OM_Kitting_Order_list();
            objRS_OM_Kitting_Order_list.Order_ID = objRS_OM_OrderRelease.Row_ID;
            objRS_OM_Kitting_Order_list.Line_ID = lineId;
            objRS_OM_Kitting_Order_list.Model_Code = objRS_OM_OrderRelease.Model_Code;
            objRS_OM_Kitting_Order_list.Order_No = objRS_OM_OrderRelease.Order_No;
            objRS_OM_Kitting_Order_list.Order_Status = "Started";
            objRS_OM_Kitting_Order_list.Parent_Model_Code = objRS_OM_OrderRelease.partno;
            objRS_OM_Kitting_Order_list.Planned_Date = System.DateTime.Now;
            objRS_OM_Kitting_Order_list.Plant_ID = plantId;
            objRS_OM_Kitting_Order_list.Shop_ID = shopId;
            objRS_OM_Kitting_Order_list.Station_ID = stationId;
            objRS_OM_Kitting_Order_list.Updated_Date = DateTime.Now;
            objRS_OM_Kitting_Order_list.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
            db.RS_OM_Kitting_Order_list.Add(objRS_OM_Kitting_Order_list);
            db.SaveChanges();
        }

        public bool checkIsModelStart(int rowId, int PlantId)
        {
            try
            {
                string m_code = db.RS_OM_OrderRelease.Where(r => r.Row_ID == rowId).Select(r => r.Model_Code).FirstOrDefault();
                var time = (from tbmc in db.RS_OM_TimeBased_ModelConfig
                            join mm in db.RS_Model_Master on tbmc.Model_ID equals mm.Model_ID
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

        public ActionResult IsValidTokenNumber(string TokenNumber)
        {
            if (TokenNumber != null)
            {
                if (db.RS_Employee.Any(m => m.Employee_No == TokenNumber))
                {
                    var validToken = (from emp in db.RS_Employee
                                      join Category in db.RS_AM_Category
                                      on emp.Category_ID equals Category.Category_ID
                                      where emp.Employee_No == TokenNumber && Category.Category_Name == "Line Officer"
                                      select new { emp.Employee_No });

                    if (validToken.Count() > 0)
                    {
                        return Json(new { status = true, msg = "Login Successfully!..." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Please Enter Line Officer Token Number!... " }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, msg = "Please Enter Valid Token Number!... " }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { status = false, msg = "Please Enter Token Number" }, JsonRequestBehavior.AllowGet);
            }

        }

        //public ActionResult SpareOrderList(int? orderListShopId, int? orderListLineId)
        //{
        //    int plantId = ((FDSession)this.Session["FDSession"]).plantId;
        //    int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
        //    int lineId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).lineId);
        //    int stationId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).stationId);

        //    IEnumerable<RS_OM_Creation> SpareOrderDetails = null;
        //    IEnumerable<RS_OM_Creation> SpareOrderList = null;
        //    RS_Partgroup partgroup;

        //    partgroup = (from partgroupData in db.RS_Partgroup
        //                 where partgroupData.Line_ID == lineId && partgroupData.Plant_ID == plantId
        //                 select partgroupData
        //              ).FirstOrDefault();
        //    ViewBag.PartGroup = partgroup;

        //    //added by ketan to display orders as ppc plan display on daily bassis.
        //    //Date 14-jul-17

        //    if (partgroup != null)
        //    {
        //        //kittingorderDetail = (from ppcplannedOrder in db.RS_OM_Planned_Orders
        //        //                      join kittingOrder in db.RS_OM_Kitting_Order_list
        //        //                      on ppcplannedOrder.Order_No equals kittingOrder.Order_No
        //        //                      into order
        //        //                      from kittingOrder in order.DefaultIfEmpty()
        //        //                      orderby ppcplannedOrder.Planned_Date ascending
        //        //                      where ppcplannedOrder.Shop_ID == shopId &&
        //        //                      ppcplannedOrder.Order_Status == "Release" && ppcplannedOrder.Plant_ID == plantId &&
        //        //                      !ppcplannedOrder.Order_No.Contains(kittingOrderNo) && kittingOrder.Station_ID == stationId
        //        //                      select ppcplannedOrder).Take(20).ToList();

        //        SpareOrderDetails = (from omcreation in db.RS_OM_Creation//.GroupBy(m=>m.Plant_OrderNo).Select(m=>m).FirstOrDefault()
        //                             join release in db.RS_OM_OrderRelease
        //                             on omcreation.Plant_OrderNo equals release.Plant_OrderNo
        //                             join orderType in db.RS_OM_Order_Type on release.Order_Type equals orderType.Order_Type_Name
        //                             where orderType.Is_Spare == true && omcreation.Plant_ID == plantId &&
        //                            omcreation.Shop_ID == shopId && omcreation.Line_ID == lineId && omcreation.Planned_Date.Value.Year == DateTime.Now.Year && omcreation.Planned_Date.Value.Month == DateTime.Now.Month && omcreation.Planned_Date.Value.Day == DateTime.Now.Day &&
        //                            //DbFunctions.TruncateTime(omcreation.Planned_Date) ==DbFunctions.TruncateTime(DateTime.Now) && 
        //                            release.Order_Status == "Release"
        //                             //group release by release.Plant_OrderNo into spareRelease
        //                             select omcreation).ToList();
        //        //added by mukesh

        //        string stationName = ((FDSession)this.Session["FDSession"]).stationName;
        //        //var platformId = db.RS_OM_Platform.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Line_ID == lineId && m.Platform_Name == stationName).Select(p => p.Platform_ID).FirstOrDefault();

        //        ///added platfrom_id condition in below query
        //        if (SpareOrderDetails.Any(m => m.Planned_Date.Value.Year == DateTime.Now.Year && m.Planned_Date.Value.Month == DateTime.Now.Month && m.Planned_Date.Value.Day == DateTime.Now.Day))
        //        {
        //            SpareOrderList = db.RS_OM_Creation.Where(m => m.Planned_Date.Value.Year == DateTime.Now.Year && m.Planned_Date.Value.Month == DateTime.Now.Month && m.Planned_Date.Value.Day == DateTime.Now.Day
        //                                                && m.Order_Type == "Spare" && m.Plant_ID == plantId && m.Shop_ID == shopId /*&& m.Platform_Id == platformId*/).Take(5).ToList().OrderByDescending(m => m.Plant_OrderNo);

        //        }
        //        Session["OrderCountExceptSpare"] = 0;
        //        if (SpareOrderDetails != null)
        //        {
        //            ViewBag.Spare_Count = SpareOrderDetails.Count();
        //            Session["OrderCountExceptSpare"] = Convert.ToInt32(ViewBag.allordercount);
        //        }
        //        //SpareOrderDetails = SpareOrderDetails.GroupBy(m=>m.Plant_OrderNo);
        //    }
        //    //var noSelectedDefect = from defect in db.MM_Quality_Checklist
        //    //                       where defect.Shop_ID == shopId && !(from defectStation in db.MM_Quality_Station_Checklist where defectStation.Station_ID == stationId select defectStation.Checklist_ID).Contains(defect.Checklist_ID) &&
        //    //                       defect.Plant_ID == plantId && defect.Attribute_ID == attributeId
        //    //                       select new
        //    //                       {
        //    //                           Checklist_ID = defect.Checklist_ID,
        //    //                           Checklist_Name = defect.Checklist_Name
        //    //                       };
        //    return PartialView("ShowSpareOrderList", SpareOrderList);
        //}
        public ActionResult IsAuthorizeBySupervisor(string orderType)
        {
            try
            {

                if (orderType != null)
                {


                    if (db.RS_OM_Order_Type.Any(m => m.Order_Type_Name == orderType))
                    {
                        bool IsAutoeizeBySupervisor = db.RS_OM_Order_Type.Where(m => m.Order_Type_Name.Trim().ToLower() == orderType.Trim().ToLower()).Select(m => m.Is_Authorize_By_Supervisor).FirstOrDefault();

                        if (IsAutoeizeBySupervisor)
                        {
                            return Json(new { status = true, msg = "Login Successfully!..." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Please Enter Line Officer Token Number!... " }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Please Enter Valid Token Number!... " }, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    return Json(new { status = false, msg = "Please Enter Token Number" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exp)
            {

                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "OrderStartController", "isAuthorizeBySupervisor(" + orderType + ")", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
                return Json(new { status = false, msg = "Error occured while checking Authorization" }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult BuildSheetPrint(string Serial_No)
        {
            var Station_ID = ((FDSession)this.Session["FDSession"]).stationId;
            string strAttachment = string.Empty;
            string fileName = string.Empty;
            var isPrint = true;
            try
            {
                var model = db.RS_OM_Order_List.Where(m => m.Serial_No == Serial_No).FirstOrDefault();
                var OrderNo = model.Order_No;
                //string rdlcfileName = Server.MapPath("~/Report/" + "Engine_BuildSheet.rdlc");
                var buildsheetfileName = db.RS_Buildsheet_Printer_Config.Where(m => m.Station_ID == Station_ID).Select(m => m.File_Name).FirstOrDefault();
                //string rdlcfileName = Server.MapPath("~/Report/" + "NEW_BIW.rdlc");
                string rdlcfileName = Server.MapPath("~/Report/" + buildsheetfileName);
                LocalReport report = new LocalReport();
                
                
                report.ReportPath = rdlcfileName;

                report.EnableExternalImages = true;
                string path = "~/Barcode/Barcode_" + Serial_No + ".Png";
                string imagePath = new Uri(Server.MapPath(path)).AbsoluteUri;
                ReportParameter parameter = new ReportParameter("BarcodeImagePath", imagePath);
                report.SetParameters(parameter);
                report.Refresh();

                var LineID = db.RS_OM_Order_List.Where(m => m.Serial_No == Serial_No).Select(m => m.Line_ID).FirstOrDefault();
                isPrint = Convert.ToBoolean(db.RS_Lines.Where(m => m.Line_ID == LineID).Select(m => m.Is_Buildsheet).FirstOrDefault());
                int LineId = ((FDSession)this.Session["FDSession"]).lineId;
                //var platForm ;
                string imagePath_IPMSSrN = "";
                var platForm = db.RS_OM_Platform.Where(m => m.Line_ID == LineId).FirstOrDefault();



                //imagePath_IPMSSrN = new Uri(Server.MapPath("~/Barcode/BarcodeIPMSSrNo.Png")).AbsoluteUri;
                //ReportParameter parameterIPMSSrNo = new ReportParameter("BarcodeIPMSSrNoImagePath", imagePath_IPMSSrN);
                //report.SetParameters(parameterIPMSSrNo);
                //report.Refresh();




                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);

                //added by mukesh for parsing json stored model master attribution parameters column 
                var modelobj = db.RS_Model_Master.Where(m => m.Model_Code == model.Model_Code && m.Plant_ID == plantId).FirstOrDefault();

                string ModelImagePath = new Uri(Server.MapPath("~/Content/ModelMaster/" + modelobj.Image_Name)).AbsoluteUri;
                //var ModelID = modelobj.Model_ID;
                //var bytes = db.RS_Model_Master_Image.Where(m => m.Model_ID == ModelID).Select(m => m.Image_Content).FirstOrDefault();
                //byte[] ModelImagePath = bytes;
                //ReportParameter parameter1 = new ReportParameter("ModelImagePath", ModelImagePath);
                //report.SetParameters(parameter1);
                //report.Refresh();
                //string path = "~/Barcode/Barcode_" + Serial_No + ".Png";
                string modelImagePath = new Uri(Server.MapPath(path)).AbsoluteUri;
                ReportParameter parameter1 = new ReportParameter("BarcodeImagePath", modelImagePath);
                report.SetParameters(parameter1);
                report.Refresh();


                var S201_VIN3 = (from tbmc in db.RS_OM_Order_List
                                 join mm in db.RS_OM_OrderRelease on tbmc.Order_No equals mm.Order_No
                                 where tbmc.Serial_No == model.Serial_No
                                 select new { tbmc.VIN_Number }).ToList();//ASSIGN VIN NO HERE FOR S201

                if (S201_VIN3.Count > 0)
                {
                    foreach (var item in S201_VIN3)
                    {
                        // S201_VIN_Barcode = item.VIN_Number;//ASSIGN VIN NO HERE ALSO FOR S201;

                        //string S201_VIN_Barcode = new Uri(Server.MapPath("~/Barcode/VIN.Png" + item.VIN_Number)).AbsoluteUri;
                        //ReportParameter parameter2 = new ReportParameter("S201_VIN_Barcode1", S201_VIN_Barcode);
                        //report.SetParameters(parameter2);
                        //report.Refresh();

                    }
                }


                ReportDataSource rds = new ReportDataSource();
                rds.DataSourceId = "DataSet1";
                rds.Name = "DataSet1";//This refers to the dataset name in the RDLC file
                rds.Value = GetBuildSheetData(model.Model_Code, model.Serial_No, OrderNo,false);
                report.DataSources.Add(rds);
                ////giving printer print
                //Printer ps = new Printer();
                //ps.Export(report);


                /////


                Byte[] mybytes = report.Render("pdf");
                DateTime dTime = DateTime.Now;
                fileName = "buildSheet" + dTime.ToString("ddMMyyyy") + "_"+dTime.Hour+"_"+dTime.Minute+"_"+dTime.Second+"_"+dTime.Millisecond +  "_" + Serial_No +  ".pdf";
                //file name to be created   
                //string strPDFFileName = string.Format("Tact_Sheet_" + dTime.ToString("ddMMyyyy") + "_"+ dTime.TimeOfDay+ "" + ".pdf");
                string strPDFFileName = string.Format(fileName);

                //---------------------------------------New Code--------------------------------------------//
             
                {
                    strAttachment = Server.MapPath("~/App_Data/" + strPDFFileName);

                    using (FileStream fs = new FileStream(strAttachment, FileMode.Create))
                    {
                        fs.Write(mybytes, 0, mybytes.Length);
                    }
                }
                //---------------------------------------------------------------------------------------------//
                RS_OM_BuildSheet_Print RS_OM_BuildSheet_Print = new RS_OM_BuildSheet_Print
                {
                    PDF_Bytes = mybytes,
                    Station_ID = Station_ID,
                    PDF_FileName = strPDFFileName,
                    Serial_No = model.Serial_No,
                    Inserted_Date = DateTime.Now,
                    Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId
                };
                db.RS_OM_BuildSheet_Print.Add(RS_OM_BuildSheet_Print);
                db.SaveChanges();
            }
            catch (Exception exp)
            {

                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "OrderStartController", "BuildsheetPrint(" + Serial_No + ") FileName:" + fileName, Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
            }
            return Json(new { status = true, msg = strAttachment, fileName = fileName, printStatus = isPrint }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BuildSheetReprint(string Serial_No)
        {
            var Station_ID = ((FDSession)this.Session["FDSession"]).stationId;
            var model = db.RS_OM_Order_List.Where(m => m.Serial_No == Serial_No).FirstOrDefault();
            var Order_No = model.Order_No;
            var buildsheetfileName = db.RS_Buildsheet_Printer_Config.Where(m => m.Station_ID == Station_ID).Select(m => m.File_Name).FirstOrDefault();
            string rdlcfileName = Server.MapPath("~/Report/" + buildsheetfileName);
            LocalReport report = new LocalReport();
            report.ReportPath = rdlcfileName;

            report.EnableExternalImages = true;
            string path = "~/Barcode/Barcode_" + Serial_No + ".Png";
            string imagePath = new Uri(Server.MapPath(path)).AbsoluteUri;
            ReportParameter parameter = new ReportParameter("BarcodeImagePath", imagePath);
            report.SetParameters(parameter);
            report.Refresh();

            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);

            //added by mukesh for parsing json stored model master attribution parameters column 
            var modelobj = db.RS_Model_Master.Where(m => m.Model_Code == model.Model_Code && m.Plant_ID == plantId && m.Shop_ID == shopId).FirstOrDefault();

         

            ReportDataSource rds = new ReportDataSource();
            rds.DataSourceId = "DataSet1";
            rds.Name = "DataSet1";//This refers to the dataset name in the RDLC file
            rds.Value = GetBuildSheetData(model.Model_Code, model.Serial_No, Order_No,true);
            report.DataSources.Add(rds);
            Byte[] mybytes = report.Render("pdf");
            DateTime dTime = DateTime.Now;
            string fileName = "buildSheet" + dTime.ToString("ddMMyyyy") + "_" + dTime.Hour + "_" + dTime.Minute + "_" + dTime.Second + "_" + dTime.Millisecond + "_" + Serial_No + ".pdf";
            //file name to be created   
            //string strPDFFileName = string.Format("Tact_Sheet_" + dTime.ToString("ddMMyyyy") + "_"+ dTime.TimeOfDay+ "" + ".pdf");
            string strPDFFileName = string.Format(fileName);

            //Byte[] mybytes = report.Render("PDF"); for exporting to PDF
            string strAttachment = Server.MapPath("~/App_Data/" + strPDFFileName);

            using (FileStream fs = new FileStream(strAttachment, FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }

            RS_OM_BuildSheet_Print RS_OM_BuildSheet_Print = new RS_OM_BuildSheet_Print
            {
                PDF_Bytes = mybytes,
                Station_ID = Station_ID,
                PDF_FileName = strPDFFileName,
                Serial_No = model.Serial_No,
                Inserted_Date = DateTime.Now,
                Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId,
                //Updated_Date = DateTime.Now
            };
            db.RS_OM_BuildSheet_Print.Add(RS_OM_BuildSheet_Print);
            db.SaveChanges();
            //string fullpath = Server.MapPath("~/Content/AvixFiles/" + documentPath);
            //string filename = HttpUtility.UrlEncode(Path.GetFileName(strAttachment));

            ////Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename);

            ////return File(fullpath, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

            //Response.AppendHeader("Content-Disposition", "inline; filename=" + filename);
            //return File(strAttachment, MimeMapping.GetMimeMapping(filename));

            return Json(new { status = true, msg = strAttachment, fileName = fileName }, JsonRequestBehavior.AllowGet);
        }
        public class BuildSheet
        {
            public string modelcode { get; set; }
            public string bodysrno { get; set; }
            public string Ipms_Serial_No { get; set; }
            public string PlatformTitle { get; set; }
            public string orderno { get; set; }
            public string color { get; set; }
            public string country { get; set; }

            public string Model_Description { get; set; }
            public string variantcode { get; set; }
            public string fuel { get; set; }
            public string AttributionParameter { get; set; }
            public string config { get; set; }
            public string wheeldrive { get; set; }
            public string ImagePath { get; set; }
            public byte[] ModelImagePath { get; set; }
            public string vserious { get; set; }
            public string BOT { get; set; }
            public string Blackout { get; set; }
            public string StyleCode { get; set; }
            public string Serial_No_Bot { get; set; }
            public string qrCodeImage { get; set; }
            public string S201_VIN { get; set; }
            public string S201_VIN_Barcode { get; set; }
            public string fdata { get; set; }
            public string sdata { get; set; }
            public string ldata { get; set; }
            public string long_desc { get; set; }
            public string info_desc { get; set; }
            public string date { get; set; }
            public string bodyno { get; set; }
            public string VIN_BarCode { get; set; }
            public string VIN { get; set; }
            public string BIW_part { get; set; }
            public string partno { get; set; }
        }
        public List<BuildSheet> GetBuildSheetData(string modelCode, string serialNo, string orderNo, bool Reprint)
        {
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
            List<BuildSheet> objBuildsheet = new List<BuildSheet>();
            try
            {
                var Country_ID = db.RS_OM_OrderRelease.Where(m => m.Model_Code == modelCode && m.Order_No == orderNo).Select(m => m.Country_ID).FirstOrDefault();
                var CountryName = Country_ID != 0 ? db.RS_Country.Where(m => m.Country_ID == Country_ID).Select(m => m.Country_Name).FirstOrDefault() : "India";
                var colorbatch = (from ol in db.RS_OM_Order_List
                                  join or in db.RS_OM_OrderRelease
                                  on ol.Order_No equals or.Order_No
                                  join cc in db.RS_Colour
                                  on or.Model_Color equals cc.Colour_ID
                                  where ol.Model_Code == modelCode
                                  select new { cc.Colour_Batch }).FirstOrDefault();
                //added by mukesh for parsing json stored model master attribution parameters column 
                var model = db.RS_Model_Master.Where(m => m.Model_Code == modelCode && m.Plant_ID == plantId && m.Shop_ID == shopId).FirstOrDefault();
                var fuel = "";
                var seat = "";
                var attrParameter = "";
                var drive = "";
                var bot = "";
                var blackout = "";
                var Series = "";
                var serial_No_Bot = "";
                var PlatformTitle = "";
                var S201_VIN = "";
                var S201_VIN_Barcode = "";

                //
                var ipms_SerialNo = "";

                S201_VIN = db.RS_OM_Order_List.Where(m => m.Serial_No == serialNo).Select(m => m.VIN_Number).FirstOrDefault();
                S201_VIN_Barcode = S201_VIN;
            
                var platformID = model.Platform_Id;
                var platformName = db.RS_OM_Platform.Where(m => m.Platform_ID == platformID).Select(m => m.Platform_Name).FirstOrDefault();
                if (platformName != null || platformName != "")
                {
                    PlatformTitle = platformName;
                }
                var bodyno = "*" + serialNo + "*";
                var VIN_BarCode = "*" + S201_VIN + "*";
                var scanModelcode = "*" + modelCode + "*";
                List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
                for (int i = 0; i < attributionParameters.Count; i++)
                {
                    AttributionParameters attributionParameter = attributionParameters[i];
                    try
                    {
                        Convert.ToInt32(attributionParameter.Value);
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                    if (attributionParameter.label.Equals("Fuel", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int attrId = Convert.ToInt32(attributionParameter.Value);
                        fuel = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                        //       attributionParameter.Value;
                    }
                    else if (attributionParameter.label.Equals("Seat", StringComparison.InvariantCultureIgnoreCase))
                    {

                        int attrId = Convert.ToInt32(attributionParameter.Value);
                        seat = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                    }
                    else if (attributionParameter.label.Equals("Wheel Drive", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int attrId = Convert.ToInt32(attributionParameter.Value);
                        attrParameter = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                    }
                    else if (attributionParameter.label.Equals("Vehicle Drive", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int attrId = Convert.ToInt32(attributionParameter.Value);
                        drive = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                    }
                    else if (attributionParameter.label.Equals("Vehicle Series", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int attrId = Convert.ToInt32(attributionParameter.Value);
                        Series = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                        bot = db.RS_Vehicle_Series.Where(m => m.Plant_ID == model.Plant_ID && m.Shop_ID == model.Shop_ID && m.Attribute_Name.ToLower() == Series.ToLower()).OrderByDescending(m => m.Inserted_Date).Select(m => m.BOT).FirstOrDefault();
                        blackout = db.RS_Vehicle_Series.Where(m => m.Plant_ID == model.Plant_ID && m.Shop_ID == model.Shop_ID && m.Attribute_Name.ToLower() == Series.ToLower()).OrderByDescending(m => m.Inserted_Date).Select(m => m.Blackout).FirstOrDefault();
                    }
                }
                //VIN NUMBER


                //for generating qr code
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(S201_VIN + " " + serialNo, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImg = qrCode.GetGraphic(20);
                string path = "";
                try
                {

                    //path = Server.MapPath("~/Barcode/Barcode.Png");
                    path = Server.MapPath("~/Barcode/" + "Barcode_"+serialNo+".Png");

                    //using (FileStream fs = new FileStream(strAttachment, FileMode.Create))
                    //{
                    //    fs.Write(mybytes, 0, mybytes.Length);
                    //}

                    //path = @"~/Barcode/Barcode_" + serialNo + ".Png";
                    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

                    //FileStream fs = System.IO.File.Create(path);
                    fs.Close(); 
                }
                catch(Exception ex)
                {

                }
                // File.Create
                //string imagePath = Server.MapPath("~/Barcode/Barcode.Png");
                //string imagePath = Server.MapPath(path);
                string imagePath = path;
                qrCodeImg.Save(imagePath, ImageFormat.Png);

              
                //string modelImagePath = Server.MapPath("~/Content/ModelMaster/" + model.Image_Name);
                var Model_ID = model.Model_ID;
                var bytedata = db.RS_Model_Master_Image.Where(m => m.Model_ID == Model_ID).Select(m => m.Image_Content).FirstOrDefault();
                byte[] modelImagePath = bytedata;
                

                var BIWPartNo = db.RS_Model_Master.Where(m => m.Model_ID == Model_ID).Select(m => m.BIW_Part_No).FirstOrDefault();
                var Long_Desc = db.RS_BIW_Part_Master.Where(m => m.Variant_Code == BIWPartNo).Select(m => m.LONG_DESC).FirstOrDefault();

                serial_No_Bot = serialNo.Insert(serialNo.Length - 2, " ");
                if (bot != null)
                {
                    serial_No_Bot = serial_No_Bot + " " + bot;
                }

                //////
                ////----vms test ipms barcode--->
                var orderReleaseObj = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Order_No == orderNo).FirstOrDefault();
                var ipmdColorCode = orderReleaseObj.Model_Color;
                //var ipmdColorCode = db.RS_Colour.Where(m => m.Plant_ID == shopId && m.Colour_ID == orderReleaseObj.Model_Color).Select(m => m.IPMS_Color_Code).FirstOrDefault();

                var ipmsSerialNo = "";

                if (PlatformTitle == "XYLO")
                {
                    ipmsSerialNo = serialNo.Substring(1, 7) + ipmdColorCode.Replace(" ", String.Empty) + model.Style_Code;

                    ipms_SerialNo = ipmsSerialNo.Trim();
                }
                else
                {
                    ipmsSerialNo = serialNo.Substring(1, 7) + "  " + ipmdColorCode;

                    ipms_SerialNo = ipmsSerialNo.Trim();

                    if (bot != null)
                    {
                        ipms_SerialNo = ipms_SerialNo + " " + bot;
                    }

                }

                var fdata = serialNo.Substring(0, 3).Trim();
                var sdata = serialNo.Substring(3, 5).Trim();
                var ldata = serialNo.Substring(8, 2).Trim();
             

                string imagePathIPMSSrNo = "";
                if (PlatformTitle == "test")
                {
                    imagePathIPMSSrNo = Server.MapPath("~/Barcode/BarcodeImage/test.Png");
                }
                else
                {
                    imagePathIPMSSrNo = Server.MapPath("~/Barcode/BarcodeImage/test.Png");
                }
            


                //for generating qr code VIN

                var S201_VIN3 = (from tbmc in db.RS_OM_Order_List
                                 join mm in db.RS_OM_OrderRelease on tbmc.Order_No equals mm.Order_No
                                 where tbmc.Serial_No == serialNo
                                 select new { tbmc.VIN_Number }).ToList();//ASSIGN VIN NO HERE FOR S201

                if (S201_VIN3.Count > 0)
                {
                    foreach (var item in S201_VIN3)
                    {
                        // S201_VIN_Barcode = item.VIN_Number;//ASSIGN VIN NO HERE ALSO FOR S201;

                        //QRCodeGenerator qrGeneratorVIN = new QRCodeGenerator();
                        //QRCodeData qrCodeDataIvin = qrGeneratorVIN.CreateQrCode(item.VIN_Number, QRCodeGenerator.ECCLevel.Q);
                        //QRCode qrCodevin = new QRCode(qrCodeDataIvin);
                        //Bitmap qrCodeImgvin = qrCodevin.GetGraphic(20);
                        //string imagePathvin = Server.MapPath("~/Barcode/VIN.Png");
                        //qrCodeImgvin.Save(imagePathvin, ImageFormat.Png);

                    }
                }


                ////////
                var date = DateTime.Now.ToString("dd-MMM-yyyy HH:mm");
                var info_desc = "";
                if(Reprint == true)
                {
                    info_desc = "Duplicate Buildsheet";
                }
                BuildSheet b1 = new BuildSheet()
                {

                    modelcode = modelCode,
                    Ipms_Serial_No = ipms_SerialNo,
                    bodysrno = serialNo,
                    orderno = orderNo,
                    color = colorbatch.Colour_Batch,
                    Model_Description = model.Model_Description,
                    variantcode = model.BIW_Part_No,// model.BIW_Part_No,
                    fuel = fuel,
                    AttributionParameter = model.RS_Model_Attribute_Master.Attribution,
                    config = seat,
                    wheeldrive = attrParameter,
                    ImagePath = imagePath,
                    country = CountryName,
                    ModelImagePath = modelImagePath,
                    vserious = Series,
                    BOT = bot,
                    Blackout = blackout,
                    Serial_No_Bot = serial_No_Bot,
                    StyleCode = "",//model.Style_Code,
                    PlatformTitle = PlatformTitle,
                    S201_VIN = S201_VIN,
                    S201_VIN_Barcode = S201_VIN_Barcode,
                    fdata = fdata,
                    sdata = sdata,
                ldata = ldata,
                long_desc =  Long_Desc,
                date = date ,
                bodyno= serialNo,
                VIN_BarCode= S201_VIN ,
                VIN = S201_VIN,
                BIW_part= model.BIW_Part_No,
                partno= modelCode  ,

                qrCodeImage = imagePath ,
                info_desc = info_desc

                };
                objBuildsheet.Add(b1);

            }
            catch (Exception exp)
            {

                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "OrderStartController", "GetBuildSheetData(" + modelCode + ", " + serialNo + ", " + orderNo + ")", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
            }
            return objBuildsheet;
        }

        //sharad
        public ActionResult GetVariant( string CrankCaseNumber)
        {
            
            
            bool status = false;
            var valmsg = "";
            if (CrankCaseNumber != "" && CrankCaseNumber.Length >= 5)
            {
                var CompareCharacter = CrankCaseNumber.Substring(4);
                var ValidateCharacter = CompareCharacter.Remove(1);
                var Variant = "";
               
                var latestOrder = db.RS_OM_Order_List.Where(m => m.Station_ID == 39).Select(m => m).OrderByDescending(m => m.Order_ID).FirstOrDefault();
                var serialNumber = latestOrder.Serial_No;
                var ModelCode = latestOrder.Model_Code;
                var stationID = latestOrder.Station_ID;
                var LineID = latestOrder.Line_ID;
                var ShopID = latestOrder.Shop_ID;
                var PlantID = latestOrder.Plant_ID;
                var scanvalue = db.RS_CrankCase_Scan_Data.Where(m => m.Scan_Value == CrankCaseNumber && m.Status == true).ToList();
                if (scanvalue.Count > 0)
                {
                    valmsg = "Crankcase block Already Scanned";
                }
                else
                {
                    var ModelId = db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Model_ID).FirstOrDefault();

                    RS_Model_Master model = db.RS_Model_Master.Find(ModelId);
                    List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));

                    for (int i = 0; i < attributionParameters.Count; i++)
                    {
                        AttributionParameters attributionParameter = attributionParameters[i];

                        if (attributionParameter.label.Equals("Variant", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int attrId = Convert.ToInt32(attributionParameter.Value);
                            Variant = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                        }
                    }

                    if (string.Equals(ValidateCharacter, Variant))
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }

                    var EngineExist = db.RS_CrankCase_Scan_Data.Where(m => m.Serial_No == serialNumber && m.Status == true).ToList();
                    if (EngineExist.Count == 0)
                    {
                        try
                        {
                            RS_CrankCase_Scan_Data scanData = new RS_CrankCase_Scan_Data();
                            scanData.Serial_No = serialNumber;
                            scanData.Scan_Value = CrankCaseNumber;
                            scanData.Status = status;

                            scanData.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            scanData.Inserted_Date = DateTime.Now;
                            scanData.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            db.RS_CrankCase_Scan_Data.Add(scanData);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            status = false;
                            valmsg = "In Exception";
                        }
                    }
                    else
                    {
                        status = false;
                        valmsg = "Crankcase number already scanned for current Engine.";
                    }

                    if (status == true && EngineExist.Count == 0)
                    {
                        //Send Engine Number to D Drive
                        //var filename1 = "EngineDetails.txt";
                        //string path = @"E:\" + filename1;

                        //using (StreamWriter sw = System.IO.File.AppendText(path))
                        //{
                        //    sw.Write(serialNumber);
                        //}


                        // process to add the record in PRN database
                        var printQuantity = db.RS_PrinterConfig.Where(m => m.Station_ID == stationID).Select(m => m.PrintQuantity).FirstOrDefault();
                        for (int i = 0; i < printQuantity; i++)
                        {
                            RS_PRN mmPRNObj = new RS_PRN();
                            mmPRNObj.Plant_ID = PlantID;
                            mmPRNObj.Shop_ID = ShopID;
                            mmPRNObj.Line_ID = LineID;
                            mmPRNObj.Station_ID = stationID;
                            // process to get Quality Ok PRN file
                            var Barcode2D = ModelCode.Trim().ToUpper() + ":" + serialNumber.Trim().ToUpper();
                            var barcodeTest = serialNumber.Trim().ToUpper() + " " + ModelCode.Trim().ToUpper();
                            String filename = "";
                            var DefaultPrinter = db.RS_PrinterConfig.Where(m => m.Station_ID == stationID).Select(m => m.Is_DefaultPrinter).FirstOrDefault();
                            if (DefaultPrinter == true)
                            {
                                filename = db.RS_PrinterConfig.Where(m => m.Station_ID == stationID).Select(m => m.DefaultPRNFileName).FirstOrDefault();
                            }
                            else
                            {
                                filename = db.RS_PrinterConfig.Where(m => m.Station_ID == stationID).Select(m => m.ExtraPRNFileName).FirstOrDefault();
                            }

                            string prnFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/prnfiles/" + filename));

                            prnFile = prnFile.Replace("012345678910", serialNumber.Trim().ToUpper());
                            prnFile = prnFile.Replace("serial_no", serialNumber.Trim().ToUpper());

                            prnFile = prnFile.Replace("model_code", ModelCode);
                            prnFile = prnFile.Replace("part_no", ModelCode);
                            prnFile = prnFile.Replace("Production Start", "Production");
                            prnFile = prnFile.Replace("name", "");
                            prnFile = prnFile.Replace("2DTrace", Barcode2D);
                            prnFile = prnFile.Replace("2DTest", barcodeTest);
                            prnFile = prnFile.Replace("Model_Desc", model.Model_Description);

                            //prnFile = prnFile.Replace("re_printed", " ");
                            prnFile = prnFile.Replace("date", Convert.ToString(DateTime.Today.Date.ToString("dd-MM-yyyy")));

                            mmPRNObj.PRN_Text = prnFile;
                            mmPRNObj.Is_OrderStart = true;

                            mmPRNObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            mmPRNObj.Inserted_Date = DateTime.Now;
                            mmPRNObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            db.RS_PRN.Add(mmPRNObj);
                            db.SaveChanges();
                            //End
                        }

                    }
                }
            }
            return Json(new { Status = status,Message = valmsg }, JsonRequestBehavior.AllowGet);
        }
        //sharad

        public ActionResult ScanBarcode(string SerialNumber)
        {
            var status = false;
            var valmsg = "";
            if (SerialNumber != "")
            {
                RS_CrankCase_Scan_Data scanData = new RS_CrankCase_Scan_Data();
                try
                { 
                    string SrNo = db.RS_CrankCase_Scan_Data.Where(m => m.Serial_No == SerialNumber && m.Status == true && m.Is_Barcode_Scanned == true).Select(m => m.Serial_No).FirstOrDefault();

                    if (string.Equals(SrNo, SerialNumber))
                    {
                        valmsg = "Barcode already scanned";
                    }
                    else
                    { 
                        //var SerialNoExist = db.RS_CrankCase_Scan_Data.Where(m => m.Serial_No == SerialNumber && m.Status == true).ToList();
                        //if (SerialNoExist.Count == 0 && SerialNumber.Length <= 10)
                        //if (string.IsNullOrEmpty(SrNo) && SerialNumber.Length < 10)
                        //{
                        //    valmsg = "Serial number is not marked";
                        //}
                        //else
                       // { 

                        //RS_OM_Order_List obj = db.RS_OM_Order_List.Where(m => m.Serial_No == SerialNumber).Select(m => m).FirstOrDefault();
                        //var stationID = obj.Station_ID;
                        //var lineID = obj.Line_ID;
                        //var ShopID = obj.Shop_ID;
                        //var PlantID = obj.Plant_ID;
                        //var tagName = db.RS_IoT_Tags.Where(m => m.Plant_ID == PlantID && m.Shop_ID == ShopID && m.Line_ID == lineID && m.Station_ID == stationID && m.Is_Line_Stop == true).Select(m => m.Tag_Name).FirstOrDefault();
                        //IoTProcess iotProcessObj = new IoTProcess();
                        //WriteIoT[] writeIoTObj = new WriteIoT[1];
                        //writeIoTObj[0] = new WriteIoT();
                        //writeIoTObj[0].id = tagName;
                        //writeIoTObj[0].v = "1";
                        //WriteResults[] writeResultObj = iotProcessObj.writePLCTag(writeIoTObj, 1);

                        //var st = writeResultObj[0].s;

                        //if (st == true)
                        //{
                        //    status = true;
                        //}

                         var Crank = db.RS_CrankCase_Scan_Data.Where(m => m.Serial_No == SerialNumber && m.Status == true).Select(m => m).FirstOrDefault();

                        if(Crank != null)
                        {
                            var Id = Crank.Row_ID;
                            scanData = db.RS_CrankCase_Scan_Data.Find(Id);

                            scanData.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            scanData.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            scanData.Updated_Date = DateTime.Now;
                            scanData.Is_Barcode_Scanned = true;
                            db.Entry(scanData).State = EntityState.Modified;
                            db.SaveChanges();
                            status = true;
                        }
                        else
                        {
                            status = false;
                        }

                        //  }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            
            return Json(new { Status = status, Message = valmsg }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkCrankCaseScans()
        {
            var latestEngine = db.RS_OM_Order_List.Where(m => m.Station_ID == 39).Select(m => m).OrderByDescending(m => m.Order_ID).FirstOrDefault();
            var SerialNo = latestEngine.Serial_No;
            var IsExist = db.RS_Engine_Marking_Data.Where(m => m.Engine_Identification_No == SerialNo).ToList();
            var status = false;
            var valmsg = "";
            if (IsExist.Count > 0)
                status = true;
            else
            {
                status = false;
                valmsg = SerialNo;
            }
                
            return Json(new { Status = status, Message = valmsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMarkedSerialNo(string ScanString)
        {
            var latestEngine = db.RS_OM_Order_List.Where(m => m.Station_ID == 39).Select(m => m).OrderByDescending(m => m.Order_ID).FirstOrDefault();
            var SerialNo = latestEngine.Serial_No;

            var IsExist = db.RS_Engine_Marking_Data.Where(m => m.Engine_Identification_No == SerialNo).ToList();
            var IsSame = string.Equals(ScanString, "Order Start");
            var status = false;
            var valmsg = "";
            if (IsExist.Count > 0 && IsSame == true)
                status = true;
            else if(IsSame == false)
            {
                status = false;
                valmsg = "Invalid";
            }
            else
            {
                status = false;
                valmsg = SerialNo;
            }
                
            return Json(new { Status = status,Message = valmsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsEngineNumberMark()
        {
            var latestEngine = db.RS_OM_Order_List.Where(m => m.Station_ID == 39).Select(m => m).OrderByDescending(m => m.Order_ID).FirstOrDefault();
            var SerialNo = latestEngine.Serial_No;

            var IsExist = db.RS_Engine_Marking_Data.Where(m => m.Engine_Identification_No == SerialNo).ToList();
            var status = false;
            if (IsExist.Count > 0)
                status = true;
            else
                status = false;
            return Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadBuildSheet(string fileName)
        {
            FileResult fileObj = null;
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileObj = File(fileName, "application/pdf");
                }
            }
            catch (Exception exp)
            {

                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "OrderStartController", "DownloadBuildSheet(" + fileName == null ? null : fileName + ")", Decimal.ToInt32(((FDSession)this.Session["FDSession"]).userId));
            }
            return fileObj;
        }
    }
}


