using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZHB_AD.Models;
namespace ZHB_AD.Models
{
   
    

    [MetadataType(typeof(MasterCategoryMetaData))]
    public partial class MM_Category
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        public bool IsCategoryExists(string Category_Name, int plantId, int Category_Id)
        {
            try
            {
               
                IQueryable<MM_Category> result;
                if (Category_Id == 0)
                {
                    result = db.MM_Category.Where(p => p.Category_Name == Category_Name && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.MM_Category.Where(p => p.Category_Name == Category_Name && p.Plant_ID == plantId && p.Category_Id != Category_Id);
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

    [MetadataType(typeof(MasterUtilityMainFeederMappingMetaData))]
    public partial class UtilityMainFeederMapping
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        public bool IsTagIndexExists(int TagIndex, int plantId, int MeterID)
        {
            try
            {

                IQueryable<UtilityMainFeederMapping> result;
                if (MeterID == 0)
                {
                    result = db.UtilityMainFeederMappings.Where(p => p.TagIndex == TagIndex && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.UtilityMainFeederMappings.Where(p => p.TagIndex == TagIndex && p.Plant_ID == plantId && p.RowId != MeterID);
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
        public bool IsFeederExists(int Feeder, int plantId, int shopId, int ParameterId, int MeterID)
        {
            try
            {

                IQueryable<UtilityMainFeederMapping> result;
                if (MeterID == 0)
                {
                    result = db.UtilityMainFeederMappings.Where(p => p.Feeder_ID == Feeder && p.Shop_ID == shopId && p.Parameter_ID == ParameterId && p.Plant_ID == plantId);
                }
                else
                {
                    result = db.UtilityMainFeederMappings.Where(p => p.Feeder_ID == Feeder && p.Shop_ID == shopId &&
                    p.Parameter_ID == ParameterId && p.Plant_ID == plantId && p.RowId != MeterID);
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

    [MetadataType(typeof(ParameterMasterMetaData))]
    public partial class MM_Parameter
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        public bool IsParameterExists(string Prameter_Name, int Plant_ID,int ParameterId)
        {
            try
            {
                IQueryable<MM_Parameter> result;
                if (ParameterId == 0)
                {
                    result = db.MM_Parameter.Where(p => p.Prameter_Name == Prameter_Name && p.Plant_ID == Plant_ID);
                }
                else
                {
                    result = db.MM_Parameter.Where(p => p.Prameter_Name == Prameter_Name && p.Plant_ID == Plant_ID && p.Prameter_ID != ParameterId);
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


    [MetadataType(typeof(MetaTransmission_Loss))]
    public partial class Total_Transmission_Loss_Consumption
    {
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();



        public bool IsFactorExists(int plantId, DateTime ConsumptionDate, int Tran_Id, Boolean status)
        {
            try
            {
                ConsumptionDate = ConsumptionDate.Date;

                IQueryable<Total_Transmission_Loss_Consumption> result;
                if (Tran_Id == 0)
                {
                    result = db.Total_Transmission_Loss_Consumption.Where(p => p.Plant_ID == plantId && p.DateTime == ConsumptionDate && p.Status == status);
                }
                else
                {
                    result = db.Total_Transmission_Loss_Consumption.Where(p => p.Plant_ID == plantId && p.DateTime == ConsumptionDate && p.Trans_ID == Tran_Id && p.Status == status);
                }

                if (Tran_Id == 0)
                {
                    if (result.Count() == 0)
                        return false;
                    else
                        return true;
                }
                else
                {
                    if (result.Count() > 0)
                        return true;
                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }

    [MetadataType(typeof(MetaManualFeeder_Consumption))]
    public partial class MM_ManualFeeder_Consumption
    {

        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();



        public bool IsConsumptionExists(int plantId, int shopId, int TagIndex, DateTime ConsumptionDate, int Tran_Id)
        {
            try
            {
                ConsumptionDate = ConsumptionDate.Date;

                IQueryable<MM_ManualFeeder_Consumption> result;
                if (Tran_Id == 0)
                {
                    result = db.MM_ManualFeeder_Consumption.Where(p => p.Plant_Id == plantId && p.DateAndTime == ConsumptionDate && p.Shop_Id == shopId && p.TagIndex == TagIndex);
                }
                else
                {
                    result = db.MM_ManualFeeder_Consumption.Where(p => p.Plant_Id == plantId && p.DateAndTime == ConsumptionDate && p.Shop_Id == shopId && p.TagIndex ==TagIndex && p.Manual_ID != Tran_Id);
                }

                if (Tran_Id == 0)
                {
                    if (result.Count() == 0)
                        return false;
                    else
                        return true;
                }
                else
                {
                    if (result.Count() > 0)
                        return true;
                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}