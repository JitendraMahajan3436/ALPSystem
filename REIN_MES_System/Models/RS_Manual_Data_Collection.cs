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
    
    public partial class RS_Manual_Data_Collection
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RS_Manual_Data_Collection()
        {
            this.RS_MT_Shop_Manual_Data_Collection = new HashSet<RS_MT_Shop_Manual_Data_Collection>();
        }
    
        public decimal MDC_ID { get; set; }
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public decimal Shift_ID { get; set; }
        public decimal Parameter_ID { get; set; }
        public decimal Minimum_Value { get; set; }
        public decimal Maximum_Value { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<int> Frequency { get; set; }
    
        public virtual RS_Employee RS_Employee { get; set; }
        public virtual RS_Employee RS_Employee1 { get; set; }
        public virtual RS_Lines RS_Lines { get; set; }
        public virtual RS_MT_Parameter RS_MT_Parameter { get; set; }
        public virtual RS_Plants RS_Plants { get; set; }
        public virtual RS_Shift RS_Shift { get; set; }
        public virtual RS_Shops RS_Shops { get; set; }
        public virtual RS_Stations RS_Stations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RS_MT_Shop_Manual_Data_Collection> RS_MT_Shop_Manual_Data_Collection { get; set; }
    }
}
