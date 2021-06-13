using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ExcelOrderUpload
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public HttpPostedFileBase Excel_File { get; set; }
    }

    public class OrderUploadRecords
    {
        public String modelCode { get; set; }
        public String series { get; set; }
        public String qty { get; set; }
        public String orderCreationError { get; set; }
        public bool isCreated { get; set; }
        public String rowId { get; set; }
        public DateTime Planned_Date { get; set; }
        public TimeSpan Planned_Time { get; set; }
        public string ColorCode { get; set; }
        public string Country { get; set; }
    }
}