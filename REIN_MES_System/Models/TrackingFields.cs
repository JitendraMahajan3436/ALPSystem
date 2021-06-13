using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class TrackingFields
    {
        public decimal Plant_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Station_ID { get; set; }
        public decimal Marriage_Station_ID { get; set; }
        public decimal Next_Station_ID { get; set; }
        public decimal Prev_Station_ID { get; set; }
        public decimal? LineStopStation_ID { get; set; }

        public string SerialNo { get; set; }
        public string Line_Name { get; set; }
        public decimal Tact_Time_Sec { get; set; }
        public Int64 Current_Stoppage_Seconds { get; set; }

        public Boolean Is_Buffer { get; set; }
        public Boolean isPLC { get; set; }
        public Boolean Line_Stop { get; set; }
        public Boolean Emergency_Call { get; set; }
        public Boolean Heart_Bit { get; set; }
        public Boolean isEmptyPitch { get; set; }

        public Boolean Material_Call { get; set; }
        public Boolean Supervisor_Call { get; set; }
        public Boolean Maintenance_Call { get; set; }
        public Boolean Is_End_Station { get; set; }
        public Boolean Is_Start_Station { get; set; }
        public Boolean isLineStop { get; set; }

        public Nullable<System.DateTime> LineMove_Time { get; set; }
        public Nullable<System.DateTime> LineStart_Time { get; set; }
        public Nullable<System.DateTime> LineStop_Time { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public DateTime todayDate { get; set; }
    }
}