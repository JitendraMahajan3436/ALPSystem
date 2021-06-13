using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using System.Net.NetworkInformation;
using ZHB_AD.App_LocalResources;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace ZHB_AD.Controllers.AssociateManagement
{
    public class HelpDeskController : BaseController
    {
        private ZHB_ADEntities db = new ZHB_ADEntities();
        GlobalData globalData = new GlobalData();
        int plantId = 0, shopId = 0, lineId = 0, stationId = 0, categoryId = 0, userId = 0;

        // GET: /HelpDesk/
        public ActionResult Index()
        {
            //double day = DateTime.Now.Day;
            //double month = DateTime.Now.Month;
            //double year = DateTime.Now.Year;

            //var mm_help_desk = db.MM_Help_Desk.Where(x => x.Inserted_Date.Day == day && x.Inserted_Date.Month == month && x.Inserted_Date.Year == year).ToList();
            double day = DateTime.Now.Day;
            double month = DateTime.Now.Month;
            double year = DateTime.Now.Year;
            var PB = (from t in db.MM_SAP_Production_Booking.AsEnumerable() where t.Inserted_Date.Value.Day == day && t.Inserted_Date.Value.Month == month && t.Inserted_Date.Value.Year == year select t).ToList();
            var EC = (from t in db.MM_SAP_Equipment_Creation.AsEnumerable() where t.Inserted_Date.Value.Day == day && t.Inserted_Date.Value.Month == month && t.Inserted_Date.Value.Year == year select t).ToList();
            // var HD = (from t in db.MM_Help_Desk.AsEnumerable() where t.Inserted_Date.Day == day && t.Inserted_Date.Month == month && t.Inserted_Date.Year == year select t).ToList();
            var HD = db.MM_Help_Desk.Where(x => x.Inserted_Date.Day == day && x.Inserted_Date.Month == month && x.Inserted_Date.Year == year).Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Help_Category).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations).Where(p => p.Is_Ticket_Closed == false || p.Is_Ticket_Closed == null);
            MESSAPLIST ij = new MESSAPLIST();
            ij.lst_PB = new List<MM_SAP_Production_Booking>();
            ij.lst_PB.AddRange((PB));
            ij.lst_EC = new List<MM_SAP_Equipment_Creation>();
            ij.lst_EC.AddRange((EC));
            ij.lst_HD = new List<MM_Help_Desk>();
            ij.lst_HD.AddRange(HD);

            //add viewbags for help Issue Matrix
            //issue type,issue log,issue code,issue code name


            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Help_Category_ID = new SelectList(db.MM_Help_Category, "Help_Category_ID", "Help_Category_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            ViewBag.Issue_Type_ID = new SelectList(db.MM_Help_Issue_Type, "Issue_Type_ID", "Issue_Type");
            ViewBag.Issue_Log_Type_ID = new SelectList(db.MM_Help_Issue_Log_Type, "Issue_Log_Type_ID", "Issue_Log_Type");
            ViewBag.Issue_Code_ID = new SelectList(db.MM_Help_Issue_Code, "Issue_Code_ID", "Issue_Code");
            ViewBag.Issue_Code_Name_ID = new SelectList(db.MM_Help_Issue_Code_Name, "Issue_Code_Name_ID", "Issue_Code_Name");
            globalData.contentTitle = "Diagnostic Screen";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            globalData.pageTitle = "Diagnostic Screen";
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "HelpDesk";
            globalData.actionName = "Index";
            globalData.contentFooter = "Diagnostic Screen";

            ViewBag.GlobalDataModel = globalData;

            return View(ij);
        }

        public ActionResult ShowHelpDeskOpenTickets()
        {
            double day = DateTime.Now.Day;
            double month = DateTime.Now.Month;
            double year = DateTime.Now.Year;
            var HD = db.MM_Help_Desk.Where(x => x.Inserted_Date.Day == day && x.Inserted_Date.Month == month && x.Inserted_Date.Year == year).Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Help_Category).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations).Where(p => p.Is_Ticket_Closed == false || p.Is_Ticket_Closed == null).OrderByDescending(x=>x.Inserted_Date);
            return PartialView(HD);
        }

        //public ActionResult ShowServerStatus()
        //{

        //    foreach (var item in db.MM_Servers_List.ToList())
        //    {
        //        var result = pingTable(item.IP_Address);
        //        MM_Servers_List server = db.MM_Servers_List.Where(x=>x.IP_Address==item.IP_Address).FirstOrDefault();
        //        if (result)
        //            server.Status = true;
        //        else
        //            server.Status = false;
        //        db.Entry(server).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    var serverslst = db.MM_Servers_List.ToList();
        //    return PartialView(serverslst);
        //}

        public ActionResult ShowPB()
        {
            double day = DateTime.Now.Day;
            double month = DateTime.Now.Month;
            double year = DateTime.Now.Year;
            var PB = (from t in db.MM_SAP_Production_Booking.AsEnumerable() where t.Inserted_Date.Value.Day == day && t.Inserted_Date.Value.Month == month && t.Inserted_Date.Value.Year == year select t).ToList();
            return PartialView(PB);
        }

        public ActionResult GetClientStatus()
        {
            Dictionary<string, Dictionary<string, bool>> ServerStatus = new Dictionary<string, Dictionary<string, bool>>();
            var ipaddress = db.MM_Stations.Where(x => x.Station_IP_Address != null && x.Station_IP_Address != "" && x.Station_IP_Address != " ").ToList();

            foreach (var item in ipaddress)
            {
                var result = pingTable(item.Station_IP_Address);
                if (!result)
                {
                    Dictionary<string, bool> station_Result = new Dictionary<string, bool>();
                    station_Result.Add(db.MM_Stations.Where(x => x.Station_IP_Address == item.Station_IP_Address).FirstOrDefault().Station_Name, result);
                    if (!ServerStatus.Keys.Contains(item.Station_IP_Address))
                        ServerStatus.Add(item.Station_IP_Address, station_Result);
                }
            }

            return Json(ServerStatus.ToList(),JsonRequestBehavior.AllowGet);
        }


        public bool pingTable(string ipAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(ipAddress);
                pingable = reply.Status == IPStatus.Success;

            }
            catch (PingException ex)
            {

            }

            return pingable;

        }

        public ActionResult ShowOrder()
        {
            double day = DateTime.Now.Day;
            double month = DateTime.Now.Month;
            double year = DateTime.Now.Year;

            //get started orders
            var OD =
               from post in db.MM_OM_Order_List
               join meta in db.MM_Partgroup on post.Partgroup_ID equals meta.Partgroup_ID
               where meta.Order_Create == true && post.Inserted_Date.Day == day && post.Inserted_Date.Month == month && post.Inserted_Date.Year == year
               select post;

            //get created orders
            var OC =
              from post in db.MM_OM_OrderRelease
              where post.Inserted_Date.Day == day && post.Inserted_Date.Month == month && post.Inserted_Date.Year == year
              select post;

            //get booked orders
            var PB =
              from post in db.MM_SAP_Production_Booking
              where post.Inserted_Date.Value.Day == day && post.Inserted_Date.Value.Month == month && post.Inserted_Date.Value.Year == year
              select post;

            MESSAPLIST obj = new MESSAPLIST();
            obj.lst_PB = new List<MM_SAP_Production_Booking>();
            obj.lst_PB.AddRange(PB);

            obj.lst_OC = new List<MM_OM_OrderRelease>();
            obj.lst_OC.AddRange(OC);

            obj.lst_OD = new List<MM_OM_Order_List>();
            obj.lst_OD.AddRange(OD);

            // var OD = (from t in db.MM_OM_Order_List.AsEnumerable() where  select t).ToList();
            return PartialView(obj);
        }

        // GET: /HelpDesk/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Help_Desk mm_help_desk = db.MM_Help_Desk.Find(id);
            if (mm_help_desk == null)
            {
                return HttpNotFound();
            }
            return View(mm_help_desk);
        }

        // GET: /HelpDesk/Create
        public ActionResult Create()
        {
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Help_Category_ID = new SelectList(db.MM_Help_Category, "Help_Category_ID", "Help_Category_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            ViewBag.Issue_Type_ID = new SelectList(db.MM_Help_Issue_Type, "Issue_Type_ID", "Issue_Type");
            ViewBag.Issue_Log_Type_ID = new SelectList(db.MM_Help_Issue_Log_Type, "Issue_Type_ID", "Issue_Type");
            ViewBag.Issue_Code_ID = new SelectList(db.MM_Help_Issue_Code, "Issue_Code_ID", "Issue_Code");
            ViewBag.Issue_Code_Name_ID = new SelectList(db.MM_Help_Issue_Code_Name, "Issue_Code_Name_ID", "Issue_Code_Name");
            return View();
        }

        // POST: /HelpDesk/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Help_Desk_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Help_Category_ID,Is_Ticket_Closed,Remark,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_Help_Desk mm_help_desk)
        {
            if (ModelState.IsValid)
            {
                db.MM_Help_Desk.Add(mm_help_desk);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_help_desk.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_help_desk.Updated_User_ID);
            ViewBag.Help_Category_ID = new SelectList(db.MM_Help_Category, "Help_Category_ID", "Help_Category_Name", mm_help_desk.Help_Category_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mm_help_desk.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mm_help_desk.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_help_desk.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mm_help_desk.Station_ID);

            return View(mm_help_desk);
        }

        // GET: /HelpDesk/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Help_Desk mm_help_desk = db.MM_Help_Desk.Find(id);
            if (mm_help_desk == null)
            {
                return HttpNotFound();
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_help_desk.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_help_desk.Updated_User_ID);
            ViewBag.Help_Category_ID = new SelectList(db.MM_Help_Category, "Help_Category_ID", "Help_Category_Name", mm_help_desk.Help_Category_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mm_help_desk.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mm_help_desk.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_help_desk.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mm_help_desk.Station_ID);
            return View(mm_help_desk);
        }

        public ActionResult GetHelpCallDetails(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            MM_Help_Desk hlpdesk = new MM_Help_Desk();
            hlpdesk= db.MM_Help_Desk.Find(id);
            return Json(hlpdesk,JsonRequestBehavior.AllowGet);
        }

        // POST: /HelpDesk/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Help_Desk_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Help_Category_ID,Is_Ticket_Closed,Remark,Is_Transferred,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_Host,Updated_User_ID,Updated_Date")] MM_Help_Desk mm_help_desk)
        {
            if (ModelState.IsValid)
            {
                mm_help_desk.Is_Edited = true;
                db.Entry(mm_help_desk).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_help_desk.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mm_help_desk.Updated_User_ID);
            ViewBag.Help_Category_ID = new SelectList(db.MM_Help_Category, "Help_Category_ID", "Help_Category_Name", mm_help_desk.Help_Category_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mm_help_desk.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mm_help_desk.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mm_help_desk.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mm_help_desk.Station_ID);
            return View(mm_help_desk);
        }

        // GET: /HelpDesk/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_Help_Desk mm_help_desk = db.MM_Help_Desk.Find(id);
            if (mm_help_desk == null)
            {
                return HttpNotFound();
            }
            return View(mm_help_desk);
        }

        // POST: /HelpDesk/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            MM_Help_Desk mm_help_desk = db.MM_Help_Desk.Find(id);
            db.MM_Help_Desk.Remove(mm_help_desk);
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

                MM_Help_Desk mmHelpDeskObj = new MM_Help_Desk();
                mmHelpDeskObj.Plant_ID = plantId;
                mmHelpDeskObj.Shop_ID = shopId;
                mmHelpDeskObj.Line_ID = lineId;
                mmHelpDeskObj.Station_ID = stationId;
                mmHelpDeskObj.Help_Category_ID = categoryId;
                mmHelpDeskObj.Inserted_User_ID = userId;

                mmHelpDeskObj.Inserted_Date = DateTime.Now;
                mmHelpDeskObj.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                db.MM_Help_Desk.Add(mmHelpDeskObj);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult saveHelpTicket(int ticketId, String remark, bool? Is_Ack, bool? Is_Resolved, string Ack_By, string Attended_By, string Resolved_By, TimeSpan? Resolved_time,decimal? Issue_Type_ID,decimal?Issue_Log_Type_ID,decimal? Issue_Code_ID,decimal? Issue_Code_Name_ID)
        {
            try
            {
                MM_Help_Desk mmHelpDeskObj = db.MM_Help_Desk.Find(ticketId);
                mmHelpDeskObj.Remark = remark;
                mmHelpDeskObj.Is_Ack = Is_Ack;
                mmHelpDeskObj.Ack_By = Ack_By;
                if (Is_Ack != false)
                {
                    mmHelpDeskObj.Ack_Time = DateTime.Now;
                }
                if (Is_Resolved == true)
                {
                    mmHelpDeskObj.Is_Resolved = Is_Resolved;
                    mmHelpDeskObj.Is_Ticket_Closed = true;
                }
                mmHelpDeskObj.Issue_Type_ID = Issue_Type_ID;
                mmHelpDeskObj.Issue_Log_Type_ID = Issue_Log_Type_ID;
                mmHelpDeskObj.Issue_Code_ID = Issue_Code_ID;
                mmHelpDeskObj.Issue_Code_Name_ID = Issue_Code_Name_ID;
                mmHelpDeskObj.Resolved_By = Resolved_By;
                mmHelpDeskObj.Attended_By = Attended_By;
                mmHelpDeskObj.Resolved_Time = Resolved_time;
                mmHelpDeskObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                mmHelpDeskObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                mmHelpDeskObj.Is_Edited = true;
                db.Entry(mmHelpDeskObj).State = EntityState.Modified;
                db.SaveChanges();


                return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLogType(decimal id)
        {
            var LogType = db.MM_Help_Issue_Log_Type.Where(x => x.Issue_Type_ID == id).Select(x => new { x.Issue_Log_Type_ID, x.Issue_Log_Type });
            return Json(LogType, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCode(decimal id)
        {
            var Code = db.MM_Help_Issue_Code.Where(x => x.Issue_Log_Type_ID == id).Select(x => new { x.Issue_Code_ID, x.Issue_Code });
            return Json(Code, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCodeName(decimal id)
        {
            var CodeName = db.MM_Help_Issue_Code_Name.Where(x => x.Issue_Code_ID == id).Select(x => new { x.Issue_Code_Name_ID, x.Issue_Code_Name });
            return Json(CodeName, JsonRequestBehavior.AllowGet);
        }


        public ActionResult HelpCallData()
        {
            ViewBag.Station_ID = new SelectList(db.MM_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.MM_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.MM_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.MM_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "HelpDesk";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Help Desk";
            globalData.contentFooter = "Help Desk";
            globalData.pageTitle = "Help Desk";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult HelpCallDataWRTtime()
        {
            ViewBag.Station_ID = new SelectList(db.MM_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.MM_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.MM_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.MM_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "HelpDesk";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Help Desk";
            globalData.contentFooter = "Help Desk";
            globalData.pageTitle = "Help Desk";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult HelpCallOccurance()
        {
            ViewBag.Station_ID = new SelectList(db.MM_Stations.ToList(), "Station_ID", "Station_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(x => x.Shop_ID != 6 && x.Shop_ID != 7).OrderBy(c => c.Shop_Name), "Shop_ID", "Shop_Name", 0);
            ViewBag.Line_ID = new SelectList(db.MM_Lines.ToList(), "Line_ID", "Line_Name");
            //ViewBag.Order_State = new SelectList(db.MM_Lines.Where(m => m.Line_ID == 0), "Line_ID", "Line_Name");
            ViewBag.Model = new SelectList(db.MM_Model_Master, "Model_Code", "Model_Description", 0);
            ViewBag.Series = new SelectList(db.MM_Series, "Series_Code", "Series_Description", 0);
            globalData.actionName = "HelpDesk";
            globalData.controllerName = "Reports";
            globalData.contentTitle = "Help Desk";
            globalData.contentFooter = "Help Desk";
            globalData.pageTitle = "Help Desk";
            globalData.subTitle = App_LocalResources.ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            return View();
        }

        public ActionResult GetPlan()
        {
            var yr = DateTime.Now.Year;
            var mm = DateTime.Now.Month;
            var dd = DateTime.Now.Day;
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            var shopID = ((FDSession)this.Session["FDSession"]).shopId;
            var lineID = ((FDSession)this.Session["FDSession"]).lineId;


            string Queries = "";
            Queries = Queries + @"WITH cte AS(SELECT a.Model_Code,b.Series_Description,a.RSN,ROW_NUMBER() OVER(ORDER BY a.RSN)"
                       + "- ROW_NUMBER() OVER(PARTITION BY a.Model_Code ORDER BY a.RSN) AS rn"
                          + " FROM  MM_OM_Planned_Orders a"
                          + " JOIN  MM_Series b ON a.Series_Code = b.Series_Code"
                          + " WHERE a.Shop_ID = " + shopID + ""
                          + " AND CONVERT(DATE, a.Planned_Date) = CONVERT(DATE, GETDATE()))"
                          + " SELECT rn,Model_Code,Series_Description,COUNT(*) AS Qty,SUM(COUNT(*)) OVER(ORDER BY MAX(RSN)) AS Cumul FROM cte"
                          + " GROUP BY rn,Model_Code,Series_Description ORDER BY Cumul";



            var sConnection = ((SqlConnection)db.Database.Connection);

            DataTable dt = new DataTable();
            if (sConnection != null && sConnection.State == ConnectionState.Closed)
            {

                sConnection.Open();
            }
            using (SqlDataAdapter ad = new SqlDataAdapter())
            {
                SqlDataAdapter com = new SqlDataAdapter(Queries, sConnection);
                com.Fill(dt);
            }
            ViewData.Model = dt;
            Queries = "";
            Queries = Queries + @"WITH cte AS(SELECT a.Model_Code,b.Series_Description,a.RSN,ROW_NUMBER() OVER(ORDER BY a.RSN)"
                       + "- ROW_NUMBER() OVER(PARTITION BY a.Model_Code ORDER BY a.RSN) AS rn"
                          + " FROM  MM_OM_Planned_Orders a"
                          + " JOIN  MM_Series b ON a.Series_Code = b.Series_Code"
                          + " WHERE a.Shop_ID = " + shopID + " AND a.Line_ID="+ lineID
                          + " AND CONVERT(DATE, a.Planned_Date) = CONVERT(DATE, GETDATE()))"
                          + " SELECT rn,Model_Code,Series_Description,COUNT(*) AS Qty,SUM(COUNT(*)) OVER(ORDER BY MAX(RSN)) AS Cumul FROM cte"
                          + " GROUP BY rn,Model_Code,Series_Description ORDER BY Cumul";
             sConnection = ((SqlConnection)db.Database.Connection);

            DataTable dt1 = new DataTable();
            if (sConnection != null && sConnection.State == ConnectionState.Closed)
            {

                sConnection.Open();
            }
            using (SqlDataAdapter ad = new SqlDataAdapter())
            {
                SqlDataAdapter com = new SqlDataAdapter(Queries, sConnection);
                com.Fill(dt1);
            }
           
            ViewBag.ModelTractor = dt1;
            return View();
        }


        public ActionResult TodayPlan()
        {
            ViewBag.Shop_ID = new SelectList(db.MM_Shops.Where(x=>x.Shop_ID!=6 && x.Shop_ID!=5), "Shop_ID", "Shop_Name");
            return View();
        }

        public ActionResult odayPlan(decimal shopid)
        {
            var yr = DateTime.Now.Year;
            var mm = DateTime.Now.Month;
            var dd = DateTime.Now.Day;
            var plantID = ((FDSession)this.Session["FDSession"]).plantId;
            //   var shopID = ((FDSession)this.Session["FDSession"]).shopId;
            var lineID = ((FDSession)this.Session["FDSession"]).lineId;


            string Queries = "";
            Queries = Queries + @"WITH cte AS(SELECT a.Model_Code,b.Series_Description,a.RSN,ROW_NUMBER() OVER(ORDER BY a.RSN)"
                       + "- ROW_NUMBER() OVER(PARTITION BY a.Model_Code ORDER BY a.RSN) AS rn"
                          + " FROM  MM_OM_Planned_Orders a"
                          + " JOIN  MM_Series b ON a.Series_Code = b.Series_Code"
                          + " WHERE a.Shop_ID = " + shopid + ""
                          + " AND CONVERT(DATE, a.Planned_Date) = CONVERT(DATE, GETDATE()))"
                          + " SELECT rn,Model_Code,Series_Description,COUNT(*) AS Qty,SUM(COUNT(*)) OVER(ORDER BY MAX(RSN)) AS Cumul FROM cte"
                          + " GROUP BY rn,Model_Code,Series_Description ORDER BY Cumul";

            var sConnection = ((SqlConnection)db.Database.Connection);

            DataTable dt = new DataTable();
            if (sConnection != null && sConnection.State == ConnectionState.Closed)
            {

                sConnection.Open();
            }
            using (SqlDataAdapter ad = new SqlDataAdapter())
            {
                SqlDataAdapter com = new SqlDataAdapter(Queries, sConnection);
                com.Fill(dt);
            }
            ViewData.Model = dt;
            var jsonobjdatatable = DataTableToJsonWithJavaScriptSerializer(dt);
            return Json(jsonobjdatatable, JsonRequestBehavior.AllowGet);

        }

        public string DataTableToJsonWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

    }
}
