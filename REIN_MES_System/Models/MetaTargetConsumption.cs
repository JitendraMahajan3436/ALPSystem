using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ZHB_AD.Models
{


    [MetadataType(typeof(MetaTargetConsumption))]
    public partial class MM_PowerTarget
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();


        public bool IsMonthExists(int plantId, int shopId, string Year, string Month, int TargetType, int TargetID)
        {
            try
            {

                IQueryable<MM_PowerTarget> result;
                if (TargetID == 0)
                {
                    result = db.MM_PowerTarget.Where(p => p.Month == Month && p.Plant_ID == plantId &&
                                                      p.Shop_ID == shopId && p.Year == Year && p.TargetType == TargetType);
                }
                else
                {
                    result = db.MM_PowerTarget.Where(p => p.Month == Month && p.Plant_ID == plantId &&
                                                       p.Shop_ID == shopId && p.Year == Year && p.Month == Month && p.TargetType == TargetType && p.Target_ID != TargetID);
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

        public bool IsShiftExists(int plantId, int shopId, string Year, string Month, int Shift, int TargetType, int TargetID)
        {
            try
            {

                IQueryable<MM_PowerTarget> result;
                if (TargetID == 0)
                {
                    result = db.MM_PowerTarget.Where(p => p.Month == Month && p.Plant_ID == plantId &&
                                                      p.Shop_ID == shopId && p.Year == Year && p.TargetType == TargetType);
                }
                else
                {
                    result = db.MM_PowerTarget.Where(p => p.Month == Month && p.Plant_ID == plantId &&
                                                       p.Shop_ID == shopId && p.Year == Year && p.Month == Month && p.TargetType == TargetType && p.Target_ID != TargetID);
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
    public class MetaTargetConsumption
    {

        [Required]
        public string Year { get; set; }

        [Required]
        public string Month { get; set; }

        [Required]
        public Nullable<double> Target { get; set; }
        public string Description { get; set; }

        [Required]
        public Nullable<decimal> Plant_ID { get; set; }

        [Required]
        public Nullable<decimal> Shop_ID { get; set; }
    }
}