using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Employee
   *  Description                : Override the RS_Employee class with MetaAddUsers class to define additional attributes, validation and properties
   *  Author, Timestamp          : Jitendra Mahajan      
   */
    [MetadataType(typeof(MetaEmployeeSkill))]
    public partial class RS_AM_Employee_SkillSet
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /*          Method Name                :IsEmpNoExists
        *           Description                :To accept only one employee number ,employee number must not get repeated
        *           Author, Timestamp          :Jitendra Mahajan
        *           Input parameter            :String empno, int plantId
        *           Return Type                :bool
        *           Revision                   :
        */
        public bool IsEmpNoExists(decimal empno, decimal stationId)
        {
            try
            {
                if (db.RS_AM_Employee_SkillSet.Where(p => p.Employee_ID == empno && p.Station_ID==stationId).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                //IQueryable<RS_Employee> result;
                //if (plantId == 0)
                //{
                //    result = db.RS_Employee.Where(p => p.Email_Address == email);
                //}
                //else 
                //{


                //    result = db.RS_Employee.Where(p => p.Email_Address == email);
                //}

                //if (result.Count() > 0)
                //    return true;
                //else
                //    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }       

    }

    public class MetaEmployeeSkill
    {
    }
}