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
    
    public partial class RS_Sub_Menus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RS_Sub_Menus()
        {
            this.RS_Menu_Role = new HashSet<RS_Menu_Role>();
        }
    
        public decimal Sub_Menu_ID { get; set; }
        public string ActionName { get; set; }
        public string LinkName { get; set; }
        public long Menu_ID { get; set; }
        public string Inserted_Host { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public Nullable<bool> Is_Transfered { get; set; }
        public Nullable<bool> Is_Purgeable { get; set; }
        public Nullable<bool> Is_Edited { get; set; }
        public int Sort_Order { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RS_Menu_Role> RS_Menu_Role { get; set; }
        public virtual RS_Sub_Menus RS_Sub_Menus1 { get; set; }
        public virtual RS_Sub_Menus RS_Sub_Menus2 { get; set; }
    }
}
