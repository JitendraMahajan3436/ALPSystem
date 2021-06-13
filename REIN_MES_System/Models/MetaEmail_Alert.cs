using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZHB_AD.Models
{
    [MetadataType(typeof(MetaEmail_Alert))]
    public partial class MM_Email_Alert

    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        [Required]
        public decimal[] Shops { get; set; }

        [Required]
        public decimal[] Alerts { get; set; }
      
        //public decimal[] Feeder { get; set; }

        public bool IsEmpExists(int plantId, int shopId, int Meterformula,int Emp)
        {
            try
            
                {

                IQueryable<Shop_Index_Config> result;
                if (Meterformula == 0)
                {
                    result = db.Shop_Index_Config.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId);
                }
                else
                {
                    result = db.Shop_Index_Config.Where(p => p.Plant_ID == plantId && p.ComFeederShopID == Meterformula);
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
    public class MetaEmail_Alert
    {
       [Required]
        public decimal Emp_ID { get; set; }
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid Day Count")]
        public int Delay_Escal { get; set; }
    }
    
}