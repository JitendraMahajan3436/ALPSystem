using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{

    /* Class Name                 : RS_Assign_SupervisorToManager
   *  Description                : Override the RS_Assign_SupervisorToManager class with MetaSupervisorToManager class to define additional attributes, validation and properties
   *  Author, Timestamp          : Jitendra Mahajan      
   */
    [MetadataType(typeof(MetaSupervisorToManager))]
    public partial class RS_Assign_SupervisorToManager
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public int[] selectedSupervisors { get; set; }
        public Boolean Type { get; set; }

        General generalObj = new General();

        /* Method Name                : deleteOperator
        *  Description                : Method is used to delete the operators added to supervisorlist
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : plantId, shopId, managerId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool deleteOperator(int shopId, int managerId, String hostName, int userId) //int plantId,
        {
            try
            {
                var supervisorList = db.RS_Assign_SupervisorToManager.Where(p => p.Manager_ID == managerId && p.Shop_ID == shopId);//&& p.Plant_ID == plantId

                foreach (var item in supervisorList.ToList())
                {
                    db.RS_Assign_SupervisorToManager.Remove(item);
                    db.SaveChanges();

                    generalObj.addPurgeDeletedRecords(Convert.ToInt16(item.Plant_ID), "RS_Assign_SupervisorToManager", "STM_ID", item.STM_ID.ToString(), hostName, userId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

    /* Class Name                 : MetaSupervisorToManager
    *  Description                : To define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class MetaSupervisorToManager
    {
       // public decimal Plant_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Supervisor_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Manager_ID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }
    }
}