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
    
    public partial class RS_BOM_History
    {
        public int Bom_ID { get; set; }
        public string Model_Code { get; set; }
        public string Model_Description { get; set; }
        public Nullable<int> Bom_Version { get; set; }
        public Nullable<int> Bom_Revision { get; set; }
        public decimal Plant_ID { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public bool Is_Active { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<System.DateTime> Effective_Date_From { get; set; }
        public Nullable<System.DateTime> Effective_Date_To { get; set; }
    }
}
