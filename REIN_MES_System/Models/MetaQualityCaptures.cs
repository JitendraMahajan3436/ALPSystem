using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaQualityCaptures))]
    public partial class RS_Quality_Captures
    {
        //

        General generalHelper = new General();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        int currentStationId = 0;
        bool isQPStationFound = false;
        bool isFirstQualityStation = false;
        RS_Route_Configurations routeConfigurationObj = new RS_Route_Configurations();
        public RS_Shift getCurrentRunningShiftByShopID(int shopId)
        {
            try
            {
                TimeSpan currDate = DateTime.Now.TimeOfDay;
                RS_Shift mmShiftObj = (from shiftObj in db.RS_Shift
                                       where shiftObj.Shop_ID == shopId
                                && TimeSpan.Compare(shiftObj.Shift_Start_Time, currDate) < 0
                                && TimeSpan.Compare(shiftObj.Shift_End_Time, currDate) > 0
                                       select shiftObj).Single();
                return mmShiftObj;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "getCurrentRunningShiftByShopID() " + shopId, 1);
                return null;
            }
        }

        public int getDefectId(String defectName)
        {
            try
            {
                int defectId = Convert.ToInt16(defectName);
                RS_Quality_Defect mmQualityDefectObj = db.RS_Quality_Defect.Where(p => p.Defect_ID == defectId).Single();
                return Convert.ToInt32(mmQualityDefectObj.Defect_ID);
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "getDefectId() " + defectName, 1);
                return 0;
            }
        }

        public int getCorrectiveActionsId(String actionName)
        {
            try
            {
                int actionId = Convert.ToInt16(actionName);
                RS_Quality_Corrective_Actions mmQualityCorrectiveActionsObj = db.RS_Quality_Corrective_Actions.Where(p => p.Corrective_Action_ID == actionId).Single();
                return Convert.ToInt32(mmQualityCorrectiveActionsObj.Corrective_Action_ID);
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "getCorrectiveActionsId() " + actionName, 1);
                return 0;
            }
        }


        public bool isProductionBookingCompleted(String serialNo)
        {
            try
            {
                RS_SAP_Production_Booking mmProductionBookingObj = db.RS_SAP_Production_Booking.Where(p => p.Serial_No == serialNo).Single();
                if (mmProductionBookingObj != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "isProductionBookingCompleted()- " + serialNo, 1);
                return false;
            }
        }

        public bool isEquipmentCreationCompleted(String serialNo)
        {
            try
            {
                RS_SAP_Equipment_Creation mmEquipmentCreateObj = db.RS_SAP_Equipment_Creation.Where(p => p.Serial_No == serialNo).Single();
                if (mmEquipmentCreateObj != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "isEquipmentCreationCompleted()- " + serialNo, 1);
                return false;
            }
        }
        public bool isQualityOkOrderAdded(String serialNo)
        {
            try
            {
                RS_Quality_OK_Order mmQualityOkOrderObj = db.RS_Quality_OK_Order.Where(p => p.Serial_No == serialNo).Single();
                if (mmQualityOkOrderObj != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "isQualityOK()- " + serialNo, 1);
                return false;

            }
        }
        public bool updateOrderListStatus(String serialNumber)
        {
            try
            {
                RS_OM_Order_List mmOrderListObj = db.RS_OM_Order_List.Where(p => p.Serial_No == serialNumber).Single();
                mmOrderListObj.Order_Status = "QOK";
                db.Entry(mmOrderListObj).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "updateOrderListStatus()- " + serialNumber, 1);
                return false;
            }
        }

        public bool insertOrderInQualityOrderQueue(String serialNo, String orderNo, int plantId, int shopId, int userId, String hostName)
        {
            try
            {
                RS_Quality_Order_Queue mmQualityOrderQueueObj = new RS_Quality_Order_Queue();
                mmQualityOrderQueueObj.Plant_ID = plantId;
                mmQualityOrderQueueObj.Shop_ID = shopId;
                mmQualityOrderQueueObj.Order_No = orderNo;
                mmQualityOrderQueueObj.Serial_No = serialNo;
                mmQualityOrderQueueObj.Inserted_User_ID = userId;
                mmQualityOrderQueueObj.Inserted_Host = hostName;
                RS_Quality_Station_List[] mmQUalityStationListObj = db.RS_Quality_Station_List.Where(p => p.Plant_ID == plantId && p.Shop_ID == shopId).ToArray();
                for (int i = 0; i < mmQUalityStationListObj.Count(); i++)
                {
                    mmQualityOrderQueueObj.Line_ID = mmQUalityStationListObj[i].Line_ID;

                    mmQualityOrderQueueObj.Station_ID = mmQUalityStationListObj[i].Station_ID;

                    mmQualityOrderQueueObj.Quality_Station_ID = mmQUalityStationListObj[i].Station_ID;
                    mmQualityOrderQueueObj.Inserted_Date = DateTime.Now;
                    db.RS_Quality_Order_Queue.Add(mmQualityOrderQueueObj);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "insertOrderInQualityOrderQueue()- ", 1);
                return false;
            }
        }

        public bool updateQualityOrderQueueProcess(int stationId, String serialNo)
        {
            try
            {
                RS_Quality_Order_Queue mmQualityOrderQueueObj = db.RS_Quality_Order_Queue.Where(p => p.Station_ID == stationId && p.Serial_No == serialNo).Single();
                mmQualityOrderQueueObj.Is_Processed = true;
                mmQualityOrderQueueObj.Updated_Date = DateTime.Now;
                db.Entry(mmQualityOrderQueueObj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "updateQualityOrderQueueProcess()- stationID-" + stationId + " serialNO - " + serialNo, 1);
                return false;
            }
        }

        public String getNextSerialNumber(int stationId, int lineId, int shopId)
        {
            try
            {

                String serialNo = "";

                // process to get all the lines upto main line
                // process to get first station of line
                RS_Route_Configurations mmRouteConfigurationObj = new RS_Route_Configurations();

                while (true)
                {
                    currentStationId = this.getNextStation(stationId, lineId);

                    if (isQPStationFound == true)
                    {
                        // process to get serial processed in previous QP but not in this QP
                        RS_Quality_Order_Queue[] mmQualityOrderQueueObj = (from qualityOrderQueueObj in db.RS_Quality_Order_Queue
                                                                           where (from mmPreviousQPObj in db.RS_Quality_Order_Queue where mmPreviousQPObj.Station_ID == currentStationId && mmPreviousQPObj.Is_Processed == true select mmPreviousQPObj.Serial_No).Contains(qualityOrderQueueObj.Serial_No)
                                                                           && (qualityOrderQueueObj.Is_Processed == null || qualityOrderQueueObj.Is_Processed == false) && qualityOrderQueueObj.Station_ID == stationId
                                                                           select qualityOrderQueueObj).OrderBy(p => p.Inserted_Date).ToArray();
                        return serialNo = mmQualityOrderQueueObj[0].Serial_No;
                    }

                    if (isFirstQualityStation == true && !(this.isShopFirstLine(Convert.ToInt16(routeConfigurationObj.Line_ID))))
                    {
                        // process to get next conveyor line
                        RS_Lines[] mmLinesObj = (from linesObj in db.RS_Lines
                                                 where (from routeMarriageStationObj in db.RS_Route_Marriage_Station where routeMarriageStationObj.Marriage_Station_ID == currentStationId select routeMarriageStationObj.Sub_Line_ID).Contains(linesObj.Line_ID)
                                                 && linesObj.isPLC == true && linesObj.Is_Conveyor == true
                                                 select linesObj).ToArray();
                        if (mmLinesObj.Count() > 0)
                        {
                            lineId = Convert.ToInt16(mmLinesObj[0].Line_ID);
                            mmRouteConfigurationObj = db.RS_Route_Configurations.Where(p => p.Line_ID == lineId && p.Is_End_Station == true).Single();
                            stationId = Convert.ToInt16(mmRouteConfigurationObj.Station_ID);
                            continue;
                        }
                        else
                        {
                            // else no conveyor line or this is first QP
                        }
                    }

                    if (isFirstQualityStation == true && this.isShopFirstLine(Convert.ToInt16(routeConfigurationObj.Line_ID)))
                    {
                        // process to get first serial number
                        RS_Quality_Order_Queue[] mmQualityOrderQueueObj = db.RS_Quality_Order_Queue.Where(p => p.Station_ID == stationId && (p.Is_Processed == null || p.Is_Processed == false) && p.Is_Moved_From_Station == true).OrderBy(p => p.Inserted_Date).ToArray();
                        return serialNo = mmQualityOrderQueueObj[0].Serial_No;
                    }

                    if (isFirstQualityStation == true)
                    {
                        // no conveyor found
                        RS_Quality_Order_Queue[] mmQualityOrderQueueObj = db.RS_Quality_Order_Queue.Where(p => p.Station_ID == stationId && (p.Is_Processed == null || p.Is_Processed == false) && p.Is_Moved_From_Station == true).OrderBy(p => p.Inserted_Date).ToArray();
                        return serialNo = mmQualityOrderQueueObj[0].Serial_No;
                    }



                    //return serialNo;
                }


                // process to get the previous station QP                

                //currentStationId = this.getNextStation(stationId, lineId);

                //int routeStationId = 0;
                //if (isFirstQualityStation == true && !(this.isShopFirstLine(Convert.ToInt16(routeConfigurationObj.Line_ID))))
                //{
                //    // process to get next conveyor line
                //    RS_Lines[] mmLinesObj = (from linesObj in db.RS_Lines
                //                             where (from routeMarriageStationObj in db.RS_Route_Marriage_Station where routeMarriageStationObj.Marriage_Station_ID == currentStationId select routeMarriageStationObj.Sub_Line_ID).Contains(linesObj.Line_ID)
                //                             && linesObj.isPLC == true && linesObj.Is_Conveyor == true
                //                             select linesObj).ToArray();


                //    for(int i=0;i<mmLinesObj.Count();i++)
                //    {
                //        // process to get last station of line

                //        int routeLineId=Convert.ToInt16(mmLinesObj[i].Line_ID);
                //        routeConfigurationObj = db.RS_Route_Configurations.Where(p => p.Line_ID == routeLineId && p.Is_End_Station == true).SingleOrDefault();

                //        routeStationId = Convert.ToInt16(routeConfigurationObj.Station_ID);
                //        routeStationId = this.getNextStation(routeStationId, routeLineId);
                //    }



                //}


                //if(isFirstQualityStation==true && this.isShopFirstLine(Convert.ToInt16(routeConfigurationObj.Line_ID)))
                //{
                //    // process to get first serial number
                //    RS_Quality_Order_Queue []mmQualityOrderQueueObj = db.RS_Quality_Order_Queue.Where(p => p.Station_ID == stationId && (p.Is_Processed == null || p.Is_Processed == false) && p.Is_Moved_From_Station == true).OrderBy(p=>p.Inserted_Date).ToArray();
                //    serialNo = mmQualityOrderQueueObj[0].Serial_No;
                //}
                //else
                //    if(isQPStationFound==true)
                //    {
                //        // process to get serial processed in previous QP but not in this QP
                //        RS_Quality_Order_Queue mmQualityOrderQueueObj = (from qualityOrderQueueObj in db.RS_Quality_Order_Queue
                //                   where (from mmPreviousQPObj in db.RS_Quality_Order_Queue where mmPreviousQPObj.Station_ID == currentStationId && mmPreviousQPObj.Is_Processed == true select mmPreviousQPObj.Serial_No).Contains(qualityOrderQueueObj.Serial_No)
                //                   && (qualityOrderQueueObj.Is_Processed == null || qualityOrderQueueObj.Is_Processed == false)
                //                   select qualityOrderQueueObj).OrderBy(p => p.Inserted_Date).SingleOrDefault();
                //        serialNo = mmQualityOrderQueueObj.Serial_No;
                //    }
                //    else
                //    {
                //        // possiblity of more line connected
                //    }

                return serialNo;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "getNextSerialNumber()- ", 1);
                return null;
            }
        }

        public bool isShopFirstLine(int shopId)
        {
            try
            {
                RS_Lines mmLinesObj = db.RS_Lines.Where(p => p.Line_ID == Line_ID && p.Is_Shop_Line_Start == true).Single();
                if (mmLinesObj != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "isShopFirstLine() ", 1);
                return false;
            }
        }

        public int getNextStation(int stationId, int lineId)
        {
            try
            {
                currentStationId = stationId;
                while (true)
                {
                    try
                    {
                        // process to get previous stations
                        routeConfigurationObj = db.RS_Route_Configurations.Where(p => p.Next_Station_ID == currentStationId).SingleOrDefault();

                        // process to check quality station
                        RS_Quality_Station_List mmQualityStationListObj = db.RS_Quality_Station_List.Where(p => p.Station_ID == routeConfigurationObj.Station_ID).SingleOrDefault();
                        if (mmQualityStationListObj != null)
                        {
                            isQPStationFound = true;
                            currentStationId = Convert.ToInt16(routeConfigurationObj.Station_ID);
                            break;
                        }
                        else
                            if (routeConfigurationObj.Is_Start_Station == true)
                        {
                            // process to get the first FIFO serial number for that station
                            // this is first quality station of line
                            isFirstQualityStation = true;
                            currentStationId = Convert.ToInt16(routeConfigurationObj.Station_ID);

                            break;
                        }

                        currentStationId = Convert.ToInt16(routeConfigurationObj.Station_ID);

                    }
                    catch (Exception ex1)
                    {
                        generalHelper.addControllerException(ex1, "Quality_Capture", "getNextStation() ", 1);
                    }
                }

                return currentStationId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //sandip 28 April
        public int GetQualityCountByStationID(int Captured_Station_ID)
        {
            try
            {
                string conStr = ConfigurationManager.ConnectionStrings["FDEntities_SP"].ToString();
                int qcountt = 0;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetQualityCount", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Captured_Station_ID", Captured_Station_ID);
                        qcountt = (int)cmd.ExecuteScalar();
                    }
                }
                return qcountt;
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "Quality_Capture", "GetQualityCountByStationID() ", 1);
                return 0;
            }
        }

    }
    public class MetaQualityCaptures
    {
        [MaxLength(200)]
        public string Remark { get; set; }
    }
}