using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;
using System.ComponentModel.DataAnnotations;
using REIN_MES_System.Helper;
using REIN_MES_System.custom_Helper;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{

    [MetadataType(typeof(MetaPartMaster))]
    public partial class RS_Partmaster
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        /*        Method Name                : IsPartMasterExists
         *        Description                : IsPartMasterExits use to check Partmaster already exit or not
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : (plantId,shopId,lineId,stationId,partgrpId,partno)
         *        Return Type                : bool 
         *        Revision                   : 1
         */

        //  [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Series_Discription", ResourceType = typeof(ResourceDisplayName))]
        public string Series_Discription { get; set; }
        public bool IsPartNoExists(int plantId, int shopId, int stationId, int lineId, decimal? partgrpId, string partno)
        {
            try
            {
                IQueryable<RS_Partmaster> result;
                if (lineId == 0)
                {
                    //result = db.RS_Partmaster.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Station_ID==stationId && p.Part_No==partno);
                    result = db.RS_Partmaster.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Part_No == partno);
                }
                else
                {
                    //result = db.RS_Partmaster.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Line_ID == lineId && p.Station_ID == stationId && p.Part_No == partno);
                    result = db.RS_Partmaster.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Part_No == partno);
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

    /* Class Name                : RS_Partmaster
     *  Description                : Override the RS_Plants class with MetaPartMaster class to define additional attributes, validation and properties
     *  Author, Timestamp          : Jitendra Mahajan       
     */
    public class MetaPartMaster
    {

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Station_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Station_ID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "PartGroup_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Partgroup_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Part_No", ResourceType = typeof(ResourceDisplayName))]

        public string Part_No { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Part_Description", ResourceType = typeof(ResourceDisplayName))]
        public string Part_Description { get; set; }

        [Display(Name = "Series_Code", ResourceType = typeof(ResourceDisplayName))]
        public string Series_Code { get; set; }
        [Display(Name = "Stage", ResourceType = typeof(ResourceDisplayName))]
        public string Stage { get; set; }
        [Display(Name = "Eror_Proofing", ResourceType = typeof(ResourceDisplayName))]
        public bool Error_Proofing { get; set; }
        [Display(Name = "Kitting", ResourceType = typeof(ResourceDisplayName))]
        public bool Kitting { get; set; }
        [Display(Name = "Sequence", ResourceType = typeof(ResourceDisplayName))]
        public bool Sequence { get; set; }
        [Display(Name = "Is_Finished", ResourceType = typeof(ResourceDisplayName))]
        public bool isFinished { get; set; }

        [Display(Name = "Genealogy", ResourceType = typeof(ResourceDisplayName))]
        public bool Genealogy { get; set; }
        [Display(Name = "is_Non_RS_Barcode", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> is_Non_RS_Barcode { get; set; }
        [RequiredIf("is_Non_RS_Barcode", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Std_Char", ResourceType = typeof(ResourceDisplayName))]
        public string Std_Char { get; set; }
        [RequiredIf("is_Non_RS_Barcode", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^(\d{2})$", ErrorMessage = "Please Enter 2 digit numbers")]

        [Display(Name = "Start_Position", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<short> Start_Position { get; set; }
        [RequiredIf("is_Non_RS_Barcode", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^(\d{2})$", ErrorMessage = "Please Enter 2 digit numbers")]

        [Display(Name = "Month_Start_Position", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<short> Month_Start_Position { get; set; }
        [RequiredIf("is_Non_RS_Barcode", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^(\d{2})$", ErrorMessage = "Please Enter 2 digit numbers")]


        [Display(Name = "Year_Start_Position", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<short> Year_Start_Position { get; set; }
        [RequiredIf("is_Non_RS_Barcode", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^(\d{1})$", ErrorMessage = "Please Enter 1 digit numbers")]

        [Display(Name = "Month_Identifier", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> Month_Identifier { get; set; }
        [RequiredIf("is_Non_RS_Barcode", true, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^(\d{1})$", ErrorMessage = "Please Enter 1 digit numbers")]

        [Display(Name = "Year_Identifier", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<int> Year_Identifier { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourcePartMaster)), ErrorMessageResourceName = "Plant_Error_PartMaster_PartSeries_Required")]
        //public string Series_Code { get; set; }


    }
}