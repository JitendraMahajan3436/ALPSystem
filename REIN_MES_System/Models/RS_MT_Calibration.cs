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
    
    public partial class RS_MT_Calibration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RS_MT_Calibration()
        {
            this.RS_MT_Calibration_Users = new HashSet<RS_MT_Calibration_Users>();
        }
    
        public decimal C_Tool_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public string Calibration_Tool { get; set; }
        public string Calibration_Description { get; set; }
        public System.DateTime Scheduled_Date { get; set; }
        public Nullable<System.DateTime> Last_Maintenance_Date { get; set; }
        public Nullable<decimal> Maintenance_User_ID { get; set; }
        public int Cycle_Period { get; set; }
        public string Receipent_Email { get; set; }
        public bool Is_Deleted { get; set; }
        public bool Is_Transfered { get; set; }
        public bool Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public string Updated_Host { get; set; }
    
        public virtual RS_Employee RS_Employee { get; set; }
        public virtual RS_Employee RS_Employee1 { get; set; }
        public virtual RS_Employee RS_Employee2 { get; set; }
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
        public virtual RS_Stations RS_Stations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RS_MT_Calibration_Users> RS_MT_Calibration_Users { get; set; }
    }
}
