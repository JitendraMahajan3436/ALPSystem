using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ExcelSkillSetAllocation
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public HttpPostedFileBase Excel_File { get; set; }
    }

    public class EmployeeSkillSetUploadRecords
    {
        public string StationName { get; set; }
        public string OperatorTokenNumber { get; set; }
        public int SkillSet { get; set; }
        public string SS_Error_Sucess { get; set; }
    }
}