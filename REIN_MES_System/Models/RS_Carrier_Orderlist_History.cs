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
    
    public partial class RS_Carrier_Orderlist_History
    {
        public decimal Carrier_ID { get; set; }
        public string Carrier_Number { get; set; }
        public string Order_No { get; set; }
        public string Serial_No { get; set; }
        public string Model_Code { get; set; }
        public Nullable<decimal> Station_ID { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Shift_ID { get; set; }
    }
}
