using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Route_Configurations
    *  Description                : Override the RS_Lines class with MetaRouteConfiguration class to define additional attributes, validation and properties
    *  Author, Timestamp          : Ajay Wagh      
    */
    [MetadataType(typeof(MetaRouteConfiguration))]
    public partial class RS_Route_Configurations
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public int[] myListBox1 { get; set; }
        

        public int Week { get; set; }
        public int Week2 { get; set; }
        public int Shift1 { get; set; }
        public int Shift2 { get; set; }
        public int Shift3 { get; set; }
        public int Day { get; set; }
        public int FromDay { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int Setup_ID { get; set; }

        /* Method Name                : deleteRouteConfigByLineId
        *  Description                : Method is used to delete the complete route configuration of line
        *  Author, Timestamp          : Ajay Wagh 
        *  Input parameter            : lineId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool deleteRouteConfigByLineId(int lineId)
        {
            try
            {
                //db.RS_Route_Configurations.RemoveRange(db.RS_Route_Configurations.Where(p => Line_ID == lineId));
                var routeConfiguration = db.RS_Route_Configurations.Where(p => p.Line_ID == lineId);

                foreach (var u in routeConfiguration)
                {
                    db.RS_Route_Configurations.Remove(u);
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /* Method Name                : deleteRouteConfigurationDisplayByLineId
        *  Description                : Method is used to delete the display route configuration record
        *  Author, Timestamp          : Ajay Wagh 
        *  Input parameter            : lineId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool deleteRouteConfigurationDisplayByLineId(int lineId)
        {
            try
            {
                //db.RS_Route_Display.RemoveRange(db.RS_Route_Display.Where(p => Line_ID == lineId));

                var routeDisplay = db.RS_Route_Display.Where(p => p.Line_ID == lineId);

                foreach (var u in routeDisplay)
                {
                    db.RS_Route_Display.Remove(u);
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    /* Class Name                 : MetaRouteConfiguration
    *  Description                : Class is used to define additional information with validation which is used in RS_Route_Configurations class
    *  Author, Timestamp          : Ajay Wagh        
    */
    public class MetaRouteConfiguration
    {
        [Required]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]

        public int Shop_ID { get; set; }
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Station_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Line_ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Inserted_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Updated_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Updated_Date { get; set; }
    }
}