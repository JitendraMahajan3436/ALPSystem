using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ExcelMaintenanceManualData
    {
        [Required]
        public HttpPostedFileBase Excel_File { get; set; }
    }
    public class MaintenanceManualDataRecords
    {
        public string Plant_Name { get; set; }
        public string Shop_Name { get; set; }
        public string Line_Name { get; set; }
        public string Station_Name { get; set; }
        public string Machine_No { get; set; }
        public string BreakDown_Date { get; set; }
        public string DownTime { get; set; }
        public string RepairTime { get; set; }
        public string Problem { get; set; }
        public string Root_Cause { get; set; }
        public string Correction { get; set; }
        public string Corrective_Action { get; set; }
        public string Preventive_Action { get; set; }
        public string Repaired_Date { get; set; }
        public string Owner { get; set; }
        public string Remark { get; set; }

        public string SS_Error_Sucess { get; set; }
    }
}