using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    public class CummulativeFields
    {
        
        public string BIW_Part_No { get; set;}

        public string Variant_Desc { get; set; }
        public string shiftName { get; set; }

        public string Model_Description { get; set; }
        public string IPMS_Color_Code { get; set; }
        public decimal Row_ID { get; set; }
        public decimal orderNo { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> CummlQty { get; set; }
        public string Style_Code { get; set; }
        public string Order_No { get; set; }
        public string parentModel_Code { get; set; }
        public string parentSeries { get; set; }
        public string Model_Code { get; set; }
        public string Series { get; set; }
        public string EngineModelCode { get; set; }
        public string TransmissionSeries { get; set; }
        public string remarks { get; set; }
        public string Auto_Remarks { get; set; }
        public string Order_Status { get; set; }
        public int PlannedQty { get; set; }
        public int HoldQty { get; set; }
        public int StartedQty { get; set; }
        public string orderType { get; set; }
        public string UToken { get; set; }
        public int Cummulative_Count { get; set; }
        public DateTime Inserted_Date { get; set; }

        public string Color_code { get; set; }
        public string Color_Name { get; set; }
        public decimal PlatformID{ get; set; }
        public string  PlatformName { get; set; }
        public decimal RSN { get; set; }

        public string SerialNumber  { get; set; }
        public Nullable<decimal> Planned_Shift_ID { get; set; }
        public string Attribution_Parameters { get; set; }
        public string Attribution { get; set; }

        public Boolean Locked { get; set; }
        public Nullable<Boolean> isNOVin { get; set; }

        public decimal Plant_OrderNo { get; set; }

    }
}