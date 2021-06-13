using ZHB_AD.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZHB_AD.Models
{

    [MetadataType(typeof(MetaBusniess))]
    public partial class MM_Business
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        public bool IsBusinessExists(string busbness_Name, int plantId, int business_Id)
        {
            try
            {

                IQueryable<MM_Business> result;
                if (business_Id == 0)
                {
                    result = db.MM_Business.Where(p => p.Business_Name == busbness_Name && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.MM_Business.Where(p => p.Business_Name == Business_Name && p.Plant_ID == plantId && p.Business_Id != business_Id);
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
    public class MetaBusniess
    {
        [Required]
        [StringLength(40, MinimumLength = 1)]


        public string Business_Name { get; set; }

        [StringLength(40, MinimumLength = 0)]
        public string Description { get; set; }

        [Required]
        
        public decimal Plant_ID { get; set; }
    }
}