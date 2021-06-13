using ZHB_AD.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZHB_AD.Models
{
    [MetadataType(typeof(MetaNoWorkingDay))]
    public partial class MM_No_Working_Day
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        /* Method Name                : isShopExists
        *  Description                : Function is used to check duplication of remark name under plant
        *  Author, Timestamp          : Rajat Dolas
        *  Input parameter            : shopName, plantId, shopId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isDayexists(DateTime day, int plantId, int dayId)
        {
            try
            {
                IQueryable<MM_No_Working_Day> result;
                if (dayId == 0)
                {
                    result = db.MM_No_Working_Day.Where(p => p.Day_Date ==day && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.MM_No_Working_Day.Where(p => p.Day_Date == day && p.Plant_ID == plantId && p.Day_ID != dayId);
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






    public class MetaNoWorkingDay
    {


        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Reason_ID", ResourceType = typeof(ResourceDisplayName))]
        public string Reason_ID { get; set; }


        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Remark", ResourceType = typeof(ResourceDisplayName))]
        //[RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "AlphaNumeric")]
        public string Remark { get; set; }


        


        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        
        [Display(Name = "Day_Date", ResourceType = typeof(ResourceDisplayName))]

       
        public DateTime Day_Date { get; set; }

       public decimal Day_ID { get; set; }

    }
}