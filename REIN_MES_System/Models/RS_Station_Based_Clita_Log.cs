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
    
    public partial class RS_Station_Based_Clita_Log
    {
        public decimal Station_Clita_Log_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public string Clita_Item { get; set; }
        public decimal Clita_Classification_ID { get; set; }
        public decimal Clita_Standard_ID { get; set; }
        public decimal Clita_Tool_ID { get; set; }
        public decimal Clita_Method_ID { get; set; }
        public decimal Cycle { get; set; }
        public string Recipent_Email { get; set; }
        public System.DateTime Start_Date { get; set; }
        public System.DateTime End_Date { get; set; }
        public Nullable<System.TimeSpan> Maitenance_Time_Taken { get; set; }
        public Nullable<System.TimeSpan> Maintenance_Time_TACT { get; set; }
        public Nullable<System.DateTime> Last_Maitenance_Date { get; set; }
        public bool Is_Maintenance_Done { get; set; }
        public Nullable<decimal> Maintenance_User_ID { get; set; }
        public string Remark { get; set; }
        public string Special_Observation { get; set; }
        public Nullable<bool> Is_Mail_Sent { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    }
}
