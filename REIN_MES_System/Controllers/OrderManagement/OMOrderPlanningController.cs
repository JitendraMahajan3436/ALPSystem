using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;
using System.Dynamic;
using System.ComponentModel;
//using Microsoft.Office.Client.Education;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp;
using System.Text;
using System.Globalization;
using System.Web.Script.Serialization;                   

namespace REIN_MES_System.Controllers.OrderManagement
{
    /*               Controller Name           : OMOrderPlanningController
     *               Description               : Controller used to Crete,update,delete and details of all Order planing or resequencing the order. 
     *               Author, Timestamp         : Jitendra Mahajan
     */
    public class OMOrderPlanningController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        /*               Action Name               : Index
         *               Description               : Action used to return the View to displaying Content for Order Planning
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : order_type
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult Index()
        {
            try
            {
                int plantid = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceModules.OM_Planning;
                globalData.subTitle = ResourceGlobal.Lists;
                globalData.controllerName = "OM_OrderPlanning";
                globalData.actionName = ResourceGlobal.Lists;
                globalData.contentTitle = ResourceModules.OM_Planning;
                globalData.contentFooter = ResourceModules.OM_Planning;


                ViewBag.GlobalDataModel = globalData;

                //Stattic Order List for Order Type
                //var ordet_List = new SelectList(new[]
                //{
                //    new { ID = "P", Name = "Parent" },
                //    new { ID = "S", Name = "Spare" },
                //},
                //        "ID", "Name", 1);

                //ViewBag.order_type = ordet_List;
                ViewBag.order_type = new SelectList(db.RS_OM_Order_Type.Where(m => m.Plant_ID == plantid), "Order_Type_Name", "Order_Type_Name");
                ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantid), "Plant_ID", "Plant_Name");
                //added by mukesh

                //DateTime time = DateTime.Now.Date;

                //var plannedDateList = db.RS_OM_Creation
                //    .Where(c => c.Planned_Date >= time && c.Plant_ID == plantid).Select(c => c.Planned_Date).Distinct().ToList();

                //List<String> datelist = new List<string>();
                //foreach (var d in plannedDateList)
                //{
                //    DateTime ds = Convert.ToDateTime(d);
                //    //datelist.Add(ds.ToShortDateString());
                //    string format = "yyyy-MM-dd";
                //    datelist.Add(ds.ToString(format));
                //}
                ////


                //ViewBag.pst = datelist.ToList();

                ViewBag.pst = "";


                ////working 
                //ViewBag.pst = db.RS_OM_Creation
                //    .Where(c => c.Planned_Date >= time && c.Plant_ID == plantid).Select(c => c.Planned_Date).Distinct().ToList();
                ////
                //ViewBag.pst = db.RS_OM_Creation
                //    .Where(c => c.Planned_Date >= time && c.Plant_ID == plantid).Select(c=>c.Planned_Date).Distinct().ToList();
                //ViewBag.Planned_Date = new SelectList(db.RS_OM_Creation.AsEnumerable()
                //    .Where(c => c.Planned_Date.Value.ToString("dd.MM.yy") == time.ToString("dd.MM.yy") && c.Plant_ID == plantid), "Planned_Date", "Planned_Date");



                //var pst = new SelectList(db.RS_OM_Creation.AsEnumerable()
                //    .Where(c => c.Planned_Date.Value.ToString("dd.MM.yy") == time.ToString("dd.MM.yy") && c.Plant_ID == plantid), "Planned_Date", "Planned_Date");
                //ViewBag.Planned_Date = new SelectList(db.RS_OM_Creation.Where(p => p.Plant_ID == plantid).Select(m => m.Planned_Date), "Planned_Date", "Planned_Date").ToList();



                ViewBag.CUMNDATA = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantid), "Plant_ID", "Plant_Name");
                //ViewBag.Shops = db.RS_Shops.Where(a => a.Plant_ID == plantid);



                //ViewBag.Platform = db.RS_OM_Platform.Where(a => a.Plant_ID == plantid);
                //ViewBag.Planned_Date = new SelectList(db.);
                var pfname = (db.RS_OM_Platform.Where(m => m.Plant_ID == plantid).Select(m => m.Platform_ID)).ToList();
                var ordPatt = db.RS_OM_Order_Pattern.Where(m => m.Plant_ID == plantid).OrderByDescending(m => m.Row_ID).GroupBy(x => x.Platform_ID).Select(x => x.FirstOrDefault()).ToList();
                var Platform = (from pf in db.RS_OM_Order_Pattern
                                    //join orderPattern in db.RS_OM_Platform on new { pf.Plant_ID, pf.Platform_ID }
                                    //equals new { orderPattern.Plant_ID, orderPattern.Platform_ID }
                                join orderPattern in db.RS_OM_Platform on pf.Platform_ID equals orderPattern.Platform_ID
                                where orderPattern.Plant_ID == plantid
                                orderby pf.Planned_Date descending
                                select new OrderPattern()
                                {
                                    Ratio = pf.Ratio,
                                    Platform_ID = orderPattern.Platform_ID,
                                    Platform_Name = orderPattern.Platform_Name,
                                    Planned_Date = pf.Planned_Date
                                }).Take(3).ToList();
                //ViewBag.Planned_Date = new SelectList((from pf in db.RS_OM_Order_Pattern
                //                                           //join orderPattern in db.RS_OM_Platform on new { pf.Plant_ID, pf.Platform_ID }
                //                                           //equals new { orderPattern.Plant_ID, orderPattern.Platform_ID }
                //                                       join orderPattern in db.RS_OM_Platform on pf.Platform_ID equals orderPattern.Platform_ID
                //                                       where orderPattern.Plant_ID == plantid
                //                                       orderby pf.Planned_Date descending
                //                                       select pf.Planned_Date).ToList(), "Planned_Date", "Planned_Date");

                if (Platform.Count == 0)
                {
                    Platform = (from pf in db.RS_OM_Platform
                                join op in db.RS_OM_Order_Pattern on pf.Platform_ID equals op.Platform_ID
                                into opf
                                from x in opf.DefaultIfEmpty()
                                where pf.Plant_ID == plantid
                                select new OrderPattern()
                                {
                                    Ratio = (x == null ? 0 : x.Ratio),
                                    Platform_ID = pf.Platform_ID,
                                    Planned_Date = pf.Inserted_Date,
                                    Platform_Name = pf.Platform_Name
                                }).OrderByDescending(m => m.Planned_Date).Take(3).ToList();
                }
                ViewBag.Platform = Platform;
                ViewBag.Shift = new SelectList(db.RS_Shift.Where(m => m.Shop_ID == 0), "Shift_ID", "Shift_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantid), "Shop_ID", "Shop_Name");
                // ViewBag.Shift = (from shift in db.RS_Shift.Where(m => m.Plant_ID == plantid) select new Shift() { Shift_ID = shift.Shift_ID, Shift_Name = shift.Shift_Name }).ToList();
                //ViewData["Platform"] = Platform;
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }

        /*               Action Name               : GetShopMode
         *               Description               : Action used to return the list of Order Type  for Order Planning
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : order_type
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult GetShopMode(String order_type)
        {
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (order_type == "S")
                {
                    var st = from shop in db.RS_Shops
                             where shop.Is_Main == false && shop.Plant_ID == plantId
                             select new
                             {
                                 Id = shop.Shop_ID,
                                 Value = shop.Shop_Name
                             };
                    return Json(st, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var st = from shop in db.RS_Shops
                             where shop.Is_Main == true && shop.Plant_ID == plantId
                             select new
                             {
                                 Id = shop.Shop_ID,
                                 Value = shop.Shop_Name
                             };
                    return Json(st, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /*               Action Name               : GetLineID
        *               Description               : Action used to return the list of Line ID for OMOrderPlanning
        *               Author, Timestamp         : Jitendra Mahajan
        *               Input parameter           : Shopid
        *               Return Type               : ActionResult
        *               Revision                  : 1
       */
        //Find Line
        public ActionResult GetLineID(int Shop_id)
        {

            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var lineDetail = (from line in db.RS_Lines
                              join partgroup in db.RS_Partgroup on line.Line_ID equals partgroup.Line_ID
                              where line.Shop_ID == Shop_id && partgroup.Order_Create == true && line.Plant_ID == plantId
                              select new
                              {
                                  line.Line_Name,
                                  line.Line_ID
                              }).Distinct().ToList();
            return Json(lineDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShiftID(int Shop_id)
        {
            var shiftDetail = (from shift in db.RS_Shift
                              where shift.Shop_ID == Shop_id
                              select new
                              {
                                  shift.Shift_Name,
                                  shift.Shift_ID
                              }).Distinct().ToList();
            return Json(shiftDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlatformID(int Line_id, int Shop_id)
        {
            var platformDetail = (from platform in db.RS_OM_Platform
                                  join partgroup in db.RS_Partgroup on platform.Line_ID equals partgroup.Line_ID
                                  where platform.Shop_ID == Shop_id && platform.Line_ID == Line_id && partgroup.Order_Create == true
                                  select new
                                  {
                                      platform.Platform_Name,
                                      platform.Platform_ID
                                  }).Distinct().ToList();
            return Json(platformDetail, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name               : GetSeriesCode
         *               Description               : Action used to return the list of Series ID  for Order Planning
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant_Id,shopid
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Series
        public ActionResult GetSeriesCode(int Plant_Id, int Shop_Id)
        {
            var Series = db.RS_Series.Where(c => c.Plant_ID == Plant_Id && c.Shop_ID == Shop_Id)
                                     .Select(c => new { c.Series_Code, c.Series_Description });
            return Json(Series, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult loadOrderSequence(FormCollection frmFieldsArr)
        {
            int lineID, shopID, ShiftID;
            DateTime nowTime = DateTime.Now;
            shopID = Convert.ToInt16(frmFieldsArr["Shop_ID"]);
            ShiftID = Convert.ToInt16(frmFieldsArr["Shift_ID"]);
            //PlatformID = Convert.ToInt16(frmFieldsArr["Platform_ID"]);
            decimal platformIdLocked = Convert.ToDecimal(frmFieldsArr["Platform_ID"]);
            lineID = Convert.ToInt16(frmFieldsArr["Line_ID"]);

            int plantid = ((FDSession)this.Session["FDSession"]).plantId;

            List<CummulativeFields> orderSequenceObjSheduled = new List<CummulativeFields>();
            List<CummulativeFields> orderSequenceObjReleased = new List<CummulativeFields>();
            List<CummulativeFields> orderSequenceObjSheduledOtherShift = new List<CummulativeFields>();


            //List < List <CummulativeFields>> orderSequenceObjList = new List<List<CummulativeFields>>();

            //remove this code later
            ViewBag.SequenceShop = "Tractor";
            ViewBag.blockAfterQty = 0;
            ViewBag.Plant_ID = plantid;
            ViewBag.Shift = new SelectList(db.RS_Shift.Where(m => m.Plant_ID == plantid), "Shift_ID", "Shift_Name");

            //
            if (frmFieldsArr["Platform_ID"] != "")
            {
                lineID = Convert.ToInt16(GetLineIdByPlatform(Convert.ToDecimal(frmFieldsArr["Platform_ID"])));
            }
            else
            {
                lineID = 0;
            }

            var Platform = (from pf in db.RS_OM_Platform
                            where pf.Plant_ID == plantid
                            select new
                            {
                                Platform_ID = pf.Platform_ID,
                                Platform_Name = pf.Platform_Name
                            }).ToList();

            ViewBag.Platform = Platform;


            //if (db.RS_OM_Platform.Find(platformIdLocked).Platform_Name.Equals("U321", StringComparison.CurrentCultureIgnoreCase))
            //{

            orderSequenceObjSheduled =
                                        // (from u321 in db.RS_OM_U321_Tactsheet_Orders
                                        //    join
                                        (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == ShiftID)
                                             //on u321.Order_No equals or.Order_No
                                         orderby or.RSN ascending
                                         select new CummulativeFields()
                                         {
                                             Row_ID = or.Row_ID,
                                             Order_No = or.Order_No,
                                             Model_Code = or.Model_Code,
                                             Model_Description = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code).Select(m => m.Model_Description).FirstOrDefault(),
                                             Variant_Desc = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code).Select(m => m.BIW_Description).FirstOrDefault(),
                                             Series = or.RS_Series.Series_Description,
                                             parentModel_Code = or.Model_Code,
                                             Inserted_Date = or.Inserted_Date,
                                             remarks = or.Remarks,
                                             orderType = or.Order_Type,
                                             UToken = or.UToken,
                                             Planned_Shift_ID = or.Planned_Shift_ID,
                                             Color_code = or.Model_Color,
                                             Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
                                             shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
                                             //Locked = (db.RS_OM_U321_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true)),
                                             Attribution = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantid && m.Shop_ID == shopID).Select(m => m.RS_Model_Attribute_Master.Attribution).FirstOrDefault() ?? ""
                                         }).ToList();

            orderSequenceObjSheduledOtherShift =
                                        (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID != ShiftID && or.Planned_Shift_ID != null)
                                         orderby or.RSN ascending
                                         select new CummulativeFields()
                                         {
                                             Row_ID = or.Row_ID,
                                             Order_No = or.Order_No,
                                             Model_Code = or.Model_Code,
                                             Model_Description = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code).Select(m => m.Model_Description).FirstOrDefault(),
                                             Variant_Desc = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code).Select(m => m.BIW_Description).FirstOrDefault(),
                                             Series = or.RS_Series.Series_Description,
                                             parentModel_Code = or.Model_Code,
                                             Inserted_Date = or.Inserted_Date,
                                             remarks = or.Remarks,
                                             orderType = or.Order_Type,
                                             UToken = or.UToken,
                                             Planned_Shift_ID = or.Planned_Shift_ID,
                                             Color_code = or.Model_Color,
                                             Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
                                             shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
                                                 //Locked = (db.RS_OM_U321_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true)),
                                                 Attribution = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantid && m.Shop_ID == shopID).Select(m => m.RS_Model_Attribute_Master.Attribution).FirstOrDefault() ?? ""

                                         }).ToList();

            orderSequenceObjReleased = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == null)
                                        orderby or.Order_No ascending
                                        select new CummulativeFields()
                                        {
                                            Row_ID = or.Row_ID,
                                            Order_No = or.Order_No,
                                            Model_Code = or.Model_Code,
                                            Model_Description = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code).Select(m => m.Model_Description).FirstOrDefault(),
                                            Variant_Desc = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code).Select(m => m.BIW_Description).FirstOrDefault(),
                                            Series = or.RS_Series.Series_Description,
                                            parentModel_Code = or.Model_Code,
                                            Inserted_Date = or.Inserted_Date,
                                            remarks = or.Remarks,
                                            orderType = or.Order_Type,
                                            UToken = or.UToken,
                                            Planned_Shift_ID = or.Planned_Shift_ID,
                                            Color_code = or.Model_Color,
                                            Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
                                            shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
                                            //Locked = (db.RS_OM_U321_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true)),
                                            Attribution = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantid && m.Shop_ID == shopID).Select(m => m.RS_Model_Attribute_Master.Attribution).FirstOrDefault() ?? ""

                                        }).ToList();
            orderSequenceObjReleased = (orderSequenceObjSheduledOtherShift.Union(orderSequenceObjReleased)).ToList();
            //}
            //else if(db.RS_OM_Platform.Find(platformIdLocked).Platform_Name.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    orderSequenceObjSheduled = (from s201 in db.RS_OM_S201_Tactsheet_Orders
            //                                join or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == ShiftID)
            //                               on s201.Order_No equals or.Order_No
            //                                orderby s201.RSN ascending
            //                                select new CummulativeFields()
            //                                {
            //                                    Row_ID = or.Row_ID,
            //                                    Order_No = or.Order_No,
            //                                    Model_Code = or.Model_Code,
            //                                    Series = or.RS_Series.Series_Description,
            //                                    parentModel_Code = or.Model_Code,
            //                                    Inserted_Date = or.Inserted_Date,
            //                                    remarks = or.Remarks,
            //                                    orderType = or.Order_Type,
            //                                    UToken = or.UToken,
            //                                    Planned_Shift_ID = or.Planned_Shift_ID,
            //                                    Color_code = or.Model_Color,
            //                                    Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
            //                                    shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
            //                                    Locked = (db.RS_OM_S201_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true)),
            //                                }).ToList();

            //    orderSequenceObjSheduledOtherShift = (from s201 in db.RS_OM_S201_Tactsheet_Orders
            //                                          join or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID != ShiftID)
            //                                         on s201.Order_No equals or.Order_No
            //                                          orderby s201.RSN ascending
            //                                          select new CummulativeFields()
            //                                          {
            //                                              Row_ID = or.Row_ID,
            //                                              Order_No = or.Order_No,
            //                                              Model_Code = or.Model_Code,
            //                                              Series = or.RS_Series.Series_Description,
            //                                              parentModel_Code = or.Model_Code,
            //                                              Inserted_Date = or.Inserted_Date,
            //                                              remarks = or.Remarks,
            //                                              orderType = or.Order_Type,
            //                                              UToken = or.UToken,
            //                                              Planned_Shift_ID = or.Planned_Shift_ID,
            //                                              Color_code = or.Model_Color,
            //                                              Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
            //                                              shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
            //                                              Locked = (db.RS_OM_S201_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true) ),
            //                                          }).ToList();

            //    orderSequenceObjReleased = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == null)
            //                                orderby or.Order_No ascending
            //                                select new CummulativeFields()
            //                                {
            //                                    Row_ID = or.Row_ID,
            //                                    Order_No = or.Order_No,
            //                                    Model_Code = or.Model_Code,
            //                                    Series = or.RS_Series.Series_Description,
            //                                    parentModel_Code = or.Model_Code,
            //                                    Inserted_Date = or.Inserted_Date,
            //                                    remarks = or.Remarks,
            //                                    orderType = or.Order_Type,
            //                                    UToken = or.UToken,
            //                                    Planned_Shift_ID = or.Planned_Shift_ID,
            //                                    Color_code = or.Model_Color,
            //                                    Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
            //                                    shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
            //                                    Locked = (db.RS_OM_S201_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true)),
            //                                }).ToList();
            //    orderSequenceObjReleased = (orderSequenceObjSheduledOtherShift.Union(orderSequenceObjReleased)).ToList();
            //}
            //else if (db.RS_OM_Platform.Find(platformIdLocked).Platform_Name.Equals("XYLO", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    orderSequenceObjSheduled = (from u321 in db.RS_OM_XYLO_Tactsheet_Orders_Sequence
            //                                join or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == ShiftID)
            //                               on u321.Order_No equals or.Order_No
            //                                orderby u321.RSN ascending
            //                                select new CummulativeFields()
            //                                {
            //                                    Row_ID = or.Row_ID,
            //                                    Order_No = or.Order_No,
            //                                    Model_Code = or.Model_Code,
            //                                    Series = or.RS_Series.Series_Description,
            //                                    parentModel_Code = or.Model_Code,
            //                                    Inserted_Date = or.Inserted_Date,
            //                                    remarks = or.Remarks,
            //                                    orderType = or.Order_Type,
            //                                    UToken = or.UToken,
            //                                    Planned_Shift_ID = or.Planned_Shift_ID,
            //                                    Color_code = or.Model_Color,
            //                                    Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
            //                                    shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
            //                                    Locked = (db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true) ),
            //                                }).ToList();

            //    orderSequenceObjSheduledOtherShift = (from u321 in db.RS_OM_XYLO_Tactsheet_Orders_Sequence
            //                                          join or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID != ShiftID)
            //                                         on u321.Order_No equals or.Order_No
            //                                          orderby u321.RSN ascending
            //                                          select new CummulativeFields()
            //                                          {
            //                                              Row_ID = or.Row_ID,
            //                                              Order_No = or.Order_No,
            //                                              Model_Code = or.Model_Code,
            //                                              Series = or.RS_Series.Series_Description,
            //                                              parentModel_Code = or.Model_Code,
            //                                              Inserted_Date = or.Inserted_Date,
            //                                              remarks = or.Remarks,
            //                                              orderType = or.Order_Type,
            //                                              UToken = or.UToken,
            //                                              Planned_Shift_ID = or.Planned_Shift_ID,
            //                                              Color_code = or.Model_Color,
            //                                              Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
            //                                              shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
            //                                              Locked = (db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true) ),
            //                                          }).ToList();

            //    orderSequenceObjReleased = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantid && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Shift_ID == null)
            //                                orderby or.Order_No ascending
            //                                select new CummulativeFields()
            //                                {
            //                                    Row_ID = or.Row_ID,
            //                                    Order_No = or.Order_No,
            //                                    Model_Code = or.Model_Code,
            //                                    Series = or.RS_Series.Series_Description,
            //                                    parentModel_Code = or.Model_Code,
            //                                    Inserted_Date = or.Inserted_Date,
            //                                    remarks = or.Remarks,
            //                                    orderType = or.Order_Type,
            //                                    UToken = or.UToken,
            //                                    Planned_Shift_ID = or.Planned_Shift_ID,
            //                                    Color_code = or.Model_Color,
            //                                    Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Model_Color && m.Plant_ID == plantid).Select(m => m.Colour_Desc).FirstOrDefault() ?? "",
            //                                    shiftName = db.RS_Shift.Where(m => m.Shift_ID == or.Planned_Shift_ID).Select(m => m.Shift_Name).FirstOrDefault() ?? "",
            //                                    Locked = (db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == or.Order_No && m.Is_Locked == true)),
            //                                }).ToList();
            //    orderSequenceObjReleased = (orderSequenceObjSheduledOtherShift.Union(orderSequenceObjReleased)).ToList();
            //}

            //for adding attribution
            //foreach (var or in orderSequenceObjSheduled)
            //{

            //    var model = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantid && m.Shop_ID == shopID).FirstOrDefault();
            //    var Attribute = "";
            //    List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
            //    for (int i = 0; i < attributionParameters.Count; i++)
            //    {
            //        AttributionParameters attributionParameter = attributionParameters[i];
            //        try
            //        {
            //            Convert.ToInt32(attributionParameter.Value);
            //        }
            //        catch (Exception)
            //        {

            //            continue;
            //        }
            //        if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            int attrId = Convert.ToInt32(attributionParameter.Value);
            //            Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
            //            break;
            //            //       attributionParameter.Value;
            //        }

            //    }
            //    or.Attribution = Attribute;

            //}
            //foreach (var or in orderSequenceObjReleased)
            //{

            //    var model = db.RS_Model_Master.Where(m => m.Model_Code == or.Model_Code && m.Plant_ID == plantid && m.Shop_ID == shopID).FirstOrDefault();
            //    var Attribute = "";
            //    List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
            //    for (int i = 0; i < attributionParameters.Count; i++)
            //    {
            //        AttributionParameters attributionParameter = attributionParameters[i];
            //        try
            //        {
            //            Convert.ToInt32(attributionParameter.Value);
            //        }
            //        catch (Exception)
            //        {

            //            continue;
            //        }
            //        if (attributionParameter.label.Equals("Attribute", StringComparison.InvariantCultureIgnoreCase))
            //        {
            //            int attrId = Convert.ToInt32(attributionParameter.Value);
            //            Attribute = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
            //            break;
            //            //       attributionParameter.Value;
            //        }

            //    }
            //    or.Attribution = Attribute;

            //}



            ////





            ViewBag.orderSequenceObjSheduled = orderSequenceObjSheduled;
            ViewBag.orderSequenceObjReleased = orderSequenceObjReleased;

            return PartialView("PVOrderSequence");

        }
        //[HttpPost]
        //public ActionResult loadOrderSequenceTactsheet(FormCollection frmFieldsArr)
        //{
        //    int plantID, lineID, shopID, blockAfterQty;
        //    blockAfterQty = Convert.ToInt16(frmFieldsArr["blockAfterSize"]);
        //    DateTime nowTime = DateTime.Now;
        //    shopID = Convert.ToInt16(frmFieldsArr["Shop_ID"]);
        //    decimal platformIdLocked = Convert.ToDecimal(frmFieldsArr["Platform_ID"]);
        //    DateTime plannedDate = Convert.ToDateTime(frmFieldsArr["Planned_Date"]);
        //    if (frmFieldsArr["Platform_ID"] != "")
        //    {
        //        lineID = Convert.ToInt16(GetLineIdByPlatform(Convert.ToDecimal(frmFieldsArr["Platform_ID"])));
        //    }
        //    else
        //    {
        //        lineID = 0;
        //    }

        //    try
        //    {
        //        string orderType, remarks = "";
        //        int plantid = ((FDSession)this.Session["FDSession"]).plantId;
        //        plantID = plantid;

        //        if (blockAfterQty < 0)
        //        {
        //            blockAfterQty = 0;
        //        }

        //        string shopName = db.RS_Shops.Find(shopID).Shop_Name;
        //        bool Is_Main = db.RS_Shops.Where(ss => ss.Shop_ID == shopID).Select(shp => shp.Is_Main).FirstOrDefault();
        //        List<CummulativeFields> orderSequenceObj = new List<CummulativeFields>();
        //        bool showOrder = false;
        //        if (Is_Main)
        //        {
        //            orderSequenceObj = GetOrderSequenceBasedOnRatio(plantid, shopID, lineID);

        //            if (orderSequenceObj != null)
        //            {
        //                foreach (CummulativeFields cummlObj in orderSequenceObj)
        //                {
        //                    string tokenId = cummlObj.UToken;

        //                    RS_OM_OrderRelease mmEngModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == cummlObj.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 1 && a.UToken == tokenId).FirstOrDefault();
        //                    RS_OM_OrderRelease mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == cummlObj.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == tokenId).FirstOrDefault();
        //                    string engineModel = "";
        //                    string tranSeries = "";
        //                    if (mmEngModelObj != null)
        //                    {
        //                        engineModel = mmEngModelObj.partno;
        //                    }
        //                    if (mmTransModelObj != null)
        //                    {
        //                        tranSeries = mmTransModelObj.RS_Series.Series_Description;
        //                    }
        //                    cummlObj.Model_Code = cummlObj.Model_Code.Trim();

        //                }
        //            }

        //            ViewBag.SequenceShop = "Tractor";
        //        }
        //        else if (shopName.Contains("Hydraulic"))
        //        {
        //            orderSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
        //                                orderby or.RSN ascending
        //                                select new CummulativeFields()
        //                                {
        //                                    Row_ID = or.Row_ID,
        //                                    Order_No = or.Order_No,
        //                                    Model_Code = or.partno,
        //                                    Series = or.RS_Series.Series_Description,
        //                                    parentModel_Code = or.Model_Code,
        //                                    Inserted_Date = or.Inserted_Date,
        //                                    remarks = or.Remarks,
        //                                    orderType = or.Order_Type,
        //                                    UToken = or.UToken,
        //                                    Planned_Shift_ID = or.Planned_Shift_ID
        //                                }).ToList();
        //            foreach (CummulativeFields cummlObj in orderSequenceObj)
        //            {
        //                string tokenId = cummlObj.UToken;

        //                RS_OM_OrderRelease mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == cummlObj.parentModel_Code && a.Model_Code != a.partno && a.Shop_ID == 2 && a.UToken == tokenId).FirstOrDefault();
        //                string transModel = "";
        //                string transSeries = "";
        //                if (mmTransModelObj != null)
        //                {
        //                    transModel = mmTransModelObj.partno;
        //                    transSeries = mmTransModelObj.RS_Series.Series_Description;
        //                }
        //                cummlObj.Model_Code = cummlObj.Model_Code.Trim();
        //                cummlObj.parentModel_Code = transModel;
        //                cummlObj.parentSeries = transSeries;
        //            }
        //            ViewBag.SequenceShop = "Hydraulic";
        //        }
        //        else
        //        {
        //            orderSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
        //                                orderby or.RSN ascending
        //                                select new CummulativeFields()
        //                                {
        //                                    Row_ID = or.Row_ID,
        //                                    Order_No = or.Order_No,
        //                                    Model_Code = or.partno.Trim(),
        //                                    Series = or.RS_Series.Series_Description,
        //                                    parentModel_Code = or.Model_Code,
        //                                    Inserted_Date = or.Inserted_Date,
        //                                    remarks = or.Remarks,
        //                                    orderType = or.Order_Type,
        //                                    Planned_Shift_ID = or.Planned_Shift_ID

        //                                }).ToList();
        //            ViewBag.SequenceShop = "Others";
        //        }

        //        //DateTime today = Convert.ToDateTime("2016-05-21").Date;
        //        DateTime today = DateTime.Today;
        //        List<CummulativeFields> orderStartedObj = new List<CummulativeFields>();
        //        string chkModel_Code = "";
        //        string chkSeries = "";
        //        string chkParentModel = "";
        //        string chkParentSeries = "";
        //        int startedQty = 0;
        //        int cummlQty = 0;
        //        int releasedQty = 0;
        //        int holdedQty = 0;

        //        List<CummulativeFields> plannedOrdersDataObj = new List<CummulativeFields>();
        //        if (Is_Main)//Tractor Shop
        //        {
        //            RS_OM_OrderRelease mmEngModelObj = new RS_OM_OrderRelease();
        //            RS_OM_OrderRelease mmTransModelObj = new RS_OM_OrderRelease();
        //            orderStartedObj = db.GetTracterOrderDetails(shopID, plannedDate, lineID).OrderBy(a => a.Cumul)
        //                                                    .Select(a => new CummulativeFields()
        //                                                    {
        //                                                        Model_Code = a.Model_Code,
        //                                                        //Color_Name = a.Colour_Desc,
        //                                                        //PlatformName = a.Platform_Name,
        //                                                        Order_Status = a.Order_Status,
        //                                                        Qty = a.Cnt,
        //                                                        CummlQty = a.Cumul,
        //                                                        remarks = a.Remarks,
        //                                                        //orderType = a.Order_Type
        //                                                    }).ToList();
        //            foreach (CummulativeFields cumFieldsObj in orderStartedObj)
        //            {
        //                ////added by mukesh


        //                if (!chkModel_Code.Equals(cumFieldsObj.Model_Code, StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    if (!String.IsNullOrWhiteSpace(chkModel_Code))
        //                    {
        //                        CummulativeFields CumlObj = new CummulativeFields();
        //                        //here is the problem (due to change in cumm )
        //                        //modified by mukesh
        //                        CumlObj.Model_Code = chkModel_Code;

        //                        //CumlObj.Model_Code = chkModel_Code;
        //                        CumlObj.Series = chkSeries;
        //                        CumlObj.StartedQty = startedQty;
        //                        CumlObj.HoldQty = holdedQty;
        //                        CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
        //                        CumlObj.EngineModelCode = chkParentModel;
        //                        CumlObj.TransmissionSeries = chkParentSeries;
        //                        CumlObj.CummlQty = cummlQty;
        //                        CumlObj.Color_Name = cumFieldsObj.Color_Name;
        //                        CumlObj.orderType = cumFieldsObj.orderType;
        //                        CumlObj.Planned_Shift_ID = cumFieldsObj.Planned_Shift_ID;
        //                        //CumlObj.orderType = db.RS_OM_OrderRelease.Where()
        //                        mmEngModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 1 && a.UToken == CumlObj.UToken).FirstOrDefault();
        //                        mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

        //                        if (mmEngModelObj != null)
        //                        {
        //                            CumlObj.EngineModelCode = mmEngModelObj.partno;
        //                        }
        //                        if (mmTransModelObj != null)
        //                        {
        //                            CumlObj.TransmissionSeries = mmTransModelObj.RS_Series.Series_Description;
        //                        }

        //                        plannedOrdersDataObj.Add(CumlObj);
        //                        startedQty = 0;
        //                        holdedQty = 0;
        //                        releasedQty = 0;
        //                    }

        //                    switch (cumFieldsObj.Order_Status)
        //                    {
        //                        case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                        case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                        default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                    }
        //                    chkModel_Code = cumFieldsObj.Model_Code;
        //                    chkSeries = cumFieldsObj.Series;
        //                    chkParentModel = cumFieldsObj.EngineModelCode;
        //                    chkParentSeries = cumFieldsObj.TransmissionSeries;

        //                    cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);

        //                }
        //                else
        //                {
        //                    switch (cumFieldsObj.Order_Status)
        //                    {
        //                        case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                        case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                        default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                    }
        //                    cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
        //                }
        //                if (cummlQty >= orderStartedObj.Last().CummlQty)
        //                {
        //                    CummulativeFields CumlObj = new CummulativeFields();

        //                    CumlObj.Model_Code = chkModel_Code;
        //                    CumlObj.Series = chkSeries;
        //                    CumlObj.StartedQty = startedQty;
        //                    CumlObj.HoldQty = holdedQty;
        //                    CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
        //                    CumlObj.EngineModelCode = chkParentModel;
        //                    CumlObj.TransmissionSeries = chkParentSeries;
        //                    CumlObj.CummlQty = cummlQty;
        //                    CumlObj.Color_Name = cumFieldsObj.Color_Name;
        //                    CumlObj.orderType = cumFieldsObj.orderType;
        //                    CumlObj.Planned_Shift_ID = cumFieldsObj.Planned_Shift_ID;
        //                    mmEngModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 1).FirstOrDefault();
        //                    mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2).FirstOrDefault();

        //                    if (mmEngModelObj != null)
        //                    {
        //                        CumlObj.EngineModelCode = mmEngModelObj.partno;
        //                    }
        //                    if (mmTransModelObj != null)
        //                    {
        //                        CumlObj.TransmissionSeries = mmTransModelObj.RS_Series.Series_Description;
        //                    }
        //                    plannedOrdersDataObj.Add(CumlObj);
        //                }

        //            }
        //        }
        //        else
        //        {
        //            orderStartedObj = db.GetShopWiseDetails(shopID).OrderBy(a => a.Cumul)
        //                                                     .Select(a => new CummulativeFields()
        //                                                     {
        //                                                         Model_Code = a.Model_Code,
        //                                                         Series = a.Child_Series,
        //                                                         parentModel_Code = a.Parent_Model,
        //                                                         parentSeries = a.Parent_Series,
        //                                                         Order_Status = a.Order_Status,
        //                                                         Qty = a.Cnt,
        //                                                         CummlQty = a.Cumul,
        //                                                         remarks = a.Remarks
        //                                                     }).ToList();
        //            List<string> remarksList = new List<string>();
        //            foreach (CummulativeFields cumFieldsObj in orderStartedObj)
        //            {
        //                if (!chkModel_Code.Equals(cumFieldsObj.Model_Code, StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    if (!String.IsNullOrWhiteSpace(chkModel_Code))
        //                    {
        //                        CummulativeFields CumlObj = new CummulativeFields();

        //                        CumlObj.Model_Code = chkModel_Code;
        //                        CumlObj.Series = chkSeries;
        //                        CumlObj.StartedQty = startedQty;
        //                        CumlObj.HoldQty = holdedQty;
        //                        CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
        //                        CumlObj.parentModel_Code = chkParentModel;
        //                        CumlObj.parentSeries = chkParentSeries;
        //                        CumlObj.Color_Name = cumFieldsObj.Color_Name;//added by mukesh
        //                        CumlObj.CummlQty = cummlQty;
        //                        CumlObj.remarks = String.Join(",", remarksList.ToArray());
        //                        CumlObj.orderType = cumFieldsObj.orderType;
        //                        CumlObj.Planned_Shift_ID = cumFieldsObj.Planned_Shift_ID;
        //                        if (shopName.Contains("Hydraulic"))
        //                        {
        //                            RS_OM_OrderRelease mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkParentModel && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

        //                            if (mmTransModelObj != null)
        //                            {
        //                                CumlObj.parentModel_Code = mmTransModelObj.partno;
        //                                CumlObj.parentSeries = mmTransModelObj.RS_Series.Series_Description;
        //                            }
        //                        }

        //                        plannedOrdersDataObj.Add(CumlObj);
        //                        startedQty = 0;
        //                        holdedQty = 0;
        //                        releasedQty = 0;
        //                        remarksList.Clear();
        //                    }
        //                    switch (cumFieldsObj.Order_Status)
        //                    {
        //                        case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                        case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                        default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                    }
        //                    chkModel_Code = cumFieldsObj.Model_Code;
        //                    chkSeries = cumFieldsObj.Series;
        //                    chkParentModel = cumFieldsObj.parentModel_Code;
        //                    chkParentSeries = cumFieldsObj.parentSeries;
        //                    cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
        //                    if (cumFieldsObj.remarks != null)
        //                    {
        //                        if (remarksList.Contains(cumFieldsObj.remarks) == false)
        //                        {
        //                            remarksList.Add(cumFieldsObj.remarks.Trim());
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    if (remarksList.Contains(cumFieldsObj.remarks) == false)
        //                    {
        //                        remarksList.Add(cumFieldsObj.remarks.Trim());
        //                    }
        //                    switch (cumFieldsObj.Order_Status)
        //                    {
        //                        case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                        case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                        default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        //                    }
        //                    cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
        //                }
        //                if (cummlQty >= orderStartedObj.Last().CummlQty)
        //                {
        //                    if (cumFieldsObj.remarks != null)
        //                    {
        //                        if (remarksList.Contains(cumFieldsObj.remarks) == false)
        //                        {
        //                            remarksList.Add(cumFieldsObj.remarks.Trim());
        //                        }
        //                    }
        //                    CummulativeFields CumlObj = new CummulativeFields();

        //                    CumlObj.Model_Code = chkModel_Code;
        //                    CumlObj.Series = chkSeries;
        //                    CumlObj.StartedQty = startedQty;
        //                    CumlObj.HoldQty = holdedQty;
        //                    CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
        //                    CumlObj.parentModel_Code = chkParentModel;
        //                    CumlObj.Color_Name = cumFieldsObj.Color_Name; //added by mukesh
        //                    CumlObj.parentSeries = chkParentSeries;
        //                    CumlObj.CummlQty = cummlQty;
        //                    CumlObj.remarks = String.Join(",", remarksList.ToArray());
        //                    CumlObj.orderType = cumFieldsObj.orderType;
        //                    CumlObj.Planned_Shift_ID = cumFieldsObj.Planned_Shift_ID;
        //                    //added by mukesh
        //                    //CumlObj.Color_Name = orderStartedObj.Last().Color_Name;
        //                    //

        //                    if (shopName.Contains("Hydraulic"))
        //                    {
        //                        RS_OM_OrderRelease mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == cumFieldsObj.parentModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

        //                        if (mmTransModelObj != null)
        //                        {
        //                            CumlObj.parentModel_Code = mmTransModelObj.partno;
        //                            CumlObj.parentSeries = mmTransModelObj.RS_Series.Series_Description;
        //                        }
        //                    }
        //                    remarksList.Clear();
        //                    plannedOrdersDataObj.Add(CumlObj);
        //                }
        //            }//FOR LOOP ENDING



        //        }

        //        if (blockAfterQty > 0)
        //        {
        //            db.RS_OM_OrderRelease.Where(a => a.Line_ID == lineID && a.Order_Status == "Release")
        //                            .OrderBy(a => a.RSN).Skip(blockAfterQty).ToList()
        //                            .ForEach(a => a.Is_Blocked = true);
        //            db.SaveChanges();
        //        }



        //        List<CummulativeFields> plannedStartedOrdersDataObj = new List<CummulativeFields>();
        //        //int? counter = 0;

        //        ViewBag.blockAfterQty = blockAfterQty;
        //        ////
        //        List<CummulativeFields> orderSequencePlatformSpecific = new List<CummulativeFields>();


        //        foreach (var order in orderSequenceObj)
        //        {

        //            var orderLineId = (from pf in db.RS_OM_Platform
        //                               join mm in db.RS_Model_Master.Where(m => m.Model_Code == order.Model_Code)
        //                               on pf.Platform_ID equals mm.Platform_Id
        //                               where pf.Plant_ID == plantid
        //                               select new
        //                               {
        //                                   pf.Line_ID
        //                               }).FirstOrDefault();

        //            int lineIDs = Convert.ToInt32(orderLineId.Line_ID);
        //            if (lineIDs == lineID)
        //            {
        //                if (db.RS_OM_Platform.Find(platformIdLocked).Platform_Name.Equals("U321", StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    order.Locked = db.RS_OM_U321_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == order.Order_No && m.Is_Locked == true);
        //                }
        //                else if (db.RS_OM_Platform.Find(platformIdLocked).Platform_Name.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
        //                {
        //                    order.Locked = db.RS_OM_S201_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == order.Order_No && m.Is_Locked == true);
        //                }

        //                orderSequencePlatformSpecific.Add(order);

        //            }

        //        }
        //        //Separating scheduled order from available orders
        //        CummulativeFieldsOrderPlanning cummulativeFieldsOrderPlanningObj = new CummulativeFieldsOrderPlanning();
        //        List<List<CummulativeFields>> orderSchduledAndAvailableList = new List<List<CummulativeFields>>();
        //        List<CummulativeFields> orderScheduledList = new List<CummulativeFields>();
        //        List<CummulativeFields> orderAvailableList = new List<CummulativeFields>();


        //        foreach (var ord in orderSequencePlatformSpecific)
        //        {
        //            if (ord.Planned_Shift_ID > 0)
        //            {
        //                orderScheduledList.Add(ord);
        //            }
        //            else
        //            {
        //                orderAvailableList.Add(ord);

        //            }
        //        }
        //        orderSchduledAndAvailableList.Add(orderScheduledList.OrderBy(m => m.RSN).ToList());
        //        orderSchduledAndAvailableList.Add(orderAvailableList.OrderBy(m => m.RSN).ToList());

        //        //seperating end
        //        ////test by mukesh for started table status
        //        List<CummulativeFields> startedOrderInfo = new List<CummulativeFields>();
        //        startedOrderInfo = orderSequencePlatformSpecific;
        //        foreach (var item in startedOrderInfo)
        //        {
        //            var orderLineId = (from pf in db.RS_OM_Platform
        //                               join mm in db.RS_Model_Master.Where(m => m.Model_Code == item.Model_Code)
        //                               on pf.Platform_ID equals mm.Platform_Id
        //                               where pf.Plant_ID == plantid
        //                               select new
        //                               {
        //                                   pf.Line_ID
        //                               }).FirstOrDefault();

        //            int lineIDs = Convert.ToInt32(orderLineId.Line_ID);

        //            if (lineIDs == lineID)
        //            {
        //                var res = plannedStartedOrdersDataObj.Where(c => c.Model_Code == item.Model_Code && c.orderType == item.orderType).FirstOrDefault();

        //                if (res == null)
        //                {
        //                    item.CummlQty = item.PlannedQty;
        //                    plannedStartedOrdersDataObj.Add(item);


        //                }
        //                else
        //                {
        //                    res.StartedQty += item.StartedQty;
        //                    res.HoldQty += item.HoldQty;
        //                    res.PlannedQty += item.PlannedQty;
        //                    res.CummlQty = res.PlannedQty;
        //                }
        //                //counter = item.CummlQty;
        //            }
        //        }
        //        ViewBag.startedOrders = GetOrderDetailGroupByOrder(plantid, shopID, lineID);//plannedStartedOrdersDataObj;
        //        ////
        //        return PartialView("PVOrderSequence", orderSchduledAndAvailableList);
        //    }
        //    catch (Exception exp)
        //    {
        //        generalHelper.addControllerException(exp, "OMOrderPlanningController", "loadOrderSequence(Post)", ((FDSession)this.Session["FDSession"]).userId);
        //    }
        //    finally
        //    {
        //        generalHelper.logUserActivity(shopID, lineID, "PPC Module", "Show Sequence(Blocked Qty = " + blockAfterQty + ")", nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
        //    }
        //    return null;
        //}

        public List<CummulativeFields> GetOrderDetailGroupByOrder(int plantID, int shopID, int lineID)
        {
            List<CummulativeFields> orderSequence = new List<CummulativeFields>();


            List<CummulativeFields> plannedStartedOrdersDataObj = new List<CummulativeFields>();
            try
            {
                orderSequence = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID)
                                 orderby or.RSN ascending
                                 select new CummulativeFields()
                                 {
                                     Row_ID = or.Row_ID,
                                     Order_No = or.Order_No,
                                     Model_Code = or.partno.Trim(),
                                     Color_code = or.Model_Color,
                                     orderType = or.Order_Type,
                                     Order_Status = or.Order_Status,
                                     Plant_OrderNo = or.Plant_OrderNo
                                     //  remarks = or.Remarks
                                 }).ToList();

                int hold = 0, started = 0, trdQty = 0, trlQty = 0, regQty = 0, spQty = 0;
                foreach (var item in orderSequence)
                {
                    var orderLineId = (from pf in db.RS_OM_Platform
                                       join mm in db.RS_Model_Master.Where(m => m.Model_Code == item.Model_Code)
                                       on pf.Platform_ID equals mm.Platform_Id
                                       where pf.Plant_ID == plantID
                                       select new
                                       {
                                           pf.Line_ID
                                       }).FirstOrDefault();

                    int lineIDs = Convert.ToInt32(orderLineId.Line_ID);

                    if (lineIDs == lineID)
                    {
                        var res = plannedStartedOrdersDataObj.Where(c => c.Model_Code == item.Model_Code && c.orderType == item.orderType && c.Plant_OrderNo == item.Plant_OrderNo).FirstOrDefault();

                        if (res == null)
                        {
                            // item.CummlQty = item.PlannedQty;
                            item.PlannedQty = 1;
                            if (item.Order_Status == "Started")
                            {
                                item.StartedQty = 1;
                            }
                            else if (item.Order_Status == "Hold")
                            {
                                item.HoldQty = 1;
                            }
                            plannedStartedOrdersDataObj.Add(item);

                        }
                        else
                        {

                            if (item.Order_Status == "Started")
                            {
                                started = 1;
                            }
                            else if (item.Order_Status == "Hold")
                            {
                                hold = 1;
                            }

                            if (item.orderType == "Regular")
                            {
                                res.PlannedQty++;
                                res.StartedQty += started;
                                res.HoldQty += hold;
                                started = 0;
                                hold = 0;

                            }
                            else if (item.orderType == "Trail")
                            {
                                res.PlannedQty++;
                                res.StartedQty += started;
                                res.HoldQty += hold;
                                started = 0;
                                hold = 0;
                            }
                            else if (item.orderType == "Tear Down")
                            {
                                res.PlannedQty++;
                                res.StartedQty += started;
                                res.HoldQty += hold;
                                started = 0;
                                hold = 0;
                            }
                            else if (item.orderType == "Spare")
                            {
                                res.PlannedQty++;
                                res.StartedQty += started;
                                res.HoldQty += hold;
                                started = 0;
                                hold = 0;
                            }

                            //res.CummlQty = res.PlannedQty;
                        }
                        //counter = item.CummlQty;
                    }
                }
                int? cummty = 0;// plannedStartedOrdersDataObj[0].PlannedQty;

                foreach (var row in plannedStartedOrdersDataObj)
                {
                    row.CummlQty = 0;
                    row.CummlQty += row.PlannedQty + cummty;
                    cummty = row.CummlQty;
                }
            }

            catch (Exception)
            {

                throw;
            }
            return plannedStartedOrdersDataObj.OrderBy(m => m.RSN).ToList();
        }
        //////////Jitendra Mahajan NAIR CODE///////////////////////
        ////[HttpPost]
        ////public ActionResult loadOrderSequence(FormCollection frmFieldsArr)
        ////{
        ////    int plantID, lineID, shopID, blockAfterQty;
        ////    blockAfterQty = Convert.ToInt16(frmFieldsArr["blockAfterSize"]);
        ////    DateTime nowTime = DateTime.Now;
        ////    shopID = Convert.ToInt16(frmFieldsArr["Shop_ID"]);
        ////    decimal platformIdLocked = Convert.ToDecimal(frmFieldsArr["Platform_ID"]);

        ////    if (frmFieldsArr["Platform_ID"] != "")
        ////    {
        ////        lineID = Convert.ToInt16(GetLineIdByPlatform(Convert.ToDecimal(frmFieldsArr["Platform_ID"])));
        ////    }
        ////    else
        ////    {
        ////        lineID = 0;
        ////    }

        ////    DateTime plannedDate = Convert.ToDateTime(frmFieldsArr["Planned_Date"]);


        ////    try
        ////    {
        ////        string orderType, remarks = "";
        ////        //orderType = frmFieldsArr["Order_Type"].ToString();
        ////        int plantid = ((FDSession)this.Session["FDSession"]).plantId;
        ////        //plantID = Convert.ToInt16(frmFieldsArr["Plant_ID"]);
        ////        plantID = plantid;

        ////        if (blockAfterQty < 0)
        ////        {
        ////            blockAfterQty = 0;
        ////        }

        ////        string shopName = db.RS_Shops.Find(shopID).Shop_Name;
        ////        //bool Is_Main = db.RS_Shops.Where(s=> s.Shop_ID == shopID).Select(shp => shp.Is_Main).FirstOrDefault();
        ////        bool Is_Main = db.RS_Shops.Where(ss => ss.Shop_ID == shopID).Select(shp => shp.Is_Main).FirstOrDefault();
        ////        List<CummulativeFields> orderSequenceObj = new List<CummulativeFields>();
        ////        // List<CummulativeFields> orderPatternSequenceObj = new List<CummulativeFields>();
        ////        bool showOrder = false;
        ////        if (Is_Main)
        ////        {
        ////            //if (db.mm_)
        ////            //{

        ////            //}
        ////            orderSequenceObj = GetOrderSequenceBasedOnRatio(plantid, shopID, lineID, plannedDate);

        ////            if (orderSequenceObj != null)
        ////            {
        ////                foreach (CummulativeFields cummlObj in orderSequenceObj)
        ////                {
        ////                    //string splitOrder = (cummlObj.Order_No).Substring(cummlObj.Order_No.Length - 8);
        ////                    string tokenId = cummlObj.UToken;
        ////                    //string enginePartGroupCode = db.RS_Partgroup.Where(a => a.Shop_ID == 1).FirstOrDefault().Partgroup_Code;
        ////                    //string transPartGroupCode = db.RS_Partgroup.Where(a => a.Shop_ID == 2).FirstOrDefault().Partgroup_Code;

        ////                    RS_OM_OrderRelease mmEngModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == cummlObj.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 1 && a.UToken == tokenId).FirstOrDefault();
        ////                    RS_OM_OrderRelease mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == cummlObj.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == tokenId).FirstOrDefault();
        ////                    string engineModel = "";
        ////                    string tranSeries = "";
        ////                    if (mmEngModelObj != null)
        ////                    {
        ////                        engineModel = mmEngModelObj.partno;
        ////                    }
        ////                    if (mmTransModelObj != null)
        ////                    {
        ////                        tranSeries = mmTransModelObj.RS_Series.Series_Description;
        ////                    }
        ////                    cummlObj.Model_Code = cummlObj.Model_Code.Trim();
        ////                    //cummlObj.EngineModelCode = engineModel;
        ////                    //cummlObj.TransmissionSeries = tranSeries;
        ////                }
        ////            }

        ////            ViewBag.SequenceShop = "Tractor";
        ////        }
        ////        else if (shopName.Contains("Hydraulic"))
        ////        {
        ////            orderSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
        ////                                orderby or.RSN ascending
        ////                                select new CummulativeFields()
        ////                                {
        ////                                    Row_ID = or.Row_ID,
        ////                                    Order_No = or.Order_No,
        ////                                    Model_Code = or.partno,
        ////                                    Series = or.RS_Series.Series_Description,
        ////                                    parentModel_Code = or.Model_Code,
        ////                                    Inserted_Date = or.Inserted_Date,
        ////                                    remarks = or.Remarks,
        ////                                    orderType = or.Order_Type,
        ////                                    UToken = or.UToken,
        ////                                    Planned_Shift_ID = or.Planned_Shift_ID
        ////                                }).ToList();
        ////            foreach (CummulativeFields cummlObj in orderSequenceObj)
        ////            {
        ////                //string splitOrder = (cummlObj.Order_No).Substring(cummlObj.Order_No.Length - 8);
        ////                string tokenId = cummlObj.UToken;
        ////                //string enginePartGroupCode = db.RS_Partgroup.Where(a => a.Shop_ID == 1).FirstOrDefault().Partgroup_Code;
        ////                //string transPartGroupCode = db.RS_Partgroup.Where(a => a.Shop_ID == 2).FirstOrDefault().Partgroup_Code;

        ////                RS_OM_OrderRelease mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == cummlObj.parentModel_Code && a.Model_Code != a.partno && a.Shop_ID == 2 && a.UToken == tokenId).FirstOrDefault();
        ////                string transModel = "";
        ////                string transSeries = "";
        ////                if (mmTransModelObj != null)
        ////                {
        ////                    transModel = mmTransModelObj.partno;
        ////                    transSeries = mmTransModelObj.RS_Series.Series_Description;
        ////                }
        ////                cummlObj.Model_Code = cummlObj.Model_Code.Trim();
        ////                cummlObj.parentModel_Code = transModel;
        ////                cummlObj.parentSeries = transSeries;
        ////            }
        ////            ViewBag.SequenceShop = "Hydraulic";
        ////        }
        ////        else
        ////        {
        ////            orderSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
        ////                                orderby or.RSN ascending
        ////                                select new CummulativeFields()
        ////                                {
        ////                                    Row_ID = or.Row_ID,
        ////                                    Order_No = or.Order_No,
        ////                                    Model_Code = or.partno.Trim(),
        ////                                    Series = or.RS_Series.Series_Description,
        ////                                    parentModel_Code = or.Model_Code,
        ////                                    //parentSeries = db.RS_Model_Master.Where(a => a.Model_Code == or.Model_Code).FirstOrDefault().RS_Series == null ? "" : db.RS_Model_Master.Where(a => a.Model_Code == or.Model_Code).FirstOrDefault().RS_Series.Series_Description,
        ////                                    Inserted_Date = or.Inserted_Date,
        ////                                    remarks = or.Remarks,
        ////                                    orderType = or.Order_Type,
        ////                                    Planned_Shift_ID = or.Planned_Shift_ID

        ////                                }).ToList();
        ////            ViewBag.SequenceShop = "Others";
        ////        }

        ////        //DateTime today = Convert.ToDateTime("2016-05-21").Date;
        ////        DateTime today = DateTime.Today;
        ////        List<CummulativeFields> orderStartedObj = new List<CummulativeFields>();
        ////        string chkModel_Code = "";
        ////        string chkSeries = "";
        ////        string chkParentModel = "";
        ////        string chkParentSeries = "";
        ////        int startedQty = 0;
        ////        int cummlQty = 0;
        ////        int releasedQty = 0;
        ////        int holdedQty = 0;

        ////        List<CummulativeFields> plannedOrdersDataObj = new List<CummulativeFields>();
        ////        if (Is_Main)//Tractor Shop
        ////        {
        ////            RS_OM_OrderRelease mmEngModelObj = new RS_OM_OrderRelease();
        ////            RS_OM_OrderRelease mmTransModelObj = new RS_OM_OrderRelease();
        ////            orderStartedObj = db.GetTracterOrderDetails(shopID, plannedDate).OrderBy(a => a.Cumul)
        ////                                                    .Select(a => new CummulativeFields()
        ////                                                    {
        ////                                                        Model_Code = a.Model_Code,
        ////                                                        //Series = a.Series_Description,
        ////                                                        Color_Name = a.Colour_Desc,
        ////                                                        PlatformName = a.Platform_Name,
        ////                                                        Order_Status = a.Order_Status,
        ////                                                        Qty = a.Cnt,
        ////                                                        CummlQty = a.Cumul,
        ////                                                        remarks = a.Remarks,
        ////                                                        orderType = a.Order_Type
        ////                                                    }).ToList();
        ////            foreach (CummulativeFields cumFieldsObj in orderStartedObj)
        ////            {
        ////                ////added by mukesh
        ////                //{
        ////                //    chkModel_Code = cumFieldsObj.Model_Code;

        ////                //}//
        ////                //chkModel_Code = cumFieldsObj.Model_Code;

        ////                if (!chkModel_Code.Equals(cumFieldsObj.Model_Code, StringComparison.CurrentCultureIgnoreCase))
        ////                {
        ////                    if (!String.IsNullOrWhiteSpace(chkModel_Code))
        ////                    {
        ////                        CummulativeFields CumlObj = new CummulativeFields();
        ////                        //here is the problem (due to change in cumm )
        ////                        //modified by mukesh
        ////                        CumlObj.Model_Code = chkModel_Code;

        ////                        //CumlObj.Model_Code = chkModel_Code;
        ////                        CumlObj.Series = chkSeries;
        ////                        CumlObj.StartedQty = startedQty;
        ////                        CumlObj.HoldQty = holdedQty;
        ////                        CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
        ////                        CumlObj.EngineModelCode = chkParentModel;
        ////                        CumlObj.TransmissionSeries = chkParentSeries;
        ////                        CumlObj.CummlQty = cummlQty;
        ////                        CumlObj.Color_Name = cumFieldsObj.Color_Name;
        ////                        CumlObj.orderType = cumFieldsObj.orderType;
        ////                        CumlObj.Planned_Shift_ID = cumFieldsObj.Planned_Shift_ID;
        ////                        //CumlObj.orderType = db.RS_OM_OrderRelease.Where()
        ////                        mmEngModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 1 && a.UToken == CumlObj.UToken).FirstOrDefault();
        ////                        mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

        ////                        if (mmEngModelObj != null)
        ////                        {
        ////                            CumlObj.EngineModelCode = mmEngModelObj.partno;
        ////                        }
        ////                        if (mmTransModelObj != null)
        ////                        {
        ////                            CumlObj.TransmissionSeries = mmTransModelObj.RS_Series.Series_Description;
        ////                        }

        ////                        plannedOrdersDataObj.Add(CumlObj);
        ////                        startedQty = 0;
        ////                        holdedQty = 0;
        ////                        releasedQty = 0;
        ////                    }

        ////                    switch (cumFieldsObj.Order_Status)
        ////                    {
        ////                        case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                        case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                        default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                    }
        ////                    chkModel_Code = cumFieldsObj.Model_Code;
        ////                    chkSeries = cumFieldsObj.Series;
        ////                    chkParentModel = cumFieldsObj.EngineModelCode;
        ////                    chkParentSeries = cumFieldsObj.TransmissionSeries;

        ////                    cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);

        ////                }
        ////                else
        ////                {
        ////                    switch (cumFieldsObj.Order_Status)
        ////                    {
        ////                        case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                        case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                        default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                    }
        ////                    cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
        ////                }
        ////                if (cummlQty >= orderStartedObj.Last().CummlQty)
        ////                {
        ////                    CummulativeFields CumlObj = new CummulativeFields();

        ////                    CumlObj.Model_Code = chkModel_Code;
        ////                    CumlObj.Series = chkSeries;
        ////                    CumlObj.StartedQty = startedQty;
        ////                    CumlObj.HoldQty = holdedQty;
        ////                    CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
        ////                    CumlObj.EngineModelCode = chkParentModel;
        ////                    CumlObj.TransmissionSeries = chkParentSeries;
        ////                    CumlObj.CummlQty = cummlQty;
        ////                    CumlObj.Color_Name = cumFieldsObj.Color_Name;
        ////                    CumlObj.orderType = cumFieldsObj.orderType;
        ////                    CumlObj.Planned_Shift_ID = cumFieldsObj.Planned_Shift_ID;
        ////                    mmEngModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 1).FirstOrDefault();
        ////                    mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2).FirstOrDefault();

        ////                    if (mmEngModelObj != null)
        ////                    {
        ////                        CumlObj.EngineModelCode = mmEngModelObj.partno;
        ////                    }
        ////                    if (mmTransModelObj != null)
        ////                    {
        ////                        CumlObj.TransmissionSeries = mmTransModelObj.RS_Series.Series_Description;
        ////                    }
        ////                    plannedOrdersDataObj.Add(CumlObj);
        ////                }

        ////            }
        ////        }
        ////        else
        ////        {
        ////            orderStartedObj = db.GetShopWiseDetails(shopID, plannedDate).OrderBy(a => a.Cumul)
        ////                                                     .Select(a => new CummulativeFields()
        ////                                                     {
        ////                                                         Model_Code = a.Model_Code,
        ////                                                         Series = a.Child_Series,
        ////                                                         parentModel_Code = a.Parent_Model,
        ////                                                         parentSeries = a.Parent_Series,
        ////                                                         Order_Status = a.Order_Status,
        ////                                                         Qty = a.Cnt,
        ////                                                         CummlQty = a.Cumul,
        ////                                                         remarks = a.Remarks
        ////                                                     }).ToList();
        ////            List<string> remarksList = new List<string>();
        ////            foreach (CummulativeFields cumFieldsObj in orderStartedObj)
        ////            {
        ////                if (!chkModel_Code.Equals(cumFieldsObj.Model_Code, StringComparison.CurrentCultureIgnoreCase))
        ////                {
        ////                    if (!String.IsNullOrWhiteSpace(chkModel_Code))
        ////                    {
        ////                        CummulativeFields CumlObj = new CummulativeFields();

        ////                        CumlObj.Model_Code = chkModel_Code;
        ////                        CumlObj.Series = chkSeries;
        ////                        CumlObj.StartedQty = startedQty;
        ////                        CumlObj.HoldQty = holdedQty;
        ////                        CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
        ////                        CumlObj.parentModel_Code = chkParentModel;
        ////                        CumlObj.parentSeries = chkParentSeries;
        ////                        CumlObj.Color_Name = cumFieldsObj.Color_Name;//added by mukesh
        ////                        CumlObj.CummlQty = cummlQty;
        ////                        CumlObj.remarks = String.Join(",", remarksList.ToArray());
        ////                        CumlObj.orderType = cumFieldsObj.orderType;
        ////                        CumlObj.Planned_Shift_ID = cumFieldsObj.Planned_Shift_ID;
        ////                        if (shopName.Contains("Hydraulic"))
        ////                        {
        ////                            RS_OM_OrderRelease mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == chkParentModel && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

        ////                            if (mmTransModelObj != null)
        ////                            {
        ////                                CumlObj.parentModel_Code = mmTransModelObj.partno;
        ////                                CumlObj.parentSeries = mmTransModelObj.RS_Series.Series_Description;
        ////                            }
        ////                        }

        ////                        plannedOrdersDataObj.Add(CumlObj);
        ////                        startedQty = 0;
        ////                        holdedQty = 0;
        ////                        releasedQty = 0;
        ////                        remarksList.Clear();
        ////                    }
        ////                    switch (cumFieldsObj.Order_Status)
        ////                    {
        ////                        case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                        case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                        default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                    }
        ////                    chkModel_Code = cumFieldsObj.Model_Code;
        ////                    chkSeries = cumFieldsObj.Series;
        ////                    chkParentModel = cumFieldsObj.parentModel_Code;
        ////                    chkParentSeries = cumFieldsObj.parentSeries;
        ////                    cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
        ////                    if (cumFieldsObj.remarks != null)
        ////                    {
        ////                        if (remarksList.Contains(cumFieldsObj.remarks) == false)
        ////                        {
        ////                            remarksList.Add(cumFieldsObj.remarks.Trim());
        ////                        }
        ////                    }

        ////                }
        ////                else
        ////                {
        ////                    if (remarksList.Contains(cumFieldsObj.remarks) == false)
        ////                    {
        ////                        remarksList.Add(cumFieldsObj.remarks.Trim());
        ////                    }
        ////                    switch (cumFieldsObj.Order_Status)
        ////                    {
        ////                        case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                        case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                        default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
        ////                    }
        ////                    cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
        ////                }
        ////                if (cummlQty >= orderStartedObj.Last().CummlQty)
        ////                {
        ////                    if (cumFieldsObj.remarks != null)
        ////                    {
        ////                        if (remarksList.Contains(cumFieldsObj.remarks) == false)
        ////                        {
        ////                            remarksList.Add(cumFieldsObj.remarks.Trim());
        ////                        }
        ////                    }
        ////                    CummulativeFields CumlObj = new CummulativeFields();

        ////                    CumlObj.Model_Code = chkModel_Code;
        ////                    CumlObj.Series = chkSeries;
        ////                    CumlObj.StartedQty = startedQty;
        ////                    CumlObj.HoldQty = holdedQty;
        ////                    CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
        ////                    CumlObj.parentModel_Code = chkParentModel;
        ////                    CumlObj.Color_Name = cumFieldsObj.Color_Name; //added by mukesh
        ////                    CumlObj.parentSeries = chkParentSeries;
        ////                    CumlObj.CummlQty = cummlQty;
        ////                    CumlObj.remarks = String.Join(",", remarksList.ToArray());
        ////                    CumlObj.orderType = cumFieldsObj.orderType;
        ////                    CumlObj.Planned_Shift_ID = cumFieldsObj.Planned_Shift_ID;
        ////                    //added by mukesh
        ////                    //CumlObj.Color_Name = orderStartedObj.Last().Color_Name;
        ////                    //

        ////                    if (shopName.Contains("Hydraulic"))
        ////                    {
        ////                        RS_OM_OrderRelease mmTransModelObj = db.RS_OM_OrderRelease.Where(a => a.Model_Code == cumFieldsObj.parentModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

        ////                        if (mmTransModelObj != null)
        ////                        {
        ////                            CumlObj.parentModel_Code = mmTransModelObj.partno;
        ////                            CumlObj.parentSeries = mmTransModelObj.RS_Series.Series_Description;
        ////                        }
        ////                    }
        ////                    remarksList.Clear();
        ////                    plannedOrdersDataObj.Add(CumlObj);
        ////                }
        ////            }//FOR LOOP ENDING

        ////            //orderStartedObj = (from ol in db.RS_OM_Order_List
        ////            //                   join or in db.RS_OM_OrderRelease on ol.Order_No equals or.Order_No
        ////            //                   where ol.Plant_ID == plantID && ol.Shop_ID == shopID && ol.Line_ID == lineID
        ////            //                   && DbFunctions.TruncateTime(ol.Inserted_Date) == today && ol.partno == or.partno
        ////            //                   join mm in db.RS_Model_Master on or.Model_Code equals mm.Model_Code
        ////            //                   orderby ol.Inserted_Date ascending
        ////            //                   select new CummulativeFields()
        ////            //                   {
        ////            //                       orderNo = ol.Order_ID,
        ////            //                       Model_Code = ol.partno,
        ////            //                       Series = ol.RS_Series.Series_Description,
        ////            //                       parentModel_Code = or.Model_Code,
        ////            //                       parentSeries = mm.RS_Series.Series_Description
        ////            //                   }).Distinct().ToList();


        ////        }

        ////        if (blockAfterQty > 0)
        ////        {
        ////            db.RS_OM_OrderRelease.Where(a => a.Line_ID == lineID && a.Order_Status == "Release")
        ////                            .OrderBy(a => a.RSN).Skip(blockAfterQty).ToList()
        ////                            .ForEach(a => a.Is_Blocked = true);
        ////            db.SaveChanges();
        ////        }



        ////        List<CummulativeFields> plannedStartedOrdersDataObj = new List<CummulativeFields>();
        ////        //int? counter = 0;

        ////        ViewBag.blockAfterQty = blockAfterQty;
        ////        ////
        ////        List<CummulativeFields> orderSequencePlatformSpecific = new List<CummulativeFields>();


        ////        foreach (var order in orderSequenceObj)
        ////        {

        ////            var orderLineId = (from pf in db.RS_OM_Platform
        ////                               join mm in db.RS_Model_Master.Where(m => m.Model_Code == order.Model_Code)
        ////                               on pf.Platform_ID equals mm.Platform_Id
        ////                               where pf.Plant_ID == plantid
        ////                               select new
        ////                               {
        ////                                   pf.Line_ID
        ////                               }).FirstOrDefault();

        ////            int lineIDs = Convert.ToInt32(orderLineId.Line_ID);
        ////            if (lineIDs == lineID)
        ////            {
        ////                if (db.RS_OM_Platform.Find(platformIdLocked).Platform_Name.Equals("U321", StringComparison.CurrentCultureIgnoreCase))
        ////                {
        ////                    order.Locked = db.RS_OM_U321_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == order.Order_No && m.Is_Locked==true);
        ////                }
        ////                else if (db.RS_OM_Platform.Find(platformIdLocked).Platform_Name.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
        ////                {
        ////                    order.Locked = db.RS_OM_S201_Tactsheet_Orders.Any(m => m.Plant_ID == plantid && m.Shop_ID == shopID && m.Order_No == order.Order_No && m.Is_Locked == true);
        ////                }

        ////                orderSequencePlatformSpecific.Add(order);

        ////            }

        ////        }
        ////        ////test by mukesh for started table status
        ////        List<CummulativeFields> startedOrderInfo = new List<CummulativeFields>();
        ////        startedOrderInfo = orderSequencePlatformSpecific;
        ////        foreach (var item in startedOrderInfo)
        ////        {
        ////            var orderLineId = (from pf in db.RS_OM_Platform
        ////                               join mm in db.RS_Model_Master.Where(m => m.Model_Code == item.Model_Code)
        ////                               on pf.Platform_ID equals mm.Platform_Id
        ////                               where pf.Plant_ID == plantid
        ////                               select new
        ////                               {
        ////                                   pf.Line_ID
        ////                               }).FirstOrDefault();

        ////            int lineIDs = Convert.ToInt32(orderLineId.Line_ID);

        ////            if (lineIDs == lineID)
        ////            {
        ////                var res = plannedStartedOrdersDataObj.Where(c => c.Model_Code == item.Model_Code && c.orderType == item.orderType).FirstOrDefault();

        ////                if (res == null)
        ////                {
        ////                    item.CummlQty = item.PlannedQty;
        ////                    plannedStartedOrdersDataObj.Add(item);


        ////                }
        ////                else
        ////                {
        ////                    res.StartedQty += item.StartedQty;
        ////                    res.HoldQty += item.HoldQty;
        ////                    res.PlannedQty += item.PlannedQty;
        ////                    res.CummlQty = res.PlannedQty;
        ////                }
        ////                //counter = item.CummlQty;
        ////            }
        ////        }
        ////        ViewBag.startedOrders = GetOrderDetailGroupByOrderType(plantid, shopID, lineID, plannedDate);//plannedStartedOrdersDataObj;
        ////        ////
        ////        return PartialView("PVOrderSequence", orderSequencePlatformSpecific.OrderBy(m => m.RSN));
        ////    }
        ////    catch (Exception exp)
        ////    {
        ////        generalHelper.addControllerException(exp, "OMOrderPlanningController", "loadOrderSequence(Post)", ((FDSession)this.Session["FDSession"]).userId);
        ////    }
        ////    finally
        ////    {
        ////        generalHelper.logUserActivity(shopID, lineID, "PPC Module", "Show Sequence(Blocked Qty = " + blockAfterQty + ")", nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
        ////    }
        ////    return null;
        ////}


        class PlannedData
        {
            public decimal Shop_ID { get; set; }
            public string Shop_Name { get; set; }
            public int? Planned_Qty { get; set; }
        }
        public ActionResult getPlannedValue(int plantID)
        {
            try
            {
                DateTime Today = DateTime.Today;
                var plannedQtyObj = (from a in db.RS_Shops
                                     join b in db.RS_OM_PPC_Daily_Plan on a.Shop_ID equals b.Shop_ID into ab
                                     from b in ab.DefaultIfEmpty()
                                     where a.Plant_ID == plantID && b.Plan_Date == Today
                                     select new PlannedData()
                                     {
                                         Shop_ID = a.Shop_ID,
                                         Shop_Name = a.Shop_Name,
                                         Planned_Qty = b.Planned_Qty
                                     }).ToList();

                return Json(plannedQtyObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult checkifUnHoldLock(decimal lineID, decimal PlatformId)
        {
            JsonData jsondataObj = new JsonData();
            //var lineID = GetLineIdByPlatform(PlatformId);
            try
            {
                if (generalHelper.isTableLock(Convert.ToInt16(lineID), "RS_OM_OrderRelease"))
                {
                    jsondataObj.result = true;
                    //unlock the table
                    db.RS_Lock_Table
                                  .Where(a => a.Line_ID == lineID && a.Updated_Date == null && a.Table_Name == "RS_OM_OrderRelease")
                                  .ToList()
                                  .ForEach(a =>
                                  {
                                      a.Updated_Date = DateTime.Now;
                                  });
                    db.SaveChanges();
                }
                else
                {
                    jsondataObj.result = false;
                }
                jsondataObj.message = "";
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "checkifUnHoldLock(lineID: " + lineID + ")", ((FDSession)this.Session["FDSession"]).userId);
                jsondataObj.result = true;
                jsondataObj.message = "Exception Occurred : " + exp.Message;
            }
            return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UnLockUnholdTable(int? PlatformId)
        {
            try
            {
                var lineID = GetLineIdByPlatform(PlatformId);
                //unlock the table
                db.RS_Lock_Table.Where(a => a.Line_ID == lineID && a.Updated_Date == null && a.Table_Name == "RS_OM_OrderRelease")
                                  .ToList()
                                  .ForEach(a =>
                                  {
                                      a.Updated_Date = DateTime.Now;
                                  });
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "UnLockUnholdTable(: " + PlatformId + ")", ((FDSession)this.Session["FDSession"]).userId);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult saveOrderSequence(List<int> sequenceList, decimal plantID, decimal shopID, decimal PlatformId, List<string> remarks, int ShiftProdCount, int ShiftID, List<int> SelectedSequenceArr)
        //{
        //    Session["tactsheetFlag"] = false;
        //    DateTime nowTime = DateTime.Now;
        //    string moduleName = "PPC Module";
        //    string activityRemarks = "Save Sequence Change";
        //    var lineID = GetLineIdByPlatform(PlatformId);
        //    //DateTime dateVal = DateTime.ParseExact(PlannedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    DateTime dateVal = DateTime.Now;
        //    try
        //    {
        //        JSONData jsonObj = new JSONData();
        //        var old_OrderList = db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
        //                      .AsEnumerable()
        //                      .OrderBy(or => or.RSN)
        //                      .Select(a => a.Row_ID).ToList();

        //        ////checking for Locked order

        //        if (db.RS_OM_Platform.Find(PlatformId).Platform_Name.Equals("U321", StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            //delete already scheduled order in same shift

        //            var lockedOrders = (from or in db.RS_OM_OrderRelease
        //                                join tact in db.RS_OM_U321_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
        //                                where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
        //                                //&& tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
        //                                && tact.Is_Locked == true
        //                                orderby tact.RSN
        //                                select new
        //                                {
        //                                    or.Row_ID,
        //                                    tact.Shift_ID
        //                                }).ToList();
        //            var shiftlockedOrders = (from or in db.RS_OM_OrderRelease
        //                                     join tact in db.RS_OM_U321_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
        //                                     where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
        //                                     && tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
        //                                     && tact.Is_Locked == true && tact.Shift_ID == ShiftID
        //                                     orderby tact.RSN
        //                                     select new
        //                                     {
        //                                         or.Row_ID,
        //                                         tact.Shift_ID
        //                                     }).ToList();
        //            var OtherlockedOrders = (from or in db.RS_OM_OrderRelease
        //                                     join tact in db.RS_OM_U321_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
        //                                     where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
        //                                     //&& tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
        //                                     && tact.Is_Locked == true 
        //                                     //&& tact.Shift_ID != ShiftID
        //                                     orderby tact.RSN
        //                                     select new
        //                                     {
        //                                         or.Row_ID,
        //                                         tact.Shift_ID
        //                                     }).ToList();
        //            int j = 0;
        //            foreach (var or in OtherlockedOrders)
        //            {
        //                    //if (or.Row_ID == item && or.Shift_ID != ShiftID)

        //                    if (or.Row_ID != SelectedSequenceArr[j++])
        //                    {
        //                        jsonObj.type = "lockedOrder";
        //                        jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

        //                        return Json(jsonObj, JsonRequestBehavior.AllowGet);
        //                    }

        //            }

        //            //var currentShiftOrder = db.RS_OM_U321_Tactsheet_Orders.Where(m=>m.)
        //            for (int s = 0; s < shiftlockedOrders.Count; s++)
        //            {
        //                if ((shiftlockedOrders[s].Row_ID != SelectedSequenceArr[s]) || (ShiftProdCount < shiftlockedOrders.Count() && shiftlockedOrders.Count() > 0))
        //                {

        //                    jsonObj.type = "lockedOrder";
        //                    jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

        //                    return Json(jsonObj, JsonRequestBehavior.AllowGet);
        //                }
        //                else if ((shiftlockedOrders[s].Shift_ID != ShiftID))
        //                {
        //                    jsonObj.type = "lockedOrder";
        //                    jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

        //                    return Json(jsonObj, JsonRequestBehavior.AllowGet);
        //                }

        //                //foreach()
        //            }
        //        }
        //        else if (db.RS_OM_Platform.Find(PlatformId).Platform_Name.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            //delete already scheduled order in same shift

        //            var lockedOrders = (from or in db.RS_OM_OrderRelease
        //                                join tact in db.RS_OM_S201_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
        //                                where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
        //                                //&& tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
        //                                && tact.Is_Locked == true
        //                                orderby tact.RSN
        //                                select new
        //                                {
        //                                    or.Row_ID,
        //                                    tact.Shift_ID
        //                                }).ToList();
        //            var shiftlockedOrders = (from or in db.RS_OM_OrderRelease
        //                                     join tact in db.RS_OM_S201_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
        //                                     where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
        //                                     && tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
        //                                     && tact.Is_Locked == true && tact.Shift_ID == ShiftID
        //                                     orderby tact.RSN
        //                                     select new
        //                                     {
        //                                         or.Row_ID,
        //                                         tact.Shift_ID
        //                                     }).ToList();
        //            var OtherlockedOrders = (from or in db.RS_OM_OrderRelease
        //                                     join tact in db.RS_OM_S201_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
        //                                     where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
        //                                     //&& tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
        //                                     && tact.Is_Locked == true 
        //                                     //&& tact.Shift_ID != ShiftID
        //                                     orderby tact.RSN
        //                                     select new
        //                                     {
        //                                         or.Row_ID,
        //                                         tact.Shift_ID
        //                                     }).ToList();
        //            int j = 0;
        //            foreach (var or in OtherlockedOrders)
        //            {
        //                //if (or.Row_ID == item && or.Shift_ID != ShiftID)

        //                if (or.Row_ID != SelectedSequenceArr[j++])
        //                {
        //                    jsonObj.type = "lockedOrder";
        //                    jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

        //                    return Json(jsonObj, JsonRequestBehavior.AllowGet);
        //                }

        //            }

        //            //var currentShiftOrder = db.RS_OM_U321_Tactsheet_Orders.Where(m=>m.)
        //            for (int s = 0; s < shiftlockedOrders.Count; s++)
        //            {
        //                if ((shiftlockedOrders[s].Row_ID != SelectedSequenceArr[s]) || (ShiftProdCount < shiftlockedOrders.Count() && shiftlockedOrders.Count() > 0))
        //                {

        //                    jsonObj.type = "lockedOrder";
        //                    jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

        //                    return Json(jsonObj, JsonRequestBehavior.AllowGet);
        //                }
        //                else if ((shiftlockedOrders[s].Shift_ID != ShiftID))
        //                {
        //                    jsonObj.type = "lockedOrder";
        //                    jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

        //                    return Json(jsonObj, JsonRequestBehavior.AllowGet);
        //                }

        //                //foreach()
        //            }
        //        }



        //        ////
        //        decimal cntrRSN = 0;

        //        if (old_OrderList.Count > 0)
        //        {
        //            cntrRSN = db.RS_OM_OrderRelease.Find(old_OrderList[0]).RSN;
        //        }

        //        int i = 0;


        //        //added for saving reassigning shifts to changed sequence
        //        DateTime date = DateTime.Now;
        //        TimeSpan currentTime = date.TimeOfDay;

        //        var shiftPlan = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == shopID && m.Line_ID == lineID && m.Plan_Date == date.Date).Select(m => m).OrderBy(m => m.Shift_ID).ToList();


        //        using (REIN_SOLUTIONEntities dc = new REIN_SOLUTIONEntities())
        //        {
        //            int rsn = 1;
        //            foreach (var rID in sequenceList)
        //            {
        //                var newOrderObj = dc.RS_OM_OrderRelease.Where(a => a.Row_ID.Equals(rID)).FirstOrDefault();
        //                if (newOrderObj != null)
        //                {
        //                    newOrderObj.RSN = rsn++;
        //                    newOrderObj.Remarks = remarks[i];
        //                    newOrderObj.Is_Edited = true;
        //                    newOrderObj.Updated_Date = DateTime.Now;
        //                    newOrderObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
        //                    newOrderObj.Is_Sequenced = true;
        //                    // dc.Entry(newOrderObj).State = EntityState.Modified;
        //                    i++;
        //                }
        //                else
        //                {
        //                    i++;
        //                    continue;
        //                }
        //            }
        //            dc.SaveChanges();
        //        }

        //        updatePlannedOrders(shopID);
        //        Session["tactsheetFlag"] = true;
        //        return Json("Success", JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exp)
        //    {
        //        Session["tactsheetFlag"] = false;
        //        try { updatePlannedOrders(shopID); } catch (Exception ex) { }

        //        while (exp.InnerException != null)
        //        {
        //            exp = exp.InnerException;
        //        }
        //        generalHelper.addControllerException(exp, "OMOrderPlanningController", "saveOrderSequence(Post)", ((FDSession)this.Session["FDSession"]).userId);
        //    }
        //    finally
        //    {
        //        generalHelper.logUserActivity(shopID, lineID, moduleName, activityRemarks, nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
        //    }
        //    Session["tactsheetFlag"] = false;
        //    return Json("Error", JsonRequestBehavior.AllowGet);

        //}
        [HttpPost]
        public ActionResult saveOrderSequence(List<int> sequenceList, decimal plantID, decimal shopID, decimal lineID, decimal PlatformId, List<string> remarks, int ShiftProdCount, int ShiftID, List<int> SelectedSequenceArr)
        {
            Session["tactsheetFlag"] = false;
            DateTime nowTime = DateTime.Now;
            string moduleName = "PPC Module";
            string activityRemarks = "Save Sequence Change";
            //var lineID = GetLineIdByPlatform(PlatformId);
            //DateTime dateVal = DateTime.ParseExact(PlannedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateVal = DateTime.Now;
            try
            {
                JSONData jsonObj = new JSONData();
                var old_OrderList = db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
                              .AsEnumerable()
                              .OrderBy(or => or.RSN)
                              .Select(a => a.Row_ID).ToList();

                ////checking for Locked order

                //if (db.RS_OM_Platform.Find(PlatformId).Platform_Name.Equals("U321", StringComparison.CurrentCultureIgnoreCase))
                //{
                //    //delete already scheduled order in same shift

                //    var lockedOrders = (from or in db.RS_OM_OrderRelease
                //                        join tact in db.RS_OM_U321_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
                //                        where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
                //                        //&& tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
                //                        && tact.Is_Locked == true
                //                        orderby tact.RSN
                //                        select new
                //                        {
                //                            or.Row_ID,
                //                            tact.Shift_ID
                //                        }).ToList();
                //    var shiftlockedOrders = (from or in db.RS_OM_OrderRelease
                //                             join tact in db.RS_OM_U321_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
                //                             where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
                //                             && tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
                //                             && tact.Is_Locked == true && tact.Shift_ID == ShiftID
                //                             orderby tact.RSN
                //                             select new
                //                             {
                //                                 or.Row_ID,
                //                                 tact.Shift_ID
                //                             }).ToList();
                //    var OtherlockedOrders = (from or in db.RS_OM_OrderRelease
                //                             join tact in db.RS_OM_U321_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
                //                             where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
                //                             //&& tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
                //                             && tact.Is_Locked == true
                //                             //&& tact.Shift_ID != ShiftID
                //                             orderby tact.RSN
                //                             select new
                //                             {
                //                                 or.Row_ID,
                //                                 tact.Shift_ID
                //                             }).ToList();
                //    int j = 0;
                //    //foreach (var or in OtherlockedOrders)
                //    //{
                //    //    //if (or.Row_ID == item && or.Shift_ID != ShiftID)

                //    //    if (or.Row_ID != SelectedSequenceArr[j++])
                //    //    {
                //    //        jsonObj.type = "lockedOrder";
                //    //        jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

                //    //        return Json(jsonObj, JsonRequestBehavior.AllowGet);
                //    //    }

                //    //}

                //    //var currentShiftOrder = db.RS_OM_U321_Tactsheet_Orders.Where(m=>m.)
                //    for (int s = 0; s < shiftlockedOrders.Count; s++)
                //    {
                //        if ((shiftlockedOrders[s].Row_ID != SelectedSequenceArr[s]) || (ShiftProdCount < shiftlockedOrders.Count() && shiftlockedOrders.Count() > 0))
                //        {

                //            jsonObj.type = "lockedOrder";
                //            jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

                //            return Json(jsonObj, JsonRequestBehavior.AllowGet);
                //        }
                //        else if ((shiftlockedOrders[s].Shift_ID != ShiftID))
                //        {
                //            jsonObj.type = "lockedOrder";
                //            jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

                //            return Json(jsonObj, JsonRequestBehavior.AllowGet);
                //        }

                //        //foreach()
                //    }
                //}
                //else if (db.RS_OM_Platform.Find(PlatformId).Platform_Name.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
                //{
                //    //delete already scheduled order in same shift

                //    var lockedOrders = (from or in db.RS_OM_OrderRelease
                //                        join tact in db.RS_OM_S201_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
                //                        where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
                //                        //&& tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
                //                        && tact.Is_Locked == true
                //                        orderby tact.RSN
                //                        select new
                //                        {
                //                            or.Row_ID,
                //                            tact.Shift_ID
                //                        }).ToList();
                //    var shiftlockedOrders = (from or in db.RS_OM_OrderRelease
                //                             join tact in db.RS_OM_S201_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
                //                             where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
                //                             && tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
                //                             && tact.Is_Locked == true && tact.Shift_ID == ShiftID
                //                             orderby tact.RSN
                //                             select new
                //                             {
                //                                 or.Row_ID,
                //                                 tact.Shift_ID
                //                             }).ToList();
                //    var OtherlockedOrders = (from or in db.RS_OM_OrderRelease
                //                             join tact in db.RS_OM_S201_Tactsheet_Orders on or.Order_No.ToUpper() equals tact.Order_No
                //                             where or.Order_Status == "Release" && or.Order_Start == false && tact.Plant_ID == plantID && tact.Shop_ID == shopID
                //                             //&& tact.Planned_Date.Year == dateVal.Year && tact.Planned_Date.Month == dateVal.Month && tact.Planned_Date.Day == dateVal.Day
                //                             && tact.Is_Locked == true
                //                             //&& tact.Shift_ID != ShiftID
                //                             orderby tact.RSN
                //                             select new
                //                             {
                //                                 or.Row_ID,
                //                                 tact.Shift_ID
                //                             }).ToList();
                //    int j = 0;
                //    foreach (var or in OtherlockedOrders)
                //    {
                //        //if (or.Row_ID == item && or.Shift_ID != ShiftID)

                //        if (or.Row_ID != SelectedSequenceArr[j++])
                //        {
                //            jsonObj.type = "lockedOrder";
                //            jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

                //            return Json(jsonObj, JsonRequestBehavior.AllowGet);
                //        }

                //    }

                //    //var currentShiftOrder = db.RS_OM_U321_Tactsheet_Orders.Where(m=>m.)
                //    for (int s = 0; s < shiftlockedOrders.Count; s++)
                //    {
                //        if ((shiftlockedOrders[s].Row_ID != SelectedSequenceArr[s]) || (ShiftProdCount < shiftlockedOrders.Count() && shiftlockedOrders.Count() > 0))
                //        {

                //            jsonObj.type = "lockedOrder";
                //            jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

                //            return Json(jsonObj, JsonRequestBehavior.AllowGet);
                //        }
                //        else if ((shiftlockedOrders[s].Shift_ID != ShiftID))
                //        {
                //            jsonObj.type = "lockedOrder";
                //            jsonObj.message = "Cant Not Save Order Sequnece, Top " + lockedOrders.Count() + " Order are locked";

                //            return Json(jsonObj, JsonRequestBehavior.AllowGet);
                //        }

                //        //foreach()
                //    }
                //}



                ////
                decimal cntrRSN = 0;

                if (old_OrderList.Count > 0)
                {
                    cntrRSN = db.RS_OM_OrderRelease.Find(old_OrderList[0]).RSN;
                }

                int i = 0;


                //added for saving reassigning shifts to changed sequence
                DateTime date = DateTime.Now;
                TimeSpan currentTime = date.TimeOfDay;

                var shiftPlan = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == shopID && m.Line_ID == lineID && m.Plan_Date == date.Date).Select(m => m).OrderBy(m => m.Shift_ID).ToList();


                using (REIN_SOLUTIONEntities dc = new REIN_SOLUTIONEntities())
                {
                    int rsn = 1;
                    foreach (var rID in sequenceList)
                    {
                        var newOrderObj = dc.RS_OM_OrderRelease.Where(a => a.Row_ID.Equals(rID)).FirstOrDefault();
                        if (newOrderObj != null)
                        {
                            newOrderObj.RSN = rsn++;
                            newOrderObj.Remarks = remarks[i];
                            newOrderObj.Is_Edited = true;
                            newOrderObj.Updated_Date = DateTime.Now;
                            newOrderObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            newOrderObj.Is_Sequenced = true;
                            // dc.Entry(newOrderObj).State = EntityState.Modified;
                            i++;
                        }
                        else
                        {
                            i++;
                            continue;
                        }
                    }
                    dc.SaveChanges();
                }

                updatePlannedOrders(shopID);
                Session["tactsheetFlag"] = true;
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                Session["tactsheetFlag"] = false;
                try { updatePlannedOrders(shopID); } catch (Exception ex) { }

                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "saveOrderSequence(Post)", ((FDSession)this.Session["FDSession"]).userId);
            }
            finally
            {
                generalHelper.logUserActivity(shopID, lineID, moduleName, activityRemarks, nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
            }
            Session["tactsheetFlag"] = false;
            return Json("Error", JsonRequestBehavior.AllowGet);

        }

        private class JsonData
        {
            public Boolean result { get; set; }
            public string message { get; set; }
        }
        public ActionResult DeleteTractorOrder(int orderRowid, string remark)
        {
            JsonData jsondataObj = new JsonData();
            try
            {
                RS_OM_OrderRelease mmOrderRelease = db.RS_OM_OrderRelease.Find(orderRowid);
                string orderNo = mmOrderRelease.Order_No;
                //string splitOrder = (orderNo).Substring(orderNo.Length - 8);
                string tokenNo = mmOrderRelease.UToken;
                string tractorModelCode = mmOrderRelease.Model_Code;

                if (db.RS_OM_OrderRelease.Any(a => a.Model_Code == tractorModelCode && a.UToken == tokenNo && a.Order_Status == "Started"))
                {
                    jsondataObj.result = false;
                    jsondataObj.message = "Aggregate Order has been started.Cannot Delete Tractor OrderNo. : " + orderNo + " | Model Code : " + tractorModelCode;
                }
                else
                {
                    List<RS_OM_OrderRelease> orderObjList = db.RS_OM_OrderRelease.Where(a => a.Model_Code == tractorModelCode && a.UToken == tokenNo && a.Order_Status != "Started").ToList();
                    foreach (RS_OM_OrderRelease orderRlsObj in orderObjList)
                    {
                        IEnumerable<RS_OM_Planned_Orders> omOrdrPlanned = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == orderRlsObj.Row_ID);
                        RS_OM_Planned_Orders[] omOrdrPlannedToDelete = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == orderRlsObj.Row_ID).ToArray();
                        db.RS_OM_Planned_Orders.RemoveRange(omOrdrPlanned);
                        db.SaveChanges();

                        //for (int i = 0; i < omOrdrPlannedToDelete.Count();i++ )
                        //{
                        //    generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Planned_Orders", "Row_ID", omOrdrPlannedToDelete[i].Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                        //}

                        foreach (var item in omOrdrPlanned)
                        {
                            generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Planned_Orders", "Row_ID", item.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                        }

                        orderRlsObj.Is_Deleted = true;
                        orderRlsObj.Is_Edited = true;
                        orderRlsObj.Updated_Date = DateTime.Now;
                        orderRlsObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.Entry(orderRlsObj).State = EntityState.Modified;
                        db.SaveChanges();

                        db.RS_OM_OrderRelease.Remove(orderRlsObj);
                        db.SaveChanges();

                        generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_OrderRelease", "Row_ID", orderRlsObj.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                        RS_OM_Order_Remarks omOrderRmrks = new RS_OM_Order_Remarks();
                        omOrderRmrks.Order_ID = orderRlsObj.Row_ID;
                        omOrderRmrks.Remark_Category = "Delete";
                        omOrderRmrks.Remark_Msg = remark;
                        omOrderRmrks.Inserted_UserID = ((FDSession)this.Session["FDSession"]).userId;
                        omOrderRmrks.Inserted_Time = DateTime.Now;
                        db.RS_OM_Order_Remarks.Add(omOrderRmrks);
                        db.SaveChanges();
                    }
                    jsondataObj.result = true;
                    jsondataObj.message = "Success";
                }
            }
            catch (Exception exp)
            {
                jsondataObj.result = false;
                jsondataObj.message = "Exception : " + exp.Message + " .Cannot Delete Tractor Order !";
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "DeleteTractorOrder()", ((FDSession)this.Session["FDSession"]).userId);
            }
            return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteOrder(int orderRowid, string remark)
        {
            JsonData jsondataObj = new JsonData();
            try
            {
                RS_OM_OrderRelease mmOrderRelease = db.RS_OM_OrderRelease.Find(orderRowid);
                string orderNo = mmOrderRelease.Order_No;

                if (mmOrderRelease.Order_Status == "Started")
                {
                    jsondataObj.result = false;
                    jsondataObj.message = "Cannot Delete Order : " + orderNo + " because its already Started";
                }
                else
                {

                    IEnumerable<RS_OM_Planned_Orders> omOrdrPlanned = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == orderRowid);
                    db.RS_OM_Planned_Orders.RemoveRange(omOrdrPlanned);
                    db.SaveChanges();

                    foreach (var item in omOrdrPlanned)
                    {
                        generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Planned_Orders", "Row_ID", item.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                    }
                    var plannedorder = mmOrderRelease.Plant_OrderNo;
                    mmOrderRelease.Is_Edited = true;
                    mmOrderRelease.Is_Deleted = true;
                    mmOrderRelease.Updated_Date = DateTime.Now;
                    mmOrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.Entry(mmOrderRelease).State = EntityState.Modified;
                    db.SaveChanges();
                    ////added by mukesh//// For updating ordercreation after deleting orders from sequencing form
                    {

                        RS_OM_Creation mmOmCreation = db.RS_OM_Creation.Where(x => x.Plant_OrderNo == mmOrderRelease.Plant_OrderNo).FirstOrDefault();
                        if (mmOmCreation != null)
                        {


                            mmOmCreation.Is_Edited = true;

                            //mmOmCreation.Order_Status = "Hold";
                            mmOmCreation.Qty--;
                            mmOmCreation.Release_Qty--;

                            mmOmCreation.Updated_Date = DateTime.Now;
                            mmOmCreation.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;

                            //for deleting order entry from order creation if qty
                            if (mmOmCreation.Release_Qty == 0)
                            {
                                mmOrderRelease.Is_Deleted = true;
                                db.RS_OM_Creation.Remove(mmOmCreation);
                            }
                            ////
                            db.Entry(mmOrderRelease).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //RS_OM_Planned_Orders plannedOrders = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == orderRowid).FirstOrDefault();
                        //if (plannedOrders != null)
                        //{
                        //    db.RS_OM_Planned_Orders.Remove(plannedOrders);
                        //    db.SaveChanges();

                        //    generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_OrderRelease", "Row_ID", plannedOrders.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                        //}

                        //RS_OM_Order_Remarks omOrderRmrks = new RS_OM_Order_Remarks();
                        //omOrderRmrks.Order_ID = orderRowid;
                        //omOrderRmrks.Remark_Category = "Hold";
                        //omOrderRmrks.Remark_Msg = remark;
                        //omOrderRmrks.Inserted_UserID = ((FDSession)this.Session["FDSession"]).userId;
                        //omOrderRmrks.Inserted_Time = DateTime.Now;
                        //db.RS_OM_Order_Remarks.Add(omOrderRmrks);
                        //db.SaveChanges();

                        //updatePlannedOrders(mmOrderRelease.Shop_ID);
                    }/////
                    db.RS_OM_OrderRelease.Remove(mmOrderRelease);
                    db.SaveChanges();
                    DateTime todaydate = DateTime.Now;
                    TimeSpan currentTime = todaydate.TimeOfDay;
                    var currentShiftID = mmOrderRelease.Planned_Shift_ID;
                    if (mmOrderRelease.Planned_Shift_ID != null)
                    {
                        //for adding unholded order at bottom of current shift and increaseing production count of current shift
                        var ppcDailyPlan = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == mmOrderRelease.Shop_ID && m.Line_ID == mmOrderRelease.Line_ID && m.Plan_Date == todaydate.Date && m.Shift_ID == currentShiftID).Select(m => m).FirstOrDefault();
                        ppcDailyPlan.Planned_Qty--;
                        ppcDailyPlan.Is_Edited = true;
                        ppcDailyPlan.Is_Edited = true;
                        ppcDailyPlan.Updated_Date = DateTime.Now;
                        ppcDailyPlan.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        db.Entry(ppcDailyPlan).State = EntityState.Modified;
                        db.SaveChanges();
                        ////
                    }
                    generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_OrderRelease", "Row_ID", mmOrderRelease.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                    RS_OM_Order_Remarks omOrderRmrks = new RS_OM_Order_Remarks();
                    omOrderRmrks.Order_ID = orderRowid;
                    omOrderRmrks.Remark_Category = "Delete";
                    omOrderRmrks.Remark_Msg = remark;
                    omOrderRmrks.Inserted_UserID = ((FDSession)this.Session["FDSession"]).userId;
                    omOrderRmrks.Inserted_Time = DateTime.Now;
                    db.RS_OM_Order_Remarks.Add(omOrderRmrks);
                    db.SaveChanges();

                    jsondataObj.result = true;
                    jsondataObj.message = "Success";
                }
            }
            catch (Exception exp)
            {
                jsondataObj.result = false;
                jsondataObj.message = "Exception : " + exp.Message + " .Cannot Delete Order !";
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "DeleteOrder()", ((FDSession)this.Session["FDSession"]).userId);
            }
            return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HoldTractorOrder(int orderRowid, string remark)
        {
            JsonData jsondataObj = new JsonData();
            try
            {
                //for current shift
                DateTime date = DateTime.Now;
                TimeSpan currentTime = date.TimeOfDay;

                ////
                RS_OM_OrderRelease mmOrderRelease = db.RS_OM_OrderRelease.Find(orderRowid);
                var currentShiftID = mmOrderRelease.Planned_Shift_ID;
                if (currentShiftID != null)
                {
                    //for adding unholded order at bottom of current shift and increaseing production count of current shift
                    var ppcDailyPlan = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == mmOrderRelease.Shop_ID && m.Line_ID == mmOrderRelease.Line_ID && m.Plan_Date == date.Date && m.Shift_ID == currentShiftID).Select(m => m).FirstOrDefault();
                    ppcDailyPlan.Planned_Qty--;
                    ppcDailyPlan.Is_Edited = true;
                    ppcDailyPlan.Is_Edited = true;
                    ppcDailyPlan.Updated_Date = DateTime.Now;
                    ppcDailyPlan.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.Entry(ppcDailyPlan).State = EntityState.Modified;
                    db.SaveChanges();
                    ////
                }
                mmOrderRelease.Is_Edited = true;
                mmOrderRelease.Hold_By_PPC = false;

                //mmOrderRelease.Planned_Shift_ID = null;
                mmOrderRelease.Order_Status = "Hold";
                mmOrderRelease.Remarks = remark;
                mmOrderRelease.Updated_Date = DateTime.Now;
                mmOrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.Entry(mmOrderRelease).State = EntityState.Modified;
                db.SaveChanges();

                RS_OM_Planned_Orders plannedOrders = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == orderRowid).FirstOrDefault();
                if (plannedOrders != null)
                {
                    db.RS_OM_Planned_Orders.Remove(plannedOrders);
                    db.SaveChanges();

                    generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_OrderRelease", "Row_ID", plannedOrders.Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                }

                RS_OM_Order_Remarks omOrderRmrks = new RS_OM_Order_Remarks();
                omOrderRmrks.Order_ID = orderRowid;
                omOrderRmrks.Remark_Category = "Hold";
                omOrderRmrks.Remark_Msg = remark;
                omOrderRmrks.Inserted_UserID = ((FDSession)this.Session["FDSession"]).userId;
                omOrderRmrks.Inserted_Time = DateTime.Now;
                db.RS_OM_Order_Remarks.Add(omOrderRmrks);
                db.SaveChanges();

                updatePlannedOrders(mmOrderRelease.Shop_ID);
                jsondataObj.result = true;
                jsondataObj.message = "Success";
            }
            catch (Exception exp)
            {
                jsondataObj.result = false;
                jsondataObj.message = "Exception: " + exp.Message + " | Cannot Hold Order .";
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "HoldTractorOrder()", ((FDSession)this.Session["FDSession"]).userId);
            }
            return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getBlockUnBlockData(decimal plantId, decimal shopId, int? PlatformId)
        {
            JsonData jsondataObj = new JsonData();
            var lineId = GetLineIdByPlatform(PlatformId);
            try
            {
                Boolean isBlocked = db.RS_OM_OrderRelease.Any(a => a.Shop_ID == shopId && a.Plant_ID == plantId && a.Is_Blocked == true && a.Line_ID == lineId);
                jsondataObj.result = true;
                if (isBlocked)
                {
                    jsondataObj.message = "Blocked";
                }
                else
                {
                    jsondataObj.message = "UnBlocked";
                }
            }
            catch (Exception exp)
            {
                jsondataObj.result = false;
                jsondataObj.message = "Exception: " + exp.Message + " | Cannot get Block/UnBlock Data .";
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "getBlockUnBlockData()", ((FDSession)this.Session["FDSession"]).userId);
            }
            return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UnBlockOrders(decimal shopID, decimal PlatformId)
        {
            JsonData jsondataObj = new JsonData();
            DateTime nowTime = DateTime.Now;
            string moduleName = "PPC Module";
            var lineID = GetLineIdByPlatform(PlatformId);
            string activityRemarks = "UnBlock Orders in Sequencing";
            try
            {
                db.RS_OM_OrderRelease.Where(a => a.Shop_ID == shopID && a.Is_Blocked == true && a.Line_ID == lineID)
                                         .OrderBy(a => a.RSN).ToList()
                                         .ForEach(a => a.Is_Blocked = false);
                db.SaveChanges();
                jsondataObj.result = true;
                jsondataObj.message = "Success";
            }
            catch (Exception exp)
            {
                jsondataObj.result = false;
                jsondataObj.message = "Exception: " + exp.Message + " | Cannot get Block/UnBlock Data .";
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "UnBlockOrders()", ((FDSession)this.Session["FDSession"]).userId);
            }
            finally
            {
                generalHelper.logUserActivity(shopID, lineID, moduleName, activityRemarks, nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
            }
            return Json(jsondataObj, JsonRequestBehavior.AllowGet);
        }
        public DateTime getPlannedDate()
        {
            DateTime plannedDateTime = DateTime.Now;
            DateTime plannedDate;
            TimeSpan currentTime = plannedDateTime.TimeOfDay;

            TimeSpan firstShiftStartTime = new TimeSpan(6, 0, 0);
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
        public ActionResult changeDailyPlan(List<string> selectedSequence, decimal shopID, decimal PlatformId, List<string> allShift, int prodCount)
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            JsonData jsondataObj = new JsonData();
            DateTime nowTime = DateTime.Now;
            string moduleName = "PPC Module";
            string activityRemarks = "Change Planned Quantity";
            List<Shift> objShift = (List<Shift>)JsonConvert.DeserializeObject(allShift[0], typeof(List<Shift>));
            List<decimal> rowIdObj = (List<decimal>)JsonConvert.DeserializeObject(selectedSequence[0], typeof(List<decimal>));
            decimal shiftID = objShift[0].Shift_ID;
            var LineID = GetLineIdByPlatform(PlatformId);
            int tact_time = 0;
            var shiftName = "";

            DateTime dateVal = getPlannedDate();

            //---production count validtion---
            if (prodCount > 0)
            {
                foreach (var id in rowIdObj)
                {
                    //var c = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopID && m.Line_ID == LineID && m.Row_ID == id && m.Order_Type.ToLower() != "spare" && m.Order_Status.ToLower() == "release").Count();

                    var c = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopID && m.Line_ID == LineID && m.Row_ID == id && m.Order_Status.ToLower() == "release").Count();


                    if (c != 1)
                    {
                        jsondataObj.result = false;
                        jsondataObj.message = "Shift can not be assign to spare order";
                        return Json(jsondataObj, JsonRequestBehavior.AllowGet);
                    }

                }

            }
            else
            {
                jsondataObj.result = false;
                jsondataObj.message = "Production count does not matches as per planned date... You have to create Order!...or Chnage Production Count";

                return Json(jsondataObj, JsonRequestBehavior.AllowGet);
            }
            //-------production count validation end-----


            try
            {
                if (objShift != null)
                {
                    for (int i = 0; i < objShift.Count; i++)
                    {

                        //int plannedQty = Convert.ToInt16(objShift[i].Shift_Name);
                        int plannedQty = rowIdObj.Count();
                        //alerady started orders count
                        // var startedCount = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopID && m.Line_ID == LineID && m.Planned_Date == dateVal && m.Order_Type.ToLower() != "spare" && m.Order_Status.ToLower() == "started").Count();

                        var startedCount = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopID && m.Line_ID == LineID && m.Planned_Date == dateVal && m.Order_Status.ToLower() == "started").Count();

                        RS_OM_PPC_Daily_Plan ppcdailyPlanObj = db.RS_OM_PPC_Daily_Plan.Where(a => a.Shop_ID == shopID && a.Line_ID == LineID && a.Plan_Date == dateVal && a.Shift_ID == shiftID).FirstOrDefault();
                        //RS_OM_PPC_Daily_Plan ppcdailyPlanObj = db.RS_OM_PPC_Daily_Plan.Where(a => a.Shop_ID == shopID && a.Line_ID == LineID && DbFunctions.TruncateTime(a.Plan_Date) == DbFunctions.TruncateTime(DateTime.Today) && a.Shift_ID == shiftID).FirstOrDefault();
                        if (ppcdailyPlanObj != null)
                        {
                            ppcdailyPlanObj.Planned_Qty = plannedQty + startedCount;
                            ppcdailyPlanObj.Is_Edited = true;
                            ppcdailyPlanObj.Updated_Date = DateTime.Now;
                            ppcdailyPlanObj.Shift_ID = shiftID;
                            ppcdailyPlanObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            ppcdailyPlanObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.Entry(ppcdailyPlanObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            RS_OM_PPC_Daily_Plan ppcDailyObj = new RS_OM_PPC_Daily_Plan();

                            ppcDailyObj.Plan_Date = getPlannedDate();//DateTime.Today;
                            ppcDailyObj.Planned_Qty = plannedQty + startedCount;
                            ppcDailyObj.Shop_ID = shopID;
                            ppcDailyObj.Line_ID = LineID;
                            ppcDailyObj.Inserted_Date = DateTime.Now;
                            ppcDailyObj.Shift_ID = shiftID;
                            ppcDailyObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                            ppcDailyObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            db.RS_OM_PPC_Daily_Plan.Add(ppcDailyObj);
                            db.SaveChanges();
                        }
                        updatePlannedOrders(shopID);
                        jsondataObj.result = true;
                        jsondataObj.message = "Success";




                    }
                    DateTime date = DateTime.Today;
                    var shiftId = objShift[0].Shift_ID;

                    //var orders = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopID && m.Line_ID == LineID && m.Order_Status == "Release" && m.Order_Type.ToLower() != "spare" && m.Order_Start == false).OrderBy(m => m.RSN).ToList();
                    var orders = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopID && m.Line_ID == LineID && m.Order_Status == "Release" && m.Order_Start == false).OrderBy(m => m.RSN).ToList();


                    if (orders != null && (shiftId != null || shiftId != 0))
                    {
                        foreach (var order in orders)
                        {

                            var status = rowIdObj.Contains(order.Row_ID);

                            if (status)
                            {
                                order.Planned_Shift_ID = shiftId;
                                order.Planned_Date = dateVal;

                                order.Is_Edited = true;
                                order.Updated_Date = DateTime.Now;
                                order.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                order.Is_Sequenced = true;
                                db.Entry(order).State = EntityState.Modified;
                            }
                            //else if (order.Planned_Shift_ID != shiftId)
                            else
                            {
                                order.Planned_Shift_ID = null;
                                order.Planned_Date = null;

                                order.Is_Edited = true;
                                order.Updated_Date = DateTime.Now;
                                order.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                order.Is_Sequenced = true;
                                db.Entry(order).State = EntityState.Modified;
                            }


                        }
                        db.SaveChanges();
                        //for updateing production count for shift except selected shift
                        //List<RS_OM_PPC_Daily_Plan> ppcdailyPlanObjOtherShift = db.RS_OM_PPC_Daily_Plan.Where(a => a.Shop_ID == shopID && a.Line_ID == LineID && DbFunctions.TruncateTime(a.Plan_Date) == DbFunctions.TruncateTime(dateVal) && a.Shift_ID != shiftID).ToList();

                        List<RS_OM_PPC_Daily_Plan> ppcdailyPlanObjOtherShift = db.RS_OM_PPC_Daily_Plan.Where(a => a.Shop_ID == shopID && a.Line_ID == LineID && a.Shift_ID != shiftID).ToList();
                        if (ppcdailyPlanObjOtherShift != null)
                        {
                            foreach (var record in ppcdailyPlanObjOtherShift)
                            {
                                if (record != null)
                                {
                                    var plannedQtycurrentshift = 0;
                                    //plannedQtycurrentshift = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopID && m.Line_ID == LineID && (m.Order_Status.ToLower() == "release" || m.Order_Status.ToLower() == "started") && m.Order_Type.ToLower() != "spare" && m.Planned_Shift_ID == record.Shift_ID && record.Plan_Date.Year == dateVal.Year && record.Plan_Date.Month == dateVal.Month && record.Plan_Date.Day == dateVal.Day).Count();
                                    plannedQtycurrentshift = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopID && m.Line_ID == LineID && (m.Order_Status.ToLower() == "release" || m.Order_Status.ToLower() == "started") && m.Planned_Shift_ID == record.Shift_ID && record.Plan_Date.Year == dateVal.Year && record.Plan_Date.Month == dateVal.Month && record.Plan_Date.Day == dateVal.Day).Count();


                                    record.Planned_Qty = plannedQtycurrentshift;
                                    record.Is_Edited = true;
                                    record.Updated_Date = DateTime.Now;

                                    record.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                                    record.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                                    db.Entry(record).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                        //
                    }
                    //if (orders != null)
                    //{
                    //    int planQty = 0;
                    //    decimal shiftId = 0;
                    //    foreach (var order in orders)
                    //    {
                    //        if (objShift.Count > k)
                    //        {
                    //            shiftId = objShift[k].Shift_ID;
                    //            //planQty = Convert.ToInt16(objShift[k].Shift_Name);
                    //            planQty = prodCount;
                    //        }
                    //        while (planQty == 0)
                    //        {
                    //            shiftId = objShift[++k].Shift_ID;
                    //            //planQty = Convert.ToInt16(objShift[k].Shift_Name);
                    //            planQty = prodCount;
                    //            j = 0;
                    //        }

                    //        if (order.Order_Type.Equals("Spare", StringComparison.CurrentCultureIgnoreCase))
                    //        {
                    //            order.Planned_Shift_ID = 0;//shiftPlan[0].Shift_ID;
                    //        }
                    //        else if (planQty > j)
                    //        {
                    //            order.Planned_Shift_ID = shiftId;
                    //            j++;
                    //        }
                    //        if (objShift.Count > k)
                    //        {
                    //            if (planQty == j)
                    //            {
                    //                k++;
                    //                j = 0;
                    //            }
                    //        }

                    //        order.Is_Edited = true;
                    //        order.Updated_Date = DateTime.Now;
                    //        order.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    //        order.Is_Sequenced = true;
                    //        db.Entry(order).State = EntityState.Modified;
                    //    }

                    //    db.SaveChanges();
                    //}
                    ////

                }

            }
            catch (Exception exp)
            {
                jsondataObj.result = false;
                jsondataObj.message = "Exception: " + exp.Message + " | Cannot change the Planned Quantity .";
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "changeDailyPlan()", ((FDSession)this.Session["FDSession"]).userId);
            }
            finally
            {
                generalHelper.logUserActivity(shopID, null, moduleName, activityRemarks, nowTime, DateTime.Now, ((FDSession)this.Session["FDSession"]).userId, ((FDSession)this.Session["FDSession"]).userHost);
            }
            Session["tactsheetFlag"] = true;
            return Json(jsondataObj, JsonRequestBehavior.AllowGet);
            //}
        }

        private void updatePlannedOrders(decimal shopID)
        {
            try
            {
                DateTime Today = DateTime.Today;
                decimal lineID = db.RS_Partgroup.Where(a => a.Shop_ID == shopID && a.Order_Create == true).Select(a => a.Line_ID).FirstOrDefault();
                int totalDeleted = 0;
                //TAKE TODAYS SHOPS PLANNED QTY
                int todaysPlan = db.RS_OM_PPC_Daily_Plan.Where(a => a.Shop_ID == shopID && a.Plan_Date == Today)
                                   .Select(a => a.Planned_Qty).FirstOrDefault();
                int todaysDone = db.RS_OM_Planned_Orders.Where(a => a.Order_Status != "Release" && a.Shop_ID == shopID && a.Planned_Date == Today)
                                   .Count();

                RS_OM_Planned_Orders[] plannedOrder = db.RS_OM_Planned_Orders.Where(p => p.Order_Status == "Release" && p.Shop_ID == shopID && p.Planned_Date == Today).ToArray();
                for (int i = 0; i < plannedOrder.Count(); i++)
                {
                    generalHelper.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_OM_Planned_Orders", "Row_ID", plannedOrder[i].Row_ID.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                }

                //DELETE ALL TODAYS ORDER WITH RELEASED STATUS FROM PLANNED TABLE
                string sqlQuery = "DELETE FROM RS_OM_Planned_Orders WHERE Order_Status = 'Release' AND Shop_ID = @p0 AND Planned_Date = @p1";
                totalDeleted = db.Database.ExecuteSqlCommand(sqlQuery, shopID, Today);
                int insertQty = 0;
                if (todaysDone <= todaysPlan)
                {
                    insertQty = todaysPlan - todaysDone;
                }
                //if (totalDeleted > todaysPlan)
                //{
                //    totalDeleted = todaysPlan;
                //}
                sqlQuery = "INSERT INTO RS_OM_Planned_Orders " +
                           " (Plant_ID,Shop_ID,Group_No,Order_ID,Order_No,Order_Status,Parent_Model_Code,Parent_Series_Code,Model_Code,Series_Code,RSN,Planned_Date,Inserted_Time) SELECT TOP " + insertQty +
                           " a.Plant_ID,a.Shop_ID,0,a.Row_ID,Order_No,Order_Status,a.Model_Code,b.Series_Code,a.partno,a.Series_Code,RSN,CONVERT(DATE,GETDATE()),a.Inserted_Date" +
                           " FROM RS_OM_OrderRelease a " +
                           " JOIN RS_Model_Master b ON a.Model_Code = b.Model_Code" +
                           " WHERE a.Shop_ID = @p0 AND a.Line_ID = @p1 AND a.Order_Status = 'Release' ORDER BY RSN";
                db.Database.ExecuteSqlCommand(sqlQuery, shopID, lineID);

            }
            catch (Exception exp)
            {
                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "updatePlannedOrders(Shop_ID:" + shopID + ")", ((FDSession)this.Session["FDSession"]).userId);
            }

        }
        /////////////////////////////////////////////////

        /*               Action Name               : GetOrderNumberList
         *               Description               : Action used to return the list of Series ID  for Part Group
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Plant_Id,Shop_id
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult GetOrderNumberList(int Plant_Id, int Shop_Id, int Line_Id)
        {
            //var OrderNumber = db.RS_OM_OrderRelease
            //                              .Where(c => c.Plant_ID == Plant_Id && c.Shop_ID == Shop_Id && c.Line_ID==Line_Id  && c.Order_Status == "Release")
            //                              .Select(c => new { c.Row_ID, c.Order_No, c.partno, c.Series_Code, c.ORN,c.RSN,c.Inserted_Date }).OrderBy(x => x.Inserted_Date);

            var OrderNumber = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == Plant_Id && or.Shop_ID == Shop_Id && or.Line_ID == Line_Id && or.Order_Status == "Release" && or.Order_Start == false)
                               orderby or.RSN ascending
                               select new
                               {
                                   Row_ID = or.Row_ID,
                                   Order_No = or.Order_No,
                                   partno = or.partno,
                                   Part_no = or.partno,
                                   Series_Code = db.RS_Series.Where(p => p.Series_Code == or.Series_Code).Select(p => p.Series_Description).FirstOrDefault(),
                                   ORN = or.ORN,
                                   RSN = or.RSN,
                                   Inserted_Date = or.Inserted_Date
                               }).ToList();
            return Json(OrderNumber, JsonRequestBehavior.AllowGet);
        }

        /*               Action Name               : ChangeSequence
         *               Description               : Action used to Change the sequense  for Order Planning
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : formPlanning
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        [HttpPost]
        public ActionResult ChangeSequence(FormCollection formPlanning)
        {
            string Order_type, Remarks;
            int plant_ID, Line_Id, Shop_ID;
            plant_ID = Convert.ToInt16(formPlanning["Plant_ID"]);
            Shop_ID = Convert.ToInt16(formPlanning["Shop_ID"]);
            Order_type = formPlanning["Order_Type"].ToString();
            Remarks = formPlanning["Remarks"].ToString();
            Line_Id = Convert.ToInt16(formPlanning["Line_ID"]);
            List<string> Order_number = new List<string>();
            List<int> old_RSN = new List<int>();

            //Load RSN Number
            //var old_RSNList = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plant_ID && or.Shop_ID == Shop_ID && or.Line_ID == Line_Id && or.Order_Status == "Release")                   
            //                   orderby or.RSN ascending 
            //                   select new { or.RSN  }).ToList();

            var old_RSNList = db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plant_ID && or.Shop_ID == Shop_ID && or.Line_ID == Line_Id && or.Order_Status == "Release" && or.Order_Start == false)
                               .AsEnumerable()
                               .OrderBy(or => or.RSN)
                               .Select(or => or.RSN).ToList();

            foreach (var item in old_RSNList)
            {
                old_RSN.Add(Convert.ToInt32(item));
            }

            // RS_OM_OrderRelease release = new RS_OM_OrderRelease();
            int j = 0;
            foreach (var orderNoRowID in formPlanning["CUMNDATA"].Split(',').ToList())
            {

                int rsn = old_RSN[j];
                var newOrderObj = db.RS_OM_OrderRelease.Find(Convert.ToDecimal(orderNoRowID));

                newOrderObj.RSN = rsn;
                newOrderObj.Remarks = Remarks;
                newOrderObj.Is_Edited = true;
                newOrderObj.Is_Edited = true;
                newOrderObj.Updated_Date = DateTime.Now;
                newOrderObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                db.Entry(newOrderObj).State = EntityState.Modified;
                db.SaveChanges();

                j++;
            }

            // Order_number.RemoveAt(0);
            //List<string> number = new List<string>();
            //number = Order_number;
            //for (int i = 0; i < number.Count; i++)
            //{
            //    string s = number[i].ToString();
            //    release = db.RS_OM_OrderRelease.Where(x => x.Row_ID.ToString() == s).FirstOrDefault();
            //    release.Remarks = Remarks;
            //    inc_cnt = cnt + i;
            //    release.RSN = inc_cnt;
            //    release.Updated_Date = DateTime.Now;
            //    release.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
            //    db.Entry(release).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.OM_Planning;
            globalData.messageDetail = ResourceModules.OM_Planning + " " + ResourceMessages.Add_Success;
            TempData["globalData"] = globalData;

            return RedirectToAction("Index");
        }
        public class JSONData
        {
            public bool status { get; set; }
            public string type { get; set; }
            public string message { get; set; }
            public List<string> dataModal { get; set; }
        }

        /*               Action Name               : SaveOrderpattern
         *               Description               : Action used to save order pattern
         *               Author, Timestamp         : Ketan Dhanuka
         *               Input parameter           : datamodel
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult SaveOrderpattern(List<string> dataModal)
        {
            JSONData objJSONData = new JSONData();
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            int ShopID = ((FDSession)this.Session["FDSession"]).shopId;
            try
            {
                List<OrderPattern> objRS_OM_Order_Pattern = (List<OrderPattern>)JsonConvert.DeserializeObject(dataModal[0], typeof(List<OrderPattern>));
                //List<OrderPattern> objRS_OM_Order_Pattern_priority = (List<OrderPattern>)JsonConvert.DeserializeObject(priorityDataModal[0], typeof(List<OrderPattern>));
                //List<string> dataModelobj = new List<string>();
                //int j = 0;
                //foreach (var pr in objRS_OM_Order_Pattern_priority)
                //{
                //    objRS_OM_Order_Pattern[j].Priority = pr.Priority;
                //    j++;

                //}
                var platformcount = db.RS_OM_Platform.Where(m => m.Plant_ID == plantID).Select(m => m.Platform_ID).Count();
                if (platformcount != objRS_OM_Order_Pattern.Count())
                {
                    objJSONData.status = false;
                    objJSONData.message = "Please provide Priority and ratio for all platforms!...";
                    objJSONData.type = "fail";
                }
                var todayDate = DateTime.Now;
                ////avoid adding save ratio and pattern again in new record

                DateTime lastDate = new DateTime(0001, 01, 01);
                if (db.RS_OM_Order_Pattern.Count() > 0)
                {
                    lastDate = db.RS_OM_Order_Pattern.Max(m => m.Inserted_Date);
                }
                var Platform = (from pf in db.RS_OM_Order_Pattern
                                where pf.Plant_ID == plantID && pf.Inserted_Date.Year == lastDate.Year &&
                                pf.Inserted_Date.Month == lastDate.Month && pf.Inserted_Date.Day == lastDate.Day &&
                                pf.Inserted_Date.Hour == lastDate.Hour && pf.Inserted_Date.Hour == lastDate.Hour &&
                                pf.Inserted_Date.Minute == lastDate.Minute && pf.Inserted_Date.Second == lastDate.Second
                                orderby pf.Platform_ID ascending
                                select pf).ToList();
                var isNotSame = false;

                foreach (var plfm in Platform)
                {
                    var orderPatternObj = objRS_OM_Order_Pattern.Where(m => m.Platform_ID == plfm.Platform_ID).FirstOrDefault();
                    if (orderPatternObj.Ratio != plfm.Ratio || orderPatternObj.Priority != plfm.Priority)
                    {
                        isNotSame = true;
                        break;
                    }
                }
                var patternCount = db.RS_OM_Order_Pattern.Where(m => m.Plant_ID == plantID).Count();
                if (!isNotSame && patternCount > 0)
                {
                    objJSONData.message = "Order Pattern Already Exist!...";
                    objJSONData.type = "Success";
                    objJSONData.status = true;
                    objJSONData.dataModal = dataModal;
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }


                ////
                if (objRS_OM_Order_Pattern != null)
                {
                    for (int i = 0; i < objRS_OM_Order_Pattern.Count; i++)
                    {
                        RS_OM_Order_Pattern obj_RS_OM_Order_Pattern = new RS_OM_Order_Pattern();
                        obj_RS_OM_Order_Pattern.Platform_ID = objRS_OM_Order_Pattern[i].Platform_ID;
                        obj_RS_OM_Order_Pattern.Ratio = objRS_OM_Order_Pattern[i].Ratio;
                        obj_RS_OM_Order_Pattern.Priority = objRS_OM_Order_Pattern[i].Priority;
                        obj_RS_OM_Order_Pattern.Planned_Date = todayDate;

                        obj_RS_OM_Order_Pattern.Inserted_Date = todayDate;
                        obj_RS_OM_Order_Pattern.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        obj_RS_OM_Order_Pattern.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        obj_RS_OM_Order_Pattern.Planned_Time = TimeSpan.Parse(DateTime.Now.ToString("hh:mm:ss"));
                        obj_RS_OM_Order_Pattern.Plant_ID = plantID;
                        obj_RS_OM_Order_Pattern.UB10_SDP_Lockbody_Count = 0;
                        obj_RS_OM_Order_Pattern.UB10_SDP_Lockbody_Complete_Count = 0;

                        db.RS_OM_Order_Pattern.Add(obj_RS_OM_Order_Pattern);
                        db.SaveChanges();
                        //OrderPatternJson orderPatternJson = new OrderPatternJson();
                        //orderPatternJson.Platform_ID = objRS_OM_Order_Pattern[i].Platform_ID;
                        //orderPatternJson.Ratio = objRS_OM_Order_Pattern[i].Ratio;
                        //orderPatternJson.Priority = objRS_OM_Order_Pattern[i].Priority;
                        ////orderPatternJson.Planned_Date = System.DateTime.Now;


                        //string patternObj = new JavaScriptSerializer().Serialize(orderPatternJson);
                        //dataModelobj.Add(patternObj);
                    }
                }
                objJSONData.message = "Order Pattern Saved Successfully!...";
                objJSONData.type = "Success";
                objJSONData.status = true;
                objJSONData.dataModal = dataModal;
                List<string> obj = new List<string>();

                string t = new JavaScriptSerializer().Serialize(objRS_OM_Order_Pattern.ToList());

                return Json(objJSONData, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
            {
                Exception raise = dbex;
                foreach (var ValidationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in ValidationErrors.ValidationErrors)
                    {
                        string Message = string.Format("{0:}{1}", ValidationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        generalHelper.addControllerException(dbex, "OMOrderPlanningController", "SaveORderPattern(Post) " + validationError, ((FDSession)this.Session["FDSession"]).userId);
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
                objJSONData.status = false;
                objJSONData.message = "Error in saving Main Line Pattern!...";
                objJSONData.type = "fail";
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }

        }
        public class OrderPatternJson
        {
            public decimal Platform_ID { get; set; }
            public int Ratio { get; set; }
            public int Priority { get; set; }
            //public DateTime Planned_Date { get; set; }
            //public int Production_Count { get; set; }

        }
        /*               Action Name               : GenerateTactSheet
         *               Description               : Action used to Generate tact sheet logic
         *               Author, Timestamp         : Ketan Dhanuka
         *               Input parameter           : datamodel
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public ActionResult GenerateTactSheet(List<string> allShift, decimal platform_Id, int ShopID, int? prodCount)
        {
            //Boolean tactsheetFlag = Convert.ToBoolean(Session["tactsheetFlag"]);
            JSONData objJSONData = new JSONData();



            int tact_time = 0;
            var shiftName = "";
            decimal shiftId = 0;
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var lineId = Convert.ToInt16(GetLineIdByPlatform(platform_Id));

            try
            {
                DataSet totalShiftDs = new DataSet();

                DateTime timespan = new DateTime();
                DateTime StartTimespan = new DateTime();
                List<CummulativeFields> neworderPatternSequenceObj = new List<CummulativeFields>();
                //List<OrderPattern> objRS_OM_Order_Pattern = (List<OrderPattern>)JsonConvert.DeserializeObject(dataModal[0], typeof(List<OrderPattern>));
                List<Shift> objShift = (List<Shift>)JsonConvert.DeserializeObject(allShift[0], typeof(List<Shift>));
                var currentShiftID = objShift[0].Shift_ID;
                shiftName = objShift[0].Shift_Name;
                shiftId = objShift[0].Shift_ID;


                if (prodCount > 0)
                {
                    //var count = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && DbFunctions.TruncateTime(m.Planned_Date) == dateVal).Count();
                    //var currentproductioncount = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && m.Planned_Date == dateVal && m.Order_Type.ToLower() != "spare" && m.Order_Status.ToLower() == "release").Count();
                    //if (!(prodCount <= currentproductioncount))
                    //{
                    //    objJSONData.status = false;
                    //    objJSONData.message = "Production count does not matches as per planned date... You have to create Order!...or Chnage Prduction Count";
                    //    objJSONData.type = "fail";

                    //    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                    //}
                }
                else
                {
                    objJSONData.status = false;
                    objJSONData.message = "Production count does not matches as per planned date... You have to create Order!...or Chnage Prduction Count";
                    objJSONData.type = "fail";

                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }


                //if (objRS_OM_Order_Pattern != null)
                if (prodCount > 0)
                {
                    //DateTime plannedDate = Convert.ToDateTime(PlannedDate);// Convert.ToDateTime(objRS_OM_Order_Pattern[0].Planned_Date);
                    DateTime plannedDate = getPlannedDate();


                    neworderPatternSequenceObj = GetOrderSequenceBasedOnRatioByPlannedDate(plantID, ShopID, lineId, plannedDate);

                    neworderPatternSequenceObj = (from or in neworderPatternSequenceObj.Where(m => m.Planned_Shift_ID == shiftId)
                                                  join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID)
                                                  on or.Model_Code equals mm.Model_Code into ormm
                                                  from om in ormm.DefaultIfEmpty()
                                                  join pf in db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID)
                                                  on om.Platform_Id equals pf.Platform_ID into ompf
                                                  from op in ompf.DefaultIfEmpty()

                                                  orderby or.RSN ascending
                                                  select new CummulativeFields()
                                                  {
                                                      Row_ID = or.Row_ID,
                                                      Order_No = or.Order_No,
                                                      Model_Code = or.Model_Code.Trim(),
                                                      Auto_Remarks = om.Auto_Remarks,
                                                      Attribution_Parameters = om.Attribution_Parameters,
                                                      Color_code = or.Color_code,
                                                      Inserted_Date = or.Inserted_Date,
                                                      PlatformName = op.Platform_Name,
                                                      //Color_Name = cc.Colour_Desc,
                                                      Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Color_code).Select(m => m.Colour_Desc).FirstOrDefault(),
                                                      //Style_Code=or.Style_Code,
                                                      Planned_Shift_ID = or.Planned_Shift_ID
                                                  }).ToList();
                    //for saving tactsheet in db
                    if (neworderPatternSequenceObj != null)
                    {
                        //if (!updateOrderStartStatus(ShopID))
                        //{
                        //    objJSONData.status = false;
                        //    objJSONData.message = "Error occured while updating order started status, for order locking";
                        //    objJSONData.type = "fail";

                        //    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                        //}

                        //var U321RSNSkipCount = 0;
                        //var S201RSNSkipCount = 0;
                        //var minLockCount = 5;// getMinLockCount(plantID, platform_Id, plannedDate);

                        //var U321RSNCount = db.RS_OM_U321_Tactsheet_Orders.Where(m => m.Is_Locked == true &&
                        //  m.Plant_ID == plantID && m.Shop_ID == ShopID 
                        //  //&& m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day
                        //  ).Max(q => (int?)(q.RSN)) ?? 0;

                        //var U321lockSkipCount = db.RS_OM_U321_Tactsheet_Orders.Where(m => m.Is_Locked == true  && m.Is_Started != true &&  !(db.RS_OM_Order_List.Any(p => p.Order_No == m.Order_No)) &&
                        // m.Plant_ID == plantID && m.Shop_ID == ShopID
                        // //&& m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day
                        // ).Count();



                        //var S201RSNCount = db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Is_Locked == true &&
                        // m.Plant_ID == plantID && m.Shop_ID == ShopID
                        // //&& m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day
                        // ).Max(q => (int?)(q.RSN)) ?? 0;
                        //var S201lockSkipCount = db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Is_Locked == true && !(db.RS_OM_Order_List.Any(p => p.Order_No == m.Order_No)) && m.Is_Started != true &&
                        //m.Plant_ID == plantID && m.Shop_ID == ShopID
                        ////&& m.Planned_Date.Year == plannedDate.Year&& m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day
                        //).Count();
                        //S201lockSkipCount++;


                        //if (U321RSNCount == 0)
                        //{
                        //    U321RSNCount = 1;
                        //}
                        //else
                        //{
                        //    //U321RSNSkipCount = U321RSNCount++;
                        //    U321RSNSkipCount = db.RS_OM_U321_Tactsheet_Orders.Where(m => m.Is_Locked == true  && m.Is_Started != true && m.Plant_ID == plantID && m.Shop_ID == ShopID && !(db.RS_OM_Order_List.Any(p => p.Order_No == m.Order_No))).Count();
                        //    U321RSNCount++;

                        //}

                        //if (S201RSNCount == 0)
                        //{
                        //    S201RSNCount = 1;

                        //}
                        //else
                        //{
                        //    //S201RSNSkipCount = S201RSNCount++;
                        //    S201RSNSkipCount = db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Is_Locked == true && m.Is_Started != true && m.Plant_ID == plantID && m.Shop_ID == ShopID && !(db.RS_OM_Order_List.Any(p => p.Order_No == m.Order_No))).Count();
                        //    S201RSNCount++;
                        //}
                        //var bot = "";

                        //
                        //tactsheetData tactsheetDataObj = new tactsheetData();
                        //if (neworderPatternSequenceObj[0].PlatformName.Equals("U321", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    //delete already scheduled order in same shift
                        //    var oldTactsheetOdr = db.RS_OM_U321_Tactsheet_Orders.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Is_Locked != true ).ToList();
                        //    db.RS_OM_U321_Tactsheet_Orders.RemoveRange(oldTactsheetOdr);
                        //    db.SaveChanges();
                        //    //retirve other shift orders
                        //    var OtherShiftOrderU21 = db.RS_OM_U321_Tactsheet_Orders.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Planned_Date.Year == plannedDate.Year
                        //      && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day && m.Is_Locked != true && m.Shift_ID != shiftId).OrderBy(m => m.RSN).ToList();
                        //    ////
                        //    ////
                        //    //for updating locked orders
                        //    var PlatformName = neworderPatternSequenceObj[0].PlatformName;
                        //    var platformIdlocked = db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && m.Platform_Name.ToUpper().Trim() == PlatformName.ToUpper().Trim()).Select(m => m.Platform_ID).FirstOrDefault();

                        //    var lockedOrdersu321 = (from or in db.RS_OM_OrderRelease
                        //                            join u321 in db.RS_OM_U321_Tactsheet_Orders on or.Order_No.ToLower() equals u321.Order_No.ToLower()
                        //                            where u321.Plant_ID == plantID && u321.Shop_ID == ShopID && u321.Platform_ID == platformIdlocked && or.Order_Status.ToLower() == "release" && u321.Is_Locked == true
                        //                            orderby u321.RSN
                        //                            select u321).ToList();
                        //    var lockC = 0;
                        //    if (neworderPatternSequenceObj.Count() < lockedOrdersu321.Count())
                        //    {
                        //        objJSONData.status = false;
                        //        objJSONData.message = "Top " + lockedOrdersu321.Count() + "locked for Resequencing.";
                        //        objJSONData.type = "fail";

                        //        return Json(objJSONData, JsonRequestBehavior.AllowGet);

                        //    }
                        //    foreach (var or in lockedOrdersu321)
                        //    {

                        //        if (!(or.Order_No.Equals(neworderPatternSequenceObj[lockC++].Order_No, StringComparison.CurrentCultureIgnoreCase)))
                        //        {
                        //            objJSONData.status = false;
                        //            objJSONData.message = "Order is Lokced: Error occured while resequening: " + or.Order_No;
                        //            objJSONData.type = "fail";

                        //            return Json(objJSONData, JsonRequestBehavior.AllowGet);
                        //        }
                        //        else
                        //        {
                        //            or.Planned_Date = plannedDate;
                        //            or.Shift_ID = shiftId;
                        //            db.Entry(or).State = EntityState.Modified;

                        //        }
                        //    }
                        //    db.SaveChanges();
                        //    //db.RS_OM_U321_Tactsheet_Orders.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Platform_ID == platformIdlocked
                        //    //&& m.).Select(m => m).FirstOrDefault();
                        //    //end of updating locked order
                        //    foreach (var item in neworderPatternSequenceObj)
                        //    {
                        //        if (U321RSNSkipCount == 0)
                        //        {
                        //            var model = db.RS_Model_Master.Where(m => m.Model_Code == item.Model_Code && m.Plant_ID == plantID && m.Shop_ID == ShopID).FirstOrDefault();
                        //            List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
                        //            var platformId = db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && m.Platform_Name.ToUpper().Trim() == item.PlatformName.ToUpper().Trim()).Select(m => m.Platform_ID).FirstOrDefault();

                        //            //////if (db.RS_OM_U321_Tactsheet_Orders.Any(m => m.Order_No.Equals(item.Order_No, StringComparison.CurrentCultureIgnoreCase)))
                        //            //////{
                        //            //////    objJSONData.status = false;
                        //            //////    objJSONData.message = "Order Already Exist: Error occured while locking Order NO: "+ item.Order_No;
                        //            //////    objJSONData.type = "fail";

                        //            //////    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                        //            //////}
                        //            for (int i = 0; i < attributionParameters.Count; i++)
                        //            {
                        //                AttributionParameters attributionParameter = attributionParameters[i];
                        //                try
                        //                {
                        //                    Convert.ToInt32(attributionParameter.Value);
                        //                }
                        //                catch (Exception)
                        //                {

                        //                    continue;
                        //                }
                        //                if (attributionParameter.label.Equals("Vehicle Series", StringComparison.InvariantCultureIgnoreCase))
                        //                {
                        //                    int attrId = Convert.ToInt32(attributionParameter.Value);
                        //                    var Series = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                        //                    bot = db.RS_Vehicle_Series.Where(m => m.Plant_ID == model.Plant_ID && m.Shop_ID == model.Shop_ID && m.Attribute_Name.ToLower() == Series.ToLower()).OrderByDescending(m => m.Inserted_Date).Select(m => m.BOT).FirstOrDefault();
                        //                }
                        //            }
                        //            //RS_OM_U321_Tactsheet_Orders U321obj = new RS_OM_U321_Tactsheet_Orders();
                        //            //U321obj.Plant_ID = plantID;
                        //            //U321obj.Shop_ID = ShopID;
                        //            //U321obj.Platform_ID = platformId;
                        //            //U321obj.Shift_ID = Convert.ToDecimal(item.Planned_Shift_ID);
                        //            //U321obj.RSN = U321RSNCount++;
                        //            //U321obj.Planned_Date = plannedDate;
                        //            //U321obj.Order_No = item.Order_No;
                        //            //U321obj.Order_Status = 1;

                        //            //if (U321lockSkipCount < 5)
                        //            //{
                        //            //    U321obj.Is_Locked = true;
                        //            //    U321lockSkipCount++;
                        //            //}
                        //            //else
                        //            //{
                        //            //    U321obj.Is_Locked = false;
                        //            //}
                        //            //U321obj.Is_Started = false;
                        //            //U321obj.Model_Code = item.Model_Code;
                        //            //U321obj.BOT = bot == null ? "NA" : bot;
                        //            //U321obj.Style_Code = model.Style_Code;//change this
                        //            //U321obj.Inserted_Date = DateTime.Now;
                        //            //db.RS_OM_U321_Tactsheet_Orders.Add(U321obj);
                        //            //db.SaveChanges();
                        //        }
                        //        else
                        //        {
                        //            U321RSNSkipCount--;
                        //        }

                        //    }
                        //    //updated rns of other shifts
                        //    //foreach (var ordr in OtherShiftOrderU21)
                        //    //{
                        //    //    ordr.RSN = U321RSNCount++;
                        //    //    if (U321lockSkipCount < minLockCount)
                        //    //    {
                        //    //        ordr.Is_Locked = true;
                        //    //        U321lockSkipCount++;
                        //    //    }
                        //    //    else
                        //    //    {
                        //    //        ordr.Is_Locked = false;
                        //    //    }
                        //    //    db.Entry(ordr).State = EntityState.Modified;
                        //    //    db.SaveChanges();
                        //    //}

                        //}
                        //else if (neworderPatternSequenceObj[0].PlatformName.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    //delete already scheduled order in same shift
                        //    var oldTactsheetOdr = db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Is_Locked != true).ToList();
                        //    db.RS_OM_S201_Tactsheet_Orders.RemoveRange(oldTactsheetOdr);
                        //    db.SaveChanges();
                        //    //retirve other shift orders
                        //    var OtherShiftOrderU21 = db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Planned_Date.Year == plannedDate.Year
                        //      && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day && m.Is_Locked != true && m.Shift_ID != shiftId).OrderBy(m => m.RSN).ToList();
                        //    ////
                        //    ////
                        //    //for updating locked orders
                        //    var PlatformName = neworderPatternSequenceObj[0].PlatformName;
                        //    var platformIdlocked = db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && m.Platform_Name.ToUpper().Trim() == PlatformName.ToUpper().Trim()).Select(m => m.Platform_ID).FirstOrDefault();

                        //    var lockedOrderss201 = (from or in db.RS_OM_OrderRelease
                        //                            join s201 in db.RS_OM_S201_Tactsheet_Orders on or.Order_No.ToLower() equals s201.Order_No.ToLower()
                        //                            where s201.Plant_ID == plantID && s201.Shop_ID == ShopID && s201.Platform_ID == platformIdlocked && or.Order_Status.ToLower() == "release" && s201.Is_Locked == true
                        //                            orderby s201.RSN
                        //                            select s201).ToList();
                        //    var lockC = 0;
                        //    if (neworderPatternSequenceObj.Count() < lockedOrderss201.Count())
                        //    {
                        //        objJSONData.status = false;
                        //        objJSONData.message = "Top " + lockedOrderss201.Count() + "locked for Resequencing.";
                        //        objJSONData.type = "fail";

                        //        return Json(objJSONData, JsonRequestBehavior.AllowGet);

                        //    }
                        //    foreach (var or in lockedOrderss201)
                        //    {

                        //        if (!(or.Order_No.Equals(neworderPatternSequenceObj[lockC++].Order_No, StringComparison.CurrentCultureIgnoreCase)))
                        //        {
                        //            objJSONData.status = false;
                        //            objJSONData.message = "Order is Lokced: Error occured while resequening: " + or.Order_No;
                        //            objJSONData.type = "fail";

                        //            return Json(objJSONData, JsonRequestBehavior.AllowGet);
                        //        }
                        //        else
                        //        {
                        //            or.Planned_Date = plannedDate;
                        //            or.Shift_ID = shiftId;
                        //            db.Entry(or).State = EntityState.Modified;

                        //        }
                        //    }
                        //    db.SaveChanges();
                        //    //db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Platform_ID == platformIdlocked
                        //    //&& m.).Select(m => m).FirstOrDefault();
                        //    //end of updating locked order
                        //    foreach (var item in neworderPatternSequenceObj)
                        //    {
                        //        if (S201RSNSkipCount == 0)
                        //        {
                        //            var model = db.RS_Model_Master.Where(m => m.Model_Code == item.Model_Code && m.Plant_ID == plantID && m.Shop_ID == ShopID).FirstOrDefault();
                        //            List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
                        //            var platformId = db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && m.Platform_Name.ToUpper().Trim() == item.PlatformName.ToUpper().Trim()).Select(m => m.Platform_ID).FirstOrDefault();

                        //            //////if (db.RS_OM_S201_Tactsheet_Orders.Any(m => m.Order_No.Equals(item.Order_No, StringComparison.CurrentCultureIgnoreCase)))
                        //            //////{
                        //            //////    objJSONData.status = false;
                        //            //////    objJSONData.message = "Order Already Exist: Error occured while locking Order NO: "+ item.Order_No;
                        //            //////    objJSONData.type = "fail";

                        //            //////    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                        //            //////}
                        //            for (int i = 0; i < attributionParameters.Count; i++)
                        //            {
                        //                AttributionParameters attributionParameter = attributionParameters[i];
                        //                try
                        //                {
                        //                    Convert.ToInt32(attributionParameter.Value);
                        //                }
                        //                catch (Exception)
                        //                {

                        //                    continue;
                        //                }
                        //                if (attributionParameter.label.Equals("Vehicle Series", StringComparison.InvariantCultureIgnoreCase))
                        //                {
                        //                    int attrId = Convert.ToInt32(attributionParameter.Value);
                        //                    var Series = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                        //                    bot = db.RS_Vehicle_Series.Where(m => m.Plant_ID == model.Plant_ID && m.Shop_ID == model.Shop_ID && m.Attribute_Name.ToLower() == Series.ToLower()).OrderByDescending(m => m.Inserted_Date).Select(m => m.BOT).FirstOrDefault();
                        //                }
                        //            }
                        //            //RS_OM_S201_Tactsheet_Orders S201obj = new RS_OM_S201_Tactsheet_Orders();
                        //            //S201obj.Plant_ID = plantID;
                        //            //S201obj.Shop_ID = ShopID;
                        //            //S201obj.Platform_ID = platformId;
                        //            //S201obj.Shift_ID = Convert.ToDecimal(item.Planned_Shift_ID);
                        //            //S201obj.RSN = S201RSNCount++;
                        //            //S201obj.Planned_Date = plannedDate;
                        //            //S201obj.Order_No = item.Order_No;
                        //            //S201obj.Order_Status = 1;

                        //            //if (S201lockSkipCount < 5)
                        //            //{
                        //            //    S201obj.Is_Locked = true;
                        //            //    S201lockSkipCount++;
                        //            //}
                        //            //else
                        //            //{
                        //            //    S201obj.Is_Locked = false;
                        //            //}
                        //            //S201obj.Is_Started = false;
                        //            //S201obj.Model_Code = item.Model_Code;
                        //            //S201obj.BOT = bot == null ? "NA" : bot;
                        //            //S201obj.Style_Code = model.Style_Code;//change this
                        //            //S201obj.Inserted_Date = DateTime.Now;
                        //            //db.RS_OM_S201_Tactsheet_Orders.Add(S201obj);
                        //            //db.SaveChanges();
                        //        }
                        //        else
                        //        {
                        //            S201RSNSkipCount--;
                        //        }

                        //    }
                        //    //updated rns of other shifts
                        //    //foreach (var ordr in OtherShiftOrderU21)
                        //    //{
                        //    //    ordr.RSN = S201RSNCount++;
                        //    //    if (S201lockSkipCount < minLockCount)
                        //    //    {
                        //    //        ordr.Is_Locked = true;
                        //    //        S201lockSkipCount++;
                        //    //    }
                        //    //    else
                        //    //    {
                        //    //        ordr.Is_Locked = false;
                        //    //    }
                        //    //    db.Entry(ordr).State = EntityState.Modified;
                        //    //    db.SaveChanges();
                        //    //}

                        //}
                        ////else if (neworderPatternSequenceObj[0].PlatformName.Equals("S201", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    //delete already scheduled order in same shift
                        //    var oldTactsheetOdr = db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID  && m.Is_Locked != true ).ToList();
                        //    db.RS_OM_S201_Tactsheet_Orders.RemoveRange(oldTactsheetOdr);
                        //    db.SaveChanges();
                        //    //retive other shift orders
                        //    var OtherShiftOrderS201 = db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Planned_Date.Year == plannedDate.Year
                        //       && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day && m.Is_Locked != true && m.Shift_ID != shiftId).ToList();
                        //    ////

                        //    //for updating locked orders
                        //    var PlatformName = neworderPatternSequenceObj[0].PlatformName;
                        //    var platformIdlocked = db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && m.Platform_Name.ToUpper().Trim() == PlatformName.ToUpper().Trim()).Select(m => m.Platform_ID).FirstOrDefault();

                        //    foreach (var item in neworderPatternSequenceObj)
                        //    {
                        //        if (S201RSNSkipCount == 0)
                        //        {
                        //            var model = db.RS_Model_Master.Where(m => m.Model_Code == item.Model_Code && m.Plant_ID == plantID && m.Shop_ID == ShopID).FirstOrDefault();
                        //            List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
                        //            var platformId = db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && m.Platform_Name.ToUpper().Trim() == item.PlatformName.ToUpper().Trim()).Select(m => m.Platform_ID).FirstOrDefault();
                        //            if (db.RS_OM_S201_Tactsheet_Orders.Any(m => m.Order_No.Equals(item.Order_No, StringComparison.CurrentCultureIgnoreCase)))
                        //            {
                        //                objJSONData.status = false;
                        //                objJSONData.message = "Order Already Exist: Error occured while locking Order NO: " + item.Order_No;
                        //                objJSONData.type = "fail";

                        //                return Json(objJSONData, JsonRequestBehavior.AllowGet);
                        //            }

                        //            for (int i = 0; i < attributionParameters.Count; i++)
                        //            {
                        //                AttributionParameters attributionParameter = attributionParameters[i];
                        //                try
                        //                {
                        //                    Convert.ToInt32(attributionParameter.Value);
                        //                }
                        //                catch (Exception)
                        //                {
                        //                    continue;
                        //                }
                        //                if (attributionParameter.label.Equals("Vehicle Series", StringComparison.InvariantCultureIgnoreCase))
                        //                {
                        //                    int attrId = Convert.ToInt32(attributionParameter.Value);
                        //                    var Series = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                        //                    bot = db.RS_Vehicle_Series.Where(m => m.Plant_ID == model.Plant_ID && m.Shop_ID == model.Shop_ID && m.Attribute_Name.ToLower() == Series.ToLower()).OrderByDescending(m => m.Inserted_Date).Select(m => m.BOT).FirstOrDefault();
                        //                }
                        //            }
                        //            RS_OM_S201_Tactsheet_Orders S201obj = new RS_OM_S201_Tactsheet_Orders();
                        //            S201obj.Plant_ID = plantID;
                        //            S201obj.Shop_ID = ShopID;
                        //            S201obj.Platform_ID = platformId;
                        //            S201obj.Shift_ID = Convert.ToDecimal(item.Planned_Shift_ID);
                        //            S201obj.RSN = S201RSNCount++;
                        //            S201obj.Planned_Date = plannedDate;
                        //            S201obj.Order_No = item.Order_No;
                        //            S201obj.Order_Status = 1;
                        //            if (S201lockSkipCount < 5)
                        //            {
                        //                S201obj.Is_Locked = true;
                        //                S201lockSkipCount++;
                        //            }
                        //            else
                        //            {
                        //                S201obj.Is_Locked = false;
                        //            }
                        //            S201obj.Is_Started = false;
                        //            S201obj.Model_Code = item.Model_Code;
                        //            S201obj.BOT = bot == null ? "NA" : bot; ;
                        //            S201obj.Style_Code = model.Style_Code;//change this
                        //            S201obj.Inserted_Date = DateTime.Now;
                        //            db.RS_OM_S201_Tactsheet_Orders.Add(S201obj);
                        //            db.SaveChanges();
                        //        }
                        //        else
                        //        {
                        //            S201RSNSkipCount--;
                        //        }
                        //    }
                        //    //updated rns of other shifts
                        //    //foreach (var ordr in OtherShiftOrderS201)
                        //    //{
                        //    //    ordr.RSN = S201RSNCount++;
                        //    //    if (S201lockSkipCount < minLockCount)
                        //    //    {
                        //    //        ordr.Is_Locked = true;
                        //    //        S201lockSkipCount++;
                        //    //    }
                        //    //    else
                        //    //    {
                        //    //        ordr.Is_Locked = false;
                        //    //    }
                        //    //    db.Entry(ordr).State = EntityState.Modified;
                        //    //    db.SaveChanges();
                        //    //}
                        //}
                        //else if (neworderPatternSequenceObj[0].PlatformName.Equals("XYLO", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    minLockCount = 0;
                        //    //delete already scheduled order in same shift
                        //    var oldTactsheetOdr = db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Is_Locked != true).ToList();
                        //    db.RS_OM_XYLO_Tactsheet_Orders_Sequence.RemoveRange(oldTactsheetOdr);
                        //    db.SaveChanges();
                        //    //retirve other shift orders

                        //    //end of updating locked order
                        //    foreach (var item in neworderPatternSequenceObj)
                        //    {

                        //            var model = db.RS_Model_Master.Where(m => m.Model_Code == item.Model_Code && m.Plant_ID == plantID && m.Shop_ID == ShopID).FirstOrDefault();
                        //            List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));
                        //            var platformId = db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID && m.Line_ID == lineId && m.Platform_Name.ToUpper().Trim() == item.PlatformName.ToUpper().Trim()).Select(m => m.Platform_ID).FirstOrDefault();

                        //            for (int i = 0; i < attributionParameters.Count; i++)
                        //            {
                        //                AttributionParameters attributionParameter = attributionParameters[i];
                        //                try
                        //                {
                        //                    Convert.ToInt32(attributionParameter.Value);
                        //                }
                        //                catch (Exception)
                        //                {

                        //                    continue;
                        //                }
                        //                if (attributionParameter.label.Equals("Vehicle Series", StringComparison.InvariantCultureIgnoreCase))
                        //                {
                        //                    int attrId = Convert.ToInt32(attributionParameter.Value);
                        //                    var Series = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                        //                    bot = db.RS_Vehicle_Series.Where(m => m.Plant_ID == model.Plant_ID && m.Shop_ID == model.Shop_ID && m.Attribute_Name.ToLower() == Series.ToLower()).OrderByDescending(m => m.Inserted_Date).Select(m => m.BOT).FirstOrDefault();
                        //                }
                        //            }
                        //            RS_OM_XYLO_Tactsheet_Orders_Sequence U321obj = new RS_OM_XYLO_Tactsheet_Orders_Sequence();
                        //            U321obj.Plant_ID = plantID;
                        //            U321obj.Shop_ID = ShopID;
                        //            U321obj.Platform_ID = platformId;
                        //            U321obj.Shift_ID = Convert.ToDecimal(item.Planned_Shift_ID);
                        //            //U321obj.RSN = U321RSNCount++;
                        //            U321obj.Planned_Date = plannedDate;
                        //            U321obj.Order_No = item.Order_No;
                        //            U321obj.Order_Status = 1;
                        //            U321obj.Is_Locked = false;
                        //            U321obj.Is_Started = false;
                        //            U321obj.Model_Code = item.Model_Code;
                        //            U321obj.BOT = bot == null ? "NA" : bot;
                        //            U321obj.Style_Code = model.Style_Code;//change this
                        //            U321obj.Inserted_Date = DateTime.Now;
                        //            db.RS_OM_XYLO_Tactsheet_Orders_Sequence.Add(U321obj);
                        //            db.SaveChanges();

                        //    }
                        //    //updated rns of other shifts

                        //}
                    }



                    ///end of saving tactsheet
                    if (neworderPatternSequenceObj != null)
                    {
                        var totalShift = (from shiftObj in db.RS_Shift
                                          where shiftObj.Plant_ID == plantID && shiftObj.Shift_ID == currentShiftID
                                          select shiftObj).ToList();
                        if (totalShift != null)
                        {
                            for (int i = 0; i < totalShift.Count; i++)
                            {


                                RS_Shift mmShiftObj = totalShift[i];
                                if (mmShiftObj != null)
                                {

                                    DataTable dt = new DataTable();
                                    dt.Columns.Add("Sr.No", typeof(string));
                                    dt.Columns.Add("Order No", typeof(string));
                                    //dt.Columns.Add("Platform", typeof(string));
                                    dt.Columns.Add("Auto Remark", typeof(string));
                                    dt.Columns.Add("Model Code", typeof(string));
                                    //dt.Columns.Add("Style Code", typeof(string));
                                    dt.Columns.Add("Color", typeof(string));
                                    //dt.Columns.Add("Drive", typeof(string));
                                    dt.Columns.Add("Start Time", typeof(string));
                                    //dt.Columns.Add("UB10 Completion Time", typeof(string));
                                    dt.Columns.Add("Completion Time", typeof(string));
                                    int rownumber = 1;
                                    string displayTime = null;
                                    string startdisplayTime = null;
                                    TimeSpan Shift_Start_Time = mmShiftObj.Shift_Start_Time;
                                    TimeSpan t1 = new TimeSpan(mmShiftObj.Shift_Start_Time.Hours, mmShiftObj.Shift_Start_Time.Minutes, mmShiftObj.Shift_Start_Time.Seconds);
                                    TimeSpan st1;
                                    //imeSpan Shift_End_Time = Shift.Break1_Time;
                                    //int tact_time =objRS_OM_Order_Pattern[0].Tact_Time;
                                    //calculating tact time//
                                    var currentShift = db.RS_Shift.Find(objShift[0].Shift_ID);//.Where(m => m.Shift_ID == objShift[0].Shift_ID && m.Plant_ID == plantID && m.Shop_ID == ShopID).ToList().Single();
                                    double availableShiftSeconds = 0;
                                    if (currentShift != null)
                                    {
                                        TimeSpan? availableShiftTime = currentShift.Shift_End_Time - currentShift.Shift_Start_Time;
                                        if (currentShift.Break1_End_Time != null && currentShift.Break1_Time != null)
                                        {
                                            availableShiftTime = availableShiftTime - (currentShift.Break1_End_Time - currentShift.Break1_Time);
                                        }
                                        if (currentShift.Break2_End_Time != null && currentShift.Break2_Time != null)
                                        {
                                            availableShiftTime = availableShiftTime - (currentShift.Break2_End_Time - currentShift.Break2_Time);
                                        }
                                        if (currentShift.Lunch_End_Time != null && currentShift.Lunch_Time != null)
                                        {
                                            availableShiftTime = availableShiftTime - (currentShift.Lunch_End_Time - currentShift.Lunch_Time);
                                        }

                                        //TimeSpan? availableShiftTime = currentShift.Shift_End_Time - currentShift.Shift_Start_Time - (currentShift.Break1_End_Time - currentShift.Break1_Time) - (currentShift.Break2_End_Time - currentShift.Break2_Time) - (currentShift.Lunch_End_Time - currentShift.Lunch_Time);
                                        if (availableShiftTime.HasValue)
                                        {
                                            availableShiftSeconds = availableShiftTime.Value.TotalSeconds;
                                        }

                                    }
                                    tact_time = (int)(availableShiftSeconds / prodCount);

                                    //
                                    int? productionCount = neworderPatternSequenceObj.Count();//prodCount;//Convert.ToInt16(objShift.Where(m => m.Shift_ID == mmShiftObj.Shift_ID).Select(m => m.Shift_Name).FirstOrDefault());
                                    for (int k = 0; k < productionCount; k++)
                                    {
                                        //////for finding drive of vehicle
                                        //string drive = string.Empty;

                                        //List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(neworderPatternSequenceObj[k].Attribution_Parameters, typeof(List<AttributionParameters>));
                                        //for (int m = 0; m < attributionParameters.Count; m++)
                                        //{
                                        //    AttributionParameters attributionParameter = attributionParameters[m];
                                        //    try
                                        //    {
                                        //        Convert.ToInt32(attributionParameter.Value);
                                        //    }
                                        //    catch (Exception)
                                        //    {

                                        //        continue;
                                        //    }
                                        //    if (attributionParameter.label.Equals("Vehicle Drive", StringComparison.InvariantCultureIgnoreCase))
                                        //    {
                                        //        int attrId = Convert.ToInt32(attributionParameter.Value);
                                        //        drive = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                                        //        //       attributionParameter.Value;
                                        //        break;
                                        //    }

                                        //}
                                        //////end vehicle drive

                                        if (t1 >= mmShiftObj.Shift_End_Time || rownumber == productionCount + 1)
                                        {
                                            break;
                                        }
                                        st1 = t1;
                                        t1 = t1.Add(new TimeSpan(0, 0, tact_time));

                                        RS_Shift firstBreakTime = (from shiftObj in db.RS_Shift
                                                                   where shiftObj.Plant_ID == plantID
                                                            && TimeSpan.Compare(shiftObj.Break1_Time.Value, t1) <= 0
                                                            && TimeSpan.Compare(shiftObj.Break1_End_Time.Value, t1) >= 0
                                                                   select shiftObj).FirstOrDefault();
                                        if (firstBreakTime != null)
                                        {
                                            TimeSpan t2 = t1.Subtract(firstBreakTime.Break1_Time.Value);
                                            if (t2.Minutes == 0 && t2.Seconds == 0)
                                            {
                                                StartTimespan = DateTime.Today.Add(st1);
                                                startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");
                                                timespan = DateTime.Today.Add(t1);
                                                displayTime = timespan.ToString("hh:mm:ss tt");


                                                dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                                rownumber++;
                                                t1 = firstBreakTime.Break1_End_Time.Value;
                                                t1 = t1.Add(new TimeSpan(0, 0, tact_time));
                                                //dt.Rows.Add("-----", "-------", "First Break", firstBreakTime.Break1_Time.Value+"-"+firstBreakTime.Break1_End_Time.Value, "------", "-------");
                                            }
                                            else
                                            {
                                                t1 = firstBreakTime.Break1_End_Time.Value.Add(t2);
                                                //t1 = t1.Subtract(new TimeSpan(0, 0, tact_time));
                                            }
                                        }
                                        RS_Shift LunchTime = (from shiftObj in db.RS_Shift
                                                              where shiftObj.Plant_ID == plantID
                                                       && TimeSpan.Compare(shiftObj.Lunch_Time.Value, t1) <= 0
                                                       && TimeSpan.Compare(shiftObj.Lunch_End_Time.Value, t1) >= 0
                                                              select shiftObj).FirstOrDefault();
                                        if (LunchTime != null)
                                        {
                                            TimeSpan t2 = t1.Subtract(LunchTime.Lunch_Time.Value);
                                            if (t2.Minutes == 0 && t2.Seconds == 0)
                                            {
                                                StartTimespan = DateTime.Today.Add(st1);
                                                startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                                timespan = DateTime.Today.Add(t1);
                                                displayTime = timespan.ToString("hh:mm:ss tt");
                                                dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                                rownumber++;
                                                t1 = LunchTime.Lunch_End_Time.Value;
                                                t1 = t1.Add(new TimeSpan(0, 0, tact_time));
                                            }
                                            else
                                            {
                                                t1 = LunchTime.Lunch_End_Time.Value.Add(t2);
                                                //t1 = t1.Subtract(new TimeSpan(0, 0, tact_time));
                                            }

                                        }
                                        RS_Shift SecondBreakTime = (from shiftObj in db.RS_Shift
                                                                    where shiftObj.Plant_ID == plantID
                                                             && TimeSpan.Compare(shiftObj.Break2_Time.Value, t1) <= 0
                                                             && TimeSpan.Compare(shiftObj.Break2_End_Time.Value, t1) >= 0
                                                                    select shiftObj).FirstOrDefault();
                                        if (SecondBreakTime != null)
                                        {
                                            TimeSpan t2 = t1.Subtract(SecondBreakTime.Break2_Time.Value);
                                            if (t2.Minutes == 0 && t2.Seconds == 0)
                                            {
                                                StartTimespan = DateTime.Today.Add(st1);
                                                startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                                timespan = DateTime.Today.Add(t1);
                                                displayTime = timespan.ToString("hh:mm:ss tt");
                                                dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                                rownumber++;
                                                t1 = SecondBreakTime.Break2_End_Time.Value;
                                                t1 = t1.Add(new TimeSpan(0, 0, tact_time));
                                                //FOR LAST CONDITION IT IS FAILING...ADDED 
                                                StartTimespan = DateTime.Today.Add(st1);
                                                startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                                timespan = DateTime.Today.Add(t1);
                                                displayTime = timespan.ToString("hh:mm:ss tt");
                                                //dt.Rows.Add(rownumber++, objRS_OM_Order_Pattern[j].Platform_Name, objRS_OM_Order_Pattern[j].ModelCode, objRS_OM_Order_Pattern[j].Color,  displayTime);
                                                dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                                rownumber++;
                                            }
                                            else
                                            {
                                                t1 = SecondBreakTime.Break2_End_Time.Value.Add(t2);
                                                //t1 = t1.Subtract(new TimeSpan(0, 0, tact_time));
                                                //FOR LAST CONDITION IT IS FAILING...ADDED 
                                                StartTimespan = DateTime.Today.Add(st1);
                                                startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                                timespan = DateTime.Today.Add(t1);
                                                displayTime = timespan.ToString("hh:mm:ss tt");
                                                //dt.Rows.Add(rownumber++, objRS_OM_Order_Pattern[j].Platform_Name, objRS_OM_Order_Pattern[j].ModelCode, objRS_OM_Order_Pattern[j].Color,  displayTime);
                                                dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                                rownumber++;
                                            }
                                        }
                                        else
                                        {
                                            StartTimespan = DateTime.Today.Add(st1);
                                            startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                            StartTimespan = DateTime.Today.Add(st1);
                                            startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                            timespan = DateTime.Today.Add(t1);
                                            displayTime = timespan.ToString("hh:mm:ss tt");
                                            timespan = DateTime.Today.Add(t1);
                                            displayTime = timespan.ToString("hh:mm:ss tt");
                                            //dt.Rows.Add(rownumber++, objRS_OM_Order_Pattern[j].Platform_Name, objRS_OM_Order_Pattern[j].ModelCode, objRS_OM_Order_Pattern[j].Color,  displayTime);
                                            dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                            rownumber++;
                                        }
                                    }
                                    totalShiftDs.Tables.Add(dt);
                                }
                            }
                        }
                        else
                        {
                            objJSONData.status = false;
                            objJSONData.message = "Shift is Not Configured... please add shift to generate tact sheet!...";
                            objJSONData.type = "Shift Not Confiured";
                        }
                    }
                }

                string fileName = "";
                if (totalShiftDs.Tables.Count > 0)
                {
                    string platformName = db.RS_OM_Platform.Where(p => p.Line_ID == lineId).Select(m => m.Platform_Name).FirstOrDefault();

                    fileName = ExportToPdf(totalShiftDs, platformName, shiftName, tact_time);
                    //return ExportToPdf(dt);
                    objJSONData.status = true;
                    objJSONData.message = "Tact Sheet created... to download file click on Download Link, below tact Sheet button";
                    objJSONData.type = fileName;
                    //by mukesh
                    //Session["pDFDownloadLink"] = fileName;
                    //
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }
                return Json(objJSONData, JsonRequestBehavior.AllowGet);
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
                        generalHelper.addControllerException(dbex, "OMOrderPlanningController", "GenerateTactSheet(Post) " + validationError, ((FDSession)this.Session["FDSession"]).userId);
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
                objJSONData.status = false;
                objJSONData.message = ex.Message;
                objJSONData.type = "fail";
                generalHelper.addControllerException(ex, "OMOrderPlanningController", "GenerateTactSheet(Post)", ((FDSession)this.Session["FDSession"]).userId);
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }
            //}
        }

        public ActionResult GenerateAndDownloadTactSheet(List<string> allShift, decimal platform_Id, int ShopID)
        {
            JSONData objJSONData = new JSONData();



            int tact_time = 0;
            var shiftName = "";
            decimal shiftId = 0;
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var lineId = Convert.ToInt16(GetLineIdByPlatform(platform_Id));
            DateTime plannedDate = getPlannedDate();
            try
            {
                DataSet totalShiftDs = new DataSet();

                DateTime timespan = new DateTime();
                DateTime StartTimespan = new DateTime();
                List<CummulativeFields> neworderPatternSequenceObj = new List<CummulativeFields>();
                //List<OrderPattern> objRS_OM_Order_Pattern = (List<OrderPattern>)JsonConvert.DeserializeObject(dataModal[0], typeof(List<OrderPattern>));
                List<Shift> objShift = (List<Shift>)JsonConvert.DeserializeObject(allShift[0], typeof(List<Shift>));
                var currentShiftID = objShift[0].Shift_ID;
                shiftName = objShift[0].Shift_Name;
                shiftId = objShift[0].Shift_ID;

                //var plannedDateVal = DateTime.Now.Date ;//objRS_OM_Order_Pattern[0].Planned_Date.ToString("yyyy-MM-dd");
                //DateTime dateVal = DateTime.ParseExact(plannedDateVal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //DateTime dateVal = DateTime.Now.Date;
                /////

                //DateTime plannedDate = Convert.ToDateTime(PlannedDate);// Convert.ToDateTime(objRS_OM_Order_Pattern[0].Planned_Date);
                //DateTime plannedDate = DateTime.Now.Date;


                neworderPatternSequenceObj = GetOrderSequenceBasedOnRatioByPlannedDate(plantID, ShopID, lineId, plannedDate);
                neworderPatternSequenceObj = (from or in neworderPatternSequenceObj.Where(m => m.Planned_Shift_ID == shiftId)
                                              join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID)
                                              on or.Model_Code equals mm.Model_Code into ormm
                                              from om in ormm.DefaultIfEmpty()
                                              join pf in db.RS_OM_Platform.Where(m => m.Plant_ID == plantID && m.Shop_ID == ShopID)
                                              on om.Platform_Id equals pf.Platform_ID into ompf
                                              from op in ompf.DefaultIfEmpty()
                                                  //join cc in db.RS_Colour//.Where(m => m.Plant_ID == plantID)
                                                  //on or.Color_code equals cc.Colour_ID //into omcc
                                                  // from omc in omcc.DefaultIfEmpty()
                                              orderby or.RSN ascending
                                              select new CummulativeFields()
                                              {
                                                  Row_ID = or.Row_ID,
                                                  Order_No = or.Order_No,
                                                  Model_Code = or.Model_Code.Trim(),
                                                  Auto_Remarks = om.Auto_Remarks,
                                                  //Attribution_Parameters = om.Attribution_Parameters,
                                                  Color_code = or.Color_code,
                                                  Inserted_Date = or.Inserted_Date,
                                                  PlatformName = op.Platform_Name,
                                                  Color_Name = db.RS_Colour.Where(m => m.Colour_ID == or.Color_code).Select(m => m.Colour_Desc).FirstOrDefault(),
                                                  //Color_Name = cc.Colour_Desc,
                                                  //Style_Code = or.Style_Code,
                                                  Planned_Shift_ID = or.Planned_Shift_ID,
                                              }).Distinct().ToList();
                if (neworderPatternSequenceObj != null)
                {
                    var prodCount = neworderPatternSequenceObj.Count();
                                                                                                 var totalShift = (from shiftObj in db.RS_Shift
                                      where shiftObj.Plant_ID == plantID && shiftObj.Shift_ID == currentShiftID
                                      select shiftObj).ToList();
                    if (totalShift != null)
                    {
                        for (int i = 0; i < totalShift.Count; i++)
                        {
                            RS_Shift mmShiftObj = totalShift[i];
                            if (mmShiftObj != null)
                            {

                                DataTable dt = new DataTable();
                                dt.Columns.Add("Sr.No", typeof(string));
                                dt.Columns.Add("Order No", typeof(string));
                                //dt.Columns.Add("Platform", typeof(string));
                                dt.Columns.Add("Auto Remark", typeof(string));
                                //dt.Columns.Add("Style Code", typeof(string));
                                dt.Columns.Add("Model Code", typeof(string));
                                dt.Columns.Add("Color", typeof(string));
                                //dt.Columns.Add("Drive", typeof(string));
                                dt.Columns.Add("Start Time", typeof(string));
                                //dt.Columns.Add("UB10 Completion Time", typeof(string));
                                dt.Columns.Add("Completion Time", typeof(string));
                                int rownumber = 1;
                                string displayTime = null;
                                string startdisplayTime = null;
                                TimeSpan Shift_Start_Time = mmShiftObj.Shift_Start_Time;
                                TimeSpan t1 = new TimeSpan(mmShiftObj.Shift_Start_Time.Hours, mmShiftObj.Shift_Start_Time.Minutes, mmShiftObj.Shift_Start_Time.Seconds);
                                TimeSpan st1;
                                //imeSpan Shift_End_Time = Shift.Break1_Time;
                                //int tact_time =objRS_OM_Order_Pattern[0].Tact_Time;
                                //calculating tact time//
                                var currentShift = db.RS_Shift.Find(objShift[0].Shift_ID);//.Where(m => m.Shift_ID == objShift[0].Shift_ID && m.Plant_ID == plantID && m.Shop_ID == ShopID).ToList().Single();
                                double availableShiftSeconds = 0;
                                if (currentShift != null)
                                {
                                    TimeSpan? availableShiftTime = currentShift.Shift_End_Time - currentShift.Shift_Start_Time;
                                    if (currentShift.Break1_End_Time != null && currentShift.Break1_Time != null)
                                    {
                                        availableShiftTime = availableShiftTime - (currentShift.Break1_End_Time - currentShift.Break1_Time);
                                    }
                                    if (currentShift.Break2_End_Time != null && currentShift.Break2_Time != null)
                                    {
                                        availableShiftTime = availableShiftTime - (currentShift.Break2_End_Time - currentShift.Break2_Time);
                                    }
                                    if (currentShift.Lunch_End_Time != null && currentShift.Lunch_Time != null)
                                    {
                                        availableShiftTime = availableShiftTime - (currentShift.Lunch_End_Time - currentShift.Lunch_Time);
                                    }

                                    //TimeSpan? availableShiftTime = currentShift.Shift_End_Time - currentShift.Shift_Start_Time - (currentShift.Break1_End_Time - currentShift.Break1_Time) - (currentShift.Break2_End_Time - currentShift.Break2_Time) - (currentShift.Lunch_End_Time - currentShift.Lunch_Time);
                                    if (availableShiftTime.HasValue)
                                    {
                                        availableShiftSeconds = availableShiftTime.Value.TotalSeconds;
                                    }

                                }
                                tact_time = (int)(availableShiftSeconds / prodCount);

                                //
                                int? productionCount = neworderPatternSequenceObj.Count();//prodCount;//Convert.ToInt16(objShift.Where(m => m.Shift_ID == mmShiftObj.Shift_ID).Select(m => m.Shift_Name).FirstOrDefault());
                                for (int k = 0; k < productionCount; k++)
                                {
                                    ////////for finding drive of vehicle
                                    ////string drive = string.Empty;

                                    ////List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(neworderPatternSequenceObj[k].Attribution_Parameters, typeof(List<AttributionParameters>));
                                    ////for (int m = 0; m < attributionParameters.Count; m++)
                                    ////{
                                    ////    AttributionParameters attributionParameter = attributionParameters[m];
                                    ////    try
                                    ////    {
                                    ////        Convert.ToInt32(attributionParameter.Value);
                                    ////    }
                                    ////    catch (Exception)
                                    ////    {

                                    ////        continue;
                                    ////    }
                                    ////    if (attributionParameter.label.Equals("Vehicle Drive", StringComparison.InvariantCultureIgnoreCase))
                                    ////    {
                                    ////        int attrId = Convert.ToInt32(attributionParameter.Value);
                                    ////        drive = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                                    ////        //       attributionParameter.Value;
                                    ////        break;
                                    ////    }

                                    ////}
                                    ////////end vehicle drive

                                    if (t1 >= mmShiftObj.Shift_End_Time || rownumber == productionCount + 1)
                                    {
                                        break;
                                    }
                                    st1 = t1;
                                    t1 = t1.Add(new TimeSpan(0, 0, tact_time));

                                    RS_Shift firstBreakTime = (from shiftObj in db.RS_Shift
                                                               where shiftObj.Plant_ID == plantID
                                                        && TimeSpan.Compare(shiftObj.Break1_Time.Value, t1) <= 0
                                                        && TimeSpan.Compare(shiftObj.Break1_End_Time.Value, t1) >= 0
                                                               select shiftObj).FirstOrDefault();
                                    if (firstBreakTime != null)
                                    {
                                        TimeSpan t2 = t1.Subtract(firstBreakTime.Break1_Time.Value);
                                        if (t2.Minutes == 0 && t2.Seconds == 0)
                                        {
                                            StartTimespan = DateTime.Today.Add(st1);
                                            startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");
                                            timespan = DateTime.Today.Add(t1);
                                            displayTime = timespan.ToString("hh:mm:ss tt");


                                            dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                            rownumber++;
                                            t1 = firstBreakTime.Break1_End_Time.Value;
                                            t1 = t1.Add(new TimeSpan(0, 0, tact_time));
                                            //dt.Rows.Add("-----", "-------", "First Break", firstBreakTime.Break1_Time.Value+"-"+firstBreakTime.Break1_End_Time.Value, "------", "-------");
                                        }
                                        else
                                        {
                                            t1 = firstBreakTime.Break1_End_Time.Value.Add(t2);
                                            //t1 = t1.Subtract(new TimeSpan(0, 0, tact_time));
                                        }
                                    }
                                    RS_Shift LunchTime = (from shiftObj in db.RS_Shift
                                                          where shiftObj.Plant_ID == plantID
                                                   && TimeSpan.Compare(shiftObj.Lunch_Time.Value, t1) <= 0
                                                   && TimeSpan.Compare(shiftObj.Lunch_End_Time.Value, t1) >= 0
                                                          select shiftObj).FirstOrDefault();
                                    if (LunchTime != null)
                                    {
                                        TimeSpan t2 = t1.Subtract(LunchTime.Lunch_Time.Value);
                                        if (t2.Minutes == 0 && t2.Seconds == 0)
                                        {
                                            StartTimespan = DateTime.Today.Add(st1);
                                            startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                            timespan = DateTime.Today.Add(t1);
                                            displayTime = timespan.ToString("hh:mm:ss tt");
                                            dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                            rownumber++;
                                            t1 = LunchTime.Lunch_End_Time.Value;
                                            t1 = t1.Add(new TimeSpan(0, 0, tact_time));
                                        }
                                        else
                                        {
                                            t1 = LunchTime.Lunch_End_Time.Value.Add(t2);
                                            //t1 = t1.Subtract(new TimeSpan(0, 0, tact_time));
                                        }

                                    }
                                    RS_Shift SecondBreakTime = (from shiftObj in db.RS_Shift
                                                                where shiftObj.Plant_ID == plantID
                                                         && TimeSpan.Compare(shiftObj.Break2_Time.Value, t1) <= 0
                                                         && TimeSpan.Compare(shiftObj.Break2_End_Time.Value, t1) >= 0
                                                                select shiftObj).FirstOrDefault();
                                    if (SecondBreakTime != null)
                                    {
                                        TimeSpan t2 = t1.Subtract(SecondBreakTime.Break2_Time.Value);
                                        if (t2.Minutes == 0 && t2.Seconds == 0)
                                        {
                                            StartTimespan = DateTime.Today.Add(st1);
                                            startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                            timespan = DateTime.Today.Add(t1);
                                            displayTime = timespan.ToString("hh:mm:ss tt");
                                            dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                            rownumber++;
                                            t1 = SecondBreakTime.Break2_End_Time.Value;
                                            t1 = t1.Add(new TimeSpan(0, 0, tact_time));
                                            //FOR LAST CONDITION IT IS FAILING...ADDED 
                                            StartTimespan = DateTime.Today.Add(st1);
                                            startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                            timespan = DateTime.Today.Add(t1);
                                            displayTime = timespan.ToString("hh:mm:ss tt");
                                            //dt.Rows.Add(rownumber++, objRS_OM_Order_Pattern[j].Platform_Name, objRS_OM_Order_Pattern[j].ModelCode, objRS_OM_Order_Pattern[j].Color,  displayTime);
                                            dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                            rownumber++;
                                        }
                                        else
                                        {
                                            t1 = SecondBreakTime.Break2_End_Time.Value.Add(t2);
                                            //t1 = t1.Subtract(new TimeSpan(0, 0, tact_time));
                                            //FOR LAST CONDITION IT IS FAILING...ADDED 
                                            StartTimespan = DateTime.Today.Add(st1);
                                            startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                            timespan = DateTime.Today.Add(t1);
                                            displayTime = timespan.ToString("hh:mm:ss tt");
                                            //dt.Rows.Add(rownumber++, objRS_OM_Order_Pattern[j].Platform_Name, objRS_OM_Order_Pattern[j].ModelCode, objRS_OM_Order_Pattern[j].Color,  displayTime);
                                            dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                            rownumber++;
                                        }
                                    }
                                    else
                                    {
                                        StartTimespan = DateTime.Today.Add(st1);
                                        startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                        StartTimespan = DateTime.Today.Add(st1);
                                        startdisplayTime = StartTimespan.ToString("hh:mm:ss tt");

                                        timespan = DateTime.Today.Add(t1);
                                        displayTime = timespan.ToString("hh:mm:ss tt");
                                        timespan = DateTime.Today.Add(t1);
                                        displayTime = timespan.ToString("hh:mm:ss tt");
                                        //dt.Rows.Add(rownumber++, objRS_OM_Order_Pattern[j].Platform_Name, objRS_OM_Order_Pattern[j].ModelCode, objRS_OM_Order_Pattern[j].Color,  displayTime);
                                        dt.Rows.Add(rownumber, neworderPatternSequenceObj[k].Order_No, neworderPatternSequenceObj[k].Auto_Remarks, neworderPatternSequenceObj[k].Model_Code, neworderPatternSequenceObj[k].Color_Name, startdisplayTime, displayTime);
                                        rownumber++;
                                    }
                                }
                                totalShiftDs.Tables.Add(dt);
                            }
                        }
                    }
                    else
                    {
                        objJSONData.status = false;
                        objJSONData.message = "Shift is Not Configured... please add shift to generate tact sheet!...";
                        objJSONData.type = "Shift Not Confiured";
                    }
                }


                string fileName = "";
                if (totalShiftDs.Tables.Count > 0)
                {
                    string platformName = db.RS_OM_Platform.Where(p => p.Line_ID == lineId).Select(m => m.Platform_Name).FirstOrDefault();

                    fileName = ExportToPdf(totalShiftDs, platformName, shiftName, tact_time);
                    //return ExportToPdf(dt);
                    objJSONData.status = true;
                    objJSONData.message = "Tact Sheet created... to download file click on Download Link, below tact Sheet button";
                    objJSONData.type = fileName;
                    //by mukesh
                    //return File(fileName, "application/pdf");

                    //Session["pDFDownloadLink"] = fileName;
                    //
                    return Json(objJSONData, JsonRequestBehavior.AllowGet);
                }
                return Json(objJSONData, JsonRequestBehavior.AllowGet);
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
                        generalHelper.addControllerException(dbex, "OMOrderPlanningController", "GenerateAndDownoadTactSheet(Post) " + validationError, ((FDSession)this.Session["FDSession"]).userId);
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
                objJSONData.status = false;
                objJSONData.message = ex.Message;
                objJSONData.type = "fail";
                generalHelper.addControllerException(ex, "OMOrderPlanningController", "GenerateAndDownoadTactSheet(Post)", ((FDSession)this.Session["FDSession"]).userId);
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }
            //}
        }

        public int getMinLockCount(int PlantID, decimal PlatformId, DateTime plannedDate)
        {
            var minLockCount = 5;
            var nonZeroCount = 0;
            var totalRatioCount = 0;
            var currentPlatformRatio = 0;
            DateTime lastDate = new DateTime(0001, 01, 01);
            if (db.RS_OM_Order_Pattern.Count() > 0)
            {
                lastDate = db.RS_OM_Order_Pattern.Max(m => m.Inserted_Date);
            }

            DateTime lastInsertedDate = new DateTime(0001, 01, 01);
            if (db.RS_OM_UB10_Orders_Sequence.Count() > 0)
            {
                lastInsertedDate = db.RS_OM_UB10_Orders_Sequence.Max(m => m.Inserted_Date);
            }

            //------Get current Main line pattern-----
            var Platform = (from pf in db.RS_OM_Order_Pattern

                            join orderPattern in db.RS_OM_Platform on pf.Platform_ID equals orderPattern.Platform_ID
                            where orderPattern.Plant_ID == PlantID && pf.Inserted_Date.Year == lastDate.Year &&
                            pf.Inserted_Date.Month == lastDate.Month && pf.Inserted_Date.Day == lastDate.Day &&
                            pf.Inserted_Date.Hour == lastDate.Hour && pf.Inserted_Date.Hour == lastDate.Hour &&
                            pf.Inserted_Date.Minute == lastDate.Minute && pf.Inserted_Date.Second == lastDate.Second


                            orderby pf.Priority ascending
                            select new OrderPattern()
                            {
                                Ratio = pf.Ratio,
                                Platform_ID = orderPattern.Platform_ID,
                                Platform_Name = orderPattern.Platform_Name,
                                Planned_Date = pf.Planned_Date,
                                lineID = orderPattern.Line_ID,
                                Priority = (int)pf.Priority

                            }).ToList();
            var CurrentPlatformRatio = Platform.Where(m => m.Platform_ID == PlatformId).Select(m => m.Ratio).FirstOrDefault();

            //-------
            //var orderPatterID = db.RS_OM_Order_Pattern.Where(m => m.Plant_ID == PlantID && m.Platform_ID == PlatformId).Select(m => m.Row_ID).FirstOrDefault();

            var CurrentPlatformOrderCount = db.RS_OM_UB10_Orders_Sequence.Where(m => m.Plant_ID == PlantID && m.Platform_ID == PlatformId &&
               m.Planned_Date.Year == plannedDate.Year && m.Planned_Date.Month == plannedDate.Month && m.Planned_Date.Day == plannedDate.Day
               && m.Inserted_Date.Year == lastInsertedDate.Year && m.Inserted_Date.Month == lastInsertedDate.Month
               && m.Inserted_Date.Day == lastInsertedDate.Day).Count();

            foreach (var pl in Platform)
            {

                if (pl.Ratio != 0)
                {
                    nonZeroCount++;
                    totalRatioCount += pl.Ratio;
                }
                if (pl.Platform_ID == PlatformId)
                {
                    currentPlatformRatio = pl.Ratio;
                }
            }
            if (nonZeroCount == 0)
            {
                minLockCount = 0;
            }
            else if (nonZeroCount > 1)
            {
                minLockCount = ((int)(40 / totalRatioCount)) * CurrentPlatformRatio;
                if ((40 % totalRatioCount) != 0)
                {
                    minLockCount += CurrentPlatformRatio;
                }
                minLockCount = minLockCount - CurrentPlatformOrderCount;
            }
            else if (nonZeroCount == 1)
            {
                minLockCount = 25;
                minLockCount = minLockCount - CurrentPlatformOrderCount;

            }
            minLockCount = minLockCount <= 5 ? 5 : minLockCount;
            return minLockCount;
        }
        //   public Boolean updateOrderStartStatus(int shopId)
        //   {
        //       DateTime today = DateTime.Now;
        //       var plantId = ((FDSession)this.Session["FDSession"]).userId;
        //       JSONData objJSONData = new JSONData();
        //       try
        //       {
        //           var UB10orderObj = db.RS_OM_UB10_Orders_Sequence.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Is_Started != true && today.Year == m.Planned_Date.Year && today.Month == m.Planned_Date.Month
        //      && today.Day == m.Planned_Date.Day).OrderBy(m => m.RSN).ToList();



        //           if (UB10orderObj != null)
        //           {
        //               foreach (var order in UB10orderObj)
        //               {
        //                   if (db.RS_OM_Order_List.Any(m => m.Order_No == order.Order_No && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Order_Status.ToUpper() == "Started"))
        //                   {
        //                       order.Is_Started = true;
        //                       db.Entry(order).State = EntityState.Modified;
        //                       db.SaveChanges();
        //                   }
        //               }
        //           }
        //       }
        //       catch (Exception ex)
        //       {
        //           while (ex.InnerException != null)
        //           {
        //               ex = ex.InnerException;
        //           }

        //           generalHelper.addControllerException(ex, "OMOrderPlanningController", "updateOrderStartStatus()", ((FDSession)this.Session["FDSession"]).userId);
        //           return false;

        //       }

        //       try
        //       {
        //           var BCAorderObj = db.RS_OM_BCA_Orders_Sequence.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Is_Started != true && today.Year == m.Planned_Date.Year && today.Month == m.Planned_Date.Month
        //&& today.Day == m.Planned_Date.Day).OrderBy(m => m.RSN).ToList();


        //           if (BCAorderObj != null)
        //           {
        //               foreach (var order in BCAorderObj)
        //               {
        //                   if (db.RS_OM_Order_List.Any(m => m.Order_No == order.Order_No && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Order_Status.ToUpper() == "Started"))
        //                   {
        //                       order.Serial_No = db.RS_OM_Order_List.Where(m => m.Order_No == order.Order_No && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Order_Status.ToUpper() == "Started").Select(m => m.Serial_No).FirstOrDefault();
        //                       order.Is_Started = true;
        //                       db.Entry(order).State = EntityState.Modified;
        //                       db.SaveChanges();
        //                   }
        //               }

        //           }
        //       }
        //       catch (Exception ex)
        //       {
        //           while (ex.InnerException != null)
        //           {
        //               ex = ex.InnerException;
        //           }

        //           generalHelper.addControllerException(ex, "OMOrderPlanningController", "updateOrderStartStatus()", ((FDSession)this.Session["FDSession"]).userId);
        //           return false;

        //       }

        //       try
        //       {
        //           var U321orderObj = db.RS_OM_U321_Tactsheet_Orders.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Is_Started != true && today.Year == m.Planned_Date.Year && today.Month == m.Planned_Date.Month
        // && today.Day == m.Planned_Date.Day).OrderBy(m => m.RSN).ToList();




        //           if (U321orderObj != null)
        //           {
        //               foreach (var order in U321orderObj)
        //               {
        //                   if (db.RS_OM_Order_List.Any(m => m.Order_No == order.Order_No && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Order_Status.ToUpper() == "Started"))
        //                   {
        //                       order.Serial_No = db.RS_OM_Order_List.Where(m => m.Order_No == order.Order_No && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Order_Status.ToUpper() == "Started").Select(m => m.Serial_No).FirstOrDefault();
        //                       order.Is_Started = true;
        //                   }
        //                   db.Entry(order).State = EntityState.Modified;
        //                   db.SaveChanges();
        //               }

        //           }
        //       }
        //       catch (Exception ex)
        //       {
        //           while (ex.InnerException != null)
        //           {
        //               ex = ex.InnerException;
        //           }

        //           generalHelper.addControllerException(ex, "OMOrderPlanningController", "updateOrderStartStatus()", ((FDSession)this.Session["FDSession"]).userId);
        //           return false;

        //       }

        //       //retrived top order
        //       try
        //       {
        //           var S201orderObj = db.RS_OM_S201_Tactsheet_Orders.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Is_Started != true && today.Year == m.Planned_Date.Year && today.Month == m.Planned_Date.Month
        //&& today.Day == m.Planned_Date.Day).OrderBy(m => m.RSN).ToList();

        //           if (S201orderObj != null)
        //           {
        //               foreach (var order in S201orderObj)
        //               {
        //                   if (db.RS_OM_Order_List.Any(m => m.Order_No == order.Order_No && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Order_Status.ToUpper() == "Started"))
        //                   {
        //                       order.Serial_No = db.RS_OM_Order_List.Where(m => m.Order_No == order.Order_No && m.Plant_ID == plantId && m.Shop_ID == shopId && m.Order_Status.ToUpper() == "Started").Select(m => m.Serial_No).FirstOrDefault();
        //                       order.Is_Started = true;
        //                   }
        //                   db.Entry(order).State = EntityState.Modified;
        //                   db.SaveChanges();
        //               }

        //           }
        //       }
        //       catch (Exception ex)
        //       {
        //           while (ex.InnerException != null)
        //           {
        //               ex = ex.InnerException;
        //           }

        //           generalHelper.addControllerException(ex, "OMOrderPlanningController", "updateOrderStartStatus()", ((FDSession)this.Session["FDSession"]).userId);
        //           return false;

        //       }
        //       return true;

        //   }
        //}
        public class tactsheetData
        {
            public decimal Plant_ID { get; set; }
            public decimal Shop_ID { get; set; }
            public int Order_ID { get; set; }
            public string Order_No { get; set; }
            public decimal Shift_ID { get; set; }
            public decimal Platform_ID { get; set; }
            public string Is_Released { get; set; }
            public string Is_Started { get; set; }
            public string Is_Sent_To_PLC { get; set; }
            public string Is_Hold { get; set; }
            public string Planned_Time { get; set; }
            public decimal RSN { get; set; }
            public Nullable<System.DateTime> Planned_Date { get; set; }
            public Nullable<System.DateTime> Inserted_Time { get; set; }
            public Nullable<System.DateTime> Last_Status_Change_Time { get; set; }

        }
        public List<CummulativeFields> GetOrderSequenceBasedOnRatioByPlannedDate(int plantID, int shopID, int lineID, DateTime plannedDate)
        {
            List<CummulativeFields> orderPatternSequenceObj = new List<CummulativeFields>();

            bool showOrder = false;
            decimal platformID = 0;
            var c = 0;
            try
            {
                if (!db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID && m.Line_ID == lineID && m.Order_Status == "Release" && m.Order_Start == false && m.Planned_Date == DbFunctions.TruncateTime(plannedDate)).Select(m => m.Is_Sequenced).FirstOrDefault())
                {
                    DateTime lastDate = new DateTime(0001, 01, 01);
                    //to get updated order pattern
                    if (db.RS_OM_Order_Pattern.Count() > 0)
                    {
                        lastDate = db.RS_OM_Order_Pattern.Max(m => m.Inserted_Date);
                    }
                    var OrderPattern = db.RS_OM_Order_Pattern.Where(m => m.Plant_ID == plantID && m.Inserted_Date.Year == lastDate.Year && m.Inserted_Date.Month == lastDate.Month && m.Inserted_Date.Day == lastDate.Day && m.Inserted_Date.Hour == lastDate.Hour && m.Inserted_Date.Minute == lastDate.Minute && m.Inserted_Date.Second == lastDate.Second && m.Inserted_Date.Year == lastDate.Year).OrderByDescending(m => m.Planned_Date).Take(3).ToList();
                    ////added to reset patter ratio for specific platform SO THAT PATTERN WONT BE USED

                    foreach (var pat in OrderPattern)
                    {
                        pat.Ratio = 0;
                    }
                    //OrderPattern[0].Ratio = 0;
                    //OrderPattern[1].Ratio = 0;
                    //OrderPattern[2].Ratio = 0;
                    int platform_id = Convert.ToInt32(db.RS_OM_Platform.Where(p => p.Line_ID == lineID).Select(m => m.Platform_ID).FirstOrDefault());
                    foreach (var pattern in OrderPattern)
                    {
                        if (pattern.Platform_ID == platform_id)
                        {
                            pattern.Ratio = 1;
                        }
                    }

                    ////
                    //  var OrderPattern = {};
                    if (OrderPattern != null)
                    {

                        if (OrderPattern.Count() > 1)
                        {
                            platformID = OrderPattern[0].Platform_ID;
                            var totalOrderbasedOnPlatform1 = (from or in db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              on or.Model_Code equals mm.Model_Code
                                                              where or.Plant_ID == plantID && DbFunctions.TruncateTime(or.Planned_Date) == DbFunctions.TruncateTime(plannedDate) && mm.Platform_Id == platformID
                                                               && or.Order_Status == "Release" && or.Order_Start == false
                                                              orderby or.RSN ascending
                                                              select new CummulativeFields()
                                                              {
                                                                  Row_ID = or.Row_ID,
                                                                  Order_No = or.Order_No,
                                                                  Model_Code = or.partno.Trim(),
                                                                  //Series = or.RS_Series.Series_Description,
                                                                  Color_code = or.Model_Color,
                                                                  Inserted_Date = or.Inserted_Date,
                                                                  remarks = or.Remarks,
                                                                  orderType = or.Order_Type,
                                                                  RSN = or.RSN,
                                                                  Style_Code = mm.Style_Code,
                                                                  Planned_Shift_ID = or.Planned_Shift_ID
                                                              }).ToList();
                            if (totalOrderbasedOnPlatform1 != null)
                            {

                                int index = 0;
                                int counter = 1;
                                //var ChangeRSN = totalOrderbasedOnPlatform.Take(OrderPattern[i].Ratio).ToList();
                                //foreach (CummulativeFields item in ChangeRSN)
                                //{
                                //    item.RSN = counter;
                                //    counter++;
                                //    orderPatternSequenceObj.Add(item);
                                //    totalOrderbasedOnPlatform.Remove(item);
                                //}
                                for (int j = 0; j < totalOrderbasedOnPlatform1.Count; j++)
                                {
                                    if (index == totalOrderbasedOnPlatform1.Count)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < OrderPattern[0].Ratio; k++)
                                    {
                                        if (index == totalOrderbasedOnPlatform1.Count)
                                        {
                                            break;
                                        }
                                        totalOrderbasedOnPlatform1[index].RSN = counter;
                                        orderPatternSequenceObj.Add(totalOrderbasedOnPlatform1[index]);
                                        counter++;
                                        index++;
                                    }
                                    //counter--;
                                    c = 0;
                                    for (int i = 0; i < OrderPattern.Count; i++)
                                    {
                                        if (i != 0)
                                        {
                                            c += OrderPattern[i].Ratio;
                                        }
                                    }
                                    counter += c;
                                    //foreach (var rec in OrderPattern)
                                    //{
                                    //    c += rec.Ratio;
                                    //}
                                    //counter += OrderPattern[1].Ratio + OrderPattern[2].Ratio;
                                    //if (i==0)
                                    //{
                                    //    counter += OrderPattern[1].Ratio + OrderPattern[2].Ratio;
                                    //}
                                    //if (i == 1)
                                    //{
                                    //    counter += OrderPattern[2].Ratio + OrderPattern[0].Ratio;
                                    //}
                                    //if (i == 2)
                                    //{
                                    //    counter += OrderPattern[0].Ratio + OrderPattern[1].Ratio;
                                    //}

                                }

                            }
                        }
                        if (OrderPattern.Count() > 1)
                        {
                            platformID = OrderPattern[1].Platform_ID;
                            var totalOrderbasedOnPlatform2 = (from or in db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              on or.Model_Code equals mm.Model_Code
                                                              where or.Plant_ID == plantID && DbFunctions.TruncateTime(or.Planned_Date) == DbFunctions.TruncateTime(plannedDate) && mm.Platform_Id == platformID
                                                               && or.Order_Status == "Release" && or.Order_Start == false
                                                              orderby or.RSN ascending
                                                              select new CummulativeFields()
                                                              {
                                                                  Row_ID = or.Row_ID,
                                                                  Order_No = or.Order_No,
                                                                  Model_Code = or.partno.Trim(),
                                                                  //Series = or.RS_Series.Series_Description,
                                                                  Color_code = or.Model_Color,
                                                                  Inserted_Date = or.Inserted_Date,
                                                                  remarks = or.Remarks,
                                                                  orderType = or.Order_Type,
                                                                  RSN = or.RSN,
                                                                  Style_Code = mm.Style_Code,
                                                                  Planned_Shift_ID = or.Planned_Shift_ID
                                                              }).ToList();
                            if (totalOrderbasedOnPlatform2 != null)
                            {
                                int counter = OrderPattern[0].Ratio + 1;
                                int index = 0;
                                for (int j = 0; j < totalOrderbasedOnPlatform2.Count; j++)
                                {
                                    if (index == totalOrderbasedOnPlatform2.Count)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < OrderPattern[1].Ratio; k++)
                                    {
                                        if (index == totalOrderbasedOnPlatform2.Count)
                                        {
                                            break;
                                        }
                                        totalOrderbasedOnPlatform2[index].RSN = counter;
                                        orderPatternSequenceObj.Add(totalOrderbasedOnPlatform2[index]);
                                        counter++;
                                        index++;
                                    }
                                    //counter--;
                                    c = 0;
                                    for (int i = 0; i < OrderPattern.Count; i++)
                                    {
                                        if (i != 1)
                                        {
                                            c += OrderPattern[i].Ratio;
                                        }
                                    }
                                    counter += c;
                                    //counter += OrderPattern[2].Ratio + OrderPattern[0].Ratio;
                                }

                            }
                        }
                        if (OrderPattern.Count() > 2)
                        {
                            platformID = OrderPattern[2].Platform_ID;
                            var totalOrderbasedOnPlatform3 = (from or in db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              on or.Model_Code equals mm.Model_Code
                                                              where or.Plant_ID == plantID && DbFunctions.TruncateTime(or.Planned_Date) == DbFunctions.TruncateTime(plannedDate) && mm.Platform_Id == platformID
                                                               && or.Order_Status == "Release" && or.Order_Start == false
                                                              orderby or.RSN ascending
                                                              select new CummulativeFields()
                                                              {
                                                                  Row_ID = or.Row_ID,
                                                                  Order_No = or.Order_No,
                                                                  Model_Code = or.partno.Trim(),
                                                                  //Series = or.RS_Series.Series_Description,
                                                                  Color_code = or.Model_Color,
                                                                  Inserted_Date = or.Inserted_Date,
                                                                  remarks = or.Remarks,
                                                                  orderType = or.Order_Type,
                                                                  RSN = or.RSN,
                                                                  Style_Code = mm.Style_Code,
                                                                  Planned_Shift_ID = or.Planned_Shift_ID
                                                              }).ToList();
                            if (totalOrderbasedOnPlatform3 != null)
                            {
                                //int counter = Convert.ToInt16(totalOrderbasedOnPlatform3[0].RSN);
                                int index = 0;
                                int counter = OrderPattern[0].Ratio + OrderPattern[1].Ratio + 1;
                                for (int j = 0; j < totalOrderbasedOnPlatform3.Count; j++)
                                {
                                    if (index == totalOrderbasedOnPlatform3.Count)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < OrderPattern[2].Ratio; k++)
                                    {
                                        if (index == totalOrderbasedOnPlatform3.Count)
                                        {
                                            break;
                                        }
                                        totalOrderbasedOnPlatform3[index].RSN = counter;
                                        orderPatternSequenceObj.Add(totalOrderbasedOnPlatform3[index]);
                                        counter++;
                                        index++;
                                    }
                                    //counter--;
                                    c = 0;
                                    for (int i = 0; i < OrderPattern.Count; i++)
                                    {
                                        if (i != 2)
                                        {
                                            c += OrderPattern[i].Ratio;
                                        }
                                    }
                                    counter += c;
                                    //counter += OrderPattern[1].Ratio + OrderPattern[0].Ratio;
                                }

                            }
                        }
                        //var newobj = (from op in db.RS_Model_Master
                        //              join mm in db.RS_Model_Master.Where(m=>m.))
                        // orderPatternSequenceObj.Add(patternObj);
                        //}
                        //}

                    }
                    else
                    {
                        showOrder = true;
                    }
                }
                else
                {
                    showOrder = true;
                }
                if (showOrder)
                {
                    //commented by ketan Date 04/09/2017
                    //orderSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Inserted_Date.Year == plannedDate.Year && or.Inserted_Date.Month == plannedDate.Month && or.Inserted_Date.Day == plannedDate.Day)
                    //orderSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Date == plannedDate && or.Order_Type == orderType)

                    //modify
                    orderPatternSequenceObj = (from or in db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                               join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                               on or.Model_Code equals mm.Model_Code
                                               where or.Plant_ID == plantID && DbFunctions.TruncateTime(or.Planned_Date) == DbFunctions.TruncateTime(plannedDate) && or.Shop_ID == shopID && or.Line_ID == lineID
                                                && or.Order_Status == "Release" && or.Order_Start == false
                                               orderby or.RSN ascending
                                               select new CummulativeFields()
                                               {
                                                   Row_ID = or.Row_ID,
                                                   Order_No = or.Order_No,
                                                   Model_Code = or.partno.Trim(),
                                                   //Series = or.RS_Series.Series_Description,
                                                   Color_code = or.Model_Color,
                                                   Inserted_Date = or.Inserted_Date,
                                                   remarks = or.Remarks,
                                                   orderType = or.Order_Type,
                                                   RSN = or.RSN,
                                                   Style_Code = mm.Style_Code,
                                                   Planned_Shift_ID = or.Planned_Shift_ID
                                               }).ToList();

                    //change old one mukesh

                    ////orderPatternSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && DbFunctions.TruncateTime(or.Planned_Date) == DbFunctions.TruncateTime(plannedDate))
                    ////                           orderby or.RSN ascending
                    ////                           select new CummulativeFields()
                    ////                           {
                    ////                               Row_ID = or.Row_ID,
                    ////                               Order_No = or.Order_No,
                    ////                               Model_Code = or.partno.Trim(),
                    ////                               //Series = or.RS_Series.Series_Description,
                    ////                               Color_code = or.Model_Color,
                    ////                               Inserted_Date = or.Inserted_Date,
                    ////                               remarks = or.Remarks,
                    ////                               orderType = or.Order_Type,

                    ////                               Planned_Shift_ID = or.Planned_Shift_ID
                    ////                               //EngineModelCode = db.RS_OM_OrderRelease.Where(a => a.Model_Code == or.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 1).FirstOrDefault().partno,
                    ////                               //TransmissionSeries = db.RS_OM_OrderRelease.Where(a => a.Model_Code == or.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 2).FirstOrDefault().RS_Series.Series_Description
                    ////                           }).ToList();
                }

            }
            catch (Exception)
            {

                throw;
            }
            return orderPatternSequenceObj.OrderBy(m => m.RSN).ToList();
        }
        public List<CummulativeFields> GetOrderSequenceBasedOnRatio(int plantID, int shopID, int lineID)
        {
            List<CummulativeFields> orderPatternSequenceObj = new List<CummulativeFields>();

            bool showOrder = false;
            decimal platformID = 0;
            var c = 0;
            try
            {
                if (!db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID && m.Line_ID == lineID && m.Order_Status == "Release" && m.Order_Start == false).Select(m => m.Is_Sequenced).FirstOrDefault())
                {
                    DateTime lastDate = new DateTime(0001, 01, 01);
                    //to get updated order pattern
                    if (db.RS_OM_Order_Pattern.Count() > 0)
                    {
                        lastDate = db.RS_OM_Order_Pattern.Max(m => m.Inserted_Date);
                    }
                    var OrderPattern = db.RS_OM_Order_Pattern.Where(m => m.Plant_ID == plantID && m.Inserted_Date.Year == lastDate.Year && m.Inserted_Date.Month == lastDate.Month && m.Inserted_Date.Day == lastDate.Day && m.Inserted_Date.Hour == lastDate.Hour && m.Inserted_Date.Minute == lastDate.Minute && m.Inserted_Date.Second == lastDate.Second && m.Inserted_Date.Year == lastDate.Year).OrderByDescending(m => m.Planned_Date).Take(3).ToList();
                    ////added to reset patter ratio for specific platform SO THAT PATTERN WONT BE USED

                    foreach (var pat in OrderPattern)
                    {
                        pat.Ratio = 0;
                    }
                    //OrderPattern[0].Ratio = 0;
                    //OrderPattern[1].Ratio = 0;
                    //OrderPattern[2].Ratio = 0;
                    int platform_id = Convert.ToInt32(db.RS_OM_Platform.Where(p => p.Line_ID == lineID).Select(m => m.Platform_ID).FirstOrDefault());
                    foreach (var pattern in OrderPattern)
                    {
                        if (pattern.Platform_ID == platform_id)
                        {
                            pattern.Ratio = 1;
                        }
                    }

                    ////
                    //  var OrderPattern = {};
                    if (OrderPattern != null)
                    {

                        if (OrderPattern.Count() > 1)
                        {
                            platformID = OrderPattern[0].Platform_ID;
                            var totalOrderbasedOnPlatform1 = (from or in db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              on or.Model_Code equals mm.Model_Code
                                                              where or.Plant_ID == plantID && mm.Platform_Id == platformID
                                                               && or.Order_Status == "Release" && or.Order_Start == false
                                                              orderby or.RSN ascending
                                                              select new CummulativeFields()
                                                              {
                                                                  Row_ID = or.Row_ID,
                                                                  Order_No = or.Order_No,
                                                                  Model_Code = or.partno.Trim(),
                                                                  //Series = or.RS_Series.Series_Description,
                                                                  Color_code = or.Model_Color,
                                                                  Inserted_Date = or.Inserted_Date,
                                                                  remarks = or.Remarks,
                                                                  orderType = or.Order_Type,
                                                                  RSN = or.RSN,
                                                                  Style_Code = mm.Style_Code,
                                                                  Planned_Shift_ID = or.Planned_Shift_ID
                                                              }).ToList();
                            if (totalOrderbasedOnPlatform1 != null)
                            {

                                int index = 0;
                                int counter = 1;
                                //var ChangeRSN = totalOrderbasedOnPlatform.Take(OrderPattern[i].Ratio).ToList();
                                //foreach (CummulativeFields item in ChangeRSN)
                                //{
                                //    item.RSN = counter;
                                //    counter++;
                                //    orderPatternSequenceObj.Add(item);
                                //    totalOrderbasedOnPlatform.Remove(item);
                                //}
                                for (int j = 0; j < totalOrderbasedOnPlatform1.Count; j++)
                                {
                                    if (index == totalOrderbasedOnPlatform1.Count)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < OrderPattern[0].Ratio; k++)
                                    {
                                        if (index == totalOrderbasedOnPlatform1.Count)
                                        {
                                            break;
                                        }
                                        totalOrderbasedOnPlatform1[index].RSN = counter;
                                        orderPatternSequenceObj.Add(totalOrderbasedOnPlatform1[index]);
                                        counter++;
                                        index++;
                                    }
                                    //counter--;
                                    c = 0;
                                    for (int i = 0; i < OrderPattern.Count; i++)
                                    {
                                        if (i != 0)
                                        {
                                            c += OrderPattern[i].Ratio;
                                        }
                                    }
                                    counter += c;
                                    //foreach (var rec in OrderPattern)
                                    //{
                                    //    c += rec.Ratio;
                                    //}
                                    //counter += OrderPattern[1].Ratio + OrderPattern[2].Ratio;
                                    //if (i==0)
                                    //{
                                    //    counter += OrderPattern[1].Ratio + OrderPattern[2].Ratio;
                                    //}
                                    //if (i == 1)
                                    //{
                                    //    counter += OrderPattern[2].Ratio + OrderPattern[0].Ratio;
                                    //}
                                    //if (i == 2)
                                    //{
                                    //    counter += OrderPattern[0].Ratio + OrderPattern[1].Ratio;
                                    //}

                                }

                            }
                        }
                        if (OrderPattern.Count() > 1)
                        {
                            platformID = OrderPattern[1].Platform_ID;
                            var totalOrderbasedOnPlatform2 = (from or in db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              on or.Model_Code equals mm.Model_Code
                                                              where or.Plant_ID == plantID && mm.Platform_Id == platformID
                                                               && or.Order_Status == "Release" && or.Order_Start == false
                                                              orderby or.RSN ascending
                                                              select new CummulativeFields()
                                                              {
                                                                  Row_ID = or.Row_ID,
                                                                  Order_No = or.Order_No,
                                                                  Model_Code = or.partno.Trim(),
                                                                  //Series = or.RS_Series.Series_Description,
                                                                  Color_code = or.Model_Color,
                                                                  Inserted_Date = or.Inserted_Date,
                                                                  remarks = or.Remarks,
                                                                  orderType = or.Order_Type,
                                                                  RSN = or.RSN,
                                                                  Style_Code = mm.Style_Code,
                                                                  Planned_Shift_ID = or.Planned_Shift_ID
                                                              }).ToList();
                            if (totalOrderbasedOnPlatform2 != null)
                            {
                                int counter = OrderPattern[0].Ratio + 1;
                                int index = 0;
                                for (int j = 0; j < totalOrderbasedOnPlatform2.Count; j++)
                                {
                                    if (index == totalOrderbasedOnPlatform2.Count)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < OrderPattern[1].Ratio; k++)
                                    {
                                        if (index == totalOrderbasedOnPlatform2.Count)
                                        {
                                            break;
                                        }
                                        totalOrderbasedOnPlatform2[index].RSN = counter;
                                        orderPatternSequenceObj.Add(totalOrderbasedOnPlatform2[index]);
                                        counter++;
                                        index++;
                                    }
                                    //counter--;
                                    c = 0;
                                    for (int i = 0; i < OrderPattern.Count; i++)
                                    {
                                        if (i != 1)
                                        {
                                            c += OrderPattern[i].Ratio;
                                        }
                                    }
                                    counter += c;
                                    //counter += OrderPattern[2].Ratio + OrderPattern[0].Ratio;
                                }

                            }
                        }
                        if (OrderPattern.Count() > 2)
                        {
                            platformID = OrderPattern[2].Platform_ID;
                            var totalOrderbasedOnPlatform3 = (from or in db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              join mm in db.RS_Model_Master.Where(m => m.Plant_ID == plantID && m.Shop_ID == shopID)
                                                              on or.Model_Code equals mm.Model_Code
                                                              where or.Plant_ID == plantID && mm.Platform_Id == platformID
                                                               && or.Order_Status == "Release" && or.Order_Start == false
                                                              orderby or.RSN ascending
                                                              select new CummulativeFields()
                                                              {
                                                                  Row_ID = or.Row_ID,
                                                                  Order_No = or.Order_No,
                                                                  Model_Code = or.partno.Trim(),
                                                                  //Series = or.RS_Series.Series_Description,
                                                                  Color_code = or.Model_Color,
                                                                  Inserted_Date = or.Inserted_Date,
                                                                  remarks = or.Remarks,
                                                                  orderType = or.Order_Type,
                                                                  RSN = or.RSN,
                                                                  Style_Code = mm.Style_Code,
                                                                  Planned_Shift_ID = or.Planned_Shift_ID
                                                              }).ToList();
                            if (totalOrderbasedOnPlatform3 != null)
                            {
                                //int counter = Convert.ToInt16(totalOrderbasedOnPlatform3[0].RSN);
                                int index = 0;
                                int counter = OrderPattern[0].Ratio + OrderPattern[1].Ratio + 1;
                                for (int j = 0; j < totalOrderbasedOnPlatform3.Count; j++)
                                {
                                    if (index == totalOrderbasedOnPlatform3.Count)
                                    {
                                        break;
                                    }
                                    for (int k = 0; k < OrderPattern[2].Ratio; k++)
                                    {
                                        if (index == totalOrderbasedOnPlatform3.Count)
                                        {
                                            break;
                                        }
                                        totalOrderbasedOnPlatform3[index].RSN = counter;
                                        orderPatternSequenceObj.Add(totalOrderbasedOnPlatform3[index]);
                                        counter++;
                                        index++;
                                    }
                                    //counter--;
                                    c = 0;
                                    for (int i = 0; i < OrderPattern.Count; i++)
                                    {
                                        if (i != 2)
                                        {
                                            c += OrderPattern[i].Ratio;
                                        }
                                    }
                                    counter += c;
                                    //counter += OrderPattern[1].Ratio + OrderPattern[0].Ratio;
                                }

                            }
                        }
                        //var newobj = (from op in db.RS_Model_Master
                        //              join mm in db.RS_Model_Master.Where(m=>m.))
                        // orderPatternSequenceObj.Add(patternObj);
                        //}
                        //}

                    }
                    else
                    {
                        showOrder = true;
                    }
                }
                else
                {
                    showOrder = true;
                }
                if (showOrder)
                {
                    //commented by ketan Date 04/09/2017
                    //orderSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Inserted_Date.Year == plannedDate.Year && or.Inserted_Date.Month == plannedDate.Month && or.Inserted_Date.Day == plannedDate.Day)
                    //orderSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false && or.Planned_Date == plannedDate && or.Order_Type == orderType)
                    orderPatternSequenceObj = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
                                               orderby or.RSN ascending
                                               select new CummulativeFields()
                                               {
                                                   Row_ID = or.Row_ID,
                                                   Order_No = or.Order_No,
                                                   Model_Code = or.partno.Trim(),
                                                   //Series = or.RS_Series.Series_Description,
                                                   Color_code = or.Model_Color,
                                                   Inserted_Date = or.Inserted_Date,
                                                   remarks = or.Remarks,
                                                   orderType = or.Order_Type,
                                                   Planned_Shift_ID = or.Planned_Shift_ID
                                                   //EngineModelCode = db.RS_OM_OrderRelease.Where(a => a.Model_Code == or.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 1).FirstOrDefault().partno,
                                                   //TransmissionSeries = db.RS_OM_OrderRelease.Where(a => a.Model_Code == or.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 2).FirstOrDefault().RS_Series.Series_Description
                                               }).ToList();
                }

            }
            catch (Exception)
            {

                throw;
            }
            return orderPatternSequenceObj.OrderBy(m => m.RSN).ToList();
        }
        public List<CummulativeFields> GetOrderDetailGroupByOrderType(int plantID, int shopID, int lineID, DateTime plannedDate)
        {
            List<CummulativeFields> orderSequence = new List<CummulativeFields>();


            List<CummulativeFields> plannedStartedOrdersDataObj = new List<CummulativeFields>();
            try
            {
                orderSequence = (from or in db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && DbFunctions.TruncateTime(or.Planned_Date) == DbFunctions.TruncateTime(plannedDate))
                                 orderby or.RSN ascending
                                 select new CummulativeFields()
                                 {
                                     Row_ID = or.Row_ID,
                                     Order_No = or.Order_No,
                                     Model_Code = or.partno.Trim(),
                                     Color_code = or.Model_Color,
                                     orderType = or.Order_Type,
                                     Order_Status = or.Order_Status,
                                     //  remarks = or.Remarks
                                 }).ToList();

                int hold = 0, started = 0, trdQty = 0, trlQty = 0, regQty = 0, spQty = 0;
                foreach (var item in orderSequence)
                {
                    var orderLineId = (from pf in db.RS_OM_Platform
                                       join mm in db.RS_Model_Master.Where(m => m.Model_Code == item.Model_Code)
                                       on pf.Platform_ID equals mm.Platform_Id
                                       where pf.Plant_ID == plantID
                                       select new
                                       {
                                           pf.Line_ID
                                       }).FirstOrDefault();

                    int lineIDs = Convert.ToInt32(orderLineId.Line_ID);

                    if (lineIDs == lineID)
                    {
                        var res = plannedStartedOrdersDataObj.Where(c => c.Model_Code == item.Model_Code && c.orderType == item.orderType).FirstOrDefault();

                        if (res == null)
                        {
                            // item.CummlQty = item.PlannedQty;
                            item.PlannedQty = 1;
                            if (item.Order_Status == "Started")
                            {
                                item.StartedQty = 1;
                            }
                            else if (item.Order_Status == "Hold")
                            {
                                item.HoldQty = 1;
                            }
                            plannedStartedOrdersDataObj.Add(item);

                        }
                        else
                        {

                            if (item.Order_Status == "Started")
                            {
                                started = 1;
                            }
                            else if (item.Order_Status == "Hold")
                            {
                                hold = 1;
                            }

                            if (item.orderType == "Regular")
                            {
                                res.PlannedQty++;
                                res.StartedQty += started;
                                res.HoldQty += hold;
                                started = 0;
                                hold = 0;

                            }
                            else if (item.orderType == "Trail")
                            {
                                res.PlannedQty++;
                                res.StartedQty += started;
                                res.HoldQty += hold;
                                started = 0;
                                hold = 0;
                            }
                            else if (item.orderType == "Tear Down")
                            {
                                res.PlannedQty++;
                                res.StartedQty += started;
                                res.HoldQty += hold;
                                started = 0;
                                hold = 0;
                            }
                            else if (item.orderType == "Spare")
                            {
                                res.PlannedQty++;
                                res.StartedQty += started;
                                res.HoldQty += hold;
                                started = 0;
                                hold = 0;
                            }

                            //res.CummlQty = res.PlannedQty;
                        }
                        //counter = item.CummlQty;
                    }
                }
                int? cummty = 0;// plannedStartedOrdersDataObj[0].PlannedQty;

                foreach (var row in plannedStartedOrdersDataObj)
                {
                    row.CummlQty = 0;
                    row.CummlQty += row.PlannedQty + cummty;
                    cummty = row.CummlQty;
                }
            }

            catch (Exception)
            {

                throw;
            }
            return plannedStartedOrdersDataObj.OrderBy(m => m.RSN).ToList();
        }

        /*               Action Name               : ExportToPdf
         *               Description               : Action used to create Tact sheet pdf
         *               Author, Timestamp         : Ketan Dhanuka
         *               Input parameter           : datatable
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public string ExportToPdf(DataSet ds, string platformName, string shiftName, int tact_time)
        {
            TimeSpan Tact_Time = TimeSpan.FromSeconds(tact_time);
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string fileName = platformName + "_TactSheet_" + dTime.ToString("ddMMyyyy") + "_" + dTime.Hour + "_" + dTime.Minute + ".pdf";
            Session["TackSheetFileName"] = fileName;
            //file name to be created   
            //string strPDFFileName = string.Format("Tact_Sheet_" + dTime.ToString("ddMMyyyy") + "_"+ dTime.TimeOfDay+ "" + ".pdf");
            string strPDFFileName = string.Format(fileName);

            iTextSharp.text.Document doc = new iTextSharp.text.Document();
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10);
            iTextSharp.text.Font font4 = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
            //font5.Color = GrayColor.BLUE;
            doc.SetMargins(50f, 40f, 70f, 30f);
            ////Create PDF Table with 5 columns  
            //PdfPTable tableLayout = new PdfPTable(5);
            //doc.SetMargins(0f, 0f, 0f, 0f);

            string strAttachment = Server.MapPath("~/App_Data/" + strPDFFileName);

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(strAttachment, FileMode.Create));
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            string imageURL = Server.MapPath("~/Content/images/ReinSolutionsLogo.jpeg");
            string ImageURL = Server.MapPath("~/Content/images/ReinSolutionsLogo.jpeg");
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
            iTextSharp.text.Image dJpg = iTextSharp.text.Image.GetInstance(ImageURL);
            //Resize image depend upon your need
            jpg.ScaleToFit(140f, 120f);
            //Give space before image
            jpg.SpacingBefore = 60f;
            //Give some space after the image
            jpg.SpacingAfter = 40;
            jpg.Alignment = Element.ALIGN_LEFT;
            //jpg.Alignment = Image.TEXTWRAP;
            //jpg.Alignment = Image.UNDERLYING;

            doc.Add(jpg);


            //Resize image depend upon your need
            dJpg.ScaleToFit(140f, 120f);
            //Give space before image
            dJpg.SpacingBefore = 30f;
            //Give some space after the image
            dJpg.SpacingAfter = 40;
            //dJpg.Alignment = Element.ALIGN_RIGHT;
            //dJpg.Alignment = Image.UNDERLYING;
            dJpg.SetAbsolutePosition(PageSize.A4.Width - 180f, PageSize.A4.Height - dJpg.Height);
            //jpg.Alignment = Element.ALIGN_RIGHT;




            doc.Add(dJpg);

            iTextSharp.text.Font tactsheetfont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_ROMAN, 20, Font.BOLDITALIC);
            Phrase cell = new Phrase(platformName + " TactSheet                                                   " + shiftName + "\n", tactsheetfont);

            //        table.AddCell(new PdfPCell(new Phrase("TactSheet", new Font(Font.FontFamily.HELVETICA, 12, 1, iTextSharp.text.BaseColor.BLACK)))
            //{
            //            HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0),
            //           // Border = bor
            //});

            doc.Add(cell);

            iTextSharp.text.Font ShiftName = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.BOLD);
            Phrase celldate = new Phrase("Date: " + DateTime.Today.ToString("dd-MM-yyyy") + "                                                                                                   " + "Tact time : " + Tact_Time + "\n", ShiftName);
            doc.Add(celldate);

            //Phrase cell5 = new Phrase("mukesh\n", tactsheetfont);
            //doc.Add(cell5);
            int shfitcnt = 1;
            if (ds.Tables.Count > 0)
            {
                for (int j = 0; j < ds.Tables.Count; j++)
                {
                    //iTextSharp.text.Font ShiftName = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12, Font.BOLD);
                    //Phrase cell1 = new Phrase(shiftName, ShiftName);
                    //Phrase cell1 = new Phrase("Shift " + shfitcnt++, ShiftName);
                    //doc.Add(cell1);

                    PdfPTable table = new PdfPTable(ds.Tables[j].Columns.Count);
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    PdfPRow row = null;
                    //float[] widths = new float[] { 1f, 2.5f, 4f, 2f,2f, 2f, 3f, 3f };
                    float[] widths = new float[] { 1f, 2.5f, 4f,2f, 2f, 3f, 3f };

                    table.SetWidths(widths);
                    table.SpacingBefore = 20;
                    table.WidthPercentage = 100;

                    int i = 0;

                    foreach (DataColumn c in ds.Tables[j].Columns)
                    {
                        if (i == 0)
                        {
                            table.AddCell(new Phrase(c.ColumnName, font4));
                        }
                        else
                        {
                            table.AddCell(new Phrase(c.ColumnName, font5));
                        }

                    }

                    foreach (DataRow r in ds.Tables[j].Rows)
                    {
                        if (ds.Tables[j].Rows.Count > 0)
                        {
                            table.AddCell(new Phrase(r[0].ToString(), font5));
                            table.AddCell(new Phrase(r[1].ToString(), font5));
                            table.AddCell(new Phrase(r[2].ToString(), font5));
                            table.AddCell(new Phrase(r[3].ToString(), font5));
                            table.AddCell(new Phrase(r[4].ToString(), font5));
                            table.AddCell(new Phrase(r[5].ToString(), font5));
                            table.AddCell(new Phrase(r[6].ToString(), font5));
                        }
                    }
                    doc.Add(table);



                    //byte[] byteInfo = workStream.ToArray();
                    //workStream.Write(byteInfo, 0, byteInfo.Length);
                    //workStream.Position = 0;
                }
            }
            doc.Close();
            //Dynamic creating pdf file




            //Create PDF Table  

            //file will created in this path  

            //cell.Colspan = dt.Columns.Count;

            //table.AddCell(cell);


            return strAttachment;
        }

        /*               Action Name               : DownloadPdf
         *               Description               : Action used to download  Tact sheet pdf
         *               Author, Timestamp         : Ketan Dhanuka
         *               Input parameter           : datatable
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        public FileResult DownloadPdf(string length, string fileName)
        {

            return File(fileName, "application/pdf");


        }

        /*               Action Name               : ValidProductionCountAsPerPlannedDate
        *               Description               : Action used to validat against production to actual order created.
        *               Author, Timestamp         : Ketan Dhanuka
        *               Input parameter           : plant,shop,Line,Planned Date
        *               Return Type               : ActionResult
        *               Revision                  : 1
        */
        public ActionResult ValidProductionCountAsPerPlannedDate(decimal plant_ID, decimal Shop_ID, string PlannedDate, int? prodCount, int? lineID)
        {
            JSONData objJSONData = new JSONData();
            //DateTime date = DateTime.ParseExact(PlannedDate.ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            IFormatProvider culture = new CultureInfo("en-US", true);
            //DateTime dateVal = DateTime.ParseExact(PlannedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dateVal = DateTime.ParseExact(PlannedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                if (prodCount > 0)
                {
                    var count = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plant_ID && m.Shop_ID == Shop_ID && m.Line_ID == lineID && DbFunctions.TruncateTime(m.Planned_Date) == dateVal).Count();
                    var t = db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plant_ID && m.Shop_ID == Shop_ID && m.Line_ID == lineID && m.Planned_Date == dateVal).Count();
                    if (prodCount <= db.RS_OM_OrderRelease.Where(m => m.Plant_ID == plant_ID && m.Shop_ID == Shop_ID && m.Line_ID == lineID && m.Planned_Date == dateVal).Count())
                    {
                        objJSONData.status = true;
                        objJSONData.message = "Production count matches as per planned date";
                        objJSONData.type = "success";
                        TempData["validProdResult"] = true;
                    }
                    else
                    {
                        objJSONData.status = false;
                        objJSONData.message = "Production count does not matches as per planned date... You have to create Order!...or Chnage Prduction Count";
                        objJSONData.type = "fail";
                        TempData["validProdResult"] = false;
                    }
                }

                return Json(objJSONData, JsonRequestBehavior.AllowGet);
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
                        generalHelper.addControllerException(dbex, "OMOrderPlanningController", "GenerateTactSheet(Post) " + validationError, ((FDSession)this.Session["FDSession"]).userId);
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
                objJSONData.status = false;
                objJSONData.message = ex.Message;
                objJSONData.type = "fail";
                TempData["validProdResult"] = false;
                generalHelper.addControllerException(ex, "OMOrderPlanningController", "GenerateTactSheet(Post)", ((FDSession)this.Session["FDSession"]).userId);
                return Json(objJSONData, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult GetPlannedDateByPlatform(decimal platformId = 0, decimal shopID = 0)
        {
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var lineID = GetLineIdByPlatform(platformId);
            DateTime time = DateTime.Now.Date;

            var plannedDateList = db.RS_OM_Creation
                .Where(c => c.Planned_Date >= time && c.Plant_ID == plantID && c.Line_ID == lineID && c.Shop_ID == shopID).Select(c => c.Planned_Date).Distinct().ToList();

            List<String> datelist = new List<string>();
            foreach (var d in plannedDateList)
            {
                DateTime ds = Convert.ToDateTime(d);
                //datelist.Add(ds.ToShortDateString());
                string format = "yyyy-MM-dd";
                datelist.Add(ds.ToString(format));
            }
            var PlannedDate = datelist.ToList();
            return Json(PlannedDate, JsonRequestBehavior.AllowGet);
        }
        public decimal? GetLineIdByPlatform(decimal? platfromId = 0)
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;

            var lineId = db.RS_OM_Platform.Where(m => m.Platform_ID == platfromId && m.Plant_ID == plantId).Select(m => m.Line_ID).FirstOrDefault();
            return lineId;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult MainLinePattern()
        {
            int plantid = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.OM_Planning;
            globalData.subTitle = ResourceGlobal.MainLinePattern;
            globalData.controllerName = "OM_OrderPlanning";
            globalData.actionName = ResourceGlobal.MainLinePattern;
            globalData.contentTitle = ResourceGlobal.MainLinePattern;
            globalData.contentFooter = ResourceGlobal.MainLinePattern;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            ViewBag.GlobalDataModel = globalData;


            ViewBag.order_type = new SelectList(db.RS_OM_Order_Type.Where(m => m.Plant_ID == plantid), "Order_Type_Name", "Order_Type_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantid), "Plant_ID", "Plant_Name");


            ViewBag.pst = "";




            ViewBag.CUMNDATA = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantid), "Plant_ID", "Plant_Name");
            ViewBag.Shops = db.RS_Shops.Where(a => a.Plant_ID == plantid);

            var pfname = (db.RS_OM_Platform.Where(m => m.Plant_ID == plantid).Select(m => m.Platform_ID)).ToList();
            var ordPatt = db.RS_OM_Order_Pattern.Where(m => m.Plant_ID == plantid).OrderByDescending(m => m.Row_ID).GroupBy(x => x.Platform_ID).Select(x => x.FirstOrDefault()).ToList();

            DateTime lastDate = new DateTime(0001, 01, 01);
            if (db.RS_OM_Order_Pattern.Count() > 0)
            {
                lastDate = db.RS_OM_Order_Pattern.Max(m => m.Inserted_Date);
            }
            var Platform = (from pf in db.RS_OM_Order_Pattern

                            join orderPattern in db.RS_OM_Platform on pf.Platform_ID equals orderPattern.Platform_ID
                            where orderPattern.Plant_ID == plantid && pf.Inserted_Date.Year == lastDate.Year &&
                                pf.Inserted_Date.Month == lastDate.Month && pf.Inserted_Date.Day == lastDate.Day &&
                                pf.Inserted_Date.Hour == lastDate.Hour && pf.Inserted_Date.Hour == lastDate.Hour &&
                                pf.Inserted_Date.Minute == lastDate.Minute && pf.Inserted_Date.Second == lastDate.Second
                            orderby pf.Planned_Date descending
                            select new OrderPattern()
                            {
                                Ratio = pf.Ratio,
                                Platform_ID = orderPattern.Platform_ID,
                                Platform_Name = orderPattern.Platform_Name,
                                Priority = (int)pf.Priority,
                                Planned_Date = pf.Planned_Date
                            }).ToList();


            if (Platform.Count == 0)
            {

                //Platform = (from pf in db.RS_OM_Platform
                //            join orderPattern in db.RS_OM_Platform on pf.Platform_ID equals orderPattern.Platform_ID
                //           into opf
                //            from x in opf.DefaultIfEmpty()
                //            where x.Plant_ID == plantid && pf.Inserted_Date.Year == lastDate.Year &&
                //                pf.Inserted_Date.Month == lastDate.Month && pf.Inserted_Date.Day == lastDate.Day &&
                //                pf.Inserted_Date.Hour == lastDate.Hour && pf.Inserted_Date.Hour == lastDate.Hour &&
                //                pf.Inserted_Date.Minute == lastDate.Minute && pf.Inserted_Date.Second == lastDate.Second
                //            orderby pf.Planned_Date descending
                //            select new OrderPattern()
                //            {
                //                Ratio = (pf == null ? 0 : pf.Ratio),
                //                Priority = (pf == null ? 0 : (int)pf.Priority),
                //                Platform_ID = pf.Platform_ID,
                //                Planned_Date = pf.Planned_Date,
                //                Platform_Name = x.Platform_Name

                //                //Ratio = pf.Ratio,
                //                //Platform_ID = orderPattern.Platform_ID,
                //                //Platform_Name = orderPattern.Platform_Name,
                //                //Priority = (int)pf.Priority,
                //                //Planned_Date = pf.Planned_Date
                //            }).ToList();


                /////-----------


                Platform = (from pf in db.RS_OM_Platform
                            join op in db.RS_OM_Order_Pattern on pf.Platform_ID equals op.Platform_ID
                            into opf
                            from x in opf.DefaultIfEmpty()
                            where pf.Plant_ID == plantid
                            select new OrderPattern()
                            {
                                Ratio = (x == null ? 0 : x.Ratio),
                                Priority = (x == null ? 0 : (int)x.Priority),
                                Platform_ID = pf.Platform_ID,
                                Planned_Date = pf.Inserted_Date,
                                Platform_Name = pf.Platform_Name
                            }).OrderByDescending(m => m.Planned_Date).ToList();
                /////-----------


                //Platform = (from pf in db.RS_OM_Platform
                //            join op in db.RS_OM_Order_Pattern on pf.Platform_ID equals op.Platform_ID
                //            into opf
                //            from x in opf.DefaultIfEmpty()
                //            where pf.Plant_ID == plantid
                //            select new OrderPattern()
                //            {
                //                Ratio = (x == null ? 0 : x.Ratio),
                //                Priority = (x == null ? 0 : (int)x.Priority),
                //                Platform_ID = pf.Platform_ID,
                //                Planned_Date = pf.Inserted_Date,
                //                Platform_Name = pf.Platform_Name
                //            }).OrderByDescending(m => m.Planned_Date).Take(3).ToList();
            }
            ViewBag.Platform = Platform;
            ViewBag.Shift = new SelectList(db.RS_Shift.Where(m => m.Plant_ID == plantid), "Shift_ID", "Shift_Name");

            return View();
        }


    }
}