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
    
    public partial class RS_Temp_PartwiseData
    {
        public int Row_ID { get; set; }
        public string Plant_Name { get; set; }
        public string Shop_Name { get; set; }
        public string Platform_Name { get; set; }
        public string Buyoff_Name { get; set; }
        public string Stage_Name { get; set; }
        public string WorkStation { get; set; }
        public string Part_Name { get; set; }
        public string Defect_Name { get; set; }
        public string Location_Name { get; set; }
        public string Severity_Name { get; set; }
        public Nullable<int> Rework_Duration { get; set; }
        public Nullable<int> Excel_Number { get; set; }
        public string Error_Msg { get; set; }
        public Nullable<bool> Is_Success { get; set; }
    }
}
