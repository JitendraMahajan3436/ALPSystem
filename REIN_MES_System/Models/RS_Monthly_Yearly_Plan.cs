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
    
    public partial class RS_Monthly_Yearly_Plan
    {
        public long Row_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public string Model_Code { get; set; }
        public decimal Series_Code { get; set; }
        public decimal Attribute_ID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Plan { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
    }
}
