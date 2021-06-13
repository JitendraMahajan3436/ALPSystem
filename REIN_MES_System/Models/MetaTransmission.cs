using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{



    public class Transmission
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
        public int Action1 { get; set; }
        public string Operation1 { get; set; }

        public Transmission(string Operation1, DateTime DateandTime, string ShopName, string Feeder, decimal Factor, double Consumption, int Action1, int Id)
        {
            this.Operation1 = Operation1;
            this.DateTime = DateandTime;
            this.Shop_Name = ShopName;
            this.Feeder = Feeder;
            this.Factor = Factor;
            this.Consumption = Consumption;
            this.Action1 = Action1;
            this.Id = Id;
        }
    }
}