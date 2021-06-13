using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REIN_MES_System.Models;
using REIN_MES_System.Controllers.BaseManagement;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.AssociateManagement
{
    public class HelpDeskController_bkp : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        
        GlobalData globalData = new GlobalData();
        
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0, categoryId = 0, userId=0;

        // GET: /HelpDesk/
        public ActionResult Index()
        {
            var RS_Help_Desk = db.RS_Help_Desk.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Help_Category).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations);
            return View(RS_Help_Desk.ToList());
        }
        
        public ActionResult ShowHelpDeskOpenTickets()
        {
            var RS_Help_Desk = db.RS_Help_Desk.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Help_Category).Include(m => m.RS_Lines).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Include(m => m.RS_Stations).Where(p => p.Is_Ticket_Closed == false || p.Is_Ticket_Closed == null);
            return PartialView(RS_Help_Desk.ToList());
        }

        // GET: /HelpDesk/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Help_Desk RS_Help_Desk = db.RS_Help_Desk.Find(id);
            if (RS_Help_Desk == null)
            {
                return HttpNotFound();
            }
            return View(RS_Help_Desk);
        }

        // GET: /HelpDesk/Create
        public ActionResult Create()
        {
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Help_Category_ID = new SelectList(db.RS_Help_Category, "Help_Category_ID", "Help_Category_Name");
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name");
            return View();
        }

        // POST: /HelpDesk/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Help_Desk_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Help_Category_ID,Is_Ticket_Closed,Remark,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Help_Desk RS_Help_Desk)
        {
            if (ModelState.IsValid)
            {
                db.RS_Help_Desk.Add(RS_Help_Desk);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Desk.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Desk.Updated_User_ID);
            ViewBag.Help_Category_ID = new SelectList(db.RS_Help_Category, "Help_Category_ID", "Help_Category_Name", RS_Help_Desk.Help_Category_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Help_Desk.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Help_Desk.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Help_Desk.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Help_Desk.Station_ID);
            return View(RS_Help_Desk);
        }

        // GET: /HelpDesk/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Help_Desk RS_Help_Desk = db.RS_Help_Desk.Find(id);
            if (RS_Help_Desk == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Desk.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Desk.Updated_User_ID);
            ViewBag.Help_Category_ID = new SelectList(db.RS_Help_Category, "Help_Category_ID", "Help_Category_Name", RS_Help_Desk.Help_Category_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Help_Desk.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Help_Desk.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Help_Desk.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Help_Desk.Station_ID);
            return View(RS_Help_Desk);
        }

        // POST: /HelpDesk/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Help_Desk_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Help_Category_ID,Is_Ticket_Closed,Remark,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] RS_Help_Desk RS_Help_Desk)
        {
            if (ModelState.IsValid)
            {
                RS_Help_Desk.Is_Edited = true;
                db.Entry(RS_Help_Desk).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Desk.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name", RS_Help_Desk.Updated_User_ID);
            ViewBag.Help_Category_ID = new SelectList(db.RS_Help_Category, "Help_Category_ID", "Help_Category_Name", RS_Help_Desk.Help_Category_ID);
            ViewBag.Line_ID = new SelectList(db.RS_Lines, "Line_ID", "Line_Name", RS_Help_Desk.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_Help_Desk.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_Help_Desk.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.RS_Stations, "Station_ID", "Station_Name", RS_Help_Desk.Station_ID);
            return View(RS_Help_Desk);
        }

        // GET: /HelpDesk/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_Help_Desk RS_Help_Desk = db.RS_Help_Desk.Find(id);
            if (RS_Help_Desk == null)
            {
                return HttpNotFound();
            }
            return View(RS_Help_Desk);
        }

        // POST: /HelpDesk/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_Help_Desk RS_Help_Desk = db.RS_Help_Desk.Find(id);
            db.RS_Help_Desk.Remove(RS_Help_Desk);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }        

        public ActionResult addRequest(int categoryId)
        {
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                shopId = ((FDSession)this.Session["FDSession"]).shopId;
                lineId = ((FDSession)this.Session["FDSession"]).lineId;
                stationId = ((FDSession)this.Session["FDSession"]).stationId;
                userId = ((FDSession)this.Session["FDSession"]).userId;

                RS_Help_Desk mmHelpDeskObj = new RS_Help_Desk();
                mmHelpDeskObj.Plant_ID = plantId;
                mmHelpDeskObj.Shop_ID = shopId;
                mmHelpDeskObj.Line_ID = lineId;
                mmHelpDeskObj.Station_ID = stationId;
                mmHelpDeskObj.Help_Category_ID = categoryId;
                mmHelpDeskObj.Inserted_User_ID = userId;

                mmHelpDeskObj.Inserted_Date = DateTime.Now;
                mmHelpDeskObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.RS_Help_Desk.Add(mmHelpDeskObj);
                db.SaveChanges();
                
                return Json(true,JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult saveHelpTicket(int ticketId, String remark)
        {
            try
            {
                RS_Help_Desk mmHelpDeskObj = db.RS_Help_Desk.Find(ticketId);
                mmHelpDeskObj.Remark = remark;
                mmHelpDeskObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mmHelpDeskObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;

                mmHelpDeskObj.Is_Ticket_Closed = true;
                mmHelpDeskObj.Is_Edited = true;
                db.Entry(mmHelpDeskObj).State = EntityState.Modified;
                db.SaveChanges();


                return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch(Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
