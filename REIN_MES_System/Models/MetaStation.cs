using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Stations
    *  Description                : Override the RS_Stations class with MetaStation class to define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    [MetadataType(typeof(MetaStation))]
    public partial class RS_Stations
    {
        public int Plant_ID { get; set; }

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /* Method Name                : isStationExists
        *  Description                : Function is used to check duplication of station name in shop and plant
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : stationName,plantId,shopId,stationId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isStationExists(String stationName, int plantId, int shopId, int lineId, int stationId)
        {
            try
            {
                IQueryable<RS_Stations> result;
                //var res = null;
                if (stationId == 0)
                {
                    result = db.RS_Stations.Where(p => p.Station_Name == stationName && p.Shop_ID == shopId && p.Line_ID == lineId);                    
                }
                else
                {
                    //result = db.RS_Stations.Where(p => p.Station_Name == stationName && p.Shop_ID == shopId && p.Line_ID == lineId && p.Station_ID != stationId);
                    result = db.RS_Stations.Where(sname => sname.Station_Name.ToLower() == stationName.ToLower()  && sname.Shop_ID == shopId && sname.Line_ID == lineId && sname.Station_ID != stationId);
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

    /* Class Name                 : MetaStation
    *  Description                : Class is used to define additional information with validation which is used in RS_Stations class
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class MetaStation
    {
        [Display(Name = "Station_ID", ResourceType = typeof(ResourceDisplayName))]
        public decimal Station_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        //[Remote("RemoteStationName", "RemoteValidation", AdditionalFields = "Shop_ID,Line_ID,Station_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        [MaxLength(100)]
        public String Station_Name { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Line_ID { get; set; }

       // [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^([\d]{1,3}\.){3}[\d]{1,3}$", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Invalid_IP")]
        [MaxLength(50)]
        [Display(Name = "Station_IP_Address", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemoteStationIPAddress", "RemoteValidation", AdditionalFields = "Station_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]

        public String Station_IP_Address { get; set; }

       // [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [MaxLength(200)]
        [Display(Name = "Station_Description", ResourceType = typeof(ResourceDisplayName))]
        public String Station_Description { get; set; }

       // [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [MaxLength(50)]
        [Display(Name = "Station_Host_Name", ResourceType = typeof(ResourceDisplayName))]
        public String Station_Host_Name { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Inserted_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Updated_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Updated_Date { get; set; }
        [Display(Name = "Is_Critical_Station", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_Critical_Station { get; set; }
        [Display(Name = "Is_Buffer", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_Buffer { get; set; }

        public Nullable<int> Sort_Order { get; set; }
        public Nullable<bool> Linemode { get; set; }
        public Nullable<bool> Reworkmode { get; set; }
        public Nullable<bool> Tprint { get; set; }
        public Nullable<bool> IsVin { get; set; }
        public Nullable<bool> IsEngine { get; set; }
        public Nullable<bool> Is_Error_Proofing { get; set; }
    }
}