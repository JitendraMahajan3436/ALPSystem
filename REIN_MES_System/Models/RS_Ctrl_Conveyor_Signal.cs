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
    
    public partial class RS_Ctrl_Conveyor_Signal
    {
        public decimal CS_ID { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public Nullable<bool> isLatest { get; set; }
        public Nullable<int> Update_Cntr { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
    
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
    }
}
