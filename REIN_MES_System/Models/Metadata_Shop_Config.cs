using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class Metadata_Shop_Config
    {

       
        public Nullable<bool> TagBoolean { get; set; }     
        public decimal Shop_ID { get; set; }
        public string Shop_Name { get; set; }



    }
    public class Email_Alert_data
    {
        public decimal Emp_ID { get; set; }
        public string Emp_Name { get; set; }

    }
}