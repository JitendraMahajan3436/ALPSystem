using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{

    /* Class Name                 : RS_Employee
    *  Description                : Override the RS_Employee class with MetaAddUsers class to define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    [MetadataType(typeof(MetaAddUsers))]
    public partial class RS_Employee
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public decimal Line_ID { get; set; }
        public decimal Shop_ID { get; set; }
        public int[] Lines_ID { get; set; }
        public int[] Shops_ID { get; set; }
        public int[] Plants_ID { get; set; }
        public String Birth_Date { get; set; }
        public String Date { get; set; }
        public String Month { get; set; }


        /*          Method Name                :IsEmpNoExists
        *           Description                :To accept only one employee number ,employee number must not get repeated
        *           Author, Timestamp          :Jitendra Mahajan
        *           Input parameter            :String empno, int plantId
        *           Return Type                :bool
        *           Revision                   :
        */
        public bool IsEmpNoExists(string empno, decimal plantId)
        {
            try
            {
                if (db.RS_Employee.Where(p => p.Employee_No == empno && p.Plant_ID == plantId && p.Is_Deleted == null).Count() > 0)
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


        /*          Method Name                :IsEmpNoExistsEdit
       *           Description                :To accept only one employee number ,employee number must not get repeated apart from edited one
       *           Author, Timestamp          :Jitendra Mahajan
       *           Input parameter            :String empno, int plantId
       *           Return Type                :bool
       *           Revision                   :
       */
        public bool IsEmpNoExistsEdit(string empno, decimal empId, decimal Plants_ID)
        {
            try
            {
                bool isEmployeeNoExist = db.RS_Employee.Any(p => p.Employee_No == empno && p.Employee_ID != empId && p.Plant_ID == Plants_ID && p.Is_Deleted == null);
                if (isEmployeeNoExist)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        /*           Method Name                :IsEmailExists
         *           Description                :To accept only one Email Id ,Email Id must not get repeated
         *           Author, Timestamp          :Jitendra Mahajan
         *           Input parameter            :String email, int plantId
         *           Return Type                :bool
         *           Revision                   :
         */
        public bool IsEmailExists(string email, decimal plantId)
        {
            try
            {
                if (db.RS_Employee.Where(p => p.Email_Address == email && p.Plant_ID == plantId && p.Is_Deleted == null && p.Email_Address != null).Count() > 0)
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

        /*           Method Name                :IsEmailExistsEdit
         *           Description                :To accept only one Email Id ,Email Id must not get repeated apart from edited one
         *           Author, Timestamp          :Jitendra Mahajan
         *           Input parameter            :String email, int plantId
         *           Return Type                :bool
         *           Revision                   :
         */
        public bool IsEmailExistsEdit(string email, decimal empId, decimal Plants_ID)
        {
            try
            {
                bool isEmailExist = db.RS_Employee.Any(p => p.Email_Address == email && p.Employee_ID != empId && p.Plant_ID == Plants_ID && p.Is_Deleted == null && p.Email_Address != null);
                if (isEmailExist)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsEmployeeTokenExists(String tokenNumber)
        {
            try
            {
                var res = db.RS_Employee.Where(p => p.Employee_No == tokenNumber);
                if (res.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }


    /* Class Name                 : MetaAddUsers
    *  Description                : To define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan      
    */
    public class MetaAddUsers
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Employee_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Employee_Name { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Employee_No", ResourceType = typeof(ResourceDisplayName))]
        public string Employee_No { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_Address", ResourceType = typeof(ResourceDisplayName))]
        public string Email_Address { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "DOB_Name", ResourceType = typeof(ResourceDisplayName))]
        public string DOB { get; set; }

        [DisplayFormat(DataFormatString = "{0:F0}")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        [Display(Name = "Contact_No", ResourceType = typeof(ResourceDisplayName))]
        public int Contact_No { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        //[Display(Name = "Gender", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Gender { get; set; }

        //[Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        //public Nullable<decimal> Plant_ID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Inserted_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Updated_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Updated_Date { get; set; }

    }


    public class PresentAbsentEmployee
    {
        public decimal Employee_ID { get; set; }
        public string Employee_Token { get; set; }
        public string Employee_Name { get; set; }
        public decimal Station_ID { get; set; }
        public string Absent { get; set; }
        public string Employee_No { get; set; }
        public decimal Skill_ID { get; set; }
        public int Is_Present { get; set; }

    }
}