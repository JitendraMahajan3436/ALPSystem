using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class CummulativeFieldsOrderPlanning
    {
        public List<REIN_MES_System.Models.CummulativeFields> orderScheduledList { get; set; }
        public List<REIN_MES_System.Models.CummulativeFields> orderAvailableList { get; set; }


    }
}