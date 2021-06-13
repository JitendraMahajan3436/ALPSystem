using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ExtensionClass
    {
        public string plant_id { get; set; }
        public string line_id { get; set; }
        public string shop_id { get; set; }
        public string station_id { get; set; }
        public Nullable<decimal> user_id { get; set; }
    }
}