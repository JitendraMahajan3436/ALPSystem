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
    
    public partial class RS_OM_Order_Pattern
    {
        public int Row_ID { get; set; }
        public decimal Platform_ID { get; set; }
        public int Ratio { get; set; }
        public System.DateTime Planned_Date { get; set; }
        public System.TimeSpan Planned_Time { get; set; }
        public decimal Plant_ID { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Priority { get; set; }
        public Nullable<decimal> UB10_SDP_Lockbody_Count { get; set; }
        public Nullable<decimal> UB10_SDP_Lockbody_Complete_Count { get; set; }
        public Nullable<bool> is_Pattern_Changed { get; set; }
    }
}
