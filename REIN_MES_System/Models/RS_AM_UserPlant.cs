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
    
    public partial class RS_AM_UserPlant
    {
        public decimal UserPlant_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Employee_ID { get; set; }
        public bool Is_Transferred { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public Nullable<bool> Is_QDMS { get; set; }
    
        public virtual RS_Employee RS_Employee { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
    }
}
