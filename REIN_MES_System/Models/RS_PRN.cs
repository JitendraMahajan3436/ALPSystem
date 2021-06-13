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
    
    public partial class RS_PRN
    {
        public decimal PRN_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public string PRN_Text { get; set; }
        public Nullable<bool> Is_PRN_Printed { get; set; }
        public Nullable<bool> Is_Transferred { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public string Serial_No { get; set; }
        public Nullable<bool> Is_OrderStart { get; set; }
        public Nullable<bool> Is_QOK { get; set; }
        public Nullable<bool> Is_CT { get; set; }
        public Nullable<bool> Is_Reprint { get; set; }
        public Nullable<bool> Is_Rework { get; set; }
        public string Reprinted_By { get; set; }
    }
}
