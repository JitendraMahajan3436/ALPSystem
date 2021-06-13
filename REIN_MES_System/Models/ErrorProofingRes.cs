using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ErrorProofingRes
    {
        public String partNo { get; set; }
        public String serialNo { get; set; }
        public String modelCode { get; set; }
        public bool isOK { get; set; }
        public String errorMessage { get; set; }
        public bool isKitAvailable { get; set; }
    }
}