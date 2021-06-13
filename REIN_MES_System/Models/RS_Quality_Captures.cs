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
    
    public partial class RS_Quality_Captures
    {
        public decimal Quality_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public string Serial_No { get; set; }
        public decimal Captured_Station_ID { get; set; }
        public decimal Shift_ID { get; set; }
        public Nullable<decimal> Checklist_ID { get; set; }
        public Nullable<decimal> Checkpoint_ID { get; set; }
        public Nullable<decimal> Checkpoint_Station_ID { get; set; }
        public Nullable<decimal> Checkpoint_Station_User_ID { get; set; }
        public Nullable<bool> Is_Value_Based { get; set; }
        public Nullable<bool> Is_First_Shot_Clear { get; set; }
        public Nullable<bool> Is_User_Value_Based { get; set; }
        public Nullable<bool> Is_Clear { get; set; }
        public Nullable<double> User_Checkpoint_Value { get; set; }
        public Nullable<double> User_Checkpoint_Corrective_Value { get; set; }
        public string User_Based_Value { get; set; }
        public Nullable<decimal> Defect_ID { get; set; }
        public Nullable<decimal> Defect_Category_ID { get; set; }
        public Nullable<decimal> Corrective_Action_ID { get; set; }
        public string Remark { get; set; }
        public Nullable<System.TimeSpan> Rework_Man_Minutes { get; set; }
        public Nullable<int> Rework_Man_Power { get; set; }
        public Nullable<decimal> Captured_User_ID { get; set; }
        public Nullable<decimal> Resolved_User_ID { get; set; }
        public Nullable<decimal> Resolved_Station_ID { get; set; }
        public Nullable<bool> CAPA_Status { get; set; }
        public Nullable<decimal> CAPA_Close_User_ID { get; set; }
        public Nullable<System.DateTime> CAPA_Close_Date { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public Nullable<decimal> Image_Group_ID { get; set; }
        public Nullable<decimal> View_ID { get; set; }
        public Nullable<decimal> Image_ID { get; set; }
        public Nullable<decimal> Image_Grid_ID { get; set; }
        public Nullable<decimal> Part_ID { get; set; }
        public Nullable<decimal> CLR_Shop_ID { get; set; }
        public Nullable<bool> Is_Image_Based { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public Nullable<decimal> Resolved_Confirm_Station_ID { get; set; }
        public Nullable<decimal> Resolved_Confirm_User_ID { get; set; }
        public Nullable<System.DateTime> Resolved_Confirm_Date { get; set; }
        public Nullable<System.DateTime> Resolved_Date { get; set; }
        public Nullable<bool> Is_Rejected { get; set; }
        public Nullable<decimal> BWT_ID { get; set; }
        public Nullable<decimal> Quality_Severity_ID { get; set; }
    }
}
