using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    /* Class Name                  : RS_OM_Configuration
     *  Description                : Override the RS_OM_Configuration class with MetaOMConfiguration class to define additional attributes, validation and properties
     *  Author, Timestamp          : Jitendra Mahajan       
     */
    [MetadataType(typeof(MetaOMConfiguration))]
    public partial class RS_OM_Configuration
    {
        public String[] SelectedPartgroup_ID { get; set; }

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /*        Method Name                : IsPartGroupExists
         *        Description                : IsPartGroupExits use to check partgroup already exit or not
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : (integer plantId,integer shopId, integer lineId)
         *        Return Type                : bool 
         *        Revision                   : 1
         */

        public bool IsOMConfigExists(int plantId, int shopId, decimal partgrp_Id, int lineId)
        {
            try
            {
                IQueryable<RS_OM_Configuration> result;
                if (lineId == 0)
                {
                    result = db.RS_OM_Configuration.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Partgroup_ID == partgrp_Id);
                }
                else
                {
                    result = db.RS_OM_Configuration.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Partgroup_ID == partgrp_Id);
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




        /*        Method Name                : GetLastOMConfigNumber
         *        Description                : GetLastOMConfigNumber use to check maximum number
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : plantId, shopId
         *        Return Type                : string 
         *        Revision                   : 1
         */
        public String GetLastOMConfigNumber(int plantId, int shopId)
        {
            try
            {
                string partOMConfig_Number = "";
                var item = db.RS_OM_Configuration.Where(c => c.Plant_ID == plantId).Select(c => new { c.OMconfig_ID }).ToList();

                if (item.Count > 0)
                {
                    var max_configno = db.RS_OM_Configuration.Max(r => r.OMconfig_ID);
                    string s = max_configno.ToString();
                    string p = s.Remove(0, 1);
                    int number = Convert.ToInt32(p);
                    number += 1;
                    partOMConfig_Number = number.ToString("D4");
                    partOMConfig_Number = "C" + partOMConfig_Number;
                }
                else
                {
                    string s = "0001";
                    int number = Convert.ToInt32(s);
                    string str = number.ToString("D4");
                    partOMConfig_Number = "C" + str;
                }

                return partOMConfig_Number;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
    }

    /* Class Name                  : MetaOMConfiguration
     *  Description                : Class is used to define additional information with validation which is used in RS_OM_Configuration class
     *  Author, Timestamp          : Jitendra Mahajan       
     */
    public class MetaOMConfiguration
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "PartGroup_Name", ResourceType = typeof(ResourceDisplayName))]
        public String[] SelectedPartgroup_ID { get; set; }
        public String[] Partgroup_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]

        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Platform_Id { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Configuration_Description", ResourceType = typeof(ResourceDisplayName))]
        public string OMconfig_Desc { get; set; }
        [Display(Name = "Configuration_ID", ResourceType = typeof(ResourceDisplayName))]
        public string OMconfig_ID { get; set; }
        [Display(Name = "Configuration_Name", ResourceType = typeof(ResourceDisplayName))]

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Config_Name { get; set; }

    }

    public class OM_ConfigurationIndex
    {
        public String OMconfig_ID { get; set; }
        public String OMconfig_Desc { get; set; }
        public String Shop_Name { get; set; }
        public string Config_Name1 { get; set; }
    }

}