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
    
    public partial class RS_Ctrl_MBCAB_CONROD
    {
        public decimal MC_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public string Machine_ID { get; set; }
        public string Serial_No { get; set; }
        public string Status { get; set; }
        public Nullable<float> Bolt1_Torque { get; set; }
        public Nullable<float> Bolt1_Angle { get; set; }
        public Nullable<float> Bolt2_Torque { get; set; }
        public Nullable<float> Bolt2_Angle { get; set; }
        public Nullable<float> Bolt3_Torque { get; set; }
        public Nullable<float> Bolt3_Angle { get; set; }
        public Nullable<float> Bolt4_Torque { get; set; }
        public Nullable<float> Bolt4_Angle { get; set; }
        public Nullable<float> Bolt5_Torque { get; set; }
        public Nullable<float> Bolt5_Angle { get; set; }
        public Nullable<float> Bolt6_Torque { get; set; }
        public Nullable<float> Bolt6_Angle { get; set; }
        public Nullable<float> Bolt7_Torque { get; set; }
        public Nullable<float> Bolt7_Angle { get; set; }
        public Nullable<float> Bolt8_Torque { get; set; }
        public Nullable<float> Bolt8_Angle { get; set; }
        public Nullable<float> Bolt9_Torque { get; set; }
        public Nullable<float> Bolt9_Angle { get; set; }
        public Nullable<float> Bolt10_Torque { get; set; }
        public Nullable<float> Bolt10_Angle { get; set; }
        public Nullable<float> Bolt11_Torque { get; set; }
        public Nullable<float> Bolt11_Angle { get; set; }
        public Nullable<float> Bolt12_Torque { get; set; }
        public Nullable<float> Bolt12_Angle { get; set; }
        public Nullable<int> Update_Counter { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public bool Is_Transfered { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
    
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
    }
}
