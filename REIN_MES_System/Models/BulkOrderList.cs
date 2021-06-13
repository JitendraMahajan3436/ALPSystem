using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class BulkOrderList
    {
        public string part_no { get; set; }
        public string part_description { get; set; }

        public string Color { get; set; }
        public int? planned_qty { get; set; }
        public int? started_qty { get; set; }
        public int? pending_qty { get; set; }
        public string order_no { get; set; }
        public decimal row_id { get; set; }

    }
}