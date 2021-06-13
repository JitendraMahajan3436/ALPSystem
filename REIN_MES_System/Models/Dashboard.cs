using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REIN_MES_System.Models;

namespace REIN_MES_System.Models
{
    public class Dashboard
    {
        public List<MM_MT_Time_Based_Maintenance> lst_TBM { get; set; }
        public List<RS_MT_Preventive_Maintenance> lst_PM { get; set; }
        public List<MM_MT_Conditional_Based_Maintenance> lst_CBM { get; set; }
        public List<RS_MT_Calibration> lst_Calibration { get; set; }
        //public List<RS_MT_Clita> lst_MachineClita { get; set; }
        public List<MM_Station_Based_Clita> lst_StationClita { get; set; }
    }
}