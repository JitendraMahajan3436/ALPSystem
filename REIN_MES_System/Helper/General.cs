using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using REIN_MES_System.Models;
using REIN_MES_System.Helper.IoT;

namespace REIN_MES_System.Helper
{
    public class General
    {

        RS_Error_Log mmErrorLogObj = new RS_Error_Log();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

     
        public String getEncryptedString(String text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public bool addControllerException(Exception ex, String controllerName, String actionName, int userId = 1)
        {

            mmErrorLogObj = new RS_Error_Log();
            mmErrorLogObj.Controller_Name = controllerName;
            mmErrorLogObj.Action_Name = actionName;

            if (ex != null)
            {

                if (ex.InnerException != null)
                    mmErrorLogObj.Inner_Exception = ex.InnerException + "";

                if (ex.Message != null)
                    mmErrorLogObj.Message = ex.Message.ToString();

                if (ex.Data != null)
                    mmErrorLogObj.Exception_Data = ex.Data.ToString();

                if (ex.TargetSite != null)
                    mmErrorLogObj.Target_Site = ex.TargetSite.ToString();

                if (ex.StackTrace != null)
                    mmErrorLogObj.Stack_Trace = ex.StackTrace.ToString();

                if (ex.Source != null)
                    mmErrorLogObj.Source = ex.Source.ToString();

                if (ex.HResult != null)
                    mmErrorLogObj.H_Result = ex.HResult.ToString();


            }
            mmErrorLogObj.Inserted_Date = DateTime.Now;
            mmErrorLogObj.Inserted_Host = "sample";
            mmErrorLogObj.Inserted_User_ID = userId;
            db.RS_Error_Log.Add(mmErrorLogObj);
            db.SaveChanges();

            return true;

        }
        public bool addShopControllerException(Exception ex, String controllerName, String actionName, int stationID, int plantID, int shopID, int lineID, int userId = 1)
        {

            mmErrorLogObj = new RS_Error_Log();
            mmErrorLogObj.Controller_Name = controllerName;
            mmErrorLogObj.Action_Name = actionName;

            if (ex != null)
            {
                if (ex.InnerException != null)
                    mmErrorLogObj.Inner_Exception = ex.InnerException + "";

                if (ex.Message != null)
                    mmErrorLogObj.Message = ex.Message.ToString();

                if (ex.Data != null)
                    mmErrorLogObj.Exception_Data = ex.Data.ToString();

                if (ex.TargetSite != null)
                    mmErrorLogObj.Target_Site = ex.TargetSite.ToString();

                if (ex.StackTrace != null)
                    mmErrorLogObj.Stack_Trace = ex.StackTrace.ToString();

                if (ex.Source != null)
                    mmErrorLogObj.Source = ex.Source.ToString();

                if (ex.HResult != null)
                    mmErrorLogObj.H_Result = ex.HResult.ToString();

            }
            mmErrorLogObj.Station_ID = stationID;
            mmErrorLogObj.Plant_ID = plantID;
            mmErrorLogObj.Shop_ID = shopID;
            mmErrorLogObj.Line_ID = lineID;
            mmErrorLogObj.Inserted_Date = DateTime.Now;
            mmErrorLogObj.Inserted_Host = "sample";
            mmErrorLogObj.Inserted_User_ID = userId;
            db.RS_Error_Log.Add(mmErrorLogObj);
            db.SaveChanges();

            return true;

        }
        public bool addMetaException(Exception ex, String metaName, String methodName, int userId = 1)
        {

            mmErrorLogObj = new RS_Error_Log();
            mmErrorLogObj.Meta_Class_Name = metaName;
            mmErrorLogObj.Method_Name = methodName;
            mmErrorLogObj.Exception_Detail = ex.ToString();

            if (ex.InnerException != null)
                mmErrorLogObj.Inner_Exception = ex.InnerException + "";

            if (ex.Message != null)
                mmErrorLogObj.Message = ex.Message.ToString();

            if (ex.Data != null)
                mmErrorLogObj.Exception_Data = ex.Data.ToString();

            if (ex.TargetSite != null)
                mmErrorLogObj.Target_Site = ex.TargetSite.ToString();

            if (ex.StackTrace != null)
                mmErrorLogObj.Stack_Trace = ex.StackTrace.ToString();

            if (ex.Source != null)
                mmErrorLogObj.Source = ex.Source.ToString();

            mmErrorLogObj.H_Result = ex.HResult.ToString();

            mmErrorLogObj.Inserted_Date = DateTime.Now;
            mmErrorLogObj.Inserted_Host = "sample";
            mmErrorLogObj.Inserted_User_ID = userId;
            db.RS_Error_Log.Add(mmErrorLogObj);
            db.SaveChanges();

            return true;

        }

        public string getIPAddress(HttpRequestBase request)
        {
            string szRemoteAddr = request.UserHostAddress;
            string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];
            string szIP = "";

            if (szXForwardedFor == null)
            {
                szIP = szRemoteAddr;
            }
            else
            {
                szIP = szXForwardedFor;
                if (szIP.IndexOf(",") > 0)
                {
                    string[] arIPs = szIP.Split(',');

                    foreach (string item in arIPs)
                    {
                        if (!isPrivateIP(item))
                        {
                            return item;
                        }
                    }
                }
            }
            return szIP;
        }

      
        private bool isPrivateIP(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        //public void logLineStopData(decimal plantID, decimal shopID, decimal lineID, decimal stationID, string reason, string remarks, decimal? EfPartGroupID = null)
        //{
        //    bool isSameCallAreadyGiven = db.RS_History_LineStop.Any(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == reason && a.Line_ID == lineID && a.Station_ID == stationID);
        //    if (!isSameCallAreadyGiven)
        //    {
        //        DateTime dateTimeNow;
        //        TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
        //        if (dateObj != null)
        //        {
        //            dateTimeNow = dateObj.todayDate;
        //        }
        //        else
        //        {
        //            dateTimeNow = DateTime.Now;
        //        }

        //        RS_History_LineStop lineStopObj = new RS_History_LineStop();
        //        lineStopObj.Plant_ID = plantID;
        //        lineStopObj.Shop_ID = shopID;
        //        lineStopObj.Line_ID = lineID;
        //        lineStopObj.Station_ID = stationID;
        //        lineStopObj.Stop_Reason = reason;
        //        lineStopObj.Remarks = remarks;
        //        lineStopObj.Stop_Time = dateTimeNow;
        //        lineStopObj.isLineStop = true;
        //        lineStopObj.isHeartBit = false;
        //        lineStopObj.isEmergencyCall = false;
        //        lineStopObj.EFPartGroup_ID = EfPartGroupID;
        //        lineStopObj.PLC_Ack = false;
        //        lineStopObj.Inserted_Date = DateTime.Now;
        //        db.RS_History_LineStop.Add(lineStopObj);
        //        db.SaveChanges();

             

        //    }
        //    if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("IsCallToIoT")))
        //    {
        //        Kepware kepware = new Kepware();
        //        kepware.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
        //        kepware.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
        //    }

        //}

      
        public void logLineResume(decimal plantID, decimal shopID, decimal lineID, decimal stationID, string reason, string EfPartGroupID = null)
        {


        }

        //public int getCurrentRunningSerial(int shopId, string partNo, decimal partgroupId)
        //{
        //    try
        //    {
        //        partNo = partNo.Trim();
        //        RS_Serial_Number_Data mm_SerialNumberData = db.RS_Serial_Number_Data.Where(a => a.Part_No.Trim() == partNo && a.Shop_ID == shopId).FirstOrDefault();
        //        RS_Model_Master modelMaster = db.RS_Model_Master.Where(p => p.Model_Code == partNo).FirstOrDefault();
        //        RS_Serial_Number_Configuration mm_serialNumberConfig = db.RS_Serial_Number_Configuration.Where(p => p.Config_ID == modelMaster.Config_ID).FirstOrDefault();
                
        //        int runningSerialCounter = mm_SerialNumberData.Running_Serial.GetValueOrDefault(0);
        //        runningSerialCounter++;
        //        RS_Partgroup mmPartGroup = db.RS_Partgroup.Where(p => p.Partgroup_ID == partgroupId).FirstOrDefault();
                
        //        if (runningSerialCounter >= Math.Pow(10, Convert.ToInt32(mm_serialNumberConfig.Series_Count)))
        //        {
        //            runningSerialCounter = 1;
        //        }
        //        else if (DateTime.Today.AddDays(-1).Year != DateTime.Today.Year)
        //        {
        //            runningSerialCounter = 1;
        //        }
        //        return runningSerialCounter;

        //    }
        //    catch (Exception exp)
        //    {
        //        General genObj = new General();
        //        genObj.addControllerException(exp, "Helper/General", "getCurrentRunningSerial(partNo: " + partNo + ") ");
        //    }
        //    return 0;
        //}

        
        public void logUserActivity(decimal? shopID, decimal? lineID, String moduleName, String remarks, DateTime startTime, DateTime? endTime, decimal userID, string userHost)
        {
            try
            {
                RS_UserActivity_Log activityLog = new RS_UserActivity_Log();
                activityLog.Inserted_User_ID = userID;
                activityLog.Inserted_Host = userHost;
                activityLog.Start_Time = startTime;
                activityLog.End_Time = endTime;
                activityLog.Module_Name = moduleName;
                activityLog.Remarks = remarks;
                activityLog.Shop_ID = shopID;
                activityLog.Line_ID = lineID;
                db.RS_UserActivity_Log.Add(activityLog);
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                General genObj = new General();
                genObj.addControllerException(exp, "Helper/General", "logUserActivity(ShopID: " + shopID + ", lineID: " + lineID + ", Modul Name: " + moduleName + ",Remark: " + remarks + ") ");
            }
        }

        public bool addLockTable(int plantId, int shopId, int lineId, String tableName, String hostName, int userId)
        {
            try
            {
                RS_Lock_Table mmLockTableObj = new RS_Lock_Table();

                mmLockTableObj.Plant_ID = plantId;
                mmLockTableObj.Shop_ID = shopId;
                mmLockTableObj.Line_ID = lineId;
                mmLockTableObj.Table_Name = tableName;
                mmLockTableObj.Inserted_Host = hostName;
                mmLockTableObj.Inserted_Date = DateTime.Now;
                mmLockTableObj.Inserted_User_ID = userId;
                db.RS_Lock_Table.Add(mmLockTableObj);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool isTableLock(int lineId, String tableName)
        {
            try
            {
                RS_Lock_Table mmLockTableObj = db.RS_Lock_Table.Where(p => p.Line_ID == lineId && p.Table_Name == tableName && (p.Updated_Date == null))
                                                 .OrderByDescending(p => p.Inserted_Date).FirstOrDefault();
                if (mmLockTableObj == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool updateLockTable(int plantId, int shopId, int lineId, String tableName, String hostName, int userId)
        {
            try
            {
                RS_Lock_Table[] mmLockTableObj = db.RS_Lock_Table.Where(p => p.Line_ID == lineId && p.Table_Name == tableName && p.Updated_Date == null).OrderByDescending(p => p.Inserted_Date).ToArray();
                mmLockTableObj[0].Updated_Date = DateTime.Now;
                mmLockTableObj[0].Updated_User_ID = userId;
                db.Entry(mmLockTableObj[0]).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool addPurgeDeletedRecords(int plantId, String tableName, String columnName, String columnValue, String hostName, int userId)
        {
            try
            {
                RS_Purge_Deleted_Records purgeDeletedRecordsobj = new RS_Purge_Deleted_Records();
                purgeDeletedRecordsobj.Plant_ID = plantId;
                purgeDeletedRecordsobj.Table_Name = tableName;
                purgeDeletedRecordsobj.Column_Name = columnName;
                purgeDeletedRecordsobj.Column_Value = columnValue;
                purgeDeletedRecordsobj.Inserted_Host = hostName;
                purgeDeletedRecordsobj.Inserted_User_ID = userId;
                purgeDeletedRecordsobj.Inserted_Date = DateTime.Now;
                db.RS_Purge_Deleted_Records.Add(purgeDeletedRecordsobj);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int updatePlannedRSN(int orderID, int userID)
        {
            try
            {
                DateTime todayDate = DateTime.Today;
                RS_OM_Planned_Orders omPlannedOrdr = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == orderID && a.Planned_Date == todayDate).FirstOrDefault();
                if (omPlannedOrdr != null)
                {
                    int newRSN = 0;
                    //RS_OM_ShopFloor_Holded_RSN holdedOrderObj = db.RS_OM_ShopFloor_Holded_RSN.Where(a => a.Order_ID == orderID).FirstOrDefault();
                    RS_OM_ShopFloor_Holded_RSN holdedOrderObj = db.RS_OM_ShopFloor_Holded_RSN.Where(a => a.Order_ID == orderID && DbFunctions.TruncateTime(a.Inserted_Date) == todayDate).FirstOrDefault();

                    if (holdedOrderObj != null)
                    {
                        //RETURN PRE SAVED RSN ID ORDER WAS HOLDED PREVIOUSLY
                        newRSN = holdedOrderObj.RSN;
                    }
                    else
                    {
                        //RETURN THE NEW RSN IF ORDER IS NOT STARTED OR HOLDED ALREADY
                        List<int> orderIDList = db.RS_OM_ShopFloor_Holded_RSN.Where(a => DbFunctions.TruncateTime(a.Inserted_Date) == todayDate).Select(a => a.Order_ID).ToList();
                        int totalStarted = db.RS_OM_Planned_Orders.Count(a => a.Order_Status == "Started" && a.Planned_Date == todayDate && a.Shop_ID == omPlannedOrdr.Shop_ID);
                        int totalHolded = db.RS_OM_Planned_Orders.Count(a => orderIDList.Contains(a.Order_ID.Value) && a.Planned_Date == todayDate && a.Shop_ID == omPlannedOrdr.Shop_ID);
                        newRSN = totalHolded + totalStarted + 1;
                    }
                    //SAVE RSN IN PLANNED ORDER TABLE
                    omPlannedOrdr.RSN = newRSN;
                    db.Entry(omPlannedOrdr).State = EntityState.Modified;
                    db.SaveChanges();
                    return newRSN;
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                addControllerException(ex, "Helper/General", "updatePlannedRSN(orderID : " + orderID + ")", userID);
            }
            return 0;
        }

        public int getCurrentRunningSerial(string partNo, int userid)
        {
            try
            {
                partNo = partNo.Trim();
                RS_Serial_Number_Data mm_SerialNumberData = db.RS_Serial_Number_Data.Where(a => a.Part_No.Trim() == partNo).FirstOrDefault();
                RS_Serial_Number_Configuration mm_serialNumberConfig = db.RS_Model_Master.Find(partNo).RS_Serial_Number_Configuration;
                int runningSerialCounter = mm_SerialNumberData.Running_Serial.GetValueOrDefault(0);
                runningSerialCounter++;
                if (runningSerialCounter >= Math.Pow(10, Convert.ToInt32(mm_serialNumberConfig.Series_Count)))
                {
                    runningSerialCounter = 1;
                }
                else if (DateTime.Today.AddDays(-1).Year != DateTime.Today.Year)
                {
                    runningSerialCounter = 1;
                }
                return runningSerialCounter;

            }
            catch (Exception exp)
            {
                General genObj = new General();
                genObj.addControllerException(exp, "Helper/General", "getCurrentRunningSerial(partNo: " + partNo + ") ", userid);
            }
            return 0;
        }

        public void logLineStopData(decimal plantID, decimal shopID, decimal lineID, decimal stationID, string reason, string remarks, decimal? EfPartGroupID = null)
        {
            bool isSameCallAreadyGiven = db.RS_History_LineStop.Any(a => a.isLineStop == true && a.Resume_Time == null && a.Stop_Reason == reason && a.Line_ID == lineID && a.Station_ID == stationID);
            if (!isSameCallAreadyGiven)
            {
                DateTime dateTimeNow;
                TrackingFields dateObj = db.Database.SqlQuery<TrackingFields>("SELECT GETDATE() AS todayDate").FirstOrDefault();
                if (dateObj != null)
                {
                    dateTimeNow = dateObj.todayDate;
                }
                else
                {
                    dateTimeNow = DateTime.Now;
                }

                RS_History_LineStop lineStopObj = new RS_History_LineStop();
                lineStopObj.Plant_ID = plantID;
                lineStopObj.Shop_ID = shopID;
                lineStopObj.Line_ID = lineID;
                lineStopObj.Station_ID = stationID;
                lineStopObj.Stop_Reason = reason;
                lineStopObj.Remarks = remarks;
                lineStopObj.Stop_Time = dateTimeNow;
                lineStopObj.isLineStop = true;
                lineStopObj.isHeartBit = false;
                lineStopObj.isEmergencyCall = false;
                lineStopObj.EFPartGroup_ID = EfPartGroupID;
                lineStopObj.PLC_Ack = false;
                lineStopObj.Inserted_Date = DateTime.Now;
                db.RS_History_LineStop.Add(lineStopObj);
                db.SaveChanges();

                //lineStopObj.Plant_ID = plantID;
                //lineStopObj.Shop_ID = shopID;
                //lineStopObj.Line_ID = lineID;
                //lineStopObj.Station_ID = stationID;
                //lineStopObj.Stop_Time = dateTimeNow;
                //lineStopObj.Stop_Reason = reason;
                //lineStopObj.Resume_Time = DateTime.Now;
                //lineStopObj.Remarks = remarks;
                //lineStopObj.Owner = "";
                //lineStopObj.PrimaryOwner_Id = 0;
                //lineStopObj.SecondaryOwner_Id = 0;
                //lineStopObj.Line_Stop_By = "";
                //lineStopObj.Status = "";
                //lineStopObj.isLineStop = true;
                //lineStopObj.isEmergencyCall = false;
                //lineStopObj.isHeartBit = false;
                //lineStopObj.EFPartGroup_ID = EfPartGroupID;
                //lineStopObj.PLC_Ack = false;
                //lineStopObj.Inserted_Date = DateTime.Now;
                //lineStopObj.Is_Transfered = false;
                //lineStopObj.Is_Purgeable = false;
                //lineStopObj.Is_Edited = false;


            }
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("IsCallToIoT")))
            {
                Kepware kepware = new Kepware();
                kepware.linePause(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "1");
                kepware.lineResume(Convert.ToInt32(lineID), Convert.ToInt32(stationID), "0");
            }

        }


    }
}