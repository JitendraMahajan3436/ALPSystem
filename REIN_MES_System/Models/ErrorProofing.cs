using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ErrorProofing
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        public RS_Partmaster[] getStationPartList(int stationId, String modelcode)
        {
            try
            {
                RS_Partmaster[] mmPartMasterObj = (from mmPartMaster in db.RS_Partmaster
                                                   where (mmPartMaster.Genealogy == true || mmPartMaster.Error_Proofing == true) && mmPartMaster.Station_ID == stationId &&
                                                   (from mmBomItem in db.RS_BOM_Item where mmBomItem.Model_Code == modelcode select mmBomItem.Part_No).Contains(mmPartMaster.Part_No)
                                                   select mmPartMaster).ToArray();


                return mmPartMasterObj;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public RS_Partmaster[] getStationPartListForStation(int stationId, String modelcode, String SerialNo)
        {
            try
            {
                RS_Partmaster[] mmPartMasterObj = (from mmPartMaster in db.RS_Partmaster
                                                   join mmPartGroupItems in db.RS_PartgroupItem on mmPartMaster.Part_No equals mmPartGroupItems.Part_No
                                                   join mmPartGroup in db.RS_Partgroup on mmPartGroupItems.Partgroup_ID equals mmPartGroup.Partgroup_ID
                                                   where (mmPartMaster.Genealogy == true || mmPartMaster.Error_Proofing == true)
                                                   && mmPartMaster.Station_ID == stationId
                                                   && (mmPartGroup.Order_Create == true || mmPartMaster.is_Non_RS_Barcode == true)
                                                   && (from mmBomItem in db.RS_BOM_Item where mmBomItem.Model_Code == modelcode select mmBomItem.Part_No)
                                                   .Contains(mmPartMaster.Part_No)
                                                   && !(from mmGeneology in db.RS_Geneaology where mmGeneology.Main_Order_Serial_No == SerialNo select mmGeneology.Child_Model_Code)
                                                   .Contains(mmPartMaster.Part_No)
                                                   select mmPartMaster).Distinct().ToArray();


                return mmPartMasterObj;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public String getModelcodeBySerialNumber(String serialNo)
        {
            try
            {

                RS_OM_Order_List mmOmOrderListObj = db.RS_OM_Order_List.Where(p => p.Serial_No == serialNo).FirstOrDefault();
                return mmOmOrderListObj.Model_Code;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public String getModelcodeByOrderNumber(String orderNo)
        {
            try
            {
                RS_OM_OrderRelease mmOmOrderReleaseObj = db.RS_OM_OrderRelease.Where(p => p.Order_No == orderNo).Single();
                return mmOmOrderReleaseObj.Model_Code;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool isErrorProofingOK(String modelCode, String partNumber)
        {
            try
            {
                var bomObj = from bomItem in db.RS_BOM_Item
                             where bomItem.Model_Code == modelCode && bomItem.Part_No == partNumber
                             select bomItem;

                if (bomObj.Count() > 0)
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

        public String getChildModelcodeBySerialNumber(String serialNo)
        {
            try
            {
                //int stationID = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).);
                RS_OM_Order_List mmOmOrderListObj = db.RS_OM_Order_List.Where(p => p.Serial_No == serialNo).FirstOrDefault();
                return mmOmOrderListObj.partno;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}