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
    
    public partial class RS_OM_BCA_Orders_Sequence
    {
        public decimal Row_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Platform_ID { get; set; }
        public decimal Shift_ID { get; set; }
        public string Order_No { get; set; }
        public int Order_Status { get; set; }
        public bool Is_Started { get; set; }
        public bool Is_Locked { get; set; }
        public bool Is_Work_Complete { get; set; }
        public string Serial_No { get; set; }
        public string Model_Code { get; set; }
        public string Style_Code { get; set; }
        public string BOT { get; set; }
        public decimal RSN { get; set; }
        public System.DateTime Planned_Date { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public Nullable<System.DateTime> Last_Status_Change_Time { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public Nullable<bool> Is_Rejected { get; set; }
    }
}
