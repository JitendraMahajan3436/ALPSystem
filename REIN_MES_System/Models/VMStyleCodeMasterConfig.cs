using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class VMStyleCodeMasterConfig
    {
        [Display(Name = "Variant_Code", ResourceType = typeof(ResourceDisplayName))]
        public decimal Row_ID { get; set; }

        public decimal Plant_ID { get; set; }

        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }

        [Display(Name = "Variant_Code", ResourceType = typeof(ResourceDisplayName))]
        public string Variant_Code { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Style_Code", ResourceType = typeof(ResourceDisplayName))]
        public decimal StyleCode_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Style_Code", ResourceType = typeof(ResourceDisplayName))]
        public string Style_Code { get; set; }

        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Shop_Name { get; set; }

       
    }
}