using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZHB_AD.Models;
using ZHB_AD.App_LocalResources;
using ZHB_AD.Controllers.BaseManagement;
using ZHB_AD.Helper;

namespace ZHB_AD.Controllers.MaintenanceManagement
{
    /* Controller Name            : Calibration
    *  Description                : To create,edit,delete and show all Calibration Tool 
    *  Author, Timestamp          : Ajay Wagh      
    */
    public class CalibrationController : BaseController
    {
        #region Decalration of Variables
        private REIN_SOLUTIONEntities db = new REIN_SOLUTIONEntities();
        private GlobalData globalData = new GlobalData();
        private MM_MT_Calibration calibration = new MM_MT_Calibration();
        #endregion

        #region Get Details of Calibration Tools

        General generalObj = new General();

        // GET: Calibration
        /*
       * Action Name          : Index
       * Input Parameter      : None
       * Return Type          : ActionResult
       * Author & Time Stamp  : Ajay Wagh
       * Description          : Get the list Calibration Tools against station
       */
        public ActionResult Index()
        {
            if (TempData["globalData"] != null)
            {
                globalData = (GlobalData)TempData["globalData"];
            }
            var mM_MT_Calibration = db.MM_MT_Calibration.Include(m => m.MM_Employee).Include(m => m.MM_Employee1).Include(m => m.MM_Employee2).Include(m => m.MM_Lines).Include(m => m.MM_Plants).Include(m => m.MM_Shops).Include(m => m.MM_Stations);
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Calibration";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Callibration + " " + ResourceGlobal.Lists;
            globalData.contentFooter = ResourceModules.Callibration + " " + ResourceGlobal.Lists;
            ViewBag.GlobalDataModel = globalData;
            // TempData["globalData"] = globalData;
            return View(mM_MT_Calibration.ToList());
        }

        // GET: Calibration/Details/5
        /*
         * Action Name          : Details
         * Input Parameter      : id
         * Return Type          : ActionResult
         * Author & Time Stamp  : Ajay Wagh
         * Description          : Get the Details Calibration Tool against station
         */
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Calibration mM_MT_Calibration = db.MM_MT_Calibration.Find(id);

            if (mM_MT_Calibration == null)
            {
                return HttpNotFound();
            }

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Details;
            globalData.controllerName = "Calibration";
            globalData.actionName = ResourceGlobal.Details;
            globalData.contentTitle = ResourceModules.Callibration +" "+ResourceGlobal.Tool+ " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Callibration +" "+ResourceGlobal.Tool+ " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            // TempData["globalData"] = globalData;
            return View(mM_MT_Calibration);
        }
        #endregion

        #region Create Calibration Tool against line,station,plant,shop
        // GET: Calibration/Create
        /*
        * Action Name          : Create
       * Input Parameter      : None
       * Return Type          : ActionResult
       * Author & Time Stamp  : Ajay Wagh
       * Description          : Get the Create Calibration Tool against station
       */
        public ActionResult Create()
        {

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Create;
            globalData.controllerName = "Calibration";
            globalData.actionName = ResourceGlobal.Create;
            globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Callibration + " " + ResourceGlobal.Tool + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Callibration + " " + ResourceGlobal.Tool + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            //  TempData["globalData"] = globalData;
            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name");
         //   ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name");
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name");
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name");
            return View();
        }

        /*
       * Action Name          : Create
       * Input Parameter      : MM_MT_Calibration
       * Return Type          : ActionResult
       * Author & Time Stamp  : Ajay Wagh
       * Description          : Create Calibration Tool against station
       */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "C_Tool_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Calibration_Tool,Calibration_Description,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Cycle_Period,Receipent_Email,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host,mails,users")] MM_MT_Calibration mM_MT_Calibration)
        {
            mM_MT_Calibration.Plant_ID = 1;
            string mails = "";
            foreach (var item in mM_MT_Calibration.mails)
            {
                if (db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).Count() > 0)
                {
                    mails += db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
                }
                else
                {
                    mails += item.ToString() + "; ";
                    //add email address which are manually entered here
                    //not present in database
                }
                // mails += db.MM_Employee.Where(x => x.Employee_ID.ToString() == item.ToString()).First().Email_Address + ";";
            }

            mM_MT_Calibration.Receipent_Email = mails;


            if (ModelState.IsValid)
            {
                if (db.MM_MT_Calibration.Where(x => x.Plant_ID == mM_MT_Calibration.Plant_ID && x.Line_ID == mM_MT_Calibration.Line_ID && x.Shop_ID == mM_MT_Calibration.Shop_ID && x.Station_ID == mM_MT_Calibration.Station_ID && x.Calibration_Tool == mM_MT_Calibration.Calibration_Tool).Count() > 0)
                {
                    globalData.stationIPAddress = GlobalOperations.GetIP();
                    globalData.hostName = GlobalOperations.GetHostName();
                    globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
                    globalData.subTitle = ResourceGlobal.Create;
                    globalData.controllerName = "Calibration";
                    globalData.actionName = ResourceGlobal.Create;
                    globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Callibration + " " + ResourceGlobal.Tool + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Callibration + " " + ResourceGlobal.Tool + " " + ResourceGlobal.Form;
                    globalData.isErrorMessage = true;
                    globalData.messageTitle = ResourceModules.Callibration;
                    globalData.messageDetail = ResourceValidation.Exist;
                    ViewBag.GlobalDataModel = globalData;
                    TempData["globalData"] = globalData;
                }
                else
                {

                    mM_MT_Calibration.Inserted_Date = DateTime.Now.Date;
                    mM_MT_Calibration.Inserted_Host = HttpContext.Request.UserHostAddress;
                    mM_MT_Calibration.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                    db.MM_MT_Calibration.Add(mM_MT_Calibration);
                    db.SaveChanges();
                    foreach (var item in mM_MT_Calibration.users)
                    {
                        MM_MT_Calibration_Users calibrationuser = new MM_MT_Calibration_Users();
                        calibrationuser.Station_ID = mM_MT_Calibration.Station_ID;
                        calibrationuser.User_ID = item;
                        calibrationuser.C_Tool_ID = mM_MT_Calibration.C_Tool_ID;
                        db.MM_MT_Calibration_Users.Add(calibrationuser);
                        db.SaveChanges();
                    }

                    globalData.stationIPAddress = GlobalOperations.GetIP();
                    globalData.hostName = GlobalOperations.GetHostName();
                    globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
                    globalData.subTitle = ResourceGlobal.Create;
                    globalData.controllerName = "Calibration";
                    globalData.actionName = ResourceGlobal.Create;
                    globalData.contentTitle = ResourceGlobal.Add + " " + ResourceModules.Callibration + " " + ResourceGlobal.Tool + " " + ResourceGlobal.Form;
                    globalData.contentFooter = ResourceGlobal.Add + " " + ResourceModules.Callibration + " " + ResourceGlobal.Tool + " " + ResourceGlobal.Form;
                    globalData.isSuccessMessage = true;
                    globalData.messageTitle = ResourceGlobal.Add + " " + ResourceModules.Callibration + " " + ResourceGlobal.Tool + " " + ResourceGlobal.Form;
                    globalData.messageDetail = ResourceModules.Callibration+" " +ResourceGlobal.Tool+" "+ ResourceMessages.Add_Success;
                    TempData["globalData"] = globalData;
                    ViewBag.GlobalDataModel = globalData;
                    return RedirectToAction("Index");
                }
            }
            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Maintenance_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Updated_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Inserted_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_MT_Calibration.Line_ID);
           // ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Calibration.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_MT_Calibration.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_MT_Calibration.Station_ID);
            return View(mM_MT_Calibration);
        }

        #endregion

        #region Edit Calibration Tool

        // GET: Calibration/Edit/5
        /*
         * Action Name          : Edit
         * Input Parameter      : id
         * Return Type          : ActionResult
         * Author & Time Stamp  : Ajay Wagh
         * Description          : Edit Calibration Tool against station
         */
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Calibration mM_MT_Calibration = db.MM_MT_Calibration.Find(id);
            if (mM_MT_Calibration == null)
            {
                return HttpNotFound();
            }


            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Edit;
            globalData.controllerName = "Calibration";
            globalData.actionName = ResourceGlobal.Edit;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Callibration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Callibration + " " + ResourceGlobal.Form;
            ViewBag.GlobalDataModel = globalData;
            //  TempData["globalData"] = globalData;

            List<string> emails = mM_MT_Calibration.Receipent_Email.Split(';').ToList();
            List<decimal> lst_Selected_EmailsID = new List<decimal>();
            List<decimal> lst_usersids = new List<decimal>();

            var lstusr = db.MM_MT_Calibration_Users.Where(x => x.C_Tool_ID == id).ToList();
            foreach (var usr in lstusr)
            {
                lst_usersids.Add(usr.User_ID);
            }
            foreach (string email in emails)
            {
                if (email != "" && email != " " && email != null)
                {
                    if (db.MM_Employee.Where(x => x.Email_Address.ToLower().Trim() == email.Trim()).Count() > 0)
                    {
                        decimal j = db.MM_Employee.Where(x => x.Email_Address.ToLower().Trim() == email.Trim()).FirstOrDefault().Employee_ID;
                        lst_Selected_EmailsID.Add(j);
                    }
                    //else
                    //{
                    //    lst_Selected_EmailsID.Add(emails.IndexOf(email));
                    //}
                }
            }
            ViewBag.mails = new MultiSelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address", (lst_Selected_EmailsID));
            ViewBag.users = new MultiSelectList(db.MM_Employee, "Employee_ID", "Employee_Name", lst_usersids);
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Maintenance_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Updated_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Inserted_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_MT_Calibration.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Calibration.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_MT_Calibration.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_MT_Calibration.Station_ID);
            return View(mM_MT_Calibration);
        }

        /*
         * Action Name          : Edit
         * Input Parameter      : MM_MT_Calibration
         * Return Type          : ActionResult
         * Author & Time Stamp  : Ajay Wagh
         * Description          : Edit Calibration Tool against station
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "C_Tool_ID,Plant_ID,Shop_ID,Line_ID,Station_ID,Calibration_Tool,Calibration_Description,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Cycle_Period,Receipent_Email,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host,mails,users")] MM_MT_Calibration mM_MT_Calibration)
        {
            mM_MT_Calibration.Plant_ID = 1;
            calibration = db.MM_MT_Calibration.Where(x => x.C_Tool_ID == mM_MT_Calibration.C_Tool_ID).FirstOrDefault();

            if (ModelState.IsValid)
            {
                string strmails = "";
                foreach (var item in mM_MT_Calibration.mails)
                {
                    if (db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).Count() > 0)
                    {
                        strmails += db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
                    }
                    else
                    {
                        strmails += item.ToString() + "; ";
                        //add email address which are manually entered here
                        //not present in database
                    }
                    // decimal j = Convert.ToDecimal(item);
                    // strmails += db.MM_Employee.Where(x => x.Employee_ID == j).FirstOrDefault().Email_Address + "; ";
                }
                //if (strmails == "")
                //{
                //    mM_MT_Calibration.Receipent_Email = mM_MT_Calibration.Receipent_Email;
                //    ViewBag.defaultvalue = mM_MT_Calibration.Receipent_Email;
                //}
                //else
                //    mM_MT_Calibration.Receipent_Email = strmails;

                mM_MT_Calibration.Receipent_Email = strmails;
                calibration.Calibration_Description = mM_MT_Calibration.Calibration_Description;
                calibration.Calibration_Tool = mM_MT_Calibration.Calibration_Tool;
                calibration.Cycle_Period = mM_MT_Calibration.Cycle_Period;
                calibration.Updated_Date = DateTime.Now;
                calibration.Updated_Host = HttpContext.Request.UserHostAddress;
                calibration.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
                calibration.Receipent_Email = mM_MT_Calibration.Receipent_Email;
                calibration.Scheduled_Date = mM_MT_Calibration.Scheduled_Date;
                calibration.Maintenance_User_ID = mM_MT_Calibration.Maintenance_User_ID;

                calibration.Is_Edited = true;
                db.Entry(calibration).State = EntityState.Modified;
                db.SaveChanges();

                var record = db.MM_MT_Calibration_Users.Where(x => x.C_Tool_ID == mM_MT_Calibration.C_Tool_ID).ToList();
                db.MM_MT_Calibration_Users.RemoveRange(record);

                foreach (var item in mM_MT_Calibration.users)
                {
                    MM_MT_Calibration_Users calibrationuser = new MM_MT_Calibration_Users();
                    calibrationuser.C_Tool_ID = calibration.C_Tool_ID;
                    calibrationuser.User_ID = item;
                    calibrationuser.Station_ID = calibration.Station_ID;
                    db.MM_MT_Calibration_Users.Add(calibrationuser);
                    db.SaveChanges();
                }

                globalData.stationIPAddress = GlobalOperations.GetIP();
                globalData.hostName = GlobalOperations.GetHostName();
                globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
                globalData.subTitle = ResourceGlobal.Edit;
                globalData.controllerName = "Calibration";
                globalData.actionName = ResourceGlobal.Edit;
                globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Callibration + " " + ResourceGlobal.Form;
                globalData.contentFooter = ResourceGlobal.Edit + " " + ResourceModules.Callibration + " " + ResourceGlobal.Form;

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceGlobal.Edit + " " + ResourceModules.Callibration + " " + ResourceGlobal.Form;
                globalData.messageDetail = ResourceModules.Callibration + " " + ResourceMessages.Edit_Success;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return RedirectToAction("Index");
            }
            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Maintenance_User_ID);
            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Updated_User_ID);
            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Calibration.Inserted_User_ID);
            ViewBag.Line_ID = new SelectList(db.MM_Lines, "Line_ID", "Line_Name", mM_MT_Calibration.Line_ID);
            ViewBag.Plant_ID = new SelectList(db.MM_Plants, "Plant_ID", "Plant_Name", mM_MT_Calibration.Plant_ID);
            ViewBag.Shop_ID = new SelectList(db.MM_Shops, "Shop_ID", "Shop_Name", mM_MT_Calibration.Shop_ID);
            ViewBag.Station_ID = new SelectList(db.MM_Stations, "Station_ID", "Station_Name", mM_MT_Calibration.Station_ID);
            return View(mM_MT_Calibration);
        }

        #endregion

        #region Delete Calibration
        /*
         * Action Name          : Delete
         * Input Parameter      : id
         * Return Type          : ActionResult
         * Author & Time Stamp  : Ajay Wagh
         * Description          : Delete Calibration Tool against station
         */
        // GET: Calibration/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MM_MT_Calibration mM_MT_Calibration = db.MM_MT_Calibration.Find(id);
            if (mM_MT_Calibration == null)
            {
                return HttpNotFound();
            }
            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Calibration";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Callibration +" "+ResourceGlobal.Tool+ " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Callibration +" "+ResourceGlobal.Tool+ " " + ResourceGlobal.Details;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(mM_MT_Calibration);
        }

        /*
        * Action Name          : DeleteConfirmed
        * Input Parameter      : id
        * Return Type          : ActionResult
        * Author & Time Stamp  : Ajay Wagh
        * Description          : Delete Calibration Tool against station
        */
        // POST: Calibration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            var cbmuser = db.MM_MT_Calibration_Users.Where(x => x.C_Tool_ID == id).ToList();
            db.MM_MT_Calibration_Users.RemoveRange(cbmuser);
            db.SaveChanges();

            MM_MT_Calibration mM_MT_Calibration = db.MM_MT_Calibration.Find(id);
            db.MM_MT_Calibration.Remove(mM_MT_Calibration);
            db.SaveChanges();

            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_MT_Calibration", "C_Tool_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

            globalData.stationIPAddress = GlobalOperations.GetIP();
            globalData.hostName = GlobalOperations.GetHostName();
            globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Calibration";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceModules.Callibration +" "+ResourceGlobal.Tool+ " " + ResourceGlobal.Details;
            globalData.contentFooter = ResourceModules.Callibration +" "+ResourceGlobal.Tool+ " " + ResourceGlobal.Details;
            globalData.isSuccessMessage = true;
            globalData.messageTitle = ResourceModules.Callibration;
            globalData.messageDetail = ResourceModules.Callibration + " " + ResourceMessages.Add_Success;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return RedirectToAction("Index");
        }

        #endregion

        #region Disposing objects/memory release

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Update DropDowns

        public ActionResult GetShopByPlantID(decimal plantid)
        {
            var shops = db.MM_Shops.Where(x => x.Plant_ID == plantid).Select(x => new { x.Shop_ID, x.Shop_Name }).ToList();
            return Json(shops, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLineByShopID(decimal shopid)
        {
            var Lines = db.MM_Lines.Where(x => x.Shop_ID == shopid).Select(x => new { x.Line_ID, x.Line_Name }).ToList();
            return Json(Lines, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStationByLineID(decimal Lineid)
        {
            var Stations = db.MM_Stations.Where(x => x.Line_ID == Lineid).Select(x => new { x.Station_ID, x.Station_Name }).ToList();
            return Json(Stations, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Confirm calibration tools when maintenance is done
        /*
      * Action Name          : Confirm
      * Input Parameter      : none
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Load Confirm machine maintenance form with details 
      */
        public ActionResult Confirm()
        {
            MM_MT_Calibration_Log _PM = new MM_MT_Calibration_Log();
            int a = ((FDSession)this.Session["FDSession"]).userId;
            DateTime today = DateTime.Now;
            var obj = (from t in db.MM_MT_Calibration_Log where t.Is_Maintenance_Done == false && (t.Scheduled_Date.Year == today.Year && t.Scheduled_Date.Month == today.Month && t.Scheduled_Date.Day == today.Day) select t).ToList();
            globalData.pageTitle = ResourceModules.Callibration+" "+ResourceGlobal.Config;
            globalData.subTitle = ResourceGlobal.Lists;
            globalData.controllerName = "Confirm Calibration";
            globalData.actionName = ResourceGlobal.Lists;
            globalData.contentTitle = ResourceGlobal.Edit + " " + ResourceModules.Callibration + " " + ResourceGlobal.Form;
            globalData.contentFooter = ResourceModules.Callibration + " " + ResourceMessages.Edit_Success; ;
            ViewBag.GlobalDataModel = globalData;
            TempData["globalData"] = globalData;
            return View(obj);
        }

        /*
     * Action Name          : SaveRemark
     * Input Parameter      : none
     * Return Type          : ActionResult
     * Author & Time Stamp  : Ajay Wagh
     * Description          : Load Confirm popup to add remark and special observation about calibration tool. 
     */
        public ActionResult SaveRemark(decimal id)
        {
            // ViewBag.id = id;
            var obj = db.MM_MT_Calibration_Log.Find(id);
            return PartialView("PartialConfirm", obj);
        }


        /*
      * Action Name          : Complete
      * Input Parameter      : id
      * Return Type          : ActionResult
      * Author & Time Stamp  : Ajay Wagh
      * Description          : Confirm callibration tools maintenance  
      */
        public ActionResult Complete(FormCollection fc)
        {
           
            if (ModelState.IsValid)
            {
                MM_MT_Calibration_Log _Cali_Tool_Log = new MM_MT_Calibration_Log();
                string id = fc["C_Tool_Log_ID"].ToString().Trim();
                _Cali_Tool_Log = db.MM_MT_Calibration_Log.Where(x => x.C_Tool_Log_ID.ToString() == id).FirstOrDefault();
                _Cali_Tool_Log.Is_Maintenance_Done = true;
                _Cali_Tool_Log.Remark = fc["Remark"].ToString();
                _Cali_Tool_Log.Special_Observation = fc["Special_Observation"].ToString();
                _Cali_Tool_Log.Is_Edited = true;
                db.Entry(_Cali_Tool_Log).State = EntityState.Modified;
                db.SaveChanges();

                globalData.isSuccessMessage = true;
                globalData.messageTitle = ResourceDisplayName.Preventive_Machine_Maintenance;
                globalData.messageDetail = ResourceDisplayName.Machine + " " + ResourceMessages.Edit_Success;
                ViewBag.GlobalDataModel = globalData;
                TempData["globalData"] = globalData;
                return RedirectToAction("confirm");
            }
            return RedirectToAction("confirm");
        }
        #endregion

    }
}
