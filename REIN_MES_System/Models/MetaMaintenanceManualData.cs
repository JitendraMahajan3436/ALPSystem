using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaMaintenanceManualData))]
    public partial class RS_MT_Maintenance_Manual_Data
    {

    }
    public class MetaMaintenanceManualData
    {
        [Display(Name = "Shop_ID", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Shop_ID { get; set; }

        [Display(Name = "Line_ID", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Line_ID { get; set; }

        [Display(Name = "Station_ID", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Station_ID { get; set; }

        [Display(Name = "Machine_No", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Machine_No { get; set; }

        [Display(Name = "BreakDown_Date", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public DateTime BreakDown_Date { get; set; }

        [Display(Name = "DownTime", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public System.TimeSpan DownTime { get; set; }

        [Display(Name = "RepairTime", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public System.TimeSpan RepairTime { get; set; }

        [Display(Name = "Problem", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Problem { get; set; }

        [Display(Name = "Root_Cause", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Root_Cause { get; set; }

        [Display(Name = "Correction", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Correction { get; set; }

        [Display(Name = "Corrective_Action", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Corrective_Action { get; set; }

        [Display(Name = "Preventive_Action", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Preventive_Action { get; set; }

        [Display(Name = "Repaired_Date", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public System.DateTime Repaired_Date { get; set; }

        [Display(Name = "Owner", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Owner { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(ResourceDisplayName))]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Remark { get; set; }
    }
}