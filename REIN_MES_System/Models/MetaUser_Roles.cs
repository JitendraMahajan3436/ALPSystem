using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    /*  Class Name              : RS_User_Roles
  *  Description                : Override the RS_User_Roles class with MetaUser_Roles class to define additional attributes, validation and properties
  *  Author, Timestamp          : Ajay Wagh    */
    [MetadataType(typeof(MetaUser_Roles))]
    public partial class RS_User_Roles
    {
        public decimal[] Roles { get; set; }
    }

    /*  Class Name                 :  MetaUser_Roles
     *  Description                :  Class to define additional attributes, validation and properties
     *  Author, Timestamp          :  Ajay Wagh    */
    public class MetaUser_Roles
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Employee_Label_Employee_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Employee_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "UserRole_Display_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal[] Roles { get; set; }
    }
}