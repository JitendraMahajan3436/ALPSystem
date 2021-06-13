using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class Parameterview
    {
        public Nullable<System.DateTime> InsertedDate { get; set; }
        public Nullable<double> ParameterValue { get; set; }

        public Parameterview(DateTime InsertedDate, double ParameterValue)
        {
            this.InsertedDate = InsertedDate;
            this.ParameterValue = ParameterValue;
        }
    }
}