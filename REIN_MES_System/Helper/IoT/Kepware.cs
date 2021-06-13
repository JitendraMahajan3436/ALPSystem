using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REIN_MES_System.Helper;
using REIN_MES_System.Helper.IoT;

namespace REIN_MES_System.Helper.IoT
{
    public class Kepware
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        //private MTTUWEntities db1 = new MTTUWEntities();
        String tagName = "";



        General generalObj = new General();
        public WriteResults linePause(int shopId, int lineId, String tagValue)
        {
            try
            {
                RS_IoT_Tags mmTags = db.RS_IoT_Tags.Where(p => p.Line_ID == lineId && p.Is_Line_Stop == true).Single();
                tagName = mmTags.Tag_Name;

                //tagValue = "1";
                IoTProcess iotProcessObj = new IoTProcess();
                WriteIoT[] writeIoTObj = new WriteIoT[1];
                writeIoTObj[0] = new WriteIoT();
                writeIoTObj[0].id = tagName;
                writeIoTObj[0].v = tagValue;
                WriteResults[] writeResultObj = iotProcessObj.writePLCTag(writeIoTObj,shopId);

                //lineResume(lineId, "0");
                return writeResultObj[0];
            }
            catch (Exception ex)
            {

                generalObj.addMetaException(ex, "Kepware", "linePause(" + lineId + ", " + tagValue + ")", 1);
                return null;
            }
        }

        public WriteResults linePause(int shopId, int lineId, int stationId, String tagValue)
        {
            try
            {
                RS_IoT_Tags mmTags = db.RS_IoT_Tags.Where(p => p.Line_ID == lineId && p.Station_ID == stationId && p.Is_Line_Stop == true).Single();
                tagName = mmTags.Tag_Name;

                //tagValue = "1";
                IoTProcess iotProcessObj = new IoTProcess();
                WriteIoT[] writeIoTObj = new WriteIoT[1];
                writeIoTObj[0] = new WriteIoT();
                writeIoTObj[0].id = tagName;
                writeIoTObj[0].v = tagValue;
                WriteResults[] writeResultObj = iotProcessObj.writePLCTag(writeIoTObj, shopId);

                //lineResume(lineId,stationId, "0");
                return writeResultObj[0];
            }
            catch (Exception ex)
            {

                generalObj.addMetaException(ex, "Kepware", "linePause(" + lineId + "," + stationId + ", " + tagValue + ")", 1);
                return null;
            }
        }

        public WriteResults machinePause(int shopId, decimal machineID, String tagValue)
        {
            try
            {
                RS_IoT_Tags mmTags = db.RS_IoT_Tags.Where(p => p.Machine_ID == machineID && p.Is_Line_Stop == true).FirstOrDefault();
                if (mmTags != null)
                {
                    tagName = mmTags.Tag_Name;

                    //tagValue = "1";
                    IoTProcess iotProcessObj = new IoTProcess();
                    WriteIoT[] writeIoTObj = new WriteIoT[1];
                    writeIoTObj[0] = new WriteIoT();
                    writeIoTObj[0].id = tagName;
                    writeIoTObj[0].v = tagValue;
                    WriteResults[] writeResultObj = iotProcessObj.writePLCTag(writeIoTObj,shopId);

                    //lineResume(lineId,stationId, "0");
                    return writeResultObj[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                generalObj.addMetaException(ex, "Kepware", "machinePause(Machine ID:" + machineID + "," + tagValue + ")", 1);
                return null;
            }
        }

        public WriteResults machineResume(int shopId, decimal machineID, String tagValue)
        {
            try
            {
                RS_IoT_Tags mmTags = db.RS_IoT_Tags.Where(p => p.Machine_ID == machineID && p.Is_Line_Resume == true).FirstOrDefault();
                if (mmTags != null)
                {
                    tagName = mmTags.Tag_Name;

                    //tagValue = "1";
                    IoTProcess iotProcessObj = new IoTProcess();
                    WriteIoT[] writeIoTObj = new WriteIoT[1];
                    writeIoTObj[0] = new WriteIoT();
                    writeIoTObj[0].id = tagName;
                    writeIoTObj[0].v = tagValue;
                    WriteResults[] writeResultObj = iotProcessObj.writePLCTag(writeIoTObj,shopId);

                    //linePause(lineId,stationId, "0");
                    return writeResultObj[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                generalObj.addMetaException(ex, "Kepware", "machineResume(Machine ID:" + machineID + "," + tagValue + ")", 1);
                return null;
            }
        }

        public WriteResults lineResume(int shopId, int lineId, String tagValue)
        {
            try
            {
                RS_IoT_Tags mmTags = db.RS_IoT_Tags.Where(p => p.Line_ID == lineId && p.Is_Line_Resume == true).Single();
                tagName = mmTags.Tag_Name;

                //tagValue = "1";
                IoTProcess iotProcessObj = new IoTProcess();
                WriteIoT[] writeIoTObj = new WriteIoT[1];
                writeIoTObj[0] = new WriteIoT();
                writeIoTObj[0].id = tagName;
                writeIoTObj[0].v = tagValue;
                WriteResults[] writeResultObj = iotProcessObj.writePLCTag(writeIoTObj,shopId);

                //linePause(lineId, "0");
                return writeResultObj[0];
            }
            catch (Exception ex)
            {

                generalObj.addMetaException(ex, "Kepware", "lineResume(" + lineId + ", " + tagValue + ")", 1);
                return null;
            }
        }

        public ReadResults[] ReadCBMData(List<string> IOTs, decimal shopId)
        {
            
            try
            {
                string[] writeIoTObj = new string[IOTs.Count];
                int i = 0;
                // MM_MT_Conditional_Based_Maintenance mmTags = db1.MM_MT_Conditional_Based_Maintenance.Where(p => p.CBM_ID == CBMID).Single();
                foreach (var item in IOTs)
                {
                    tagName = item;
                    writeIoTObj[i] = tagName;
                    i++;
                }


                //tagValue = "1";
                IoTProcess iotProcessObj = new IoTProcess();
                // string[] writeIoTObj = new string[1];
                // writeIoTObj[0] = tagName;

                ReadResults[] writeResultObj = iotProcessObj.readPLCTag(writeIoTObj);

                //linePause(lineId,stationId, "0");
                return writeResultObj;
            }
            catch (Exception ex)
            {

                generalObj.addMetaException(ex, "Kepware", "ReadCBMData(" + tagName + ")", 1);
                return null;
            }
        }
        public WriteResults lineResume(int shopId, int lineId, int stationId, String tagValue)
        {
            try
            {
                RS_IoT_Tags mmTags = db.RS_IoT_Tags.Where(p => p.Line_ID == lineId && p.Station_ID == stationId && p.Is_Line_Resume == true).Single();
                tagName = mmTags.Tag_Name;

                //tagValue = "1";
                IoTProcess iotProcessObj = new IoTProcess();
                WriteIoT[] writeIoTObj = new WriteIoT[1];
                writeIoTObj[0] = new WriteIoT();
                writeIoTObj[0].id = tagName;
                writeIoTObj[0].v = tagValue;
                WriteResults[] writeResultObj = iotProcessObj.writePLCTag(writeIoTObj,shopId);

                //linePause(lineId,stationId, "0");
                return writeResultObj[0];
            }
            catch (Exception ex)
            {

                generalObj.addMetaException(ex, "Kepware", "lineResume(" + lineId + "," + stationId + ", " + tagValue + ")", 1);
                return null;
            }
        }


        public ReadResults lineResumeRead(int lineId, int stationId)
        {
            try
            {
                RS_IoT_Tags mmTags = db.RS_IoT_Tags.Where(p => p.Line_ID == lineId && p.Station_ID == stationId && p.Is_Line_Resume == true).Single();
                tagName = mmTags.Tag_Name;

                //tagValue = "1";
                IoTProcess iotProcessObj = new IoTProcess();
                string[] writeIoTObj = new string[1];
                writeIoTObj[0] = tagName;

                ReadResults[] writeResultObj = iotProcessObj.readPLCTag(writeIoTObj);

                //linePause(lineId,stationId, "0");
                return writeResultObj[0];
            }
            catch (Exception ex)
            {

                generalObj.addMetaException(ex, "Kepware", "lineResumeRead(" + lineId + "," + stationId + ")", 1);
                return null;
            }
        }
        public ReadResults linePauseRead(int lineId, int stationId)
        {
            try
            {
                RS_IoT_Tags mmTags = db.RS_IoT_Tags.Where(p => p.Line_ID == lineId && p.Station_ID == stationId && p.Is_Line_Stop == true).Single();
                tagName = mmTags.Tag_Name;

                //tagValue = "1";
                IoTProcess iotProcessObj = new IoTProcess();
                string[] writeIoTObj = new string[1];

                writeIoTObj[0] = tagName;

                ReadResults[] writeResultObj = iotProcessObj.readPLCTag(writeIoTObj);

                //lineResume(lineId,stationId, "0");
                return writeResultObj[0];
            }
            catch (Exception ex)
            {

                generalObj.addMetaException(ex, "Kepware", "linePauseRead(" + lineId + "," + stationId + ")", 1);
                return null;
            }
        }
    }
}