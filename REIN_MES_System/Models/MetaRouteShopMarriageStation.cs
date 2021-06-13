using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaRouteShopMarriageStation))]
    public partial class RS_Route_Marriage_Shop
    {

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        public bool isShopConnected(int subShopId, int subLineId, int subStationId, int marriageShopId, int marriageLineId, int marriageStationId)
        {
            try
            {
                IQueryable<RS_Route_Marriage_Shop> result;

                result = db.RS_Route_Marriage_Shop.Where(p => p.Sub_Shop_ID == subShopId && p.Sub_Line_ID == subLineId && p.Sub_Line_Station_ID == subStationId && p.Marriage_Shop_ID == marriageShopId && p.Marriage_Line_ID == Marriage_Line_ID && p.Marriage_Station_ID == marriageStationId);

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

    public class MetaRouteShopMarriageStation
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]

        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Sub_Shop_Name", ResourceType = typeof(ResourceDisplayName))]

        public int Sub_Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Sub_Line", ResourceType = typeof(ResourceDisplayName))]

        public int Sub_Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Marriage_Shop", ResourceType = typeof(ResourceDisplayName))]

        public int Marriage_Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Sub_Line_Station", ResourceType = typeof(ResourceDisplayName))]

        public int Sub_Line_Station_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Marriage_Line", ResourceType = typeof(ResourceDisplayName))]

        public int Marriage_Line_ID { get; set; }


        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Marriage_Line_Station", ResourceType = typeof(ResourceDisplayName))]

        public int Marriage_Station_ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Inserted_Date", ResourceType = typeof(ResourceDisplayName))]

        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Updated_Date", ResourceType = typeof(ResourceDisplayName))]

        public DateTime? Updated_Date { get; set; }
    }
}