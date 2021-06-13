using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaStyleCode))]
    public partial class RS_Style_Code
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        public bool RemoteStyleCode(decimal? StyleCode_ID, string Style_Code = "")
        {
            var result = false;
            if (StyleCode_ID == null)
            {
                result = (!db.RS_Style_Code.Any(astring => astring.Style_Code.ToLower().Trim() == Style_Code.ToLower().Trim()));

            }
            else
            {
                result = (!db.RS_Style_Code.Any(astring => astring.Style_Code.ToLower().Trim() == Style_Code.ToLower().Trim() && astring.StyleCode_ID != StyleCode_ID));

            }
            return result;
        }

    }
    public class MetaStyleCode
    {
        public decimal StyleCode_ID { get; set; }

        [Remote("RemoteStyleCode", "RemoteValidation", AdditionalFields = "StyleCode_ID,Style_Code", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Style_Code", ResourceType = typeof(ResourceDisplayName))]
        public string Style_Code { get; set; }

        [Display(Name = "Style_Code_Description", ResourceType = typeof(ResourceDisplayName))]
        public string Style_Code_Description { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Style_Code_Number", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> Style_Code_Number { get; set; }
    }
}