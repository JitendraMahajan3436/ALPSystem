using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class SlideScreenController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        // GET: SlideScreen
        public ActionResult Index()
        {
            // process to get the shop id of login user form line supervisor mapping
            int userId = ((FDSession)this.Session["FDSession"]).userId;
            
            RS_AM_Line_Supervisor_Mapping []mmAMLineSupervisorMappingObj = db.RS_AM_Line_Supervisor_Mapping.Where(p => p.Employee_ID == userId).ToArray();
            if (mmAMLineSupervisorMappingObj.Count()>0)
            {
                ViewBag.Shop_ID = mmAMLineSupervisorMappingObj[0].Shop_ID;
            }
            return View();
        }

        public ActionResult News()
        {
            return View();
        }
    }
}