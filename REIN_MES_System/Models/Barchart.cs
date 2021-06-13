using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace REIN_MES_System.Models
{


    [DataContract]
    public class Barchart
    {

        public string Shopname { get; set; }
        public Double Consumption { get; set; }

        public int status { get; set; }

        public string function { get; set; }
        public Barchart(string label, double y, string function, int status)
        {
            this.Label = label;
            this.Y = y;
            this.function = function;
            this.status = status;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }

    public class Processchart
    {

        public string Shopname { get; set; }
        public Double Consumption { get; set; }

        public int ShopId { get; set; }
        public Processchart(string label, double y, int function)
        {
            this.Label = label;
            this.Y = y;
            this.ShopId = function;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }

    public class Areachart
    {

        public string Areaname { get; set; }
        public Double Consumption { get; set; }

        public int AreaId { get; set; }
        public Areachart(string label, double y, int function)
        {
            this.Label = label;
            this.Y = y;
            this.AreaId = function;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}