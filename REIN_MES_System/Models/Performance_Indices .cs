using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class Performance_Indices
    {
        public string Shopname { get; set; }
        public Double ConsumptionperEngine { get; set; }
        public Double BestConsumption { get; set; }
        public Double AvgConsumption { get; set; }

        public Double TotalConsumtion { get; set; }

        public int Production { get; set; }
        public Double Efficiency { get; set; }
        public int ShopId { get; set; }

        public bool Generation { get; set; }

        public bool Is_Production { get; set; }

        public Performance_Indices(string Shopname, double Consumption, double Best, double Avg, double Efficiency, double TotalConsumption, int Production, int ShopId, bool Generation, bool Is_Product)
        {
            this.Shopname = Shopname;
            this.ConsumptionperEngine = Consumption;
            this.BestConsumption = Best;
            this.AvgConsumption = Avg;
            this.Efficiency = Efficiency;
            this.TotalConsumtion = TotalConsumption;
            this.Production = Production;
            this.ShopId = ShopId;
            this.Generation = Generation;
            this.Is_Production = Is_Product;

        }
    }
}