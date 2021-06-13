using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;
using System.Web.Mvc;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Lines
    *  Description                : Override the RS_Lines class with MetaLine class to define additional attributes, validation and properties
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    [MetadataType(typeof(MetaLine))]
    public partial class RS_Lines
    {
        public List<RS_Plants> plantLists { get; set; }
        //public int Plant_ID { get; set; }

        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /* Method Name                : isLineExists
        *  Description                : Method is used to check the line is already added in shop or not
        *  Author, Timestamp          : Jitendra Mahajan :: 12-08-2015
        *  Input parameter            : lineName, shopId, lineTypeId, lineId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isLineExists(String lineName, int shopId, int lineId, int PlantID)
        {
            try
            {
                IQueryable<RS_Lines> result;
                if (lineId == 0)
                {
                    //result = db.RS_Lines.Where(p => p.Line_Name == lineName && p.Shop_ID == shopId && p.Line_Type_Id == lineTypeId);
                    result = db.RS_Lines.Where(p => p.Line_Name == lineName && p.Shop_ID == shopId);
                }
                else
                {
                    //result = db.RS_Lines.Where(p => p.Line_Name == lineName && p.Shop_ID == shopId && p.Line_Type_Id == lineTypeId  && p.Line_ID != lineId);
                    result = db.RS_Lines.Where(p => p.Line_Name == lineName && p.Shop_ID == shopId && p.Line_ID != lineId);
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

        /* Method Name                : isShopLineStartExists
        *  Description                : Method is used to check line start is added in shop
        *  Author, Timestamp          : Jitendra Mahajan :: 12-08-2015
        *  Input parameter            : shopId, lineId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isShopLineStartExists(int shopId, int lineId)
        {
            try
            {
                IQueryable<RS_Lines> result;
                if (lineId == 0)
                {
                    result = db.RS_Lines.Where(p => p.Is_Shop_Line_Start == true && p.Shop_ID == shopId);
                }
                else
                {
                    result = db.RS_Lines.Where(p => p.Is_Shop_Line_Start == true && p.Shop_ID == shopId && p.Line_ID != lineId);
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

        /* Method Name                : isShopLineEndExists
        *  Description                : Method is used to line end is added in shop or not
        *  Author, Timestamp          : Jitendra Mahajan :: 12-08-2015
        *  Input parameter            : shopId, lineId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isShopLineEndExists(int shopId, int lineId)
        {
            try
            {
                IQueryable<RS_Lines> result;
                if (lineId == 0)
                {
                    result = db.RS_Lines.Where(p => p.Is_Shop_Line_End == true && p.Shop_ID == shopId);
                }
                else
                {
                    result = db.RS_Lines.Where(p => p.Is_Shop_Line_End == true && p.Shop_ID == shopId && p.Line_ID != lineId);
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


        /* Method Name                : isLineCodeExists
        *  Description                : Method is used to line end is added in shop or not
        *  Author, Timestamp          : Sandip :: 19-7-2017
        *  Input parameter            : shopId, lineId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isLineCodeExists(String line_Code, int shopId, int lineId, int PlantID)
        {
            try
            {
                IQueryable<RS_Lines> result;
                if (lineId == 0)
                {
                    //result = db.RS_Lines.Where(p => p.Line_Name == lineName && p.Shop_ID == shopId && p.Line_Type_Id == lineTypeId);
                    result = db.RS_Lines.Where(p => p.Line_Code == line_Code && p.Shop_ID == shopId);
                }
                else
                {
                    //result = db.RS_Lines.Where(p => p.Line_Name == lineName && p.Shop_ID == shopId && p.Line_Type_Id == lineTypeId  && p.Line_ID != lineId);
                    result = db.RS_Lines.Where(p => p.Line_Code == line_Code && p.Shop_ID == shopId && p.Line_ID != lineId);
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

    /* Class Name                 : MetaLine
    *  Description                : Class is used to define additional information with validation which is used in MM_Line class
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class MetaLine
    {
        [Required]
        public int Line_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [MaxLength(50)]
        [Display(Name = "Line_Name", ResourceType = typeof(ResourceDisplayName))]
        ///[Remote("RemoteLineName", "RemoteValidation", AdditionalFields = "Line_Name,Line_ID,Shop_ID,plant_Id", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]

        public String Line_Name { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemoteLineName", "RemoteValidation", AdditionalFields = "Line_Name,Line_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]

        public int Shop_ID { get; set; }

        //[Required]
        //public int Line_Type_Id { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Display(Name = "Is_Conveyor", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_Conveyor { get; set; }



        [MaxLength(100)]
        [Display(Name = "Line_Description", ResourceType = typeof(ResourceDisplayName))]
        public String Line_Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Inserted_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Updated_Date", ResourceType = typeof(ResourceDisplayName))]
        public DateTime? Updated_Date { get; set; }
        [Display(Name = "Line_Tact_Time", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<System.TimeSpan> TACT_Time { get; set; }
        [Display(Name = "IS_PLC", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> isPLC { get; set; }
        [Display(Name = "Is_Line_Start", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_Shop_Line_Start { get; set; }
        [Display(Name = "Is_Line_End", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<bool> Is_Shop_Line_End { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [MaxLength(50)]
        [Display(Name = "Line_Code", ResourceType = typeof(ResourceDisplayName))]
        [Remote("RemoteLineCode", "RemoteValidation", AdditionalFields = "Line_Name,Line_ID,Shop_ID,plant_Id", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        public Nullable<bool> Line_Code { get; set; }

        [Display(Name = "Is_Buildsheet", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_Buildsheet { get; set; }

        [Display(Name = "Is_PRN", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_PRN { get; set; }
    }
}