using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaOrderCreation))]
    public partial class RS_OM_Creation
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        //public string Series_Code { get; set; }
        public int Started_Qty { get; set; }
        public int Cumulative_Count { get; set; }

        public bool IsOrderStarted(int ID)
        {
            try
            {
                bool result = db.RS_OM_OrderRelease.Any(p => p.Plant_OrderNo == ID && p.Order_Status == "Started");
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //added by mukesh
        public bool IsOrderHold(int ID)
        {
            try
            {
                bool result = db.RS_OM_OrderRelease.Any(p => p.Plant_OrderNo == ID && p.Order_Status == "Hold");
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //*
        /*               Action Name               : GetLastOrderNumber
         *               Description               : Function used to find Last Order Number/ Maximum number for order creation. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : PlantId,shopId
         *               Return Type               : String
         *               Revision                  : 1
        */
        public Decimal GetLastOrderNumber(int plantId, int shopId)
        {
            try
            {
                decimal serialNumber;
                var item = db.RS_OM_Creation.Where(c => c.Plant_ID == plantId).Select(c => new { c.Plant_OrderNo }).ToList();

                if (item.Count > 0)
                {
                    var max_orderno = db.RS_OM_Creation.Max(r => r.Plant_OrderNo);
                    string s = max_orderno.ToString();
                    int number = Convert.ToInt32(s);
                    number += 1;
                    //serialNumber = number.ToString("D5");                    
                    serialNumber = number;
                }
                else
                {
                    //string s = "00001";
                    decimal s = 1;
                    int number = Convert.ToInt32(s);
                    //string str = number.ToString("D5");
                    decimal str = number;
                    serialNumber = str;
                }

                return serialNumber;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        /*               Action Name               : GetValidateOrderNumber
         *               Description               : Function used to find dublicate Part Number validate  for order creation. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : PlantId,shopId,partNo
         *               Return Type               : String
         *               Revision                  : 1
        */
        public Boolean GetValidateOrderNumber(int plantId, int shopId, string partno, int lineId)
        {
            try
            {
                IQueryable<RS_OM_Creation> result;
                if (lineId == 0)
                {
                    result = db.RS_OM_Creation.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Model_Code == partno);
                }
                else
                {
                    result = db.RS_OM_Creation.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Model_Code == partno);
                }

                if (result.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*               Action Name               : checkQuantity
         *               Description               : Function used to find actual qauntity order creation. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : PlantId,shopId,partNo
         *               Return Type               : String
         *               Revision                  : 1
         */
        public Boolean checkQuantity(int plantId, int shopId, string partno, int lineId)
        {
            try
            {
                int cnt = 0;

                IQueryable<RS_OM_Creation> result;
                int qty = 0;
                int rel_qty = 0;
                if (lineId == 0)
                {
                    //result = db.RS_OM_Creation.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Model_Code == partno);

                    RS_OM_Creation result1 = (from ocreation in db.RS_OM_Creation
                                              where ocreation.Shop_ID == shopId && ocreation.Plant_ID == plantId && ocreation.Model_Code == partno
                                              select ocreation).Take(1).FirstOrDefault();

                    qty = Convert.ToInt16(result1.Qty);
                    rel_qty = Convert.ToInt16(result1.Release_Qty);

                    if (rel_qty > qty)
                    {
                        cnt = 1;
                    }
                    else
                    {
                        cnt = 0;
                    }

                }
                else
                {
                    //result = db.RS_OM_Creation.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Model_Code == partno);
                    RS_OM_Creation result1 = (from ocreation in db.RS_OM_Creation
                                              where ocreation.Shop_ID == shopId && ocreation.Plant_ID == plantId && ocreation.Model_Code == partno
                                              select ocreation).Take(1).FirstOrDefault();

                    qty = Convert.ToInt16(result1.Qty);
                    rel_qty = Convert.ToInt16(result1.Release_Qty);

                    if (rel_qty > qty)
                    {
                        cnt = 1;
                    }
                    else
                    {
                        cnt = 0;
                    }

                }

                if (cnt > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            { return false; }

        }

        public bool isModelCodeAvailableInShop(int shopId, String modelCode)
        {
            try
            {
                var res = from modelMasterObj in db.RS_Model_Master
                          where modelMasterObj.Shop_ID == shopId && modelMasterObj.Model_Code == modelCode
                          select modelMasterObj;
                if (res.Count() > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool isSeriesAddedForModel(int shopId, String modelCode, String series)
        {
            try
            {
                decimal qu = (from seriesObj in db.RS_Series where seriesObj.Series_Description == series select seriesObj.Series_Code).FirstOrDefault();
                //var res = from modelMasterObj in db.RS_Model_Master
                //          where modelMasterObj.Shop_ID == shopId && modelMasterObj.Model_Code == modelCode
                //          && (from seriesObj in db.RS_Series where seriesObj.Series_Description == series select seriesObj.Series_Code).Equals(modelMasterObj.Series_Code)
                //          select modelMasterObj;
                var res = from modelMasterObj in db.RS_Model_Master
                          where modelMasterObj.Shop_ID == shopId && modelMasterObj.Model_Code == modelCode
                          && qu == modelMasterObj.Series_Code
                          select modelMasterObj;
                if (res.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool isSeriesAddedForShop(int shopId, String series)
        {
            try
            {
                var res = from modelMasterObj in db.RS_Model_Master
                          where modelMasterObj.Shop_ID == shopId
                          && (from seriesObj in db.RS_Series where seriesObj.Series_Description == series select seriesObj.Series_Code).Equals(modelMasterObj.Series_Code)
                          select modelMasterObj;
                if (res.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public String getModelCode(String series)
        {
            try
            {
                RS_Model_Master res = (from modelMasterObj in db.RS_Model_Master
                                       where (from seriesObj in db.RS_Series where seriesObj.Series_Description == series select seriesObj.Series_Code).Equals(modelMasterObj.Series_Code)
                                       select modelMasterObj).Single();

                return res.Model_Code;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool isStringContainSpecialCharacters(String item)
        {
            try
            {
                item = item.ToLower();
                if (item.Contains('i') || item.Contains('o') || item.Contains('q'))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public bool isSerialDataContainSpecialCharacters(String item)
        //{
        //    try
        //    {
        //        var res=from mmSerialNumberDataObj in db.RS_Serial_Number_Data
        //                where 
        //                mmSerialNumberDataObj.Plant_Code.Contains('i') || mmSerialNumberDataObj.Plant_Code.Contains('o') || mmSerialNumberDataObj.Plant_Code.Contains('q') ||
        //                mmSerialNumberDataObj.Year_Code.Contains('i') || mmSerialNumberDataObj.Year_Code.Contains('o') || mmSerialNumberDataObj.Year_Code.Contains('q') ||
        //                mmSerialNumberDataObj.Month_Code.Contains('i') || mmSerialNumberDataObj.Month_Code.Contains('o') || mmSerialNumberDataObj.Month_Code.Contains('q') ||
        //                mmSerialNumberDataObj.Model.Contains('i') || mmSerialNumberDataObj.Model.Contains('o') || mmSerialNumberDataObj.Model.Contains('q') ||
        //                mmSerialNumberDataObj.Varient_Group.Contains('i') || mmSerialNumberDataObj.Varient_Group.Contains('o') || mmSerialNumberDataObj.Varient_Group.Contains('q') ||
        //                mmSerialNumberDataObj.Common.Contains('i') || mmSerialNumberDataObj.Common.Contains('o') || mmSerialNumberDataObj.Common.Contains('q') ||
        //                mmSerialNumberDataObj.Tractor_Family.Contains('i') || mmSerialNumberDataObj.Tractor_Family.Contains('o') || mmSerialNumberDataObj.Tractor_Family.Contains('q') ||
        //                mmSerialNumberDataObj.Brand.Contains('i') || mmSerialNumberDataObj.Brand.Contains('o') || mmSerialNumberDataObj.Brand.Contains('q') ||
        //                mmSerialNumberDataObj.Engine_Family.Contains('i') || mmSerialNumberDataObj.Engine_Family.Contains('o') || mmSerialNumberDataObj.Engine_Family.Contains('q') ||
        //                mmSerialNumberDataObj.Tractor_Model.Contains('i') || mmSerialNumberDataObj.Tractor_Model.Contains('o') || mmSerialNumberDataObj.Tractor_Model.Contains('q') ||
        //                mmSerialNumberDataObj.Tractor_Feture_A.Contains('i') || mmSerialNumberDataObj.Tractor_Feture_A.Contains('o') || mmSerialNumberDataObj.Tractor_Feture_A.Contains('q') ||
        //                mmSerialNumberDataObj.Month_Code.Contains('i') || mmSerialNumberDataObj.Month_Code.Contains('o') || mmSerialNumberDataObj.Month_Code.Contains('q') ||
        //                mmSerialNumberDataObj.Common.Contains('i') || mmSerialNumberDataObj.Common.Contains('o') || mmSerialNumberDataObj.Common.Contains('q')
        //        item = item.ToLower();
        //        if (item.Contains('i') || item.Contains('o') || item.Contains('q'))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

    }

    /* Class Name                  : RS_OM_Creation
     *  Description                : Override the RS_OM_Creation class with MetaOrderCreation class to define additional attributes, validation and properties
     *  Author, Timestamp          : Jitendra Mahajan       
     */
    public class MetaOrderCreation
    {

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Shop_ID { get; set; }


        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Qty { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
      
        public DateTime Planned_Date { get; set; }
        //[Series_code]

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]

        public string Model_Code { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Order_Type { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Colour_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Platform_Id { get; set; }
    }

}