using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaShift))]
    public partial class RS_Shift
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        public bool isShiftExists(String shiftName, int shopId, int shiftId)
        {
            try
            {
                IQueryable<RS_Shift> result;
                if (shiftId == 0)
                {
                    result = db.RS_Shift.Where(p => p.Shift_Name == shiftName && p.Shop_ID == shopId);
                }
                else
                {
                    result = db.RS_Shift.Where(p => p.Shift_Name == shiftName && p.Shop_ID == shopId && p.Shift_ID != shiftId);
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

        public bool isShiftStartTimeCorrect(int shiftId, int shopId, TimeSpan shiftStartTime)
        {
            try
            {
                IQueryable<RS_Shift> result;
                if (shiftId == 0)
                {
                    result = db.RS_Shift.Where(p => p.Shop_ID == shopId && (p.Shift_Start_Time <= shiftStartTime && p.Shift_End_Time >= shiftStartTime));
                }
                else
                {
                    result = db.RS_Shift.Where(p => p.Shop_ID == shopId && (p.Shift_Start_Time <= shiftStartTime && p.Shift_End_Time >= shiftStartTime) && p.Shift_ID != shiftId);
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

        public bool isShiftEndTimeCorrect(int shiftId, int shopId, TimeSpan shiftEndTime)
        {
            try
            {
                IQueryable<RS_Shift> result;
                if (shiftId == 0)
                {
                    result = db.RS_Shift.Where(p => p.Shop_ID == shopId && (p.Shift_Start_Time <= shiftEndTime && p.Shift_End_Time >= shiftEndTime));
                }
                else
                {
                    result = db.RS_Shift.Where(p => p.Shop_ID == shopId && (p.Shift_Start_Time <= shiftEndTime && p.Shift_End_Time >= shiftEndTime) && p.Shift_ID != shiftId);
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

        public bool isShiftBreakTimeCorrect(int shiftId, int shopId, TimeSpan shiftStartTime, TimeSpan shiftEndTime, TimeSpan breakTime)
        {
            try
            {
                IQueryable<RS_Shift> result;
                if (shiftId == 0)
                {
                    result = db.RS_Shift.Where(p => p.Shop_ID == shopId && (p.Shift_Start_Time <= shiftEndTime || p.Shift_End_Time <= shiftEndTime));
                }
                else
                {
                    result = db.RS_Shift.Where(p => p.Shop_ID == shopId && (p.Shift_Start_Time <= shiftEndTime || p.Shift_End_Time <= shiftEndTime) && p.Shift_ID != shiftId);
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

        public int getCurrentRunningShiftByShopID(int shopId)
        {
            try
            {
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                RS_Shift mmShiftObj = (from shiftObj in db.RS_Shift
                                       where shiftObj.Shop_ID == shopId
                                && TimeSpan.Compare(shiftObj.Shift_Start_Time, currDate) < 0
                                && TimeSpan.Compare(shiftObj.Shift_End_Time, currDate) > 0
                                       select shiftObj).FirstOrDefault();
                return Convert.ToInt16(mmShiftObj.Shift_ID);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }

    public class MetaShift
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_Name", ResourceType = typeof(ResourceDisplayName))]
        public String Shift_Name { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_Start_Time", ResourceType = typeof(ResourceDisplayName))]
        public TimeSpan Shift_Start_Time { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_End_Time", ResourceType = typeof(ResourceDisplayName))]
        public TimeSpan Shift_End_Time { get; set; }
        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_First_Break_Time", ResourceType = typeof(ResourceDisplayName))]
        public TimeSpan Break1_Time { get; set; }
        // [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_Second_Break_Time", ResourceType = typeof(ResourceDisplayName))]
        public TimeSpan Break2_Time { get; set; }
        // [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_Lunch_Time", ResourceType = typeof(ResourceDisplayName))]
        public TimeSpan Lunch_Time { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }
        // [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_First_Break_End_Time", ResourceType = typeof(ResourceDisplayName))]
        public TimeSpan Break1_End_Time { get; set; }
        // [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_Second_Break_End_Time", ResourceType = typeof(ResourceDisplayName))]
        public TimeSpan Break2_End_Time { get; set; }
        //  [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shift_Lunch_End_Time", ResourceType = typeof(ResourceDisplayName))]
        public TimeSpan Lunch_End_Time { get; set; }

        public int getShiftID()
        {
            List<RS_Shift> RS_Shift = new List<RS_Shift>();
            int shiftid = 0;
            using (REIN_SOLUTIONEntities Nag_db = new REIN_SOLUTIONEntities())
            {
                RS_Shift = Nag_db.RS_Shift.ToList();
            }
            foreach (var item in RS_Shift)
            {
                bool result = IsTimeOfDayBetween(item.Shift_Start_Time, item.Shift_End_Time);
                if (result == true)
                {
                    shiftid = Convert.ToInt32(item.Shift_ID);
                    break;

                }
            }//foreach
            return shiftid;
        }

        static public bool IsTimeOfDayBetween(TimeSpan startTime, TimeSpan endTime)
        {
            DateTime time = System.DateTime.Now;
            return time.TimeOfDay >= startTime &&
                  time.TimeOfDay <= endTime;
        }
    }
}