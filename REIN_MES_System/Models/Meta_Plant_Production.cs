using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{

    [MetadataType(typeof(Meta_Plant_Production))]
    public partial class MM_Plant_Production
    {
        //private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        public decimal[] Shop { get; set; }
        //public bool IsShopExists(int shop_Id, int plantId, int Pro_Id)
        //{
        //    try
        //    {

        //        IQueryable<MM_Plant_Production> result;
        //        if (Pro_Id == 0)
        //        {
        //            result = db.MM_Plant_Production.Where(p => p.Shop_ID == shop_Id && p.Plant_ID == plantId);
        //        }
        //        else
        //        {
        //            result = db.MM_Plant_Production.Where(p => p.Shop_ID == shop_Id && p.Plant_ID == plantId && p.Pro_Id != Pro_Id);
        //        }

        //        if (result.Count() > 0)
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }
    public class Meta_Plant_Production
    {
        [Required]
        [Display(Name = "Plant Name")]
        public decimal Plant_ID { get; set; }

        public decimal Shop_ID { get; set; }
    }
}