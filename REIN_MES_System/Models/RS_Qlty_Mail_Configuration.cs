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
    
    public partial class RS_Qlty_Mail_Configuration
    {
        public decimal Mail_Config_Id { get; set; }
        public string Mail_Config_Name { get; set; }
        public string Email_ID { get; set; }
        public Nullable<bool> Is_Pareto_Mail { get; set; }
        public Nullable<decimal> Attribute_ID { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    }
}
