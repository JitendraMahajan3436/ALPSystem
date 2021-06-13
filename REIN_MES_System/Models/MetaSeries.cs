using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaSeries))]
    public partial class RS_Series
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        // public String Plant_Name { get; set; }

        public bool IsSeriesCodeExists(string SeriesCode, decimal shopID)
        {
            try
            {
                bool isSeriesCodeExists = db.RS_Series.Any(p => p.Series_Description == SeriesCode && p.Shop_ID == shopID);
                //if (db.RS_Series.Where(p => p.Series_Code == SeriesCode && p.Shop_ID == shopID).Count() > 0)
                if (isSeriesCodeExists)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsSeriesCodeExistsEdit(string seriesDesc, decimal seriesCode, int shopID)
        {
            try
            {
                bool isSeriesCodeExists = db.RS_Series.Any(p => p.Series_Description == seriesDesc && p.Shop_ID == shopID && p.Series_Code != seriesCode);
                if (isSeriesCodeExists)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
    }

    public class MetaSeries
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceSeries)), ErrorMessageResourceName = "SeriesCode_Error_Name_Required")]
        //[Display(Name = "SeriesCode_Label_SeriesCode_Name", ResourceType = typeof(ResourceSeries))]
        //public String Series_Code { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Series_Description", ResourceType = typeof(ResourceDisplayName))]
        public String Series_Description { get; set; }

    }
}