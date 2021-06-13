using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaStationClita))]
    public partial class MM_Station_Based_Clita
    {
        public int[] myListBox1 { get; set; }
        public string[] mails { get; set; }
        public int[] users { get; set; }
    }

    public class MetaStationClita
    {

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Label_Shop_Name", ResourceType = typeof(ResourceShop))]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Label_Plant_Name", ResourceType = typeof(ResourceLine))]
        public decimal Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Station_Label_Name", ResourceType = typeof(ResourceStation))]
        public decimal Station_ID { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceMachine)), ErrorMessageResourceName = "Machine_Error_Equipment_Code_Required")]
        //[Display(Name = "Machine", ResourceType = typeof(ResourceMachine))]
        //public int Machine_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Clita_Item", ResourceType = typeof(ResourceClitaItem))]
        public string Clita_Item { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceClitaItem)), ErrorMessageResourceName = "Clita_Classification_required")]
        [Display(Name = "Clita_Classification", ResourceType = typeof(ResourceClitaItem))]
        public decimal Clita_Classification_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceClitaItem)), ErrorMessageResourceName = "Clita_Standards_required")]
        [Display(Name = "Clita_standards", ResourceType = typeof(ResourceClitaItem))]
        public decimal Clita_Standard_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceClitaItem)), ErrorMessageResourceName = "Clita_Tools_required")]
        [Display(Name = "Clita_Tools", ResourceType = typeof(ResourceClitaItem))]
        public decimal Clita_Tool_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceClitaItem)), ErrorMessageResourceName = "Clita_Method_required")]
        [Display(Name = "Clita_Method", ResourceType = typeof(ResourceClitaItem))]
        public decimal Clita_Method_ID { get; set; }

        [Range(0, 12, ErrorMessageResourceType = (typeof(ResourceMachineMaintenance)), ErrorMessageResourceName = "Error_Frequency_Limit")]
        [Display(Name = "Frequency", ResourceType = typeof(ResourceMachineMaintenance))]
        public decimal Cycle { get; set; }


        //[RegularExpression("^([0-9]|0[0-9]|[0-9]|2[0-3]):[0-5][0-9]&")]
        [DataType(DataType.Time, ErrorMessageResourceType = (typeof(ResourceClitaItem)), ErrorMessageResourceName = "Error_TACT_Time")]
        [Display(Name = "Expected Duration")]
        public Nullable<System.TimeSpan> Maintenance_Time_TACT { get; set; }

        //[EmailAddress]
        //[Required(ErrorMessageResourceType = (typeof(ResourceMachineMaintenance)), ErrorMessageResourceName = "Email_Required")]
        //[Display(Name = "Receipent_Email", ResourceType = typeof(ResourceMachineMaintenance))]
        public string Recipent_Email { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> Start_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public Nullable<System.DateTime> End_Date { get; set; }
    }
}