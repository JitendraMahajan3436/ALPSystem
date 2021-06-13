using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class Feederlist
    {
      
        
       
     
        public decimal Shop { get; set; }
        public decimal feeder { get; set; }
        public decimal Category { get; set; }
        public decimal Area { get; set; }
        public decimal Meter { get; set; }
        public Nullable<bool> sharedfeeder { get; set; }
        public Nullable<bool> Manualfeeder { get; set; }
        public decimal Prameter { get; set; }
        public decimal Unit { get; set; }
        public decimal TagIndex { get; set; }
        public string TagName { get; set; }
        public bool Active { get; set; }

        public decimal Load_ID { get; set; }
        public float RatedLoad { get; set; }

        public float Ideal { get; set; }

    }

    public class CBMlist
    {
        public decimal Shop { get; set; }
        public decimal Area { get; set; }
        public decimal Machine { get; set; }
        public decimal CBM { get; set; }
    }
}