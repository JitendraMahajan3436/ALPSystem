using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class ClitaCAPAController : Controller
    {
        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        // GET: ClitaCAPA
        public ActionResult Index()
        {
            try
            {
                //var plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                //var ctrMachineHistory = db.MM_Ctrl_Equipment_Status_History.Where(c => c.Plant_ID == plant_ID).ToList();
                //ViewBag.Users = db.MM_Employee.Where(c => c.Plant_ID == plant_ID).ToList();
                //ViewBag.Done_By_User_ID = new SelectList(db.MM_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", 0);
                //return View(ctrMachineHistory);
                return View();
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        public ActionResult PartialCAPA()
        {
            try
            {
                var plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                var ctrMachineHistory = db.MM_Ctrl_Equipment_Status_History.Where(c => c.Plant_ID == plant_ID && c.Is_CLITA_Done==false).ToList();
                ViewBag.Users = db.MM_Employee.Where(c => c.Plant_ID == plant_ID).ToList();
                ViewBag.Done_By_User_ID = new SelectList(db.MM_Employee.Where(c => c.Plant_ID == plant_ID), "Employee_ID", "Employee_Name", 0);
                return PartialView("PartialCAPA",ctrMachineHistory);
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        public ActionResult saveClitaInformation(decimal userID,string CorrectiveAction ,string PreventiveAction ,string RemarkClita ,string ESHID )
        {
            try
            {
                MM_MT_CLITA_Capa clita = new MM_MT_CLITA_Capa();
                clita.Corrective_Action = CorrectiveAction;
                clita.Preventive_Action = PreventiveAction;
                clita.Remark = RemarkClita;
                clita.Done_By = userID;
                clita.Done_Date = DateTime.Now;
                clita.ESH_ID =Convert.ToInt32( ESHID);
                clita.Inserted_Date = DateTime.Now;
                clita.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                clita.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                clita.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                clita.Shop_ID = ((FDSession)this.Session["FDSession"]).shopId;
                clita.Station_ID = ((FDSession)this.Session["FDSession"]).stationId;
                db.MM_MT_CLITA_Capa.Add(clita);
                db.SaveChanges();
                int ESHIDs = Convert.ToInt32(ESHID);
                var res = db.MM_Ctrl_Equipment_Status_History.Where(c=>c.ESH_ID== ESHIDs).FirstOrDefault();
                if(res!=null)
                {
                    res.Is_CLITA_Done = true;
                    res.Is_Edited = true;
                    db.Entry(res).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    //return Json(true, JsonRequestBehavior.AllowGet);
                    return RedirectToAction("PartialCAPA");
                }
                return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}