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
    
    public partial class RS_MT_Ticket_Users
    {
        public decimal T_U_ID { get; set; }
        public decimal T_ID { get; set; }
        public decimal User_ID { get; set; }
        public string Machine_ID { get; set; }
        public Nullable<decimal> Station_ID { get; set; }
        public bool Is_Transfered { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
    
        public virtual RS_MT_Ticket RS_MT_Ticket { get; set; }
    }
}
