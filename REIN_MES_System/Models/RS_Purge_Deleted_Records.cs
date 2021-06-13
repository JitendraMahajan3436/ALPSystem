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
    
    public partial class RS_Purge_Deleted_Records
    {
        public decimal Row_ID { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public string Table_Name { get; set; }
        public string Column_Name { get; set; }
        public string Column_Value { get; set; }
        public Nullable<bool> Is_Processed { get; set; }
        public bool Is_Transfered { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    
        public virtual RS_Employee RS_Employee { get; set; }
        public virtual RS_Employee RS_Employee1 { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
    }
}