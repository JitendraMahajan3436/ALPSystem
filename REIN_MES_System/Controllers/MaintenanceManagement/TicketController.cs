using ZHB_AD.App_LocalResources;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;
using ZHB_AD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    public class TicketController : BaseController
    {

        ZHB_ADEntities db = new ZHB_ADEntities();
        GlobalData globalData = new GlobalData();
        MM_MT_Ticket mmticket = new MM_MT_Ticket();

        FDSession fdSession;

        General generalObj = new General();

        // GET: Ticket
        public ActionResult Close(string ticket)
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }


            ticket = ticket.Replace("\"", string.Empty).Trim();
            if (db.MM_MT_Ticket.Where(x => x.Ticket_Number.ToString().ToLower() == ticket.ToString().ToLower().Trim() && x.Is_Ticket_Closed != true).Count() > 0)
            {
                //check user who are related to particular ticket and make sure only respective users are login and should not allowed for other users
                if (db.MM_MT_Ticket.Where(x => x.Ticket_Number.ToString().ToLower() == ticket.ToString().ToLower().Trim()).Count() > 0)
                {
                    decimal userid = ((FDSession)this.Session["FDSession"]).userId;
                    decimal id = db.MM_MT_Ticket.Where(x => x.Ticket_Number.ToString().ToLower() == ticket.ToString().ToLower().Trim()).FirstOrDefault().T_ID;
                    if (db.MM_MT_Ticket_Users.Where(x => x.T_ID == id && userid == x.User_ID).Count() > 0)
                    {
                        var users = db.MM_MT_Ticket_Users.Where(x => x.T_ID == id).Select(x => x.User_ID).ToList();
                        ViewBag.users = new MultiSelectList(db.MM_Employee, "Employee_ID", "Employee_Name", users);
                        return View(db.MM_MT_Ticket.Where(x => x.Ticket_Number.ToString().ToLower() == ticket.ToString().ToLower()).FirstOrDefault());
                    }
                    else
                    {
                        globalData.pageTitle = "Ticket ";
                        globalData.subTitle = ResourceGlobal.Create;
                        globalData.controllerName = "Ticket";
                        globalData.actionName = ResourceGlobal.Create;

                        globalData.isErrorMessage = true;
                        globalData.messageTitle = "Ticket Close";
                        globalData.messageDetail = "You are not authorized user to close this ticket please re-login with correct credentials!";
                        ViewBag.GlobalDataModel = globalData;
                        TempData["globalData"] = globalData;
                        //    decimal userid = ((FDSession)this.Session["FDSession"]).userId;
                        //     decimal id = db.MM_MT_Ticket.Where(x => x.Ticket_Number.ToString().ToLower() == ticket.ToString().ToLower().Trim()).FirstOrDefault().T_ID;
                        var users = db.MM_MT_Ticket_Users.Where(x => x.T_ID == id).Select(x => x.User_ID).ToList();
                        ViewBag.users = new MultiSelectList(db.MM_Employee, "Employee_ID", "Employee_Name", users);
                        return View(db.MM_MT_Ticket.Where(x => x.Ticket_Number.ToString().ToLower() == ticket.ToString().ToLower()).FirstOrDefault());
                    }
                }
                else
                {
                    return View();
                }

            }
            else
            {
                globalData.pageTitle = "Ticket ";
                globalData.subTitle = ResourceGlobal.Create;
                globalData.controllerName = "Ticket";
                globalData.actionName = ResourceGlobal.Create;

                globalData.isErrorMessage = true;
                globalData.messageTitle = "Ticket Close";
                globalData.messageDetail = "Given link is not valid. This Ticket is already closed previously. Thank you!";
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                decimal userid = ((FDSession)this.Session["FDSession"]).userId;
                decimal id = db.MM_MT_Ticket.Where(x => x.Ticket_Number.ToString().ToLower() == ticket.ToString().ToLower().Trim()).FirstOrDefault().T_ID;
                var users = db.MM_MT_Ticket_Users.Where(x => x.T_ID == id).Select(x => x.User_ID).ToList();
                ViewBag.users = new MultiSelectList(db.MM_Employee, "Employee_ID", "Employee_Name", users);
                return View(db.MM_MT_Ticket.Where(x => x.Ticket_Number.ToString().ToLower() == ticket.ToString().ToLower()).FirstOrDefault());
            }
        }


        public ActionResult SaveRemark(decimal id)
        {
            // ViewBag.id = id;
            var obj = db.MM_MT_Ticket.Find(id);

            var users = db.MM_MT_Ticket_Users.Where(x => x.T_ID == id).Select(x => x.User_ID).ToList();

            return PartialView("CloseTicket", obj);
        }

        [HttpPost]
        public ActionResult Close(FormCollection fc)
        {
            MM_MT_Ticket mm_MT_Ticket = new MM_MT_Ticket();
            string id = fc["T_ID"].ToString();
            mm_MT_Ticket = db.MM_MT_Ticket.Where(x => x.T_ID.ToString() == id).FirstOrDefault();
            mm_MT_Ticket.Is_Ticket_Closed = true;
            mm_MT_Ticket.Remark = fc["Remark"].ToString();
            mm_MT_Ticket.Is_Edited = true;
            db.Entry(mm_MT_Ticket).State = EntityState.Modified;
            db.SaveChanges();
            var users = db.MM_MT_Ticket_Users.Where(x => x.T_ID.ToString() == id.ToString()).Select(x => x.User_ID).ToList();
            ViewBag.users = new MultiSelectList(db.MM_Employee, "Employee_ID", "Employee_Name", users);
            globalData.pageTitle = "Ticket ";
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Ticket";
            globalData.actionName = ResourceGlobal.Create;

            globalData.isSuccessMessage = true;
            globalData.messageTitle = "Ticket Close";
            globalData.messageDetail = "Ticket has been closed. Thank you!";
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(mm_MT_Ticket);
        }


        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Ticket;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Ticket";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Ticket + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Ticket + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;

            int shopID = ((FDSession)this.Session["FDSession"]).shopId;
            decimal userID = ((FDSession)this.Session["FDSession"]).userId;
            var ticket_user = db.MM_Ticket_User.Where(user => user.Employee_ID == userID).ToList();

            if (ticket_user.Count > 0)
            {
                ViewBag.flag = 1;
                var ticket_Details = db.MM_Ticket.Where(ticket => ticket.Shop_ID == shopID && ticket.Ticket_Status == "OPEN");
                return View(ticket_Details.ToList());
            }
            else
            {
                ViewBag.flag = 0;
                return View();
            }


        }
        public ActionResult CloseTicket(int rowId, string status, string remark)
        {
            string ticketStatus = db.MM_Ticket.Where(ticket => ticket.Ticket_ID == rowId).Select(t => t.Ticket_Status).FirstOrDefault();
            if (ticketStatus == "OPEN" && status == "CLOSE")
            {
                MM_Ticket mM_Ticket = new MM_Ticket();
                mM_Ticket = db.MM_Ticket.Find(rowId);
                mM_Ticket.Ticket_Status = status;
                mM_Ticket.Acknowledge_Remark = remark;
                mM_Ticket.Ticket_Closed_Date = DateTime.Now;
                mM_Ticket.Ticket_Closed_Employee_ID = ((FDSession)this.Session["FDSession"]).userId;
                mM_Ticket.Updated_Date = DateTime.Now;
                mM_Ticket.Is_Edited = true;
                db.Entry(mM_Ticket).State = EntityState.Modified;
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceModules.Ticket;
                globalData.messageDetail = ResourceModules.Ticket + " " + ResourceMessages.close_Ticket;
                TempData["globalData"] = globalData;

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceModules.Ticket;
                globalData.messageDetail = ResourceModules.Ticket + " " + ResourceMessages.allready_closed;
                TempData["globalData"] = globalData;
                //allready_closed
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetModuleName(int rowId)
        {
            decimal? modelid = db.MM_Ticket.Where(t => t.Ticket_ID == rowId).Select(t => t.Module_ID).FirstOrDefault();
            var moduleName = (from ticket in db.MM_Ticket
                              join mod in db.MM_Notification_Modules on ticket.Module_ID equals mod.Module_ID
                              where mod.Module_ID == modelid
                              select mod.Module_Name).FirstOrDefault();
            return Json(new { module_Name = moduleName }, JsonRequestBehavior.AllowGet);
        }
    }
}