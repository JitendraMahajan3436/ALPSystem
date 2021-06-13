using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{

    [MetadataType(typeof(MetaAbsOperatorTransferAllocation))]
    public partial class RS_Abs_Operator_Transfer_Allocation
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public int[] selectedOperators { get; set; }
        General generalObj = new General();
        public bool deleteOperator(int plantId, int shopId, int lineId, int shiftId) //int stationId, int plantId,
        {
            try
            {
                var operatorList = db.RS_Abs_Operator_Transfer_Allocation.Where(p => p.Shift_ID == shiftId && p.Old_Shop_ID == shopId && p.Old_Line_ID == lineId && p.Plant_ID == plantId);//&& p.Plant_ID == plantId
                foreach (var item in operatorList.ToList())
                {
                    db.RS_Abs_Operator_Transfer_Allocation.Remove(item);
                    db.SaveChanges();

                    //generalObj.addPurgeDeletedRecords(Convert.ToInt16(item.Plant_ID), "RS_Assign_OperatorToSupervisor", "OTS_ID", item.OTS_ID.ToString(), hostName, userId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    public class MetaAbsOperatorTransferAllocation
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<decimal> Old_Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<decimal> Old_Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<decimal> New_Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<decimal> New_Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<decimal> Shift_ID { get; set; }
    }
}