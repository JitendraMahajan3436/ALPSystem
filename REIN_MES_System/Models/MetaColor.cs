using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaColor))]
    public partial class RS_Colour
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public bool RemoteColorCode(string Colour_ID, int? Row_ID)
        {
            var result = false;
            if (Row_ID == null)
            {
                result = (!db.RS_Colour.Any(colorcode => colorcode.Colour_ID.ToLower() == Colour_ID.ToLower()));
            }
            else
            {
                result = (!db.RS_Colour.Any(colorcode => colorcode.Colour_ID.ToLower() == Colour_ID.ToLower() && colorcode.Row_ID != Row_ID));
            }
            return result;
        }
    }
    public class MetaColor
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemotePlantSAPCode", "RemoteValidation", AdditionalFields = "Plant_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        public String Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Colour_ID", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemoteColorCode", "RemoteValidation", AdditionalFields = "Colour_ID,Row_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        public String Colour_ID { get; set; }
    }


}