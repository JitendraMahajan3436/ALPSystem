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
    
    public partial class RS_Trim_In
    {
        public decimal Row_ID { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public Nullable<decimal> Station_ID { get; set; }
        public string BodySrNo { get; set; }
        public string Model_Code { get; set; }
        public string Sub_Group { get; set; }
        public string DSN { get; set; }
        public string Model_Desc { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public Nullable<bool> Is_Transferred { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    }
}
