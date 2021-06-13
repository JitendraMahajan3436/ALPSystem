using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;
using System.Web.Mvc;
using REIN_MES_System.Helper;
using REIN_MES_System.custom_Helper;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaPartGroup))]
    public partial class RS_Partgroup
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        /*        Method Name                : IsPartGroupExists
         *        Description                : IsPartGroupExits use to check partgroup already exit or not
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : (integer plantId,integer shopId, integer lineId)
         *        Return Type                : bool 
         *        Revision                   : 1
         */

        public bool IsPartGroupExists(int plantId, int shopId, string partDesc, int lineId)
        {
            try
            {
                IQueryable<RS_Partgroup> result;
                if (lineId == 0)
                {
                    result = db.RS_Partgroup.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Partgrup_Desc == partDesc);
                }
                else
                {
                    result = db.RS_Partgroup.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Partgrup_Desc == partDesc);
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

        /*        Method Name                : GetLastPartGroupNumber
         *        Description                : GetLastPartGroupNumber use to check maximum number
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : (integer plantId,integer shopId)
         *        Return Type                : string 
         *        Revision                   : 1
         */
        //public decimal GetLastPartGroupNumber(int plantId, int shopId)
        //{
        //    try
        //    {
        //        decimal? partGroupNumber = null;
        //        var item = db.RS_Partgroup.Where(c => c.Plant_ID == plantId).Select(c => new { c.Partgroup_ID }).ToList();

        //        if (item.Count > 0)
        //        {
        //            var max_orderno = db.RS_Partgroup.Max(r => r.Partgroup_ID);
        //            string s = max_orderno.ToString();
        //            string p = s.Remove(0, 1);
        //            int number = Convert.ToInt32(p);
        //            number += 1;
        //            partGroupNumber = number.ToString("D4");
        //            partGroupNumber = "P" + partGroupNumber;
        //        }
        //        else
        //        {
        //            string s = "0001";
        //            int number = Convert.ToInt32(s);
        //            string str = number.ToString("D4");
        //            partGroupNumber = "P" + str;
        //        }

        //        return partGroupNumber;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}

    }

    /* Class Name                 : MetaPartgroup
     *  Description                : Class is used to define additional information with validation which is used in RS_Partgroup class
     *  Author, Timestamp          : Jitendra Mahajan       
     */
    public class MetaPartGroup
    {
        [Display(Name = "PartGroup_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Partgroup_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "PartGroup_Name", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemotePartGroupName", "RemoteValidation", AdditionalFields = "Partgroup_ID,Shop_ID,Plant_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]

        public String Partgrup_Desc { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]

        public decimal Line_ID { get; set; }
        [Display(Name = "Is_Part_Number_Exit", ResourceType = typeof(ResourceDisplayName))]
        public bool Order_Create { get; set; }

        [Display(Name = "Consumption_Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> Consumption_Line_ID { get; set; }

        //[RequiredIf("Order_Create", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        //[Display(Name = "Serial_No_Config", ResourceType = typeof(ResourceDisplayName))]
        //public Nullable<long> Serial_Config_ID { get; set; }

        [Display(Name = "Consumption_Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> Consumption_Station_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Quantity", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> Qty { get; set; }
        [RequiredIf("Order_Create", false, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [StringLength(maximumLength: 2, ErrorMessage = "Please Enter 2 Digit Part group Code")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "Part_Group_Code", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemotePartGroupCode", "RemoteValidation", AdditionalFields = "Partgroup_ID,Plant_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        public string Partgroup_Code { get; set; }
        [Display(Name = "Is_Kitting", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_Kitting { get; set; }
        [RequiredIf("Is_Kitting", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Kitting_Release_Line", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> Kitting_Release_Line_ID { get; set; }
        [RequiredIf("Is_Kitting", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Kitting_Release_Station", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> Kitting_Release_Station_ID { get; set; }
    }

}