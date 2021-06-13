//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using ZHB_AD.Models;
//using ZHB_AD.Helper;
//using System.Data.OleDb;
//using System.IO;
//using ZHB_AD.App_LocalResources;
//using ZHB_AD.Controllers.BaseManagement;

//namespace ZHB_AD.Controllers
//{
//    /*  Controller Name          : PreventiveMaintenance
//     *  Description                : To create,edit,delete and show all Preventive Maintenance 
//     *  Author, Timestamp          : Vijaykuumar Kagne       
//     */
//    public class PreventiveMaintenanceController : BaseController
//    {
//        #region Global Variable Declaration
//        private DRONA_NGPEntities db = new DRONA_NGPEntities();
//        GlobalData globalData = new GlobalData();

//        MM_MT_Preventive_Maintenance mmPreventivemachine = new MM_MT_Preventive_Maintenance();
//        #endregion

//        General generalObj = new General();

//        #region Show all or Single Machine Maintenance Details
//        /*
//         * Action Name          : Index
//         * Input Parameter      : None
//         * Return Type          : ActionResult
//         * Author & Time Stamp  : Vijaykumar Kagne
//         * Description          : Get the list of Machines Maintenance
//         */

//        // GET: PreventiveMaintenance
//        public ActionResult Index()
//        {
//            if (TempData["globalData"] != null)
//            {
//                globalData = (GlobalData)TempData["globalData"];
//            }
//            var mM_MT_Preventive_Maintenance = db.MM_MT_Preventive_Maintenance.ToList();// db.MM_MT_Preventive_Maintenance.Include(m => m.MM_Employee).Include(m => m.MM_MT_Machines).Include(m => m.MM_MT_Preventive_Equipment);

//                      globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//            globalData.subTitle = ResourceGlobal.Lists;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Lists;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Add_Machine_Maintenance;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Add_Machine_Maintenance;
//            ViewBag.GlobalDataModel = globalData;
//            //ViewBag.Shop_ID = db.MM_Shops.Where(t => t.Shop_ID == db.MM_MT_Machines.SingleOrDefault().Shop_ID).First();
//            // TempData["globalData"] = globalData;
//            return View(mM_MT_Preventive_Maintenance.ToList());
//        }

//        /*
//         * Action Name          : Details
//         * Input Parameter      : id
//         * Return Type          : ActionResult
//         * Author & Time Stamp  : Vijaykumar Kagne
//         * Description          : Get the details of specified Machines Maintenance using id
//         */
//        // GET: PreventiveMaintenance/Details/5
//        public ActionResult Details(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance = db.MM_MT_Preventive_Maintenance.Find(id);
//            if (mM_MT_Preventive_Maintenance == null)
//            {
//                return HttpNotFound();
//            }
//                      globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//            globalData.subTitle = ResourceGlobal.Details;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Details;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Machine_Maintenance_Detail;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Machine_Maintenance_Detail;
//            ViewBag.GlobalDataModel = globalData;
//            // TempData["globalData"] = globalData;

//            return View(mM_MT_Preventive_Maintenance);
//        }
//        #endregion

//        #region Preventive Machine Maintenance Creation
//        /*
//         * Action Name          : Create
//         * Input Parameter      : none
//         * Return Type          : ActionResult
//         * Author & Time Stamp  : Vijaykumar Kagne
//         * Description          : Load form of Add machine maintenance 
//         */
//        // GET: PreventiveMaintenance/Create
//        public ActionResult Create()
//        {
//                      globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//            globalData.subTitle = ResourceGlobal.Create;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Create;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Add_Machine_Maintenance;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Add_Machine_Maintenance;
//            ViewBag.GlobalDataModel = globalData;
//            //  TempData["globalData"] = globalData;
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name");
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment.OrderBy(x => x.Equipment_Name), "EQP_ID", "Equipment_Name");
//            return View();
//        }
//        /*
//         * Action Name          : Create
//         * Input Parameter      : none
//         * Return Type          : ActionResult
//         * Author & Time Stamp  : Vijaykumar Kagne
//         * Description          : Add machine maintenance 
//         */
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "PM_ID,Machine_ID,EQP_ID,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Cycle_Period,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host,mails,users")] MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance)
//        {

//            string mails = "";
//            mM_MT_Preventive_Maintenance.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//            foreach (var item in mM_MT_Preventive_Maintenance.mails)
//            {
//                if (db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).Count() > 0)
//                {
//                    mails += db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
//                }
//                else
//                {

//                    //add email address which are manually entered here
//                    //not present in database
//                    mails += item.ToString() + "; ";
//                }
//                //mails += db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
//            }

//            mM_MT_Preventive_Maintenance.Receipent_Email = mails;


//            if (ModelState.IsValid)
//            {

//                if (db.MM_MT_Preventive_Maintenance.Where(x => x.Machine_ID == mM_MT_Preventive_Maintenance.Machine_ID && x.EQP_ID == mM_MT_Preventive_Maintenance.EQP_ID).Count() > 0)
//                {
//                    ModelState.AddModelError("EQP_ID", ResourceEquipmentMaintenance.Equipment_Error_Equipment_Name_Exists);
//                }
//                else
//                {
//                    mM_MT_Preventive_Maintenance.Inserted_Date = DateTime.Now.Date;
//                    mM_MT_Preventive_Maintenance.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                    mM_MT_Preventive_Maintenance.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
//                    db.MM_MT_Preventive_Maintenance.Add(mM_MT_Preventive_Maintenance);
//                    db.SaveChanges();

//                    foreach (var item in mM_MT_Preventive_Maintenance.users)
//                    {
//                        MM_MT_PM_Users pmuser = new MM_MT_PM_Users();
//                        pmuser.Machine_ID = mM_MT_Preventive_Maintenance.Machine_ID;
//                        pmuser.User_ID = item;
//                        pmuser.PM_ID = mM_MT_Preventive_Maintenance.PM_ID;
//                        db.MM_MT_PM_Users.Add(pmuser);
//                        db.SaveChanges();
//                    }
//                              globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//                    globalData.subTitle = ResourceGlobal.Create;
//                    globalData.controllerName = "Preventive Machine Maintenance";
//                    globalData.actionName = ResourceGlobal.Create;
//                    globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Add_Machine_Maintenance;
//                    globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Add_Machine_Maintenance;

//                    globalData.isSuccessMessage = true;
//                    globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//                    globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Add_Success;
//                    ViewBag.GlobalDataModel = globalData;


//                    TempData["globalData"] = globalData;
//                    return RedirectToAction("Index");
//                }
//            }
//            ViewBag.mails = new SelectList(db.MM_Employee, "Employee_ID", "Email_Address", mM_MT_Preventive_Maintenance.Machine_ID);
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Maintenance_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Updated_User_ID);
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Preventive_Maintenance.Machine_ID);
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Preventive_Maintenance.Machine_ID);
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mmPreventivemachine.Inserted_User_ID);
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment.Where(x => x.Machine_ID == mM_MT_Preventive_Maintenance.Machine_ID), "EQP_ID", "Equipment_Name", mM_MT_Preventive_Maintenance.EQP_ID);
//            return View(mM_MT_Preventive_Maintenance);
//        }
//        #endregion

//        #region Edit Specific machine Preventive Maintenance
//        /*
//        * Action Name          : Edit
//        * Input Parameter      : id
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Load Edit machine maintenance form with details 
//        */
//        // GET: PreventiveMaintenance/Edit/5
//        public ActionResult Edit(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance = db.MM_MT_Preventive_Maintenance.FirstOrDefault(x => x.PM_ID == id);
//            if (mM_MT_Preventive_Maintenance == null)
//            {
//                return HttpNotFound();
//            }
//                      globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//            globalData.subTitle = ResourceGlobal.Edit;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Edit;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            ViewBag.GlobalDataModel = globalData;
//            // TempData["globalData"] = globalData;

//            List<string> emails = mM_MT_Preventive_Maintenance.Receipent_Email.Split(';').ToList();
//            List<decimal> lst_Selected_EmailsID = new List<decimal>();
//            List<decimal> lst_usersids = new List<decimal>();

//            var lstusr = db.MM_MT_PM_Users.Where(x => x.PM_ID == id).ToList();
//            foreach (var usr in lstusr)
//            {
//                lst_usersids.Add(usr.User_ID);
//            }
//            foreach (string email in emails)
//            {
//                if (email != "" && email != " " && email != null)
//                {
//                    decimal j = db.MM_Employee.Where(x => x.Email_Address.ToLower().Trim() == email.Trim()).FirstOrDefault().Employee_ID;
//                    lst_Selected_EmailsID.Add(j);
//                }
//            }
//            ViewBag.mails = new MultiSelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address", lst_Selected_EmailsID);
//            ViewBag.users = new MultiSelectList(db.MM_Employee, "Employee_ID", "Employee_Name", lst_usersids);
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Maintenance_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Updated_User_ID);
//            ViewBag.Inserted_user_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Inserted_User_ID);
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Preventive_Maintenance.Machine_ID);
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Preventive_Maintenance.Machine_ID);
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment.Where(x => x.Machine_ID == mM_MT_Preventive_Maintenance.Machine_ID), "EQP_ID", "Equipment_Name", mM_MT_Preventive_Maintenance.EQP_ID);
//            return View(mM_MT_Preventive_Maintenance);
//        }

//        /*
//        * Action Name          : Edit
//        * Input Parameter      : id
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Edit machine maintenance  
//        */
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "PM_ID,Machine_ID,Plant_ID,EQP_ID,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Cycle_Period,Receipent_Email,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host,mails,users,Remark")] MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance)
//        {


//            if (ModelState.IsValid)
//            {
//                string strmails = "";
//                mM_MT_Preventive_Maintenance.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//                foreach (var item in mM_MT_Preventive_Maintenance.mails)
//                {
//                    if (db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).Count() > 0)
//                    {
//                        strmails += db.MM_Employee.Where(x => x.Employee_ID.ToString().ToLower().Trim() == item.ToString().ToLower().Trim()).First().Email_Address + ";";
//                    }
//                    else
//                    {
//                        strmails += item.ToString() + "; ";
//                        //add email address which are manually entered here
//                        //not present in database
//                    }
//                    //    decimal j = Convert.ToDecimal(item);
//                    //    strmails += db.MM_Employee.Where(x => x.Employee_ID == j).FirstOrDefault().Email_Address+"; "; 
//                }
//                if (db.MM_MT_Preventive_Maintenance.Where(x => x.Machine_ID == mM_MT_Preventive_Maintenance.Machine_ID && x.EQP_ID == mM_MT_Preventive_Maintenance.EQP_ID).Count() > 0)
//                {
//                    ModelState.AddModelError("EQP_ID", ResourceEquipmentMaintenance.Equipment_Error_Equipment_Name_Exists);
//                              globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//                    globalData.subTitle = ResourceGlobal.Edit;
//                    globalData.controllerName = "Preventive Machine Maintenance";
//                    globalData.actionName = ResourceGlobal.Edit;
//                    globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//                    globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//                }
//                else
//                {
//                    mmPreventivemachine = db.MM_MT_Preventive_Maintenance.FirstOrDefault(x => x.PM_ID == mM_MT_Preventive_Maintenance.PM_ID);
//                    mM_MT_Preventive_Maintenance.Receipent_Email = strmails;
//                    mmPreventivemachine.Cycle_Period = mM_MT_Preventive_Maintenance.Cycle_Period;
//                    mmPreventivemachine.Maintenance_User_ID = mM_MT_Preventive_Maintenance.Maintenance_User_ID;
//                    mmPreventivemachine.Receipent_Email = mM_MT_Preventive_Maintenance.Receipent_Email;
//                    mmPreventivemachine.Last_Maintenance_Date = mM_MT_Preventive_Maintenance.Last_Maintenance_Date;
//                    mmPreventivemachine.Scheduled_Date = mM_MT_Preventive_Maintenance.Scheduled_Date;
//                    if (mmPreventivemachine.Cycle_Period != mM_MT_Preventive_Maintenance.Cycle_Period)
//                    {
//                        if (mM_MT_Preventive_Maintenance.Remark != null)
//                        {
//                            mmPreventivemachine.Remark = mM_MT_Preventive_Maintenance.Remark;
//                        }

//                    }
//                    mmPreventivemachine.Updated_Date = DateTime.Now;
//                    mmPreventivemachine.Updated_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                    mmPreventivemachine.Updated_Host = HttpContext.Request.UserHostAddress;
//                    db.Entry(mmPreventivemachine).State = EntityState.Modified;
//                    db.SaveChanges();

//                    var record = db.MM_MT_PM_Users.Where(x => x.PM_ID == mM_MT_Preventive_Maintenance.PM_ID).ToList();
//                    db.MM_MT_PM_Users.RemoveRange(record);

//                    foreach (var item in mM_MT_Preventive_Maintenance.users)
//                    {
//                        MM_MT_PM_Users pmuser = new MM_MT_PM_Users();
//                        pmuser.PM_ID = mmPreventivemachine.PM_ID;
//                        pmuser.User_ID = item;
//                        pmuser.Machine_ID = mmPreventivemachine.Machine_ID;
//                        db.MM_MT_PM_Users.Add(pmuser);
//                        db.SaveChanges();
//                    }

//                }
//                globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//                globalData.subTitle = ResourceGlobal.Edit;
//                globalData.controllerName = "Preventive Machine Maintenance";
//                globalData.actionName = ResourceGlobal.Edit;
//                globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//                globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//                globalData.isSuccessMessage = true;
//                globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//                globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;

//                ViewBag.GlobalDataModel = globalData;
//                TempData["globalData"] = globalData;
//                return RedirectToAction("Index");
//            }

//                      globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//            globalData.subTitle = ResourceGlobal.Edit;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Edit;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;

//            globalData.isSuccessMessage = true;
//            globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//            globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;

//            ViewBag.GlobalDataModel = globalData;
//            ViewBag.mails = new SelectList(db.MM_Employee.Where(x => x.Email_Address != null && x.Email_Address != ""), "Employee_ID", "Email_Address");
//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Maintenance_User_ID);
//            ViewBag.Inserted_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Inserted_User_ID);
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Updated_User_ID);
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Preventive_Maintenance.Machine_ID);
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Preventive_Maintenance.Machine_ID);
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment.Where(x => x.Machine_ID == mM_MT_Preventive_Maintenance.Machine_ID), "EQP_ID", "Equipment_Name", mM_MT_Preventive_Maintenance.EQP_ID);
//            return View(mM_MT_Preventive_Maintenance);
//        }
//        #endregion

//        #region Confirm Specific machine Preventive Maintenance as completed
//        /*
//        * Action Name          : Confirm
//        * Input Parameter      : none
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Load Confirm machine maintenance form with details 
//        */

//        public ActionResult Confirm()
//        {
//            MM_MT_Preventive_Maintenance_Log _PM = new MM_MT_Preventive_Maintenance_Log();
//            int a = ((FDSession)this.Session["FDSession"]).userId;
//            DateTime scheduleObj = DateTime.Now;

//            var obj = (from t in db.MM_MT_Preventive_Maintenance_Log where t.Is_Maintenance_Done == false && DateTime.Compare(t.Scheduled_Date, DateTime.Now) < 0 select t).ToList();

//            globalData.pageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//            globalData.subTitle = ResourceGlobal.Lists;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Lists;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Edit_Machine_Maintenance;
//            ViewBag.GlobalDataModel = globalData;
//            TempData["globalData"] = globalData;
//            return View(obj);
//        }
//        /*
//      * Action Name          : Confirm
//      * Input Parameter      : none
//      * Return Type          : ActionResult
//      * Author & Time Stamp  : Vijaykumar Kagne
//      * Description          : Load Confirm machine maintenance form with details 
//      */

//        public ActionResult SaveRemark(decimal id)
//        {
//            // ViewBag.id = id;
//            var obj = db.MM_MT_Preventive_Maintenance_Log.Find(id);
//            return PartialView("PartialConfirm", obj);
//        }

//        /*
//        * Action Name          : Complete
//        * Input Parameter      : id
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Confirm machine maintenance  
//        */

//        public ActionResult Complete(FormCollection fc)
//        {

//            MM_MT_Preventive_Maintenance_Log mM_MT_Preventive_Maintenance = new MM_MT_Preventive_Maintenance_Log();
//            // mmPreventivemachine = db.MM_MT_Preventive_Maintenance.Where(x => x.PM_ID == id).FirstOrDefault();
//            if (ModelState.IsValid)
//            {
//                MM_MT_Preventive_Maintenance_Log _PMLog = new MM_MT_Preventive_Maintenance_Log();
//                string id = fc["PM_Log_ID"].ToString().Trim();
//                _PMLog = db.MM_MT_Preventive_Maintenance_Log.Where(x => x.PM_Log_ID.ToString() == id).FirstOrDefault();
//                _PMLog.Is_Maintenance_Done = true;
//                _PMLog.Remark = fc["Remark"].ToString();
//                _PMLog.Special_Observation = fc["Special_Observation"].ToString();
//                db.Entry(_PMLog).State = EntityState.Modified;
//                db.SaveChanges();

//                globalData.isSuccessMessage = true;
//                globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//                globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Edit_Success;
//                ViewBag.GlobalDataModel = globalData;
//                TempData["globalData"] = globalData;
//                return RedirectToAction("confirm");
//            }
//            return RedirectToAction("confirm");
//        }

//        #endregion

//        #region Delete Specific Preventive maintenance

//        /*
//        * Action Name          : Delete
//        * Input Parameter      : id(PM_ID)
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : load Delete Machine Maintenance with details
//        */

//        // GET: PreventiveMaintenance/Delete/5
//        public ActionResult Delete(decimal id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance = db.MM_MT_Preventive_Maintenance.Find(id);
//            if (mM_MT_Preventive_Maintenance == null)
//            {
//                return HttpNotFound();
//            }
//                      globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//            globalData.subTitle = ResourceGlobal.Delete;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Delete;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Delete_Machine_Maintenance;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Delete_Machine_Maintenance;
//            ViewBag.GlobalDataModel = globalData;
//            TempData["globalData"] = globalData;
//            return View(mM_MT_Preventive_Maintenance);
//        }

//        /*
//        * Action Name          : Delete
//        * Input Parameter      : id(PM_ID)
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Delete Machine Maintenance
//        */
//        // POST: PreventiveMaintenance/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(decimal id)
//        {
//            var mmpmuser = db.MM_MT_PM_Users.Where(x => x.PM_ID == id).ToList();
//            db.MM_MT_PM_Users.RemoveRange(mmpmuser);
//            db.SaveChanges();

//            MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance = db.MM_MT_Preventive_Maintenance.Find(id);
//            db.MM_MT_Preventive_Maintenance.Remove(mM_MT_Preventive_Maintenance);
//            db.SaveChanges();

//            generalObj.addPurgeDeletedRecords(((FDSession)this.Session["FDSession"]).plantId, "MM_MT_Preventive_Maintenance", "PM_ID", id.ToString(), ((FDSession)this.Session["FDSession"]).userHost, ((FDSession)this.Session["FDSession"]).userId);

//                      globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//            globalData.subTitle = ResourceGlobal.Delete;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Delete;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Delete_Success;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Delete_Success;

//            globalData.isSuccessMessage = true;
//            globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//            globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Delete_Success;
//            TempData["globalData"] = globalData;

//            ViewBag.GlobalDataModel = globalData;

//            return RedirectToAction("Index");
//        }
//        #endregion

//        #region Disposing Objects(Releasing memory)
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//                //TempData["globalData"] = new GlobalData();

//            }
//            base.Dispose(disposing);
//            // TempData["globalData"] = new GlobalData();
//        }
//        #endregion

//        #region Upload Preventive maintenance of machine using Excel file
//        /*
//         * Action Name          : Upload
//         * Input Parameter      : None
//         * Return Type          : ActionResult
//         * Author & Time Stamp  : Vijaykumar Kagne
//         * Description          : Action used to show upload form
//        */


//        //GET: Upload file page load
//        public ActionResult Upload()
//        {

//            if (TempData["PMRecords"] != null)
//            {
//                ViewBag.pmRecords = TempData["PMRecords"];
//                ViewBag.pmDataTable = TempData["PMDataTable"];
//            }

//                      globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//            globalData.subTitle = ResourceGlobal.Upload;
//            globalData.controllerName = "Preventive Machine Maintenance";
//            globalData.actionName = ResourceGlobal.Upload;
//            globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Title_Add_Machine_Maintenance;
//            globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Title_Add_Machine_Maintenance;

//            ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Maintenance_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//            ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number");
//            ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name");
//            ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment.OrderBy(x => x.Equipment_Name), "EQP_ID", "Equipment_Name");


//            ViewBag.GlobalDataModel = globalData;
//            return View();
//        }
//        /*
//        * Action Name          : Upload
//       * Input Parameter      : Files object and MM_MT_PreventiveMachine obj
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Action used get upload excel file and generationg datatable
//        */



//        //GET: GET The file from upload control 
//        [HttpPost]
//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Upload(HttpPostedFileBase files, [Bind(Include = "PM_ID,Plant_ID,Machine_ID,EQP_ID,Scheduled_Date,Last_Maintenance_Date,Maintenance_User_ID,Cycle_Period,Receipent_Email,Is_Deleted,Is_Transfered,Is_Purgeable,Inserted_Host,Inserted_User_ID,Inserted_Date,Updated_User_ID,Updated_Date,Updated_Host,users")] MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance)
//        {
//            try
//            {
//                mM_MT_Preventive_Maintenance.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//                GlobalOperations globalOperations = new GlobalOperations();
//                string fileName = Path.GetFileName(files.FileName);
//                string fileExtension = Path.GetExtension(files.FileName);
//                string fileLocation = Server.MapPath("~/App_Data/" + fileName);
//                DataTable dt = globalOperations.ExcelToDataTable(files, fileLocation, fileExtension);
//                ExcelMTPMRecords[] pmrecords = new ExcelMTPMRecords[dt.Rows.Count];
//                int j = 0;
//                if (dt.Rows.Count > 0)
//                {
//                    foreach (DataRow pmlist in dt.Rows)
//                    {
//                        MM_MT_Preventive_Maintenance _PM = new MM_MT_Preventive_Maintenance();
//                        ExcelMTPMRecords pmsg = new ExcelMTPMRecords();
//                        string machineName = pmlist["Machine_Name"].ToString().Trim();
//                        string machinePart = pmlist["Equipment_Name"].ToString().Trim();
//                        string scheduledDate = pmlist["Scheduled_Date"].ToString().Trim();
//                        string cycle = pmlist["Cycle_Period"].ToString().Trim();
//                        pmsg.MachineName = machineName;
//                        pmsg.EquipmentName = machinePart;

//                        decimal machine_id = db.MM_MT_Machines.Where(x => x.Machine_Name.ToLower() == machineName.ToLower()).FirstOrDefault().Machine_ID;
//                        decimal? eqp_id = null;
//                        if (db.MM_MT_Preventive_Equipment.Where(x => x.Equipment_Name.ToLower() == machinePart.ToLower() && x.Machine_ID == machine_id).Count() > 0)
//                        {
//                            eqp_id = db.MM_MT_Preventive_Equipment.Where(x => x.Equipment_Name.ToLower() == machinePart.ToLower() && x.Machine_ID == machine_id).FirstOrDefault().EQP_ID;
//                        }

//                        if (machineName == "" && machineName == null)
//                        {
//                            pmsg.PM_Error = "Machine is can not be null";
//                        }
//                        else if (db.MM_MT_Preventive_Maintenance.Where(x => x.EQP_ID == eqp_id && x.Machine_ID == machine_id).Count() > 0)
//                        {
//                            pmsg.PM_Error = "Machine is already Exists";
//                        }
//                        else if (eqp_id == null)
//                        {
//                            pmsg.PM_Error = "No Machine Configured with this Part";
//                        }
//                        else if (eqp_id != null)
//                        {
//                            _PM.Machine_ID = machine_id;
//                            _PM.EQP_ID = (decimal)eqp_id;
//                            _PM.Cycle_Period = Convert.ToInt32(cycle);
//                            _PM.Plant_ID = ((FDSession)this.Session["FDSession"]).plantId;
//                            _PM.Scheduled_Date = Convert.ToDateTime(scheduledDate);

//                            string[] users = pmlist["Users_Token"].ToString().Split(';').ToArray();
//                            string email = "";
//                            int[] user = new int[users.Count()];
//                            for (int i = 0; i < users.Count(); i++)
//                            {
//                                string usr = users[i].ToString();
//                                decimal id = db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Employee_ID;
//                                user[i] = Convert.ToInt32(id);
//                                email += db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Email_Address + ";";
//                            }
//                            _PM.Receipent_Email = email;
//                            _PM.Inserted_Date = DateTime.Now;
//                            _PM.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
//                            _PM.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                            db.MM_MT_Preventive_Maintenance.Add(_PM);
//                            db.SaveChanges();
//                            for (int i = 0; i < users.Count(); i++)
//                            {
//                                if (users[i] != "" && users[i] != null && users[i]!=" ")
//                                {
//                                    MM_MT_PM_Users pmuser = new MM_MT_PM_Users();
//                                    pmuser.Machine_ID = machine_id;
//                                    pmuser.PM_ID = _PM.PM_ID;
//                                    string usr = users[i].ToString();
//                                    decimal id = db.MM_Employee.Where(x => x.Employee_No.ToString().ToLower() == usr.ToLower().Trim()).FirstOrDefault().Employee_ID;
//                                    pmuser.User_ID = id;
//                                }
//                            }
//                            pmsg.PM_Error = "Machine is added sucessfully";
//                            globalData.isSuccessMessage = true;
//                            globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//                            globalData.messageDetail = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Add_Success;
//                        }
//                        pmrecords[j] = pmsg;
//                        j = j + 1;
//                    }

//                }

//                // InsertIntoDataTable(dt, mM_MT_Preventive_Maintenance);
//                          globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//                globalData.subTitle = ResourceGlobal.Upload;
//                globalData.controllerName = "Preventive Machine Maintenance";
//                globalData.actionName = ResourceGlobal.Upload;
//                globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Add_Success;
//                globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Add_Success;
//                TempData["globalData"] = globalData;
//                TempData["PMRecords"] = pmrecords;
//                TempData["PMDataTable"] = dt;
//                ViewBag.pmRecords = pmrecords;
//                ViewBag.pmDataTable = dt;
//                ViewBag.GlobalDataModel = globalData;
//                ViewBag.users = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name");
//                ViewBag.Updated_User_ID = new SelectList(db.MM_Employee, "Employee_ID", "Employee_Name", mM_MT_Preventive_Maintenance.Updated_User_ID);
//                ViewBag.Machine_ID = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Number", mM_MT_Preventive_Maintenance.Machine_ID);
//                ViewBag.Machine_Name = new SelectList(db.MM_MT_Machines.OrderBy(x => x.Machine_Name), "Machine_ID", "Machine_Name", mM_MT_Preventive_Maintenance.Machine_ID);
//                ViewBag.EQP_ID = new SelectList(db.MM_MT_Preventive_Equipment, "EQP_ID", "Equipment_Name", mM_MT_Preventive_Maintenance.EQP_ID);


//                ViewBag.GlobalDataModel = globalData;

//            }
//            catch (Exception ex)
//            {

//                          globalData.pageTitle = ResourceMachineMaintenance.PM_Configuration;
//                globalData.subTitle = ResourceGlobal.Upload;
//                globalData.controllerName = "Preventive Machine Maintenance";
//                globalData.actionName = ResourceGlobal.Upload;
//                globalData.contentTitle = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Add_Success;
//                globalData.contentFooter = ResourceMachineMaintenance.Machine_Maintenance_Success_Machine_Maintenance_Add_Success;

//                globalData.isErrorMessage = true;
//                if (files == null)
//                {
//                    globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//                    globalData.messageDetail = "Please upload file, file can not be empty.";
//                }
//                else
//                {
//                    globalData.messageTitle = ResourceMachineMaintenance.Machine_Maintenance;
//                    globalData.messageDetail = ex.Message.ToString();
//                }
//                ViewBag.GlobalDataModel = globalData;
//            }
//            return View();
//        }
//        #endregion

//        #region Insert all Data of DataTable into respective Database Table
//        /*
//        * Action Name          : InsertIntoDataTable
//        * Input Parameter      : Datatable and MM_MT_Machines obj
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Action used to add new Machine maintenance into database table which are present in datattable under plant with shop and line 
//        */
//        private bool InsertIntoDataTable(DataTable dt, MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance)
//        {
//            try
//            {

//                //if (ModelState.IsValid)
//                //{
//                foreach (var item in mM_MT_Preventive_Maintenance.users)
//                {
//                    MM_MT_PM_Users pmuser = new MM_MT_PM_Users();
//                    pmuser.Machine_ID = mM_MT_Preventive_Maintenance.Machine_ID;
//                    pmuser.User_ID = item;
//                    pmuser.PM_ID = mM_MT_Preventive_Maintenance.PM_ID;
//                    db.MM_MT_PM_Users.Add(pmuser);
//                    db.SaveChanges();
//                }
//                foreach (DataRow dr in dt.Rows)
//                {
//                    // MM_MT_Preventive_Maintenance mM_MT_Preventive_Maintenance = new MM_MT_Preventive_Maintenance();

//                    mM_MT_Preventive_Maintenance.Cycle_Period = Convert.ToInt32(dr["Cycle_Period"].ToString().Trim());
//                    // mM_MT_Preventive_Maintenance.Last_Maintenance_Date = Convert.ToDateTime(dr["Last_Maintenance_Date"].ToString().Trim());
//                    mM_MT_Preventive_Maintenance.Receipent_Email = Convert.ToString(dr["Receipent_Email"].ToString().Trim());


//                    //mM_MT_Preventive_Maintenance.Maintenance_User_ID = Convert.ToDecimal(dr["Maintenance_User_ID"].ToString().Trim());
//                    //  mM_MT_Preventive_Maintenance.EQP_ID = Convert.ToDecimal(dr["EQP_ID"].ToString().Trim());
//                    // mM_MT_Preventive_Maintenance.Machine_ID = Convert.ToDecimal(dr["Machine_ID"].ToString().Trim());
//                    //mM_MT_Preventive_Maintenance.Is_Deleted = Convert.ToBoolean(dr["Is_Deleted"]);
//                    //mM_MT_Preventive_Maintenance.Is_Purgeable = Convert.ToBoolean(dr["Is_Purgeable"]);
//                    //mM_MT_Preventive_Maintenance.Is_Transfered = Convert.ToBoolean(dr["Is_Transfered"]);

//                    mM_MT_Preventive_Maintenance.Inserted_Date = DateTime.Now;
//                    mM_MT_Preventive_Maintenance.Inserted_User_ID = ((FDSession)this.Session["FDSession"]).userId;
//                    mM_MT_Preventive_Maintenance.Inserted_Host = HttpContext.Request.ServerVariables["REMOTE_ADDR"];

//                    mM_MT_Preventive_Maintenance.Scheduled_Date = Convert.ToDateTime(dr["Scheduled_Date"].ToString().Trim());
//                    db.MM_MT_Preventive_Maintenance.Add(mM_MT_Preventive_Maintenance);
//                    db.SaveChanges();
//                }
//                //  }

//            }
//            catch (Exception ex)
//            {

//            }
//            return true;
//        }
//        #endregion

//        #region Get Update child Drop Dwon
//        /*
//        * Action Name          : GetEquipmentByMachineID
//        * Input Parameter      : Machines_ID
//        * Return Type          : ActionResult
//        * Author & Time Stamp  : Vijaykumar Kagne
//        * Description          : Action used to get machine equipment with reqpect to machine
//        */

//        public ActionResult GetEquipmentByMachineID(int machineid)
//        {
//            var Lines = db.MM_MT_Preventive_Equipment.Where(c => c.Machine_ID == machineid).Select(a => new { a.EQP_ID, a.Equipment_Name }).OrderBy(x => x.Equipment_Name);
//            return Json(Lines, JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult GetEmailsByEmployeeName(string[] empid)
//        {
//            Dictionary<int, string> obj = new Dictionary<int, string>();
//            foreach (var item in empid)
//            {
//                obj.Add(Convert.ToInt32(item), db.MM_Employee.Where(x => x.Employee_ID.ToString() == item).First().Email_Address);
//                var emails = db.MM_Employee.Where(c => c.Employee_ID.ToString() == item).Select(a => new { a.Employee_ID, a.Email_Address });

//            }

//            return Json(obj, JsonRequestBehavior.AllowGet);
//        }
//        #endregion
//    }

//}
