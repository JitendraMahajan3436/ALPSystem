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
using REIN_MES_System.App_LocalResources;
using REIN_MES_System.Helper;

namespace REIN_MES_System.Controllers.OrderManagement
{
    public class ManageOrdersController : BaseController
    {
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        int plantId = 0, shopId = 0;
        GlobalData globalData = new GlobalData();
        General generalHelper = new General();
        // GET: ManageOrders
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }

            globalData.pageTitle = ResourceModules.Hold_Order;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "ManageOrders";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Hold_Order;
            globalData.contentFooter = ResourceModules.Hold_Order;

            ViewBag.GlobalDataModel = globalData;

            int plantID = ((FDSession)this.Session["FDSession"]).plantId;
            ViewBag.Plant_ID = new SelectList(db.RS_Plants.Where(m => m.Plant_ID == plantID), "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops.Where(m=>m.Plant_ID== plantID), "Shop_ID", "Shop_Name");
            //ViewBag.Line_ID = new SelectList(db.RS_Lines.Where(p => p.Shop_ID == shopId), "Line_ID", "Line_Name");
            ViewBag.Platform_ID = new SelectList(db.RS_OM_Platform.Where(p => p.Shop_ID == shopId), "Platform_ID", "Platform_Name");


            //var RS_OM_OrderRelease = db.RS_OM_OrderRelease.Include(m => m.RS_Plants).Include(m => m.RS_Shops);
            return View();
        }

        /*               Action Name               : GetPlantID
        *               Description               : Action used to Get the shop Id of Model Master
        *               Author, Timestamp         : Jitendra Mahajan
        *               Input parameter           : id
        *               Return Type               : ActionResult
        *               Revision                  : 1
       */
        //Find Shop 
        public ActionResult GetPlantID(int Plant_Id)
        {
            var Shop_Id = db.RS_Shops
                                       .Where(c => c.Plant_ID == Plant_Id)
                                       .Select(c => new { c.Shop_ID, c.Shop_Name });

            return Json(Shop_Id, JsonRequestBehavior.AllowGet);
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult GetHoldData(int Plant_ID = 0, int Shop_ID = 0 , int Platform_ID = 0)
        {
            int Line_ID = 0;
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;
            Line_ID = Convert.ToInt16(db.RS_OM_Platform.Where(m => m.Platform_ID == Platform_ID && m.Plant_ID == plantId).Select(m => m.Line_ID).FirstOrDefault());
            int userID = ((FDSession)this.Session["FDSession"]).userId;
            var order = (from release in db.RS_OM_OrderRelease
                         orderby release.RSN ascending
                         where release.Plant_ID == Plant_ID && release.Shop_ID == Shop_ID && release.Line_ID==Line_ID  && release.Order_Status == "Hold" && release.Hold_By_PPC != true
                         //&& release.Updated_User_ID == userID
                         select release).ToList();

            DataTable dt = GlobalOperations.ToDataTable<RS_OM_OrderRelease>(order);

            return PartialView("ManageOrder", dt);
        }

        public decimal? GetLineIdByPlatform(decimal? platfromId = 0)
        {
            var plantId = ((FDSession)this.Session["FDSession"]).plantId;

            var lineId = db.RS_OM_Platform.Where(m => m.Platform_ID == platfromId && m.Plant_ID == plantId).Select(m => m.Line_ID).FirstOrDefault();
            return lineId;
        }

        /*               Action Name               : GetLineID
         *               Description               : Action used to return the list of Line ID for Mange Orders
         *               Author, Timestamp         : Jitendra Mahajan
         *               Input parameter           : Shopid
         *               Return Type               : ActionResult
         *               Revision                  : 1
        */
        //Find Line
        public ActionResult GetPlatformID(int Shop_id, int PlatformId = 0)
        {
            var PlatformDetail = (from platform in db.RS_OM_Platform
                              join partgroup in db.RS_Partgroup on platform.Line_ID equals partgroup.Line_ID
                              where platform.Shop_ID == Shop_id  && partgroup.Order_Create == true
                              select new
                              {
                                  platform.Platform_Name,
                                  platform.Platform_ID
                              }).Distinct().ToList();
            //var lineDetail = (from line in db.RS_Lines
            //                  where line.Shop_ID == Shop_id
            //                  select new
            //                  {
            //                      line.Line_Name,
            //                      line.Line_ID
            //                  }).ToList();
            return Json(PlatformDetail, JsonRequestBehavior.AllowGet);
        }
       


        public ActionResult HoldOrder(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            // process to change the statuse to hold order

            RS_OM_OrderRelease RS_OM_OrderRelease = db.RS_OM_OrderRelease.Where(p => p.Order_No == id).Single();
            if (RS_OM_OrderRelease == null)
            {
                return HttpNotFound();
            }

            RS_OM_OrderRelease s = (from orderRelease in db.RS_OM_OrderRelease
                                    where orderRelease.Order_No == id
                                    select orderRelease).First();

            s.Order_Status = "Hold";
          
            s.Hold_By_PPC = false;
           
            s.Order_Start = true;
            s.Manage_Remarks = RS_OM_OrderRelease.Manage_Remarks;

            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ResumeOrder(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // process to change the status to resume order


            RS_OM_OrderRelease RS_OM_OrderRelease = db.RS_OM_OrderRelease.Where(p => p.Order_No == id).Single();
            if (RS_OM_OrderRelease == null)
            {
                return HttpNotFound();
            }

            RS_OM_OrderRelease s = (from orderRelease in db.RS_OM_OrderRelease
                                    where orderRelease.Order_No == id
                                    select orderRelease).First();

            s.Order_Status = "Release";
            s.Hold_By_PPC = false;
            s.Order_Start = false;
            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: ManageOrders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_OrderRelease RS_OM_OrderRelease = db.RS_OM_OrderRelease.Find(id);
            if (RS_OM_OrderRelease == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_OrderRelease);
        }

        // GET: ManageOrders/Create
        public ActionResult Create()
        {
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name");
            return View();
        }

        // POST: ManageOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Row_ID,Plant_ID,Shop_ID,Plant_OrderNo,Order_No,Model_Code,Model_Color,Order_Type,Country,Priority,Order_Status,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Is_Active,Is_Deleted,Remarks")] RS_OM_OrderRelease RS_OM_OrderRelease)
        {
            if (ModelState.IsValid)
            {
                db.RS_OM_OrderRelease.Add(RS_OM_OrderRelease);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_OrderRelease.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_OM_OrderRelease.Shop_ID);
            return View(RS_OM_OrderRelease);
        }

        // GET: ManageOrders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_OrderRelease RS_OM_OrderRelease = db.RS_OM_OrderRelease.Find(id);
            if (RS_OM_OrderRelease == null)
            {
                return HttpNotFound();
            }
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_OrderRelease.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_OM_OrderRelease.Shop_ID);
            return View(RS_OM_OrderRelease);
        }

        // POST: ManageOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Row_ID,Plant_ID,Shop_ID,Plant_OrderNo,Order_No,Model_Code,Model_Color,Order_Type,Country,Priority,Order_Status,Is_Transfered,Is_Purgeable,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Is_Active,Is_Deleted,Remarks")] RS_OM_OrderRelease RS_OM_OrderRelease)
        {
            if (ModelState.IsValid)
            {
                db.Entry(RS_OM_OrderRelease).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Plant_ID = new SelectList(db.RS_Plants, "Plant_ID", "Plant_Name", RS_OM_OrderRelease.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.RS_Shops, "Shop_ID", "Shop_Name", RS_OM_OrderRelease.Shop_ID);
            return View(RS_OM_OrderRelease);
        }

        // GET: ManageOrders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RS_OM_OrderRelease RS_OM_OrderRelease = db.RS_OM_OrderRelease.Find(id);
            if (RS_OM_OrderRelease == null)
            {
                return HttpNotFound();
            }
            return View(RS_OM_OrderRelease);
        }

        // POST: ManageOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RS_OM_OrderRelease RS_OM_OrderRelease = db.RS_OM_OrderRelease.Find(id);
            db.RS_OM_OrderRelease.Remove(RS_OM_OrderRelease);
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

        public ActionResult changeOrderState(String status, String orderNo, String remark, int shopId)
        {
            try
            {
                //for current shift
                DateTime date = DateTime.Now;
                TimeSpan currentTime = date.TimeOfDay;
                var plantId = ((FDSession)this.Session["FDSession"]).plantId;
                var shift = db.RS_Shift.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId ).Select(m => m).OrderByDescending(m=>m.Shift_ID).FirstOrDefault();
                ////
                RS_OM_OrderRelease s = (from orderRelease in db.RS_OM_OrderRelease
                                        where orderRelease.Order_No == orderNo && orderRelease.Shop_ID == shopId
                                        select orderRelease).First();

                decimal cntrRSN = 0;
                cntrRSN = db.RS_OM_OrderRelease.Where(or => or.Plant_ID == s.Plant_ID && or.Shop_ID == s.Shop_ID && or.Line_ID == s.Line_ID && or.Order_Status == "Release" && or.Order_Start == false)
                            .Max(a => a.RSN);
                //for adding unholded order at bottom of current shift and increaseing production count of current shift
                var ppcDailyPlan = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == s.Shop_ID && m.Line_ID == s.Line_ID && m.Plan_Date == date.Date && m.Shift_ID == shift.Shift_ID).Select(m => m).FirstOrDefault();
                if (ppcDailyPlan != null)
                {
                    ppcDailyPlan.Planned_Qty++;
                    ppcDailyPlan.Is_Edited = true;
                    ppcDailyPlan.Is_Edited = true;
                    ppcDailyPlan.Updated_Date = DateTime.Now;
                    ppcDailyPlan.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.Entry(ppcDailyPlan).State = EntityState.Modified;
                    db.SaveChanges();
                }
             
                ///
                ++cntrRSN;

                s.Order_Status = "Release";
                s.Hold_By_PPC = false;
                s.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                s.Updated_Date = DateTime.Now;
              
                //s.Planned_Shift_ID = shift.Shift_ID; 
                s.Manage_Remarks = remark;
                
                s.RSN = cntrRSN;     //new rsn at last

                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();

                RS_OM_Order_Remarks omOrdrRemarks = new RS_OM_Order_Remarks();
                omOrdrRemarks.Order_ID = s.Row_ID;
                omOrdrRemarks.Remark_Category = "UnHold";
                omOrdrRemarks.Remark_Msg = remark;
                omOrdrRemarks.Inserted_UserID = ((FDSession)this.Session["FDSession"]).userId;
                omOrdrRemarks.Inserted_Time = DateTime.Now;
                db.RS_OM_Order_Remarks.Add(omOrdrRemarks);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //HOLD ORDER FROM SHOPFLOOR BY CELL MEMBER
        public ActionResult changeOrderStateLinewise(string status, int rowID)
        {
            String orderNo = "";
            String remark = "";
            int shopId = ((FDSession)this.Session["FDSession"]).shopId;
            int lineId = ((FDSession)this.Session["FDSession"]).lineId;
            int plantId = ((FDSession)this.Session["FDSession"]).plantId;
            //for current shift
            DateTime date = DateTime.Now;
            TimeSpan currentTime = date.TimeOfDay;

            var shift = db.RS_Shift.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Shift_Start_Time <= currentTime && currentTime <= m.Shift_End_Time).Select(m => m).FirstOrDefault();
            ////
            RS_OM_OrderRelease ordrRlsObj = db.RS_OM_OrderRelease.Find(rowID);
            orderNo = ordrRlsObj.Order_No;
            try
            {
                int RSN = 0;
                try
                {
                    //GET NEW RSN FOR THE HOLDED ORDER
                    RSN = generalHelper.updatePlannedRSN(rowID, ((FDSession)this.Session["FDSession"]).userId);

                    //SAVE THE HOLDED ORDER ID WITH THE NEW RSN INTO TABLE RS_OM_ShopFloor_Holded_RSN
                    RS_OM_ShopFloor_Holded_RSN shopFloorObj = new RS_OM_ShopFloor_Holded_RSN();
                    shopFloorObj.Order_ID = rowID;
                    shopFloorObj.Inserted_Date = DateTime.Now;
                    shopFloorObj.RSN = RSN;
                    db.RS_OM_ShopFloor_Holded_RSN.Add(shopFloorObj);
                    db.SaveChanges();
                }
                catch (Exception exp)
                {
                    while (exp.InnerException != null)
                    {
                        exp = exp.InnerException;
                    }
                    generalHelper.addControllerException(exp, "ManageOrdersController", "new Logic For PPC Hold Order", ((FDSession)this.Session["FDSession"]).userId);
                }

                RS_OM_OrderRelease s = db.RS_OM_OrderRelease.Find(rowID);
                s.Order_Status = status;
                if(status.Equals("hold", StringComparison.CurrentCultureIgnoreCase))
                {
                    s.Hold_By_PPC = false;
                }
                s.Manage_Remarks = remark;
                s.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                s.Updated_Date = DateTime.Now;
                s.RSN = RSN;
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();
                //for adding unholded order at bottom of current shift and increaseing production count of current shift
                //var ppcDailyPlan = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == shopId && m.Line_ID == lineId && m.Plan_Date == date.Date && m.Shift_ID == shift.Shift_ID).Select(m => m).FirstOrDefault();
                //ppcDailyPlan.Planned_Qty++;
                //ppcDailyPlan.Is_Edited = true;
                //ppcDailyPlan.Is_Edited = true;
                //ppcDailyPlan.Updated_Date = DateTime.Now;
                //ppcDailyPlan.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                //db.Entry(ppcDailyPlan).State = EntityState.Modified;
                //db.SaveChanges();
                //////
                RS_OM_Planned_Orders plannedOrdersObj = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == rowID && a.Planned_Date == DateTime.Today).FirstOrDefault();
                if (plannedOrdersObj != null)
                {
                    plannedOrdersObj.Order_Status = status;
                    plannedOrdersObj.Last_Status_Change_Time = DateTime.Now;
                    db.Entry(plannedOrdersObj).State = EntityState.Modified;
                    db.SaveChanges();
                }

                RS_OM_Order_Remarks omOrdrRemarks = new RS_OM_Order_Remarks();
                omOrdrRemarks.Order_ID = rowID;
                omOrdrRemarks.Remark_Category = "Hold";
                omOrdrRemarks.Remark_Msg = remark;
                omOrdrRemarks.Inserted_UserID = ((FDSession)this.Session["FDSession"]).userId;
                omOrdrRemarks.Inserted_Time = DateTime.Now;
                db.RS_OM_Order_Remarks.Add(omOrdrRemarks);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "OrderStartController", "ManageOrders", ((FDSession)this.Session["FDSession"]).userId);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //UnHold From PPC ReSequencing Screen by PPC User
        public ActionResult UnHoldOrders()
        {
            try
            {
                decimal empID = ((FDSession)this.Session["FDSession"]).userId;
                decimal shopid = ((FDSession)this.Session["FDSession"]).shopId;
                var holdOrdersObjList = db.RS_OM_OrderRelease.Where(a => a.Order_Status == "Hold" && a.Shop_ID == shopid && a.Updated_User_ID == empID).Distinct().OrderByDescending(a => a.Updated_Date);

                return View(holdOrdersObjList);
            }
            catch (Exception ex)
            {
                generalHelper.addControllerException(ex, "ManageOrdersController", "UnHoldOrders", ((FDSession)this.Session["FDSession"]).userId);
            }
            return HttpNotFound();
        }

        //UnHold From Shop Floor Screen by Cell Member
        public ActionResult UnHoldOrder(int rowID, string remark)
        {
            RS_OM_OrderRelease RS_OM_OrderRelease = null;
            try
            {
                int plantId = ((FDSession)this.Session["FDSession"]).plantId;
                int shopId = Decimal.ToInt32(((FDSession)this.Session["FDSession"]).shopId);
                //for current shift
                DateTime date = DateTime.Now;
                TimeSpan currentTime = date.TimeOfDay;

                var shift = db.RS_Shift.Where(m => m.Plant_ID == plantId && m.Shop_ID == shopId && m.Shift_Start_Time <= currentTime && currentTime <= m.Shift_End_Time).Select(m => m).FirstOrDefault();
                ////
                RS_OM_OrderRelease = db.RS_OM_OrderRelease.Find(rowID);
                if (RS_OM_OrderRelease == null)
                {
                    return HttpNotFound();
                }
                generalHelper.addLockTable(Convert.ToInt16(RS_OM_OrderRelease.Plant_ID), Convert.ToInt16(RS_OM_OrderRelease.Shop_ID), Convert.ToInt16(RS_OM_OrderRelease.Line_ID), "RS_OM_OrderRelease", ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
                decimal plantID = RS_OM_OrderRelease.Plant_ID;
                decimal shopID = RS_OM_OrderRelease.Shop_ID;
                decimal? lineID = RS_OM_OrderRelease.Line_ID;

                //Take the CURRENT Orders from ORderRelease Table
                //var old_OrderList = db.RS_OM_OrderRelease.Where(or => or.Plant_ID == plantID && or.Shop_ID == shopID && or.Line_ID == lineID && or.Order_Status == "Release")
                //              .AsEnumerable()
                //              .OrderBy(or => or.RSN)
                //              .Select(a => a.Row_ID).ToList();

                decimal cntrRSN = 0;
                //Take the current smallest RSN
                cntrRSN = db.RS_OM_OrderRelease.Where(or => or.Plant_ID == RS_OM_OrderRelease.Plant_ID && or.Shop_ID == RS_OM_OrderRelease.Shop_ID && or.Line_ID == RS_OM_OrderRelease.Line_ID && or.Order_Status == "Release")
                            .Min(a => a.RSN);
                cntrRSN--;
                //TAKE MAX RSN FROM ORDER WHICH ARE NOT STARTED OR HOLDED IN RS_OM_Planned_Orders TABLE
                //RS_OM_Planned_Orders plannedOrdersObj = db.RS_OM_Planned_Orders.Find(rowID);

                //decimal cntrRSN = plannedOrdersObj.RSN.GetValueOrDefault(0);

                //Update status and RSN in OM_OrderRelease Table
                RS_OM_OrderRelease.RSN = cntrRSN;
                RS_OM_OrderRelease.Order_Status = "Release";
                RS_OM_OrderRelease.Hold_By_PPC = false;
                RS_OM_OrderRelease.Order_Start = false;
                RS_OM_OrderRelease.Manage_Remarks = remark;
                RS_OM_OrderRelease.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                RS_OM_OrderRelease.Updated_Date = DateTime.Now;

                //for adding unholded order at bottom of current shift and increaseing production count of current shift
                //var ppcDailyPlan = db.RS_OM_PPC_Daily_Plan.Where(m => m.Shop_ID == shopID &&m.Line_ID==lineID && m.Plan_Date == date.Date && m.Shift_ID == shift.Shift_ID).Select(m => m).FirstOrDefault();
                //ppcDailyPlan.Planned_Qty++;
                //ppcDailyPlan.Is_Edited = true;
                //ppcDailyPlan.Is_Edited = true;
                //ppcDailyPlan.Updated_Date = DateTime.Now;
                //ppcDailyPlan.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                //db.Entry(ppcDailyPlan).State = EntityState.Modified;
                //db.SaveChanges();
                ////

                RS_OM_OrderRelease.Planned_Shift_ID = shift.Shift_ID;
                db.Entry(RS_OM_OrderRelease).State = EntityState.Modified;
                db.SaveChanges();

                //Update RSN in RS_OM_Planned_Orders Table
                RS_OM_Planned_Orders plannedOrdersObj = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == rowID && a.Planned_Date == DateTime.Today).FirstOrDefault();
                if (plannedOrdersObj != null)
                {
                    plannedOrdersObj.RSN = cntrRSN;
                    plannedOrdersObj.Order_Status = "Release";
                    plannedOrdersObj.Last_Status_Change_Time = DateTime.Now;
                    db.Entry(plannedOrdersObj).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //RS_OM_Planned_Orders plannedOrdrObj = db.RS_OM_Planned_Orders.Where(a => a.Order_ID == rowID).FirstOrDefault();
                //plannedOrdrObj.RSN = cntrRSN;
                //db.SaveChanges();

                //Add Data in Remarks Log table
                RS_OM_Order_Remarks omOrdrRemarks = new RS_OM_Order_Remarks();
                omOrdrRemarks.Order_ID = rowID;
                omOrdrRemarks.Remark_Category = "UnHold";
                omOrdrRemarks.Remark_Msg = remark;
                omOrdrRemarks.Inserted_UserID = ((FDSession)this.Session["FDSession"]).userId;
                omOrdrRemarks.Inserted_Time = DateTime.Now;
                db.RS_OM_Order_Remarks.Add(omOrdrRemarks);
                db.SaveChanges();
                //------------------------------------------

                //Increment The RSN of all the Old/Current Orders by one
                //foreach (RS_OM_Planned_Orders plannedOrderObj in plannedOrdersObjList)
                //{
                //    --cntrRSN;
                //    //var newOrderObj = db.RS_OM_OrderRelease.Find(plannedOrderObj.Order_ID);

                //    //newOrderObj.RSN = rsn;

                //    //newOrderObj.Updated_Date = DateTime.Now;
                //    //newOrderObj.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                //    //db.Entry(newOrderObj).State = EntityState.Modified;
                //    //db.SaveChanges();

                //    plannedOrderObj.RSN = cntrRSN;
                //    db.Entry(plannedOrderObj).State = EntityState.Modified;
                //    db.SaveChanges();

                //}

                //UPDATE THE Planned Orders Table with new data
                //updatePlannedOrders(shopID);
                //------------------------------------------
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                generalHelper.addControllerException(ex, "ManageOrdersController", "UnHoldOrder(rowID:" + rowID + ")", ((FDSession)this.Session["FDSession"]).userId);
            }
            //finally
            //{
            //    generalHelper.updateLockTable(Convert.ToInt16(RS_OM_OrderRelease.Plant_ID), Convert.ToInt16(RS_OM_OrderRelease.Shop_ID), Convert.ToInt16(RS_OM_OrderRelease.Line_ID), "RS_OM_OrderRelease", ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);
            //}
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        private void updatePlannedOrders(decimal shopID)
        {
            try
            {
                DateTime Today = DateTime.Today;
                decimal lineID = db.RS_Partgroup.Where(a => a.Shop_ID == shopID && a.Order_Create == true).Select(a => a.Line_ID).FirstOrDefault();
                int totalDeleted = 0;
                //TAKE TODAYS SHOPS PLANNED QTY
                int todaysPlan = db.RS_OM_PPC_Daily_Plan.Where(a => a.Shop_ID == shopID && a.Plan_Date == Today)
                                   .Select(a => a.Planned_Qty).FirstOrDefault();
                int todaysDone = db.RS_OM_Planned_Orders.Where(a => a.Order_Status != "Release" && a.Shop_ID == shopID && a.Planned_Date == Today)
                                   .Count();
                //DELETE ALL TODAYS ORDER WITH RELEASED STATUS FROM PLANNED TABLE
                string sqlQuery = "DELETE FROM RS_OM_Planned_Orders WHERE Order_Status = 'Release' AND Shop_ID = @p0 AND Planned_Date = @p1";
                totalDeleted = db.Database.ExecuteSqlCommand(sqlQuery, shopID, Today);
                int insertQty = 0;
                if (todaysDone <= todaysPlan)
                {
                    insertQty = todaysPlan - todaysDone;
                }
                //if (totalDeleted > todaysPlan)
                //{
                //    totalDeleted = todaysPlan;
                //}
                sqlQuery = "INSERT INTO RS_OM_Planned_Orders " +
                           " (Plant_ID,Shop_ID,Group_No,Order_ID,Order_No,Order_Status,Parent_Model_Code,Parent_Series_Code,Model_Code,Series_Code,RSN,Planned_Date,Inserted_Time) SELECT TOP " + insertQty +
                           " a.Plant_ID,a.Shop_ID,0,a.Row_ID,Order_No,Order_Status,a.Model_Code,b.Series_Code,a.partno,a.Series_Code,RSN,CONVERT(DATE,GETDATE()),a.Inserted_Date" +
                           " FROM RS_OM_OrderRelease a " +
                           " JOIN RS_Model_Master b ON a.Model_Code = b.Model_Code" +
                           " WHERE a.Shop_ID = @p0 AND a.Line_ID = @p1 AND a.Order_Status = 'Release' ORDER BY RSN";
                db.Database.ExecuteSqlCommand(sqlQuery, shopID, lineID);

            }
            catch (Exception exp)
            {
                while (exp.InnerException != null)
                {
                    exp = exp.InnerException;
                }
                generalHelper.addControllerException(exp, "OMOrderPlanningController", "updatePlannedOrders(Shop_ID:" + shopID + ")", ((FDSession)this.Session["FDSession"]).userId);
            }
        }

    }
}
