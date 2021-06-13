using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class TransmissionFactor
    {
        public Nullable<decimal> Plant_ID { get; set; }
        public Nullable<decimal> Shop_ID { get; set; }
        public Nullable<decimal> Factor { get; set; }

        public string Shop_Name { get; set; }
        public string Feeder { get; set; }
        public Nullable<int> Opertion { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<int> TagIndex { get; set; }
        public Nullable<double> Consumption { get; set; }
        public Nullable<bool> Action { get; set; }
        public Nullable<int> Id { get; set; }
        public int   Action1 { get; set; }
        public string Operation1 { get; set; }

       
    }

    

}