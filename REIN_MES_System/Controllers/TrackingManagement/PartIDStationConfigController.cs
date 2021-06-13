using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class PartIDStationConfigController : Controller
    {
        // GET: PartIDStationConfig

        FDSession fdSession = new FDSession();
        General general = new General();
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        RS_PartID_Station mmpartIdStationObj = new RS_PartID_Station();
        //String PartID = "";
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0;
        General generalObj = new General();

        

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
                ViewBag.Shop_ID = new SelectList("");
                ViewBag.Line_ID = new SelectList("");
                ViewBag.station_ID = new SelectList("");

            }
            else
            {
                ViewBag.station_ID = new SelectList(db.RS_Stations.Where(p => p.Station_ID == stationId), "Station_ID", "Station_Name");
                //ViewBag.ListofSupervisor = new SelectList(db.RS_Station_Setup_Mapping, "Station_ID", "Station_Name");
                //ViewBag.AssignedSupervisor_ID = new SelectList(db.RS_Station_Setup_Mapping, "Station_ID", "Station_Name");

            }
            globalData.pageTitle = ResourceModules.PartIDStationConfig;
            globalData.controllerName = "PartIDStationConfig";
            globalData.actionName = ResourceGlobal.Create;
        

            ViewBag.GlobalDataModel = globalData;
            //ViewBag.STS_ID = new SelectList(db.RS_Station_Setup_Mapping, "Station_Setup_ID", "Station_Setup_ID");
            //ViewBag.STS_ID = new SelectList(db.RS_Station_Setup_Mapping, "Station_Setup_ID", "Station_Setup_ID");
       

            ViewBag.Plant_ID = plantId;
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(c => c.Plant_ID == plantId), "Shop_ID", "Shop_Name" );
            ViewBag.Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name");
            return View();
        }
        public ActionResult GetLineListByShopId(int shopId)
        {
            try
            {
                decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                var st = from line in db.RS_Lines
                         where line.Shop_ID == shopId
                         orderby line.Line_Name
                         select new
                         {
                             Id = line.Line_ID,
                             Value = line.Line_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSetupListByLineId(int lineId)
        {
            try
            {

                var st = from s in db.RS_Stations
                         where s.Line_ID == lineId
                         orderby s.Station_Name
                         select new
                         {
                             Id = s.Station_ID,
                             Value = s.Station_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPartIDBySatationID(int StationId, int? shopId, int? lineId, int? plantId)//
        {
            try
            {

                var st = (from partId in db.RS_PartID
                          where !(from SM in db.RS_PartID_Station
                                  where SM.Station_ID == StationId
                                  select SM.PartID)
              .Contains(partId.RowID)

                          where partId.Active== true
                          orderby partId.PartID
                          select new
                          {
                              Id = partId.RowID,
                              Value = partId.PartID,
                              Desc = partId.PartIDDescription
                          }).Distinct();


                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAssignedPartIDByStationID(int StationId, int shopId, int lineId, int plantId)
        {

            try
            {
                var st = from part in db.RS_PartID
                         where (from parttostation in db.RS_PartID_Station
                                where parttostation.Station_ID == StationId && parttostation.Shop_ID == shopId &&
                                parttostation.Line_ID == lineId && parttostation.Plant_ID == plantId
                                select parttostation.PartID).Contains(part.RowID)
                         orderby part.PartID
                         select new
                         {
                             Id = part.RowID,
                             Value = part.PartID,
                             Desc = part.PartIDDescription
                         };


                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CheckAssignedPartIds(string PartIDList, int StationId, int shopId, int lineId, int plantId)
        {
            List<CheckAssignPartId> CheckartId = new List<CheckAssignPartId>();
            string[] words;
            words = PartIDList.Split(',');
            foreach (var value in words)
            {
               

                decimal Part_ID = Convert.ToDecimal(value);
                var checkPart = (from m in db.RS_PartID_Station
                                 join
                                 s in db.RS_Stations on
                                 m.Station_ID equals s.Station_ID
                                 join
                                 p in db.RS_PartID on
                                 m.PartID equals p.RowID
                                 where !(from SM in db.RS_PartID_Station
                                         where SM.Station_ID == StationId && SM.PartID == Part_ID
                                         select SM.PartID).Contains(m.PartID)
                                 where m.PartID == Part_ID
                                 select new
                                 {
                                     p.PartID,
                                     s.Station_Name
                                 }).FirstOrDefault();

                if(checkPart!= null)
                {
                    CheckAssignPartId obj = new CheckAssignPartId(checkPart.PartID, checkPart.Station_Name);
                    CheckartId.Add(obj);

                }
                else
                {

                }

            }
            return Json(CheckartId, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveAssignedPartIds(string PartIDList, int StationId, int shopId, int lineId, int plantId)
        {
            RS_PartID_Station obj = new RS_PartID_Station();

            obj.deleteOperator(plantId, shopId, lineId, StationId);  


            string[] words;
            words = PartIDList.Split(',');

            foreach (string value in words)
            {
                decimal Part_ID = Convert.ToDecimal(value);
                var checkPart = (from m in db.RS_PartID_Station
                                 join
                                 s in db.RS_Stations on
                                 m.Station_ID equals s.Station_ID
                                 join
                                 p in db.RS_PartID on
                                 m.PartID equals p.RowID
                                 where m.PartID == Part_ID
                                 select new
                                 { 
                                     m.RowID,
                                     p.PartID,
                                     s.Station_Name
                                 }).FirstOrDefault();
                if (checkPart != null)
                {
                    RS_PartID_Station mmPartsataion = db.RS_PartID_Station.Find(checkPart.RowID);
                    db.RS_PartID_Station.Remove(mmPartsataion);
                    db.SaveChanges();
                }
                else
                {

                }

                int i = 0;
                if (value == "")
                {
                    i = 0;
                }
                else
                {
                    i = Convert.ToInt32(value);
                }
                if (i == 0)
                    continue;




                obj.Plant_ID = plantId;
                obj.Shop_ID = shopId;
                obj.Line_ID = lineId;
                obj.Inserted_Date = DateTime.Now;
                obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                obj.PartID = i;
                obj.Station_ID = StationId;


                db.Entry(obj).State = EntityState.Detached;
                db.RS_PartID_Station.Add(obj);
                db.SaveChanges();


                i = 0;
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NoallredyAssignedPartIds(string PartIDList, int StationId, int shopId, int lineId, int plantId)
        {
            RS_PartID_Station obj = new RS_PartID_Station();

            obj.deleteOperator(plantId, shopId, lineId, StationId);

            int i = 0;
            string[] words;
            words = PartIDList.Split(',');

            foreach (string value in words)
            {
                decimal Part_ID = Convert.ToDecimal(value);
                var checkPart = (from m in db.RS_PartID_Station
                                 join
                                 s in db.RS_Stations on
                                 m.Station_ID equals s.Station_ID
                                 join
                                 p in db.RS_PartID on
                                 m.PartID equals p.RowID
                                 where m.PartID == Part_ID
                                 select new
                                 {
                                     m.RowID,
                                     p.PartID,
                                     s.Station_Name
                                 }).FirstOrDefault();
                if (checkPart != null)
                {
                    //RS_PartID_Station mmPartsataion = db.RS_PartID_Station.Find(checkPart.RowID);
                    //db.RS_PartID_Station.Remove(mmPartsataion);
                    //db.SaveChanges();
                }
                else
                {
                    
                    if (value == "")
                    {
                        i = 0;
                    }
                    else
                    {
                        i = Convert.ToInt32(value);
                    }
                    if (i == 0)
                        continue;




                    obj.Plant_ID = plantId;
                    obj.Shop_ID = shopId;
                    obj.Line_ID = lineId;
                    obj.Inserted_Date = DateTime.Now;
                    obj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    obj.PartID = i;
                    obj.Station_ID = StationId;


                    db.Entry(obj).State = EntityState.Detached;
                    db.RS_PartID_Station.Add(obj);
                    db.SaveChanges();
                   
                }

                i = 0;



            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}