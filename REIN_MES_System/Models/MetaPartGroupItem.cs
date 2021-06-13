using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaPartGroupItem))]

    public partial class RS_PartgroupItem
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /*        Method Name                : GetLastPartGroupItemNumber
         *        Description                : GetLastPartGroupItemNumber use to check maximum number
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : Nothing
         *        Return Type                : string 
         *        Revision                   : 1
         */
        public String GetLastPartGroupItemNumber()
        {
            try
            {
                string partGroupItemNumber = "";
                string item = db.RS_PartgroupItem.Max(r => r.PartgroupItem_ID).ToString();

                if (item.ToString() == null)
                {

                    string s = "1";
                    int number = Convert.ToInt32(s);
                    string str = number.ToString();
                    partGroupItemNumber = str;

                }
                else
                {
                    string s = item.ToString();
                    int number = Convert.ToInt32(s);
                    number += 1;
                    partGroupItemNumber = Convert.ToString(number);


                }

                return partGroupItemNumber;
            }
            catch (Exception ex)
            {
                string partGroupItemNumber = "";

                string s = "1";
                int number = Convert.ToInt32(s);
                string str = number.ToString();
                partGroupItemNumber = str;
                return partGroupItemNumber;
            }
        }

    }


    public class MetaPartGroupItem
    {
    }
}