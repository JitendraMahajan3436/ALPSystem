using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZHB_AD.Models
{

    [MetadataType(typeof(MetaShopwiseManaualProduction))]
    public partial class ShopwiseManualProduction
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();



        public bool IsProductionExists(int plantId, int shopId, int Meterformula, DateTime ConsumptionDate)
        {
            try
            {
                ConsumptionDate = ConsumptionDate.Date;

                IQueryable<ShopwiseManualProduction> result;
                if (Meterformula == 0)
                {
                    result = db.ShopwiseManualProductions.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Pro_Datetime == ConsumptionDate);
                }
                else
                {
                    result = db.ShopwiseManualProductions.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Pro_Datetime== ConsumptionDate && p.Pro_Manual_ID != Meterformula);
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

    public class MetaShopwiseManaualProduction
    {
        [Required(ErrorMessage = "The Shop Name field is required.")]
        public Nullable<decimal> Shop_ID { get; set; }

        [Required(ErrorMessage = "The Production field is required.")]
        [Range(1,100000)]          
        public Nullable<int> Production { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Pro_Datetime { get; set; }
    }
}