using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ZHB_AD.Helper;
using ZHB_AD.App_LocalResources;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class MachineStatusController : BaseController
    {
        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        REIN_SOLUTION_MEntities db1 = new REIN_SOLUTION_MEntities();
        GlobalData globalData = new GlobalData();

        // GET: MachineStatus
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Status()
        {
            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            //ar shops = db.MM_MT_Machines.AsEnumerable().Distinct().Select(a => new DistinctShop { Shop_ID = a.Shop_ID, Shop_Name = a.MM_Shops.Shop_Name }).ToList();
            var shops = (from t in db.MM_MT_Machines join r in db.MM_Shops on t.Shop_ID equals r.Shop_ID where r.Plant_ID == plantID select new DistinctShop { Shop_ID = t.Shop_ID, Shop_Name = r.Shop_Name }).Distinct();
            // var shops = db.MM_MT_Machines.AsEnumerable().Select(x => new DistinctShop{ Shop_ID= x.Shop_ID,Shop_Name= x.MM_Shops.Shop_Name });
            var machines = db.MM_MT_Machines.Where(x => x.Is_Status_Machine == true && x.IsActive == true).OrderBy(x => x.Machine_ID).ToList();
            //var machines1 = (from machine in db.MM_MT_Machines
            //                join status in db.MM_Ctrl_Equipment_Status
            //                on machine.Machine_ID equals status.Machine_ID
            //                where machine.IsActive == true && machine.Is_Status_Machine == true
            //                select new
            //                {
            //                    machine
            //                });
            //var list = callList.GroupBy(x => x.ApplicationID).Select(x => x.First()).ToList();
            //var machines = machines1.GroupBy(x => x.machine.Machine_ID).ToList();
            //var machines = (from machine in db.MM_MT_Machines
            //             join status in db.MM_Ctrl_Equipment_Status
            //            on machine.Machine_ID equals status.Machine_ID
            //            where machine.IsActive == true && machine.Is_Status_Machine == true
            //           orderby status.isFaulty, machine.Machine_ID select machine);


            var plantName = db.MM_Plants.Where(m => m.Plant_ID == plantID).Select(m => m.Plant_Name).FirstOrDefault();

            ViewBag.Shops = shops;
            ViewData["shops"] = shops;
            globalData.pageTitle = "Machine Status";
            globalData.plantName = plantName;
            ViewBag.GlobalDataModel = globalData;
            return View(machines);
        }

        public ActionResult getStatus(string ShopID)
        {

            globalData.pageTitle = "Machine Status Screen";
            globalData.subTitle = "Status";
            globalData.contentTitle = "Machine Status Screen";
            ViewBag.GlobalDataModel = globalData;
            var ID = Convert.ToDecimal(ShopID);
            var Name = db.MM_Shops.Where(m => m.Shop_ID == ID).Select(m => m.Shop_Name).FirstOrDefault();
            var status = db.MM_Ctrl_Equipment_Status
                  // .Where(x=>x.Inserted_Date.Value.Year==DateTime.Now.Year && x.Inserted_Date.Value.Month==DateTime.Now.Month && x.Inserted_Date.Value.Day==DateTime.Now.Day)
                  .Where(c => c.Shop_ID == ID)
                .GroupBy(c => new { c.Machine_ID })
                //.OrderBy(m => m.OrderBy(ae => ae.isFaulty))
                .Select(g => g.OrderByDescending(c => c.EQ_ID).FirstOrDefault())
                .Select(c => new { c.Machine_ID, c.isFaulty, c.isHealthy, c.isIdle, c.Heart_Bit });

            var MPLCStatus = db1.MM_Ctrl_PLC_Status
               .OrderByDescending(m => m.PS_ID)
                .Select(m => m.Is_MasterPLC).FirstOrDefault();

            return Json(new { Status = status, MStatus = MPLCStatus, ShopName = Name }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Machinestatus()
        {
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            var shops = (from t in db.MM_MT_Machines join r in db.MM_Shops on t.Shop_ID equals r.Shop_ID where r.Plant_ID == plant_ID select new DistinctShop { Shop_ID = t.Shop_ID, Shop_Name = r.Shop_Name }).Distinct();
            // var shops = db.MM_MT_Machines.AsEnumerable().Select(x => new DistinctShop{ Shop_ID= x.Shop_ID,Shop_Name= x.MM_Shops.Shop_Name });
            var machines = db.MM_MT_Machines.Where(x => x.Is_Status_Machine == true && x.IsActive == true).OrderBy(x => x.Machine_ID).ToList();


            var plantName = db.MM_Plants.Where(m => m.Plant_ID == plant_ID).Select(m => m.Plant_Name).FirstOrDefault();

            ViewBag.Shops = shops;
            ViewData["shops"] = shops;
            globalData.pageTitle = "Machine Status";
            globalData.plantName = plantName;
            ViewBag.GlobalDataModel = globalData;
            return View(machines);

        }

        public ActionResult Machinestatus1()
        {
            decimal plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
            var shops = (from t in db.MM_MT_Machines join r in db.MM_Shops on t.Shop_ID equals r.Shop_ID where r.Plant_ID == plant_ID select new DistinctShop { Shop_ID = t.Shop_ID, Shop_Name = r.Shop_Name }).Distinct();
            // var shops = db.MM_MT_Machines.AsEnumerable().Select(x => new DistinctShop{ Shop_ID= x.Shop_ID,Shop_Name= x.MM_Shops.Shop_Name });
            var machines = db.MM_MT_Machines.Where(x => x.Is_Status_Machine == true && x.IsActive == true).OrderBy(x => x.Machine_ID).ToList();


            var plantName = db.MM_Plants.Where(m => m.Plant_ID == plant_ID).Select(m => m.Plant_Name).FirstOrDefault();

            ViewBag.Shops = shops;
            ViewData["shops"] = shops;
            // ViewBag.Shops_ID = new SelectList(db.MM_Shops.Where(c => c.Plant_ID == plant_ID).OrderBy(s => s.Shop_ID), "Shop_ID", "Shop_Name");

            var shops1 = (from shop in db1.MM_MTTUW_Shops
                          join cr in db1.MM_MT_MTTUW_Machines
                          on shop.Shop_ID equals cr.Shop_ID
                          select shop).Distinct();

            ViewBag.Shops_ID = new SelectList(shops1, "Shop_ID", "Shop_Name");


            globalData.pageTitle = "Machine Status";
            globalData.plantName = plantName;
            ViewBag.GlobalDataModel = globalData;
            return View(machines);

        }
        public ActionResult GetFirstShop()
        {
            var PlantID = ((FDSession)this.Session["FDSession"]).plantId;
            var st = (from shop in db.MM_Shops
                      where shop.Plant_ID == PlantID
                      select new
                      {
                          Id = shop.Shop_ID,
                          Value = shop.Shop_Name
                      }
                    ).FirstOrDefault();
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getshopname(int shopid)
        {
            var st = (from shopp in db.MM_Shops
                      where shopp.Shop_ID == shopid
                      select new
                      {
                          Value = shopp.Shop_Name
                      }
                    ).Distinct();
            return Json(st, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getmachine(int shopid)
        {
            var st1 = (from machine in db.MM_MT_Machines
                       where machine.Shop_ID == shopid && machine.IsActive == true
                       select new
                       {
                           machine_Id = machine.Machine_ID,
                           machine_name = machine.Machine_Name,
                           machine_no = machine.Machine_Number,
                           machine_ip = machine.IP_Address
                       }
                     ).Distinct().ToList();
            var ShopName = db.MM_Shops.Where(m => m.Shop_ID == shopid).Select(m => m.Shop_Name).FirstOrDefault();
            globalData.pageTitle = "Machine Status - " + ShopName;
            //globalData.plantName = plantName;
            ViewBag.GlobalDataModel = globalData;
            return Json(st1, JsonRequestBehavior.AllowGet);
        }

    }
}