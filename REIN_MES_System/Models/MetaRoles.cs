using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Roles
    *  Description                : Override the RS_Roles class with MetaRoles class to define additional attributes, validation and properties
    *  Author, Timestamp          : Ajay Wagh       
    */
    [MetadataType(typeof(MetaRoles))]
    public partial class RS_Roles
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal[] Menu_ID { get; set; }
        /* Method Name                : isRoleExists
        *  Description                : Function is used to check duplication of role name
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : role_name,role_id
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isRoleExists(String Role_Name, int Role_ID)
        {
            try
            {
                IQueryable<RS_Roles> result;
                if (Role_ID == 0)
                {
                    result = db.RS_Roles.Where(p => p.Role_Name == Role_Name);
                }
                else
                {
                    result = db.RS_Roles.Where(p => p.Role_Name == Role_Name && p.Role_ID != Role_ID);
                }

                if (result.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    /* Class Name                 : MetaRoles
    *  Description                : Class is used to define additional information with validation which is used in RS_Roles class
    *  Author, Timestamp          : Ajay Wagh      
    */
    public class MetaRoles
    {
        public int Role_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [StringLength(50)]
        [Display(Name = "Roles_Label_Role_Name", ResourceType = typeof(ResourceDisplayName))]
        public String Role_Name { get; set; }

        [StringLength(100)]
        [Display(Name = "Roles_Label_Role_Description", ResourceType = typeof(ResourceDisplayName))]
        public String Role_Description { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Sort_Order", ResourceType = typeof(ResourceDisplayName))]
        public int Sort_Order { get; set; }
        // public decimal[] Menu_ID { get; set; }
    }
}