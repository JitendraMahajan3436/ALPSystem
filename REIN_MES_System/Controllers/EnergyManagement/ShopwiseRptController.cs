using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.Reports
{
    public class ShopwiseRptController : Controller
    {
        // GET: ShopwiseRpt
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();
        public ActionResult Index()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s=>s.Plant_ID ==plantID  && s.Energy ==true), "Shop_ID", "Shop_Name");
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange, "DateID", "DateName", 2);
                var shift = db.MM_MTTUW_Shift.Where(s => s.Shift_ID == 25).Select(s => s.Shift_Start_Time).FirstOrDefault();
                ViewBag.ShiftTime = shift;
                globalData.pageTitle = "Shopwise Data Log";
                globalData.subTitle = "Report";
                //globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;
                return View();
            }
            catch
            {
                return View();
            }
            
            
        }

        public ActionResult Reading()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID), "Shop_ID", "Shop_Name");
                ViewBag.Parameter_ID = new SelectList(db.MM_Parameter.Where(s => s.Plant_ID == plantID), "Prameter_ID", "Prameter_Name");

               // var shift = db.MM_Shift.Where(s => s.Shift_ID == 1).Select(s => s.Shift_Start_Time).FirstOrDefault();
                //ViewBag.ShiftTime = shift;
                globalData.pageTitle = "Utility Data Log";
                globalData.subTitle = "Report";
                //globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "User");
            }


        }

        public ActionResult Plantwise()
        {
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
               
                ViewBag.PlantID = plantID;
                ViewBag.ddlDateRange = new SelectList(db.MM_DateRange, "DateID", "DateName", 2);
                globalData.pageTitle = "Plantwise Data Log";
                globalData.subTitle = "Report";
                //globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
                ViewBag.GlobalDataModel = globalData;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult Close()
        {

            return RedirectToAction("Index", "Dashbord");

        }
    }
}