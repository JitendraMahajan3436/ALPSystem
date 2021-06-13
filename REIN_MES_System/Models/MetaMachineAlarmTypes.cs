using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaMachineAlarmTypes))]
    public partial class MM_Ctrl_Machine_Alarm_Types
    {
        //private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        public bool isAlarmTypeExists(String alarmName, int plantId, int alarmId = 0)
        {
            try
            {
                IQueryable<MM_Ctrl_Machine_Alarm_Types> result;
                if (alarmId == 0)
                {
                    result = db.MM_Ctrl_Machine_Alarm_Types.Where(p => p.Alarm_Type_Name == alarmName && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.MM_Ctrl_Machine_Alarm_Types.Where(p => p.Alarm_Type_Name == alarmName && p.Plant_ID == plantId && p.Alarm_Type_ID != alarmId);
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
    public class MetaMachineAlarmTypes
    {
    }
}