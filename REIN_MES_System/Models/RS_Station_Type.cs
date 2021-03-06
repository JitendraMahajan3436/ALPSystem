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
    
    public partial class RS_Station_Type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RS_Station_Type()
        {
            this.RS_AM_Operator_Station_Allocation = new HashSet<RS_AM_Operator_Station_Allocation>();
            this.RS_AM_Operator_Station_Allocation_History = new HashSet<RS_AM_Operator_Station_Allocation_History>();
            this.RS_Stations = new HashSet<RS_Stations>();
        }
    
        public decimal Station_Type_ID { get; set; }
        public string Station_Type_Name { get; set; }
        public string Station_Type_Description { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RS_AM_Operator_Station_Allocation> RS_AM_Operator_Station_Allocation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RS_AM_Operator_Station_Allocation_History> RS_AM_Operator_Station_Allocation_History { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RS_Stations> RS_Stations { get; set; }
    }
}
