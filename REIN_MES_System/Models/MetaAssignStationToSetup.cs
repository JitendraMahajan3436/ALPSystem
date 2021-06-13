using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaAssignStationToSetup))]
    public partial class RS_Station_Setup_Mapping
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public int[] selectedStations { get; set; }
        General generalObj = new General();
        public bool deleteOperator(int plantId, int shopId, int lineId, int setupId) //int stationId, int plantId,
        {
            try
            {
                var operatorList = db.RS_Station_Setup_Mapping.Where(p => p.Setup_ID == setupId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Plant_ID == plantId);//&& p.Plant_ID == plantId
                foreach (var item in operatorList.ToList())
                {
                    db.RS_Station_Setup_Mapping.Remove(item);
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
    public class MetaAssignStationToSetup
    {

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        
        public decimal Setup_ID { get; set; }
    }
}