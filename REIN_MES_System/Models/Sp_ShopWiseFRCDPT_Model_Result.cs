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
    
    public partial class Sp_ShopWiseFRCDPT_Model_Result
    {
        public Nullable<int> StageID { get; set; }
        public string BuyoffName { get; set; }
        public Nullable<int> DefectCount { get; set; }
        public Nullable<int> TotalCount { get; set; }
        public Nullable<int> NoDefectCount { get; set; }
        public Nullable<int> ReworkedCount { get; set; }
        public Nullable<decimal> FRC { get; set; }
        public Nullable<decimal> FBO { get; set; }
        public Nullable<decimal> PPM { get; set; }
        public Nullable<decimal> DPT { get; set; }
        public Nullable<int> Flag { get; set; }
        public Nullable<int> As_Is_Ok { get; set; }
    }
}
