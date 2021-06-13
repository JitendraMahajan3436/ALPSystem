using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{

    /* Class Name                 : RS_AM_Shop_Manager_Mapping
    *  Description                : Override the RS_AM_Shop_Manager_Mapping class with MetaManagerConfig class to define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    [MetadataType(typeof(MetaManagerConfig))]
    public partial class RS_AM_Shop_Manager_Mapping
    {
       private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
      // public decimal Manager_ID { get; set; }

       /*          Method Name                :IsManagerExists
       *           Description                :To configure one employee as manager only once
       *           Author, Timestamp          :Jitendra Mahajan
       *           Input parameter            :decimal empId, decimal shopId
       *           Return Type                :bool
       *           Revision                   :
       */
       public bool IsManagerExists(decimal empId, decimal shopId)
       {
           try
           {
               IQueryable<RS_AM_Shop_Manager_Mapping> result;
               if (empId == 0)
               {
                   result = db.RS_AM_Shop_Manager_Mapping.Where(p => p.Employee_ID == empId);
               }
               else
               {
                   result = db.RS_AM_Shop_Manager_Mapping.Where(p => p.Employee_ID == empId && p.Shop_ID == shopId);
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

    /* Class Name                 : MetaManagerConfig
    *  Description                : To define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class MetaManagerConfig
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Label_Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceShop)), ErrorMessageResourceName = "Shop_Error_Shop_Name_Required")]
        [Display(Name = "Shop_Label_Shop_Name", ResourceType = typeof(ResourceShop))]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceManagerConfig)), ErrorMessageResourceName = "Manager_Error_Manager_Name_Required")]
        [Display(Name = "Manager_Label_Manager_Name", ResourceType = typeof(ResourceManagerConfig))]
        public decimal Employee_ID { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }
       
       
    }
}