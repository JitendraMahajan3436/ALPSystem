using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZHB_AD.Models
{



    [MetadataType(typeof(MetaPlantTarget))]
    public partial class MM_PlantTarget
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();


        public bool IsMonthExists(int plantId, string Year, string Month, int TargetID)
        {
            try
            {

                IQueryable<MM_PlantTarget> result;
                if (TargetID == 0)
                {
                    result = db.MM_PlantTarget.Where(p => p.Month == Month && p.Plant_ID == plantId &&
                                                     p.Year == Year);
                }
                else
                {
                    result = db.MM_PlantTarget.Where(p => p.Month == Month && p.Plant_ID == plantId &&
                                                      p.Year == Year && p.Month == Month && p.Target_ID != TargetID);
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
    public class MetaPlantTarget
    {


        [Required]
        public string Year { get; set; }

        [Required]
        public string Month { get; set; }

        [Required]
        public Nullable<double> Target { get; set; }
        public string Description { get; set; }

       

    
       
    }
}