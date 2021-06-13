using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaStationRoles))]
    public partial class RS_Station_Roles
    {
        public decimal[] Roles { get; set; }
    }
    public class MetaStationRoles
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Station_Label_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Station_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "UserRole_Display_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal[] Roles { get; set; }
    }
}