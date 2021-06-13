using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REIN_MES_System.App_LocalResources;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using REIN_MES_System.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaModelMaster))]

    public partial class RS_Model_Master
    {

        public int[] Countries { get; set; }
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        /*        Method Name                : IsModelMasterExists
         *        Description                : IsModelMasterExits use to check Model master already exit or not
         *        Author, Timestamp          : Jitendra Mahajan
         *        Input parameter            : (plantId,shopId,lineId,stationId,partgrpId,partno)
         *        Return Type                : bool 
         *        Revision                   : 1
         */
        //// [NotMapped]
        public string Series_Description { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        //[Display(Name = "Model_Type1", ResourceType = typeof(ResourceDisplayName))]
        public string Model_Type1 { get; set; }
        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        ////[Display(Name = "Varient1", ResourceType = typeof(ResourceDisplayName))]
        public string Varient1 { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        //// [Display(Name = "Family1", ResourceType = typeof(ResourceDisplayName))]
        public string Family1 { get; set; }



        public bool IsModelCodeExists(int plantId, int shopId, int lineId, int platformId, string model_code)
        {
            try
            {
                IQueryable<RS_Model_Master> result;
                if (lineId == 0)
                {
                    result = db.RS_Model_Master.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Platform_Id == platformId && p.Model_Code == model_code);
                }
                else
                {
                    result = db.RS_Model_Master.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId && p.Platform_Id == platformId && p.Model_Code == model_code);
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

    /* Class Name                  : MM_ModelMaster
     *  Description                : Override the MM_ModelMaster class with MetaModelMaster class to define additional attributes, validation and properties
     *  Author, Timestamp          : Jitendra Mahajan       
     */

    public class MetaModelMaster
    {
        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Model_Code", ResourceType = typeof(ResourceDisplayName))]
        // [Remote("RemoteModelCode", "RemoteValidation", AdditionalFields = "Shop_ID", ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Exist")]
        public string Model_Code { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Model_Description", ResourceType = typeof(ResourceDisplayName))]
        public string Model_Description { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Order_Config", ResourceType = typeof(ResourceDisplayName))]
        public string OMconfig_ID { get; set; }

        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Plant_Name", ResourceType = typeof(ResourceDisplayName))]

        public int Plant_ID { get; set; }

        
        public decimal Sub_Assembly_ID { get; set; }
        //  public int Shop_ID { get; set; }



        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        public string Platform_Id { get; set; }


        [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Shop_Name", ResourceType = typeof(ResourceDisplayName))]
        [NotMapped]
        public int Shop_ID
        {
            get
            {
                if (Shop_ID == 4)
                {
                    return (1);
                }
                else
                {
                    return (0);
                }

            }
        }

        //[RequiredIf("Shop_ID", 1)]
        ////[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Tyre_Make_Size_Front", ResourceType = typeof(ResourceDisplayName))]
        public string Tyre_Make_Size_Front { get; set; }
        //[RequiredIf("Shop_ID", 1)]
        ////[RequiredIf("Shop_ID", false, ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Tyre_Make_Size_Rear", ResourceType = typeof(ResourceDisplayName))]
        public string Tyre_Make_Size_Rear { get; set; }
        ////[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        ////[Display(Name = "Series_Code", ResourceType = typeof(ResourceDisplayName))]
        ////public string Series_Code { get; set; }
        ////[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        ////[Display(Name = "Model_Type", ResourceType = typeof(ResourceDisplayName))]
        ////public decimal Model_Type { get; set; }

       // [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Serial_Config", ResourceType = typeof(ResourceDisplayName))]
        public Nullable<long> Config_ID { get; set; }
        //[RequiredIf("Shop_ID", 1)]
        //// [Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        //[Display(Name = "Select_Color", ResourceType = typeof(ResourceDisplayName))]
        //public Nullable<decimal> Colour_ID { get; set; }
        [Display(Name = "Auto_Remarks", ResourceType =typeof(ResourceDisplayName))]
        public string Auto_Remarks { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]{6}$", ErrorMessage = "Please Enter 6 Alphanumeric Characters")]
        [Display(Name = "Vin_Code", ResourceType =typeof(ResourceDisplayName))]
        public string VIN_Code { get; set; }

        //[Required(ErrorMessageResourceType = (typeof(ResourceValidation)), ErrorMessageResourceName = "Required")]
        [Display(Name = "Image_Name", ResourceType = typeof(ResourceDisplayName))]
        public string Image_Name { get; set; }

    }

    

}