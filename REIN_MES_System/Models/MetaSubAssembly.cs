using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaSubAssembly))]
    public partial class RS_Major_Sub_Assembly
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public bool IsSubAssemblyExists(string assemblyName, decimal Id)
        {
            try
            {
                if(Id == 0)
                {
                    if (db.RS_Major_Sub_Assembly.Where(p => p.Sub_Assembly_Name == assemblyName.ToLower().Trim()).Count() > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    bool isEmployeeNoExist = db.RS_Major_Sub_Assembly.Any(p => p.Sub_Assembly_Name == assemblyName.ToLower().Trim() && p.Sub_Assembly_ID != Id);
                    if (isEmployeeNoExist)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    public class MetaSubAssembly
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Sub_Assembly_Name { get; set; }


        public string Inserted_Host { get; set; }
        public decimal Inserted_User_ID { get; set; }
        public System.DateTime Inserted_Date { get; set; }
    }
}