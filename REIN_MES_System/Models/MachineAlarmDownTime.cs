using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class MachineAlarmDownTime
    {
        public decimal Machine_ID { get; set; }
        public Nullable<System.DateTime> DownTime { get; set; }
        public String Type { get; set; }
    }
}