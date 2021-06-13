using System;
using System.Collections.Generic;
using REIN_MES_System.Models;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Helper
{
    public class FDSession
    {
        public int userId { get; set; }
        public int userRoleId { get; set; }
        public int plantId { get; set; }
        public int shopId { get; set; }
        public int lineId { get; set; }
        public int stationId { get; set; }
        public string stationName { get; set; }
        public string userHost { get; set; }

        public bool isSuperAdmin { get; set; }
        public bool isPlantAdmin { get; set; }
        public bool isOrderStartStation { get; set; }


        public string userName { get; set; }
        public string plantName { get; set; }
        public DateTime insertedDate { get; set; }

        public IEnumerable<RS_Menus> menuObj { get; set; }
        public IEnumerable<RS_Roles> rolesObj { get; set; }
    }
}