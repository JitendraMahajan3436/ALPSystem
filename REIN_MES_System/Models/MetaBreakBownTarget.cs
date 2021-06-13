using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaBreakBownTarget))]
    public partial class RS_MT_BreakDown_Target
    {
        //  [Required(ErrorMessageResourceType = (typeof(ResourceModel_Master)), ErrorMessageResourceName = "Plant_Error_ModelMaster_ModelCode_Required")]
        // [Display(Name = "Model_Master", ResourceType = typeof(ResourceModel_Master))]
        public string[] Months { get; set; }
    }

    public class MetaBreakBownTarget
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Target_ID", ResourceType = typeof(ResourceDisplayName))]
        public decimal Target_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Type", ResourceType = typeof(ResourceDisplayName))]
        public string Type { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_ID", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_ID", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Year", ResourceType = typeof(ResourceDisplayName))]
        public string Year { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Month", ResourceType = typeof(ResourceDisplayName))]
        public string Month { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Value_Type", ResourceType = typeof(ResourceDisplayName))]
        public string Value_Type { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Module", ResourceType = typeof(ResourceDisplayName))]
        public string Module { get; set; }
    }
}