using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(metaOperatorStationAllocationHistory))]
    public partial class RS_AM_Operator_Station_Allocation_History
    {
        // public DateTime AllocationDate { get; set; }

        public int? RStatus { get; set; }
        public string Request_Status { get; set; }
        public string Employee_No { get; set; }
        public string Employee_Name { get; set; }
        public string Shift_Name { get; set; }
        public string NewShift { get; set; }
    }
    public class metaOperatorStationAllocationHistory
    {
        // public string Allocation_Date { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Allocation_Date { get; set; }
    }
}