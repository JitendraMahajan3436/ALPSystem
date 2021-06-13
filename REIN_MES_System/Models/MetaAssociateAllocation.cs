using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{

    /* Class Name                 : RS_AM_Operator_Station_Allocation
    *  Description                : Override the RS_AM_Operator_Station_Allocation class with MetaAssociateAllocation class to define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    [MetadataType(typeof(MetaAssociateAllocation))]
    public partial class RS_AM_Operator_Station_Allocation
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public String Token_ID { get; set; }
        public int Operator_ID { get; set; }


        public bool IsNosameOperatorToSameStationExists(decimal operatorId, decimal stationid, decimal shiftid)
        {
            try
            {
                // IQueryable<RS_AM_Shop_Manager_Mapping> result;
                //if (empId == 0)
                //{
                //    result = db.RS_AM_Shop_Manager_Mapping.Where(p => p.Employee_ID == empId);
                //}
                // else
                // {
                bool result = db.RS_AM_Operator_Station_Allocation.Where(p => p.Employee_ID == operatorId && p.Shift_ID == shiftid).Count() > 0;
                //}

                if (result == true)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*            Method Name                :IsOperatorToOneStationExists
         *           Description                :To assign a operator to station only once n the same shift
         *           Author, Timestamp          :Jitendra Mahajan
         *           Input parameter            :decimal operatorId, decimal stationid, decimal shiftid
         *           Return Type                :bool
         *           Revision                   :
         */
        public bool IsOperatorToOneStationExists(decimal operatorId, decimal stationid, decimal shiftid, decimal shopId, DateTime allocationDate)
        {
            try
            {
                bool result = db.RS_AM_Operator_Station_Allocation.Where(p => p.Employee_ID == operatorId && p.Shift_ID == shiftid &&
                p.Station_ID == stationid && p.Shop_ID == shopId &&
                DbFunctions.TruncateTime(p.Allocation_Date) ==
                DbFunctions.TruncateTime(allocationDate)).Count() > 0;
                if (result == true)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /*            Method Name                :IsOperatorToStationAllocated
        *           Description                :To check a operator is allocated to station or not
        *           Author, Timestamp          :Jitendra Mahajan
        *           Input parameter            :decimal operatorId, decimal stationid
        *           Return Type                :bool
        *           Revision                   :
        */
        public bool IsOperatorToStationAllocated(String operatorTokenId, decimal stationId, decimal shiftId)
        {
            try
            {
                var userStationAllocation = from stationAllocation in db.RS_AM_Operator_Station_Allocation
                                            where stationAllocation.Station_ID == stationId && stationAllocation.Shift_ID == shiftId && (from employee in db.RS_Employee where employee.Employee_No == operatorTokenId select employee.Employee_ID).Contains(stationAllocation.Employee_ID)

                                            select stationAllocation;
                //bool result = db.RS_AM_Operator_Station_Allocation.Where(p => p.Employee_N == operatorId && p.Station_ID == stationid).Count() > 0;

                if (userStationAllocation.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool isEmployeePresent(String employee_no)
        {
            try
            {
                DateTime dt = DateTime.Today;
                var userStationAllocation = from attendance in db.RS_User_Attendance_Sheet
                                            where attendance.Employee_No == employee_no && attendance.Entry_Date == dt
                                            select attendance;
                //bool result = db.RS_AM_Operator_Station_Allocation.Where(p => p.Employee_N == operatorId && p.Station_ID == stationid).Count() > 0;

                if (userStationAllocation.Count() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public RS_AM_User_Sessions[] isUserLoggedInOtherStation(decimal employee_id, int stationID)
        {
            try
            {
                DateTime dt = DateTime.Today;

                //String query = "delete from " + purgeMaster[j].Table_Name + " where Is_Purgeable=1";

                //var userLogin =db.Database.SqlQuery<string>(query).ToArray();
                //db.Database.ExecuteSqlCommand(query);


                RS_AM_User_Sessions[] userLogin = (from userSession in db.RS_AM_User_Sessions
                                                   where userSession.Employee_ID == employee_id && userSession.Login_Date >= dt && userSession.Logout_Time == null && userSession.Station_ID != stationID
                                                   select userSession).OrderByDescending(p => p.Login_Date).ToArray();
                //bool result = db.RS_AM_Operator_Station_Allocation.Where(p => p.Employee_N == operatorId && p.Station_ID == stationid).Count() > 0;
                return userLogin;

                //if (userLogin.Count() > 0)
                //    return true;
                //else
                //    return false;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }


    /* Class Name                 : MetaAssociateAllocation
    *  Description                : To define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class MetaAssociateAllocation
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceShop)), ErrorMessageResourceName = "Shop_Error_Shop_Name_Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceLine)), ErrorMessageResourceName = "Line_Error_Name_Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceStation)), ErrorMessageResourceName = "Station_Error_Name_Required")]
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Station_ID { get; set; }


        [Required(ErrorMessageResourceType = (typeof(ResourceOperatorAllocation)), ErrorMessageResourceName = "Shift_Error_Name_Required")]
        [Display(Name = "Shift_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shift_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceOperatorAllocation)), ErrorMessageResourceName = "Operator_Error_Operator_Name_Required")]
        [Display(Name = "Operator_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Employee_ID { get; set; }

        //[Required(ErrorMessage = "Please Enter Skill Set")]
        [Display(Name = "Please Enter Skill Set")]
        public decimal Skill_ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }

    }
}