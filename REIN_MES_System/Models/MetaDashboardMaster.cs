using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaDashboardMaster))]
    
    public partial class RS_Dashboard_Master
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public bool IsIPAddressAgainstShopExist(decimal shopId, decimal shiftId,string hostNAme)
        {
            var result = db.RS_Dashboard_Master.Where(m => m.Host_Name.ToLower() == hostNAme.ToLower()).ToList();

            if (result.Count > 0 && result[0].Shop_ID != shopId)
            {  
                return true;
            }
                
            else
                return false;
        }

        public bool IsIPAddressAgainstShopEditExist(decimal dashId,decimal shopId, decimal shiftId, string hostNAme)
        {
            var exist = db.RS_Dashboard_Master.Where(m => m.Host_Name.ToLower() == hostNAme.ToLower() && m.Dash_ID == dashId).ToList();
            if (exist.Count > 0 && exist[0].Shop_ID != shopId)
                return true;
            else
                return false;

        }

        public bool IsDuplicate(decimal lineId, decimal shiftId, string hostNAme)
        {
            var exist = db.RS_Dashboard_Master.Where(m => m.Host_Name.ToLower() == hostNAme.ToLower() && m.Line_ID==lineId && m.Shift_ID==shiftId).ToList();

            if (exist.Count == 0)
                return true;

            else
            {
                    return false;
            }

        }

        public bool IsEditDuplicate(decimal dashId,decimal lineId, decimal shiftId, string hostNAme)
        {
            var exist = db.RS_Dashboard_Master.Where(m => m.Host_Name.ToLower() == hostNAme.ToLower() && m.Line_ID == lineId && m.Shift_ID == shiftId && m.Dash_ID == dashId).ToList();

            if (exist.Count > 0)
                return true;

            else
            {
                return false;
            }

        }
    }
    public class MetaDashboardMaster
    {
    }

    public class getlist
    {
        public decimal Employee_ID { get; set; }
        public string Employee_No { get; set; }
        public string Employee_Name { get; set; }
        public byte[] Image_Content { get; set; }
        public string Line_Name { get; set; }
        public decimal Station_ID { get; set; }
        public string Absent { get; set; }
        public string Station_Name { get; set; }
        public decimal Line_ID { get; set; }

        public bool? IsOJT { get; set; }
            
        public string Station_Type_ID { get; set; }
    }
}