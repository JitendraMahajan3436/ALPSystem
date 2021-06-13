using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaAttributionParameters))]
    public partial class RS_Attribution_Parameters
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public bool IsAttributionDescExists(string type, string desc, int shopId)
        {
            try
            {
                if (db.RS_Attribution_Parameters.Where(p => p.Attribute_Type == type && p.Attribute_Desc == desc && p.Shop_ID == shopId).Count() > 0)
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

        public bool IsDescriptionExistEdit(string type, string desc, int shopId, decimal attributeId)
        {
            try
            {
                bool isDescExist = db.RS_Attribution_Parameters.Any(p => p.Attribute_Type == type && p.Attribute_Desc == desc && p.Shop_ID == shopId && p.Attribute_ID != attributeId);
                if (isDescExist)
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

    }
    public class MetaAttributionParameters
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Attribution_Type_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Attribute_Type { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Attribution_Parameter_Description", ResourceType = typeof(ResourceDisplayName))]
        public string Attribute_Desc { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }

    }
}