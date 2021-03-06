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
    
    public partial class QDMSList_Result
    {
        public string Stage { get; set; }
        public decimal BuyoffCode { get; set; }
        public decimal Shop_ID { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public Nullable<bool> Is_Dependancy { get; set; }
        public Nullable<bool> Is_Offline { get; set; }
        public Nullable<bool> Is_Common_Buyoff { get; set; }
        public Nullable<bool> Is_SAP_Declaration { get; set; }
        public Nullable<bool> Rework_Parameter_Required { get; set; }
        public Nullable<bool> Is_Final_Buyoff { get; set; }
        public Nullable<bool> Is_Part_Base { get; set; }
        public Nullable<bool> IS_Active { get; set; }
        public Nullable<bool> Is_Image_Base { get; set; }
        public Nullable<int> Part_Base_Priority { get; set; }
        public Nullable<int> Image_Base_Priority { get; set; }
        public Nullable<bool> Is_Wrap_Buyoff { get; set; }
        public Nullable<bool> Is_Line_Stop { get; set; }
        public Nullable<bool> Is_Vin { get; set; }
        public Nullable<bool> Is_Bin { get; set; }
        public Nullable<bool> Is_Aggregate { get; set; }
        public string Host_Name { get; set; }
        public Nullable<bool> Is_Genology { get; set; }
        public Nullable<bool> Is_Validate { get; set; }
        public Nullable<bool> Is_STR_Show { get; set; }
        public Nullable<bool> Is_LBLT_Parameter { get; set; }
        public Nullable<bool> Is_PartChange_Buyoff { get; set; }
        public Nullable<bool> Is_Equipment_Creation { get; set; }
        public Nullable<bool> Is_Paintshop_Buyoff { get; set; }
        public Nullable<bool> Is_Other_BIN { get; set; }
        public Nullable<bool> Is_RFD_Station { get; set; }
        public int SortOrder { get; set; }
        public Nullable<bool> Is_Attribute_Required { get; set; }
    }
}
