using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class ExcelAddUsers
    {
        [Required]
        public int Plant_ID { get; set; }

        
        //[Required]
        //public int Shop_ID { get; set; }

        //[Required]
        //public int Line_ID { get; set; }

        [Required]
        public HttpPostedFileBase Excel_File { get; set; }
    }

    public class AddUsersRecords
    {
        public String token { get; set; }
        public String name { get; set; }
        //public String email { get; set; }
        public String mobile { get; set; }
        public String dob { get; set; }
        public String gender { get; set; }
        public String addUseError { get; set; }
    }
}