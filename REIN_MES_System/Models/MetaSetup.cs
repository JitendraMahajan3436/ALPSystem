using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaSetup))]
    public partial class RS_Setup
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public bool isSetupExists(String setupName,int shopId, int lineId, int setupId)
        {
            try
            {
                IQueryable<RS_Setup> result;
                if (setupId == 0)
                {
                    result = db.RS_Setup.Where(p => p.Setup_Name == setupName && p.Shop_ID == shopId && p.Line_ID == lineId);
                }
                else
                {
                    result = db.RS_Setup.Where(p => p.Setup_Name == setupName && p.Shop_ID == shopId && p.Line_ID == lineId &&  p.Setup_ID != setupId);
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
    }

    public class MetaSetup
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        
        public string Setup_Name { get; set; }

        public string Inserted_Host { get; set; }
        public Nullable<decimal> Inserted_User_ID { get; set; }
        public Nullable<System.DateTime> Inserted_Date { get; set; }
        public string Updated_Host { get; set; }
        public Nullable<decimal> Updated_User_ID { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    }
}