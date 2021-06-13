using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Quality_Station_List
    *  Description                : Override the RS_Quality_Station_List class with MetaQualityStationList class to define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    [MetadataType(typeof(MetaQualityStationList))]
    public partial class RS_Quality_Station_List
    {

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /* Method Name                : isStationAddedInQualityList
        *  Description                : Method is used to check station is added for quality
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : stationId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isStationAddedInQualityList(int stationId)
        {
            try
            {
                IQueryable<RS_Quality_Station_List> result;
                if (stationId == 0)
                {
                    return false;
                }
                else
                {
                    result = db.RS_Quality_Station_List.Where(p => p.Station_ID == stationId);
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

    /* Class Name                 : MetaQualityStationList
    *  Description                : Class is used to define additional information with validation which is used in RS_Quality_Station_List class
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class MetaQualityStationList
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Plant_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Shop_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public int Station_ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }
    }
}