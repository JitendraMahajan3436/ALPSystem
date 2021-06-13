using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaHelpCategory))]
    public partial class RS_Help_Category
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        public bool isHelpCategoryExists(String categoryName, int plantId, int helpCategoryId = 0)
        {
            try
            {
                IQueryable<RS_Help_Category> result;
                //var res = null;
                if (helpCategoryId == 0)
                {
                    result = db.RS_Help_Category.Where(p => p.Help_Category_Name == categoryName && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.RS_Help_Category.Where(p => p.Help_Category_Name == categoryName && p.Plant_ID == plantId && p.Help_Category_ID != helpCategoryId);
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
    public class MetaHelpCategory
    {
        [Required]
        public String Help_Category_Name { get; set; }
    }
}