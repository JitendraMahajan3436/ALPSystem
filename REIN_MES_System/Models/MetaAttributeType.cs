using REIN_MES_System.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaAttributeType))]
    public partial class RS_AttributionType_Master
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_Attribution_Parameters mmAttributionParametersObj = new RS_Attribution_Parameters();
        GlobalData globalData = new GlobalData();
        General generalObj = new General();


        //public bool isDublicateAttributeName(string Attribute_Type,int plant_id)
        //{
        //    try
        //    {
        //        bool isAttExist = db.RS_AttributionType_Master.Any(p => p.Attribution_Type == Attribute_Type && p.Plant_ID == plant_id);

        //        //RS_AttributionType_Master Attribute_list = db.RS_AttributionType_Master.Where(p => p.Attribution_Type == Attribute_Type).FirstOrDefault();
        //        if (isAttExist)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        public bool isDublicateAttributeName(string Attribute_Type, decimal? Attribution_ID, int Plant_ID)
        {
            var result = false;
            //if ((Kitting_Zone_Part_ID == null || Kitting_Zone_Part_ID == 0) && Kitting_Zone_ID!= null && Shop_ID != null && LineID != null && StationID!= null)
            if (Attribution_ID == 0 && Plant_ID != null)
            {
                result = (db.RS_AttributionType_Master.Any(bstring => bstring.Attribution_Type.ToLower() == Attribute_Type.ToLower() && bstring.Plant_ID == Plant_ID));

            }
            else
            {
                result = (db.RS_AttributionType_Master.Any(bstring => bstring.Attribution_Type.ToLower() == Attribution_Type.ToLower() && bstring.Attribution_ID != Attribution_ID
                     && bstring.Plant_ID == Plant_ID));

            }
            return result;
        }

        public bool isDuplicateTextPosition(int? Toolbox_Pos, decimal? Attribution_ID, int Plant_ID,int Sub_Assembly_ID)
        {
            var result = false;
            if(Toolbox_Pos != null)
            {
                if (Attribution_ID == 0)
                {
                    result = (db.RS_AttributionType_Master.Any(m => m.ToolBox_Post == Toolbox_Pos && m.Plant_ID == Plant_ID && m.Sub_Assembly_ID == Sub_Assembly_ID));

                }
                else
                {
                    result = (db.RS_AttributionType_Master.Any(m => m.ToolBox_Post == Toolbox_Pos && m.Attribution_ID != Attribution_ID
                         && m.Plant_ID == Plant_ID));
                }
            }
            
            return result;
        }
    }

    public class MetaAttributeType
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        // [Display(Name = "AttributionParameters_Label_Shop", ResourceType = typeof(ResourceAttributionParameters))]

        public string Attribution_Type { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        // [Display(Name = "AttributionParameters_Label_Shop", ResourceType = typeof(ResourceAttributionParameters))]
        [Display(Name = "ToolBox_Post", ResourceType = typeof(ResourceDisplayName))]
        public int ToolBox_Post { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public decimal Sub_Assembly_ID { get; set; }
    }
}