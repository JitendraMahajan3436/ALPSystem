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
    
    public partial class RS_PartID
    {
        public decimal RowID { get; set; }
        public string PartID { get; set; }
        public string PartIDDescription { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<decimal> PlantID { get; set; }
    }
}