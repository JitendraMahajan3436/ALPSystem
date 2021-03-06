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
    
    public partial class RS_PM_Activity_Part_Log
    {
        public decimal PM_Activity_Part_Log_ID { get; set; }
        public decimal PM_Activity_Log_ID { get; set; }
        public decimal Machine_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public decimal Maintenance_Part_ID { get; set; }
        public Nullable<int> Qty { get; set; }
        public bool Is_Transferred { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Maintenance_Part RS_Maintenance_Part { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_PM_Activity_Log RS_PM_Activity_Log { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
        public virtual RS_Stations RS_Stations { get; set; }
    }
}
