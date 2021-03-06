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
    
    public partial class RS_Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RS_Department()
        {
            this.RS_Employee = new HashSet<RS_Employee>();
        }
    
        public decimal Department_ID { get; set; }
        public string Department_Name { get; set; }
        public Nullable<bool> Supervisor_Flag { get; set; }
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<decimal> Line_ID { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RS_Employee> RS_Employee { get; set; }
    }
}
