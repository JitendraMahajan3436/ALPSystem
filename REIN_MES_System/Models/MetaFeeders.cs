using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZHB_AD.Models
{

    [MetadataType(typeof(MetaFeeders))]
    public partial class MM_Feeders
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();


        public bool IsFeedersExists(int plantId, int shopId, string feeder,  int FeederID)
        {
            try
            {

                IQueryable<MM_Feeders> result;
                if (FeederID == 0)
                {
                    result = db.MM_Feeders.Where(p => p.FeederName == feeder && p.Plant_ID == plantId &&
                                                      p.Shop_ID == shopId);
                }
                else
                {
                    result = db.MM_Feeders.Where(p => p.FeederName == feeder && p.Plant_ID == plantId &&
                                                      p.Shop_ID == shopId && p.Feeder_ID != Feeder_ID);
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
    public class MetaFeeders
    {
 
         [Required]
        public string FeederName { get; set; }
     
        [Required]
        public decimal Shop_ID { get; set; }
        
    }
}