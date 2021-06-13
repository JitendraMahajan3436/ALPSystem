using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaMachineAlarmMaster))]
    public partial class MM_Ctrl_Machine_Alarms_Master
    {
        //private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        [Display(Name = "Machine_IO_List", ResourceType = typeof(ResourceMachineAlarmMaster))]
        public Int64[] AlarmIOList { get; set; }

        public bool isAlarmExists(String alarmName, int plantId, int machineId, int alarmId = 0)
        {
            try
            {

                IQueryable<MM_Ctrl_Machine_Alarms_Master> result;
                if (alarmId == 0)
                {
                    result = db.MM_Ctrl_Machine_Alarms_Master.Where(p => p.Alarm_Name == alarmName && p.Plant_ID == plantId && p.Machine_ID == machineId);
                }
                else
                {
                    result = db.MM_Ctrl_Machine_Alarms_Master.Where(p => p.Alarm_Name == alarmName && p.Plant_ID == plantId && p.Machine_ID == machineId && p.Alarm_ID != alarmId);
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
    public class MetaMachineAlarmMaster
    {
        [Required(ErrorMessageResourceType = typeof(ResourceValidation), ErrorMessageResourceName = "Required")]
        [Display(Name = "Alarm_Name", ResourceType = typeof(ResourceDisplayName))]
        public String Alarm_Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceValidation), ErrorMessageResourceName = "Required")]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceValidation), ErrorMessageResourceName = "Required")]
        public Nullable<decimal> Machine_ID { get; set; }

        [Display(Name = "Alarm_Message", ResourceType = typeof(ResourceDisplayName))]
        public string Alarm_Message { get; set; }

        [Display(Name = "Is_MTTR", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_MTTR { get; set; }

        [Display(Name = "Alarm_Type_ID", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> Alarm_Type_ID { get; set; }
    }
}