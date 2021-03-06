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
    
    public partial class RS_OM_Order_Status_Log
    {
        public decimal Plant_ID { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public Nullable<decimal> Station_ID { get; set; }
        public string Order_No { get; set; }
        public string Serial_No { get; set; }
        public string From_Status { get; set; }
        public string Action_Status { get; set; }
        public string Remark { get; set; }
        public string Carrier_No { get; set; }
        public string Skid_No { get; set; }
        public string Hanger_No { get; set; }
        public string Barcode { get; set; }
        public Nullable<decimal> Paint_Shop_ID { get; set; }
        public System.DateTime Entry_Date { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public Nullable<bool> Is_Rejected { get; set; }
        public string Param_1 { get; set; }
        public string Param_2 { get; set; }
        public string Param_3 { get; set; }
        public string Param_4 { get; set; }
        public string Param_5 { get; set; }
    
        public virtual RS_Employee RS_Employee { get; set; }
        public virtual RS_Employee RS_Employee1 { get; set; }
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
        public virtual RS_Stations RS_Stations { get; set; }
    }
}
