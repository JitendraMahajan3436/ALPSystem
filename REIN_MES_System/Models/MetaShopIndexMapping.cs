using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ZHB_AD.Models
{

    [MetadataType(typeof(MetaShopIndexMapping))]
    public partial class Shop_Index_Config
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        [Required]
        public decimal[] IncomeraddFeeder { get; set; }
        public decimal[] IncomersubFeeder { get; set; }
        public decimal[] ConsumeaddFeeder { get; set; }
        public decimal[] ConsumesubFeeder { get; set; }

        public bool IsFeederExists(int plantId, int shopId, int Meterformula)
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

    public class MetaShopIndexMapping
    {


        public Nullable<bool> TagBoolean { get; set; }

        [Required(ErrorMessage = "The Shop Name field is required.")]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessage = "The Plant Name field is required.")]
        public decimal Plant_ID { get; set; }
    }
}