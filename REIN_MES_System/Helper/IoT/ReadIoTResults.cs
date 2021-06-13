using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Helper.IoT
{
    public class ReadIoTResults
    {
        public ReadResults[] readResults { get; set; }
    }

    public class ReadResults
    {
        public String id { get; set; }
        public Boolean s { get; set; }
        public String r { get; set; }
        public String v { get; set; }
        public long t { get; set; }
    }
}