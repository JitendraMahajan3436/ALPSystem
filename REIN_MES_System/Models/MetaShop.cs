using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Shops
    *  Description                : Override the RS_Shops class with MetaShop class to define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    [MetadataType(typeof(MetaShop))]
    public partial class RS_Shops
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /* Method Name                : isShopExists
        *  Description                : Function is used to check duplication of shop name under plant
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : shopName, plantId, shopId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isShopExists(String shopName, int plantId, int shopId)
        {
            try
            {
                IQueryable<RS_Shops> result;
                if (shopId == 0)
                {
                    result = db.RS_Shops.Where(p => p.Shop_Name == shopName && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.RS_Shops.Where(p => p.Shop_Name == shopName && p.Plant_ID == plantId && p.Shop_ID != shopId);
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

    /* Class Name                 : MetaShop
    *  Description                : Class is used to define additional information with validation which is used in RS_Shops class
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class MetaShop
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [MaxLength(50)]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemoteShopName", "RemoteValidation", AdditionalFields = "Shop_ID,Plant_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        public String Shop_Name { get; set; }

        [MaxLength(16)]
        [Display(Name = "Shop_SAP", ResourceType = typeof(ResourceDisplayName))]
        public String Shop_SAP { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Inserted_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Updated_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Updated_Date { get; set; }

        [Display(Name = "Is_Main", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_Main { get; set; }

        [Display(Name = "Is_Parallel_Order", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_Parallel_Order { get; set; }
    }
}