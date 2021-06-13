using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaBWT))]

    public partial class RS_BWT
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public bool isBWTNameExists(String BWTName, decimal? plantId, decimal? ShopID, decimal? LineID, decimal? BWTID)
        {
            try
            {
                var result = false;
                if (BWT_ID == null || BWTID==0)
                {
                    result = !(db.RS_BWT.Any(p => p.BWT_Name == BWT_Name && p.Plant_ID == Plant_ID && p.Shop_ID == Shop_ID && p.Line_ID == Line_ID));
                }
                else
                {
                    result = (!db.RS_BWT.Any(p => p.BWT_Name.ToLower().Trim() == BWT_Name.ToLower().Trim() && p.BWT_ID != BWT_ID && p.Plant_ID == Plant_ID && p.Shop_ID == Shop_ID && p.Line_ID == Line_ID));
                }

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    public class MetaBWT
    {
        public decimal BWT_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Shop_ID { get; set; }
        
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Line_ID { get; set; }


        [Remote("isBWTNameExists", "RemoteValidation", AdditionalFields = "BWT_ID,Plant_ID,Shop_ID,Line_ID,BWT_Name", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name ="BWT Name")]
        public string BWT_Name { get; set; }
    }
}