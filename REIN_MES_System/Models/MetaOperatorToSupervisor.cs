using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Assign_OperatorToSupervisor
   *  Description                : Override the RS_Assign_OperatorToSupervisor class with MetaAssociateAllocation class to define additional attributes, validation and properties
   *  Author, Timestamp          : Jitendra Mahajan      
   */
    [MetadataType(typeof(MetaOperatorToSupervisor))]
    public partial class RS_Assign_OperatorToSupervisor
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public int[] selectedOperators { get; set; }
        public Boolean Type { get; set; }
        General generalObj = new General();
        /* Method Name                : deleteOperator
        *  Description                : Method is used to delete the operators added to operatorlist
        *  Author, Timestamp          : Jitendra Mahajan
        *  Input parameter            : stationId, plantId, shopId, lineId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool deleteOperator(int shopId, int lineId, int supervisorId, String hostName, int userId) //int stationId, int plantId,
        {
            try
            {
                var operatorList = db.RS_Assign_OperatorToSupervisor.Where(p => p.Supervisor_ID == supervisorId && p.Shop_ID == shopId && p.Line_ID == lineId);//&& p.Plant_ID == plantId
                foreach (var item in operatorList.ToList())
                {
                    db.RS_Assign_OperatorToSupervisor.Remove(item);
                    db.SaveChanges();

                    generalObj.addPurgeDeletedRecords(Convert.ToInt16(item.Plant_ID), "RS_Assign_OperatorToSupervisor", "OTS_ID", item.OTS_ID.ToString(), hostName, userId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

    /* Class Name                 : MetaOperatorToSupervisor
    *  Description                : To define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class MetaOperatorToSupervisor
    {
        //public decimal OTS_ID { get; set; }
        //[Required(ErrorMessageResourceType = (typeof(ResourcePlant)), ErrorMessageResourceName = "Supervisor_Error_Supervisor_Name_Required")]
        //[Display(Name = "Supervisor_Label_Supervisor_Name", ResourceType = typeof(ResourcePlant))]
        //public decimal Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Supervisor_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Supervisor_ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }
        //public decimal AssignedOperator_ID { get; set; }
        //public bool Is_Deleted { get; set; }
        //public Nullable<bool> Is_Transfered { get; set; }
        //public Nullable<bool> Is_Purgeable { get; set; }
        //public decimal Inserted_User_ID { get; set; }
        //public System.DateTime Inserted_Date { get; set; }
        //public string Updated_User_ID { get; set; }
        //public Nullable<System.DateTime> Updated_Date { get; set; }

        //public virtual RS_Employee RS_Employee { get; set; }
        //public virtual RS_Lines RS_Lines { get; set; }
        //public virtual RS_Plants RS_Plants { get; set; }
        //public virtual RS_Shops RS_Shops { get; set; }
    }
}