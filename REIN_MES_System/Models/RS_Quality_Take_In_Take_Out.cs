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
    
    public partial class RS_Quality_Take_In_Take_Out
    {
        public decimal TITO_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public decimal Take_Out_Line_ID { get; set; }
        public decimal Take_Out_Station_ID { get; set; }
        public Nullable<decimal> Take_In_Line_ID { get; set; }
        public Nullable<decimal> Take_In_Station_ID { get; set; }
        public System.DateTime Take_Out_Date { get; set; }
        public Nullable<System.DateTime> Take_In_Date { get; set; }
        public string Model_Code { get; set; }
        public string Serial_No { get; set; }
        public string Order_No { get; set; }
        public string VIN_No { get; set; }
        public decimal Quality_TO_Reason_ID { get; set; }
        public string Remark { get; set; }
        public string Rework_Status { get; set; }
        public Nullable<bool> Is_Transferred { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<bool> Is_Rejected { get; set; }
        public string Skid_Number { get; set; }
    }
}
