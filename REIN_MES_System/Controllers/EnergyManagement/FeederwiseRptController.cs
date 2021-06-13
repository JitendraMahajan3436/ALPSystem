using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.Reports
{
    public class FeederwiseRptController : Controller
    {
        // GET: FeederwiseRpt
        REIN_SOLUTION_MEntities db = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();
        FDSession adSession = new FDSession();
        General generalHelper = new General();

        public ActionResult Index()
        {
           
     
           
            try
            {
                int plantID = ((FDSession)this.Session["FDSession"]).plantId;
                ViewBag.Shop_ID = new SelectList(db.MM_MTTUW_Shops.Where(s => s.Plant_ID == plantID && s.Energy ==true), "Shop_ID", "Shop_Name");
                var shift = db.MM_MTTUW_Shift.Where(s => s.Shift_ID == 25).Select(s => s.Shift_Start_Time).FirstOrDefault();
                ViewBag.ShiftTime = shift;
                

                globalData.pageTitle = "Feederwise Report";
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
        public ActionResult Close()
        {

            return RedirectToAction("Index", "Dashbord");

        }
    }
}