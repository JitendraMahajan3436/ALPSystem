using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaOrderType))]
    public partial class RS_OM_Order_Type
    {

    }
    public class MetaOrderType
    {
        [Display(Name = "Order_Type_ID", ResourceType = typeof(ResourceDisplayName))]
        public decimal Order_Type_ID { get; set; }

        [Display(Name = "Order_Type_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Order_Type_Name { get; set; }

        [Display(Name = "Order_Type_Code", ResourceType = typeof(ResourceDisplayName))]
        public string Order_Type_Code { get; set; }


        [Display(Name = "Shop_ID", ResourceType = typeof(ResourceDisplayName))]
        public int Shop_ID { get; set; }


        [Display(Name = "Is_Spare", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_Spare { get; set; }

        [Display(Name = "Is_Build_Sheet_Print", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_Build_Sheet_Print { get; set; }

        [Display(Name = "Is_Production_Booking", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_Production_Booking { get; set; }

        [Display(Name = "Is_Serial_No_Generation", ResourceType = typeof(ResourceDisplayName))]
        public bool Is_Serial_No_Generation { get; set; }
    }
}