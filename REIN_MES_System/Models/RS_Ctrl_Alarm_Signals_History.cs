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
    
    public partial class RS_Ctrl_Alarm_Signals_History
    {
        public decimal Row_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public Nullable<decimal> Station_ID { get; set; }
        public Nullable<decimal> Shift_ID { get; set; }
        public Nullable<bool> Line_Stop { get; set; }
        public Nullable<bool> Material_Call { get; set; }
        public Nullable<bool> Supervisor_Call { get; set; }
        public Nullable<bool> Maintenance_Call { get; set; }
        public Nullable<bool> Emergency_Call { get; set; }
        public Nullable<bool> Work_Delay { get; set; }
        public Nullable<int> Update_Cntr { get; set; }
        public Nullable<decimal> Start_Row_ID { get; set; }
        public Nullable<decimal> End_Row_ID { get; set; }
        public Nullable<System.DateTime> Start_Time { get; set; }
        public Nullable<System.DateTime> End_Time { get; set; }
        public Nullable<System.TimeSpan> Total_Time { get; set; }
        public Nullable<System.TimeSpan> Down_Time { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
    
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shift RS_Shift { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
        public virtual RS_Stations RS_Stations { get; set; }
    }
}
