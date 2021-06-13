using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Models;

namespace REIN_MES_System.Models
{
    
    [MetadataType(typeof(MetaChildPartMaster))]
    public partial class RS_BIW_Part_Master
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public bool isPartExists(String PartNo, int plantId, int partId)
        {
            try
            {
                IQueryable<RS_BIW_Part_Master> result;
                if (partId == 0)
                {
                    result = db.RS_BIW_Part_Master.Where(p => p.Variant_Code == PartNo && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.RS_BIW_Part_Master.Where(p => p.Variant_Code == PartNo && p.Plant_ID == plantId && p.Row_ID != partId);
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
    }
    public class MetaChildPartMaster
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Variant_Code", ResourceType = typeof(ResourceDisplayName))]
        public string Variant_Code { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "VARIANT_DESC", ResourceType = typeof(ResourceDisplayName))]
        public string VARIANT_DESC { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Variant_Long_Desc", ResourceType = typeof(ResourceDisplayName))]
        public string LONG_DESC { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Platform_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Sub_Assembly_ID", ResourceType = typeof(ResourceDisplayName))]
        public decimal Sub_Assembly_ID { get; set; }
    }
}