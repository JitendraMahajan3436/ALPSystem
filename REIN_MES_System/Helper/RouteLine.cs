using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Helper
{
    public class RouteLine
    {
        public String currentStation { get; set; }
        public String nextStation { get; set; }
        public String isStartStation { get; set; }
        public String isEndStation { get; set; }
    }
}