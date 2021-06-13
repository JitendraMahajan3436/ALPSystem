using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using REIN_MES_System.Helper;
using System.Text.RegularExpressions;

namespace REIN_MES_System.Models
{

    [MetadataType(typeof(MetaOrderStart))]
    public partial class RS_OM_Order_List
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
    
        /*               Action Name               : getPSNByDate
         *               Description               : Funcion used to genrating date wise the PSN number in order start form. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : None
         *               Return Type               : int
         *               Revision                  : 1
         */
        public String oldSeriesCode = "";
        public int getPSNByDate(int ShopId, int LineId)
        {
            try
            {
                DateTime time = DateTime.Now;             // Use current time.
                string format = "MM-dd-yy";   // Use this format.
                string dtObj = time.ToString(format);

                // String query = "select * from RS_OM_Order_List where CONVERT(VARCHAR(10),Inserted_Date,10)='" + dtObj + "'";
                String query = "SELECT MAX(PSN) FROM RS_OM_Order_List WHERE Shop_Id='" + Shop_ID + "' and Line_Id='" + Line_ID + "'";
                IEnumerable<RS_OM_Order_List> totalStarted = db.Database.SqlQuery<RS_OM_Order_List>(query);

                return totalStarted.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /*               Action Name               : getDSNByDateShop
     *               Description               : Funcion used to genrating date wise the DSN number in order start form. 
     *               Author, Timestamp         : Jitendra Mahajan
     *               Input parameter           : None
     *               Return Type               : int
     *               Revision                  : 1
     */
        public int getDSNByDateShop(int shopId, int lineId)
        {
            try
            {
                DateTime time = DateTime.Now;             // Use current time.
                string format = "MM-dd-yy";   // Use this format.
                string dtObj = time.ToString(format);

                //String query = "select * from RS_OM_Order_List where CONVERT(VARCHAR(10),Inserted_Date,10)='" + dtObj + "' and Shop_ID='" + shopId + "' and Line_ID='" + lineId + "'";
                String query = "select * from RS_OM_Order_List where CONVERT(VARCHAR(10),Inserted_Date,10)='" + dtObj + "' and Shop_ID='" + shopId + "'";
                IEnumerable<RS_OM_Order_List> totalStarted = db.Database.SqlQuery<RS_OM_Order_List>(query);

                return totalStarted.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /*               Action Name               : getSerialNumber
         *               Description               : Funcion used to genrating the Serial Number in order start form. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : totalOrderReleased,orderNo
         *               Return Type               : string
         *               Revision                  : 1
         */
        public String getSerialNumber(int shopId, int lineId, int totalOrderReleased, String orderNo, String modelCode, String partno, decimal? series_code, int serial_Config)
          //  public String getSerialNumber(decimal shopId, decimal lineId, String orderNo, String modelCode, String partno, long? serial_Config)
        {
            try
            {
                RS_Partgroup partGroupObj = db.RS_Partgroup.Where(a => a.Line_ID == lineId).FirstOrDefault();
                if ((shopId == 2 || shopId == 3 || shopId == 1 || shopId == 4) && partGroupObj.Order_Create == true)
                {
                    String number = "";
                    String serialNo = "";
                    RS_Serial_Number_Configuration mm_serialNumberConfig;
                    RS_Lines mm_line;

                    mm_line = (from mm_line_data in db.RS_Lines
                               where mm_line_data.Line_ID == lineId
                               select mm_line_data).FirstOrDefault();

                    //IF PARTNO DOES NOT EXIST IN MODEL MASTER
                    var modelMast = db.RS_Model_Master.Where(a => a.Model_Code == partno).FirstOrDefault();
                    if (modelMast != null)
                    {
                        serial_Config = Convert.ToInt32(modelMast.Config_ID);
                    }

                    mm_serialNumberConfig = (from mm_serialNumber in db.RS_Serial_Number_Configuration
                                             where mm_serialNumber.Config_ID == serial_Config
                                             select mm_serialNumber
                                             ).FirstOrDefault();

                    if (mm_serialNumberConfig != null)
                    {
                        RS_Serial_Number_Data mm_SerialNumberData;
                        mm_SerialNumberData = (from mm_serialNumberData in db.RS_Serial_Number_Data
                                               where mm_serialNumberData.Part_No.Trim() == partno.Trim()
                                               select mm_serialNumberData
                                               ).FirstOrDefault();

                        if (mm_SerialNumberData != null)
                        {
                            string serial_logic = "";
                            serial_logic = mm_serialNumberConfig.Serial_Logic;

                            string[] thisArray = serial_logic.Split(',');//<string1/string2/string3/--->     
                            List<string> myList = new List<string>(); //make a new string list    
                            myList.AddRange(thisArray);

                            for (int i = 0; i < myList.Count(); i++)
                            {

                                var names = typeof(RS_Serial_Number_Data).GetProperties()
                                            .Select(property => property.Name)
                                           
                                            .ToArray();

                                for (int j = 0; j < names.Count(); j++)
                                {
                                    if (myList[i].ToString() == names[j].ToString())
                                    {
                                        string str = "Select " + myList[i].ToString() + " from RS_Serial_Number_Data where Part_No = '" + partno + "'";
                                        IEnumerable<String> query = db.Database.SqlQuery<String>(str).ToList();

                                        string val = query.FirstOrDefault();

                                        if ((myList[i].ToString().ToUpper()).Contains("YEAR"))
                                        {
                                            int currentYear = DateTime.Now.Year;
                                            int identifier = mm_serialNumberConfig.Year_Identifier.GetValueOrDefault(0);
                                            val = db.RS_Year.Where(a => a.Identifier_ID == identifier && a.Year == currentYear).FirstOrDefault().Year_Code.Trim();

                                        }
                                        else if ((myList[i].ToString().ToUpper()).Contains("MONTH"))
                                        {
                                            int currentMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                                            int identifier = mm_serialNumberConfig.Month_Identifier.GetValueOrDefault(0);
                                            val = db.RS_Month.Where(a => a.Identifier_ID == identifier && a.Month_No == currentMonth).FirstOrDefault().Month_Code.Trim();
                                        }
                                        number = number + val;
                                    }
                                }
                            }
                            // String serialNo=orderNo.Substring(0,3);
                            serialNo = number;

                            if (mm_serialNumberConfig.Running_Serial_Number < 10)
                            {
                                serialNo = serialNo.Trim() + "0000" + mm_serialNumberConfig.Running_Serial_Number.ToString();
                            }
                            else
                            if (mm_serialNumberConfig.Running_Serial_Number >= 10 && mm_serialNumberConfig.Running_Serial_Number <= 99)
                            {
                                serialNo = serialNo.Trim() + "000" + mm_serialNumberConfig.Running_Serial_Number.ToString();
                            }
                            else
                            if (mm_serialNumberConfig.Running_Serial_Number > 99 && mm_serialNumberConfig.Running_Serial_Number < 999)
                            {
                                serialNo = serialNo.Trim() + "00" + mm_serialNumberConfig.Running_Serial_Number.ToString();
                            }
                            else if (mm_serialNumberConfig.Running_Serial_Number > 999 && mm_serialNumberConfig.Running_Serial_Number < 9999)
                            {
                                serialNo = serialNo.Trim() + "0" + mm_serialNumberConfig.Running_Serial_Number.ToString();
                            }
                            else if (mm_serialNumberConfig.Running_Serial_Number > 9999 && mm_serialNumberConfig.Running_Serial_Number < 99999)
                            {
                                serialNo = serialNo.Trim() + "" + mm_serialNumberConfig.Running_Serial_Number.ToString();
                            }
                            //serialNo += mm_serialNumberConfig.Running_Serial_Number;
                            string prefix = "";
                            string suffix = "";
                            if (!String.IsNullOrEmpty(mm_SerialNumberData.Prefix))
                            {
                                prefix = mm_SerialNumberData.Prefix;
                            }
                            if (!String.IsNullOrEmpty(mm_SerialNumberData.Suffix))
                            {
                                suffix = mm_SerialNumberData.Suffix;
                            }

                            General genObj = new General();

                            //int runningSerialCounter = genObj.getCurrentRunningSerial(partno);
                            int runningSerialCounter = 0;
                            //serialNo = prefix.Trim().ToUpper() + (serialNo + Convert.ToString(runningSerialCounter).PadRight(Convert.ToInt32(mm_serialNumberConfig.Series_Count), '0')).Trim() + suffix.Trim().ToUpper();

                        }
                        else
                        {
                            serialNo = (serialNo + "1".PadLeft(Convert.ToInt32(mm_serialNumberConfig.Series_Count), '0')).Trim();
                        }
                    }
                    else
                    {
                        serialNo = mm_line.Line_Code + orderNo;
                    }

                    return serialNo;
                }
                else
                {
                    String number = "";
                    String serialNo = "";
                    RS_Serial_Number_Configuration mm_serialNumberConfig;
                    RS_Lines mm_line;

                    mm_line = (from mm_line_data in db.RS_Lines
                               where mm_line_data.Line_ID == lineId
                               select mm_line_data).FirstOrDefault();

                    mm_serialNumberConfig = (from mm_serialNumber in db.RS_Serial_Number_Configuration
                                             where mm_serialNumber.Config_ID == serial_Config
                                             select mm_serialNumber
                                             ).FirstOrDefault();

                    if (mm_serialNumberConfig != null)
                    {
                        RS_Serial_Number_Data mm_SerialNumberData;
                        mm_SerialNumberData = (from mm_serialNumberData in db.RS_Serial_Number_Data
                                               where mm_serialNumberData.Part_No.Trim() == partno.Trim()
                                               select mm_serialNumberData
                                               ).FirstOrDefault();

                        if (mm_SerialNumberData != null)
                        {
                            string serial_logic = "";
                            serial_logic = mm_serialNumberConfig.Serial_Logic;

                            string[] thisArray = serial_logic.Split(',');//<string1/string2/string3/--->     
                            List<string> myList = new List<string>(); //make a new string list    
                            myList.AddRange(thisArray);

                            for (int i = 0; i < myList.Count(); i++)
                            {

                                var names = typeof(RS_Serial_Number_Data).GetProperties()
                                            .Select(property => property.Name)
                                            .ToArray();

                                for (int j = 0; j < names.Count(); j++)
                                {
                                    if (myList[i].ToString() == names[j].ToString())
                                    {
                                        string str = "Select " + myList[i].ToString() + " from RS_Serial_Number_Data where Part_No='" + partno + "'";
                                        IEnumerable<String> query = db.Database.SqlQuery<String>(str).ToList();

                                        string val = query.FirstOrDefault();
                                        if ((myList[i].ToString().ToUpper()).Contains("YEAR"))
                                        {
                                            int currentYear = DateTime.Now.Year;
                                            int identifier = mm_serialNumberConfig.Year_Identifier.GetValueOrDefault(0);
                                            val = db.RS_Year.Where(a => a.Identifier_ID == identifier && a.Year == currentYear).FirstOrDefault().Year_Code.Trim();
                                        }
                                        else if ((myList[i].ToString().ToUpper()).Contains("MONTH"))
                                        {
                                            int currentMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                                            int identifier = mm_serialNumberConfig.Month_Identifier.GetValueOrDefault(0);
                                            val = db.RS_Month.Where(a => a.Identifier_ID == identifier && a.Month_No == currentMonth).FirstOrDefault().Month_Code.Trim();
                                        }
                                        number = number + val;
                                    }
                                }
                            }
                            // String serialNo=orderNo.Substring(0,3);
                            serialNo = number;

                            //find last 4 digit from order_list
                            var order_list = (from or_list in db.RS_OM_Order_List
                                              where or_list.Shop_ID == shopId && or_list.Line_ID == Line_ID
                                              select or_list).ToList();
                            if (order_list.Count() > 0)
                            {
                                var maxObject = order_list.OrderByDescending(item => item.PSN).First();
                                int psn = Convert.ToInt16(maxObject.PSN);
                                string serial = Convert.ToString(psn);
                                String strDigit = serial.TrimStart(new Char[] { '0' });
                                int lastDigit = Convert.ToInt16(strDigit) + 1;
                                //Count Zero
                                serialNo = (serialNo + Convert.ToString(lastDigit).PadLeft(Convert.ToInt32(mm_serialNumberConfig.Series_Count), '0')).Trim();
                            }
                            else
                            {
                                serialNo = serialNo + "1".PadLeft(Convert.ToInt32(mm_serialNumberConfig.Series_Count), '0').Trim();
                            }
                        }
                        else
                        {
                            serialNo = mm_line.Line_Code + orderNo;
                        }
                    }
                    else
                    {
                        serialNo = mm_line.Line_Code + orderNo;
                    }

                    return serialNo;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        /*               Action Name               : Hold_Order_Check
         *               Description               : Funcion used the Hold_Order_check  in order start form. 
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : rowId,shopId,lineId,orderNo,modelCode,rsn
         *               Return Type               : Boolean
         *               Revision                  : 1
         */
        public Boolean Hold_Order_Check(int rowId, int shopId, int lineId, String orderNo, String modelCode, int rsn)
        {
            try
            {
                var st = (from orderReleaseItem in db.RS_OM_OrderRelease
                          where orderReleaseItem.Shop_ID == shopId && orderReleaseItem.Row_ID == rowId && orderReleaseItem.Order_Status == "Hold"
                          select orderReleaseItem).ToList();
                if (st.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }

        }

        public Boolean Line_stop_or_not(int shopId, int lineId)
        {
            try
            {
                var st = (from line_stop in db.RS_Lines
                          where line_stop.Line_ID == lineId && line_stop.isLineStop == true
                          select line_stop
                          ).ToList();

                if (st.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }

        }

        public string getVendorSerialNumberAD(string ModelCode,string Vendor_Number, string order_No)
        {
            try
            {
                string serialNo = "";
                if (ModelCode != null)
                {
                    var ModelId = db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Model_ID).FirstOrDefault();

                    RS_Model_Master obj = db.RS_Model_Master.Find(ModelId);
                    var SerialConfig = obj.Config_ID;
                    var PlantCode = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == SerialConfig).Select(m => m.Plant_Code).FirstOrDefault();
                    serialNo = PlantCode;
                    var LineCode = (from model in db.RS_Model_Master.Where(m => m.Model_Code == ModelCode)
                                    join line in db.RS_OM_Platform
                                    on model.Platform_Id equals line.Platform_ID
                                    //where model.Plant_ID==Plant_ID && model.Shop_ID==Shop_ID
                                    select new
                                    {
                                        line.Serial_No_Code
                                    }).FirstOrDefault();
                    if (LineCode != null)
                    {
                        serialNo += LineCode.Serial_No_Code.Trim().ToUpper();
                    }

                    serialNo += Vendor_Number.Trim().ToUpper();

                    var color_code = "";
                    bool IsColorCheck = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Color_Code).FirstOrDefault());
                    if (IsColorCheck)
                    {
                        color_code = ModelCode.Substring(ModelCode.Length - 2, 2);
                        serialNo += color_code;
                    }
                    else
                    {
                        color_code = db.RS_OM_OrderRelease.Where(m => m.Model_Code == ModelCode && m.Order_No == order_No).Select(m => m.Model_Color).FirstOrDefault();
                        serialNo += color_code;
                    }
                }
                return serialNo;
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        public string getSerialNumberAD(string ModelCode, int? runningSerialNumber, string order_No)
        {
            try
            {
                string serialNo = "";
                if (ModelCode != null)
                {
                    var ModelId = db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Model_ID).FirstOrDefault();

                    RS_Model_Master obj = db.RS_Model_Master.Find(ModelId);
                    var SerialConfig = obj.Config_ID;
                    var DisplayName = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == SerialConfig).Select(m => m.Display_Name).FirstOrDefault().Trim();
                    //var LineCode = (from model in db.RS_Model_Master.Where(m => m.Model_Code == ModelCode)
                    //                join line in db.RS_Lines
                    //                on model.Body_Line equals line.Line_ID
                    //                select new
                    //                {
                    //                    line.Line_Code
                    //                }).FirstOrDefault();
                    if (DisplayName == "Engine")
                    {

                        RS_Model_Master model = db.RS_Model_Master.Find(ModelId);
                        List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));

                        for (int i = 0; i < attributionParameters.Count; i++)
                        {
                            AttributionParameters attributionParameter = attributionParameters[i];

                            if (attributionParameter.label.Equals("Series", StringComparison.InvariantCultureIgnoreCase))
                            {
                                int attrId = Convert.ToInt32(attributionParameter.Value);
                                var Series = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                                serialNo = Series;
                            }
                        }
                        //serialNo = serialNo;
                         var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year).Select(m => m.Year_Code).FirstOrDefault();
                        if (year != null)
                        {
                            serialNo += year.Trim();
                        }
                        var plantCode = db.RS_Serial_Number_Configuration.Where(m => m.Config_ID == SerialConfig).Select(m => m.Plant_Code).FirstOrDefault(); ;
                        if (plantCode != null)
                        {
                            serialNo += plantCode;
                        }
                        var month = db.RS_Month.Where(m => m.Month_No == DateTime.Now.Month).Select(m => m.Month_Code).FirstOrDefault();
                        if (month != null)
                        {
                            serialNo += month.Trim();
                        }
                        if (runningSerialNumber < 10)
                        {
                            serialNo = serialNo.Trim() + "0000" + runningSerialNumber.ToString();
                        }
                        else
                            if (runningSerialNumber >= 10 && runningSerialNumber <= 99)
                        {
                            serialNo = serialNo.Trim() + "000" + runningSerialNumber.ToString();
                        }
                        else
                            if (runningSerialNumber > 99 && runningSerialNumber <= 999)
                        {
                            serialNo = serialNo.Trim() + "00" + runningSerialNumber.ToString();
                        }
                        else if (runningSerialNumber > 999 && runningSerialNumber <= 9999)
                        {
                            serialNo = serialNo.Trim() + "0" + runningSerialNumber.ToString();
                        }
                        else if (runningSerialNumber > 9999 && runningSerialNumber <= 99999)
                        {
                            serialNo = serialNo.Trim() + "" + runningSerialNumber.ToString();
                        }
                    }
                    else if(DisplayName == "CylenderHead")
                    {
                        serialNo = "ZHB";
                        RS_Model_Master model = db.RS_Model_Master.Find(ModelId);
                        List<AttributionParameters> attributionParameters = (List<AttributionParameters>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.Attribution_Parameters, typeof(List<AttributionParameters>));

                        for (int i = 0; i < attributionParameters.Count; i++)
                        {
                            AttributionParameters attributionParameter = attributionParameters[i];

                            if (attributionParameter.label.Equals("Model", StringComparison.InvariantCultureIgnoreCase))
                            {
                                int attrId = Convert.ToInt32(attributionParameter.Value);
                                var Series = db.RS_Attribution_Parameters.Find(attrId).Attribute_Desc;
                                serialNo += Series.Trim();
                            }
                        }
                        var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year).Select(m => m.Year).FirstOrDefault();
                        var yearNo = year != null ? year.ToString().Substring(2, 2):"";
                        if (yearNo != null)
                        {
                            serialNo += yearNo.Trim();
                        }
                        var month = db.RS_Month.Where(m => m.Month_No == DateTime.Now.Month).Select(m => m.Month_Code).FirstOrDefault();
                        if (month != null)
                        {
                            serialNo += month.Trim();
                        }
                        if (runningSerialNumber < 10)
                        {
                            serialNo = serialNo.Trim() + "0000" + runningSerialNumber.ToString();
                        }
                        else
                           if (runningSerialNumber >= 10 && runningSerialNumber <= 99)
                        {
                            serialNo = serialNo.Trim() + "000" + runningSerialNumber.ToString();
                        }
                        else
                           if (runningSerialNumber > 99 && runningSerialNumber <= 999)
                        {
                            serialNo = serialNo.Trim() + "00" + runningSerialNumber.ToString();
                        }
                        else if (runningSerialNumber > 999 && runningSerialNumber <= 9999)
                        {
                            serialNo = serialNo.Trim() + "0" + runningSerialNumber.ToString();
                        }
                        else if (runningSerialNumber > 9999 && runningSerialNumber <= 99999)
                        {
                            serialNo = serialNo.Trim() + "" + runningSerialNumber.ToString();
                        }
                    }
                    else if (DisplayName == "CrankCase")
                    {
                        serialNo = "ZBC";
                        var month = db.RS_Month.Where(m => m.Month_No == DateTime.Now.Month).Select(m => m.Month_Code).FirstOrDefault();
                        if (month != null)
                        {
                            serialNo += month.Trim();
                        }
                        var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year).Select(m => m.Year).FirstOrDefault();
                        var yearNo = year != null ? year.ToString().Substring(2, 2) : "";
                        if (yearNo != null)
                        {
                            serialNo += yearNo.Trim();
                        }
                        if (runningSerialNumber < 10)
                        {
                            serialNo = serialNo.Trim() + "0000" + runningSerialNumber.ToString();
                        }
                        else
                           if (runningSerialNumber >= 10 && runningSerialNumber <= 99)
                        {
                            serialNo = serialNo.Trim() + "000" + runningSerialNumber.ToString();
                        }
                        else
                           if (runningSerialNumber > 99 && runningSerialNumber <= 999)
                        {
                            serialNo = serialNo.Trim() + "00" + runningSerialNumber.ToString();
                        }
                        else if (runningSerialNumber > 999 && runningSerialNumber <= 9999)
                        {
                            serialNo = serialNo.Trim() + "0" + runningSerialNumber.ToString();
                        }
                        else if (runningSerialNumber > 9999 && runningSerialNumber <= 99999)
                        {
                            serialNo = serialNo.Trim() + "" + runningSerialNumber.ToString();
                        }
                    }
                    else
                    {
                        var PlantCode = db.RS_Serial_Number_Configuration.Where(m=>m.Config_ID == SerialConfig).Select(m=>m.Plant_Code).FirstOrDefault();
                        serialNo = PlantCode;
                        var LineCode = (from model in db.RS_Model_Master.Where(m => m.Model_Code == ModelCode)
                                        join line in db.RS_OM_Platform
                                        on model.Platform_Id equals line.Platform_ID
                                        //where model.Plant_ID==Plant_ID && model.Shop_ID==Shop_ID
                                        select new
                                        {
                                            line.Serial_No_Code
                                        }).FirstOrDefault();
                        if (LineCode != null)
                        {
                            serialNo += LineCode.Serial_No_Code.Trim().ToUpper();
                        }
                        else
                        {

                        }
                        var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year && m.Identifier_ID ==1).Select(m => m.Year_Code).FirstOrDefault();
                        if (year != null)
                        {
                            serialNo += year.Trim();
                        }
                        if (runningSerialNumber < 10)
                        {
                            serialNo = serialNo.Trim() + "0000" + runningSerialNumber.ToString();
                        }
                        else
                            if (runningSerialNumber >= 10 && runningSerialNumber <= 99)
                        {
                            serialNo = serialNo.Trim() + "000" + runningSerialNumber.ToString();
                        }
                        else
                            if (runningSerialNumber > 99 && runningSerialNumber <= 999)
                        {
                            serialNo = serialNo.Trim() + "00" + runningSerialNumber.ToString();
                        }
                        else if (runningSerialNumber > 999 && runningSerialNumber <= 9999)
                        {
                            serialNo = serialNo.Trim() + "0" + runningSerialNumber.ToString();
                        }
                        else if (runningSerialNumber > 9999 && runningSerialNumber <= 99999)
                        {
                            serialNo = serialNo.Trim() + "" + runningSerialNumber.ToString();
                        }

                        var color_code = "";
                        bool IsColorCheck = Convert.ToBoolean(db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Color_Code).FirstOrDefault());
                        if (IsColorCheck)
                        {
                            color_code = ModelCode.Substring(ModelCode.Length - 2, 2);
                            serialNo += color_code;
                        }
                        else
                        {
                            color_code = db.RS_OM_OrderRelease.Where(m => m.Model_Code == ModelCode && m.Order_No == order_No).Select(m => m.Model_Color).FirstOrDefault();
                            serialNo += color_code;
                        }
                    }
                    
                }
                return serialNo;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        public string getTrialBodySerialNumber(string ModelCode, int? runningSerialNumber, string order_No)
        {
            try
            {
                string serialNo = "N";
                if (ModelCode != null)
                {
                 
                    var LineCode = (from model in db.RS_Model_Master.Where(m => m.Model_Code == ModelCode)
                                    join line in db.RS_OM_Platform
                                    on model.Platform_Id equals line.Platform_ID
                                    //where model.Plant_ID==Plant_ID && model.Shop_ID==Shop_ID
                                    select new
                                    {
                                        line.Serial_No_Code
                                    }).FirstOrDefault();
                    if (LineCode != null)
                    {
                        serialNo += LineCode.Serial_No_Code.Trim().ToUpper();
                    }
                    else
                    {

                    }
                    var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year).Select(m => m.Year_Code).FirstOrDefault();
                    if (year != null)
                    {
                        serialNo += year.Trim();
                    }

                    if (runningSerialNumber != null)
                    {
                        //runningSerialNumber;
                    }
                    //if (runningSerialNumber < 10)
                    //{
                    //    serialNo = serialNo + "0" + runningSerialNumber.ToString();
                    //}

                    //else
                    //{
                    //    serialNo = serialNo + "" + runningSerialNumber.ToString();
                    //}
                    int number = 0;
                    if (runningSerialNumber < 10)
                    {
                        serialNo = serialNo.Trim() + "0000" + runningSerialNumber.ToString();
                    }
                    else
                        if (runningSerialNumber >= 10 && runningSerialNumber <= 99)
                    {
                        serialNo = serialNo + "000" + runningSerialNumber.ToString();
                    }
                    else
                        if (runningSerialNumber > 99 && runningSerialNumber < 999)
                    {
                        serialNo = serialNo + "00" + runningSerialNumber.ToString();
                    }
                    else if (runningSerialNumber > 999 && runningSerialNumber < 9999)
                    {
                        serialNo = serialNo + "0" + runningSerialNumber.ToString();
                    }
                    else if (runningSerialNumber > 9999 && runningSerialNumber < 99999)
                    {
                        serialNo = serialNo + " " + runningSerialNumber.ToString();
                    }
                    var color_code = "TR";
                    //bool IsColorCheck = db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Color_Code).FirstOrDefault();
                    //if (IsColorCheck)
                    //{
                    //    color_code = ModelCode.Substring(ModelCode.Length - 2, 2);
                    //    serialNo += color_code;
                    //    //var color_Name = db.MM_Color.Where(m => m.COLOUR_ID == color_code).Select(c => c.COLOUR_DESC );
                    //}
                    //else
                    //{
                    //    color_code = db.RS_OM_OrderRelease.Where(m => m.Model_Code == ModelCode && m.Order_No == order_No).Select(m => m.Model_Color).FirstOrDefault();
                    //    serialNo += color_code;
                    //}
                    serialNo += color_code;
                }
                return serialNo;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        //added by mukesh for tear down serial number generation ending with TD
        public string getTearDownBodySerialNumber(string ModelCode, int? runningSerialNumber, string order_No)
        {
            try
            {
                string serialNo = "N";
                if (ModelCode != null)
                {

                    var LineCode = (from model in db.RS_Model_Master.Where(m => m.Model_Code == ModelCode)
                                    join line in db.RS_OM_Platform
                                    on model.Platform_Id equals line.Platform_ID
                                    //where model.Plant_ID==Plant_ID && model.Shop_ID==Shop_ID
                                    select new
                                    {
                                        line.Serial_No_Code
                                    }).FirstOrDefault();
                    if (LineCode != null)
                    {
                        serialNo += LineCode.Serial_No_Code.Trim().ToUpper();
                    }
                    else
                    {

                    }
                    var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year).Select(m => m.Year_Code).FirstOrDefault();
                    if (year != null)
                    {
                        serialNo += year.Trim();
                    }

                    if (runningSerialNumber != null)
                    {
                        //runningSerialNumber;
                    }
                    //if (runningSerialNumber < 10)
                    //{
                    //    serialNo = serialNo + "0" + runningSerialNumber.ToString();
                    //}

                    //else
                    //{
                    //    serialNo = serialNo + "" + runningSerialNumber.ToString();
                    //}
                    int number = 0;
                    if (runningSerialNumber < 10)
                    {
                        serialNo = serialNo.Trim() + "0000" + runningSerialNumber.ToString();
                    }
                    else
                        if (runningSerialNumber >= 10 && runningSerialNumber <= 99)
                    {
                        serialNo = serialNo + "000" + runningSerialNumber.ToString();
                    }
                    else
                        if (runningSerialNumber > 99 && runningSerialNumber < 999)
                    {
                        serialNo = serialNo + "00" + runningSerialNumber.ToString();
                    }
                    else if (runningSerialNumber > 999 && runningSerialNumber < 9999)
                    {
                        serialNo = serialNo + "0" + runningSerialNumber.ToString();
                    }
                    else if (runningSerialNumber > 9999 && runningSerialNumber < 99999)
                    {
                        serialNo = serialNo + " " + runningSerialNumber.ToString();
                    }
                    var color_code = "TD";
                    //bool IsColorCheck = db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.Color_Code).FirstOrDefault();
                    //if (IsColorCheck)
                    //{
                    //    color_code = ModelCode.Substring(ModelCode.Length - 2, 2);
                    //    serialNo += color_code;
                    //    //var color_Name = db.MM_Color.Where(m => m.COLOUR_ID == color_code).Select(c => c.COLOUR_DESC );
                    //}
                    //else
                    //{
                    //    color_code = db.RS_OM_OrderRelease.Where(m => m.Model_Code == ModelCode && m.Order_No == order_No).Select(m => m.Model_Color).FirstOrDefault();
                    //    serialNo += color_code;
                    //}
                    serialNo += color_code;
                }
                return serialNo;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        /*               Action Name               : getVinNumberGenration
 *               Description               : Genration VIN Number logic 
 *               Author, Timestamp         : Jitendra Mahajan
 *               Input parameter           : ModelCode,lineId
 *               Return Type               : String
 *               Revision                  : VinNo
 */
        public string getVinNumberGenration(string ModelCode, decimal lineId,string OrderNo)
        {
            try
            {
                //Total VIN NUMBER LENGTH : (17)
                //string VinNo = "MA1N";
                string VinNo = "";
                string dom_Year = ""; string exp_Year = "";
                string dom_month = ""; string exp_month = "";

                if(ModelCode != null)
                {
                    var MA1 = (from model in db.RS_MAST_VIN_LINE.Where(m => m.AssyLine_ID == lineId)
                               select new
                               {
                                   model.VIN1TO3,
                                   model.isauto_domestic,
                                   model.isauto_export,
                                   model.current_year_domestic,
                                   model.Current_year_export,
                                   model.current_month_domestic,
                                   model.Current_month_export,
                                   model.PlantCode,
                                   model.FROM_VIN,
                                   model.To_VIN ,
                                   model.LastNo
                               }).FirstOrDefault();

                    //First 3 characters are fixed
                    if (MA1 != null)
                    {
                        VinNo = MA1.VIN1TO3;
                    }
                    else
                    {
                        VinNo = "MA1";
                    }

                    // Next 6 character take it from Vin_Code field of Modelmaster
                    var V_Code = db.RS_Model_Master.Where(m => m.Model_Code == ModelCode).Select(m => m.VIN_Code).FirstOrDefault();
                    VinNo += V_Code.Trim();

                    var Country_ID = db.RS_OM_OrderRelease.Where(m => m.Order_No == Order_No && m.Line_ID == lineId).Select(m => m.Country_ID).FirstOrDefault();

                    if (Country_ID == 1)
                    {
                        var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year && m.Identifier_ID==1).Select(m => m.Year_Code).FirstOrDefault();
                        if(year != null)
                        {
                            VinNo += year.Trim();
                        }

                       
                        if(MA1.PlantCode != 0)
                        {
                            VinNo += MA1.PlantCode;
                        }

                        var month = db.RS_Month.Where(m => m.Month_No == DateTime.Now.Month && m.Identifier_ID == 1).Select(m => m.Month_Code).FirstOrDefault();
                        if (month != null)
                        {
                            VinNo += month.Trim();
                        }
                    }
                    else
                    {
                        var MonthNo = DateTime.Now.Month;
                        if(MonthNo <= 6)
                        {
                            var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year && m.Identifier_ID == 1).Select(m => m.Year_Code).FirstOrDefault();
                            if (year != null)
                            {
                                VinNo += year.Trim();
                            }


                            if (MA1.PlantCode != 0)
                            {
                                VinNo += MA1.PlantCode;
                            }

                            var month = db.RS_Month.Where(m => m.Month_No == DateTime.Now.Month && m.Identifier_ID ==1).Select(m => m.Month_Code).FirstOrDefault();
                            if (month != null)
                            {
                                VinNo += month.Trim();
                            }
                        }
                        else
                        {
                            var year = db.RS_Year.Where(m => m.Year == DateTime.Now.Year + 1 && m.Identifier_ID == 1).Select(m => m.Year_Code).FirstOrDefault();
                            if (year != null)
                            {
                                VinNo += year.Trim();
                            }


                            if (MA1.PlantCode != 0)
                            {
                                VinNo += MA1.PlantCode;
                            }

                            var month = db.RS_Month.Where(m => m.Month_No == 13).Select(m => m.Month_Code).FirstOrDefault();
                            if (month != null)
                            {
                                VinNo += month.Trim();
                            }
                        }

                    }

                    // add last 5 characters of Running Serial No
                    decimal runningSerialNumber = 0;
                    decimal to_VIN = 0;
                    decimal from_Vin = 0;
                    runningSerialNumber = MA1.LastNo;

                    to_VIN = MA1.To_VIN;
                    from_Vin = MA1.FROM_VIN;
                    if (to_VIN >= from_Vin)
                    {
                        runningSerialNumber = runningSerialNumber + 1;
                    }

                    if(runningSerialNumber > to_VIN)
                    {
                        runningSerialNumber = from_Vin + 1;
                    }
                    


                    if (runningSerialNumber < 10)
                    {
                        VinNo = VinNo.Trim() + "0000" + runningSerialNumber.ToString().Trim();
                    }
                    else
                        if (runningSerialNumber >= 10 && runningSerialNumber <= 99)
                    {
                        VinNo = VinNo + "000" + runningSerialNumber.ToString().Trim();
                    }
                    else
                        if (runningSerialNumber > 99 && runningSerialNumber < 999)
                    {
                        VinNo = VinNo + "00" + runningSerialNumber.ToString().Trim();
                    }
                    else if (runningSerialNumber > 999 && runningSerialNumber < 9999)
                    {
                        VinNo = VinNo + "0" + runningSerialNumber.ToString().Trim();
                    }
                    else if (runningSerialNumber > 9999 && runningSerialNumber <= 99999)
                    {
                        VinNo = VinNo + "" + runningSerialNumber.ToString().Trim();
                    }


                    //---------------Update in FROM_VIN ------------------------//
                    RS_MAST_VIN_LINE vin_Line;
                    vin_Line = new RS_MAST_VIN_LINE();
                    vin_Line = db.RS_MAST_VIN_LINE.Find(lineId);
                    vin_Line.LastNo = runningSerialNumber;

                    db.Entry(vin_Line).State = EntityState.Modified;
                    db.SaveChanges();
                    //----------------------------------------------------------//
                }
                return VinNo;
                //if (ModelCode != null)
                //{
                //    RS_MAST_VIN_LINE vin_Line;
                //    string lineIdS = Convert.ToString(lineId);
                //    //VIN Genration First 3 Digit Fixed -  Total : (3)
                //    var MA1 = (from model in db.RS_MAST_VIN_LINE.Where(m => m.AssyLine_ID == lineId)
                //               select new
                //               {
                //                   model.VIN1TO3,
                //                   model.isauto_domestic,
                //                   model.isauto_export,
                //                   model.current_year_domestic,
                //                   model.Current_year_export,
                //                   model.current_month_domestic,
                //                   model.Current_month_export,
                //                   model.PlantCode,
                //                   model.FROM_VIN,
                //                   model.To_VIN
                //               }).FirstOrDefault();

                //    if (MA1 != null)
                //    {
                //        VinNo = MA1.VIN1TO3;
                //    }
                //    else
                //    {
                //        VinNo = "MA1";
                //    }

                //    //VIN Genration Fourth 1 Digit Fixed  -  Total : (4)  
                //    VinNo += "N";


                //    //VIN Genration Fifth 1 Digit Country/Left Hand Drive,Right Hand Drive  -  Total : (5)
                //    //Domestic Logic Start
                //    if (MA1 != null)
                //    {
                //        if (MA1.isauto_domestic == true)
                //        {
                //            VinNo += "M";

                //            //Domestic Year
                //            decimal d_year = MA1.current_year_domestic;
                //            var Domestic_Year = (from model in db.RS_Year.Where(m => m.Year == d_year && m.Identifier_ID == 1)
                //                                 select new
                //                                 {
                //                                     model.Year_Code
                //                                 }).FirstOrDefault();

                //            if (Domestic_Year != null)
                //            {
                //                dom_Year = Domestic_Year.Year_Code;
                //            }

                //            //Domestic Month
                //            int d_month = Convert.ToInt32(MA1.current_month_domestic);
                //            var Domestic_Month = (from model in db.RS_Month.Where(m => m.Month_No == d_month && m.Identifier_ID == 1)
                //                                  select new
                //                                  {
                //                                      model.Month_Code
                //                                  }).FirstOrDefault();

                //            if (Domestic_Month != null)
                //            {
                //                dom_month = Domestic_Month.Month_Code;
                //            }

                //        }//Domestic Logic End

                //        //Export Logic Start
                //        if (MA1.isauto_export == true)
                //        {
                //            var Export = (from model in db.RS_Model_Master.Where(m => m.Model_Code == ModelCode)
                //                          select new
                //                          {
                //                              model.Attribution_Parameters
                //                          }).FirstOrDefault();

                //            string model_code = Export.Attribution_Parameters;
                //            string[] words = model_code.Split('}');
                //            string resultString = "";
                //            int cnt = 0;

                //            foreach (string word in words)
                //            {
                //                if (word != "" && word != null)
                //                {
                //                    resultString = Regex.Match(word, @"\d+").Value;

                //                    if (resultString != "" && resultString != null)
                //                    {

                //                        switch (cnt)
                //                        {
                //                            case 0:
                //                                break;
                //                            case 1:
                //                                break;
                //                            case 2:
                //                                try
                //                                {
                //                                    var Vehicel_Drive = (from model in db.RS_Attribution_Parameters.Where(m => m.Attribute_ID == Convert.ToDecimal(resultString))
                //                                                         select new
                //                                                         {
                //                                                             model.Attribute_Desc

                //                                                         }).FirstOrDefault();

                //                                    if (Vehicel_Drive != null)
                //                                    {
                //                                        string Vehicle_start = Vehicel_Drive.Attribute_Desc.Substring(1, 1);
                //                                        if (Vehicle_start == "R")
                //                                        {
                //                                            VinNo += "N";
                //                                        }
                //                                        else if (Vehicle_start == "L")
                //                                        {
                //                                            VinNo += "P";
                //                                        }
                //                                    }
                //                                }
                //                                catch (Exception VD)
                //                                {
                //                                }

                //                                break;
                //                            default:
                //                                break;
                //                        }
                //                    }
                //                    cnt = cnt + 1;
                //                }
                //            };


                //            //Export Year
                //            decimal e_year = MA1.Current_year_export;
                //            var Export_Year = (from model in db.RS_Year.Where(m => m.Year == e_year && m.Identifier_ID == 1)
                //                               select new
                //                               {
                //                                   model.Year_Code
                //                               }).FirstOrDefault();

                //            if (Export_Year != null)
                //            {
                //                exp_Year = Export_Year.Year_Code;
                //            }

                //            //Export Month
                //            int e_month = Convert.ToInt32(MA1.Current_month_export);
                //            var Export_Month = (from model in db.RS_Month.Where(m => m.Month_No == e_month && m.Identifier_ID == 1)
                //                                select new
                //                                {
                //                                    model.Month_Code
                //                                }).FirstOrDefault();

                //            if (Export_Month != null)
                //            {
                //                exp_month = Export_Month.Month_Code;
                //            }

                //        } //Export Logic End
                //    }

                //    ////VIN Genration Sixth 1 Digit Wheel Drive -  Total : (6)
                //    string Wheel_Drive = "";
                //    Wheel_Drive = ModelCode.Substring(3, 1);
                //    VinNo += Wheel_Drive;

                //    //VIN Genration Seventh,Eighth 2 Digit Engine Code -  Total : (8)
                //    string Engine_Code = "";
                //    Engine_Code = ModelCode.Substring(6, 2);
                //    VinNo += Engine_Code;

                //    //VIN Genration Ninth 1 Digit Transmission  -  Total : (9)
                //    string Transmission = "";
                //    Transmission = ModelCode.Substring(10, 1);
                //    VinNo += Transmission;

                //    if (MA1 != null)
                //    {
                //        //VIN Genration Tenth 1 Digit Year Code -  Total : (10)
                //        if (MA1.isauto_domestic == true)
                //        {
                //            VinNo += dom_Year;
                //        }
                //        else if (MA1.isauto_export == true)
                //        {
                //            VinNo += exp_Year;
                //        }

                //        //VIN Genration Eleventh 1 Digit Plant Code  -  Total : (11)
                //        string Plant_code = Convert.ToString(MA1.PlantCode);
                //        VinNo += Plant_code;

                //        //VIN Genration Tweleth 1 Digit Month Code  -  Total : (12)
                //        if (MA1.isauto_domestic == true)
                //        {
                //            VinNo += dom_month;
                //        }
                //        else if (MA1.isauto_export == true)
                //        {
                //            VinNo += exp_month;
                //        }

                //        //VIN Genration 5 Digit Serial Number -  Total : (17)

                //        decimal Serial_number = 0;
                //        decimal to_VIN = 0;
                //        Serial_number = MA1.FROM_VIN;
                //        to_VIN = MA1.To_VIN;
                //        if (to_VIN >= Serial_number)
                //        {
                //            if (Serial_number != 0)
                //            {
                //                Serial_number = Serial_number + 1;
                //                VinNo += Serial_number;

                //                //---------------Update in FROM_VIN ------------------------//

                //                vin_Line = new RS_MAST_VIN_LINE();
                //                vin_Line = db.RS_MAST_VIN_LINE.Find(lineId);
                //                vin_Line.FROM_VIN = Serial_number;

                //                db.Entry(vin_Line).State = EntityState.Modified;
                //                db.SaveChanges();
                //                //----------------------------------------------------------//
                //            }
                //        }
                //        else
                //        {
                //            VinNo = null;
                //        }

                //    }
                //}
                //return VinNo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }////







    public class MetaOrderStart
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Entry_Date { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan? Entry_Time { get; set; }
    }
}