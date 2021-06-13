using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mahindra_FD.App_LocalResources;
using Mahindra_FD.Helper;
using Mahindra_FD.Models;
using Mahindra_FD.Controllers.BaseManagement;

namespace Mahindra_FD.Controllers.ANDONManagement
{
    public class PPCDailyPlanANDONController : BaseController
    {
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        MVML_MGMTEntities db = new MVML_MGMTEntities();
        
        public ActionResult TractorShop()
        {
            return View();
        }
        public ActionResult EngineShop()
        {
            return View();
        }
        public ActionResult VTUShop()
        {
            return View();
        }
        public ActionResult TransmissionShop()
        {
            return View();
        }

        public ActionResult loadOrderSequence(int ShopId, int LineId)
        {
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.ToList(), "Shop_ID", "Shop_Name");
            int plantID = ((FDSession)this.Session["FDSession"]).plantId, lineID = 0, shopID, blockAfterQty;

            DateTime nowTime = DateTime.Now;
            shopID = ShopId;
            string orderType;
            lineID = LineId;
            if (ShopId == 1)
            {
                orderType = "P";
            }
            else
            {
                orderType = "S";
            }

            try
            {
                blockAfterQty = 0;


                string shopName = db.MM_Shops.Find(shopID).Shop_Name;

                List<CummulativeFields> orderSequenceObj = new List<CummulativeFields>();
                if (shopName.Contains("Tractor"))
                {
                    orderSequenceObj = (from or in db.MM_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
                                        orderby or.RSN ascending
                                        select new CummulativeFields()
                                        {
                                            Row_ID = or.Row_ID,
                                            Order_No = or.Order_No,
                                            Model_Code = or.partno.Trim(),
                                            Series = or.MM_Series.Series_Description,
                                            Inserted_Date = or.Inserted_Date,
                                            remarks = or.Remarks,
                                            orderType = or.Order_Type,
                                            EngineModelCode = db.MM_OM_OrderRelease.Where(a => a.Model_Code == or.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 1).FirstOrDefault().partno,
                                            TransmissionSeries = db.MM_OM_OrderRelease.Where(a => a.Model_Code == or.Model_Code && a.partno != a.Model_Code && a.Shop_ID == 2).FirstOrDefault().MM_Series.Series_Description
                                        }).ToList();
                    foreach (CummulativeFields cummlObj in orderSequenceObj)
                    {
                      
                        string tokenId = cummlObj.UToken;
                     
                        cummlObj.Model_Code = cummlObj.Model_Code.Trim();
                     
                    }
                    ViewBag.SequenceShop = "Tractor";
                }
                else if (shopName.Contains("Hydraulic"))
                {
                    orderSequenceObj = (from or in db.MM_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
                                        orderby or.RSN ascending
                                        select new CummulativeFields()
                                        {
                                            Row_ID = or.Row_ID,
                                            Order_No = or.Order_No,
                                            Model_Code = or.partno,
                                            Series = or.MM_Series.Series_Description,
                                            parentModel_Code = or.Model_Code,
                                            Inserted_Date = or.Inserted_Date,
                                            remarks = or.Remarks,
                                            orderType = or.Order_Type,
                                            UToken = or.UToken
                                        }).ToList();
                    foreach (CummulativeFields cummlObj in orderSequenceObj)
                    {
                       
                        string tokenId = cummlObj.UToken;
                     
                        MM_OM_OrderRelease mmTransModelObj = db.MM_OM_OrderRelease.Where(a => a.Model_Code == cummlObj.parentModel_Code && a.Model_Code != a.partno && a.Shop_ID == 2 && a.UToken == tokenId).FirstOrDefault();
                        string transModel = "";
                        string transSeries = "";
                        if (mmTransModelObj != null)
                        {
                            transModel = mmTransModelObj.partno;
                            transSeries = mmTransModelObj.MM_Series.Series_Description;
                        }
                        cummlObj.Model_Code = cummlObj.Model_Code.Trim();
                        cummlObj.parentModel_Code = transModel;
                        cummlObj.parentSeries = transSeries;
                    }
                    ViewBag.SequenceShop = "Hydraulic";
                }
                else
                {
                    orderSequenceObj = (from or in db.MM_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release" && or.Order_Start == false)
                                        orderby or.RSN ascending
                                        select new CummulativeFields()
                                        {
                                            Row_ID = or.Row_ID,
                                            Order_No = or.Order_No,
                                            Model_Code = or.partno.Trim(),
                                            Series = or.MM_Series.Series_Description,
                                            parentModel_Code = or.Model_Code,
                                            parentSeries = db.MM_Model_Master.Where(a => a.Model_Code == or.Model_Code).FirstOrDefault().MM_Series == null ? "" : db.MM_Model_Master.Where(a => a.Model_Code == or.Model_Code).FirstOrDefault().MM_Series.Series_Description,
                                            Inserted_Date = or.Inserted_Date,
                                            remarks = or.Remarks,
                                            orderType = or.Order_Type

                                        }).ToList();
                    ViewBag.SequenceShop = "Others";
                }

                //DateTime today = Convert.ToDateTime("2016-05-21").Date;
                DateTime today = DateTime.Today;
                List<CummulativeFields> orderStartedObj = new List<CummulativeFields>(); ;
                string chkModel_Code = "";
                string chkSeries = "";
                string chkParentModel = "";
                string chkParentSeries = "";
                int startedQty = 0;
                int cummlQty = 0;
                int releasedQty = 0;
                int holdedQty = 0;

                List<CummulativeFields> plannedOrdersDataObj = new List<CummulativeFields>();
                if (shopName.Contains("Tractor"))
                {
                    MM_OM_OrderRelease mmEngModelObj = new MM_OM_OrderRelease();
                    MM_OM_OrderRelease mmTransModelObj = new MM_OM_OrderRelease();
                    orderStartedObj = db.MM_TractStartedOrders_Cumml_View.OrderBy(a => a.Cumul)
                                                            .Select(a => new CummulativeFields()
                                                            {
                                                                Model_Code = a.Model_Code,
                                                                Series = a.Series_Description,
                                                                Order_Status = a.Order_Status,
                                                                Qty = a.Cnt,
                                                                CummlQty = a.Cumul,
                                                                remarks = a.Remarks
                                                            }).ToList();
                    foreach (CummulativeFields cumFieldsObj in orderStartedObj)
                    {

                        if (!chkModel_Code.Equals(cumFieldsObj.Model_Code, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (!String.IsNullOrWhiteSpace(chkModel_Code))
                            {
                                CummulativeFields CumlObj = new CummulativeFields();

                                CumlObj.Model_Code = chkModel_Code;
                                CumlObj.Series = chkSeries;
                                CumlObj.StartedQty = startedQty;
                                CumlObj.HoldQty = holdedQty;
                                CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
                                CumlObj.EngineModelCode = chkParentModel;
                                CumlObj.TransmissionSeries = chkParentSeries;
                                CumlObj.CummlQty = cummlQty;

                                mmEngModelObj = db.MM_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 1 && a.UToken == CumlObj.UToken).FirstOrDefault();
                                mmTransModelObj = db.MM_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

                                if (mmEngModelObj != null)
                                {
                                    CumlObj.EngineModelCode = mmEngModelObj.partno;
                                }
                                if (mmTransModelObj != null)
                                {
                                    CumlObj.TransmissionSeries = mmTransModelObj.MM_Series.Series_Description;
                                }

                                plannedOrdersDataObj.Add(CumlObj);
                                startedQty = 0;
                                holdedQty = 0;
                                releasedQty = 0;
                            }

                            switch (cumFieldsObj.Order_Status)
                            {
                                case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                                case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                                default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                            }
                            chkModel_Code = cumFieldsObj.Model_Code;
                            chkSeries = cumFieldsObj.Series;
                            chkParentModel = cumFieldsObj.EngineModelCode;
                            chkParentSeries = cumFieldsObj.TransmissionSeries;

                            cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);

                        }
                        else
                        {
                            switch (cumFieldsObj.Order_Status)
                            {
                                case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                                case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                                default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                            }
                            cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
                        }
                        if (cummlQty >= orderStartedObj.Last().CummlQty)
                        {
                            CummulativeFields CumlObj = new CummulativeFields();

                            CumlObj.Model_Code = chkModel_Code;
                            CumlObj.Series = chkSeries;
                            CumlObj.StartedQty = startedQty;
                            CumlObj.HoldQty = holdedQty;
                            CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
                            CumlObj.EngineModelCode = chkParentModel;
                            CumlObj.TransmissionSeries = chkParentSeries;
                            CumlObj.CummlQty = cummlQty;

                            mmEngModelObj = db.MM_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 1).FirstOrDefault();
                            mmTransModelObj = db.MM_OM_OrderRelease.Where(a => a.Model_Code == chkModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2).FirstOrDefault();

                            if (mmEngModelObj != null)
                            {
                                CumlObj.EngineModelCode = mmEngModelObj.partno;
                            }
                            if (mmTransModelObj != null)
                            {
                                CumlObj.TransmissionSeries = mmTransModelObj.MM_Series.Series_Description;
                            }
                            plannedOrdersDataObj.Add(CumlObj);
                        }

                    }
                }
                else
                {
                    if (shopName.Contains("Hydraulic"))
                    {
                        orderStartedObj = db.MM_HydraStartedOrders_Cumml_View.OrderBy(a => a.Cumul)
                                                             .Select(a => new CummulativeFields()
                                                             {
                                                                 Model_Code = a.Model_Code,
                                                                 Series = a.Child_Series,
                                                                 parentModel_Code = a.Parent_Model,
                                                                 parentSeries = a.Parent_Series,
                                                                 Order_Status = a.Order_Status,
                                                                 Qty = a.Cnt,
                                                                 CummlQty = a.Cumul,
                                                                 remarks = a.Remarks
                                                             }).ToList();

                    }
                    else if (shopName.Contains("Engine"))
                    {
                        orderStartedObj = db.MM_EngStartedOrders_Cumml_View.OrderBy(a => a.Cumul)
                                                            .Select(a => new CummulativeFields()
                                                            {
                                                                Model_Code = a.Model_Code,
                                                                Series = a.Child_Series,
                                                                parentModel_Code = a.Parent_Model,
                                                                parentSeries = a.Parent_Series,
                                                                Order_Status = a.Order_Status,
                                                                Qty = a.Cnt,
                                                                CummlQty = a.Cumul,
                                                                remarks = a.Remarks
                                                            }).ToList();
                    }
                    else
                    {
                        orderStartedObj = db.MM_TransStartedOrders_Cumml_View.OrderBy(a => a.Cumul)
                                                            .Select(a => new CummulativeFields()
                                                            {
                                                                Model_Code = a.Model_Code,
                                                                Series = a.Child_Series,
                                                                parentModel_Code = a.Parent_Model,
                                                                parentSeries = a.Parent_Series,
                                                                Order_Status = a.Order_Status,
                                                                Qty = a.Cnt,
                                                                CummlQty = a.Cumul,
                                                                remarks = a.Remarks
                                                            }).ToList();
                    }
                    List<string> remarksList = new List<string>();
                    foreach (CummulativeFields cumFieldsObj in orderStartedObj)
                    {
                        if (!chkModel_Code.Equals(cumFieldsObj.Model_Code, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (!String.IsNullOrWhiteSpace(chkModel_Code))
                            {
                                CummulativeFields CumlObj = new CummulativeFields();

                                CumlObj.Model_Code = chkModel_Code;
                                CumlObj.Series = chkSeries;
                                CumlObj.StartedQty = startedQty;
                                CumlObj.HoldQty = holdedQty;
                                CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
                                CumlObj.parentModel_Code = chkParentModel;
                                CumlObj.parentSeries = chkParentSeries;
                                CumlObj.CummlQty = cummlQty;
                                CumlObj.remarks = String.Join(",", remarksList.ToArray());
                                if (shopName.Contains("Hydraulic"))
                                {
                                    MM_OM_OrderRelease mmTransModelObj = db.MM_OM_OrderRelease.Where(a => a.Model_Code == chkParentModel && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

                                    if (mmTransModelObj != null)
                                    {
                                        CumlObj.parentModel_Code = mmTransModelObj.partno;
                                        CumlObj.parentSeries = mmTransModelObj.MM_Series.Series_Description;
                                    }
                                }

                                plannedOrdersDataObj.Add(CumlObj);
                                startedQty = 0;
                                holdedQty = 0;
                                releasedQty = 0;
                                remarksList.Clear();
                            }
                            switch (cumFieldsObj.Order_Status)
                            {
                                case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                                case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                                default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                            }
                            chkModel_Code = cumFieldsObj.Model_Code;
                            chkSeries = cumFieldsObj.Series;
                            chkParentModel = cumFieldsObj.parentModel_Code;
                            chkParentSeries = cumFieldsObj.parentSeries;
                            cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
                            if (cumFieldsObj.remarks != null)
                            {
                                if (remarksList.Contains(cumFieldsObj.remarks) == false)
                                {
                                    remarksList.Add(cumFieldsObj.remarks.Trim());
                                }
                            }

                        }
                        else
                        {
                            if (remarksList.Contains(cumFieldsObj.remarks) == false)
                            {
                                remarksList.Add(cumFieldsObj.remarks.Trim());
                            }
                            switch (cumFieldsObj.Order_Status)
                            {
                                case "Started": startedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                                case "Hold": holdedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                                default: releasedQty += cumFieldsObj.Qty.GetValueOrDefault(0); break;
                            }
                            cummlQty = cumFieldsObj.CummlQty.GetValueOrDefault(0);
                        }
                        if (cummlQty >= orderStartedObj.Last().CummlQty)
                        {
                            if (cumFieldsObj.remarks != null)
                            {
                                if (remarksList.Contains(cumFieldsObj.remarks) == false)
                                {
                                    remarksList.Add(cumFieldsObj.remarks.Trim());
                                }
                            }
                            CummulativeFields CumlObj = new CummulativeFields();

                            CumlObj.Model_Code = chkModel_Code;
                            CumlObj.Series = chkSeries;
                            CumlObj.StartedQty = startedQty;
                            CumlObj.HoldQty = holdedQty;
                            CumlObj.PlannedQty = (releasedQty + holdedQty + startedQty);
                            CumlObj.parentModel_Code = chkParentModel;
                            CumlObj.parentSeries = chkParentSeries;
                            CumlObj.CummlQty = cummlQty;
                            CumlObj.remarks = String.Join(",", remarksList.ToArray());
                            if (shopName.Contains("Hydraulic"))
                            {
                                MM_OM_OrderRelease mmTransModelObj = db.MM_OM_OrderRelease.Where(a => a.Model_Code == cumFieldsObj.parentModel_Code && a.partno != a.Model_Code && a.Shop_ID == 2 && a.UToken == CumlObj.UToken).FirstOrDefault();

                                if (mmTransModelObj != null)
                                {
                                    CumlObj.parentModel_Code = mmTransModelObj.partno;
                                    CumlObj.parentSeries = mmTransModelObj.MM_Series.Series_Description;
                                }
                            }
                            remarksList.Clear();
                            plannedOrdersDataObj.Add(CumlObj);
                        }
                    }

                }

                if (blockAfterQty > 0)
                {
                    db.MM_OM_OrderRelease.Where(a => a.Line_ID == lineID && a.Order_Status == "Release")
                                    .OrderBy(a => a.RSN).Skip(blockAfterQty).ToList()
                                    .ForEach(a => a.Is_Blocked = true);
                    db.SaveChanges();
                }

               
                ViewBag.startedOrders = plannedOrdersDataObj;
                ViewBag.blockAfterQty = blockAfterQty;

                return PartialView("PPCOrderSequence", orderSequenceObj);
            }
            catch (Exception exp)
            {
            }
            finally
            {
               
            }
            return null;
        }
    }
}