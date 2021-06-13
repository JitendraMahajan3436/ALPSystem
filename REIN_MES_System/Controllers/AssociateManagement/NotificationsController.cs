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
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class NotificationsController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        RS_AM_Notification mmnotificationObj = new RS_AM_Notification();
        GlobalData globalData = new GlobalData();

        int plantId = 0, shopId = 0;
        General generalObj = new General();

        // GET: /Notifications/
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            globalData.pageTitle = ResourceNotifications.Notifications_Title_Lists;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Notifications";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceNotifications.Notifications_Title_Lists;
            globalData.contentFooter = ResourceNotifications.Notifications_Title_Lists;
            ViewBag.GlobalDataModel = globalData;

            var RS_AM_Notification = db.RS_AM_Notification.Include(m => m.RS_Employee).Include(m => m.RS_Employee1).Include(m => m.RS_Plants).Include(m => m.RS_Shops).Where(p => p.Plant_ID == plantId);
            return View(RS_AM_Notification.ToList());
        }

        // GET: /Notifications/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Notification RS_AM_Notification = db.RS_AM_Notification.Find(id);
            if (RS_AM_Notification == null)
            {
                return HttpNotFound();
            }

            globalData.pageTitle = ResourceNotifications.Notifications_Title_Notifications_Detail;
            globalData.subTitle = ResourceNotifications.Notifications_Title_Notifications_Detail;
            globalData.controllerName = "Notifications";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceNotifications.Notifications_Title_Notifications_Detail;
            globalData.contentFooter = ResourceNotifications.Notifications_Title_Notifications_Detail;
            ViewBag.GlobalDataModel = globalData;

            return View(RS_AM_Notification);
        }

        // GET: /Notifications/Create
        public ActionResult Create()
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.subTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.controllerName = "Notifications";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.contentFooter = ResourceNotifications.Notifications_Title_Add_Notifications;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", plantId);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name");
            return View();
        }

        // POST: /Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RS_AM_Notification RS_AM_Notification)
        {
            plantId = ((FDSession)this.Session["FDSession"]).plantId;
            if (ModelState.IsValid)
            {
                RS_AM_Notification.Inserted_Date = DateTime.Now;
                RS_AM_Notification.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                RS_AM_Notification.Inserted_Host = ((FDSession)this.Session["FDSession"]).userHost;
                RS_AM_Notification.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;

                db.RS_AM_Notification.Add(RS_AM_Notification);
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceNotifications.Notifications;
                globalData.messageDetail = ResourceNotifications.Notifications_Success_Add_Success;
                TempData["globalData"] = globalData;

                return RedirectToAction("Index");
            }

            globalData.pageTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.subTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.controllerName = "Notifications";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.contentFooter = ResourceNotifications.Notifications_Title_Add_Notifications;
            ViewBag.GlobalDataModel = globalData;

            //plantId = Convert.ToInt16(RS_AM_Notification.Plant_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name", RS_AM_Notification.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name", RS_AM_Notification.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_AM_Notification.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_AM_Notification.Shop_ID);

            return View(RS_AM_Notification);
        }

        // GET: /Notifications/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Notification RS_AM_Notification = db.RS_AM_Notification.Find(id);
            if (RS_AM_Notification == null)
            {
                return HttpNotFound();
            }

            plantId = ((FDSession)this.Session["FDSession"]).plantId;

            globalData.pageTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.subTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.controllerName = "Notifications";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceNotifications.Notifications_Title_Add_Notifications;
            globalData.contentFooter = ResourceNotifications.Notifications_Title_Add_Notifications;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name", RS_AM_Notification.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name", RS_AM_Notification.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_AM_Notification.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_AM_Notification.Shop_ID);
            return View(RS_AM_Notification);
        }

        // POST: /Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RS_AM_Notification RS_AM_Notification)
        {
            try
            {
                plantId = ((FDSession)this.Session["FDSession"]).plantId;
                mmnotificationObj = new RS_AM_Notification();
                if (ModelState.IsValid)
                {
                    mmnotificationObj = db.RS_AM_Notification.Find(RS_AM_Notification.Notification_ID);

                    mmnotificationObj.Shop_ID = RS_AM_Notification.Shop_ID;
                    mmnotificationObj.Display_Date = RS_AM_Notification.Display_Date;
                    mmnotificationObj.Message = RS_AM_Notification.Message;

                    plantId = ((FDSession)this.Session["FDSession"]).plantId;
                    mmnotificationObj.Inserted_Date = db.RS_AM_Notification.Find(RS_AM_Notification.Notification_ID).Inserted_Date;
                    mmnotificationObj.Inserted_User_ID = db.RS_AM_Notification.Find(RS_AM_Notification.Notification_ID).Inserted_User_ID;
                    mmnotificationObj.Inserted_Host = db.RS_AM_Notification.Find(RS_AM_Notification.Notification_ID).Inserted_Host;


                    mmnotificationObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    mmnotificationObj.Updated_Date = DateTime.Now;
                    mmnotificationObj.Updated_Host = ((FDSession)this.Session["FDSession"]).userHost;
                    mmnotificationObj.Is_Edited = true;
                    db.Entry(mmnotificationObj).State = EntityState.Modified;
                    db.SaveChanges();

                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceNotifications.Notifications;
                    globalData.messageDetail = ResourceNotifications.Notifications_Success_Edit_Success;
                    TempData["globalData"] = globalData;
                    return RedirectToAction("Index");
                }
                ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_AM_Notification.Shop_ID);
                return View(RS_AM_Notification);
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceNotifications.Notifications;
                globalData.messageDetail = ex.ToString();
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            globalData.pageTitle = ResourceNotifications.Notifications_Title_Edit_Notifications;
            globalData.subTitle = ResourceNotifications.Notifications_Title_Edit_Notifications;
            globalData.controllerName = "Notifications";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceNotifications.Notifications_Title_Edit_Notifications;
            globalData.contentFooter = ResourceNotifications.Notifications_Title_Edit_Notifications;
            ViewBag.GlobalDataModel = globalData;

            ViewBag.Inserted_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name", RS_AM_Notification.Inserted_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.RS_Employee.Where(p => p.Plant_ID == plantId), "Employee_ID", "Employee_Name", RS_AM_Notification.Updated_User_ID);
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(p => p.Plant_ID == plantId), "Plant_ID", "Plant_Name", RS_AM_Notification.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(p => p.Plant_ID == plantId), "Shop_ID", "Shop_Name", RS_AM_Notification.Shop_ID);
            return View(RS_AM_Notification);
        }

        // GET: /Notifications/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_AM_Notification RS_AM_Notification = db.RS_AM_Notification.Find(id);
            if (RS_AM_Notification == null)
            {
                return HttpNotFound();
            }
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceNotifications.Notifications;
            globalData.subTitle = ResourceGlobal.Delete;
            globalData.controllerName = "Notifications";
            globalData.actionName = ResourceGlobal.Delete;
            globalData.contentTitle = ResourceNotifications.Notifications_Title_Delete_Notifications;
            globalData.contentFooter = ResourceNotifications.Notifications_Title_Delete_Notifications;
            ViewBag.GlobalDataModel = globalData;
            return View(RS_AM_Notification);
        }

        // POST: /Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            RS_AM_Notification RS_AM_Notification = db.RS_AM_Notification.Find(id);
            try
            {
                db.RS_AM_Notification.Remove(RS_AM_Notification);
                db.SaveChanges();


                generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "RS_AM_Notification", "Notification_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceNotifications.Notifications;
                globalData.messageDetail = ResourceNotifications.Notifications_Success_Delete_Success;

                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                globalData.isErrorMessage = true;
                globalData.messageTitle = ResourceNotifications.Notifications;
                globalData.messageDetail = ResourceGlobal.Delete_Reference_Error;
                TempData["globalData"] = globalData;
                return RedirectToAction("Delete", RS_AM_Notification);

            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetTodaysNews(int shopId)
        {
            try
            {
                //RS_AM_Notification[] notificationObj = new RS_AM_Notification().getNotificationByShop(shopId);
                DateTime date = DateTime.Now;
                var res = (from notificationObj in db.RS_AM_Notification
                           where ( System.Data.Entity.DbFunctions.TruncateTime(notificationObj.From_Date) <= date.Date
                           && System.Data.Entity.DbFunctions.TruncateTime(notificationObj.To_Date) >= date.Date)
                           && notificationObj.Shop_ID == shopId
                           select new
                           {
                               Message = notificationObj.Message
                           });

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTodaysBirthday(int shopId)
        {
            try
            {
                //RS_AM_Notification[] notificationObj = new RS_AM_Notification().getBirthday(shopId);
                DateTime date = DateTime.Now;
                int dayOfYear = date.DayOfYear;
                String dateObj = "1999-" + date.Month + "-" + date.Day;
                var res = from employeeObj in db.RS_Employee
                          where (from operatorStationAllocationObj in db.RS_AM_Operator_Station_Allocation where operatorStationAllocationObj.Shop_ID == shopId select operatorStationAllocationObj.Employee_ID).Contains(employeeObj.Employee_ID)
                          && employeeObj.DOB.Value.Month == date.Month && employeeObj.DOB.Value.Day == date.Day
                          //&& EntityFunctions.TruncateTime(employeeObj.DOB).Value.Day == date.Day && EntityFunctions.TruncateTime(employeeObj.DOB).Value.Month == date.Month
                          select new
                          {

                              Employee_No = employeeObj.Employee_No,
                              Employee_Name = employeeObj.Employee_Name
                          };
                String SQLQuery = "";
                SQLQuery = "SELECT * ";
                SQLQuery += " FROM RS_Employee";
                SQLQuery += " WHERE DATEPART(d, DOB) = DATEPART(d, GETDATE())";
                SQLQuery += " AND DATEPART(m, DOB) = DATEPART(m, GETDATE())";
                SQLQuery += " and Employee_ID in(select Employee_ID from RS_AM_Operator_Station_Allocation where Shop_ID=" + shopId + ")";
                //string SQLQuery = "call MangerModule();";
                var objectContext = ((IObjectContextAdapter)db).ObjectContext;
                List<object> listobj = new List<object>();
                List<RS_Employee> data = objectContext.ExecuteStoreQuery<RS_Employee>(SQLQuery).AsQueryable().ToList();

                var shopID = ((FDSession)this.Session["FDSession"]).shopId;
                var lineID = ((FDSession)this.Session["FDSession"]).lineId;
                var stationID = ((FDSession)this.Session["FDSession"]).stationId;

                //RS_Quality_Captures mmQualityCapturesObj = new RS_Quality_Captures();
                //mmQualityCapturesObj.getNextSerialNumber(25, 4, 1);
                //mmQualityCapturesObj.getNextSerialNumber(stationID, lineID, shopID);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
