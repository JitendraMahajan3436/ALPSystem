using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    /* Class Name                  : RS_MT_Machines
    *  Description                : Override the RS_MT_Machines class with MetaMachines class to define additional attributes, validation and properties
    *  Author, Timestamp          : Vijaykuumar Kagne       
    */
    [MetadataType(typeof(MetaMachines))]
    public partial class RS_MT_Machines
    {
        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();


        //MVML_MGMTEntities db = new MVML_MGMTEntities();
        HttpPostedFileBase files { get; set; }

        public bool isMachineExists(String machineNumber, decimal machineid, decimal plantid)
        {
            try
            {
                IQueryable<RS_MT_Machines> result;
                if (machineid == 0)
                {
                    result = db.RS_MT_Machines.Where(p => p.Machine_Number == machineNumber && p.Plant_ID == plantid);
                }
                else
                {
                    result = db.RS_MT_Machines.Where(p => p.Machine_Number == machineNumber && p.Machine_ID != machineid && p.Plant_ID == plantid);
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

    /* Class Name                  : MetaMachines
    *  Description                : MetaMachines class to define additional attributes, validation and properties
    *  Author, Timestamp          : Vijaykuumar Kagne       
    */
    public class MetaMachines
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine", ResourceType = typeof(ResourceDisplayName))]
        public int Machine_ID { get; set; }
        
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Name", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemoteMachineName", "RemoteValidation", AdditionalFields = "Machine_ID,Plant_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        public string Machine_Name { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Remote("RemoteMachineNumber", "RemoteValidation", AdditionalFields = "Machine_ID,Plant_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        [Display(Name = "Machine_No", ResourceType = typeof(ResourceDisplayName))]
        public string Machine_Number { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> Station_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Category", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> Machine_Category_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Manufacturig_Year", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<System.DateTime> Manufaturing_Year { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Desc", ResourceType = typeof(ResourceDisplayName))]
        public string Machine_Description { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Type_Name", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> M_Type_ID { get; set; }

        [Display(Name = "FMEA_Document", ResourceType = typeof(ResourceDisplayName))]
        public string FMEA_Document { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(ResourceDisplayName))]
        public bool IsActive { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Make", ResourceType = typeof(ResourceDisplayName))]
        public string Machine_Make { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Model", ResourceType = typeof(ResourceDisplayName))]
        public string Machine_Model { get; set; }


        [Display(Name = "IsCBM", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> IsCBM { get; set; }

        [Display(Name = "IsTBM", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> IsTBM { get; set; }

        [Display(Name = "IsMinorStoppage", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> IsMinorStoppage { get; set; }

        [Display(Name = "CBM_Matrix1", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> CBM_Matrix1 { get; set; }

        [Display(Name = "CBM_Matrix2", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> CBM_Matrix2 { get; set; }

        [Display(Name = "CBM_Matrix3", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> CBM_Matrix3 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string CBM_Email1 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string CBM_Email2 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string CBM_Email3 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> CBM_SMS1 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> CBM_SMS2 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> CBM_SMS3 { get; set; }

        [Display(Name = "TBM_Matrix1", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> TBM_Matrix1 { get; set; }

        [Display(Name = "TBM_Matrix2", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> TBM_Matrix2 { get; set; }

        [Display(Name = "TBM_Matrix3", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> TBM_Matrix3 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string TBM_Email1 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string TBM_Email2 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string TBM_Email3 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> TBM_SMS1 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> TBM_SMS2 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> TBM_SMS3 { get; set; }

        [Display(Name = "MS_Matrix1", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> MS_Matrix1 { get; set; }

        [Display(Name = "MS_Matrix2", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> MS_Matrix2 { get; set; }

        [Display(Name = "MS_Matrix3", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> MS_Matrix3 { get; set; }

        [Display(Name = "MS_Occurences", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> MS_Occurences1 { get; set; }

        [Display(Name = "MS_Occurences", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> MS_Occurences2 { get; set; }

        [Display(Name = "MS_Occurences", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> MS_Occurences3 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string MS_Email1 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string MS_Email2 { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "Email_ID", ResourceType = typeof(ResourceDisplayName))]
        public string MS_Email3 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> MS_SMS1 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> MS_SMS2 { get; set; }

        [Display(Name = "Mobile_No", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<decimal> MS_SMS3 { get; set; }

        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "AlphaNumeric")]
        [Display(Name = "M_Asset_Number", ResourceType = typeof(ResourceDisplayName))]
        public string M_Asset_Number { get; set; }
    }
}