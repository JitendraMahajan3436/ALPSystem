using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_AM_Line_Supervisor_Mapping
   *  Description                : Override the RS_AM_Line_Supervisor_Mapping class with MetaLineSupervisorMapping class to define additional attributes, validation and properties
   *  Author, Timestamp          : Jitendra Mahajan      
   */
    [MetadataType(typeof(MetaLineSupervisorMapping))]
    public partial class RS_AM_Line_Supervisor_Mapping
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public Nullable<bool> checkBox { get; set; }

        /*           Method Name                :IsEmailExists
         *           Description                :To accept only one Email Id ,Email Id must not get repeated
         *           Author, Timestamp          :Jitendra Mahajan
         *           Input parameter            :String email, int plantId
         *           Return Type                :bool
         *           Revision                   :
         */
        public bool IsSupervisorExists(int line, int supervisor)
        {
            try
            {
                if (db.RS_AM_Line_Supervisor_Mapping.Where(p => p.Line_ID == line && p.Employee_ID != supervisor).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                //IQueryable<RS_Employee> result;
                //if (plantId == 0)
                //{
                //    result = db.RS_Employee.Where(p => p.Email_Address == email);
                //}
                //else 
                //{


                //    result = db.RS_Employee.Where(p => p.Email_Address == email);
                //}

                //if (result.Count() > 0)
                //    return true;
                //else
                //    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

    public class MetaLineSupervisorMapping
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Officer_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceSupervisorConfig)), ErrorMessageResourceName = "Supervisor_Error_Supervisor_Name_Required")]
        //[Display(Name = "Supervisor_Label_Supervisor_Name", ResourceType = typeof(ResourceSupervisorConfig))]
        //public Nullable<decimal> Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Officer_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Employee_ID { get; set; }
    }
}