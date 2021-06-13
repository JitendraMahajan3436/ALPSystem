using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaMaintenanceMachinePart))]
    public partial class RS_Maintenance_Machine_Part
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public int[] selectedParts { get; set; }
        public Boolean Type { get; set; }

        General generalObj = new General();


        public bool deleteOperator(int machineId, String hostName, int userId)
        {
            try
            {
                var AssignedPart = db.RS_PM_Activity_Part.Where(s=>s.Machine_ID == machineId).Select(s => s.Maintenance_Part_ID ).ToList();
                var partList = db.RS_Maintenance_Machine_Part.Where(p => p.Machine_ID == machineId && !AssignedPart.Contains(p.Maintenance_Part_ID));


                foreach (var item in partList.ToList())
                {
                    db.RS_Maintenance_Machine_Part.Remove(item);
                    db.SaveChanges();

                    generalObj.addPurgeDeletedRecords(Convert.ToInt16(item.Plant_ID), "RS_Maintenance_Machine_Part", "Machine_Part_ID", item.Machine_Part_ID.ToString(), hostName, userId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    public class MetaMaintenanceMachinePart
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Station_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Machine_ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }
    }
}