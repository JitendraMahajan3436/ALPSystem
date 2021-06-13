using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class AssignStationsToSetupController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0, setupId = 0;
        // GET: AssignStationsToSetup
        public ActionResult Index()
        {
            var RS_Station_Setup_Mapping = db.RS_Station_Setup_Mapping;
            return View(RS_Station_Setup_Mapping.ToList());
        }

        public ActionResult Create()
        {
            try
            {


                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                    ViewBag.Shop_ID = new SelectList("");
                    ViewBag.Line_ID = new SelectList("");
                    ViewBag.Setup_ID = new SelectList("");
                    ViewBag.ListofSupervisor = new SelectList("");
                    ViewBag.selectedSupervisors = new SelectList("");
                }
                else
                {
                    ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(p => p.Setup_ID == setupId), "Setup_ID", "Setup_Name");
                    ViewBag.ListofSupervisor = new SelectList(db.RS_Station_Setup_Mapping, "Station_ID", "Station_Name");
                    ViewBag.AssignedSupervisor_ID = new SelectList(db.RS_Station_Setup_Mapping, "Station_ID", "Station_Name");

                }

                globalData.pageTitle = "Assign Stations To Setup";
                globalData.controllerName = "AssignStationsToSetup";


                ViewBag.GlobalDataModel = globalData;
                ViewBag.STS_ID = new SelectList(db.RS_Station_Setup_Mapping, "Station_Setup_ID", "Station_Setup_ID");
                ViewBag.STS_ID = new SelectList(db.RS_Station_Setup_Mapping, "Station_Setup_ID", "Station_Setup_ID");
                ViewBag.Setup_ID = new SelectList(db.RS_Setup.Where(S => S.Setup_ID == 0), "Setup_ID", "Setup_Name");

                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m => m.Plant_ID == plantId).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name");
                ViewBag.Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_Station_Setup_Mapping RS_Station_Setup_Mapping)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    plantId = Convert.ToInt32(RS_Station_Setup_Mapping.Plant_ID);
                    shopId = Convert.ToInt32(RS_Station_Setup_Mapping.Shop_ID);
                    lineId = Convert.ToInt32(RS_Station_Setup_Mapping.Line_ID);
                    setupId = Convert.ToInt32(RS_Station_Setup_Mapping.Setup_ID);

                    {
                        // process to delete all the defects added for the station
                        RS_Station_Setup_Mapping.deleteOperator(plantId, shopId, lineId, setupId);//plantId,
                    }

                    for (int i = 0; i < RS_Station_Setup_Mapping.selectedStations.Count(); i++)
                    {
                        var operatorId = Convert.ToInt16(RS_Station_Setup_Mapping.selectedStations[i]);

                        if (operatorId == 0)
                            continue;

                        //if (RS_Assign_OperatorToSupervisor.isDefectAddedToStation(operatorId, stationId, plantId, shopId, lineId))
                        //{
                        //    // defect is already added no need to the defect
                        //}
                        else
                        {
                            //    if(i>0)
                            //        db.Entry(RS_Assign_SupervisorToManager).State = EntityState.Modified;

                            RS_Station_Setup_Mapping.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                            RS_Station_Setup_Mapping.Inserted_Date = DateTime.Now;
                            RS_Station_Setup_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                            // RS_Assign_OperatorToSupervisor. = ((FDSession)this.Session["FDSession"]).userHost;

                            RS_Station_Setup_Mapping.Station_ID = RS_Station_Setup_Mapping.selectedStations[i];

                            RS_Station_Setup_Mapping mmAssignSupervisorObj = new RS_Station_Setup_Mapping();
                            mmAssignSupervisorObj = RS_Station_Setup_Mapping;

                            db.Entry(mmAssignSupervisorObj).State = EntityState.Detached;
                            //db.RS_Assign_SupervisorToManager.ChangeObjectState(mmAssignSupervisorObj, EntityState.Unchanged);

                            db.RS_Station_Setup_Mapping.Add(mmAssignSupervisorObj);

                            db.SaveChanges();
                        }

                    }
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceSupervisorToManager.OperatorToSupervisor;
                    globalData.messageDetail = ResourceSupervisorToManager.OfficerToManager_Success_OfficerToManager_Add_Success;
                    TempData["globalData"] = globalData;
                    TempData["plantId"] = globalData;
                    TempData["shopId"] = globalData;
                    TempData["managerId"] = globalData;
                    //db.RS_Assign_SupervisorToManager.Add(RS_Assign_SupervisorToManager);
                    //db.SaveChanges();
                    //return RedirectToAction("Index");
                }


                globalData.pageTitle = "Assign Station To Setup";
                globalData.controllerName = "Assign Stations To Setup";
                ViewBag.GlobalDataModel = globalData;


                ViewBag.Setup_ID = new SelectList(db.RS_Setup, "Setup_ID", "Setup_Name", RS_Station_Setup_Mapping.Setup_ID);
                ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Station_Setup_Mapping.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", RS_Station_Setup_Mapping.Shop_ID);
                ViewBag.Line_ID = new SelectList(db.RS_Lines.OrderBy(c => c.Line_Name), "Line_ID", "Line_Name", RS_Station_Setup_Mapping.Line_ID);
                return RedirectToAction("Create");
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }
        }

        public ActionResult GetShopListByPlantId(int plantId)
        {
            try
            {
               
                var st = from shop in db.RS_Shops
                         where shop.Plant_ID == plantId
                         orderby shop.Shop_Name
                         select new
                         {
                             Id = shop.Shop_ID,
                             Value = shop.Shop_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
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
                
                var st = from setup in db.RS_Setup
                         where setup.Line_ID == lineId
                         orderby setup.Setup_Name
                         select new
                         {
                             Id = setup.Setup_ID,
                             Value = setup.Setup_Name,
                         };

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStationsBySetupID(int setupId, int? shopId, int? lineId, int? plantId)//
        {
            try
            {
                
                var st = (from station in db.RS_Stations
                          where !(from SM in db.RS_Station_Setup_Mapping
                                  where SM.Setup_ID == setupId
                                  select SM.Station_ID)
              .Contains(station.Station_ID)
                          
                          where  station.Line_ID == lineId
                          orderby station.Station_Name
                          select new
                          {
                              Id = station.Station_ID,
                              Value = station.Station_Name
                          }).Distinct();


                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAssignedStationsBySetupID(int setupId, int shopId,int lineId)
        {

            try
            {
                var st = from station in db.RS_Stations
                         where (from stationToSetup in db.RS_Station_Setup_Mapping
                                where stationToSetup.Setup_ID == setupId && stationToSetup.Shop_ID == shopId &&
                                stationToSetup.Line_ID == lineId 
                                select stationToSetup.Station_ID).Contains(station.Station_ID)
                         orderby station.Station_Name
                         select new
                         {
                             Id = station.Station_ID,
                             Value = station.Station_Name
                         };
          

                return Json(st, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult SaveAssignedStations(string Stations, int setupId, int shopId, int lineId)
        {
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                RS_Station_Setup_Mapping RS_Station_Setup_Mapping = new RS_Station_Setup_Mapping();

                RS_Station_Setup_Mapping.deleteOperator(plantId, shopId, lineId, setupId);  //plantId, 


                string[] words;
                words = Stations.Split(',');

                foreach (string value in words)
                {
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




                    RS_Station_Setup_Mapping.Plant_ID = plantId;
                    RS_Station_Setup_Mapping.Shop_ID = shopId;
                    RS_Station_Setup_Mapping.Line_ID = lineId;
                    RS_Station_Setup_Mapping.Inserted_Date = DateTime.Now;
                    RS_Station_Setup_Mapping.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    RS_Station_Setup_Mapping.Setup_ID = setupId;
                    RS_Station_Setup_Mapping.Station_ID = i;


                    db.Entry(RS_Station_Setup_Mapping).State = EntityState.Detached;
                    db.RS_Station_Setup_Mapping.Add(RS_Station_Setup_Mapping);
                    db.SaveChanges();


                    i = 0;
                }
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }

            }
    }
}