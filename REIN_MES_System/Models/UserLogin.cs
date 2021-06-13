using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class UserLogin
    {
        [Required]
        public String User_Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String User_Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public String Email_Address { get; set; }
    }
}