using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    /* Class Name                 : RS_Line_Types
    *  Description                : Override the RS_Line_Types class with MetaLineType class to define additional attributes and properties
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    [MetadataType(typeof(MetaLineType))]
    public partial class RS_Line_Types
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        /* Method Name                : isLineTypeExists
        *  Description                : Function is used to check duplication of line type
        *  Author, Timestamp          : Jitendra Mahajan :: 12-08-2015
        *  Input parameter            : typeName, plantId, typeId
        *  Return Type                : bool
        *  Revision                   : 1.0
        */
        public bool isLineTypeExists(String typeName, int plantId, int typeId)
        {
            try
            {
                IQueryable<RS_Line_Types> result;
                if (typeId == 0)
                {
                    result = db.RS_Line_Types.Where(p => p.Type_Name == typeName && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.RS_Line_Types.Where(p => p.Type_Name == typeName && p.Plant_ID == plantId && p.Line_Type_ID != typeId);
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

    /* Class Name                 : MetaLineType
    *  Description                : Class is used to define additional information with validation which is used in RS_Line_Types class
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class MetaLineType
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public int Plant_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [StringLength(50)]
        [Display(Name = "Line_Type_Name", ResourceType = typeof(ResourceDisplayName))]
        public String Type_Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Inserted_Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Updated_Date { get; set; }
    }
}