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
    
    public partial class RS_Training_Users
    {
        public decimal TU_ID { get; set; }
        public decimal Training_Session_ID { get; set; }
        public decimal Employee_ID { get; set; }
        public Nullable<decimal> Skill_ID { get; set; }
        public Nullable<bool> Is_Attended { get; set; }
        public string Training_Remarks { get; set; }
        public bool Is_Transfered { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    
        public virtual RS_AM_Skill_Set RS_AM_Skill_Set { get; set; }
        public virtual RS_Training_Sessions RS_Training_Sessions { get; set; }
    }
}