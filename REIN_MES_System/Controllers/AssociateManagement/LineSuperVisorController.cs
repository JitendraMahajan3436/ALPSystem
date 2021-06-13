using REIN_MES_System.Helper;
using REIN_MES_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace REIN_MES_System.Controllers.AssociateManagement
{

    public class LineSuperVisorController : Controller
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();

        [HttpGet]
        public ActionResult Index()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var UserId = ((FDSession)this.Session["FDSession"]).userId;
            var LinesBySuperVisor = (from line in db.RS_Lines
                                     join LineSuperVisor in db.RS_AM_Line_Supervisor_Mapping
                                     on line.Line_ID equals LineSuperVisor.Line_ID
                                     where LineSuperVisor.Employee_ID == UserId
                                     && LineSuperVisor.Plant_ID == plantId
                                     && line.Is_Conveyor == true
                                     select line
                                    ).ToList();
            return View(LinesBySuperVisor);
        }

        public ActionResult IndexPartial()
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            var UserId = ((FDSession)this.Session["FDSession"]).userId;
            var LinesBySuperVisor = (from line in db.RS_Lines
                                     join LineSuperVisor in db.RS_AM_Line_Supervisor_Mapping
                                     on line.Line_ID equals LineSuperVisor.Line_ID
                                     where LineSuperVisor.Employee_ID == UserId
                                     && LineSuperVisor.Plant_ID == plantId
                                     && line.Is_Conveyor == true
                                     select line
                                    ).ToList();
            return PartialView(LinesBySuperVisor);
        }

        [HttpPost]
        public ActionResult Edit(int Line_ID, TimeSpan? TACT_Time, decimal Line_Stop_Percentage)
        {
            try
            {
                var LinesData = db.RS_Lines.Where(c => c.Line_ID == Line_ID).FirstOrDefault();
                if (LinesData != null)
                {
                    var OldTactTime = LinesData.TACT_Time;
                    var OldLineStop = LinesData.Line_Stop_Percentage;
                    LinesData.TACT_Time = TACT_Time;
                    LinesData.Updated_Date = DateTime.Now;
                    LinesData.Line_Stop_Percentage = Line_Stop_Percentage;
                    LinesData.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    LinesData.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    LinesData.Is_Edited = true;
                    db.Entry(LinesData).State = EntityState.Modified;
                    db.SaveChanges();

                    RS_History_Line_TACT_Time TactTime = new RS_History_Line_TACT_Time();

                    TactTime.Old_TACT_Time = OldTactTime;
                    TactTime.New_TACT_Time = LinesData.TACT_Time;
                    TactTime.Old_Line_Stop_Percent = OldLineStop;
                    TactTime.New_Line_Stop_Percent1 = LinesData.Line_Stop_Percentage;
                    TactTime.Inserted_Date = DateTime.Now;
                    TactTime.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    TactTime.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    TactTime.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
                    TactTime.Shop_ID = ((FDSession)this.Session["FDSession"]).shopId;
                    TactTime.Line_ID = LinesData.Line_ID;
                    TactTime.Station_ID = ((FDSession)this.Session["FDSession"]).stationId;
                    db.RS_History_Line_TACT_Time.Add(TactTime);
                    db.SaveChanges();
                    return RedirectToAction("IndexPartial");
                }
                return View(LinesData);

            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
