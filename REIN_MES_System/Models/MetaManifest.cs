using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.custom_Helper;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaManifest))]
    public partial class RS_Manifest
    {
        [Display(Name = "Model_Master", ResourceType = typeof(ResourceModel_Master))]
        public string[] ModelCodes { get; set; }
    }
    public class MetaManifest
    {
        [RequiredIf("Is_ParentModel_Manifest", false, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Part_No", ResourceType = typeof(ResourceDisplayName))]
        [Remote("IsPartNoValid", "RemoteValidation", AdditionalFields = "Is_ParentModel_Manifest", ErrorMessage = "This {0} is INVALID.")]
        public string Part_No { get; set; }

        [Display(Name = "Manifest_Image", ResourceType = typeof(ResourceDisplayName))]
        public string Image_Name { get; set; }
        
        [Display(Name = "Part_Description", ResourceType = typeof(ResourceDisplayName))]
        public string Part_Description { get; set; }
        public Nullable<bool> Is_ParentModel_Manifest { get; set; }
    }
}