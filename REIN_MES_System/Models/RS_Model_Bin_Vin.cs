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
    
    public partial class RS_Model_Bin_Vin
    {
        public decimal Row_ID { get; set; }
        public string BODYSRNO { get; set; }
        public string VIN { get; set; }
        public string Model_Code { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<decimal> Platform_ID { get; set; }
        public Nullable<System.DateTime> S_Date { get; set; }
        public Nullable<System.DateTime> Insert_Date { get; set; }
        public string Model_Desc { get; set; }
    }
}