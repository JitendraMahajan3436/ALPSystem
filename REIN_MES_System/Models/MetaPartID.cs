using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{

    [MetadataType(typeof(MetaPartID))]
    public partial class RS_PartID
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();


        public bool isPartIDExists(string PartId, int plantId, int  RowId)
        {
            try
            {
                IQueryable<RS_PartID> result;
                if (RowId == 0)
                {
                    result = db.RS_PartID.Where(p => p.PartID == PartId && p.PlantID == plantId);
                }
                else
                {
                    result = db.RS_PartID.Where(p => p.PartID == PartId && p.PlantID == plantId && p.RowID != RowId);
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
    public class MetaPartID
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [MaxLength(3)]
        [Display(Name = "PartID", ResourceType = typeof(ResourceDisplayName))]
        public string PartID { get; set; }

        [MaxLength(50)]
        [Display(Name = "PartID_Desc", ResourceType = typeof(ResourceDisplayName))]
        public string PartIDDescription { get; set; }
    }
}