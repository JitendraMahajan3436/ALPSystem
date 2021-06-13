using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class OJTUpload
    {
        [Required]
        public HttpPostedFileBase Excel_File { get; set; }
    }
    public class OJTUpload1
    {
        public string Plant_Name { get; set; }
        public string Shop_Name { get; set; }
        public string Token_No { get; set; }
        public string Line_Name { get; set; }
        public string Station_Name { get; set; }
        public string Shift_Name { get; set; }
        public string Setup_Name { get; set; }

        public string OJT_Date { get; set; }

        public string SS_Error_Sucess { get; set; }
    }
}