using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class StationCurrentData
    {
        public string serialNo { get; set; }
        public string modelSeries { get; set; }
        public string modelFamily { get; set; }
        public string modelCode { get; set; }
        public string nextSerial1 { get; set; }
        public string nextSerial2 { get; set; }
        public string TakeInTakeOut { get; set; }
        public bool? isStartStation { get; set; }
    }
}