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
    
    public partial class RS_PartgroupItem
    {
        public int Row_Id { get; set; }
        public int PartgroupItem_ID { get; set; }
        public decimal Partgroup_ID { get; set; }
        public string Part_No { get; set; }
        public Nullable<decimal> Series_Code { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public Nullable<bool> Is_Order_Create { get; set; }
    
        public virtual RS_Partgroup RS_Partgroup { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
    }
}
