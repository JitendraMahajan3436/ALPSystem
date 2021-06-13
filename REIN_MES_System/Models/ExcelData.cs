using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ExcelData
    {
        public string datetime { get; set; }

        public Double cmbdata { get; set; }

        public ExcelData(string Datetime, Double Cmbdata)
        {
            this.datetime = Datetime;
            this.cmbdata = Cmbdata;

        }
    }

    public class TBMExcelData
    {
        public string datetime { get; set; }

        public int desginated_data { get; set; }
        public int consumed_data { get; set; }

        public string User { get; set; }

        public TBMExcelData(string Datetime, int desginated_data, int consumed_data, string user)
        {
            this.datetime = Datetime;
            this.desginated_data = desginated_data;
            this.consumed_data = consumed_data;
            this.User = user;


        }
    }

    public class MinorStoppagesData
    {
        public DateTime datetime { get; set; }
        public bool Is_Faulty { get; set; }
        public int Machine { get; set; }

        public MinorStoppagesData(DateTime date, bool stoppage, int machine)
        {
            this.datetime = date;
            this.Is_Faulty = stoppage;
            this.Machine = machine;
        }
    }
    public class CycleStoppageData
    {
        public string CyclestepName { get; set; }
        public int CycleCount { get; set; }

        public int CycleID { get; set; }

        public int MachineID { get; set; }

        public CycleStoppageData(string CyclestepName, int CycleCount, int CycleID, int machine)
        {
            this.CyclestepName = CyclestepName;
            this.CycleCount = CycleCount;
            this.CycleID = CycleID;
            this.MachineID = machine;
        }
    }

    public class MinorStoppagesHistorData
    {
        public string Machine { get; set; }
        public String Cycle { get; set; }

        public DateTime Date { get; set; }

        public string Alaram { get; set; }

        public MinorStoppagesHistorData(string machine, string cycle, DateTime date, string alaram)
        {
            this.Machine = machine;
            this.Cycle = cycle;
            this.Date = date;
            this.Alaram = alaram;
        }
    }



    public class TBMResetHistoryData
    {

        public DateTime Date { get; set; }

        public int Designated_Life { get; set; }

        public int Consumed_Life { get; set; }

        public string Remarks { get; set; }

        public string ReplacedBy { get; set; }

        public TBMResetHistoryData(DateTime Date, int DesgLife, int ConsLife, string Remarks, string ReplacedBy)
        {
            this.Date = Date;
            this.Designated_Life = DesgLife;
            this.Consumed_Life = ConsLife;
            this.Remarks = Remarks;
            this.ReplacedBy = ReplacedBy;
        }
    }
}