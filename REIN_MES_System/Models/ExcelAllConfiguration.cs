using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ExcelAllConfiguration
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Day { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string FromDay { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public HttpPostedFileBase Excel_File { get; set; }
    }

    public class AllConfigurationUploadRecords
    {
        public string Plant_Name { get; set; }

        public string Shop_Name { get; set; }
        public string Line_Name { get; set; }
        public string Station_Name { get; set; }
        public string Shift_Name { get; set; }
        public string Token_No { get; set; }
        public string Skill_Set { get; set; }
        public string Setup_Name { get; set; }
        public string Line_Officer_Name { get; set; }

        public string LineOfficer_Token_No { get; set; }

        public string Manager_Name { get; set; }
        public string Manager_Token_No { get; set; }
        DateTime Inserted_Date { get; set; }
        int Excel_No { get; set; }
        public string SS_Error_Sucess { get; set; }
    }
}