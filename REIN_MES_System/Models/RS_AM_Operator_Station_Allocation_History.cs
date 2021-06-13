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
    
    public partial class RS_AM_Operator_Station_Allocation_History
    {
        public decimal Row_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public decimal Shift_ID { get; set; }
        public Nullable<decimal> Opt_ID { get; set; }
        public decimal Employee_ID { get; set; }
        public System.DateTime Allocation_Date { get; set; }
        public Nullable<decimal> Skill_ID { get; set; }
        public bool Is_Transferred { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public Nullable<bool> Is_Present { get; set; }
        public Nullable<int> Week_Number { get; set; }
        public string Week_Day { get; set; }
        public Nullable<int> Copy_Week_Number { get; set; }
        public Nullable<bool> SP_Flag { get; set; }
        public Nullable<bool> Is_Buffer_Station { get; set; }
        public Nullable<decimal> Prev_Shop_ID { get; set; }
        public Nullable<decimal> Prev_Line_ID { get; set; }
        public Nullable<decimal> Prev_Station_ID { get; set; }
        public Nullable<bool> Is_Transfer_Operator { get; set; }
        public Nullable<decimal> Setup_ID { get; set; }
        public Nullable<bool> Is_OJT_Operator { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public Nullable<decimal> Station_Type_ID { get; set; }
        public Nullable<bool> Is_Setup_Freeze { get; set; }
    
        public virtual RS_AM_Skill_Set RS_AM_Skill_Set { get; set; }
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Lines RS_Lines1 { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Setup RS_Setup { get; set; }
        public virtual RS_Shift RS_Shift { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
        public virtual RS_Shops RS_Shops1 { get; set; }
        public virtual RS_Station_Type RS_Station_Type { get; set; }
        public virtual RS_Stations RS_Stations { get; set; }
        public virtual RS_Stations RS_Stations1 { get; set; }
    }
}