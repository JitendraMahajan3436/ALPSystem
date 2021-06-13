using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class MTTUFaultAlarmFields
    {
        public decimal Machine_ID { get; set; }
        public decimal Alarm_ID { get; set; }
        public Boolean Status { get; set; }
        public DateTime? Inserted_Date { get; set; }
        public string alarmDate { get; set; }
    }
}