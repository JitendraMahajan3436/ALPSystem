using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.custom_Helper;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaPMActivity))]
    public partial class RS_PM_Activity
    {
        public Nullable<decimal>[] Maintenance_Part_ID { get; set; }

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        //public List<RS_MT_Machines> GetMachineByStationId(int plant_ID,int Station_ID)
        //{
        //    try
        //    {
        //        var MachineList = db.RS_MT_Machines.Where(c => c.Plant_ID == plant_ID && c.Station_ID == Station_ID).OrderBy(c=>c.Machine_Name).ToList();
        //        return MachineList;
        //    }
        //    catch (Exception)
        //    {
        //        return null;

        //    }
        //}

        public List<RS_MT_Machines> GetMachineByShopId(int plant_ID, int Shop_ID)
        {
            try
            {
                var MachineList = db.RS_MT_Machines.Where(c => c.Plant_ID == plant_ID && c.Shop_ID == Shop_ID && c.IsActive == true).OrderBy(c => c.Machine_Name).ToList();
                return MachineList;
            }
            catch (Exception)
            {
                return null;

            }
        }
    }
    public class MetaPMActivity
    {


        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Machine_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Station_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [MaxLength(100, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
        [MinLength(4, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Min_Length")]
        [Display(Name = "Activity_Name", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemoteActivityName", "RemoteValidation", AdditionalFields = "Activity_Name,Activity_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        [RegularExpression("^[a-zA-Z0-9._ -]*$", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "AlphaNumeric")]
        public string Activity_Name { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Frequency", ResourceType = typeof(ResourceDisplayName))]
        [Range(1, int.MaxValue, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Numeric")]
        public int Frequency { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Start_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime Start_Date { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Activity_Owner", ResourceType = typeof(ResourceDisplayName))]
        public decimal Activity_Owner_ID { get; set; }

        [Display(Name = "Activity_Description", ResourceType = typeof(ResourceDisplayName))]
        [MaxLength(100, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Max_Length")]
        [RegularExpression("^[a-zA-Z0-9._ -]*$", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "AlphaNumeric")]

        public string Activity_Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Last_Date", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<System.DateTime> Last_Date { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Equipment_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal EQP_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Part_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Maintenance_Part_ID { get; set; }

        [Display(Name = "Is_Value_Based", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_Value_Based { get; set; }

        [RequiredIf("Is_Value_Based", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<double> Lower_Limit { get; set; }

        [RequiredIf("Is_Value_Based", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<double> Upper_Limit { get; set; }

        [Display(Name = "Is_User_Value_Based", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_User_Value_Based { get; set; }

        [Display(Name = "M_ID", ResourceType = typeof(ResourceDisplayName))]
        [RequiredIf("Is_Value_Based", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal M_ID { get; set; }
    }
}