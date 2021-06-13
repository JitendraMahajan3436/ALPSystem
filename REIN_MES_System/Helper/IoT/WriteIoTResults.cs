using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Helper.IoT
{
    public class WriteIoTResults
    {
        public WriteResults[] writeResults { get; set; }
    }

    public class WriteResults
    {
        public String id { get; set; }
        public Boolean s { get; set; }
        public String r { get; set; }
    }
}