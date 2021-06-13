using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaSOP))]
    public partial class RS_SOP
    {
        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Model_Master", ResourceType = typeof(ResourceDisplayName))]
        public string[] ModelCodes { get; set; }

    }
    public class MetaSOP
    {
        [Display(Name = "SOP_Image", ResourceType = typeof(ResourceDisplayName))]
        public string Image_Name { get; set; }

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
        [Display(Name = "Platform_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Platform_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Attribute_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Model_Attribute_ID { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceModel_Master)), ErrorMessageResourceName = "Plant_Error_ModelMaster_ModelCode_Required")]
        [Display(Name = "Model_Code", ResourceType = typeof(ResourceDisplayName))]
        public string Model_Code { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Select_Family", ResourceType = typeof(ResourceDisplayName))]
        public decimal Attribute_ID { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceSOPManifest)), ErrorMessageResourceName = "SOP_Image_Required")]
        //public File SOPImage { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "SOP_Name", ResourceType = typeof(ResourceDisplayName))]
        public string SOP_Name { get; set; }

    }
}