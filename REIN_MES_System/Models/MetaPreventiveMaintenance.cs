using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_MT_Preventive_Maintenance
   *  Description                : RS_MT_Preventive_Maintenance class with MetaPreventiveMaintenance class to define additional attributes, validation and properties
   *  Author, Timestamp          : Ajay Wagh      
   */
    [MetadataType(typeof(MetaPreventiveMaintenance))]
    public partial class RS_MT_Preventive_Maintenance
    {
        public string[] mails { get; set; }
        public int[] users { get; set; }

        public System.Web.Mvc.ModelStateDictionary modelSateClientSide { get; set; }
    }
    /* Class Name                : MetaPreventiveMaintenance
   *  Description                : MetaPreventiveMaintenance class to define additional attributes, validation and properties
   *  Author, Timestamp          : Ajay Wagh     
   */
    public class MetaPreventiveMaintenance
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Machine_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Equipment", ResourceType = typeof(ResourceDisplayName))]
        public decimal EQP_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Schedule_Date", ResourceType = typeof(ResourceDisplayName))]
        public System.DateTime Scheduled_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public System.DateTime Last_Maintenance_Date { get; set; }

        
        public decimal Maintenance_User_ID { get; set; }

        [Range(0, 12, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Error_Cycle_Limit")]
        [Display(Name = "Cycle_Period", ResourceType = typeof(ResourceDisplayName))]
        public int Cycle_Period { get; set; }

        
        public string Receipent_Email { get; set; }

    }
}