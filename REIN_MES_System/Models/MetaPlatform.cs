using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaPlatform))]
    public partial class RS_Platform
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        /*        Method Name                : IsPlatformExists
         *        Description                : IsPlatformExists use to check Platform already exit or not
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : plantId,
         *        Return Type                : bool 
         *        Revision                   : 1
         */
        public bool IsPlatformExists(int plantId, string partgroupDesc, int lineId)
        {

            try
            {
                 IQueryable<RS_Platform> result;
                 if (lineId == 0)
                 {
                     result = db.RS_Platform.Where(p => p.Plant_ID == plantId && p.Platform_Description==partgroupDesc);
                 }
                 else
                 {
                     result = db.RS_Platform.Where(p => p.Plant_ID == plantId && p.Platform_Description == partgroupDesc);
                 }

                 if (result.Count() > 0)
                     return true;
                 else
                     return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        
        }


        /*        Method Name                : GetLastPlatformNumber
         *        Description                : GetLastPlatformNumber use to check maximum number
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : plantId
         *        Return Type                : string 
         *        Revision                   : 1
         */

        public String GetLastPlatformNumber(int plantId)
        {
            try
            {
                string platformNumber = "";
                var item = db.RS_Platform.Where(c => c.Plant_ID == plantId).Select(c => new { c.Platform_Id }).ToList();

                if (item.Count > 0)
                {
                    var max_orderno = db.RS_Platform.Max(r => r.Platform_Id);
                    string s = max_orderno.ToString();
                    string p = s.Remove(0, 1);
                    int number = Convert.ToInt32(p);
                    number += 1;
                    platformNumber = number.ToString("D4");
                    platformNumber = "P" + platformNumber;
                }
                else
                {
                    string s = "P0001";
                    int number = Convert.ToInt32(s);
                    string str = number.ToString("D5");
                    platformNumber = str;
                }

                return platformNumber;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }

    public class MetaPlatform
    {
    }
}