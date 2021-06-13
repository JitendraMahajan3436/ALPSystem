using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaOMPlatform))]
    public partial class RS_OM_Platform
    {

        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        public bool IsAlreadyExistPlatformName(decimal? Platform_ID, decimal? Shop_ID, decimal? Line_ID, string Platform_Name)
        {
            var result = false;
            if (Platform_ID == null && Shop_ID != null && Line_ID != null)
            {
                result = (!db.RS_OM_Platform.Any(bstring => bstring.Platform_Name.ToLower() == Platform_Name.ToLower() && bstring.Shop_ID == Shop_ID
                    && bstring.Line_ID == Line_ID));

            }
            else
            {
                result = (!db.RS_OM_Platform.Any(bstring => bstring.Platform_Name.ToLower() == Platform_Name.ToLower() && bstring.Platform_ID != Platform_ID && bstring.Shop_ID == Shop_ID
                     && bstring.Line_ID == Line_ID));

            }
            return result;
        }
    }
    public class MetaOMPlatform
    {
        public decimal Platform_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Remote("IsAlreadyExistPlatformName", "RemoteValidation", AdditionalFields = "Platform_ID,Shop_ID,Line_ID,Platform_Name", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]

        public string Platform_Name { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Serial_No_Code { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]

        public Nullable<decimal> Shop_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<decimal> Line_ID { get; set; }
    }
}