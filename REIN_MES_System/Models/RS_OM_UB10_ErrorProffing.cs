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
    
    public partial class RS_OM_UB10_ErrorProffing
    {
        public decimal Order_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public string Order_No { get; set; }
        public string Serial_No { get; set; }
        public string Model_Code { get; set; }
        public string partno { get; set; }
        public Nullable<decimal> Series_Code { get; set; }
        public System.DateTime Entry_Date { get; set; }
        public System.TimeSpan Entry_Time { get; set; }
        public Nullable<decimal> Partgroup_ID { get; set; }
        public Nullable<bool> Is_Transferred { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public bool Is_Xylo_UB10_Error_Proffing_Done { get; set; }
    }
}
