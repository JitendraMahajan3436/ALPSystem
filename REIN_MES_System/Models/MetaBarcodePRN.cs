using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REIN_MES_System.Models
{
    [MetadataType(typeof(MetaBarcodePRN))]
    public partial class RS_PRN_Files
    {
        private REIN_SOLUTIONEntities  db = new REIN_SOLUTIONEntities();

        //[Required]
        //[DataType(DataType.Upload)]
        //public HttpPostedFileBase PRN_File { get; set; }

        public bool isBarcodePRNFileExists(String prnName, int plantId, int prnId)
        {
            try
            {
                IQueryable<RS_PRN_Files> result;
                if (prnId == 0)
                {
                    result = db.RS_PRN_Files.Where(p => p.Plant_ID == plantId && p.PRN_Name == prnName);
                }
                else
                {
                    result = db.RS_PRN_Files.Where(p => p.Plant_ID == plantId && p.PRN_Name == prnName && p.PRN_ID != prnId);
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

        public String getPRNFileName(int stationId, bool isOrderStart = false, bool isQualityOK = false)
        {
            try
            {
                RS_PRN_Files mmPRNFilesObj = null;
                if (isOrderStart == true)
                {
                    mmPRNFilesObj = (from mmPRNFiles in db.RS_PRN_Files
                                     where (from mmPRNStationObj in db.RS_PRN_Station where mmPRNStationObj.Is_Order_Start == true && mmPRNStationObj.Station_ID == stationId select mmPRNStationObj.PRN_ID).Contains(mmPRNFiles.PRN_ID)
                                     select mmPRNFiles).Single();
                }
                else
                    if (isQualityOK == true)
                        mmPRNFilesObj = (from mmPRNFiles in db.RS_PRN_Files
                                         where (from mmPRNStationObj in db.RS_PRN_Station where mmPRNStationObj.Is_Final_Quality_OK == true && mmPRNStationObj.Station_ID == stationId select mmPRNStationObj.PRN_ID).Contains(mmPRNFiles.PRN_ID)
                                         select mmPRNFiles).Single();

                if (mmPRNFilesObj != null)
                {
                    return mmPRNFilesObj.File_Name;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
    public class MetaBarcodePRN
    {
    }
}