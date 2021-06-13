using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Menus
   *  Description                : Override the RS_Menus class with MetaMenu class to define additional attributes, validation and properties
   *  Author, Timestamp          : Ajay Wagh       
   */
    [MetadataType(typeof(MetaMenu))]
    public partial class RS_Menus
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();


        /* Method Name                : isMenuExists
        *  Description                : Function is used to check duplication of menu name
        *  Author, Timestamp          : Ajay Wagh
        *  Input parameter            : Menu_Name,Menu_ID
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isMenuExists(String Menu_Name, int Menu_ID)
        {
            try
            {
                IQueryable<RS_Menus> result;
                if (Menu_ID == 0)
                {
                    result = db.RS_Menus.Where(p => p.LinkName == Menu_Name);
                }
                else
                {
                    result = db.RS_Menus.Where(p => p.LinkName == Menu_Name && p.Menu_ID != Menu_ID);
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

    /* Class Name                 : MetaMenu
    *  Description                : Class is used to define additional information with validation which is used in RS_Menus class
    *  Author, Timestamp          : Ajay Wagh      
    */
    public class MetaMenu
    {
        public int Menu_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "LinkName", ResourceType = typeof(ResourceDisplayName))]
        public string LinkName { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "ActionName", ResourceType = typeof(ResourceDisplayName))]
        public string ActionName { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "ControllerName", ResourceType = typeof(ResourceDisplayName))]
        public string ControllerName { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Sort_Order", ResourceType = typeof(ResourceDisplayName))]
        public int Sort_Order { get; set; }
    }
}