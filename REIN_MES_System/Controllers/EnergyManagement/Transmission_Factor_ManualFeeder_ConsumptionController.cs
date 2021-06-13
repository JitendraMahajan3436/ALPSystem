using ZHB_AD.App_LocalResources;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers
{
    public class Transmission_Factor_ManualFeeder_ConsumptionController : Controller
    {
        // GET: Transmission_Factor_ManualFeeder_Consumption
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();

        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalObj = new General();
        public ActionResult Index()
        {
            try
            {

          
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            int userID = ((FDSession)Session["FDSession"]).userId;

            //this.Session["UserID"] = adsessionobj.userRoleId;
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            List<Transmission> Result = new List<Transmission>();
            DateTime consumptiondate = System.DateTime.Now.Date;
            consumptiondate = consumptiondate.AddDays(-7);
            var trans = (from t in db.Total_Transmission_Loss_Consumption
                         where t.DateTime > consumptiondate && t.Plant_ID == plantID
                         select new
                         {
                             t.DateTime,
                             t.Factor,
                             t.Trans_ID,
                             t.Status

                         }).ToList();
            foreach (var item in trans)
            {
                Double consumption = 0;
                int action = 0;
                if(item.Status ==true)
                {
                    Transmission obj = new Transmission("Transmission Factor", Convert.ToDateTime(item.DateTime), null, null, Convert.ToDecimal(item.Factor), consumption, action, Convert.ToInt16(item.Trans_ID));
                    Result.Add(obj);
                }
                else
                {
                    Transmission obj = new Transmission("kVAh Factor", Convert.ToDateTime(item.DateTime), null, null, Convert.ToDecimal(item.Factor), consumption, action, Convert.ToInt16(item.Trans_ID));
                    Result.Add(obj);
                }
               
            }
            var manual = (from m in db.MM_ManualFeeder_Consumption
                          where m.DateAndTime > consumptiondate && m.Plant_Id == plantID
                          select (m)).ToList();
          foreach (var id1 in manual)
            {
                var shop = (from s in db.MM_MTTUW_Shops
                            where s.Shop_ID == id1.Shop_Id
                            select new { s.Shop_Name }).FirstOrDefault();
                var feeder = (from u in db.UtilityMainFeederMappings join
                              f in db.MM_Feeders on
                              u.Feeder_ID equals f.Feeder_ID
                              where u.TagIndex == id1.TagIndex
                              select new { f.FeederName }).FirstOrDefault();
                int action = 0;
                decimal factor = 0;
                Transmission obj = new Transmission("Manual Consumption", Convert.ToDateTime(id1.DateAndTime), shop.Shop_Name, feeder.FeederName, factor, Convert.ToDouble(id1.Consumption),  action, Convert.ToInt16(id1.Manual_ID));
                Result.Add(obj);

            }
          globalData.pageTitle = ResourceTransmissionLossesFactor.PageTitle;
          ViewBag.GlobalDataModel = globalData;
            return View(Result.OrderByDescending(s=>s.DateTime));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "user");
            }
        }
        public ActionResult Create()
        {
            try
            {
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceTransmissionLossesFactor.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = plantID;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "user");
            }

        }


        [HttpPost]
        public ActionResult Create(TransmissionFactor obj)
        {
            try
            {
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceShopwiseManualReading.PageTitle;
                DateTime consumptionDate = Convert.ToDateTime(obj.DateTime);
                //consumptionDate = consumptionDate.AddDays(-1);
                //consumptionDate = consumptionDate.Date;
                if (obj.Opertion == null)
                {
                    ModelState.AddModelError("Opertion", ResourceValidation.Required);
                }
                if (obj.Opertion == 1 )
                {

                    Total_Transmission_Loss_Consumption trans = new Total_Transmission_Loss_Consumption();
                    if (trans.IsFactorExists(plantID, consumptionDate, 0,true))
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = ResourceTransmissionLossesFactor.PageTitle;
                        globalData.messageDetail = ResourceTransmissionLossesFactor.Trasmission_Factor;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                       else
                    {
                       

                        trans.Factor = obj.Factor;
                        trans.Plant_ID = plantID;
                        trans.DateTime = consumptionDate;
                        trans.Status = true;
                        trans.Inserted_Date = System.DateTime.Now;
                        trans.Inserted_User_ID = userId;
                        trans.Inserted_Host = Request.UserHostName;
                        db.Total_Transmission_Loss_Consumption.Add(trans);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceTransmissionLossesFactor.PageTitle;
                        globalData.messageDetail = ResourceTransmissionLossesFactor.Trasmission_Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }   
                else if(obj.Opertion == 2)
                {
                    Total_Transmission_Loss_Consumption trans = new Total_Transmission_Loss_Consumption();
                    if (trans.IsFactorExists(plantID, consumptionDate, 0, false))
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = ResourceTransmissionLossesFactor.PageTitle;
                        globalData.messageDetail = ResourceTransmissionLossesFactor.Trasmission_Factor;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                    else
                    {


                        trans.Factor = obj.Factor;
                        trans.Plant_ID = plantID;
                        trans.Status = false;
                        trans.DateTime = consumptionDate;
                        trans.Inserted_Date = System.DateTime.Now;
                        trans.Inserted_User_ID = userId;
                        trans.Inserted_Host = Request.UserHostName;
                        db.Total_Transmission_Loss_Consumption.Add(trans);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceTransmissionLossesFactor.PageTitle;
                        globalData.messageDetail = ResourceTransmissionLossesFactor.Trasmission_Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }
                }
                else
                {

                    MM_ManualFeeder_Consumption consumtion = new MM_ManualFeeder_Consumption();
                    if (consumtion.IsConsumptionExists(plantID, Convert.ToInt32(obj.Shop_ID), Convert.ToInt32(obj.TagIndex), consumptionDate, 0))
                    {
                        globalData.isErrorMessage = true;
                        globalData.messageTitle = ResourceTransmissionLossesFactor.PageTitle;
                        globalData.messageDetail = ResourceTransmissionLossesFactor.Trasmission_Consumption;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        //var feeder = (from f in db.Feeder_DailyReport
                        //              where f.TagIndex == obj.TagIndex && f.Shop_ID == obj.Shop_ID
                        //                   && (f.DateAndTime).Year == consumptionDate.Year && (f.DateAndTime).Month == consumptionDate.Month
                        //                     && (f.DateAndTime).Day == consumptionDate.Day
                        //              select new {
                        //                  RowId= Convert.ToDouble(f.RowId) }).ToList();
                        //foreach(var Id in feeder)
                        //{
                        //    var rowId = Id.RowId;
                        //    Feeder_DailyReport obj1 = db.Feeder_DailyReport.Find(rowId);
                        //    obj1.ShiftConsumption = 0;
                        //    db.SaveChanges();
                        //}
                      
                       
                        consumtion.Shop_Id = obj.Shop_ID;
                        consumtion.Plant_Id = plantID;
                        consumtion.TagIndex = obj.TagIndex;
                        consumtion.Consumption = obj.Consumption;
                        consumtion.DateAndTime = consumptionDate;
                        consumtion.Action = obj.Action;
                        consumtion.Inserted_Date = System.DateTime.Now;
                        consumtion.Inserted_User_ID = userId;
                        consumtion.Inserted_Host = Request.UserHostName;
                        db.MM_ManualFeeder_Consumption.Add(consumtion);
                        db.SaveChanges();
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceTransmissionLossesFactor.PageTitle;
                        globalData.messageDetail = ResourceTransmissionLossesFactor.Trasmission_Add_Success;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");

                    }

                    

                }
               
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
            
       
        }
    }
}