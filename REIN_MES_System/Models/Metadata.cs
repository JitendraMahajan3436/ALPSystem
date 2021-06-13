using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using REIN_MES_System.App_LocalResources;

namespace REIN_MES_System.Models
{
    public class RS_PlantsMetaData
    {
        [Required(ErrorMessage = " Please enter the Plant Name")]
        [StringLength(40, MinimumLength =3)]
        [Display (Name  = "Plant Name")]
        public string Plant_Name { get; set; }
        [Required(ErrorMessage = " Please enter the Plant Code")]
        [StringLength(40, MinimumLength = 2)]
        public string Plant_Code_SAP { get; set; }
       
        [Required(ErrorMessage = "Please enter the Plant Address")]
        [StringLength(40, MinimumLength = 4)]
        public string Address { get; set; }

        [Required(ErrorMessage = " Please enter the City")]
        [StringLength(10, MinimumLength = 2)]
        public string City { get; set; }

        [Required(ErrorMessage = " Please enter the Country")]
        [StringLength(10, MinimumLength = 3)]
        public string Country { get; set; }
    }

    public class  MasterShopsMetaData
    {
       [Required]
        [StringLength(40, MinimumLength = 4)]
        public string Shop_Name { get; set; }
        [Required(ErrorMessage = " Shop Group field is required")]
        [StringLength(40, MinimumLength = 3)]

        public string Shop_SAP { get; set; }
        [Required]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }  
    
        [Required(ErrorMessage = " IP Address field is required")]
        [StringLength(20, MinimumLength = 3)]
     
        public string Inserted_Host { get; set; }
     
        public Nullable<decimal> ShopsCat_ID { get; set; }



    }

    public class MasterCategoryMetaData
    {
        [Required]
        [StringLength(40, MinimumLength = 3)]
        public string Category_Name { get; set; }
        [StringLength(20, MinimumLength = 3)]
        public string Description { get; set; }
      
        [Required]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }
      
    }

    public class MasterUtilityMainFeederMappingMetaData
    {

        [Required]
        public int Feeder_ID { get; set; }

        [Required]
        public Nullable<decimal> Parameter_ID { get; set; }

        [Required]
        [Range(0, 10000000)]
        public Nullable<decimal> TagIndex { get; set; }

        
        public string Unit { get; set; }

        [Required]
        public string TagName { get; set; }

        [Required]
        public Nullable<decimal> Shop_ID { get; set; }

     
        public string Meter { get; set; }

        [Required]
        public Nullable<bool> Consider { get; set; }


    }

    public class ParameterMasterMetaData
    {
        [Required]      
        [StringLength(40, MinimumLength = 3)]
        public string Prameter_Name { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 3)]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]
        public decimal Plant_ID { get; set; }

    }

    public class MetaTransmission_Loss
    {

    }
    public class MetaManualFeeder_Consumption 
    {

    }

   
}