using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Models
{

    public partial class RS_PartID_Station
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        public int[] selectedPartID { get; set; }
        General generalObj = new General();
        public bool deleteOperator(int plantId, int shopId, int lineId, int StationID) //int stationId, int plantId,
        {
            try
            {
                var operatorList = db.RS_PartID_Station.Where(p => p.Station_ID == StationID && p.Shop_ID == shopId && p.Line_ID == lineId && p.Plant_ID == plantId);//&& p.Plant_ID == plantId
                foreach (var item in operatorList.ToList())
                {
                    db.RS_PartID_Station.Remove(item);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    public class MetaPartIDStationConfig
    {
    }
}