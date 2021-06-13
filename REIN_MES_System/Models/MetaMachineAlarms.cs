using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaMachineAlarms))]
    public partial class MM_Ctrl_Machine_Alarms
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        public int[] selectedAlarms { get; set; }
        General generalObj = new General();
        public bool isAlarmAddedForMachine(int alarmId, int machineId)
        {
            try
            {
                var res = from machineAlarmObj in db.MM_Ctrl_Machine_Alarms
                          where machineAlarmObj.Alarm_ID == alarmId && machineAlarmObj.Machine_ID == Machine_ID
                          select machineAlarmObj;

                if (res.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool removeAddedAlarmsByMachine(int[] alarmIds, int machineId, String hostName, int userId)
        {
            try
            {
                var res = db.MM_Ctrl_Machine_Alarms.Where(p => p.Machine_ID == machineId);
                foreach (var item in res.ToList())
                {
                    int alarmId = Convert.ToInt16(item.Alarm_ID);
                    if (Array.Exists(alarmIds, p => p == alarmId))
                    {

                    }
                    else
                    {


                        db.MM_Ctrl_Machine_Alarms.Remove(item);
                        db.SaveChanges();
                        generalObj.addPurgeDeletedRecords(Convert.ToInt16(item.Plant_ID), "MM_Ctrl_Machine_Alarms", "Machine_Alarm_ID", item.Machine_Alarm_ID.ToString(), hostName, userId);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    public class MetaMachineAlarms
    {
    }
}