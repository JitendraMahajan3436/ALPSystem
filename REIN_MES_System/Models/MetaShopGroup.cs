using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{


    [MetadataType(typeof(MetaShopGroup))]
    public partial class RS_ShopsCategory
    {
        //private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();


        //public bool IsShopGroupExists(String shopGroup, int plantId, int shopGroupId)
        //{
        //    try
        //    {
        //        IQueryable<RS_ShopsCategory> result;
        //        if (shopGroupId == 0)
        //        {
        //            result = db.RS_ShopsCategory.Where(p => p.ShopsCategory_Name == shopGroup && p.Plant_ID == plantId);
        //        }
        //        else
        //        {
        //            result = db.RS_ShopsCategory.Where(p => p.ShopsCategory_Name == shopGroup && p.Plant_ID == plantId && p.ShopsCat_ID != shopGroupId);
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
    public class MetaShopGroup
    {

        [Required(ErrorMessage = " Please enter the ShopsGroup Name")]
        [StringLength(40, MinimumLength = 2)]
        public string ShopsCategory_Name { get; set; }

       
        public Nullable<int> Sort_Order { get; set; }
    }
}