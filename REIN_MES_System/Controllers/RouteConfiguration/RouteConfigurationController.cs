using REIN_MES_System.Models;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;
using REIN_MES_System.Controllers.BaseManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net;
using System.Data.Entity.Infrastructure;

namespace REIN_MES_System.Controllers.RouteConfiguration
{
    /* Controller Name            : RouteConfigurationController
    *  Description                : This controller is used to define and configure the line with station
    *  Author, Timestamp          : Jitendra Mahajan       
    */
    public class RouteConfigurationController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        int plantId = 0, shopId = 0, lineId = 0, subShopId = 0, subLineId = 0, subStationId = 0, marriageShopId = 0, marriageLineId = 0, marriageStationId = 0;
        bool isStartStationAdded, isEndStationAdded;
        int startStationId = 0, endStationId = 0;

        String routeConfiguration = "";
        String routeConfigurationDisplay = "";
        GlobalData globalData = new GlobalData();
        RS_Route_Configurations routeConfigurationObj = new RS_Route_Configurations();
        RS_Route_Display routeDisplayObj = new RS_Route_Display();


        General generalObj = new General();

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : Index
        // Input Parameter      : None
        // Return Type          : ActionResult
        // Author & Time Stamp  : Jitendra Mahajan :: 04-Oct-2015
        // Description          : Action used to show the line, allow the user to configure line
        //////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // GET: /RouteConfiguration/
        public ActionResult Index()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId).ToList(), "Shop_ID", "Shop_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Line_ID == 0).ToList(), "Line_ID", "Line_Name");
            var stationItem = from station in db.RS_Stations
                              where !(from routeConfig in db.RS_Route_Configurations select routeConfig.Station_ID).Contains(station.Station_ID)
                              select station;

            ViewBag.Station_ID = new SelectList(db.RS_Stations.Where(p => p.Shop_ID == 0).ToList(), "Station_ID", "Station_Name");

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Route_Configuration;
            globalData.subTitle = ResourceGlobal.Index;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.Route_Configuration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceModules.Route_Configuration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRouteConfig(FormCollection form)
        {
            plantId = Convert.ToInt16(form["Plant_ID"].ToString());
            shopId = Convert.ToInt16(form["Shop_ID"].ToString());
            lineId = Convert.ToInt16(form["Line_ID"].ToString());
            startStationId = Convert.ToInt16(form["hdnStartStaionID"].ToString());
            endStationId = Convert.ToInt16(form["hdnEndStaionID"].ToString());

            routeConfiguration = form["hdnRouteConfiguration"].ToString();
            routeConfigurationDisplay = form["hdnRouteConfigurationDisplay"].ToString();

            List<RouteLine> routeLine = (List<RouteLine>)Newtonsoft.Json.JsonConvert.DeserializeObject(routeConfiguration, typeof(List<RouteLine>));

            if (routeLine == null)
            {
                // process to delete all the station configured for the line before processing
                routeConfigurationObj.deleteRouteConfigByLineId(lineId);

                // single station on line
                routeConfigurationObj = new RS_Route_Configurations();
                routeConfigurationObj.Plant_ID = plantId;
                routeConfigurationObj.Shop_ID = shopId;
                routeConfigurationObj.Line_ID = lineId;
                routeConfigurationObj.Station_ID = endStationId;
                routeConfigurationObj.Is_Start_Station = true;
                routeConfigurationObj.Is_End_Station = true;
                routeConfigurationObj.Inserted_Date = DateTime.Now;
                routeConfigurationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                routeConfigurationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.RS_Route_Configurations.Add(routeConfigurationObj);
                db.SaveChanges();
            }
            else
            if (routeLine.Count > 0)
            {
                // process to delete all the station configured for the line before processing
                routeConfigurationObj.deleteRouteConfigByLineId(lineId);

                for (int i = 0; i < routeLine.Count; i++)
                {
                    RouteLine obj = routeLine[i];
                    if (obj == null)
                    {
                        // empty record
                    }
                    else
                    {
                        // process to add the records
                        routeConfigurationObj = new RS_Route_Configurations();
                        routeConfigurationObj.Plant_ID = plantId;
                        routeConfigurationObj.Shop_ID = shopId;
                        routeConfigurationObj.Line_ID = lineId;
                        routeConfigurationObj.Station_ID = Convert.ToInt16(obj.currentStation);
                        routeConfigurationObj.Next_Station_ID = Convert.ToInt16(obj.nextStation);

                        if (startStationId == Convert.ToInt16(obj.currentStation))
                        {
                            routeConfigurationObj.Is_Start_Station = true;
                        }

                        if (endStationId == Convert.ToInt16(obj.currentStation))
                        {
                            routeConfigurationObj.Is_End_Station = true;
                        }

                        routeConfigurationObj.Inserted_Date = DateTime.Now;
                        routeConfigurationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        routeConfigurationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.RS_Route_Configurations.Add(routeConfigurationObj);
                        db.SaveChanges();
                    }
                }

                // process to save end station information

                routeConfigurationObj = new RS_Route_Configurations();
                routeConfigurationObj.Plant_ID = plantId;
                routeConfigurationObj.Shop_ID = shopId;
                routeConfigurationObj.Line_ID = lineId;
                routeConfigurationObj.Station_ID = endStationId;
                routeConfigurationObj.Is_End_Station = true;
                routeConfigurationObj.Inserted_Date = DateTime.Now;
                routeConfigurationObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                routeConfigurationObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.RS_Route_Configurations.Add(routeConfigurationObj);
                db.SaveChanges();

            }

            // process to delete the route display
            routeConfigurationObj.deleteRouteConfigurationDisplayByLineId(lineId);

            List<RouteDisplay> routeDisplay = (List<RouteDisplay>)Newtonsoft.Json.JsonConvert.DeserializeObject(routeConfigurationDisplay, typeof(List<RouteDisplay>));

            if (routeDisplay != null)
            {
                for (int i = 0; i < routeDisplay.Count; i++)
                {
                    RouteDisplay displayObj = routeDisplay[i];
                    if (displayObj != null)
                    {
                        routeDisplayObj = new RS_Route_Display();
                        routeDisplayObj.Plant_ID = plantId;
                        routeDisplayObj.Shop_ID = shopId;
                        routeDisplayObj.Line_ID = lineId;
                        routeDisplayObj.Shop_ID = shopId;
                        routeDisplayObj.Station_ID = Convert.ToInt16(displayObj.Station_ID);
                        routeDisplayObj.Row_ID = Convert.ToInt16(displayObj.Row_ID);

                        routeDisplayObj.Col_ID = Convert.ToInt16(displayObj.Col_ID);

                        if (displayObj.Is_Up_Arrow == "1")
                            routeDisplayObj.Is_Up_Arrow = true;

                        if (displayObj.Is_Down_Arrow == "1")
                            routeDisplayObj.Is_Down_Arrow = true;

                        if (displayObj.Is_Left_Arrow == "1")
                            routeDisplayObj.Is_Left_Arrow = true;

                        if (displayObj.Is_Right_Arrow == "1")
                            routeDisplayObj.Is_Right_Arrow = true;

                        if (displayObj.Is_Start_Line == "1")
                            routeDisplayObj.Is_Start_Line = true;

                        if (displayObj.Is_Stop_Line == "1")
                            routeDisplayObj.Is_Stop_Line = true;

                        RS_Stations stnObject = new RS_Stations();
                        if (routeDisplayObj.Station_ID > 0)
                        {
                            stnObject = db.RS_Stations.Find(routeDisplayObj.Station_ID);
                            routeDisplayObj.Station_Name = stnObject.Station_Name;
                        }
                        else
                        {
                            //routeDisplayObj.Station_Name = "";
                        }

                        routeDisplayObj.Inserted_Date = DateTime.Now;
                        routeDisplayObj.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                        routeDisplayObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                        db.RS_Route_Display.Add(routeDisplayObj);
                        db.SaveChanges();
                    }

                }
            }

            globalData.pageTitle = ResourceModules.Route_Configuration;
            globalData.subTitle = ResourceGlobal.Index;
            globalData.controllerName = "Line";
            globalData.actionName = ResourceGlobal.Index;
            globalData.contentTitle = ResourceModules.Route_Configuration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceModules.Route_Configuration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.Route_Configuration;
            globalData.messageDetail = ResourceModules.Route_Configuration + " " + ResourceMessages.Add_Success;
            TempData["globalData"] = globalData;

            return RedirectToAction("Index");

        }

        public ActionResult RouteMarriageStation()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(m => m.Plant_ID == plantId), "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(m => m.Plant_ID == plantId), "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m => m.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId).ToList(), "Shop_ID", "Shop_Name");
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines.Where(p => p.Line_ID == 0).ToList(), "Line_ID", "Line_Name");
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines.Where(p => p.Line_ID == 0).ToList(), "Line_ID", "Line_Name");

            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations.Where(p => p.Shop_ID == 0).ToList(), "Station_ID", "Station_Name");
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations.Where(p => p.Shop_ID == 0).ToList(), "Station_ID", "Station_Name");

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Route_Marriage;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "RouteConfiguration";
            globalData.actionName = "RouteMarriageStation";
            globalData.contentTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RouteMarriageStation([Bind(Include = "Route_Marriage_Station,Plant_ID,Shop_ID,Sub_Line_ID,Sub_Line_Station_ID,Marriage_Line_ID,Marriage_Station_ID,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Marriage_Station RS_Route_Marriage_Station)
        {
            if (ModelState.IsValid)
            {
                plantId = Convert.ToInt16(RS_Route_Marriage_Station.Plant_ID);
                shopId = Convert.ToInt16(RS_Route_Marriage_Station.Shop_ID);
                int subLineId = Convert.ToInt16(RS_Route_Marriage_Station.Sub_Line_ID);
                int subStationId = Convert.ToInt16(RS_Route_Marriage_Station.Sub_Line_Station_ID);
                int marriageLineId = Convert.ToInt16(RS_Route_Marriage_Station.Marriage_Line_ID);
                int marriageStationId = Convert.ToInt16(RS_Route_Marriage_Station.Marriage_Station_ID);

                // process to save the record
                if (RS_Route_Marriage_Station.isMarriageExists(plantId, shopId, subLineId, subStationId, marriageLineId, marriageStationId))
                {
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config;
                    globalData.messageDetail = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config + " " + ResourceMessages.Allready_Exit;
                    ViewBag.GlobalDataModel = globalData;
                }
                else
                {
                    RS_Route_Marriage_Station.Inserted_Date = DateTime.Now;
                    RS_Route_Marriage_Station.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Route_Marriage_Station.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Route_Marriage_Station.Add(RS_Route_Marriage_Station);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config;
                    globalData.messageDetail = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("RouteMarriageStationList");
                }

            }

            var endStation = from station in db.RS_Stations
                             where (from routeConfiguration in db.RS_Route_Configurations where routeConfiguration.Is_End_Station == true && routeConfiguration.Line_ID == RS_Route_Marriage_Station.Sub_Line_ID select routeConfiguration.Station_ID).Contains(station.Station_ID)
                             select new
                             {
                                 Station_ID = station.Station_ID,
                                 Station_Name = station.Station_Name,
                             };

            var startStation = from station in db.RS_Stations
                               where (from routeConfiguration in db.RS_Route_Configurations where routeConfiguration.Is_Start_Station == true && routeConfiguration.Line_ID == RS_Route_Marriage_Station.Marriage_Line_ID select routeConfiguration.Station_ID).Contains(station.Station_ID)
                               select new
                               {
                                   Station_ID = station.Station_ID,
                                   Station_Name = station.Station_Name,
                               };


            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Station.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Station.Updated_User_ID);
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Route_Marriage_Station.Shop_ID), "Line_ID", "Line_Name", RS_Route_Marriage_Station.Marriage_Line_ID);
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Route_Marriage_Station.Shop_ID), "Line_ID", "Line_Name", RS_Route_Marriage_Station.Sub_Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Marriage_Station.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == RS_Route_Marriage_Station.Plant_ID), "Shop_ID", "Shop_Name", RS_Route_Marriage_Station.Shop_ID);
            ViewBag.Marriage_Station_ID = new SelectList(startStation, "Station_ID", "Station_Name", RS_Route_Marriage_Station.Marriage_Station_ID);
            ViewBag.Sub_Line_Station_ID = new SelectList(endStation, "Station_ID", "Station_Name", RS_Route_Marriage_Station.Sub_Line_Station_ID);

            globalData.pageTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "RouteConfiguration";
            globalData.actionName = "RouteMarriageStation";
            globalData.contentTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View("RouteMarriageStation", RS_Route_Marriage_Station);
        }



        public ActionResult RouteMarriageStationList()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "RouteConfiguration";
            globalData.actionName = "RouteMarriageStationList";
            globalData.contentTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config;
            globalData.contentFooter = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;

            var RS_Route_Marriage_Station = db.RS_Route_Marriage_Station.Where(r => r.Plant_ID == plantId).Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Lines1).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations).Include(m => m.RS_Stations1);
            return View(RS_Route_Marriage_Station.ToList());
        }

        // POST: /RouteMarriageStation/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteMarriageStation(decimal id)
        {
            try
            {

                RS_Route_Marriage_Station RS_Route_Marriage_Station = db.RS_Route_Marriage_Station.Find(id);
                db.RS_Route_Marriage_Station.Remove(RS_Route_Marriage_Station);
                db.SaveChanges();
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Route_Marriage_Station", "Route_Marriage_Station", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config + " " + ResourceMessages.Delete_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("RouteMarriageStationList");
            }
            catch (DbUpdateException ex)
            {
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Route_Marriage_Station", "Route_Marriage_Station", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Route_Marriage + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("RouteMarriageStationList");

            }
        }

        public ActionResult RouteShopMarriageStationList()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "RouteConfiguration";
            globalData.actionName = "RouteMarriageStation";
            globalData.contentTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
            globalData.contentFooter = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
            ViewBag.GlobalDataModel = globalData;

            var RS_Route_Marriage_Shop = db.RS_Route_Marriage_Shop.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Lines).Include(m => m.RS_Lines1).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Shops1).Include(m => m.RS_Stations).Include(m => m.RS_Stations1);
            return View(RS_Route_Marriage_Shop.ToList());
        }

        public ActionResult RouteShopMarriageStation()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Sub_Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Marriage_Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == 0), "Shop_ID", "Shop_Name");
            ViewBag.Sub_Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations.Where(p => p.Shop_ID == 0), "Station_ID", "Station_Name");
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations.Where(p => p.Shop_ID == 0), "Station_ID", "Station_Name");

            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "RouteConfiguration";
            globalData.actionName = "RouteShopMarriageStation";
            globalData.contentTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        // POST: /RouteShopMarriage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RouteShopMarriageStation([Bind(Include = "Route_Marriage_Shop_Station,Plant_ID,Sub_Shop_ID,Sub_Line_ID,Sub_Line_Station_ID,Marriage_Shop_ID,Marriage_Line_ID,Marriage_Station_ID,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Route_Marriage_Shop RS_Route_Marriage_Shop)
        {
            if (ModelState.IsValid)
            {

                subShopId = Convert.ToInt16(RS_Route_Marriage_Shop.Sub_Shop_ID);
                subLineId = Convert.ToInt16(RS_Route_Marriage_Shop.Sub_Line_ID);
                subStationId = Convert.ToInt16(RS_Route_Marriage_Shop.Sub_Line_Station_ID);
                marriageShopId = Convert.ToInt16(RS_Route_Marriage_Shop.Marriage_Shop_ID);
                marriageLineId = Convert.ToInt16(RS_Route_Marriage_Shop.Marriage_Line_ID);
                marriageStationId = Convert.ToInt16(RS_Route_Marriage_Shop.Marriage_Station_ID);

                if (!RS_Route_Marriage_Shop.isShopConnected(subShopId, subLineId, subStationId, marriageShopId, marriageLineId, marriageStationId))
                {
                    // process to add the connection

                    RS_Route_Marriage_Shop.Inserted_Date = DateTime.Now;
                    RS_Route_Marriage_Shop.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Route_Marriage_Shop.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    db.RS_Route_Marriage_Shop.Add(RS_Route_Marriage_Shop);
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
                    globalData.messageDetail = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config + " " + ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;

                    return RedirectToAction("RouteShopMarriageStationList");
                }
                else
                {
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
                    globalData.messageDetail = ResourceModules.Route_Shop_Marriage + " " + ResourceMessages.Allready_Exit;
                    TempData["globalData"] = globalData;
                }


            }

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Shop.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Route_Marriage_Shop.Updated_User_ID);
            ViewBag.Marriage_Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == RS_Route_Marriage_Shop.Marriage_Shop_ID), "Line_ID", "Line_Name", RS_Route_Marriage_Shop.Marriage_Line_ID);

            // sub line with end line true
            var subLine = from line in db.RS_Lines
                          where line.Shop_ID == RS_Route_Marriage_Shop.Sub_Shop_ID && (line.Is_Shop_Line_End == true)
                          select line;
            ViewBag.Sub_Line_ID = new SelectList(subLine, "Line_ID", "Line_Name", RS_Route_Marriage_Shop.Sub_Line_ID);

            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Route_Marriage_Shop.Plant_ID);
            ViewBag.Marriage_Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == RS_Route_Marriage_Shop.Plant_ID && p.Shop_ID != RS_Route_Marriage_Shop.Sub_Shop_ID), "Shop_ID", "Shop_Name", RS_Route_Marriage_Shop.Marriage_Shop_ID);
            ViewBag.Sub_Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == RS_Route_Marriage_Shop.Plant_ID), "Shop_ID", "Shop_Name", RS_Route_Marriage_Shop.Sub_Shop_ID);
            ViewBag.Marriage_Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == RS_Route_Marriage_Shop.Marriage_Line_ID), "Station_ID", "Station_Name", RS_Route_Marriage_Shop.Marriage_Station_ID);
            ViewBag.Sub_Line_Station_ID = new SelectList(db.RS_Stations.Where(p => p.Line_ID == RS_Route_Marriage_Shop.Sub_Line_ID), "Station_ID", "Station_Name", RS_Route_Marriage_Shop.Sub_Line_Station_ID);

            globalData.pageTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "RouteConfiguration";
            globalData.actionName = "RouteShopMarriageStation";
            globalData.contentTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_Route_Marriage_Shop);
        }



        // POST: /RouteShopMarriage/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteShopRouteConfiguration(decimal id)
        {

            try
            {
                RS_Route_Marriage_Shop RS_Route_Marriage_Shop = db.RS_Route_Marriage_Shop.Find(id);
                db.RS_Route_Marriage_Shop.Remove(RS_Route_Marriage_Shop);
                db.SaveChanges();

                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Route_Marriage_Shop", "Route_Marriage_Shop_Station", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Form;
                TempData["globalData"] = globalData;

                return RedirectToAction("RouteShopMarriageStationList");
            }
            catch (DbUpdateException ex)
            {
                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_Route_Marriage_Shop", "Route_Marriage_Shop_Station", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Route_Shop_Marriage + " " + ResourceGlobal.Config;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;

                return RedirectToAction("RouteShopMarriageStationList");

            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Action Name          : GetRouteDisplayByLineID
        // Input Parameter      : lineId
        // Return Type          : ActionResult
        // Author & Time Stamp  : Jitendra Mahajan :: 10-07-2015
        // Description          : Action used to return the route display
        //////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult GetRouteDisplayByLineID(int lineId)
        {
            try
            {
                var st = from routeDisplay in db.RS_Route_Display
                             //join station in db.RS_Stations on routeDisplay.Station_ID equals station.Station_ID
                         where routeDisplay.Line_ID == lineId
                         select routeDisplay;

               

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}