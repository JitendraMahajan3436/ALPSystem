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
    
    public partial class RS_EngineDropping
    {
        public decimal EngineDropingRow_ID { get; set; }
        public string EngineSrNo { get; set; }
        public string Model_Code { get; set; }
        public System.DateTime Scanning_Date { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public string Inserted_Host { get; set; }
        public bool Is_EngineOk { get; set; }
    }
}
