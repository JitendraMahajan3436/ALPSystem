using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaModelAttribute))]
    
    public partial class RS_Model_Attribute_Master
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public bool IsAttributeExists(string Attribute,decimal shopId,decimal lineId, decimal platformId,decimal ID, decimal plantId)
        {
            try
            {
                if(ID==0)
                {
                    if (db.RS_Model_Attribute_Master.Where(p => p.Attribution == Attribute && p.Plant_ID == plantId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Platform_ID == platformId).Count() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    bool isAttributeExist = db.RS_Model_Attribute_Master.Any(p => p.Attribution == Attribute && p.Model_Attribute_ID != ID && p.Plant_ID == plantId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Platform_ID == platformId);
                    if (isAttributeExist)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
               
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class MetaModelAttribute
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Platform_ID", ResourceType = typeof(ResourceDisplayName))]
        public decimal Platform_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Sub_Assembly_ID", ResourceType = typeof(ResourceDisplayName))]
        public decimal Sub_Assembly_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Attribution", ResourceType = typeof(ResourceDisplayName))]
        public string Attribution { get; set; }
    }
}