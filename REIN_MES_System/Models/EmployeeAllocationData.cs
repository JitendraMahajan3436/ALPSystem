using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REIN_MES_System.Models
{
    public class EmployeeAllocationData
    {
        public decimal Employee_ID { get; set; }
        public decimal Station_ID { get; set; }
        public string Employee_Name { get; set; }
        public string Employee_No { get; set; }
        public decimal Line_ID { get; set; }
        public string Station_Name { get; set; }
        public decimal Skill_ID { get; set; }
    }
}
