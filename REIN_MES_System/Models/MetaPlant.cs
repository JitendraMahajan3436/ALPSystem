using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Plants
    *  Description                : Override the RS_Plants class with MetaPlant class to define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    [MetadataType(typeof(MetaPlant))]
    public partial class RS_Plants
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /* Method Name                : isPlantExists
        *  Description                : Function is used to check plant is already exist or not
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : plantName, plantId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isPlantExists(String plantName, int plantId)
        {
            try
            {
                IQueryable<RS_Plants> result;
                if (plantId == 0)
                {
                    result = db.RS_Plants.Where(p => p.Plant_Name == plantName);
                }
                else
                {
                    result = db.RS_Plants.Where(p => p.Plant_Name == plantName && p.Plant_ID != plantId);
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

        public bool isSAPCodeExists(String sapCode, int plantId)
        {
            try
            {
                IQueryable<RS_Plants> result;
                if (plantId == 0)
                {
                    result = db.RS_Plants.Where(p => p.Plant_Code_SAP == sapCode);
                }
                else
                {
                    result = db.RS_Plants.Where(p => p.Plant_Code_SAP == sapCode && p.Plant_ID != plantId);
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

    /* Class Name                 : MetaPlant
    *  Description                : Class is used to define additional information with validation which is used in MM_Plant class
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class MetaPlant
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        [MaxLength(100,ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
        //[MinLength(3, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Min_Length")]
       // [Remote("RemotePlantName","RemoteValidation",AdditionalFields ="Plant_ID",ErrorMessageResourceType =(typeof(ResourceValidation)),ErrorMessageResourceName ="Exist")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "AlphaNumeric")]
         public String Plant_Name { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
        //[MinLength(3, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Min_Length")]
        [Display(Name = "Plant_Code", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemotePlantSAPCode", "RemoteValidation",AdditionalFields ="Plant_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "AlphaNumeric")]
        public String Plant_Code_SAP { get; set; }

        [MaxLength(200, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
          [Display(Name = "Plant_Address", ResourceType = typeof(ResourceDisplayName))]
        public String Address { get; set; }

        [MaxLength(50, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
        
        [Display(Name = "Plant_Country", ResourceType = typeof(ResourceDisplayName))]
        public String Country { get; set; }

        [MaxLength(50, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
        [Display(Name = "Plant_State", ResourceType = typeof(ResourceDisplayName))]
        public String State { get; set; }

         [MaxLength(50, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
       
        [Display(Name = "Plant_City", ResourceType = typeof(ResourceDisplayName))]
        public String City { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Inserted_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Updated_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Updated_Date { get; set; }
    }
}