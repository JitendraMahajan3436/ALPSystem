using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZHB_AD.Models
{

    [MetadataType(typeof(MetaShopwiseManualReading))]
    public partial class ShopwiseManualConsumption
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
      
     

        public bool IsFeederExists(int plantId, int shopId,  int Meterformula, DateTime ConsumptionDate, int Tagindex )
        {
            try
            {
                ConsumptionDate = ConsumptionDate.Date;

                IQueryable<ShopwiseManualConsumption> result;
                if (Meterformula == 0)
                {
                    result = db.ShopwiseManualConsumptions.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.ConsumptionDate ==ConsumptionDate && p.TagIndex == Tagindex);
                }
                else
                {
                    result = db.ShopwiseManualConsumptions.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.ConsumptionDate == ConsumptionDate &&  p.TagIndex == Tagindex &&  p.Manual_ID == Meterformula );
                }

                if (Meterformula == 0)
                {
                    if (result.Count() == 0)
                        return false;
                    else
                        return true;
                }
                else
                {
                    if (result.Count() > 0)
                        return true;
                    else
                        return false;
                }   
              
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }





    public class MetaShopwiseManualReading
    {


        [Required(ErrorMessage = "The Shop Name field is required.")]
        public Nullable<decimal> Shop_ID { get; set; }

        [Required(ErrorMessage = "The Feeder Name field is required.")]
        public Nullable<int> TagIndex { get; set; }

        [Required(ErrorMessage = "The Consumption field is required.")]
       
        public Nullable<double> Consumption { get; set; }

        [Required]
        public Nullable<bool> Consider { get; set; }
    }
}