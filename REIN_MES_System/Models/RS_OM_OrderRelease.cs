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
    
    public partial class RS_OM_OrderRelease
    {
        public int Row_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Plant_OrderNo { get; set; }
        public string Order_No { get; set; }
        public string Model_Code { get; set; }
        public string partno { get; set; }
        public Nullable<decimal> Series_Code { get; set; }
        public string Model_Color { get; set; }
        public string Order_Type { get; set; }
        public int Country_ID { get; set; }
        public int Priority { get; set; }
        public string Order_Status { get; set; }
        public decimal RSN { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> Is_Blocked { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public decimal Updated_User_ID { get; set; }
        public System.DateTime Updated_Date { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Deleted { get; set; }
        public string Manage_Remarks { get; set; }
        public Nullable<decimal> ORN { get; set; }
        public Nullable<decimal> CUMN { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public bool Order_Start { get; set; }
        public string UToken { get; set; }
        public Nullable<System.DateTime> Planned_Date { get; set; }
        public string Planned_Time { get; set; }
        public Nullable<bool> Is_Rejected { get; set; }
        public bool Is_Sequenced { get; set; }
        public Nullable<decimal> Planned_Shift_ID { get; set; }
        public Nullable<bool> Hold_By_PPC { get; set; }
        public Nullable<bool> is_NoVIN { get; set; }
    
        public virtual RS_Series RS_Series { get; set; }
    }
}
