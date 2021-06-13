using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    /* Class Name                  : MM_MT_TimeBasedMaintenance
      *  Description                : Override the MM_MT_TimeBasedMaintenance class with MetaMMMTPreventiveEquipment class to define additional attributes, validation and properties
      *  Author, Timestamp          : Ajay Wagh       
      */
    [MetadataType(typeof(MetaTimeBasedMaintenance))]
    public partial class MM_MT_Time_Based_Maintenance
    {
        public string[] mails { get; set; }
        public int[] users { get; set; }
    }
    /* Class Name                  : MetaTimeBasedMaintenance
    *  Description                : MetaTimeBasedMaintenance class to define additional attributes, validation and properties
    *  Author, Timestamp          : Ajay Wagh      
    */
    public class MetaTimeBasedMaintenance
    {

        //[Required]
        //[Display(Name = "Plant Name")]
        //public decimal Plant_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Machine_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Schedule_Date_Name", ResourceType = typeof(ResourceDisplayName))]
        public System.DateTime Scheduled_Date { get; set; }


        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public System.DateTime Last_Maintenance_Date { get; set; }

        [Range(0, 12, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Error_Cycle_Limit")]
        [Display(Name = "Cycle_Period", ResourceType = typeof(ResourceDisplayName))]
        public int Cycle_Period { get; set; }
    }
}