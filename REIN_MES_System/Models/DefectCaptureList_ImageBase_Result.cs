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
    
    public partial class DefectCaptureList_ImageBase_Result
    {
        public string BODYSRNO { get; set; }
        public decimal Platform_ID { get; set; }
        public Nullable<decimal> Image_ID { get; set; }
        public Nullable<decimal> Image_Grid_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public Nullable<bool> isAlreadySaved { get; set; }
        public Nullable<bool> IsToModify { get; set; }
        public Nullable<bool> isToDelete { get; set; }
        public decimal Row_ID { get; set; }
        public string Stage_Name { get; set; }
        public string Defect_Name { get; set; }
        public string Stage { get; set; }
        public byte[] File_Content { get; set; }
        public string Part_Name { get; set; }
        public Nullable<decimal> Attribute_ID { get; set; }
        public Nullable<bool> As_Is_Ok { get; set; }
        public bool DEFECT_STATUS { get; set; }
        public Nullable<System.DateTime> REWORK_DATE { get; set; }
        public string Severity_Name { get; set; }
        public Nullable<bool> REWORK_STATUS { get; set; }
        public string ModelCode { get; set; }
        public Nullable<decimal> Cause_ID { get; set; }
        public string Rework_Details { get; set; }
        public Nullable<int> REWORK_DURATION { get; set; }
        public string Rework_After { get; set; }
        public string Rework_Before { get; set; }
        public Nullable<decimal> REWORKED_BY { get; set; }
        public Nullable<System.DateTime> Updated_DateRework { get; set; }
        public decimal BuyoffCode { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Source_ID { get; set; }
        public string Source_Name { get; set; }
        public string Platform_Name { get; set; }
        public Nullable<bool> Is_Clear { get; set; }
        public Nullable<decimal> Part_ID { get; set; }
        public Nullable<decimal> Location_ID { get; set; }
        public string VinNo { get; set; }
        public Nullable<System.DateTime> Inserted_DateRework { get; set; }
    }
}
