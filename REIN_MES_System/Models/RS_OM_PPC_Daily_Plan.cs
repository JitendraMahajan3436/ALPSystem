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
    
    public partial class RS_OM_PPC_Daily_Plan
    {
        public long Row_ID { get; set; }
        public System.DateTime Plan_Date { get; set; }
        public decimal Shop_ID { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public int Planned_Qty { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public decimal Shift_ID { get; set; }
    }
}
