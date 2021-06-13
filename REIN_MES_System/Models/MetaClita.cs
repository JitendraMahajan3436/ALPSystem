using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.custom_Helper;

namespace REIN_MES_System.Models
{
    /* Class Name                  : MM_MT_Clita
     *  Description                : Override the MM_MT_Clita class with MetaClita class to define additional attributes, validation and properties
     *  Author, Timestamp          : Ajay Wagh       
     */
    [MetadataType(typeof(MetaClita))]
    public partial class MM_MT_Clita
    {

        public int[] myListBox1 { get; set; }
        public string[] mails { get; set; }
        public int[] users { get; set; }
        public int[] Clita_Tools { get; set; }
        public string Clita_Classification { get; set; }
        public string Clita_Standard { get; set; }
        REIN_SOLUTION_MEntities M_db = new REIN_SOLUTION_MEntities();
        MetaShift objShift = new MetaShift();
        public Boolean status;

        public bool isClitaChecked(decimal clitaID)
        {
            try
            {
                //MTTUWEntities db = new MTTUWEntities();
                //return db.MM_MT_Daily_Clita_Log.Any(a => a.Clita_ID == clitaID && DbFunctions.TruncateTime(a.Inserted_Date) == DateTime.Today);
                var frequency = Convert.ToInt32(M_db.MM_MT_Clita.Where(m => m.Clita_ID == clitaID).Select(m => m.Cycle).FirstOrDefault());
                var date = DateTime.Now;
                var clitadate = date.AddDays(-frequency);

                int shiftID = objShift.getShiftID();
                return M_db.MM_MT_Daily_Clita_Log.Any(a => a.Clita_ID == clitaID && DbFunctions.TruncateTime(a.Inserted_Date) > clitadate && DbFunctions.TruncateTime(a.Inserted_Date) <= date && a.Shift_ID == shiftID);
                
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string getClitaValue(decimal clitaID)
        {
            try
            {
                int shiftID = objShift.getShiftID();
                return M_db.MM_MT_Daily_Clita_Log.Where(a => a.Clita_ID == clitaID && DbFunctions.TruncateTime(a.Inserted_Date) == DateTime.Today && a.Shift_ID == shiftID).FirstOrDefault().Input_Value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public Boolean? getClitaStatus(decimal clitaID)
        {
            REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
            // var freq= db.MM_MT_Clita.Where(a => a.Clita_ID == clitaID && DbFunctions.TruncateTime(a.Inserted_Date) == DateTime.Today && a.Shift_ID == shiftID).FirstOrDefault();
            var frequency = Convert.ToInt32(db.MM_MT_Clita.Where(m => m.Clita_ID == clitaID).Select(m => m.Cycle).FirstOrDefault());
            var date = DateTime.Now;
            var clitadate = date.AddDays(-frequency);
            


            try
            {
               // REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
                int shiftID = objShift.getShiftID();
                var clitaObj = db.MM_MT_Daily_Clita_Log.Where(a => a.Clita_ID == clitaID && DbFunctions.TruncateTime(a.Inserted_Date)>clitadate && DbFunctions.TruncateTime(a.Inserted_Date) <= date && a.Shift_ID == shiftID).OrderByDescending(a=>a.CLITA_DailyCheck_ID).FirstOrDefault();
                if (clitaObj != null)
                {
                    return clitaObj.Status;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    /* Class Name                 : MetaClita
    *  Description                : MetaClita class to define additional attributes, validation and properties
    *  Author, Timestamp          : Ajay Wagh       
    */
    public class MetaClita
    {

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Station_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Machine_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Machine_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "CLITA_Item", ResourceType = typeof(ResourceDisplayName))]
        public string Clita_Item { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Classification", ResourceType = typeof(ResourceDisplayName))]
        public string Clita_Classification { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Standard", ResourceType = typeof(ResourceDisplayName))]
        public string Clita_Standard { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "CLITA_Tool", ResourceType = typeof(ResourceDisplayName))]
        public decimal Clita_Tools { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Method", ResourceType = typeof(ResourceDisplayName))]
        public string Clita_Method_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        //[Range(0, 12, ErrorMessageResourceType = (typeof(ResourceMachineMaintenance)), ErrorMessageResourceName = "Error_Frequency_Limit")]
        [Display(Name = "Frequency", ResourceType = typeof(ResourceDisplayName))]
        public decimal Cycle { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<int> Sequence_No { get; set; }

        //([01]?[0-9]|2[0-3]):[0-5][0-9]
        //^([0-9]|0[0-9]|[0-9]|2[0-3]):[0-5][0-9]&
        //[RegularExpression("^([0-9]|0[0-9]|[0-9]|2[0-3]):[0-5][0-9]&")]
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Time, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Error_TACT_Time")]
        [Display(Name = "Expected_Duration", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<System.TimeSpan> Maintenance_Time_TACT { get; set; }

        //[EmailAddress]
        //[Required(ErrorMessageResourceType = (typeof(ResourceMachineMaintenance)), ErrorMessageResourceName = "Email_Required")]
        //[Display(Name = "Receipent_Email", ResourceType = typeof(ResourceMachineMaintenance))]
        public string Recipent_Email { get; set; }

        [Display(Name = "Lower_Limit", ResourceType = typeof(ResourceDisplayName))]
        [RequiredIf("Is_Value_Based", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<double> Lower_Limit { get; set; }

        [Display(Name = "Upper_Limit", ResourceType = typeof(ResourceDisplayName))]
        [RequiredIf("Is_Value_Based", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public Nullable<double> Upper_Limit { get; set; }
        //[DisplayFormat(DataFormatString="{0:yyyy/MM/dd}")]
        //public Nullable<System.DateTime> Start_Date { get; set; }

        //[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        //public Nullable<System.DateTime> End_Date { get; set; }
    }
}