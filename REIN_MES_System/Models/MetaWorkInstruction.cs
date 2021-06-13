using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;
namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaWorkInstruction))]
    public partial class RS_WorkInstruction_Master
    {
        public bool Is_Submitted { get; set; }
        public string Status { get; set; }

    }
    public class MetaWorkInstruction
    {
    }
}