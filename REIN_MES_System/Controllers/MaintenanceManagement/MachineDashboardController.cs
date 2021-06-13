using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class MachineDashboardController : BaseController
    {
        REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        private REIN_SOLUTION_MEntities db1 = new REIN_SOLUTION_MEntities();
        // GET: MachineDashboard
        public ActionResult Index()
        {
            Dashboard lstdash = new Dashboard();
            lstdash.lst_PM = db.MM_MT_Preventive_Maintenance.ToList();
            lstdash.lst_TBM = db.MM_MT_Time_Based_Maintenance.ToList();
            //Commented AFTER MTTU Migration
            //lstdash.lst_CBM = db.MM_MT_Conditional_Based_Maintenance.ToList();
            lstdash.lst_MachineClita = db1.MM_MT_Clita.ToList(); 
            lstdash.lst_StationClita = db.MM_Station_Based_Clita.ToList();
            lstdash.lst_Calibration = db.MM_MT_Calibration.ToList();
            return View(lstdash);
        }
    }
}