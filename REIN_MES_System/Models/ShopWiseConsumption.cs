using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ShopWiseConsumption
    {
        public decimal ShopsCat_ID { get; set; }
        public int ShopId { get; set; }
        public String ShopName { get; set; }
        public string ShopsCategory_Name { get; set; }
        public double? totalconsumption { get; set; }

        public ShopWiseConsumption(decimal ShopsCat_ID, int ShopId, String ShopName, string ShopsCategory_Name, double totalconsumption)
        {
            this.ShopsCat_ID = ShopsCat_ID;
            this.ShopId = ShopId;
            this.ShopName = ShopName;
            this.ShopsCategory_Name = ShopsCategory_Name;
            this.totalconsumption = totalconsumption;
        }

    }

    public class ShopWiseConsumption1
    {
        public decimal ShopsCat_ID { get; set; }
        public int ShopId { get; set; }
        public String ShopName { get; set; }
        public string ShopsCategory_Name { get; set; }
        public double? totalconsumption { get; set; }
    }



    public class ShopWiseConsumption2
    {
        public Nullable<bool> TagBoolean { get; set; }
        public int Shop_ID { get; set; }

        public string Shop_Name { get; set; }
    }

    public class Summary
    {
        public double totalpower { get; set; }
        public double totalavg { get; set; }
        public double totalavgsec { get; set; }
        public int totalprod { get; set; }

        public double best { get; set; }
        public double bestpower { get; set; }
        public int bestprod { get; set; }

        public double maxpower { get; set; }

        public string maxdate { get; set; }
        public string bestdate { get; set; }
        public Summary(double totalpower, double totalavg, double totalavgsec, int totalprod, double best, double bestpower, int bestprod, double maxpower, string bestdate, string maxdate)
        {
            this.totalpower = totalpower;
            this.totalavg = totalavg;
            this.totalavgsec = totalavgsec;
            this.totalprod = totalprod;
            this.best = best;
            this.bestpower = bestpower;
            this.bestprod = bestprod;
            this.maxpower = maxpower;
            this.bestdate = bestdate;
            this.maxdate = maxdate;

        }

    }

    public class ShiftPlantData
    {

        public double Consumption { get; set; }
        public double TotalConsumption { get; set; }
        public int Production { get; set; }

        public double Best { get; set; }
        public string date { get; set; }
        public ShiftPlantData(double Consumption, double TotalConsumption, int Production, double Best, string date)
        {
            this.Consumption = Consumption;
            this.TotalConsumption = TotalConsumption;
            this.Production = Production;
            this.Best = Best;
            this.date = date;
        }

    }


}