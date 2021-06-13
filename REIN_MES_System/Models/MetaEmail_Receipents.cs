using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ZHB_AD.Models
{
    [MetadataType(typeof(MetaEmail_Receipents))]
    public partial class MM_EMAIL_RECEIPENTS
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        
        public bool IsEmailExistsEdit(string email, int Plant, decimal ID)
        {
            try
            {
                bool isEmailExist;
                if ( ID == 0)
                {
                     isEmailExist = db.MM_EMAIL_RECEIPENTS.Any(p => p.EmailID == email && p.PlantID == Plant);
                }
                else
                {
                     isEmailExist = db.MM_EMAIL_RECEIPENTS.Any(p => p.EmailID == email && p.PlantID == Plant && p.ID != ID);
                }

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
    }
    public class MetaEmail_Receipents
    {
       [Required]
        public string Name { get; set; }


        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        public string EmailID { get; set; }
        [Required]
        public Nullable<bool> EmailGroup { get; set; }
        [Required]
        public Nullable<decimal> PlantID { get; set; }
    }
}