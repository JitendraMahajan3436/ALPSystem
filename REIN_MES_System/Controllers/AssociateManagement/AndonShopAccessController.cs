using Mahindra_FD.Controllers.BaseManagement;
using Mahindra_FD.Helper;
using Mahindra_FD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mahindra_FD.App_LocalResources;

namespace Mahindra_FD.Controllers.AssociateManagement
{
    public class AndonShopAccessController : BaseController
    {
        private DRONA_NGPEntities db = new DRONA_NGPEntities();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0;

        GlobalData globalData = new GlobalData();

        //
        // GET: /AndonShopAccess/
        public ActionResult SelectStation()
        {

            globalData.pageTitle = ResourceModules.Andon_Shop_Screen;
            
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "AndonShopAccess";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Andon_Shop_Screen;
            globalData.contentFooter = ResourceModules.Andon_Shop_Screen;
            ViewBag.GlobalDataModel = globalData;

            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(p => p.Shop_ID == shopId), "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(p => p.Shop_ID == shopId), "Station_ID", "Station_Name");
            return View();
        }

        [HttpPost]
        public ActionResult SelectStation(MetaAndonShopAccess metaAndonAccess)
        {
            if (ModelState.IsValid)
            {

                FDSession fdSessionObj = new FDSession();
                fdSessionObj.stationId = (int)metaAndonAccess.Station_ID;

                MM_Stations stationObj = db.MM_Stations.Where(a => a.Station_ID == metaAndonAccess.Station_ID).FirstOrDefault();
                if (stationObj.Station_IP_Address == "" || stationObj.Station_IP_Address == null)
                {
                    ModelState.AddModelError("Station_ID", ResourceValidation.Invalid_IP);
                }
                else
                {
                    fdSessionObj.stationName = stationObj.Station_Name;
                    fdSessionObj.plantId = (int)stationObj.MM_Shops.MM_Plants.Plant_ID;
                    fdSessionObj.shopId = (int)stationObj.Shop_ID;
                    fdSessionObj.lineId = (int)stationObj.Line_ID;
                    fdSessionObj.userHost = stationObj.Station_IP_Address;

                    var stationRoleObj = db.MM_Station_Roles.Where(a => a.Station_ID == metaAndonAccess.Station_ID).ToList();

                    if (stationRoleObj.Count > 0)
                    {
                        List<decimal> roleList = new List<decimal>();
                        foreach (var station in stationRoleObj)
                        {
                            roleList.Add(station.Role_ID);
                        }

                        IEnumerable<MM_Menus> menuObj = db.MM_Menu_Role.Where(a => roleList.Contains(a.Role_ID)).Select(a => a.MM_Menus).Distinct().ToList();
                        fdSessionObj.menuObj = menuObj;
                    }
                    this.Session["FDSession"] = fdSessionObj;

                    if (checkifStationCritical(metaAndonAccess.Station_ID))//CHECK IF STATION IS CRITICAL STATION
                    {
                        return RedirectToAction("Create", "CriticalStationLogin");
                    }
                    else
                    {
                        return RedirectToAction("ShopLogin", "User");
                        //return View("ShopLogin");
                    }
                }
            }


            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            ViewBag.Line_ID = new SelectList(db.MM_Lines.Where(p => p.Shop_ID == metaAndonAccess.Shop_ID), "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations.Where(p => p.Line_ID == metaAndonAccess.Line_ID), "Station_ID", "Station_Name");

            globalData.pageTitle = ResourceModules.Andon_Shop_Screen;

            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "AndonShopAccess";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Andon_Shop_Screen;
            globalData.contentFooter = ResourceModules.Andon_Shop_Screen;
            ViewBag.GlobalDataModel = globalData;

            return View();
        }

        private bool checkifStationCritical(decimal Station_ID)
        {
            try
            {
                return db.MM_Stations.Any(a => a.Station_ID == Station_ID && a.Is_Critical_Station == true);
            }
            catch (Exception ex)
            {
                General genObj = new General();
                genObj.addControllerException(ex, "UserController", "checkifStationCritical", 1);
                return false;
            }
        }
    }
}