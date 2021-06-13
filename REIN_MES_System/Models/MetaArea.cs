using ZHB_AD.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZHB_AD.Models
{

    [MetadataType(typeof(MetaArea))]
    public partial class MM_Area
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        public bool IsAreaExists(string Areay_Name, int plantId, int Area_Id)
        {
            try
            {

                IQueryable<MM_Area> result;
                if (Area_Id == 0)
                {
                    result = db.MM_Area.Where(p => p.Area_Name == Areay_Name && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.MM_Area.Where(p => p.Area_Name == Area_Name && p.Plant_ID == plantId && p.Area_Id != Area_Id);
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
    public class MetaArea
    {
        [Required]
        [StringLength(40, MinimumLength = 1)]
        public string Area_Name { get; set; }

        [StringLength(40, MinimumLength = 0)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }
    }
}