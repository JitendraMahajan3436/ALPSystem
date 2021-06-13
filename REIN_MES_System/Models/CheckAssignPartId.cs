using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class CheckAssignPartId
    {
        public string PartID { get; set; }

        public string Station { get; set; }
        public CheckAssignPartId(string PartID, string Station)
        {
            this.PartID = PartID;
            this.Station = Station;
         
        }
    }
}