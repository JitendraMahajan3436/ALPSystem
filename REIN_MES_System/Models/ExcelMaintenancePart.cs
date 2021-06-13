using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ExcelMaintenancePart
    {
        [Required]
        public int Plant_ID { get; set; }

        [Required]
        public int Shop_ID { get; set; }

        [Required]
        public int Line_ID { get; set; }

        [Required]
        public int Station_ID { get; set; }

        [Required]
        public int Machine_ID { get; set; }

        [Required]
        public HttpPostedFileBase Excel_File { get; set; }
    }

    public class MaintenancePartListRecords
    {
        public String partName { get; set; }
        public String partDescription { get; set; }
        public String checkListError { get; set; }
    }
}