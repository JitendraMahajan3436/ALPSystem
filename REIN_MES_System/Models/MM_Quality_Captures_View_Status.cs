//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZHB_AD.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MM_Quality_Captures_View_Status
    {
        public decimal Row_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public string Serial_No { get; set; }
        public decimal View_ID { get; set; }
        public Nullable<decimal> Image_ID { get; set; }
        public Nullable<bool> Is_Clear { get; set; }
        public Nullable<bool> Is_Transferred { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    
        public virtual MM_Employee MM_Employee { get; set; }
        public virtual MM_Employee MM_Employee1 { get; set; }
        public virtual MM_Lines MM_Lines { get; set; }
        public virtual MM_Plants MM_Plants { get; set; }
        public virtual MM_Quality_Image_View MM_Quality_Image_View { get; set; }
        public virtual MM_Quality_Images MM_Quality_Images { get; set; }
        public virtual MM_Shops MM_Shops { get; set; }
        public virtual MM_Stations MM_Stations { get; set; }
    }
}
