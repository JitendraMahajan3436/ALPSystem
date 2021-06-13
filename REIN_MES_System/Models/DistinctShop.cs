using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class DistinctShop
    {
        public decimal Shop_ID { get; set; }
        public string Shop_Name { get; set; }
        public string Link { get; set; }

    }

    public class partList
    {
        public decimal Part_ID { get; set; }
        public string Part_Name { get; set; }
        public int part_Quantity { get; set; }


    }
}