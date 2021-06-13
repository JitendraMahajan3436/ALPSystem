using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaBomItem))]

    public partial class RS_BOM_Item
    {
        //[Required]
        public HttpPostedFileBase Excel_File { get; set; }
        public string Model_Desc { get; set; }
        public DateTime FGEffective_Date_From { get; set; }
        public DateTime FGEffective_Date_To{ get; set; }
        public bool Is_Sucess { get; set; }

        public string SS_Error_Sucess { get; set; }

    }
    public class MetaBomItem
    {
    
      
    }
}