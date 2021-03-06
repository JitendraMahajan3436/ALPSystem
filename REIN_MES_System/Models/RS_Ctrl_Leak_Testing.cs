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
    
    public partial class RS_Ctrl_Leak_Testing
    {
        public decimal LT_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public string Machine_ID { get; set; }
        public string Serial_No { get; set; }
        public string Leak_Value_Fuel { get; set; }
        public string Leak_Value_Water { get; set; }
        public string Leak_Value_Normal { get; set; }
        public string Leak_Test_Fuel { get; set; }
        public string Leak_Test_Water { get; set; }
        public string Leak_Test_Normal { get; set; }
        public Nullable<int> Update_Counter { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public bool Is_Transfered { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public Nullable<bool> Is_Rejected { get; set; }
    
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
    }
}
