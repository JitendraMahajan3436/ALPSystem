using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class AllocationDashboard
    {
        public decimal Employee_ID { get; set; }
        public string Employee_Token { get; set; }
        public string Employee_Name { get; set; }

        public decimal Station_ID { get; set; }
        public decimal Line_ID { get; set; }
        public decimal Shop_ID { get; set; }

        public string Shop_Name { get; set; }
        public string Line_Name { get; set; }
        public string Station_Name { get; set; }

    }
}