//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace REIN_MES_System.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RS_CVI_Station
    {
        public decimal Row_ID { get; set; }
        public Nullable<int> STA_ResultID { get; set; }
        public Nullable<int> STA_ResultNumber { get; set; }
        public Nullable<short> STA_StationNumber { get; set; }
        public string STA_Report { get; set; }
        public string STA_VIN { get; set; }
        public Nullable<short> STA_CycleNumber { get; set; }
        public string STA_CycleComment { get; set; }
        public Nullable<short> STA_CycleOKProgrammed { get; set; }
        public Nullable<short> STA_CycleOKCount { get; set; }
        public Nullable<short> STA_TorqueUnit { get; set; }
        public string STA_StationName { get; set; }
        public string STA_StationComment { get; set; }
        public Nullable<short> STA_SpindleCount { get; set; }
        public Nullable<System.DateTime> STA_DateTime { get; set; }
        public bool Is_Transfered { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
    }
}
