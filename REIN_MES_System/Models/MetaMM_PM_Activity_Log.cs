using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaRS_PM_Activity_Log))]
    public partial class RS_PM_Activity_Log
    {
        public string[] months { get; set; }

        public List<partList> partsData { get; set; }
    }


    public class MetaRS_PM_Activity_Log
    {

        public decimal PM_Activity_Log_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Activity_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Activity_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Machine_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Line_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Station_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Due_Date", ResourceType = typeof(ResourceDisplayName))]
        public System.DateTime Due_Date { get; set; }

        [Display(Name = "Confirmed", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_Confirmed { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Observation_Before_PM", ResourceType = typeof(ResourceDisplayName))]
        public string Observation_Before_PM { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Observation_After_PM", ResourceType = typeof(ResourceDisplayName))]
        public string Observation_After_PM { get; set; }
    }
}