using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Route_Marriage_Station
     *  Description                : Override the RS_Route_Marriage_Station class with MetaRouteMarriageStation class to define additional attributes, validation and properties
     *  Author, Timestamp          : Ajay Wagh       
     */
    [MetadataType(typeof(MetaRouteMarriageStation))]
    public partial class RS_Route_Marriage_Station
    {

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /* Method Name                : isMarriageExists
        *  Description                : Method is used to check marriage is added for the line or not
        *  Author, Timestamp          : Ajay Wagh 
        *  Input parameter            : plantId, shopId, subLineId, subStationId, marriageLine, marriageStation
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isMarriageExists(int plantId, int shopId, int subLineId, int subStationId, int marriageLine, int marriageStation)
        {
            try
            {
                IQueryable<RS_Route_Marriage_Station> result;

                result = db.RS_Route_Marriage_Station.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Sub_Line_ID == subLineId && p.Sub_Line_Station_ID == subStationId && p.Marriage_Line_ID == marriageLine && p.Marriage_Station_ID == marriageStation);

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

    /* Class Name                 : MetaRouteMarriageStation
    *  Description                : Class is used to define additional information with validation which is used in RS_Route_Marriage_Station class
    *  Author, Timestamp          : Ajay Wagh       
    */
    public class MetaRouteMarriageStation
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Sub_Line", ResourceType = typeof(ResourceDisplayName))]
        public int Sub_Line_ID { get; set; }

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