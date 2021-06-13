using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Helper;
using ZHB_AD.App_LocalResources;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ZHB_AD.Controllers.MasterManagement
{
    public class ShopwiseManualProductionController : Controller
    {


        // GET: ShopwiseManualProduction
        private REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData=new GlobalData();
        FDSession fdSession = new FDSession();
        General generalOBJ = new General();
        
        public ActionResult Index()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                if (TempData["globalData"] != null)
                {
                    globalData = (GlobalData)TempData["globalData"];
                }
                int userId = ((FDSession)this.Session["FDsession"]).userId;
                
                globalData.pageTitle = ResourceShopwiseManualProduction.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                DateTime consumptiondate = System.DateTime.Now;
                DateTime Startdate;
                consumptiondate = consumptiondate.AddDays(0);
                Startdate = consumptiondate.AddDays(-8);
                var Manualproduction = (from t in db.ShopwiseManualProductions
                            
                            
                             where  (t.Inserted_Date > Startdate &&  t.Inserted_Date <= consumptiondate ) && t.Plant_ID == plantID
                             select (t)).ToList();

        
                //var Manualproduction = (from p in db.ShopwiseManualProductions
                //                        join s in db.MM_MTTUW_Shops on
                //                        p.Shop_ID equals s.Shop_ID
                //                        join u in db.MM_Plant_User on
                //                        p.Plant_ID equals u.Plant_ID
                //                        where u.User_ID == userId && u.Plant_ID == plantID
                //                        select (p)).ToList();

                return View(Manualproduction);
            }
            catch (Exception ex)
            {
                return View("Index", "user");
            }
            

        }
        public ActionResult Create()
        {
            try
            {
                int userId = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                globalData.pageTitle = ResourceShopwiseManualProduction.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                ViewBag.Plant_ID = plantID;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(p => p.Plant_ID == plantID && p.Energy ==true), "Shop_ID", "Shop_Name");
                var months = DateTimeFormatInfo
 .InvariantInfo
 .MonthNames
 .TakeWhile(monthName => monthName != String.Empty)
 .Select((monthName, index) => new SelectListItem
 {
     Value = string.Format("{0} ", monthName),
     Text = string.Format("{0} ", monthName)
 });
                ViewBag.Month = months;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index","user");
            }
            
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShopwiseManualProduction obj)
        {
            try
            {

                int userId = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                var Pro_DateTime = obj.Pro_Datetime;


                if (ModelState.IsValid)
                {
                    if (obj.IsProductionExists(plantID, Convert.ToInt16(obj.Shop_ID), 0, Convert.ToDateTime(Pro_DateTime)))
                    {
                        ModelState.AddModelError("Shop_ID", ResourceShopwiseManualProduction.Shop_Error_Production);
                    }
                    else
                    {
                        if (obj.Category == false)
                        {

                        }
                        else
                        {

                            obj.Year = null;
                        }
                        obj.Inserted_User_ID = userId;
                        obj.Plant_ID = plantID;
                        obj.Pro_Datetime = Pro_DateTime;
                        obj.Inserted_Date = System.DateTime.Now;
                        db.ShopwiseManualProductions.Add(obj);
                        db.SaveChanges();
                        globalData.pageTitle = ResourceShopwiseManualProduction.PageTitle;
                        globalData.isSuccessMessage = true;
                        globalData.messageTitle = ResourceShopwiseManualProduction.PageTitle;
                        globalData.messageDetail = ResourceShopwiseManualProduction.Added_Manaul_Production;
                        TempData["globalData"] = globalData;
                        ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name", obj.Shop_ID);
                        return RedirectToAction("Index");
                    }
                }
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Energy==true), "Shop_ID", "Shop_Name", obj.Shop_ID);
                return View(obj);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }


        }

        public ActionResult Edit(int? id)
        {
            try
            {

                ShopwiseManualProduction obj = db.ShopwiseManualProductions.Find(id);
                if (obj == null)
                {
                    return HttpNotFound();
                }
                if (obj.Category == true)
                {
                    ViewBag.Pro_Datetime = obj.Pro_Datetime.Value.ToShortDateString();
                }
                ViewBag.Pro_Datetime = obj.Pro_Datetime.Value.ToShortDateString();
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Energy ==true), "Shop_ID", "Shop_Name", obj.Shop_ID);               
                globalData.pageTitle = ResourceShopwiseManualProduction.PageTitle;
                ViewBag.GlobalDataModel = globalData;
                return View(obj);

            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceParameterMaster.pageTitle;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");

            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShopwiseManualProduction shopwiseManualProduction)
        {
            try
            {
                int userID = ((FDSession)this.Session["FDSession"]).userId;
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                DateTime Pro_DateTime = System.DateTime.Now.Date;
                var Pro_DateTime1 = shopwiseManualProduction.Pro_Datetime;
                if (ModelState.IsValid)
                {
                    if (shopwiseManualProduction.IsProductionExists(plantID, Convert.ToInt16(shopwiseManualProduction.Shop_ID), Convert.ToInt32(shopwiseManualProduction.Pro_Manual_ID), Convert.ToDateTime(Pro_DateTime1)))
                    {
                        ModelState.AddModelError("Shop_ID", ResourceShopwiseManualProduction.Shop_Error_Production);
                    }
                    else
                    {
                        ShopwiseManualProduction obj = db.ShopwiseManualProductions.Find(shopwiseManualProduction.Pro_Manual_ID);


                        int shopId = Convert.ToInt16(shopwiseManualProduction.Shop_ID);
                        int Production = Convert.ToInt16(shopwiseManualProduction.Production);
                        if (shopwiseManualProduction.Category == true)
                        {
                            obj.Month = null;
                            obj.Year = null;
                        }

                        obj.Pro_Datetime = Pro_DateTime;
                        obj.Updated_Date = DateTime.Now;
                        obj.Updated_Host = Request.UserHostName;
                        obj.Updated_User_ID = userID;
                        obj.Category = shopwiseManualProduction.Category;
                        obj.ProductionType = shopwiseManualProduction.ProductionType;
                        obj.Production = shopwiseManualProduction.Production;
                        obj.Shop_ID = shopwiseManualProduction.Shop_ID;
                        db.SaveChanges();


                        //db.Entry(shopwiseManualProduction).State = EntityState.Modified;


                        globalData.isSuccessMessage = true;
                        globalData.messageDetail = ResourceShopwiseManualProduction.Edit_Manaul_Production;
                        TempData["globalData"] = globalData;
                        return RedirectToAction("Index");
                    }

                }


                //                  select (s)).ToList(), "Plant_ID", "Plant_Name", utilityMainFeederMapping.Plant_ID);
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Energy==true), "Shop_ID", "Shop_Name", shopwiseManualProduction.Shop_ID);
                //ViewBag.Category_ID = new SelectList(db.MM_Category.Where(s => s.Plant_ID == plantID), "Category_Id", "Category_Name", utilityMainFeederMapping.Category_ID);
                //ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name", utilityMainFeederMapping.Parameter_ID);
                //ViewBag.Meter = new SelectList(db.MM_Meter, "MeterName", "MeterName", utilityMainFeederMapping.Meter);
                return View(shopwiseManualProduction);
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceParameterMaster.pageTitle;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }

        }

      

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                int Id = Convert.ToInt32(id);
                ShopwiseManualProduction shopwiseManualProduction = db.ShopwiseManualProductions.Find(Id);
                db.ShopwiseManualProductions.Remove(shopwiseManualProduction);
                db.SaveChanges();
                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceShopwiseManualProduction.Delete_Manaul_Production;
                globalData.messageDetail = ResourceShopwiseManualProduction.Delete_Manaul_Production;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                generalOBJ.addControllerException(ex, "ShopwiseManualProduction", "delete", ((FDSession)this.Session["FDSession"]).userId);
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceShopwiseManualProduction.Added_Manaul_Production;
                //globalData.messageDetail = ResourceMainFeederMapping.MainFeederMapping_Delete_Dependency_Failure;
                TempData["globalData"] = globalData;
                //ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_Category.Plant_ID);
                return RedirectToAction("Index");

            }

        }

    }
}