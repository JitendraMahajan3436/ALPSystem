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
    
    public partial class RS_Qlty_LBLT_ParameterList
    {
        public int Row_ID { get; set; }
        public string Defect_Name { get; set; }
        public Nullable<decimal> Defect_ID { get; set; }
        public string Model_Code { get; set; }
        public string Stage_Name { get; set; }
        public Nullable<decimal> Stage_ID { get; set; }
        public Nullable<decimal> Location_ID { get; set; }
        public string Location_Name { get; set; }
        public Nullable<decimal> Severity_ID { get; set; }
        public string Severity_Name { get; set; }
        public Nullable<decimal> Min_Val { get; set; }
        public Nullable<decimal> Max_Val { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> BuyoffCode { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<int> Sort_Order { get; set; }
        public Nullable<bool> Is_Active { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public Nullable<decimal> Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    
        public virtual RS_Qlty_Mstr_BuyOff RS_Qlty_Mstr_BuyOff { get; set; }
    }
}
