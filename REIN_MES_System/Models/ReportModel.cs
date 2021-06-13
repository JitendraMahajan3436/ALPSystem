using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ReportModel
    {

    }

    public class PlannedVsActualModel
    {
        public Nullable<int> Shop_ID { get; set; }
        public Nullable<int> Planned_Qty { get; set; }
        public Nullable<int> Actual_Qty { get; set; }
        public Nullable<int> Hold_Qty { get; set; }
        public String Report_Date { get; set; }
        public String Part_No { get; set; }
    }
}